using System;
using System.Collections.Generic;
using Docker.DotNet;
using Docker.DotNet.Models;
using GameServers;
using UnityEngine;


public class ServerStarter : MonoBehaviour
{
    [SerializeField] private InMemoryService ims;
    // Replace this with the path to your Docker image
    [SerializeField] private string ImagePath = "YOUR UNIY SERVER IMAGE:Tag";

    // Replace this with your Docker API endpoint (e.g. "unix:///var/run/docker.sock")
    [SerializeField] private string Endpoint = "tcp://localhost:2375";
    
    static string host => "IP SERVER HOST";

    //Docker Client
    private DockerClient _client;
    
    //List of GameServers
    public List<GameServer> _addedServers = new List<GameServer>();
    
    // Your Dockers Container Port
    public int containerPort = 7777;
    
    
    [SerializeField] private bool standbyServersRunning = false;
    
    [SerializeField] private int numServers = 0;
    public int _portPoolStart = 56100;

    private void Start()
    {
        // Create a new Docker client using the specified endpoint
        _client = new DockerClientConfiguration(new Uri(Endpoint)).CreateClient();
    }

    public async void StartServer()
    {
        numServers++;
        Debug.Log($"Creating Standby Server:{numServers} on {_portPoolStart}");
        // Create a new container using the specified image and configuration options
        var response = await _client.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = ImagePath,
            Name = $"Server-Instance-{numServers}",
            ExposedPorts = new Dictionary<string, EmptyStruct>
            {
                { containerPort+"/udp", default }
            },
            HostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    { _portPoolStart.ToString()+"/udp", new List<PortBinding> { new PortBinding { HostPort = _portPoolStart.ToString()+"/udp" } } }
                }
            }
        });
        Debug.Log($"Created GameSever: {host}:{_portPoolStart} with ID: {response.ID}");

        // Start the container
        await _client.Containers.StartContainerAsync(response.ID, new ContainerStartParameters());
        _addedServers.Add(new GameServer(host, _portPoolStart, 0, 16, response.ID));

        // Increment the port pool
        _portPoolStart++;
        
        ims.SaveGameServerData();
    }
}