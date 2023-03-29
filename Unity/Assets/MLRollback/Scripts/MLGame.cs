using SharedGame;
using System;
using Unity.Collections;

[Serializable]
public class MLGame : IGame
{
    public int Framenumber { get; private set; }
    public int Checksum => GetHashCode();

    public MLGame(int numPlayers) 
    {
        
    }
    
    public void Update(long[] inputs, int disconnectFlags) 
    {
        throw new System.NotImplementedException();
    }

    public void FromBytes(NativeArray<byte> data) 
    {
        throw new System.NotImplementedException();
    }

    public NativeArray<byte> ToBytes() 
    {
        throw new System.NotImplementedException();
    }

    public long ReadInputs(int controllerId) 
    {
        throw new System.NotImplementedException();
    }

    public void LogInfo(string filename) 
    {
        throw new System.NotImplementedException();
    }

    public void FreeBytes(NativeArray<byte> data) 
    {
        throw new System.NotImplementedException();
    }
}