namespace ApiModel
{
    /// <summary>
    /// 树形结构实体(供继承)
    /// </summary>
    public class TreeBase : DataBase, ITree
    {
        public int LValue { get; set; }
        public int RValue { get; set; }
        public string ParentId { get; set; }
        public string NodeType { get; set; }
        public string ObjId { get; set; }
        public string OrganizationId { get; set; }
        public string RootOrganizationId { get; set; }
    }
}
