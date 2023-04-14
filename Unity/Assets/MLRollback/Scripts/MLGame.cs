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

    public MLGame(int numPlayers, MLAnimationData[] allAnimData) {
        FrameNumber = 0;
        GM = GameManager.Instance as MLGameManager;
        characters = new MLCharacter[Mathf.Min(numPlayers, MLConsts.MAX_PLAYERS)];
        for (int i = 0; i < characters.Length; i++) {
            characters[i] = new MLCharacter(i, GetStartingPosition(i), allAnimData);
            IMLCharacterPhysicsObject PO = characters[i];
            GM.physics.RegisterCharacterObject(ref PO);
        }
    }

    // Order should be: Physics->Animation->InputProcessing->GameLogic
    public void Update(long[] inputs, int disconnectFlags) {
        FrameNumber++;
        
        // Physics
        GM.physics.UpdatePhysics(FrameNumber);
        
        // Animation
        foreach (var character in characters) {
            UpdateAnimation(character);
        }

        // InputProcessing/GameLogic
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
            characters[i].UseInput(frameButtons, FrameNumber);
        }
    }

    private void UpdateAnimation(MLCharacter character) {
        if (character.playerIndex == 0 && characters.Length > 1) {
            character.facingRight = character.physicsObject.curPosition.x <= characters[1].physicsObject.curPosition.x;
        }
        else if (character.playerIndex == 1) {
            character.facingRight = !characters[0].facingRight;
        }
            
        MLAnimationManager anim = character.animManager;
        bool grounded = GM.physics.IsGrounded(character.physicsObject.curPosition);
        fp xVelocity = character.physicsObject.velocity.x;
        if (anim.curAnimationType == AnimationTypes.Idle && !grounded) {
            anim.StartAnimation(AnimationTypes.Airborne, grounded);
        }
        else if (anim.curAnimationType == AnimationTypes.Idle && xVelocity != 0) {
            anim.StartAnimation(AnimationTypes.Run, grounded);
        } 
        else if (anim.curAnimationType == AnimationTypes.Airborne && grounded) {
            anim.StartAnimation(AnimationTypes.Idle, grounded);
        }
        else if (anim.curAnimationType == AnimationTypes.Run && xVelocity == 0) {
            anim.StartAnimation(AnimationTypes.Idle, grounded);
        }
        else {
            anim.ProgressAnimation(grounded);
        }  
    }

    #region DONE_FOR_NOW
    public void LogInfo(string filename) {
        StringBuilder SB = new StringBuilder("");
        SB.Append("GameState object.\n");
        SB.AppendFormat("  num_characters: {0}.\n", characters.Length);
        for (int i = 0; i < characters.Length; i++) {
            var character = characters[i];
            SB.AppendFormat("  ship {0} position:  {1}\n", i, character.physicsObject.curPosition);
            SB.AppendFormat("  ship {0} facing direction: {1}.\n", i, character.facingRight ? "Right" : "Left");
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