using ApiModel.Consts;
using ApiModel.Entities;
using ApiServer.Data;
using BambooCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ApiServer
{
    public class DatabaseInitTool
    {
        /// <summary>
        /// 应用备份文件夹
        /// </summary>
        public const string BackupFoler = "AppBackup";


        public static void InitDatabase(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, ApiDbContext context)
        {
            var contentRoot = env.ContentRootPath;
            var backupFolder = Path.Combine(contentRoot, "wwwroot", BackupFoler);
            if (!Directory.Exists(backupFolder))
                Directory.CreateDirectory(backupFolder);

            #region 创建基础用户角色
            {
                var innerRoles = context.UserRoles.Where(x => x.IsInner == true).ToList();
                if (innerRoles.Count <= 0)
                    Console.WriteLine("Auto Create Inner UserRoles");

                //超级管理员
                if (innerRoles.Where(x => x.Id == UserRoleConst.SysAdmin).Count() <= 0)
                {
                    var role = new UserRole();
                    role.Id = UserRoleConst.SysAdmin;
                    role.ApplyOrgans = OrganTyeConst.System_Supplier;
                    role.Name = "系统超级管理员";
                    role.Creator = AppConst.BambooAdminId;
                    role.Modifier = AppConst.BambooAdminId;
                    role.CreatedTime = DateTime.Now;
                    role.ModifiedTime = DateTime.Now;
                    role.ActiveFlag = 1;
                    role.IsInner = true;
                    context.UserRoles.Add(role);
                }
                //系统客服
                if (innerRoles.Where(x => x.Id == UserRoleConst.SysService).Count() <= 0)
                {
                    var role = new UserRole();
                    role.Id = UserRoleConst.SysService;
                    role.ApplyOrgans = OrganTyeConst.System_Supplier;
                    role.Name = "系统客服";
                    role.Creator = AppConst.BambooAdminId;
                    role.Modifier = AppConst.BambooAdminId;
                    role.CreatedTime = DateTime.Now;
                    role.ModifiedTime = DateTime.Now;
                    role.ActiveFlag = 1;
                    role.IsInner = true;
                    context.UserRoles.Add(role);
                }
                //品牌商管理员
                if (innerRoles.Where(x => x.Id == UserRoleConst.BrandAdmin).Count() <= 0)
                {
                    var role = new UserRole();
                    role.Id = UserRoleConst.BrandAdmin;
                    role.ApplyOrgans = OrganTyeConst.Brand;
                    role.Name = "品牌商管理员";
                    role.Creator = AppConst.BambooAdminId;
                    role.Modifier = AppConst.BambooAdminId;
                    role.CreatedTime = DateTime.Now;
                    role.ModifiedTime = DateTime.Now;
                    role.ActiveFlag = 1;
                    role.IsInner = true;
                    context.UserRoles.Add(role);
                }
                //品牌商管用户
                if (innerRoles.Where(x => x.Id == UserRoleConst.BrandMember).Count() <= 0)
                {
                    var role = new UserRole();
                    role.Id = UserRoleConst.BrandMember;
                    role.ApplyOrgans = OrganTyeConst.Brand;
                    role.Name = "品牌商用户";
                    role.Creator = AppConst.BambooAdminId;
                    role.Modifier = AppConst.BambooAdminId;
                    role.CreatedTime = DateTime.Now;
                    role.ModifiedTime = DateTime.Now;
                    role.ActiveFlag = 1;
                    role.IsInner = true;
                    context.UserRoles.Add(role);
                }
                //代理商管理员
                if (innerRoles.Where(x => x.Id == UserRoleConst.PartnerAdmin).Count() <= 0)
                {
                    var role = new UserRole();
                    role.Id = UserRoleConst.PartnerAdmin;
                    role.ApplyOrgans = OrganTyeConst.Partner;
                    role.Name = "代理商管理员";
                    role.Creator = AppConst.BambooAdminId;
                    role.Modifier = AppConst.BambooAdminId;
                    role.CreatedTime = DateTime.Now;
                    role.ModifiedTime = DateTime.Now;
                    role.ActiveFlag = 1;
                    role.IsInner = true;
                    context.UserRoles.Add(role);
                }
                //代理商用户
                if (innerRoles.Where(x => x.Id == UserRoleConst.PartnerMember).Count() <= 0)
                {
                    var role = new UserRole();
                    role.Id = UserRoleConst.PartnerMember;
                    role.ApplyOrgans = OrganTyeConst.Partner;
                    role.Name = "代理商用户";
                    role.Creator = AppConst.BambooAdminId;
                    role.Modifier = AppConst.BambooAdminId;
                    role.CreatedTime = DateTime.Now;
                    role.ModifiedTime = DateTime.Now;
                    role.ActiveFlag = 1;
                    role.IsInner = true;
                    context.UserRoles.Add(role);
                }
                //供应商管理员
                if (innerRoles.Where(x => x.Id == UserRoleConst.SupplierAdmin).Count() <= 0)
                {
                    var role = new UserRole();
                    role.Id = UserRoleConst.SupplierAdmin;
                    role.ApplyOrgans = OrganTyeConst.Supplier;
                    role.Name = "供应商管理员";
                    role.Creator = AppConst.BambooAdminId;
                    role.Modifier = AppConst.BambooAdminId;
                    role.CreatedTime = DateTime.Now;
                    role.ModifiedTime = DateTime.Now;
                    role.ActiveFlag = 1;
                    role.IsInner = true;
                    context.UserRoles.Add(role);
                }
                //供应商用户
                if (innerRoles.Where(x => x.Id == UserRoleConst.SupplierMember).Count() <= 0)
                {
                    var role = new UserRole();
                    role.Id = UserRoleConst.SupplierMember;
                    role.ApplyOrgans = OrganTyeConst.Supplier;
                    role.Name = "供应商用户";
                    role.Creator = AppConst.BambooAdminId;
                    role.Modifier = AppConst.BambooAdminId;
                    role.CreatedTime = DateTime.Now;
                    role.ModifiedTime = DateTime.Now;
                    role.ActiveFlag = 1;
                    role.IsInner = true;
                    context.UserRoles.Add(role);
                }
                context.SaveChanges();
            }
            #endregion

            #region 创建基础组织类型
            {
                var innerOrganTypes = context.OrganizationTypes.Where(x => x.IsInner == true).ToList();
                if (innerOrganTypes.Count <= 0)
                    Console.WriteLine("Auto Create Inner Organization Type");

                if (innerOrganTypes.Where(x => x.Id == OrganTyeConst.System_Supplier).Count() <= 0)
                {
                    var organType = new OrganizationType();
                    organType.Id = OrganTyeConst.System_Supplier;
                    organType.Name = "系统供应商";
                    organType.Creator = AppConst.BambooAdminId;
                    organType.Modifier = AppConst.BambooAdminId;
                    organType.CreatedTime = DateTime.Now;
                    organType.ModifiedTime = DateTime.Now;
                    organType.ActiveFlag = 1;
                    organType.IsInner = true;
                    context.OrganizationTypes.Add(organType);
                }

                if (innerOrganTypes.Where(x => x.Id == OrganTyeConst.Brand).Count() <= 0)
                {
                    var organType = new OrganizationType();
                    organType.Id = OrganTyeConst.Brand;
                    organType.Name = "品牌商";
                    organType.Creator = AppConst.BambooAdminId;
                    organType.Modifier = AppConst.BambooAdminId;
                    organType.CreatedTime = DateTime.Now;
                    organType.ModifiedTime = DateTime.Now;
                    organType.ActiveFlag = 1;
                    organType.IsInner = true;
                    context.OrganizationTypes.Add(organType);
                }

                if (innerOrganTypes.Where(x => x.Id == OrganTyeConst.Partner).Count() <= 0)
                {
                    var organType = new OrganizationType();
                    organType.Id = OrganTyeConst.Partner;
                    organType.Name = "代理商";
                    organType.Creator = AppConst.BambooAdminId;
                    organType.Modifier = AppConst.BambooAdminId;
                    organType.CreatedTime = DateTime.Now;
                    organType.ModifiedTime = DateTime.Now;
                    organType.ActiveFlag = 1;
                    organType.IsInner = true;
                    context.OrganizationTypes.Add(organType);
                }

                if (innerOrganTypes.Where(x => x.Id == OrganTyeConst.Supplier).Count() <= 0)
                {
                    var organType = new OrganizationType();
                    organType.Id = OrganTyeConst.Supplier;
                    organType.Name = "供应商";
                    organType.Creator = AppConst.BambooAdminId;
                    organType.Modifier = AppConst.BambooAdminId;
                    organType.CreatedTime = DateTime.Now;
                    organType.ModifiedTime = DateTime.Now;
                    organType.ActiveFlag = 1;
                    organType.IsInner = true;
                    context.OrganizationTypes.Add(organType);
                }

                context.SaveChanges();

            }
            #endregion

            #region 创建基础组织
            {
                var existBambooOrgan = context.Organizations.Where(x => x.Id == "bamboo").Count() > 0;
                if (!existBambooOrgan)
                {
                    Console.WriteLine("Auto Create Bamboo Organization");
                    var organ = new Organization();
                    organ.Id = AppConst.BambooOrganId;
                    organ.Name = "竹烛信息科技有限公司";
                    organ.OrganizationTypeId = OrganTyeConst.System_Supplier;
                    organ.ActivationTime = DateTime.Now.AddDays(-7);
                    organ.ExpireTime = DateTime.Now.AddYears(100);
                    organ.OwnerId = AppConst.BambooAdminId;
                    organ.Creator = AppConst.BambooAdminId;
                    organ.Modifier = AppConst.BambooAdminId;
                    organ.CreatedTime = DateTime.Now;
                    organ.ModifiedTime = DateTime.Now;
                    organ.ActiveFlag = 1;
                    context.Organizations.Add(organ);
                    context.SaveChanges();
                }
            }
            #endregion

            #region 创建基础用户
            {
                var existAdmin = context.Accounts.Where(x => x.Id == AppConst.BambooAdminId).Count() > 0;
                if (!existAdmin)
                {
                    Console.WriteLine("Auto Create Admin Account");
                    var admin = new Account();
                    admin.Id = AppConst.BambooAdminId;
                    admin.Name = "系统超级管理员";
                    admin.Mail = AppConst.BambooAdminId;
                    admin.Phone = "15721457986";
                    admin.Location = "上海徐汇区漕宝路航天大厦1405号";
                    admin.Password = Md5.CalcString("bambooAdmin");
                    admin.Type = UserRoleConst.SysAdmin;
                    admin.ActivationTime = DateTime.Now.AddDays(-1);
                    admin.ExpireTime = DateTime.Now.AddYears(100);
                    admin.Creator = AppConst.BambooAdminId;
                    admin.Modifier = AppConst.BambooAdminId;
                    admin.CreatedTime = DateTime.Now;
                    admin.ModifiedTime = DateTime.Now;
                    admin.ActiveFlag = 1;
                    admin.OrganizationId = AppConst.BambooOrganId;
                    context.Accounts.Add(admin);
                    context.SaveChanges();
                }
            }
            #endregion

            #region 备份或者恢复导航栏项
            {
                var navs = context.Navigations.ToList();
                var filePath = Path.Combine(backupFolder, "navigations.json");
                if (navs.Count <= 0)
                {
                    //从json恢复导航栏项信息
                    if (File.Exists(filePath))
                    {
                        var strNavs = File.ReadAllText(filePath);
                        var navList = JsonConvert.DeserializeObject<List<Navigation>>(strNavs);
                        foreach (var item in navList)
                            context.Navigations.Add(item);
                        context.SaveChanges();
                        Console.WriteLine("Auto Recover Navigations From Backup");
                    }
                }
                else
                {
                    //备份导航栏项到json
                    var strNavs = JsonConvert.SerializeObject(navs);
                    File.WriteAllText(filePath, strNavs);
                    Console.WriteLine("Auto Backup Navigations");
                }
            }
            #endregion

            #region 备份或者恢复基础用户角色导航栏
            {

                #region 角色导航栏
                {
                    var userNavs = context.UserNavs.ToList();
                    var filePath = Path.Combine(backupFolder, "user-navs.json");
                    if (userNavs.Count <= 0)
                    {
                        //从json恢复角色导航栏信息
                        //从json恢复导航栏项信息
                        if (File.Exists(filePath))
                        {
                            var strUserNavs = File.ReadAllText(filePath);
                            var userNavList = JsonConvert.DeserializeObject<List<UserNav>>(strUserNavs);
                            foreach (var item in userNavList)
                                context.UserNavs.Add(item);
                            context.SaveChanges();
                            Console.WriteLine("Auto Recover UserNavs From Backup");
                        }
                    }
                    else
                    {
                        //备份角色导航栏信息到json
                        var strUserNavs = JsonConvert.SerializeObject(userNavs);
                        File.WriteAllText(filePath, strUserNavs);
                        Console.WriteLine("Auto Backup UserNavs");
                    }
                }
                #endregion

                #region 角色导航栏详情
                {
                    var userNavDetails = context.UserNavDetails.ToList();
                    var filePath = Path.Combine(backupFolder, "user-nav-details.json");
                    if (userNavDetails.Count <= 0)
                    {
                        //从json恢复角色导航栏详情信息
                        if (File.Exists(filePath))
                        {
                            var strUserNavDetails = File.ReadAllText(filePath);
                            var userNavDetailList = JsonConvert.DeserializeObject<List<UserNavDetail>>(strUserNavDetails);
                            foreach (var item in userNavDetailList)
                                context.UserNavDetails.Add(item);
                            context.SaveChanges();
                            Console.WriteLine("Auto Recover UserNavDetails From Backup");
                        }
                    }
                    else
                    {
                        //备份角色导航栏详情信息到json
                        //清空UserNav不然JsonConvert会报循环应用
                        for (int idx = userNavDetails.Count - 1; idx >= 0; idx--)
                        {
                            var item = userNavDetails[idx];
                            item.UserNavId = item.UserNav.Id;
                            item.UserNav = null;
                        }
                        var strUserNavDetails = JsonConvert.SerializeObject(userNavDetails);
                        File.WriteAllText(filePath, strUserNavDetails);
                        Console.WriteLine("Auto Backup UserNavDetails");
                    }
                }
                #endregion

            }
            #endregion
        }
    }
}
