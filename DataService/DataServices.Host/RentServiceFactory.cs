using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;
using Unity.Wcf;
using WhatsRent.DataServices.RentService;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace WhatsRent.DataServices.Host
{
    public class RentServiceFactory : UnityServiceHostFactory
    {
        private Uri _baseUri;

        protected override System.ServiceModel.ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            _baseUri = baseAddresses.First(uri => uri.Scheme.StartsWith("http"));
            var vip = ConfigurationManager.AppSettings["vip"];

            if (!string.IsNullOrEmpty(vip))
                _baseUri = new UriBuilder { Scheme = _baseUri.Scheme, Host = vip, Path = _baseUri.AbsolutePath + "/" }.Uri;

            return base.CreateServiceHost(serviceType, baseAddresses);
        }

        protected override void ConfigureContainer(IUnityContainer container)
        {
            const string CONNECTION_STRING = @"Data Source = MUNISH-LENOVO; Initial Catalog = WhatsRent; Integrated Security = True";
            container.RegisterType<IRentService, RentService.RentService>
                (new InjectionConstructor(CONNECTION_STRING));
        }
    }
}