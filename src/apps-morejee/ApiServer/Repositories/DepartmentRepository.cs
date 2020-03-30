using ApiModel.Consts;
using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ApiServer.Repositories
{
    public class DepartmentRepository : ListableRepository<Department, DepartmentDTO>
    {
        public DepartmentRepository(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep)
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

        #region CreateAsync 新建部门信息
        /// <summary>
        /// 新建部门信息
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public override async Task CreateAsync(string accid, Department data)
        {
            using (var tx = _DbContext.Database.BeginTransaction())
            {
                try
                {
                    await base.CreateAsync(accid, data);
                    var otree = new PermissionTree();
                    otree.Name = data.Name;
                    otree.OrganizationId = data.OrganizationId;
                    otree.NodeType = AppConst.S_NodeType_Department;
                    otree.ObjId = data.Id;
                    if (string.IsNullOrWhiteSpace(data.ParentId))
                    {
                        var refOrganNode = await _DbContext.PermissionTrees.Where(x => x.ObjId == data.OrganizationId).FirstOrDefaultAsync();
                        if (refOrganNode != null)
                        {
                            otree.ParentId = refOrganNode.Id;
                            await _PermissionTreeRepository.AddChildNode(otree);
                        }
                    }
                    else
                    {
                        var parentDepartmentNode = await _DbContext.PermissionTrees.Where(x => x.ObjId == data.ParentId).FirstOrDefaultAsync();
                        if (parentDepartmentNode != null)
                        {
                            otree.ParentId = parentDepartmentNode.Id;
                            otree.OrganizationId = data.OrganizationId;
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

        #region GetByOrgan 根据组织id获取部门信息
        /// <summary>
        /// 根据组织id获取部门信息
        /// </summary>
        /// <param name="organId"></param>
        /// <returns></returns>
        public async Task<List<DepartmentDTO>> GetByOrgan(string organId)
        {
            var organ = await _DbContext.Organizations.FindAsync(organId);
            var owner = await _DbContext.Accounts.FindAsync(organ.OwnerId);
            var treeQ = from ps in _DbContext.Set<PermissionTree>()
                        where ps.OrganizationId == organId && ps.NodeType == AppConst.S_NodeType_Department
                        select ps;
            var query = from it in _DbContext.Departments
                        join ps in treeQ on it.Id equals ps.ObjId
                        where it.ActiveFlag == AppConst.I_DataState_Active
                        && it.Id != owner.DepartmentId
                        select it;
            return await query.Select(x => x.ToDTO()).ToListAsync();
        }
        #endregion
    }
}
