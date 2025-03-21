using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor.Infratructures.Models.UserModel
{
    public class LanguagesDTO
    {
        public int? LanguageId { get; set; }
        public string LanguageName { get; set; }
        public string Description { get; set; }
        public string? UserName { get; set; }
    }
}
