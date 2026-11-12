using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Box : MonoBehaviour
{
    [SerializeField] private bool canBePushed = true;
    [SerializeField] private bool followsGravity = true;

    static int blockingMask = -1;

    BoxCollider2D boxCollider;
    bool gravityUp;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        if (blockingMask == -1)
            blockingMask = LayerMask.GetMask("Ground", "Box", "Flag");
    }

    void OnEnable()
    {
        PlayerSc.GravityChanged += ApplyGravity;
        ApplyGravity(PlayerSc.IsTop);
    }

    void OnDisable()
    {
        PlayerSc.GravityChanged -= ApplyGravity;
    }

    void ApplyGravity(bool isTop)
    {
        gravityUp = isTop;
    }

    void FixedUpdate()
    {
        if (!followsGravity) return;

        Vector2Int dir = gravityUp ? Vector2Int.up : Vector2Int.down;
        if (IsFree(dir))
            Move(dir);
    }

    public bool TryPush(Vector2Int dir)
    {
        if (!canBePushed || !IsFree(dir)) return false;

        Move(dir);
        return true;
    }

    bool IsFree(Vector2Int dir)
    {
        Vector2 current = transform.position;
        Vector2 center = new Vector2(Snap(current.x), Snap(current.y)) + dir;

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, new Vector2(0.9f, 0.9f), 0f, blockingMask);
        foreach (Collider2D hit in hits)
        {
            if (hit != boxCollider) return false;
        }
        return true;
    }

    void Move(Vector2Int dir)
    {
        Vector3 position = transform.position + new Vector3(dir.x, dir.y, 0f);
        transform.position = new Vector3(Snap(position.x), Snap(position.y), position.z);
        Physics2D.SyncTransforms();
    }

    static float Snap(float value)
    {
        return Mathf.Round(value - 0.5f) + 0.5f;
    }
}
