using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryManagement.Enums
{
    public class RoleType
    {
        public Guid Guid { get; }
        public string Name { get; }

        public RoleType(Guid guid, string name)
        {
            this.Guid = guid;
            this.Name = name;
        }

        public static readonly RoleType None = new RoleType(Guid.Parse("00000000-0000-0000-0000-000000000000"), "None");
        public static readonly RoleType Admin = new RoleType(Guid.Parse("0017E6E5-8D2D-445E-A07F-A494F66F6469"), "Admin");
        public static readonly RoleType EmployerAdmin = new RoleType(Guid.Parse("2D5CA9D5-0F00-4662-9AD8-A68A24191EC2"), "EmployerAdmin");
        public static readonly RoleType EmployerUser = new RoleType(Guid.Parse("74EF7BF7-D9E3-40F7-9B05-B37A92A6C01F"), "EmployerUser");
        public static readonly RoleType Supplier = new RoleType(Guid.Parse("D1E24854-56DB-4266-83B1-C2E7C14F35C5"), "Supplier");

        public static RoleType GetRoleType(string guid)
        {
            RoleType result = RoleType.None;
            switch (guid)
            {
                case "None":
                    {
                        result = RoleType.None;
                        break;
                    }
                case "Admin":
                    {
                        result = RoleType.Admin;
                        break;
                    }
                case "EmployerAdmin":
                    {
                        result = RoleType.EmployerAdmin;
                        break;
                    }
                case "EmployerUser":
                    {
                        result = RoleType.EmployerUser;
                        break;
                    }
                case "Supplier":
                    {
                        result = RoleType.Supplier;
                        break;
                    }
                default:
                    break;
            }
            return result;
        }
    }
}