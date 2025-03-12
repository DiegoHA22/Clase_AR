using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColMat : MonoBehaviour
{
    public GameObject model;
    public Color color, color2, color3, color4, color5;
    public Material[] materials; // Arreglo de materiales con 5 opciones.

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ChangeColor_BTN()
    {
        // Selección aleatoria de color
        Color[] colors = { color, color2, color3, color4, color5 };
        Color randomColor = colors[Random.Range(0, colors.Length)];

        // Selección aleatoria de material
        Material randomMaterial = materials[Random.Range(0, materials.Length)];

        // Aplicar material y cambiar su color
        model.GetComponent<Renderer>().material = randomMaterial;
        randomMaterial.color = randomColor;
    }
}
