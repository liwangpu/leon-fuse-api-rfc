using System;
using System.Collections.Generic;
using System.Text;

namespace Apps.OMS.Export.DTOs
{
    public class NationalUrbanDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public string NationalUrbanType { get; set; }
        public List<NationalUrbanDTO> Children { get; set; }
    }
}
