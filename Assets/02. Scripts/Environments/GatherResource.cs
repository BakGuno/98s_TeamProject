using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherResource : MonoBehaviour
{
    public ItemData itemToGive;

    public int quantityPerHit = 1;
    public int capacity=5;

    private float resetTime = 10f;

    public void Gather(Vector3 hitPoint,Vector3 hitNormal)
    {
        for (int i = 0; i < quantityPerHit; i++)
        {
            if (capacity <= 0)
            {
                break;
            }

            capacity -= 1;
            Instantiate(itemToGive.dropPrefab,hitPoint+Vector3.up,Quaternion.LookRotation(hitNormal,Vector3.up));
            //충돌에 대한 방향벡터를 넘겨줌
        }

        if (capacity <= 0)
        {
            SetDisabled();
            Invoke(nameof(Reset),resetTime);
        }
            
    }

    void SetDisabled()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;
    }

    void Reset()
    {
        capacity = 5;
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        gameObject.GetComponent<Collider>().enabled = true;
    }

}
