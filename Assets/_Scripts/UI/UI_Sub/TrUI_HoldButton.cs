using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrUI_HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler{

    
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

    public void OnPointerDown(PointerEventData eventData) {
        if ((GameManager.xInstance._state == TT.enumGameState.Play && !GameManager.xInstance._isGameStarted) || !GameManager._canBtnClick || !_isInteract) return;
        //Debug.Log($">>> DOWN <<<: pressed mouse button is {eventData.button} and currently pointed gameobject is {eventData.pointerCurrentRaycast.gameObject}!", gameObject);
        _goCurrentDown = eventData.pointerCurrentRaycast.gameObject;
        _buttonPressedDown?.Invoke(eventData);

        if (TrPuzzlePeePee.xInstance != null)
        {
            if (TrPuzzlePeePee.xInstance._btnAnim == false)
            {
                _isUseAnimator = false;
            }
            else
            {
                _isUseAnimator = true;
            }
        }

        if (_isUseAnimator)
            yPointerDown();
        if (_isUseParticle){
            if (_particleOnDown != null)
                _particleOnDown?.Play();
            if(_particleTouch != null)
                _particleTouch?.Play();
        }

        if (_goCurrentDown != null)
            _isStay = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        if ((GameManager.xInstance._state == TT.enumGameState.Play && !GameManager.xInstance._isGameStarted) || !GameManager._canBtnClick || !_isInteract) return;
        //Debug.Log(">>> UP <<<: pressed mouse button is " + eventData.button + " and currently pointed gameobject is " + eventData.pointerCurrentRaycast.gameObject + "!", gameObject);

        if (!_isStayingDown){
            if (_isStay)
                _buttonPressedUp?.Invoke(eventData);
        }
        else
            _buttonPressedUp?.Invoke(eventData);

        if (_isStay){
            if (_isNotClickAfterClick)
                GameManager._canBtnClick = false;
        }

        _goCurrentDown = null;
        _isStay = false;

        if (_isUseAnimator)
            yPointerUp();
        if (_isUseParticle){
            if (_particleOnDown != null)
                _particleOnDown?.Stop();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.xInstance._state == TT.enumGameState.Play && !GameManager.xInstance._isGameStarted){
            if (_goCurrentDown){
                if(_isUseParticle)
                    _particleOnDown.Stop();
                _goCurrentDown = null;
            }
        }

        if (_goCurrentDown)
        {
            if (eventData.pointerCurrentRaycast.gameObject != _goCurrentDown){
                _isStay = false;
                if (_isUseParticle){
                    if (_particleOnDown != null)
                        _particleOnDown.Stop();
                }
            }
            else{
                _isStay = true;
                if (_isUseParticle){
                    if (_particleOnDown != null)
                        _particleOnDown.Play();
                }
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

    void yPointerDown(){
        _anim.SetTrigger("IsClickDown");
    }

    void yPointerUp(){
        //_anim.SetBool("IsClickDown", false);
    }

    void Awake(){
        if (_isUseAnimator)
            _anim = GetComponent<Animator>();
        _img = GetComponent<Image>();
        _isInteract = true;
    }
}
