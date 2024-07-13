namespace Models.DTOs.Account
{
    public class AuthenticationRequest
    {
        public string  UserNameOrEmail { get; set; }
        public string Password { get; set; }
    }
}