using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forces : MonoBehaviour
{
    public Mesh mesh;
    public LineDraw draw;

    public float K1 = 20;  //Ks de ressort 1
    public float K2 = 15f;  //Ks de ressort 2
    public float K3 = 10;  //Ks de ressort 3

    public float longueurRessortAvide1 = 1f;
    public float longueurRessortAvide2 = 1.5f;
    public float longueurRessortAvide3 = 2f;

    public float amortissementRessort1 = 0.05f;
    public float amortissementRessort2 = 0.05f;
    public float amortissementRessort3 = 0.05f;

    public float masse = 0.05f;  // masse des spheres
    public float Cdis = 0.1f; //coefficient d'amortissement
    public Vector3 gravity;  
    public float viscosityConstant = 0.1f;
    public Vector3 vitesseAir = new Vector3(7, 0, 0);


    private Vector3 forceAmorti;
    private Vector3 P;

    void Start()
    {
        gravity = new Vector3(0, -9.8f, 0);
    }

    public void CalculateForces(int i, int j)
    {
        mesh.forces[i, j] = Vector3.zero;

        P = gravity * masse;
        forceAmorti = -Cdis * mesh.vitesses[i, j]; // viscous damping F = -C * v(i,j)

        mesh.forces[i, j] += P;
        mesh.forces[i, j] += forceAmorti;
        //calculer les forces des ressorts
        mesh.forces[i, j] += ForceRessorts(i, j);
        //force d'air
        mesh.forces[i, j] += ForceAir(i, j);
    }

    //calculer les forces des ressorts
    Vector3 ForceRessorts(int i, int j)
    {
        Vector3 forceRessort = Vector3.zero;

        //type 1 : structural springs
        //springs linking [i,j] and [i+1,j]  
        if (i > 0)
        {
            forceRessort += CalculateforceRessort(i, j, i - 1, j, longueurRessortAvide1, K1, amortissementRessort1);
            draw.DrawSpring(i, j, i - 1, j);
        }
        if (i < mesh.colonnes - 1)
        {
            forceRessort += CalculateforceRessort(i, j, i + 1, j, longueurRessortAvide1, K1, amortissementRessort1);
            draw.DrawSpring(i, j, i + 1, j);
        }
        // springs linking [i,j] and [i,j+1] 
        if (j > 0)
        {
            forceRessort += CalculateforceRessort(i, j, i, j - 1, longueurRessortAvide1, K1, amortissementRessort1);
            draw.DrawSpring(i, j, i, j - 1);
        }
        if (j < mesh.lignes - 1)
        {
            forceRessort += CalculateforceRessort(i, j, i, j + 1, longueurRessortAvide1, K1, amortissementRessort1);
             draw.DrawSpring(i, j, i, j + 1);
        }
        //type 2 : shear springs
        // springs linking [i,j] and [i+1,j+1] 
        if (j < mesh.lignes - 1 && i > 0)
        {
            forceRessort += CalculateforceRessort(i, j, i - 1, j + 1, longueurRessortAvide2, K2, amortissementRessort2);
             draw.DrawSpring(i, j, i - 1, j + 1);
        }

        if (j < mesh.lignes - 1 && i < mesh.colonnes - 1)
        {
            forceRessort += CalculateforceRessort(i, j, i + 1, j + 1, longueurRessortAvide2, K2, amortissementRessort2);
             draw.DrawSpring(i, j, i + 1, j + 1);
        }
        //springs linking [i+1,j] and [i,j+1] 
        if (j > 0 && i > 0)
        {
            forceRessort += CalculateforceRessort(i, j, i - 1, j - 1, longueurRessortAvide2, K2, amortissementRessort2);
             draw.DrawSpring(i, j, i - 1, j - 1);
        }

        if (j > 0 && i < mesh.colonnes - 1)
        {
            forceRessort += CalculateforceRessort(i, j, i + 1, j - 1, longueurRessortAvide2, K2, amortissementRessort2);
             draw.DrawSpring(i, j, i + 1, j - 1);
        }
        //type 3 : flexion springs
        //springs linking [i,j] and [i+2,j] 
        if (i > 1)
        {
            forceRessort += CalculateforceRessort(i, j, i - 2, j, longueurRessortAvide3, K3, amortissementRessort3);
             draw.DrawSpring(i, j, i - 2, j);
        }
        if (i < mesh.colonnes - 2)
        {
            forceRessort += CalculateforceRessort(i, j, i + 2, j, longueurRessortAvide3, K3, amortissementRessort3);
             draw.DrawSpring(i, j, i + 2, j);
        }
        //springs linking [i,j] and [i,j+2] 
        if (j > 1)
        {
            forceRessort += CalculateforceRessort(i, j, i, j - 2, longueurRessortAvide3, K3, amortissementRessort3);
             draw.DrawSpring(i, j, i, j - 2);
        }
        if (j < mesh.lignes - 2)
        {
            forceRessort += CalculateforceRessort(i, j, i, j + 2, longueurRessortAvide3, K3, amortissementRessort3);
             draw.DrawSpring(i, j, i, j + 2);
        }


        return forceRessort;
    }

    Vector3 CalculateforceRessort(int x1, int x2, int x3, int x4, float longueurRessortAvide, float Ks, float amortissementRessort)
    {
        Vector3 vecteurDistance = mesh.spheres[x3, x4].transform.position - mesh.spheres[x1, x2].transform.position;
       
        float distance = vecteurDistance.magnitude;
        
        Vector3 direction = vecteurDistance.normalized;

        // Loi de hook (force de rappel de ressort)
        Vector3 tensionRessort = Ks * (distance - longueurRessortAvide) * direction;

        return tensionRessort;
    }

    Vector3 ForceAir(int i, int j)
    {
        // Calculer la différence de vitesse entre le fluide(air) et sphere
        Vector3 difVitesses = vitesseAir - mesh.vitesses[i, j];

        // Calculer la force visqueuse : F = C * [n(i,j) . (u_fluid - v)] * n(i,j)
        float viscousForceMagnitude = viscosityConstant * Vector3.Dot(UnitNormalSurface(i, j), difVitesses);
        Vector3 viscosityForce = viscousForceMagnitude * UnitNormalSurface(i, j);

        return viscosityForce;
    }

    Vector3 UnitNormalSurface(int i, int j)
    {
        Vector3 normal = Vector3.zero;
        Vector3 position = mesh.spheres[i, j].transform.position;

        Vector3 voisinDroite, voisinGauche, voisinHaut, voisinBas;

        if (i < mesh.colonnes - 1)
            voisinDroite = mesh.spheres[i + 1, j].transform.position;
        else
            voisinDroite = position;

        if (i > 0)
            voisinGauche = mesh.spheres[i - 1, j].transform.position;
        else
            voisinGauche = position;

        if (j < mesh.lignes - 1)
            voisinHaut = mesh.spheres[i, j + 1].transform.position;
        else
            voisinHaut = position;

        if (j > 0)
            voisinBas = mesh.spheres[i, j - 1].transform.position;
        else
            voisinBas = position;

        // Calculer les vecteurs entre les voisins (gauche, droite et haut, bas)
        Vector3 vecteurHorizontale = voisinGauche - voisinDroite;
        Vector3 vecteurVerticale = voisinHaut - voisinBas;

        // Calculer la normale : (Cross : produit vectoriel)
        // N = A*B/ ||A*B||
        normal = Vector3.Cross(vecteurHorizontale, vecteurVerticale).normalized;


        return normal;
    }
}
