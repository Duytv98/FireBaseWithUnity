using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase;
using Firebase.Auth;
using System.Globalization;
public class CloudFirestore : MonoBehaviour
{
    private string _characterPath = "character_sheets/one_cool_dude";

    [Header("In Data")]
    [SerializeField] private InputField _nameField;
    [SerializeField] private InputField _descriptionField;
    [SerializeField] private InputField _attackField;
    [SerializeField] private InputField _defenseField;
    [SerializeField] private Text _loadingText;


    [Header("Out Data")]
    [SerializeField] private Text _nameText;
    [SerializeField] private Text _descriptionText;
    [SerializeField] private Text _attackText;
    [SerializeField] private Text _defenseText;
    [SerializeField] private Text _outLoadingText;

    private FirebaseFirestore firestore;

    private void Start()
    {
        firestore = FirebaseFirestore.DefaultInstance;
        _loadingText.gameObject.SetActive(false);
        _outLoadingText.gameObject.SetActive(false);
    }

    public void SetData()
    {

        _loadingText.gameObject.SetActive(true);
        _loadingText.text = "Loading ......";
        var characterData = new CharacterData
        {
            Name = _nameField.text,
        };
        firestore.Document(_characterPath).SetAsync(characterData).ContinueWithOnMainThread(task =>
        {
            Assert.IsNull(task.Exception);
            _loadingText.text = "Finished uploading...";

        });
    }
    public void GetData()
    {
        _outLoadingText.gameObject.SetActive(true);
        _outLoadingText.text = "Loading ......";

        firestore.Document(_characterPath).GetSnapshotAsync()
        .ContinueWithOnMainThread(task =>
        {
            Assert.IsNull(task.Exception);
            var characterData = task.Result.ConvertTo<CharacterData>();
            _nameText.text = $"Name: {characterData.Name}";
            _outLoadingText.text = "Complete ......";

        });
    }
    public async void GetDataWithScore()
    {
        _outLoadingText.text = "Loading ......";
        CollectionReference citiesRef = firestore.Collection("User");
        Query query = citiesRef.OrderByDescending("Score").Limit(10);
        QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
        Debug.Log("await");
        Debug.Log("querySnapshot.Documents.count: " + querySnapshot.Count);
        foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
        {
            var data = documentSnapshot.ConvertTo<CharacterData>();
            Debug.Log("name: " + data.Name + "  Score: " + data.Score + "  TotalTime: " + GetTimeString(data.TotalTime));
        }
    }

    public void DataDefault()
    {
        string[] arrName = { "Dịp Thị Tú Giang", "Nguyễn Bảo Lộc", "Nguyễn Thúy Hiền", "Lê Minh Triệu", "Nguyễn Thu Hằng", "Mai Thị Diễm", "Nguyễn Bảo Hoàng", "Nguyễn Thị Hồng", "Nghiêm Thị Vân", "Nguyễn Đỗ Anh", "Nguyễn Xuân Bình", "Cao Hữu Thanh", "Bùi Thanh Huyền", "Trần Thị Hồng", "Phạm Mạnh Toàn", "Dương Hà Thi", "Trần Thanh Huyền", "Lê Thị Thảo" };
        string[] arrUid = { "sNJWqEsThNdHFmJhWTup47UHmFs2", "juIwDh6AVSZAhLBRPM5xXz902Y53", "fYyG0EYgz3e20yyw5mHiW6QWIzG3", "xLl5zwuK4SbM1vgDnZo5Is0cxdk1", "KDwW5Q1PvmV7ICS1BCR1ITIce3b2", "I6LKvrp66qNg0GE2uKdcevoYkR42", "3kS4hK9YtcMAmPndV52qJMKSvK63", "g76eJyUhKHX1GXjedmhnhKUV9Q43", "cOyniB72hChVJaqoEetPx3tE8zk1", "2RQMLfLTLOfkpCguGBUOytTXBqF3", "4X6Qw7tqCoSsvJ5ml4Yn96bX4kD2", "aTd36yf6YKY1PNlTaGZc7zVz8d82", "4qXKgxZn3KTC7L8yEWOQK6MGovh2", "72yE07TvbTYYfZdZZ9sCUcKzbTn1", "yr4ztYRBkTYbfrT9vhXFzxr3BaR2", "jqIV8GEG86Vruty4qUHuO5yQNMA2", "kCecvDFdvOd0QHghBiAbNG9ZPzi2", "fidzdDzVJ8YTc2CFmlaP1uvZBXB3" };

        for (int i = 0; i < arrName.Length; i++)
        {
            var name = arrName[i];
            var uid = arrUid[i];
            var path = GetPath("User", uid);
            var characterData = new CharacterData
            {
                Name = name,
                Score = UnityEngine.Random.Range(5, 11),
                TotalTime = UnityEngine.Random.Range(300, 3600)
            };
            SetData(path, characterData);
        }
    }
    public void SetData(string path, CharacterData characterData)
    {

        _loadingText.gameObject.SetActive(true);
        _loadingText.text = "Loading ......";

        firestore.Document(path).SetAsync(characterData).ContinueWithOnMainThread(task =>
        {
            Assert.IsNull(task.Exception);
            _loadingText.text = "Finished uploading...";
        });
    }
    private string GetPath(string collectionName, string docID)
    {
        return string.Format("{0}/{1}", collectionName, docID);
    }
    private string GetTimeString(int TotalTime)
    {
        TimeSpan time = TimeSpan.FromSeconds(TotalTime);
        return time.ToString(@"hh\:mm\:ss");
    }




}

