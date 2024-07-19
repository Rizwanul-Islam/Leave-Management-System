using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.DTOs.LeaveType;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveTypes.Handlers.Commands;
using HR.LeaveManagement.Application.Features.LeaveTypes.Requests.Commands;
using HR.LeaveManagement.Application.Profiles;
using HR.LeaveManagement.Application.Responses;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using HR.LeaveManagement.Domain;
using Moq;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.LeaveTypes.Commands
{
    public class CreateLeaveTypeCommandHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly CreateLeaveTypeDto _leaveTypeDto;
        private readonly CreateLeaveTypeCommandHandler _handler;

        public CreateLeaveTypeCommandHandlerTests()
        {
            _mockUow = MockUnitOfWork.GetUnitOfWork();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = mapperConfig.CreateMapper();
            _handler = new CreateLeaveTypeCommandHandler(_mockUow.Object, _mapper);

            _leaveTypeDto = new CreateLeaveTypeDto
            {
                DefaultDays = 15,
                Name = "Test DTO"
            };
        }

        [Fact]
        public async Task Valid_LeaveType_Added()
        {
            // Arrange
            var command = new CreateLeaveTypeCommand { LeaveTypeDto = _leaveTypeDto };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            var leaveTypes = await _mockUow.Object.LeaveTypeRepository.GetAll();

            result.ShouldBeOfType<BaseCommandResponse>();
            result.Success.ShouldBe(true);
            result.Message.ShouldBe("Creation Successful");
            leaveTypes.Count.ShouldBe(4); // Assuming there were initially 3 leave types in Mock
        }

        [Fact]
        public async Task Invalid_LeaveType_Not_Added()
        {
            // Arrange
            _leaveTypeDto.DefaultDays = -1;
            var command = new CreateLeaveTypeCommand { LeaveTypeDto = _leaveTypeDto };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            var leaveTypes = await _mockUow.Object.LeaveTypeRepository.GetAll();

            result.ShouldBeOfType<BaseCommandResponse>();
            result.Success.ShouldBe(false);
            result.Message.ShouldBe("Creation Failed");
            leaveTypes.Count.ShouldBe(3); // No new leave type should be added
        }
    }
}
