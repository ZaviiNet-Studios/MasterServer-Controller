using System;
using System.Collections;
using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameServers
{
    public class InMemoryService: MonoBehaviour
    {
        
        private bool saveDataFlag = true;

        [SerializeField] private ServerStarter serverStarter;


        private void Start()
        {
            var addServer = AddServer();
            addServer.Wait();
        }

        [Button]
        public void AddSever()
        {
            var addServer = AddServer();
            addServer.Wait();
        }

        private async Task AddServer()
        {
            bool allServersFull = await AreAllServersFull();
            if (allServersFull)
            {
                Debug.Log("All servers are full");
                saveDataFlag = true;
                serverStarter.StartServer();
                Debug.Log("Adding Server");
            }
            else
            {
                Debug.Log("Server is not full Not adding server");
            }
        }

        /// <summary>
        /// Save game servers to database, Use UpdateServer("123.456.789.0", 7777); to update server
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        public void UpdateServer(string ipAddress, int port)
        {
            GameServer server = serverStarter._addedServers.Find(s => s.ipAddress == ipAddress && s.port == port);
            if (server != null)
            {
                server.playerCount++;
            }
        }
        
        [Button]
        // Called whenever a player joins or leaves a game server
        public void UpdateGameServerData()
        {
            saveDataFlag = true;
        }


        private async Task<bool> AreAllServersFull()
        {
            using (MySqlConnection connection = new MySqlConnection(DatabaseConnectionInfo.ConnectionString))
            {
                // Open the connection
                await connection.OpenAsync();

                // Create a new command to select the player count and maximum capacity for all game servers
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT player_count, max_capacity FROM game_servers";

                    // Execute the command and iterate over the results
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int playerCount = reader.GetInt32(0);
                            int maxCapacity = reader.GetInt32(1);

                            // If any server has not reached maximum capacity, return false
                            if (playerCount < maxCapacity)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            // If all servers have reached maximum capacity, return true
            return true;
        }
        
        //Not used yet
        public IEnumerator SaveGameServersToDatabase()
        {
            while (true)
            {
                // If saveDataFlag is true, save game server data to database
                if (saveDataFlag)
                {
                    // Save game server data to database
                    SaveGameServerData();
                    
                }

                // Wait for 5 seconds before checking again
                yield return new WaitForSeconds(5f);
            }
        }

        public void SaveGameServerData()
        {
            using (MySqlConnection connection = new MySqlConnection(DatabaseConnectionInfo.ConnectionString))
            {
                connection.Open();
        
                // Insert new game servers or update player count of existing game servers
                foreach (GameServer server in serverStarter._addedServers)
                {
                    string ipAddress = server.ipAddress;
                    int port = server.port;
                    int playerCount = server.playerCount;
                    int maxCapacity = server.maxCapacity;
                    string instanceid = server.instanceid;

                    // Check if a row with the same IP address and port already exists in the table
                    string selectSql = "SELECT COUNT(*) FROM game_servers WHERE ip_address = @ip_address AND port = @port";
                    using (MySqlCommand selectCmd = new MySqlCommand(selectSql, connection))
                    {
                        selectCmd.Parameters.AddWithValue("@ip_address", ipAddress);
                        selectCmd.Parameters.AddWithValue("@port", port);
                        int count = Convert.ToInt32(selectCmd.ExecuteScalar());

                        if (count == 0)
                        {
                            // No rows with the same IP address and port were found, so insert a new row
                            string insertSql =
                                "INSERT INTO game_servers (ip_address, port, player_count, max_capacity, instanceid) " +
                                "VALUES (@ip_address, @port, @player_count, @max_capacity, @instancedid)";

                            using (MySqlCommand insertCmd = new MySqlCommand(insertSql, connection))
                            {
                                insertCmd.Parameters.AddWithValue("@ip_address", ipAddress);
                                insertCmd.Parameters.AddWithValue("@port", port);
                                insertCmd.Parameters.AddWithValue("@player_count", playerCount);
                                insertCmd.Parameters.AddWithValue("@max_capacity", maxCapacity);
                                insertCmd.Parameters.AddWithValue("@instancedid", instanceid);

                                insertCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                // Reset saveDataFlag
                saveDataFlag = false;
            }
        }
    }
}