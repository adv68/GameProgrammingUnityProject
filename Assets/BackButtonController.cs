using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackButtonController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("StartScene");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
