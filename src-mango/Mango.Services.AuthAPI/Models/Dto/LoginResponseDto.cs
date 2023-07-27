namespace Mango.Services.AuthAPI.Models.Dto
{
    public class LoginResponseDto
    {
        public UserDto? User { get; set; }
        /// <summary>
        /// This would be jwt token
        /// This token is generated with a help of secret key
        /// along with other settings specified in appsettings.json
        /// look for jwtOptions
        /// </summary>
        public string Token { get; set; }
    }
}
