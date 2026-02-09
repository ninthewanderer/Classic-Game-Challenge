using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Variables needed to track game state.
    private int _score;
    private int _lives;
    private int _time;
    
    // Variables used to store audio.
    private AudioSource audioSource;
    public AudioClip duckSqueak;

    // An array of Home prefabs which stores all the home GameObjects.
    private Home[] _homes;

    // The player.
    private Frogger _player;
    
    // The game over & next level screens.
    public GameObject gameOverScreen;
    public GameObject nextLevelScreen;
    
    // Text for UI
    public Text _scoreText;
    public Text _livesText;
    public Text _timeText;

    // As soon as the script loads, this method triggers.
    private void Awake()
    {
    // Finds all the Home prefabs & the player in the level scene.
        _homes = FindObjectsOfType<Home>();
        _player = FindObjectOfType<Frogger>();
        audioSource = GetComponent<AudioSource>();
    }
    
    // As soon as the game starts, this method triggers.
    private void Start()
    {
        // Starts a new game.
        NewGame();
    }
    
    private void SetScore(int score)
    {
        this._score = score;
        _scoreText.text = score.ToString();
    }

    private void SetLives(int lives)
    {
        this._lives = lives;
        _livesText.text = lives.ToString();
    }
    
    // The beginning of the game.
    private void NewGame()
    { 
        // Hides the game over menu/UI.
        gameOverScreen.SetActive(false);
        
        // Resets score and provides a fresh set of lives.
        SetScore(0);
        SetLives(3);
        
        // Starts a new level.
        NewLevel();
    }

    // Triggers after collecting all home objects (frogs or rubber ducks).
    private void NewLevel()
    {
        // Ensures all the homes are reset at the start of a new level.
        for (int i = 0; i < _homes.Length; i++)
        {
            _homes[i].enabled = false;
        }
        
        // Respawns the player.
        Respawn();
    }

    // Respawns the player.
    private void Respawn()
    {
        // Stops all current coroutines since this includes the timer.
        StopAllCoroutines();
        
        // Calls the Frogger.cs Respawn() method and then re-starts necessary coroutines.
        _player.Respawn();
        StartCoroutine(Timer(30)); 
    }
    
    // Ends the game.
    private void GameOver()
    {
        // Hides the player.
        _player.gameObject.SetActive(false);
        
        // Turns on the game over menu/UI.
        gameOverScreen.SetActive(true);
        
        // Stops all coroutines, including the game timer. 
        StopAllCoroutines();
        
        // Starts the PlayAgain() coroutine.
        StartCoroutine(PlayAgain());
    }

    private IEnumerator Timer(int duration)
    {
        // Sets the time to the duration provided by Respawn().
        _time = duration;
        _timeText.text = _time.ToString();

        // Counts down every second while the player is alive.
        while (_time > 0)
        {
            yield return new WaitForSeconds(1);
            _time--;
            _timeText.text = _time.ToString();
        }
        
        // If time runs out, the player dies.
        _player.Death();
    }
    
    // Checks if the player would like to play again.
    private IEnumerator PlayAgain()
    {
        // False by default.
        bool playAgain = false;

        // If the player presses the enter key, they will play again.
        while (!playAgain)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                playAgain = true;
                
                // Checks the current scene. If the player is on Level 2, they will restart at Level 1.
                if (SceneManager.GetActiveScene().buildIndex == 2)
                {
                    SceneManager.LoadScene(1);
                }
                else // If the player is already on Level 1, they will just restart.
                {
                    NewGame();
                }
            }
            yield return null;
        }
    }
    
    // Coroutine that switches to the next scene.
    private IEnumerator NewScene()
    {
        // Waits for the celebration animation to finish playing before continuing.
        yield return new WaitForSecondsRealtime(3);
        
        // If the player has cleared the level, they will either move onto the next level or see the win screen.
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            // Turns on the next level screen.
            nextLevelScreen.SetActive(true);
            
            // Wait for another few seconds before continuing to switch the scene.
            yield return new WaitForSecondsRealtime(3);
            SceneManager.LoadScene(2);
        }
        else
        {
            SceneManager.LoadScene(3);
        }

        // Ensures in case there are any issues that the coroutine ends.
        yield break;
    }

    // Is called by Home.cs to track when a home has been collected.
    public void HomeCollected()
    {
        // FIXME: fix this sprite if we add a celebration animation/sprite.
        _player.gameObject.SetActive(false);

        audioSource.PlayOneShot(duckSqueak); //plays squeak when duck is picked up

        // Calculates bonus points based on time remaining. For every second remaining, you get 20 points.
        int bonusPoints = _time * 20;
        
        // Player gets 50 points + however many bonusPoints after clearing a home.
        SetScore(_score + bonusPoints + 50);
        
        // If the level has been cleared after the home is collected, a new level will begin.
        if (Cleared())
        {
            // Player gets 1000 points after clearing a level.
            SetScore(_score + 1000);
            
            // NewScene() is called to switch scenes.
            StartCoroutine(NewScene());
        }
        else // Otherwise, a new round begins and the player is respawned normally.
        {
            Invoke(nameof(Respawn), 1f);
        }
    }

    // Whenever the player moves to a new farthest row, they will gain 10 points.
    public void AdvancedRow()
    {
        SetScore(_score + 10);
    }
    
    // Removes a life, respawns player, determines if game continues or ends.
    public void Died()
    {
        // Takes 1 life away from the player.
        SetLives(_lives - 1);

        // If the player has lives left, they can respawn. Otherwise, the game will end.
        if (_lives > 0)
        {
            Invoke(nameof(Respawn), 1f);
        }
        else
        {
            Invoke(nameof(GameOver), 1f);
        }
    }
    
    // Checks if the level has been cleared.
    private bool Cleared()
    {
        // Checks the homes array to see if all the homes have been enabled. If not, level has not been cleared.
        for (int i = 0; i < _homes.Length; i++)
        {
            if (!_homes[i].enabled)
            {
                return false;
            }
        }

        // If all the homes have been enabled, the level has been cleared.
        return true;
    }
}
