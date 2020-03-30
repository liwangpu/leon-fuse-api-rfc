using ApiModel;
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

    public class RepositoryBase<T, DTO>
       where T : class, IEntity, IDTOTransfer<DTO>, new()
       where DTO : class, IData, new()
    {
        public ApiDbContext _DbContext { get; }
        public ITreeRepository<PermissionTree> _PermissionTreeRepository { get; }

        public RepositoryBase(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep)
        {
            _DbContext = context;
            _PermissionTreeRepository = permissionTreeRep;
        }


        /// <summary>
        /// 资源访问类型
        /// </summary>
        public virtual ResourceTypeEnum ResourceTypeSetting
        {
            get
            {
                return ResourceTypeEnum.Personal;
            }
        }

        /**************** protected method ****************/

        #region _QSearchFilter 查询参数过滤
        /// <summary>
        /// 查询参数过滤
        /// </summary>
        /// <param name="query"></param>
        /// <param name="q"></param>
        protected void _QSearchFilter(ref IQueryable<T> query, string q)
        {
            if (!string.IsNullOrWhiteSpace(q))
            {
                var wheres = QueryParser.Parse<T>(q);
                foreach (var item in wheres)
                    query = query.Where(item);
            }
        }
        #endregion

        #region _KeyWordSearchFilter 基本关键字过滤
        /// <summary>
        /// 基本关键字过滤
        /// </summary>
        /// <param name="query"></param>
        /// <param name="search"></param>
        protected async Task<IQueryable<T>> _KeyWordSearchFilter(IQueryable<T> query, string search)
        {
            if (!string.IsNullOrWhiteSpace(search))
            {
                //var accids = await _DbContext.Accounts.Where(x => x.Name.ToLower().Contains(search.ToLower()) ).Select(x => x.Id).ToListAsync();
                //if (accids.Count > 0)
                //    query = query.Where(d => d.Name.Contains(search) || accids.Contains(d.Creator));
                //else
                    query = query.Where(d => d.Name.ToLower().Contains(search.ToLower()));
            }
            return query;
        }
        #endregion

        #region _OrderByPipe 基本排序过滤管道
        /// <summary> 
        /// 基本排序过滤管道
        /// </summary>
        /// <param name="query"></param>
        /// <param name="orderBy"></param>
        /// <param name="desc"></param>
        protected void _OrderByPipe(ref IQueryable<T> query, string orderBy, bool desc)
        {
            //默认以修改时间作为排序
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                orderBy = "ModifiedTime";
                desc = true;
            }

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                /*
                 * api提供的字段信息是大小写不敏感,或者说经过转换大小写了
                 * client在排序的时候并不知道真实属性的名字,server需要经过反射获取原来属性的名字信息
                 * 确保在排序的时候不会出现异常
                 */
                var realProperty = string.Empty;
                var properties = typeof(T).GetProperties();
                for (int idx = properties.Length - 1; idx >= 0; idx--)
                {
                    var propName = properties[idx].Name.ToString();
                    if (propName.ToLower() == orderBy.ToLower())
                    {
                        realProperty = propName;
                        break;
                    }
                }

                if (!string.IsNullOrWhiteSpace(realProperty))
                {
                    if (desc)
                        query = query.OrderByDescendingBy(realProperty);
                    else
                        query = query.OrderBy(realProperty);
                }
            }
        }
        #endregion

        /**************** public method ****************/

        #region GetUserRootOrgan 获取用户的根组织
        /// <summary>
        /// 获取用户的根组织
        /// </summary>
        /// <param name="accid"></param>
        /// <returns></returns>
        public async Task<Organization> GetUserRootOrgan(string accid)
        {
            var account = await _DbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accid);
            if (account.OrganizationId == AppConst.BambooOrganId)
                return await _DbContext.Organizations.FirstOrDefaultAsync(x => x.Id == AppConst.BambooOrganId);
            if (account != null)
            {
                var rootNode = await _DbContext.PermissionTrees.FirstAsync(x => x.ObjId == account.OrganizationId);
                var organ = await _DbContext.Organizations.FirstOrDefaultAsync(x => x.Id == rootNode.RootOrganizationId);
                return organ;
            }
            return null;
        }
        #endregion

        #region _GetByIdAsync 根据id信息返回实体数据信息
        /// <summary>
        /// 根据id信息返回实体数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> _GetByIdAsync(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var data = await _DbContext.Set<T>().FindAsync(id);
                if (data != null)
                    return data;
            }
            return new T();
        }
        #endregion

        #region virtual _GetPermissionData 获取用户权限数据
        /// <summary>
        /// 获取用户权限数据
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="dataOp"></param>
        /// <param name="withInActive"></param>
        /// <returns></returns>
        public virtual async Task<IQueryable<T>> _GetPermissionData(string accid, DataOperateEnum dataOp, bool withInActive = false)
        {
            var emptyQuery = Enumerable.Empty<T>().AsQueryable();
            var query = emptyQuery;

            var currentAcc = await _DbContext.Accounts.Select(x => new Account() { Id = x.Id, OrganizationId = x.OrganizationId, Type = x.Type }).FirstOrDefaultAsync(x => x.Id == accid);
            if (currentAcc == null)
                return query;

            //数据状态
            if (withInActive)
                query = _DbContext.Set<T>();
            else
                query = _DbContext.Set<T>().Where(x => x.ActiveFlag == AppConst.I_DataState_Active);

            /*
             * 读取,修改,删除操作
             *      管理员:操作自己组织的创建的
             *      用户:操作自己创建的数据
             */


            if (currentAcc.Type == AppConst.AccountType_BrandAdmin || currentAcc.Type == AppConst.AccountType_PartnerAdmin || currentAcc.Type == AppConst.AccountType_SupplierAdmin)
            {
                return query.Where(x => x.OrganizationId == currentAcc.OrganizationId);
            }


            return query.Where(x => x.Creator == accid);

            #region 待移除
            //#region func getPersonalResource 获取个人资源
            //var getPersonalResource = new Func<IQueryable<T>, IQueryable<T>>((q) =>
            //{
            //    return q.Where(x => x.Creator == currentAcc.Id);
            //});
            //#endregion
            //#region func getCurrentOrganResource 获取当前组织资源
            //var getCurrentOrganResource = new Func<IQueryable<T>, IQueryable<T>>((q) =>
            //{
            //    return q.Where(x => x.OrganizationId == currentAcc.OrganizationId);
            //});
            //#endregion
            //#region func getSupResource 获取上级组织资源
            //var getSupResource = new Func<IQueryable<T>, IQueryable<T>>((q) =>
            //{
            //    var treeQ = _PermissionTreeRepository.GetAncestorNode(organNode, new List<string>() { AppConst.S_NodeType_Organization });
            //    return q.Where(x => x.Creator == currentAcc.Id);
            //});
            //#endregion
            //#region func getSubOrganResource 获取下级组织资源
            //var getSubOrganResource = new Func<IQueryable<T>, IQueryable<T>>((q) =>
            //{
            //    var treeQ = _PermissionTreeRepository.GetDescendantNode(organNode, new List<string>() { AppConst.S_NodeType_Organization });
            //    return from it in query
            //           join tq in treeQ on it.OrganizationId equals tq.ObjId
            //           select it;
            //});
            //#endregion


            //#region [SysAdmin]
            //if (currentAcc.Type == AppConst.AccountType_SysAdmin)
            //{
            //    return query;
            //}
            //#endregion
            //#region [BrandAdmin]
            //else if (currentAcc.Type == AppConst.AccountType_BrandAdmin)
            //{
            //    var treeQ = _PermissionTreeRepository.GetDescendantNode(organNode, new List<string>() { AppConst.S_NodeType_Organization }, true);

            //    return from it in query
            //           join tq in treeQ on it.OrganizationId equals tq.ObjId
            //           select it;
            //}
            //#endregion
            //#region [BrandMember]
            //else if (currentAcc.Type == AppConst.AccountType_BrandMember)
            //{
            //    if (ResourceTypeSetting == ResourceTypeEnum.Organizational)
            //    {
            //        if (dataOp == DataOperateEnum.Retrieve)
            //            return getCurrentOrganResource(query);
            //    }

            //    if (ResourceTypeSetting == ResourceTypeEnum.Organizational_SubShare)
            //    {
            //        if (dataOp == DataOperateEnum.Retrieve)
            //        {
            //            var currentOrganQ = getCurrentOrganResource(query);
            //            var supOrganQ = getSupResource(query);
            //            return currentOrganQ.Union(supOrganQ);
            //        }
            //    }
            //}
            //#endregion
            //#region [PartnerAdmin]
            //else if (currentAcc.Type == AppConst.AccountType_PartnerAdmin)
            //{
            //    //var treeQ =await _PermissionTreeRepository.GetAncestorNode(organNode, new List<string>() { AppConst.S_NodeType_Organization }, true);
            //    //return from it in query
            //    //       join tq in treeQ on it.OrganizationId equals tq.ObjId
            //    //       select it;

            //    if (ResourceTypeSetting == ResourceTypeEnum.Organizational)
            //    {
            //        return getCurrentOrganResource(query);
            //    }

            //    if (ResourceTypeSetting == ResourceTypeEnum.Organizational_SubShare)
            //    {
            //        if (dataOp == DataOperateEnum.Retrieve)
            //        {
            //            var treeQ = await _PermissionTreeRepository.GetAncestorNode(organNode, new List<string>() { AppConst.S_NodeType_Organization }, true);
            //            return from it in query
            //                   join tq in treeQ on it.OrganizationId equals tq.ObjId
            //                   select it;
            //        }
            //    }
            //}
            //#endregion
            //#region [PartnerMember]
            //else if (currentAcc.Type == AppConst.AccountType_PartnerMember)
            //{
            //    if (ResourceTypeSetting == ResourceTypeEnum.Organizational)
            //    {
            //        if (dataOp == DataOperateEnum.Retrieve)
            //        {
            //            return getCurrentOrganResource(query);
            //        }
            //    }

            //    if (ResourceTypeSetting == ResourceTypeEnum.Organizational_SubShare)
            //    {
            //        if (dataOp == DataOperateEnum.Retrieve)
            //        {
            //            var treeQ = await _PermissionTreeRepository.GetAncestorNode(organNode, new List<string>() { AppConst.S_NodeType_Organization }, true);
            //            return from it in query
            //                   join tq in treeQ on it.OrganizationId equals tq.ObjId
            //                   select it;
            //        }
            //    }
            //}
            //#endregion
            //#region [SupplierAdmin]
            //else if (currentAcc.Type == AppConst.AccountType_SupplierAdmin)
            //{
            //    var treeQ = _PermissionTreeRepository.GetDescendantNode(organNode, new List<string>() { AppConst.S_NodeType_Organization }, true);
            //    return from it in query
            //           join tq in treeQ on it.OrganizationId equals tq.ObjId
            //           select it;
            //}
            //#endregion
            //#region [SupplierMember]
            //else if (currentAcc.Type == AppConst.AccountType_SupplierMember)
            //{
            //    if (ResourceTypeSetting == ResourceTypeEnum.Organizational)
            //    {
            //        if (dataOp == DataOperateEnum.Retrieve)
            //            return getCurrentOrganResource(query);
            //    }

            //    if (ResourceTypeSetting == ResourceTypeEnum.Organizational_SubShare)
            //    {
            //        if (dataOp == DataOperateEnum.Retrieve)
            //        {
            //            var currentOrganQ = getCurrentOrganResource(query);
            //            var supOrganQ = getSupResource(query);
            //            return currentOrganQ.Union(supOrganQ);
            //        }
            //    }
            //}
            //#endregion
            //#region [Default]
            //else
            //{
            //    return getPersonalResource(query);
            //}
            //#endregion

            //return getPersonalResource(query); 
            #endregion
        }
        #endregion

        #region virtual GetByIdAsync 根据id信息返回实体DTO数据信息
        /// <summary>
        /// 根据id信息返回实体DTO数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<DTO> GetByIdAsync(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var data = await _DbContext.Set<T>().FindAsync(id);
                if (data != null)
                    return data.ToDTO();
            }
            return new DTO();
        }
        #endregion

        #region virtual ExistAsync 判断id对应记录是否存在
        /// <summary>
        /// 判断id对应记录是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="withInActive"></param>
        /// <returns></returns>
        public virtual async Task<bool> ExistAsync(string id, bool withInActive = false)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (withInActive)
                    return await _DbContext.Set<T>().CountAsync(x => x.Id == id) > 0;
                return await _DbContext.Set<T>().CountAsync(x => x.Id == id && x.ActiveFlag == AppConst.I_DataState_Active) > 0;
            }
            return false;
        }
        #endregion

        #region SatisfyCreateAsync 判断数据是否满足创建条件
        /// <summary>
        /// 判断数据是否满足创建
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="data"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public virtual async Task SatisfyCreateAsync(string accid, T data, ModelStateDictionary modelState)
        {
            await Task.FromResult(string.Empty);
        }
        #endregion

        #region SatisfyUpdateAsync 判断数据是否满足修改条件
        /// <summary>
        /// 判断数据是否满足修改条件
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="data"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public virtual async Task SatisfyUpdateAsync(string accid, T data, ModelStateDictionary modelState)
        {
            await Task.FromResult(string.Empty);
        }
        #endregion

        #region SatisfyDeleteAsync 判断数据是否满足删除条件
        /// <summary>
        /// 判断数据是否满足删除条件
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="data"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public virtual async Task SatisfyDeleteAsync(string accid, T data, ModelStateDictionary modelState)
        {
            await Task.FromResult(string.Empty);
        }
        #endregion

        #region virtual CanCreateAsync 判断用户是否有权限创建数据
        /// <summary>
        /// 判断用户是否有权限创建数据
        /// </summary>
        /// <param name="accid"></param>
        /// <returns></returns>
        public virtual async Task<bool> CanCreateAsync(string accid)
        {
            //var currentAcc = await _DbContext.Accounts.FindAsync(accid);
            //if (currentAcc == null)
            //    return false;

            //if (currentAcc.Type == AppConst.AccountType_SysAdmin)
            //{
            //    return true;
            //}
            //else if (currentAcc.Type == AppConst.AccountType_BrandAdmin)
            //{
            //    if (ResourceTypeSetting == ResourceTypeEnum.Organizational)
            //        return true;
            //}
            //else
            //{
            //    if (ResourceTypeSetting == ResourceTypeEnum.Personal || ResourceTypeSetting == ResourceTypeEnum.Organizational_DownView_UpView_OwnEdit)
            //        return true;
            //}
            return await Task.FromResult(true);
        }
        #endregion

        #region virtual CanUpdateAsync 判断用户是否有权限更新数据
        /// <summary>
        /// 判断用户是否有权限更新数据
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<bool> CanUpdateAsync(string accid, string id)
        {
            var query = await _GetPermissionData(accid, DataOperateEnum.Update, true);

            return await query.Where(x => x.Id == id).CountAsync() > 0;
        }
        #endregion

        #region virtual CanDeleteAsync 判断用户是否有权限删除数据
        /// <summary>
        /// 判断用户是否有权限删除数据
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<bool> CanDeleteAsync(string accid, string id)
        {
            var query = await _GetPermissionData(accid, DataOperateEnum.Delete, true);
            return await query.Where(x => x.Id == id).CountAsync() > 0;
        }
        #endregion

        #region virtual CanReadAsync 判断用户是否有权限读取数据
        /// <summary>
        /// 判断用户是否有权限读取数据
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<bool> CanReadAsync(string accid, string id)
        {
            var query = await _GetPermissionData(accid, DataOperateEnum.Retrieve, true);
            return await query.Where(x => x.Id == id).CountAsync() > 0;
        }
        #endregion

        #region virtual CreateAsync 新建实体信息
        /// <summary>
        /// 新建实体信息
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual async Task CreateAsync(string accid, T data)
        {
            var currentAcc = await _DbContext.Accounts.FindAsync(accid);
            data.Id = GuidGen.NewGUID();
            //如果创建人和修改人有指定,说明有自定义需要,应该按传入的参数处理
            if (string.IsNullOrWhiteSpace(data.Creator))
                data.Creator = accid;
            if (string.IsNullOrWhiteSpace(data.Modifier))
                data.Modifier = accid;
            if (!string.IsNullOrWhiteSpace(currentAcc.OrganizationId))
                data.OrganizationId = currentAcc.OrganizationId;
            data.CreatedTime = DateTime.UtcNow;
            data.ModifiedTime = DateTime.UtcNow;
            _DbContext.Set<T>().Add(data);
            await _DbContext.SaveChangesAsync();
        }
        #endregion

        #region virtual UpdateAsync 更新实体信息
        /// <summary>
        /// 更新实体信息
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(string accid, T data)
        {
            //var currentAcc = await _DbContext.Accounts.FindAsync(accid);
            //如果修改人有指定,说明有自定义需要,应该按传入的参数处理
            if (string.IsNullOrWhiteSpace(data.Modifier))
                data.Modifier = accid;
            data.ModifiedTime = DateTime.Now;
            //data.OrganizationId = currentAcc.OrganizationId;
            _DbContext.Set<T>().Update(data);
            await _DbContext.SaveChangesAsync();
        }
        #endregion

        #region virtual DeleteAsync 删除实体信息
        /// <summary>
        /// 删除材质信息
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(string accid, string id)
        {
            var data = await _GetByIdAsync(id);
            if (data != null)
            {
                data.Modifier = accid;
                data.ModifiedTime = DateTime.Now;
                data.ActiveFlag = AppConst.I_DataState_InActive;
                _DbContext.Set<T>().Update(data);
                await _DbContext.SaveChangesAsync();
            }
        }
        #endregion

        #region virtual SimplePagedQueryAsync 分页查询
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="model"></param>
        /// <param name="accid"></param>
        /// <param name="advanceQuery">注意,一旦有高级过滤条件,默认非活动数据也显示,请自行对ActiveFlag做过滤</param>
        /// <returns></returns>
        public virtual async Task<PagedData<T>> SimplePagedQueryAsync(PagingRequestModel model, string accid, Func<IQueryable<T>, Task<IQueryable<T>>> advanceQuery = null)
        {
            //读取设置也取非活动状态数据,后面在advanceQuery再修改是否真的需要
            var query = await _GetPermissionData(accid, DataOperateEnum.Retrieve, true);
            if (advanceQuery != null)
                query = await advanceQuery(query);
            else
                query = query.Where(x => x.ActiveFlag == AppConst.I_DataState_Active);

            _QSearchFilter(ref query, model.Q);
            query = await _KeyWordSearchFilter(query, model.Search);
            _OrderByPipe(ref query, model.OrderBy, model.Desc);
            var result = await query.SimplePaging(model.Page, model.PageSize, PagedSelectExpression());

            #region 补充CreateName,ModifierName信息
            if (result.Data != null && result.Data.Count > 0)
            {
                var users = new List<Account>();
                //如果用left join,那需要自己定义返回实体字段信息的值
                //默认查询出该组织所有的人员,如果有creator,modifier不在组织里面,再进行查询,然后添加到users列表备用
                var currentUser = await _DbContext.Accounts.FindAsync(accid);
                if (string.IsNullOrWhiteSpace(currentUser.OrganizationId))
                {
                    users.AddRange(_DbContext.Accounts.Where(x => x.OrganizationId == currentUser.OrganizationId).Select(x => new Account() { Id = x.Id, Name = x.Name }));
                }
                else
                {
                    //如果是超级管理员,默认是没有组织的,这时候选出所有的用户,不过用户表可能比较大,这个是后期改善项
                    users.AddRange(_DbContext.Accounts.Select(x => new Account() { Id = x.Id, Name = x.Name }));
                }


                for (int idx = result.Data.Count - 1; idx >= 0; idx--)
                {
                    var item = result.Data[idx];

                    #region 补充CreateName
                    if (!string.IsNullOrWhiteSpace(item.Creator))
                    {
                        var refUser = users.FirstOrDefault(x => item.Creator == x.Id);
                        if (refUser != null)
                        {
                            item.CreatorName = refUser.Name;
                        }
                        else
                        {
                            //去数据库查找该用户,然后添加到users列表
                            var us = await _DbContext.Accounts.Where(x => x.Id == item.Creator).Select(x => new Account() { Id = x.Id, Name = x.Name }).FirstOrDefaultAsync();
                            if (us != null)
                            {
                                item.CreatorName = us.Name;
                                users.Add(us);
                            }
                        }
                    }
                    #endregion

                    #region 补充ModifierName
                    if (!string.IsNullOrWhiteSpace(item.Modifier))
                    {
                        var refUser = users.FirstOrDefault(x => item.Modifier == x.Id);
                        if (refUser != null)
                        {
                            item.ModifierName = refUser.Name;
                        }
                        else
                        {
                            //去数据库查找该用户,然后添加到users列表
                            var us = await _DbContext.Accounts.Where(x => x.Id == item.Modifier).Select(x => new Account() { Id = x.Id, Name = x.Name }).FirstOrDefaultAsync();
                            if (us != null)
                            {
                                item.ModifierName = us.Name;
                                users.Add(us);
                            }
                        }
                    }
                    #endregion
                }
            }
            #endregion
            return result;
        }
        #endregion

        #region PagedSelectExpression 匹配分页返回字段信息
        /// <summary>
        /// 匹配分页返回字段信息
        /// </summary>
        /// <returns></returns>
        public virtual Expression<Func<T, T>> PagedSelectExpression()
        {
            return x => x;
        }
        #endregion

        /**************** public static method ****************/

        #region static PageQueryDTOTransfer 将分页查询PagedData转为PagedData DTO
        /// <summary>
        /// 将分页查询PagedData转为PagedData DTO
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static PagedData<DTO> PageQueryDTOTransfer(PagedData<T> result)
        {
            if (result != null)
            {
                if (result.Total > 0)
                {
                    return new PagedData<DTO>() { Data = result.Data.Select(x => x.ToDTO()).ToList(), Page = result.Page, Size = result.Size, Total = result.Total };
                }
            }
            return new PagedData<DTO>();
        }
        #endregion



    }
}
