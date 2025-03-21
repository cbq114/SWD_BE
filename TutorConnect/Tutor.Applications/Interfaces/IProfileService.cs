namespace Tutor.Applications.Interfaces
{
    public interface IProfileService
    {
        Task<List<string>> GetAllCountriesAsync();
    }
}
