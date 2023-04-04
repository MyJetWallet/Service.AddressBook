using System.ServiceModel;
using System.Threading.Tasks;
using Service.AddressBook.Grpc.Models;

namespace Service.AddressBook.Grpc
{
    [ServiceContract]
    public interface IHelloService
    {
        [OperationContract]
        Task<HelloMessage> SayHelloAsync(HelloRequest request);
    }
}