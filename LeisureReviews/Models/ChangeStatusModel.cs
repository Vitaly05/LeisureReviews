using LeisureReviews.Data;
using System.ComponentModel.DataAnnotations;

namespace LeisureReviews.Models
{
    public class ChangeStatusModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public AccountStatus Status { get; set; }
    }
}
