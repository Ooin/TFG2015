using UnityEngine;
using System.Collections.Generic;
using System.IO;


public class Island : MonoBehaviour {

    //Definitives ISLAND.CS
    public float[,] mapHeights;

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

    //private GameObject camera;

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
        gameObject.AddComponent<MeshCollider>();
        generateNewIsland(false , new Vector3(100, 100, 100), nBlobs, 0);
	}

    public void generateNewIsland(bool godus, Vector3 size, int numBlobs, int shape)
    {
        nBlobs = numBlobs;
        width = (int)size.x;
        height = (int)size.y;
        depth = (int)size.z;
        TerrainCollider terrainCollider = gameObject.AddComponent<TerrainCollider>();
        Terrain terrain = gameObject.AddComponent<Terrain>();
        TerrainData illa = new TerrainData();

        illa.heightmapResolution = heightmapResolution;
        illa.size = new Vector3(width, height, depth);

        terrainCollider.terrainData = illa;
        terrain.terrainData = illa;

        HeightMapGenerator generator = new HeightMapGenerator();

        mapHeights = generator.generateHeightMap(width, height, depth, heightmapResolution, nBlobs, shape);

        terrain.terrainData.SetHeights(0, 0, mapHeights);
        
        ExportTerrain exportador = new ExportTerrain();
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        Mesh mesh = exportador.Init(terrain, SaveFormat.Triangles, SaveResolution.Full, godus);
        meshFilter.mesh = mesh;


        meshRenderer.material.shader = Shader.Find("Standard");

        Material[] mats;
        
        if (!godus)
        {
            mats = new Material[6];
            mats[0] = Resources.Load("water", typeof(Material)) as Material;
            mats[1] = Resources.Load("sand", typeof(Material)) as Material;
            mats[2] = Resources.Load("forest", typeof(Material)) as Material;
            mats[3] = Resources.Load("grass", typeof(Material)) as Material;
            mats[4] = Resources.Load("stone", typeof(Material)) as Material;
            mats[5] = Resources.Load("snow", typeof(Material)) as Material;
            meshRenderer.materials = mats;
        }

        if (godus)
        {
            mats = new Material[11];
            mats[0] = Resources.Load("water", typeof(Material)) as Material;
            mats[1] = Resources.Load("godus1", typeof(Material)) as Material;
            mats[2] = Resources.Load("godus2", typeof(Material)) as Material;
            mats[3] = Resources.Load("godus3", typeof(Material)) as Material;
            mats[4] = Resources.Load("godus4", typeof(Material)) as Material;
            mats[5] = Resources.Load("godus5", typeof(Material)) as Material;
            mats[6] = Resources.Load("godus6", typeof(Material)) as Material;
            mats[7] = Resources.Load("godus7", typeof(Material)) as Material;
            mats[8] = Resources.Load("godus8", typeof(Material)) as Material;
            mats[9] = Resources.Load("godus9", typeof(Material)) as Material;
            mats[10] = Resources.Load("godus10", typeof(Material)) as Material;

            meshRenderer.materials = mats;
            
        }

        mesh.RecalculateNormals();

        Destroy(terrain);
        Destroy(terrainCollider);
        Destroy(illa);

        Camera.main.GetComponent<Camara>().centre = new Vector3(-width / 2, 0, depth / 2);
    }

    private void setGodusShape(Mesh mesh)
    {
        //resize dels vertexs per a que s'assembli a "Godus"
        Vector3[] vertexs = mesh.vertices;
        
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
    }

    //inicialitzacio de les variables publiques a la primera execucio
    void init()
    {
        height = 100;
        width = 100;
        depth = 100;
        heightmapResolution = 256;
        nBlobs = 20;

        border_pixels = new List<Vector2>();
    }
  
	// Update is called once per frame
	void Update () {    }

}
