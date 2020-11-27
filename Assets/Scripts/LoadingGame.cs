using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingGame : MonoBehaviour
{
    public TextMeshProUGUI gameStateText;

    // Start is called before the first frame update
    void Start()
    {
        if (SessionData.Instance.isDemoOver == false)
            gameStateText.text = "Demo start";
        else
            gameStateText.text = "Demo over";

        StartCoroutine(FakeLoading());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator FakeLoading()
    {
        yield return new WaitForSeconds(2);

        if (SessionData.Instance.isDemoOver == false)
            SceneManager.LoadScene("MainScene");
        else
            SceneManager.LoadScene("MainMenu");
    }
}
