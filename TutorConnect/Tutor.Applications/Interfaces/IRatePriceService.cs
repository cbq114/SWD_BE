namespace Tutor.Applications.Interfaces
{
    public interface IRatePriceService
    {
        Task<bool> UpdateRatePriceAsync(double newRate);
        double GetCurrentRatePrice();
    }
}
