namespace MuseuDeBugs.Api.DTOs.Auth
{
    public class MeResponse
    {
        public string Username { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; }
        public string[] Roles { get; set; } = [];
    }
}