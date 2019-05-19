using System;
using System.Collections.Generic;
using System.Text;

namespace JelaCoreLib.Extension
{
    //HOX! Fetched ready if there's ever need to start use akka.NET

    /*/// <summary>
    /// Extension for AKKA, to simply create and handle scopes.
    /// </summary>
    public class ServiceScopeExtension : IExtension
    {
        /// <summary>
        /// Handle for storing the service scope creation factory.
        /// </summary>
        private IServiceScopeFactory _serviceScopeFactory;

        /// <summary>
        /// Initialize the extension.
        /// </summary>
        /// <param name="serviceScopeFactory"></param>
        public void Initialize(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        /// <summary>
        /// Create new scope for services.
        /// </summary>
        /// <returns></returns>
        public IServiceScope CreateScope()
        {
            return _serviceScopeFactory.CreateScope();
        }
    }

    /// <summary>
    /// Provider for service scope extension.
    /// </summary>
    public class ServiceScopeExtensionIdProvider : ExtensionIdProvider<ServiceScopeExtension>
    {
        /// <summary>
        /// Override the create action.
        /// </summary>
        /// <param name="system"></param>
        /// <returns>New service scope extension.</returns>
        public override ServiceScopeExtension CreateExtension(ExtendedActorSystem system)
        {
            return new ServiceScopeExtension();
        }

        public static ServiceScopeExtensionIdProvider Instance = new ServiceScopeExtensionIdProvider();
    }

    /// <summary>
    /// Extensions for AKKA.
    /// </summary>
    public static class AkkaExtensions
    {

        public static void AddServiceScopeFactory(this ActorSystem system, IServiceScopeFactory serviceScopeFactory)
        {
            system.RegisterExtension(ServiceScopeExtensionIdProvider.Instance);
            ServiceScopeExtensionIdProvider.Instance.Get(system).Initialize(serviceScopeFactory);
        }

        public static IServiceScope CreateScope(this IActorContext context)
        {
            return ServiceScopeExtensionIdProvider.Instance.Get(context.System).CreateScope();
        }
    }*/
}
