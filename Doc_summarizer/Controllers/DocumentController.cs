using Doc_summarizer.Models;
using Doc_summarizer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Doc_summarizer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpPost("summarize")]
        public async Task<IActionResult> Summarize([FromBody] DocumentSummarizationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Base64Pdf) || string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Both 'name' and 'base64Pdf' are required.");

            var summary = await _documentService.SummarizeDocumentAsync(request.Base64Pdf, request.Name);
            return Ok(new { Summary = summary });
        }
    }
}
