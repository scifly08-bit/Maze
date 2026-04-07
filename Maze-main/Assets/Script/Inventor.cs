using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventor : MonoBehaviour
{
    private int ActiveSlot = 0;
    private int LateSlot = 0;
    private Vector3 UnActive = new Vector3(2, 2, 2);
    private Vector3 Active = new Vector3(2.5f, 2.5f, 2.5f);
    [SerializeField] private float Taking_Distance;
    [SerializeField] private Transform Hand;
    
    [SerializeField] List<InventorSlot> slots = new List<InventorSlot>();
    private void Scroll()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (ActiveSlot != slots.Count - 1)
            {
                LateSlot = ActiveSlot;
            }

            ActiveSlot = Mathf.Clamp(ActiveSlot + 1, 0, slots.Count - 1);
            SelectSlot();
        }
        
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (ActiveSlot != 0)
            {
                LateSlot = ActiveSlot;
            }
            ActiveSlot = Mathf.Clamp(ActiveSlot - 1, 0, slots.Count - 1);
            SelectSlot();
        }
        
    }

    private void Switch()
    {
        if (Input.GetKey(KeyCode.Alpha1)&& ActiveSlot!= 0)
        {
            LateSlot = ActiveSlot;
            ActiveSlot = 0;
            SelectSlot();
        }

        if (Input.GetKey(KeyCode.Alpha2)&& ActiveSlot!= 1)
        {
            LateSlot = ActiveSlot;
            ActiveSlot = 1;
            SelectSlot();
        }

        if (Input.GetKey(KeyCode.Alpha3)&& ActiveSlot!= 2)
        {
            LateSlot = ActiveSlot;
            ActiveSlot = 2;
            SelectSlot();
        }

        if (Input.GetKey(KeyCode.Alpha4)&& ActiveSlot!= 3)
        {
            LateSlot = ActiveSlot;
            ActiveSlot = 3;
            SelectSlot();
        }
    }

    private void SelectSlot()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i == ActiveSlot)
            {
                slots[i].Transformation.localScale = Active;
                if (slots[i].ConnectedItem != null) slots[i].ConnectedItem.SetActive(true);
            }
            else
            {
                slots[i].Transformation.localScale = UnActive;
                if (slots[i].ConnectedItem != null) slots[i].ConnectedItem.SetActive(false);
            }
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SelectSlot();
    }

    // Update is called once per frame
    void Update()
    {
      Scroll(); 
      Switch();
      Interact();
    }
    void Interact()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Transform rayOrigin = Camera.main ? Camera.main.transform : transform;
            
            LayerMask Ray = 1 << 3;
            RaycastHit hit;
            Debug.DrawRay(rayOrigin.position, rayOrigin.forward * Taking_Distance, Color.red);
            
            if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out hit, Taking_Distance, Ray))
            {
                if(hit.collider.TryGetComponent(out Interactable interactable))
                {
                    // Fallback if Hand is not assigned
                    Transform targetHand = Hand != null ? Hand : rayOrigin;
                    interactable.Interact(slots[ActiveSlot], targetHand); 
                }
            }   
        }
    }
}
