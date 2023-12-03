using System.IO;
using Unity.Mathematics.FixedPoint;
using UnityEngine;

public enum AnimationTypes {
    Idle,
    Run,
    Dash,
    Jump,
    Light,
    Medium,
    Heavy,
    Dead,
    Airborne,
    Hit,
    Block
}

public struct MLAnimationFrameData {
    public MLPhysics.Rect hurtbox;
    public MLPhysics.Rect[] hitboxes;
    public int damage;
    public fp2 normalLaunchAngle;
    public fp2 blockLaunchAngle;
    public int hitStun;
    public int blockStun;

    public MLAnimationFrameData(MLUAnimationSO.AnimationFrameData frameData) {
        Rect rect = frameData.hurtbox;
        fp2 boxCenter = new fp2((fp)rect.x, (fp)rect.y);
        this.hurtbox = new MLPhysics.Rect(boxCenter, (fp)rect.width, (fp)rect.height);

        int length = 0;
        if (frameData.hitboxes != null) {
            length = frameData.hitboxes.Count;
        }
        
        hitboxes = new MLPhysics.Rect[length];
        for (int i = 0; i < hitboxes.Length; i++) {
            rect = frameData.hitboxes[i];
            boxCenter = new fp2((fp)rect.x, (fp)rect.y);
            hitboxes[i] = new MLPhysics.Rect(boxCenter, (fp)rect.width, (fp)rect.height);
        }
        
        damage = frameData.damage;
        normalLaunchAngle = new fp2((fp)frameData.launchAngle.x, (fp)frameData.launchAngle.y);
        blockLaunchAngle = new fp2((fp)frameData.launchAngle.x * (fp)frameData.blockKnockback, 0);
        hitStun = frameData.hitStun;
        blockStun = frameData.blockStun;
    }
}

public struct MLAnimationData {
    public AnimationTypes animationType;
    public int frameLength;
    public bool loopable;
    public MLAnimationFrameData[] frameData;

    public MLAnimationData(MLUAnimationSO animationData) {
        animationType = animationData.animationType;
        frameLength = animationData.GetLength();
        loopable = animationData.loopable;
        frameData = new MLAnimationFrameData[frameLength];
        for (int i = 0; i < frameData.Length; i++) {
            var newFrame = new MLAnimationFrameData(animationData.GetAnimationData(i));
            frameData[i] = newFrame;
        }
    }
}

public class MLAnimationManager : IMLSerializable {
    public int currentAnimationFrame;
    public AnimationTypes curAnimationType; 
    
    private int currentAnimationDataIndex;
    public bool currentAnimationCombatUsed;
    
    // Not rolled back
    private MLAnimationData[] animData;

    public MLAnimationManager(MLAnimationData[] animData) {
        this.animData = animData;
        currentAnimationFrame = 0;
        currentAnimationDataIndex = 0;
        curAnimationType = AnimationTypes.Dead;
        currentAnimationCombatUsed = false;
    }

    public void StartAnimation(AnimationTypes animationType, bool grounded) {
        if (curAnimationType != animationType) {
            curAnimationType = animationType;
            currentAnimationFrame = -1;
            for (int i = 0; i < animData.Length; i++) {
                if (animData[i].animationType == animationType) {
                    currentAnimationDataIndex = i;
                    currentAnimationCombatUsed = false;
                    ProgressAnimation(grounded);
                    break;
                }
            }
        }
    }

    public void ProgressAnimation(bool grounded) {
        currentAnimationFrame++;
        if (currentAnimationFrame == animData[currentAnimationDataIndex].frameLength) {
            if (animData[currentAnimationDataIndex].loopable) {
                currentAnimationFrame = 0;
            }
            else {
                ReturnToIdle(grounded);
            }
        }
    }

    public MLAnimationData GetCurrentAnimationData() {
        return animData[currentAnimationDataIndex];
    }
    
    public MLAnimationFrameData GetCurrentAnimationFrameData() {
        return animData[currentAnimationDataIndex].frameData[currentAnimationFrame];
    }

    public void ReturnToIdle(bool grounded) {
        StartAnimation(grounded ? AnimationTypes.Idle : AnimationTypes.Airborne, grounded);
    }

    public void Serialize(BinaryWriter bw) {
        bw.Write(currentAnimationFrame);
        bw.Write((int)curAnimationType);
        bw.Write(currentAnimationDataIndex);
        bw.Write(currentAnimationCombatUsed);
    }

    public void Deserialize(BinaryReader br) {
        currentAnimationFrame = br.ReadInt32();
        curAnimationType = (AnimationTypes)br.ReadInt32();
        currentAnimationDataIndex = br.ReadInt32();
        currentAnimationCombatUsed = br.ReadBoolean();
    }

    public override int GetHashCode() {
        int hashcode = 1024118323;
        hashcode = hashcode * -1485059183 + currentAnimationFrame.GetHashCode();
        hashcode = hashcode * -1485059183 + curAnimationType.GetHashCode();
        hashcode = hashcode * -1485059183 + currentAnimationDataIndex.GetHashCode();
        hashcode = hashcode * -1485059183 + currentAnimationCombatUsed.GetHashCode();
        return hashcode;
    }
}