using System.Collections.Generic;
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
    public class TemplatesModel : PageModel
    {
        private readonly IAmazonSimpleEmailService _emailService;

        public TemplatesModel(IAmazonSimpleEmailService emailService)
        {
            _emailService = emailService;
            EmailTemplates = new List<EmailTemplateViewModel>();
        }

        public async Task<IActionResult> OnGetDeleteAsync(
            [Required(AllowEmptyStrings = false)]
            [StringLength(255)]
            [DataType(DataType.Text)]
            string templateName,
            CancellationToken ct = default(CancellationToken))
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var request = new DeleteTemplateRequest { TemplateName = templateName };
            var response = await _emailService.DeleteTemplateAsync(request, ct);

            if (!response.HttpStatusCode.Equals(HttpStatusCode.OK))
                return BadRequest();

            return Page();
        }

        public async Task OnGetAsync(CancellationToken ct = default(CancellationToken))
        {
            var request = new ListTemplatesRequest();
            var response = await _emailService.ListTemplatesAsync(request, ct);

            foreach (var template in response.TemplatesMetadata)
            {
                EmailTemplates.Add(new EmailTemplateViewModel
                {
                    Name = template.Name,
                    Created = template.CreatedTimestamp
                });
            }
        }

        public List<EmailTemplateViewModel> EmailTemplates { get; set; }
    }
}
