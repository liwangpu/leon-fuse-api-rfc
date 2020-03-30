using ApiModel.Entities;
using BambooCommon;
using Microsoft.EntityFrameworkCore;

namespace ApiServer.Data
{

    public class ApiDbContext : DbContext
    {
        //global------------------------------------------------------------------

        /// <summary>
        /// 全局设置
        /// </summary>
        public DbSet<SettingsItem> Settings { get; set; }

        //account-----------------------------------------------------------------

        /// <summary>
        /// 账号
        /// </summary>
        public DbSet<Account> Accounts { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        public DbSet<UserRole> UserRoles { get; set; }

        /// <summary>
        /// 组织
        /// </summary>
        public DbSet<Organization> Organizations { get; set; }

        public DbSet<Department> Departments { get; set; }

        //assets------------------------------------------------------------------

        /// <summary>
        /// 文件夹
        /// </summary>
        public DbSet<AssetFolder> AssetFolders { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public DbSet<AssetCategory> AssetCategories { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public DbSet<AssetTag> AssetTags { get; set; }
        /// <summary>
        /// 客户端资源
        /// </summary>
        public DbSet<ClientAsset> ClientAssets { get; set; }
        /// <summary>
        /// 文件资源
        /// </summary>
        public DbSet<FileAsset> Files { get; set; }

        //production--------------------------------------------------------------

        /// <summary>
        /// 地图
        /// </summary>
        public DbSet<Product> Products { get; set; }
        /// <summary>
        /// 方案
        /// </summary>
        public DbSet<Solution> Solutions { get; set; }
        /// <summary>
        /// 户型
        /// </summary>
        public DbSet<Layout> Layouts { get; set; }
        /// <summary>
        /// 墙体装饰，踢脚线，顶角线等
        /// </summary>
        public DbSet<Skirting> Skirtings { get; set; }
        /// <summary>
        /// 订单
        /// </summary>
        public DbSet<Order> Orders { get; set; }

        /// <summary>
        /// 订单详情
        /// </summary>
        public DbSet<OrderDetail> OrderDetails { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public DbSet<PermissionItem> Permissions { get; set; }

        /// <summary>
        /// 产品规格
        /// </summary>
        public DbSet<ProductSpec> ProductSpec { get; set; }

        /// <summary>
        /// 模型
        /// </summary>
        public DbSet<StaticMesh> StaticMeshs { get; set; }

        /// <summary>
        /// 材质
        /// </summary>
        public DbSet<Material> Materials { get; set; }

        /// <summary>
        /// 贴图
        /// </summary>
        public DbSet<Texture> Textures { get; set; }

        /// <summary>
        /// 地图，场景
        /// </summary>
        public DbSet<Map> Maps { get; set; }

        /// <summary>
        /// 组织,部门,用户权限树
        /// </summary>
        public DbSet<PermissionTree> PermissionTrees { get; set; }

        /// <summary>
        /// 分类资源树
        /// </summary>
        public DbSet<AssetCategoryTree> AssetCategoryTrees { get; set; }

        /// <summary>
        /// 套餐
        /// </summary>
        public DbSet<Package> Packages { get; set; }

        /// <summary>
        /// 媒体文件
        /// </summary>
        public DbSet<Media> Medias { get; set; }

        /// <summary>
        /// 媒体文件分享
        /// </summary>
        public DbSet<MediaShareResource> MediaShareResources { get; set; }

        /// <summary>
        /// 套餐区域类型
        /// </summary>
        public DbSet<AreaType> AreaTypes { get; set; }

        /// <summary>
        /// 产品组
        /// </summary>
        public DbSet<ProductGroup> ProductGroups { get; set; }

        /// <summary>
        /// 收藏
        /// </summary>
        public DbSet<Collection> Collections { get; set; }

        /// <summary>
        /// 偏好设置
        /// </summary>
        public DbSet<Preference> Preferences { get; set; }

        /// <summary>
        /// 资源权限
        /// </summary>
        public DbSet<ResourcePermission> ResourcePermissions { get; set; }

        /// <summary>
        /// 产品替换组
        /// </summary>
        public DbSet<ProductReplaceGroup> ProductReplaceGroups { get; set; }

        /// <summary>
        /// 导航栏菜单
        /// </summary>
        public DbSet<Navigation> Navigations { get; set; }

        public DbSet<UserNav> UserNavs { get; set; }

        public DbSet<UserNavDetail> UserNavDetails { get; set; }

        public DbSet<WorkFlow> WorkFlows { get; set; }

        public DbSet<WorkFlowItem> WorkFlowItems { get; set; }

        public DbSet<WorkFlowRule> WorkFlowRules { get; set; }

        public DbSet<WorkFlowRuleDetail> WorkFlowRuleDetails { get; set; }

        public DbSet<OrganizationType> OrganizationTypes { get; set; }

        public DbSet<AccountRole> AccountRoles { get; set; }

        public DbSet<OrderFlowLog> OrderFlowLogs { get; set; }

        public DbSet<MemberTree> MemberTrees { get; set; }

        public DbSet<MemberRegistry> MemberRegistries { get; set; }
        //------------------------------------------------------------------------

        public ApiDbContext() { }

        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(@"Server=localhost;Database=dmz;User Id=postgres;Password=root");
            //optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-EA0ERLU\SQLEXPRESS;Initial Catalog=DMZ;Persist Security Info=True;User ID=sa;Password=root");
        }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<PermissionItem>().HasIndex(d => new { d.AccountId, d.ResId, d.ResType });
            b.Entity<ResourcePermission>().HasIndex(d => new { d.OrganizationId, d.ResType });
            b.Entity<FileAsset>().HasIndex(d => new { d.Md5 });
        }
    }
}
