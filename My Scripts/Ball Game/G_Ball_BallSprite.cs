using DG.Tweening;
using TMPro;
using UnityEngine;

public class G_Ball_BallSprite : MonoBehaviour
{
    [HideInInspector] public bool hasBall;
    [SerializeField] GameObject onVictoryEffectPrefab;
    [SerializeField] GameObject onVictoryTextPrefab;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    
    public bool isAddedToScore;

    bool hasDetonated;
    bool hasSpawnedText;
    
    private void Start()
    {
        hasDetonated = false;
        hasSpawnedText = false;
        
        if (spriteRenderer != null)
        {
            if (spriteRenderer.enabled)
            {
                hasBall = true;
            }
        }
    }

    public void SetScale(Vector3 scale)
    {
        transform.localScale = scale;
    }
    public void HandleMovement()
    {
        if(spriteRenderer == null) return;
        if(!hasBall) return;
        if(transform == null) return;
        
        var pos = spriteRenderer.transform.position.x + 3f;

        transform.DOMoveX(pos, 1f).SetEase(Ease.OutBack);
        transform.DOShakeScale(1f,0.15f);
    }

    public void AssignSprite(G_Ball_DefaultBall ball)
    {
        spriteRenderer.sprite = ball.ballType.ballIcon;
    }

    public void OnVictoryAndRemaining()
    {
        isAddedToScore = true;
    }

    public void SpawnOnVictoryParticles()
    {
        if (!hasDetonated)
        {
            hasDetonated = true;
            var effect = Instantiate(onVictoryEffectPrefab, transform);
            effect.transform.position = transform.GetChild(0).transform.position;
            effect.transform.localScale = Vector3.one * 2f;
            Invoke(nameof(TurnOffSpriteRenderer),0.06f);
        
            Destroy(effect,0.25f);
        }
    }

    public void SpawnOnVictoryText()
    {
        if (!hasSpawnedText)
        {
            hasSpawnedText = true;
            onVictoryTextPrefab.SetActive(true);
            onVictoryTextPrefab.transform.DOLocalMoveY(8f,0.8f).SetEase(Ease.InOutBack);
            onVictoryTextPrefab.transform.localScale = Vector3.one * 2.65f;
        
            Destroy(gameObject,1.5f);
        }
    }

    private void TurnOffSpriteRenderer()
    {
        spriteRenderer.enabled = false;
    }

}
