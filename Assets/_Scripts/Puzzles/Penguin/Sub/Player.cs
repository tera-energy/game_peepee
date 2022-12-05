using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Player : MonoBehaviour
{
    
    static Player _instance;

    public static Player xInstance { get { return _instance; } }
    [SerializeField] GameObject _exo;
    [SerializeField] ParticleSystem _spitFish;
    [HideInInspector] public bool _peepeeHit = false;
    [HideInInspector] public bool _isFall = false;
    [SerializeField] ParticleSystem _stunStar;
    [SerializeField] ParticleSystem _parTakeFishEffect;
    [SerializeField] ParticleSystem _parHitSad;
    [SerializeField] ParticleSystem _parBooster;
    [SerializeField] ParticleSystem _parPowerUp;
    [SerializeField] ParticleSystem _parTakePower;
    [SerializeField] ParticleSystem _parTakeBooster;
    [SerializeField] ParticleSystem _parLeftFallW;
    [SerializeField] ParticleSystem _parRightFallW;
    public GameObject _HitStun;

    [SerializeField] SpriteRenderer _body;
    [SerializeField] SpriteRenderer _peeLA;
    [SerializeField] SpriteRenderer _peeRA;
    [SerializeField] SpriteRenderer _peeLF;
    [SerializeField] SpriteRenderer _peeRF;
    [SerializeField] SpriteRenderer _peeWing;


    [HideInInspector] public bool _isObstacleTag = false;
    [HideInInspector] public bool _jumpKey = false;
    
    [HideInInspector] public bool _btnLock = false;

    Coroutine _coEscapeCo;
    Coroutine _fallCo;

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.gameObject.CompareTag("Obstacle") && this.tag == "Player")
        {
            if (TrPuzzlePeePee.xInstance._currJumping == true)
            {
                //Debug.Log("구멍을 점프해서 통과");
            }
            else if (TrPuzzlePeePee.xInstance._currJumping == false)
            {
                _peepeeHit = true;
                _HitStun.SetActive(true);
                _spitFish.Play();
                _stunStar.Play();
                _parHitSad.Play();
                TrPuzzlePeePee.xInstance.yPause();
                TrPuzzlePeePee.xInstance._canMove = false;

                
                StartCoroutine(TrPuzzlePeePee.xInstance.yPeePeeHitMove(other.transform.position));
                this.tag = "Untagged";
                _isObstacleTag = true;
                StartCoroutine(yHit());
                //Time.timeScale = 0;
                TrAudio_SFX.xInstance.zzPlayFallHole();
            }
        }
        else if (other.gameObject.CompareTag("BigObstacle") && this.tag == "Player")
        {
            if (TrPuzzlePeePee.xInstance._currJumping == true)
            {
                //Debug.Log("큰 구멍을 점프해서 통과");
            }
            else if (TrPuzzlePeePee.xInstance._currJumping == false)
            {
                _isFall = true;
                TrPuzzlePeePee.xInstance._canMove = false;
                float otherPosX = other.transform.position.x;
                this.transform.localPosition = new Vector3(otherPosX, -3.034f, 0);
                _parLeftFallW.Play();
                _parRightFallW.Play();
                _parHitSad.Play();
                TrPuzzlePeePee.xInstance.yPause();
                this.tag = "Untagged";
                _isObstacleTag = true;
                //Time.timeScale = 0;
                _fallCo = StartCoroutine(yFall());
                TrAudio_SFX.xInstance.zzPlayFallHole();

            }
            
        }
        else if (other.gameObject.CompareTag("Heart") && this.tag == "Player")
        {
            //TrPuzzlePeePee.xInstance._isHeartFish = true;
            other.transform.gameObject.SetActive(false);
            TrPuzzlePeePee.xInstance.zGetItem("Heart");
            _parPowerUp.Play();
            _parTakePower.Play();
            TrAudio_SFX.xInstance.zzPlayGetFish(0f);
            TrPuzzlePeePee.xInstance._isHeartFish = true;
        }
        else if (other.gameObject.CompareTag("Booster") && this.tag == "Player")
        {
            //TrPuzzlePeePee.xInstance._isBoosterFish = true;
            other.transform.gameObject.SetActive(false);
            TrPuzzlePeePee.xInstance.zGetItem("Booster");
            _parBooster.Play();
            _parTakeBooster.Play();
            TrAudio_SFX.xInstance.zzPlayGetFish(0f);
            TrPuzzlePeePee.xInstance._isBoosterFish = true;
        }
    }
    
    
    IEnumerator yHit()
    {
        TrPuzzlePeePee.xInstance._gameStop = true;
        yield return TT.WaitForSeconds(2f);
        
        _spitFish.Stop();
        _stunStar.Stop();
        _parHitSad.Stop();
        _HitStun.SetActive(false);
        _peepeeHit = false;

        
        TrPuzzlePeePee.xInstance.yPlay();
        TrPuzzlePeePee.xInstance._canMove = true;
        TrPuzzlePeePee.xInstance._gameStop = false;

        yield return TT.WaitForSeconds(1.5f);
        _isObstacleTag = false;
        this.tag = "Player";
    }
    
    IEnumerator yFall()
    {
        TrPuzzlePeePee.xInstance._gameStop = true;
        for (int i = 0; i < 6; i++)
        {
            TrPuzzlePeePee.xInstance._PeePee.transform.GetChild(0).GetChild(i).GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }

        yield return new WaitUntil(() => _isFall == false);


        TrUI_PuzzlePeePee.xInstance._peepeeEscapeJump = 0;
        _jumpKey = false;

        _isFall = false;

        TrPuzzlePeePee.xInstance._canMove = true;
        
        TrPuzzlePeePee.xInstance.yPlay();
        

        _parLeftFallW.Stop();
        _parRightFallW.Stop();
        _parHitSad.Stop();
        for (int i = 0; i < 6; i++)
        {
            TrPuzzlePeePee.xInstance._PeePee.transform.GetChild(0).GetChild(i).GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
        }
        TrPuzzlePeePee.xInstance._gameStop = false;

        _coEscapeCo = StartCoroutine(yPeePeeEscapeEffect());
        yield return TT.WaitForSeconds(1.5f);

        this.tag = "Player";
        _isObstacleTag = false;
    }

    IEnumerator yPeePeeEscapeEffect()
    {
        int countTime = 0;

        while (countTime < 8)
        {
            if (countTime % 2 == 0)
            {
                _body.color = new Color32(255, 255, 255, 0);
                _peeLA.color = new Color32(255, 255, 255, 0);
                _peeLF.color = new Color32(255, 255, 255, 0);
                _peeRA.color = new Color32(255, 255, 255, 0);
                _peeRF.color = new Color32(255, 255, 255, 0);
                _peeWing.color = new Color32(255, 255, 255, 0);
            }
            else
            {
                _body.color = new Color32(255, 255, 255, 255);
                _peeLA.color = new Color32(255, 255, 255, 255);
                _peeLF.color = new Color32(255, 255, 255, 255);
                _peeRA.color = new Color32(255, 255, 255, 255);
                _peeRF.color = new Color32(255, 255, 255, 255);
                _peeWing.color = new Color32(255, 255, 255, 255);
            }

                

            yield return TT.WaitForSeconds(0.2f);
            countTime++;
        }
        _body.color = new Color32(255, 255, 255, 255);
        _peeLA.color = new Color32(255, 255, 255, 255);
        _peeLF.color = new Color32(255, 255, 255, 255);
        _peeRA.color = new Color32(255, 255, 255, 255);
        _peeRF.color = new Color32(255, 255, 255, 255);
        _peeWing.color = new Color32(255, 255, 255, 255);
    }


    void Update()
    {
        if (TrUI_PuzzleNotice.xInstance._isGameOver == true)
        {
            this.tag = "Untagged";
            _parLeftFallW.Stop();
            _parRightFallW.Stop();
            _spitFish.Stop();
            _stunStar.Stop();
            _parHitSad.Stop();

            if (_coEscapeCo != null)
            {
                StopCoroutine(yPeePeeEscapeEffect());
            }
            if (_fallCo != null)
            {
                StopCoroutine(yFall());
            }
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        
    }
}

