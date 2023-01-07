
# MasterServer-Controller

Introduction:
This project contains scripts for managing game servers using Docker containers. The scripts are written in C# and are intended to be used in a Unity project.


## Features

- Start and stop Docker containers running game servers
- Connect to a MySQL database to store and retrieve data about the game servers
- Remove idle game servers (i.e. servers with 0 players)




## Prerequisites

- Docker must be installed on the machine running the scripts
- A MySQL server must be running and accessible from the machine running the scripts


## Getting Started

1) Clone the repository and open the project in Unity
2) Modify the connection string in the DatabaseManager script to match your MySQL server setup
3) Modify the ImagePath and PortPoolStart variables in the ServerStarter script to match your desired Docker image and starting port for the game servers
4) Attach the ServerStarter and ServerStopper scripts to a game object in your scene
5) Use the StartServer() and StopServer() methods of the ServerStarter and ServerStopper scripts to start and stop game servers


## API Reference

#### The ServerStarter script is responsible for starting Docker containers running game servers.



| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `ImagePath` | `string` | **Required**. The path to the Docker image to use for the game servers |
| `PortPoolStart` | int | **Required**. The starting port number for the game server containers. Each subsequent game server will use the next available port in the pool.
| `_client` | DockerClient | A client for interacting with the Docker daemon
| `_addedServers` | List<> |  A list of game servers that have been started by this script

#### Starting A Server (Recommended to use the AddServer Command)

```http
  StartServer() //Starts a new game server in a Docker container
```




## Usage


To start a new game server, use the StartServer method in the ServerStarter class. This will create a new Docker container for the game server and add it to the _addedServers list.

To remove idle game servers, use the RemoveIdleServers method in the ServerStopper class. This will stop and remove any game servers that have a player count of 0, and delete the corresponding records from the MySQL database.

To save data about game servers to the database, use the SaveGameServerData method in the Database class. This will insert new game servers into the database or update the player count of existing game servers.

To retrieve data about game servers from the database, use the GetPlayerCount method in the Database class. This will return the player count for the specified game server.



## Conclusion

This server management system provides an easy way to host and manage multiple game servers in Docker containers, and saves and retrieves data about the servers from a MySQL database.


## Authors

- [@ZaviiNet](https://github.com/ZaviiNet)




## Contributing

Contributions are always welcome!

Open an Issue to get started, Pull Requests welcome!

Please adhere to this project's `code of conduct`.



## License

[gpl-3.0](https://choosealicense.com/licenses/gpl-3.0/)

