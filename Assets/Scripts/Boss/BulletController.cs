using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float _deleteTime = 3.0f;

    void Start()
    {
        Destroy(gameObject, _deleteTime);        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
