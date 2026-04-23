using FoodApp.Models;
using FoodApp.Repositories;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    /// <summary>
    /// API controller for managing email templates. Only accessible by admin users.
    /// Provides endpoints for creating, retrieving, and deleting email templates used for system communications.
    /// </summary>
    [ApiController]
    public class EmailTemplateController : Controller
    {
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailTemplateController"/> class.
        /// </summary>
        /// <param name="emailTemplateRepository">Repository for email template data operations.</param>
        /// <param name="authService">Authentication service for permission checks.</param>
        public EmailTemplateController(IEmailTemplateRepository emailTemplateRepository, IAuthService authService)
        {
            _emailTemplateRepository = emailTemplateRepository;
            _authService = authService;
        }

        /// <summary>
        /// Retrieves a single email template by identifier.
        /// </summary>
        /// <param name="identifier">Either the numeric ID or the unique name of the template.</param>
        /// <returns>The requested email template if found.</returns>
        /// <response code="200">Returns the requested email template.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <response code="404">Not Found - template with the specified identifier does not exist.</response>
        /// <remarks>
        /// Requires admin role authentication. Identifier can be either the integer ID or the template name.
        /// </remarks>
        [HttpGet("api/email-templates/{id}")]
        public IActionResult GetTemplate(int id)
        {
            _authService.CheckPermissions(Request, [Roles.admin]);
            
            var template = _emailTemplateRepository.GetTemplate(id, null);

            return Ok(template);
        }

        /// <summary>
        /// Creates a new email template.
        /// </summary>
        /// <param name="request">The template data to create.</param>
        /// <returns>The created email template with its assigned ID.</returns>
        /// <response code="200">Returns the created email template.</response>
        /// <response code="400">Bad Request - invalid template data or missing required fields.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. Template name must be unique.
        /// Template content can include placeholders in {{PlaceholderName}} format that will be replaced when sending emails.
        /// </remarks>
        [HttpPost("api/email-templates")]
        public IActionResult CreateTemplate([FromBody] CreateEmailTemplateRequest request)
        {
            _authService.CheckPermissions(Request, [Roles.admin]);

            if (string.IsNullOrWhiteSpace(request.name) || string.IsNullOrWhiteSpace(request.template))
            {
                return BadRequest(new { error = "Name and template content are required" });
            }

            var created = _emailTemplateRepository.AddTemplate(request);
            return Ok(created);
        }
    }
}