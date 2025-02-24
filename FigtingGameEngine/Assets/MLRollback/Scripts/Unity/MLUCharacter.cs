﻿using SharedGame;
using System;
using System.Collections.Generic;
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
    [SerializeField] private BoxCollider2D debugCollider;
    public List<MLUAnimationSO> animations;

    private MLUStatusBar statusBar;

    private void Awake() {
        statusBar = FindObjectOfType<MLUStatusBar>();
    }

    public void UpdateCharacter(MLCharacter character, PlayerConnectionInfo info) {
        UpdatePosition(character.physicsObject.curPosition);
        Vector3 scale = characterArt.localScale;
        characterArt.localScale = new Vector3((character.facingRight ? 1 : -1) * Mathf.Abs(scale.x), scale.y, scale.z);
        bodyArt.color = character.playerIndex == 0 ? Color.white : Color.red;
        UpdateConnectionInfo(info);
        foreach (MLUAnimationSO animationSO in animations) {
            if (animationSO.animationType == character.animManager.curAnimationType) {
                var data = animationSO.GetAnimationData(character.animManager.currentAnimationFrame);
                Sprite sprite = data.sprite;
                if (sprite) {
                    bodyArt.sprite = sprite;
                    debugCollider.offset = new Vector2(data.hurtbox.x, data.hurtbox.y);
                    debugCollider.size = new Vector2(data.hurtbox.width, data.hurtbox.height);
                }
                break;
            }
        }
        MLPhysics.Rect hurtBox = character.GetHurtBoxes()[0];
        DrawRect(hurtBox, Color.blue);
        foreach (MLPhysics.Rect hitbox in character.GetHitboxes()) {
            DrawRect(hitbox, Color.green);
        }

        statusBar.UpdateHealth(
            character.playerIndex, 
            (float)character.GetHealthPercentage(), 
            (float)character.GetBlockPercentage(),
            character.name);
    }

    private void UpdatePosition(fp2 newPosition) {
        float xPos = (float)newPosition.x;
        float yPos = (float)newPosition.y;
        transform.position = new Vector3(xPos, yPos, 0);
    }

    private void DrawRect(MLPhysics.Rect Rect, Color color) {
        Debug.DrawLine(new Vector2((float)Rect.Left, (float)Rect.Top), new Vector2((float)Rect.Right, (float)Rect.Top), color, Time.deltaTime);
        Debug.DrawLine(new Vector2((float)Rect.Right, (float)Rect.Top), new Vector2((float)Rect.Right, (float)Rect.Bottom), color, Time.deltaTime);
        Debug.DrawLine(new Vector2((float)Rect.Right, (float)Rect.Bottom), new Vector2((float)Rect.Left, (float)Rect.Bottom), color, Time.deltaTime);
        Debug.DrawLine(new Vector2((float)Rect.Left, (float)Rect.Bottom), new Vector2((float)Rect.Left, (float)Rect.Top), color, Time.deltaTime);

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