using System.ComponentModel.DataAnnotations;

namespace Luftborn.Application.IServices.Models
{
    public class RefreshTokenModel
    {
        [Required(ErrorMessage = "Access token is required")]
        public string AccessToken { get; set; }

        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; }
    }
} 