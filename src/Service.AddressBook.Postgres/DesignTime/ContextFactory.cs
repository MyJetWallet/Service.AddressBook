using MyJetWallet.Sdk.Postgres;
using Service.AddressBook.Postgres;

namespace Service.BuyCryptoProcessor.Postgres.DesignTime
{
    public class ContextFactory : MyDesignTimeContextFactory<DatabaseContext>
    {
        public ContextFactory() : base(options => new DatabaseContext(options))
        {

        }
    }
}