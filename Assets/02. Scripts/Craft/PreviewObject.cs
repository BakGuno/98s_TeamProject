using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    public Building.Type needType;
    private bool needTypeFlag;

    private List<Collider> colliderList = new List<Collider>();

    [SerializeField] private int layerGround;
    private const int IGNORE_RAYCAST_LAYER = 2;

    [SerializeField] private Material green;
    [SerializeField] private Material red;



    // Update is called once per frame
    void Update()
    {
        ChangeColor();
    }

    private void ChangeColor()
    {
        if(needType == Building.Type.Normal)
        {
            if (colliderList.Count > 0)
                SetColor(red);
            else
                SetColor(green);
        }
        else
        {
            if (colliderList.Count > 0 || !needTypeFlag)
                SetColor(red);
            else
                SetColor(green);
        }
    }

    private void SetColor(Material material)
    {
        foreach (Transform child in this.transform)
        {
            Material[] newMaterials = new Material[child.GetComponent<Renderer>().materials.Length];

            for(int i = 0; i < newMaterials.Length; ++i)
            {
                newMaterials[i] = material;
            }

            child.GetComponent<Renderer>().materials = newMaterials;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Structure")
        {
            if (other.GetComponent<Building>().type != needType)
                colliderList.Add(other);
            else
                needTypeFlag = true;

        }
        else
        {
            if (other.gameObject.layer != layerGround && other.gameObject.layer != IGNORE_RAYCAST_LAYER)
            {
                colliderList.Add(other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Structure")
        {
            if (other.GetComponent<Building>().type != needType)
                colliderList.Remove(other);
            else
                needTypeFlag = false;

        }
        else
        {
            if (other.gameObject.layer != layerGround && other.gameObject.layer != IGNORE_RAYCAST_LAYER)
            {
                colliderList.Remove(other);
            }
        }
    }

    public bool IsBuildable()
    {
        if(needType == Building.Type.Normal)
            return colliderList.Count == 0;
        else
            return colliderList.Count == 0 && needTypeFlag;
    }
}
