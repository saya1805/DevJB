using System.ComponentModel.DataAnnotations.Schema;
using DevJBackend.Model;

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
        public List<UserCourseDetails> userCourse { get; set; } = new List<UserCourseDetails>();
        [NotMapped]
        public int courseCount => userCourse != null ? userCourse.Count : 0;

    }

    public class UserCourseDetails
    {
        public int id { get; set; }
        public int crsId { get; set; }
        public string Coursename { get; set; } = string.Empty;
        public string Course_status { get; set; } = string.Empty;
        public string Course_fee { get; set; } = string.Empty;
        public int course_days { get; set; }
        public string payment_mode { get; set; } = string.Empty;
        public string payment_status { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
    }

}
