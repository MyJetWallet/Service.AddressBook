using System.Runtime.Serialization;

namespace Service.AddressBook.Grpc.Models;

[DataContract]
public class UpdateNicknameRequest
{
    [DataMember(Order = 1)] public string OldNickname { get; set; }
    [DataMember(Order = 2)] public string NewNickname { get; set; }
}