using ApiModel.Consts;
using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Data;
using ApiServer.Models;
using BambooCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Repositories
{
    public class AccountRepository : ListableRepository<Account, AccountDTO>
    {
        public AccountRepository(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep)
            : base(context, permissionTreeRep)
        {
        }

        #region SatisfyCreateAsync 判断数据是否满足存储规范
        /// <summary>
        /// 判断数据是否满足存储规范
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="data"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public override async Task SatisfyCreateAsync(string accid, Account data, ModelStateDictionary modelState)
        {
            if (!string.IsNullOrWhiteSpace(data.Mail))
            {
                var existMail = await _DbContext.Accounts.CountAsync(x => x.Mail == data.Mail) > 0;
                if (existMail)
                    modelState.AddModelError("Mail", "该邮箱已经使用");
            }

            //if (!string.IsNullOrWhiteSpace(data.Phone))
            //{
            //    var existPhone = await _DbContext.Accounts.CountAsync(x => x.Phone == data.Phone) > 0;
            //    if (existPhone)
            //        modelState.AddModelError("Phone", "该电话已经使用");
            //}

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
        public override async Task SatisfyUpdateAsync(string accid, Account data, ModelStateDictionary modelState)
        {
            if (!string.IsNullOrWhiteSpace(data.Mail))
            {
                var existMail = await _DbContext.Accounts.CountAsync(x => x.Mail == data.Mail && x.Id != data.Id) > 0;
                if (existMail)
                    modelState.AddModelError("Mail", "该邮箱已经使用");
            }
            if (!string.IsNullOrWhiteSpace(data.Phone))
            {
                var existPhone = await _DbContext.Accounts.CountAsync(x => x.Phone == data.Phone && x.Id != data.Id) > 0;
                if (existPhone)
                    modelState.AddModelError("Phone", "该电话已经使用");
            }
        }
        #endregion

        public override async Task<bool> CanReadAsync(string accid, string id)
        {
            return await Task.FromResult(true);
        }

        #region CanCreateAsync 判断数据是否满足创建规范
        /// <summary>
        /// CanCreateAsync
        /// </summary>
        /// <param name="accid"></param>
        /// <returns></returns>
        public override async Task<bool> CanCreateAsync(string accid)
        {
            var currentAcc = await _DbContext.Accounts.FindAsync(accid);
            if (currentAcc == null)
                return false;

            if (currentAcc.Type == AppConst.AccountType_SysAdmin)
            {
                return true;
            }
            else if (currentAcc.Type == AppConst.AccountType_BrandAdmin)
            {
                return true;
            }
            else if (currentAcc.Type == AppConst.AccountType_PartnerAdmin)
            {
                return true;
            }
            else if (currentAcc.Type == AppConst.AccountType_SupplierAdmin)
            {
                return true;
            }
            else
            {

            }
            return await Task.FromResult(false);
        }
        #endregion

        #region CreateAsync 新建用户信息
        /// <summary>
        /// 新建用户信息
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public override async Task CreateAsync(string accid, Account data)
        {
            using (var tx = _DbContext.Database.BeginTransaction())
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(data.DepartmentId))
                        data.Department = await _DbContext.Departments.FindAsync(data.DepartmentId);
                    await base.CreateAsync(accid, data);
                    var otree = new PermissionTree();
                    otree.Name = data.Name;
                    //为了确保OrganizationId和Organization只赋一个导致OrganizationId被遗漏
                    if (!string.IsNullOrWhiteSpace(data.OrganizationId))
                        otree.OrganizationId = data.OrganizationId;
                    if (data.Organization != null)
                        otree.OrganizationId = data.Organization.Id;
                    otree.NodeType = AppConst.S_NodeType_Account;
                    otree.ObjId = data.Id;
                    if (!string.IsNullOrWhiteSpace(data.DepartmentId))
                    {
                        var refDepartmentNode = await _DbContext.PermissionTrees.Where(x => x.ObjId == data.DepartmentId).FirstOrDefaultAsync();
                        if (refDepartmentNode != null)
                        {
                            otree.ParentId = refDepartmentNode.Id;
                            await _PermissionTreeRepository.AddChildNode(otree);
                        }
                    }

                    tx.Commit();
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    throw ex;
                }
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
        public override async Task<IQueryable<Account>> _GetPermissionData(string accid, DataOperateEnum dataOp, bool withInActive = false)
        {
            var emptyQuery = Enumerable.Empty<Account>().AsQueryable();
            var query = emptyQuery;

            var currentAcc = await _DbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accid);
            if (currentAcc == null)
                return query;

            //数据状态
            if (withInActive)
                query = _DbContext.Set<Account>();
            else
                query = _DbContext.Set<Account>().Where(x => x.ActiveFlag == AppConst.I_DataState_Active);


            if (currentAcc.Type == AppConst.AccountType_SysAdmin)
            {
                return query;
            }
            else
            {
                //组织管理员(包括合伙人|供应商)能管理自己组织下普通用户和第一层级管理员信息
                //组织所属管理员可以管理自己组织管理员

                #region [U,D]
                if (dataOp == DataOperateEnum.Update || dataOp == DataOperateEnum.Delete)
                {
                    var ownOrganUserQ = query.Where(x => x.OrganizationId == currentAcc.OrganizationId);
                    var onLevelDownOrganIdsQ = _DbContext.Organizations.Where(x => x.ParentId == currentAcc.OrganizationId).Select(x => x.Id);
                    var oneLevelDownOrganAdminQ = query.Where(x => onLevelDownOrganIdsQ.Contains(x.OrganizationId));

                    var a1 = ownOrganUserQ.ToList();
                    var a2 = onLevelDownOrganIdsQ.ToList();
                    var a3 = oneLevelDownOrganAdminQ.ToList();
                    var a4 = ownOrganUserQ.Union(oneLevelDownOrganAdminQ).ToList();

                    return ownOrganUserQ.Union(oneLevelDownOrganAdminQ);
                }
                #endregion

                #region [R]
                if (dataOp == DataOperateEnum.Retrieve)
                {
                    var ownOrganIdsQ = (await (_PermissionTreeRepository as PermissionTreeRepository).GetOrganManageNode(currentAcc.OrganizationId, new List<string>() { AppConst.S_NodeType_Organization }, true)).Select(x => x.ObjId);
                    return query.Where(x => ownOrganIdsQ.Contains(x.OrganizationId));
                }
                #endregion

            }
            return emptyQuery;
        }
        #endregion

        #region SimplePagedQueryAsync 获取分页数据信息
        /// <summary>
        /// SimplePagedQueryAsync
        /// </summary>
        /// <param name="model"></param>
        /// <param name="accid"></param>
        /// <param name="advanceQuery"></param>
        /// <returns></returns>
        public override async Task<PagedData<Account>> SimplePagedQueryAsync(PagingRequestModel model, string accid, Func<IQueryable<Account>, Task<IQueryable<Account>>> advanceQuery = null)
        {
            var res = await base.SimplePagedQueryAsync(model, accid, advanceQuery);
            if (res.Data != null && res.Data.Count > 0)
            {
                var account = await _DbContext.Accounts.FindAsync(accid);
                var departments = await _DbContext.Departments.Where(x => x.OrganizationId == account.OrganizationId && x.ActiveFlag == AppConst.I_DataState_Active).ToListAsync();
                for (int idx = res.Data.Count - 1; idx >= 0; idx--)
                {
                    var curAccount = res.Data[idx];
                    if (!string.IsNullOrWhiteSpace(curAccount.DepartmentId))
                    {
                        curAccount.Department = departments.FirstOrDefault(x => x.Id == curAccount.DepartmentId);
                    }
                }
            }
            return res;
        }
        #endregion

        #region GetByIdAsync 根据Id返回实体DTO数据信息
        /// <summary>
        /// 根据Id返回实体DTO数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<AccountDTO> GetByIdAsync(string id)
        {
            var data = await _DbContext.Accounts.Include(x => x.AdditionRoles).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (data == null)
                return new AccountDTO();
            if (!string.IsNullOrWhiteSpace(data.OrganizationId))
            {
                var organ = await _DbContext.Organizations.FirstOrDefaultAsync(x => x.Id == data.OrganizationId);
                data.OrganizationName = organ != null ? organ.Name : string.Empty;
                if (!string.IsNullOrEmpty(organ.Icon))
                    data.OrganizationIcon = await _DbContext.Files.Where(x => x.Id == organ.Icon).Select(x => x.Url).FirstOrDefaultAsync();
            }

            if (!string.IsNullOrWhiteSpace(data.Icon))
            {
                var fs = await _DbContext.Files.FirstOrDefaultAsync(x => x.Id == data.Icon);
                if (fs != null)
                {
                    data.IconFileAssetUrl = fs.Url;
                }
            }
            if (!string.IsNullOrWhiteSpace(data.DepartmentId))
                data.Department = await _DbContext.Departments.FindAsync(data.DepartmentId);
            if (data.AdditionRoles != null && data.AdditionRoles.Count > 0)
            {
                for (var idx = data.AdditionRoles.Count - 1; idx >= 0; idx--)
                {
                    var roleItem = data.AdditionRoles[idx];
                    var refRole = await _DbContext.UserRoles.Where(x => x.Id == roleItem.UserRoleId).FirstOrDefaultAsync();
                    roleItem.UserRoleName = refRole != null ? refRole.Name : "";
                    roleItem.Account = null;
                }
            }


            return data.ToDTO();
        }
        #endregion
    }
}
