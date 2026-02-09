using System.Collections;
using UnityEngine;

public class ExitGameCheck : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ExitGame());
    }

    // Checks if the player is trying to exit the game (for debug purposes/assignment requirement).
    private IEnumerator ExitGame()
    {
        bool exitGame = false;

        while (!exitGame)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                exitGame = true;
                Application.Quit();
            }
            
            yield return null;
        }
    }
}
