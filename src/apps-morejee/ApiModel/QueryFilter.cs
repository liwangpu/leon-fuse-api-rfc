using System;
using System.Collections.Generic;
using System.Text;

namespace ApiModel
{
    public class QueryFilter
    {
        public string Field { get; set; }
        public string Operate { get; set; }
        public string Value { get; set; }
    }
}
