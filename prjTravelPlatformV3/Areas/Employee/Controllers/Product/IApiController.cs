using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Hotel;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Product;
using prjTravelPlatformV3.Models;
using System.Dynamic;
using System.Globalization;

namespace prjTravelPlatformV3.Areas.Employee.Controllers.Product
{
    [Route("/api/Product/{action}/{id?}")]
    public class IApiController : Controller
    {
        private readonly dbTravalPlatformContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public IApiController(dbTravalPlatformContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult GetProductOrder()
        {
            return Json(_context.TIorderViews);
        }
        public IActionResult GetOrderById(string orderId)
        {
            var order = _context.TIorderViews.FirstOrDefault(od => od.FOrderId == orderId);
            return Json(order);
        }
        public IActionResult GetDetailByOrderId(string orderId)
        {
            var orderDetail = _context.TIorderDetailViews.Where(od => od.FOrderId == orderId).ToList();
            return Json(orderDetail);
        }
        public IActionResult GetProductList()
        {
            var productList = _context.TIproducts
                .Select(p => new
                {
                    fProductId = p.FProductId,
                    fProductName = p.FProductName,
                    fProSource = p.FProSource,
                    fSupplierId = p.FSupplierId,
                    fSupplierName = p.FSupplier != null ? p.FSupplier.FCompanyName : null,
                    fTypeName = p.FType != null ? p.FType.FType : null,
                    fRelease = p.FRelease,
                    fProStatus = p.FProStatus.HasValue && p.FProStatus.Value ? "上架" : "未上架", // 將布林值轉換為字符串表示形式
                    fImagePath = p.FImagePath
                })
                .ToList();
            return Json(productList);
        }
        public async Task<IActionResult> GetProductInfoById(int productId)
        {
            var product = await _context.TIproducts.FirstOrDefaultAsync(p => p.FProductId == productId);
            return Json(product);
        }

        public async Task<IActionResult> GetSupplierInfoByType()
        {
            var suppliers = await _context.TCcompanyInfos
                .Where(s => s.FType == "I") // 查詢 FType 為 'I' 的記錄
                .Select(s => new { s.FId, s.FCompanyName }) // 選擇 FId 和 FCompanyName 欄位
                .ToListAsync(); // 將結果轉換為清單
            return Json(suppliers);
        }

        public async Task<IActionResult> GetSpecListById(int productId)
        {
            var itemSpecsList = await _context.TIproductSpecs
                .Where(p => p.FProductId == productId)
                .ToListAsync(); // 將結果轉換為清單
            return Json(itemSpecsList);
        }

        public async Task<IActionResult> GetProTypeByTypeId()
        {
            var types = await _context.TItypes
                .Select(t => new { t.FTypeId, t.FType }) // 選擇 FId 和 FCompanyName 欄位
                .ToListAsync(); // 將結果轉換為清單
            return Json(types);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductViewModel product)
        {
            // 驗證模型是否有效
            try
            {
                var p = product;
                if (ModelState.IsValid)
                {
                    // 處理圖片上傳
                    string? uniqueFileName = null;
                    if (!string.IsNullOrEmpty(product.ImagePath))
                    {
                        // 将 Base64 编码的字符串转换为字节数组
                        byte[] imageBytes = Convert.FromBase64String(product.ImagePath.Split(',')[1]);

                        // 指定图片保存的文件夹路径
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");

                        // 生成唯一的文件名
                        uniqueFileName = Guid.NewGuid().ToString() + ".jpg";

                        // 拼接图片的完整文件路径
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // 将字节数组写入到文件中
                        await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
                    }
                    TIproduct? rproduct = new TIproduct();
                    if (rproduct != null)
                    {
                        rproduct.FProductId = product.ProductId;
                        rproduct.FProductName = product.ProductName;
                        rproduct.FProSource = product.ProSource;
                        rproduct.FSupplierId = product.SupplierId;
                        rproduct.FTypeId = product.TypeId;
                        rproduct.FRelease = product.Release;
                        rproduct.FProStatus = product.ProStatus;
                        if (!string.IsNullOrEmpty(product.ImagePath)) rproduct.FImagePath = uniqueFileName;
                        _context.Update(rproduct);

                        await _context.SaveChangesAsync();
                        return Json(new { success = true, message = "資料修改成功" });
                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            message = "資料儲存失敗"
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "資料驗證失敗",
                        product = product
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"資料修改失敗：{ex.ToString()}" });
            }

        }
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] ProductViewModel product)
        {
            // 驗證模型是否有效
            try
            {
                //var p = product;
                if (ModelState.IsValid)
                {
                    // 處理圖片上傳
                    string? uniqueFileName = null;
                    if (!string.IsNullOrEmpty(product.ImagePath))
                    {
                        // 将 Base64 编码的字符串转换为字节数组
                        byte[] imageBytes = Convert.FromBase64String(product.ImagePath.Split(',')[1]);

                        // 指定图片保存的文件夹路径
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");

                        // 生成唯一的文件名
                        uniqueFileName = Guid.NewGuid().ToString() + ".jpg";

                        // 拼接图片的完整文件路径
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // 将字节数组写入到文件中
                        await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
                    }
                    TIproduct? rproduct = _context.TIproducts.Find(product.ProductId);
                    if (rproduct != null)
                    {
                        rproduct.FProductId = product.ProductId;
                        rproduct.FProductName = product.ProductName;
                        rproduct.FProSource = product.ProSource;
                        rproduct.FSupplierId = product.SupplierId;
                        rproduct.FTypeId = product.TypeId;
                        rproduct.FRelease = product.Release;
                        rproduct.FProStatus = product.ProStatus;
                        rproduct.FDescription = product.Description;
                        if (!string.IsNullOrEmpty(product.ImagePath)) rproduct.FImagePath = uniqueFileName;
                        _context.Update(rproduct);

                        await _context.SaveChangesAsync();
                        return Json(new { success = true, message = "資料修改成功" });
                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            message = "資料儲存失敗"
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "資料驗證失敗",
                        product = product
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"資料修改失敗：{ex.ToString()}" });
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int productId)
        {
            var productToDelete = _context.TIproducts.Find(productId);
            if (productToDelete == null)
            {
                return NotFound();
            }

            _context.TIproducts.Remove(productToDelete);
            _context.SaveChanges();

            return Ok();
        }
    }
}
