using System.IO;

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

public struct MLAnimationData {
    public AnimationTypes animationType;
    public int frames;
    public bool loopable;
}

public class MLAnimationManager : IMLSerializable {
    public int currentAnimationFrame;
    public AnimationTypes curAnimationType; 
    public int curAnimationDataIndex;
    
    // Not rolled back
    private MLAnimationData[] animData;

    public MLAnimationManager(MLAnimationData[] animData) {
        this.animData = animData;
        currentAnimationFrame = 0;
        curAnimationDataIndex = 0;
        curAnimationType = AnimationTypes.Idle;
    }

    public void ProgressAnimation() {
        currentAnimationFrame++;
        if (currentAnimationFrame == animData[curAnimationDataIndex].frames) {
            if (animData[curAnimationDataIndex].loopable) {
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
                    curAnimationDataIndex = i;
                    break;
                }
            }
        }
    }

    public void Serialize(BinaryWriter bw) {
        bw.Write(currentAnimationFrame);
        bw.Write((int)curAnimationType);
        bw.Write(curAnimationDataIndex);
    }

    public void Deserialize(BinaryReader br) {
        currentAnimationFrame = br.ReadInt32();
        curAnimationType = (AnimationTypes)br.ReadInt32();
        curAnimationDataIndex = br.ReadInt32();
    }

    public override int GetHashCode() {
        int hashcode = 1024118323;
        hashcode = hashcode * -1485059183 + currentAnimationFrame.GetHashCode();
        hashcode = hashcode * -1485059183 + curAnimationType.GetHashCode();
        return hashcode;
    }
}