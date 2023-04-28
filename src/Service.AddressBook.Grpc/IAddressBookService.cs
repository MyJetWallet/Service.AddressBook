using System.ServiceModel;
using System.Threading.Tasks;
using Service.AddressBook.Domain.Models;
using Service.AddressBook.Grpc.Models;

namespace Service.AddressBook.Grpc
{
    [ServiceContract]
    public interface IAddressBookService
    {
        [OperationContract]
        Task<AddressBookListResponse> SearchAsync(SearchRequest request);
        
        [OperationContract]
        Task<AddressBookRecord> GetByNicknameAsync(GetByNicknameRequest request);

        [OperationContract]
        Task<AddressBookListResponse> GetRecordsAsync(GetListRequest request);
        
        [OperationContract]
        Task ChangeNickname(UpdateNicknameRequest request);
        
        [OperationContract]
        Task<OperationResponse> ApproveAsync(ApproveRequest request);
        
        [OperationContract]
        Task<OperationResponse> DeleteAsync(DeleteRequest request);
        
        [OperationContract]
        Task<OperationResponse> CreateWithNicknameAsync(AddNicknameRequest nicknameRequest);
        
        [OperationContract]
        Task<OperationResponse> CreateWithIbanAsync(AddIbanRequest request);
        
        [OperationContract]
        Task<OperationResponse> UpdateContactAsync(UpdateContactRequest request);
        
        [OperationContract]
        Task<OperationResponse> GetByIdAsync(GetByIdRequest request);
    }
}