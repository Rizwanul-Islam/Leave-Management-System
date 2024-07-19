using HR.LeaveManagement.Application.Contracts.Identity;
using HR.LeaveManagement.Application.Models.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HR.LeaveManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authenticationService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthService authenticationService, ILogger<AccountController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        /// <summary>
        /// Handles login requests by authenticating the user.
        /// Logs the request and response details.
        /// </summary>
        /// <param name="request">The authentication request containing email and password.</param>
        /// <returns>An ActionResult with AuthResponse if successful, otherwise an error response.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(AuthRequest request)
        {
            try
            {
                _logger.LogInformation("Received login request with email: {Email}", request.Email);

                // Process the login request
                var response = await _authenticationService.Login(request);

                _logger.LogInformation("Login response: {Response}", response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log the exception details
                _logger.LogError(ex, "Failed: login request for email: {Email}", request.Email);

                // Return a generic error message
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Handles registration requests by creating a new user account.
        /// Logs the request and response details.
        /// </summary>
        /// <param name="request">The registration request containing user details.</param>
        /// <returns>An ActionResult with RegistrationResponse if successful, otherwise an error response.</returns>
        [HttpPost("register")]
        public async Task<ActionResult<RegistrationResponse>> Register(RegistrationRequest request)
        {
            try
            {
                _logger.LogInformation("Received registration request for email: {Email}", request.Email);

                // Process the registration request
                var response = await _authenticationService.Register(request);

                _logger.LogInformation("Registration response: {Response}", response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log the exception details
                _logger.LogError(ex, "Failed: registration request for email: {Email}", request.Email);

                // Return a generic error message
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
