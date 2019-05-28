using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour, Attachable
{

    public Material m_OutlineMaterial;

    private Material m_DefaultMaterial;

    private Color m_defaultColor;

    public MeshRenderer meshRender;

    public float m_OutlineTimer = 3f;

    void Awake()
    {
        meshRender = this.GetComponent<MeshRenderer>();
        m_DefaultMaterial = meshRender.material;

        m_defaultColor = m_DefaultMaterial.color;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void attach(GameObject sword)
    {
        sword.transform.SetParent(this.gameObject.transform);
    }

    public void TurnOnOutline()
    {
        StopAllCoroutines();
        //m_OutlineMaterial.color = m_defaultColor;
        meshRender.material = m_OutlineMaterial;
        meshRender.material.color = m_defaultColor;
        StartCoroutine(OutlineTimer());
    }

    public void TurnOffOutline()
    {
        meshRender.material = m_DefaultMaterial;
    }

    private IEnumerator OutlineTimer()
    {
        yield return new WaitForSeconds(1f);
        TurnOffOutline();

    }
}
