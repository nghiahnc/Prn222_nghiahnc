using System.ComponentModel.DataAnnotations;
using Domain;
using Services;

namespace Prn222_Nghiahnc_Mvc.Models
{
    public class BookingHistoryViewModel
    {
        public IList<Booking> Bookings { get; set; } = new List<Booking>();
    }

    public class BookingDetailsViewModel
    {
        public BookingDetailsResult Details { get; set; } = default!;

        public string? RazorBaseUrl { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }
    }

    public static class CustomerLabels
    {
        public static string BookingStatus(int status)
        {
            return CustomerWorkflowService.BookingStatusLabel(status);
        }

        public static string PaymentStatus(int status)
        {
            return CustomerWorkflowService.TransactionStatusLabel(status);
        }
    }
}
