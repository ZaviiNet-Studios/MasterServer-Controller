using System;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using MySql.Data.MySqlClient;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameServers
{
    public class ServerStopper : MonoBehaviour
    {
        [SerializeField] private string Endpoint = "tcp://localhost:2375";
        
        private DockerClient _client;
        [SerializeField] private ServerStarter serverStarter;
        
        private void Start()
        {
            _client = new DockerClientConfiguration(new Uri(Endpoint)).CreateClient();
        }
        
        /// <summary>
        /// Gets the container id of the server
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        private async Task<int> GetPlayerCount(string instanceId)
        {
            using (MySqlConnection connection = new MySqlConnection(DatabaseConnectionInfo.ConnectionString))
            {
                // Open the connection
                await connection.OpenAsync();

                // Create a new command to select the player count for the specified game server
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT player_count FROM game_servers WHERE instanceid = @instanceId";
                    command.Parameters.AddWithValue("@instanceId", instanceId);

                    // Execute the command and retrieve the player count
                    object result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }


        [Button]
        public void RemoveServers()
        {
            RemoveIdleServers();
        }

        private async Task RemoveIdleServers()
        {
            using (MySqlConnection connection = new MySqlConnection(DatabaseConnectionInfo.ConnectionString))
            {
                // Open the connection
                await connection.OpenAsync();

                // Create a new command to select all game servers and their instance IDs
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT instanceid FROM game_servers";

                    // Execute the command and retrieve the server instance IDs
                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Get the instance ID of the current game server
                            string instanceId = reader.GetString("instanceid");
                           

                            // Check if the player count of the current game server is 0
                            if (await GetPlayerCount(instanceId) == 0)
                            {
                                Debug.Log("Server is idle, Removing... " + instanceId);
                                // Stop the container with the matching instance ID
                                await _client.Containers.StopContainerAsync(instanceId, new ContainerStopParameters());
                                Debug.Log("Server" + instanceId + " Stopped!");

                                // Remove the container with the matching instance ID
                                await _client.Containers.RemoveContainerAsync(instanceId, new ContainerRemoveParameters());
                                Debug.Log("Server" + instanceId + " Removed");
                                // Close the reader before executing the DELETE query
                                reader.Close();
                                
                                // Delete the record of the stopped and removed game server from the database
                                string deleteSql = "DELETE FROM game_servers WHERE instanceid = @instanceid";

                                using (MySqlCommand deleteCmd = new MySqlCommand(deleteSql, connection))
                                {
                                    deleteCmd.Parameters.AddWithValue("@instanceid", instanceId);

                                    deleteCmd.ExecuteNonQuery();
                                }
                                reader.Close();
                                Debug.Log("Removed Server " + instanceId);
                                
                                serverStarter._addedServers.RemoveAll(server => server.instanceid == instanceId);
                                
                                
                            }
                        }
                    }
                }
            }
        }

    }
}