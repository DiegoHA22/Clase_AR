using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public GameObject model;
    public Color color, color2, color3, color4, color5;
    public Material colorMaterial;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeColor_BTN()
    {
    Color[] colors = { color, color2, color3, color4, color5 };
    Color randomColor = colors[Random.Range(0, colors.Length)];

    model.GetComponent<Renderer>().material.color = randomColor;
    colorMaterial.color = randomColor;
    }
}
