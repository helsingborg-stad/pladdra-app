using System.Collections.Generic;
using System.Threading.Tasks;
using Abilities.ARRoomAbility;
using Data.Dialogs;

namespace Abilities.Tutorial
{

    /// <summary>
    /// A hardcoded repository for learning and testing
    /// </summary>
    public class TutorialRepository : IDialogProjectRepository
    {
        public Task<DialogProject> Load()
        {
            return Task.FromResult(new DialogProject()
            {
                // our basis for object placement is a 16m x 16m square plane 
                Plane = new DialogPlane() { Width = 16, Height = 16},
                // we have no featured scenes
                FeaturedScenes = new List<DialogScene>(),
                // a list of hardcoded resources that will later be downloaded to the workspace
                Resources = new List<DialogResource>(){
                    new DialogResource(){
                        Id = "dinosaurie",
                        Type = "model",
                        ModelUrl = "https://modul-test.helsingborg.io/wp-content/uploads/sites/42/2022/06/marker_playground_dinosaur.glb",
                        MarkerModelUrl = "https://modul-test.helsingborg.io/wp-content/uploads/sites/42/2022/06/marker_playground_dinosaur.glb"
                    }
                },
                // we skip use of AR marker detection
                Marker = null
            });
        }
        
        public Task<Dictionary<string, DialogScene>> LoadScenes()
        {
            // We currently dont support persistence of scenes
            return Task.FromResult(new Dictionary<string, DialogScene>());
        }

        public Task<DialogScene> SaveScene(DialogScene scene)
        {
            // We currently dont support persistence of scenes
            return Task.FromResult(scene);
        }
    }
}
/*
"name": "Dinosaurie",
"model": "https://modul-test.helsingborg.io/wp-content/uploads/sites/42/2022/06/playground_dinosaurielekan.glb",
"marker_model": "https://modul-test.helsingborg.io/wp-content/uploads/sites/42/2022/06/marker_playground_dinosaur.glb",
"disable": false
*/