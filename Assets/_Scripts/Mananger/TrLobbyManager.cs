using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Firebase.Database;
using TMPro;

public class TrLobbyManager : MonoBehaviour
{
    static TrLobbyManager _instance;
    public static TrLobbyManager xInstance { get { return _instance; } }
    public CanvasGroup _imgFade;
    [SerializeField] Animator _anim;
    [SerializeField] Transform _trUIPuzzle;
    [SerializeField] GameObject _goQuitButton;

    // 메뉴 탭
    [Space]
    bool _isActivated = false;
    [SerializeField] TrUI_HoldButton _btnMenu;
    [SerializeField] Transform[] _trMenuComponents;
    int _numMenuComponenets;
    [SerializeField] float _distanceYMenuButton;
    [SerializeField] float _distanceYMenuComponents;
    [SerializeField] float _speedMCMove;
    Tween[] _tweenMoveMenuComponents;

    // 랭크 보드
    [Space]
    [SerializeField] TrPlayRankListBox _rankBoxOwn;
    [SerializeField] GameObject _goPreRankInfo;
    [SerializeField] Transform _trSelector;
    [SerializeField] TextMeshProUGUI _txtPersonalRank;
    [SerializeField] TextMeshProUGUI _txtTotalRank;
    [SerializeField] Color _colorPurple;
    [SerializeField] Sprite[] _spRanks;

    // 랭크 보드(전체 랭킹)
    [Space]
    [SerializeField] GameObject _goTotalRank;
    [SerializeField] Transform _trTotalRanksParent;
    [SerializeField] GameObject _goWaiting;
    [SerializeField] GameObject _goRanks;
    TrPlayRankListBox[] _rankBoxsTotal;
    [SerializeField] int _numRanks;
    [SerializeField] Transform _trTotalBtnTab;
    [SerializeField] int _maxReloadCoolTimeRank;
    float _currCoolTimeRank;


    // 랭크 보드(개인 랭킹)
    [Space]
    [SerializeField] GameObject _goPersonalRank;
    [SerializeField] Transform _trPersonalRanksParent;
    [SerializeField] Transform _trPersonalBtnTab;

    // 설정 창
    [Space]
    [SerializeField] TrUI_Window_ _winSetting;
    [SerializeField] Slider _sliderBGM;
    [SerializeField] Slider _sliderSFX;
    [SerializeField] GameObject _goVibCheck;
    [SerializeField] TextMeshProUGUI _txtVersion;

    [Space]
    [SerializeField] TextMeshProUGUI _txtCurrStamina;
    [SerializeField] TrUI_HoldButton _btnStart;

    [Space]
    public static bool _isFirstLobby = true;
    [SerializeField] GameObject _goChecks;
    [SerializeField] Image _imgCheckFill;
    [SerializeField] TextMeshProUGUI _txtCheck;
    float _currValue = 0;
    float _targetValue = 0;
    [SerializeField] string _urlTotalRank;

    //오브젝트
    [Space]
    [SerializeField] GameObject _sunRotation;
    [SerializeField] GameObject _enna;
    [SerializeField] GameObject _Bear;
    [SerializeField] Animator _animPeePee;
    [SerializeField] Animator _animBear;
    [SerializeField] Animator _animEnna;
    [SerializeField] ParticleSystem _bigSnowBlo;
    [SerializeField] ParticleSystem _smallSnowBlo;
    [SerializeField] ParticleSystem _snow;

    [Space]
#if PLATFORM_ANDROID
    [SerializeField] TrUI_Window_ _windowQuit;
#endif

    public void zActivateAndDisableMenu()
    {
        for (int i = 0; i < _numMenuComponenets; i++)
        {
            if (_tweenMoveMenuComponents[i] != null)
                _tweenMoveMenuComponents[i].Kill();
        }

        _isActivated = !_isActivated;
        if (_isActivated)
        {
            _btnMenu.zInteractDisable(true);

            float targetY = _btnMenu.transform.localPosition.y - _distanceYMenuButton;
            float speed = _speedMCMove;
            for (int i = 0; i < _numMenuComponenets; i++)
            {
                _tweenMoveMenuComponents[i] = _trMenuComponents[i].DOLocalMoveY(targetY, speed).SetSpeedBased();
                targetY -= _distanceYMenuComponents;
                speed *= 2;
            }
        }
        else
        {
            _btnMenu.zInteractiActive(true);

            float targetY = _btnMenu.transform.localPosition.y;
            float speed = _speedMCMove;
            for (int i = 0; i < _numMenuComponenets; i++)
            {
                _tweenMoveMenuComponents[i] = _trMenuComponents[i].DOLocalMoveY(targetY, speed).SetSpeedBased();
                speed *= 2;
            }
        }
    }

    public void zClickTotalTankURL()
    {
        Application.OpenURL(_urlTotalRank);
    }

    #region OnClickEvent
    public void zQuitApplication()
    {
#if PLATFORM_ANDROID
        Application.Quit();
#endif
    }
   

    public void zOnClickActiveVibrate()
    {
        TrAudio_UI.xInstance.zzPlay_ClickButtonNormal();
        int isActive = PlayerPrefs.GetInt(TT.strConfigVibrate);

        if (isActive == 1)
        {
            _goVibCheck.SetActive(false);
            PlayerPrefs.SetInt(TT.strConfigVibrate, 0);
        }
        else
        {
            _goVibCheck.SetActive(true);
            PlayerPrefs.SetInt(TT.strConfigVibrate, 1);
        }
    }

    public void zOnClickGameStart()
    {
        if (StaminaManager.zUseStamina(ref _txtCurrStamina))
        {
            TrAudio_UI.xInstance.zzPlay_ClickButtonNormal();
            _trUIPuzzle.GetComponent<CanvasGroup>().DOFade(0, 1f);
            _imgFade.DOFade(1, 2f).OnComplete(() => GameManager.xInstance.zSetPuzzleGame());
            _animPeePee.SetBool("PeePeeArm", false);
            _animBear.SetBool("Bear", false);
            _animEnna.SetBool("_animEnna", false);
            _snow.Stop();
            //_bigSnowBlo.Stop();
            _smallSnowBlo.Stop();
            _Bear.SetActive(false);
        }
    }
    #endregion

    #region RankBoard
    /// <summary>
    /// 0:Personal
    /// 1:Total
    /// </summary>
    /// <param name="num"></param>
    public void xClickRank(int num)
    {
        TrAudio_UI.xInstance.zzPlay_ClickButtonSmall();
        yChangeRank(num);
    }

    void yChangeRank(int num)
    {
        if (num == 0)
        {
            _goPersonalRank.SetActive(true);
            _goTotalRank.SetActive(false);
            _txtPersonalRank.color = Color.white;
            _txtTotalRank.color = _colorPurple;
            _trSelector.position = _trPersonalBtnTab.position;
        }
        else if (num == 1)
        {
            _goPersonalRank.SetActive(false);
            _goTotalRank.SetActive(true);
            if (_currCoolTimeRank >= _maxReloadCoolTimeRank)
            {
                _goWaiting.SetActive(true);
                _goRanks.SetActive(false);
                _currCoolTimeRank = 0;
                StartCoroutine(ySetTotalRankBoard());
            }
            _trSelector.position = _trTotalBtnTab.position;
            _txtPersonalRank.color = _colorPurple;
            _txtTotalRank.color = Color.white;
        }
    }

    // 개인 랭킹 셋팅
    IEnumerator ySetPersonalRankBaord()
    {
        DatabaseManager.xInstance._isDataRead = true;
        DatabaseManager.xInstance.zGetDataMyScores();
        yield return new WaitUntil(() => !DatabaseManager.xInstance._isDataRead);
        List<TrScoresDTO> rankList = DatabaseManager.xInstance._myScores;

        TrPlayRankListBox[] rankUIs = _trPersonalRanksParent.GetComponentsInChildren<TrPlayRankListBox>();

        int listCount = rankList.Count;
        string nickname = DatabaseManager._myDatas.nickName;

        for (int i = 0; i < listCount; i++)
        {
            TrPlayRankListBox uis = rankUIs[i];
            uis._txtScore.text = string.Format("{0}P", rankList[i].maxScore.ToString());
            uis._txtName.text = nickname;
        }

        for (int i = listCount; i < 5; i++)
        {
            TrPlayRankListBox uis = rankUIs[i];
            uis._txtScore.text = "-";
            uis._txtName.text = nickname;
        }


        if (listCount == 0)
        {
            rankUIs[0]._txtScore.text = string.Format("{0}P", DatabaseManager._myDatas.maxScore.ToString());
        }
    }

    // 전체 랭킹 비활성화(랭킹탭)
    void yDisableTotalRankInfos()
    {
        for (int i = 0; i < _numRanks; i++)
        {
            GameObject go = _rankBoxsTotal[i].gameObject;
            if (!go.activeSelf) return;
            go.SetActive(false);
        }
    }

    // 전체 랭킹 설정
    IEnumerator ySetTotalRankBoard()
    {
        DatabaseManager.xInstance._isDataRead = true;
        DatabaseManager.xInstance.zGetDataTotalScores();
        yield return new WaitUntil(() => !DatabaseManager.xInstance._isDataRead);
        if (DatabaseManager.xInstance._liScores != null)
        {
            List<TrScoresDTO> li = DatabaseManager.xInstance._liScores;
            int listCount = li.Count;
            int yourRanking = -1;
            int rank = 0;
            int index = 0;
            string ownName = DatabaseManager._myDatas.nickName;
            _rankBoxOwn._txtName.text = ownName;
            _rankBoxOwn._txtScore.text = string.Format("{0}P", DatabaseManager._myDatas.maxScore.ToString());
            while (index < _numRanks && index < listCount)
            {
                var user = li[index];
                //string name = user.Child("Nickname").Value.ToString();
                //string score = user.Child("MaxScore").Value.ToString();
                string name = user.nickName;
                string score = user.maxScore.ToString();

                Transform box = _rankBoxsTotal[index].transform;
                box.gameObject.SetActive(true);
                box.SetParent(_trTotalRanksParent);
                box.localScale = new Vector3(1f, 1f, 1f);
                TrPlayRankListBox list = box.GetComponent<TrPlayRankListBox>();
                list._txtName.text = name;
                list._txtScore.text = string.Format("{0}P", score);
                list._imgValueRanking.gameObject.SetActive(true);
                list._txtValueRanking.text = "";
                if (rank == 0) list._imgValueRanking.sprite = _spRanks[rank];
                else if (rank == 1) list._imgValueRanking.sprite = _spRanks[rank];
                else if (rank == 2) list._imgValueRanking.sprite = _spRanks[rank];
                else
                {
                    list._imgValueRanking.gameObject.SetActive(false);
                    list._txtValueRanking.text = (rank + 1).ToString();
                }

                list._imgValueRanking.SetNativeSize();
                if (name == ownName) yourRanking = rank;
                rank++;
                index++;
            }

            Image imgRank = _rankBoxOwn._imgValueRanking;
            TextMeshProUGUI txtRank = _rankBoxOwn._txtValueRanking;

            if (yourRanking <= 2 && yourRanking >= 0)
            {
                txtRank.text = "";
                imgRank.gameObject.SetActive(true);
                imgRank.sprite = _spRanks[yourRanking];
            }
            else if (yourRanking == -1)
            {
                imgRank.gameObject.SetActive(false);
                txtRank.gameObject.SetActive(true);
                txtRank.text = "-";
                txtRank.fontSize = 35;
            }
            else
            {
                imgRank.gameObject.SetActive(false);
                txtRank.gameObject.SetActive(true);
                txtRank.text = (yourRanking + 1).ToString();
                txtRank.fontSize = 70;
            }
            imgRank.SetNativeSize();
        }
        
        _goWaiting.SetActive(false);
        _goRanks.SetActive(true);
    }

    // 전체 랭킹 박스들 생성
    void yCreateRankBoxes()
    {
        _rankBoxsTotal = new TrPlayRankListBox[_numRanks];
        for (int i = 0; i < _numRanks; i++)
        {
            TrPlayRankListBox rankBox = Instantiate(_goPreRankInfo).GetComponent<TrPlayRankListBox>();
            rankBox.transform.SetParent(_trTotalRanksParent);
            _rankBoxsTotal[i] = rankBox;
            rankBox.gameObject.SetActive(false);
        }
    }
    #endregion

    #region setting
    // 설정창 초기 셋팅
    void yInitSetting()
    {
        _txtVersion.text = string.Format("version {0}", Application.version);
        _sliderBGM.value = PlayerPrefs.GetFloat(TT.strConfigMusic, 1);
        _sliderSFX.value = PlayerPrefs.GetFloat(TT.strConfigSFX, 1);
        _goVibCheck.SetActive(PlayerPrefs.GetInt(TT.strConfigVibrate, 1) == 1);
    }
    public void xActiveSettings(bool isActive)
    {
        if (isActive) _winSetting.zShow();
        else _winSetting.zHide();
    }
    #endregion

    // 버전 체크 기다리기
    IEnumerator yWaitCheckVersion()
    {
        _imgFade.DOFade(0, 2f);

        GameManager.xInstance._state = TT.enumGameState.Main;
        yCreateRankBoxes();
        yChangeRank(0);
        yInitSetting();
        TrAudio_Music.xInstance.zzPlayMain(0.25f);
        _currCoolTimeRank = _maxReloadCoolTimeRank;

        if (_goQuitButton)
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor)
                _goQuitButton.SetActive(true);
            else
                _goQuitButton.SetActive(false);
        }
        if (_isFirstLobby)
        {
            _goChecks.SetActive(true);
            AuthManager.xInstance.zIsSignIn(true, true);
/*#if PLATFORM_ANDROID && !UNITY_EDITOR
            yield return new WaitUntil(() => !DeviceManager._isNeedUpdate);
#endif*/
            _targetValue = 0.3f;

            AuthManager.xInstance.zSetFirebase();
            yield return new WaitUntil(() => AuthManager.xInstance._isCheckAutoSignIn);
            _targetValue = 0.6f;


            if (AuthManager.xInstance._isAutoSignIn)
            {
                yield return new WaitUntil(() => AuthManager._isCompleteSignIn && StaminaManager.xInstance._isSetStamina);
                _targetValue = 1.1f;
                yield return new WaitUntil(() => _currValue >= 1);
                _isFirstLobby = false;
                _goChecks.SetActive(false);
            }
            else
            {
                _targetValue = 1.1f;
                yield return new WaitUntil(() => _currValue >= 1);
                _isFirstLobby = false;
                _goChecks.SetActive(false);
                AuthManager.xInstance.zIsSignIn(false);
                //Debug.Log("before login and stamina");
                yield return new WaitUntil(() => AuthManager._isCompleteSignIn && StaminaManager.xInstance._isSetStamina);
            }
            GameManager._canBtnClick = true;
            //Debug.Log("before check nickname");
            yield return new WaitUntil(() => DatabaseManager._myDatas.nickName != "");
            //Debug.Log("complete");
            AuthManager.xInstance.zIsSignIn(true);

        }
        else
        {
            _goChecks.SetActive(false);
            GameManager._canBtnClick = true;
            AuthManager.xInstance.zIsSignIn(true);
        }

        StartCoroutine(ySetPersonalRankBaord());

        // TODO: 친구창 셋팅
        /*DatabaseManager.xInstance._isDataRead = true;
        DatabaseManager.xInstance.zGetDataByKey(TT.zFormatQueueByString(TT.USERTABLE, AuthManager.User.UserId, "Friends", "Request", AuthManager.User.UserId));
        yield return new WaitUntil(() => !DatabaseManager.xInstance._isDataRead);

        DatabaseManager.xInstance.zRemoveDataByKey(TT.zFormatQueueByString(TT.USERTABLE, AuthManager.User.UserId, "Friends", "Request", AuthManager.User.UserId));*/

        //DatabaseManager.xInstance.zPushDataByKeyNormal(TT.zFormatQueueByString(TT.USERTABLE, AuthManager.User.UserId, TT.FRIENDS, TT.LIST));

        //DatabaseManager.xInstance.zSetStatus();
    }

    IEnumerator ySun()
    {
        Transform sun = _sunRotation.transform;
        float startPosZ = 45;
        float targetPosZ = -45f;
        Tween moveTween = null;
        while (true)
        {
            if (moveTween != null)
                moveTween.Kill();
            moveTween = sun.DOLocalRotate(new Vector3(0f, 0f, targetPosZ), 30).SetEase(Ease.Linear);
            yield return TT.WaitForSeconds(28f);
            sun.rotation = Quaternion.Euler(0f, 0f, startPosZ);
        }

    }

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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GameManager.xInstance.zSetCamera();
        GameManager._canBtnClick = true;
        _numMenuComponenets = _trMenuComponents.Length;
        _tweenMoveMenuComponents = new Tween[_numMenuComponenets];
        for (int i = 0; i < _numMenuComponenets; i++){
            _trMenuComponents[i].localPosition = _btnMenu.transform.localPosition;
        }
        _imgFade.alpha = 1;
        StartCoroutine(yWaitCheckVersion());
        StartCoroutine(ySun());
        
        _animPeePee.SetBool("PeePeeArm", true);
        _animBear.SetBool("Bear", true);
        _animEnna.SetBool("EnnaMove", true);
        _snow.Play();
        //_bigSnowBlo.Play();
        _smallSnowBlo.Play();
        
    }

    void FixedUpdate()
    {
        if (_maxReloadCoolTimeRank > _currCoolTimeRank)
        {
            _currCoolTimeRank += Time.fixedDeltaTime;
        }

        if (_winSetting.transform.gameObject.activeSelf)
        {
            TrAudio_Music.xInstance.zzSetFlatVolume(_sliderBGM.value);
            TrAudio_SFX.xInstance.zzSetFlatVolume(_sliderSFX.value);
            TrAudio_UI.xInstance.zzSetFlatVolume(_sliderSFX.value);
        }

        if (DatabaseManager._myDatas != null)
        {
            if (DatabaseManager._myDatas.stamina <= 0)
                _btnStart.zInteractDisable();
            else
                _btnStart.zInteractiActive();
        }

        if (_isFirstLobby)
        {
            if (_currValue <= _targetValue)
            {
                _currValue += Time.fixedDeltaTime;
                _imgCheckFill.fillAmount = _currValue;
            }
        }
    }
#if PLATFORM_ANDROID
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            _windowQuit.zShow();
    }
#endif
}
