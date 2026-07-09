using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain;

namespace Prn222_Nghiahnc_Mvc.Models
{
    public class TicketTypeFormViewModel
    {
        public int? Id { get; set; }
        
        [Required(ErrorMessage = "Event selection is required.")]
        public int EventId { get; set; }
        
        [Required(ErrorMessage = "Ticket type Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Price is required.")]
        [Range(50000, 100000000, ErrorMessage = "Price must be between 50,000 ₫ and 100,000,000 ₫.")]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, 100000, ErrorMessage = "Quantity must be between 1 and 100,000.")]
        public int Quantity { get; set; }
        
        public List<Event> Events { get; set; } = new List<Event>();
    }
}
