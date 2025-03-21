using Tutor.Domains.Enums;
using Tutor.Infratructures.Models.UserModel;

namespace Tutor.Infratructures.Models.UpgradeModel
{
    public class UpgraViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string DocumentUrl { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public UserDTO User { get; set; }
    }
    public class UpgradeRequestDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string DocumentUrl { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
