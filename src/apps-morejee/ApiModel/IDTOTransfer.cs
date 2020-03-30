namespace ApiModel
{
    /// <summary>
    /// To DTO数据转化接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDTOTransfer<T>
        where T : IData
    {
        T ToDTO();
    }

}
