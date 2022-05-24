using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Workspace;

namespace Data.Dialogs
{
    public class DialogScene
    {
        public static DialogScene Describe(IWorkspaceScene scene, string name)
        {
            return new DialogScene()
            {
                Name = name,
                Plane = CopyTransformTo(scene.Plane, new PlaneDescription()),
                Items = scene.ObjectsManager.Objects.Select(o => CopyTransformTo(o.GameObject, new ItemDescription()
                    {
                        ResourceId = o.WorkspaceResource.ResourceID,
                        Active = o.GameObject.activeSelf
                    }))
                    .ToList()
            };
        }
        private static T CopyTransformTo<T>(GameObject from, T to) where T : TransformDescription
        {
            to.Position = MakeV3(from.transform.localPosition);
            to.Scale = MakeV3(from.transform.localScale);
            to.Rotation = MakeQ(from.transform.localRotation);
            return to;
        }

        private static Q MakeQ(Quaternion q) => new()
            {
                X = q.x,
                Y = q.y,
                Z = q.z,
                W = q.w
            };

        private static V3 MakeV3(Vector3 v) => new()
            {
                X = v.x,
                Y = v.y,
                Z = v.z,
            };

        public class V3
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }

            public Vector3 ToVector3() => new (X, Y, Z);
        }

        public class Q : V3
        {
            public float W { get; set; }
            public Quaternion ToQuaternion() => new(X, Y, Z, W);
        }
        
        public string Name { get; set; }
        public PlaneDescription Plane { get; set; }
        public List<ItemDescription> Items { get; set; }


        public class TransformDescription 
        {
            public V3 Position { get; set; }
            public V3 Scale { get; set; }
            public Q Rotation { get; set; }
        }
        public class PlaneDescription: TransformDescription 
        {
        }
        public class ItemDescription:  TransformDescription 
        {
            public ItemDescription()
            {
                Active = true;
            }

            public string ResourceId { get; set; }
            public bool Active { get; set; }
        }
    }
}