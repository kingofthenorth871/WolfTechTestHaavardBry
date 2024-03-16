using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Reflection;
using WolfTechTestHaavardBry.Controllers;
using WolfTechTestHaavardBry.Models;
using WolfTechTestHaavardBry.Services;
using Xunit;

namespace ProjectTests
{   

    public class DepartmentServiceUnitTests
    {
        [Fact]
        public void ReadFileAndParseIntoDepartments_Returns_Correct_Departments()
        {
            // Arrange
            var filePath = "C:/Users/howar/Downloads/filForWolftech.txt"; // This path has to be where your file is located on your hard drive and it has to contain an equal amount of departments as set up in the test
            var expectedDepartments = new List<Department>
        {
            new Department { OID = 1, Title = "US News", Color = "#F52612", DepartmentParent_OID = null },
            new Department { OID = 2, Title = "Crime + Justice", Color = "#F52612", DepartmentParent_OID = 1 },
            new Department { OID = 3, Title = "Energy + Environment", Color = "#F52612", DepartmentParent_OID = 1 },
            new Department { OID = 4, Title = "Extreme Weather", Color = "#F52612", DepartmentParent_OID = 1 },
            new Department { OID = 5, Title = "Space + Science", Color = "#F52612", DepartmentParent_OID = 1 },
            new Department { OID = 6, Title = "International News", Color = "#EB5F25", DepartmentParent_OID = null },
            new Department { OID = 7, Title = "Africa", Color = "#EB5F25", DepartmentParent_OID = 6 },
            new Department { OID = 8, Title = "Americas", Color = "#EB5F25", DepartmentParent_OID = 6 },
            new Department { OID = 9, Title = "Asia", Color = "#EB5F25", DepartmentParent_OID = 6 },
            new Department { OID = 10, Title = "Europe", Color = "#EB5F25", DepartmentParent_OID = 6 },
            // Add more expected departments as needed
        };

            var service = new DepartmentService();

            // Act           

            var methodInfo = GetPrivateMethod<DepartmentService>("readFileAndParseIntoDepartments");
            var actualDepartments = methodInfo.Invoke(service, new object[] { filePath }) as List<Department>;

            // Assert
            Assert.Equal(expectedDepartments.Count, actualDepartments.Count);
            for (int i = 0; i < expectedDepartments.Count; i++)
            {
                Assert.Equal(expectedDepartments[i].OID, actualDepartments[i].OID);
                Assert.Equal(expectedDepartments[i].Title, actualDepartments[i].Title);
                Assert.Equal(expectedDepartments[i].Color, actualDepartments[i].Color);
                Assert.Equal(expectedDepartments[i].DepartmentParent_OID, actualDepartments[i].DepartmentParent_OID);               
            }
        }

        [Fact]
        public void FilterAwayDuplicatesAndKeepRootDepartments_Returns_RootDepartments()
        {
            // Arrange
            var departments = new List<Department>
        {
            new Department { OID = 1, DepartmentParent_OID = null },
            new Department { OID = 2, DepartmentParent_OID = 1 },
            new Department { OID = 3, DepartmentParent_OID = null },
            new Department { OID = 4, DepartmentParent_OID = 3 }
        };

            var service = new DepartmentService();

            // Act
            var methodInfo = GetPrivateMethod<DepartmentService>("filterAwayDuplicatesAndKeepRootDepartments");
            var filteredDepartments = methodInfo.Invoke(service, new object[] { departments }) as List<Department>;

            // Assert
            Assert.Equal(2, filteredDepartments.Count); // There are 2 root departments
            Assert.Contains(departments[0], filteredDepartments); // First department is a root department
            Assert.Contains(departments[2], filteredDepartments); // Third department is a root department
        }

        [Fact]
        public void FilterAwayDuplicatesAndKeepRootDepartments_Returns_EmptyList_If_No_RootDepartments()
        {
            // Arrange
            var departments = new List<Department>
        {
            new Department { OID = 1, DepartmentParent_OID = 2 },
            new Department { OID = 2, DepartmentParent_OID = 3 },
            new Department { OID = 3, DepartmentParent_OID = 4 }
        };

            var service = new DepartmentService();

            // Act
            var methodInfo = GetPrivateMethod<DepartmentService>("filterAwayDuplicatesAndKeepRootDepartments");
            List<Department>? filteredDepartments = new List<Department>();
            filteredDepartments = methodInfo.Invoke(service, new object[] { departments }) as List<Department>;

            // Assert
            Assert.Empty(filteredDepartments); // No root departments, so the list should be empty
        }

        private MethodInfo GetPrivateMethod<T>(string methodName)
        {
            var type = typeof(T);
            var method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (method == null)
            {
                throw new ArgumentException($"Method '{methodName}' not found.");
            }
            return method;
        }

        [Fact]
        public void CalculateDescendantCount_Returns_Zero_For_Empty_Department()
        {
            // Arrange
            var department = new Department();
            var departments = new List<Department>();

            var service = new DepartmentService();

            // Act
            var descendantCount = GetPrivateMethod<DepartmentService>("CalculateDescendantCount")
                .Invoke(service, new object[] { department, departments });           

            // Assert
            Assert.Equal(0, descendantCount);
        }

        [Fact]
        public void CalculateDescendantCount_Returns_Correct_Count_For_Department_With_Descendants()
        {
            // Arrange
            var department1 = new Department { OID = 1, Departments = new List<Department>() };
            var department2 = new Department { OID = 2, Departments = new List<Department>() };
            var department3 = new Department { OID = 3, Departments = new List<Department>() };
            department1.Departments.Add(department2);
            department2.Departments.Add(department3);

            var departments = new List<Department> { department1, department2, department3 };

            var service = new DepartmentService();

            // Act           
            var descendantCount = GetPrivateMethod<DepartmentService>("CalculateDescendantCount")
                .Invoke(service, new object[] { department1, departments });

            // Assert
            Assert.Equal(2, descendantCount); // department2 and department3 are descendants of department1
        }

        [Fact]
        public void AddDepartmentToParent_Adds_Department_To_Parent_If_Parent_Exists()
        {
            // Arrange
            var parent = new Department { OID = 1, Departments = new List<Department>() };
            var departments = new List<Department> { parent };
            var department = new Department { OID = 2, DepartmentParent_OID = 1 };

            var service = new DepartmentService();

            // Act
                GetPrivateMethod<DepartmentService>("AddDepartmentToParent")
                .Invoke(service, new object[] { departments, department });           

            // Assert
            Assert.Single(parent.Departments);
            Assert.Equal(department, parent.Departments[0]);
        }

        [Fact]
        public void AddDepartmentToParent_Throws_Exception_If_Parent_Does_Not_Exist()
        {
            // Arrange
            var departments = new List<Department>();
            var department = new Department { OID = 2, DepartmentParent_OID = 1 };

            var service = new DepartmentService();

            // Act & Assert
            var exception = Assert.Throws<TargetInvocationException>(() => GetPrivateMethod<DepartmentService>("AddDepartmentToParent")
                .Invoke(service, new object[] { departments, department }));
            Assert.IsType<Exception>(exception.InnerException);
            Assert.Equal("Parent department not found.", exception.InnerException.Message);
        }

        [Fact]
        public void AddDepartmentToParent_Does_Not_Add_Department_If_Parent_Does_Not_Exist()
        {
            // Arrange
            var departments = new List<Department>();
            var department = new Department { OID = 2, DepartmentParent_OID = 1 };

            var service = new DepartmentService();

            // Act
            try
            {   
                GetPrivateMethod<DepartmentService>("AddDepartmentToParent")
                .Invoke(service, new object[] { departments, department });                
            }
            catch (Exception)
            {
                // Ignore the exception
            }

            // Assert
            Assert.Empty(departments);
        }
    }    
}