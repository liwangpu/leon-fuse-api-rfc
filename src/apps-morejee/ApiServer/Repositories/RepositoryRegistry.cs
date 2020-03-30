using ApiModel.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace ApiServer.Repositories
{
    public class RepositoryRegistry
    {
        public static void Registry(IServiceCollection services)
        {
            services.AddScoped<IRepository<Navigation, NavigationDTO>, NavigationRepository>();
            services.AddScoped<IRepository<UserNav, UserNavDTO>, UserNavRepository>();
            services.AddScoped<ITreeRepository<PermissionTree>, PermissionTreeRepository>();
            services.AddScoped<ITreeRepository<AssetCategoryTree>, AssetCategoryTreeRepository>();
            services.AddScoped<IRepository<Map, MapDTO>, MapRepository>();
            services.AddScoped<IRepository<Account, AccountDTO>, AccountRepository>();
            services.AddScoped<IRepository<Department, DepartmentDTO>, DepartmentRepository>();
            services.AddScoped<IRepository<Organization, OrganizationDTO>, OrganizationRepository>();
            services.AddScoped<IRepository<AssetCategory, AssetCategoryDTO>, AssetCategoryRepository>();
            services.AddScoped<IRepository<FileAsset, FileAssetDTO>, FileRepository>();
            services.AddScoped<IRepository<Media, MediaDTO>, MediaRepository>();
            services.AddScoped<IRepository<MediaShareResource, MediaShareResourceDTO>, MediaShareRepository>();
            services.AddScoped<IRepository<AreaType, AreaTypeDTO>, AreaTypeRepository>();
            services.AddScoped<IRepository<Layout, LayoutDTO>, LayoutRepository>();
            services.AddScoped<IRepository<Material, MaterialDTO>, MaterialRepository>();
            services.AddScoped<IRepository<Package, PackageDTO>, PackageRepository>();
            services.AddScoped<IRepository<ProductGroup, ProductGroupDTO>, ProductGroupRepository>();
            services.AddScoped<IRepository<Product, ProductDTO>, ProductRepository>();
            services.AddScoped<IRepository<ProductSpec, ProductSpecDTO>, ProductSpecRepository>();
            services.AddScoped<IRepository<Solution, SolutionDTO>, SolutionRepository>();
            services.AddScoped<IRepository<StaticMesh, StaticMeshDTO>, StaticMeshRepository>();
            services.AddScoped<IRepository<Texture, TextureDTO>, TextureRepository>();
            //services.AddScoped<IRepository<Order, OrderDTO>, OrderRepository>();
            services.AddScoped<IRepository<ProductReplaceGroup, ProductReplaceGroupDTO>, ProductReplaceGroupRepository>();
            services.AddScoped<IRepository<UserRole, UserRoleDTO>, UserRoleRepository>();
            //services.AddScoped<IRepository<WorkFlow, WorkFlowDTO>, WorkFlowRepository>();
            //services.AddScoped<IRepository<WorkFlowRule, WorkFlowRuleDTO>, WorkFlowRuleRepository>();
            services.AddScoped<IRepository<OrganizationType, OrganizationTypeDTO>, OrganizationTypeRepository>();
            services.AddScoped<IRepository<MemberRegistry, MemberRegistryDTO>, MemberRegistryRepository>();
            services.AddScoped<ISettingRepository, SettingRepository>();

        }
    }
}
