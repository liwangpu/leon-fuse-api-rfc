using System.Collections.Generic;

namespace ApiModel.Entities
{
    /// <summary>
    /// 方案的一个区域的数据
    /// </summary>
    public class SolutionRegionData
    {
        /// <summary>
        /// 对应户型中的区域
        /// </summary>
        public string RegionId { get; set; }
        /// <summary>
        /// 区域中摆放的物品信息
        /// </summary>
        public List<AreaItem> Items { get; set; }
    }
}
