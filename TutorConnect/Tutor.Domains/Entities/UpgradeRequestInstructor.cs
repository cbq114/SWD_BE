using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Tutor.Domains.Enums;
using Tutor.Shared.Helper;

namespace Tutor.Domains.Entities
{
    public class UpgradeRequest
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string DocumentUrl { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public string reason { get; set; }
        public DateTime RequestedAt { get; set; } = DateTimeHelper.GetVietnamNow();
        [ForeignKey("UserName")]
        [JsonIgnore]
        public Users User { get; set; }
    }
}
