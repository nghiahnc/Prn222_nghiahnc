using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Repositories;

namespace Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepo;
        private readonly IEventCategoryRepository _categoryRepo;
        private readonly ITicketTypeRepository _ticketTypeRepo;

        public EventService(
            IEventRepository eventRepo,
            IEventCategoryRepository categoryRepo,
            ITicketTypeRepository ticketTypeRepo)
        {
            _eventRepo = eventRepo;
            _categoryRepo = categoryRepo;
            _ticketTypeRepo = ticketTypeRepo;
        }

        public Task<List<Event>> GetAllAsync()
        {
            return _eventRepo.GetAllAsync();
        }

        public Task<Event?> GetDetailsAsync(int id)
        {
            return _eventRepo.GetDetailsAsync(id);
        }

        public Task<List<EventCategory>> GetCategoriesAsync()
        {
            return _categoryRepo.GetAllAsync();
        }

        public async Task<ServiceResult<int>> CreateAsync(CreateEventRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return ServiceResult<int>.Fail("Event Name is required.");
            if (string.IsNullOrWhiteSpace(request.Detail))
                return ServiceResult<int>.Fail("Event Detail is required.");
            if (string.IsNullOrWhiteSpace(request.Location))
                return ServiceResult<int>.Fail("Event Location is required.");

            var categoryExists = await _categoryRepo.ExistsAsync(request.EventCategoryId);
            if (!categoryExists)
                return ServiceResult<int>.Fail("Event category does not exist.");

            if (request.TimeStart <= DateTime.Now)
                return ServiceResult<int>.Fail("TimeStart must be in the future.");
            if (request.TimeStart >= request.TimeEnd)
                return ServiceResult<int>.Fail("TimeStart must be before TimeEnd.");

            var newEvent = new Event
            {
                Name = request.Name.Trim(),
                EventCategoryId = request.EventCategoryId,
                Detail = request.Detail.Trim(),
                Location = request.Location.Trim(),
                TimeStart = request.TimeStart,
                TimeEnd = request.TimeEnd,
                Status = 0, // Forced to Draft
                CreatedBy = request.CreatedBy
            };

            await _eventRepo.AddAsync(newEvent);
            await _eventRepo.SaveChangesAsync();

            return ServiceResult<int>.Ok(newEvent.Id);
        }

        public async Task<ServiceResult> UpdateAsync(UpdateEventRequest request)
        {
            var existingEvent = await _eventRepo.GetByIdAsync(request.Id);
            if (existingEvent == null)
                return ServiceResult.Fail("Event not found.");

            if (request.CurrentUserRole != 0 && (request.CurrentUserRole != 2 || existingEvent.CreatedBy != request.CurrentUserId))
                return ServiceResult.Fail("Unauthorized: You do not have permission to manage this event.");

            // Block updating Published event if it has already started
            if (existingEvent.Status == 1 && existingEvent.TimeStart <= DateTime.Now)
                return ServiceResult.Fail("Cannot update a published event that has already started.");

            if (string.IsNullOrWhiteSpace(request.Name))
                return ServiceResult.Fail("Event Name is required.");
            if (string.IsNullOrWhiteSpace(request.Detail))
                return ServiceResult.Fail("Event Detail is required.");
            if (string.IsNullOrWhiteSpace(request.Location))
                return ServiceResult.Fail("Event Location is required.");

            var categoryExists = await _categoryRepo.ExistsAsync(request.EventCategoryId);
            if (!categoryExists)
                return ServiceResult.Fail("Event category does not exist.");

            if (request.TimeStart <= DateTime.Now)
                return ServiceResult.Fail("TimeStart must be in the future.");
            if (request.TimeStart >= request.TimeEnd)
                return ServiceResult.Fail("TimeStart must be before TimeEnd.");

            existingEvent.Name = request.Name.Trim();
            existingEvent.EventCategoryId = request.EventCategoryId;
            existingEvent.Detail = request.Detail.Trim();
            existingEvent.Location = request.Location.Trim();
            existingEvent.TimeStart = request.TimeStart;
            existingEvent.TimeEnd = request.TimeEnd;
            // Preserve existing Status

            _eventRepo.Update(existingEvent);
            await _eventRepo.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteAsync(int id, int currentUserId, int currentUserRole)
        {
            var existingEvent = await _eventRepo.GetByIdAsync(id);
            if (existingEvent == null)
                return ServiceResult.Fail("Event not found.");

            if (currentUserRole != 0 && (currentUserRole != 2 || existingEvent.CreatedBy != currentUserId))
                return ServiceResult.Fail("Unauthorized: You do not have permission to manage this event.");

            // Do not delete Published events
            if (existingEvent.Status == 1)
                return ServiceResult.Fail("Cannot delete a published event.");

            // Do not delete event if it has ticket types that already have sold tickets
            var ticketTypes = await _ticketTypeRepo.GetByEventIdAsync(id);
            foreach (var tt in ticketTypes)
            {
                var hasSold = await _ticketTypeRepo.HasSoldTicketsAsync(tt.Id);
                if (hasSold)
                    return ServiceResult.Fail("Cannot delete event because it has ticket types with sold tickets.");
            }

            _eventRepo.Delete(existingEvent);
            await _eventRepo.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> PublishAsync(int id, int currentUserId, int currentUserRole)
        {
            var existingEvent = await _eventRepo.GetByIdAsync(id);
            if (existingEvent == null)
                return ServiceResult.Fail("Event not found.");

            if (currentUserRole != 0 && (currentUserRole != 2 || existingEvent.CreatedBy != currentUserId))
                return ServiceResult.Fail("Unauthorized: You do not have permission to manage this event.");

            // Status must be 0 Draft
            if (existingEvent.Status != 0)
                return ServiceResult.Fail("Only draft events can be published.");

            // Required fields validation
            if (string.IsNullOrWhiteSpace(existingEvent.Name))
                return ServiceResult.Fail("Event Name is required to publish.");
            if (string.IsNullOrWhiteSpace(existingEvent.Detail))
                return ServiceResult.Fail("Event Detail is required to publish.");
            if (string.IsNullOrWhiteSpace(existingEvent.Location))
                return ServiceResult.Fail("Event Location is required to publish.");

            if (existingEvent.TimeStart <= DateTime.Now)
                return ServiceResult.Fail("Event TimeStart must be in the future to publish.");
            if (existingEvent.TimeStart >= existingEvent.TimeEnd)
                return ServiceResult.Fail("Event TimeStart must be before TimeEnd to publish.");

            // Event must have at least one TicketType
            var ticketTypes = await _ticketTypeRepo.GetByEventIdAsync(id);
            if (ticketTypes.Count == 0)
                return ServiceResult.Fail("Event must have at least one ticket type to publish.");

            // All TicketTypes must have Price > 0 and Quantity > 0
            foreach (var tt in ticketTypes)
            {
                if (tt.Price <= 0)
                    return ServiceResult.Fail($"Ticket type '{tt.Name}' must have a price greater than zero to publish.");
                if (tt.Quantity <= 0)
                    return ServiceResult.Fail($"Ticket type '{tt.Name}' must have a quantity greater than zero to publish.");
            }

            existingEvent.Status = 1; // Published

            _eventRepo.Update(existingEvent);
            await _eventRepo.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> CancelAsync(int id, int currentUserId, int currentUserRole)
        {
            var existingEvent = await _eventRepo.GetByIdAsync(id);
            if (existingEvent == null)
                return ServiceResult.Fail("Event not found.");

            if (currentUserRole != 0 && (currentUserRole != 2 || existingEvent.CreatedBy != currentUserId))
                return ServiceResult.Fail("Unauthorized: You do not have permission to manage this event.");

            if (existingEvent.Status == 2)
                return ServiceResult.Fail("Event is already cancelled.");

            if (existingEvent.TimeStart <= DateTime.Now)
                return ServiceResult.Fail("Cannot cancel an event that has already started or past.");

            existingEvent.Status = 2; // Cancelled
            _eventRepo.Update(existingEvent);
            await _eventRepo.SaveChangesAsync();
            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> PostponeAsync(PostponeEventRequest request)
        {
            var existingEvent = await _eventRepo.GetByIdAsync(request.EventId);
            if (existingEvent == null)
                return ServiceResult.Fail("Event not found.");

            if (request.CurrentUserRole != 0 && (request.CurrentUserRole != 2 || existingEvent.CreatedBy != request.CurrentUserId))
                return ServiceResult.Fail("Unauthorized: You do not have permission to manage this event.");

            if (existingEvent.Status == 2)
                return ServiceResult.Fail("Cannot postpone a cancelled event.");

            if (existingEvent.TimeStart <= DateTime.Now)
                return ServiceResult.Fail("Cannot postpone an event that has already started or past.");

            if (request.NewTimeStart <= DateTime.Now)
                return ServiceResult.Fail("NewTimeStart must be in the future.");

            if (request.NewTimeStart >= request.NewTimeEnd)
                return ServiceResult.Fail("NewTimeStart must be before NewTimeEnd.");

            existingEvent.TimeStart = request.NewTimeStart;
            existingEvent.TimeEnd = request.NewTimeEnd;

            if (!string.IsNullOrWhiteSpace(request.Reason))
            {
                existingEvent.Detail += $"\n[Postponed]: {request.Reason}";
            }

            _eventRepo.Update(existingEvent);
            await _eventRepo.SaveChangesAsync();
            return ServiceResult.Ok();
        }
    }
}
