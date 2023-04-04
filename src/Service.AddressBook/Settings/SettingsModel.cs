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
    }
}
