using Microsoft.AspNetCore.Mvc;
using WookieBooks.API.Interfaces;
using WookieBooks.ViewModel.Authentication.Request;
using WookieBooks.ViewModel.Authentication.Response;

namespace WookieBooks.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly ITokenService _tokenService;
        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticationResponse>> Authenticate([FromBody] AuthenticationRequest request)
        {
            try
            {
                var response = await _tokenService.Authenticate(request);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }
    }
}
