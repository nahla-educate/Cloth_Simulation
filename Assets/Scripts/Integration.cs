using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Integration : MonoBehaviour
{
    public Mesh mesh;
    public Forces calculForces;

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < mesh.colonnes; i++)
        {
            for (int j = 0; j < mesh.lignes; j++)
            {
                calculForces.CalculateForces(i, j);
            }
        }

        for (int i = 0; i < mesh.colonnes; i++)
        {
            for (int j = 0; j < mesh.lignes; j++)
            {

                AppliquerForces(i, j, mesh.forces[i, j]);

            }
        }

    }

    void AppliquerForces(int i, int j, Vector3 force)
    {
        if (j == 0 && (i == 0 || i == mesh.colonnes - 1))
            return;

        //Euler method

        float deltaT = Time.deltaTime;

        //loi fondamentale de la dynamique F = m*a
        //calcul d'acceleration : a = F(i,j) / m
        Vector3 acceleration = mesh.forces[i, j] / calculForces.masse;

        //mettre à jour vitesse = vitesse + acc * deltaTime
        mesh.vitesses[i, j] += acceleration * deltaT;

        //mettre à jour position = position + vitesse * deltaTime
        mesh.spheres[i, j].transform.position += mesh.vitesses[i, j] * deltaT;
    }
}
