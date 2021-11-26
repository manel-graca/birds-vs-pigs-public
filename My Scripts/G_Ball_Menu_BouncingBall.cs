using System.Collections;
using UnityEngine;

public class G_Ball_Menu_BouncingBall : MonoBehaviour
{
    [SerializeField] Rigidbody2D[] bouncingBallsLeft;
    [SerializeField] Rigidbody2D[] bouncingBallsRight;
    private void Start()
    {
        StartCoroutine(LeftStartBounceRoutine());
        StartCoroutine(RightStartBounceRoutine());
    }
    IEnumerator LeftStartBounceRoutine()
    {
        yield return new WaitForSeconds(1f);
        bouncingBallsLeft[0].velocity += Vector2.up * 20f;
        yield return new WaitForSeconds(1f);
        bouncingBallsLeft[1].velocity += Vector2.up * 20f;
        yield return new WaitForSeconds(1f);
        bouncingBallsLeft[2].velocity += Vector2.up * 20f;
    }
    IEnumerator RightStartBounceRoutine()
    {
        yield return new WaitForSeconds(1f);
        bouncingBallsRight[0].velocity += Vector2.up * 20f;
        yield return new WaitForSeconds(1f);
        bouncingBallsRight[1].velocity += Vector2.up * 20f;
        yield return new WaitForSeconds(1f);
        bouncingBallsRight[2].velocity += Vector2.up * 20f;
    }
}
