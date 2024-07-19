using AutoMapper;
using HR.LeaveManagement.Application.DTOs.LeaveRequest.Validators;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveAllocations.Requests.Commands;
using HR.LeaveManagement.Application.Features.LeaveRequests.Requests.Commands;
using HR.LeaveManagement.Application.Features.LeaveTypes.Requests.Commands;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Domain;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HR.LeaveManagement.Application.Features.LeaveRequests.Handlers.Commands
{
    /// <summary>
    /// Handler for updating a leave request.
    /// </summary>
    public class UpdateLeaveRequestCommandHandler : IRequestHandler<UpdateLeaveRequestCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateLeaveRequestCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the update of a leave request, including updating details or changing approval status.
        /// </summary>
        /// <param name="request">The leave request command containing update details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<Unit> Handle(UpdateLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            // Retrieve the leave request to be updated
            var leaveRequest = await _unitOfWork.LeaveRequestRepository.Get(request.Id);

            // Check if the leave request exists
            if (leaveRequest == null)
                throw new NotFoundException(nameof(leaveRequest), request.Id);

            // Update leave request details
            if (request.LeaveRequestDto != null)
            {
                var validator = new UpdateLeaveRequestDtoValidator(_unitOfWork.LeaveTypeRepository);
                var validationResult = await validator.ValidateAsync(request.LeaveRequestDto);

                // Validate request details
                if (!validationResult.IsValid)
                    throw new ValidationException(validationResult);

                // Map updated details to the leave request
                _mapper.Map(request.LeaveRequestDto, leaveRequest);
                await _unitOfWork.LeaveRequestRepository.Update(leaveRequest);
            }
            // Handle leave request approval status change
            else if (request.ChangeLeaveRequestApprovalDto != null)
            {
                await _unitOfWork.LeaveRequestRepository.ChangeApprovalStatus(leaveRequest, request.ChangeLeaveRequestApprovalDto.Approved);
                if (request.ChangeLeaveRequestApprovalDto.Approved)
                {
                    // Update leave allocation if approved
                    var allocation = await _unitOfWork.LeaveAllocationRepository.GetUserAllocations(leaveRequest.RequestingEmployeeId, leaveRequest.LeaveTypeId);
                    int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;
                    allocation.NumberOfDays -= daysRequested;
                    await _unitOfWork.LeaveAllocationRepository.Update(allocation);
                }
            }

            await _unitOfWork.Save();
            return Unit.Value;
        }
    }
}
