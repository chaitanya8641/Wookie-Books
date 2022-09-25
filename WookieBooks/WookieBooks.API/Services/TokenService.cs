using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WookieBooks.API.Interfaces;
using WookieBooks.Common;
using WookieBooks.Entities;
using WookieBooks.Repository;
using WookieBooks.ViewModel.Authentication.Request;
using WookieBooks.ViewModel.Authentication.Response;

namespace WookieBooks.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;
        public TokenService(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings)
        {
            _unitOfWork = unitOfWork;
            _appSettings = appSettings.Value;
        }
        public async Task<AuthenticationResponse> Authenticate(AuthenticationRequest authRequest)
        {
            
            var users = await _unitOfWork.Users.Find(x => x.UserName == authRequest.Username && x.Password == authRequest.Password);
            AuthenticationResponse response = new();
            if (users.Any())
            {
                response.Token = GenerateJwtToken(users.First());
            }
            return response;
        }

        private string GenerateJwtToken(User user)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.SecretKey));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>() {

                new Claim("UserId" , user.Id.ToString()),
            };

            var tokeOptions = new JwtSecurityToken(
                issuer: _appSettings.Issuer,
                audience: _appSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_appSettings.Expires)),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

            return tokenString;
        }
    }
}
