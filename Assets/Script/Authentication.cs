using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using UnityEngine.Networking;
using Firebase;
using Firebase.Auth;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Firebase.Extensions;

using Google;
//https://github.com/googlesamples/google-signin-unity
public class Authentication : MonoBehaviour
{
    private FirebaseAuth auth;

    private FirebaseUser user;
    [SerializeField] private string webClientId;
    private GoogleSignInConfiguration configuration;

    [SerializeField] private GameObject btnSignIn;
    [SerializeField] private GameObject btnSignOut;
    [SerializeField] private GameObject btnPlayGame;

    [SerializeField] private Text _userId;
    [SerializeField] private Text _providerId;
    [SerializeField] private Text _displayName;
    [SerializeField] private Text _email;
    [SerializeField] private Text _photoUrl;


    [SerializeField] private Text _loadingText;

    private string uid;
    private string displayName;





    void Start()
    {
        _loadingText.text = "Loading .......";
        configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };
        CheckFirebaseDependencies();

        StartCoroutine(LoadPlayer());
        // CreateListUser();
    }

    private void CheckFirebaseDependencies()
    {
        Debug.Log("CheckFirebaseDependencies");
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result == DependencyStatus.Available)
                {
                    Debug.Log("task.Result == DependencyStatus.Available");

                    auth = FirebaseAuth.DefaultInstance;

                    GoogleSignIn.Configuration = configuration;
                    GoogleSignIn.Configuration.UseGameSignIn = false;
                    GoogleSignIn.Configuration.RequestIdToken = true;
                    CheckCurrentUser();
                    // CreateListUser();
                }

                else
                    Debug.Log("Could not resolve all Firebase dependencies: " + task.Result.ToString());
            }
            else
            {
                Debug.Log("Dependency check was not completed. Error : " + task.Exception.Message);
            }
        });
    }

    private void CheckCurrentUser()
    {
        Debug.Log(" ===== CheckCurrentUser");
        user = auth.CurrentUser;
        if (user != null)
        {
            _userId.text = $"UserId: {user.UserId}";
            _providerId.text = $"ProviderId: {user.ProviderId}";
            _displayName.text = $"DisplayName: {user.DisplayName}";
            _email.text = $"Email: {user.Email}";
            _photoUrl.text = $"PhotoUrl: {user.PhotoUrl}";
            Debug.Log(string.Format("UserId: {0}\nProviderId: {1}\nDisplayName: {2}\nEmail: {3}\nPhotoUrl: {4}\n", user.UserId, user.ProviderId, user.DisplayName, user.Email, user.PhotoUrl));


            PlayerPrefs.SetString("uid", user.UserId);
            PlayerPrefs.SetString("DisplayName", user.DisplayName);

            btnSignIn.SetActive(false);
            btnSignOut.SetActive(true);
        }
        else
        {
            _userId.text = "";
            _providerId.text = "";
            _displayName.text = "";
            _email.text = "";
            _photoUrl.text = "";

            PlayerPrefs.SetString("uid", "");
            PlayerPrefs.SetString("DisplayName", "");

            btnSignIn.SetActive(true);
            btnSignOut.SetActive(false);
            btnPlayGame.SetActive(false);
        }
        _loadingText.text = "Complete ......";
    }


    public void SignInWithGoogle()
    {
        _loadingText.text = "Loading .......";
        GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(OnAuthenticationFinished);
    }

    public void SignOutFromGoogle()
    {

        _loadingText.text = "Loading .......";
        OnSignOut();
    }

    private void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {

        Debug.Log("======== OnAuthenticationFinished");
        if (task.IsFaulted)
        {
            using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    Debug.Log("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    Debug.Log("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            Debug.Log("Canceled");
        }
        else
        {
            SignInWithGoogleOnFirebase(task.Result.IdToken);
        }
    }

    private void SignInWithGoogleOnFirebase(string idToken)
    {

        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
        auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            AggregateException ex = task.Exception;
            if (ex != null)
            {
                if (ex.InnerExceptions[0] is FirebaseException inner && (inner.ErrorCode != 0))
                    Debug.Log("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
            }
            else
            {
                CheckCurrentUser();
                LoginSuccess();
            }
        });
    }

    private void OnSignOut()
    {
        auth.SignOut();
        CheckCurrentUser();
        if (index < arrName.Length)
        {
            index++;
            auth.SignOut();
        }
    }

    private Coroutine _coroutine;
    private void LoginSuccess()
    {
        StartCoroutine(LoadPlayer());
    }

    private IEnumerator LoadPlayer()
    {
        Debug.Log(user == null);
        var loadPlayerTask = RealtimeDatabase.Instance.LoadPlayer("LoadPlayer");
        yield return new WaitUntil(() => loadPlayerTask.IsCompleted);
        var player = loadPlayerTask.Result;
        Debug.Log(player);
        Debug.Log(player == null);

        btnPlayGame.SetActive(true);
        if (player == null)
        {
            RealtimeDatabase.Instance.SetDataDefault(user.UserId, user.DisplayName);

        }
        else
        {
            btnPlayGame.SetActive(true);
        }
        PlayGame();
    }
    public void PlayGame()
    {
        RealtimeDatabase.Instance.SetMatchingPlayer("LoadPlayer", true);
        StartCoroutine(MatchingPlayer());
    }
    private IEnumerator MatchingPlayer()
    {
        var loadPlayerTask = RealtimeDatabase.Instance.MatchingPlayer();
        yield return new WaitUntil(() => loadPlayerTask.IsCompleted);
        var player = loadPlayerTask.Result;
        Debug.Log(JsonUtility.ToJson(player));
        Debug.Log(player.name);
        Debug.Log(player == null);
    }
    string[] arrName = { "Dịp Thị Tú Giang", "Nguyễn Bảo Lộc", "Nguyễn Thúy Hiền", "Lê Minh Triệu", "Nguyễn Thu Hằng", "Mai Thị Diễm", "Nguyễn Bảo Hoàng", "Nguyễn Thị Hồng", "Nghiêm Thị Vân", "Nguyễn Đỗ Anh", "Nguyễn Xuân Bình", "Cao Hữu Thanh", "Bùi Thanh Huyền", "Trần Thị Hồng", "Phạm Mạnh Toàn", "Dương Hà Thi", "Trần Thanh Huyền", "Lê Thị Thảo" };
    string[] arrUid = { "sNJWqEsThNdHFmJhWTup47UHmFs2", "juIwDh6AVSZAhLBRPM5xXz902Y53", "fYyG0EYgz3e20yyw5mHiW6QWIzG3", "xLl5zwuK4SbM1vgDnZo5Is0cxdk1", "KDwW5Q1PvmV7ICS1BCR1ITIce3b2", "I6LKvrp66qNg0GE2uKdcevoYkR42", "3kS4hK9YtcMAmPndV52qJMKSvK63", "g76eJyUhKHX1GXjedmhnhKUV9Q43", "cOyniB72hChVJaqoEetPx3tE8zk1", "2RQMLfLTLOfkpCguGBUOytTXBqF3", "4X6Qw7tqCoSsvJ5ml4Yn96bX4kD2", "aTd36yf6YKY1PNlTaGZc7zVz8d82", "4qXKgxZn3KTC7L8yEWOQK6MGovh2", "72yE07TvbTYYfZdZZ9sCUcKzbTn1", "yr4ztYRBkTYbfrT9vhXFzxr3BaR2", "jqIV8GEG86Vruty4qUHuO5yQNMA2", "kCecvDFdvOd0QHghBiAbNG9ZPzi2", "fidzdDzVJ8YTc2CFmlaP1uvZBXB3" };

    private int index = 0;
    public void CreateListUser()
    {
        var password = "123@123a";
        CreateUserWithEmailAndPassword(NameToEmail(arrName[index]), password, arrName[index]);
    }
    private string NameToEmail(string name)
    {
        var text = RemoveUnicode(name);
        string[] names = text.ToString()
                             .Split(new string[] { " ", "-" },
                                    StringSplitOptions.RemoveEmptyEntries);
        var email = names[names.Length - 1];
        for (int i = 0; i < names.Length - 1; i++)
        {

            email += names[i][0];
        }
        email += "@gmail.com";
        return email;
    }
    public string RemoveUnicode(string text)
    {
        string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
    "đ",
    "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
    "í","ì","ỉ","ĩ","ị",
    "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
    "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
    "ý","ỳ","ỷ","ỹ","ỵ",};
        string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
    "d",
    "e","e","e","e","e","e","e","e","e","e","e",
    "i","i","i","i","i",
    "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
    "u","u","u","u","u","u","u","u","u","u","u",
    "y","y","y","y","y",};
        for (int i = 0; i < arr1.Length; i++)
        {
            text = text.Replace(arr1[i], arr2[i]);
            text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
        }
        return text;
    }

    private void CreateUserWithEmailAndPassword(string email, string password, string displayName)
    {
        Debug.Log("email: " + email);
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Debug.Log("Tao Tai Khoan Thanh Cong");
            SetDataUser(displayName);

        });
    }
    private void SetDataUser(string displayName)
    {
        Debug.Log(" ===== SetDataUser =========");
        CheckCurrentUser();
        if (user != null)
        {
            Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
            {
                DisplayName = displayName,
                PhotoUrl = new System.Uri("https://example.com/jane-q-user/profile.jpg"),
            };
            user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User profile updated successfully.");

            });
        }
    }
}
