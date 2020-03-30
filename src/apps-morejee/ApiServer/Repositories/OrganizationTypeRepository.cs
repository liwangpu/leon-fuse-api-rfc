using ApiModel.Consts;
using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Repositories
{
    public class OrganizationTypeRepository : RepositoryBase<OrganizationType, OrganizationTypeDTO>, IRepository<OrganizationType, OrganizationTypeDTO>
    {
        #region 构造函数
        public OrganizationTypeRepository(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep)
        : base(context, permissionTreeRep)
        {
        }
        #endregion

        public override async Task SatisfyCreateAsync(string accid, OrganizationType data, ModelStateDictionary modelState)
        {
            if (!string.IsNullOrWhiteSpace(data.Name))
            {
                var exist = await _DbContext.OrganizationTypes.CountAsync(x => x.Name == data.Name && data.ActiveFlag == 1 && x.Id != data.Id) > 0;
                if (exist)
                    modelState.AddModelError("Name", "该组织类型名称已经使用");
            }
        }

        public override async Task SatisfyUpdateAsync(string accid, OrganizationType data, ModelStateDictionary modelState)
        {
            if (data.IsInner)
            {
                modelState.AddModelError("IsInner", "不能修改内置量信息");
            }

            if (!string.IsNullOrWhiteSpace(data.Name))
            {
                var exist = await _DbContext.OrganizationTypes.CountAsync(x => x.Name == data.Name && data.ActiveFlag == 1 && x.Id != data.Id) > 0;
                if (exist)
                    modelState.AddModelError("Name", "该组织类型名称已经使用");
            }

        }

        public override async Task SatisfyDeleteAsync(string accid, OrganizationType data, ModelStateDictionary modelState)
        {
            if (data.IsInner)
            {
                modelState.AddModelError("IsInner", "不能删除内置量信息");
                await Task.FromResult(string.Empty);
            }
        }

        public override async Task<IQueryable<OrganizationType>> _GetPermissionData(string accid, DataOperateEnum dataOp, bool withInActive = false)
        {
            var query = Enumerable.Empty<OrganizationType>().AsQueryable();

            var currentAcc = await _DbContext.Accounts.Select(x => new Account() { Id = x.Id, OrganizationId = x.OrganizationId, Type = x.Type }).FirstOrDefaultAsync(x => x.Id == accid);
            if (currentAcc == null)
                return query;

            //数据状态
            if (withInActive)
                query = _DbContext.Set<OrganizationType>();
            else
                query = _DbContext.Set<OrganizationType>().Where(x => x.ActiveFlag == AppConst.I_DataState_Active);



            if (dataOp != DataOperateEnum.Retrieve)
            {
                //超级管理员只管理内置角色
                if (currentAcc.Type == AppConst.AccountType_SysAdmin || currentAcc.Type == AppConst.AccountType_SysService)
                {
                    return query;
                }
                else
                {
                    return query.Where(x => x.Creator == currentAcc.Id);
                }
            }

            return await Task.FromResult(query);
        }
    }
}
