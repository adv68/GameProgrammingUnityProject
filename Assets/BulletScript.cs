using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    Rigidbody rigidBody;
    float lifeTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.AddForce(transform.up * 62.5f, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime += Time.deltaTime;

        if (lifeTime > 2.0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            Destroy(gameObject);
        }
    }
}
