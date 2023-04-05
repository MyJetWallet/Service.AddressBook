using System.Runtime.Serialization;

namespace Service.AddressBook.Grpc.Models;

[DataContract]
public class ApproveRequest
{
    [DataMember(Order = 1)] public string SenderClientId { get; set; }
    [DataMember(Order = 2)] public string ReceiverClientId { get; set; }
}