using AutoMapper;
using HR.LeaveManagement.Application.DTOs;
using HR.LeaveManagement.Application.DTOs.LeaveRequest;
using HR.LeaveManagement.Application.Features.LeaveRequests.Requests.Queries;
using HR.LeaveManagement.Application.Features.LeaveTypes.Requests;
using HR.LeaveManagement.Application.Features.LeaveTypes.Requests.Queries;
using HR.LeaveManagement.Application.Contracts.Persistence;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using HR.LeaveManagement.Application.Contracts.Identity;

namespace HR.LeaveManagement.Application.Features.LeaveRequests.Handlers.Queries
{
    /// <summary>
    /// Handler for retrieving detailed information about a leave request.
    /// </summary>
    public class GetLeaveRequestDetailRequestHandler : IRequestHandler<GetLeaveRequestDetailRequest, LeaveRequestDto>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public GetLeaveRequestDetailRequestHandler(
            ILeaveRequestRepository leaveRequestRepository,
            IMapper mapper,
            IUserService userService)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _mapper = mapper;
            _userService = userService;
        }

        /// <summary>
        /// Handles the request to get detailed information about a specific leave request.
        /// </summary>
        /// <param name="request">The query request containing the leave request ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation, with a result of LeaveRequestDto.</returns>
        public async Task<LeaveRequestDto> Handle(GetLeaveRequestDetailRequest request, CancellationToken cancellationToken)
        {
            // Retrieve the leave request with its details
            var leaveRequest = await _leaveRequestRepository.GetLeaveRequestWithDetails(request.Id);

            // Map the leave request entity to the DTO
            var leaveRequestDto = _mapper.Map<LeaveRequestDto>(leaveRequest);

            // Retrieve and set the employee details in the DTO
            leaveRequestDto.Employee = await _userService.GetEmployee(leaveRequest.RequestingEmployeeId);

            return leaveRequestDto;
        }
    }
}
