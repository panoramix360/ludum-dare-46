using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PancakeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject pancakePrefab;
    [SerializeField] private GameObject superPancakePrefab;
    [SerializeField] private GameObject pancakeGroup;
    [SerializeField] private Text levelTxt;
    private Animator levelTxtAnimator;

    [SerializeField] private int pancakeInitial;
    [SerializeField] private float pancakeMultiplier;
    [SerializeField] private int levels;

    [SerializeField] private int pancakeToSpawn;

    [SerializeField] private GameObject player;

    [SerializeField] private Text pancakeScore;
    [SerializeField] private int score;

    private AudioSource audioSource;

    private Camera cam;

    private GameController GC { get { return GameController.Instance; } }

    private void Awake()
    {
        LoadPlayerSprite($"eating_pose_0_lvl_{GC.currentPlayerLevel - 1}");
        cam = Camera.main;
        levelTxtAnimator = levelTxt.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void BeginMinigame()
    {
        pancakeToSpawn = pancakeInitial;

        StartCoroutine(BeginSpawningPancakes());
    }

    private IEnumerator BeginSpawningPancakes()
    {
        levelTxtAnimator.SetTrigger("Play");

        int currentLevel = 1;

        while (currentLevel <= levels)
        {
            yield return new WaitForSeconds(1f);

            bool hasSuperPancake = Random.Range(0f, 1f) >= 0.8f;
            bool superPancakeSpawned = false;
            int indexToSpawnSuperPancake = Random.Range(1, pancakeToSpawn);

            for (int i = 0; i < pancakeToSpawn; i++)
            {
                float randomX = Random.Range(cam.ScreenToWorldPoint(new Vector2(0, 0)).x, cam.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);

                Vector2 randomPosition = new Vector2(randomX, cam.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);

                float random = Random.Range(0f, 1f);

                if (indexToSpawnSuperPancake == i)
                {
                    yield return SpawnSuperPancake(randomPosition);
                }
                else
                {
                    Instantiate(pancakePrefab, randomPosition, Quaternion.identity, pancakeGroup.transform);
                }

                yield return new WaitForSeconds(.5f);
            }

            yield return new WaitForSeconds(2f);

            Eat();

            yield return new WaitForSeconds(3f);

            LoadPlayerSprite($"eating_pose_0_lvl_{GC.currentPlayerLevel - 1}");
            player.GetComponent<BoxCollider2D>().enabled = true;

            if (currentLevel == levels) break;

            currentLevel++;

            levelTxt.text = "Level " + currentLevel + ", Bro!";
            levelTxtAnimator.SetTrigger("Play");

            pancakeToSpawn = (int)(pancakeToSpawn * pancakeMultiplier);
        }

        Debug.Log("finish game!");
        GameController.Instance.UpdateMinigameScore(Constants.EatActivity, score);
        GameController.Instance.NextActivity();
    }

    private void LoadPlayerSprite(string name)
    {
        player.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(name);
    }

    private void Eat()
    {
        audioSource.Play();

        LoadPlayerSprite($"eating_pose_1_lvl_{GC.currentPlayerLevel - 1}");
        player.GetComponent<BoxCollider2D>().enabled = false;

        int pancakesCount = pancakeGroup.gameObject.transform.childCount;

        score += pancakesCount;
        pancakeScore.text = "Score: " + score;

        Debug.Log(pancakesCount);

        foreach (Transform children in pancakeGroup.gameObject.transform)
        {
            int randomX = Random.Range(-150, 150);
            children.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("pancake_eated");
            children.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(randomX, 60f), ForceMode2D.Impulse);
        }
    }

    private IEnumerator SpawnSuperPancake(Vector2 randomPosition)
    {
        levelTxt.text = "Super Pancake, Bro!";
        levelTxtAnimator.SetTrigger("Play");
        yield return new WaitForSeconds(1f);
        Instantiate(superPancakePrefab, randomPosition, Quaternion.identity, pancakeGroup.transform);
    }
}
