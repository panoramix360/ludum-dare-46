using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxeBag : MonoBehaviour
{
    [SerializeField] private BoxeGameController boxeGameController;
    [SerializeField] private AudioSource punch1;
    [SerializeField] private AudioSource punch2;
    private Rigidbody2D rigidbody2d;

    private void Awake()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        rigidbody2d.AddForce(new Vector2(700f, 0));
        boxeGameController.OnHitBoxe();

        int randomNum = Random.Range(0, 2);

        if(randomNum == 0)
        {
            punch1.Play();
        }
        else
        {
            punch2.Play();
        }
    }
}
