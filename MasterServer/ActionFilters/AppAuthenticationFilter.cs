using MasterServer.ErrorHandling;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MasterServer
{
    public class AppAuthenticationFilter : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var request = context.HttpContext.Request;

            // Get timeStamp
            if (!request.Headers.TryGetValue("TimeStamp", out StringValues timeStampValue)) throw new AppAuthenticationException($"No timeStamp provided");
            string timeStampString = timeStampValue.FirstOrDefault();



            // Check if timeout
            if (!DateTime.TryParse(timeStampString, out DateTime timeStamp)) throw new AppAuthenticationException($"Invalid TimeStamp");
            Console.WriteLine(timeStampString);
            Console.WriteLine(DateTime.UtcNow.ToString());
            if (timeStamp.AddMinutes(1) < DateTime.UtcNow) throw new Exception($"Request is too old");

            // Get signature
            if (!request.Headers.TryGetValue("Signature", out StringValues signatureValue)) throw new AppAuthenticationException($"No signature provided");
            string signature = signatureValue.FirstOrDefault();

            // Read url
            var url = request.GetEncodedUrl();

            // Read body as json
            var body = new StreamReader(request.Body);
            //The modelbinder has already read the stream and need to reset the stream index
            body.BaseStream.Seek(0, SeekOrigin.Begin);
            var bodyString = (await body.ReadToEndAsync()).ToLower();

            // Check signature
            var mySignature = Encode(url + timeStampString + bodyString, Constants.secret);
            if (mySignature != signature) throw new AppAuthenticationException($"Incorrect signature");

            await base.OnActionExecutionAsync(context, next);
        }

        private static string Encode(string input, byte[] key)
        {
            using (var myhmacsha1 = new HMACSHA256(key))
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(input.ToLower());
                byte[] hash = myhmacsha1.ComputeHash(byteArray);
                return Convert.ToBase64String(hash, 0, hash.Length);
            }
        }
    }
}
