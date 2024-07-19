
using System.ComponentModel.DataAnnotations;


namespace Models.DTOs.ContactUs.Request
{
    public class ContactUsRequestDto
    {
        
        [Required]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Phone]
        public string Phone { get; set; }
        [Required]
        public string ContactContent { get; set; }

    }
}
