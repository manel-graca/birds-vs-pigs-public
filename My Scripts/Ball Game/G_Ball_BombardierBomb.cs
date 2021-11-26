using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Ball_BombardierBomb : MonoBehaviour
{
    [SerializeField] SingleExplosion explosion;
    public void Explode()
    {
        explosion.Activate();
        G_Ball_EffectsManager.instance.OnExplosiveBallExplosion(transform.position);
        Debug.Log("exploded");
    }
    
    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Ground") || 
            other.transform.CompareTag("Block") || 
            other.transform.CompareTag("Objective"))
        {
            GetComponent<Animator>().SetTrigger("armed");
        }
    }
}
