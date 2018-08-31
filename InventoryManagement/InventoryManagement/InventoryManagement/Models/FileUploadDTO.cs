using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryManagement.Models
{
    public class FileUploadDTO
    {
        public long ID { get; set; }
        public string FileName { get; set; }
        public string Base64String { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}