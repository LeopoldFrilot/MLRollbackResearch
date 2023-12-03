using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAnimation", menuName = "NewAnimation", order = 0)]
public class MLUAnimationSO : ScriptableObject {
    [Serializable]
    public struct AnimationFrameData {
        public int frame;
        public Sprite sprite;
        public Rect hurtbox;
        public List<Rect> hitboxes;
        public int damage;
        public Vector2 launchAngle;
        public float blockKnockback;
        public int hitStun;
        public int blockStun;
    }
    
    public AnimationTypes animationType;
    public List<AnimationFrameData> spritesData;
    public bool loopable;

    [ContextMenu("Sort by frame and clean up")]
    private void Sort() {
        spritesData.Sort(SortAnimationFrames);

        List<AnimationFrameData> framesToRemove = new List<AnimationFrameData>();
        for (int i = 0; i < spritesData.Count; i++) {
            if (i != 0 && spritesData[i].frame == 0) {
                framesToRemove.Add(spritesData[i]);
            }
        }

        foreach (var frame in framesToRemove) {
            spritesData.Remove(frame);
        }
    }

    private int SortAnimationFrames(AnimationFrameData x, AnimationFrameData y) {
        return x.frame - y.frame;
    }

    public AnimationFrameData GetAnimationData(int frameIndex) {
        if (loopable) {
            frameIndex %= (GetLength());
        }
        if (frameIndex < GetLength()) {
            return GetKeyFrame(frameIndex);
        }
        
        AnimationFrameData frameData = new AnimationFrameData();
        return frameData;
    }

    private AnimationFrameData GetKeyFrame(int frameIndex) {
        AnimationFrameData currentKeyFrame = new AnimationFrameData();
        foreach (AnimationFrameData keyFrame in spritesData) {
            if (keyFrame.frame <= frameIndex) {
                currentKeyFrame = keyFrame;
            }
            else {
                break;
            }
        }
        
        return currentKeyFrame;
    }

    public int GetLength() {
        if (spritesData.Count > 0) {
            return spritesData[^1].frame + 1;
        }
        
        return 0;
    }
}