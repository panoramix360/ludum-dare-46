using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoxeGameController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI broCoachTxt;
    [SerializeField] private GameObject boxeBag;
    [SerializeField] private Animator rightArmAnim;
    [SerializeField] private Animator leftArmAnim;

    [SerializeField] private int startLimitToChangeOption = 10;
    [SerializeField] private int endLimitToChangeOption = 30;

    [Header("Don't change this fields!")]
    [SerializeField] private int randomLimitToChangeOption;

    [SerializeField] private int qKeyCount = 0;
    [SerializeField] private int eKeyCount = 0;

    [SerializeField] private int broCoachRandomOption;

    [SerializeField] private State currentState;

    private float strenghPush = 100f;

    private string[] broCoachOptions =  {
        "LEFT BRO!",
        "Keep up Bro!",
        "RIGHT BRO!"
    };

    private void Awake()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
    }

    private void Start()
    {
        do
        {
            broCoachRandomOption = Random.Range(0, broCoachOptions.Length);
        } while (broCoachRandomOption == 1);

        SetStateBasedOnCoachOption(broCoachRandomOption);

        StartCoroutine(StartCoaching());
    }

    private void Update()
    {
        bool qKeyPressed = Input.GetKeyDown(KeyCode.Q);
        bool eKeyPressed = Input.GetKeyDown(KeyCode.E);

        if (qKeyPressed || eKeyPressed)
        {
            if (qKeyPressed)
            {
                OnQKeyPressed();
                rightArmAnim.SetBool("isMoving", true);
                boxeBag.GetComponent<Rigidbody2D>().AddForce(new Vector2(strenghPush, 0));
            }

            if (eKeyPressed)
            {
                OnEKeyPressed();
                leftArmAnim.SetBool("isMoving", true);
                boxeBag.GetComponent<Rigidbody2D>().AddForce(new Vector2(strenghPush, 0));
            }
        }
        else
        {
            rightArmAnim.SetBool("isMoving", false);
            leftArmAnim.SetBool("isMoving", false);
        }
    }

    private IEnumerator StartCoaching()
    {
        yield return new WaitForSeconds(1f);

        broCoachTxt.gameObject.SetActive(true);

        UpdateBroCoachText(broCoachOptions[broCoachRandomOption]);

        GenerateLimitToChange();
    }

    private void SetStateBasedOnCoachOption(int broOption)
    {
        switch (broOption)
        {
            case 0:
                currentState = State.LEFT;
                break;
            case 2:
                currentState = State.RIGHT;
                break;
            default:
                break;
        }
    }

    private void OnQKeyPressed()
    {
        if (currentState == State.LEFT)
        {
            qKeyCount++;

            CheckIfLimitIsReached(qKeyCount);
        }
    }

    private void OnEKeyPressed()
    {
        if (currentState == State.RIGHT)
        {
            eKeyCount++;

            CheckIfLimitIsReached(eKeyCount);
        }
    }

    private void GenerateLimitToChange()
    {
        randomLimitToChangeOption = Random.Range(startLimitToChangeOption, endLimitToChangeOption);
    }

    private void CheckIfLimitIsReached(int keyCount)
    {
        if (keyCount >= randomLimitToChangeOption)
        {
            GenerateNextCoachOption();
        }
    }

    private void GenerateNextCoachOption()
    {
        int lastCoachOption = broCoachRandomOption;
        do
        {
            broCoachRandomOption = Random.Range(0, broCoachOptions.Length);
        } while (lastCoachOption == broCoachRandomOption);

        SetStateBasedOnCoachOption(broCoachRandomOption);

        UpdateBroCoachText(broCoachOptions[broCoachRandomOption]);

        GenerateLimitToChange();

        qKeyCount = 0;
        eKeyCount = 0;
    }

    private void UpdateBroCoachText(string text)
    {
        broCoachTxt.text = text;
    }

    private enum State
    {
        LEFT, RIGHT
    }
}