using System.Runtime.Serialization;

namespace Service.AddressBook.Grpc.Models;

[DataContract]
public class GetByIdRequest
{
    [DataMember(Order = 1)] public string ContactId { get; set; }
}