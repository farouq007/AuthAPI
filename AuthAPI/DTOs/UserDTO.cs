namespace AuthAPI.DTOs
{
    public class UserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastLogin { get; set; }
    }
}