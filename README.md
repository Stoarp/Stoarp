# <img src="https://stoat.chat/favicon.svg" width="32" style="vertical-align: middle; border-radius: 8px;" /> Stoarp

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET Version">
  <img src="https://img.shields.io/badge/platform-Windows%20%7C%20macOS%20%7C%20Linux-blue?style=for-the-badge" alt="Platforms">
  <img src="https://img.shields.io/github/license/Stoarp/Stoarp?style=for-the-badge" alt="License">
</p>

<p align="center">
  A cross-platform desktop chat client for <a href="https://stoat.chat/">Stoat.chat</a> built with <strong>Avalonia</strong> and <strong>.NET 8</strong>.
</p>

---

## Features

<ul>
  <li>Real-time messaging with Stoat platform</li>
  <li>Native cross-platform desktop experience (Windows, macOS, Linux)</li>
  <li>Clean, modern chat interface</li>
  <li>Voice chat capabilities (VoIP/SIP)</li>
  <li>Built on robust, scalable architecture</li>
</ul>

---

## Tech Stack

| Technology | Description |
|------------|-------------|
| [.NET 8](https://dotnet.microsoft.com/) | Modern .NET runtime |
| [Avalonia](https://avaloniaui.net/) | Cross-platform .NET UI framework |
| [ReactiveUI](https://reactiveui.net/) | MVVM framework for reactive applications |
| [StoatSharp](https://github.com/Stoarp/StoatSharp) | C# client library for the Stoat API |
| [SIPSorcery](https://github.com/sipsorcery/sipsorcery) | VoIP/SIP library for voice capabilities |

---

## Requirements

- **.NET 8.0 SDK** or later
- **Windows 10/11**, **macOS 10.15+**, or **Linux**

---

## Building

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the application
dotnet run --project Stoarp/Stoarp.csproj
```

---

## Architecture

Stoarp follows the **MVVM** pattern using **ReactiveUI**:

| Component | Description |
|-----------|-------------|
| **Views** | Avalonia UI components (`.axaml` files) |
| **ViewModels** | ReactiveUI ViewModels with data binding |
| **Services** | Business logic and API communication via StoatSharp |

<details>
<summary>Project Structure</summary>

```
Stoarp/
├── Stoarp/                  # Main application
│   ├── Views/               # Avalonia UI views
│   ├── ViewModels/          # ReactiveUI ViewModels
│   ├── Services/            # Business logic services
│   └── Assets/              # Application assets
├── StoatSharp/              # Stoat API client library
└── *.sln                    # Solution files
```

</details>

---

## License

See individual project licenses.

---

## Acknowledgments

<table>
  <tr>
    <td align="center">
      <a href="https://github.com/FluxpointDev/StoatSharp">
        <strong>FluxpointDev/StoatSharp</strong><br>
        <span>Original Stoat client library</span>
      </a>
    </td>
    <td align="center">
      <a href="https://github.com/revoltchat/revoltsharp">
        <strong>RevoltSharp</strong><br>
        <span>Foundation for StoatSharp</span>
      </a>
    </td>
    <td align="center">
      <a href="https://avaloniaui.net/">
        <strong>Avalonia Team</strong><br>
        <span>Cross-platform UI framework</span>
      </a>
    </td>
  </tr>
</table>

---

<p align="center">
  Star us on GitHub if you like what you see!
</p>