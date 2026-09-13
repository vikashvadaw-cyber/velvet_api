using System.ComponentModel.DataAnnotations;

namespace AdminService.Models
{
    public class Mst_Users
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public DateTime ? CreatedOn {  get; set; }
        public DateTime ? UpdatedOn { get; set; }

        public DateTime ? LastLogin { get; set; }
        public string ? RefreshToken { get; set; }
        public DateTime ? RefreshTokenExpiry { get; set; }
    }

    
}
