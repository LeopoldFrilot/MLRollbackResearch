using SharedGame;
using System;
using System.IO;
using System.Text;
using Unity.Collections;
using Unity.Mathematics.FixedPoint;
using UnityEngine;

[Serializable]
public class MLGame : IGame, IMLSerializable {
    public int FrameNumber { get; private set; }
    public int Checksum => GetHashCode();
    public MLCharacter[] characters;

    private MLGameManager GM;

    public MLGame(int numPlayers) {
        FrameNumber = 0;
        GM = GameManager.Instance as MLGameManager;
        characters = new MLCharacter[Mathf.Min(numPlayers, MLConsts.MAX_PLAYERS)];
        for (int i = 0; i < characters.Length; i++) {
            characters[i] = new MLCharacter(i, GetStartingPosition(i));
        }
    }

    public void Update(long[] inputs, int disconnectFlags) {
        FrameNumber++;
        // Input Usage
        for (int i = 0; i < characters.Length; i++) {
            MLInput.FrameButtons frameButtons = new MLInput.FrameButtons();
            if ((disconnectFlags & (1 << i)) != 0) {
                characters[i].HandleDisconnectedFrame();
            }
            else {
                frameButtons = MLInput.ParseInputs(inputs[i], out string debugString);
                if (debugString != "") {
                    GGPORunner.LogGame($"Inputs frame {FrameNumber}, Player: {characters[i].playerIndex}: {debugString}");
                }
            }
            characters[i].UseInput(frameButtons);
        }

        PostInputUpdate();
    }

    private void PostInputUpdate() {
        if (characters.Length > 1) {
            if (GM.physics.IsGrounded(characters[0].position)) {
                characters[0].facingRight = characters[0].position.x <= characters[1].position.x;
            }
            if (GM.physics.IsGrounded(characters[1].position)) {
                characters[1].facingRight = characters[1].position.x <= characters[0].position.x;
            }
        }
    }

    #region DONE_FOR_NOW
    public void LogInfo(string filename) {
        StringBuilder SB = new StringBuilder("");
        SB.Append("GameState object.\n");
        SB.AppendFormat("  num_characters: {0}.\n", characters.Length);
        for (int i = 0; i < characters.Length; i++) {
            var character = characters[i];
            SB.AppendFormat("  ship {0} position:  %.4f, %.4f\n", i, character.position.x, character.position.y);
            SB.AppendFormat("  ship {0} facing direction: %s.\n", i, character.facingRight ? "Right" : "Left");
        }
        File.WriteAllText(filename, SB.ToString());
    }
    
    public void Deserialize(BinaryReader br) {
        FrameNumber = br.ReadInt32();
        int characterCount = br.ReadInt32();
        if (characterCount != characters.Length) {
            characters = new MLCharacter[characterCount];
        }
        foreach (var character in characters)
        {
            character.Deserialize(br);
        }
    }

    public void Serialize(BinaryWriter bw) {
        bw.Write(FrameNumber);
        bw.Write(characters.Length);
        for (int i = 0; i < characters.Length; i++) {
            characters[i].Serialize(bw);
        }
    }
    
    public override int GetHashCode() {
        int hashCode = -1214587014;
        hashCode = hashCode * -1521134295 + FrameNumber.GetHashCode();
        foreach (var character in characters) {
            hashCode = hashCode * -1521134295 + character.GetHashCode();
        }
        return hashCode;
    }
    #endregion

    #region DONE
    private fp2 GetStartingPosition(int characterIndex) {
        switch (characterIndex) {
            case 0:
                return new fp2(-MLConsts.STARTING_POSITION_X, 0);
            case 1:
                return new fp2(MLConsts.STARTING_POSITION_X, 0);
            default:
                return fp2.zero;
        }
    }
    
    public long ReadInputs(int controllerId) {
        return MLInput.SerializeInputs(controllerId);
    }
    
    public void FromBytes(NativeArray<byte> bytes) {
        using (var memoryStream = new MemoryStream(bytes.ToArray())) {
            using (var reader = new BinaryReader(memoryStream)) {
                Deserialize(reader);
            }
        }
    }

    public NativeArray<byte> ToBytes() {
        using (var memoryStream = new MemoryStream()) {
            using (var writer = new BinaryWriter(memoryStream)) {
                Serialize(writer);
            }
            return new NativeArray<byte>(memoryStream.ToArray(), Allocator.Persistent);
        }
    }
    
    public void FreeBytes(NativeArray<byte> data) {
        if (data.IsCreated) {
            data.Dispose();
        }
    }
    #endregion
}