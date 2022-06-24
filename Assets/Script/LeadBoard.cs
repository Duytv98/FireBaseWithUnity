using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeadBoard : MonoBehaviour
{
    public static LeadBoard Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
}
