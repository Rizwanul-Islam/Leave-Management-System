using HR.LeaveManagement.Application.DTOs.LeaveRequest;
using HR.LeaveManagement.Application.Features.LeaveRequests.Requests.Commands;
using HR.LeaveManagement.Application.Features.LeaveRequests.Requests.Queries;
using HR.LeaveManagement.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
    public class LeaveRequestsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<LeaveRequestsController> _logger;

        public LeaveRequestsController(IMediator mediator, ILogger<LeaveRequestsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a list of leave requests.
        /// Logs the request and response details.
        /// </summary>
        /// <param name="isLoggedInUser">Flag to determine if the list should only include requests for the logged-in user.</param>
        /// <returns>A list of LeaveRequestListDto objects.</returns>
        [HttpGet]
        public async Task<ActionResult<List<LeaveRequestListDto>>> Get(bool isLoggedInUser = false)
        {
            try
            {
                _logger.LogInformation("Fetching leave requests. IsLoggedInUser: {IsLoggedInUser}", isLoggedInUser);

                var leaveRequests = await _mediator.Send(new GetLeaveRequestListRequest { IsLoggedInUser = isLoggedInUser });

                _logger.LogInformation("Fetched {Count} leave requests.", leaveRequests.Count);

                return Ok(leaveRequests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching leave requests. IsLoggedInUser: {IsLoggedInUser}", isLoggedInUser);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Retrieves the details of a specific leave request by ID.
        /// Logs the request and response details.
        /// </summary>
        /// <param name="id">The ID of the leave request.</param>
        /// <returns>The details of the leave request.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveRequestDto>> Get(int id)
        {
            try
            {
                _logger.LogInformation("Fetching leave request details for ID: {Id}", id);

                var leaveRequest = await _mediator.Send(new GetLeaveRequestDetailRequest { Id = id });

                if (leaveRequest == null)
                {
                    _logger.LogWarning("Leave request with ID: {Id} not found.", id);
                    return NotFound();
                }

                _logger.LogInformation("Fetched leave request details for ID: {Id}", id);

                return Ok(leaveRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching leave request details for ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Creates a new leave request.
        /// Logs the request and response details.
        /// </summary>
        /// <param name="leaveRequest">The leave request to be created.</param>
        /// <returns>A response containing the result of the operation.</returns>
        [HttpPost]
        public async Task<ActionResult<BaseCommandResponse>> Post([FromBody] CreateLeaveRequestDto leaveRequest)
        {
            try
            {
                _logger.LogInformation("Creating new leave request with details: {@LeaveRequest}", leaveRequest);

                var command = new CreateLeaveRequestCommand { LeaveRequestDto = leaveRequest };
                var response = await _mediator.Send(command);

                _logger.LogInformation("Created new leave request with response: {@Response}", response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new leave request with details: {@LeaveRequest}", leaveRequest);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Updates an existing leave request.
        /// Logs the request and response details.
        /// </summary>
        /// <param name="id">The ID of the leave request to be updated.</param>
        /// <param name="leaveRequest">The leave request with updated details.</param>
        /// <returns>No content if successful, otherwise an error response.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] UpdateLeaveRequestDto leaveRequest)
        {
            try
            {
                _logger.LogInformation("Updating leave request with ID: {Id} and details: {@LeaveRequest}", id, leaveRequest);

                var command = new UpdateLeaveRequestCommand { Id = id, LeaveRequestDto = leaveRequest };
                await _mediator.Send(command);

                _logger.LogInformation("Updated leave request with ID: {Id}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating leave request with ID: {Id} and details: {@LeaveRequest}", id, leaveRequest);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Changes the approval status of a leave request.
        /// Logs the request and response details.
        /// </summary>
        /// <param name="id">The ID of the leave request to change approval status.</param>
        /// <param name="changeLeaveRequestApproval">The details of the approval change.</param>
        /// <returns>No content if successful, otherwise an error response.</returns>
        [HttpPut("changeapproval/{id}")]
        public async Task<ActionResult> ChangeApproval(int id, [FromBody] ChangeLeaveRequestApprovalDto changeLeaveRequestApproval)
        {
            try
            {
                _logger.LogInformation("Changing approval status of leave request with ID: {Id} to: {@ChangeLeaveRequestApproval}", id, changeLeaveRequestApproval);

                var command = new UpdateLeaveRequestCommand { Id = id, ChangeLeaveRequestApprovalDto = changeLeaveRequestApproval };
                await _mediator.Send(command);

                _logger.LogInformation("Changed approval status of leave request with ID: {Id}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while changing approval status of leave request with ID: {Id} to: {@ChangeLeaveRequestApproval}", id, changeLeaveRequestApproval);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Deletes a specific leave request by ID.
        /// Logs the request and response details.
        /// </summary>
        /// <param name="id">The ID of the leave request to be deleted.</param>
        /// <returns>No content if successful, otherwise an error response.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Deleting leave request with ID: {Id}", id);

                var command = new DeleteLeaveRequestCommand { Id = id };
                await _mediator.Send(command);

                _logger.LogInformation("Deleted leave request with ID: {Id}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting leave request with ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
