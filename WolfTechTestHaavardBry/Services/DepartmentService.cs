using WolfTechTestHaavardBry.Models;
using System.Linq;
using System.ComponentModel;

namespace WolfTechTestHaavardBry.Services
{
    public class DepartmentService : IDepartmentService
    {
        public async Task<List<Department>> GetDepartmentHierarchyAsync(string filePath)
        {
            try
            {
                var departments = readFileAndParseIntoDepartments(filePath);                

                foreach (var department in departments)
                {
                    AddDepartmentToParent(departments, department);
                }

                foreach (var department in departments)
                {
                    department.NumDecendants = CalculateDescendantCount(department, departments);
                }

                departments = filterAwayDuplicatesAndKeepRootDepartments(departments);

                return departments;
            }
            catch (Exception ex)
            {
                // Log or handle the parsing failure
                throw new Exception("Parsing failed: " + ex.Message);
            }
        }        

        private int CalculateDescendantCount(Department department, List<Department> departments)
        {
            int count = 0;     
            
            if(department.Departments!=null) {
                count += department.Departments.Count;

                foreach (var childDepartment in department.Departments)
                {
                    count += CalculateDescendantCount(childDepartment, departments);
                }
            }
                       
            return count;
        }

        private void AddDepartmentToParent(List<Department> departments, Department department)
        {
            if (department.DepartmentParent_OID.HasValue)
            {
                var parent = departments.FirstOrDefault(d => d.OID == department.DepartmentParent_OID);
                if (parent != null)
                {
                    parent.Departments.Add(department);
                }
                else
                {
                    // Handle the case where the parent department does not exist
                    throw new Exception("Parent department not found.");
                }
            }
        }

        private List<Department> readFileAndParseIntoDepartments(string filePath)
        {
                         var departments = System.IO.File.ReadAllLines(filePath)
                        .Skip(1) // Skip header            
                        .Select(line => line.Split(','))
                        .Select(parts => new Department
                        {
                            OID = int.Parse(parts[0]),
                            Title = parts[1],
                            Color = parts[2],
                            DepartmentParent_OID = string.IsNullOrEmpty(parts[3]) ? (int?)null : int.Parse(parts[3]),
                            Departments = new List<Department>()
                        })
                        .ToList();
                
                return departments;
        }

        private List<Department> filterAwayDuplicatesAndKeepRootDepartments(List<Department> departments)
        {
            var rootDepartments = departments.Where(d => !d.DepartmentParent_OID.HasValue).ToList();
            return rootDepartments;
        }

    }
}
