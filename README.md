# MSFS2024 model matching generator

> [!WARNING]
> This doesn't do anything useful yet. It's pending an update to MSFS2024 due in mid-December
> that adds the necessary interface to retrieve the list of aircraft and liveries.

Creates a VATSIM model matching file for MSFS2024 aircraft and livery.

## Developer notes

The vast majority of this app is boilerplate WPF with a single MVVM (`ModelMatchingViewModel`) commands. The interesting SimConnect code is in the following locations:

- `ViewModels\ModelMatching\ConnectCommand.cs`: Methods for opening the connection to the simulator with SimConnect.
- `ViewModels\ModelMatching\GetLiveriesCommand.cs`: Methods for retrieving the list of aircraft and liveries from the simulator.
- `ViewModels\ModelMatching\SimConnect.cs`: Methods for handing Win32 messages and ensuring incoming SimConnect messages get processed.

The project can be opened and built using Visual Studio Code if the C# Dev kit is installed. Open the repo as a workspace and it will automatically prompt to install the relevant extensions. Simply press `F5` to build and launch the app.
