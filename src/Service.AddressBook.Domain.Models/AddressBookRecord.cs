using System;
using System.Runtime.Serialization;

namespace Service.AddressBook.Domain.Models
{
    [DataContract]
    public class AddressBookRecord
    {
        [DataMember(Order = 1)] public string OwnerClientId { get; set; }
        [DataMember(Order = 2)] public string ContactClientId { get; set; }
        [DataMember(Order = 3)] public string Nickname { get; set; }
        [DataMember(Order = 4)] public string Name { get; set; }
        [DataMember(Order = 5)] public DateTime LastTs { get; set; }
        [DataMember(Order = 6)] public bool ReceiveApprovalGranted { get; set; }
        [DataMember(Order = 7)] public int TransfersCount { get; set; }
    }
}