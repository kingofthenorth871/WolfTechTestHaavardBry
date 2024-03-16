using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using WolfTechTestHaavardBry.Models;
using WolfTechTestHaavardBry.Services;

namespace WolfTechTestHaavardBry.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }
       
        [HttpGet("/DepartmentHierarchy")]
        public async Task<ActionResult<List<Department>>> GetDepartmentHierarchy([FromQuery][DefaultValue("C:/Users/howar/Downloads/filForWolftech.txt")][SwaggerParameter("GET method can only parse input of .txt files seperated by lines.")] string filePath)        
        {
            try
            {
                var departments = await _departmentService.GetDepartmentHierarchyAsync(filePath);
                return Json(departments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
