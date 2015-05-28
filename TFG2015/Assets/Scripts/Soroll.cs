using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Soroll{

    public static Texture2D aplicaSorollCostes(Texture2D original, int width, int height){

        List<Vector2> isCosta = new List<Vector2>();
        Color sample;
        float perlin = 0.0f;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                sample = original.GetPixel(i, j);
                perlin = Mathf.PerlinNoise(i, j);
                sample = new Color(sample.r * perlin, sample.g * perlin, sample.b * perlin);
                original.SetPixel(i, j, sample);
            }
        }
        return original;
    }
    // heights[i, k] = Mathf.PerlinNoise(((float)i / (float)terrain.terrainData.heightmapWidth) * tileSize, ((float)k / (float)terrain.terrainData.heightmapHeight) * tileSize)/10.0f;
    /*private void getBorder()
    {
        for (int i = 0; i < heightmapResolution; i++)
        {
            for (int j = 0; j < heightmapResolution; j++)
            {
                if (mapHeights[i, j] != 0)
                {
                    if (isBorder(i, j)) border_pixels.Add(new Vector2(i, j));
                }
            }
        }
    }

    private bool isBorder(int x, int y)
    {
        float alt = 0;

        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                alt = mapHeights[i, j];
                if (alt == 0 && i >= 0 && j >= 0 && i < heightmapResolution && j < heightmapResolution) return true;
            }
        }
        return false;
    }*/

}
