using System.Collections;
using Animancer;
using DG.Tweening;
using UnityEngine;

public class G_Ball_UI_MenuBirds : MonoBehaviour
{
    [SerializeField] private AnimancerComponent animancer;
    [SerializeField] private AnimationClip anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flySpeed;
    
    [SerializeField] private float maxX;
    [SerializeField] private float minX;
    
    private Vector3 dir = Vector3.left;
    
    bool flipped;


    private void Start()
    {
        animancer.Play(anim).Play();
    }

    void Update()
    {
        transform.Translate(dir * flySpeed * Time.deltaTime);

        if(transform.position.x <= minX)
        {
            dir = Vector3.right;
            FlipSprite();
        }
        else if(transform.position.x >= maxX)
        {
            dir = Vector3.left;
            FlipSprite();
        }
    }

    void FlipSprite()
    {
        flipped = !flipped;
        spriteRenderer.flipX = flipped;
    }
}
