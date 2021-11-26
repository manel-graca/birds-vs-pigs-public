using System.Collections;
using Lean.Pool;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using Sirenix.OdinInspector;

public class G_Ball_BallController : MonoBehaviour
{
    public static G_Ball_BallController instance;
    
    [BoxGroup("Ball Related", CenterLabel = true)] [Title("Ball Behaviour")]
    [SerializeField] float minLaunchDistance;
    [BoxGroup("Ball Related")]
    [SerializeField] private float detachDelay;
    [Space(5)]
    [BoxGroup("Ball Related")] [Title("Spawner")]
    [SerializeField] Transform pivot;
    [BoxGroup("Ball Related")]
    [SerializeField] Transform ballSpawn;
    [BoxGroup("Ball Related")]
    [SerializeField] Transform leftLineSpawn;
    [BoxGroup("Ball Related")]
    [SerializeField] Transform rightLineSpawn;
    
    [Space(5)]
    
    [BoxGroup("Physics", CenterLabel = true)]
    [SerializeField] SpringJoint2D spring;
    
    [Space(5)]
    
    [BoxGroup("Flight Path", CenterLabel = true)]
    [SerializeField] Transform flightDotsParent;
    

    public Rigidbody2D ball;
    private GameObject oldBall;
    private Camera cam;
    private G_Ball_GameManager manager;
    private G_Ball_LevelManager levelManager;
    private LineRenderer leftLineRenderer;
    private LineRenderer rightLineRenderer;

    [SerializeField] private bool canLaunch = false;
    private bool isDragging = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else {
            Destroy(this);
        }
    }

    void Start()
    {
        levelManager = G_Ball_LevelManager.instance;
        manager = G_Ball_GameManager.instance;
        cam = Camera.main;

        leftLineRenderer = leftLineSpawn.GetComponent<LineRenderer>();
        rightLineRenderer = rightLineSpawn.GetComponent<LineRenderer>();

        StartCoroutine(StartLevelRoutine());
    }
    IEnumerator StartLevelRoutine()
    {
        yield return new WaitForSeconds(0.4f);
        SpawnBall();
    }
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }
    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }
    void Update()
    {
        if(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
        if(manager.isInputBlock) return;
        if (ball == null || pivot == null) return;
        if (Touch.activeTouches.Count > 0)
        {
            if (!HandleLaunchConditions() && !isDragging)
            {
                return;
            }
        }
        if (Touch.activeTouches.Count == 0)
        {
            if(manager.isInputBlock) return;
            
            if (isDragging && canLaunch) // WHEN IS DRAGGING AND LIFTS FINGER UP
            {
                isDragging = false;
                
                if (Vector2.Distance(ball.position, pivot.position) > minLaunchDistance)
                {
                    LaunchBall();
                    return;
                }
                Debug.Log("not draging but cant launch");

                ball.isKinematic = false;
                RepositionBall();
            }
            ball.isKinematic = false;
            return;
        }
        Vector2 vTouchPos = Touchscreen.current.primaryTouch.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(vTouchPos);
        RaycastHit vHit;
        if(Physics.Raycast(ray.origin,ray.direction, out vHit))
        {
            if (vHit.transform.CompareTag("InvisibleCollider"))
            {
                canLaunch = false;
                ball.isKinematic = true;
                isDragging = false;
                RepositionBall();
                Debug.Log("clicking confiner");
                return;
            }
        }
        Debug.Log("dragging");
        isDragging = true;
        ball.isKinematic = true;
        ball.position = GetTouchPos();
        HandleTensionLines();
    }

    public Vector2 GetPivotPosition()
    {
        return pivot.transform.position;
    }

    private Vector2 GetBallPivotVector()
    {
        Vector2 pivotPos = pivot.position;
        Vector2 ballPos = ball.position;
        return pivotPos - ballPos;
    }

    private Vector2 GetTouchPivotVector()
    {
        Vector2 touch = GetTouchPos();
        Vector2 pivotPos = pivot.position;
        return pivotPos - touch;
    }

    private Vector2 GetTouchPos()
    {
        if(manager.isInputBlock) return pivot.position;

        var touchPosition = new Vector2();
        foreach (Touch touch in Touch.activeTouches)
        {
            touchPosition += touch.screenPosition;
        }

        touchPosition /= Touch.activeTouches.Count;

        var worldPos = cam.ScreenToWorldPoint(touchPosition);
        return worldPos;
    }

    private void RepositionBall()
    {
        if (ball != null && spring.enabled)
        {
            leftLineRenderer.enabled = false;
            rightLineRenderer.enabled = false;
            ball.transform.LeanMove(pivot.position,0.15f);
        }
    }

    private bool HandleLaunchConditions()
    {
        if(manager.isInputBlock) return false;

        if (isDragging)
        {
            if (GetBallPivotVector().x < -0.1f)
            {
                canLaunch = false;
                ball.isKinematic = false;
                isDragging = false;
                RepositionBall();
                return false;
            }
        }

        if (GetTouchPivotVector().x < -0.1f)
        {
            canLaunch = false;
            return false;
        }
        
        canLaunch = true;
        return true;
    }

    private void HandleTensionLines()
    {
        if (ball != null && isDragging && canLaunch)
        {
            leftLineRenderer.enabled = true;
            rightLineRenderer.enabled = true;

            var ballScript = ball.GetComponent<G_Ball_DefaultBall>();

            leftLineRenderer.SetPosition(0, leftLineSpawn.position);
            leftLineRenderer.SetPosition(1, ballScript.GetLineGrabbers(true, false).position);

            rightLineRenderer.SetPosition(0, rightLineSpawn.position);
            rightLineRenderer.SetPosition(1, ballScript.GetLineGrabbers(false, true).position);
        }
        else
        {
            leftLineRenderer.enabled = false;
            rightLineRenderer.enabled = false;
        }
    }

    public void FlushFlightDots()
    {
        var dots = GameObject.FindGameObjectsWithTag("FlightDot");
        foreach (var dot in dots)
        {
            LeanPool.Despawn(dot);
        }
    }

    private void StartFlightTrail()
    {
        StartCoroutine(FlightTrailRoutine());
    }

    IEnumerator FlightTrailRoutine()
    {
        var ballScript = oldBall.GetComponent<G_Ball_DefaultBall>();
        yield return new WaitForSeconds(0.1f);
        while (ballScript.isInAir)
        {
            if(ballScript == null) yield break;
            var dot = LeanPool.Spawn(ballScript.dotPrefab, ballScript.transform.position, Quaternion.identity);
            dot.transform.localScale = Vector3.zero;
            dot.transform.LeanScale(new Vector2(ballScript.dotSize,ballScript.dotSize), 0.075f).delay = 0.01f;
            dot.transform.SetParent(flightDotsParent);
            yield return new WaitForSeconds(ballScript.timeBetweenDotsSpawned);
            yield return null;
        }
    }

    private void LaunchBall()
    {
        FlushFlightDots();

        leftLineRenderer.enabled = false;
        rightLineRenderer.enabled = false;
        
        oldBall = ball.gameObject;
        
        var ballScript = ball.GetComponent<G_Ball_DefaultBall>();
        ballScript.StartCoroutine(ballScript.OnLaunchRoutine(ball.position));
        
        ball = null;
        
        Invoke(nameof(DetachBall), detachDelay);
        Invoke(nameof(StartFlightTrail), detachDelay / 3);
    }

    private void DetachBall()
    {
        if (spring != null)
        {
            spring.enabled = false;
        }
        levelManager.CheckLevelState();
    }


    public void SpawnBall()
    {
        if (GameObject.FindGameObjectWithTag("Ball"))
        {
            return;
        }
        
        oldBall = null;
        
        GameObject newBall = levelManager.InstantiateBall(ballSpawn.position);
        newBall.name = newBall.GetComponent<G_Ball_DefaultBall>().ballType.ballName;
        spring = newBall.GetComponent<SpringJoint2D>();
        ball = newBall.GetComponent<Rigidbody2D>();
        newBall.GetComponent<G_Ball_DefaultBall>().isInAir = false;
        
        spring.enabled = true;
        canLaunch = false;

        spring.connectedBody = pivot.GetComponent<Rigidbody2D>();

        levelManager.OnBallSpawned();
    }
    
}