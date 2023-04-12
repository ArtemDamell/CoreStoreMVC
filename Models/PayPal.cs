using System.IO;
using Microsoft.Extensions.Configuration;

namespace CoreStoreMVC.Models
{
    public class PayPal
    {
        public static PayPalConfig GetPayPalConfig()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                                 .AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            return new PayPalConfig()
            {
                AuthToken = configuration["PayPal:AuthToken"],
                PostUrl = configuration["PayPal:PostUrl"],
                Business = configuration["PayPal:Business"],
            };
        }
    }
}
