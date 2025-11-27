using Microsoft.AspNetCore.Http;

namespace SmartServer.DTOs
{
    public class UploadRequest
    {
        public IFormFile File { get; set; } = null!;
        public int? ProjectId { get; set; }
    }
}
