using System.ComponentModel.DataAnnotations;

namespace ZonefyDotnet.DTOs
{
    public class CreateUserDTO
    {
        [Required, EmailAddress]
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        [Required, MinLength(6, ErrorMessage = "Please enter at least 6 characters!")]
        public required string Password { get; set; }
    }

    public class GetUserDto
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }

    public class LoginUserDto
    {
        [Required, EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }

    public class ChangePasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string CurrentPassword { get; set; }
        [Required, MinLength(6, ErrorMessage = "Please enter at least 6 characters!")]
        public string NewPassword { get; set; }
    }

    public class ResetPasswordDto
    {
        [Required, MinLength(6, ErrorMessage = "Please enter at least 6 characters!")]
        public string NewPassword { get; set; }
        [Required, Compare("NewPassword", ErrorMessage = "Password do not match")]
        public string ConfirmNewPassword { get; set; }
    }

    public class TokenDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }

    public class RefreshTokenDto
    {
        public string Token { get; set; } = null!;
    }
}
