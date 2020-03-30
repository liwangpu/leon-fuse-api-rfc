using System.Collections.Generic;

namespace ApiModel.Entities
{
    /// <summary>
    /// 区域，一个二维的封闭多边形拉高而成的三维几何体的区域
    /// </summary>
    public class Region
    {
        /// <summary>
        /// 区域名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 包围盒大小，单位厘米，格式 x,y,z，使用Vector3类来解析
        /// </summary>
        public string BoundBoxVec { get; set; }
        /// <summary>
        /// 区域的面积，单位 平方米
        /// </summary>
        public float AreaCentare { get; set; }
        /// <summary>
        /// 区域的体积，立方米
        /// </summary>
        public float VolumeStere { get; set; }
        /// <summary>
        /// 组成这个区域的二位多边形的顶点信息。格式为 x1,y1;x2,y2;...xn,yn;
        /// </summary>
        public string Points { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public float Height { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 所有的墙体
        /// </summary>
        public List<WallFace> Walls { get; set; }

    }
}
