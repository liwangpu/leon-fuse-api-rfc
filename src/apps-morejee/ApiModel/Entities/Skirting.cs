using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiModel.Entities
{
    /// <summary>
    /// 墙上条状装饰覆盖物，由一个横截面拉长而成的物体，比如踢脚线，顶角线，腰线等。
    /// 轴心在左下角，靠墙的顶角上。
    /// </summary>
    public class Skirting : EntityBase, IAsset
    {
        public string Icon { get; set; }
    }
}
