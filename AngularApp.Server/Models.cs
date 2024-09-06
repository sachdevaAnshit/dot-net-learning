namespace AngularApp.Server
{
    public class User
    {
        public int? employeeID { get; set; }
        public string? firstName { get; set; }
        public string? lastName{ get; set; }
        public int? age { get; set; }
        public string? managerName { get; set; }
        public int? managerID { get; set; }
        public int? salary { get; set; }
    }

    public class AuthenticationInput
    {
        public string? UserName { get; set; }

        public string? Password { get; set; }

    }

    public class AuthenticationResponse
    {
        public string? message { get; set; }

        public string? token { get; set; }

    }
}
