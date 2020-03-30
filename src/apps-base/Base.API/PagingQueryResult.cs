using System.Collections.Generic;

namespace Base.API
{
    public class PagingQueryResult<DTO>
        where DTO : class
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public int? Count { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public List<DTO> Items { get; set; }
    }
}
