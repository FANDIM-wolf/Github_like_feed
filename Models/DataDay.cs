using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq; // Для Count()

namespace ActivityPerson.Models
{
    public class DataDay
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        [Required]
        [Range(0, 4)]
        public int ActivityLevel { get; set; }

        [NotMapped]
        public IBrush Color => ActivityLevel switch
        {
            0 => Brush.Parse("#EBEDF0"),
            1 => Brush.Parse("#9BE9A8"),
            2 => Brush.Parse("#40C463"),
            3 => Brush.Parse("#30A14E"),
            4 => Brush.Parse("#216E39"),
            _ => Brush.Parse("#EBEDF0")
        };

        [NotMapped]
        public IBrush BorderColor => Brushes.Black;

        // Навигационное свойство для Tasks
        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

        // Свойство для отображения количества задач
        [NotMapped]
        public int TaskCount => Tasks.Count;
    }
}