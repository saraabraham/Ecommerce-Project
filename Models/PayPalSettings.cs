using System;
namespace ElectronicsStoreMVC.Models
{
    public class PayPalSettings
    {
        public string ClientId { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}

