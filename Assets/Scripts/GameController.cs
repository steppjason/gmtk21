using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    const int CREDITS = 3;

    public GameObject startScreen;
    public GameObject gameOverScreen;
    public GameObject creditsScreen;
    public GameObject player_1;
    public GameObject player_2;

    public GameState State {get;set;}
    public int LevelIndex {get; set;}

    private Fader fader;
    private GameOverText gametext;

    private GameObject Player_1_Start;
    private GameObject Player_1_End;
    private GameObject Player_2_Start;
    private GameObject Player_2_End;

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
    }

    void Update()
    {
        Debug.Log(State);

        if(State == GameState.StartScreen){

            if(Input.GetKeyDown("return"))
                StartCoroutine(SwitchScene(LevelIndex, GameState.Game));
            
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
                

        } 
        
        if(State == GameState.Game || State == GameState.Dead) {

            if(Input.GetKeyDown("r")){
                Debug.Log("Game Reset");
                ResetLevel();
                StartCoroutine(gametext.FadeOut(0f));
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

        if(player_1_win && player_2_win)
            return true;

        return false;
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
        yield return new WaitForSeconds(3f);
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



