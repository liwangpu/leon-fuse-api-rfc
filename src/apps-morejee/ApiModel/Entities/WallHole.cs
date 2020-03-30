using BambooCommon;
namespace ApiModel.Entities
{
    /// <summary>
    /// 表示墙上的洞，门，窗，壁龛等
    /// </summary>
    public class WallHole : DataBase
    {
        /// <summary>
        /// 在当前户型里唯一的ID
        /// </summary>
        public Vector2 Location { get; set; }
        /// <summary>
        /// 如果Path为空则此洞为标准的长方体洞。此属性表示洞的长宽的一半的数值
        /// </summary>
        public Vector2 HalfSize { get; set; }
        /// <summary>
        /// 洞的深度
        /// </summary>
        public int Thickness { get; set; }
        /// <summary>
        /// 如果此属性不为空，则表示此洞为一个复杂的封闭区域，由SVG的Path语法描述一个封闭区域。
        /// </summary>
        public string Path { get; set; }
    }
}
