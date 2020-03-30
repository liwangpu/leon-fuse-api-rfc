using System.Collections.Generic;

namespace ApiModel.Entities
{
    /// <summary>
    /// 方案的数据结构，在内存和客户端使用。服务端只是存储这个对象的json字符串格式 到 Solution::Data属性
    /// </summary>
    public class SolutionData
    {
        public List<SolutionRegionData> Regions { get; set; }
    }
}
