using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PeePeeObstacle : MonoBehaviour
{
    static PeePeeObstacle _instance;
    public static PeePeeObstacle xInstance { get { return _instance; } }
    public Sprite[] _spObstacle;
    public SpriteRenderer _spriteRenderer;

    [Space]
    [Header("æ∆¿Ã≈€")]
    [SerializeField] Sprite _spriteMaskSet;
    [SerializeField] SpriteMask _spriteMask;
    [SerializeField] SpriteRenderer _randItemSR;
    [SerializeField] Sprite[] _randItemSP;
    [SerializeField] GameObject _item;
    [SerializeField] float _fishJumpForce;
    [SerializeField] float _itemSclaeTerm;
    [SerializeField] GameObject _obstacle;
    [SerializeField] GameObject _ObsMask;

    bool _isBoosterFish;
    bool _isHeartFish;
    bool _fishRotate;
    public List<Tween> _setItemDTList = new List<Tween>();
    public Tween _inputDT;

    Coroutine _fisgRotate;

    


    public void ySetItem()
    {
        yItemReset();
        int randItem = Random.Range(0, 10);
        if (TrPuzzlePeePee.xInstance._isSmallHole == true)
        {
            _spriteMask.transform.localScale = new Vector3(1f, 0.5f, 0f);
            _spriteMask.transform.localPosition = new Vector3(0f, -0.6f, 0f);
            _obstacle.GetComponent<BoxCollider2D>().size = new Vector2(1.4f, 0.1f);
            if (randItem < 4)
            {
                _randItemSR.sprite = _randItemSP[0];
                _item.tag = "Untagged";
                _item.GetComponent<BoxCollider2D>().enabled = true;
                _isHeartFish = true;
                _item.transform.localScale = new Vector3(0.3f, 0.3f, 1.3f);
            }
            else if (randItem >= 4 && randItem < 7)
            {
                _randItemSR.sprite = _randItemSP[1];
                _item.tag = "Untagged";
                _item.GetComponent<BoxCollider2D>().enabled = true;
                _isBoosterFish = true;
                _item.transform.localScale = new Vector3(0.3f, 0.3f, 1.3f);
            }

            else if (randItem >= 7 && randItem < 9)
            {
                _randItemSR.sprite = _randItemSP[2];
                _item.tag = "Untagged";
                _item.GetComponent<BoxCollider2D>().enabled = false;
                _item.transform.localScale = new Vector3(0.55f, 0.6f, 1f);
                _spriteMask.transform.localPosition = new Vector3(0f, -0.7f, 0f);
                _isBoosterFish = false;
                _isHeartFish = false;
            }
            else if (randItem == 9)
            {
                _randItemSR.sprite = null;
                _item.tag = "Untagged";
                _item.GetComponent<BoxCollider2D>().enabled = false;
                _isBoosterFish = false;
                _isHeartFish = false;
            }
        }
        else if (TrPuzzlePeePee.xInstance._isSmallHole == false)
        {
            _randItemSR.sprite = null;
            _item.tag = "Untagged";
            _item.GetComponent<BoxCollider2D>().enabled = false;
            _obstacle.GetComponent<BoxCollider2D>().size = new Vector2(3.5f, 0.1f);
            _isBoosterFish = false;
            _isHeartFish = false;
            _spriteMask.transform.localScale = new Vector3(1f, 0.55f, 0f);
            _spriteMask.transform.localPosition = new Vector3(0f, -0.7f, 0f);

        }
            yItemMove();
            StartCoroutine(yTagTerm());
    }

    IEnumerator yTagTerm()
    {
            
            yield return TT.WaitForSeconds(TrPuzzlePeePee.xInstance._itemTagTerm);
            if (_isBoosterFish == true)
            {
                _item.tag = "Booster";
            }
            
            if (_isHeartFish == true)
            {
                _item.tag = "Heart";
            }
            yield return TT.WaitForSeconds(0.3f);
            if (TrPuzzlePeePee.xInstance._isSmallHole == true)
            {
            _ObsMask.SetActive(false);
            }
            yield return TT.WaitForSeconds(1.7f);
            _item.tag = "Untagged";
    }


    void yItemReset()
    {
        _randItemSR.sprite = null;
        _item.transform.localPosition = new Vector3(0f, -1f, 0f);
        _spriteMask.sprite = _spriteMaskSet;
        _randItemSR.sortingOrder = 101;
        _item.SetActive(true);
        _ObsMask.SetActive(true);
        _isBoosterFish = false;
        _isHeartFish = false;
    }

    void yItemMove()
    {
        float itemMovePosX = 6.5f;
        float itemMovePosXSpeed = TrPuzzlePeePee.xInstance._fishMoveXSpeed;
        float itemScaleSpeed = TrPuzzlePeePee.xInstance._fishScaleSpeed;
        
        float itemPosX = _item.transform.localPosition.x;
        _fishRotate = true;
        if (_isBoosterFish == true || _isHeartFish == true)
        {
            if (_isBoosterFish == true)
            _inputDT = _item.transform.DOScale(new Vector3(0.4f, 0.4f, 1f), itemScaleSpeed).SetEase(Ease.Linear);
            else if (_isHeartFish == true) 
            _inputDT = _item.transform.DOScale(new Vector3(0.55f, 0.55f, 1f), itemScaleSpeed).SetEase(Ease.Linear);

            if (TrPuzzlePeePee.xInstance._fishRihgt == true)
            {
                _inputDT = _item.transform.DOLocalMoveY(_fishJumpForce, 0.2f).SetEase(Ease.Linear)/*.OnComplete(()=> _item.transform.DOLocalMove(new Vector3(itemPosX + itemMovePosX, -5f, 0), itemMovePosXSpeed).SetEase(Ease.OutQuad))*/;
            }else if (TrPuzzlePeePee.xInstance._fishLeft == true)
            {
                _inputDT = _item.transform.DOLocalMoveY(_fishJumpForce, 0.2f).SetEase(Ease.Linear)/*.OnComplete(() => _item.transform.DOLocalMove(new Vector3(itemPosX - itemMovePosX, -5f, 0), itemMovePosXSpeed).SetEase(Ease.OutQuad))*/;
            }
                /*_fisgRotate = */StartCoroutine(yItemRotate());
        }
        else
        {
            _fishRotate = false;
            _inputDT = _item.transform.DOLocalMoveY(0.262f, 0.5f).SetEase(Ease.Linear);
            /*if (_fisgRotate != null)
            {
                StopCoroutine(yItemRotate());
            }*/
        }
        _setItemDTList.Add(_inputDT);
    }

    IEnumerator yItemRotate()
    {
        while (_fishRotate == true)
        {
            yield return TT.WaitForSeconds(0.5f);
            _item.transform.localRotation = Quaternion.Euler(0, 180f, 0);
            yield return TT.WaitForSeconds(0.5f);
            _item.transform.localRotation = Quaternion.Euler(0, 0f, 0);
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



    void Update()
    {
        
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }


}
