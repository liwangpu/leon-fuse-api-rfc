using BambooCommon;
namespace ApiModel.Entities
{
    /// <summary>
    /// 裙料，墙上条状装饰覆盖物的配置信息，比如踢脚线，顶角线，腰线等的位置与长度，材质。
    /// </summary>
    public class WallSkirting : DataBase
    {
        /// <summary>
        /// 在当前户型里唯一的ID
        /// </summary>

        public int WallId { get; set; }
        public string SkirtingId { get; set; }
        public Vector2 Location { get; set; }
        /// <summary>
        /// 踢脚线等为简单的直线延伸，则此属性表示延伸长度。如果启用Path属性，则忽略此属性
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// 因为裙料是紧随墙面弯曲的，这是指定其在墙面的2D平面上的形状，默认就是从墙的开始到结束的直线，https://developer.mozilla.org/zh-CN/docs/Web/SVG/Tutorial/Paths 
        /// </summary>
        public string Path { get; set; }
        public string MaterialId { get; set; }
    }
}
