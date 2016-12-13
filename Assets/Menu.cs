using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    int index = 0;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {

        var cursor = GameObject.FindGameObjectWithTag("Cursor");
        if (Input.GetKeyDown(KeyCode.UpArrow) && index > 0)
        {
            cursor.transform.localPosition = cursor.transform.localPosition + new Vector3(0, 30, 0);
            index--;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && index < 3)
        {
            cursor.transform.localPosition = cursor.transform.localPosition + new Vector3(0, -30, 0);
            index++;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (index == 3)
            {
                Application.Quit();
            } else
            {
                Global.playerCount = index + 2;
                SceneManager.LoadScene(1, LoadSceneMode.Single);
            }
        }
    }
}
