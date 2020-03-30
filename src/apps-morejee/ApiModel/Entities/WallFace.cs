namespace ApiModel.Entities
{
    /// <summary>
    /// 墙面，一堵墙有多个面，墙面可以被划在一个区域中。但是墙体属于整个户型
    /// </summary>
    public class WallFace : DataBase
    {
        public int WallId { get; set; }
        public int FaceIndex { get; set; }
        public string Remark { get; set; }
    }
}
