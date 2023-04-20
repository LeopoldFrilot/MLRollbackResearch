using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MLUHealthbar : MonoBehaviour {
    [SerializeField] private Slider healthbarSlider;
    [SerializeField] private Slider blockbarSlider;
    [SerializeField] private TextMeshProUGUI nameText;

    public void UpdateHealthbar(float healthPercentage, float blockPercentage, string characterName) {
        healthbarSlider.value = healthPercentage;
        blockbarSlider.value = blockPercentage;
        nameText.text = characterName;
    }
}
