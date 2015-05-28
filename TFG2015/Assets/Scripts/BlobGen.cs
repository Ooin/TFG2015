using UnityEngine;
using System.Collections.Generic;

public static class BlobGen {

    //Restraints
    // Height/Radius !> min(Width ,Depth) / 2

    public static List<Blob> generateBlobs(int blobAmmount, Vector3 worldSize)
    {
        if (blobAmmount > 0)
        {
            float height = restraintHeight(worldSize), width = worldSize.x, depth = worldSize.z;
            List<Blob> llista = new List<Blob>();
            Blob auxBlob = getRandomBlob(width, height, depth);

            llista.Add(auxBlob);

            bool done = false;

            for (int i = 1; i < blobAmmount; i++)
            {
                while (!done)
                {
                    auxBlob = getRandomBlob(width, height, depth);
                }

            }
        }
        return new List<Blob>();
    }

    
    //Cap blob sortira per la vora del terrain, com a màxim ocupara el 100% de l'amplada o l'alçada(el que sigui mes petit dels dos)
    //Retorna el radi máxim que pot tenir una esfera per a que no es surti de l'escenari
    private static float restraintHeight(Vector3 worldSize)
    {
        float width = worldSize.x;
        float height = worldSize.y;
        float depth = worldSize.z;

        if (2 * height > width || 2 * height > depth) height = Mathf.Min(depth, width) / 2;

        return height;
    }

    //genera un blob que estigui completament dintre del escenari
    private static Blob getRandomBlob(float width, float height, float depth)
    {
        float x = Random.Range(0, width);
        float y = Random.Range(0, depth);
        float radius = Random.Range(0, height);
        bool surt = true;
        while (surt)
        {
            if (x + radius <= width && y + radius < depth && x >= radius && y >= radius) return new Blob(radius, x, y, 1, 0);
            x = Random.Range(0, width);
            y = Random.Range(0, depth);
            radius = Random.Range(0, height);
        }
        return new Blob(0.0f, 0.0f, 0.0f, 1, 0);
    }
}