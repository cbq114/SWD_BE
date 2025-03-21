using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tutor.Domains.Entities;

public class Certifications
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CertificationId { get; set; }
    [Required]
    [MaxLength(100)]
    public string CertificationName { get; set; }
    [Required]
    public string CertificationFile { get; set; }
    [Required]
    [MaxLength(50)]
    public string Type { get; set; }
    public int ProfileId { get; set; }

    [ForeignKey("ProfileId")]
    public Profile profile { get; set; }
}