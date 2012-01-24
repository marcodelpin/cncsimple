using Framework.Abstractions.Wpf.ServiceLocation;
using Microsoft.Practices.ServiceLocation;
using System;
using Microsoft.Practices.Unity;

namespace ServiceLocation.Silverlight.Unity
{
    public class UnityServiceLocator : ISimpleServiceLocator
    {
        private readonly UnityContainer unityContainer;

        public UnityServiceLocator()
        {
            this.unityContainer = new UnityContainer();
        }

        public T Get<T>()
        {
            return this.unityContainer.Resolve<T>();
        }

        public Type Get(Type type)
        {
           return (Type) this.unityContainer.Resolve(type);
        }

        public T Get<T>(string key)
        {
            return this.unityContainer.Resolve<T>(key);
        }

        public void Inject<T>(T instance)
        {
            this.unityContainer.RegisterInstance(instance, new ExternallyControlledLifetimeManager());
        }

        public void InjectAsSingleton<T>(T instance)
        {
            this.unityContainer.RegisterInstance(instance);
        }

        public void Register<TInterface, TImplementor>() where TImplementor : TInterface
        {
            this.unityContainer.RegisterType<TInterface, TImplementor>();
        }

        public void Register<TInterface, TImplementor>(string key) where TImplementor : TInterface
        {
            this.unityContainer.RegisterType<TInterface, TImplementor>(key);            
        }

        public void RegisterAsSingleton<TInterface, TImplementor>() where TImplementor : TInterface
        {
            this.unityContainer.RegisterType<TInterface, TImplementor>(new ContainerControlledLifetimeManager());
        }
    }
}

