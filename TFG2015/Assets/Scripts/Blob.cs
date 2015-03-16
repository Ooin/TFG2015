using UnityEngine;
using System.Collections;

public class Blob {

	//De moment sera una esfera situada a unes coordenades 2D del pla.
	//en un futur pot tenir un cert offset vertical per a no fer nomes mitja esfera, sino una mica menys o una mica mes.
	private float radius {
		get {
			return radius;
		}
	}
	private Vector2 coordinates {
		get {
			return coordinates;
		}
	}

	public Blob(float rad, Vector2 coord){
		this.radius = rad;
		this.coordinates = coord;
	}

}
