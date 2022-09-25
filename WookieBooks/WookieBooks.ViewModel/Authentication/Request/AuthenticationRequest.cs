using System.ComponentModel.DataAnnotations;

namespace WookieBooks.ViewModel.Authentication.Request
{
    public class AuthenticationRequest
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

    }
}
