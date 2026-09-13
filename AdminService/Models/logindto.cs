namespace AdminService.Models
{
   
    public class LoginRequest
    {
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;

    }
    public class RefreshTokenReqDto
    {
        public int UserId { get; set; }
        public string refreshtoken { get; set; }
    }

    public class TokenresponseDto
    {
        public int userid { get; set; }
        public string username { get; set; }
        public int roleid { get; set; }
        public string email { get; set; }
        public string accesstoke { get; set; }
        public string refreshtoken { get; set; }
    }
}
