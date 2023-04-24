using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DescriptionController : MonoBehaviour
{
    const string Text = "You've ran out of gas in the middle of nowhere. Follow the path to the cabin to hide away for the night. There's packs of wolves prowling about, so watch out and use your gun when necessary.";

    TMP_Text textBox;
    GameObject playButton;

    // Start is called before the first frame update
    void Start()
    {
        playButton = GameObject.Find("Button");
        playButton.SetActive(false);

        textBox = GameObject.Find("Description").GetComponent<TMP_Text>();
        StartCoroutine(PlayText());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator PlayText()
    {
        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < Text.Length; i++)
        {
            textBox.text = Text[0..(i + 1)];
            yield return new WaitForSeconds(0.05f);
        }

        playButton.SetActive(true);
    }
}