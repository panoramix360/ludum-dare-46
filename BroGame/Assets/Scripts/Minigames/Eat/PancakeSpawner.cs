using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PancakeSpawner : MonoBehaviour
{
  [SerializeField] private GameObject pancakePrefab;
  [SerializeField] private GameObject pancakeGroup;
  [SerializeField] private Text levelTxt;
  private Animator levelTxtAnimator;

  [SerializeField] private int pancakeInitial;
  [SerializeField] private float pancakeMultiplier;
  [SerializeField] private int levels;

  [SerializeField] private int pancakeToSpawn;

  [SerializeField] private GameObject player;

  [SerializeField] private int score;

  private Camera cam;

  private void Awake()
  {
    cam = Camera.main;

    levelTxtAnimator = levelTxt.GetComponent<Animator>();
  }

  private void Start()
  {
    pancakeToSpawn = pancakeInitial;

    StartCoroutine(BeginSpawningPancakes());
  }

  private IEnumerator BeginSpawningPancakes()
  {
    levelTxtAnimator.SetBool("isAnimating", true);

    int currentLevel = 1;

    while (currentLevel <= levels)
    {
      yield return new WaitForSeconds(1f);

      levelTxtAnimator.SetBool("isAnimating", false);

      for (int i = 0; i < pancakeToSpawn; i++)
      {
        float randomX = Random.Range(cam.ScreenToWorldPoint(new Vector2(0, 0)).x, cam.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);

        Vector2 randomPosition = new Vector2(randomX, cam.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);

        Instantiate(pancakePrefab, randomPosition, Quaternion.identity, pancakeGroup.transform);

        yield return new WaitForSeconds(.5f);
      }

      yield return new WaitForSeconds(2f);

      Eat();

      yield return new WaitForSeconds(3f);

      player.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("player");
      player.GetComponent<BoxCollider2D>().enabled = true;

      if (currentLevel == levels) break;

      currentLevel++;

      levelTxt.text = "Level " + currentLevel + ", Bro!";
      levelTxtAnimator.SetBool("isAnimating", true);

      pancakeToSpawn = (int)(pancakeToSpawn * pancakeMultiplier);
    }

    Debug.Log("finish game!");
    GameController.Instance.UpdateMinigameScore(Constants.EatActivity, score);
    GameController.Instance.NextActivity();
  }

  private void Eat()
  {
    player.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("player_eating");
    player.GetComponent<BoxCollider2D>().enabled = false;

    int pancakesCount = pancakeGroup.gameObject.transform.childCount;

    score += pancakesCount;

    Debug.Log(pancakesCount);

    foreach (Transform children in pancakeGroup.gameObject.transform)
    {
      int randomX = Random.Range(-150, 150);
      children.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("pancake_eated");
      children.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(randomX, 60f), ForceMode2D.Impulse);
    }
  }
}
