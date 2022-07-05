using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Extensions;



public class RealtimeDatabase : MonoBehaviour
{
    // Start is called before the first frame update
    public static RealtimeDatabase Instance;

    private DatabaseReference reference;
    [SerializeField] private Leaderboard leaderboard = null;
    [SerializeField] private Text _loadingText;

    private List<User> listUser;
    private bool loadData = true;
    private int limit = 11;
    private int plusLimit = 5;
    private string conditionCol;

    public List<User> ListUser { get => listUser; set => listUser = value; }
    public int Limit { get => limit; set => limit = value; }
    public string ConditionCol { get => conditionCol; set => conditionCol = value; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    void Start()
    {
        ConditionCol = GetConditionCol();
        ListUser = new List<User>();
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        Debug.Log("count: " + ListUser.Count);
    }

    private string GetConditionCol()
    {
        if (leaderboard.TxtCondition.Equals("Score")) return "Score";
        else if (leaderboard.TxtCondition.Equals("TotalTime")) return "TotalTime";
        else return null;
    }

    // Update is called once per frame
    public void GetData(int index, bool status = true)
    {
        if (!loadData || listUser.Count != index) return;
        limit += plusLimit;
        if (ConditionCol.Equals("Score"))
        {
            StartCoroutine(GetDataWithScore(status));
        }
        else if (ConditionCol.Equals("TotalTime"))
        {
            StartCoroutine(GetDataWithTotalTime(status));
        }

    }
    private void SaveDataDefault()
    {
        string[] arrName = { "Dịp Thị Tú Giang", "Nguyễn Bảo Lộc", "Nguyễn Thúy Hiền", "Lê Minh Triệu", "Nguyễn Thu Hằng", "Mai Thị Diễm", "Nguyễn Bảo Hoàng", "Nguyễn Thị Hồng", "Nghiêm Thị Vân", "Nguyễn Đỗ Anh", "Nguyễn Xuân Bình", "Cao Hữu Thanh", "Bùi Thanh Huyền", "Trần Thị Hồng", "Phạm Mạnh Toàn", "Dương Hà Thi", "Trần Thanh Huyền", "Lê Thị Thảo" };
        string[] arrUid = { "sNJWqEsThNdHFmJhWTup47UHmFs2", "juIwDh6AVSZAhLBRPM5xXz902Y53", "fYyG0EYgz3e20yyw5mHiW6QWIzG3", "xLl5zwuK4SbM1vgDnZo5Is0cxdk1", "KDwW5Q1PvmV7ICS1BCR1ITIce3b2", "I6LKvrp66qNg0GE2uKdcevoYkR42", "3kS4hK9YtcMAmPndV52qJMKSvK63", "g76eJyUhKHX1GXjedmhnhKUV9Q43", "cOyniB72hChVJaqoEetPx3tE8zk1", "2RQMLfLTLOfkpCguGBUOytTXBqF3", "4X6Qw7tqCoSsvJ5ml4Yn96bX4kD2", "aTd36yf6YKY1PNlTaGZc7zVz8d82", "4qXKgxZn3KTC7L8yEWOQK6MGovh2", "72yE07TvbTYYfZdZZ9sCUcKzbTn1", "yr4ztYRBkTYbfrT9vhXFzxr3BaR2", "jqIV8GEG86Vruty4qUHuO5yQNMA2", "kCecvDFdvOd0QHghBiAbNG9ZPzi2", "fidzdDzVJ8YTc2CFmlaP1uvZBXB3" };

        for (int i = 0; i < arrName.Length; i++)
        {

            var name = arrName[i];
            var uid = arrUid[i];

            var user = new User();
            user.DisplayName = name;
            user.Score = UnityEngine.Random.Range(5, 11);
            user.TotalTime = UnityEngine.Random.Range(300, 3600);


            string json = JsonUtility.ToJson(user);
            reference.Child("users").Child(uid).SetRawJsonValueAsync(json)
                  .ContinueWith(task =>
                  {
                      if (task.IsCompleted)
                      {
                          Debug.Log("successdully added data to firebase");
                      }
                      else Debug.Log("not successdully");
                  }); ;
        }
    }
    private IEnumerator GetDataWithScore(bool status)
    {
        loadData = false;
        // OrderByChild Tăng dần
        List<User> list = new List<User>();

        var DBTask = FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild(ConditionCol).LimitToLast(limit).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Faild to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null) { Debug.Log("DBTask.Result.Value == null"); }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            // foreach (DataSnapshot childSnapshot in snapshot.Children)
            {
                string displayName = childSnapshot.Child("DisplayName").Value.ToString();
                int score = int.Parse(childSnapshot.Child("Score").Value.ToString());
                int totalTime = int.Parse(childSnapshot.Child("TotalTime").Value.ToString());
                var user = new User();
                user.DisplayName = displayName;
                user.Score = score;
                user.TotalTime = totalTime;
                list.Add(user);
                // Debug.Log("displayName: " + displayName + "   Score: " + score + "totalTime: " + totalTime);
            }
            if (list.Count == listUser.Count) limit -= plusLimit;
            ListUser.Clear();
            listUser = list;
            leaderboard.Show(ListUser.Count, status);
            loadData = true;
        }
    }
    private IEnumerator GetDataWithTotalTime(bool status)
    {

        loadData = false;
        // OrderByChild Tăng dần
        List<User> list = new List<User>();

        var DBTask = FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild(ConditionCol).LimitToFirst(limit).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Faild to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null) { Debug.Log("DBTask.Result.Value == null"); }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            // foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            foreach (DataSnapshot childSnapshot in snapshot.Children)
            {
                string displayName = childSnapshot.Child("DisplayName").Value.ToString();
                int score = int.Parse(childSnapshot.Child("Score").Value.ToString());
                int totalTime = int.Parse(childSnapshot.Child("TotalTime").Value.ToString());
                var user = new User();
                user.DisplayName = displayName;
                user.Score = score;
                user.TotalTime = totalTime;
                list.Add(user);
                // Debug.Log("displayName: " + displayName + "   Score: " + score + "totalTime: " + totalTime);
            }
            if (list.Count == listUser.Count) limit -= plusLimit;
            ListUser.Clear();
            listUser = list;
            leaderboard.Show(ListUser.Count, status);
            loadData = true;
        }
    }




    public async Task<bool> GetDataUser(string uid, string name)
    {
        var dataSnapshot = await reference.Child("User").Child(uid).GetValueAsync();
        return dataSnapshot.Exists;
    }
    public async Task<Player?> LoadPlayer(string uid)
    {
        var dataSnapshot = await reference.Child("Player").Child(uid).GetValueAsync();
        if (!dataSnapshot.Exists) return null;
        return JsonUtility.FromJson<Player>(dataSnapshot.GetRawJsonValue());
    }
    public void RemoveSolo(string uid)
    {
        reference.Child("Player").Child(uid).RemoveValueAsync();
    }


    public void SetDataDefault(string uid, string name)
    {
        var player = new Player(name);
        string json = JsonUtility.ToJson(player);
        Debug.Log("json: " + json);
        reference.Child("Player").Child(uid).SetRawJsonValueAsync(json)
         .ContinueWith(task =>
                  {
                      if (task.IsCompleted)
                      {
                          Debug.Log("successdully added data to firebase");
                      }
                      else Debug.Log("not successdully");
                  });
    }
    public void SetMatchingPlayer(string uid, bool value)
    {
        reference.Child("Player").Child(uid).Child("isPlay").SetValueAsync(value)
         .ContinueWith(task =>
                  {
                      if (task.IsCompleted)
                      {
                          Debug.Log("successdully added isPlay firebase");
                      }
                      else Debug.Log("not successdully");
                  });
    }

    public async Task<Player?> MatchingPlayer()
    {
        var dataSnapshot = await FirebaseDatabase.DefaultInstance.GetReference("Player").OrderByChild("status").EqualTo(true).OrderByChild("isPlay").EqualTo(false).LimitToFirst(2).GetValueAsync();
        if (!dataSnapshot.Exists) return null;
        Debug.Log(dataSnapshot.Key);

        var player = new Player();
        foreach (var item in dataSnapshot.Children)
        {
            string name = item.Child("name").Value.ToString();
            bool status = bool.Parse(item.Child("status").Value.ToString());
            bool isPlay = bool.Parse(item.Child("isPlay").Value.ToString());
            int win = int.Parse(item.Child("win").Value.ToString());
            int lose = int.Parse(item.Child("lose").Value.ToString());
            player = new Player(name, status, isPlay, win, lose);
        }
        return player;
    }


}
