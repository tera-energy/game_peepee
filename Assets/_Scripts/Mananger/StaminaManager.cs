using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class StaminaManager : MonoBehaviour
{
    static StaminaManager _instance;

    public static StaminaManager xInstance { get { return _instance; } }



    public static int _maxStamina = 5;      // 최대 스테미나 개수
    //public static int DatabaseManager._myDatas.staminaTemp;
    [SerializeField] int _coolTimeMinute;   // 총 쿨타임이 몇 분인지
    float _maxCoolTimeSecond;               // 총 쿨타임(초)
    float _currCoolTimeSecond;              // 현재 쿨타임
    static string _lastDate;

    // UI
    [SerializeField] GameObject _goStaminaCount;
    [SerializeField] TextMeshProUGUI _txtCurrStamina;
    [SerializeField] TextMeshProUGUI _txtCountMinute;
    [SerializeField] TextMeshProUGUI _txtCountSecond;

    public bool _isFull = true;
    bool _isAlreadyUpdate = false;
    public bool _isSetStamina;


    // 스테미너 사용
    public static bool zUseStamina(ref TextMeshProUGUI txt)
    {
        Debug.Log(txt);
        Debug.Log(DatabaseManager._myDatas);
        Debug.Log(DatabaseManager._myDatas.stamina);
        if (DatabaseManager._myDatas.stamina <= 0)
            return false;

        string date;
        if (DatabaseManager._myDatas.stamina == _maxStamina)
        {
            date = DateTime.Now.ToString();
            _lastDate = date;
        }
        else if (DatabaseManager._myDatas.stamina < _maxStamina)
            date = _lastDate;
        else
            date = " ";

        DatabaseManager._myDatas.stamina--;
        txt.text = DatabaseManager._myDatas.stamina.ToString();
        DatabaseManager.xInstance.zSetUseStamina(date);

        return true;
    }

    // 초기 스테미너 셋팅
    public void zSetRemainStamina(string lastDate)
    {
        if (DatabaseManager._myDatas.stamina >= _maxStamina)
        {
            yFullStamina();
            _isSetStamina = true;
            return;
        }

        _lastDate = lastDate;

        float needFullSecond = (_maxStamina - DatabaseManager._myDatas.stamina) * _maxCoolTimeSecond;
        int diffSecond = TT.zGetDateDiffCurrToSeconds(ref lastDate);

        // 시간차가 총 필요한 스테미나들의 쿨타임보다 많을 때
        if (needFullSecond <= diffSecond)
        {
            DatabaseManager._myDatas.stamina = _maxStamina;
            yFullStamina(true);
        }
        else
        {
            _isFull = false;
            _goStaminaCount.SetActive(true);
        }

        _isSetStamina = true;
    }

    IEnumerator yInitStamina()
    {
        yield return new WaitUntil(() => AuthManager._isCompleteSignIn);
        _maxCoolTimeSecond = 60 * _coolTimeMinute;
        zSetRemainStamina(DatabaseManager._myDatas.staminaDate);

        _txtCurrStamina.text = DatabaseManager._myDatas.stamina.ToString();
    }

    void yFullStamina(bool isDatabaseUpdate = false)
    {
        _isFull = true;
        _goStaminaCount.SetActive(false);
        if (isDatabaseUpdate)
        {
            DatabaseManager.xInstance.zSetUseStamina();
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
        _isFull = true;
        _isSetStamina = false;
    }

    void Start()
    {
        StartCoroutine(yInitStamina());
    }

    void FixedUpdate()
    {
        if (_isFull || GameManager.xInstance._state == TT.enumGameState.Play) return;

        int diffSecond = TT.zGetDateDiffCurrToSeconds(ref _lastDate);
        _currCoolTimeSecond = (diffSecond % _maxCoolTimeSecond) + 1;
        float restCool = _maxCoolTimeSecond - _currCoolTimeSecond;
        int min = (int)restCool / 60;
        int sec = (int)restCool % 60;
        _txtCountMinute.text = min.ToString("00");
        _txtCountSecond.text = sec.ToString("00");

        if (diffSecond >= _maxCoolTimeSecond)
        {
            int numCharge = diffSecond / (int)_maxCoolTimeSecond;
            int addMin = _coolTimeMinute * numCharge;
            DatabaseManager._myDatas.stamina += numCharge;
            DateTime d = DateTime.Parse(_lastDate);
            d = d.AddMinutes(addMin);
            _lastDate = d.ToString();

            if (!_isAlreadyUpdate && DatabaseManager._myDatas.stamina >= _maxStamina)
            {
                DatabaseManager._myDatas.stamina = _maxStamina;
                _isAlreadyUpdate = true;
                yFullStamina(true);
            }

            if (DatabaseManager._myDatas.stamina < _maxStamina)
            {
                DatabaseManager.xInstance.zSetUseStamina(_lastDate);
            }
            _txtCurrStamina.text = DatabaseManager._myDatas.stamina.ToString();
        }
    }
}
