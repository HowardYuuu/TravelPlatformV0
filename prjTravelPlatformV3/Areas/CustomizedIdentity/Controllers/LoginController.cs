using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using prjTravelPlatform_release.Areas.CustomizedIdentity.ViewModel.Customer;
using prjTravelPlatformV3.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Google.Apis.Auth;
using prjTravelPlatform_release.Areas.CustomizedIdentity.DTO.Res;
using System.Net;

namespace prjTravelPlatform_release.Areas.CustomizedIdentity.Controllers
{
    [Area("CustomizedIdentity")]

    public class LoginController : Controller
    {
        private dbTravalPlatformContext _context;
        private readonly TokenManager _tokenManager;

        public LoginController(dbTravalPlatformContext context, TokenManager tokenManager)
        {
            _context = context;
            _tokenManager = tokenManager;
        }

        public IActionResult Index(string returnUrl, string currentUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.CurrentUrl = currentUrl;
            return View();
        }

        public IActionResult Employee()
        {         
            return View();
        }

        public IActionResult ForgetPwd()
        {
            return View();
        }

        public IActionResult ResetPwd(string email, string token)
        {
            try
            {
                var principal = _tokenManager.ValidateToken(token);
                if (principal != null && principal.Identity != null && principal.Identity.IsAuthenticated)
                {
                    // 取得jwt中的郵件
                    var emailClaim = principal.FindFirst(ClaimTypes.Email);
                    if (emailClaim != null)
                    {
                        var emailClaimVal = emailClaim.Value;
                        // 在这里检查邮箱地址是否与请求匹配
                        if (emailClaimVal == email)
                        {
                            var customer = _context.TCustomers.FirstOrDefault(x => x.FEmail == email);

                            if (customer != null)
                            {
                                var resetViewModel = new ResetPasswordViewModel
                                {
                                    token = token,
                                    UserId = customer.FCustomerId,
                                };
                                return View("ResetPwdSuccess", resetViewModel);
                            }
                        }
                        else
                        {
                            return View("ResetPwdFail");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return View("ResetPwdFail");
            }
            return View("ResetPwdFail");
        }
        public string SavePicture(string imageUrl)
        {
            string fileName = Guid.NewGuid().ToString() + ".jpg"; // 使用 Guid 生成檔名
            string imagePath = Path.Combine("wwwroot", "img", "uploads", fileName);

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(imageUrl, imagePath);
                }
                return "img/uploads/" + fileName; // 回傳檔名
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving image from URL: {ex.Message}");
                return null; // 若發生錯誤則回傳 null
            }
        }

        //google第三方登入
        public async Task<IActionResult> ValidGoogleLoginAsync()
        {
            string? formCredential = Request.Form["credential"]; //回傳憑證
            string? formToken = Request.Form["g_csrf_token"]; //回傳令牌
            string? cookiesToken = Request.Cookies["g_csrf_token"]; //Cookie 令牌
            string pic;
            string id;
            // 驗證 Google Token
            GoogleJsonWebSignature.Payload? payload = VerifyGoogleToken(formCredential, formToken, cookiesToken).Result;
            if (payload == null)
            {
                // 驗證失敗
                //ViewData["Msg"] = "驗證 Google 授權失敗";
            }
            else
            {
                if (_context.TCustomers.FirstOrDefault(x => x.FEmail == payload.Email) == null)
                {
                    TCustomer customer = new TCustomer()
                    {
                        FEmail = payload.Email,
                        FName = payload.Name,
                        FImagePath = SavePicture(payload.Picture),
                    };
                    await _context.AddAsync(customer);
                    await _context.SaveChangesAsync();
                }
            }
            pic = _context.TCustomers.FirstOrDefault(x => x.FEmail == payload.Email).FImagePath;
            id = _context.TCustomers.FirstOrDefault(x => x.FEmail == payload.Email).FCustomerId.ToString();
			var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, payload.Name),
                    new Claim(ClaimTypes.Email, payload.Email),
					new Claim(ClaimTypes.Role, "Customer"),
					new Claim(ClaimTypes.Uri, pic),
					new Claim(ClaimTypes.NameIdentifier, id),

				};
            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                //IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = < string >
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authProperties);

            var userInfo = new CusResDTO()
            {
                Success = true,
                FullName = payload.Name,
                Email = payload.Email,
            };

            //return Ok(userInfo);
            return Redirect("/");
        }
        public async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(string? formCredential, string? formToken, string? cookiesToken)
        {
            // 檢查空值
            if (formCredential == null || formToken == null && cookiesToken == null)
            {
                return null;
            }

            GoogleJsonWebSignature.Payload? payload;
            try
            {
                // 驗證 token
                if (formToken != cookiesToken)
                {
                    return null;
                }

                // 驗證憑證
                IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
                string GoogleApiClientId = Config.GetSection("GoogleApiClientId").Value;
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { GoogleApiClientId }
                };
                payload = await GoogleJsonWebSignature.ValidateAsync(formCredential, settings);
                if (!payload.Issuer.Equals("accounts.google.com") && !payload.Issuer.Equals("https://accounts.google.com"))
                {
                    return null;
                }
                if (payload.ExpirationTimeSeconds == null)
                {
                    return null;
                }
                else
                {
                    DateTime now = DateTime.Now.ToUniversalTime();
                    DateTime expiration = DateTimeOffset.FromUnixTimeSeconds((long)payload.ExpirationTimeSeconds).DateTime;
                    if (now > expiration)
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
            return payload;
        }

        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Login(LoginViewModel customer)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            if (customer != null)
        //            {
        //                if (Authenticate(customer.Account, customer.Password))
        //                {                
        //                    var claims = new List<Claim>()
        //                    {
        //                        new Claim(ClaimTypes.Name, customer.Account),
        //                        new Claim("Description", customer.Password)
        //                    };
        //                    var authProperties = new AuthenticationProperties
        //                    {
        //                        //AllowRefresh = <bool>,
        //                        // Refreshing the authentication session should be allowed.

        //                        //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
        //                        // The time at which the authentication ticket expires. A 
        //                        // value set here overrides the ExpireTimeSpan option of 
        //                        // CookieAuthenticationOptions set with AddCookie.

        //                        //IsPersistent = true,
        //                        // Whether the authentication session is persisted across 
        //                        // multiple requests. When used with cookies, controls
        //                        // whether the cookie's lifetime is absolute (matching the
        //                        // lifetime of the authentication ticket) or session-based.

        //                        //IssuedUtc = <DateTimeOffset>,
        //                        // The time at which the authentication ticket was issued.

        //                        //RedirectUri = < string >
        //                        // The full path or absolute URI to be used as an http 
        //                        // redirect response value.
        //                    };
        //                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);                
        //                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authProperties);

        //                    if (TempData["ReturnUrl"] != null && !string.IsNullOrEmpty(TempData["ReturnUrl"].ToString()))
        //                    {
        //                        return Redirect(TempData["ReturnUrl"].ToString());
        //                    }
        //                    return RedirectToAction("Index", "Home", new { area = "Customer" });
        //                }
        //                else
        //                {                       
        //                    return View(customer);
        //                }
        //            }                          
        //        }
        //        return View(customer);
        //    }

        //    public async Task<IActionResult> Logout()
        //    {
        //        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //        return RedirectToAction("Index", "Home", new { area = "Customer" });
        //    }

        //    public bool Authenticate(string email, string password)
        //    {
        //        //驗證用戶是存在
        //        TCustomer customer = _context.TCustomers.FirstOrDefault(x => x.FEmail == email && x.FPassword == password);
        //        if (customer == null)
        //        {
        //            return false;
        //        }
        //        return true;
        //    }
    }
}
