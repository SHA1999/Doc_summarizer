namespace Doc_summarizer.Models;

public class DocumentSummarizationRequest
{
    public string? Name { get; set; }          // e.g., "bank_statement"
    public string? Base64Pdf { get; set; }     // base64-encoded PDF file
}
