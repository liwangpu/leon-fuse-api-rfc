using Apps.OMS.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Apps.OMS.Service.Contexts
{
    public class AppDbContext : DbContext
    {

        #region 构造函数
        protected AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        #endregion

        public DbSet<Member> Members { get; set; }

        public DbSet<MemberRegistry> MemberRegistries { get; set; }

        public DbSet<MemberHierarchyParam> MemberHierarchyParams { get; set; }

        public DbSet<MemberHierarchySetting> MemberHierarchySettings { get; set; }

        public DbSet<NationalUrban> NationalUrbans { get; set; }

        public DbSet<OrderPointExchange> OrderPointExchanges { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<OrderFlowLog> OrderFlowLogs { get; set; }

        public DbSet<WorkFlow> WorkFlows { get; set; }

        public DbSet<WorkFlowItem> WorkFlowItems { get; set; }

        public DbSet<WorkFlowRule> WorkFlowRules { get; set; }

        public DbSet<WorkFlowRuleDetail> WorkFlowRuleDetails { get; set; }

        public DbSet<ProductPackage> ProductPackages { get; set; }

        public DbSet<OrderDetailPackage> OrderDetailPackages { get; set; }

    }
}
