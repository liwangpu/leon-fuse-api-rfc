
using Apps.Base.Common.Consts;
using Apps.OMS.Data.Consts;
using Apps.OMS.Data.Entities;
using Apps.OMS.Service.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Apps.OMS.Service
{
    public class DatabaseInitTool
    {
        /// <summary>
        /// 应用备份文件夹
        /// </summary>
        public const string BackupFoler = "AppBackup";

        /// <summary>
        /// 初始化数据库信息
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="context"></param>
        public static void InitDatabase(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, AppDbContext context)
        {
            var contentRoot = env.ContentRootPath;
            var backupFolder = Path.Combine(contentRoot, "wwwroot", BackupFoler);
            if (!Directory.Exists(backupFolder))
                Directory.CreateDirectory(backupFolder);


            #region 积分参数初始值
            {
                var hierachies = context.MemberHierarchyParams.Where(x => x.IsInner == true).ToList();
                if (hierachies.Count <= 0)
                    Console.WriteLine("Auto Create Inner Member Hierarchy Param");

                if (hierachies.Where(x => x.Id == MemberHierarchyParamConst.FirstHierarchy).Count() <= 0)
                {
                    var param = new MemberHierarchyParam();
                    param.Id = MemberHierarchyParamConst.FirstHierarchy;
                    param.Name = "第一级";
                    param.Creator = AppConst.BambooAdminId;
                    param.Modifier = AppConst.BambooAdminId;
                    param.CreatedTime = DateTime.Now;
                    param.ModifiedTime = DateTime.Now;
                    param.IsInner = true;
                    context.MemberHierarchyParams.Add(param);
                }

                if (hierachies.Where(x => x.Id == MemberHierarchyParamConst.SecondHierarchy).Count() <= 0)
                {
                    var param = new MemberHierarchyParam();
                    param.Id = MemberHierarchyParamConst.SecondHierarchy;
                    param.Name = "第二级";
                    param.Creator = AppConst.BambooAdminId;
                    param.Modifier = AppConst.BambooAdminId;
                    param.CreatedTime = DateTime.Now;
                    param.ModifiedTime = DateTime.Now;
                    param.IsInner = true;
                    context.MemberHierarchyParams.Add(param);
                }

                if (hierachies.Where(x => x.Id == MemberHierarchyParamConst.ThirdHierarchy).Count() <= 0)
                {
                    var param = new MemberHierarchyParam();
                    param.Id = MemberHierarchyParamConst.ThirdHierarchy;
                    param.Name = "第三级";
                    param.Creator = AppConst.BambooAdminId;
                    param.Modifier = AppConst.BambooAdminId;
                    param.CreatedTime = DateTime.Now;
                    param.ModifiedTime = DateTime.Now;
                    param.IsInner = true;
                    context.MemberHierarchyParams.Add(param);
                }
                context.SaveChanges();
            }
            #endregion

            #region 初始化工作流基础信息
            {
                var rules = context.WorkFlowRules.ToList();

                if (rules.Where(x => x.Id == WorkFlowRuleConst.OrderWorkFlow).Count() <= 0)
                {
                    var rule = new WorkFlowRule();
                    rule.Id = WorkFlowRuleConst.OrderWorkFlow;
                    rule.Name = "订单流程";
                    rule.Creator = AppConst.BambooAdminId;
                    rule.Modifier = AppConst.BambooAdminId;
                    rule.CreatedTime = DateTime.Now;
                    rule.ModifiedTime = DateTime.Now;
                    rule.IsInner = true;
                    context.WorkFlowRules.Add(rule);
                }

                context.SaveChanges();
            }
            #endregion

            #region 初始化省市区信息
            {
                var filePath = Path.Combine(backupFolder, "national-urban.json");
                //从json恢复省市区信息
                if (File.Exists(filePath))
                {
                    if (context.NationalUrbans.Count() <= 0)
                    {
                        var strUrbans = File.ReadAllText(filePath);
                        var urbanDatas = JsonConvert.DeserializeObject<List<NationalUrbanData>>(strUrbans);
                        if (urbanDatas != null)
                        {
                            Console.WriteLine("Auto Recover National Urban Data From Backup");
                            foreach (var province in urbanDatas)
                            {
                                var prov = new NationalUrban();
                                prov.Id = province.value;
                                prov.Name = Regex.Replace(province.label, @"\s", string.Empty);
                                prov.CodeNumber = Convert.ToInt32(province.value);
                                prov.NationalUrbanType = NationalUrbanTypeConst.Province;
                                prov.CreatedTime = DateTime.Now;
                                prov.Creator = AppConst.BambooAdminId;
                                prov.ModifiedTime = prov.CreatedTime;
                                prov.Modifier = AppConst.BambooAdminId;
                                context.NationalUrbans.Add(prov);

                                if (province.children != null)
                                {
                                    foreach (var city in province.children)
                                    {
                                        var cit = new NationalUrban();
                                        cit.Id = city.value;
                                        cit.Name = Regex.Replace(city.label, @"\s", string.Empty);
                                        cit.CodeNumber = Convert.ToInt32(city.value);
                                        cit.NationalUrbanType = NationalUrbanTypeConst.City;
                                        cit.CreatedTime = DateTime.Now;
                                        cit.Creator = AppConst.BambooAdminId;
                                        cit.ModifiedTime = cit.CreatedTime;
                                        cit.Modifier = AppConst.BambooAdminId;
                                        cit.ParentId = prov.Id;
                                        context.NationalUrbans.Add(cit);

                                        if (city.children != null)
                                        {
                                            foreach (var county in city.children)
                                            {
                                                var count = new NationalUrban();
                                                count.Id = county.value;
                                                count.Name = Regex.Replace(county.label, @"\s", string.Empty);
                                                count.CodeNumber = Convert.ToInt32(county.value);
                                                count.NationalUrbanType = NationalUrbanTypeConst.County;
                                                count.CreatedTime = DateTime.Now;
                                                count.Creator = AppConst.BambooAdminId;
                                                count.ModifiedTime = count.CreatedTime;
                                                count.Modifier = AppConst.BambooAdminId;
                                                count.ParentId = cit.Id;
                                                context.NationalUrbans.Add(count);
                                            }//foreach
                                        }
                                    }//foreach
                                }
                            }//foreach
                            context.SaveChanges();
                        }

                    }
                }
            }
            #endregion
        }
    }

    public class NationalUrbanData
    {
        public string value { get; set; }
        public string label { get; set; }
        public List<NationalUrbanData> children { get; set; }
    }
}
