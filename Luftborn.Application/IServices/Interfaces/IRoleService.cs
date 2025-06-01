using Microsoft.AspNetCore.Identity;
using Luftborn.Application.IServices.Models;
using Luftborn.Application.IServices.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luftborn.Application.IServices.Interfaces
{
    public interface IRoleService
    {
        Task<(IEnumerable<RoleWithTranslationDto> Roles, int TotalCount)> GetAllRolesAsync(PaginationParams paginationParams);
        Task<IdentityResult> CreateRoleAsync(RoleModel model);
        Task<IdentityResult> UpdateRoleAsync(string currentName, RoleUpdateModel model);
        Task<IdentityResult> DeleteRoleAsync(string roleName);
        Task<IdentityResult> AssignRoleToUserAsync(string userId, string roleName);
        Task<IdentityResult> RemoveRoleFromUserAsync(string userId, string roleName);
        Task<bool> RoleExistsAsync(string roleName);


    }
}


