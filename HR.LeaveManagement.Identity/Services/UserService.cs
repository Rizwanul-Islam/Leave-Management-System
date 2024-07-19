using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Identity;
using HR.LeaveManagement.Application.Models.Identity;
using HR.LeaveManagement.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HR.LeaveManagement.Identity.Services
{
    // Service for handling user-related operations
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor to initialize UserManager dependency
        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Method to retrieve employee details by user ID
        public async Task<Employee> GetEmployee(string userId)
        {
            // Find user by ID
            var employee = await _userManager.FindByIdAsync(userId);

            // Map ApplicationUser properties to Employee object and return
            return new Employee
            {
                Email = employee.Email,
                Id = employee.Id,
                Firstname = employee.FirstName,
                Lastname = employee.LastName
            };
        }

        // Method to retrieve a list of all employees
        public async Task<List<Employee>> GetEmployees()
        {
            // Get users in the "Employee" role
            var employees = await _userManager.GetUsersInRoleAsync("Employee");

            // Map each ApplicationUser to an Employee object and return as a list
            return employees.Select(q => new Employee
            {
                Id = q.Id,
                Email = q.Email,
                Firstname = q.FirstName,
                Lastname = q.LastName
            }).ToList();
        }
    }
}
