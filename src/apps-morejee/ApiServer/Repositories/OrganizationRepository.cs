using ApiModel;
using ApiModel.Consts;
using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Data;
using ApiServer.Models;
using ApiServer.Services;
using BambooCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Repositories
{
    public class OrganizationRepository : ListableRepository<Organization, OrganizationDTO>
    {
        public IRepository<Department, DepartmentDTO> _DepartmentRep { get; }
        public IRepository<Account, AccountDTO> _AccountRep { get; }
        public OrganizationRepository(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep, IRepository<Department, DepartmentDTO> departmentRep, IRepository<Account, AccountDTO> accountRep)
         : base(context, permissionTreeRep)
        {
            _DepartmentRep = departmentRep;
            _AccountRep = accountRep;
        }

        #region SatisfyCreateAsync 判断数据是否满足存储规范
        /// <summary>
        /// 判断数据是否满足存储规范
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="data"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public override async Task SatisfyCreateAsync(string accid, Organization data, ModelStateDictionary modelState)
        {
            var currentAcc = await _DbContext.Accounts.FindAsync(accid);
            if (!string.IsNullOrWhiteSpace(data.Mail))
            {
                var organMailExist = await _DbContext.Organizations.CountAsync(x => x.Mail == data.Mail) > 0;
                if (organMailExist)
                    modelState.AddModelError("Mail", "该邮箱已经使用");
            }

            if (!string.IsNullOrWhiteSpace(data.OrganizationTypeId))
            {
                if (currentAcc.Type == AppConst.AccountType_SysAdmin)
                {
                    if (data.OrganizationTypeId != AppConst.OrganType_Brand)
                        modelState.AddModelError("Type", "您没有权限创建该组织类型");
                }
                else if (currentAcc.Type == AppConst.AccountType_BrandAdmin)
                {
                    if (!(data.OrganizationTypeId == AppConst.OrganType_Partner || data.OrganizationTypeId == AppConst.OrganType_Supplier))
                        modelState.AddModelError("Type", "您没有权限创建该组织类型");
                }
                else
                {
                    modelState.AddModelError("Type", "您没有权限创建该组织类型");
                }

            }
            else
            {
                modelState.AddModelError("Type", "组织类型不能为空");
            }

        }
        #endregion

        #region SatisfyUpdateAsync 判断数据是否满足更新规范
        /// <summary>
        /// 判断数据是否满足更新规范
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="data"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public override async Task SatisfyUpdateAsync(string accid, Organization data, ModelStateDictionary modelState)
        {
            if (!string.IsNullOrWhiteSpace(data.Mail))
            {
                var organMailExist = await _DbContext.Organizations.CountAsync(x => x.Mail == data.Mail && x.Id != data.Id) > 0;
                if (organMailExist)
                    modelState.AddModelError("Mail", "该邮箱已经使用");
            }
        }
        #endregion

        #region CanCreateAsync 判断用户是否可以创建数据
        /// <summary>
        /// 判断用户是否可以创建数据
        /// </summary>
        /// <param name="accid"></param>
        /// <returns></returns>
        public override async Task<bool> CanCreateAsync(string accid)
        {
            var currentAcc = await _DbContext.Accounts.FindAsync(accid);
            if (currentAcc == null)
                return false;

            if (currentAcc.Type == AppConst.AccountType_SysAdmin || currentAcc.Type == AppConst.AccountType_BrandAdmin)
                return true;

            return false;
        }
        #endregion

        #region CreateAsync 新建组织信息
        /// <summary>
        /// 新建组织信息
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public override async Task CreateAsync(string accid, Organization data)
        {
            var currentAcc = await _DbContext.Accounts.FindAsync(accid);
            var bCreateDefaultResource = false;
            using (var tx = _DbContext.Database.BeginTransaction())
            {
                try
                {
                    if (data.OrganizationTypeId == AppConst.OrganType_Partner || data.OrganizationTypeId == AppConst.OrganType_Supplier)
                    {
                        data.ParentId = currentAcc.OrganizationId;
                    }


                    await base.CreateAsync(accid, data);
                    var otree = new PermissionTree();
                    otree.Id = GuidGen.NewGUID();
                    otree.Name = data.Name;
                    otree.OrganizationId = data.Id;
                    otree.NodeType = AppConst.S_NodeType_Organization;
                    otree.ObjId = data.Id;
                    if (string.IsNullOrWhiteSpace(data.ParentId))
                    {
                        await _PermissionTreeRepository.AddNewNode(otree);
                        bCreateDefaultResource = true;
                    }
                    else
                    {
                        var parentOrganNode = _DbContext.PermissionTrees.Where(x => x.ObjId == data.ParentId).FirstOrDefault();
                        if (parentOrganNode != null)
                        {
                            otree.ParentId = parentOrganNode.Id;
                            otree.OrganizationId = data.ParentId;
                            await _PermissionTreeRepository.AddChildNode(otree);
                            bCreateDefaultResource = true;
                        }
                    }
                    tx.Commit();
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    throw ex;//抛出异常让middleware捕获
                }
            }

            if (bCreateDefaultResource)
            {
                #region 创建默认部门
                //var department = new Department();
                //department.OrganizationId = data.Id;
                //department.Organization = data;
                //department.Name = data.Name;
                //await _DepartmentRep.CreateAsync(accid, department);
                #endregion

                #region 创建默认管理员
                var account = new Account();
                if (data.OrganizationTypeId == AppConst.OrganType_Brand)
                    account.Type = AppConst.AccountType_BrandAdmin;
                else if (data.OrganizationTypeId == AppConst.OrganType_Partner)
                    account.Type = AppConst.AccountType_PartnerAdmin;
                else if (data.OrganizationTypeId == AppConst.OrganType_Supplier)
                    account.Type = AppConst.AccountType_SupplierAdmin;
                else
                { }
                account.Name = "管理员";
                account.Mail = GuidGen.NewGUID();
                account.Password = ConstVar.DefaultNormalPasswordMd5;
                account.Location = data.Location;

                account.ExpireTime = DateTime.Now.AddYears(10);
                account.ActivationTime = DateTime.UtcNow;
                //account.Department = department;
                //account.DepartmentId = department.Id;
                account.Organization = data;
                account.OrganizationId = data.Id;
                await _AccountRep.CreateAsync(accid, account);
                //将创建人和修改人改为该默认管理员
                account.Creator = account.Id;
                account.Modifier = account.Id;
                _DbContext.Accounts.Update(account);
                data.OwnerId = account.Id;
                _DbContext.Organizations.Update(data);
                await _DbContext.SaveChangesAsync();
                #endregion
            }
        }
        #endregion

        #region _GetPermissionData 获取操作权限数据
        /// <summary>
        /// 获取操作权限数据
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="dataOp"></param>
        /// <param name="withInActive"></param>
        /// <returns></returns>
        public override async Task<IQueryable<Organization>> _GetPermissionData(string accid, DataOperateEnum dataOp, bool withInActive = false)
        {
            var emptyQuery = Enumerable.Empty<Organization>().AsQueryable();
            var query = emptyQuery;

            var currentAcc = await _DbContext.Accounts.FirstAsync(x => x.Id == accid);
            if (currentAcc == null)
                return query;

            //数据状态
            if (withInActive)
                query = _DbContext.Organizations;
            else
                query = _DbContext.Organizations.Where(x => x.ActiveFlag == AppConst.I_DataState_Active);

            if (currentAcc.Type == AppConst.AccountType_SysAdmin)
            {
                //管理员不管理第一子层级以外的组织,自治权交给相关组织
                #region [U,D]
                if (dataOp == DataOperateEnum.Update || dataOp == DataOperateEnum.Delete)
                {
                    return query.Where(x => string.IsNullOrEmpty(x.ParentId));
                }
                #endregion

                #region [R]
                if (dataOp == DataOperateEnum.Retrieve)
                {
                    return query;
                }
                #endregion
            }
            else
            {
                //品牌商,合伙人等组织管理员不能管理自身,但是可以管理第一子层级的组织,自治权还是交给相关组织
                #region [U,D]
                if (dataOp == DataOperateEnum.Update || dataOp == DataOperateEnum.Delete)
                {
                    return query.Where(x => x.ParentId == currentAcc.OrganizationId);
                }
                #endregion

                #region [R]
                if (dataOp == DataOperateEnum.Retrieve)
                {
                    var ownOrganIdsQ = (await (_PermissionTreeRepository as PermissionTreeRepository).GetOrganManageNode(currentAcc.OrganizationId, new List<string>() { AppConst.S_NodeType_Organization }, false)).Select(x => x.ObjId);
                    return query.Where(x => ownOrganIdsQ.Contains(x.Id));
                }
                #endregion
            }

            return emptyQuery;
        }
        #endregion

        #region override SimplePagedQueryAsync
        /// <summary>
        /// SimplePagedQueryAsync
        /// </summary>
        /// <param name="model"></param>
        /// <param name="accid"></param>
        /// <param name="advanceQuery"></param>
        /// <returns></returns>
        public override async Task<PagedData<Organization>> SimplePagedQueryAsync(PagingRequestModel model, string accid, Func<IQueryable<Organization>, Task<IQueryable<Organization>>> advanceQuery = null)
        {
            var result = await base.SimplePagedQueryAsync(model, accid, advanceQuery);

            if (result.Total > 0)
            {
                for (int idx = result.Data.Count - 1; idx >= 0; idx--)
                {
                    var curData = result.Data[idx];
                    //if (!string.IsNullOrWhiteSpace(curData.Icon))
                    //    curData.IconFileAsset = await _DbContext.Files.FindAsync(curData.Icon);
                    if (!string.IsNullOrWhiteSpace(curData.Icon))
                    {
                        var fs = await _DbContext.Files.FirstOrDefaultAsync(x => x.Id == curData.Icon);
                        if (fs != null)
                        {
                            curData.IconFileAssetUrl = fs.Url;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(curData.OrganizationTypeId))
                    {
                        var organType = await _DbContext.OrganizationTypes.Where(x => x.ActiveFlag == AppConst.I_DataState_Active && x.Id == curData.OrganizationTypeId).FirstOrDefaultAsync();
                        if (organType != null)
                            curData.OrganizationTypeName = organType.Name;
                    }
                }
            }
            return result;
        }
        #endregion

        #region GetByIdAsync 根据Id返回实体DTO数据信息
        /// <summary>
        /// 根据Id返回实体DTO数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<OrganizationDTO> GetByIdAsync(string id)
        {
            var data = await _GetByIdAsync(id);
            if (!string.IsNullOrWhiteSpace(data.Icon))
            {
                var fs = await _DbContext.Files.FirstOrDefaultAsync(x => x.Id == data.Icon);
                if (fs != null)
                {
                    data.IconFileAssetUrl = fs.Url;
                }
            }
            if (!string.IsNullOrWhiteSpace(data.OrganizationTypeId))
            {
                var organType = await _DbContext.OrganizationTypes.Where(x => x.ActiveFlag == AppConst.I_DataState_Active && x.Id == data.OrganizationTypeId).FirstOrDefaultAsync();
                if (organType != null)
                    data.OrganizationTypeName = organType.Name;
            }
            return data.ToDTO();
        }
        #endregion

        #region GetOrganOwner 根据组织Id获取组织管理员信息
        /// <summary>
        /// 根据组织Id获取组织管理员信息
        /// </summary>
        /// <param name="organId"></param>
        /// <returns></returns>
        public async Task<IData> GetOrganOwner(string organId)
        {
            var organ = await _DbContext.Organizations.FirstOrDefaultAsync(x => x.Id == organId);
            if (organ != null && !string.IsNullOrWhiteSpace(organ.OwnerId))
            {
                var owner = await _DbContext.Accounts.FindAsync(organ.OwnerId);
                if (owner != null)
                    return owner.ToDTO();
            }
            return new AccountDTO();
        }
        #endregion
    }
}
