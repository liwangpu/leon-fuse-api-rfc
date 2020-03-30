using IdentityServer4.Models;
using IdentityServer4.Validation;
using IDS.Domain.AggregateModels.UserAggregate;
using IDS.Infrastructure.Specifications.IdentitySpecifications;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IDS.API.Infrastructure.IdentityServer
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IIdentityRepository identityRepository;

        public ResourceOwnerPasswordValidator(IIdentityRepository identityRepository)
        {
            this.identityRepository = identityRepository;
        }
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var identity = await identityRepository.Get(new GetIdentityByUsernameAndPassword(context.UserName, context.Password)).FirstOrDefaultAsync();
            if (identity == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "账号或者密码不存在");
                return;
            }
            var claims = new List<Claim>();

            //var tenantRoles = await identityRoleService.GetUserRolesAsync(identityDto.Id);
            //long tenantId = 0;
            //if (tenantRoles.Count > 0)
            //{
            //    tenantId = tenantRoles[0].TenantId;
            //}

            //var employees = await employeeService.QueryAsync(new EmployeeQueryDto { TenantId = tenantId, IdentityId = identityDto.Id });
            //if (employees != null && employees.Items != null && employees.Items.Count() > 0)
            //{
            //    var employee = employees.Items.First();
            //    claims.Add(new Claim("UserId", employee.Id.ToString()));
            //    claims.Add(new Claim("Username", employee.Name));
            //}

            claims.Add(new Claim("IdentityId", identity.Id.ToString()));
            //claims.Add(new Claim("TenantId", tenantId.ToString()));
            context.Result = new GrantValidationResult(context.UserName, "custom", claims);
        }
    }
}
