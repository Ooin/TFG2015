using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {

    private Island illa;

	// Use this for initialization
	void Awake () {
        GameObject x = GameObject.FindGameObjectWithTag("Island");
        illa = (Island) x.GetComponent(typeof(Island));
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void generateButton() {
        bool godus = GameObject.Find("GodusToggle").GetComponent<UnityEngine.UI.Toggle>().isOn;
        illa.generateNewIsland(godus, new Vector3(200, 200, 200));
    }
}
