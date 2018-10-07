using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace InventoryManagement.Models
{
    public class FileUploadDTO : CommonBaseDTO<Guid>
    {
        public string FileName { get; set; }
        public string Base64String { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}