using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour,IInteractable
{
   public ItemData item;
   private float resetTime = 10f;
   
   public string GetInteractPrompt()
   {
      return string.Format("Pickup {0}", item.displayName);
   }

   public void OnInteract()
   {
      Inventory.instance.AddItem(item);
      if (item.type == ItemType.Equipable || item.type == ItemType.Resource)
      {
         Destroy(gameObject);
      }
      else
      {
         if (item.displayName != "물")
         {
            SetDisabled();
            Invoke(nameof(Reset),resetTime); 
         }
      }
   }
   
   void SetDisabled()
   {
      gameObject.GetComponent<MeshRenderer>().enabled = false;
      gameObject.GetComponent<Collider>().enabled = false;
   }

   void Reset()
   {
      gameObject.GetComponent<MeshRenderer>().enabled = true;
      gameObject.GetComponent<Collider>().enabled = true;
   }
}
