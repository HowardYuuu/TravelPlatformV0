using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjTravelPlatform_release.Areas.CustomizedIdentity.ViewModel.Customer;
using prjTravelPlatformV3.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using prjTravelPlatform_release.Areas.CustomizedIdentity.DTO;
using prjTravelPlatform_release.Areas.CustomizedIdentity.DTO.Req;
using prjTravelPlatform_release.Areas.CustomizedIdentity.DTO.Res;
using System.Drawing;
using System.Text.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Azure.Core;
using NuGet.Common;


namespace prjTravelPlatform_release.Areas.CustomizedIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiLoginController : ControllerBase
    {
        private readonly dbTravalPlatformContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PasswordResetService _passwordResetService;
        private readonly TokenManager _tokenManager;

        public ApiLoginController(dbTravalPlatformContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, PasswordResetService passwordResetService, TokenManager tokenManager)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _passwordResetService = passwordResetService;
            _tokenManager = tokenManager;
            //_httpContextAccessor = httpContextAccessor;
        }

        #region jwtLogin
        //[HttpGet]
        //[Route("test")]
        //[Authorize]
        //public EmpResDTO test()
        //{
        //    return new TokenManager(_configuration, _httpContextAccessor).GetUser();
        //}
        #endregion


        [HttpPost]
        [Route("ForgetPwd")]
        public async Task<IActionResult> ForgetPwd([FromBody] ForgetPwdReqDTO forgetPwdDTO)
        {
            var dbCustomer = _context.TCustomers.Where(c => c.FEmail == forgetPwdDTO.Email).SingleOrDefault();
            if (dbCustomer == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "此Email帳號不存在"
                });
            }
            string resetPasswordUrl = "https://localhost:7119/CustomizedIdentity/Login/ResetPwd";
            await _passwordResetService.SendPasswordResetEmail(forgetPwdDTO.Email, resetPasswordUrl);
            return Ok(new
            {
                success = true,
                message = "Password reset email sent successfully."
            });
        }

        [HttpPost]
        [Route("ResetPwd")]
        public IActionResult ResetPwd([FromBody] ResetPwdReqDTO resetPwdReq)
        {

            if (resetPwdReq != null && resetPwdReq.Password != null && resetPwdReq.Token != null)
            {
                try
                {
                    var principal = _tokenManager.ValidateToken(resetPwdReq.Token);
                    if (principal != null && principal.Identity != null && principal.Identity.IsAuthenticated)
                    {

                        byte[] salt = EncryptionService.GenerateSalt();
                        byte[] combinedBytes = EncryptionService.CombineBytes(Encoding.UTF8.GetBytes(resetPwdReq.Password), salt);
                        byte[] hashedBytes = EncryptionService.ComputeHash(combinedBytes);
                        var customer = _context.TCustomers.FirstOrDefault(c => c.FCustomerId == resetPwdReq.UserId);
                        if (customer != null)
                        {
                            customer.FCustomerId = resetPwdReq.UserId;
                            customer.FPassword = Convert.ToBase64String(hashedBytes);
                            customer.FSalt = Convert.ToBase64String(salt);
                            _context.TCustomers.Update(customer);
                            _context.SaveChanges();
                            return Ok(new
                            {
                                success = true,
                            });
                        }
                        else
                        {
                            return BadRequest(new
                            {
                                success = false,
                                message = "找不到此用戶"
                            });
                        }

                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = ex.Message
                    });
                }
            }

            return BadRequest(new
            {
                success = false,
                message = "請求已過期，請重新申請"
            });

        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] CusReqDTO customer)
        {
            if (ModelState.IsValid)
            {
                var dbCustomer = Authenticate(customer);
                if (dbCustomer == null)
                {
                    return BadRequest(new CusResDTO()
                    {
                        Success = false,
                        Errors = new List<string>(){
                                    "請確認帳號密碼是否正確。"
                                }
                    });
                }
                if(dbCustomer.FImagePath==null || dbCustomer.FImagePath == "")
                {
                    dbCustomer.FImagePath = "img/uploads/avatar.png";
                }   
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, dbCustomer.FName),
                    new Claim(ClaimTypes.Email, dbCustomer.FEmail),
                    new Claim(ClaimTypes.NameIdentifier, dbCustomer.FCustomerId.ToString()),
                    new Claim(ClaimTypes.Role, "Customer"),
                    new Claim(ClaimTypes.Uri,dbCustomer.FImagePath)
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
                    FullName = dbCustomer.FName,
                    Email = dbCustomer.FEmail,
                    CustomerId = dbCustomer.FCustomerId,
                    RedirectUrl = customer.ReturnUrl,
                    CurrentUrl = customer.CurrentUrl,
                };

                return Ok(userInfo);
            }
            var error = new CusResDTO()
            {
                Success = false,
                Errors = new List<string>()
                {
                    "無效的payload"
                }
            };
            return BadRequest(error);
        }

        [HttpPost]
        [Route("empLogin")]
        public async Task<IActionResult> EmpLogin([FromBody] EmpReqDTO employee)
        {
            if (ModelState.IsValid)
            {
                var dbEmp = EmpAuthenticate(employee);
                if (dbEmp == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "請確認帳號密碼是否正確。"
                    });
                }

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, dbEmp.FName),
                    new Claim(ClaimTypes.Email, dbEmp.FEmail),
                    new Claim(ClaimTypes.NameIdentifier, dbEmp.FEmployeeId.ToString()),
                    new Claim(ClaimTypes.Role, "Employee")
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


                var empInfo = new EmpResDTO()
                {
                    Success = true,
                    FullName = dbEmp.FName,
                    Email = dbEmp.FEmail,
                    EmployeeId = dbEmp.FEmployeeId
                };

                return Ok(empInfo);
            }
            return BadRequest(new
            {
                success = false,
                message = "無效的payload"
            });
        }

        [HttpDelete]
        public async Task<string> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return "登出成功";
        }

        [HttpPost]
        [Route("SignUp")]
        public IActionResult SignUp([FromBody] CusSignUpReqDTO cusSignUpReqDTO)
        {
            string _errMsg = "";
            try
            {
                if (cusSignUpReqDTO != null && cusSignUpReqDTO.Password != null)
                {

                    byte[] salt = EncryptionService.GenerateSalt();
                    byte[] combinedBytes = EncryptionService.CombineBytes(Encoding.UTF8.GetBytes(cusSignUpReqDTO.Password), salt);
                    byte[] hashedBytes = EncryptionService.ComputeHash(combinedBytes);

                    //存進資料庫
                    var customer = new TCustomer
                    {
                        FName = cusSignUpReqDTO.Name,
                        FPassword = Convert.ToBase64String(hashedBytes),
                        FEmail = cusSignUpReqDTO.Email,
                        FSalt = Convert.ToBase64String(salt)
                    };

                    _context.TCustomers.Add(customer);
                    _context.SaveChanges();

                    return Ok(new
                    {
                        success = true,
                    });
                }
            }
            catch (Exception ex)
            {
                _errMsg = ex.Message;
            }
            return BadRequest(new
            {
                success = false,
                message = _errMsg,
            });
        }

        public TCustomer? Authenticate(CusReqDTO request)
        {
            //驗證用戶是存在
            TCustomer? customer = _context.TCustomers
                                  .FirstOrDefault(x => x.FEmail == request.Account);
            if (customer != null)
            {
                try
                {
                    byte[] salt = Convert.FromBase64String(customer.FSalt);
                    byte[] combinedBytes = EncryptionService.CombineBytes(Encoding.UTF8.GetBytes(request.Password), salt);
                    byte[] hashedBytes = EncryptionService.ComputeHash(combinedBytes);
                    if (customer.FPassword == Convert.ToBase64String(hashedBytes))
                    {
                        return customer;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }

            }
            return null;
        }

        public TEmployee? EmpAuthenticate(EmpReqDTO request)
        {
            //驗證用戶是存在
            TEmployee? emp = _context.TEmployees
                                  .FirstOrDefault(x => x.FAccountNumber == request.Account);
            if (emp != null)
            {
                try
                {
                    byte[] salt = Convert.FromBase64String(emp.FSalt);
                    byte[] combinedBytes = EncryptionService.CombineBytes(Encoding.UTF8.GetBytes(request.Password), salt);
                    byte[] hashedBytes = EncryptionService.ComputeHash(combinedBytes);
                    if (emp.FPassword == Convert.ToBase64String(hashedBytes))
                    {
                        return emp;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }

            }
            return null;
        }



    }
}
