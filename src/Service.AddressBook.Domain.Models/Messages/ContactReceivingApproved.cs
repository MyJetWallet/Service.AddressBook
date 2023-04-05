using System.Runtime.Serialization;

namespace Service.AddressBook.Domain.Models.Messages
{
    [DataContract]
    public class ContactReceivingApproved
    {
        public const string TopicName = "address-book-contact-receiving-approved";
        
        [DataMember(Order = 1)] public string SenderClientId { get; set; }
        [DataMember(Order = 2)] public string ReceiverClientId { get; set; }
    }
}