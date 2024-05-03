using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjTravelPlatformV3.Areas.Employee.ViewModels.Product;
using prjTravelPlatformV3.Models;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.RegularExpressions;
using prjTravelPlatform_release.Areas.Customer.ViewModel.Products;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Security.Cryptography;

namespace prjTravelPlatform_release.Areas.Customer.Controllers.Product
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductApiController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly dbTravalPlatformContext _context;

        private readonly HttpClient _httpClient;
        //private readonly IHttpClientFactory _httpClientFactory;

        public ProductApiController(IHttpContextAccessor httpContextAccessor, dbTravalPlatformContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            // 實例化 HttpClient
            _httpClient = new HttpClient();
        }
        // 展示 index 頁面
        [HttpGet]
        [Route("ItemListShow")]
        public async Task<IActionResult> ItemListShow()
        {
            var itemList = await _context.TIproductListViews
                .Select(item => new
                {
                    item.FProductId,
                    item.FCompanyName,
                    item.FImagePath,
                    item.FProductName,
                    item.FDescription,
                    item.FPrice,
                    item.AvgScore,
                    item.TotalQty
                })
                .ToListAsync();

            return Json(itemList);
        }
        // 展示銷售量排序
        [HttpGet]
        [Route("ShowItemBySalesQty")]
        public async Task<IActionResult> ShowItemBySalesQty()
        {
            var itemList = await _context.TIproductListViews
                .OrderByDescending(item => item.TotalQty) // 按 TotalQty 由高到低排序
                .Select(item => new
                {
                    item.FProductId,
                    item.FCompanyName,
                    item.FImagePath,
                    item.FProductName,
                    item.FDescription,
                    item.FPrice,
                    item.AvgScore,
                    item.TotalQty
                })
                .ToListAsync();

            return Json(itemList);
        }
        // 展示平台周邊篩選
        [HttpGet]
        [Route("ShowItemByProSource")]
        public async Task<IActionResult> ShowItemByProSource(bool includePlatformPeripheral = false)
        {
            IQueryable<TIproductListView> query = _context.TIproductListViews;

            if (includePlatformPeripheral)
            {
                // 如果勾選了平台周邊，則只返回公司名稱為"平台"的商品資料
                query = query.Where(item => item.FCompanyName == "平台");
            }

            var itemList = await query
                .Select(item => new
                {
                    item.FProductId,
                    item.FCompanyName,
                    item.FImagePath,
                    item.FProductName,
                    item.FDescription,
                    item.FPrice,
                    item.AvgScore,
                    item.TotalQty
                })
                .ToListAsync();

            return Json(itemList);
        }

        // 計算 index 頁面商品銷售量
        [HttpGet]
        [Route("ItemSaleCount")]
        public async Task<IActionResult> ItemSaleCount()
        {
            var salesCount = await _context.TIorderDetailViews
                .GroupBy(od => od.FProductName)
                .Select(group => new
                {
                    FProductName = group.Key,
                    FSaleCount = group.Sum(od => od.FQty)
                })
                .ToListAsync();

            return Json(salesCount);
        }
        // 計算 index 頁面商品評分
        [HttpGet]
        [Route("ItemReviewCount")]
        public async Task<IActionResult> ItemReviewCount()
        {
            var reviewData = await _context.TIproductReviewsViews
                            .GroupBy(review => review.FProductName)
                            .Select(group => new
                        {
                            FProductName = group.Key,
                            AverageScore = group.Average(review => review.FProductScore), // 計算平均分數
                            TotalReviews = group.Count() // 總評論數
                        })
                        .ToListAsync();
            return Json(reviewData);
        }
        // 依據傳入的 productId 抓 itemdetail 頁面資料
        [HttpGet]
        [Route("GetItemDetail")]
        public async Task<IActionResult> GetItemDetail(int productId)
        {
            // 從資料庫中取得商品詳細資料
            var itemDetail = await _context.TIproductListViews
                .Where(p => p.FProductId == productId)
                .ToListAsync();

            // 回傳 JSON 格式的商品詳細資料
            return Json(itemDetail);
        }

        [HttpGet]
        [Route("GetItemSpecsDetail")]
        // 依據傳入的 specId 抓 TIproductSpecs 其他資料
        public async Task<IActionResult> GetItemSpecsDetail(int specId)
        {
            // 從資料庫中取得商品規格詳細資料
            var itemSpecsDetail = await _context.TIproductSpecs
                .Where(ps => ps.FSpecId == specId)
                .ToListAsync();

            // 回傳 JSON 格式的商品規格詳細資料
            return Json(itemSpecsDetail);
        }

        [HttpGet]
        [Route("GetItemSpecList")]
        // 依據傳入的 productId 抓商品規格資料
        public async Task<IActionResult> GetItemSpecList(int productId)
        {
            // 從資料庫中取得商品規格資料列表
            var itemSpecList = await _context.TIproductSpecs
                .Where(p => p.FProductId == productId)
                .ToListAsync();

            // 回傳 JSON 格式的商品規格資料列表
            return Json(itemSpecList);
        }
        [HttpGet]
        [Route("GetItemReviewCountById")]
        public async Task<IActionResult> GetItemReviewCountById(int productId)
        {
            // 透過 fProductId 查詢對應的 fProductName
            var productName = await _context.TIproducts
                                .Where(p => p.FProductId == productId)
                                .Select(p => p.FProductName)
                                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(productName))
            {
                return NotFound("Product not found");
            }

            // 以 fProductName 查詢對應的評論資料筆數與平均評分
            var reviewData = await _context.TIproductReviewsViews
                            .Where(review => review.FProductName == productName)
                            .GroupBy(review => review.FProductName)
                            .Select(group => new
                            {
                                FProductName = group.Key,
                                AverageScore = group.Average(review => review.FProductScore), // 計算平均分數
                                TotalReviews = group.Count() // 總評論數
                            })
                            .FirstOrDefaultAsync();

            if (reviewData == null)
            {
                return NotFound("Reviews not found");
            }

            return Json(reviewData);
        }
        // 依據傳入的 productId 抓商品評論資料
        [HttpGet]
        [Route("GetItemReview")]
        public async Task<IActionResult> GetItemReview(int productId)
        {
            //// 先從 TIproductSpecs 中獲取對應的 FSpecId 和 FSpecName
            //var productSpecs = await _context.TIproductSpecs
            //    .Where(ps => ps.FProductId == productId)
            //    .Select(ps => new { ps.FSpecId, ps.FSpecName })
            //    .ToListAsync();

            //if (productSpecs == null || productSpecs.Count == 0)
            //{
            //    return NotFound("No product specifications found for the given productId.");
            //}
            // 透過 fProductId 查詢對應的 fProductName
            var productName = await _context.TIproducts
                                .Where(p => p.FProductId == productId)
                                .Select(p => p.FProductName)
                                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(productName))
            {
                return NotFound("Product not found");
            }

            //// 提取 specName 列表
            //var specNames = productSpecs.Select(ps => ps.FSpecName).ToList();

            // 從 TIproductReviewsViews 中查詢 FSpecName 對應的所有內容
            var reviews = _context.TIproductReviewsViews
                .AsEnumerable()  // 將資料庫查詢轉為 .NET 集合
                .Where(review => review.FProductName == productName)
                .Take(5)  // 只取最多5筆資料
                .ToList();

            // 檢查是否有評論資料，如果沒有，回傳一個空的集合或 null
            return Ok(reviews.Any() ? reviews : null);
        }
        //依據商品店家查詢商品清單
        [HttpGet]
        [Route("GetItemListByShop")]
        public async Task<IActionResult> GetItemListByShop(string companyName)
        {
            // 從資料庫中取得商品規格資料列表
            var shopItemList = await _context.TIproductListViews
                .Where(p => p.FCompanyName == companyName)
                .ToListAsync();

            // 回傳 JSON 格式的商品規格資料列表
            return Json(shopItemList);
        }
        //依據商品類別查詢商品清單
        [HttpGet]
        [Route("GetItemListByType")]
        public async Task<IActionResult> GetItemListByType(string itemType)
        {
            // 從資料庫中取得商品規格資料列表
            var typeItemList = await _context.TIproductListViews
                .Where(p => p.FType == itemType)
                .OrderBy(x => Guid.NewGuid()) // 隨機排序
                .Take(10) // 取前10筆
                .ToListAsync();

            // 回傳 JSON 格式的商品規格資料列表
            return Json(typeItemList);
        }
        //選擇規格之後加入購物車，並存入資料庫
        [HttpPost]
        [Route("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] TIshoppingCart cartItem)
        {
            // 驗證模型是否有效
            try
            {
                //cartItem.FCustomerId = 2;
                // 將購物車項目添加到資料庫
                _context.TIshoppingCarts.Add(cartItem);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "成功將商品添加到購物車。" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"購物車新增失敗：{ex.ToString()}" });
            }
        }
        //載入購物車頁面
        [HttpGet]
        [Route("GetCartList")]
        public IActionResult GetCartList(int customerId)
        {
            try
            {
                // 使用 LINQ 查詢，選擇購物車項目、規格和產品資訊
                var cartList = _context.TIshoppingCarts
                    .Where(cart => cart.FCustomerId == customerId) // 根據客戶ID篩選購物車項目
                    .Join(
                        _context.TIproductSpecs, // 規格表
                        cart => cart.FSpecId,     // 購物車表的規格ID
                        spec => spec.FSpecId,     // 規格表的規格ID
                        (cart, spec) => new { cart, spec } // 將購物車項目和規格進行連接
                    )
                    .Join(
                        _context.TIproductListViews, // 產品檢視表
                        cartSpec => cartSpec.spec.FProductId, // 購物車和規格的產品ID
                        product => product.FProductId,         // 產品檢視表的產品ID
                        (cartSpec, product) => new
                        {
                            // 選擇需要的欄位
                            cartSpec.cart.FShoppingCartId,
                            cartSpec.cart.FCustomerId,
                            cartSpec.cart.FSpecId,
                            cartSpec.cart.FQty,
                            subTotal= cartSpec.cart.FQty* cartSpec.spec.FPrice,
                            cartSpec.spec.FSpecName,
                            specImg = cartSpec.spec.FImagePath,
                            product.FProductId,
                            itemImg = product.FImagePath,
                            product.FProductName,
                            product.FCompanyName
                        }
                    )
                    .ToList();

                return Json(cartList);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"查詢資料失敗：{ex.ToString()}" });
            }
        }
        [HttpPost]
        [Route("UpdateQtyAndSubtotal")]
        public IActionResult UpdateQtyAndSubtotal([FromQuery] int fShoppingCartId, [FromQuery] int newQty, [FromQuery] int fSpecId)
        {
            try
            {
                // 根據 fShoppingCartId 更新資料庫中的 fQty
                var cartItem = _context.TIshoppingCarts
                    .Where(cart => cart.FShoppingCartId == fShoppingCartId)
                    .Join(_context.TIproductSpecs,
                            cart => cart.FSpecId,
                            spec => spec.FSpecId,
                            (cart, spec) => new { cart, spec }
                            ).FirstOrDefault();
                if (cartItem != null)
                {
                    cartItem.cart.FQty = newQty;
                    // 重新計算小計
                    var newSubtotal = newQty * cartItem.spec.FPrice;
                    cartItem.cart.FSubtotal = newSubtotal;

                    _context.SaveChanges();

                    return Json(new { success = true, newSubtotal = newSubtotal });
                }

                return Json(new { success = false, message = "找不到對應的購物車項目。" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"更新數量和小計時發生錯誤：{ex.ToString()}" });
            }
        }
        [HttpPut]
        [Route("UpdateCartSeleSpec")]
        public IActionResult UpdateCartSeleSpec([FromQuery] int fShoppingCartId, [FromQuery] int newSpecId)
        {
            try
            {
                // 根據 fShoppingCartId 更新資料庫中的 fSpecId
                var cartItem = _context.TIshoppingCarts
                    .Where(cart => cart.FShoppingCartId == fShoppingCartId)
                    .Join(_context.TIproductSpecs,
                            cart => cart.FSpecId,
                            spec => spec.FSpecId,
                            (cart, spec) => new { cart, spec }
                            ).FirstOrDefault();

                if (cartItem != null)
                {
                    cartItem.cart.FSpecId = newSpecId;
                    // 根據新的 fSpecId 查找相應的 FPrice
                    var newSpec = _context.TIproductSpecs
                        .Where(spec => spec.FSpecId == newSpecId)
                        .FirstOrDefault();
                    if (newSpec != null)
                    {
                        // 使用新的 FPrice 重新計算小計
                        var newSubtotal = cartItem.cart.FQty * newSpec.FPrice;
                        cartItem.cart.FSubtotal = newSubtotal;

                        _context.SaveChanges();

                        return Json(new { success = true, newSubtotal = newSubtotal });
                    }
                }

                return Json(new { success = false, message = "找不到對應的購物車項目。" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"更新選擇的規格時發生錯誤：{ex.ToString()}" });
            }
        }
        //刪除購物車項目
        [HttpDelete]
        [Route("DeleteCartItem")]
        public IActionResult DeleteCartItem(int CartId)
        {
            try
            {
                // 根據 fShoppingCartId 查詢資料庫中的購物車項目
                var cartItem = _context.TIshoppingCarts
                    .FirstOrDefault(cart => cart.FShoppingCartId == CartId);

                if (cartItem != null)
                {
                    // 從資料庫中移除購物車項目
                    _context.TIshoppingCarts.Remove(cartItem);
                    _context.SaveChanges();

                    return Json(new { success = true, message = "刪除成功" });
                }

                return Json(new { success = false, message = "找不到對應的購物車項目。" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"刪除購物車項目時發生錯誤：{ex.ToString()}" });
            }
        }
        //計算購物車總金額
        [HttpGet]
        [Route("GetTotalAmount")]
        public IActionResult GetTotalAmount(string checkspCartIds)
        {
            try
            {
                if (string.IsNullOrEmpty(checkspCartIds))
                {
                    // 如果传入的购物车ID列表为空，则直接返回总金额为零
                    return Json(new { totalAmount = 0 });
                }

                // 将逗号分隔的字符串解析为整数数组
                int[] cartIds = checkspCartIds.Split(',').Select(int.Parse).ToArray();

                // 使用 LINQ 查询，选择购物车项目、规格和产品信息
                var totalAmount = _context.TIshoppingCarts
                    .Where(cart => cartIds.Contains(cart.FShoppingCartId))
                    .Sum(cart => cart.FSubtotal);

                return Json(new { totalAmount = totalAmount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"查询数据失败：{ex.ToString()}" });
            }
        }
        //結帳頁面載入
        [HttpGet]
        [Route("GetCheckoutList")]
        public IActionResult GetCheckoutList(int checkoutIds)
        {
            try
            {
                var checkoutList = _context.TIshoppingCarts
                    .Where(cart => cart.FShoppingCartId == checkoutIds) // 根據客戶ID篩選購物車項目
                    .Join(
                        _context.TIproductSpecs, // 規格表
                        cart => cart.FSpecId,     // 購物車表的規格ID
                        spec => spec.FSpecId,     // 規格表的規格ID
                        (cart, spec) => new { cart, spec } // 將購物車項目和規格進行連接
                    )
                    .Join(
                        _context.TIproductListViews, // 產品檢視表
                        cartSpec => cartSpec.spec.FProductId, // 購物車和規格的產品ID
                        product => product.FProductId,         // 產品檢視表的產品ID
                        (cartSpec, product) => new
                        {
                            // 選擇需要的欄位
                            cartSpec.cart.FShoppingCartId,
                            cartSpec.cart.FCustomerId,
                            cartSpec.cart.FSpecId,
                            cartSpec.cart.FQty,
                            cartSpec.cart.FSubtotal,
                            cartSpec.spec.FSpecName,
                            cartSpec.spec.FPrice,
                            specImg = cartSpec.spec.FImagePath,
                            product.FProductId,
                            itemImg = product.FImagePath,
                            product.FProductName,
                            product.FCompanyName
                        }
                    )
                    .ToList();

                return Json(checkoutList);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"查询数据失败：{ex.ToString()}" });
            }
        }
        //載入配送方式選項
        [HttpGet]
        [Route("GetLogisticsList")]
        public IActionResult GetLogisticsList(string companyName)
        {
            try
            {
                var logisticsList = _context.TIsupLogisticsViews
                    .Where(cart => cart.FCompanyName == companyName).ToList();

                return Json(logisticsList);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"查询数据失败：{ex.ToString()}" });
            }
        }

        //優惠卷
        //[HttpGet]
        //[Route("GetCusCouponList")]
        //public async Task<IActionResult> GetCusCouponList()
        //{
        //    try
        //    {
        //        ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        int customerID = Convert.ToInt32(ViewBag.CusID);

        //        // 查看會員持有優惠卷
        //        var coupons = from cusCoupon in _context.TDcCusCouponQties
        //                      where cusCoupon.FCustomerId == customerID && cusCoupon.FUsed == false
        //                      join couponList in _context.TDcCouponLists on cusCoupon.FCouponId equals couponList.FCouponId
        //                      where couponList.FProductType == "伴手禮"
        //                      select new
        //                      {
        //                          couponId = cusCoupon.FCouponId,
        //                          couponCode = couponList.FCouponCode,
        //                          discount = couponList.FDiscount
        //                      };

        //        return Json(coupons);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = $"目前無可用優惠卷：{ex.ToString()}" });
        //    }
        //}

        [HttpGet]
        [Route("GetCusCouponList")]
        public async Task<IActionResult> GetCusCouponList(int customerId)
        {
            try
            {
                var coupons = _context.TDcCusCouponQties
                    .Where(cusCoupon => cusCoupon.FCustomerId == customerId && cusCoupon.FUsed == false)
                    .Join(
                        _context.TDcCouponLists,
                        cusCoupon => cusCoupon.FCouponId,
                        couponList => couponList.FCouponId,
                        (cusCoupon, couponList) => new
                        {
                            couponId = cusCoupon.FCouponId,
                            couponCode = couponList.FCouponCode,
                            discount = couponList.FDiscount
                        }
                    )
                    .ToList();

                return Json(coupons);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"目前無可用優惠卷：{ex.ToString()}" });
            }
        }

        //建立訂單
        [HttpPost]
        [Route("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] List<TIorderViewModel> orders)
        {           
            try
            {
                List<string> createdOrderIds = new List<string>(); // 用於存儲已創建訂單的 FOrderId
                // 取得今天的日期
                DateTime today = DateTime.Today;
                // 格式化日期成 "yyyyMMdd" 的字串格式
                string formattedDate = today.ToString("yyyyMMdd");

                foreach (var order in orders)
                {
                    TIorder rorder = new TIorder
                    {
                        FMemberId = order.FMemberId,
                        FOrderDate = formattedDate,
                        FCoupponId = order.FCoupponId,
                        FStatusId = 1,
                        FPayId = order.FPayId,
                        FNotes = order.FNotes,
                        FLogisticsId = order.FLogisticsId, // 這個數值是你自己設定的，請確認是否正確
                        FShipName = order.FShipName,
                        FShipPhone = order.FShipPhone,
                        FShipAddress = order.FShipAddress,
                        FReviewed = false,
                        FLastModified = formattedDate
                    };
                    _context.TIorders.Add(rorder);
                    await _context.SaveChangesAsync();

                    createdOrderIds.Add(rorder.FOrderId); // 將已創建訂單的 FOrderId 添加到集合中

                    // 創建訂單詳細信息
                    foreach (var orderDetail in order.OrderDetails)
                    {

                        TIorderDetail rorderDetail = new TIorderDetail
                        {
                            FOrderId = rorder.FOrderId, // 使用新增的訂單的 FOrderId
                            FSpecId = orderDetail.FSpecId,
                            FQty = orderDetail.FQty,
                            FSubtotal = orderDetail.FSubtotal
                        };

                        _context.TIorderDetails.Add(rorderDetail);
                        // 移除購物車中的商品
                        var shoppingCartItems = _context.TIshoppingCarts
                            .Where(item => item.FShoppingCartId == orderDetail.FShoppingCartId)
                            .ToList();
                        _context.TIshoppingCarts.RemoveRange(shoppingCartItems);
                    }

                    await _context.SaveChangesAsync();
                }
                var paymentId = _context.TIorders
                                        .OrderByDescending(o => o.FOrderId)
                                        .Select(o => o.FPayId).FirstOrDefault();
                object paymentParams = null;
                if (paymentId != null && paymentId == 5)
                {
                    var latestOrderId = _context.TIorderViews
                                        .OrderByDescending(o => o.FOrderId)
                                        .Select(o => o.FOrderId).FirstOrDefault();

                    var amount = _context.TIorderViews
                                .OrderByDescending(o => o.FOrderId)
                                .Select(o => Convert.ToInt32(o.FTotal)).FirstOrDefault();

                    var TradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    var checkstr = "HashKey=5294y06JbISpM5x9&ChoosePayment=Credit&ItemName=Item&MerchantID=2000132&MerchantTradeDate=" + TradeDate + "&MerchantTradeNo=" + latestOrderId
                    + "&OrderResultURL=https://localhost:7119/Customer/Products/BookingFinish"
                    + "&PaymentType=aio&ReturnURL=https://localhost:7119/Customer/Products/BookingFinish" + "&TotalAmount=" + amount
                    + "&TradeDesc=test&HashIV=v77hoKGq4kWxNNIS";

                    var encodedCheckstr = HttpUtility.UrlEncode(checkstr).ToLower();
                    string hashedString;
                    // 將字串轉換為位元組陣列
                    byte[] bytes = Encoding.UTF8.GetBytes(encodedCheckstr);

                    // 建立 SHA256 實例
                    using (SHA256 sha256 = SHA256.Create())
                    {
                        // 計算雜湊值
                        byte[] hash = sha256.ComputeHash(bytes);

                        // 將位元組陣列轉換為十六進位字串
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (byte b in hash)
                        {
                            stringBuilder.Append(b.ToString("x2"));
                        }
                        hashedString = stringBuilder.ToString().ToUpper();
                    }

                    // 構建支付參數
                    paymentParams = new
                    {
                        // 根據綠界的API文件設置參數
                        MerchantID = "2000132",
                        MerchantTradeNo = latestOrderId,
                        MerchantTradeDate = TradeDate,
                        PaymentType = "aio",
                        TotalAmount = amount,
                        TradeDesc = "test",
                        ItemName = "Item",
                        ReturnURL = "https://localhost:7119/Customer/Products/BookingFinish",
                        OrderResultURL = "https://localhost:7119/Customer/Products/BookingFinish",
                        ChoosePayment = "Credit",
                        CheckMacValue = hashedString
                    };
                }

                return Json(new { success = true, message = "訂單新增成功", orderIds = createdOrderIds, paymentParams = paymentParams });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"訂單新增失敗：{ex.ToString()}" });
            }
        }
    }
}
