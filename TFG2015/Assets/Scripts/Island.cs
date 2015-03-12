using UnityEngine;
using System.Collections.Generic;

public class Island : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.AddComponent <TerrainCollider> ();
		gameObject.AddComponent <Terrain> ();
		TerrainData illa = new TerrainData ();

		illa.size = new Vector3(10, 128, 10);
		illa.heightmapResolution = 1024;
		illa.baseMapResolution = 256;
		illa.SetDetailResolution(256, 16);

		int heightmapWidth = illa.heightmapWidth;
		int heightmapHeight = illa.heightmapHeight;
		TerrainCollider terrainCollider = gameObject.GetComponent<TerrainCollider>();
		Terrain terrain = gameObject.GetComponent<Terrain>();
		terrainCollider.terrainData= illa;
		terrain.terrainData = illa;
		GenerateHeights (terrain, 12.5f);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void GenerateHeights(Terrain terrain, float tileSize)
	{
		float[,] heights = new float[terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight];
		
		for (int i = 0; i < terrain.terrainData.heightmapWidth; i++)
		{
			for (int k = 0; k < terrain.terrainData.heightmapHeight; k++)
			{
				heights[i, k] = Mathf.PerlinNoise(((float)i / (float)terrain.terrainData.heightmapWidth) * tileSize, ((float)k / (float)terrain.terrainData.heightmapHeight) * tileSize)/10.0f;
			}
		}
		
		terrain.terrainData.SetHeights(0, 0, heights);
	}

}
