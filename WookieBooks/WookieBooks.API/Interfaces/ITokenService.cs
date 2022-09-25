using WookieBooks.ViewModel.Authentication.Request;
using WookieBooks.ViewModel.Authentication.Response;

namespace WookieBooks.API.Interfaces
{
    public interface ITokenService
    {
        Task<AuthenticationResponse> Authenticate(AuthenticationRequest authRequest);
    }
}
