using HR.LeaveManagement.Application.Contracts.Persistence;
using Moq;
using System.Threading.Tasks;

namespace HR.LeaveManagement.Application.UnitTests.Mocks
{
    public static class MockUnitOfWork
    {
        public static Mock<IUnitOfWork> GetUnitOfWork(
            Mock<ILeaveTypeRepository> mockLeaveTypeRepository = null,
            Mock<ILeaveAllocationRepository> mockLeaveAllocationRepository = null,
            Mock<ILeaveRequestRepository> mockLeaveRequestRepository = null)
        {
            var mockUow = new Mock<IUnitOfWork>();

            mockLeaveTypeRepository ??= MockLeaveTypeRepository.GetLeaveTypeRepository();

            // Mocking the repositories
            mockUow.Setup(u => u.LeaveTypeRepository).Returns(mockLeaveTypeRepository.Object);
            mockUow.Setup(u => u.LeaveAllocationRepository).Returns(mockLeaveAllocationRepository?.Object);
            mockUow.Setup(u => u.LeaveRequestRepository).Returns(mockLeaveRequestRepository?.Object);

            // Mocking Save method to simply complete the task
            mockUow.Setup(u => u.Save()).Returns(Task.CompletedTask);

            return mockUow;
        }
    }
}
