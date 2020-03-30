namespace ApiModel
{
    /// <summary>
    /// 树形资源类型接口
    /// </summary>
    public interface ITree : IData
    {
        int LValue { get; set; }
        int RValue { get; set; }
        string ParentId { get; set; }
        string NodeType { get; set; }
        string ObjId { get; set; }
        string OrganizationId { get; set; }
        string RootOrganizationId { get; set; }
    }
}
