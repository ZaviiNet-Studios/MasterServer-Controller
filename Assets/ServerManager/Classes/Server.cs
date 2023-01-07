namespace GameServers
{
    public class GameServer
    {
        public string ipAddress;
        public int port;
        public int playerCount;
        public int maxCapacity;
        public string instanceid;


        public GameServer(string ipAddress, int port, int playerCount, int maxCapacity, string instanceid)
        {
            this.ipAddress = ipAddress;
            this.port = port;
            this.playerCount = playerCount;
            this.maxCapacity = maxCapacity;
            this.instanceid = instanceid;
        }
    }
}