using CONST = MLGameConstants;
using SharedGame;
using System;
using System.IO;
using System.Text;
using Unity.Collections;
using Unity.Mathematics.FixedPoint;
using UnityEngine;

[Serializable]
public class MLGame : IGame {
    public int FrameNumber { get; private set; }
    public int Checksum => GetHashCode();
    public MLCharacter[] characters;

    public MLGame(int numPlayers) {
        FrameNumber = 0;
        characters = new MLCharacter[Mathf.Min(numPlayers, CONST.MAX_PLAYERS)];
        for (int i = 0; i < characters.Length; i++) {
            characters[i] = new MLCharacter(GetStartingPosition(i));
        }
    }

    public void Update(long[] inputs, int disconnectFlags) {
        FrameNumber++;
        for (int i = 0; i < characters.Length; i++) {
            MLInput.FrameButtons frameButtons = new MLInput.FrameButtons();
            if ((disconnectFlags & (1 << i)) != 0) {
                
            }
            else {
                frameButtons = MLInput.ParseInputs(inputs[i], out string debugString);
                if (debugString != "") {
                    Debug.Log($"Inputs frame {FrameNumber}:{debugString}");
                }
            }
            characters[i].UseInput(frameButtons);
        }
    }

    public long ReadInputs(int controllerId) {
        return MLInput.SerializeInputs(controllerId);
    }

    private fp2 GetStartingPosition(int characterIndex) {
        switch (characterIndex) {
            case 0:
                return new fp2(-CONST.STARTING_POSITION_X, 0);
            case 1:
                return new fp2(CONST.STARTING_POSITION_X, 0);
            default:
                return fp2.zero;
        }
    }

    public void FromBytes(NativeArray<byte> bytes) {
        using (var memoryStream = new MemoryStream(bytes.ToArray())) {
            using (var reader = new BinaryReader(memoryStream)) {
                Deserialize(reader);
            }
        }
    }

    private void Deserialize(BinaryReader reader) {
        //throw new NotImplementedException();
    }

    public NativeArray<byte> ToBytes() {
        using (var memoryStream = new MemoryStream()) {
            using (var writer = new BinaryWriter(memoryStream)) {
                Serialize(writer);
            }
            return new NativeArray<byte>(memoryStream.ToArray(), Allocator.Persistent);
        }
    }

    private void Serialize(BinaryWriter writer) {
        //throw new NotImplementedException();
    }

    public void LogInfo(string filename) {
        StringBuilder SB = new StringBuilder("");
        /*
        string fp = "";
        fp += "GameState object.\n";
        fp += string.Format("  bounds: {0},{1} x {2},{3}.\n", _bounds.xMin, _bounds.yMin, _bounds.xMax, _bounds.yMax);
        fp += string.Format("  num_ships: {0}.\n", _ships.Length);
        for (int i = 0; i < _ships.Length; i++) {
            var ship = _ships[i];
            fp += string.Format("  ship {0} position:  %.4f, %.4f\n", i, ship.position.x, ship.position.y);
            fp += string.Format("  ship {0} velocity:  %.4f, %.4f\n", i, ship.velocity.x, ship.velocity.y);
            fp += string.Format("  ship {0} radius:    %d.\n", i, ship.radius);
            fp += string.Format("  ship {0} heading:   %d.\n", i, ship.heading);
            fp += string.Format("  ship {0} health:    %d.\n", i, ship.health);
            fp += string.Format("  ship {0} cooldown:  %d.\n", i, ship.cooldown);
            fp += string.Format("  ship {0} score:     {1}.\n", i, ship.score);
            for (int j = 0; j < ship.bullets.Length; j++) {
                fp += string.Format("  ship {0} bullet {1}: {2} {3} -> {4} {5}.\n", i, j,
                    ship.bullets[j].position.x, ship.bullets[j].position.y,
                    ship.bullets[j].velocity.x, ship.bullets[j].velocity.y);
            }
        }*/
        File.WriteAllText(filename, SB.ToString());
    }

    public void FreeBytes(NativeArray<byte> data) {
        if (data.IsCreated) {
            data.Dispose();
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
}