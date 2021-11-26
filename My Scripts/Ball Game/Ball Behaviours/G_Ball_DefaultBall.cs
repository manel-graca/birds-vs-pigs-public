using System.Collections;
using MoreMountains.Feedbacks;
using UnityEngine;
using Unity.Mathematics;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using Sirenix.OdinInspector;

public class G_Ball_DefaultBall : MonoBehaviour
{
    [BoxGroup("Ball Data Object")]
    public G_Ball_BallScriptableObj ballType;

    [Space(10f)]
    [BoxGroup("Global Visual Elements", CenterLabel = true)]
    [SerializeField]
    private Transform leftLineGrab;
    [BoxGroup("Global Visual Elements")]
    [SerializeField]
    private Transform rightLineGrab;
    [Space(5f)]
    [BoxGroup("Global Visual Elements")][Title("Flight Trail Settings", TitleAlignment = TitleAlignments.Centered)]
    public float timeBetweenDotsSpawned;
    [BoxGroup("Global Visual Elements")]
    public float dotSize;
    [BoxGroup("Global Visual Elements")]
    public GameObject dotPrefab;
    
    [Space(10f)]
    [BoxGroup("Animations", CenterLabel = true)]
    [SerializeField] private AnimatorOverrideController animatorOverride;
    
    [Space(10f)]
    
    [BoxGroup("Global Shared Variables", CenterLabel = true)]
    [SerializeField] float maxSpeed;
    [BoxGroup("Global Shared Variables")]
    [SerializeField] private float noseDiveSpeed;
    [BoxGroup("Global Shared Variables")]
    [SerializeField] private Vector2 noseDiveDirection;

    [Space(10f)]
    [BoxGroup("Bombardier Only", CenterLabel = true)]
    [SerializeField] private GameObject bombardierMissile;
    [BoxGroup("Bombardier Only")]
    [SerializeField] private Transform bombardierMissileDropTransform;

    [Space(10f)]
    [BoxGroup("Magnet Only", CenterLabel = true)]
    [SerializeField] private PointEffector2D magnet;
    [BoxGroup("Magnet Only")]
    [SerializeField] private float magnetStrength;
    [BoxGroup("Magnet Only")]
    [SerializeField] private float magnetVariation;
    [BoxGroup("Magnet Only")]
    [SerializeField] private float magnetDuration;
    [BoxGroup("Magnet Only")]
    [SerializeField] private GameObject magneticFieldEffect;
    
    [Space(10f)]
    [BoxGroup("Division Only", CenterLabel = true)]
    [SerializeField] GameObject dividedBallPrefab;
    [BoxGroup("Division Only")]
    [SerializeField] Transform divideTopPos;
    [BoxGroup("Division Only")]
    [SerializeField] Transform divideMidPos;
    [BoxGroup("Division Only")]
    [SerializeField] Transform divideBotPos;
    [BoxGroup("Division Only")]
    [SerializeField] float divisionBallsSpeed;
    [BoxGroup("Division Only")]
    [SerializeField] bool isDividing;
    
    public bool isInAir;
    public bool canUseSpecialAction;
    
    private bool fxSpawned;

    private bool isExploding,
        isMagnetic,
        isNoseDiving,
        isBombarding;

    
    private static readonly int IsFlying = Animator.StringToHash("isFlying");
    private static readonly int IsDead = Animator.StringToHash("isDead");
    private static readonly int NoseDive = Animator.StringToHash("noseDive");
    
    Animator animator;
    Rigidbody2D rb;
    G_Ball_EffectsManager effects;
    G_Ball_GameManager manager;
    G_Ball_LevelManager levelManager;
    TrailRenderer trail;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        manager = G_Ball_GameManager.instance;
        levelManager = G_Ball_LevelManager.instance;
        effects = G_Ball_EffectsManager.instance;
        trail = GetComponentInChildren<TrailRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        canUseSpecialAction = false;
        isInAir = false;
        trail.enabled = false;
        HandleAnimatonLayers();
        
    }
    
    private void Update()
    {
        if(manager.isInputBlock) return;
        Debug.Log("is in air? " + isInAir);
        
        if (isInAir)
        {
            trail.enabled = true;
        }
        //TurnBlocksContinuousDetection();

        if (Touch.activeTouches.Count > 0 && isInAir && canUseSpecialAction)
        {
            ApplyPowerup();
        }
        

    }

    private void FixedUpdate()
    {
        if (isNoseDiving)
        {
            rb.mass = 18;
            ballType.PitchNoseDown(rb, noseDiveDirection, noseDiveSpeed);
            
            transform.LeanRotateZ(-47.67f, .5f).setEaseLinear();
        }
    }

    void ClampVelocity() 
    {
        var smoothness = 0.99f;
        var maxVelocity = 25f;
        
        if(rb.velocity.sqrMagnitude > maxVelocity)
        {
            rb.velocity *= smoothness;
        }
        // // clamps rb vel directly
        // // should use diff approach
        // // get direction vector and apply negative force
    }

    void HandleAnimatonLayers()
    {
        if (animatorOverride != null)
        {
            animator.runtimeAnimatorController = animatorOverride;
        }
    }


    public void ApplyPowerup()
    {
        switch (ballType.ballType)
        {
            case BallType.Eagle:
                NoseDivePowerup();
                break;
            case BallType.Explosive:
                Explode();
                break;
            case BallType.Rocket:
                Debug.Log("Rocket lacks powerup");
                break;
            case BallType.Bombardier:
                DropMissile();
                break;
            case BallType.Magnet:
                Magnetize();
                break;
            case BallType.Division:
                Divide();
                break;
        }
    }

    public Transform GetLineGrabbers(bool left, bool right)
    {
        if (left) return leftLineGrab;
        if (right) return rightLineGrab;
        return null;
    }

    #region SpecialActions

    private void Divide()
    {
        if(isDividing) return;
        StartCoroutine(Special_DivideRoutine());
    }
    
    private void DropMissile()
    {
        if (isBombarding) return;
        StartCoroutine(Special_BombardierRoutine());
    }

    private void NoseDivePowerup()
    {
        if (isNoseDiving) return;
        animator.SetTrigger(NoseDive);
        StartCoroutine(Special_NoseDiveRoutine());
    }

    private void Magnetize()
    {
        if (isMagnetic) return;
        StartCoroutine(Special_MagnetRoutine());
    }

    private void Explode()
    {
        if (isExploding) return;
        StartCoroutine(Special_ExplodeRoutine());
    }

    #endregion

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Boundaries"))
        {
            DestroySelf(0.3f);
            return;
        }
        if (!isInAir) return;
        levelManager.CheckLevelState();
        var blocks = FindObjectsOfType<G_Ball_Block>();
        foreach (var block in blocks)
        {
            block.SetKinematic(false);
            block.SetBlocksDetection(CollisionDetectionMode2D.Continuous);
        }
        
        isNoseDiving = false;
        isExploding = false;
        isMagnetic = false;
        isBombarding = false;
        isInAir = false;

        animator.SetBool(IsFlying, false);
        animator.SetBool(IsDead, true);

        if (!fxSpawned)
        {
            effects.OnBallCollisionEffect(true, transform.position);
            fxSpawned = true;
        }
        Destroy(gameObject,4.5f);
    }

    private void DestroySelf(float timer)
    {
        spriteRenderer.enabled = false;
        effects.OnBallDestroyedEffect(transform.position);
        levelManager.CheckLevelState();
        levelManager.CheckIfLostGame();
        Destroy(gameObject,timer);
    }


    IEnumerator Special_DivideRoutine()
    {
        isDividing = true;
        
        ballType.Divide(rb,dividedBallPrefab, divideTopPos, divideMidPos, divideBotPos, divisionBallsSpeed);
        effects.OnBallsDivided(rb.position);
        yield return null;
        spriteRenderer.enabled = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
        Destroy(GetComponent<SpringJoint2D>());
        Destroy(rb);
        enabled = false;
    }

    IEnumerator Special_ExplodeRoutine()
    {
        isExploding = true;

        var explosion = GetComponent<SingleExplosion>();
        ballType.Explode(explosion);
        effects.OnExplosiveBallExplosion(transform.position);

        yield return new WaitForSeconds(4f);

        isExploding = false;
    }

    IEnumerator Special_NoseDiveRoutine()
    {
        isNoseDiving = true;

        yield return new WaitForSeconds(2f);

        isNoseDiving = false;
    }

    IEnumerator Special_MagnetRoutine()
    {
        isMagnetic = true;

        ballType.Magnetize(spriteRenderer, magnet,magneticFieldEffect, -magnetStrength, magnetVariation);
        rb.mass = 60;
        yield return new WaitForSeconds(magnetDuration);
        GetComponent<PointEffector2D>().enabled = false;
        magneticFieldEffect.SetActive(false);
        isMagnetic = false;
    }

    IEnumerator Special_BombardierRoutine()
    {
        isBombarding = true;

        var missile = Instantiate(bombardierMissile, bombardierMissileDropTransform.position, quaternion.identity);
        missile.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, -50f);

        rb.AddForce(Vector2.up, ForceMode2D.Impulse);

        yield return new WaitForSeconds(4f);

        isBombarding = false;
    }

    public IEnumerator OnLaunchRoutine(Vector2 launchPos)
    {
        animator.SetBool(IsFlying, true);
        isInAir = true;
        rb.isKinematic = false;
        
        while (Vector2.Distance(transform.position, launchPos) < 8f)
        {
            yield return null;
        }
        canUseSpecialAction = true;
    }

}