using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tutor.Domains.Entities
{
    public class Languagues
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LanguageId { get; set; }
        public string LanguageName { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }

        [ForeignKey("UserName")]
        public Users User { get; set; }
    }
}
