# Settings

## Overview
This page documents each configurable setting.

## Server Configuration
### Server name
The name that will be displayed to users in the Server Browser.

### Password
The password required to join the server.

### Admin Password
Defines the server's admin password, allows a server administrator to login and control the server.
To access this either open the chat input box by pressing C in the lobby or Enter in-game followed by: `#login <password>`.

### Max Players
Set the maximum amount of players on the server.

### Server Visible
Set the visibility of the server in the Server Browser.

### Bind Address
IP address to which the server socket will be bound. In most cases, this should be left empty. It can be used to restrict connections to a particular network interface. When left out or empty, 0.0.0.0 is used, which allows connections through any IP address.

### Bind Port
UDP port to which the server socket will be bound.

### Public Address
IP address registered in backend. This should be set to the public IP address to which clients can connect in order to reach the server (either IP of the server itself or IP of the machine that will forward data to the server). If the entry is missing, empty or 0.0.0.0, then the public IP address will be automatically detected and used by the backend.

### Public Port
UDP port registered in backend. If the server itself has a public IP address, this should be the same value as in bindPort.

### Steam Query Port
UDP port on which the game listens to A2S requests.

### Player Save Time
Default period in seconds for saving players for both Online and Local storage (player save can still be requested on demand).

### Server Max View Distance
Maximum view distance in meters. Enforced by the server.

### Server Min Grass Distance
Minimum grass distance in meters. If set to 0, no distance is forced upon clients.

### Disable Third Person
Forces clients to use the first-person view.

### Fast Validation
Validation of map entities and components loaded on client when it joins, ensuring things match with initial server state.

- Enabled - minimum information required to make sure data matches is exchanged between client. When a mismatch occurs, no additional information will be available for determining where client and server states start to differ. All servers that expect clients to connect over internet should have fast validation enabled.
- Disabled - extra data for every replicated entity and component in the map will be transferred when new client connects to the server.
When a mismatch occurs, it is possible to point at particular entity or component where things start to differ. When developing locally (ie. both server and client run on the same machine), it is fine to disable fast validation to more easily pin point source of the problem.

### BattleEye
Enable BattlEye Anti-Cheat.

### Lobby Player Synchronise
If enabled, the list of player identities present on server is sent to the GameAPI along with the server's heartbeat.

### VON Disable UI
Force clients to not have VON (Voice Over Network) UI.

### VON Disable Direct Speech UI
Force clients to not have VON (Voice Over Network) Direct Speech UI.

### VON Can Transmit Cross Faction
Option to allow players to transmit on other factions radios.
Tick to allow to transmission. Uncheck for listen-only.

### Cross Platform
When enabled, Xbox clients can connect to the server.

### AI Limit
Sets the top limit of AIs. No system will be able to spawn any AIs when this ceiling is reached. A negative number is not considered as valid value and is then ignored - limit is not applied.

### Slot Reservation Timeout
Sets the duration (in seconds) for how long will the backend and server reserve a slot for a kicked player. It is considered disabled when set to the minimal value, the value being the same as for a normal disconnect.

### Disable Navmesh Streaming
If enabled, the server will disable navmesh streaming on all navmesh components and load the entire navmesh into memory. This setting provides slightly better server performance and reaction times of moving AIs at the cost of higher memory consumption (up to hundreds of MB depending on the terrain).

### Disable Server Shutdown
If enabled, the server will not automatically shutdown if connection to backend is lost. Related to room requests errors - other causes like corrupted config will still shutdown the server.

### Disable Crash Reported
If enabled, the automatic server-side Crash Report is disabled.

### Disable AI
If enabled, the server will prevent initialization and ticking of AIWorld and its components. Will completely disable AI functionality on the server.

## Server Management

### Limit Server Max FPS
**Recommended**
The maximum FPS of the server. It is recommended to use 60 FPS.

### Automatically Restart
Automatically restarts the server after the specified period has elapsed.

### Override Port
Override the ports specified in the Server Configuration.

### Network Dynamic Simulation
Network Dynamic Simulation (NDS) is a server feature that only streams in relevant replicated entities for each client. The provided value stands for diameter, or the number of cells which are being replicated.
Default: `2`  
To turn the feature off use `0`.
A higher diameter will result in a bigger networked view range, which will lower server performance.

### Spatial Map Resolution
Defines what resolution Spatial Map cells should be set at in a 100m - 1000m range.  
A smaller resolution will result in less pop-in, but lower networked view range.

### Staggering Budget
Defines how many stationary spatial map cells are allowed to be processed in one tick in a `1` - `10201` range.  
If not set it uses the NDS diameter. A lower number will limit how many cells the server has to process per tick, but increase the time it takes for a client to have all relevant entities streamed in.  
If the server experiences significant performance drops on spawning/teleporting then the number is set too high.  
If the client experiences "pop-in" of replicated items then the number is set too low.

### Streaming Budget
The global streaming budget that is equally distributed between all connections.  
To decrement the budget, it uses the replicated hierarchy size of each entity that needs to be streamed in.  
It cannot go under 100 to prevent the system stalling.  
A lower number will limit how many entities the server has to process per tick, but increase the time it takes for a client to have that entity streamed in.  
If the server experiences significant performance drops on spawning/teleporting then the number is set too high.  
If the client experiences "pop-in" of replicated items then the number is set too low.

### Streams Delta
A tool to limit the amount of streams being opened for a client in range `1` - `1000` (default `100`).  
If the difference between 'the number of streams the server has open' and 'the number of streams the client has open' is larger than the `Streams Delta Value`, then the server will not open any more streams this tick.  
To be adjusted based on average client networking speed.

### Load Session Save
If this option is enabled and the text field is empty, the latest savegame will be loaded.  
Enter the path to a savegame file to load a specific save.