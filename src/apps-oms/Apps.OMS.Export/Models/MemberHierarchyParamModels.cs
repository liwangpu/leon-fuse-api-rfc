using System;
using System.Collections.Generic;
using System.Text;

namespace Apps.OMS.Export.Models
{
    public class MemberHierarchyParamSettingUpdateModel
    {
        public string MemberHierarchyParamId { get; set; }
        public decimal Rate { get; set; }
    }

    public class OrderPointExchangeUpdateModel
    {
        public decimal Rate { get; set; }
    }
}
