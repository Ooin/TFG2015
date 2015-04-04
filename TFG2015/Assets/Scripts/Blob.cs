using UnityEngine;
using System.Collections;

public class Blob {

	//De moment sera una esfera situada a unes coordenades 2D del pla.
	//en un futur pot tenir un cert offset vertical per a no fer nomes mitja esfera, sino una mica menys o una mica mes.
    public float radius;
    public float x;
    public float y;

	public Blob(float rad, float x, float y){
		this.radius = rad;
		this.x = x;
        this.y = y;
	}

}
