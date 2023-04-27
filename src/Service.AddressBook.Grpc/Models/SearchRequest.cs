using System.Runtime.Serialization;
using Service.AddressBook.Domain.Models;

namespace Service.AddressBook.Grpc.Models;

[DataContract]
public class SearchRequest
{
    [DataMember(Order = 1)] public string OwnerClientId { get; set; }
    [DataMember(Order = 2)] public string SearchText { get; set; }
    [DataMember(Order = 3)] public bool WithIban { get; set; }
    [DataMember(Order = 4)] public bool WithNickname { get; set; }
}