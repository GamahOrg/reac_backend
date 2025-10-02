namespace REAC.Application.Contracts.Dtos.Integration
{
    public class UploadComplaintFileRequestDto
    {
        public UploadComplaintFileNoteDto note { get; set; }
    }

    public class UploadComplaintFileNoteDto
    {
        public string? id { get; set; }
        public string filename { get; set; }
        public string file { get; set; }
    }

    public class UploadComplaintFileNoteResponseDto
    {
        public string noteId { get; set; }
        public string filename { get; set; }
    }
}
