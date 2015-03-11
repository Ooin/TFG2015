using UnityEngine;
using System.Collections.Generic;

public class Island : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.AddComponent <TerrainCollider> ();
		gameObject.AddComponent <Terrain> ();
		TerrainData illa = new TerrainData ();

		illa.size = new Vector3(10, 600, 10);
		illa.heightmapResolution = 512;
		illa.baseMapResolution = 1024;
		illa.SetDetailResolution(1024, 16);

		int heightmapWidth = illa.heightmapWidth;
		int heightmapHeight = illa.heightmapHeight;
		TerrainCollider terrainCollider = gameObject.GetComponent<TerrainCollider>();
		Terrain terrain = gameObject.GetComponent<Terrain>();
		terrainCollider.terrainData= illa;
		terrain.terrainData = illa;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


}
