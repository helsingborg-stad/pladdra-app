# AR Sandbox 

For clarity, each manager is assigned to its own GameObject under parent GameObject Managers. 

# Dialogue Ability

An ability that allows both for interactive dialogues like architecture proposals, and static exhibitions of AR content.

## Projects

All projects are taken from wordpress. You can specify which url to use in the deeplink or in `DialogueAppManager`.

## Local testing

There is currently no way to create a test project in Unity. Class `ProjectReference` has a field for a `TextAsset` that can be parsed as a json if it has no url. To assign this you need to create a local representation of a list of project references.

## Settings

To change stuff like prefabs and interaction settings:

1. Make a new settings file from `Assets` > `Create` > `Pladdra` > `Dialogue Ability settings`.
2. Assign the settings file to the `DialogueUXManager` in managers in the inspector.
3. Change the contents in the settings file.

## Dialogue UX Manager

Dialogue Ability implements the `UXHandler` system, which is handled by `DialogueUXManager`. This is basically a simple state machine, where each `UXHandler` is a state that defines what happens when the user interacts with the app. The `DialogueUXManager` is responsible for creating and switching between states.

## Geospatial 

Some projects use Google's Visual Positioning System to place content in the real world. This is handled by `GeospatialManager`, each project enables and disables this depending on the project's settings.