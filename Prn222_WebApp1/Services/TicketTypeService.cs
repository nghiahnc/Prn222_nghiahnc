using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Repositories;

namespace Services
{
    public class TicketTypeService : ITicketTypeService
    {
        private readonly ITicketTypeRepository _ticketTypeRepo;
        private readonly IEventRepository _eventRepo;

        public TicketTypeService(
            ITicketTypeRepository ticketTypeRepo,
            IEventRepository eventRepo)
        {
            _ticketTypeRepo = ticketTypeRepo;
            _eventRepo = eventRepo;
        }

        public Task<List<TicketType>> GetAllAsync()
        {
            return _ticketTypeRepo.GetAllAsync();
        }

        public Task<TicketType?> GetDetailsAsync(int id)
        {
            return _ticketTypeRepo.GetDetailsAsync(id);
        }

        public Task<List<TicketType>> GetByEventIdAsync(int eventId)
        {
            return _ticketTypeRepo.GetByEventIdAsync(eventId);
        }

        public async Task<ServiceResult<int>> CreateAsync(TicketTypeRequest request)
        {
            var eventEntity = await _eventRepo.GetByIdAsync(request.EventId);
            if (eventEntity == null)
                return ServiceResult<int>.Fail("Event not found.");

            if (request.CurrentUserRole != 0 && (request.CurrentUserRole != 2 || eventEntity.CreatedBy != request.CurrentUserId))
                return ServiceResult<int>.Fail("Unauthorized: You do not have permission to manage ticket types for this event.");

            if (eventEntity.Status == 2)
                return ServiceResult<int>.Fail("Cannot configure ticket types for a cancelled event.");

            if (eventEntity.TimeStart <= DateTime.Now)
                return ServiceResult<int>.Fail("Cannot configure ticket types for an event that has already started.");

            if (string.IsNullOrWhiteSpace(request.Name))
                return ServiceResult<int>.Fail("Ticket type Name is required.");
            if (request.Price < 50000 || request.Price > 100000000)
                return ServiceResult<int>.Fail("Price must be between 50,000 ₫ and 100,000,000 ₫.");
            if (request.Quantity < 1 || request.Quantity > 100000)
                return ServiceResult<int>.Fail("Quantity must be between 1 and 100,000.");

            var ticketType = new TicketType
            {
                EventId = request.EventId,
                Name = request.Name.Trim(),
                Price = request.Price,
                Quantity = request.Quantity
            };

            await _ticketTypeRepo.AddAsync(ticketType);
            await _ticketTypeRepo.SaveChangesAsync();

            return ServiceResult<int>.Ok(ticketType.Id);
        }

        public async Task<ServiceResult> UpdateAsync(TicketTypeRequest request)
        {
            if (request.Id == null)
                return ServiceResult.Fail("Ticket type Id is required.");

            var existingTicketType = await _ticketTypeRepo.GetByIdAsync(request.Id.Value);
            if (existingTicketType == null)
                return ServiceResult.Fail("Ticket type not found.");

            var eventEntity = await _eventRepo.GetByIdAsync(existingTicketType.EventId);
            if (eventEntity == null)
                return ServiceResult.Fail("Event not found.");

            if (request.CurrentUserRole != 0 && (request.CurrentUserRole != 2 || eventEntity.CreatedBy != request.CurrentUserId))
                return ServiceResult.Fail("Unauthorized: You do not have permission to manage ticket types for this event.");

            if (eventEntity.Status == 2)
                return ServiceResult.Fail("Cannot configure ticket types for a cancelled event.");

            if (eventEntity.TimeStart <= DateTime.Now)
                return ServiceResult.Fail("Cannot configure ticket types for an event that has already started.");

            if (string.IsNullOrWhiteSpace(request.Name))
                return ServiceResult.Fail("Ticket type Name is required.");
            if (request.Price < 50000 || request.Price > 100000000)
                return ServiceResult.Fail("Price must be between 50,000 ₫ and 100,000,000 ₫.");
            if (request.Quantity < 1 || request.Quantity > 100000)
                return ServiceResult.Fail("Quantity must be between 1 and 100,000.");

            existingTicketType.Name = request.Name.Trim();
            existingTicketType.Price = request.Price;
            existingTicketType.Quantity = request.Quantity;

            _ticketTypeRepo.Update(existingTicketType);
            await _ticketTypeRepo.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteAsync(int id, int currentUserId, int currentUserRole)
        {
            var existingTicketType = await _ticketTypeRepo.GetByIdAsync(id);
            if (existingTicketType == null)
                return ServiceResult.Fail("Ticket type not found.");

            var eventEntity = await _eventRepo.GetByIdAsync(existingTicketType.EventId);
            if (eventEntity == null)
                return ServiceResult.Fail("Event not found.");

            if (currentUserRole != 0 && (currentUserRole != 2 || eventEntity.CreatedBy != currentUserId))
                return ServiceResult.Fail("Unauthorized: You do not have permission to manage ticket types for this event.");

            if (eventEntity.Status == 2)
                return ServiceResult.Fail("Cannot configure ticket types for a cancelled event.");

            if (eventEntity.TimeStart <= DateTime.Now)
                return ServiceResult.Fail("Cannot configure ticket types for an event that has already started.");

            // Do not delete ticket type if it already has sold Ticket records
            var hasSold = await _ticketTypeRepo.HasSoldTicketsAsync(id);
            if (hasSold)
                return ServiceResult.Fail("Cannot delete ticket type because there are already sold tickets of this type.");

            _ticketTypeRepo.Delete(existingTicketType);
            await _ticketTypeRepo.SaveChangesAsync();

            return ServiceResult.Ok();
        }
    }
}
