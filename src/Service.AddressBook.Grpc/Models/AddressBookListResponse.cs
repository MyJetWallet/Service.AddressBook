using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.AddressBook.Domain.Models;

namespace Service.AddressBook.Grpc.Models;

[DataContract]
public class AddressBookListResponse
{
    [DataMember(Order = 1)] public List<AddressBookRecord> Records { get; set; }
    [DataMember(Order = 2)] public List<AddressBookRecord> TopContacts { get; set; }
}