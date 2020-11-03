using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerScript : MonoBehaviour
{
    // Start is called before the first frame update
    private float spawnTime = Mathf.Infinity;
    void Start()
    {
        spawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > spawnTime + 0.1f)
        {
            Destroy(gameObject);
        }
    }
}
