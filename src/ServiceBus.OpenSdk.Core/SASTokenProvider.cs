//=======================================================================================
// Copyright © daenet GmbH Frankfurt am Main
//
// LICENSED UNDER THE APACHE LICENSE, VERSION 2.0 (THE "LICENSE"); YOU MAY NOT USE THESE
// FILES EXCEPT IN COMPLIANCE WITH THE LICENSE. YOU MAY OBTAIN A COPY OF THE LICENSE AT
// http://www.apache.org/licenses/LICENSE-2.0
// UNLESS REQUIRED BY APPLICABLE LAW OR AGREED TO IN WRITING, SOFTWARE DISTRIBUTED UNDER THE
// LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, EITHER EXPRESS OR IMPLIED. SEE THE LICENSE FOR THE SPECIFIC LANGUAGE GOVERNING
// PERMISSIONS AND LIMITATIONS UNDER THE LICENSE.
//=======================================================================================
namespace ServiceBus.OpenSdk
{
    using System;
    using System.Text;

    public class SASTokenProvider : TokenProvider
    {

        string keyName { get { return base.issuerName; } }
        string keySecret { get { return base.issuerSecret; } }
        uint tokenExpiryInSeconds = 1200;

        const long ticksPerSecond = 1000000000 / 100; // 1 tick = 100 nano seconds

        public SASTokenProvider(string keyName, string keySecret, uint tokenExpiryInSeconds = 1200) 
            : base(keyName, keySecret)
        {
            this.tokenExpiryInSeconds = tokenExpiryInSeconds;
        }

        protected override TokenAndExpiration GetTokenNoCache(string serviceNamespace, string acsHostName, string sbHostName, string path)
        {
            string uri = "http://" + serviceNamespace + "." + sbHostName + path;
            var expiry = GetExpiry(this.tokenExpiryInSeconds); 
            string stringToSign = HttpUtility.UrlEncode(uri) + "\n" + expiry;

            var hmac = SHA.computeHMAC_SHA256(Encoding.UTF8.GetBytes(this.keySecret), Encoding.UTF8.GetBytes(stringToSign));
            string signatureString = Convert.ToBase64String(hmac);
            var tokenAndExpiration = new TokenAndExpiration();

            tokenAndExpiration.ExpirationTime = DateTime.UtcNow.AddSeconds(this.tokenExpiryInSeconds - 60); // Treat as expired 60 seconds earlier

            tokenAndExpiration.Token = "SharedAccessSignature sr=" + HttpUtility.UrlEncode(uri) + "&sig=" + HttpUtility.UrlEncode(signatureString) + "&se=" + expiry + "&skn=" + this.keyName;
            return tokenAndExpiration;
        }

        static uint GetExpiry(uint tokenLifetimeInSeconds) 
        { 
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0); 
            TimeSpan diff = DateTime.Now.ToUniversalTime() - origin; 
            return ((uint) (diff.Ticks / ticksPerSecond)) + tokenLifetimeInSeconds; 
        } 

    }
}