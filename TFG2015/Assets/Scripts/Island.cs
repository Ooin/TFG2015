using UnityEngine;
using System.Collections.Generic;
using System.IO;


public class Island : MonoBehaviour {

    //matrius que contenen les alçades en coordenades de mon i coordenades de "heightMap"
    private float[,] worldHeights;
    private float[,] normalizedWorldHeights;
    private float[,] mapHeights;

    private Texture2D imatgeBN;

    //Resolucion del heightmap en escales potencies de 2 fins a 2048
    public int heightmapResolution;
    //alçada, amplada i profunditat del mon
    public int height;
    public int width;
    public int depth;
    //Numero de blobs que es generen
    public int nBlobs;

    private Terrain terrain;

    private List<Vector2> border_pixels;

	// Use this for initialization
	void Start ()
    {
        init();
		TerrainCollider terrainCollider = gameObject.AddComponent <TerrainCollider> ();
		terrain = gameObject.AddComponent <Terrain> ();
		TerrainData illa = new TerrainData ();

        illa.heightmapResolution = heightmapResolution;
        illa.size = new Vector3(width, height, depth);
        
		//TerrainCollider terrainCollider = gameObject.GetComponent<TerrainCollider>();
		//Terrain terrain = gameObject.GetComponent<Terrain>();

		terrainCollider.terrainData= illa;
		terrain.terrainData = illa;

		GenerateNewMap ();
	
	}

    //inicialitzacio de les variables publiques a la primera execucio
    void init()
    {
        height = 100;
        width = 100;
        depth = 100;
        heightmapResolution = 256;
        nBlobs = 10;

        border_pixels = new List<Vector2>();
    }

	void GenerateNewMap(){
        
		Blob[] illes = new Blob[nBlobs];
		float radi, auxX, auxY;//coordenades auxiliar per a generar els randoms
        
        for (int i = 0; i < nBlobs; i++)
        {
            radi = Random.Range(0, height/4);
            auxX = Random.Range((int) width / 4,(int) 3 * width / 4);
            auxY = Random.Range((int) depth / 4,(int) 3 * depth / 4);

            illes[i] = new Blob(radi, auxX, auxY);
        }

        generateHeightMap(illes);

        getBorder();

        Debug.Log(border_pixels.Count);
        /*foreach (Vector2 item in border_pixels)
        {
            mapHeights[(int) item.x, (int) item.y] = 1;
        }*/

        terrain.terrainData.SetHeights(0, 0, mapHeights);

        ExportTerrain exportador = new ExportTerrain();
        exportador.Init(terrain, SaveFormat.Triangles, SaveResolution.Half);

        Mesh illa = new Mesh();
        

	}

    private void getBorder()
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
            for (int j = y - 1; j <= y+1; j++)
            {
                alt = mapHeights[i, j];
                if (alt == 0 && i >= 0 && j >= 0 && i < heightmapResolution && j < heightmapResolution ) return true;
            }
        }
        return false;
    }
    
    private void generateHeightMap(Blob[] illes)
    {
        worldHeights = new float[width, depth];
        Texture2D imatgeBN = new Texture2D(width, depth);
        Vector2 auxVec1, auxVec2;
        float dist, alt, auxAlt;
        int count = 0;
        //generem el mapa en coordenades de mon
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                auxVec1 = new Vector2(i, j); //la posicio del punt
                alt = 0.0f; 
                foreach (Blob auxBlob in illes)
                {
                    auxVec2 = new Vector2(auxBlob.x, auxBlob.y); //la posicio del centre del blob
                    dist = (auxVec1 - auxVec2).magnitude;
                    if (dist < auxBlob.radius){
                        auxAlt = Mathf.Sqrt((auxBlob.radius * auxBlob.radius) - (dist * dist));
                        if (alt < auxAlt) alt = auxAlt;
                        count++;
                    }
                }
                worldHeights[i, j] = alt;
            }
        }
        Debug.Log("Comptabilitzo " + count.ToString() + " caselles amb dades sense normalitzar");
        //normalitzem les alçades
        count = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                worldHeights[i, j] = (float) worldHeights[i, j] / height;
                if (worldHeights[i, j] > 0) count++;
            }
        }
        Debug.Log("Comptabilitzo " + count.ToString() + " caselles amb dades normalitzades");


        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                imatgeBN.SetPixel(i, j, new Color(worldHeights[i, j], worldHeights[i, j], worldHeights[i, j]));
            }
        }
        byte[] bytes = imatgeBN.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/../imgOriginal.png", bytes);

        TextureScale.Bilinear(imatgeBN, heightmapResolution, heightmapResolution);

        bytes = imatgeBN.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/../imgFinal.png", bytes);

        mapHeights = new float[heightmapResolution, heightmapResolution];
        for (int i = 0; i < heightmapResolution; i++)
        {
            for (int j = 0; j < heightmapResolution; j++)
            {
                mapHeights[i, j] = (float) imatgeBN.GetPixel(i, j).r; //es indiferent si agafem R, G o B ja que tenen el mateix valor
            }
        }
    }

    private float calculaAlçadaEsfera(float catet, float hipotenusa)
    {
        float res = Mathf.Sqrt((hipotenusa * hipotenusa) - (catet * catet));
        return res;

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
