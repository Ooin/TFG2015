using UnityEngine;
using System.Collections.Generic;
using System.IO;


public class Island : MonoBehaviour {

    //Definitives ISLAND.CS
    private float[,] mapHeights;

    //Resolucion del heightmap en escales potencies de 2 fins a 2048
    public int heightmapResolution;

    //alçada, amplada i profunditat del mon
    //posiblement ho fem fixe, amb dos o tres opcions de mapa
    public int height;
    public int width;
    public int depth;
    ////////////////////////////////////////////////////

    //matrius que contenen les alçades en coordenades de mon i coordenades de "heightMap"
    private float[,] worldHeights;
    private float[,] normalizedWorldHeights;
    
    private Texture2D imatgeBN;

    //Numero de blobs que es generen
    public int nBlobs;

    //ELIMINAR SI O SI

    private List<Vector2> border_pixels;

    enum SelectorColors
    {
        Neu,
        Pedra,
        Gespa,
        Bosc,
        Sorra,
        Aigua
    };

	// Use this for initialization
	void Start ()
    {
        init();
        TerrainCollider terrainCollider = gameObject.AddComponent<TerrainCollider>();
        Terrain terrain = gameObject.AddComponent<Terrain>();
		TerrainData illa = new TerrainData ();

        illa.heightmapResolution = heightmapResolution;
        illa.size = new Vector3(width, height, depth);
        
		terrainCollider.terrainData= illa;
		terrain.terrainData = illa;

        HeightMapGenerator generator = new HeightMapGenerator();

        mapHeights = generator.generateHeightMap(width, height, depth, heightmapResolution, nBlobs);

        terrain.terrainData.SetHeights(0, 0, mapHeights);

        ExportTerrain exportador = new ExportTerrain();
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        Mesh mesh = exportador.Init(terrain, SaveFormat.Triangles, SaveResolution.Half);
        meshFilter.mesh = mesh;

        
        //setColorSelection(mesh);
        meshRenderer.material.shader = Shader.Find("Custom/Vertex Colored");
        /*
        Material[] mats = new Material[6];
        mats[0] = Resources.Load("water", typeof(Material)) as Material;
        mats[1] = Resources.Load("sand", typeof(Material)) as Material;
        mats[2] = Resources.Load("forest", typeof(Material)) as Material;
        mats[3] = Resources.Load("grass", typeof(Material)) as Material;
        mats[4] = Resources.Load("stone", typeof(Material)) as Material;
        mats[5] = Resources.Load("snow", typeof(Material)) as Material;

        meshRenderer.materials = mats;*/

        Destroy(terrain);
        Destroy(terrainCollider);
        Destroy(illa);
	
	}

    private void setColorSelection(Mesh mesh)
    {
        float alt = 0.0f;
        float maxAlt = 0.0f;

        //resize dels vertexs per a que s'assembli a "Godus"
        /*
        
        float maxim = 0.0f;
        foreach (Vector3 aux in vertexs)
        {
            if (aux.y > maxim) maxim = aux.y;
        }

        float auxalt = 0.0f;
        for (int i = 0; i < vertexs.Length; i++)
        {
            auxalt = vertexs[i].y;
            auxalt /= maxim;
            if (auxalt > 0.9) vertexs[i].y = maxim * 1.0f;
            else if (auxalt > 0.8f) vertexs[i].y = maxim * 0.9f;
            else if (auxalt > 0.7f) vertexs[i].y = maxim * 0.8f;
            else if (auxalt > 0.6f) vertexs[i].y = maxim * 0.7f;
            else if (auxalt > 0.5f) vertexs[i].y = maxim * 0.6f;
            else if (auxalt > 0.4f) vertexs[i].y = maxim * 0.5f;
            else if (auxalt > 0.3f) vertexs[i].y = maxim * 0.4f;
            else if (auxalt > 0.2f) vertexs[i].y = maxim * 0.3f;
            else if (auxalt > 0.1f) vertexs[i].y = maxim * 0.2f;
            else if (auxalt != 0.0f) vertexs[i].y = maxim * 0.1f;
            else vertexs[i].y = 0.0f;
        }

        mesh.vertices = vertexs;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();*/

        //La List<Tris> no em permet fer el ADD de l'objecte Tris aixi que ho farem tot parametritzat
        List<int> trisNeu = new List<int>();
        List<int> trisPedra = new List<int>();
        List<int> trisGespa = new List<int>();
        List<int> trisBosc = new List<int>();
        List<int> trisSorra = new List<int>();
        List<int> trisAigua = new List<int>();
        List<int> auxList = new List<int>();
        

        float y1, y2, y3;
        int j = 0;

        foreach (Vector3 aux in mesh.vertices)
        {
            if (aux.y > maxAlt) maxAlt = aux.y;
        }
        
        for (int i = 0; i < mesh.triangles.Length; i+=3)
        {
            y1 = mesh.vertices[mesh.triangles[i]].y;
            y2 = mesh.vertices[mesh.triangles[i + 1]].y;
            y3 = mesh.vertices[mesh.triangles[i + 2]].y;

            alt = ((y1 + y2 + y3) / 3)/maxAlt;
            
            if (alt >= 0.95f)
            {
                auxList = trisNeu;
            }
            else if (alt > 0.75f)
            {
                auxList = trisPedra;
            }
            else if (alt > 0.5f)
            {
                auxList = trisGespa;
            }
            else if (alt > 0.15f)
            {
                auxList = trisBosc;
            }
            else if (alt != 0)
            {
                auxList = trisSorra;
            }
            else
            {
                auxList = trisAigua;
            }
            auxList.Add(mesh.triangles[i]);
            auxList.Add(mesh.triangles[i + 1]);
            auxList.Add(mesh.triangles[i + 2]);
        
        }
        
        mesh.subMeshCount = 6;
        mesh.SetTriangles(trisAigua.ToArray(), 0);
        mesh.SetTriangles(trisSorra.ToArray(), 1);
        mesh.SetTriangles(trisBosc.ToArray(), 2);
        mesh.SetTriangles(trisGespa.ToArray(), 3);
        mesh.SetTriangles(trisPedra.ToArray(), 4);
        mesh.SetTriangles(trisNeu.ToArray(), 5);

        //mesh.RecalculateBounds();
        //mesh.RecalculateNormals();
    }

    //inicialitzacio de les variables publiques a la primera execucio
    void init()
    {
        height = 100;
        width = 100;
        depth = 100;
        heightmapResolution = 256;
        nBlobs = 1;

        border_pixels = new List<Vector2>();
    }


    //metodes per reciclar
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
            for (int j = y - 1; j <= y + 1; j++)
            {
                alt = mapHeights[i, j];
                if (alt == 0 && i >= 0 && j >= 0 && i < heightmapResolution && j < heightmapResolution) return true;
            }
        }
        return false;
    }
   
	// Update is called once per frame
	void Update () {    }

}
