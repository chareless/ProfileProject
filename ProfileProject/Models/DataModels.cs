namespace ProfileProject.Models
{
    public class DataModels
    {
        public class LoginModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class ChangePasswordModel
        {
            public string? Username { get; set; }
            public string OldPassword { get; set; }
            public string Password { get; set; }
        }


    }
}
