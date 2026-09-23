namespace StudyFlow.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        //Not encrypted password 
        public string PasswordHash { get; set; } = string.Empty;
    }
}
