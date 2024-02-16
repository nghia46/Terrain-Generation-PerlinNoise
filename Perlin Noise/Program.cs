using System;
using System.Drawing;
public class PerlinTerrainGenerator
{
    private const int Width = 512;
    private const int Height = 512;
    private const float Scale = 0.01f;
    private const int Octaves = 4;
    private const float Persistence = 0.5f;
    private const float Lacunarity = 2.0f;

    private static readonly Random Random = new Random();

    public static Bitmap GenerateTerrain()
    {
        float[,] terrain = new float[Width, Height];

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < Octaves; i++)
                {
                    float xCoord = x * Scale * frequency;
                    float yCoord = y * Scale * frequency;

                    float perlinValue = PerlinNoise(xCoord, yCoord);
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= Persistence;
                    frequency *= Lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                terrain[x, y] = noiseHeight;
            }
        }

        // Normalize terrain heights
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                terrain[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, terrain[x, y]);
            }
        }

        return CreateBitmap(terrain);
    }

    private static Bitmap CreateBitmap(float[,] terrain)
    {
        Bitmap bitmap = new Bitmap(Width, Height);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int value = (int)(terrain[x, y] * 255);
                Color color = GetTerrainColor(terrain[x, y]);
                bitmap.SetPixel(x, y, color);
            }
        }

        return bitmap;
    }

    private static float PerlinNoise(float x, float y)
    {
        int X = (int)Math.Floor(x) & 255;
        int Y = (int)Math.Floor(y) & 255;

        x -= (int)Math.Floor(x);
        y -= (int)Math.Floor(y);

        float u = Fade(x);
        float v = Fade(y);

        int A = _p[X] + Y, AA = _p[A], AB = _p[A + 1],
            B = _p[X + 1] + Y, BA = _p[B], BB = _p[B + 1];

        return Mathf.Lerp(
            Mathf.Lerp(Grad(_p[AA], x, y), Grad(_p[BA], x - 1, y), u),
            Mathf.Lerp(Grad(_p[AB], x, y - 1), Grad(_p[BB], x - 1, y - 1), u),
            v
        );
    }

    private static float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private static float Grad(int hash, float x, float y)
    {
        int h = hash & 15;
        float u = h < 8 ? x : y,
              v = h < 4 ? y : h == 12 || h == 14 ? x : 0;
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }

    private static int[] _p = new int[512];

    static PerlinTerrainGenerator()
    {
        for (int i = 0; i < 256; i++)
        {
            _p[i] = _p[i + 256] = Random.Next(256);
        }
    }
    private static Color GetTerrainColor(float height)
    {
        if (height < 0.1f)
        {
            //deep ocean
            return Color.FromArgb(54, 62, 164);
        }
        else if (height < 0.2f)
        {
            // Ocean
            return Color.FromArgb(75, 128, 202);
        }
        else if (height < 0.4f)
        {
            // Beach
            return Color.FromArgb(104, 194, 211);
        }
        else if (height < 0.49f)
        {
            // sand
            return Color.FromArgb(237, 225, 158);
        }
        else if (height < 0.7f)
        {
            // Forest
            return Color.FromArgb(194, 211, 104);
        }
        else if(height < 0.8)
        {
            //High land
            return Color.FromArgb(138, 176, 96);
        }
        else if (height < 0.85f)
        {
            // Low Mountain
            return Color.FromArgb(146, 146, 146);
        }
        else if (height < 0.9f)
        {
            // Mountains
            return Color.FromArgb(100, 99, 101);
        }
        else
        {
            // Snow
            return Color.FromArgb(242, 240, 229);
        }
    }
}

public static class Mathf
{
    public static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    public static float InverseLerp(float a, float b, float value)
    {
        if (a != b)
            return Clamp01((value - a) / (b - a));
        else
            return 0.0f;
    }

    public static float Clamp01(float value)
    {
        if (value < 0.0f) return 0.0f;
        if (value > 1.0f) return 1.0f;
        return value;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Bitmap terrainBitmap = PerlinTerrainGenerator.GenerateTerrain();
        terrainBitmap.Save("terrain.bmp");
    }
}
