using System.Runtime.Serialization;

namespace Service.AddressBook.Grpc.Models;

[DataContract]
public class AddNicknameRequest
{
    [DataMember(Order = 1)] public string OwnerClientId { get; set; }
    [DataMember(Order = 2)] public string Nickname { get; set; }
    [DataMember(Order = 3)] public string Name { get; set; }
}