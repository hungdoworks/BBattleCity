using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [SerializeField] private Animator touchToPlayTextAnim = null;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(GameStart());
        }
    }

    IEnumerator GameStart()
    {
        touchToPlayTextAnim.speed = 3.0f;
        
        yield return new WaitForSeconds(2.0f);

        SceneManager.LoadScene("LoadingScene");
    }
}
