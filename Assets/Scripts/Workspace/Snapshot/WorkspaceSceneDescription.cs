using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Workspace.Snapshot
{
    public class WorkspaceSceneDescription
    {
        public static WorkspaceSceneDescription Describe(IWorkspaceScene scene)
        {
            return new WorkspaceSceneDescription()
            {
                Plane = UpdateTransform(new PlaneDescription(), scene.Plane),
                Items = scene.ObjectsManager.Objects.Select(o => UpdateTransform(
                        new ItemDescription()
                        {
                            ResourceId = o.WorkspaceResource.ResourceID
                        }, o.GameObject))
                    .ToList()
            };
        }
        private static T UpdateTransform<T>(T obj, GameObject go) where T : TransformDescription
        {
            obj.Position = MakeV3(go.transform.localPosition);
            obj.Scale = MakeV3(go.transform.localScale);
            obj.Rotation = MakeQ(go.transform.localRotation);
            return obj;
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
            public string ResourceId { get; set; }
        }
    }
}