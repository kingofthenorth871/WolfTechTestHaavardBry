using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfTechTestHaavardBry.Controllers;
using WolfTechTestHaavardBry.Models;
using WolfTechTestHaavardBry.Services;
using Xunit;

namespace ProjectTests
{
    public class DepartmentControllerTests
    {
        [Fact]
        public async Task GetDepartmentHierarchy_Returns_Departments_When_Service_Returns_Data()
        {
            // Arrange
            var mockService = new Mock<IDepartmentService>();
            var expectedDepartments = new List<Department> { };
            mockService.Setup(service => service.GetDepartmentHierarchyAsync(It.IsAny<string>())).ReturnsAsync(expectedDepartments);
            var controller = new DepartmentController(mockService.Object);
            var filePath = "C:/Users/howar/Downloads/filForWolftech.txt"; // This path has to be where your file is located on your hard drive and it has to contain an equal amount of departments as set up in the test

            // Act
            var result = await controller.GetDepartmentHierarchy(filePath);

            // Assert
            var okResult = Assert.IsType<JsonResult>(result.Result);
            Assert.IsAssignableFrom<List<Department>>(okResult.Value);

        }

        [Fact]
        public async Task GetDepartmentHierarchy_Returns_InternalServerError_When_Service_Throws_Exception()
        {
            // Arrange
            var mockService = new Mock<IDepartmentService>();
            mockService.Setup(service => service.GetDepartmentHierarchyAsync(It.IsAny<string>())).ThrowsAsync(new Exception("Test exception"));
            var controller = new DepartmentController(mockService.Object);
            var filePath = "C:/Users/howar/Downloads/filForWolftech.txt"; // This path has to be where your file is located on your hard drive and it has to contain an equal amount of departments as set up in the test
            // Act
            var result = await controller.GetDepartmentHierarchy(filePath);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Test exception", statusCodeResult.Value.ToString());
        }
    }
}
