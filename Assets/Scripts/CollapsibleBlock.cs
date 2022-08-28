using System.Collections;
using UnityEngine;

public class CollapsibleBlock : MonoBehaviour, IInteractableItem
{
    [SerializeField] private float timeBeforeFall = 1;
    
    private Rigidbody2D _rigidbody2D;
    private BoxCollider2D _boxCollider2D;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }
    
    public void Act()
    {
        StartCoroutine(FallAfterDelay(timeBeforeFall));
    }

    private IEnumerator FallAfterDelay(float time)
    {
        yield return new WaitForSeconds(time);
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
    }
}
