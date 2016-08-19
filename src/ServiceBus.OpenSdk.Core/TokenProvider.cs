namespace ServiceBus.OpenSdk
{
    using System;
    using System.Linq;      
    using System.Collections.Generic;
    using Hashtable = System.Collections.Generic.Dictionary<object, object>;
    using System.IO;
    using System.Text; 
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
          
        }

        private static string createSasToken(string resourceUri, string keyName, string key)
        {
            return null;
        }

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
    }
}