using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UntoldGarden.Utils
{
    public static class MeshExtensions
    {
        public static Mesh ReverseNormals(this Mesh mesh)
        {
            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
                normals[i] = -normals[i];
            mesh.normals = normals;

            for (int m = 0; m < mesh.subMeshCount; m++)
            {
                int[] triangles = mesh.GetTriangles(m);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int temp = triangles[i + 0];
                    triangles[i + 0] = triangles[i + 1];
                    triangles[i + 1] = temp;
                }
                mesh.SetTriangles(triangles, m);
            }

            return mesh;
        }

        // TODO Can we do this async?
        public static Mesh CombineMeshesInChildren(this GameObject gameObject)
        {
            MeshFilter[] meshFilters = gameObject.FindDeepComponents<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];

            for (int i = 0; i < meshFilters.Length; i++)
            {
                if (meshFilters[i].sharedMesh != null)
                {
                    combine[i].mesh = meshFilters[i].sharedMesh;
                    combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                }
            }

            Mesh m = new Mesh();
            m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            m.CombineMeshes(combine);

            return m;
        }

        public static Bounds GetBounds(this GameObject go)
        {
            Bounds b = new Bounds(go.transform.position, Vector3.zero);

            List<MeshRenderer> mrs = new List<MeshRenderer>();
            MeshRenderer[] mrs2 = go.FindDeepComponents<MeshRenderer>();
            if (mrs2 != null) mrs.AddRange(mrs2);
            if (go.GetComponent<MeshRenderer>() != null) mrs.Add(go.GetComponent<MeshRenderer>());
            if (mrs.Count > 0)
            {
                b = mrs[0].bounds;
                foreach (Renderer r in mrs) { b.Encapsulate(r.bounds); }
            }
            else
            {
                List<SkinnedMeshRenderer> smrs = new List<SkinnedMeshRenderer>();
                SkinnedMeshRenderer[] smrs2 = go.FindDeepComponents<SkinnedMeshRenderer>();
                if (smrs2 != null) smrs.AddRange(smrs2);
                if (go.GetComponent<SkinnedMeshRenderer>() != null) smrs.Add(go.GetComponent<SkinnedMeshRenderer>());
                if (smrs.Count > 0)
                {
                    b = smrs[0].bounds;
                    foreach (Renderer r in smrs) { b.Encapsulate(r.bounds); }
                }
            }
            return b;
        }
    }
}
