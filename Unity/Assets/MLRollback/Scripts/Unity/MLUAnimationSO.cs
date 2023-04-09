using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAnimation", menuName = "NewAnimation", order = 0)]
public class MLUAnimationSO : ScriptableObject {
    [Serializable]
    public struct AnimationFrameData {
        public Sprite sprite;
        public Rect hitbox;
    }

    public struct KeyFrame {
        public AnimationFrameData data;
        public int frame;
    }
    
    public AnimationTypes animationType;
    public List<AnimationFrameData> spritesData;
    public bool loopable;
    
    private List<KeyFrame> keyFrames = new List<KeyFrame>();

    private void OnValidate() {
        keyFrames.Clear();
        for (int i = 0; i < spritesData.Count; i++) {
            if (spritesData[i].sprite != null) {
                KeyFrame newKey = new KeyFrame();
                newKey.data = spritesData[i];
                newKey.frame = i;
                keyFrames.Add(newKey);
            }
        }
    }

    public AnimationFrameData GetAnimationData(int frameIndex) {
        if (loopable) {
            frameIndex %= (spritesData.Count);
        }
        if (frameIndex < spritesData.Count) {
            return GetKeyFrame(frameIndex);
        }
        
        AnimationFrameData frameData = new AnimationFrameData();
        return frameData;
    }

    private AnimationFrameData GetKeyFrame(int frameIndex) {
        KeyFrame currentKeyFrame = new KeyFrame();
        
        foreach (KeyFrame keyFrame in keyFrames) {
            if (keyFrame.frame <= frameIndex) {
                currentKeyFrame = keyFrame;
            }
            else {
                break;
            }
        }
        
        return currentKeyFrame.data;
    }
}