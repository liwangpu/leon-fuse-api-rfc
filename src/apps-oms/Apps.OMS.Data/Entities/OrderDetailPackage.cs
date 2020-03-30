using System;
using System.Collections.Generic;
using System.Text;

namespace Apps.OMS.Data.Entities
{
    public class OrderDetailPackage
    {
        public string Id { get; set; }
        public string ProductPackageId { get; set; }
        public decimal Num { get; set; }
        public string OrderDetailId { get; set; }
        public OrderDetail OrderDetail { get; set; }
    }
}
