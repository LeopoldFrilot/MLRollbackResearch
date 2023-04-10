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
    Airborne
}

public struct MLAnimationFrameData {
    public MLPhysics.Rect hitbox;
    public int damage;
    public fp2 normalLaunchAngle;
    public fp2 blockLaunchAngle;
    public int hitStun;
    public int blockStun;

    public MLAnimationFrameData(MLUAnimationSO.AnimationFrameData frameData) {
        fp2 center = new fp2((fp)frameData.hitbox.center.x, (fp)frameData.hitbox.center.y);
        hitbox = new MLPhysics.Rect(center, (fp)frameData.hitbox.width, (fp)frameData.hitbox.height);
        damage = frameData.damage;
        normalLaunchAngle = new fp2((fp)frameData.launchAngle.x, (fp)frameData.launchAngle.y);
        blockLaunchAngle = new fp2((fp)frameData.launchAngle.x * (fp)frameData.blockKnockback, 0);
        hitStun = frameData.hitStun;
        blockStun = frameData.blockStun;
    }

    public override int GetHashCode() {
        int hashCode = 1716021829;
        hashCode = hashCode * -1777407407 + hitbox.GetHashCode();
        hashCode = hashCode * -1777407407 + damage.GetHashCode();
        hashCode = hashCode * -1777407407 + normalLaunchAngle.GetHashCode();
        hashCode = hashCode * -1777407407 + blockLaunchAngle.GetHashCode();
        hashCode = hashCode * -1777407407 + hitStun.GetHashCode();
        hashCode = hashCode * -1777407407 + blockStun.GetHashCode();
        return hashCode;
    }
}

public struct MLAnimationData {
    public AnimationTypes animationType;
    public int frames;
    public bool loopable;
    public MLAnimationFrameData[] frameData;

    public MLAnimationData(MLUAnimationSO animationData) {
        animationType = animationData.animationType;
        frames = animationData.spritesData.Count;
        loopable = animationData.loopable;
        frameData = new MLAnimationFrameData[animationData.spritesData.Count];
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
    
    // Not rolled back
    private MLAnimationData[] animData;

    public MLAnimationManager(MLAnimationData[] animData) {
        this.animData = animData;
        currentAnimationFrame = 0;
        currentAnimationDataIndex = 0;
        curAnimationType = AnimationTypes.Idle;
    }

    public void ProgressAnimation() {
        currentAnimationFrame++;
        Debug.Log(curAnimationType.ToString());
        if (currentAnimationFrame == animData[currentAnimationDataIndex].frames) {
            if (animData[currentAnimationDataIndex].loopable) {
                currentAnimationFrame = 0;
            }
            else {
                StartAnimation(AnimationTypes.Idle);
            }
        }
    }

    public void StartAnimation(AnimationTypes animationType) {
        if (curAnimationType != animationType) {
            curAnimationType = animationType;
            currentAnimationFrame = 0;
            for (int i = 0; i < animData.Length; i++) {
                if (animData[i].animationType == animationType) {
                    currentAnimationDataIndex = i;
                    break;
                }
            }
        }
    }

    public MLAnimationData GetCurrentAnimationData() {
        return animData[currentAnimationDataIndex];
    }

    public void Serialize(BinaryWriter bw) {
        bw.Write(currentAnimationFrame);
        bw.Write((int)curAnimationType);
        bw.Write(currentAnimationDataIndex);
    }

    public void Deserialize(BinaryReader br) {
        currentAnimationFrame = br.ReadInt32();
        curAnimationType = (AnimationTypes)br.ReadInt32();
        currentAnimationDataIndex = br.ReadInt32();
    }

    public override int GetHashCode() {
        int hashcode = 1024118323;
        hashcode = hashcode * -1485059183 + currentAnimationFrame.GetHashCode();
        hashcode = hashcode * -1485059183 + curAnimationType.GetHashCode();
        return hashcode;
    }
}