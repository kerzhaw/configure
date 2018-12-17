using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
            string templateName, CancellationToken ct = default(CancellationToken)
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

        private async Task CreateTemplate(Template template, CancellationToken ct = default(CancellationToken))
        {
            var request = new CreateTemplateRequest
            {
                Template = template
            };

            await _emailService.CreateTemplateAsync(request, ct);
        }

        private async Task UpdateTemplate(Template template, CancellationToken ct = default(CancellationToken))
        {
            var request = new UpdateTemplateRequest
            {
                Template = template
            };

            await _emailService.UpdateTemplateAsync(request, ct);
        }

        public async Task<IActionResult> OnGetAsync(CancellationToken ct = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(TemplateName))
                CreateMode = true;

            if (CreateMode)
                return Page();

            var template = await GetTemplateAsync(TemplateName, ct);

            if (template.HttpStatusCode.Equals(HttpStatusCode.NotFound))
                return NotFound();

            SubjectPart = template.Template.SubjectPart;
            HtmlPart = template.Template.HtmlPart;
            TextPart = template.Template.TextPart;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken ct = default(CancellationToken))
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var template = new Template
            {
                TemplateName = TemplateName,
                SubjectPart = SubjectPart,
                HtmlPart = HtmlPart,
                TextPart = TextPart
            };

            if (CreateMode)
                await CreateTemplate(template, ct);
            else
                await UpdateTemplate(template, ct);

            return RedirectToPage("/Templates");
        }

        [BindProperty]
        [HiddenInput(DisplayValue = false)]
        public bool CreateMode { get; set; }

        [Required(AllowEmptyStrings = false)]
        [BindProperty(SupportsGet = true)]
        [StringLength(255)]
        [DataType(DataType.Text)]
        [Display(Name = "Template name", Prompt = "Template name")]
        public string TemplateName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [BindProperty]
        [StringLength(255)]
        [DataType(DataType.Text)]
        [Display(Name = "Subject", Prompt = "Subject")]
        public string SubjectPart { get; set; }

        [Required(AllowEmptyStrings = false)]
        [BindProperty]
        [StringLength(24990000)]
        [DataType(DataType.Html)]
        [Display(Name = "HTML version", Prompt = "<h1>Hello {{name}},</h1><p>Bye!.</p>")]
        public string HtmlPart { get; set; }

        [Required(AllowEmptyStrings = false)]
        [BindProperty]
        [StringLength(24990000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Text-only version", Prompt = "Hello {{name}}, Bye!")]
        public string TextPart { get; set; }
    }
}