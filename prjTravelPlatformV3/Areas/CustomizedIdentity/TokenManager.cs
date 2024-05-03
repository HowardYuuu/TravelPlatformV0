using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using prjTravelPlatform_release.Areas.CustomizedIdentity.DTO.Res;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace prjTravelPlatform_release.Areas.CustomizedIdentity
{
    public class TokenManager
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TokenManager(IConfiguration configuration, IHttpContextAccessor httpContextAccessor) 
        { 
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        private SecurityKey? _SecurityKey;
        public string CreateToken(List<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:KEY"]));

            var jwt = new JwtSecurityToken
            (
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                //使用者資訊
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                //金鑰產生
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            );
            
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return token;
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();

            SecurityToken validatedToken;
            return tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
        }

        private TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["JWT:Issuer"],
                ValidAudience = _configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:KEY"])),
                ClockSkew = TimeSpan.Zero // 设置零以确保 JWT 令牌不会提前过期
            };
        }




        #region
        //public EmpResDTO GetUser()
        //{
        //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:KEY"]));
        //    string? token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"];

        //    if (token == null)
        //    {
        //        return null;
        //    }

        //    var split = token.Split('.');
        //    var iv = split[0];
        //    var encrypt = split[1];
        //    var signature = split[2];
        //    byte[] signatureBytes = Convert.FromBase64String(signature);

        //    string tt = Convert.ToBase64String(securityKey.Key);
        //    //檢查簽章是否正確
        //    //if (signature != TokenCrypto
        //    // .ComputeHMACSHA256(iv + "." + encrypt, securityKey.Substring(0, 64)))
        //    //{
        //    //    return null;
        //    //}

        //    //使用 AES 解密 Payload
        //    var base64 =  TokenCrypto.AESDecrypt(encrypt, securityKey.ToString().Substring(0, 16), iv);
        //    var json = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
        //    var payload = JsonConvert.DeserializeObject<EmpResDTO>(json);

        //    //檢查是否過期
        //    if (payload.Exp < Convert.ToInt32(
        //        (DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds))
        //    {
        //        return null;
        //    }

        //    return payload;
        //}
        #endregion

    }








}

