using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class TrUI_ResultManager : MonoBehaviour
{
    static TrUI_ResultManager _instance;
    [SerializeField] TextMeshProUGUI _scoreTxt;
    [SerializeField] TextMeshProUGUI _ribText;
    [SerializeField] CanvasGroup _imgFade;
    [SerializeField] RectTransform[] _iceFish;
    [SerializeField] Image[] _imgFish;
    [SerializeField] Sprite _fish;
    [SerializeField] ParticleSystem[] _parBrokenIce;
    [SerializeField] ParticleSystem _snow;
    [SerializeField] ParticleSystem _snowBlo;

    [SerializeField] ParticleSystem _dust;
    [SerializeField] RectTransform _newRecord;
    [SerializeField] RectTransform _bestScore;
    [SerializeField] Animator[] _bestScoreAnim;


    [SerializeField] AudioClip _acGood;
    [SerializeField] AudioClip _acBad;
    [SerializeField] AudioClip _acGreat;


    [SerializeField] TextMeshProUGUI _txtCurrStamina;
    [SerializeField] TrUI_HoldButton _btnRetry;

    bool _isSetScoreCom = false;
    bool _isSkipScoreEffect = false;

    [Space]
    [SerializeField] Animator _animBear;
    [SerializeField] Animator _animFishing;

    [Space]
    [SerializeField] RectTransform _pinkFish;
    [SerializeField] ParticleSystem _parPinkFishJump;
    [SerializeField] ParticleSystem _parPinkFishGoBack;
    [Space]
    [SerializeField] RectTransform _greenFish;
    [SerializeField] ParticleSystem _parGreenFishJump;
    [SerializeField] ParticleSystem _parGreenFishGoBack;
    [Space]
    [SerializeField] Sprite[] _bucketFisgSp;
    [SerializeField] Image _bucketFishImg;
    [SerializeField] RectTransform _bucketFish;
    public static TrUI_ResultManager xInstance { get { return _instance; } }

    IEnumerator yPinkFishJump()
    {
        while (true)
        {
            yield return TT.WaitForSeconds(3f);
            _pinkFish.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            _pinkFish.transform.DOLocalMoveY(60f, 1f).SetEase(Ease.OutQuad);
            _parPinkFishJump.Play();
            yield return TT.WaitForSeconds(1.2f);
            _pinkFish.transform.rotation = Quaternion.Euler(0f, 180f, -90f);
            _pinkFish.transform.DOLocalMoveY(-140f, 0.7f).SetEase(Ease.InQuad);
            yield return TT.WaitForSeconds(0.7f);
            _parPinkFishGoBack.Play();
        }
    }
    IEnumerator yGreenFishJump()
    {
        while (true)
        {
            yield return TT.WaitForSeconds(4f);
            _greenFish.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            _greenFish.transform.DOLocalMoveY(60f, 1f).SetEase(Ease.OutQuad);
            yield return TT.WaitForSeconds(0.2f);
            _parGreenFishJump.Play();
            yield return TT.WaitForSeconds(0.6f);
            _greenFish.transform.rotation = Quaternion.Euler(0f, 180f, 90f);
            _greenFish.transform.DOLocalMoveY(-140f, 0.7f).SetEase(Ease.InQuad);
            yield return TT.WaitForSeconds(0.5f);
            _parGreenFishGoBack.Play();
        }
        
    }

    IEnumerator yBucketFish()
    {
        while (true)
        {
            int randFish = Random.Range(0, 3);
            _bucketFish.transform.rotation = Quaternion.Euler(0, 0, 0f);
            yield return TT.WaitForSeconds(1f);
            if (randFish == 0)
            {
                _bucketFishImg.sprite = _bucketFisgSp[0];
            }else if (randFish == 1)
            {
                _bucketFishImg.sprite = _bucketFisgSp[1];
            }
            else
            {
                _bucketFishImg.sprite = _bucketFisgSp[2];
            }

            _bucketFish.DOLocalMoveY(-35f, 1f).SetEase(Ease.OutQuad);
            yield return TT.WaitForSeconds(1f);
            _bucketFish.transform.rotation = Quaternion.Euler(0, 180f, 0);
            yield return TT.WaitForSeconds(0.5f);
            _bucketFish.transform.rotation = Quaternion.Euler(0, 0f, 0);
            yield return TT.WaitForSeconds(0.5f);
            _bucketFish.transform.rotation = Quaternion.Euler(0, 180f, 0);
            yield return TT.WaitForSeconds(0.5f);
            _bucketFish.transform.rotation = Quaternion.Euler(0, 0f, 0);
            yield return TT.WaitForSeconds(0.5f);
            if (_bucketFishImg.sprite == _bucketFisgSp[0] || _bucketFishImg.sprite == _bucketFisgSp[2]) 
            _bucketFish.transform.rotation = Quaternion.Euler(0, 0, -90f);
            else
            _bucketFish.transform.rotation = Quaternion.Euler(0, 0, 90f);


            _bucketFish.DOLocalMoveY(-128f, 0.6f).SetEase(Ease.InQuad);
            yield return TT.WaitForSeconds(1f);
        }
    }

    public void yBtnExit()
    {
        TrAudio_UI.xInstance.zzPlay_ClickButtonSmall();
        GameManager.xInstance._state = TT.enumGameState.Main;
        _imgFade.DOFade(1, 0.5f).OnComplete(() => SceneManager.LoadScene(TT.strLOBBY));

    }
    public void yBtnRetry()
    {
        if(StaminaManager.zUseStamina(ref _txtCurrStamina)){
            TrAudio_UI.xInstance.zzPlay_ClickButtonSmall();
            _imgFade.DOFade(1, 0.5f).OnComplete(() => GameManager.xInstance.zSetPuzzleGame());
        }
    }

        IEnumerator yEffectIncreaseScore(TextMeshProUGUI text, float score, bool isScore){
        float maxScore = score;
        float currScore = maxScore / 2;
        float speed = maxScore - currScore;
        int soundScore = 1;
        while (currScore < maxScore){
            if (_isSkipScoreEffect)
            {
                text.text = ((int)maxScore).ToString();
                break;
            }
            currScore += Time.deltaTime * speed;
            if (isScore && currScore >= soundScore){
                while (currScore > soundScore)
                    soundScore++;

                if (speed > 20){
                    if ((int)speed % 3 == 0)
                        TrAudio_SFX.xInstance.zzPlay_AnimalsBomb();
                }
                else
                    TrAudio_SFX.xInstance.zzPlay_AnimalsBomb();
            }
            text.text = ((int)currScore).ToString();
            speed = (maxScore - currScore);
            if (speed < 1)
                speed = 1;
            yield return null;
        }
        text.text = score.ToString();

        if (isScore){
            yield return TT.WaitForSeconds(0.5f);
            _isSetScoreCom = true;
        }
    }


    IEnumerator ySetGameDatas()
    {
        int correctNum = GameManager.xInstance._correctNum;
        
        int score = GameManager.xInstance._numScore;
        int rank = -1;
        if (DatabaseManager._myDatas != null)
        {
            DatabaseManager.xInstance._isDataRead = true;
            DatabaseManager.xInstance.zGetDataMyScores();
            yield return new WaitUntil(() => !DatabaseManager.xInstance._isDataRead);
            int numRanks = DatabaseManager.xInstance._myScores.Count;
            for(int i=0; i<numRanks; i++){
                if (DatabaseManager.xInstance._myScores[i].maxScore <= rank){
                    rank = i;
                    break;
                }
            }
            DatabaseManager.xInstance._isDataRead = true;
            DatabaseManager.xInstance.zSetUserDataForScore(score);
            yield return new WaitUntil(() => !DatabaseManager.xInstance._isDataRead);
            if (score >= DatabaseManager._myDatas.maxScore)
            {
                DatabaseManager._myDatas.maxScore = score;
            }
        }
        GameManager._canBtnClick = true;



        _imgFade.DOFade(0, 1f);
        yield return new WaitUntil(() => _imgFade.alpha == 0);
        StartCoroutine(yEffectIncreaseScore(_scoreTxt, score, true));

        yield return new WaitUntil(() => _isSetScoreCom);
        Color color = Color.white;
        int Fish = 0;

        bool isNewRecord = rank == -1 ? false : true;
        bool isBestScore = rank == 0 ? true : false;
        if (isNewRecord)
        {
            if (isBestScore)
            {
                _bestScore.gameObject.SetActive(true);
            }
            else
                _newRecord.gameObject.SetActive(true);
            TrAudio_SFX.xInstance.zzPlayNewScore(0.1f);
            _dust.Play();
            yield return TT.WaitForSeconds(1f);
        }


        if (score <= 150)
        {
            Fish = 1;
            for (int i = 0; i < Fish; i++)
            {
                _iceFish[i].GetComponent<Image>().sprite = _fish;
                _imgFish[i].SetNativeSize();
                _parBrokenIce[i].Play();
                TrAudio_SFX.xInstance.zzPlayBurnCookie(0f);
                yield return TT.WaitForSeconds(1f);
            }
            _ribText.text = "BAD";
            ColorUtility.TryParseHtmlString("#FF6666", out color);
            TrAudio_UI.xInstance.zzPlay_WangWaWang(0f);
            TrAudio_Music.xInstance.zzPlayMain(1.7f, _acBad);
        }
        else if (score > 150 && score <= 300)
        {
            Fish = 2;
            for (int i = 0; i < Fish; i++)
            {
                _iceFish[i].GetComponent<Image>().sprite = _fish;
                _parBrokenIce[i].Play();
                _imgFish[i].SetNativeSize();
                TrAudio_SFX.xInstance.zzPlayBurnCookie(0f);
                yield return TT.WaitForSeconds(1f);
            }
            _ribText.text = "GOOD!";
            ColorUtility.TryParseHtmlString("#ffffff", out color);
            TrAudio_UI.xInstance.zzPlay_PangPang(1.2f);
            TrAudio_Music.xInstance.zzPlayMain(3f, _acGood);

            yield return TT.WaitForSeconds(1f);
        }
        else if (score > 300)
        {
            Fish = 3;
            for (int i = 0; i < Fish; i++)
            {
                _iceFish[i].GetComponent<Image>().sprite = _fish;
                _imgFish[i].SetNativeSize();
                _parBrokenIce[i].Play();
                TrAudio_SFX.xInstance.zzPlayBurnCookie(0f);
                yield return TT.WaitForSeconds(1f);
            }
            _ribText.text = "GREAT!!";
            ColorUtility.TryParseHtmlString("#fff04e", out color);
            _ribText.color = TT.zSetColor(TT.enumTrRainbowColor.PURPLE);
            TrAudio_UI.xInstance.zzPlay_GreatSound(0.5f);

            TrAudio_Music.xInstance.zzPlayMain(3.8f, _acGreat);
        }


        _ribText.color = color;
            _ribText.gameObject.SetActive(true);

            yield return TT.WaitForSeconds(10f);
        
    }
    void Awake(){
        if (_instance == null){
            _instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start(){
        GameManager._canBtnClick = false;
        GameManager.xInstance.zSetCamera();
        _scoreTxt.text = "0";
        _ribText.gameObject.SetActive(false);
        _imgFade.alpha = 1;
        if (DatabaseManager._myDatas != null){
            _txtCurrStamina.text = DatabaseManager._myDatas.stamina.ToString();
            if (DatabaseManager._myDatas.stamina <= 0)
                _btnRetry.zInteractDisable();
        }
        StartCoroutine(ySetGameDatas());
        StartCoroutine(yPinkFishJump());
        StartCoroutine(yGreenFishJump());
        StartCoroutine(yBucketFish());

        _animBear.SetBool("Bear", true);
        _animFishing.SetBool("Fishing", true);
        _snow.Play();
        _snowBlo.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isSkipScoreEffect = true;
        }
        
    }
}
