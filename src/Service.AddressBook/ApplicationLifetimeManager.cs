using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;
using Service.AddressBook.Services;

namespace Service.AddressBook
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly ServiceBusLifeTime _serviceBusLifeTime;
        private readonly AddressBookRepositoryMemoryCache _addressBookRepositoryMemoryCache;

        public ApplicationLifetimeManager(IHostApplicationLifetime appLifetime, ILogger<ApplicationLifetimeManager> logger, ServiceBusLifeTime serviceBusLifeTime, AddressBookRepositoryMemoryCache addressBookRepositoryMemoryCache)
            : base(appLifetime)
        {
            _logger = logger;
            _serviceBusLifeTime = serviceBusLifeTime;
            _addressBookRepositoryMemoryCache = addressBookRepositoryMemoryCache;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
            _serviceBusLifeTime.Start();
            _addressBookRepositoryMemoryCache.Start().GetAwaiter().GetResult();
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}
