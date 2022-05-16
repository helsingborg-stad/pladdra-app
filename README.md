# Pladdra

## Downloading Unity

Unity can be downloaded [here](https://store.unity.com/download)

## Setting up VSCode

1. Install [.NET Core SDK](https://dotnet.microsoft.com/download)
2. [Windows only] Logout or restart Windows to allow changes to `%PATH%` to take effect.
3. [macOS only] Install the latest [mono](https://www.mono-project.com/download/stable/) release
4. Install the [C# extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) for VSCode

## Setup VS Code as Unity Script Editor

Open up **`Unity Preferences`** > **`External Tools`** then browse for the Visual Studio Code executable as External Script Editor

![./Docs/Images/unity-editor-settings.png](./Docs/Images/unity-editor-settings.png)

> The Visual Studio Code executable can be found at `/Applications/Visual Studio Code.app` on macOS and `%localappdata%\Programs\Microsoft VS Code\Code.exe` on Windows by default.

## Adding the project to Unity

After cloning the repository, you can add it to Unity by clicking the `Add` button in Unity Hub.

## Installing dotnet and mono

In order to run the extensions for VSCode, you need to install dotnet from [here](https://dotnet.microsoft.com/download), and, once installed, run `ln -s /usr/local/share/dotnet/dotnet /usr/local/bin/`

You will also need to install [mono](https://www.mono-project.com/download/stable/#download-mac), and set your vscode settings to use a global mono installation

## Debugging Unity in VSCode

Using the [`Debugger for unity`](https://marketplace.visualstudio.com/items?itemName=Unity.unity-debug) extension, you can launch the debugger from the debug menu.

In order for the debugger to attach, you have to start the project in Unity as well.
