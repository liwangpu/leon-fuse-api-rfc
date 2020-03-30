using ApiModel.Entities;
using ApiServer.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Repositories
{
    public class UserNavRepository : RepositoryBase<UserNav, UserNavDTO>, IRepository<UserNav, UserNavDTO>
    {
        public UserNavRepository(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep)
            : base(context, permissionTreeRep)
        {
        }

        public override async Task<UserNavDTO> GetByIdAsync(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var data = await _DbContext.Set<UserNav>().Include(x => x.UserNavDetails).FirstOrDefaultAsync(x => x.Id == id);
                if (data != null)
                {
                    if (!string.IsNullOrWhiteSpace(data.Role))
                    {
                        var refNav = await _DbContext.UserRoles.Where(x => x.Id == data.Role).FirstOrDefaultAsync();
                        if (refNav != null)
                        {
                            data.RoleName = refNav.Name;
                        }
                    }

                    if (data.UserNavDetails != null && data.UserNavDetails.Count > 0)
                    {
                        var navItems = await _DbContext.Navigations.ToListAsync();
                        for (int idx = data.UserNavDetails.Count - 1; idx >= 0; idx--)
                        {
                            var item = data.UserNavDetails[idx];
                            var refNav = navItems.Where(x => x.Id == item.RefNavigationId).FirstOrDefault();
                            if (refNav == null)
                                continue;
                            item.Title = refNav.Title;
                            item.Name = refNav.Name;
                            item.Url = refNav.Url;
                            item.Icon = refNav.Icon;
                            item.PagedModel = refNav.PagedModel;
                            item.NodeType = refNav.NodeType;
                            item.Resource = refNav.Resource;
                            item.NewTapOpen = refNav.NewTapOpen;

                            if (!string.IsNullOrWhiteSpace(item.ExcludeQueryParams))
                            {
                                var excludeArr = item.ExcludeQueryParams.Split(",");
                                var fullArr = refNav.QueryParams.Split(",");

                                var destArr = fullArr.Where(x => !excludeArr.Contains(x)).ToList();
                                item.QueryParams = string.Join(',', destArr);
                            }
                            else
                            {
                                item.QueryParams = refNav.QueryParams;
                            }

                            if (!string.IsNullOrWhiteSpace(item.ExcludeFiled))
                            {
                                var excludeArr = item.ExcludeFiled.Split(",");
                                var fullArr = refNav.Field.Split(",");
                                var destArr = fullArr.Where(x => !excludeArr.Contains(x)).ToList();
                                item.Field = string.Join(',', destArr);
                            }
                            else
                            {
                                item.Field = refNav.Field;
                            }

                            if (!string.IsNullOrWhiteSpace(item.ExcludePermission))
                            {
                                var excludeArr = item.ExcludePermission.Split(",");
                                var fullArr = refNav.Permission.Split(",");
                                var destArr = fullArr.Where(x => !excludeArr.Contains(x)).ToList();
                                item.Permission = string.Join(',', destArr);
                            }
                            else
                            {
                                item.Permission = refNav.Permission;
                            }
                        }
                    }

                    return data.ToDTO();
                }
            }
            return new UserNavDTO();
        }
    }
}
