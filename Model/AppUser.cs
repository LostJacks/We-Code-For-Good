using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class AppUser
    {
		public int Id { get; set; }

		public string FirstName { get; set; } = string.Empty;

		public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

		public string PasswordHash { get; set; } = string.Empty;

		public string UserName { get; set; } = string.Empty;

    }
}
