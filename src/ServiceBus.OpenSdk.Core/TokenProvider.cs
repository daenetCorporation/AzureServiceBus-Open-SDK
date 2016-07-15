// 
//  (c) Microsoft Corporation. See LICENSE.TXT file for licensing details
//  

// #undef MF_FRAMEWORK_VERSION_V4_2 // Contains NET MF4.2 specific code

namespace ServiceBus.OpenSdk
{
    using System;
    using System.Linq;
#if !MF_FRAMEWORK_VERSION_V4_2
    using System.Collections.Generic;
    using Hashtable = System.Collections.Generic.Dictionary<object, object>;
#endif
    using System.IO;
    using System.Text;
    //using ElzeKool;
    using System.Net.Http;

    public class TokenProvider
    {
        protected readonly string issuerName;
        protected readonly string issuerSecret;

        protected readonly string sasKey;
        protected readonly string sasKeyName;

        protected readonly byte[] issuerSecretBytes;
        Hashtable cachedTokens;

        protected class TokenAndExpiration
        {
            public string Token;
            public DateTime ExpirationTime;
        }

        public TokenProvider(string issuerName, string issuerSecret)
        {
            this.issuerName = issuerName;
            this.issuerSecret = issuerSecret;
            this.issuerSecretBytes = Convert.FromBase64String(issuerSecret);
            this.cachedTokens = new Hashtable();
        }

        public virtual string GetToken(string serviceNamespace, string acsHostName, string sbHostName, string path)
        {
            string tokenCacheKey = serviceNamespace + path;
            if (this.cachedTokens.ContainsKey(tokenCacheKey))
            {
                TokenAndExpiration tokenAndExpiration = (TokenAndExpiration)this.cachedTokens[tokenCacheKey];
                if (tokenAndExpiration.ExpirationTime.CompareTo(DateTime.UtcNow) > 0)
                {
                    return tokenAndExpiration.Token;
                }
                else
                {
                    this.cachedTokens.Remove(tokenCacheKey);
                }
            }

            var newTokenAndExpiration = GetTokenNoCache(serviceNamespace, acsHostName, sbHostName, path);

            this.cachedTokens.Add(tokenCacheKey, newTokenAndExpiration);

            return newTokenAndExpiration.Token;
        }

        protected virtual TokenAndExpiration GetTokenNoCache(string serviceNamespace, string acsHostName, string sbHostName, string path)
        {
            throw new NotSupportedException();
            //var acsEndpoint = "https://" + serviceNamespace + "-sb." + acsHostName + "/WRAPv0.9/";
            //var realm = "http://" + serviceNamespace + "." + sbHostName + path;
            //var values = new Hashtable {{"wrap_name", this.issuerName}, {"wrap_password", this.issuerSecret}, {"wrap_scope", realm}};
            //var responseString = ExecuteRequest(acsEndpoint, values);
            //var responseProperties = responseString.Split('&');
            ////var tokenProperty = responseProperties[0].Split('=');

            //var tokenAndExpiration = new TokenAndExpiration();

            //foreach(var responseProperty in responseProperties)
            //{
            //    var tokenProperty = responseProperty.Split('=');

            //    if (String.Equals(tokenProperty[0], "wrap_access_token"))
            //    {
            //        tokenAndExpiration.Token = "WRAP access_token=\"" + HttpUtility.UrlDecode(tokenProperty[1]) + "\"";
            //    }
            //    else if (String.Equals(tokenProperty[0], "wrap_access_token_expires_in"))
            //    {
            //        uint tokenExpiresIn = Convert.ToUInt32(tokenProperty[1]);
            //        tokenAndExpiration.ExpirationTime = DateTime.UtcNow.AddSeconds(tokenExpiresIn - 60); // Treat as expired 60 seconds earlier
            //    }
            //}
            //if (tokenAndExpiration.Token == null)
            //{
            //    throw new Exception("Unexpected token format from ACS");
            //}

            //return tokenAndExpiration;
        }

        //static string ExecuteRequest(string webEndpoint, Hashtable values)
        //{
        //    var wr = (HttpWebRequest) WebRequest.Create(webEndpoint);
        //    wr.Method = "POST";
        //    wr.ContentType = "application/x-www-form-urlencoded";
        //    var payload = string.Empty;
        //    foreach (string key in values.Keys)
        //    {
        //        payload += HttpUtility.UrlEncode(key) + "=" + HttpUtility.UrlEncode((string) values[key]) + "&";
        //    }
        //    var bytes = Encoding.UTF8.GetBytes(payload.Substring(0, payload.Length - 1));
        //    wr.ContentLength = bytes.Length;
        //    var rqs = wr.GetRequestStream();
        //    rqs.Write(bytes, 0, bytes.Length);
        //    rqs.Close();

        //    var wq = wr.GetResponse();
        //    var wrs = wq.GetResponseStream();


        //    var responseString = new StreamReader(wrs).ReadToEnd();
        //    wq.Close();
        //    return responseString;
        //}

        private static string createSasToken(string resourceUri, string keyName, string key)
        {
            //HMACSHA256 a;
            //TimeSpan sinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
            //var expiry = Convert.ToString((int)sinceEpoch.TotalSeconds + 3600); //EXPIRES in 1h 
            //string stringToSign = HttpUtility.UrlEncode(resourceUri) + "\n" + expiry;
            //HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));

            //var hash = SHA.computeHMAC_SHA256(Encoding.UTF8.GetBytes(stringToSign));
            //var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
            //var sasToken = String.Format(CultureInfo.InvariantCulture,
            //"SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}",
            //    HttpUtility.UrlEncode(resourceUri), HttpUtility.UrlEncode(signature), expiry, keyName);

            //return sasToken;
            return null;
        }

        //public string GetSasKey()
        //{
        //    var sasToken = String.Format(CultureInfo.InvariantCulture,
        //   "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}",
        //       HttpUtility.UrlEncode(resourceUri), HttpUtility.UrlEncode(signature), expiry, keyName);

        //    //SharedAccessSignature sr=http%3A%2F%2Ffrankfurt.servicebus.windows.net%2Fsamplequeue&sig=g5uwloG3OA4im1M9SKyQMWY%2FdlYWdWg%2BHqmky8c%2BMzU%3D&se=1429869869.324&skn=device_send_listen

        //}

        public string GetToken(Uri serviceNamespace)
        {
            var host = serviceNamespace.Host;
            var @namespace = host.Substring(0, host.IndexOf("."));
            var token = this.GetToken(@namespace, "accesscontrol.windows.net", "servicebus.windows.net", serviceNamespace.AbsolutePath);

            return token;
        }

        public void SignRequest(HttpClient httpClient, HttpContent wq)
        {
            var memStream = new MemoryStream();
            var signedHeaders = String.Empty;
            foreach (var headerName in wq.Headers)
            {
                signedHeaders += headerName + "&";
                var headerValue = headerName.Value.GetEnumerator().Current;
                var buffer = Encoding.UTF8.GetBytes(headerValue);
                memStream.Write(buffer, 0, buffer.Length);
            }
            memStream.Flush();

            var hmac = SHA.computeHMAC_SHA256(this.issuerSecretBytes, memStream.ToArray());
            var signatureString = Convert.ToBase64String(hmac);

#if MF_FRAMEWORK_VERSION_V4_2
            // Adjust for .NET MF 4.2 character set difference
            signatureString = Base64NetMf42ToRfc4648(signatureString);
#endif

            signedHeaders += HttpUtility.UrlEncode(signatureString);
            httpClient.DefaultRequestHeaders.Add("Authorization", "SignedHeaders " + signedHeaders);
        }

        public bool ValidateResponse(HttpResponseMessage wr)
        {
            string signature;
            IEnumerable<string> values;
            if (wr.Headers.TryGetValues("Signature", out values))
            {
                signature = values.First();
            }
            else
            {
                return false;
            }

            var memStream = new MemoryStream();
            var signatureElements = signature.Split('&');
            foreach (var sigEl in signatureElements)
           
            {
                string headerValue;
                if (wr.Headers.TryGetValues(sigEl, out values))
                {
                    headerValue = values.First();
                    var buffer = Encoding.UTF8.GetBytes(headerValue);
                    memStream.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    return false;
                }
             
            }
            memStream.Flush();

            var hmac = SHA.computeHMAC_SHA256(this.issuerSecretBytes, memStream.ToArray());

            var signatureString = HttpUtility.UrlDecode(signatureElements[signatureElements.Length - 1]);

#if MF_FRAMEWORK_VERSION_V4_2
            // Adjust for .NET MF 4.2 character set difference
            signatureString = Base64Rfc4648ToNetMf42(signatureString);
#endif

            var fmac = Convert.FromBase64String(signatureString);
            if (hmac.Length !=
                fmac.Length)
            {
                return false;
            }
            for (var i = 0; i < hmac.Length; i++)
            {
                if (fmac[i] !=
                    hmac[i])
                {
                    return false;
                }
            }
            return true;
        }

#if MF_FRAMEWORK_VERSION_V4_2

        // Fix up base64 encoded strings for NetMF4.2 (see http://netmf.codeplex.com/workitem/1388)
        // - NetMF4.2 uses * instead of / and ! instead of +
        // - NetMF4.3 uses RFC4648 standard characters (/ and !), just like regular .NET Framework.

        protected static string Base64NetMf42ToRfc4648(string base64netMf)
        {
            var base64Rfc = string.Empty;
            
            for (var i = 0; i < base64netMf.Length; i++)
            {
                if (base64netMf[i] == '!')
                {
                    base64Rfc += '+';
                }
                else if (base64netMf[i] == '*')
                {
                    base64Rfc += '/';
                }
                else
                {
                    base64Rfc += base64netMf[i];
                }
            }
            return base64Rfc;
        }

        protected static string Base64Rfc4648ToNetMf42(string base64Rfc)
        {
            var base64NetMf = string.Empty;
            for (var i = 0; i < base64Rfc.Length; i++)
            {
                if (base64Rfc[i] == '/')
                {
                    base64NetMf += '*';
                }
                else if (base64Rfc[i] == '+')
                {
                    base64NetMf += '!';
                }
                else
                {
                    base64NetMf += base64Rfc[i];
                }
            }
            return base64NetMf;
        }
#endif

    }
}