using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.AddressBook.Settings
{
    public class SettingsModel
    {
        [YamlProperty("AddressBook.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("AddressBook.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("AddressBook.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }
        
        [YamlProperty("AddressBook.SpotServiceBusHostPort")]
        public string  SpotServiceBusHostPort{ get; set; } 
        
        [YamlProperty("AddressBook.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }
        
        [YamlProperty("AddressBook.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }
        
        [YamlProperty("AddressBook.ClientProfileGrpcServiceUrl")]
        public string ClientProfileGrpcServiceUrl { get; set; }
        
        [YamlProperty("AddressBook.ClearjunctionGrpcServiceUrl")]
        public string ClearjunctionGrpcServiceUrl { get; set; }
    }
}
