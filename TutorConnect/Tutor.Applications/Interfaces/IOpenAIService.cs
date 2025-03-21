namespace Tutor.Applications.Interfaces
{
    public interface IOpenAIService
    {
        Task<string> GenerateResponse(string prompt);
    }
}
