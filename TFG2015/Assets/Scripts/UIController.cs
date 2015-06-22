using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System;

public class UIController : MonoBehaviour {

    private Island illa;
    private Vector3 coordenades;
    private int numBlobs;
    private int profile;
    private Toggle gauss, cub, esfera;
    private int resolucio;

	// Use this for initialization
	void Awake () {
        GameObject x = GameObject.FindGameObjectWithTag("Island");
        illa = (Island) x.GetComponent(typeof(Island));
        coordenades = new Vector3(200, 200, 200);
        numBlobs = 10;
        profile = 0;
        resolucio = 256;
        gauss = GameObject.Find("selectorGauss").GetComponent<UnityEngine.UI.Toggle>();
        esfera = GameObject.Find("selectorSemiEsfera").GetComponent<UnityEngine.UI.Toggle>();
        cub = GameObject.Find("selectorCub").GetComponent<UnityEngine.UI.Toggle>();
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void generateButton() {
        bool godus = GameObject.Find("GodusToggle").GetComponent<UnityEngine.UI.Toggle>().isOn;
        if (gauss.isOn) profile = 0;
        else if (cub.isOn) profile = 1;
        else profile = 2;
        illa.generateNewIsland(godus, coordenades, numBlobs, profile);
    }

    public void getUIValueX(UnityEngine.UI.InputField IF)
    {
        coordenades.x = float.Parse(IF.text);
    }

    public void getUIValueY(UnityEngine.UI.InputField IF)
    {
        coordenades.y = float.Parse(IF.text);
    }

    public void getUIValueZ(UnityEngine.UI.InputField IF)
    {
        coordenades.z = float.Parse(IF.text);
    }

    public void setnumBlobs(UnityEngine.UI.InputField IF)
    {
        numBlobs = int.Parse(IF.text);
    }

    public void setResolucio(InputField IF)
    {
        resolucio = int.Parse(IF.text);
    }

    public void exportImatge()
    {
        Texture2D imatgeBN = new Texture2D(illa.heightmapResolution, illa.heightmapResolution);

        Debug.Log("La resolucio del HeightMap es de " + illa.heightmapResolution.ToString());

        for (int i = 0; i < illa.heightmapResolution; i++)
        {
            for (int j = 0; j < illa.heightmapResolution; j++)
            {
                imatgeBN.SetPixel(i, j, new Color(illa.mapHeights[i, j], illa.mapHeights[i, j], illa.mapHeights[i, j]));
            }
        }

        byte[] bytes = imatgeBN.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/../imgFinal.png", bytes);
    }
}
