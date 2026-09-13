using System.ComponentModel.DataAnnotations;

namespace AdminService.Models
{
    public class Mst_Menu
    {
        [Key]
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public int? MainMenuId { get; set; }
        public int? SubMenuId { get; set; }
        public string RoutePath { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
}
