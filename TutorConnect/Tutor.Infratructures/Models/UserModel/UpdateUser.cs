using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor.Infratructures.Models.UserModel
{
    public class UpdateUser
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DOB { get; set; }
    }
    public class UpdateTutor : UpdateUser
    {
        public string Address { get; set; }
        public string TeachingExperience { get; set; }
        public string Education { get; set; }
        public decimal? Price { get; set; }
        public string Country { get; set; }
    }
}
