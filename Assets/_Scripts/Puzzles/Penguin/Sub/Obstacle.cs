using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;


public class Obstacle : Item
{
    static Obstacle _instance;
    public static Obstacle xInstance { get { return _instance; } }
    [SerializeField] Sprite[] _spObstacle;
    [SerializeField] SpriteRenderer _sprite;

    [Space]
    [Header("장애물")]
    [SerializeField] public Obstacle _obstacle;
    [SerializeField] Obstacle _infoObstacle;
    [SerializeField] int _maxIndex;
    [HideInInspector]public float _obstaclePosX;
    [HideInInspector]public bool _obstacleEx;
    [HideInInspector] public List<Obstacle> _listObstacle = new List<Obstacle>();
    
    Vector3 _obstacleFirPos;
    Vector3 _obstacleFirScale;
    
    Obstacle[] _arrObstacle;
    
    int _obstacleType;
    [HideInInspector] public int _obstacleRandPosX;

    [Space]
    [Header("장애물/추가 장애물 난이도 조정")]
    public float _scaleSpeed;
    public float _obstacleSpeed;
    public float _obstacleTerm;


    //장애물 조건, 정보
    void ySetListObstacle(int index)
    {
        Vector3 scale = new Vector3(1.3f, 1.3f, 1.3f);
        Vector3 tartgetL = new Vector3(-11f, -10f, 0f);
        Vector3 tartgetR = new Vector3(11f, -10f, 0f);
        Vector3 tartgetM = new Vector3(0, -10f, 0f);
        
        _listObstacle.Add(_arrObstacle[index]);
        _arrObstacle[index].tag = "Untagged";
        _obstacleType = Random.Range(0, 1);
        _obstacleRandPosX = Random.Range(0, 3);
        _obstacleFirScale = new Vector3(0f, 0f, 0f);


        if (_obstacleType == 0)
        {
            _arrObstacle[index].transform.localScale = _obstacleFirScale;
            if (_obstacleRandPosX == 0)
                _obstaclePosX = -0.7f;
            else if (_obstacleRandPosX == 1)
                _obstaclePosX = 0f;
            else if (_obstacleRandPosX == 2)
                _obstaclePosX = 0.7f;
        }
        _obstacleFirPos = new Vector3(_obstaclePosX, 1.45f, 0);
        _arrObstacle[index].transform.localPosition = _obstacleFirPos;
        _arrObstacle[index].gameObject.SetActive(true);
        _obstacleEx = true;

        
        //yListObstacleAct(index);
        if (_obstacleEx == true)
        {
            if (_obstaclePosX == 0)
            {
                _listObstacle[index].transform.DOScale(scale, _scaleSpeed).SetEase(Ease.Linear);
                _listObstacle[index].transform.DOLocalMove(tartgetM, _obstacleSpeed).SetEase(Ease.Linear);
            }
            else if (_obstaclePosX > 0)
            {
                _listObstacle[index].transform.DOScale(scale, _scaleSpeed).SetEase(Ease.Linear);
                _listObstacle[index].transform.DOLocalMove(tartgetR, _obstacleSpeed).SetEase(Ease.Linear);
            }
            else if (_obstaclePosX < 0)
            {
                _listObstacle[index].transform.DOScale(scale, _scaleSpeed).SetEase(Ease.Linear);
                _listObstacle[index].transform.DOLocalMove(tartgetL, _obstacleSpeed).SetEase(Ease.Linear);
            }
        }
            AddObstacle.xInstance.ySetListAddObstacle(index);


        
        //_arrObstacle[index].zSetObstacleItem();

    }

    


    void yTagObstacle()
    {
        if (Player.xInstance._peepeeHit == false)
        {
            if (_obstacle.transform.localPosition.y < -3.2f && _obstacle.transform.localPosition.y > -4.4f)
            {
                _obstacle.tag = "Obstacle";

            }    
        }
        else if (Player.xInstance._peepeeHit == true)
        {
                _obstacle.tag = "Untagged";
        }

        if (_obstacle.transform.localPosition.y < -3.6f)
        {
            _obstacle.tag = "Untagged";
        }

    }

    //구멍생성 및 초기화
    #region
    
    
    //구멍 초기화
    public IEnumerator zInitListObstacle()
    {
        int index = 0;
        while (GameManager.xInstance._isGameStarted)
        {
            ySetListObstacle(index);
            _arrObstacle[index].zSetObstacleItem();
            yield return TT.WaitForSeconds(_obstacleTerm);
            _arrObstacle[index].zResetTween();
            //Item.xInstance.zResetTween();
            _obstacleEx = true;
            index++;
            if (index >= _maxIndex)
            {
                index = 0;
            }
            
        }
    }


    //구멍 생성
    public void zInstantiateListObstacle()
    {
        _arrObstacle = new Obstacle[_maxIndex];
        AddObstacle.xInstance._arrAddObstacle = new AddObstacle[AddObstacle.xInstance._maxAddIndex];

        for (int i = 0; i < AddObstacle.xInstance._maxAddIndex; i++)
        {
                AddObstacle addObstacle = Instantiate(AddObstacle.xInstance._addInfoObstacle);
                AddObstacle.xInstance._arrAddObstacle[i] = addObstacle;
                AddObstacle.xInstance._arrAddObstacle[i].gameObject.SetActive(false);
        }
        
        for (int i = 0; i < _maxIndex; i++)
        {
            Obstacle InsObstacle = Instantiate(_infoObstacle);
            _arrObstacle[i] = InsObstacle;
            _arrObstacle[i].gameObject.SetActive(false);;
        }
        

        _infoObstacle.gameObject.SetActive(false);
        AddObstacle.xInstance._addInfoObstacle.gameObject.SetActive(false);

    }
    #endregion

    public void zObstacleSprite(int index)
    {
        _sprite.sprite = _spObstacle[index];

    }


    public void yListInfoReset()
    {
        _randItemSR.transform.localPosition = new Vector3(0f, -1f, 0f);
        _spriteMask.sprite = _spriteMaskSet;
        _randItemSR.sortingOrder = 99;
        _randItemSR.gameObject.SetActive(true);


        _obstacle.gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1.83f, 0.2f);
        _obstacle.gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0.1f);
        AddObstacle.xInstance._addObstacle.gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1.83f, 0.2f);
        AddObstacle.xInstance._addObstacle.gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0.1f);
    }


    public void zSetObstacleItem()
    {
        
        int randObs = Random.Range(0, 6);
        yListInfoReset();
        zObstacleSprite(0);
        if (randObs == 0)
        {
            _randItemSR.sprite = _randItemSP[0];
            _randItemSR.gameObject.tag = "Untagged";
            _obstacle.gameObject.GetComponent<BoxCollider2D>().size = new Vector2(2, 1.5f);
            _obstacle.gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0.6f);
            _randItemSR.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            _randItemSR.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            _isFish = false;
        }
        else if (randObs == 1)
        {
            _randItemSR.sprite = _randItemSP[1];
            _randItemSR.gameObject.tag = "Item";
            _randItemSR.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            _randItemSR.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
            _isFish = true;
        }
        else if (randObs == 2)
        {
            _randItemSR.sprite = _randItemSP[2];
            _randItemSR.gameObject.tag = "Item";
            _randItemSR.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            _randItemSR.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
            _isFish = true;
        }
        else if (randObs == 3)
        {
            _randItemSR.sprite = _randItemSP[3];
            _randItemSR.gameObject.tag = "Item";
            _randItemSR.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            _randItemSR.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
            _isFish = true;
        }
        else if (randObs == 4)
        {
            _randItemSR.sprite = _randItemSP[4];
            _randItemSR.gameObject.tag = "Item";
            _randItemSR.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            _randItemSR.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
            _isFish = true;

        }
        else
        {
            _randItemSR.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            _randItemSR.sprite = null;
            _isFish = false;
        }
        yItemMove();
    }
    void yItemMove()
    {
        float itemPosX = _randItemSR.transform.localPosition.x;

        float itemRandPosX = Random.Range(4f, 8f);
        if (_isFish == true)
        {
            if (_obstacle.transform.position.x < 0)
            {
                _inputDT = _randItemSR.transform.DOLocalMoveY(_fishJump, 0.5f).OnComplete(() => _randItemSR.transform.DOLocalMove(new Vector3(itemRandPosX, -5f, 0), _randItemSRDoPosXTerm).SetEase(Ease.OutQuad));
                _randItemSR.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), _itemSclaeTerm);
                _setItemDTList.Add(_inputDT);
            }
            else if (_obstacle.transform.position.x == 0)
            {
                int randPosX = Random.Range(0, 2);
                if (randPosX == 0)
                {
                    _inputDT = _randItemSR.transform.DOLocalMoveY(_fishJump, _randItemSRDoPosYTerm).OnComplete(() => _randItemSR.transform.DOLocalMove(new Vector3(itemPosX + itemRandPosX, -5f, 0), _randItemSRDoPosXTerm).SetEase(Ease.OutQuad));
                    _randItemSR.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), _itemSclaeTerm);

                    _setItemDTList.Add(_inputDT);
                }
                else
                {
                    _inputDT = _randItemSR.transform.DOLocalMoveY(_fishJump, _randItemSRDoPosYTerm).OnComplete(() => _randItemSR.transform.DOLocalMove(new Vector3(itemPosX - itemRandPosX, -5f, 0), _randItemSRDoPosXTerm).SetEase(Ease.OutQuad));
                    _randItemSR.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), _itemSclaeTerm);
                    _setItemDTList.Add(_inputDT);
                }
            }
            else if (_obstacle.transform.position.x > 0)
            {
                _inputDT = _randItemSR.transform.DOLocalMoveY(_fishJump, _randItemSRDoPosYTerm).OnComplete(() => _randItemSR.transform.DOLocalMove(new Vector3(-itemRandPosX, -4f, 0), _randItemSRDoPosXTerm).SetEase(Ease.OutQuad));
                _randItemSR.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), _itemSclaeTerm);
                _setItemDTList.Add(_inputDT);
            }

        }
        else if (_isFish == false)
        {
            _randItemSR.transform.DOLocalMoveY(0.5f, 1.3f);
        }
        StartCoroutine(yMaskSet());
    }
    IEnumerator yMaskSet()
    {
        yield return TT.WaitForSeconds(1f);
        _spriteMask.sprite = null;
        if (_isFish == true)
        {
            _randItemSR.sortingOrder = 203;
            yield return TT.WaitForSeconds(_itemTagTerm);
            _randItemSR.tag = "Untagged";

        }
        else if (_isFish == false)
        {
            _randItemSR.sortingOrder = 99;
        }
    }




    public void zResetTween()
    {
        foreach (Tween setDT in _setItemDTList)
        {
            if (setDT != null)
                setDT.Kill();
        }
        _setItemDTList.Clear();
    }


    void Start()
    {

    }


    private void FixedUpdate()
    {
        
    }
    void Update()
    {
        if (!GameManager.xInstance._isGameStarted) return;
            yTagObstacle();
        
    }

    void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        if (_instance == null)
        {
            _instance = this;
        }
    }

}
