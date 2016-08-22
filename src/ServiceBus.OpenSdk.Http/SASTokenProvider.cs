namespace ServiceBus.OpenSdk
{
    using System;
    using System.Text;
    //using ElzeKool;

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
            var expiry = GetExpiry(this.tokenExpiryInSeconds); // Set token lifetime to 20 minutes. 
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