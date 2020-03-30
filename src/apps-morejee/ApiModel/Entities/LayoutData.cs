using System.Collections.Generic;

namespace ApiModel.Entities
{
    /// <summary>
    /// Layout的Data属性的实际结构
    /// </summary>
    public class LayoutData
    {
        /// <summary>
        /// 墙体
        /// </summary>
        public List<Wall> Walls { get; set; }
        /// <summary>
        /// 区域定义
        /// </summary>
        public List<Region> Regions { get; set; }

        /// <summary>
        /// 下次分配对象时用的ID，从1开始，最大9999. 方案里面的对象从10000开始
        /// </summary>
        public int NextId { get; set; }
    }
}
