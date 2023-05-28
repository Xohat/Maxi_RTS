using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CSelectable : MonoBehaviour
{
    [HideInInspector]
    public bool selected = false;

    public MeshRenderer mesh;

    private Collider collider;
    private Material outlineMaterial;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        outlineMaterial = mesh.GetComponent<Renderer>().materials[1];
    }

    // Start is called before the first frame update
    void Start()
    {
        outlineMaterial.SetFloat("_Outline_Width", 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSelected()
    {
        outlineMaterial.SetFloat("_Outline_Width", 0.08f);
        selected = true;
    }

    public void SetDeselected()
    {
        outlineMaterial.SetFloat("_Outline_Width", 0f);
        selected = false;
    }

    public void SetColor(Color color)
    {
        outlineMaterial.SetColor("_Outline_Color", color);
    }

    private void OnDestroy()
    {
        Destroy(outlineMaterial);
    }
}
