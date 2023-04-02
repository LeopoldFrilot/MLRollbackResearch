using SharedGame;
using System;
using CONST = MLGameConstants;
using UnityEngine;

public class MLUGameManager : MonoBehaviour {
    [SerializeField] private GameObject characterPrefab;
    
    private MLUCharacter[] registeredCharacters = Array.Empty<MLUCharacter>();
    private GameManager gameManager => GameManager.Instance;

    private void Update() {
        if (gameManager.IsRunning) {
            UpdateGame(gameManager.Runner);
        }
    }

    public void UpdateGame(IGameRunner runner) {
        MLGame gameState = runner.Game as MLGame;
        if (gameState == null) {
            return;
        }
            
        GameInfo info = runner.GameInfo;
        MLCharacter[] characters = gameState.characters;
        if (registeredCharacters.Length != characters.Length) {
            ResetGame(gameState, info);
        }

        for (int i = 0; i < registeredCharacters.Length; i++){
            registeredCharacters[i].UpdatePosition(characters[i].position);
        }
    }

    private void ResetGame(MLGame gameState, GameInfo info) {
        var characters = gameState.characters;
        registeredCharacters = new MLUCharacter[characters.Length];
        for (int i = 0; i < characters.Length; i++) {
            MLUCharacter newCharacter = Instantiate(characterPrefab).GetComponent<MLUCharacter>();
            newCharacter.Initialize(characters[i], info.players[i]);
            registeredCharacters[i] = newCharacter;
        }
    }
}