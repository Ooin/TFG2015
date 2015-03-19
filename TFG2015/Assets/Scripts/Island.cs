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
		GenerateNewMap (terrain);

	
	}

	void GenerateNewMap(Terrain terrain){
		Blob[] illes = new Blob[50];
		float radi;
		Vector2 coord_ini; //coordenades auxiliar per a generar els randoms
		float[,] heights = new float[terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight];
        
		for (int i = 0; i < illes.Length; i++) {
			radi = Random.Range(10, 200) /  10.0f;
			coord_ini.x = Random.Range(32, 100);
			coord_ini.y = Random.Range(32, 100);
			
			illes[i] = new Blob(radi, coord_ini);
		}
        
		//////////////////////////////////////////
        Debug.Log(terrain.terrainData.heightmapWidth);
        Debug.Log(terrain.terrainData.heightmapHeight);
        
		for (int i = 0; i < terrain.terrainData.heightmapWidth; i++)
		{
			for (int k = 0; k < terrain.terrainData.heightmapHeight; k++)
			{
                heights[i, k] = giffMeHeights(illes, new Vector2(i, k));
			}
		}

        float max = maxValue2D(heights);
        heights = normalitzaArray(heights, max);

        terrain.terrainData.SetHeights(0, 0, heights);

	}

    private float[,] normalitzaArray(float[,] array, float val)
    {
        int len = (int)Mathf.Sqrt(array.Length);
        for (int i = 0; i < len; i++)
        {
            for (int j = 0; j < len; j++)
            {
                array[i, j] /= val;
            }
        }
        
        return array;
    }

    private float maxValue2D(float[,] array)
    {
        float max = array[0, 0];
        int len = (int)Mathf.Sqrt(array.Length);
        for (int i = 0; i < len; i++)
        {
            for (int j = 0; j < len; j++)
            {
                array[i, j] *= Mathf.PerlinNoise(((float)i / (float)len) * 20, ((float)j / (float)len) * 20) / 10.0f;
                if (array[i, j] > max) max = array[i, j];

            }
        }
        return max;
    }

    private float giffMeHeights(Blob[] illes, Vector2 point)
    {
        float height = 0.0f;
        float dist = 0.0f;
        foreach (Blob b in illes)
        {
            dist = (b.coordinates - point).magnitude;
            if (dist <= b.radius) height += calculaAlçadaEsfera(dist, b.radius, point);
        }
        return height;
    }

    private float calculaAlçadaEsfera(float dist, float rad, Vector2 point)
    {
        //Debug.Log("estic a calcula alçada esfera i m'ha donat V");
        float res = Mathf.Sqrt((rad*rad) - (dist*dist));
        return res;

    }

	
	void GenerateHeights(Terrain terrain, float tileSize)
	{
		float[,] heights = new float[terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight];
        
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
