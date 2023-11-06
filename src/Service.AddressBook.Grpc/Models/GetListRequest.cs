using System.Runtime.Serialization;
using Service.AddressBook.Domain.Models;

namespace Service.AddressBook.Grpc.Models;

[DataContract]
public class GetListRequest
{
    [DataMember(Order = 1)] public string OwnerClientId { get; set; }
    [DataMember(Order = 2)] public int Skip { get; set; }
    [DataMember(Order = 3)] public int Take { get; set; }
    
    [DataMember(Order = 4)] public bool WithIban { get; set; }
    [DataMember(Order = 5)] public bool WithNickname { get; set; }
    [DataMember(Order = 6)] public IbanType IbanType { get; set; }
}