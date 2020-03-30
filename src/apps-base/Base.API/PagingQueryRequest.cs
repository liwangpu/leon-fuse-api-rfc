namespace Base.API
{
    public class PagingQueryRequest
    {
        /// <summary>
        /// 页码(从1开始)
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// 分页记录大小
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string OrderBy { get; set; }
        /// <summary>
        /// 降序
        /// </summary>
        public bool Desc { get; set; }
        /// <summary>
        /// 搜索关键词,默认是Name
        /// </summary>
        public string Search { get; set; }

        /// <summary>
        /// 校验并修复分页参数信息
        /// </summary>
        public void CheckPagingParam()
        {
            Page = Page < 1 ? 1 : Page;
            PageSize = PageSize < 1 ? 15 : PageSize;
        }
    }
}
