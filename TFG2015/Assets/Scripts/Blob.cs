using UnityEngine;
using System.Collections;

public class Blob {

	//De moment sera una esfera situada a unes coordenades 2D del pla.
	//en un futur pot tenir un cert offset vertical per a no fer nomes mitja esfera, sino una mica menys o una mica mes.
    public float radius;
    public float x;
    public float y;
    public int shape; //0 = circle, 1 = square
    public int profile; //0 = gauss, 1 = quadrat, 2 = semiesferic

	public Blob(float rad, float x, float y, int shape, int profile){
		this.radius = rad;
		this.x = x;
        this.y = y;
        this.shape = shape;
        this.profile = profile;
	}

}
