using LeisureReviews.Data;
using System.ComponentModel.DataAnnotations;

namespace LeisureReviews.Models
{
    public class ChangeRoleModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public Roles Role { get; set; }
    }
}
