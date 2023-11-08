using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using Service.AddressBook.Domain;
using Service.AddressBook.Domain.Models;
using Service.AddressBook.Domain.Models.Messages;
using Service.AddressBook.Grpc;
using Service.AddressBook.Grpc.Models;
using Service.AddressBook.Settings;
using Service.Clearjunction.Grpc;
using Service.Clearjunction.Grpc.Models;
using Service.ClientProfile.Grpc;
using Service.ClientProfile.Grpc.Models.Requests;

namespace Service.AddressBook.Services
{
    public class AddressBookService: IAddressBookService
    {
        private readonly ILogger<AddressBookService> _logger;
        private readonly IAddressBookRepository _addressBookRepository;
        private readonly IClientProfileService _clientProfileService;
        private readonly IServiceBusPublisher<ContactReceivingApproved> _contactReceivingApprovedPublisher;
        private readonly IIbanService _ibanService;
        public AddressBookService(ILogger<AddressBookService> logger, IAddressBookRepository addressBookRepository, IClientProfileService clientProfileService, IServiceBusPublisher<ContactReceivingApproved> contactReceivingApprovedPublisher, IIbanService ibanService)
        {
            _logger = logger;
            _addressBookRepository = addressBookRepository;
            _clientProfileService = clientProfileService;
            _contactReceivingApprovedPublisher = contactReceivingApprovedPublisher;
            _ibanService = ibanService;
        }

        public async Task<AddressBookListResponse> SearchAsync(SearchRequest request)
        {
            _logger.LogInformation("Requested search for client {clientId} with text {searchText}", request.OwnerClientId, request.SearchText);
            try
            {
                var records = await _addressBookRepository.FindAsync(request.OwnerClientId, request.SearchText, request.WithIban, request.WithNickname, request.IbanType);
                var topContacts = await _addressBookRepository.GetListAsync(request.OwnerClientId, 0, 5, request.WithIban, request.WithNickname, request.IbanType);

                return new AddressBookListResponse()
                {
                    Records = records,
                    TopContacts = topContacts,
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot search for client {clientId} with text {searchText}", request.OwnerClientId, request.SearchText);
                throw;
            }
        }

        public async Task<AddressBookRecord> GetByNicknameAsync(GetByNicknameRequest request)
        {
            _logger.LogInformation("Requested get by nickname for client {clientId} with nickname {nickname}", request.OwnerClientId, request.Nickname);
            try
            {
                return await _addressBookRepository.GetByNicknameAsync(request.OwnerClientId, request.Nickname);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot get by nickname for client {clientId} with nickname {nickname}", request.OwnerClientId, request.Nickname);
                throw;
            }
        }

        public async Task<AddressBookListResponse> GetRecordsAsync(GetListRequest request)
        {
            _logger.LogInformation("Requested get list for client {clientId}", request.OwnerClientId);
            try
            {
                var records = await _addressBookRepository.GetListAsync(request.OwnerClientId, request.Skip, request.Take, request.WithIban, request.WithNickname, request.IbanType);
                var topContacts = await _addressBookRepository.GetListAsync(request.OwnerClientId, 0, 5,  request.WithIban, request.WithNickname, request.IbanType);
                return new AddressBookListResponse()
                {
                    Records = records,
                    TopContacts = topContacts
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot get list for client {clientId}", request.OwnerClientId);
                throw;
            }
        }

        public async Task ChangeNickname(UpdateNicknameRequest request)
        {
            _logger.LogInformation("Nickname was changed from {oldNickname} to {newNickname} for client {clientId}", request.OldNickname, request.NewNickname);
            try
            {
                var records = await _addressBookRepository.GetAllByNicknameAsync(request.OldNickname);
                foreach (var record in records)
                {
                    record.Nickname = request.NewNickname;
                    await _addressBookRepository.UpsertAsync(record);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot change nickname from {oldNickname} to {newNickname} for client {clientId}", request.OldNickname, request.NewNickname);
                throw;
            }
        }

        public async Task<OperationResponse> ApproveAsync(ApproveRequest request)
        {
            _logger.LogInformation("Setting approve for sender {senderClientId} and receiver {receiverClientId}", request.SenderClientId, request.ReceiverClientId);
            try
            {
                // var record = await _addressBookRepository.GetAsync(TODO);
                // if (record == null)
                // {
                //     _logger.LogError("Cannot find record for sender {senderClientId} and receiver {receiverClientId}", request.SenderClientId, request.ReceiverClientId);
                //     return new OperationResponse()
                //     {
                //         IsSuccess = false,
                //         ErrorMessage = "Cannot find record"
                //     };
                // }
                // record.ReceiveApprovalGranted = true;
                // await _addressBookRepository.UpsertAsync(record);
                //
                // await _contactReceivingApprovedPublisher.PublishAsync(new ContactReceivingApproved()
                // {
                //     SenderClientId = request.SenderClientId,
                //     ReceiverClientId = request.ReceiverClientId
                // });
                
                return new OperationResponse()
                {
                    IsSuccess = true
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot set approve for sender {senderClientId} and receiver {receiverClientId}", request.SenderClientId, request.ReceiverClientId);
                return new OperationResponse()
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message,
                    ErrorCode = GlobalSendErrorCode.InternalError
                };
            }
        }

        public async Task<OperationResponse> DeleteAsync(DeleteRequest request)
        {
            _logger.LogInformation("Requested delete for client {clientId} and contact {contactId}", request.OwnerClientId, request.RemoveContactClientId);
            try
            {
                await _addressBookRepository.DeleteAsync(request.OwnerClientId, request.RemoveContactClientId);
                return new OperationResponse()
                {
                    IsSuccess = true,
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot delete for client {clientId} and contact {contactId}", request.OwnerClientId, request.RemoveContactClientId);
                return new OperationResponse()
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message,
                    ErrorCode = GlobalSendErrorCode.InternalError
                };
            }
        }

        public async Task<OperationResponse> CreateWithNicknameAsync(AddNicknameRequest nicknameRequest)
        {
            _logger.LogInformation("Requested create for client {clientId} and contact {contactId}", nicknameRequest.OwnerClientId, nicknameRequest.Nickname);
            try
            {
                var client = await _clientProfileService.GetClientByNickname(new GetClientByNicknameRequest
                {
                    Nickname = nicknameRequest.Nickname
                });

                if (!client.IsExists)
                    return new OperationResponse()
                    {
                        IsSuccess = false,
                        ErrorMessage = "Client not found",
                        ErrorCode = GlobalSendErrorCode.InvalidNickname,
                    };

                var existingRecord =
                    await _addressBookRepository.GetByNicknameAsync(nicknameRequest.OwnerClientId, nicknameRequest.Nickname);
                if (existingRecord != null)
                    return new OperationResponse()
                    {
                        IsSuccess = false,
                        ErrorMessage = "Record already exists",
                        ErrorCode = GlobalSendErrorCode.NicknameAlreadyUsed,
                    };
                
                existingRecord = await _addressBookRepository.GetByNameAsync(nicknameRequest.OwnerClientId, nicknameRequest.Name);
                if (existingRecord != null)
                    return new OperationResponse()
                    {
                        IsSuccess = false,
                        ErrorMessage = "Record already exists",
                        ErrorCode = GlobalSendErrorCode.NameAlreadyUsed,
                    };
                
                
                var record = new AddressBookRecord()
                {
                    OwnerClientId = nicknameRequest.OwnerClientId, 
                    ContactId = Guid.NewGuid().ToString("N"),
                    Nickname = nicknameRequest.Nickname,
                    Name = nicknameRequest.Name,
                    Order = DateTime.UtcNow.UnixTime(),
                };

                await _addressBookRepository.UpsertAsync(record);
                
                return new OperationResponse()
                {
                    IsSuccess = true,
                    Record = record
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot create for client {clientId} and contact {contactId}", nicknameRequest.OwnerClientId, nicknameRequest.Nickname);
                return new OperationResponse()
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message,
                    ErrorCode = GlobalSendErrorCode.InternalError
                };
            }
        }

        public async Task<OperationResponse> CreateWithIbanAsync(AddIbanRequest request)
        {
            _logger.LogInformation("Requested create for client {clientId} and iban {iban}", request.OwnerClientId, request.Iban);
            try
            {
                var existingRecord =
                    await _addressBookRepository.GetByIbanAsync(request.OwnerClientId, request.Iban);
                if (existingRecord != null)
                    return new OperationResponse()
                    {
                        IsSuccess = false,
                        ErrorMessage = "Record already exists",
                        ErrorCode = GlobalSendErrorCode.IbanAlreadyUsed
                    };

                existingRecord = await _addressBookRepository.GetByNameAsync(request.OwnerClientId, request.Name);
                if (existingRecord != null)
                    return new OperationResponse()
                    {
                        IsSuccess = false,
                        ErrorMessage = "Record already exists",
                        ErrorCode = GlobalSendErrorCode.NameAlreadyUsed
                    };

                string bankName;
                string bankSwiftCode;
                if(request.IbanType == IbanType.Simple)
                {
                    var (error, ibanCheck) = await CheckIban(request.Iban);
                    if (error != GlobalSendErrorCode.OK)
                        return new OperationResponse()
                        {
                            IsSuccess = false,
                            ErrorCode = error
                        };
                    bankName = ibanCheck.BankName;
                    bankSwiftCode = ibanCheck.BankSwiftCode;
                }
                else
                {
                    var (error, ibanCheck) = await CheckIban(request.Iban);
                    if (error != GlobalSendErrorCode.OK)
                    {
                        if(error == GlobalSendErrorCode.InternalError)
                            _logger.LogError("Cannot check iban {iban}", request.Iban);
                        else
                            _logger.LogError("Iban {iban} is not valid", request.Iban);
                    }
                        
                    if (ibanCheck?.BankSwiftCode != null && ibanCheck?.BankSwiftCode != request.Bic)
                        _logger.LogError("Bic {bic} is not equal to iban {iban} bic {ibanBic}", request.Bic,
                            request.Iban, ibanCheck.BankSwiftCode);
                    if(ibanCheck?.BankName != null && ibanCheck?.BankName != request.BankName)
                        _logger.LogError(
                            "Bank name {bankName} is not equal to iban {iban} bank name {ibanBankName}",
                            request.BankName, request.Iban, ibanCheck.BankName);

                    var bicRegex = new Regex("^[A-Z]{4}[A-Z]{2}[0-9A-Z]{2}[0-9A-Z]{3}$");
                    if (!bicRegex.Match(request.Bic).Success)
                    {
                        _logger.LogError("Bic {bic} is not valid", request.Bic);
                        return new OperationResponse()
                        {
                            IsSuccess = false,
                            ErrorCode = GlobalSendErrorCode.InvalidIban
                        };
                    }
                    
                    bankName = request.BankName;
                    bankSwiftCode = request.Bic;
                }
                
                var record = new AddressBookRecord
                {
                    OwnerClientId = request.OwnerClientId,
                    ContactId = Guid.NewGuid().ToString("N"),
                    Iban = request.Iban,
                    Bic = bankSwiftCode,
                    BankName = bankName,
                    Order = DateTime.UtcNow.UnixTime(),
                    Name = request.Name,
                    BankCountry = request.BankCountry,
                    FullName = request.FullName,
                    IbanType = request.IbanType
                };

                await _addressBookRepository.UpsertAsync(record);
                
                return new OperationResponse()
                {
                    IsSuccess = true,
                    Record = record
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot create for client {clientId} and iban {iban}", request.OwnerClientId, request.Iban);
                return new OperationResponse()
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message,
                    ErrorCode = GlobalSendErrorCode.InternalError
                };
            }
        }

        public async Task<OperationResponse> UpdateContactAsync(UpdateContactRequest request)
        {
            _logger.LogInformation("Requested update for client {clientId} and contact {contactId}", request.OwnerClientId, request.ContactId);
            try
            {
                var record = await _addressBookRepository.GetAsync(request.ContactId);
                if (record == null)
                {
                    _logger.LogError("Cannot find record for client {clientId} and contact {contactId}", request.OwnerClientId, request.ContactId);
                    return new OperationResponse()
                    {
                        IsSuccess = false,
                        ErrorMessage = "Cannot find record",
                        ErrorCode = GlobalSendErrorCode.ContactNotFound
                    };
                }

                if(!string.IsNullOrWhiteSpace(request.Name) && request.Name != record.Name)
                {
                    var existing = await _addressBookRepository.GetByNameAsync(request.OwnerClientId, request.Name);
                    if (existing != null)
                    {
                        return new OperationResponse()
                        {
                            IsSuccess = false,
                            ErrorCode = GlobalSendErrorCode.NameAlreadyUsed
                        };
                    }
                    record.Name = request.Name;
                }
                
                if(!string.IsNullOrWhiteSpace(request.Iban))
                {
                    var existing = await _addressBookRepository.GetByIbanAsync(request.OwnerClientId, request.Iban);
                    if (existing != null)
                    {
                        return new OperationResponse()
                        {
                            IsSuccess = false,
                            ErrorCode = GlobalSendErrorCode.IbanAlreadyUsed
                        };
                    }
                    
                    record.Iban = request.Iban;

                    if(record.IbanType == IbanType.Simple)
                    {
                        var (error, ibanCheck) = await CheckIban(request.Iban);
                        if (error != GlobalSendErrorCode.OK)
                            return new OperationResponse()
                            {
                                IsSuccess = false,
                                ErrorCode = error
                            };

                        record.Bic = ibanCheck.BankSwiftCode;
                        record.BankName = ibanCheck.BankName;
                    }
                    else
                    {
                        var (error, ibanCheck) = await CheckIban(request.Iban);
                        if (error != GlobalSendErrorCode.OK)
                        {
                            if(error == GlobalSendErrorCode.InternalError)
                                _logger.LogError("Cannot check iban {iban}", request.Iban);
                            else
                                _logger.LogError("Iban {iban} is not valid", request.Iban);
                        }
                        
                        record.Bic = request.Bic;
                        if (ibanCheck?.BankSwiftCode != null && ibanCheck?.BankSwiftCode != request.Bic)
                            _logger.LogError("Bic {bic} is not equal to iban {iban} bic {ibanBic}", request.Bic,
                                request.Iban, ibanCheck.BankSwiftCode);

                        record.BankName = request.BankName;
                        if(ibanCheck?.BankName != null && ibanCheck?.BankName != request.BankName)
                            _logger.LogError(
                                "Bank name {bankName} is not equal to iban {iban} bank name {ibanBankName}",
                                request.BankName, request.Iban, ibanCheck.BankName);
                        
                        var bicRegex = new Regex("[A-Z]{6,6}[A-Z2-9][A-NP-Z0-9]([A-Z0-9]{3,3}){0,1}\n");
                        if (!bicRegex.Match(request.Bic).Success)
                        {
                            _logger.LogError("Bic {bic} is not valid", request.Bic);
                            return new OperationResponse()
                            {
                                IsSuccess = false,
                                ErrorCode = GlobalSendErrorCode.InvalidIban
                            };
                        }
                    }
                }
                
                if(!string.IsNullOrWhiteSpace(request.BankCountry) && request.BankCountry != record.BankCountry)
                    record.BankCountry = request.BankCountry;
                
                if(!string.IsNullOrWhiteSpace(request.FullName) && request.FullName != record.FullName)
                    record.FullName = request.FullName;

                record.Order = DateTime.UtcNow.UnixTime();
                
                await _addressBookRepository.UpsertAsync(record);
                
                return new OperationResponse()
                {
                    IsSuccess = true
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot update for client {clientId} and contact {contactId}", request.OwnerClientId, request.ContactId);
                return new OperationResponse()
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message,
                    ErrorCode = GlobalSendErrorCode.InternalError
                };
            }
        }

        public async Task<OperationResponse> GetByIdAsync(GetByIdRequest request)
        {
            _logger.LogInformation("Requested get by id for  contact {contactId}",  request.ContactId);
            try
            {
                var record = await _addressBookRepository.GetAsync(request.ContactId);
                if(record == null)
                    return new OperationResponse()
                    {
                        IsSuccess = false,
                        ErrorMessage = "Record not found",
                        ErrorCode = GlobalSendErrorCode.ContactNotFound
                    };

                return new OperationResponse()
                {
                    IsSuccess = true,
                    Record = record
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot get by id for contact {contactId}", request.ContactId);
                return new OperationResponse()
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message,
                    ErrorCode = GlobalSendErrorCode.InternalError
                };
            }
        }

        private async Task<(GlobalSendErrorCode, SepaIban)> CheckIban(string iban)
        {
            try
            {
                var ibanCheck = await _ibanService.CheckSepaIbanRequisiteAsync(new IbanInfoRequest
                {
                    Iban = iban
                });
                
                if(!ibanCheck.IsSuccess)
                    return (GlobalSendErrorCode.InvalidIban, null);
                if (!ibanCheck.Data.SepaReachable && !ibanCheck.Data.SepaInstReachable)
                    return (GlobalSendErrorCode.IbanNotReachable, null);
            
                return (GlobalSendErrorCode.OK, ibanCheck.Data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot check iban {iban}", iban);
                return (GlobalSendErrorCode.InternalError, null);
            }
        }
    }
}
