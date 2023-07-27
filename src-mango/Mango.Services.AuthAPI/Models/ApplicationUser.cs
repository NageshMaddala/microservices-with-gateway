using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Models
{
    /// <summary>
    /// This is how IdentityUser is extended
    /// Post that we need to ApplicationUser in DBContext
    /// and also in program.cs relatedt to AddIdentity
    /// </summary>
    /// <remarks>
    /// Here in this case ApplicationUser table won't be created
    /// instead columns specified in ApplicationUser would be added to AspNetUsers table
    /// This is by design of aspnet identity
    /// </remarks>    
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}