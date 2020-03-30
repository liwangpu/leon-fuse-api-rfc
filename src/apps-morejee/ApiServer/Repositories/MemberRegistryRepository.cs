using ApiModel.Entities;
using ApiServer.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Repositories
{
    public class MemberRegistryRepository : RepositoryBase<MemberRegistry, MemberRegistryDTO>, IRepository<MemberRegistry, MemberRegistryDTO>
    {

        #region 构造函数
        public MemberRegistryRepository(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep)
        : base(context, permissionTreeRep)
        {
        }
        #endregion

    }
}
