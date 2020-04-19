using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoxeGameController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI broCoachTxt;

    private string[] broCoachOptions =  {
        "LEFT BRO!",
        "Keep up Bro!",
        "RIGHT BRO!"
    };

    private int qKeyCount = 0;
    private int eKeyCount = 0;

    private int broCoachRandomOption;

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

        StartCoroutine(StartCoaching());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OnQKeyPressed();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            OnEKeyPressed();
        }
    }

    private IEnumerator StartCoaching()
    {
        yield return new WaitForSeconds(1f);

        broCoachTxt.gameObject.SetActive(true);

        UpdateBroCoachText(broCoachOptions[broCoachRandomOption]);
    }

    private void OnQKeyPressed()
    {
        Debug.Log("OnQKeyPressed");
        qKeyCount++;
    }

    private void OnEKeyPressed()
    {
        Debug.Log("OnEKeyPressed");
        eKeyCount++;
    }

    private void UpdateBroCoachText(string text)
    {
        broCoachTxt.text = text;
    }
}
