namespace Smart_Server.Models
{
    public class UploadRequest
    {
        public IFormFile File { get; set; } = null!;
        public int? ProjectId { get; set; }
    }
}
