using UnityEngine;
using System.Collections.Generic;

public class Island : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.AddComponent <TerrainCollider> ();
		gameObject.AddComponent <Terrain> ();
		TerrainData illa = new TerrainData ();

		illa.size = new Vector3(10, 10, 10);
		illa.heightmapResolution = 100;
		illa.baseMapResolution = 256;
		illa.SetDetailResolution(256, 16);

		int heightmapWidth = illa.heightmapWidth;
		int heightmapHeight = illa.heightmapHeight;
		TerrainCollider terrainCollider = gameObject.GetComponent<TerrainCollider>();
		Terrain terrain = gameObject.GetComponent<Terrain>();
		terrainCollider.terrainData= illa;
		terrain.terrainData = illa;
		GenerateHeights (terrain, 100.5f);
	
	}

	void GenerateNewMap(Terrain terrain){
		Blob[] illes = new Blob[2];
		float radi;
		Vector2 coord_ini; //coordenades auxiliar per a generar els randoms
		bool fin = false;
		float[,] heights = new float[terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight];

		for (int i = 0; i < illes.Length; i++) {
			radi = Random.Range(1.0, 100.0) / 10;
			coord_ini.x = Random.Range(0, 256);
			coord_ini.y = Random.Range(0, 256);
			
			illes[i] = new Blob(radi, coord_ini);
		}

		//////////////////////////////////////////
		for (int i = 0; i < terrain.terrainData.heightmapWidth; i++)
		{
			for (int k = 0; k < terrain.terrainData.heightmapHeight; k++)
			{
				heights[i, k] = Mathf.PerlinNoise(
										((float)i / (float)terrain.terrainData.heightmapWidth) * tileSize,
										((float)k / (float)terrain.terrainData.heightmapHeight) * tileSize
													)/10.0f;
				heights[i,k] = setHeights(illes, i, k);
			}
		}


	}

	private float[,] setHeights(Blob[] illes, int x, int y){

	}
	
	void GenerateHeights(Terrain terrain, float tileSize)
	{
		float[,] heights = new float[terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight];

		Debug.Log (terrain.terrainData.heightmapWidth);
		Debug.Log (terrain.terrainData.heightmapHeight);

		int alt = 1;

		for (int i = 0; i < terrain.terrainData.heightmapWidth; i++)
		{
			for (int k = 0; k < terrain.terrainData.heightmapHeight; k++)
			{
				heights[i, k] = Mathf.PerlinNoise(((float)i / (float)terrain.terrainData.heightmapWidth) * tileSize, ((float)k / (float)terrain.terrainData.heightmapHeight) * tileSize)/10.0f;
			}
		}
		
		terrain.terrainData.SetHeights(0, 0, heights);
	}

	// Update is called once per frame
	void Update () {
	}

}
