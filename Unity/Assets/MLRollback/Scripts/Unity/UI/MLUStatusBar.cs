using UnityEngine;

public class MLUStatusBar : MonoBehaviour {
    [SerializeField] private MLUHealthbar player1Healthbar;
    [SerializeField] private MLUHealthbar player2Healthbar;

    public void UpdateHealth(int playerIndex, float healthPercentage, string characterName) {
        if (playerIndex == 0) {
            player1Healthbar.UpdateHealthbar(healthPercentage, characterName);
        }
        else if (playerIndex == 1) {
            player2Healthbar.UpdateHealthbar(healthPercentage, characterName);
        }
    }
}
