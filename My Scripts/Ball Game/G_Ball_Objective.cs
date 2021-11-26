using System;
using System.Collections;
using Slicer2D;
using UnityEngine.UI;
using UnityEngine;

public class G_Ball_Objective : MonoBehaviour
{
    [SerializeField] float velocityToDestruct;
    [SerializeField] float startingHP;
    [SerializeField] GameObject hpCanvas;
    [SerializeField] Image healthBarFront;
    [SerializeField] GameObject victoryBomb;
    
    public int scoreToAddWhenDestroyed = 250;
    
    
    [SerializeField] GameObject runtimeEffectsParent;
    
    public bool isAlive = true;
    
    private bool wasDestructed = true;
    
    Rigidbody2D rb;
    PolygonCollider2D col;
    
    G_Ball_GameManager manager;
    G_Ball_LevelManager levelManager;
    G_Ball_EffectsManager fxManager;
    
    private float rbVelocity;
    private float currentHP;
    
    void Start()
    {
        levelManager = G_Ball_LevelManager.instance;
        manager = G_Ball_GameManager.instance;
        fxManager = G_Ball_EffectsManager.instance;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<PolygonCollider2D>();
        
        currentHP = startingHP;
        healthBarFront.fillAmount = 1f;
    }

    void FixedUpdate()
    {
        if(rb == null) return;
        rbVelocity = rb.velocity.magnitude;
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        
        var hpPercent = currentHP / startingHP;
        healthBarFront.fillAmount = hpPercent;
        
        if (currentHP <= 0)
        {
            Die();
        }
    }

    public float GetCurrentHP()
    {
        return currentHP;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Boundaries"))
        {
            TakeDamage(startingHP * 10);
        }
        
        if (other.transform.CompareTag("Block") || other.transform.CompareTag("Ball"))
        {
            if (other.transform.CompareTag("Block"))
            {
                var damage = other.relativeVelocity.magnitude;
                TakeDamage(damage);
                return;
            }

            if (other.transform.CompareTag("Ball"))
            {
                var damage = other.relativeVelocity.magnitude;
                if (damage < 10f)
                {
                    damage *= 2;
                }
                TakeDamage(damage);
                return;
            }
            return;
        }
        
        var damage2 = rbVelocity;
        TakeDamage(damage2);
    }

    private void Die()
    {
        isAlive = false;
        if (wasDestructed)
        {
            Destroy(rb);
            Destroy(col);
            
            runtimeEffectsParent.SetActive(false);
            hpCanvas.SetActive(false);
            
            fxManager.PlayFeedbacks("onObjectiveDestroyed");
            levelManager.WinGame();
            StartCoroutine(DestructionRoutine());
            
            wasDestructed = false;
        }
    }

    IEnumerator DestructionRoutine()
    {
        GetComponentInChildren<Sliceable2D>().Explode(8);
        fxManager.OnObjectiveDestroyedEffect(transform.position + Vector3.up * 2);
        yield return null;
        
        var childs = transform.GetComponentsInChildren<Sliceable2D>();
        foreach (var var in childs)
        {
            var newRb = var.transform.gameObject.AddComponent<Rigidbody2D>();
            newRb.mass = 2;
            newRb.gravityScale = 3;
        }
        
        yield return new WaitForSeconds(0.1f);
        victoryBomb.SetActive(true);   
        yield return new WaitForSeconds(0.35f);
        levelManager.AddToScore(scoreToAddWhenDestroyed);
        yield return new WaitForSeconds(1f);
        yield return null;
        this.enabled = false;
    }

}
