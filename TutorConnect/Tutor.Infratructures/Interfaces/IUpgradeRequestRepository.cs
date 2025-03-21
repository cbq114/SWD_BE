using Tutor.Domains.Entities;
using Tutor.Infratructures.Models.Authen;
using Tutor.Infratructures.Models.UpgradeModel;

namespace Tutor.Infratructures.Interfaces
{
    public interface IUpgradeRequestRepository
    {
        Task<string> CreateUpgradeRequest(string username, UpgradeToInstructorModel model);
        Task<List<UpgradeRequestDto>> GetPendingRequests();
        Task<bool> ApproveRequest(int requestId);
        Task<bool> RejectRequest(int requestId, string reason);
        Task<UpgradeRequest> GetRequestById(int requestId);
        Task<List<UpgradeRequestDto>> GetAllRequests();
    }
}
