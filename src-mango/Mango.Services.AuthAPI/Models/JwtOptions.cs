namespace Mango.Services.AuthAPI.Models
{
    // provides a way to read stuff from appsettings.json
    public class JwtOptions
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
    }
}