using Autofac;
using Service.AddressBook.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.AddressBook.Client
{
    public static class AutofacHelper
    {
        public static void RegisterAddressBookClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new AddressBookClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetHelloService()).As<IAddressBookService>().SingleInstance();
        }
    }
}
