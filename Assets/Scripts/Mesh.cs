using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mesh : MonoBehaviour
{
    public GameObject spherePrefab;
    public int lignes = 10;  // lignes
    public int colonnes = 10;  // colonnes

    //initialiser matrices : spheres, forces et vitesses
    public GameObject[,] spheres;
    public Vector3[,] forces;
    public Vector3[,] vitesses;

    public float espacement = 1.0f;  // espacement entre les spheres

    // Start is called before the first frame update
    void Start()
    {
        //construire matrice de spheres ( générer les sphéres )
        spheres = new GameObject[colonnes, lignes];
        vitesses = new Vector3[colonnes, lignes];
        forces = new Vector3[colonnes, lignes];

        for (int i = 0; i < colonnes; i++)
        {
            for (int j = 0; j < lignes; j++)
            {
                // j lignes  i colonnes
                Vector3 position = new Vector3(i * espacement, -j * espacement, 0);
                spheres[i, j] = Instantiate(spherePrefab, position, Quaternion.identity);
                spheres[i, j].name = "Sphere [" + i + "," + j + "]";
            }
        }

    }
}
