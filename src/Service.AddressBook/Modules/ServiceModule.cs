using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using MyJetWallet.Sdk.ServiceBus;
using Service.AddressBook.Domain;
using Service.AddressBook.Domain.Models.Messages;
using Service.AddressBook.Services;
using Service.ClientProfile.Client;

namespace Service.AddressBook.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var noSqlClient = builder.RegisterMyServiceBusTcpClient(() => Program.Settings.SpotServiceBusHostPort, Program.LogFactory);
            builder.RegisterMyServiceBusPublisher<ContactReceivingApproved>(noSqlClient,
                ContactReceivingApproved.TopicName, true);
            builder.RegisterClientProfileClientWithoutCache(Program.Settings.ClientProfileGrpcServiceUrl);


            builder
                .RegisterType<AddressBookRepositoryMemoryCache>()
                .As<IAddressBookRepository>()
                .AsSelf()
                .SingleInstance();
        }
    }
}