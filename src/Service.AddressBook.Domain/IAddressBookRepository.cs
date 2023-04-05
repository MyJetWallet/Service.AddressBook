using System.Collections.Generic;
using System.Threading.Tasks;
using Service.AddressBook.Domain.Models;

namespace Service.AddressBook.Domain;

public interface IAddressBookRepository
{
    Task<AddressBookRecord> GetAsync(string ownerClientId, string clientId);
    Task<AddressBookRecord> GetByNicknameAsync(string ownerClientId, string nickname);
    Task<AddressBookRecord> GetByNameAsync(string ownerClientId, string name);
    
    Task<List<AddressBookRecord>> GetListAsync(string ownerClientId, int skip, int take);
    
    Task UpsertAsync(AddressBookRecord record);
    Task DeleteAsync(string ownerClientId, string clientId);
    
    Task<List<AddressBookRecord>> FindAsync(string ownerClientId, string searchText);
    Task<List<AddressBookRecord>> GetAllByNicknameAsync(string nickname);

}