namespace GameServers
{
    public class DatabaseConnectionInfo
    {
        public static string Host => "localhost";
        public static string Database => "DATABASE";
        public static string Username => "root";
        public static string Password => "Password";

        public static string ConnectionString => $"server={Host};database={Database};uid={Username};pwd={Password};SslMode=none";
        //ToDo : Add SSL Mode
    }
}