namespace DevJBackend.Entity
{
    public class User
    {
        public Guid Id { get; set; }
        public string? fullname { get; set; }
        public string? username { get; set; }
        public string? mailid { get; set; }
        public string? PasswordHash { get; set; }
        public string? ProfilePictureUrl { get; set; }

        public string? Roles { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
