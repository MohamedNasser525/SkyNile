using System.Text.Json.Serialization;

namespace SkyNile.HelperModel
{
    public class AuthPassenger
    {
        public Guid? Id { get; set; }
        public string? Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpiresOn { get; set; }

    }
}
