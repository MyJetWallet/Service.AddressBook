using System;
using System.Runtime.Serialization;

namespace Service.AddressBook.Domain.Models
{
    [DataContract]
    public class AddressBookRecord
    {
        [DataMember(Order = 1)] public string OwnerClientId { get; set; }
        [DataMember(Order = 2)] public string ContactId { get; set; }
        [DataMember(Order = 3)] public string Nickname { get; set; }
        [DataMember(Order = 4)] public string Name { get; set; }
        [DataMember(Order = 5)] public DateTime LastTs { get; set; }
        [DataMember(Order = 6)] public bool ReceiveApprovalGranted { get; set; }
        [DataMember(Order = 7)] public int TransfersCount { get; set; }
        [DataMember(Order = 8)] public string Iban { get; set; }
        [DataMember(Order = 9)] public string Bic { get; set; }
        [DataMember(Order = 10)] public string BankName { get; set; }
        [DataMember(Order = 11)] public long Order { get; set; }
        
    }
}