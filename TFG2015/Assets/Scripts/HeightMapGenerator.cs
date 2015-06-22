using UnityEngine;
using System.Collections;
using System.IO;

public class HeightMapGenerator {

    enum SelectorColors
    {
        Neu,
        Pedra,
        Gespa,
        Bosc,
        Sorra,
        Aigua
    };

    private int height;
    private int width;
    private int depth;
    private int heightmapResolution;
    
    public HeightMapGenerator() { }

    private float[,] giveHeightMap(Blob[] illes)
    {
        float[,] worldHeights = new float[width, depth];
        Texture2D imatgeBN = new Texture2D(width, depth);
        Vector2 auxVec1, auxVec2;
        float dist, alt;
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
                    if (auxBlob.shape == 0)
                    {
                        dist = (auxVec1 - auxVec2).magnitude;
                        if (dist < auxBlob.radius)
                        {

                            
                            //alt = phi(dist);
                            //if (alt < auxAlt) alt = auxAlt;

                            switch (auxBlob.profile)
                            {
                                case 0:
                                    alt += gauss(auxBlob.radius/2, 10.0f, dist);
                                    break;
                                case 1:
                                    alt += auxBlob.radius / 2;
                                    break;
                                case 2:
                                    alt += Mathf.Sqrt(Mathf.Pow(auxBlob.radius, 2) - Mathf.Pow(dist, 2));
                                    break;
                            }
                        }
                        
                       
                    }
                    else
                    {
                        float minX = auxBlob.x - auxBlob.radius;
                        float maxX = auxBlob.x + auxBlob.radius;
                        float minY = auxBlob.y - auxBlob.radius;
                        float maxY = auxBlob.y + auxBlob.radius;
                        if (i >= minX && i <= maxX && j >= minY && j <= maxY)
                        {
                            switch (auxBlob.profile)
                            {
                                case 0:
                                    alt += gauss(auxBlob.radius/2, 10.0f, (auxVec1 - auxVec2).magnitude);
                                    break;
                                case 1:
                                    alt += auxBlob.radius / 2;
                                    break;
                                default:
                                    break;
                            }
                            
                        }
                    }
                }
                worldHeights[i, j] = alt;
            }
        }
        
        //normalitzem les alçades
        count = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                worldHeights[i, j] = (float)worldHeights[i, j] / height;
                if (worldHeights[i, j] > 0) count++;
            }
        }
        
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                imatgeBN.SetPixel(i, j, new Color(worldHeights[i, j], worldHeights[i, j], worldHeights[i, j]));
            }
        }
        
        //byte[] bytes = imatgeBN.EncodeToPNG();
        //File.WriteAllBytes(Application.dataPath + "/../imgOriginal.png", bytes);

        //imatgeBN = Soroll.aplicaSorollCostes(imatgeBN, width, depth);
        TextureScale.Bilinear(imatgeBN, heightmapResolution, heightmapResolution);

        //bytes = imatgeBN.EncodeToPNG();
        //File.WriteAllBytes(Application.dataPath + "/../imgFinal.png", bytes);

        float[,] mapHeights = new float[heightmapResolution, heightmapResolution];
        for (int i = 0; i < heightmapResolution; i++)
        {
            for (int j = 0; j < heightmapResolution; j++)
            {
                mapHeights[i, j] = (float)imatgeBN.GetPixel(i, j).r; //es indiferent si agafem R, G o B ja que tenen el mateix valor
            }
        }

        return mapHeights;
    }

    //Retorna l'alçada d'una X respecte el su centre en un campana de gawuss
    //hmax:     Alçada màxima presa quan la X = 0
    //sigma:    Amplada de la campana
    //x:        Punt a calcular
    private float gauss(float hmax, float sigma, float x)
    {
        float a = Mathf.Pow((x / sigma), 2) * -1;
        return hmax * Mathf.Exp(a);
    }

    public float[,] generateHeightMap(int width, int height, int depth, int heightmapResolution, int nBlobs, int profile)
    {

        this.width = width;
        this.height = height;
        this.depth = depth;
        this.heightmapResolution = heightmapResolution;

        Blob[] illes = new Blob[nBlobs];
        float radi, auxX, auxY;//coordenades auxiliar per a generar els randoms
        int auxShape = 0;

        for (int i = 0; i < nBlobs; i++)
        {
            radi = Random.Range(0, height / 4);
            auxX = Random.Range((int)width / 4, (int)3 * width / 4);
            auxY = Random.Range((int)depth / 4, (int)3 * depth / 4);
            auxShape = Random.Range(0, 2);
           
            
            illes[i] = new Blob(radi, auxX, auxY, auxShape, profile);
        }

        return giveHeightMap(illes);
    }


    private float phi(float x)
    {
        // constants
        double a1 = 0.254829592;
        double a2 = -0.284496736;
        double a3 = 1.421413741;
        double a4 = -1.453152027;
        double a5 = 1.061405429;
        double p = 0.3275911;

        // Save the sign of x
        int sign = 1;
        if (x < 0)
            sign = -1;
        x = x / Mathf.Sqrt(2.0f);
        
        // A&S formula 7.1.26
        double t = 1.0 / (1.0 + p * x);
        double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Mathf.Exp(-x * x);

        return (float) (0.5 * (1.0 + sign * y));
    }


 	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
