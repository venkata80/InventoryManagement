using InventoryManagement.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryManagement.Models
{
    public class UserSecurityToken
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string Fullname { get { return string.Concat(FirstName, " ", LastName); } }

        public string Email { get; set; }

        public RoleType Role { get; set; }
    }
}