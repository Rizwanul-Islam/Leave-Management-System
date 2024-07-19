using HR.LeaveManagement.Application.DTOs.LeaveAllocation;
using HR.LeaveManagement.Application.Features.LeaveAllocations.Requests.Commands;
using HR.LeaveManagement.Application.Features.LeaveAllocations.Requests.Queries;
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
    public class LeaveAllocationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<LeaveAllocationsController> _logger;

        public LeaveAllocationsController(IMediator mediator, ILogger<LeaveAllocationsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a list of leave allocations.
        /// Logs the request and response details.
        /// </summary>
        /// <param name="isLoggedInUser">Flag to determine if the list should only include allocations for the logged-in user.</param>
        /// <returns>A list of LeaveAllocationDto objects.</returns>
        [HttpGet]
        public async Task<ActionResult<List<LeaveAllocationDto>>> Get(bool isLoggedInUser = false)
        {
            try
            {
                _logger.LogInformation("Fetching leave allocations. IsLoggedInUser: {IsLoggedInUser}", isLoggedInUser);

                var leaveAllocations = await _mediator.Send(new GetLeaveAllocationListRequest { IsLoggedInUser = isLoggedInUser });

                _logger.LogInformation("Fetched {Count} leave allocations.", leaveAllocations.Count);

                return Ok(leaveAllocations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching leave allocations. IsLoggedInUser: {IsLoggedInUser}", isLoggedInUser);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Retrieves the details of a specific leave allocation by ID.
        /// Logs the request and response details.
        /// </summary>
        /// <param name="id">The ID of the leave allocation.</param>
        /// <returns>The details of the leave allocation.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveAllocationDto>> Get(int id)
        {
            try
            {
                _logger.LogInformation("Fetching leave allocation details for ID: {Id}", id);

                var leaveAllocation = await _mediator.Send(new GetLeaveAllocationDetailRequest { Id = id });

                if (leaveAllocation == null)
                {
                    _logger.LogWarning("Leave allocation with ID: {Id} not found.", id);
                    return NotFound();
                }

                _logger.LogInformation("Fetched leave allocation details for ID: {Id}", id);

                return Ok(leaveAllocation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching leave allocation details for ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Creates a new leave allocation.
        /// Logs the request and response details.
        /// </summary>
        /// <param name="leaveAllocation">The leave allocation to be created.</param>
        /// <returns>A response containing the result of the operation.</returns>
        [HttpPost]
        public async Task<ActionResult<BaseCommandResponse>> Post([FromBody] CreateLeaveAllocationDto leaveAllocation)
        {
            try
            {
                _logger.LogInformation("Creating new leave allocation with details: {@LeaveAllocation}", leaveAllocation);

                var command = new CreateLeaveAllocationCommand { LeaveAllocationDto = leaveAllocation };
                var response = await _mediator.Send(command);

                _logger.LogInformation("Created new leave allocation with response: {@Response}", response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new leave allocation with details: {@LeaveAllocation}", leaveAllocation);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Updates an existing leave allocation.
        /// Logs the request and response details.
        /// </summary>
        /// <param name="leaveAllocation">The leave allocation with updated details.</param>
        /// <returns>No content if successful, otherwise an error response.</returns>
        [HttpPut]
        public async Task<ActionResult> Put([FromBody] UpdateLeaveAllocationDto leaveAllocation)
        {
            try
            {
                _logger.LogInformation("Updating leave allocation with details: {@LeaveAllocation}", leaveAllocation);

                var command = new UpdateLeaveAllocationCommand { LeaveAllocationDto = leaveAllocation };
                await _mediator.Send(command);

                _logger.LogInformation("Updated leave allocation with details: {@LeaveAllocation}", leaveAllocation);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating leave allocation with details: {@LeaveAllocation}", leaveAllocation);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Deletes a specific leave allocation by ID.
        /// Logs the request and response details.
        /// </summary>
        /// <param name="id">The ID of the leave allocation to be deleted.</param>
        /// <returns>No content if successful, otherwise an error response.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Deleting leave allocation with ID: {Id}", id);

                var command = new DeleteLeaveAllocationCommand { Id = id };
                await _mediator.Send(command);

                _logger.LogInformation("Deleted leave allocation with ID: {Id}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting leave allocation with ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
