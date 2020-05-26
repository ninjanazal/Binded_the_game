using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaractherAnimation : MonoBehaviour
{
    public string Text;
    private string currentText = "";
    private float speed = 0.1f;
    void Start()
    {
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        for (int i = 0; i < Text.Length; i++)
            currentText = Text.Substring(0, i);
        this.GetComponent<Text>().text = currentText;
        yield return new WaitForSeconds(speed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
