namespace ApiModel
{
    #region DataBase 最基础的数据实体(供继承)
    /// <summary>
    /// 最基础的数据实体(供继承)
    /// </summary>
    public class DataBase : IData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 判断数据是否持久化
        /// </summary>
        /// <returns></returns>
        public bool IsPersistence()
        {
            if (!string.IsNullOrWhiteSpace(Id))
                return true;
            return false;
        }
    }
    #endregion
}
