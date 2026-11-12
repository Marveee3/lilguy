using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PlayerSc : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]private bool isRight;
    [SerializeField]private bool isTop;
    [SerializeField]private bool isMove;
    [Header("CameraShake")]
    [SerializeField]private cameraShake cameraShake;
    [SerializeField]private float duration;
    [SerializeField]private float magnitude;
    [Header("LevlManager")]
    [SerializeField]private levlManager levlManager;
    [Header("Audio")]
    [SerializeField]private AudioSource step;
    [SerializeField]private AudioSource up;
    [SerializeField]private AudioSource win;

    public static bool IsTop { get; private set; }
    public static event System.Action<bool> GravityChanged;

    int solidMask;
    int flagMask;
    bool isFinished;

    void Awake()
    {
        IsTop = isTop;
        solidMask = LayerMask.GetMask("Ground", "Box");
        flagMask = LayerMask.GetMask("Flag");
    }

    void Start()
    {
        cameraShake = FindObjectOfType<Camera>().GetComponent<cameraShake>();
        levlManager = FindObjectOfType<levlManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && isMove)
        {
            Vector2 dir = isRight ? Vector2.left : Vector2.right;

            if (WallHit(dir))
            {
                cameraShake.StartShake(duration, magnitude);
                step.Play();
                transform.Translate(dir.x, 0f, 0f);
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            cameraShake.StartShake(duration, magnitude + 0.1f);
            up.Play();
            isTop = !isTop;
            IsTop = isTop;
            GravityChanged?.Invoke(isTop);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            levlManager.LvlMng(1);
        }
    }

    void FixedUpdate()
    {
        CheckFlag();
        gravityOff();

        Vector2 newScale = transform.localScale;
        if (isTop)
        {
            newScale.y = -1;
        }
        else
        {
            newScale.y = 1;
        }
        transform.localScale = newScale;
    }

    bool WallHit(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1f, solidMask);
        if (hit.collider != null)
        {
            Box box = hit.collider.GetComponent<Box>();
            if (box != null && box.TryPush(Vector2Int.RoundToInt(dir)))
                return true;

            Flip();
            return false;
        }
        return true;
    }

    void CheckFlag()
    {
        if (isFinished) return;

        Vector2 dir = isRight ? Vector2.left : Vector2.right;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1f, flagMask);
        if (hit.collider != null && hit.distance <= 0.1f)
        {
            isFinished = true;
            win.Play();
            levlManager.LvlMng(0);
        }
    }

    void gravityOff()
    {
        Vector2 dir = isTop ? Vector2.up : Vector2.down;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1f, solidMask);
        bool grounded = hit.collider != null && hit.distance < 1f;

        isMove = grounded;
        if (!grounded)
        {
            transform.Translate(0f, isTop ? 0.5f : -0.5f, 0f);
        }

        RaycastHit2D flagHit = Physics2D.Raycast(transform.position, dir, 1f, flagMask);
        if (flagHit.collider != null && flagHit.distance <= 1f)
        {
            Vector3 newPosition = transform.localPosition;
            newPosition.y += (isTop ? 1 : -1);
            transform.localPosition = newPosition;
        }
    }

    void Flip()
    {
        Vector2 newScale = transform.localScale;
        newScale.x = -newScale.x;
        transform.localScale = newScale;

        isRight = !isRight;
    }
}
