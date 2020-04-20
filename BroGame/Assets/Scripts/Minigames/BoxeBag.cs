using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxeBag : MonoBehaviour
{
    private Rigidbody2D rigidbody2d;


    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        rigidbody2d.AddForce(new Vector2(300f, 0));
    }
}
