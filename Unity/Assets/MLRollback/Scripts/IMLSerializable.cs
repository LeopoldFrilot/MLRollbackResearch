using System.IO;

public interface IMLSerializable {
    public void Serialize(BinaryWriter bw);
    public void Deserialize(BinaryReader br);
}