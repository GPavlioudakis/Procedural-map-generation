using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode {NoiseMap,ColorMap, Mesh};

    public DrawMode drawMode;

    

    public int mapWidth;
    public int mapHeight;
    public int seed;
    public float noiseScale;
    public float heightMultiplier;

    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;
    public Vector2 offset;
    public AnimationCurve heightCurve;

    public TerrainType[] regions;

    public bool autoUpdate;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseScale, seed, octaves, persistance, lacunarity, offset);
        Color[] colorMap = new Color[mapWidth * mapHeight]; 

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];

                for (int i = 0; i < regions.Length; i++)
                {

                    if (currentHeight <= regions[i].height)
                    {
                        colorMap[y * mapWidth + x] = regions[i].TerrainColor;
                        break;
                    }
                }
            }
        }

        MapDisplay display = GetComponent<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, heightMultiplier, heightCurve), TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }
    }

    [System.Serializable]
    public struct TerrainType 
    {
        public string TerrainName;
        public float height;
        public Color TerrainColor;
    }

    public void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1.0f;
        }

        if (mapWidth < 1)
        {
            mapWidth = 1;
        }

        if (mapHeight < 1)
        {
            mapHeight = 1;
        }

        if (octaves < 0)
        {
            octaves = 0;
        }
        
    }
}
