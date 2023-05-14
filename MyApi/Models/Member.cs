namespace MyApi.Models
{
    public class Member : BaseEntity
    {
        public string FullName  { get; set; }
        public DateTime Birthdate { get; set; }
        public string? RG { get; set; }
        public string? CPF { get; set; }
        public string? Occupation { get; set; } // job or profession
    }
}
