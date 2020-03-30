using BambooCommon;
using System.Collections.Generic;

namespace ApiModel.Entities
{
    public class Wall : DataBase
    {
        /// <summary>
        /// 厚度，mm
        /// </summary>
        public int ThicknessMM { get; set; }
        /// <summary>
        /// 长度，mm
        /// </summary>
        public int LengthMM { get; set; }
        /// <summary>
        /// 高度, mm
        /// </summary>
        public int HeightMM { get; set; }

        /// <summary>
        /// 定义墙体在水平平面的形状。如果Path为空，则墙体为简单的长方体，尺寸为length,height,thickness。否则使用SVG的Path语法定义形状。直线，贝塞尔曲线，圆弧
        /// </summary>
        public string Path { get; set; }

        public Vector2 StartPoint { get; set; }

        public Vector2 EndPoint { get; set; }

        /// <summary>
        /// 墙上的洞，门窗之类的
        /// </summary>
        public List<WallHole> Holes { get; set; }
        /// <summary>
        /// 踢脚线，顶角线，腰线等装饰
        /// </summary>
        public List<WallSkirting> Skirtings { get; set; }
    }
}
