using Microsoft.AspNetCore.Identity;
using Luftborn.Application.IServices.Models;
using System.Threading.Tasks;

namespace Luftborn.Application.IServices.Interfaces
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticates a user and returns authentication response
        /// </summary>
        Task<AuthResponse> LoginAsync(LoginModel model);

        /// <summary>
        /// Refreshes the authentication token
        /// </summary>
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenModel model);

        /// <summary>
        /// Signs out the current user
        /// </summary>
        Task<AuthResponse> LogoutAsync();

        /// <summary>
        /// Validates the current token and returns user information
        /// </summary>
        Task<AuthResponse> ValidateTokenAsync(string userId);
    }
} 