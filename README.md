# Stoarp

A cross-platform desktop chat client for [Stoat.chat](https://stoat.chat/) built with Avalonia and .NET 8.

## Overview

Stoarp is a native desktop application that provides a modern chat experience for the Stoat messaging platform. It features real-time messaging, a clean user interface, and cross-platform support.

## Tech Stack

- **.NET 8** - Modern .NET runtime
- [Avalonia](https://avaloniaui.net/) - Cross-platform .NET UI framework
- [ReactiveUI](https://reactiveui.net/) - MVVM framework for reactive applications
- [StoatSharp](https://github.com/Stoarp/StoatSharp) - C# client library for the Stoat API (fork of FluxpointDev/StoatSharp)
- [SIPSorcery](https://github.com/sipsorcery/sipsorcery) - VoIP/SIP library for voice capabilities

## Requirements

- .NET 8.0 SDK or later
- Windows 10/11, macOS 10.15+, or Linux

## Building

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the application
dotnet run --project Stoarp/Stoarp.csproj
```

## Architecture

Stoarp follows the MVVM (Model-View-ViewModel) pattern using ReactiveUI:

- **Views** - Avalonia UI components (`.axaml` files)
- **ViewModels** - ReactiveUI ViewModels with data binding
- **Services** - Business logic and API communication via StoatSharp

## License

See individual project licenses.

## Acknowledgments

- [FluxpointDev/StoatSharp](https://github.com/FluxpointDev/StoatSharp) - Original Stoat client library
- [RevoltSharp](https://github.com/revoltchat/revoltsharp) - Foundation for StoatSharp
- [Avalonia Team](https://avaloniaui.net/) - Cross-platform UI framework
