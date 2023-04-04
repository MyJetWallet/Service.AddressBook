using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using Service.AddressBook.Grpc;

namespace Service.AddressBook.Client
{
    [UsedImplicitly]
    public class AddressBookClientFactory: MyGrpcClientFactory
    {
        public AddressBookClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public IHelloService GetHelloService() => CreateGrpcService<IHelloService>();
    }
}
