using SocialNode.Mvc.Models;
using System.ComponentModel.DataAnnotations;

namespace SocialNode.Mvc.ViewModels
{
    public class RegisterViewModel : User
    {
        [Required]
        [Display(Name = "Login")]
        [EmailAddress(ErrorMessage = "Informe um endereço de email válido.")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar senha")]
        [Compare("Password", ErrorMessage = "A senha e a confirmação devem ser iguais.")]
        public string ConfirmPassword { get; set; }
    }
}
