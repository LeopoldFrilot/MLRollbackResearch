using SharedGame;
using System;
using UnityEngine;

public class MLUGameManager : MonoBehaviour {
    [SerializeField] private GameObject characterPrefab;
    
    private MLUCharacter[] registeredCharacters = Array.Empty<MLUCharacter>();
    private GameManager GameManager => GameManager.Instance;

    private bool savedInputs;

    private void Update() {
        if (GameManager.IsRunning) {
            UpdateGame(GameManager.Runner);
        }
    }

    private void UpdateGame(IGameRunner runner) {
        MLGame gameState = runner.Game as MLGame;
        if (gameState == null) {
            return;
        }
        
        if (gameState.GameOver()) {
            if (!savedInputs) {
                MLGameManager GM = GameManager as MLGameManager;
                if (GM) {
                    GM.inputDataSO.AddInputData(gameState.playerInputs);
                }
                savedInputs = true;
            }
        }
        
        GameInfo info = runner.GameInfo;
        MLCharacter[] characters = gameState.characters;
        if (registeredCharacters.Length != characters.Length) {
            ResetGame(gameState, info);
        }
        
        for (int i = 0; i < registeredCharacters.Length; i++){
            registeredCharacters[i].UpdateCharacter(characters[i], info.players[i]);
        }
    }

    private void ResetGame(MLGame gameState, GameInfo info) {
        savedInputs = false;
        var characters = gameState.characters;
        registeredCharacters = new MLUCharacter[characters.Length];
        for (int i = 0; i < characters.Length; i++) {
            MLUCharacter newCharacter = Instantiate(characterPrefab).GetComponent<MLUCharacter>();
            registeredCharacters[i] = newCharacter;
        }
    }

    public void PrepareForNewRound() {
        savedInputs = false;
    }
}