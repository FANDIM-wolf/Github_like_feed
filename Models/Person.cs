using System.ComponentModel.DataAnnotations;

namespace ActivityPerson.Models
{
    public class Person
    {
        public int Id { get; set; }

        [Required]
        public string PersonName { get; set; } = string.Empty;

        public int PersonAge { get; set; }
    }
}