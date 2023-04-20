using System;
using System.IO;

public enum LagTypes {
    None,
    JumpStart,
    Dash,
    LandingLag,
    Attack,
    Hit,
    Block
}

public class MLLag : IMLSerializable {
    private LagTypes currentLagType;
    private int frameLagFinishes;

    public Action<LagTypes> OnLagStarted;
    public void TriggerLagStarted(LagTypes LagType) {
        OnLagStarted?.Invoke(LagType);
    }

    public Action<LagTypes> OnLagEnded;
    public void TriggerLagEnded(LagTypes LagType) {
        OnLagEnded?.Invoke(LagType);
    }
    
    public MLLag() {
        currentLagType = LagTypes.None;
    }

    public void UpdateLag(int currentFrame) {
        if (currentLagType != LagTypes.None && currentFrame > frameLagFinishes) {
            ApplyLag(LagTypes.None, currentFrame, 0);
        }
    }

    public void ApplyLag(LagTypes lagType, int currentFrame, int lagLengthOverride = -1) {
        TriggerLagEnded(currentLagType);
        currentLagType = lagType;
        frameLagFinishes = lagLengthOverride >= 0 ? currentFrame + lagLengthOverride : MLConsts.GetLagAmount(lagType) + currentFrame;
        TriggerLagStarted(currentLagType);
    }

    public LagTypes GetLagType() {
        return currentLagType;
    }
    
    public void Serialize(BinaryWriter bw) {
        bw.Write((int)currentLagType);
        bw.Write(frameLagFinishes);
    }

    public void Deserialize(BinaryReader br) {
        currentLagType = (LagTypes)br.ReadInt32();
        frameLagFinishes = br.ReadInt32();
    }

    public override int GetHashCode() {
        int hashCode = 1786049731;
        hashCode = hashCode * -1879759213 + currentLagType.GetHashCode();
        hashCode = hashCode * -1879759213 + frameLagFinishes.GetHashCode();
        return hashCode;
    }
}