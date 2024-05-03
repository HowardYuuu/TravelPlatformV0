using Microsoft.AspNetCore.Mvc;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Employee;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Hotel;
using prjTravelPlatformV3.Models;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.Employee
{
    [Route("/api/Employee/{action}/{id?}")]
    public class EApiController : Controller
    {
        private readonly dbTravalPlatformContext _context;

        public EApiController(dbTravalPlatformContext context)
        {
            _context = context;
        }
        public IActionResult GetAll()
        {
            var employees = from e in _context.TEmployees
                         select e;
            return Json(employees);
        }
        public IActionResult GetById(int id)
        {
            var employee = _context.TEmployees.FirstOrDefault(e => e.FEmployeeId == id);
            return Json(employee);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    TEmployee tEmployee = new TEmployee
                    {
                        FEmployeeId = employee.EmployeeId,
                        FName = employee.Name,
                        FAccountNumber = employee.AccountNumber,
                        FPassword = employee.Password,
                        FPhone = employee.Phone,
                        FIdentityNumber = employee.IdentityNumber,
                        FGender = employee.Gender,
                        FBirth = employee.Birth,
                        FEmail = employee.Email,
                        FAddress = employee.Address,
                        FStatus = employee.Status,
                        FStaffId = employee.StaffId,
                    };
                    _context.Add(tEmployee);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "資料新增成功" });
                }
                catch (Exception e)
                {
                    return Json(new { success = false, message = $"資料新增失敗：{e.Message}" });
                }
            }
            //驗證沒過            
            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
            );
            //var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return Json(new
            {
                success = false,
                message = "資料驗證失敗",
                errors
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeViewModel employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    TEmployee tEmployee = new TEmployee
                    {
                        FEmployeeId = employee.EmployeeId,
                        FName = employee.Name,
                        FAccountNumber = employee.AccountNumber,
                        FPassword = employee.Password,
                        FPhone = employee.Phone,
                        FIdentityNumber = employee.IdentityNumber,
                        FGender = employee.Gender,
                        FBirth = employee.Birth,
                        FEmail = employee.Email,
                        FAddress = employee.Address,
                        FStatus = employee.Status,
                        FStaffId = employee.StaffId,
                    };
                    _context.Update(tEmployee);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "資料修改成功" });
                }
                catch (Exception e)
                {
                    return Json(new { success = false, message = $"資料修改失敗：{e.Message}" });
                }
            }
            //驗證沒過            
            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
            );
            //var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return Json(new
            {
                success = false,
                message = "資料驗證失敗",
                errors
            });
        }
        public IActionResult Delete(int id)
        {
            var employee = _context.TEmployees.FirstOrDefault(e => e.FEmployeeId == id);
            if (employee == null)
            {
                return BadRequest("無法找到此筆資料");
            }
            try
            {
                _context.TEmployees.Remove(employee);
                _context.SaveChanges();
                return Ok("刪除成功");
            }
            catch
            {
                return BadRequest("刪除失敗");
            }
        }
    }
}
