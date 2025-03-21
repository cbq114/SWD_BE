using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor.Infratructures.Models.UserModel
{
    public class AddCertification
    {
        [Required]
        public string CertificationName { get; set; }
        [Required]
        public IFormFile Certification { get; set; }
    }
    public class UpdateCertification
    {
        public string CertificationName { get; set; }
        public IFormFile Certification { get; set; }
    }
}
