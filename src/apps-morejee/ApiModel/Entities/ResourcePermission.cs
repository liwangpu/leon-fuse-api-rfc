namespace ApiModel.Entities
{
    public class ResourcePermission
    {
        public string Id { get; set; }
        public int OpRetrieve { get; set; }
        public int OpUpdate { get; set; }
        public int OpDelete { get; set; }
        public string ResId { get; set; }
        public int ResType { get; set; }
        public string OrganizationId { get; set; }
    }
}
