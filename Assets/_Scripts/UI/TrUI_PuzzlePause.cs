using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;
using TMPro;


public class TrUI_PuzzlePause : MonoBehaviour
{
    static TrUI_PuzzlePause _instance;
    public static TrUI_PuzzlePause xInstance { get { return _instance; } }

    [SerializeField] GameObject _goPause; // 게임 정지시 나오는 메뉴 게임 오브젝트
    [SerializeField] GameObject _goPauseRank; // 랭크시
    public CanvasGroup _fade;

    [SerializeField] TextMeshProUGUI _txtCurrStamina;
    [SerializeField] Button _btnRestart;

    /*[SerializeField] GameObject _goQuitButton;
    [SerializeField] GameObject _goQuitAskWindow;*/

    bool _isAction = false;
    public bool _isWinOpen = false;

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
            yInitFade();
        }
        else
        {
            _instance.yInitFade();
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (_goPause != null)
        {
            yActiveGoPause(false);
            Time.timeScale = 1;
            if (DatabaseManager._myDatas != null){
                _txtCurrStamina.text = DatabaseManager._myDatas.stamina.ToString();
                if (DatabaseManager._myDatas.stamina <= 0)
                    _btnRestart.interactable = false;
            }
            /*if (Application.platform != RuntimePlatform.Android)
                _goQuitButton.SetActive(false);
            else
                _goQuitButton.SetActive(true);*/
        }
    }

    void yInitFade()
    {
        _fade.alpha = 1;
        _fade.DOFade(0, 2f);
    }

    public void xAppearPauseWindow()
    {
        if (_isAction || !GameManager.xInstance._isGameStarted) return;
        TrAudio_UI.xInstance.zzPlay_Pause();
        yActiveGoPause(true);
        TrUI_PuzzleBtnPause.xInstance.zActivePause();
        
        PlayerAnim.xInstance._anim.GetComponent<Animator>().speed = 0;
        Time.timeScale = 0;

    }
    public void xDisappearPauseWindow()
    {
        if (_isAction) return;

        TrAudio_UI.xInstance.zzPlay_ClickNo();
        yActiveGoPause(false);
        TrUI_PuzzleBtnPause.xInstance.zDisablePause();
        
        PlayerAnim.xInstance._anim.GetComponent<Animator>().speed = 1;
        Time.timeScale = 1;

    }
    public void xClickRestart()
    {
        if (_isAction) return;
        _isAction = true;
        Time.timeScale = 1;
        if (StaminaManager.zUseStamina(ref _txtCurrStamina))
        {
            TrAudio_UI.xInstance.zzPlay_ClickButtonNormal();
            _fade.DOFade(1, 1f).OnComplete(() => GameManager.xInstance.zSetPuzzleGame());
        }
        else
        {
            _isAction = false;
            Time.timeScale = 0;
            
        }
    }

    public void zOnClickSetting()
    {
        TrAudio_UI.xInstance.zzPlay_ClickButtonNormal();
        TrUI_Window_Setting.xInstance.zShow(true);
    }

    public void xClickExit()
    {
        if (_isAction) return;
        _isAction = true;

        Time.timeScale = 1;
        TrAudio_UI.xInstance.zzPlay_ClickButtonNormal();
        GameManager.xInstance._isGameStarted = false;
        GameManager.xInstance._state = TT.enumGameState.Main;
        _fade.DOFade(1, 2f).OnComplete(() => SceneManager.LoadScene(TT.strLOBBY));
    }

    public void zGameEndAndSceneMove()
    {
        StartCoroutine(yEndGame());
    }

    void yActiveGoPause(bool active)
    {
        _isWinOpen = active;
        if (GameManager.xInstance._type != TT.enumPlayGameType.Rank)
        {
            _goPause.SetActive(active);
        }
        else
        {
            _goPauseRank.SetActive(active);
        }
    }

    IEnumerator yEndGame()
    {
        TrAudio_Music.xInstance.zStopMusic();
        TrAudio_UI.xInstance.zzPlay_GameOver1();
        TrAudio_UI.xInstance.zzPlay_GameOver2(2f);

        /* TT.UtilDelayedFunc.zCreate(() => TrAudio_UI.xInstance.zzPlay_GameOver2(),2f,null,true);
         yield return new WaitForSecondsRealtime(3f);
         _fade.DOFade(1, 1f).SetUpdate(true);*/
        yield return TT.WaitForSeconds(2.5f);
        _fade.DOFade(1, 2f);
        yield return TT.WaitForSeconds(2f);
        GameManager.xInstance._state = TT.enumGameState.Result;
        //yield return null;
        //SceneManager.LoadScene(GameManager.xInstance._comeSceneName);
        SceneManager.LoadScene("Result");
    }

    public void zActiveQuitAskWindow(bool isActive)
    {
        //_goQuitAskWindow.SetActive(isActive);
    }

    public void zQuitApplication()
    {
#if PLATFORM_ANDROID
        Application.Quit();
#endif
    }
}