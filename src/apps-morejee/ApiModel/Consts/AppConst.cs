namespace ApiModel.Consts
{
    public class AppConst
    {
        //用户角色
        public const string AccountType_SysAdmin = "sysadmin";
        public const string AccountType_SysService = "sysservice";
        public const string AccountType_BrandAdmin = "brandadmin";
        public const string AccountType_BrandMember = "brandmember";
        public const string AccountType_PartnerAdmin = "partneradmin";
        public const string AccountType_PartnerMember = "partnermember";
        public const string AccountType_SupplierAdmin = "supplieradmin";
        public const string AccountType_SupplierMember = "suppliermember";
        //组织类型
        public const string OrganType_Brand = "brand";
        public const string OrganType_Partner = "partner";
        public const string OrganType_Supplier = "supplier";
        //数据状态
        public const int I_DataState_InActive = 0;
        public const int I_DataState_Active = 1;
        //权限树节点类型
        public const string S_NodeType_Organization = "Organization";
        public const string S_NodeType_Department = "Department";
        public const string S_NodeType_Account = "Account";
        //分类树节点类型
        public const string S_NodeType_Product = "Product";
        //媒体文件类型
        public const string S_MediaType_ScreenShot = "ScreenShot";
        //Query Operate
        public const string S_QueryOperate_Eq = "eq";
        public const string S_QueryOperate_Lt = "lt";
        public const string S_QueryOperate_Le = "le";
        public const string S_QueryOperate_Gt = "gt";
        public const string S_QueryOperate_Ge = "ge";
        public const string S_QueryOperate_Like = "like";

        /// <summary>
        /// 不同层级之间差距
        /// </summary>
        public const int I_Permission_GradeStep = 10;
        /// <summary>
        /// 不同类型直接差距
        /// </summary>
        public const int I_Permission_TypeStep = 100;


        public const string S_Category_Product = "product";
        public const string S_Category_Material = "material";
        public const string S_Category_ProductGroup = "product-group";

        public const string BambooAdminId = "admin";
        public const string BambooOrganId = "bamboo";
    }
}
