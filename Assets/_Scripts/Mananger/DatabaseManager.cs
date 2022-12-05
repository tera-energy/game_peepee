using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TrUserNormalInfo
{
    public string UserId;
    public TrUserNormalInfo(string userId)
    {
        UserId = userId;
    }
}

[Serializable]
public class TrPlayRank
{
    public string _name;
    public int _score;
    public TrPlayRank(string name, int score)
    {
        _name = name;
        _score = score;
    }
}

/*public class TrJsonAbleListWrapper
{
    public List<TrPlayRank> _list;
    public TrJsonAbleListWrapper(List<TrPlayRank> list) => _list = list;
}*/

public class DatabaseManager : MonoBehaviour
{
    static DatabaseManager _instance;

    public static int _maxWaitingTime = 30;

    //public DataSnapshot _dataSnapshot;
    //public Dictionary<string, object> _dicfindDatas;
    [HideInInspector] public bool _isDataRead = false;
    [HideInInspector] public List<TrScoresDTO> _liScores;
    [HideInInspector] public List<TrScoresDTO> _myScores;

    [HideInInspector] public static TrUserDTO _myDatas;

    [HideInInspector] public bool _isSuccess;
    public static DatabaseManager xInstance { get { return _instance; } }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void yResetDomainCodes()
    {
        _instance = null;
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            zSetMyData();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            zGetMyData();
        }

    }

    #region Score

    public void zGetDataMyScores()
    {
        StartCoroutine(yGetDataMyScores());
    }

    IEnumerator yGetDataMyScores()
    {
        _myScores = null;
        DateTime baseDate = DateTime.Now;
        var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
        var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
        string addr = "a_user_rank_unity_api";
        string options = $"stDate={thisWeekStart.ToString("yyyy-MM-dd")}&endDate={thisWeekEnd.ToString("yyyy-MM-dd")}&{TT.USERID}={AuthManager._userId}";
        var www = UnityWebRequest.Get($"{TrEtcSetting.API_URL}/{TrProjectSettings._character}/{addr}?{options}");
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            //Debug.LogError(www.error);
        }
        else
        {
            byte[] results = www.downloadHandler.data;
            var json = Encoding.UTF8.GetString(results);
            _myScores = TT.zFromJson<TrScoresDTO>(json).ToList();
        }
        _isDataRead = false;
    }

    public void zGetDataTotalScores()
    {
        StartCoroutine(yGetDataTotalScores());
    }

    IEnumerator yGetDataTotalScores()
    {
        _liScores = null;
        DateTime baseDate = DateTime.Now;
        var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
        var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
        string addr = "rank_unity_api";
        string options = $"stDate={thisWeekStart.ToString("yyyy-MM-dd")}&endDate={thisWeekEnd.ToString("yyyy-MM-dd")}";
        var www = UnityWebRequest.Get($"{TrEtcSetting.API_URL}/{TrProjectSettings._character}/{addr}?{options}");
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            //Debug.LogError(www.error);
        }
        else
        {
            byte[] results = www.downloadHandler.data;
            var json = Encoding.UTF8.GetString(results);
            //Debug.Log(json);
            _liScores = TT.zFromJson<TrScoresDTO>(json).ToList();
        }
        _isDataRead = false;
    }
    public void zSetUserDataForScore(int maxScore)
    {
        StartCoroutine(ySetUserDataForScore(maxScore));
    }

    IEnumerator ySetUserDataForScore(int maxScore)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection(TT.USERID, AuthManager._userId));
        formData.Add(new MultipartFormDataSection(TT.MAXSCORE, maxScore + ""));
        string addr = "upload_best_score";
        UnityWebRequest www = UnityWebRequest.Post($"{TrEtcSetting.API_URL}/{TrProjectSettings._character}/{addr}", formData);
        www.SetRequestHeader("CustAuth", TrProjectSettings._character);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            //Debug.Log(www.error);
        }
        else
        {
            ////Debug.Log("Form upload complete!");
        }
        _isDataRead = false;
    }
    #endregion
    #region UserData
    public void zGetMyData()
    {
        StartCoroutine(yGetMyData());
    }
    IEnumerator yGetMyData()
    {
        _myDatas = null;
        DateTime baseDate = DateTime.Now;
        var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
        var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
        string addr = "a_user_info_unity";
        string options = $"{TT.USERID}={AuthManager._userId}&stDate={thisWeekStart.ToString("yyyy-MM-dd")}&endDate={thisWeekEnd.ToString("yyyy-MM-dd")}";
        var www = UnityWebRequest.Get($"{TrEtcSetting.API_URL}/{TrProjectSettings._character}/{addr}?{options}");
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            //Debug.LogError(www.error);
        }
        else
        {
            //Debug.Log("Compelete Get User Data");
            byte[] results = www.downloadHandler.data;
            var json = Encoding.UTF8.GetString(results);
            List<TrUserDTO> liUser = TT.zFromJson<TrUserDTO>(json).ToList();
            if (liUser.Count != 0)
            {
                _myDatas = TT.zFromJson<TrUserDTO>(json).ToList()[0];
            }
        }
        _isDataRead = false;
    }

    public void zSetMyData()
    {
        StartCoroutine(ySetMyData());
    }
    IEnumerator ySetMyData()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection(TT.USERID, AuthManager._userId));
        if (AuthManager.User != null && AuthManager.User.Email != "")
            formData.Add(new MultipartFormDataSection(TT.EMAIL, AuthManager.User.Email));
        else
            formData.Add(new MultipartFormDataSection(TT.EMAIL, "Guest"));
        formData.Add(new MultipartFormDataSection(TT.STAMINA, StaminaManager._maxStamina.ToString()));
        string addr = "register_unity";

        UnityWebRequest www = UnityWebRequest.Post($"{TrEtcSetting.API_URL}/{TrProjectSettings._character}/{addr}", formData);
        www.SetRequestHeader("CustAuth", TrProjectSettings._character);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            //Debug.Log(www.error);
        }
        else
        {
            //Debug.Log("Form upload complete!");
        }
        _isDataRead = false;
    }

    public void zPutNickname(string nick)
    {
        _isSuccess = false;
        StartCoroutine(yPutNickname(nick));
    }

    IEnumerator yPutNickname(string nick)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection(TT.USERID, AuthManager._userId));
        formData.Add(new MultipartFormDataSection(TT.NICKNAME, nick));

        string addr = "change_nick_unity";

        UnityWebRequest www = UnityWebRequest.Post($"{TrEtcSetting.API_URL}/{TrProjectSettings._character}/{addr}", formData);
        www.SetRequestHeader("CustAuth", TrProjectSettings._character);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            //Debug.Log(www.error);
        }
        else
        {
            //Debug.Log("Form upload complete!");
            byte[] results = www.downloadHandler.data;
            var json = Encoding.UTF8.GetString(results);
            bool check = JsonUtility.FromJson<TrCheckDTO>(json).success;
            if (check)
            {
                //Debug.Log("changing nick has successed");
                _myDatas.nickName = nick;
                _isSuccess = true;
            }
        }
        _isDataRead = false;
    }

    public void zDeleteUserData()
    {
        StartCoroutine(yDeleteUserData());
    }

    IEnumerator yDeleteUserData()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection(TT.USERID, AuthManager._userId));
        string addr = "unity_withdrawal";
        var www = UnityWebRequest.Post($"{TrEtcSetting.API_URL}/{TrProjectSettings._character}/{addr}", formData);
        www.SetRequestHeader("CustAuth", TrProjectSettings._character);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            //Debug.LogError(www.error);
        }
        else
        {
            //Debug.Log("Compelete Delete User Data");
            _myDatas = null;
        }
        _isDataRead = false;
    }

    public void zConnectOtherPlatform(string oldId)
    {
        StartCoroutine(yConnectOtherPlatform(oldId));
    }

    IEnumerator yConnectOtherPlatform(string oldId)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        _isSuccess = true;

        formData.Add(new MultipartFormDataSection("guestId", oldId));
        formData.Add(new MultipartFormDataSection(TT.USERID, AuthManager._userId));
        formData.Add(new MultipartFormDataSection(TT.EMAIL, AuthManager.User.Email));
        string addr = "unity_guest_to_user";
        var www = UnityWebRequest.Post($"{TrEtcSetting.API_URL}/{TrProjectSettings._character}/{addr}", formData);
        www.SetRequestHeader("CustAuth", TrProjectSettings._character);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            //Debug.LogError(www.error);
        }
        else
        {
            byte[] results = www.downloadHandler.data;
            var json = Encoding.UTF8.GetString(results);
            TrCheckDTO dto = JsonUtility.FromJson<TrCheckDTO>(json);
            //Debug.Log(json);
            if (!dto.success)
            {
                if (dto.msg == "duplicate")
                {
                    _isSuccess = false;
                }
            }
        }
        _isDataRead = false;
    }

    #endregion
    #region Stamina
    public void zSetUseStamina(string date = " ")
    {
        StartCoroutine(ySetUseStamina(date));
    }

    IEnumerator ySetUseStamina(string date)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection(TT.USERID, AuthManager._userId));
        formData.Add(new MultipartFormDataSection(TT.STAMINA, _myDatas.stamina.ToString()));
        formData.Add(new MultipartFormDataSection(TT.STAMINADATE, date));
        string addr = "change_stamina";
        UnityWebRequest www = UnityWebRequest.Post($"{TrEtcSetting.API_URL}/{TrProjectSettings._character}/{addr}", formData);
        www.SetRequestHeader("CustAuth", TrProjectSettings._character);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            //Debug.Log(www.error);
        }
        else
        {
            _myDatas.staminaDate = date;
            //Debug.Log("Form upload complete!");
        }
        _isDataRead = false;
    }
    #endregion

    public void zCheckVersion()
    {
        StartCoroutine(yCheckVersion());
    }

    IEnumerator yCheckVersion()
    {
        _isSuccess = false;
        string addr = "unity_latest_version";
        var www = UnityWebRequest.Get($"{TrEtcSetting.API_URL}/{TrProjectSettings._character}/{addr}");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            //Debug.LogError(www.error);
        }
        else
        {
            //Debug.Log("Compelete Get User Data");
            byte[] results = www.downloadHandler.data;
            var json = Encoding.UTF8.GetString(results);
            string getVersion = json.ToString();

            if (getVersion == Application.version)
                _isSuccess = true;
            else
                _isSuccess = false;

        }
        _isDataRead = false;
    }
}

#region TeraDB DTOs

[Serializable]
public class TrUserDTO
{
    public int id;
    public string userId;
    public string email;
    public string nickName;
    public int stamina;
    public string staminaDate;
    public int maxScore;
    public string createdAt;
    public string updatedAt;
}

[Serializable]
public class TrScoresDTO
{
    public int id;
    public string userId;
    public string email;
    public string nickName;
    public int maxScore;
    public string createdAt;
    public int rank;
}

[Serializable]
public class TrCheckDTO
{
    public bool success;
    public string msg;
}
#endregion
