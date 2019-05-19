using Microsoft.AspNetCore.Mvc.Filters;

namespace JelaCoreLib.Base.Controller.Filter
{
    /// <summary>
    /// Attribute for use with <see cref="JelaApiController"/> as global attribute for all actions.
    /// </summary>
    public class JelaActionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Create new instance of <see cref="JelaActionFilterAttribute"/> that is designed to be used on the <see cref="JelaApiController"/> class.
        /// </summary>
        public JelaActionFilterAttribute()
        {
        }

        /// <summary>
        /// Call before action is executed.
        /// </summary>
        /// <param name="context">A context for action filters.</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Controller is JelaApiController controller)
            {
                controller.SyncModelState();
            }
        }
    }
}
