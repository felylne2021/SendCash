using Microsoft.IdentityModel.Tokens;
using SendCash.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SendCash.ViewModels;
using Microsoft.AspNetCore.Http;

namespace SendCash.Controllers {
    public class ValidateTokenHandler : DelegatingHandler{

        private SendCashEntities db = new SendCashEntities();

        private static bool TryRetrieveToken(HttpRequestMessage httpRequest, out string token) {

            token = null;
            IEnumerable<string> authHeaders;

            if (!httpRequest.Headers.TryGetValues("Authorization", out authHeaders) || authHeaders.Count() > 1)
                return false;

            var bearerToken = authHeaders.ElementAt(0);
            token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;

            return true;

        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequest, CancellationToken cancellationToken) {

            HttpStatusCode statusCode;

            string token;

            //jwt exists?
            if(!TryRetrieveToken(httpRequest, out token)) {
                statusCode = HttpStatusCode.Unauthorized;

                // allow no token with claimauthorization
                return base.SendAsync(httpRequest, cancellationToken);
            }

            try {
                const string sec = "bca123!";
                var now = DateTime.UtcNow;
                var securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(sec));                

                SecurityToken securityToken;
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                TokenValidationParameters validationParameters = new TokenValidationParameters{

                    //ValidAudience = "https://localhost:50191",
                    //ValidIssuer = "https://localhost:50191",
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    LifetimeValidator = this.LifetimeValidator,
                    IssuerSigningKey = securityKey

                };

                // extract and assign the user of JWT
                Thread.CurrentPrincipal = handler.ValidateToken(token, validationParameters, out securityToken);
                HttpContext.Current.User = handler.ValidateToken(token, validationParameters, out securityToken);

                return base.SendAsync(httpRequest, cancellationToken);

            }
            catch (SecurityTokenValidationException e) {
                
                statusCode = HttpStatusCode.Unauthorized;
            }
            catch (Exception ex) {
                statusCode = HttpStatusCode.InternalServerError;
            }

            return Task <HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(statusCode) { });
        }

        public bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
            {

            if (expires != null)
                if (DateTime.UtcNow < expires) return true;

            return false;
        }

        [HttpGet]
        public Object GetToken(LoginViewModel model) {           

            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string key = "bcabca123123123!"; //Secret key which will be used later during validation    
            var issuer = domainName;  //normally this will be your site URL    

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Create a List of Claims, Keep claims name short              

            var permClaims = new List<Claim>();
            permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            permClaims.Add(new Claim("valid", "1"));
            permClaims.Add(new Claim("AccountName", model.AccountName));
            permClaims.Add(new Claim("AccountNumber", model.AccountNumber));
            permClaims.Add(new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()));

            //Create Security Token object by giving required parameters    
            var token = new JwtSecurityToken(issuer, //Issure    
                            issuer,  //Audience    
                            permClaims,
                            DateTime.Now,
                            expires: DateTime.Now.AddDays(1),                            
                            signingCredentials: credentials);


            var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);
            CookieOptions cookieOptions = new CookieOptions {
                Expires = DateTimeOffset.UtcNow.AddDays(1),//you can set this to a suitable timeframe for your situation 
                Domain = issuer,
                Path = "/"
            };
            
            // yang disini bermaslaha .. 
            // IHttpContextAccessor.Response.Cookies.Append("first_request", DateTime.Now.ToString(), cookieOptions);
            


            return new { data = jwt_token };
        }

    }
}