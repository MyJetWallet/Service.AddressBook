using System.Collections.Generic;
using System.Threading.Tasks;
using Service.AddressBook.Domain.Models;

namespace Service.AddressBook.Domain;

public interface IAddressBookRepository
{
    Task<AddressBookRecord> GetAsync(string contactId);
    Task<AddressBookRecord> GetByNicknameAsync(string ownerClientId, string nickname);
    Task<AddressBookRecord> GetByNameAsync(string ownerClientId, string name);
    Task<AddressBookRecord> GetByIbanAsync(string ownerClientId, string iban, IbanType type);

    Task<List<AddressBookRecord>> GetListAsync(string ownerClientId, int skip, int take, bool withIban,
        bool withNickname, IbanType? requestIbanType = null);
    
    Task UpsertAsync(AddressBookRecord record);
    Task DeleteAsync(string ownerClientId, string clientId);
    
    Task<List<AddressBookRecord>> FindAsync(string ownerClientId, string searchText,bool withIban, bool withNickname, IbanType? requestIbanType = null);
    Task<List<AddressBookRecord>> GetAllByNicknameAsync(string nickname);
}