namespace ServiceBus.OpenSdk
{
    public class SubscriptionClient : MessagingClient
    {
        public SubscriptionClient(string sbnamespace, string entityPath,
      TokenProvider tokenProvider, string protocol) :
       base(sbnamespace, entityPath, tokenProvider, protocol)
        {
        }

        public static SubscriptionClient FromConnectionString(string connStr, string topicName, string name)
        {
            connStr = connStr + ";EntityPath=" + topicName + "/subscriptions/" + name + ";TransportType=http";
            var dict = ParseConnectionString(connStr);
            return new SubscriptionClient(dict[SBNAMESPACE].ToString(), dict[ENTITY].ToString(), (TokenProvider)dict[TOKEN_PROVIDER], dict[PROTOCOL].ToString());
        }
        public static SubscriptionClient FromConnectionString(string connStr, string name)
        {
            connStr = connStr + "/subscriptions/" + name + ";TransportType=http";
            var dict = ParseConnectionString(connStr);
            return new SubscriptionClient(dict[SBNAMESPACE].ToString(), dict[ENTITY].ToString(), (TokenProvider)dict[TOKEN_PROVIDER], dict[PROTOCOL].ToString());
        }
        public static SubscriptionClient FromConnectionString(string connStr, string topicName, string name, string protocol)
        {
            connStr = connStr + ";EntityPath=" + topicName + "/subscriptions/" + name + ";TransportType=" + protocol;
            var dict = ParseConnectionString(connStr);
            return new SubscriptionClient(dict[SBNAMESPACE].ToString(), dict[ENTITY].ToString(), (TokenProvider)dict[TOKEN_PROVIDER], dict[PROTOCOL].ToString());
        }
    }
}
