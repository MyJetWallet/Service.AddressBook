using System.Runtime.Serialization;

namespace Service.AddressBook.Grpc.Models;

[DataContract]
public class UpdateContactRequest
{
    [DataMember(Order = 1)] public string OwnerClientId { get; set; }
    [DataMember(Order = 2)] public string ContactClientId { get; set; }
    [DataMember(Order = 3)] public string Name { get; set; }
    [DataMember(Order = 4)] public string Iban { get; set; }
    [DataMember(Order = 5)] public string Bic { get; set; }
    [DataMember(Order = 6)] public string BankName { get; set; }
}