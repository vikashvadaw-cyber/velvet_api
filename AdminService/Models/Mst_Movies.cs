using System.ComponentModel.DataAnnotations;

namespace AdminService.Models
{
    public class Mst_Movies
    {
        [Key]
        public int MovieId { get; set; }

        public string MovieName { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string MoviePath { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }
    }
}
