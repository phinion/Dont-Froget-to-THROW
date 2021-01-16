using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public enum GameState
    {
        PlayerTurn,
        EnemyTurn,
        CalculatingShot,
        Win,
        Lose
    }

    public GameState gameState;
    //public List<GameObject> PlayerCharacters;
    //public List<GameObject> EnemyCharacters;
    //public int playerCharacterTurnCount = 0;
    //public int enemyCharacterTurnCount = 0;

    public GameObject NextTurnScreen;

    public Player player;
    public Enemy enemy;

    public CameraScript cameraScript;

    // Use this for initialization
    void Start()
    {
        gameState = GameState.PlayerTurn;
        PlayerTurnStart();
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator NextTurnTransitionScreen()
    {

        //show transition screen canvas
        NextTurnScreen.SetActive(true);
        cameraScript.ClearFocusObject();
        //cameraScript.FocusFull();

        StartCoroutine(cameraScript.ZoomOut(2));
        yield return new WaitForSeconds(2);

        //hide canvas
        NextTurnScreen.SetActive(false);

        if (gameState == GameState.PlayerTurn)
        {
            PlayerTurnStart();
        }
        else
        {
            EnemyTurnStart();
        }
    }

    public void PlayerTurnStart()
    {
        Debug.Log("Player Turn");
        gameState = GameState.PlayerTurn;
        player.isPlayerTurn = true;
        player.changeState(1);
        player.currentCharacter = player.PlayerCharacters[player.currentCharacterInt].GetComponent<Character>();
        player.power.value = player.currentCharacter.maxEnergy;

        cameraScript.SetFocusObject(player.currentCharacter.gameObject);
    }

    public void EnemyTurnStart()
    {
        Debug.Log("Enemy Turn");
        player.isPlayerTurn = false;
        player.changeState(2);
        gameState = GameState.EnemyTurn;
        enemy.currentCharacter = enemy.EnemyCharacters[enemy.currentCharacterInt].GetComponent<Character>();
        cameraScript.SetFocusObject(enemy.currentCharacter.gameObject);
        enemy.EnemyTurn();
    }

    public void EndTurn()
    {

        Debug.Log("Gamemanager End Turn");

        if (gameState == GameState.PlayerTurn)
        {
            player.changeState(2);
            player.currentCharacter.currentEnergy = player.currentCharacter.maxEnergy;
            player.currentCharacterInt++;
            checkOverflow(ref player.PlayerCharacters, ref player.currentCharacterInt);
            gameState = GameState.EnemyTurn;
        }
        else
        {
            enemy.currentCharacter.currentEnergy = enemy.currentCharacter.maxEnergy;
            enemy.currentCharacterInt++;
            checkOverflow(ref enemy.EnemyCharacters, ref enemy.currentCharacterInt);
            gameState = GameState.PlayerTurn;
        }

        if (!CheckIfEnd())
        {
            StartCoroutine(NextTurnTransitionScreen());
        }

    }

    void checkOverflow(ref List<GameObject> characters, ref int characterCount)
    {
        if (characterCount > characters.Count - 1)
        {
            characterCount = 0;
        }
    }

    bool CheckIfEnd()
    {
        //Debug.Log("Players " + PlayerCharacters.Count);
        //Debug.Log("Enemys " + EnemyCharacters.Count);
        player.removeDeadCharactersInList();
        enemy.removeDeadCharactersInList();

        if (player.PlayerCharacters.Count == 0 || enemy.EnemyCharacters.Count == 0)
        {
            Debug.Log("Game end");
            cameraScript.ClearFocusObject();
            //cameraScript.FocusFull();

            StartCoroutine(cameraScript.ZoomOut(2));
            return true;
        }
        Debug.Log("Characters still alive. Switching to next turn");
        return false;
    }
}
