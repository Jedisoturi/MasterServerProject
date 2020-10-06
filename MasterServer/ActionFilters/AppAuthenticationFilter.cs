using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
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
            //string signature;
            var request = context.HttpContext.Request;
            //request.Headers.TryGetValue("signature", out StringValues sv);
            //string signature = sv.FirstOrDefault();
            string signature = context.ActionArguments.SingleOrDefault(p => p.Key == "signature").Value.ToString();

            if (signature == null) throw new Exception($"No signature provided");

            // Remove the signature parameter from the url
            var url = request.GetEncodedUrl();
            int indexQuestionmark = url.LastIndexOf("?");
            int indexAnd = url.LastIndexOf("&");
            int index = indexAnd != -1 ? indexAnd : indexQuestionmark;
            url = url.Substring(0, index);  // TODO: remove check

            // Read body as json
            var body = new StreamReader(request.Body);
            //The modelbinder has already read the stream and need to reset the stream index
            body.BaseStream.Seek(0, SeekOrigin.Begin);
            var bodyString = (await body.ReadToEndAsync()).ToLower();

            // Check signature
            var mySignature = Encode(url + bodyString, Constants.secret);
            if (mySignature != signature) throw new Exception($"Incorrect signature");

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
