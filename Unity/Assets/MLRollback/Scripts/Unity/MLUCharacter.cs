using SharedGame;
using TMPro;
using Unity.Mathematics.FixedPoint;
using UnityEngine;
using UnityEngine.UI;
using UnityGGPO;

public class MLUCharacter : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI statusTextBox;
    [SerializeField] private Image imgProgress;
    [SerializeField] private Transform characterArt;
    [SerializeField] private SpriteRenderer bodyArt;
    
    public void UpdateCharacter(MLCharacter character, PlayerConnectionInfo info) {
        UpdatePosition(character.position);
        Vector3 scale = characterArt.localScale;
        characterArt.localScale = new Vector3((character.facingRight ? 1 : -1) * Mathf.Abs(scale.x), scale.y, scale.z);
        bodyArt.color = character.playerIndex == 0 ? Color.blue : Color.red;
        UpdateConnectionInfo(info);
    }

    private void UpdatePosition(fp2 newPosition) {
        float xPos = (float)newPosition.x;
        float yPos = (float)newPosition.y;
        transform.position = new Vector3(xPos, yPos, 0);
    }

    private void UpdateConnectionInfo(PlayerConnectionInfo info)
    {
        string status = "";
        int progress = -1;
        switch (info.state)
        {
            case PlayerConnectState.Connecting:
                status = (info.type == GGPOPlayerType.GGPO_PLAYERTYPE_LOCAL) ? "Local Player" : "Connecting...";
                break;

            case PlayerConnectState.Synchronizing:
                progress = info.connect_progress;
                status = (info.type == GGPOPlayerType.GGPO_PLAYERTYPE_LOCAL) ? "Local Player" : "Synchronizing...";
                break;

            case PlayerConnectState.Disconnected:
                status = "Disconnected";
                break;

            case PlayerConnectState.Disconnecting:
                status = "Waiting for player...";
                progress = (Utils.TimeGetTime() - info.disconnect_start) * 100 / info.disconnect_timeout;
                break;
        }

        if (progress > 0)
        {
            imgProgress.gameObject.SetActive(true);
            imgProgress.fillAmount = progress / 100f;
        }
        else
        {
            imgProgress.gameObject.SetActive(false);
        }

        if (status.Length > 0)
        {
            statusTextBox.gameObject.SetActive(true);
            statusTextBox.text = status;
        }
        else
        {
            statusTextBox.gameObject.SetActive(false);
        }
    }

}