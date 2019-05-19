using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;

namespace JelaCoreLib.Base.Controller
{
    /// <summary>
    /// Base for API Controllers with identity user related to them. In the special way by Jela.
    /// </summary>
    [Authorize]
    public abstract class JelaIdentityApiController<TUser> : JelaApiController
        where TUser : class
    {
        /// <summary>
        /// The manager for user related data.
        /// </summary>
        protected readonly UserManager<TUser> _userManager;

        /// <summary>
        /// Constructor for base of the other controllers.
        /// </summary>
        /// <param name="userManager">Manager for the user.</param>
        /// <param name="logger">Logging service.</param>
        /// <param name="urlEncoder">Url encoder for safe urls.</param>
        public JelaIdentityApiController(
            UserManager<TUser> userManager,
            ILogger logger,
            UrlEncoder urlEncoder)
            :base(logger,urlEncoder)
        {
            _userManager = userManager;
        }

        #region Helpers

        /// <summary>
        /// Get ID of the logged in user if present. Otherwise returns null.
        /// </summary>
        /// <returns>Returns ID of the logged in user.</returns>
        protected string GetUserID()
        {
            return _userManager.GetUserId(HttpContext.User);
        }

        /// <summary>
        /// Get username of the logged in user if present. Otherwise returns null.
        /// </summary>
        /// <returns>Returns username of the logged in user.</returns>
        protected string GetUserName()
        {
            return _userManager.GetUserName(HttpContext.User);
        }

        #endregion
    }
}
