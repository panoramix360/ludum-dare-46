using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMinigameEat : MonoBehaviour
{
    [SerializeField] private int speed;

    private Rigidbody2D rigidbody2d;

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 move = new Vector2(Input.GetAxis("Horizontal") * speed * Time.deltaTime, 0f);
        rigidbody2d.velocity = move;
    }
}
