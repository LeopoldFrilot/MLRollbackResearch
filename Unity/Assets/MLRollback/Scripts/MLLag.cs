using System.IO;

public enum LagTypes {
    None,
    JumpStart,
    Dash,
    LandingLag,
}

public class MLLag : IMLSerializable {
    private LagTypes currentLagType;
    private int frameLagFinishes;

    public MLLag() {
        currentLagType = LagTypes.None;
    }

    public void ApplyLag(LagTypes lagType, int currentFrame) {
        currentLagType = lagType;
        frameLagFinishes = MLConsts.GetLagAmount(lagType) + currentFrame;
    }

    public LagTypes GetLagType(int currentFrame) {
        if (currentFrame > frameLagFinishes) {
            return LagTypes.None;
        }
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