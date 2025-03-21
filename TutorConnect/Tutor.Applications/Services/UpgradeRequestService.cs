using Tutor.Applications.Interfaces;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.Authen;
using Tutor.Infratructures.Models.UpgradeModel;

namespace Tutor.Applications.Services
{
    public class UpgradeRequestService : IUpgradeRequestService
    {
        private readonly IUpgradeRequestRepository _repository;

        public UpgradeRequestService(IUpgradeRequestRepository repository)
        {
            _repository = repository;
        }

        public Task<bool> ApproveRequest(int requestId)
        {
            return _repository.ApproveRequest(requestId);
        }

        public Task<string> CreateUpgradeRequest(string username, UpgradeToInstructorModel model)
        {
            return _repository.CreateUpgradeRequest(username, model);
        }

        public Task<List<UpgradeRequestDto>> GetPendingRequests()
        {
            return _repository.GetPendingRequests();
        }

        public Task<UpgradeRequest> GetRequestById(int requestId)
        {
            return _repository.GetRequestById(requestId);
        }

        public Task<bool> RejectRequest(int requestId, string reason)
        {
            return _repository.RejectRequest(requestId, reason);
        }
        public Task<List<UpgradeRequestDto>> GetAllRequests()
        {
            return _repository.GetAllRequests();
        }
    }
}
