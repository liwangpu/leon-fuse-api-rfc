namespace Apps.OMS.Data.Entities
{
    public class OrderPackage
    {
        public string Id { get; set; }
        public string ProductPackageId { get; set; }
        public int Num { get; set; }
        public string OrderId { get; set; }
        public Order Order { get; set; }
    }
}
