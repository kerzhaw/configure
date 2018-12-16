using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace configure.Pages
{
    public class TemplateModel : PageModel
    {
        private readonly IAmazonSimpleEmailService _emailService;

        public TemplateModel(IAmazonSimpleEmailService emailService)
        {
            _emailService = emailService;
        }

        private async Task<GetTemplateResponse> GetTemplateAsync(
            string templateName, CancellationToken ct
        )
        {
            var request = new GetTemplateRequest { TemplateName = templateName };

            try
            {
                return (await _emailService.GetTemplateAsync(request, ct));
            }
            catch (TemplateDoesNotExistException)
            {
                return new GetTemplateResponse
                {
                    HttpStatusCode = HttpStatusCode.NotFound
                };
            }
        }

        public async Task<IActionResult> OnGetAsync(CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var template = await GetTemplateAsync(TemplateName, ct);

            if (template.HttpStatusCode.Equals(HttpStatusCode.NotFound))
            {
                return NotFound();
            }

            SubjectPart = template.Template.SubjectPart;
            HtmlPart = template.Template.HtmlPart;
            TextPart = template.Template.TextPart;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken ct)
        {
            return Page();
        }

        [BindProperty(SupportsGet = true)]
        [Required(AllowEmptyStrings = false)]
        public string TemplateName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(255)]
        [DataType(DataType.Text)]
        [Display(Name = "Subject", Prompt = "Subject")]
        public string SubjectPart { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(24990000)]
        [DataType(DataType.Html)]
        [Display(Name = "HTML version", Prompt = "<h1>Hello {{name}},</h1><p>Bye!.</p>")]
        public string HtmlPart { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(24990000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Text-only version", Prompt = "Hello {{name}}, Bye!")]
        public string TextPart { get; set; }
    }
}