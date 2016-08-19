namespace ServiceBus.OpenSdk
{
    public class QueueClient : MessagingClient
    {
        public QueueClient(string sbnamespace, string entityPath, 
            TokenProvider tokenProvider, string protocol) : 
            base(sbnamespace, entityPath, tokenProvider, protocol)
        {

        }
        
        public static QueueClient FromConnectionString(string connStr)
        {
            connStr = connStr + ";TransportType=" + "http";
            var dict = ParseConnectionString(connStr);
            return new QueueClient(dict[SBNAMESPACE].ToString(), dict[ENTITY].ToString(), (TokenProvider)dict[TOKEN_PROVIDER], dict[PROTOCOL].ToString());
        }

        public static QueueClient FromConnectionString(string connStr, string path)
        {
            
            connStr = connStr + ";EntityPath=" + path;
            return FromConnectionString(connStr);
        }
        public static QueueClient FromConnectionString(string connStr, string path, string protocol)
        {   
            connStr = connStr + ";EntityPath=" + path+";TransportType="+protocol;
            var dict = ParseConnectionString(connStr);
            return new QueueClient(dict[SBNAMESPACE].ToString(), dict[ENTITY].ToString(), (TokenProvider)dict[TOKEN_PROVIDER], dict[PROTOCOL].ToString());
        }
    }
}
