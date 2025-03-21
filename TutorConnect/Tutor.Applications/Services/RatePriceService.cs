using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Globalization;
using Tutor.Applications.Interfaces;
using Tutor.Infratructures.Persistence;

namespace Tutor.Applications.Services
{

    public class RatePriceService : IRatePriceService
    {
        private readonly IConfiguration _configuration;
        private readonly string _configPath;
        private readonly TutorDBContext _context;

        public RatePriceService(IConfiguration configuration, TutorDBContext context)
        {
            _configuration = configuration;
            _configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            _context = context;
        }
        public double GetCurrentRatePrice()
        {
            var rateString = _configuration["RatePrice:rate"];
            if (double.TryParse(rateString, NumberStyles.Any, CultureInfo.InvariantCulture, out double rate))
            {
                return rate;
            }
            return 1.0;
        }

        public async Task<bool> UpdateRatePriceAsync(double newRate)
        {
            return await UpdateRateAsync("RatePrice", newRate);
        }
        private async Task<bool> UpdateRateAsync(string rateType, double newRate)
        {
            try
            {
                var json = await File.ReadAllTextAsync(_configPath);
                var jsonObj = JObject.Parse(json);

                jsonObj[rateType]["rate"] = newRate.ToString(CultureInfo.InvariantCulture);

                await File.WriteAllTextAsync(_configPath, jsonObj.ToString());

                ((IConfigurationRoot)_configuration).Reload();


                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
