using JelaCoreLib.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Encodings.Web;

namespace JelaCoreLib.Base.Controller
{
    /// <summary>
    /// Base for API Controller, in the special way by Jela.
    /// </summary>
    [ApiController]
    public abstract class JelaApiController : ControllerBase
    {
        /// <summary>
        /// Logger service.
        /// </summary>
        protected readonly ILogger _logger;
        /// <summary>
        /// Service for encoding url's.
        /// </summary>
        protected readonly UrlEncoder _urlEncoder;
        /// <summary>
        /// Handle for custom validation dictionary that should be merged back to modelstate when checked.
        /// According to alot of documentation, the linking should work straight, but it doesn't.
        /// </summary>
        protected readonly MsValidationDictionary _validationModelState;

        /// <summary>
        /// Constructor for base of the other controllers.
        /// </summary>
        /// <param name="logger">Logging service.</param>
        /// <param name="urlEncoder">Url encoder for safe urls.</param>
        public JelaApiController(
            ILogger logger,
            UrlEncoder urlEncoder)
        {
            _logger = logger;
            _urlEncoder = urlEncoder;

            _validationModelState = new MsValidationDictionary(this.ModelState);
        }

        /// <summary>
        /// Syncronize the reference for the custom modelState behaviour.
        /// </summary>
        protected void SyncModelState()
        {
            //First merge the states over.
            this.ModelState.Merge(_validationModelState.ModelState);
            //Then update reference.
            _validationModelState.ModelState = this.ModelState;
        }

        #region Helpers
        /// <summary>
        /// Add errors from IdentityResult into the modelstate.
        /// </summary>
        /// <param name="result">The result where we get the errors from.</param>
        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        /// <summary>
        /// Perform Local app redirect. Redirects to home on external url's.
        /// </summary>
        /// <param name="returnUrl">Url, where we redirect to.</param>
        /// <returns>Action that redirects to returnUrl</returns>
        protected IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="unformattedKey"></param>
        /// <returns></returns>
        protected string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        #endregion
    }
}
