using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryManagement.Models
{
    public class CommonBaseDTO
    {
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class CommonBaseDTO<IDType> : CommonBaseDTO  where IDType : struct
    {
        public IDType Id { get; set; }
       
    }
}