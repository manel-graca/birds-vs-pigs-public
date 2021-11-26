using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObject : MonoBehaviour
{
    public static PersistentObject instance;

    private void Awake()
    {
        if (instance == null){

            instance = this;
            DontDestroyOnLoad(this.gameObject);
    
        } else {
            Destroy(this);
        }
    }
}
