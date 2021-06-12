using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    const int CREDITS = 9;

    public GameObject startScreen;
    public GameObject gameOverScreen;
    public GameObject creditsScreen;
    public GameObject player_1;
    public GameObject player_2;

    public AudioSource startSound;
    public AudioSource levelWarpSound;
    public AudioSource winSound;
    public AudioSource deathSound;

    public GameState State {get;set;}
    public int LevelIndex {get; set;}

    private Fader fader;
    private GameOverText gametext;

    private GameObject Player_1_Start;
    private GameObject Player_1_End;
    private GameObject Player_2_Start;
    private GameObject Player_2_End;

    private PlayerController player1Controller;
    private PlayerController player2Controller;

    private bool player_1_win;
    private bool player_2_win;

    private void Awake() {
        this.LevelIndex = 1;
        fader = FindObjectOfType<Fader>();
        gametext = FindObjectOfType<GameOverText>();
    }

    void Start()
    {
        UpdateGameState(GameState.StartScreen);
        player_1_win = false;
        player_2_win = false;
        player1Controller = player_1.GetComponent<PlayerController>();
        player2Controller = player_2.GetComponent<PlayerController>();
    }

    void Update()
    {
       
        if(State == GameState.StartScreen){

            if(Input.GetKeyDown("return")){
                startSound.Play();
                StartCoroutine(SwitchScene(LevelIndex, GameState.Game));
            }

            if(Input.GetKeyDown("escape")){
                QuitGame();
            }
        }
        
        if(State == GameState.Game){
            
            if(CheckForWin()){
                UpdateGameState(GameState.Win);
                LevelIndex += 1;
                
                if(LevelIndex == CREDITS)
                    StartCoroutine(NextLevel(LevelIndex, GameState.Credits));
                else
                    StartCoroutine(NextLevel(LevelIndex, GameState.Game));
            }
                
            if(Input.GetKeyDown("escape")){
                QuitGame();
            }
        } 
        
        if(State == GameState.Game || State == GameState.Dead) {

            if(Input.GetKeyDown("r") && !player1Controller.isMoving && !player2Controller.isMoving ){
                Debug.Log("Game Reset");
                ResetLevel();
                StartCoroutine(gametext.FadeOut(0f));
            }

            if(Input.GetKeyDown("escape")){
                QuitGame();
            }
        }
            
    }

    public bool CheckForWin(){
        
        if((player_1.transform.position - Player_1_End.transform.position).sqrMagnitude < Mathf.Epsilon){
            player_1_win = true;
        } else {
            player_1_win = false;
        }

        if((player_2.transform.position - Player_2_End.transform.position).sqrMagnitude < Mathf.Epsilon){
            player_2_win = true;
        } else {
            player_2_win = false;
        }

        if(player_1_win && player_2_win){
            winSound.Play();
            return true;
        }
            

        return false;
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif

        #if UNITY_64
            Application.Quit();
        #endif
    }

    public void UpdateGameState(GameState state){
        
        ResetGame();
        if(state == GameState.StartScreen)
            startScreen.SetActive(true);

        else if(state == GameState.Dead){
            gameOverScreen.SetActive(true);
            StartCoroutine(gametext.FadeIn(2f));
        }

        else if(state == GameState.Credits)
            creditsScreen.SetActive(true);
            
        else if(state == GameState.Game || state == GameState.Win){
            player_1.SetActive(true);
            player_2.SetActive(true);
        }

        State = state;       
    }

    public void ResetGame(){
        startScreen.SetActive(false);
        gameOverScreen.SetActive(false);
    }

    public void ResetLevel(){
        player_1.GetComponent<PlayerController>().Reset();
        player_2.GetComponent<PlayerController>().Reset();
        player_1.transform.position = Player_1_Start.transform.position;
        player_2.transform.position = Player_2_Start.transform.position;
        UpdateGameState(GameState.Game);
    }

    IEnumerator SwitchScene(int scene, GameState gameState){
        yield return fader.FadeIn(1f);
        yield return SceneManager.LoadSceneAsync(scene);
        UpdateGameState(gameState);
        Player_1_Start = GameObject.Find("Player_1_Start");
        Player_1_End = GameObject.Find("Player_1_End");
        Player_2_Start = GameObject.Find("Player_2_Start");
        Player_2_End = GameObject.Find("Player_2_End");
        ResetLevel();
        yield return fader.FadeOut(1f);
    }

    IEnumerator NextLevel(int scene, GameState gameState){
        player_1_win = false;
        player_2_win = false;
        yield return new WaitForSeconds(1f);
        levelWarpSound.Play();
        yield return fader.FadeIn(1f);
        yield return SceneManager.LoadSceneAsync(scene);
        UpdateGameState(gameState);
        Player_1_Start = GameObject.Find("Player_1_Start");
        Player_1_End = GameObject.Find("Player_1_End");
        Player_2_Start = GameObject.Find("Player_2_Start");
        Player_2_End = GameObject.Find("Player_2_End");
        ResetLevel();
        yield return fader.FadeOut(1f);
    }
}

public enum GameState{
    StartScreen,
    Menu,
    Game,
    Win,
    Credits,
    Dead
}



