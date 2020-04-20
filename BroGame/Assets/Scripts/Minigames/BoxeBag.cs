using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxeBag : MonoBehaviour
{
    [SerializeField] private BoxeGameController boxeGameController;
    private Rigidbody2D rigidbody2d;

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        rigidbody2d.AddForce(new Vector2(700f, 0));
        boxeGameController.OnHitBoxe();
    }
}
