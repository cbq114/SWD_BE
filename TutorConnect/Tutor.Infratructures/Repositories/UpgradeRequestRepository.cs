using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.Authen;
using Tutor.Infratructures.Models.UpgradeModel;
using Tutor.Infratructures.Persistence;
using Tutor.Shared.Helper;

namespace Tutor.Infratructures.Repositories
{
    public class UpgradeRequestRepository : IUpgradeRequestRepository
    {
        private readonly IConfiguration _configuration;
        private readonly TutorDBContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IWalletRepository _walletRepository;

        public UpgradeRequestRepository(
            IConfiguration configuration,
            TutorDBContext context,
            IMapper mapper,
            IEmailSender emailSender,
            IWalletRepository walletRepository)
        {
            _configuration = configuration;
            _context = context;
            _mapper = mapper;
            _emailSender = emailSender;
            _walletRepository = walletRepository;
        }

        public async Task<string> CreateUpgradeRequest(string username, UpgradeToInstructorModel model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
                return "User not found";

            if (user.RoleId == 2)
                return "User is already an instructor";

            var existingRequest = await _context.upgradeRequests
                .AnyAsync(r => r.UserName == username && r.Status == RequestStatus.Pending);

            if (existingRequest)
                return "You already have a pending upgrade request";

            string documentUrl = null;
            if (model.Document != null)
            {
                var cloudinaryService = new CloundinaryRepository(_configuration);
                documentUrl = await cloudinaryService.UploadImage(model.Document) ?? "null docs";
            }

            var profile = new Domains.Entities.Profile
            {
                UserName = username,
                Address = model.Address,
                Price = model.Price,
                LanguageId = model.LanguageId,
                TeachingExperience = model.TeachingExperience,
                Country = model.Country,
                Education = model.Education,
                TutorStatus = TutorStatus.NotAvaliable
            };

            var request = new UpgradeRequest
            {
                UserName = username,
                DocumentUrl = documentUrl,
                Status = RequestStatus.Pending,
                reason = "I want",
                RequestedAt = DateTimeHelper.GetVietnamNow()
            };

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Profile.AddAsync(profile);
                await _context.upgradeRequests.AddAsync(request);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return "Upgrade request submitted successfully";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return $"Error: {ex.Message}";
            }
        }

        public async Task<List<UpgradeRequestDto>> GetPendingRequests()
        {
            var pendingRequests = await _context.upgradeRequests
                .Include(r => r.User)
                .Where(r => r.Status == RequestStatus.Pending)
                .OrderBy(r => r.RequestedAt)
                .ToListAsync();

            return _mapper.Map<List<UpgradeRequestDto>>(pendingRequests);
        }

        public async Task<UpgradeRequest> GetRequestById(int requestId)
        {
            return await _context.upgradeRequests
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == requestId);
        }

        public async Task<bool> ApproveRequest(int requestId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var request = await _context.upgradeRequests
                    .Include(r => r.User)
                    .FirstOrDefaultAsync(r => r.Id == requestId);

                if (request == null || request.Status != RequestStatus.Pending)
                    return false;

                // Update request status
                request.Status = RequestStatus.Approved;

                // Update user role to instructor
                request.User.RoleId = 2;

                // Update profile status
                var profile = await _context.Profile.FirstOrDefaultAsync(p => p.UserName == request.UserName);
                if (profile != null)
                {
                    profile.TutorStatus = TutorStatus.Avaliable;
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Send approval notification email
                await SendApprovalEmail(request.User.Email, request.User.UserName);

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error approving request: {ex.Message}");
            }
        }

        public async Task<bool> RejectRequest(int requestId, string reason)
        {
            try
            {
                var request = await _context.upgradeRequests
                    .Include(r => r.User)
                    .FirstOrDefaultAsync(r => r.Id == requestId);

                if (request == null || request.Status != RequestStatus.Pending)
                    return false;

                request.Status = RequestStatus.Rejected;
                request.reason = reason;
                await _context.SaveChangesAsync();

                // Send rejection notification email
                await SendRejectionEmail(request.User.Email, request.User.UserName, reason);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error rejecting request: {ex.Message}");
            }
        }

        public async Task<List<UpgradeRequestDto>> GetAllRequests()
        {
            var pendingRequests = await _context.upgradeRequests
                .Include(r => r.User).Where(r => r.Status != RequestStatus.Pending)
                .OrderBy(r => r.RequestedAt)
                .ToListAsync();
            return _mapper.Map<List<UpgradeRequestDto>>(pendingRequests);
        }

        // Private method to send approval email
        private async Task<bool> SendApprovalEmail(string email, string username)
        {
            string subject = "Your Instructor Upgrade Request Has Been Approved";
            string message = GetApprovalEmailBody(username);
            return await _emailSender.EmailSendAsync(email, subject, message);
        }

        // Private method to send rejection email
        private async Task<bool> SendRejectionEmail(string email, string username, string reason)
        {
            string subject = "Your Instructor Upgrade Request Status";
            string message = GetRejectionEmailBody(username, reason);
            return await _emailSender.EmailSendAsync(email, subject, message);
        }

        // Email body for approval
        private string GetApprovalEmailBody(string username)
        {
            return string.Format(@"
    <div style='text-align: center; font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
        <div style='max-width: 500px; margin: auto; background: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
            <h1 style='color: #333;'>Congratulations, <span style='color: #008CBA;'>{0}</span>!</h1>
            <p style='font-size: 16px; color: #555;'>Your request to become an instructor has been approved.</p>
            <p style='font-size: 16px; color: #555;'>You now have access to instructor features and can start creating courses and connecting with students.</p>
            <a href='{1}' style='display: inline-block; 
                                 text-decoration: none;
                                 background-color: #008CBA;
                                 color: #ffffff;
                                 font-size: 18px;
                                 font-weight: bold;
                                 padding: 12px 24px;
                                 border-radius: 8px;
                                 box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                                 transition: background-color 0.3s ease;'
               onmouseover='this.style.backgroundColor=""#0077A8""'
               onmouseout='this.style.backgroundColor=""#008CBA""'>
                Go To Dashboard
            </a>
            <p style='margin-top: 20px; font-size: 14px; color: #777;'>Thank you for joining our instructor community!</p>
        </div>
    </div>", username, _configuration["Host:https"] + "/dashboard");
        }

        // Email body for rejection
        private string GetRejectionEmailBody(string username, string reason)
        {
            return string.Format(@"
    <div style='text-align: center; font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
        <div style='max-width: 500px; margin: auto; background: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
            <h1 style='color: #333;'>Hello, <span style='color: #008CBA;'>{0}</span></h1>
            <p style='font-size: 16px; color: #555;'>We've reviewed your instructor upgrade request and unfortunately, we are unable to approve it at this time.</p>
            <div style='background-color: #f5f5f5; border-left: 4px solid #008CBA; padding: 15px; margin: 20px 0; text-align: left;'>
                <p style='margin: 0; color: #555;'><strong>Reason:</strong> {1}</p>
            </div>
            <p style='font-size: 16px; color: #555;'>You're welcome to submit a new request after addressing the issues mentioned above.</p>
            <a href='{2}' style='display: inline-block; 
                                 text-decoration: none;
                                 background-color: #008CBA;
                                 color: #ffffff;
                                 font-size: 18px;
                                 font-weight: bold;
                                 padding: 12px 24px;
                                 border-radius: 8px;
                                 box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                                 transition: background-color 0.3s ease;'
               onmouseover='this.style.backgroundColor=""#0077A8""'
               onmouseout='this.style.backgroundColor=""#008CBA""'>
                Submit New Request
            </a>
            <p style='margin-top: 20px; font-size: 14px; color: #777;'>If you have any questions, please contact our support team.</p>
        </div>
    </div>", username, reason, "http://localhost:5173/admin/approved");
        }
    }
}