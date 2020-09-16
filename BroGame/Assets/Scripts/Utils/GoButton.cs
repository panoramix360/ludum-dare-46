using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GoButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetBool("Flex", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool("Flex", false);
    }
}
