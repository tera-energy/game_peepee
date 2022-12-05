using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    static public GameManager xInstance { get { return _instance; } }

    public TT.enumGameState _state;
    public bool _isGameStarted;
    public bool _isGameStopped;

    public TT.enumPlayGameType _type = TT.enumPlayGameType.Develop; // 게임 타입(캠패인, 연습, 랭크)
    public TT.enumDifficultyLevel _level; // 게임 레벨(쉬움, 노말, 어려움)
    public string _comeSceneName; // 게임 들어가기전 씬의 이름
    public string _playSceneName; // 플레이중인 게임 씬 이름
    public int _valuePlayTime;
    public int _numScore;
    public int _correctNum;
    public int _numMaxCombo;
    public static bool _canBtnClick = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void yResetDomainCodes()
    {
        _instance = null;
    }

    void Awake()
    {
        if (_instance == null)
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //_type = TT.enumPlayGameType.Develop;
        Application.targetFrameRate = 60;
    }

    public void zSetUIRect(ref RectTransform[] rts, RectTransform can = null)
    {

        int num = rts.Length;
        Rect rect = can.rect;
        float width = rect.width;
        float height = rect.height;
        for (int i = 0; i < num; i++)
        {
            rts[i].sizeDelta = new Vector2(width, height);
        }
    }

    public void zSetCamera()
    {
        Camera cam = Camera.main;
        Rect rect = cam.rect;
        float scaleHeight = ((float)Screen.width / Screen.height) / ((float)16 / 9);
        float scaleWidth = 1f / scaleHeight;

        if (scaleHeight < 1)
        {
            rect.height = scaleHeight;
            rect.y = (1f - scaleHeight) / 2f;
        }
        else
        {
            rect.width = scaleWidth;
            rect.x = (1f - scaleWidth) / 2f;
        }
        cam.rect = rect;
    }


    public void zSetPuzzleGame()
    {
        _state = TT.enumGameState.Play;
        _numScore = 0;
        _correctNum = 0;
        _level = TT.enumDifficultyLevel.Easy;
        string sceneName = TT.strPUZZLE;

        SceneManager.LoadScene(sceneName);
    }
}
