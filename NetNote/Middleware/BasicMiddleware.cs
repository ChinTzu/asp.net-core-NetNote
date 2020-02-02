using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace NetNote.Middleware
{
    public class BasicMiddleware
    {
        private readonly RequestDelegate _next;
        public const string AuthorizationHeader = "Authorization";
        public const string WWWAuthenticateHeader = "WWW-Authenticate";
        private BasicUser _user;
        public BasicMiddleware(RequestDelegate next,BasicUser user)
        {
            _next = next;
            _user = user;
        }

        public Task Invoke(HttpContext httpContext)
        {
            var Request = httpContext.Request;
            string auth = Request.Headers[AuthorizationHeader];
            if (auth == null)
            {
                return BasicResult(httpContext);
            }
            // Get Base64 and decode it into a string
            string[] authParts = auth.Split(' ');
            if (authParts.Length != 2)
                return BasicResult(httpContext);
            string base64 = authParts[1];
            string authValue;
            try
            {
                byte[] bytes = Convert.FromBase64String(base64);
                authValue = Encoding.ASCII.GetString(bytes);
            }
            catch
            {
                authValue = null;
            }
            if (string.IsNullOrEmpty(authValue))
                return BasicResult(httpContext);

            // parse username and password
            string userName;
            string password;
            int sepIndex = authValue.IndexOf(':');
            if (sepIndex == -1)
            {
                userName = authValue;
                password = string.Empty;
            }
            else
            {
                userName = authValue.Substring(0, sepIndex);
                password = authValue.Substring(sepIndex + 1);
            }
            // check the username and password
            if (_user.UserName.Equals(userName) && _user.Password.Equals(password))
                return _next(httpContext);
            else
                return BasicResult(httpContext);
        }
        /// <summary>
        /// authentication output required
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private static Task BasicResult(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 401;
            httpContext.Response.Headers.Add(WWWAuthenticateHeader, "Basic realm=\"localhost\"");
            return Task.FromResult(httpContext);
        }
    }

    public static class BasicMiddlewareExtensions
    {
        public static IApplicationBuilder UseBasicMiddleware(this IApplicationBuilder builder,BasicUser user)
        {
            if(user==null)
                throw new ArgumentException("Need to set up Basic user");
            return builder.UseMiddleware<BasicMiddleware>(user);
        }
    }
}
