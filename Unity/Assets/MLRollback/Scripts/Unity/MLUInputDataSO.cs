using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "DynamicInputSave", menuName = "DynamicInputSave", order = 0)]
public class MLUInputDataSO : ScriptableObject {
    [SerializeField] private int maxNumberAllowed = 20;
    public List<ArrayHolder> player0Input = new();
    public List<ArrayHolder> player1Input = new();
    
    [Serializable]
    public struct ArrayHolder {
        public List<long> array;

        public ArrayHolder(List<long> newArray) {
            array = newArray;
        }
    }

    public void AddInputData(List<long>[] newData) {
        if (player0Input.Count + 1 > maxNumberAllowed) {
            player0Input.RemoveAt(0);
            player1Input.RemoveAt(0);
        }
        
        List<List<ArrayHolder>> arrays = new List<List<ArrayHolder>> {
            player0Input,
            player1Input
        };
        
        for (int i = 0; i < arrays.Count; i++) {
            if (i < newData.Length) {
                arrays[i].Add(new ArrayHolder(newData[i]));
            }
            else {
                player0Input.Add(new ArrayHolder(new List<long>()));
            }
        }
    }

    public List<long> GetRandomData(int playerIndex) {
        if (player0Input.Count == 0) {
            return new ();
        }
        
        if (playerIndex == 0) {
            return player0Input[Random.Range(0, player0Input.Count)].array;
        }
        
        if (playerIndex == 1) {
            return player1Input[Random.Range(0, player1Input.Count)].array;
        }

        return new ();
    }
}