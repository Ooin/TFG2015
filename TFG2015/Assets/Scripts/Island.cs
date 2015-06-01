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

        meshRenderer.material.shader = Shader.Find("Custom/Vertex Colored");

        /*setColorSelection(mesh);
        Material[] mats = new Material[6];
        mats[0] = Resources.Load("water", typeof(Material)) as Material;
        mats[1] = Resources.Load("sand", typeof(Material)) as Material;
        mats[2] = Resources.Load("forest", typeof(Material)) as Material;
        mats[3] = Resources.Load("grass", typeof(Material)) as Material;
        mats[4] = Resources.Load("stone", typeof(Material)) as Material;
        mats[5] = Resources.Load("snow", typeof(Material)) as Material;

        meshRenderer.materials = mats;*/
        setGodusShape(mesh);

        Destroy(terrain);
        Destroy(terrainCollider);
        Destroy(illa);
	
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
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private void setColorSelection(Mesh mesh)
    {
        float alt = 0.0f;
        float maxAlt = 0.0f;
        
        //La List<Tris> no em permet fer el ADD de l'objecte Tris aixi que ho farem tot parametritzat
        List<int> trisNeu = new List<int>(); trisNeu.Capacity = mesh.triangles.Length * 2 / 3;
        List<int> trisPedra = new List<int>(); trisPedra.Capacity = mesh.triangles.Length * 2 / 3;
        List<int> trisGespa = new List<int>(); trisGespa.Capacity = mesh.triangles.Length * 2 / 3;
        List<int> trisBosc = new List<int>(); trisBosc.Capacity = mesh.triangles.Length * 2 / 3;
        List<int> trisSorra = new List<int>(); trisSorra.Capacity = mesh.triangles.Length * 2 / 3;
        List<int> trisAigua = new List<int>(); trisAigua.Capacity = mesh.triangles.Length * 2 / 3;
        List<int> auxList = new List<int>();
        /*
        int[] trisNeu = new int[mesh.triangles.Length];
        int[] trisPedra = new int[mesh.triangles.Length];
        int[] trisGespa = new int[mesh.triangles.Length];
        int[] trisBosc = new int[mesh.triangles.Length];
        int[] trisSorra = new int[mesh.triangles.Length];
        int[] trisAigua = new int[mesh.triangles.Length];
        */
        int indexNeu = 0, indexPedra = 0, indexGespa = 0, indexSorra = 0, indexAigua = 0, indexBosc = 0, indexAuxList = 0;

        float y1, y2, y3;

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
                /*
                trisNeu[indexNeu] = mesh.triangles[i];
                trisNeu[indexNeu+1] = mesh.triangles[i+1];
                trisNeu[indexNeu+2] = mesh.triangles[i+2];
                indexNeu += 3;
                 */
            }
            else if (alt > 0.75f)
            {
                auxList = trisPedra;
                /*
                trisPedra[indexPedra] = mesh.triangles[i];
                trisPedra[indexPedra + 1] = mesh.triangles[i + 1];
                trisPedra[indexPedra + 2] = mesh.triangles[i + 2];
                indexPedra += 3;
                 */
            }
            else if (alt > 0.5f)
            {
                auxList = trisGespa;
                /*trisGespa[indexGespa] = mesh.triangles[i];
                trisGespa[indexGespa + 1] = mesh.triangles[i + 1];
                trisGespa[indexGespa + 2] = mesh.triangles[i + 2];
                indexGespa += 3;*/
            }
            else if (alt > 0.15f)
            {
                auxList = trisBosc;
                /*
                trisBosc[indexBosc] = mesh.triangles[i];
                trisBosc[indexBosc + 1] = mesh.triangles[i + 1];
                trisBosc[indexBosc + 2] = mesh.triangles[i + 2];
                indexBosc += 3;
                 */
            }
            else if (alt != 0)
            {
                auxList = trisSorra;
                /*
                trisSorra[indexSorra] = mesh.triangles[i];
                trisSorra[indexSorra + 1] = mesh.triangles[i + 1];
                trisSorra[indexSorra + 2] = mesh.triangles[i + 2];
                indexSorra += 3;
                 */
            }
            else
            {
                auxList = trisAigua;
                /*
                trisAigua[indexAigua] = mesh.triangles[i];
                trisAigua[indexAigua + 1] = mesh.triangles[i + 1];
                trisAigua[indexAigua + 2] = mesh.triangles[i + 2];
                indexAigua += 3;
                 */
            }
            auxList.Add(mesh.triangles[i]);
            auxList.Add(mesh.triangles[i+1]);
            auxList.Add(mesh.triangles[i+2]);

        
        }
        
        mesh.subMeshCount = 6;
        mesh.SetTriangles(trisAigua.ToArray(), 0);
        mesh.SetTriangles(trisSorra.ToArray(), 1);
        mesh.SetTriangles(trisBosc.ToArray(), 2);
        mesh.SetTriangles(trisGespa.ToArray(), 3);
        mesh.SetTriangles(trisPedra.ToArray(), 4);
        mesh.SetTriangles(trisNeu.ToArray(), 5);
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
