/*using System.Collections;
using UnityEngine;
using Google.Play.AppUpdate;
using Google.Play.Common;


public class DeviceManager : MonoBehaviour
{
    static DeviceManager _instance;
    public static DeviceManager xInstance { get { return _instance; } }

    public static bool _isNeedUpdate = true;

    [SerializeField] GameObject _goWinUpdate;
    const string _urlGooglePlay = "https://play.google.com/store/apps/details?id=com.blazar.lex";
    AppUpdateManager appUpdateManager;

    public void zOnClickUpdate()
    {
        Application.OpenURL(_urlGooglePlay);
    }

    IEnumerator CheckForUpdate()
    {
        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
            appUpdateManager.GetAppUpdateInfo();

        // Wait until the asynchronous operation completes.
        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                _goWinUpdate.SetActive(true);
            }
            else
            {
                _isNeedUpdate = false;
            }

            //var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();
            //StartCoroutine(StartImmediateUpdate(appUpdateInfoResult, appUpdateOptions));
        }
        else
        {
            //Debug.Log("Check Update Error");
            _isNeedUpdate = false;
        }
        *//*        else
                {
                    // Log appUpdateInfoOperation.Error.
                }*//*
    }

    IEnumerator StartImmediateUpdate(AppUpdateInfo appUpdateInfoOp_i, AppUpdateOptions appUpdateIptions_i)
    {
        var startUpdateRequest = appUpdateManager.StartUpdate(
            appUpdateInfoOp_i,
            appUpdateIptions_i
            );
        yield return startUpdateRequest;
    }

    IEnumerator yCheckNetwork()
    {
        if (!TrNetworkManager.zGetIsConnectNetwork())
            TrNetworkManager.xInstance.zActiveConnectWindow(true);
        yield return new WaitUntil(() => TrNetworkManager.zGetIsConnectNetwork());
        TrNetworkManager.xInstance.zActiveConnectWindow(false);

        appUpdateManager = new AppUpdateManager();
        _goWinUpdate.SetActive(false);

        yield return TT.WaitForSeconds(2f);

        StartCoroutine(CheckForUpdate());
        *//*if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }*//*
    }
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


#if PLATFORM_ANDROID && !UNITY_EDITOR
    void Start(){
        if (AuthManager.User == null){
            StartCoroutine(yCheckNetwork());
        }
    }
#endif
}*/