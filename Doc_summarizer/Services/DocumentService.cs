using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Doc_summarizer.Models;
using System.Text.Json;
using UglyToad.PdfPig;
using Microsoft.Extensions.Options;

namespace Doc_summarizer.Services
{
    public interface IDocumentService
    {
        Task<string> SummarizeDocumentAsync(string base64Pdf, string name);
    }

    public class DocumentService : IDocumentService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAIOptions _openAIOptions;
        public DocumentService(HttpClient httpClient, IOptions<OpenAIOptions> options)
        {
            _httpClient = httpClient;
            _openAIOptions = options.Value;
        }
        public async Task<string> SummarizeDocumentAsync(string base64Pdf, string name)
        {
            var pdfBytes = Convert.FromBase64String(base64Pdf);
            var extractedText = ExtractTextFromPdf(pdfBytes);

            var prompt = BuildPrompt(extractedText, name);

            var summary = await CallGptApiAsync(prompt); 
            return summary;
        }

        private string ExtractTextFromPdf(byte[] pdfBytes)
        {
            using var pdfStream = new MemoryStream(pdfBytes);
            using var pdf = PdfDocument.Open(pdfStream);
            var sb = new StringBuilder();

            foreach (var page in pdf.GetPages())
            {
                sb.AppendLine(page.Text);
            }

            return sb.ToString();
        }

        private string BuildPrompt(string extractedText, string name)
        {
            return $"You are a financial analyst. Summarize the key information from this {name}:\n\n{extractedText}";
        }

        private async Task<string> CallGptApiAsync(string prompt)
        {
            var requestBody = new
            {
                model = "tinyllama",
                prompt = prompt,
                stream = false,
                options = new { num_predict = 200 }
            };

            using var http = new HttpClient();
            var response = await http.PostAsJsonAsync("http://localhost:11434/api/generate", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to call DeepSeek locally.");
            }

            var result = await response.Content.ReadFromJsonAsync<DeepSeekResponse>();
            return result?.Response!;
        }
        public class DeepSeekResponse
        {
            public string? Response { get; set; }
        }
    }
    }
