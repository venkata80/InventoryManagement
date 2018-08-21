using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryManagement.Util
{
    public class Response
    {
        public string Message { get; set; } = string.Empty;

        public object Result { get; set; }

        public AjaxResponse Status { get; set; } = AjaxResponse.Failed;
    }

    public enum AjaxResponse
    {
        Success = 1,
        Failed = 2,
        SessionExpired = 3,
        ModelError = 4,
        Exception = 5
    }
}