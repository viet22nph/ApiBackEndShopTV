
namespace Models.DTOs.ContactUs
{
    public class ContactUsDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ContactContent { get; set; }
        public DateTime DateCreate { get; set; }
        public bool IsReply { get; set; }
    }
}
