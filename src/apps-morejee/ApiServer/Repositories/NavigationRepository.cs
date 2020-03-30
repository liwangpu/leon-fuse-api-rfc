using ApiModel.Consts;
using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Repositories
{
    public class NavigationRepository : RepositoryBase<Navigation, NavigationDTO>, IRepository<Navigation, NavigationDTO>
    {
        public NavigationRepository(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep) : base(context, permissionTreeRep)
        {
        }

        public override async Task<IQueryable<Navigation>> _GetPermissionData(string accid, DataOperateEnum dataOp, bool withInActive = false)
        {
            var emptyQuery = Enumerable.Empty<Navigation>().AsQueryable();
            var query = emptyQuery;


            //数据状态
            if (withInActive)
                query = _DbContext.Set<Navigation>();
            else
                query = _DbContext.Set<Navigation>().Where(x => x.ActiveFlag == AppConst.I_DataState_Active);

            return await Task.FromResult(query);
        }
    }
}
