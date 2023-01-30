using GLTFast;
using UnityEngine;
using GLTFast.Export;
using GLTFast.Logging;
using UnityEngine.UIElements;

namespace Pladdra
{
    public static class ExportGLB
    {
        public static async void AdvancedExport(GameObject root, string path, Button button = null)
        {
            if (button != null)
                button.text = "Exporterar...";

            // CollectingLogger lets you programatically go through
            // errors and warnings the export raised
            var logger = new CollectingLogger();

            // ExportSettings and GameObjectExportSettings allow you to configure the export
            // Check their respective source for details

            // ExportSettings provides generic export settings
            var exportSettings = new ExportSettings
            {
                Format = GltfFormat.Binary,
                FileConflictResolution = FileConflictResolution.Overwrite,
                // Export everything except cameras or animation
                // ComponentMask = ~(ComponentType.Camera | ComponentType.Animation),
                // Boost light intensities 
                // LightIntensityFactor = 100f,
            };

            // GameObjectExportSettings provides settings specific to a GameObject/Component based hierarchy 
            var gameObjectExportSettings = new GameObjectExportSettings
            {
                // // Include inactive GameObjects in export
                // OnlyActiveInHierarchy = false,
                // // Also export disabled components
                // DisabledComponents = true,
                // // Only export GameObjects on certain layers
                // LayerMask = LayerMask.GetMask("Default", "MyCustomLayer"),
            };

            // GameObjectExport lets you create glTFs from GameObject hierarchies
            var export = new GameObjectExport(exportSettings, gameObjectExportSettings, logger: logger);

            // Example of gathering GameObjects to be exported (recursively)
            var rootLevelNodes = new GameObject[] { root };

            // Add a scene
            export.AddScene(rootLevelNodes, "My new glTF scene");

            // Async glTF export
            var success = await export.SaveToFileAndDispose(path);

            if (!success)
            {
                Debug.LogError("Something went wrong exporting a glTF");
                // Log all exporter messages
                logger.LogAll();
            }
            else
            {
                Debug.Log("glTF export successful");
                new NativeShare().AddFile(path)
        .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
        .Share();
            }

            if (button != null)
            {
                button.text = "Klar!";
                button.SetEnabled(false);
            }

        }
    }
}