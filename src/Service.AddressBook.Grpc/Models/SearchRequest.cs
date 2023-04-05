using System.Runtime.Serialization;

namespace Service.AddressBook.Grpc.Models;

[DataContract]
public class SearchRequest
{
    [DataMember(Order = 1)] public string OwnerClientId { get; set; }
    [DataMember(Order = 2)] public string SearchText { get; set; }
}