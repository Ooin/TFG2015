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

    //[MenuItem("Terrain/Export To Obj...")]
    public Mesh Init(Terrain terrainObject, SaveFormat saveFormat, SaveResolution saveResolution)
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

        Mesh temp = Export();
        return temp;
    }
    
    private Mesh Export()
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

        float maxAlt = 0.0f;

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

        float alt = 0.0f, y1 = 0.0f, y2 = 0.0f, y3 = 0.0f;
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

                addSubMesh(alt, tPolys[index - 3], tPolys[index - 2], tPolys[index - 1]);
                    
                tPolys[index++] = ((y + 1) * w) + x;
                tPolys[index++] = ((y + 1) * w) + x + 1;
                tPolys[index++] = (y * w) + x + 1;

                y1 = tVertices[tPolys[index - 1]].y;
                y2 = tVertices[tPolys[index - 2]].y;
                y3 = tVertices[tPolys[index - 3]].y;

                alt = ((y1 + y2 + y3) / 3) / maxAlt;

                addSubMesh(alt, tPolys[index - 3], tPolys[index - 2], tPolys[index - 1]);
            }
        }
        
        //Suposadament fins a aqui ja tenim tot el que hauria de ser una MESH.
        //tVertices conté tots els vertexs
        //tUV conté totes les UVs
        //tPolys conté els triangles.

        
        mesh.vertices = tVertices;
        mesh.triangles = tPolys;
        mesh.uv = tUV;

        mesh.subMeshCount = 6;
        mesh.SetTriangles(trisAigua.ToArray(), 0);
        mesh.SetTriangles(trisSorra.ToArray(), 1);
        mesh.SetTriangles(trisBosc.ToArray(), 2);
        mesh.SetTriangles(trisGespa.ToArray(), 3);
        mesh.SetTriangles(trisPedra.ToArray(), 4);
        mesh.SetTriangles(trisNeu.ToArray(), 5);

        return mesh;
    }

    void addSubMesh(float alt, int idx1, int idx2, int idx3)
    {
        List<int> auxList;

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
        auxList.Add(idx1);
        auxList.Add(idx2);
        auxList.Add(idx3);


    }

}