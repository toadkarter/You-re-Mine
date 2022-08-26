using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float jumpSpeed = 10;
    [SerializeField] private float fallSpeed = 5;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask ground;
    
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private Animator _animatorController;
    private bool _isDead;
    private bool fallDamageIsActive;
    private static readonly int HasDied = Animator.StringToHash(HasDiedAnimation);
    private static readonly int IsRunning = Animator.StringToHash(IsRunningAnimation);

    private const string IsRunningAnimation = "isRunning";
    private const string HasDiedAnimation = "hasDied";


    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animatorController = GetComponent<Animator>();
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (Die()) return;
        
        Move();
        Jump();
        Fall();
    }

    private bool Die()
    {
        if (!_isDead) return false;
        _animatorController.SetTrigger(HasDied);
        return true;
    }

    private void Move()
    {
        float currentDirection = Input.GetAxis("Horizontal") * moveSpeed;
        _rigidbody2D.velocity = new Vector2(currentDirection, _rigidbody2D.velocity.y);
        _spriteRenderer.flipX = currentDirection < 0;
        PlayRunningAnimation(currentDirection);
    }

    private void PlayRunningAnimation(float currentDirection)
    {
        if (currentDirection == 0)
        {
            _animatorController.ResetTrigger(IsRunning);
        }
        else
        {
            _animatorController.SetTrigger(IsRunning);
        }
    }

    private void Jump()
    {
        bool isJumping = Input.GetButtonDown("Jump");
        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, ground);

        Debug.Log("isJumping " + isJumping);
        Debug.Log("isGrounded " + isGrounded);
        
        if (isJumping && isGrounded)
        {
            Debug.Log("We are jumping");
            _rigidbody2D.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
        };
    }

    private void Fall()
    {
        if (!IsFalling()) return;
        
        _rigidbody2D.AddForce(Vector2.down * fallSpeed);
        if (_rigidbody2D.velocity.y < -30.0)
        {
            fallDamageIsActive = true;
        }
    }

    private bool IsFalling()
    {
        return _rigidbody2D.velocity.y < 0;
    }

    // Added new thing
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Obstacle"))
        {
            _isDead = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        InteractWithItem(col);
    }
    
    private static void InteractWithItem(Collision2D col)
    {
        var interactableItem = col.gameObject.GetComponent<IInteractableItem>();
        interactableItem?.Act();
    }
}
