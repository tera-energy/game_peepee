using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;



public class TrPuzzlePeePee : TrPuzzleManager
{

    static TrPuzzlePeePee _instance;
    public static TrPuzzlePeePee xInstance { get { return _instance; } }


    bool _isTimeTenseEffectExec;
    bool _isOnVibration;

    [HideInInspector] public bool _goTimeTense = true;
    [HideInInspector] public bool _gameStop;

    [Header("피피")]
    [SerializeField] Rigidbody2D _rigi;
    public float _playerMaxSpeed;
    public float _peeJumpForce;
    [SerializeField] public GameObject _PeePee;
    [HideInInspector] public bool _canJump = true;
    [HideInInspector] public bool _canMove = true;
    [HideInInspector] public bool _currJumping = true;
    [SerializeField] GameObject _peepeeShadow;


    [Space]
    [Header("기본 장애물")]
    [SerializeField] PeePeeObstacle _infoObstacle;
    [SerializeField] int _obsMaxIndex;

    PeePeeObstacle[] _arrObstacle;
    List<PeePeeObstacle> _listObstacle = new List<PeePeeObstacle>();
    [HideInInspector] public int _obstacleType;
    float _obstacleFirPosX;
    Vector3 _obstacleFirPos;



    [Space]
    [Header("추가 장애물1")]
    [SerializeField] PeePeeAddObstacle _infoAddObstacle;
    PeePeeAddObstacle[] _arrAddObstacle;
    List<PeePeeAddObstacle> _listAddObstacle = new List<PeePeeAddObstacle>();
    float _addObstacleFirPosX;
    Vector3 _addObstacleFirPos;

    [Space]
    [Header("추가 장애물2")]
    [SerializeField] PeePeeAddObstacle1 _infoAdd1Obstacle;
    PeePeeAddObstacle1[] _arrAdd1Obstacle;
    List<PeePeeAddObstacle1> _listAdd1Obstacle = new List<PeePeeAddObstacle1>();
    float _add1ObstacleFirPosX;
    Vector3 _add1ObstacleFirPos;

    [Space]
    [Header("장애물 난이도")]
    float _scaleSpeed;
    float _obstacleMoveSpeed;
    float _obstacleTerm;
    float _nextHoleTerm;
    [HideInInspector] public float _itemTagTerm;

    [Space]
    [Header("아이템")]
    [SerializeField] Image _imgJumpBtn;
    [SerializeField] Sprite[] _spJumpBtn;
    [SerializeField] public GameObject _peepeeWing;
    [HideInInspector] public bool _isGetItem = false;
    [HideInInspector] public bool _isHeartFish = false;
    [HideInInspector] public bool _isBoosterFish = false;
    [HideInInspector] public bool _getItem;
    [HideInInspector] public bool _nextItem;
    [HideInInspector] public bool _fishRihgt;
    [HideInInspector] public bool _fishLeft;


    [Space]
    int _checkCount = 0;
    int _jumpNum = 1;
    [HideInInspector] public bool _btnAnim = false;
    float _km;
    [HideInInspector] public bool _isPeePeeEscape = false;
    [HideInInspector] public bool _isPeePeeEscapeTry = false;
    [HideInInspector] public bool _isSmallHole = false;

    [Space]
    [SerializeField] GameObject _removeObject;
    bool _add1Obstacle = false;
    List<List<Tween>> _liMoveObstacles = new List<List<Tween>>();
    bool _isNextObstacle = false;
    [HideInInspector] public float _objectSpeed;
    [HideInInspector] public float _fishScaleSpeed;
    [HideInInspector] public float _fishMoveXSpeed;
    Coroutine _setObstcleCo;
    float shadow = 1;
    bool _isDoubleJump = false;


    void yPeeShadow()
    {
        float peeY;
        float peeMinY;
        float peeMaxY;
        float Wtf;

        _peepeeShadow.transform.localPosition = new Vector3(_PeePee.transform.localPosition.x, -4.934f, 0);

        peeY = _PeePee.transform.localPosition.y + 5;
        peeMinY = -3.034f + 5;
        peeMaxY = -0.01f + 5;
        Wtf = (peeY - peeMinY) / (peeMaxY - peeMinY);

        _peepeeShadow.transform.localScale = new Vector3(1-Wtf, 1 - Wtf, 1-Wtf);

        if (Player.xInstance._isFall == true)
            _peepeeShadow.SetActive(false);
        else
            _peepeeShadow.SetActive(true);
    }

    void yGameLevel()
    {
        yCountCheck();
        if (_checkCount < 15)
        {
            _scaleSpeed = 2.4f;
            _obstacleMoveSpeed = 5f;
            _nextHoleTerm = 1.7f;
            _itemTagTerm = 1f;
            _objectSpeed = 4f;
            _fishMoveXSpeed = 6f;
            _fishScaleSpeed = 2.4f;
            _km = 2f;
        }
        else if (_checkCount >= 15 && _checkCount < 30)
        {
            _scaleSpeed = 2f;
            _obstacleMoveSpeed = 4f;
            _nextHoleTerm = 1.5f;
            _itemTagTerm = 0.7f;
            _objectSpeed = 5f;
            _fishMoveXSpeed = 7f;
            _fishScaleSpeed = 2.2f;
            _km = 4f;
        }
        else if (_checkCount >= 30)
        {
            _scaleSpeed = 1.7f;
            _obstacleMoveSpeed = 3f;
            _nextHoleTerm = 1.3f;
            _itemTagTerm = 0.5f;
            _objectSpeed = 6f;
            _fishMoveXSpeed = 4f;
            _fishScaleSpeed = 1.6f;
            _km = 6f;
        }


    }






    void ySetObstacle(int index, ref List<Tween> moves)
    {
        Vector2 obstacleFirScale = new Vector2(0f, 0f);
        Vector2 obstacleScale = new Vector2(2.5f, 2.3f);
        Vector2 obstacleScale2 = new Vector2(4f, 1.8f);
        int randSprite = Random.Range(0, 8);
        int randType0FirPosX = Random.Range(0, 3);

        Vector3 targetL = new Vector3(-11f, -14f, 0f);
        Vector3 targetR = new Vector3(11f, -14f, 0f);
        Vector3 targetM = new Vector3(0, -14f, 0f);
        _arrObstacle[index].tag = "Untagged";

        if (randSprite > 0)
        {
            PeePeeObstacle.xInstance._spriteRenderer.sprite = PeePeeObstacle.xInstance._spObstacle[0];
            _isSmallHole = true;
            _arrObstacle[index].tag = "Obstacle";
        }
        else if (randSprite == 0)
        {
            PeePeeObstacle.xInstance._spriteRenderer.sprite = PeePeeObstacle.xInstance._spObstacle[1];
            _isSmallHole = false;

            _arrObstacle[index].tag = "BigObstacle";
        }

        _arrObstacle[index]._spriteRenderer.sprite = PeePeeObstacle.xInstance._spriteRenderer.sprite;

        _arrObstacle[index].transform.localScale = obstacleFirScale;
        _listObstacle.Add(_arrObstacle[index]);



        if (_isSmallHole == false)
        {
            _obstacleFirPosX = 0f;
            moves.Add(_listObstacle[index].transform.DOScale(obstacleScale2, _scaleSpeed).SetEase(Ease.Linear));
            moves.Add(_listObstacle[index].transform.DOLocalMove(targetM, _obstacleMoveSpeed).SetEase(Ease.Linear));
        }
        else if (_isSmallHole == true)
        {
            if (randType0FirPosX == 0)
            {
                _obstacleFirPosX = -0.7f;
                _fishRihgt = true;
            }
            else if (randType0FirPosX == 1)
            {
                _obstacleFirPosX = 0f;
                int randFishMove = Random.Range(0, 2);
                if (randFishMove == 0)
                {
                    _fishRihgt = true;
                }else
                    _fishLeft = true;
            }

            else if (randType0FirPosX == 2)
            {
                _obstacleFirPosX = 0.7f;
                _fishLeft = true;
            }

            moves.Add(_listObstacle[index].transform.DOScale(obstacleScale, _scaleSpeed).SetEase(Ease.Linear));


        }

        if (_obstacleFirPosX == -0.7f)
        {
            moves.Add(_listObstacle[index].transform.DOLocalMove(targetL, _obstacleMoveSpeed).SetEase(Ease.Linear));
        }
        else if (_obstacleFirPosX == 0 && _isSmallHole == true)
        {
            moves.Add(_listObstacle[index].transform.DOLocalMove(targetM, _obstacleMoveSpeed).SetEase(Ease.Linear));
        }
        else if (_obstacleFirPosX == 0.7f)
        {
            moves.Add(_listObstacle[index].transform.DOLocalMove(targetR, _obstacleMoveSpeed).SetEase(Ease.Linear));
        }


        _obstacleFirPos = new Vector3(_obstacleFirPosX, 1.5f, 0);
        _arrObstacle[index].transform.localPosition = _obstacleFirPos;
        _arrObstacle[index].gameObject.SetActive(true);


    }
    void ySetAddObstacle(int index, ref List<Tween> moves)
    {
        Vector2 addObstacleFirScale = new Vector2(0f, 0f);
        Vector2 addObstacleScale = new Vector2(2.5f, 2.3f);

        int randAddObstaclePosX = Random.Range(0, 2);

        Vector3 targetL = new Vector3(-11f, -14f, 0f);
        Vector3 targetR = new Vector3(11f, -14f, 0f);
        Vector3 targetM = new Vector3(0, -14f, 0f);

        int holeNum = Random.Range(0, 3);

        _arrAddObstacle[index].tag = "Obstacle";
        _arrAddObstacle[index].transform.localScale = addObstacleFirScale;
        _listAddObstacle.Add(_arrAddObstacle[index]);

        _removeObject.transform.localScale = new Vector2(1f, 1f);

        if (_obstacleFirPosX == -0.7f)
        {
            switch (randAddObstaclePosX)
            {
                case 0:
                    _addObstacleFirPosX = 0f;
                    break;
                case 1:
                    _addObstacleFirPosX = 0.7f;
                    break;
            }
        }
        else if (_obstacleFirPosX == 0f)
        {
            switch (randAddObstaclePosX)
            {
                case 0:
                    _addObstacleFirPosX = -0.7f;
                    break;
                case 1:
                    _addObstacleFirPosX = 0.7f;
                    break;
            }
        }
        else if (_obstacleFirPosX == 0.7f)
        {
            switch (randAddObstaclePosX)
            {
                case 0:
                    _addObstacleFirPosX = -0.7f;
                    break;
                case 1:
                    _addObstacleFirPosX = 0f;
                    break;
            }
        }



        if (_addObstacleFirPosX == -0.7f)
        {
            moves.Add(_listAddObstacle[index].transform.DOLocalMove(targetL, _obstacleMoveSpeed).SetEase(Ease.Linear));
        }
        else if (_addObstacleFirPosX == 0f)
        {
            moves.Add(_listAddObstacle[index].transform.DOLocalMove(targetM, _obstacleMoveSpeed).SetEase(Ease.Linear));
        }
        else if (_addObstacleFirPosX == 0.7f)
        {
            moves.Add(_listAddObstacle[index].transform.DOLocalMove(targetR, _obstacleMoveSpeed).SetEase(Ease.Linear));
        }

        moves.Add(_listAddObstacle[index].transform.DOScale(addObstacleScale, _scaleSpeed).SetEase(Ease.Linear));
        _addObstacleFirPos = new Vector3(_addObstacleFirPosX, 1.5f, 0);
        _arrAddObstacle[index].transform.localPosition = _addObstacleFirPos;
        _arrAddObstacle[index].gameObject.SetActive(true);


        if (holeNum != 0 || _isSmallHole == false)
        {
            _arrAddObstacle[index]._spriteRenderer.sprite = null;
            _arrAddObstacle[index].GetComponent<BoxCollider2D>().enabled = false;
            _add1Obstacle = false;

        }
        else
            _add1Obstacle = true;
    }
    void ySetAdd1Obstacle(int index, ref List<Tween> moves)
    {
        Vector2 add1ObstacleFirScale = new Vector2(0f, 0f);
        Vector2 add1ObstacleScale = new Vector2(2.5f, 2.3f);

        int randAdd1ObstaclePosX = Random.Range(0, 2);

        Vector3 targetL = new Vector3(-11f, -14f, 0f);
        Vector3 targetR = new Vector3(11f, -14f, 0f);
        Vector3 targetM = new Vector3(0, -14f, 0f);

        _arrAdd1Obstacle[index].tag = "Obstacle";
        _arrAdd1Obstacle[index].transform.localScale = add1ObstacleFirScale;
        _listAdd1Obstacle.Add(_arrAdd1Obstacle[index]);

        int randAdd1obstacle = Random.Range(0, 3);

        if (_obstacleFirPosX == -0.7f)
        {
            if (_addObstacleFirPosX == 0f)
            {
                _add1ObstacleFirPosX = 0.7f;
            }
            else if (_addObstacleFirPosX == 0.7f)
            {
                _add1ObstacleFirPosX = 0f;
            }
        }
        else if (_obstacleFirPosX == 0f)
        {
            if (_addObstacleFirPosX == -0.7f)
            {
                _add1ObstacleFirPosX = 0.7f;
            }
            else if (_addObstacleFirPosX == 0.7f)
            {
                _add1ObstacleFirPosX = -0.7f;
            }
        }
        else if (_obstacleFirPosX == 0.7f)
        {
            if (_addObstacleFirPosX == -0.7f)
            {
                _add1ObstacleFirPosX = 0f;
            }
            else if (_addObstacleFirPosX == 0f)
            {
                _add1ObstacleFirPosX = -0.7f;
            }
        }





        if (_add1ObstacleFirPosX == -0.7f)
        {

            moves.Add(_listAdd1Obstacle[index].transform.DOLocalMove(targetL, _obstacleMoveSpeed).SetEase(Ease.Linear))/*.OnComplete(() => yRemoveObjectList()))*/;
        }
        else if (_add1ObstacleFirPosX == 0f)
        {
            moves.Add(_listAdd1Obstacle[index].transform.DOLocalMove(targetM, _obstacleMoveSpeed).SetEase(Ease.Linear));
        }
        else if (_add1ObstacleFirPosX == 0.7f)
        {
            moves.Add(_listAdd1Obstacle[index].transform.DOLocalMove(targetR, _obstacleMoveSpeed).SetEase(Ease.Linear));
        }

        moves.Add(_listAdd1Obstacle[index].transform.DOScale(add1ObstacleScale, _scaleSpeed).SetEase(Ease.Linear));

        _add1ObstacleFirPos = new Vector3(_add1ObstacleFirPosX, 1.5f, 0);
        _arrAdd1Obstacle[index].transform.localPosition = _add1ObstacleFirPos;
        _arrAdd1Obstacle[index].gameObject.SetActive(true);

        if (_isSmallHole == true)
        {
            if (_add1Obstacle == true || randAdd1obstacle == 0)
            {
                //Debug.Log("ㅇㅇ");
            }
            else
            {
                _arrAdd1Obstacle[index]._spriteRenderer.sprite = null;
                _arrAdd1Obstacle[index].GetComponent<BoxCollider2D>().enabled = false;
            }
        }
        else
        {
            _arrAdd1Obstacle[index]._spriteRenderer.sprite = null;
            _arrAdd1Obstacle[index].GetComponent<BoxCollider2D>().enabled = false;
        }

    }

    void ySetRemoveObject(int index)
    {
        GameObject removeObstacle = Instantiate(_removeObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
        removeObstacle.transform.SetParent(_arrObstacle[index].transform);
        removeObstacle.transform.localScale = new Vector3(1f, 1f, 1f);
        removeObstacle.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    public void yRemoveObjectList()
    {
        //Debug.Log("remove");
        _liMoveObstacles.RemoveAt(0);
    }
    public void yPause()
    {
        foreach (var a in _liMoveObstacles)
        {
            foreach (var b in a)
            {
                b.Pause();
            }
        }
    }

    public void yPlay()
    {
        StartCoroutine(yObstacleCount());     
        foreach (var a in _liMoveObstacles)
        {
            foreach (var b in a)
            {
                b.Play();
            }
        }
    }
    IEnumerator yInitObstacle()
    {

        int index = 0;

        while (GameManager.xInstance._isGameStarted)
        {
            
            yield return new WaitUntil(() => Player.xInstance._peepeeHit == false && Player.xInstance._isFall == false);
            //Debug.Log("생성");

            List<Tween> moves = new List<Tween>();
            ySetObstacle(index, ref moves);
            ySetAddObstacle(index, ref moves);
            ySetAdd1Obstacle(index, ref moves);

            _liMoveObstacles.Add(moves);


            if (_isSmallHole == true)
            {
                _arrObstacle[index].ySetItem();
            }
            else
            {
                _isSmallHole = false;
                _arrObstacle[index].ySetItem();
            }
            _add1Obstacle = false;
            yield return new WaitUntil(() => _isNextObstacle == true);
            _fishRihgt = false;
            _fishLeft = false;
            _obstacleTerm = 0f;
            _isNextObstacle = false;
            _arrObstacle[index].zResetTween();
            index++;
            if (index >= _obsMaxIndex)
            {
                index = 0;
            }

        }
    }

    void yInstantiateObstacle()
    {
        _arrObstacle = new PeePeeObstacle[_obsMaxIndex];
        _arrAddObstacle = new PeePeeAddObstacle[_obsMaxIndex];
        _arrAdd1Obstacle = new PeePeeAddObstacle1[_obsMaxIndex];
        for (int i = 0; i < _obsMaxIndex; i++)
        {
            PeePeeObstacle InsObstacle = Instantiate(_infoObstacle);
            PeePeeAddObstacle InsAddObstacle = Instantiate(_infoAddObstacle);
            PeePeeAddObstacle1 InsAdd1Obstacle = Instantiate(_infoAdd1Obstacle);

            _arrObstacle[i] = InsObstacle;
            _arrObstacle[i].gameObject.SetActive(false);

            _arrAddObstacle[i] = InsAddObstacle;
            _arrAddObstacle[i].gameObject.SetActive(false);

            _arrAdd1Obstacle[i] = InsAdd1Obstacle;
            _arrAdd1Obstacle[i].gameObject.SetActive(false);
        }
        _infoObstacle.gameObject.SetActive(false);
        _infoAddObstacle.gameObject.SetActive(false);
        _infoAdd1Obstacle.gameObject.SetActive(false);

        for (int i = 0; i < _obsMaxIndex; i++)
        {
            ySetRemoveObject(i);
        }
    }

    public void zGetItem(string type)
    {
        if (type == "Heart")
        {
            //Debug.Log("앙 하트 띠");
            _currGameTime += 5;
            _isHeartFish = false;
            _getItem = true;
        }
        else if (type == "Booster")
        {
            //Debug.Log("앙 부트터 띠");
            _jumpNum = 2;
            _peepeeWing.SetActive(true);
            _getItem = true;
        }
    }

    public void zJump()
    {
        if (_jumpNum == 2)
        {
            _currGameTime -= 1;
        }
    }

    void yBoosterCondition()
    {
        if (Player.xInstance._isFall == false)
        {
            if (_isBoosterFish == true)
            {
                if (_currJumping == true)
                {
                    _imgJumpBtn.sprite = _spJumpBtn[2];
                    _canJump = true;
                }
                else if (_currJumping == false)
                {
                    _imgJumpBtn.sprite = _spJumpBtn[0];
                }
            }
            else if (_isBoosterFish == false)
            {
                if (_currJumping == true)
                {
                    _imgJumpBtn.sprite = _spJumpBtn[1];
                    _btnAnim = false;
                    _canJump = false;
                }
                else if (_currJumping == false)
                {
                    _imgJumpBtn.sprite = _spJumpBtn[0];
                }
            }
        }
        if (Player.xInstance._isFall == true && Player.xInstance._btnLock == true)
        {
            _imgJumpBtn.sprite = _spJumpBtn[1];
            _canJump = false;
            _btnAnim = false;
        }
        else if (Player.xInstance._isFall == true && Player.xInstance._btnLock == false)
        {
            _imgJumpBtn.sprite = _spJumpBtn[0];
            _btnAnim = true;
        }
        if (Player.xInstance._peepeeHit == true)
        {
            _imgJumpBtn.sprite = _spJumpBtn[1];
            _canJump = false;
            _btnAnim = false;
        }

    }

    void yPeePeeEscape()
    {
        if (TrUI_PuzzlePeePee.xInstance._peepeeEscapeJump == 3)
        {
            Player.xInstance._isFall = false;
            _isPeePeeEscape = true;
        }
        else
            _isPeePeeEscape = false;
    }



    void yCountCheck()
    {
        if (GameManager.xInstance._isGameStarted == true)
        {
            if (Player.xInstance._peepeeHit == true || Player.xInstance._isFall == true)
            {
                _checkCount = 0;
            }
        }
    }

    IEnumerator yRunCheck()
    {
        while (true)
        {
            yield return TT.WaitForSeconds(1f);
            _checkCount++;
        }

    }

    IEnumerator yKmPointCheck()
    {
        while (true)
        {
            yield return new WaitUntil(() => Player.xInstance._peepeeHit == false && Player.xInstance._isFall == false);
            yield return TT.WaitForSeconds(0.5f);
            zCorrect(false, (int)_km);
        }
    }
    public IEnumerator yPeePeeHitMove(Vector2 targetPos)
    {
        
        _rigi.velocity = new Vector2(_playerMaxSpeed * 0, _rigi.velocity.y);
        int move = transform.localPosition.x - targetPos.x > 0 ? 1 : -1;
        _rigi.AddForce(new Vector2(move, 1f) * 3, ForceMode2D.Impulse);
        yield return TT.WaitForSeconds(0.6f);
        _rigi.AddForce(new Vector2(move, 1f) * 1, ForceMode2D.Impulse);
    }


    void yPeePeeMove()
    {
        if (_canMove == true)
        {
            if (TrUI_JoyStick.xInstance._isLeft == true)
            {
                _rigi.velocity = new Vector2(_playerMaxSpeed * (-1), _rigi.velocity.y); //왼쪽           
            }
            else if (TrUI_JoyStick.xInstance._isRight == true)
            {
                _rigi.velocity = new Vector2(_playerMaxSpeed, _rigi.velocity.y); //오른쪽
            }
        }


    }


    public void zPeePeeJump()
    {
        _jumpNum++;
        if (_canJump == true && (Player.xInstance._isFall == false || Player.xInstance._peepeeHit == false))
        {

            _rigi.AddForce(Vector3.up * _peeJumpForce, ForceMode2D.Impulse);
            if (TrUI_JoyStick.xInstance._isLeft == true)
            {
                _rigi.AddForce(Vector2.left * _peeJumpForce, ForceMode2D.Impulse);
            }
            else if (TrUI_JoyStick.xInstance._isRight == true)
            {
                _rigi.AddForce(Vector2.right * _peeJumpForce, ForceMode2D.Impulse);
            }
        }

    }

    void yPeePeeJumpCondition()
    {
        if (_PeePee.transform.localPosition.y < -3f)
        {
            _currJumping = false;
            _canJump = true;
            _jumpNum = 1;
            _btnAnim = true;
        }
        else
        {
            
            _currJumping = true;
            _canJump = false;

            if (_isBoosterFish == true)
            {
                _canJump = true;
            }
        }

        if (_currJumping == true && _jumpNum == 1)
        {
            _jumpNum = 2;
        }

        if (_jumpNum == 3)
        {
            _isBoosterFish = false;
            _peepeeWing.SetActive(false);
            _isDoubleJump = true;
        }

        if (Input.GetButtonDown("Jump") && _canJump == true)
        {
            _rigi.AddForce(Vector3.up * _peeJumpForce, ForceMode2D.Impulse);
        }

    }

    void FixedUpdate()
    {
        if (GameManager.xInstance._isGameStarted != false)
        {
            yPeePeeMove();
            //피피 애니메이션 멈춤, 콜라이더 없이(태그 없이)
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

    IEnumerator yObstacleCount()
    {
        while (Player.xInstance._peepeeHit == false && Player.xInstance._isFall == false)
        {
            yield return TT.WaitForSeconds(0.1f);
            _obstacleTerm = _obstacleTerm + 0.1f;
            if (_obstacleTerm >= _nextHoleTerm)
            {
                _isNextObstacle = true;
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
        GameManager.xInstance._isGameStarted = true;
        TrUI_PuzzleNotice.xInstance._goPause = true;

        yInstantiateObstacle();
        StartCoroutine(yInitObstacle());

        StartCoroutine(yRunCheck());
        StartCoroutine(yKmPointCheck());
        _setObstcleCo = StartCoroutine(yObstacleCount());

        StartCoroutine(PeePeeObject.xInstance.yBearAndSeal());

    }
 
    protected override void Update()
    {
        base.Update();
        if (!GameManager.xInstance._isGameStarted) return;
        if (_currGameTime <= _maxGameTime * 0.25f && !_isTimeTenseEffectExec)
        {
            _isTimeTenseEffectExec = true;

            TrUI_PuzzlePeePee.xInstance.zSetTimerTenseEffect();
            StartCoroutine(yExecTicTok());

        }

        if (_currJumping == true)
        {
            _canJump = false;
        }

        yPeePeeEscape();

        yGameLevel();

        yPeePeeJumpCondition();

        yBoosterCondition();

        yPeeShadow();

        if (Player.xInstance._isFall == true)
        {
            _PeePee.transform.localPosition = new Vector3(0, _PeePee.transform.localPosition.y, 0);
            if (_currJumping == true)
            {
                _rigi.velocity = Vector3.zero;
            }
        }

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