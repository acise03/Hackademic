using UnityEngine;

public class BoyCharacter : MonoBehaviour
{
    public GameObject paperPrefab;
    public float paperThrownSpeed = 10f;

    public Sprite[] frames;
    public float framesPerSecond = 20f;
    public float moveSpeed = 10f;

    private SpriteRenderer spriteRenderer;
    private int currentFrame;
    private float timer;

    private Vector2 lastMoveDirection = Vector2.right;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (frames.Length > 0)
            spriteRenderer.sprite = frames[0];
    }

    void Update()
    {
        HandleMovement();
        HandleAnimation();

        if (Input.GetKeyDown(KeyCode.Space) && Points.rating > 0)
        {
            ShootPaper();
            Points.rating--;
        }
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(moveX, moveY).normalized;
        if (move != Vector2.zero)
        {
            lastMoveDirection = move;
        }

        transform.Translate(move * moveSpeed * Time.deltaTime);

        if (moveX != 0)
        {
            spriteRenderer.flipX = moveX < 0;
        }
    }

    void HandleAnimation()
    {
        if (frames.Length == 0) return;

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            timer += Time.deltaTime;
            if (timer >= 1f / framesPerSecond)
            {
                currentFrame = (currentFrame + 1) % frames.Length;
                spriteRenderer.sprite = frames[currentFrame];
                timer = 0f;
            }
        }
        else
        {
            currentFrame = 0;
            spriteRenderer.sprite = frames[currentFrame];
        }
    }

    void ShootPaper()
{
    Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mouseWorldPos.z = 0f; 
    Vector2 direction = (mouseWorldPos - transform.position).normalized;
    GameObject paper = Instantiate(paperPrefab, transform.position, Quaternion.identity);
    Rigidbody2D rb = paper.GetComponent<Rigidbody2D>();
    rb.velocity = direction * paperThrownSpeed;
}

}
