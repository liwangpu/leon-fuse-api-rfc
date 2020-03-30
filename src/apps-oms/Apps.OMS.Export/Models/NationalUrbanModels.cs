using System;
using System.Collections.Generic;
using System.Text;

namespace Apps.OMS.Export.Models
{
    public class NationalUrbanQueryModel
    {
        public string NationalUrbanTypes { get; set; }
        public string ParentId { get; set; }
        public string Search { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; }
        public bool? Desc { get; set; }
    }
}
