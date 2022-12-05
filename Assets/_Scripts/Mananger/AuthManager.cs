using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Google;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text.RegularExpressions;


public class AuthManager : MonoBehaviour
{
    static AuthManager _instance;
    static public AuthManager xInstance { get { return _instance; } }
    public bool IsFirebaseReady { get; private set; }
    public bool IsSignInOnProgress { get; private set; }

    // ´Ð³×ÀÓ ÀÔ·ÂÃ¢
    [SerializeField] TrUI_Window_ _goInputNickName;
    [SerializeField] TMP_InputField _textNickName;

    // ¾Ë¸²Ã¢
    [SerializeField] TrUI_Window_ _goNoticeWindow;
    [SerializeField] TextMeshProUGUI _txtNotice;
    Coroutine _coNotice;

    [SerializeField] GameObject _goBeforeSignIn;
    [SerializeField] GameObject _goAfterSignIn;

    [SerializeField] Button[] _btnSignIns;
    [SerializeField] Button[] _btnConnects;
    [SerializeField] TextMeshProUGUI _txtId;

    public string webClientId = "<your client id here>";

    public static FirebaseApp firebaseApp;
    public static FirebaseAuth firebaseAuth;
    public static GoogleSignInConfiguration configuration;

    public static FirebaseUser User = null;
    public static string _userId;

    public bool _isCheckAutoSignIn = false;
    public bool _isAutoSignIn;
    public static bool _isCompleteSignIn;
    bool _doSignOut = false;
    bool _doConnect = false;
    public static bool _isGuest = true;
    public enum TrPlatformType
    {
        NONE,
        GUEST,
        GOOGLE,
        APPLE,
    }

    public void zGuestLogin()
    {
        if (!IsFirebaseReady || IsSignInOnProgress || User != null) return;

        TT.zSetInteractButtons(ref _btnSignIns, false);
        TrAudio_UI.xInstance.zzPlay_ClickButtonNormal();
        IsSignInOnProgress = true;

        firebaseAuth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            /*            if (task.IsFaulted)
                            Debug.LogError(task.Exception);*/

            User = task.Result;
            _userId = User.UserId;
            PlayerPrefs.SetString(TT.AUTOLOGINID, _userId);
            PlayerPrefs.SetInt(TT.AUTOLOGINPLATFORM, (int)TrPlatformType.GUEST);
            PlayerPrefs.Save();
            IsSignInOnProgress = false;
            StartCoroutine(yCertify(TrPlatformType.GUEST));
        });
    }

    public void zAppleSignIn()
    {
        /*Credential credential = OAuthProvider.GetCredential("apple.com", appleIdToken, rawNonce, null);
        firebaseAuth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });*/
    }

    // ±¸±Û ·Î±×ÀÎ ¼ÂÆÃ
    public void zGoogleSignIn()
    {
        if (!IsFirebaseReady || IsSignInOnProgress || User != null) return;

        TT.zSetInteractButtons(ref _btnSignIns, false);
        TrAudio_UI.xInstance.zzPlay_ClickButtonNormal();
        IsSignInOnProgress = true;
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        //yNotice("Logging in....");
        //Debug.Log("Google login");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(yOnAuthenticationFinished);
    }
    // ±¸±Û ·Î±×ÀÎ °Ë»ç
    internal void yOnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    //Debug.Log("Got Error: " + error.Status + " " + error.Message);
                    yNotice("Failed login to Google");
                    TT.zSetInteractButtons(ref _btnSignIns, true);
                    IsSignInOnProgress = false;
                }
                else
                {
                    //Debug.Log(task.Exception.ToString());
                    yNotice("Failed login to Google");
                    TT.zSetInteractButtons(ref _btnSignIns, true);
                    IsSignInOnProgress = false;
                }
            }
        }
        else if (task.IsCanceled)
        {
            yNotice("Canceled");
            IsSignInOnProgress = false;
        }
        else
        {
            ySignInWithGoogleOnFirebase(task.Result.IdToken);
        }
    }
    // ±¸±Û ·Î±×ÀÎ ½ÇÇà
    void ySignInWithGoogleOnFirebase(string idToken)
    {
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
        firebaseAuth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                yNotice("Failed login to Google");
                TT.zSetInteractButtons(ref _btnSignIns, true);
                //Debug.Log(task.Exception);
                IsSignInOnProgress = false;
            }
            else
            {
                IsSignInOnProgress = false;
                User = task.Result;
                _userId = User.UserId;
                PlayerPrefs.SetString(TT.AUTOLOGINID, _userId);
                PlayerPrefs.SetInt(TT.AUTOLOGINPLATFORM, (int)TrPlatformType.GOOGLE);
                PlayerPrefs.Save();
                StartCoroutine(yCertify(TrPlatformType.GOOGLE));
            }
        });
    }

    // DatabaseManager·Î ·Î±×ÀÎ Á¤º¸¸¦ °Ë»ç ¹× Àû¿ë
    IEnumerator yCertify(TrPlatformType type)
    {
        DatabaseManager.xInstance._isDataRead = true;
        DatabaseManager.xInstance.zGetMyData();
        yield return new WaitUntil(() => !DatabaseManager.xInstance._isDataRead);

        if (_doConnect)
        {
            _isCompleteSignIn = true;
            yield break;
        }

        if (DatabaseManager._myDatas == null)
        {
            DatabaseManager.xInstance._isDataRead = true;
            DatabaseManager.xInstance.zSetMyData();
            yield return new WaitUntil(() => !DatabaseManager.xInstance._isDataRead);

            DatabaseManager.xInstance._isDataRead = true;
            DatabaseManager.xInstance.zGetMyData();
            yield return new WaitUntil(() => !DatabaseManager.xInstance._isDataRead);
        }

        _txtId.text = _userId;
        _isCompleteSignIn = true;

        if (type == TrPlatformType.GUEST)
        {
            _isGuest = true;
            TT.zSetInteractButtons(ref _btnConnects, true);
        }
        else if (type == TrPlatformType.GOOGLE || type == TrPlatformType.APPLE)
        {
            _isGuest = false;
            TT.zSetInteractButtons(ref _btnConnects, false);
        }

        if (DatabaseManager._myDatas.nickName == "" || DatabaseManager._myDatas.nickName == null)
        {
            _goInputNickName.zShow();
        }
        else
        {
            if (_doSignOut)
                zIsSignIn(true);
            //yNotice("Succeed login to Google");
        }
    }

    IEnumerator yCheckNickname()
    {
        string txtName = _textNickName.text;

        if (txtName.Length < 2 || txtName.Length > 10)
        {
            yNotice("Length of nickname doesn't match");
            yield break;
        }

        // Æ¯¼ö¹®ÀÚ µé¾î°¡¸é true, ¾Æ´Ï¸é false
        bool checkSL = Regex.IsMatch(txtName, @"[^a-zA-Z0-9°¡-ÆR]");

        if (checkSL)
        {
            yNotice("Special characters are not allowed");
            yield break;
        }
        DatabaseManager.xInstance._isDataRead = true;
        DatabaseManager.xInstance.zPutNickname(txtName);
        yield return new WaitUntil(() => !DatabaseManager.xInstance._isDataRead);

        if (!DatabaseManager.xInstance._isSuccess)
        {
            yNotice("It's a nickname that already exists");
            yield break;
        }

        yNotice("The nickname is decided");
        _goInputNickName.zHide();
    }

    public void zSelectNickname()
    {
        TrAudio_UI.xInstance.zzPlay_ClickButtonNormal();
        StartCoroutine(yCheckNickname());
    }

    public void zSignoutAuth()
    {
        TrAudio_UI.xInstance.zzPlay_ClickButtonNormal();
        yResetStatics();
        PlayerPrefs.SetString(TT.AUTOLOGINID, "");
        PlayerPrefs.SetInt(TT.AUTOLOGINPLATFORM, (int)TrPlatformType.NONE);
        PlayerPrefs.Save();
        TrLobbyManager._isFirstLobby = true;

        SceneManager.LoadScene(TT.strLOBBY);
    }

    IEnumerator yDeleteAuth()
    {
        /*if(User == null){
            yield return new WaitUntil(() => IsFirebaseReady);
            TrPlatformType type = (TrPlatformType)PlayerPrefs.GetInt(TT.AUTOLOGINPLATFORM);
            switch (type){
                case TrPlatformType.GOOGLE:
                    zGoogleSignIn();
                    break;
                case TrPlatformType.APPLE:
                    zAppleSignIn();
                    break;
            }
            yield return new WaitUntil(() => User != null);
            if(type != TrPlatformType.GUEST)
                firebaseAuth.CurrentUser.DeleteAsync();
        }*/
        DatabaseManager.xInstance._isDataRead = true;
        DatabaseManager.xInstance.zDeleteUserData();
        yield return new WaitUntil(() => !DatabaseManager.xInstance._isDataRead);

        zSignoutAuth();
    }

    public void zDeleteAuth()
    {
        StartCoroutine(yDeleteAuth());
    }

    IEnumerator yConnectPlatform(TrPlatformType type)
    {
        yNotice("Connecting...");
        string guestId = _userId;
        FirebaseUser tempUser = null;
        if (User != null)
        {
            tempUser = User;
        }
        yResetStatics();
        TrUserDTO userData = DatabaseManager._myDatas;

        switch (type)
        {
            case TrPlatformType.GOOGLE:
                zGoogleSignIn();
                break;
            case TrPlatformType.APPLE:
                zAppleSignIn();
                break;
        }
        yield return new WaitUntil(() => _isCompleteSignIn);

        DatabaseManager.xInstance._isDataRead = true;
        DatabaseManager.xInstance.zConnectOtherPlatform(guestId);
        yield return new WaitUntil(() => !DatabaseManager.xInstance._isDataRead);

        if (!DatabaseManager.xInstance._isSuccess)
        {
            yNotice("Account already exists");
            _userId = guestId;
            PlayerPrefs.SetString(TT.AUTOLOGINID, _userId);
            PlayerPrefs.SetInt(TT.AUTOLOGINPLATFORM, (int)TrPlatformType.GUEST);
            PlayerPrefs.Save();
            if (tempUser != null)
                User = tempUser;
            DatabaseManager._myDatas = userData;
        }
        else
        {
            yNotice("Completed connection");
            TT.zSetInteractButtons(ref _btnConnects, false);
            _txtId.text = _userId;
        }

        _doConnect = false;
    }

    public void zConnectOtherPlatformForGuest(int type)
    {
        if (!_doConnect)
        {
            _doConnect = true;
            StartCoroutine(yConnectPlatform((TrPlatformType)type));
        }
    }

    void yResetStatics()
    {
        User = null;
        IsSignInOnProgress = false;
        _isCompleteSignIn = false;
    }

    public void zDeleteSessions()
    {
        PlayerPrefs.DeleteAll();
        // Debug.Log("Delete Sessions");
        yNotice("sessions have been removed");
    }

    // Error Text º¯°æ
    public void yNotice(string text)
    {
        if (_coNotice != null)
        {
            _goNoticeWindow.zHide();
            StopCoroutine(_coNotice);
        }
        _goNoticeWindow.zShow();
        _txtNotice.text = text;
        _coNotice = StartCoroutine(yCancelNoticeWindow(text));
    }

    IEnumerator yCancelNoticeWindow(string text)
    {
        yield return TT.WaitForSeconds(2f);

        _goNoticeWindow.zHide();
    }

    // Error Text Ã¢ ºñÈ°¼ºÈ­
    public void zCancelInfoWIndow()
    {
        TrAudio_UI.xInstance.zzPlay_ClickNo();
        _goNoticeWindow.zHide(false);
    }

    public void zIsSignIn(bool isAfter, bool isInit = false)
    {
        if (isInit)
        {
            _goAfterSignIn.SetActive(false);
            _goBeforeSignIn.SetActive(false);
            return;
        }
        _goAfterSignIn.SetActive(isAfter);
        _goBeforeSignIn.SetActive(!isAfter);
    }

    IEnumerator yCreateDummyUser()
    {
        yield return new WaitUntil(() => IsFirebaseReady);
        bool isDummyCom = false;
        firebaseAuth.SignInWithEmailAndPasswordAsync("test@test.com", "123456").ContinueWithOnMainThread(task =>
        {
            User = task.Result;
            isDummyCom = true;
            _userId = User.UserId;
        });

        yield return new WaitUntil(() => isDummyCom);

        DatabaseManager.xInstance._isDataRead = true;
        DatabaseManager.xInstance.zGetMyData();
        yield return new WaitUntil(() => !DatabaseManager.xInstance._isDataRead);
        _isCompleteSignIn = true;
    }

    IEnumerator yAutoLogin()
    {
        yield return new WaitUntil(() => IsFirebaseReady);
        _userId = PlayerPrefs.GetString(TT.AUTOLOGINID);
        TrPlatformType type = (TrPlatformType)PlayerPrefs.GetInt(TT.AUTOLOGINPLATFORM);
        StartCoroutine(yCertify(type));
    }

    void yCheckAutoLogin()
    {
        _isAutoSignIn = PlayerPrefs.GetString(TT.AUTOLOGINID, "") != "";
        _isCheckAutoSignIn = true;
        if (_isAutoSignIn)
        {
            StartCoroutine(yAutoLogin());
        }
    }

    IEnumerator ySetFirebaseCo()
    {
        if (!TrNetworkManager.zGetIsConnectNetwork())
        {
            TrNetworkManager.xInstance.zActiveConnectWindow(true);
            yield return new WaitUntil(() => TrNetworkManager.zGetIsConnectNetwork());
            TrNetworkManager.xInstance.zActiveConnectWindow(false);
        }

#if PLATFORM_ANDROID || PLATFORM_IOS
        // ±¸±Û ·Î±×ÀÎÀ» À§ÇÑ WebClientId
        if (configuration == null)
            configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };

        IsSignInOnProgress = false;
        IsFirebaseReady = false;
#endif
        // Firebase¿Í ¿¬°áÀ» Ã¼Å© ÈÄ º¯¼öµéÀ» ÃÊ±âÈ­ÇØ ÁØ´Ù.
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var result = task.Result;

            if (result != DependencyStatus.Available)
            {
                yNotice(result.ToString());
                IsFirebaseReady = false;
            }
            else
            {
                IsFirebaseReady = true;
                if (firebaseApp == null)
                    firebaseApp = FirebaseApp.DefaultInstance;
                if (firebaseAuth == null)
                    firebaseAuth = FirebaseAuth.DefaultInstance;
            }
        });

#if UNITY_EDITOR
        StartCoroutine(yCreateDummyUser());
        _isAutoSignIn = true;
#endif

#if !UNITY_EDITOR
        yCheckAutoLogin();
#endif
#if UNITY_EDITOR
        _isCheckAutoSignIn = true;
#endif
    }

    public void zSetFirebase()
    {
        StartCoroutine(ySetFirebaseCo());
    }

    public void zSetBtnsConnect()
    {
        TT.zSetInteractButtons(ref _btnConnects, _isGuest);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void yResetDomainCodes()
    {
        firebaseApp = null;
        firebaseAuth = null;
        User = null;
        configuration = null;
        _instance = null;
    }

    void Awake()
    {
        if (_instance == null)
        {
            //DontDestroyOnLoad(gameObject);
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}