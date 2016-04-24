using Microsoft.AspNet.SignalR;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveChat.App_Start
{
    public class UnityHubConfig : DefaultDependencyResolver
    {
        private UnityContainer _container;

        public UnityHubConfig()
        {
            _container = new UnityContainer();
            SetHubDependencies(_container);
        }

        private static void SetHubDependencies(UnityContainer container)
        {
            //container.RegisterType<MessengerHub>();
        }

        public override object GetService(Type serviceType)
        {
            if (_container.IsRegistered(serviceType))
            {
                return _container.Resolve(serviceType);
            }
            return base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            return _container.ResolveAll(serviceType).Concat(base.GetServices(serviceType));
        }
    }
}