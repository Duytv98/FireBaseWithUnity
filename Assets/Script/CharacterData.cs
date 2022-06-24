using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
[FirestoreData]
public struct CharacterData
{
    [FirestoreProperty]
    public string Name { get; set; }
    [FirestoreProperty]
    public int Score { get; set; }
    [FirestoreProperty]
    public int TotalTime { get; set; }

}
