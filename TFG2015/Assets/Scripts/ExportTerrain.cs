// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// C # manual conversion work by Yun Kyu Choi

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

enum SaveFormat { Triangles, Quads }
enum SaveResolution { Full = 0, Half, Quarter, Eighth, Sixteenth }

class ExportTerrain
{
    SaveFormat saveFormat = SaveFormat.Triangles;
    SaveResolution saveResolution = SaveResolution.Half;

    static TerrainData terrain;
    static Vector3 terrainPos;

    int tCount;
    int counter;
    int totalCount;
    int progressUpdateInterval = 10000;

    List<int> trisNeu;
    List<int> trisPedra;
    List<int> trisGespa;
    List<int> trisBosc;
    List<int> trisSorra;
    List<int> trisAigua;
    List<int> trisGodus1;
    List<int> trisGodus2;
    List<int> trisGodus3;
    List<int> trisGodus4;
    List<int> trisGodus5;
    List<int> trisGodus6;
    List<int> trisGodus7;
    List<int> trisGodus8;
    List<int> trisGodus9;
    List<int> trisGodus10;


    //[MenuItem("Terrain/Export To Obj...")]
    public Mesh Init(Terrain terrainObject, SaveFormat saveFormat, SaveResolution saveResolution, bool godus)
    {
        terrain = null;
        //Terrain terrainObject = Selection.activeObject as Terrain;
        if (!terrainObject)
        {
            terrainObject = Terrain.activeTerrain;
        }
        if (terrainObject)
        {
            terrain = terrainObject.terrainData;
            terrainPos = terrainObject.transform.position;
        }

        saveFormat = this.saveFormat;

        saveResolution = this.saveResolution;

        trisNeu = new List<int>();
        trisPedra = new List<int>();
        trisGespa = new List<int>();
        trisBosc = new List<int>();
        trisSorra = new List<int>();
        trisAigua = new List<int>();
        trisGodus1 = new List<int>();
        trisGodus2 = new List<int>();
        trisGodus3 = new List<int>();
        trisGodus4 = new List<int>();
        trisGodus5 = new List<int>();
        trisGodus6 = new List<int>();
        trisGodus7 = new List<int>();
        trisGodus8 = new List<int>();
        trisGodus9 = new List<int>();
        trisGodus10 = new List<int>();

        Mesh temp = Export(godus);
        return temp;
    }
    
    private Mesh Export(bool godus)
    {
        string fileName = (String)Directory.GetCurrentDirectory() + "\\terrain.obj";// EditorUtility.SaveFilePanel("Export .obj file", "", "Terrain", "obj");
        int w = terrain.heightmapWidth;
        int h = terrain.heightmapHeight;
        Vector3 meshScale = terrain.size;
        int tRes = (int)Mathf.Pow(2, (int)saveResolution);
        meshScale = new Vector3(meshScale.x / (w - 1) * tRes, meshScale.y, meshScale.z / (h - 1) * tRes);
        Vector2 uvScale = new Vector2(1.0f / (w - 1), 1.0f / (h - 1));
        float[,] tData = terrain.GetHeights(0, 0, w, h);

        w = (w - 1) / tRes + 1;
        h = (h - 1) / tRes + 1;
        Vector3[] tVertices = new Vector3[w * h];
        Vector2[] tUV = new Vector2[w * h];

        int[] tPolys;

        if (saveFormat == SaveFormat.Triangles)
        {
            tPolys = new int[(w - 1) * (h - 1) * 6];
        }
        else
        {
            tPolys = new int[(w - 1) * (h - 1) * 4];
        }

        float maxAlt = 0.0f, alt = 0.0f;

        // Build vertices and UVs
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                tVertices[y * w + x] = Vector3.Scale(meshScale, new Vector3(-y, tData[x * tRes, y * tRes], x)) + terrainPos;
                if (tVertices[y * w + x].y > maxAlt) maxAlt = tVertices[y * w + x].y;
                tUV[y * w + x] = Vector2.Scale(new Vector2(x * tRes, y * tRes), uvScale);
            }
        }

        if (godus)
        {
            for (int i = 0; i < tVertices.Length; i++)
            {
                alt = tVertices[i].y / maxAlt;
                if (alt > 0.9) tVertices[i].y = maxAlt * 1.0f;
                else if (alt > 0.8f) tVertices[i].y = maxAlt * 0.9f;
                else if (alt > 0.7f) tVertices[i].y = maxAlt * 0.8f;
                else if (alt > 0.6f) tVertices[i].y = maxAlt * 0.7f;
                else if (alt > 0.5f) tVertices[i].y = maxAlt * 0.6f;
                else if (alt > 0.4f) tVertices[i].y = maxAlt * 0.5f;
                else if (alt > 0.3f) tVertices[i].y = maxAlt * 0.4f;
                else if (alt > 0.2f) tVertices[i].y = maxAlt * 0.3f;
                else if (alt > 0.1f) tVertices[i].y = maxAlt * 0.2f;
                else if (alt != 0.0f) tVertices[i].y = maxAlt * 0.1f;
                else tVertices[i].y = 0.0f;
            }
        }

        float y1 = 0.0f, y2 = 0.0f, y3 = 0.0f;
        int index = 0;
        Mesh mesh = new Mesh();
        // Build triangle indices: 3 indices into vertex array for each triangle
        for (int y = 0; y < h - 1; y++)
        {
            for (int x = 0; x < w - 1; x++)
            {
                // For each grid cell output two triangles
                tPolys[index++] = (y * w) + x;
                tPolys[index++] = ((y + 1) * w) + x;
                tPolys[index++] = (y * w) + x + 1;

                y1 = tVertices[tPolys[index - 1]].y;
                y2 = tVertices[tPolys[index - 2]].y;
                y3 = tVertices[tPolys[index - 3]].y;

                alt = ((y1 + y2 + y3) / 3) / maxAlt;

                addSubMesh(alt, tPolys[index - 3], tPolys[index - 2], tPolys[index - 1], godus);
                    
                tPolys[index++] = ((y + 1) * w) + x;
                tPolys[index++] = ((y + 1) * w) + x + 1;
                tPolys[index++] = (y * w) + x + 1;

                y1 = tVertices[tPolys[index - 1]].y;
                y2 = tVertices[tPolys[index - 2]].y;
                y3 = tVertices[tPolys[index - 3]].y;

                alt = ((y1 + y2 + y3) / 3) / maxAlt;

                addSubMesh(alt, tPolys[index - 3], tPolys[index - 2], tPolys[index - 1], godus);
            }
        }
        
        //Suposadament fins a aqui ja tenim tot el que hauria de ser una MESH.
        //tVertices conté tots els vertexs
        //tUV conté totes les UVs
        //tPolys conté els triangles.

        mesh.vertices = tVertices;
        mesh.triangles = tPolys;
        mesh.uv = tUV;
        if (!godus)
        {
            mesh.subMeshCount = 6;
            mesh.SetTriangles(trisAigua.ToArray(), 0);
            mesh.SetTriangles(trisSorra.ToArray(), 1);
            mesh.SetTriangles(trisBosc.ToArray(), 2);
            mesh.SetTriangles(trisGespa.ToArray(), 3);
            mesh.SetTriangles(trisPedra.ToArray(), 4);
            mesh.SetTriangles(trisNeu.ToArray(), 5);
        }
        else
        {
            mesh.subMeshCount = 11;
            mesh.SetTriangles(trisAigua.ToArray(), 0);
            mesh.SetTriangles(trisGodus1.ToArray(), 1);
            mesh.SetTriangles(trisGodus2.ToArray(), 2);
            mesh.SetTriangles(trisGodus3.ToArray(), 3);
            mesh.SetTriangles(trisGodus4.ToArray(), 4);
            mesh.SetTriangles(trisGodus5.ToArray(), 5);
            mesh.SetTriangles(trisGodus6.ToArray(), 6);
            mesh.SetTriangles(trisGodus7.ToArray(), 7);
            mesh.SetTriangles(trisGodus8.ToArray(), 8);
            mesh.SetTriangles(trisGodus9.ToArray(), 9);
            mesh.SetTriangles(trisGodus10.ToArray(), 10);

        }
        return mesh;
    }

    void addSubMesh(float alt, int idx1, int idx2, int idx3, bool godus)
    {
        List<int> auxList;
        if (!godus)
        {
            if (alt >= 0.95f)
                auxList = trisNeu;
            else if (alt > 0.75f)
                auxList = trisPedra;
            else if (alt > 0.5f)
                auxList = trisGespa;
            else if (alt > 0.15f)
                auxList = trisBosc;
            else if (alt != 0)
                auxList = trisSorra;
            else
                auxList = trisAigua;
        }
        else
        {
            if (alt > 0.9) auxList = trisGodus10;
            else if (alt > 0.8f) auxList = trisGodus9;
            else if (alt > 0.7f) auxList = trisGodus8;
            else if (alt > 0.6f) auxList = trisGodus7;
            else if (alt > 0.5f) auxList = trisGodus6;
            else if (alt > 0.4f) auxList = trisGodus5;
            else if (alt > 0.3f) auxList = trisGodus4;
            else if (alt > 0.2f) auxList = trisGodus3;
            else if (alt > 0.1f) auxList = trisGodus2;
            else if (alt != 0.0f) auxList = trisGodus1;
            else auxList = trisAigua;
        }
        auxList.Add(idx1);
        auxList.Add(idx2);
        auxList.Add(idx3);


    }

}