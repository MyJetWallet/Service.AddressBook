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
        Task<OperationResponse> CreateAsync(AddRequest request);
    }
}