using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryManagement.Enums
{
    public enum UserAccountStatus
    {
        NotSet = Int32.MinValue,
        Active = 1,
        Inactive = 2,
        LockedOut = 3,
        InvalidCredentials = 4,
        Success = 5,
        InactiveEmployer = 6,
        Unsupported = 7,
        InvalidEmail = 8,  //used for forgot password
        AccountNotFound = 9,
        UsernameMismatch = 10
    }
}