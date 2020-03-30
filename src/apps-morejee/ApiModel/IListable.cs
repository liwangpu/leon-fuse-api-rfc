using ApiModel.Entities;

namespace ApiModel
{
    /// <summary>
    /// 列表类型资源接口
    /// </summary>
    public interface IListable : IEntity
    {
        string Icon { get; set; }
        string IconFileAssetUrl { get; set; }
    }
}
