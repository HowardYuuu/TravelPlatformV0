using Microsoft.AspNetCore.Mvc;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Customer;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Employee;
using prjTravelPlatformV3.Models;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.Customer
{
    [Route("/api/Customer/{action}/{id?}")]
    public class CApiController : Controller
    {
        
        private readonly dbTravalPlatformContext _context;

        public CApiController(dbTravalPlatformContext context)
        {
            _context = context;
        }
        public IActionResult GetAll()
        {
            var customers = from c in _context.TCustomers
                            select c;
            return Json(customers);
        }
        public IActionResult GetById(int id)
        {
            var customer = _context.TCustomers.FirstOrDefault(c => c.FCustomerId == id);
            return Json(customer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerViewModel customer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    TCustomer tCustomer = new TCustomer
                    {
                        FCustomerId = customer.CustomerId,
                        FName = customer.Name,
                        FPassword = customer.Password,
                        FPhone = customer.Phone,
                        FIdentityNumber = customer.IdentityNumber,
                        FGender = customer.Gender,
                        FBirth = customer.Birth,
                        FEmail = customer.Email,
                        FAddress = customer.Address,
                        FIsCheck = customer.IsCheck,
                    };
                    _context.Add(tCustomer);
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
        public async Task<IActionResult> Edit(CustomerViewModel customer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    TCustomer tCustomer = new TCustomer
                    {
                        FCustomerId = customer.CustomerId,
                        FName = customer.Name,
                        FPassword = customer.Password,
                        FPhone = customer.Phone,
                        FIdentityNumber = customer.IdentityNumber,
                        FGender = customer.Gender,
                        FBirth = customer.Birth,
                        FEmail = customer.Email,
                        FAddress = customer.Address,
                        FIsCheck = customer.IsCheck,
                    };
                    _context.Update(tCustomer);
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
            var customer = _context.TCustomers.FirstOrDefault(c => c.FCustomerId == id);
            if (customer == null)
            {
                return BadRequest("無法找到此筆資料");
            }
            try
            {
                _context.TCustomers.Remove(customer);
                _context.SaveChanges();
                return Ok("刪除成功");
            }
            catch 
            {
                return BadRequest("刪除失敗");
            }
        }




    //    [HttpPost, ActionName("Delete")]
    //    [ValidateAntiForgeryToken]
    //    public async Task<IActionResult> DeleteConfirmed(int id)
    //    {
    //        if (_context.TCustomers == null)
    //        {
    //            return Problem("Entity set 'dbTravalPlatformContext.TCustomers'  is null.");
    //        }
    //        var tCustomer = await _context.TCustomers.FindAsync(id);
    //        if (tCustomer != null)
    //        {
    //            _context.TCustomers.Remove(tCustomer);
    //        }

    //        await _context.SaveChangesAsync();
    //        return RedirectToAction(nameof(Index));
    //    }
    }
}
