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
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace ApiServer.Repositories
{
    public class SolutionRepository : ResourceRepositoryBase<Solution, SolutionDTO>
    {
        public SolutionRepository(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep)
            : base(context, permissionTreeRep)
        {
        }

        public override ResourceTypeEnum ResourceTypeSetting
        {
            get
            {
                return ResourceTypeEnum.Organizational;
            }
        }
        public override int ResType => ResourceTypeConst.Solution;

        #region SatisfyCreateAsync 判断数据是否满足存储规范
        /// <summary>
        /// 判断数据是否满足存储规范
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="data"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public async override Task SatisfyCreateAsync(string accid, Solution data, ModelStateDictionary modelState)
        {
            if (!string.IsNullOrEmpty(data.LayoutId))
            {
                var exist = await _DbContext.Layouts.CountAsync(x => x.Id == data.LayoutId && x.ActiveFlag == AppConst.I_DataState_Active) > 0;
                if (!exist)
                    modelState.AddModelError("LayoutId", "没有找到该记录信息");
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
        public async override Task SatisfyUpdateAsync(string accid, Solution data, ModelStateDictionary modelState)
        {
            if (!string.IsNullOrEmpty(data.LayoutId))
            {
                var exist = await _DbContext.Layouts.CountAsync(x => x.Id == data.LayoutId && x.ActiveFlag == AppConst.I_DataState_Active) > 0;
                if (!exist)
                    modelState.AddModelError("LayoutId", "没有找到该记录信息");
            }
        }
        #endregion

        #region _GetPermissionData 获取权限数据
        /// <summary>
        /// 获取权限数据
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="dataOp"></param>
        /// <param name="withInActive"></param>
        /// <returns></returns>
        public async override Task<IQueryable<Solution>> _GetPermissionData(string accid, DataOperateEnum dataOp, bool withInActive = true)
        {
            IQueryable<Solution> query;

            var currentAcc = await _DbContext.Accounts.Select(x => new Account() { Id = x.Id, OrganizationId = x.OrganizationId, Type = x.Type }).FirstOrDefaultAsync(x => x.Id == accid);

            //数据状态
            if (withInActive)
                query = _DbContext.Set<Solution>();
            else
                query = _DbContext.Set<Solution>().Where(x => x.ActiveFlag == AppConst.I_DataState_Active);

            //超级管理员系列不走权限判断
            if (currentAcc.Type == AppConst.AccountType_SysAdmin || currentAcc.Type == AppConst.AccountType_SysService)
                return await Task.FromResult(query);


            if (dataOp == DataOperateEnum.Retrieve)
            {
                /*
                 * 读取操作
                 *      管理员
                 *              品牌管理员/用户:获取自己组织的创建的产品
                 *       合伙人/供应商
                 *              管理员/用户:获取自己组织有读取权限的
                 */


                if (currentAcc.Type == AppConst.AccountType_BrandAdmin || currentAcc.Type == AppConst.AccountType_BrandMember)
                {
                    return query.Where(x => x.OrganizationId == currentAcc.OrganizationId);
                }
                else
                {
                    var permissionIdQ = _DbContext.ResourcePermissions.Where(x => x.OrganizationId == currentAcc.OrganizationId && x.ResType == ResType && x.OpRetrieve == 1);

                    query = from it in query
                            join ps in permissionIdQ on it.Id equals ps.ResId
                            select it;
                    return query;
                }
            }
            else
            {
                /*
                 * 改,删操作
                 *      管理员:只获取自己组织创建的
                 *      普通用户:获取自己创建的
                 */

                if (currentAcc.Type == AppConst.AccountType_BrandAdmin || currentAcc.Type == AppConst.AccountType_PartnerAdmin || currentAcc.Type == AppConst.AccountType_SupplierAdmin)
                {
                    var permissionIdQ = _DbContext.ResourcePermissions.Where(x => x.OrganizationId == currentAcc.OrganizationId && x.ResType == ResType && x.OpRetrieve == 1);

                    query = from it in query
                            join ps in permissionIdQ on it.Id equals ps.ResId
                            select it;
                    return query;
                }
            }

            //普通用户
            return await Task.FromResult(query.Where(x => x.Creator == currentAcc.Id));
        }

        #endregion

        //public async Task<PagedData<Solution>> SampleDataQueryAsync(PagingRequestModel model, string accid, Func<IQueryable<Solution>, Task<IQueryable<Solution>>> advanceQuery = null)
        //{
        //    var currentAcc = await _DbContext.Accounts.Select(x => new Account() { Id = x.Id, OrganizationId = x.OrganizationId, Type = x.Type }).FirstOrDefaultAsync(x => x.Id == accid);
        //    var organNode = await _DbContext.PermissionTrees.FirstOrDefaultAsync(x => x.ObjId == currentAcc.OrganizationId);
        //    var organNodeQ = await _PermissionTreeRepository.GetAncestorNode(organNode, new List<string>() { AppConst.S_NodeType_Organization }, true);
        //    var organIds = organNodeQ.Select(x => x.ObjId).ToList();

        //    var query = _DbContext.Solutions.Where(x => organIds.Contains(x.OrganizationId) && x.ResourceType == (int)ResourceTypeEnum.Organizational_SubShare);
        //    if (advanceQuery != null)
        //        query = await advanceQuery(query);
        //    else
        //        query = query.Where(x => x.ActiveFlag == AppConst.I_DataState_Active);

        //    _QSearchFilter(ref query, model.Q);
        //    query = await _KeyWordSearchFilter(query, model.Search);
        //    _OrderByPipe(ref query, model.OrderBy, model.Desc);
        //    var result = await query.SimplePaging(model.Page, model.PageSize, PagedSelectExpression());

        //    #region 补充CreateName,ModifierName信息
        //    if (result.Data != null && result.Data.Count > 0)
        //    {
        //        var users = new List<Account>();
        //        //如果用left join,那需要自己定义返回实体字段信息的值
        //        //默认查询出该组织所有的人员,如果有creator,modifier不在组织里面,再进行查询,然后添加到users列表备用
        //        var currentUser = await _DbContext.Accounts.FindAsync(accid);
        //        if (string.IsNullOrWhiteSpace(currentUser.OrganizationId))
        //        {
        //            users.AddRange(_DbContext.Accounts.Where(x => x.OrganizationId == currentUser.OrganizationId).Select(x => new Account() { Id = x.Id, Name = x.Name }));
        //        }
        //        else
        //        {
        //            //如果是超级管理员,默认是没有组织的,这时候选出所有的用户,不过用户表可能比较大,这个是后期改善项
        //            users.AddRange(_DbContext.Accounts.Select(x => new Account() { Id = x.Id, Name = x.Name }));
        //        }


        //        for (int idx = result.Data.Count - 1; idx >= 0; idx--)
        //        {
        //            var item = result.Data[idx];

        //            #region 补充CreateName
        //            if (!string.IsNullOrWhiteSpace(item.Creator))
        //            {
        //                var refUser = users.FirstOrDefault(x => item.Creator == x.Id);
        //                if (refUser != null)
        //                {
        //                    item.CreatorName = refUser.Name;
        //                }
        //                else
        //                {
        //                    //去数据库查找该用户,然后添加到users列表
        //                    var us = await _DbContext.Accounts.Where(x => x.Id == item.Creator).Select(x => new Account() { Id = x.Id, Name = x.Name }).FirstOrDefaultAsync();
        //                    if (us != null)
        //                    {
        //                        item.CreatorName = us.Name;
        //                        users.Add(us);
        //                    }
        //                }
        //            }
        //            #endregion

        //            #region 补充ModifierName
        //            if (!string.IsNullOrWhiteSpace(item.Modifier))
        //            {
        //                var refUser = users.FirstOrDefault(x => item.Modifier == x.Id);
        //                if (refUser != null)
        //                {
        //                    item.ModifierName = refUser.Name;
        //                }
        //                else
        //                {
        //                    //去数据库查找该用户,然后添加到users列表
        //                    var us = await _DbContext.Accounts.Where(x => x.Id == item.Modifier).Select(x => new Account() { Id = x.Id, Name = x.Name }).FirstOrDefaultAsync();
        //                    if (us != null)
        //                    {
        //                        item.ModifierName = us.Name;
        //                        users.Add(us);
        //                    }
        //                }
        //            }
        //            #endregion
        //        }
        //    }
        //    #endregion
        //    return result;
        //}

        #region PagedSelectExpression
        /// <summary>
        /// PagedSelectExpression
        /// </summary>
        /// <returns></returns>
        public override Expression<Func<Solution, Solution>> PagedSelectExpression()
        {
            return x => new Solution()
            {
                Id = x.Id,
                Name = x.Name,
                Icon = x.Icon,
                Description = x.Description,
                CategoryId = x.CategoryId,
                OrganizationId = x.OrganizationId,
                Creator = x.Creator,
                Modifier = x.Modifier,
                CreatedTime = x.CreatedTime,
                ModifiedTime = x.ModifiedTime,
                LayoutId = x.LayoutId,
                ResourceType = x.ResourceType
            };
        }
        #endregion

    }
}
