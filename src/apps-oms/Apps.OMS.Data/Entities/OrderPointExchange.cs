namespace Apps.OMS.Data.Entities
{
    public class OrderPointExchange
    {
        public string Id { get; set; }
        /// <summary>
        /// 该比率是1元兑多少积分
        /// </summary>
        public decimal Rate { get; set; }
        public string OrganizationId { get; set; }
    }
}
