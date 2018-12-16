using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace configure.Pages
{
    public class TemplatesModel : PageModel
    {
        private readonly IAmazonSimpleEmailService _emailService;

        public TemplatesModel(IAmazonSimpleEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task OnGetAsync(CancellationToken ct)
        {
            var request = new ListTemplatesRequest();
            var response = await _emailService.ListTemplatesAsync(request, ct);

            EmailTemplates = new List<EmailTemplate>();

            foreach (var template in response.TemplatesMetadata)
            {
                EmailTemplates.Add(new EmailTemplate
                {
                    Name = template.Name,
                    Created = template.CreatedTimestamp
                });
            }
        }

        public List<EmailTemplate> EmailTemplates { get; set; }
    }
}
