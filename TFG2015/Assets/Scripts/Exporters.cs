using UnityEngine;
using System.Collections;
using System.IO;

public class Exporters : MonoBehaviour {

    public void exportImatge(float[,] imatgeraw, int width, int depth)
    {
        Texture2D imatgeBN = new Texture2D(width, depth);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                imatgeBN.SetPixel(i, j, new Color(imatgeraw[i, j], imatgeraw[i, j], imatgeraw[i, j]));
            }
        }

        byte[] bytes = imatgeBN.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/../imgFinal.png", bytes);
    }

}
