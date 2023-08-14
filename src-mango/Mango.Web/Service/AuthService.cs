using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Web.Models;

namespace Mango.Web.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;

        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto> AssignRoleAsync(RegistrationRequestDto registrationRequestDto)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registrationRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/AssignRole"
            });
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<ResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/login"
            });
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<ResponseDto> RegisterAsync(RegistrationRequestDto registrationRequestDto)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registrationRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/register"
            });
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
