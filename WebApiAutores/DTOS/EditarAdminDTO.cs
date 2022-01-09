using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTOS
{
    public class EditarAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
