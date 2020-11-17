using Assets.Scripts;
using TMPro;
using UnityEngine;

public class Environment : MonoBehaviour
{
    public Coin coinPrefab;
    public Obstacle obstaclePrefab;
    public Player playerPrefab;
    public float maxTime = 2f;
    public float minTime = 1f;

    private SpawnLine spawnLine;
    private Player player;
    private TextMeshPro scoreBoard;
    private GameObject coins;
    private GameObject obstacles;
    private Vector3 spawnLineLocation;
    private GameObject floor;
    private Quaternion spawnLineRotation;

    //current time
    private float currentTimer;

    //The time to spawn the object
    private float randomTimed;

    // Start is called before the first frame update
    private void Start()
    {
        currentTimer = 0f;
        SetRandomTimed();
    }

    private void SetRandomTimed()
    {
        randomTimed = Random.Range(minTime, maxTime);
    }

    public void OnEnable()
    {
        coins = transform.Find("Coins").gameObject;
        obstacles = transform.Find("Obstacles").gameObject;
        scoreBoard = transform.GetComponentInChildren<TextMeshPro>();
        player = transform.GetComponentInChildren<Player>();
        spawnLine = transform.GetComponentInChildren<SpawnLine>();
        floor = transform.Find("Floor").gameObject;

        spawnLineLocation = spawnLine.transform.position;
        spawnLineRotation = spawnLine.transform.rotation;
    }

    public void ClearEnvironment()
    {
        foreach (Transform coin in coins.transform)
        {
            Destroy(coin.gameObject);
        }

        foreach (Transform obstacle in obstacles.transform)
        {
            Destroy(obstacle.gameObject);
        }
    }

    public void SpawnPlayer()
    {
        Player newPlayer = Instantiate(playerPrefab, new Vector3(0, 1.5f, 42.2f), new Quaternion(0f, 180f, 0f, 0f));
        newPlayer.transform.SetParent(gameObject.transform);
    }

    private void SpawnObstacle()
    {
        ResetTimer();
        Obstacle newObstacle = Instantiate(obstaclePrefab, new Vector3(spawnLineLocation.x, floor.transform.position.y + 0.5f, spawnLineLocation.z), spawnLineRotation);
        newObstacle.transform.SetParent(obstacles.transform);
    }

    private void ResetTimer()
    {
        currentTimer = 0;
    }

    private void SpawnCoin()
    {
        ResetTimer();
        Coin newCoin = Instantiate(coinPrefab, new Vector3(spawnLineLocation.x, floor.transform.position.y + 3f, spawnLineLocation.z), spawnLineRotation);
        newCoin.transform.SetParent(coins.transform);
    }

    private void FixedUpdate()
    {
        //Counts up
        currentTimer += Time.deltaTime;
        int randomValue = Random.Range(1, 3);

        //Check if its the right time to spawn the object
        if (CanSpawn())
        {
            switch (randomValue)
            {
                case 1:
                    SpawnCoin();
                    break;
                case 2:
                    SpawnObstacle();
                    break;
                default:
                    break;
            }

            RestartCurrentTimer();
        }

        scoreBoard.text = player.GetCumulativeReward().ToString("f3");
    }

    private bool CanSpawn()
    {
        return randomTimed <= currentTimer;
    }

    private void RestartCurrentTimer()
    {
        currentTimer = 0f;
    }
}
