using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SocialNode.Mvc.Models
{
    public class User : IdentityUser<Guid>
    {
        [Display(Name = "Login")]
        public override string UserName { get; set; }

        [Display(Name = "Nome Completo")]
        [MaxLength(100)]
        [Required]
        public string CompleteName { get; set; }
    }
}
