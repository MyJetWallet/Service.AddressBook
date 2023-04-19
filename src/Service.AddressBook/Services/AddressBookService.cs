using System;
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
        public AddressBookService(ILogger<AddressBookService> logger, IAddressBookRepository addressBookRepository, IClientProfileService clientProfileService, IServiceBusPublisher<ContactReceivingApproved> contactReceivingApprovedPublisher)
        {
            _logger = logger;
            _addressBookRepository = addressBookRepository;
            _clientProfileService = clientProfileService;
            _contactReceivingApprovedPublisher = contactReceivingApprovedPublisher;
        }

        public async Task<AddressBookListResponse> SearchAsync(SearchRequest request)
        {
            _logger.LogInformation("Requested search for client {clientId} with text {searchText}", request.OwnerClientId, request.SearchText);
            try
            {
                var records = await _addressBookRepository.FindAsync(request.OwnerClientId, request.SearchText);
                var topContacts = await _addressBookRepository.GetListAsync(request.OwnerClientId, 0, 5);

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
                var records = await _addressBookRepository.GetListAsync(request.OwnerClientId, request.Skip, request.Take);
                var topContacts = await _addressBookRepository.GetListAsync(request.OwnerClientId, 0, 5);
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
                var record = await _addressBookRepository.GetAsync(request.SenderClientId, request.ReceiverClientId);
                if (record == null)
                {
                    _logger.LogError("Cannot find record for sender {senderClientId} and receiver {receiverClientId}", request.SenderClientId, request.ReceiverClientId);
                    return new OperationResponse()
                    {
                        IsSuccess = false,
                        ErrorMessage = "Cannot find record"
                    };
                }
                record.ReceiveApprovalGranted = true;
                await _addressBookRepository.UpsertAsync(record);

                await _contactReceivingApprovedPublisher.PublishAsync(new ContactReceivingApproved()
                {
                    SenderClientId = request.SenderClientId,
                    ReceiverClientId = request.ReceiverClientId
                });
                
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
                    ErrorMessage = e.Message
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
                    IsSuccess = true
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot delete for client {clientId} and contact {contactId}", request.OwnerClientId, request.RemoveContactClientId);
                return new OperationResponse()
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message
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
                        ErrorMessage = "Client not found"
                    };

                var existingRecord =
                    await _addressBookRepository.GetByNicknameAsync(nicknameRequest.OwnerClientId, nicknameRequest.Nickname);
                if (existingRecord != null)
                    return new OperationResponse()
                    {
                        IsSuccess = false,
                        ErrorMessage = "Record already exists"
                    };
                
                existingRecord = await _addressBookRepository.GetByNameAsync(nicknameRequest.OwnerClientId, nicknameRequest.Name);
                if (existingRecord != null)
                    return new OperationResponse()
                    {
                        IsSuccess = false,
                        ErrorMessage = "Record already exists"
                    };
                
                
                var record = new AddressBookRecord()
                {
                    OwnerClientId = nicknameRequest.OwnerClientId, 
                    ContactClientId = client.ClientId,
                    Nickname = nicknameRequest.Nickname,
                    Name = nicknameRequest.Name,
                    Order = DateTime.UtcNow.UnixTime(),
                };

                await _addressBookRepository.UpsertAsync(record);
                
                return new OperationResponse()
                {
                    IsSuccess = true
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot create for client {clientId} and contact {contactId}", nicknameRequest.OwnerClientId, nicknameRequest.Nickname);
                return new OperationResponse()
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message
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
                        ErrorMessage = "Record already exists"
                    };
                
                existingRecord = await _addressBookRepository.GetByNameAsync(request.OwnerClientId, request.Name);
                if (existingRecord != null)
                    return new OperationResponse()
                    {
                        IsSuccess = false,
                        ErrorMessage = "Record already exists"
                    };

                var profile = await _clientProfileService.GetOrCreateProfile(new GetClientProfileRequest()
                {
                    ClientId = request.ClientId
                });
                
                var record = new AddressBookRecord
                {
                    OwnerClientId = request.OwnerClientId,
                    ContactClientId = request.ClientId,
                    Nickname = profile.Nickname,
                    Iban = request.Iban,
                    Bic = request.Bic,
                    BankName = request.BankName,
                    Order = DateTime.UtcNow.UnixTime(),
                    Name = request.Name,
                };

                await _addressBookRepository.UpsertAsync(record);
                
                return new OperationResponse()
                {
                    IsSuccess = true
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot create for client {clientId} and iban {iban}", request.OwnerClientId, request.Iban);
                return new OperationResponse()
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public async Task<OperationResponse> UpdateContactAsync(UpdateContactRequest request)
        {
            _logger.LogInformation("Requested update for client {clientId} and contact {contactId}", request.OwnerClientId, request.ContactClientId);
            try
            {
                var record = await _addressBookRepository.GetAsync(request.OwnerClientId, request.ContactClientId);
                if (record == null)
                {
                    _logger.LogError("Cannot find record for client {clientId} and contact {contactId}", request.OwnerClientId, request.ContactClientId);
                    return new OperationResponse()
                    {
                        IsSuccess = false,
                        ErrorMessage = "Cannot find record"
                    };
                }

                if(request.Name != null)
                    record.Name = request.Name;
                if(request.Iban != null)
                    record.Iban = request.Iban;
                if(request.Bic != null)
                    record.Bic = request.Bic;
                if(request.BankName != null)
                    record.BankName = request.BankName;
                
                await _addressBookRepository.UpsertAsync(record);
                
                return new OperationResponse()
                {
                    IsSuccess = true
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot update for client {clientId} and contact {contactId}", request.OwnerClientId, request.ContactClientId);
                return new OperationResponse()
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message
                };
            }
        }
    }
}
