using System.ComponentModel.DataAnnotations;

namespace MyApi.Models.DTOs
{
    public class MemberDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "O nome não deve exceder {1} caracteres.")]
        [Display(Name = "Nome completo")]
        public required string FullName { get; set; }

        [Required]
        [Display(Name = "Data de Nascimento")]
        public DateTime Birthdate { get; set; }


        public string? RG { get; set; }
        public string? CPF { get; set; }

        [Display(Name = "Profissão")]
        public string? Occupation { get; set; }
    }
}
