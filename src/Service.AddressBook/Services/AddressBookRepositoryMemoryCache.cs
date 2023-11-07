using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.AddressBook.Domain;
using Service.AddressBook.Domain.Models;
using Service.AddressBook.Postgres;

namespace Service.AddressBook.Services
{
    public class AddressBookRepositoryMemoryCache : IAddressBookRepository
    {
        private readonly ILogger<AddressBookRepositoryMemoryCache> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        private static HashSet<AddressBookRecord> _records = new HashSet<AddressBookRecord>();

        public AddressBookRepositoryMemoryCache(ILogger<AddressBookRepositoryMemoryCache> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
        }

        private static bool IsStarted { get; set; }

        public async Task<AddressBookRecord> GetAsync(string contactId)
        {
            return _records.FirstOrDefault(t => t.ContactId == contactId);
        }

        public async Task<AddressBookRecord> GetByNicknameAsync(string ownerClientId, string nickname)
        {
            return _records.FirstOrDefault(t => t.OwnerClientId == ownerClientId && t.Nickname == nickname);
        }

        public async Task<AddressBookRecord> GetByNameAsync(string ownerClientId, string name)
        {
            return _records.FirstOrDefault(t => t.OwnerClientId == ownerClientId && t.Name == name);
        }

        public async Task<AddressBookRecord> GetByIbanAsync(string ownerClientId, string iban)
        {
            return _records.FirstOrDefault(t => t.OwnerClientId == ownerClientId && t.Iban == iban);
        }

        public async Task<List<AddressBookRecord>> GetListAsync(string ownerClientId, int skip, int take, bool withIban,
            bool withNickname, IbanType? requestIbanType = null)
        {
            if (!IsStarted)
                return new List<AddressBookRecord>();

            var records = _records.Where(e => e.OwnerClientId == ownerClientId).ToList();

            if (withNickname)
                records = records.Where(t => !string.IsNullOrEmpty(t.Nickname)).ToList();

            if (withIban)
            {
                records = records.Where(t => !string.IsNullOrEmpty(t.Iban)).ToList();
                if (requestIbanType == IbanType.Simple)
                {
                    records = records.Where(t => t.IbanType == IbanType.Simple).ToList();
                }

                if (requestIbanType == IbanType.Personal)
                {
                    records = records.Where(t => t.IbanType is IbanType.Personal).ToList();
                }
            }

            records = records
                .OrderByDescending(t => t.Order)
                .Skip(skip)
                .Take(take)
                .ToList();

            return records;
        }

        public async Task UpsertAsync(AddressBookRecord record)
        {
            record.LastTs = DateTime.UtcNow;

            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            await context.UpsertAsync(new[] {record});

            var oldRecord = _records.FirstOrDefault(e =>
                e.OwnerClientId == record.OwnerClientId && e.ContactId == record.ContactId);
            if (oldRecord != null)
                _records.Remove(oldRecord);

            _records.Add(record);
        }

        public async Task DeleteAsync(string ownerClientId, string clientId)
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var record =
                await context.AddressBook.FirstOrDefaultAsync(e =>
                    e.OwnerClientId == ownerClientId && e.ContactId == clientId);
            if (record != null)
                context.AddressBook.Remove(record);

            await context.SaveChangesAsync();

            var oldRecord = _records.FirstOrDefault(e => e.OwnerClientId == ownerClientId && e.ContactId == clientId);
            if (oldRecord != null)
                _records.Remove(oldRecord);
        }

        public async Task<List<AddressBookRecord>> FindAsync(string ownerClientId, string searchText, bool withIban,
            bool withNickname, IbanType? requestIbanType = null)
        {
            if (!IsStarted)
                return new List<AddressBookRecord>();

            var records = _records.Where(e =>
                    e.OwnerClientId == ownerClientId)
                .ToList();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                records = records.Where(e =>
                    (e.Name?.Contains(searchText, StringComparison.InvariantCultureIgnoreCase) ?? false) ||
                    (e.Iban?.Contains(searchText, StringComparison.InvariantCultureIgnoreCase) ?? false)||
                    (e.Nickname?.Contains(searchText, StringComparison.InvariantCultureIgnoreCase) ?? false)).ToList();
            }
            if (withIban)
            {
                records = records.Where(e => !string.IsNullOrWhiteSpace(e.Iban)).ToList();
                if (requestIbanType == IbanType.Simple)
                {
                    records = records.Where(t => t.IbanType == IbanType.Simple).ToList();
                }

                if (requestIbanType == IbanType.Personal)
                {
                    records = records.Where(t => t.IbanType is IbanType.Personal).ToList();
                }
            }

            if (withNickname)
                records = records.Where(e => !string.IsNullOrWhiteSpace(e.Nickname)).ToList();

            return records;
        }

        public async Task<List<AddressBookRecord>> GetAllByNicknameAsync(string nickname)
        {
            if (!IsStarted)
                return new List<AddressBookRecord>();

            return _records.Where(t => t.Nickname == nickname).ToList();
        }

        public async Task Start()
        {
            _logger.LogInformation("Updating in-memory cache for address book");
            var sw = Stopwatch.StartNew();
            var limit = 100;
            var offset = 0;
            List<AddressBookRecord> response;
            do
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                response = await context.AddressBook
                    .Skip(offset)
                    .Take(limit)
                    .ToListAsync();

                _records.UnionWith(response);
                offset += response.Count;
            } while (response.Any());

            sw.Stop();
            _logger.LogInformation("Updated in-memory cache for address book in {time} ms", sw.ElapsedMilliseconds);
            IsStarted = true;
        }
    }
}