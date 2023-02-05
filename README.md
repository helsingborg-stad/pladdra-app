# Pladdra

# Setting up your environment 

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

# Abilities

Pladdra is structured in abilities, each of which is a specific AR use case with its own functionality.

## Creating an Ability

1.  Create a new folder in `Assets/Abilities` with the name of your ability.
2.  Create a new scene in your ability folder.
3.  Add your scene to the build settings.
4.  Add your Ability in `Assets/Settings/Abilities/AbilityList`.

### Settings

-   `name`: name of your ability.
-   `description`: description of your ability.
-   `deepLinkIdentifiers`: unique identifiers for this ability. Any deep link specifying these identifiers will open this ability.
-   `showInList`: if set to false, this ability will not be shown in the list of abilities shown on app start.
-   `scene`: the start scene of this ability.

5.  Your ability will be displayed in the list of abilities shown on app start, and be triggered by deeplinks pointing to your `deepLinkIdentifiers`.

## Deeplinks

Deeplinks are handled by `AbilityManager` in the Lobby scene.

1.  Make sure the Lobby scene is added to the build settings and is the first scene in the list.
2.  When a deeplink is opened, `AbilityManager` will open the ability with the corresponding `deepLinkIdentifiers`.
3.  In your ability, create a script that implements `IDeepLinkHandler` to handle deeplink parsing.

### Testing Deeplinks

1.  Add your deeplink in `DeepLinkTester.deeplink` in the Lobby scene.
2.  Press play.

# Assembly

If you add new assemblies you need to add them to `Assets/Pladdra.asmdef` for them to be included.
`Assets/Pladdra.asmdef` is auto referenced.

# Building

The app uses cocoapods for Google ARCore Extensions, which is used for geolocation with Google visual positioning system.

1.  After building an Xcode project, open the Xcode project file called `Unity-iPhone.xcworkspace`, NOT `Unity-iPhone.xcodeproj`.
2.  Define the team in two places:

-   `Unity-iPhone` > `signing and capabilities`.
-   `Pods` > `ARCore-ARCoreResources` > `signing and capabilities`.

3.  Build to device.