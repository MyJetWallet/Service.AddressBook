using System.Runtime.Serialization;
using Service.AddressBook.Domain.Models;

namespace Service.AddressBook.Grpc.Models
{
    [DataContract]
    public class HelloMessage : IHelloMessage
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }
    }
}