using System.Collections.Generic;

namespace Data.Dialogs
{
    public class DialogProject
    {
        public string Id { get; set; }
        
        public DialogPlane Plane { get; set; }

        public List<DialogResource> Resources { get; set; }
        public List<DialogScene> FeaturedScenes { get; set; }
        public DialogueMarker Marker { get; set; }
    }
}
