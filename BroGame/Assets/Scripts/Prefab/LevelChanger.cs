using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelChanger : MonoBehaviour
{
    [SerializeField] private UnityEvent animationEnd;

    public void OnAnimationEnd()
    {
        animationEnd.Invoke();
    }
}
