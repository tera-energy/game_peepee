using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;
public class TrUI_JoyStick : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    static TrUI_JoyStick _instance;
    public static TrUI_JoyStick xInstance { get { return _instance; } }

    [SerializeField, Range(10f, 150f)] private float leverRange;
    [HideInInspector]public bool _isLeft = false;
    [HideInInspector]public bool _isRight = false;
    [SerializeField] RectTransform _joyStick;
    float _currClickPosX;
    float _currDragPosX;
    float _dragPos;
    RectTransform _joyStickTransform;


    [SerializeField] bool _isUseAnimator;
    [SerializeField] bool _isUseParticle;
    [SerializeField] bool _isNotClickAfterClick;
    [SerializeField] bool _isStayingDown;
    Animator _anim;
    [SerializeField] ParticleSystem _particleTouch;
    [SerializeField] ParticleSystem _particleOnDown;
    GameObject _goCurrentDown;
    public UnityEvent<PointerEventData> _buttonPressedDown;
    public UnityEvent<PointerEventData> _buttonPressedUp;
    Image _img;

    bool _isStay = false;
    bool _isInteract = false;
    


    public void OnPointerDown(PointerEventData eventData)
    {
        if (Player.xInstance._peepeeHit == false || Player.xInstance._isFall == false)
        {
            if ((GameManager.xInstance._state == TT.enumGameState.Play && !GameManager.xInstance._isGameStarted) || !GameManager._canBtnClick || !_isInteract) return;
            //Debug.Log($">>> DOWN <<<: pressed mouse button is {eventData.button} and currently pointed gameobject is {eventData.pointerCurrentRaycast.gameObject}!", gameObject);
            _goCurrentDown = eventData.pointerCurrentRaycast.gameObject;
            _buttonPressedDown?.Invoke(eventData);

            if (_isUseAnimator)
                yPointerDown();
            if (_isUseParticle)
            {
                if (_particleOnDown != null)
                    _particleOnDown?.Play();
                if (_particleTouch != null)
                    _particleTouch?.Play();
            }

            if (_goCurrentDown != null)
                _isStay = true;

            //클릭위치 받고
            _currClickPosX = eventData.position.x;
        }
            
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {

            if ((GameManager.xInstance._state == TT.enumGameState.Play && !GameManager.xInstance._isGameStarted) || !GameManager._canBtnClick || !_isInteract) return;
            //Debug.Log(">>> UP <<<: pressed mouse button is " + eventData.button + " and currently pointed gameobject is " + eventData.pointerCurrentRaycast.gameObject + "!", gameObject);

            if (!_isStayingDown)
            {
                if (_isStay)
                    _buttonPressedUp?.Invoke(eventData);
            }
            else
                _buttonPressedUp?.Invoke(eventData);

            if (_isStay)
            {
                if (_isNotClickAfterClick)
                    GameManager._canBtnClick = false;
            }

            _goCurrentDown = null;
            _isStay = false;

            if (_isUseAnimator)
                yPointerUp();
            if (_isUseParticle)
            {
                if (_particleOnDown != null)
                    _particleOnDown?.Stop();
            }

            //원래 위치로
            _isLeft = false;
            _isRight = false;
            _joyStick.transform.DORotate(new Vector3(0f, 0f, 0f), 0f).SetEase(Ease.Linear);
        
        
    }



    public void OnDrag(PointerEventData eventData)
    {
        

            if (GameManager.xInstance._isGameStarted == true)
            {
                _currDragPosX = eventData.position.x;
                yZoyStickRotate();
            }

            if (GameManager.xInstance._state == TT.enumGameState.Play && !GameManager.xInstance._isGameStarted)
            {
                if (_goCurrentDown)
                {
                    if (_isUseParticle)
                        _particleOnDown.Stop();
                    _goCurrentDown = null;
                }
            }

            if (_goCurrentDown)
            {
                if (eventData.pointerCurrentRaycast.gameObject != _goCurrentDown)
                {
                    _isStay = false;
                    if (_isUseParticle)
                    {
                        if (_particleOnDown != null)
                            _particleOnDown.Stop();
                    }
                }
                else
                {
                    _isStay = true;
                    if (_isUseParticle)
                    {
                        if (_particleOnDown != null)
                            _particleOnDown.Play();
                    }
                }
            }
            //클릭 위치에서 이동 후 불 

            //조이스틱 이동은 메소드 생성
        


    }
    
    void yZoyStickRotate()
    {
        _dragPos = Mathf.Abs(_currClickPosX - _currDragPosX);

        if (_currClickPosX < _currDragPosX)
        {
            _isRight = true;
            _isLeft = false;
            if (_dragPos < 20)
            {
                _joyStick.transform.DORotate(new Vector3(0f, 0f, -5f), 0f).SetEase(Ease.Linear);
                TrPuzzlePeePee.xInstance._playerMaxSpeed = 1;
            }
            else if (20 <= _dragPos && _dragPos < 40)
            {
                _joyStick.transform.DORotate(new Vector3(0f, 0f, -10f), 0f).SetEase(Ease.Linear);
                TrPuzzlePeePee.xInstance._playerMaxSpeed = 2;
            }
            else if (40 <= _dragPos && _dragPos < 60)
            {
                _joyStick.transform.DORotate(new Vector3(0f, 0f, -15f), 0f).SetEase(Ease.Linear);
                TrPuzzlePeePee.xInstance._playerMaxSpeed = 3;
            }
            else if (60 <= _dragPos && _dragPos < 80)
            {
                _joyStick.transform.DORotate(new Vector3(0f, 0f, -20f), 0f).SetEase(Ease.Linear);
                TrPuzzlePeePee.xInstance._playerMaxSpeed = 4;
            }
            else if (80 <= _dragPos)
            {
                _joyStick.transform.DORotate(new Vector3(0f, 0f, -25f), 0f).SetEase(Ease.Linear);
                TrPuzzlePeePee.xInstance._playerMaxSpeed = 5;
            }
        }
        else if (_currClickPosX > _currDragPosX)
        {
            _isLeft = true;
            _isRight = false;
            if (_dragPos < 20)
            {
                _joyStick.transform.DORotate(new Vector3(0f, 0f, 5f), 0f).SetEase(Ease.Linear);
                TrPuzzlePeePee.xInstance._playerMaxSpeed = 1;
            }
            else if (20 <= _dragPos && _dragPos < 40)
            {
                _joyStick.transform.DORotate(new Vector3(0f, 0f, 10f), 0f).SetEase(Ease.Linear);
                TrPuzzlePeePee.xInstance._playerMaxSpeed = 2;
            }
            else if (40 <= _dragPos && _dragPos < 60)
            {
                _joyStick.transform.DORotate(new Vector3(0f, 0f, 15f), 0f).SetEase(Ease.Linear);
                TrPuzzlePeePee.xInstance._playerMaxSpeed = 3;
            }
            else if (60 <= _dragPos && _dragPos < 80)
            {
                _joyStick.transform.DORotate(new Vector3(0f, 0f, 20f), 0f).SetEase(Ease.Linear);
                TrPuzzlePeePee.xInstance._playerMaxSpeed = 4;
            }
            else if (80 <= _dragPos)
            {
                _joyStick.transform.DORotate(new Vector3(0f, 0f, 25f), 0f).SetEase(Ease.Linear);
                TrPuzzlePeePee.xInstance._playerMaxSpeed = 5;
            }
        }
    }

    public void zInteractDisable(bool isOnlyColor = false)
    {
        _img.color = new Color(0.5f, 0.5f, 0.5f);

        if (!isOnlyColor)
            _isInteract = false;
    }

    public void zInteractiActive(bool isOnlyColor = false)
    {
        _img.color = new Color(1f, 1f, 1f);

        if (!isOnlyColor)
            _isInteract = true;
    }

    void yPointerDown()
    {
        _anim.SetTrigger("IsClickDown");
    }

    void yPointerUp()
    {
        //_anim.SetBool("IsClickDown", false);
    }

    private void FixedUpdate()
    {

    }


    void Awake()
    {
        if (_isUseAnimator)
            _anim = GetComponent<Animator>();
        _img = GetComponent<Image>();
        _isInteract = true;

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
