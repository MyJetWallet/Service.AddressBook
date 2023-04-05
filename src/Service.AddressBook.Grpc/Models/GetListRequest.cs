using System.Runtime.Serialization;

namespace Service.AddressBook.Grpc.Models;

[DataContract]
public class GetListRequest
{
    [DataMember(Order = 1)] public string OwnerClientId { get; set; }
    [DataMember(Order = 2)] public int Skip { get; set; }
    [DataMember(Order = 3)] public int Take { get; set; }
}