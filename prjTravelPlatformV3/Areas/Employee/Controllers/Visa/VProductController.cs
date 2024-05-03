using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Visa;
using prjTravelPlatformV3.Models;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.Visa
{
    [Area("Employee")]
    public class VProductController : Controller
    {
        private readonly dbTravalPlatformContext _context;

        public VProductController(dbTravalPlatformContext context)
        {
            _context = context;
        }

        //view
        public IActionResult Index()
        {
            return View();
        }

        //api
        public JsonResult allVProducts()
        {
            var VProdcuts = _context.VVproductViews;
            return Json(VProdcuts);
        }

        public JsonResult countriesByRegion(string region)
        {
            var countries = _context.TVcountries.Where(c => c.FRegion == region);
            return Json(countries);
        }

        public JsonResult all()
        {
            var pro = _context.TVproducts;
            return Json(pro);
        }


        //啟用未啟用
        public async Task<IActionResult> Enabled(int? id, bool enabled)
        {
            if (id == null)
            {
                return Json(new { success = false, message = $"更改失敗" });
            }

            var tvProduct = _context.TVproducts.Find(id);
            if (tvProduct == null)
            {
                return Json(new { success = false, message = $"更改失敗" });
            }

            try
            {
                tvProduct.FEnabled = enabled;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "更改成功" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = $"更改失敗：{e.Message}" });
            }
        }

        //get modal partial
        public IActionResult GetPartial(int? id)
        {
            var allForms = _context.TVformPaths;
            var Region = _context.TVcountries.Select(t => t.FRegion).Distinct();
            if (id == 0)
            {
                ViewBag.allForms = allForms;
                ViewBag.FCountryId = new SelectList(_context.TVcountries, "FId", "FCountry");
                ViewBag.FRegionId = new SelectList(Region);
                ViewBag.FLengthOfStayId = new SelectList(_context.TVlengthOfStays, "FId", "FLengthOfStay");
                ViewBag.FProcessingTimeId = new SelectList(_context.TVprocessingTimes, "FId", "FId");
                ViewBag.FSupplierId = new SelectList(_context.TCcompanyInfos.Where(t => t.FType == "V"), "FId", "FCompanyName");
                ViewBag.FValidityPeriodId = new SelectList(_context.TVvalidityPeriods, "FId", "FValidityPeriod");
                ViewBag.formId = "Create";
                ViewBag.title = "新增商品";
                return PartialView("_ModalPartial", new VProductViewModel());
            }
            if (_context.TVproducts == null)
            {
                return NotFound();
            }
            var TVProduct = _context.TVproducts.Include(p => p.TVproductFormsRequireds).FirstOrDefault(t => t.FId == id);
            if (TVProduct == null)
            {
                return NotFound();
            }
            var selectedForms = _context.TVproductFormsRequireds.Where(p => p.FProductId == id).Select(f => f.FFormId).ToList();
            VProductViewModel vp = new VProductViewModel
            {
                FId = TVProduct.FId,
                FCountryId = TVProduct.FCountryId,
                FSupplierId = TVProduct.FSupplierId,
                FName = TVProduct.FName,
                FNewOrLost = TVProduct.FNewOrLost,
                FInterviewRequirement = TVProduct.FInterviewRequirement,
                FEntityOrElectronic = TVProduct.FEntityOrElectronic,
                FProcessingTimeId = TVProduct.FProcessingTimeId,
                FValidityPeriodId = TVProduct.FValidityPeriodId,
                FLengthOfStayId = TVProduct.FLengthOfStayId,
                FPrice = TVProduct.FPrice,
                FEnabled = TVProduct.FEnabled,
                TVproductFormsRequireds = TVProduct.TVproductFormsRequireds
            };
            ViewBag.selectedForms = selectedForms;
            ViewBag.allForms = allForms;
            ViewBag.FCountryId = new SelectList(_context.TVcountries, "FId", "FCountry");
            ViewBag.FRegionId = new SelectList(Region);
            ViewBag.FLengthOfStayId = new SelectList(_context.TVlengthOfStays, "FId", "FLengthOfStay");
            ViewBag.FProcessingTimeId = new SelectList(_context.TVprocessingTimes, "FId", "FProcessingTime");
            ViewBag.FSupplierId = new SelectList(_context.TCcompanyInfos.Where(t => t.FType == "V"), "FId", "FCompanyName");
            ViewBag.FValidityPeriodId = new SelectList(_context.TVvalidityPeriods, "FId", "FValidityPeriod");
            ViewBag.formId = "Edit";
            ViewBag.title = "編輯商品";
            return PartialView("_ModalPartial", vp);
        }

        //checkBoxPartial
        public IActionResult RefreshAllForms(int id)
        {
            var allForms = _context.TVformPaths;
            if (id == 0)
            {
                ViewBag.allForms = allForms;
                return PartialView("_CheckBoxPartial");
            }
            var TVProduct = _context.TVproducts.Include(p => p.TVproductFormsRequireds).FirstOrDefault(t => t.FId == id);
            if (TVProduct == null)
            {
                return NotFound();
            }
            VProductViewModel vp = new VProductViewModel
            {
                FId = TVProduct.FId,
                FCountryId = TVProduct.FCountryId,
                FSupplierId = TVProduct.FSupplierId,
                FName = TVProduct.FName,
                FNewOrLost = TVProduct.FNewOrLost,
                FInterviewRequirement = TVProduct.FInterviewRequirement,
                FEntityOrElectronic = TVProduct.FEntityOrElectronic,
                FProcessingTimeId = TVProduct.FProcessingTimeId,
                FValidityPeriodId = TVProduct.FValidityPeriodId,
                FLengthOfStayId = TVProduct.FLengthOfStayId,
                FPrice = TVProduct.FPrice,
                FEnabled = TVProduct.FEnabled,
                TVproductFormsRequireds = TVProduct.TVproductFormsRequireds
            };
            ViewBag.allForms = allForms;
            return PartialView("_CheckBoxPartial", vp);
        }




        //新增商品
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VProductViewModel vp)
        {
            if (vp == null)
            {
                return Json(new { success = false, message = "新增失敗:無商品" });
            }
            if (ModelState.IsValid)
            {
                try
                {
                    TVproduct tVproduct = new TVproduct
                    {
                        FId = vp.FId,
                        FCountryId = vp.FCountryId,
                        FSupplierId = vp.FSupplierId,
                        FName = vp.FName,
                        FNewOrLost = vp.FNewOrLost,
                        FInterviewRequirement = vp.FInterviewRequirement,
                        FEntityOrElectronic = vp.FEntityOrElectronic,
                        FProcessingTimeId = vp.FProcessingTimeId,
                        FValidityPeriodId = vp.FValidityPeriodId,
                        FLengthOfStayId = vp.FLengthOfStayId,
                        FPrice = vp.FPrice,
                        FEnabled = vp.FEnabled,
                        TVproductFormsRequireds = vp.TVproductFormsRequireds
                    };
                    _context.Add(tVproduct);
                    await _context.SaveChangesAsync();


                    return Json(new { success = true, message = "商品訂單成功" });
                }
                catch (Exception e)
                {
                    return Json(new { success = false, message = $"商品新增失敗：{e.Message}" });
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

        //修改商品
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VProductViewModel vp)
        {
            if (vp == null)
            {
                return Json(new { success = false, message = "修改失敗:無商品" });
            }
            //Console.WriteLine( string.Join("---",vp.TVproductFormsRequireds));
            if (ModelState.IsValid)
            {
                try
                {
                    TVproduct tVproduct = new TVproduct
                    {
                        FId = vp.FId,
                        FCountryId = vp.FCountryId,
                        FSupplierId = vp.FSupplierId,
                        FName = vp.FName,
                        FNewOrLost = vp.FNewOrLost,
                        FInterviewRequirement = vp.FInterviewRequirement,
                        FEntityOrElectronic = vp.FEntityOrElectronic,
                        FProcessingTimeId = vp.FProcessingTimeId,
                        FValidityPeriodId = vp.FValidityPeriodId,
                        FLengthOfStayId = vp.FLengthOfStayId,
                        FPrice = vp.FPrice,
                        FEnabled = vp.FEnabled,
                    };
                    _context.Update(tVproduct);
                    await _context.SaveChangesAsync();

                    //formsrequired先刪後加
                    var oldfrs = _context.TVproductFormsRequireds.Where(f => f.FProductId == vp.FId).ToList();
                    if (oldfrs.Any())
                    {
                        _context.RemoveRange(oldfrs);
                    }

                    List<TVproductFormsRequired> frs = (List<TVproductFormsRequired>)vp.TVproductFormsRequireds;
                    await _context.AddRangeAsync(frs);

                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "商品修改成功" });
                }
                catch (Exception e)
                {
                    return Json(new { success = false, message = $"商品修改失敗: {e.Message}" });
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


        //上傳單一表格
        [HttpPost]
        public async Task<IActionResult> UploadForm(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = $"上傳失敗: 無檔案" });
            }

            try
            {
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "VForms", fileName);

                //複製檔案到wwwroot/VForms
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }



                //資料表存路徑
                var formPath = new TVformPath
                {
                    FFormName = fileName,
                    FFormPath = filePath
                };
                _context.TVformPaths.Add(formPath);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = " 上傳成功" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = $"上傳失敗: {e.Message}" });
            }
        }

        //刪除表格
        [HttpDelete]
        public async Task<IActionResult> DeleteForms([FromBody] List<int> Ids)
        {
            try
            {
                foreach (var Id in Ids)
                {
                    var formrequireds = _context.TVproductFormsRequireds.Where(p => p.FFormId == Id).ToList();
                    _context.RemoveRange(formrequireds);
                    var form = _context.TVformPaths.FirstOrDefault(t => t.FId == Id);
                    if (form != null)
                    {
                        _context.Remove(form);
                    }
                }
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = " 刪除資料成功" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = $"刪除資料失敗: {e.Message}" });
            }
        }


        //api for VOrder
        //查詢價格與相關資訊
        public JsonResult getPrInEnByProductName(string p)
        {
            var Info = _context.TVproducts.Where(t => t.FName == p).Select(t => new {price = t.FPrice, interview = t.FInterviewRequirement,entity = t.FEntityOrElectronic}).FirstOrDefault();
            return Json(Info);
        }
        public JsonResult getPrInEnByProductId(int id)
        {
            var Info = _context.TVproducts.Where(t => t.FId == id).Select(t => new { price = t.FPrice, interview = t.FInterviewRequirement, entity = t.FEntityOrElectronic }).FirstOrDefault();
            return Json(Info);
        }

        //查詢優惠券折扣
        public IActionResult getDiscount(string coupon)
        {
            var discount = _context.TDcCouponLists.Where(t => t.FCouponName == coupon).Select(t => t.FDiscount).FirstOrDefault();
            return Json(discount);
        }
    }
}
