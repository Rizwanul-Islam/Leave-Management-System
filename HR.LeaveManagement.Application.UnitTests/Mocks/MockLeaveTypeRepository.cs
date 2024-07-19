using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Domain;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HR.LeaveManagement.Application.UnitTests.Mocks
{
    public static class MockLeaveTypeRepository
    {
        public static Mock<ILeaveTypeRepository> GetLeaveTypeRepository()
        {
            // Sample data
            var leaveTypes = new List<LeaveType>
            {
                new LeaveType
                {
                    Id = 1,
                    DefaultDays = 10,
                    Name = "Test Vacation"
                },
                new LeaveType
                {
                    Id = 2,
                    DefaultDays = 15,
                    Name = "Test Sick"
                },
                new LeaveType
                {
                    Id = 3,
                    DefaultDays = 15,
                    Name = "Test Maternity"
                }
            };

            var mockRepo = new Mock<ILeaveTypeRepository>();

            // Mocking GetAll method
            mockRepo.Setup(r => r.GetAll()).ReturnsAsync(leaveTypes);

            // Mocking Add method
            mockRepo.Setup(r => r.Add(It.IsAny<LeaveType>())).ReturnsAsync((LeaveType leaveType) =>
            {
                leaveTypes.Add(leaveType);
                return leaveType;
            });

            // Mocking Get method
            mockRepo.Setup(r => r.Get(It.IsAny<int>())).ReturnsAsync((int id) =>
            {
                return leaveTypes.FirstOrDefault(l => l.Id == id);
            });

            // Mocking Delete method
            mockRepo.Setup(r => r.Delete(It.IsAny<LeaveType>())).Callback<LeaveType>(leaveType =>
            {
                var itemToRemove = leaveTypes.FirstOrDefault(l => l.Id == leaveType.Id);
                if (itemToRemove != null)
                {
                    leaveTypes.Remove(itemToRemove);
                }
            });

            // Mocking Update method
            mockRepo.Setup(r => r.Update(It.IsAny<LeaveType>())).Callback<LeaveType>(leaveType =>
            {
                var itemToUpdate = leaveTypes.FirstOrDefault(l => l.Id == leaveType.Id);
                if (itemToUpdate != null)
                {
                    itemToUpdate.Name = leaveType.Name;
                    itemToUpdate.DefaultDays = leaveType.DefaultDays;
                }
            });

            return mockRepo;
        }
    }
}
