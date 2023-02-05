using System;
using System.Collections.Generic;
using UnityEngine;

namespace UntoldGarden.Utils
{
    public static class VectorExtensions
    {
        /// <summary>
        /// Levels Vector3 origin with Vector3 level on Axis axis
        /// </summary>
        /// <param name="origin">Vector to change</param>
        /// <param name="level">Control Vector </param>
        /// <param name="axis">Axis to level on</param>
        /// <returns></returns>
        public static Vector3 Level(this Vector3 origin, Vector3 level, SnapAxis axis = SnapAxis.Y)
        {
            if (axis == SnapAxis.Y)
                origin.y = level.y;
            else if (axis == SnapAxis.X)
                origin.x = level.x;
            else if (axis == SnapAxis.Z)
                origin.z = level.z;
            return origin;
        }

        public enum RelativeToObjectOptions { None, OnGroundLayers, OnNavMesh, OnGroundLayersAndNavMesh }

        /// <summary>
        /// Gets a position relative to a GameObject, with options to place it on a specific ground layer and/or on the NavMesh.
        /// </summary>
        /// <param name="origin">GameObject to get a relative position to.</param>
        /// <param name="offset">Offset from that GameObject, e.g. 2m in front.</param>
        /// <param name="args">OnGroundLayer gets a raycast downward from the relative position and returns the first hit.point. 
        /// OnNavMesh returns the closest point to the relative position on the NavMesh.
        /// OnGroundLayersAndNavMesh does both of the above.</param>
        /// <param name="layers">The GroundLayers to look for RaycastHit's on.</param>
        /// <returns></returns>
        public static Vector3 RelativeToObjectOnGround(this GameObject origin, Vector3? offset, RelativeToObjectOptions args, LayerMask layerMask = default)
        {

            Vector3 pos = origin.transform.position;
            if (offset.HasValue) pos +=
                (origin.transform.forward * offset.Value.z) +
                (origin.transform.right * offset.Value.x) +
                (origin.transform.up * offset.Value.y);

            pos.y = origin.transform.position.y;

            if (args == RelativeToObjectOptions.OnGroundLayers || args == RelativeToObjectOptions.OnGroundLayersAndNavMesh)
            {
                RaycastHit hit;
                if (Physics.Raycast(pos, Vector3.down, out hit, 100, layerMask))
                {
                    pos = hit.point;
                }
                else
                {
                    Debug.Log("VectorExtensions.RelativeToUser could not get RaycastHit!");
                    // gets closest point on all meshes within layermask

                }
            }

            if (args == RelativeToObjectOptions.OnNavMesh || args == RelativeToObjectOptions.OnGroundLayersAndNavMesh)
            {
                if (UnityEngine.AI.NavMesh.SamplePosition(pos, out UnityEngine.AI.NavMeshHit navHit, 10f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    pos = navHit.position;
                }
                else
                {
                    Debug.Log("VectorExtensions.RelativeToUser could not get NavMesh hit!");
                    // TODO Continue until you get one
                }
            }
            return pos;
        }

        public static bool AlignsWithAxis(this Vector3 compareVector, Vector3 originVector, SnapAxis[] axes, float tolerance, out Vector3? alignedVector)
        {

            alignedVector = null;
            for (int i = 0; i < axes.Length; i++)
            {
                switch (axes[i])
                {
                    case SnapAxis.X:
                        if (compareVector.x < originVector.x + tolerance && compareVector.x > originVector.x - tolerance)
                        {
                            if (alignedVector != null)
                                alignedVector = new Vector3(originVector.x, alignedVector.Value.y, alignedVector.Value.z);
                            else
                                alignedVector = new Vector3(originVector.x, compareVector.y, compareVector.z);
                        }
                        break;
                    case SnapAxis.Y:
                        if (compareVector.y < originVector.y + tolerance && compareVector.y > originVector.y - tolerance)
                        {
                            if (alignedVector != null)
                                alignedVector = new Vector3(alignedVector.Value.x, originVector.y, alignedVector.Value.z);
                            else
                                alignedVector = new Vector3(compareVector.x, originVector.y, compareVector.z);
                        }
                        break;
                    case SnapAxis.Z:
                        if (compareVector.z < originVector.z + tolerance && compareVector.z > originVector.z - tolerance)
                        {
                            if (alignedVector != null)
                                alignedVector = new Vector3(alignedVector.Value.x, alignedVector.Value.y, originVector.z);
                            else
                                alignedVector = new Vector3(compareVector.x, compareVector.y, originVector.z);
                        }
                        break;
                    default:
                        break;
                }
            }
            return alignedVector != null;
        }

        #region NavMesh

        public static Vector3 ClosestPointOnNavMesh(this GameObject origin)
        {
            Vector3 pos = origin.transform.position;
            if (UnityEngine.AI.NavMesh.SamplePosition(pos, out UnityEngine.AI.NavMeshHit navHit, 10f, UnityEngine.AI.NavMesh.AllAreas))
            {
                pos = navHit.position;
            }
            else
            {
                Debug.Log("No navMeshHit");
            }
            return pos;
        }
        public static Vector3 ClosestPointOnNavMesh(this Vector3 origin, float maxDistance = 10f)
        {
            Vector3 pos = origin;
            if (UnityEngine.AI.NavMesh.SamplePosition(pos, out UnityEngine.AI.NavMeshHit navHit, maxDistance, UnityEngine.AI.NavMesh.AllAreas))
            {
                pos = navHit.position;
            }
            else
            {
                Debug.Log("No navMeshHit");
            }
            return pos;
        }

        public static Vector3 RandomPointOnNavMesh(this Vector3 origin, float radius)
        {
            Vector3 pos = origin += UnityEngine.Random.insideUnitSphere * radius;
            if (UnityEngine.AI.NavMesh.SamplePosition(pos, out UnityEngine.AI.NavMeshHit navHit, 10f, UnityEngine.AI.NavMesh.AllAreas))
            {
                return navHit.position;
            }
            Debug.Log("No navMeshHit, sending a UnityEngine.Random point.");
            return pos;
        }


        public static Vector3 FollowSpot(this GameObject origin, bool inFront)
        {
            Vector3 pos = origin.transform.position + origin.transform.forward;

            if (inFront)
            {
                Ray ray = new Ray(pos, Vector3.down);
                RaycastHit hit;
                LayerMask lm;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    lm = hit.transform.gameObject.layer;
                    if (lm == 0 || lm == 8)
                    {
                        return (hit.point);
                    }
                    else
                    {
                        Debug.Log("No NavMesh hit was found!");
                    }
                }
            }
            else
            {
                UnityEngine.AI.NavMeshHit navHit;
                if (UnityEngine.AI.NavMesh.SamplePosition(origin.transform.position, out navHit, 2f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    return navHit.position;
                }
                else
                {
                    Debug.Log("No NavMesh hit was found!");
                }
            }

            //TODO below shouldn't happen
            Debug.Log("No real follow point supplied");
            pos.y += -1;
            return pos;
        }

        public static Vector3 GoAwayOnNavmesh(this GameObject origin)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector3 pos = new Vector3(UnityEngine.Random.Range(-10, 10), 0, UnityEngine.Random.Range(-10, 10)); //TODO make UnityEngine.Random but not small values
                UnityEngine.AI.NavMeshHit navHit;
                if (UnityEngine.AI.NavMesh.SamplePosition(pos, out navHit, 10f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    return navHit.position;
                }
            }

            Debug.Log("No NavMesh hit was found, sending a UnityEngine.Random position.");
            return new Vector3(5, 0, 5);
        }

        public static Vector3 GoAway(this GameObject origin, Vector3? dist = null)
        {
            Vector3 pos = origin.transform.position;
            if (dist.HasValue) pos += dist.Value;
            else pos += new Vector3(5, 2, 5);
            return pos;
        }
        #endregion

        #region Touch input
        public static Vector3 GetTouchCameraPoint(this Touch touch, float depth, Camera cam)
        {
            var ray = cam.ScreenPointToRay(touch.position);
            return ray.origin + ray.direction * depth;
        }
        #endregion

        #region Make local and global

        public static Vector3 MakeLocal(this Vector3 pos, Transform root)
        {
            return root.InverseTransformPoint(pos);
        }

        public static Vector3 MakeGlobal(this Vector3 pos, Transform root)
        {
            return root.TransformPoint(pos);
        }

        public static Vector3[] MakeLocal(this Vector3[] pos, Transform root)
        {
            Vector3[] vectors = new Vector3[pos.Length];

            for (int i = 0; i < pos.Length; i++)
            {
                vectors[i] = root.InverseTransformPoint(pos[i]);
            }

            return vectors;

        }

        public static Vector3[] MakeGlobal(this Vector3[] pos, Transform root)
        {
            Vector3[] vectors = new Vector3[pos.Length];

            for (int i = 0; i < pos.Length; i++)
            {
                vectors[i] = root.TransformPoint(pos[i]);
            }

            return vectors;
        }
        #endregion

        #region Maths

        public static List<Vector3> UniqueVectorList(this GameObject origin, Vector3 min, Vector3 max, int count, float tolerance)
        {
            List<Vector3> positions = new List<Vector3>();

            if (tolerance > Vector3.Distance(min, max))
            {
                Debug.Log("Tolerance too great");
                return null;
            }

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = origin.transform.position + new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
                int c = 0;
                while (!IsOK(pos, positions, tolerance))
                {
                    pos = origin.transform.position + new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
                    c++;
                    if (c > 1000)
                    {
                        Debug.LogError("count is too big");
                        return null;
                    }
                }
                positions.Add(pos);
            }
            return positions;
        }

        private static bool IsOK(Vector3 pos, List<Vector3> poss, float tol)
        {
            foreach (Vector3 p in poss)
            {
                if (Vector3.Distance(p, pos) < tol)
                    return false;
            }
            return true;
        }
        public static Vector3 MidPoint(this Vector3[] points)
        {
            float[] centers = new float[3];

            for (int i = 0; i < 3; i++)
            {
                float total = 0;
                foreach (Vector3 point in points)
                    total += point[i];

                centers[i] = total / points.Length;
            }

            return new Vector3(centers[0], centers[1], centers[2]);
        }

        public static Vector3[] DrawCirclePoints(this Vector3 center, int points, double radius)
        {
            List<Vector3> vecs = new List<Vector3>();
            double slice = 2 * Math.PI / points;
            for (int i = 0; i < points; i++)
            {
                double angle = slice * i;
                float newX = (float)(center.x + radius * Math.Cos(angle));
                float newZ = (float)(center.z + radius * Math.Sin(angle));
                vecs.Add(new Vector3(newX, center.y, newZ));
            }
            return vecs.ToArray();
        }
        #endregion

        #region Rotate
        public static Vector3 RotateAround(this Vector3 vec, float angle)
        {
            // Rotates the vector around 0
            var a = angle * System.Math.PI / 180.0;
            double cosa = Math.Cos(a), sina = Math.Sin(a);
            return new Vector3(vec.x * (float)cosa - vec.y * (float)sina, vec.x * (float)sina + vec.y * (float)cosa, 0);
        }

        public static Vector3 GetPointAtDistanceBetweenTwoPoints(this Vector3 origin, Vector3 target, float distance)
        {
            Vector3 directionOfTravel = target - origin;

            Vector3 finalDirection = directionOfTravel - directionOfTravel.normalized * distance;

            Vector3 targetPosition = target - finalDirection;

            return targetPosition;
        }

        /// <summary>
        /// Gets the internal angle of a Vector3 position in a triangle of three Vector3 
        /// </summary>
        /// <param name="angleToGet">The Vector3 of which angle to get</param>
        /// <param name="b">Second Vector3 in triangle</param>
        /// <param name="c">Third Vector3 in triangle</param>
        /// <returns></returns>
        public static float GetInternalVectorAngle(this Vector3 angleToGet, Vector3 b, Vector3 c)
        {
            return Vector3.Angle(b - angleToGet, c - angleToGet);
        }
        #endregion

        #region Vector2

        public static Vector2 CalculateScreenBounds(this Bounds bounds)
        {
            Vector3 c = bounds.center;
            Vector3 e = bounds.extents;

            Vector3[] worldCorners = new[] {
                new Vector3( c.x + e.x, c.y + e.y, c.z + e.z ),
                new Vector3( c.x + e.x, c.y + e.y, c.z - e.z ),
                new Vector3( c.x + e.x, c.y - e.y, c.z + e.z ),
                new Vector3( c.x + e.x, c.y - e.y, c.z - e.z ),
                new Vector3( c.x - e.x, c.y + e.y, c.z + e.z ),
                new Vector3( c.x - e.x, c.y + e.y, c.z - e.z ),
                new Vector3( c.x - e.x, c.y - e.y, c.z + e.z ),
                new Vector3( c.x - e.x, c.y - e.y, c.z - e.z ),
            };

            // IEnumerable<Vector3> screenCorners = worldCorners.select(corner => Camera.main.WorldToScreenPoint(corner));
            // float max_x = screenCorners.Max(corner => corner.x);
            // float min_x = screenCorners.Min(corner => corner.x);
            // float max_y = screenCorners.Max(corner => corner.y);
            // float min_y = screenCorners.Min(corner => corner.y);
            float min_x = worldCorners[0].x;
            float min_y = worldCorners[0].y;
            float max_x = worldCorners[0].x;
            float max_y = worldCorners[0].y;

            for (int i = 1; i < 8; i++)
            {
                if (worldCorners[i].x < min_x)
                {
                    min_x = worldCorners[i].x;
                }
                if (worldCorners[i].y < min_y)
                {
                    min_y = worldCorners[i].y;
                }
                if (worldCorners[i].x > max_x)
                {
                    max_x = worldCorners[i].x;
                }
                if (worldCorners[i].y > max_y)
                {
                    max_y = worldCorners[i].y;
                }
            }

            Vector3 topRight = new Vector3(max_x, max_y, 0);
            Vector3 topLeft = new Vector3(min_x, max_y, 0);
            Vector3 bottomRight = new Vector3(max_x, min_y, 0);
            Vector3 bottomLeft = new Vector3(min_x, min_y, 0);

            return new Vector2(Vector3.Distance(topLeft, topRight), Vector3.Distance(topLeft, bottomLeft));
        }
        #endregion

        #region Not sure if relevant
        public static Vector3 InFront(this GameObject origin, float side = 0, float dist = 1, float? up = null)
        {
            Vector3 pos = origin.transform.position + (origin.transform.forward + origin.transform.right * side) * dist;
            if (up.HasValue)
                pos.y = up.Value;
            return pos;
        }

        public static Vector3 InFrontOnNavMesh(this GameObject origin, float side = 0, float dist = 1)
        {
            Vector3 pos = origin.transform.position + (origin.transform.forward + origin.transform.right * side) * dist;
            if (UnityEngine.AI.NavMesh.SamplePosition(pos, out UnityEngine.AI.NavMeshHit navHit, 10f, UnityEngine.AI.NavMesh.AllAreas))
            {
                return navHit.position;
            }
            Debug.Log("No navMeshHit");
            return pos;
        }

        public static Vector3 Behind(this GameObject origin, float dist = 1, float side = 0, bool sameLevelasCam = false)
        {
            Vector3 pos = origin.transform.position - (origin.transform.forward + origin.transform.right * side) * dist;
            if (sameLevelasCam)
                pos.y = origin.transform.position.y;
            return pos;
        }

        public static Vector3 BehindOnNavMesh(this GameObject origin, float dist = 1, bool sameLevelasCam = false)
        {
            Vector3 pos = origin.transform.position - origin.transform.forward * dist;
            if (UnityEngine.AI.NavMesh.SamplePosition(pos, out UnityEngine.AI.NavMeshHit navHit, 10f, UnityEngine.AI.NavMesh.AllAreas))
            {
                pos = navHit.position;
            }
            else
            {
                Debug.Log("No navMeshHit");
            }
            if (sameLevelasCam)
                pos.y = origin.transform.position.y;
            return pos;
        }

        public static Vector3 Under(this GameObject origin, float dist = 1)
        {
            Vector3 pos = origin.transform.position - origin.transform.up * dist;
            return pos;
        }

        public static Vector3 Above(this GameObject origin, float dist = 1)
        {
            Vector3 pos = origin.transform.position + origin.transform.up * dist;
            return pos;
        }


        #endregion
    }
}