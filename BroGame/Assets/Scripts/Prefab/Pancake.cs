using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pancake : MonoBehaviour
{
    [SerializeField] private PancakeType type;

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    public PancakeType GetPancakeType()
    {
        return type;
    }
}
