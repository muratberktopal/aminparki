using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform Spawnpoint;
    public GameObject EnemyPrefab;
    public float spawnrate = 2.0f;
    float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;


        if (timer <= 0)
        {
            Instantiate(EnemyPrefab, transform.position, Quaternion.identity);
                timer = spawnrate;
        }
    }
}
