using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    GameObject wolfPrefab;

    Vector3[] startPositionOptions =
    {
        new Vector3(925.0f, 20.0f, 925.0f),
        new Vector3(225.0f, 20.0f, 225.0f),
        new Vector3(225.0f, 20.0f, 900.0f),
        new Vector3(780.0f, 20.0f, 449.0f)
    };

    const int targetNumberOfWolves = 10;

    bool checkScene = true;
    

    // Start is called before the first frame update
    void Start()
    {
        wolfPrefab = Resources.Load<GameObject>("Wolf");
    }

    // Update is called once per frame
    void Update()
    {
        if (checkScene)
        {
            for (int i = GameObject.FindGameObjectsWithTag("Wolf").Length; i < targetNumberOfWolves; i++)
            {
                Vector3 randOffset = new(Random.Range(-10.0f, 10.0f), 0.0f, Random.Range(-10.0f, 10.0f));
                Instantiate(wolfPrefab, startPositionOptions[Random.Range(0, startPositionOptions.Length)] + randOffset, Quaternion.identity);
            }

            StartCoroutine(WaitCoroutine());
        }
    }

    IEnumerator WaitCoroutine()
    {
        checkScene = false;

        yield return new WaitForSeconds(5.0f);

        checkScene = true;
    }
}
