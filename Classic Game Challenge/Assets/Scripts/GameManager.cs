using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Variables needed to track game state.
    private int _score;
    private int _lives;
    private int _time;
    
    // An array of Home prefabs which stores all the home GameObjects.
    private Home[] _homes;

    // The player.
    private Frogger _player;
    
    // The game over screen.
    public GameObject gameOverScreen;

    public Text _scoreText;
    public Text _livesText;
    public Text _timeText;
    
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
    }
    
    // As soon as the game starts, this method triggers.
    private void Start()
    {
        // Starts a new game.
        NewGame();
        
        // Evokes the coroutine which will constantly check for the player pressing the ESC key.
        StartCoroutine(ExitGame());
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
        StartCoroutine(ExitGame());
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
        
        // Restarts the ExitGame coroutine since it needs to always run and then starts PlayAgain().
        StartCoroutine(ExitGame());
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
    
    // Checks if the player would like to play again.
    private IEnumerator PlayAgain()
    {
        // False by default.
        bool playAgain = false;

        // FIXME: currently, the player can play again if they press tab.
        while (!playAgain)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                playAgain = true;
                NewGame();
            }
            
            yield return null;
        }

        // If the player wants to play again, NewGame() is called & the player sprite is re-enabled.
        _player.gameObject.SetActive(true);
        NewGame();
    }

    // Is called by Home.cs to track when a home has been collected.
    public void HomeCollected()
    {
        // FIXME: fix this sprite if we add a celebration animation/sprite.
        _player.gameObject.SetActive(false);
        
        // Calculates bonus points based on time remaining. For every second remaining, you get 20 points.
        int bonusPoints = _time * 20;
        
        // Player gets 50 points + however many bonusPoints after clearing a home.
        SetScore(_score + bonusPoints + 50);
        
        // If the level has been cleared after the home is collected, a new level will begin.
        if (Cleared())
        {
            // Player gets 1000 points after clearing a level.
            SetScore(_score + 1000);
            
            // NewLevel() is called after a 1-second delay.
            Invoke(nameof(NewLevel), 1f);
        }
        else // Otherwise, a new round begins.
        {
            // NewRound() is called after a 1-second delay.
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
