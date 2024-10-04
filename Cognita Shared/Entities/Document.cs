namespace Cognita_Shared.Entities {
    public class Document {
        public int DocumentId { get; set; }
        public string DocumentName { get; set; }
        public DateTime UploadingTime { get; set; }
        public string FilePath { get; set; }
    }
}
