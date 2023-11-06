using System.Runtime.Serialization;
using Service.AddressBook.Domain.Models;

namespace Service.AddressBook.Grpc.Models;

[DataContract]
public class AddIbanRequest
{
    [DataMember(Order = 1)] public string OwnerClientId { get; set; }
    [DataMember(Order = 2)] public string Name { get; set; }
    [DataMember(Order = 3)] public string Iban { get; set; }
    [DataMember(Order = 4)] public string Bic { get; set; }
    [DataMember(Order = 5)] public string BankName { get; set; }
    [DataMember(Order = 6)] public string ClientId { get; set; }
    [DataMember(Order = 7)] public string BankCountry { get; set; }
    [DataMember(Order = 8)] public string FullName { get; set; }
    [DataMember(Order = 9)] public IbanType IbanType { get; set; }
}