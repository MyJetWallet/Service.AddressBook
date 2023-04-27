using System.Runtime.Serialization;
using Service.AddressBook.Domain.Models;

namespace Service.AddressBook.Grpc.Models;

[DataContract]
public class OperationResponse
{
    [DataMember(Order = 1)] public bool IsSuccess { get; set; }
    [DataMember(Order = 2)] public string ErrorMessage { get; set; }
    [DataMember(Order = 3)] public AddressBookRecord Record { get; set; }
}