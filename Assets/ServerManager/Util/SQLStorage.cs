// using System;
// using MySql.Data.MySqlClient;
// using UnityEngine;
//
// namespace GameServers
// {
//     public class SQLStorage : MonoBehaviour
//     {
//
//        
//         private void Start()
//         {
//
//             Debug.Log("Servers: " + servers.Length);
//
//             // Open a connection to the database
//             using (MySqlConnection connection = new MySqlConnection(DatabaseConnectionInfo.ConnectionString))
//             {
//                 connection.Open();
//
//                 // Iterate over the list of servers
//                 foreach (GameServer server in servers)
//                 {
//                     // Check if the server already exists in the database
//                     string sql = "SELECT COUNT(*) FROM game_servers WHERE ip_address=@ipAddress AND port=@port";
//                     MySqlCommand cmd = new MySqlCommand(sql, connection);
//                     cmd.Parameters.AddWithValue("@ipAddress", server.ipAddress);
//                     cmd.Parameters.AddWithValue("@port", server.port);
//                     int count = Convert.ToInt32(cmd.ExecuteScalar());
//
//                     if (count == 0)
//                     {
//                         // Server does not exist, insert it into the database
//                         sql = "INSERT INTO game_servers (ip_address, port, player_count, max_capacity) " +
//                               "VALUES (@ipAddress, @port, @playerCount, @maxCapacity)";
//                         cmd = new MySqlCommand(sql, connection);
//                         cmd.Parameters.AddWithValue("@ipAddress", server.ipAddress);
//                         cmd.Parameters.AddWithValue("@port", server.port);
//                         cmd.Parameters.AddWithValue("@playerCount", server.playerCount);
//                         cmd.Parameters.AddWithValue("@maxCapacity", server.maxCapacity);
//                         cmd.ExecuteNonQuery();
//                     }
//                 }
//             }
//
//
//         }
//
//         private void Update()
//         {
//             // Check for client requests to join a game server
//             if (ClientRequestedToJoin())
//             {
//                 // Select a game server for the client to join
//                 using (var connection = new MySqlConnection(DatabaseConnectionInfo.ConnectionString))
//                 {
//                     connection.Open();
//                     using (var command = connection.CreateCommand())
//                     {
//                         command.CommandText =
//                             "SELECT * FROM game_servers WHERE player_count < max_capacity ORDER BY player_count ASC LIMIT 1";
//                         using (var reader = command.ExecuteReader())
//                         {
//                             if (reader.Read())
//                             {
//                                 string ipAddress = reader.GetString("ip_address");
//                                 int port = reader.GetInt32("port");
//
//                                 // Send the IP address and port of the selected game server to the client
//                                 SendResponseToClient(ipAddress, port);
//                             }
//                             else
//                             {
//                                 // No game servers are available
//                                 SendErrorToClient("No game servers are available");
//                             }
//                         }
//                     }
//                 }
//             }
//         }
//
//         bool ClientRequestedToJoin()
//         {
//             // Return true if a client has requested to join a game server, false otherwise
//
//             return true;
//         }
//
//         void SendResponseToClient(string ipAddress, int port)
//         {
//             // Send the IP address and port of the selected game server to the client
//         }
//
//         void SendErrorToClient(string message)
//         {
//             // Send an error message to the client
//         }
//     }
// }
