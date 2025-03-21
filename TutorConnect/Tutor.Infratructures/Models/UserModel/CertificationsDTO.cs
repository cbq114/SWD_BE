using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor.Infratructures.Models.UserModel
{
    public class CertificationsDTO
    {
        public int CertificationId { get; set; }
        public string CertificationName { get; set; }
        public string CertificationFile { get; set; }
        public string Type { get; set; }
        public int ProfileId { get; set; }
    }
}
