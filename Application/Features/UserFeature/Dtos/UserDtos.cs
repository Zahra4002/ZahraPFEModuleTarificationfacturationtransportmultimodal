namespace Application.Features.UserFeature.Dtos
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        //Enumeration public Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastLoginDate { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime LockOutEnd { get; set; }
        protected string RefreshToken { get; set; }
        protected DateTime RefreshTokenExpiryTime { get; set; }
    }
}
