using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;
using Sirenix.OdinInspector;

public enum BlockType
{
    Wood,
    Stone,
    Glass,
    Pig
}

public class G_Ball_Block : MonoBehaviour
{
    //[InfoBox(" <b>Wooden Blocks -> 100HP</b> \n <b>Stone Blocks -> 200HP</b>", InfoMessageType.Info)]

    [BoxGroup("Core Variables", CenterLabel = true)]
    [EnumToggleButtons] public BlockType blockType;
    [BoxGroup("Core Variables")]
    [Range(10, 750)] [SerializeField] private int startingHP;
    [Range(1f, 100f)] [SerializeField] private float damageMultiplier;
    public int scoreToAddWhenDestroyed;
    public bool startsStatic;

    [SerializeField] GameObject[] sprites;

    private float rbVel;
    private int currentHP;
    private bool sprite1enabled;
    private bool sprite2enabled;
    private bool sprite3enabled;

    private Rigidbody2D rb;
    private bool canTakeDamage;

    private G_Ball_GameManager manager;

    private void Start()
    {
        manager = G_Ball_GameManager.instance;
        rb = GetComponent<Rigidbody2D>();
        if (startsStatic)
        {
            SetKinematic(true);
        }

        currentHP = startingHP;
        sprite1enabled = true;
        sprite2enabled = false;
        sprite3enabled = false;
        
        StartCoroutine(StartRoutine());
    }

    private void Update()
    {
        if (rb == null) return;
        if (manager.isInputBlock)
        {
            rb.isKinematic = true;
            rb.simulated = false;
        }
        else
        {
            rb.isKinematic = false;
            rb.simulated = true;
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;
        rbVel = rb.velocity.magnitude;
    }
    
    
    public void SetBlocksDetection(CollisionDetectionMode2D collisionDetection)
    {
        rb.collisionDetectionMode = collisionDetection;
    }

    public void SetKinematic(bool kinematic)
    {
        rb.isKinematic = kinematic;
    }

    private void HandleDamage()
    {
        if (currentHP < (startingHP / 2))
        {

            sprites[0].SetActive(false);
            sprites[1].SetActive(true);
        }
        if (currentHP < (startingHP / 3))
        {

            sprites[0].SetActive(false);
            sprites[1].SetActive(false);
            sprites[2].SetActive(true);
        }
    }

    public void TakeDamage(float amount)
    {
        if (!canTakeDamage) return;
        currentHP -= (int)amount;
        if (currentHP <= 0)
        {
            Die();
        }
        HandleDamage();
    }

    private float GetRbVelocity()
    {
        return rbVel;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Ball"))
        {
            var vel = other.relativeVelocity.magnitude;
            vel += other.transform.GetComponent<Rigidbody2D>().mass;
            TakeDamage(vel);
            return;
        }

        var damage = GetRbVelocity();
        damage *= damageMultiplier;
        TakeDamage(damage);
    }

    IEnumerator StartRoutine()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(2f);
        canTakeDamage = true;
    }

    private void Die()
    {
        G_Ball_LevelManager.instance.AddToScore(scoreToAddWhenDestroyed);

        sprites[0].SetActive(false);
        sprites[1].SetActive(false);
        sprites[2].SetActive(true);

        if (blockType == BlockType.Pig)
        { 
            //soundManager.PlayEffectSound(pigGrunt);
            G_Ball_LevelManager.instance.WinGame();
        }

        G_Ball_EffectsManager.instance.OnBlockDeathEffect(blockType, transform.position);
        Destroy(gameObject, 0.1f);
    }
}