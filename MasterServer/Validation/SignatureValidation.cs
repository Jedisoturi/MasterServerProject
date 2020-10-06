using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
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
    public class SignatureValidation
    {
        public async static Task ValidateSignature(HttpRequest request, string signature = null)
        {
            if (signature == null)
            {
                Console.WriteLine("No signature provided");
                throw new Exception($"No signature provided");
            }

            var url = request.GetEncodedUrl();
            var body = new StreamReader(request.Body);
            //The modelbinder has already read the stream and need to reset the stream index
            body.BaseStream.Seek(0, SeekOrigin.Begin);

            // Remove the signature parameter from the url
            int indexQuestionmark = url.LastIndexOf("?");
            int indexAnd = url.LastIndexOf("&");
            int index = indexAnd != -1 ? indexAnd : indexQuestionmark;
            if (index > 0) url = url.Substring(0, index);  // TODO: remove check

            Console.WriteLine(signature);

            Console.WriteLine(url);
            var bodyString = (await body.ReadToEndAsync()).ToLower();
            Console.WriteLine(bodyString);

            var encryption = Encode(url + bodyString, Constants.secret);
            Console.WriteLine(encryption);
            if (encryption != signature) throw new Exception($"Incorrect signature");
        }

        public static string Encode(string input, byte[] key)
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
