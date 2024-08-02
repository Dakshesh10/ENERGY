using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    private bool isActive = false;

    public event System.Action<bool> onStateChanged;

    public bool IsActive 
    { 
        get { return isActive; }
        
        set 
        { 
            isActive = value;
            onStateChanged?.Invoke(isActive);
        }
    }

    public void initializeSlot(System.Action<bool> callback)
    {
        onStateChanged += callback;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IsActive = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IsActive = false;
    }
}
