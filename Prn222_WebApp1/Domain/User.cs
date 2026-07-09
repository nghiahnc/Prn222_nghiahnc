using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class User
    {
        public int Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public int Role { get; set; }

        public int? MembershipId { get; set; }

        public int Status { get; set; }

        public Membership? Membership { get; set; }
    }
}