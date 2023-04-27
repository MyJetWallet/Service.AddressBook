using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;
using Service.AddressBook.Domain;
using Service.AddressBook.Domain.Models.Messages;
using Service.AddressBook.Jobs;
using Service.AddressBook.Services;
using Service.Clearjunction.Client;
using Service.ClientProfile.Client;
using Service.InternalTransfer.Domain.Models;

namespace Service.AddressBook.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var queueName = "Service.AddressBook";
            var myServiceBusTcpClient = builder.RegisterMyServiceBusTcpClient(() => Program.Settings.SpotServiceBusHostPort, Program.LogFactory);
            builder.RegisterMyServiceBusPublisher<ContactReceivingApproved>(myServiceBusTcpClient,
                ContactReceivingApproved.TopicName, true);
            builder.RegisterMyServiceBusSubscriberSingle<Transfer>(myServiceBusTcpClient, Transfer.TopicName, queueName,
                TopicQueueType.PermanentWithSingleConnection);

            builder.RegisterClientProfileClientWithoutCache(Program.Settings.ClientProfileGrpcServiceUrl);

            builder
                .RegisterType<AddressBookRepositoryMemoryCache>()
                .As<IAddressBookRepository>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<TransferCounterJob>().AsSelf().SingleInstance().AutoActivate();
            
            builder.RegisterClearjunctionClient(Program.Settings.ClearjunctionGrpcServiceUrl);
        }
    }
}