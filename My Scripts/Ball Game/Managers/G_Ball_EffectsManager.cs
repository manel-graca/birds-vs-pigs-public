using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using Lean.Pool;

public class G_Ball_EffectsManager : MonoBehaviour
{
    public static G_Ball_EffectsManager instance;
    
    public GameObject blockCollisionParticles;
    
    public MMFeedbacks onBallHitFeedback;
    public MMFeedbacks onExplosionFeedback;
    public MMFeedbacks onVictoryFeedback;
    public MMFeedbacks onObjectiveDestroyed;
    

    [SerializeField] GameObject[] fireworks;
    [SerializeField] GameObject explosionPrefab;

    [SerializeField] GameObject pigDeathFX;
    [SerializeField] GameObject woodDestructFX;
    [SerializeField] GameObject stoneDestructFX;
    [SerializeField] GameObject objectiveDestructFX;
    [SerializeField] GameObject ballDestructFX;
    [SerializeField] GameObject ballDivisionFX;

    [SerializeField] Transform runtimeFXParent;
    
    G_Ball_SettingsManager settings;

    private void Awake()
    {
        if (instance == null){

            instance = this;
    
    
        } else {
            Destroy(this);
        }
    }

    private void Start()
    {
        settings = G_Ball_SettingsManager.instance;
    }

    public void SpawnFireworks()
    {
        foreach (var firework in fireworks)
        {
            firework.SetActive(true);
        }
    }

    public void PlayVictoryEffects()
    {
        SpawnFireworks();
        PlayFeedbacks("onVictory");
    }

    public void OnObjectiveDestroyedEffect(Vector2 spawnPos)
    {
        var ps = LeanPool.Spawn(objectiveDestructFX, spawnPos, Quaternion.identity);
        var time = ps.GetComponent<ParticleSystem>().main.duration;
        LeanPool.Despawn(ps,time);
    }

    public void OnBallDestroyedEffect(Vector2 spawnPos)
    {
        var ps = LeanPool.Spawn(ballDestructFX, spawnPos, Quaternion.identity);
        var time = ps.GetComponent<ParticleSystem>().main.duration;
        LeanPool.Despawn(ps,time);
    }

    public void OnBallCollisionEffect(bool a, Vector2 spawnPos)
    {
        PlayFeedbacks("onBallHit");
        var psStatic = LeanPool.Spawn(blockCollisionParticles, spawnPos, Quaternion.identity);
        psStatic.transform.SetParent(runtimeFXParent);
        LeanPool.Despawn(psStatic.gameObject, 5f);
    }

    public void OnExplosiveBallExplosion(Vector2 spawnPos)
    {
        //onExplosionFeedback play feedback
        var explosionPS = LeanPool.Spawn(explosionPrefab, spawnPos, Quaternion.identity);
        explosionPS.transform.SetParent(runtimeFXParent);
        Vibrate();
        LeanPool.Despawn(explosionPS.gameObject, 3f);
    }

    public void OnBallsDivided(Vector2 oldBallPos)
    {
        var fx = LeanPool.Spawn(ballDivisionFX, oldBallPos, Quaternion.identity);
        LeanPool.Despawn(fx, 2f);
    }

    public void OnBlockDeathEffect(BlockType block, Vector2 pos)
    {
        switch (block)
        {
            case BlockType.Stone:
                var ps1 = LeanPool.Spawn(stoneDestructFX, pos, Quaternion.identity);
                LeanPool.Despawn(ps1, 2f);
                break;
            case BlockType.Wood:
                var ps2 = LeanPool.Spawn(woodDestructFX, pos, Quaternion.identity);
                LeanPool.Despawn(ps2, 2f);
                break;
            case BlockType.Pig:
                var ps3 = LeanPool.Spawn(pigDeathFX, pos, Quaternion.identity);
                LeanPool.Despawn(ps3, 2f);
                break;
        }
    }

    public void Vibrate()
    {
        if (settings.GetIfVibrateOn())
        {
            Handheld.Vibrate();
        }
        else
        {
            Debug.LogWarning("Vibration is off");
        }
    }

    public void PlayFeedbacks(string feedbackName)
    {
        switch (feedbackName)
        {
            case "onVictory":
                onVictoryFeedback.PlayFeedbacks();
                break;
            case "onBlockDestroyed":
                
                break;
            case "onBallHit":
                onBallHitFeedback.PlayFeedbacks();
                break;
            case "onBombardierExplosion":
                
                break;
            case "onObjectiveDestroyed":
                onObjectiveDestroyed.PlayFeedbacks();
                break;
        }
    }
}