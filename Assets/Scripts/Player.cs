using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float jumpSpeed = 10;
    [SerializeField] private float fallSpeed = 15;
    [SerializeField] private float fallDamageThreshold = -30f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask ground;
    [SerializeField] private AudioClip _deathAudio;
    [SerializeField] private GameObject _winnerText;
    
    private Rigidbody2D _rigidbody2D;
    private CircleCollider2D _fallDamageCollider;
    private SpriteRenderer _spriteRenderer;
    private Animator _animatorController;
    private AudioSource _audioSource;
    private bool _isDead;
    private bool _fallDamageIsActive = false;
    private static readonly int HasDied = Animator.StringToHash(HasDiedAnimation);
    private static readonly int IsRunning = Animator.StringToHash(IsRunningAnimation);
    private bool playedDeathSound = false;
    
    
    private const string IsRunningAnimation = "isRunning";
    private const string HasDiedAnimation = "hasDied";


    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _fallDamageCollider = GetComponent<CircleCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animatorController = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (Die())
        {
            StartCoroutine(SetNextLevel());
            return;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        Move();
        Jump();
        Fall();
    }

    private bool Die()
    {
        if (!_isDead) return false;
        _animatorController.Play("Death");

        if (playedDeathSound) return true;
        
        _audioSource.PlayOneShot(_deathAudio, 1f);
        playedDeathSound = true;
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

        if (isJumping && isGrounded)
        {
            _rigidbody2D.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
        }
    }

    private void Fall()
    {
        if (!IsFalling()) return;
        _rigidbody2D.AddForce(Vector2.down * fallSpeed);
        if (_rigidbody2D.velocity.y < fallDamageThreshold)
        {
            _fallDamageIsActive = true;
        }

        if (_rigidbody2D.velocity.y > fallDamageThreshold)
        {
            _fallDamageIsActive = false;
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
        CheckForFallDamage(col);
    }

    private void CheckForFallDamage(Collision2D col)
    {
        if (_fallDamageIsActive && _fallDamageCollider.IsTouchingLayers(ground))
        {
            _isDead = true;
        }
    }

    private static void InteractWithItem(Collision2D col)
    {
        var interactableItem = col.gameObject.GetComponent<IInteractableItem>();
        interactableItem?.Act();
    }

    private IEnumerator SetNextLevel()
    {
        yield return new WaitForSeconds(1f);
        
        _winnerText.SetActive(true);

        yield return new WaitForSeconds(2f);
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
