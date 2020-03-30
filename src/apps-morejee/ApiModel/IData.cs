namespace ApiModel
{
    /// <summary>
    /// 基础数据实体接口
    /// </summary>
    public interface IData
    {
        string Id { get; set; }
        string Name { get; set; }

        bool IsPersistence();
    }
}
