using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BallType{Default,Explosive,Magnet,Rocket, Bombardier, Eagle, Division}
[CreateAssetMenu(fileName = "Default Ball", menuName = "Ball Game/New Default Ball")]
public class G_Ball_BallScriptableObj : ScriptableObject
{
    public BallType ballType;
    
    public string ballName;
    public Sprite ballIcon;
    
    public Vector3 spriteSize;
    
    public Material specialActionMaterial;

    public void Magnetize(SpriteRenderer spriteRenderer, PointEffector2D effector, GameObject effect, float strength, float variation)
    {
        spriteRenderer.material = specialActionMaterial;
        effector.forceMagnitude = strength;
        effector.forceVariation = variation;
        effector.enabled = true;
        effect.SetActive(true);
    }

    public void Explode(SingleExplosion explosion)
    {
        explosion.Activate();
    }

    public void PitchNoseDown(Rigidbody2D rb, Vector2 noseDiveDirection, float noseDiveSpeed)
    {
        rb.MovePosition(rb.position + (noseDiveDirection * noseDiveSpeed) * Time.deltaTime);
    }

    public void Divide(Rigidbody2D rb, GameObject dividedBall, Transform topPos, Transform midPos, Transform botPos, float speed)
    {
        var parentVelocity = rb.velocity * 2f;
        var parentMass = rb.mass;
        var parentGravityScale = rb.gravityScale;
        
        var downDir = ((Vector2.down * 1.25f) + Vector2.right) * speed * Time.deltaTime;
        downDir.Normalize();
        
        List<GameObject> dividedBallsList = new List<GameObject>();
        
        var upBall = Instantiate(dividedBall, topPos.position, Quaternion.identity);
        var midBall = Instantiate(dividedBall, midPos.position, Quaternion.identity);
        var botBall = Instantiate(dividedBall, botPos.position, Quaternion.identity);
        
        dividedBallsList.Add(upBall);
        dividedBallsList.Add(midBall);
        dividedBallsList.Add(botBall);

        foreach (var dBall in dividedBallsList)
        {
            Rigidbody2D dBall_rb = dBall.GetComponent<Rigidbody2D>();
            
            dBall_rb.mass = parentMass;
            dBall_rb.gravityScale = parentGravityScale;
            dBall_rb.velocity = parentVelocity;
            dBall_rb.velocity *= downDir;
        }
    }

}
