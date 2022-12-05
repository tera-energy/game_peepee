using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;



public class TrPuzzlePenguin : TrPuzzleManager
{

    static TrPuzzlePenguin _instance;
    public static TrPuzzlePenguin xInstance { get { return _instance; } }


    bool _isTimeTenseEffectExec;
    bool _isOnVibration;

    [HideInInspector] public bool _goTimeTense = true;




    int _checkRunCount = 0;

    TT.enumButtonInput _playerMoving = TT.enumButtonInput.Neutral;
    public TT.enumButtonInput xPlayerMovingDir { get { return _playerMoving; } }





    [HideInInspector] public bool _gameStop;
    [SerializeField] Rigidbody2D _rigi;
    [SerializeField] float _playerMaxSpeed;
    public float _peeJumpForce;
    [SerializeField] public GameObject _PeePee;
    bool _canMove = true;
    [HideInInspector] public int _jumpCondition;
    [HideInInspector] public bool _currJumping = false;
    bool _moveLock;


    /*[SerializeField] ParticleSystem _spitFish;
    [SerializeField] ParticleSystem _stunStar;
    [SerializeField] ParticleSystem _parTakeFishEffect;
    [SerializeField] ParticleSystem _parHitSad;
    [SerializeField] GameObject _HitStun;
    [SerializeField] GameObject _takeFishEffect;*/
    public int _num = 0;
    [HideInInspector] public bool _fishTrigger;
    int _countScore = 1;

    
    void yHitMove()
    {
        Transform peepeeTf = _PeePee.transform;
        if (peepeeTf.localPosition.x < 0)
        {
            peepeeTf.Translate(1f * Time.unscaledDeltaTime, 1f * Time.unscaledDeltaTime, 0);
        }
        else if (peepeeTf.localPosition.x > 0)
        {
            peepeeTf.Translate(-1f * Time.unscaledDeltaTime, 1f * Time.unscaledDeltaTime, 0);
        }
        else if (peepeeTf.localPosition.x == 0)
        {
            peepeeTf.Translate(0, 1f * Time.unscaledDeltaTime, 0);
        }

    }
    public void zPeePeeJump()
    {
        if (_jumpCondition < 1 && _gameStop == false)
        {
            if (TrUI_PuzzlePenguin.xInstance._jumpClick == true || Input.GetButtonDown("Jump"))
            {
                _rigi.AddForce(Vector3.up * _peeJumpForce, ForceMode2D.Impulse);
                _jumpCondition = 1;
                if (TrUI_PuzzlePenguin.xInstance._leftMove == true)
                {
                    _rigi.AddForce(Vector3.left * 2, ForceMode2D.Impulse);
                }
                else if (TrUI_PuzzlePenguin.xInstance._rightMove == true)
                {
                    _rigi.AddForce(Vector3.right * 2, ForceMode2D.Impulse);
                }
            }

        }
    }

    //점프 조건
    void yPeePeeJump()
    {
        float _currPeePosY = _PeePee.transform.localPosition.y;
        float _currPeePosX = _PeePee.transform.localPosition.x;
        if (_currPeePosY < -2.9f)
        {
            _jumpCondition = 0;
            _currJumping = false;
            _canMove = true;
        }
        else
        {
            _jumpCondition = 1;
            _currJumping = true;
            _canMove = false;
        }
        if (Player.xInstance._peepeeHit == true)
        {
            TrUI_PuzzlePenguin.xInstance._jumpClick = false;
        }

    }


    //피피이동
    #region
    void yPeePeeMove()
    {
        if (_canMove == true)
        {
            if ((TrUI_PuzzlePenguin.xInstance._leftMove == true && _currJumping == false) && _gameStop == false)
            {
                _rigi.velocity = new Vector2(_playerMaxSpeed * (-1), _rigi.velocity.y); //왼쪽
            }
            if ((TrUI_PuzzlePenguin.xInstance._rightMove == true && _currJumping == false) && _gameStop == false)
            {
                _rigi.velocity = new Vector2(_playerMaxSpeed, _rigi.velocity.y); //오른쪽
            }
        }
    }
    #endregion
    void yStopSpeed()
    {
        if (Input.GetButtonUp("Horizontal"))
        {
            if (_rigi.velocity.y > _peeJumpForce)
            {
                _rigi.velocity = new Vector2(_rigi.velocity.normalized.x * 0.5f, _rigi.velocity.y);
            }
        }
    }
    void yPeePeeJumpTest()
    {
        float _currPeePosY = _PeePee.transform.localPosition.y;
        if (_currPeePosY < -2.9f)
        {
            _jumpCondition = 0;
            _currJumping = false;
            _canMove = true;
        }
        else
        {
            _jumpCondition = 1;
            _canMove = false;
            _currJumping = true;
        }

        if (Input.GetButtonDown("Jump") && _jumpCondition == 0)
        {
            _rigi.AddForce(Vector3.up * _peeJumpForce, ForceMode2D.Impulse);
        }
    }
    void yPeePeeMoveTest()
    {
        float axis = Input.GetAxisRaw("Horizontal");
        if (_canMove == true)
        {
            _rigi.AddForce(Vector2.right * axis, ForceMode2D.Impulse);
            if (_rigi.velocity.x > _playerMaxSpeed)
                _rigi.velocity = new Vector2(_playerMaxSpeed, _rigi.velocity.y); //오른쪽
            else if (_rigi.velocity.x < _playerMaxSpeed * (-1))
                _rigi.velocity = new Vector2(_playerMaxSpeed * (-1), _rigi.velocity.y); //왼쪽
        }
    }
    IEnumerator yPoint()
    {
        while (true)
        {
            yield return TT.WaitForSeconds(1f);
            zCorrect(false, _countScore);
        }

    }
    IEnumerator yFish()
    {
        //물고기 마다 점수 다르게, 물고기 없어지게
        while (GameManager.xInstance._isGameStarted == true)
        {
            yield return TT.WaitForSeconds(Obstacle.xInstance._obstacleTerm);

            _num++;
            if (_num > 9)
            {
                _num = 0;
            }

        }
    }

    IEnumerator yExecTicTok()
    {
        while (_currGameTime > 0 && _currGameTime < 20)
        {
            TrAudio_UI.xInstance.zzPlay_TimerTicTok(0.1f);
            yield return TT.WaitForSeconds(0.5f);
        }
    }
    IEnumerator yRunCheck()
    {
        while (true)
        {
            //yObstacleSpeed(); 필요
            yield return TT.WaitForSeconds(1f);
            _checkRunCount++;
        }
    }
    void yGameLevel()
    {
        if (_checkRunCount < 5)
        {
            Obstacle.xInstance._obstacleTerm = 4f;
            Obstacle.xInstance._scaleSpeed = 5f;
            Obstacle.xInstance._obstacleSpeed = 9f;
            Obstacle.xInstance._itemTagTerm = 0.7f;
            _playerMaxSpeed = 4f;
            _countScore = 5;
        }
        else if (_checkRunCount >= 5 && _checkRunCount < 20)
        {
            Obstacle.xInstance._obstacleTerm = 3f;
            Obstacle.xInstance._scaleSpeed = 4.5f;
            Obstacle.xInstance._obstacleSpeed = 8f;
            Obstacle.xInstance._itemTagTerm = 0.5f;
            _playerMaxSpeed = 4.3f;
            _countScore = 10;
        }
        else if (_checkRunCount >= 20 && _checkRunCount < 30)
        {
            Obstacle.xInstance._obstacleTerm = 2.5f;
            Obstacle.xInstance._scaleSpeed = 4f;
            Obstacle.xInstance._obstacleSpeed = 6.5f;
            Obstacle.xInstance._itemTagTerm = 0.3f;
            _playerMaxSpeed = 4.5f;
            _countScore = 15;
        }
        else if (_checkRunCount >= 30 && _checkRunCount < 40)
        {
            Obstacle.xInstance._obstacleTerm = 2f;
            Obstacle.xInstance._scaleSpeed = 3.5f;
            Obstacle.xInstance._obstacleSpeed = 6f;
            Obstacle.xInstance._itemTagTerm = 0f;
            _playerMaxSpeed = 4.7f;
            _countScore = 20;
        }
        else if (_checkRunCount >= 40)
        {
            Obstacle.xInstance._obstacleTerm = 2f;
            Obstacle.xInstance._scaleSpeed = 3f;
            Obstacle.xInstance._obstacleSpeed = 5f;
            Obstacle.xInstance._itemTagTerm = 0f;
            _playerMaxSpeed = 5f;
            _countScore = 25;
        }
    }


    void FixedUpdate()
    {

        if (GameManager.xInstance._isGameStarted == true)
        {
            yPeePeeMoveTest();
            yPeePeeJumpTest();
            if (_moveLock == false)
            {
                //캐릭터 이동
                yPeePeeMove();
                //캐릭터 점프
            }
        }

    }

    

    public void zQuitApplication()
    {
#if PLATFORM_ANDROID
        Application.Quit();
#endif
    }


    protected override void yBeforeReadyGame()
    {
        base.yBeforeReadyGame();
        TrAudio_Music.xInstance.zzPlayMain(0.25f);
        _isOnVibration = PlayerPrefs.GetInt(TT.strConfigVibrate, 1) == 1 ? true : false;
        _isThridChallengeSame = true;
        PlayerAnim.xInstance._anim.SetBool("GameStart", true);
    }

    protected override void yAfterReadyGame()
    {
        base.yAfterReadyGame();
        _jumpCondition = 0;
        GameManager.xInstance._isGameStarted = true;
        TrUI_PuzzleNotice.xInstance._goPause = true;
        //yStartSet();

        Obstacle.xInstance.zInstantiateListObstacle();
        
        StartCoroutine(Obstacle.xInstance.zInitListObstacle());
        StartCoroutine(yRunCheck());
        StartCoroutine(yPoint());
        StartCoroutine(yFish());

    }

    protected override void Update()
    {   
        base.Update();
        if (!GameManager.xInstance._isGameStarted) return;   
        if (_currGameTime <= _maxGameTime * 0.25f && !_isTimeTenseEffectExec){
            _isTimeTenseEffectExec = true;
            TrUI_PuzzlePenguin.xInstance.zSetTimerTenseEffect();
            StartCoroutine(yExecTicTok());
        }

        if (GameManager.xInstance._isGameStarted == true)
        {
            if (Player.xInstance._peepeeHit == true)
            {
                _checkRunCount = 0;
            }
            
        }
        yPeePeeJump();
        yStopSpeed();
        //yGameLevel();
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


    

}