using WolfTechTestHaavardBry.Models;

namespace WolfTechTestHaavardBry.Services
{
    public interface IDepartmentService
    {
        Task<List<Department>> GetDepartmentHierarchyAsync(string filePath);      
    }
}
