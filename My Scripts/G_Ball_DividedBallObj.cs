using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class G_Ball_DividedBallObj : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        Destroy(gameObject,3.25f);
    }
}
