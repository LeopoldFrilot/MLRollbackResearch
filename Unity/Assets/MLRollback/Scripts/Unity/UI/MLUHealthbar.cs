using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MLUHealthbar : MonoBehaviour {
    [SerializeField] private Slider healthbarSlider;
    [SerializeField] private TextMeshProUGUI nameText;

    public void UpdateHealthbar(float healthPercentage, string characterName) {
        healthbarSlider.value = healthPercentage;
        nameText.text = characterName;
    }
}
