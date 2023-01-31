using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UntoldGarden.Utils
{
    public static class MathExtensions
    {
        public static float StepFloat(this float value, float step)
        {
            return step * Mathf.RoundToInt(value / step);
        }

        // Equivalent to javascripts Map function
        public static float Map(this float value, float fromSource, float toSource, float fromTarget, float toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }

        public static float[] Resize(this List<float> list, int size)
        {
            if (list.Count == 1)
            {
                float[] a = new float[size];
                Array.Fill(a, list[0]);
                return a;
            }

            if(list.Count == size)
            {
                return list.ToArray();
            }

            if(list.Count == 0 || size == 0)
            {
                return new float[0];
            }

            float[] array = new float[size];
            array[0] = list[0];
            array[size - 1] = list[list.Count - 1];

            if (list.Count < size)
            {
                int steps = size / (list.Count - 1);
                int j = 0;
                int k = 0;

                for (int i = 1; i < size - 1; i++)
                {
                    array[i] = list[k] + (((list[k + 1] - list[k]) / steps) * j);

                    j++;
                    if (j > steps)
                    {
                        j = 0;
                        k++;
                    }
                }
            }
            else
            {
                for (int i = 1; i < size - 1; i++)
                {
                    array[i] = list[(int)(i * (list.Count / size - 1))];
                }
            }

            return array;
        }
    }
}