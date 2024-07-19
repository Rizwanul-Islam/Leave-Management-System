using HR.LeaveManagement.Application.DTOs.LeaveType;
using HR.LeaveManagement.Application.Features.LeaveTypes.Requests.Commands;
using HR.LeaveManagement.Application.Features.LeaveTypes.Requests.Queries;
using HR.LeaveManagement.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HR.LeaveManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LeaveTypesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<LeaveTypesController> _logger;

        public LeaveTypesController(IMediator mediator, IHttpContextAccessor httpContextAccessor, ILogger<LeaveTypesController> logger)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a list of leave types.
        /// Logs the request and response details.
        /// </summary>
        /// <returns>A list of LeaveTypeDto objects.</returns>
        [HttpGet]
        public async Task<ActionResult<List<LeaveTypeDto>>> Get()
        {
            try
            {
                _logger.LogInformation("Fetching leave types.");

                var leaveTypes = await _mediator.Send(new GetLeaveTypeListRequest());

                _logger.LogInformation("Fetched {Count} leave types.", leaveTypes.Count);

                return Ok(leaveTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching leave types.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Retrieves the details of a specific leave type by ID.
        /// Logs the request and response details.
        /// </summary>
        /// <param name="id">The ID of the leave type.</param>
        /// <returns>The details of the leave type.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveTypeDto>> Get(int id)
        {
            try
            {
                _logger.LogInformation("Fetching leave type details for ID: {Id}", id);

                var leaveType = await _mediator.Send(new GetLeaveTypeDetailRequest { Id = id });

                if (leaveType == null)
                {
                    _logger.LogWarning("Leave type with ID: {Id} not found.", id);
                    return NotFound();
                }

                _logger.LogInformation("Fetched leave type details for ID: {Id}", id);

                return Ok(leaveType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching leave type details for ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Creates a new leave type.
        /// Logs the request and response details.
        /// </summary>
        /// <param name="leaveType">The leave type to be created.</param>
        /// <returns>A response containing the result of the operation.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<BaseCommandResponse>> Post([FromBody] CreateLeaveTypeDto leaveType)
        {
            try
            {
                _logger.LogInformation("Creating new leave type with details: {@LeaveType}", leaveType);

                var command = new CreateLeaveTypeCommand { LeaveTypeDto = leaveType };
                var response = await _mediator.Send(command);

                _logger.LogInformation("Created new leave type with response: {@Response}", response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new leave type with details: {@LeaveType}", leaveType);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Updates an existing leave type.
        /// Logs the request and response details.
        /// </summary>
        /// <param name="id">The ID of the leave type to be updated.</param>
        /// <param name="leaveType">The leave type with updated details.</param>
        /// <returns>No content if successful, otherwise an error response.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Put(int id, [FromBody] LeaveTypeDto leaveType)
        {
            try
            {
                _logger.LogInformation("Updating leave type with ID: {Id} and details: {@LeaveType}", id, leaveType);

                var command = new UpdateLeaveTypeCommand { LeaveTypeDto = leaveType };
                await _mediator.Send(command);

                _logger.LogInformation("Updated leave type with ID: {Id}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating leave type with ID: {Id} and details: {@LeaveType}", id, leaveType);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Deletes a specific leave type by ID.
        /// Logs the request and response details.
        /// </summary>
        /// <param name="id">The ID of the leave type to be deleted.</param>
        /// <returns>No content if successful, otherwise an error response.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Deleting leave type with ID: {Id}", id);

                var command = new DeleteLeaveTypeCommand { Id = id };
                await _mediator.Send(command);

                _logger.LogInformation("Deleted leave type with ID: {Id}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting leave type with ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
