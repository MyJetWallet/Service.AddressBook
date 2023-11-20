using System.Runtime.Serialization;
using Service.AddressBook.Domain.Models;

namespace Service.AddressBook.Grpc.Models;

[DataContract]
public class OperationResponse
{
    [DataMember(Order = 1)] public bool IsSuccess { get; set; }
    [DataMember(Order = 2)] public string ErrorMessage { get; set; }
    [DataMember(Order = 3)] public AddressBookRecord Record { get; set; }
    [DataMember(Order = 4)] public GlobalSendErrorCode ErrorCode { get; set; } = GlobalSendErrorCode.OK;
}

public enum GlobalSendErrorCode
{
    OK,
    InternalError,
    IbanAlreadyUsed,
    NameAlreadyUsed,
    NicknameAlreadyUsed,
    ContactNotFound,
    InvalidIban,
    InvalidNickname,
    IbanNotReachable,
    InvalidBic,
}