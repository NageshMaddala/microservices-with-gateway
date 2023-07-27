using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        // we are using default role, so using IdentityRole
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        /// <summary>
        /// These helper methods are injected automatically,
        /// we do not have to make any configuration
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        public AuthService(AppDbContext db,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        // RoleManager would help us adding roles in database
        public async Task<bool> AssignRole(string email, string roleName)
        {
            // retrieve user first based on email
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == email.ToLower());

            if(user != null)
            {
                // check if role already exists

                // below line would make it as sync
                if(!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    // create role if it doesn't exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            if (user == null || isValid == false)
            {
                return new LoginResponseDto()
                {
                    User = null,
                    Token = ""
                };
            }

            // if user was found, Generate JWT Token and send it back to caller
            var token = _jwtTokenGenerator.GenerateToken(user);

            UserDto userDto = new UserDto()
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };

            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDto,
                Token = token
            };

            return loginResponseDto;
        }

        /// <summary>
        /// Ideal scenario: If registration is successful return empty
        /// For failure return result.Errors.FirstOrDefault().Description 
        /// which is provided by .net identity service
        /// </summary>
        /// <param name="registrationRequestDto"></param>
        /// <returns></returns>
        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            // .netIdentity provides lots of helper function
            // to automatically create user, setup roles and so on..
            // all those services or helper classes are used here within this class

            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber
            };

            try
            {
                // below will create user, it will add an entry in database
                // all the other related columns are populated with .net identity
                // we do not have to do anything ex: passwordHash would be created for us
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);

                if (result.Succeeded)
                {
                    // User reg is successful
                    // we can retrieve user based on email

                    var userToReturn = _db.ApplicationUsers.First(u => u.UserName == registrationRequestDto.Email);

                    UserDto userDto = new()
                    {
                        Email = userToReturn.Email,
                        Id = userToReturn.Id,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber
                    };
                    //return userDto;
                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex)
            {

            }
            return "Error Encountered";
        }
    }
}