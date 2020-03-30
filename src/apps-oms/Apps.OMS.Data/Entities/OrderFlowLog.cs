using System;

namespace Apps.OMS.Data.Entities
{
    public class OrderFlowLog
    {
        public string Id { get; set; }
        public bool Approve { get; set; }
        public string Remark { get; set; }
        public string WorkFlowItemId { get; set; }
        public string OrderId { get; set; }
        public Order Order { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Creator { get; set; }
    }
}
