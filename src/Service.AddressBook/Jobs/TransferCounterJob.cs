using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using Service.AddressBook.Domain;
using Service.InternalTransfer.Domain.Models;

namespace Service.AddressBook.Jobs
{
    public class TransferCounterJob
    {
        private readonly ILogger<TransferCounterJob> _logger;
        private readonly IAddressBookRepository _addressBookRepository;

        public TransferCounterJob(ILogger<TransferCounterJob> logger, ISubscriber<Transfer> subscriber, IAddressBookRepository addressBookRepository)
        {
            _logger = logger;
            _addressBookRepository = addressBookRepository;
            subscriber.Subscribe(HandleTransfer);
        }

        private async ValueTask HandleTransfer(Transfer transfer)
        {
            // if(transfer.Status != TransferStatus.Completed || transfer.TransferType != TransferType.NicknameTransfer)
            //     return;
            //
            // var record = await _addressBookRepository.GetAsync(TODO);
            // if(record == null)
            //     return;
            //
            // _logger.LogInformation("Increment transfer counter for {clientId} -> {destinationClientId}", transfer.ClientId, transfer.DestinationClientId);
            // record.TransfersCount++;
            // await _addressBookRepository.UpsertAsync(record);
        }
    }
}