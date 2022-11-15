using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UntoldGarden
{
    public static class TextureExtensions
    {
        public static Texture2D BuildAtlas(this Texture2D[] textures, int width, int height, out int _width, out int _height)
        {
            if (textures.Length == 0)
            {
                Debug.LogWarning("No textures to build atlas with.");
                _width = 0;
                _height = 0;
                return null;
            }

            _width = Mathf.CeilToInt(Mathf.Sqrt(textures.Length));
            _height = _width;

            //textures = textures.ScaleTextures(width / _width, height / _height);
            textures = textures.ScaleTextures(width / _width);
            textures = textures.CropSquare(width / _width);
            textures = textures.FillGrid((int)Mathf.Pow(_width, 2));

            int originalWidth = textures[0].width;
            int originalHeight = textures[0].height;
            int countx = 0;
            int county = 0;

            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false, false)
            {
                filterMode = FilterMode.Point
            };
            for (int i = 0; i < textures.Length; i++)
            {
                Color32[] tex = textures[i].GetPixels32();
                texture.SetPixels32(countx, county, originalWidth, originalHeight, tex);

                countx += originalWidth;
                if (countx > width - originalWidth)
                    countx = 0;
                if (i != 0 && (i + 1) % _width == 0)
                    county += originalHeight;
            }
            texture.Apply();

            return texture;
        }

        public static Texture2D[] ScaleTextures(this Texture2D[] textures, int size) //scaleWidth = 0, int scaleHeight = 0)
        {
            for (int i = 0; i < textures.Length; i++)
            {
                int originalWidth = textures[i].width;
                int originalHeight = textures[i].height;
                Color32[] tex = textures[i].GetPixels32();
                textures[i] = new Texture2D(originalWidth, originalHeight, TextureFormat.ARGB32, false, false)
                {
                    filterMode = FilterMode.Point
                };
                textures[i].SetPixels32(tex);
                textures[i].Apply();

                int scaleWidth = 0;
                int scaleHeight = 0;

                if (originalWidth > originalHeight)
                {
                    scaleHeight = size;
                    float scaleFactor = (float)scaleHeight / (float)originalHeight;
                    float h = scaleFactor * originalWidth;
                    scaleWidth = (int)h;
                }
                else
                {
                    scaleWidth = size;
                    float scaleFactor = (float)scaleWidth / (float)originalWidth;
                    float h = scaleFactor * originalHeight;
                    scaleHeight = (int)h;
                }
                //if (scaleWidth == 0) scaleWidth = originalWidth;
                //if (scaleHeight == 0)
                //{
                //    float scaleFactor = (float)scaleWidth / (float)originalWidth;
                //    float h = scaleFactor * originalHeight;
                //    scaleHeight = (int)h;
                //}
                //Debug.Log($"Texture {textures[i].name} orig width is {originalWidth}, scale width is {scaleWidth}, original height is {originalHeight} and scale height is {scaleHeight}");

                TextureScale.Bilinear(textures[i], scaleWidth, scaleHeight);
            }
            return textures;
        }

        public static Texture2D[] FillGrid(this Texture2D[] textures, int targetCount)
        {
            int originalCount = textures.Length;

            Texture2D[] filledGrid = new Texture2D[targetCount];

            for (int i = 0; i < targetCount; i++)
            {
                if (i < originalCount)
                    filledGrid[i] = textures[i];
                else
                {
                    //Texture2D tex = new Texture2D(textures[i - originalCount].width, textures[i - originalCount].height, textures[i - originalCount].format, false, false);
                    //Graphics.CopyTexture(textures[i - originalCount], tex);
                    filledGrid[i] = textures[i - originalCount];
                }
            }

            return filledGrid;
        }

        //Needs to be scaled first
        public static Texture2D[] CropSquare(this Texture2D[] textures, int size)
        {
            List<Texture2D> croppedTextures = new List<Texture2D>();
            for (int i = 0; i < textures.Length; i++)
            {
                int startX = 0;
                int startY = 0;

                if (textures[i].width > textures[i].height)
                    startX = (textures[i].width - size) / 2;
                else
                    startY = (textures[i].height - size) / 2;

                try
                {
                    Color[] c = textures[i].GetPixels(startX, startY, size, size);
                    Texture2D texture = new Texture2D(size, size);
                    texture.SetPixels(c);
                    texture.Apply();
                    //textures[i] = texture;
                    croppedTextures.Add(texture);
                }
                catch (System.Exception e)
                {
                    Debug.Log(textures[i].name + ", " + e);
                }

            }
            return croppedTextures.ToArray();
        }

        public static Texture2D toTexture2D(this RenderTexture rTex)
        {
            Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
            var old_rt = RenderTexture.active;
            RenderTexture.active = rTex;

            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();

            RenderTexture.active = old_rt;
            return tex;
        }

        public static Texture2D CreateTexture2DFromGradient(this List<float> gradient, int width, int height)
        {
            if (gradient.Count <= 1)
                return null;

            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            float[] gradientNormalised = gradient.Resize(height);
            // float[] gradientNormalised = new float[width];
            // gradientNormalised[0] = gradient[0];
            // gradientNormalised[height - 1] = gradient[gradient.Count - 1];

            // if (gradient.Count < height)
            // {
            //     int steps = height / (gradient.Count - 1);
            //     int j = 0;
            //     int k = 0;

            //     for (int i = 1; i < height - 1; i++)
            //     {
            //         gradientNormalised[i] = gradient[k] + (((gradient[k + 1] - gradient[k]) / steps) * j);

            //         j++;
            //         if (j > steps)
            //         {
            //             j = 0;
            //             k++;
            //         }
            //     }
            // }
            // else
            // {
            //     for (int i = 1; i < height - 1; i++)
            //     {
            //         gradientNormalised[i] = gradient[(int)(i * (gradient.Count / height - 1))];
            //     }
            // }


            // Color[] colors = new Color[width * height];
            // for (int i = 0; i < height; i++)
            // {
            //     for (int j = 0; j < width; j++)
            //     {
            //         colors[j + (256 * i)] = Color.Lerp(Color.black, Color.white, gradientNormalised[i]);
            //     }
            // }
            // texture.SetPixels(colors);
            Color black = Color.black;
            Color white = Color.white;
            Color color;
            for (int x = 0; x < height; x++)
            {
                color = Color.Lerp(black, white, gradientNormalised[x]);
                for (int y = 0; y < width; y++)
                {
                    texture.SetPixel(x, y, color);
                }
            }
            texture.Apply();
            return texture;
        }

        public static Texture2D GenerateNoiseTexture(float pitch, int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);

            Color color;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    color = CalculateColor(x, y, pitch, 1f, 0f, width, height);
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();

            return texture;
        }

        public static Color CalculateColor(int x, int y, float scale, float scaleMod, float offset, int width, int height)
        {
            float xSample = (float)x / width * (scale + scaleMod) + offset;
            float ySample = (float)y / height * (scale + scaleMod) + offset;

            float sample = Mathf.PerlinNoise(xSample, ySample);

            return new Color(sample, sample, sample);
        }
    }
}