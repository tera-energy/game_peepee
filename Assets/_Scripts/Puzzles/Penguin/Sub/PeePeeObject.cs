using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class PeePeeObject : MonoBehaviour
{
    static PeePeeObject _instance;
    public static PeePeeObject xInstance { get { return _instance; } }

    [Space]
    [SerializeField] Sprite[] _sunSP;
    [SerializeField] SpriteRenderer _sunSR;
    [Space]
    [SerializeField] Sprite[] _underBackgroundSP;
    [SerializeField] SpriteRenderer _underBackgroundSR;
    [Space]
    [SerializeField] Sprite[] _onBackgroundSP;
    [SerializeField] SpriteRenderer _onBackgroundSR;
    [Space]
    [SerializeField] Sprite[] _backSP;
    [SerializeField] SpriteRenderer _backSR;
    [Space]
    [SerializeField] GameObject _smllCloud;
    [SerializeField] GameObject _bigCloud;
    [SerializeField] GameObject _sunRotation;
    [SerializeField] GameObject _Orora;
    [SerializeField] GameObject _meteor;
    [SerializeField] GameObject _twinkleStar;
    [Space]
    [SerializeField] Sprite[] _meteorSP;
    [SerializeField] SpriteRenderer _meteorSR;
    int _count = 0;
    bool _isNight = false;
    Coroutine _coNight;
    Coroutine _coAfternoon;
    Coroutine _coMeteor;
    [Space]
    [SerializeField] ParticleSystem[] _snow;

    [Space]
    [SerializeField] Animator _animNight;
    [SerializeField] public Animator _animAfternoon;
    [SerializeField] public Animator _animGomPlay;

    [SerializeField] Animator _animBack;
    [SerializeField] Animator _animSC;
    Tween _meteorTween;
    Tween _sunTween;
    int _nightCount;

    IEnumerator yNight()
    {

            _sunRotation.transform.rotation = Quaternion.Euler(0f, 0f, 45f);
            _snow[0].gameObject.SetActive(true);
            _snow[1].gameObject.SetActive(true);
            _twinkleStar.SetActive(true);
            _meteor.SetActive(true);
            _Orora.SetActive(true);
            StartCoroutine(yMeteor());
            _sunSR.sprite = _sunSP[1];
            _smllCloud.SetActive(false);
            _bigCloud.SetActive(false);
            _underBackgroundSR.sprite = _underBackgroundSP[1];
            _onBackgroundSR.sprite = _onBackgroundSP[1];
            _backSR.sprite = _backSP[1];
            _snow[0].Play();
            _snow[1].Play();
            _animNight.SetBool("Orora", true);
            //_animAfternoon.SetBool("StartAfternoon", false);
            _sunRotation.transform.DOLocalRotate(new Vector3(0f, 0f, -45f), 30).SetEase(Ease.Linear);


            yield return TT.WaitForSeconds(35f);
            _snow[0].Stop();
            _snow[1].Stop();
            _isNight = false;
            
            if (_coAfternoon != null)
            {
                _coAfternoon = null;
            }
            StartCoroutine(yAfternoon());


          
    }
    IEnumerator yAfternoon()
    {
            _sunRotation.transform.rotation = Quaternion.Euler(0f, 0f, 45f);

            _snow[0].gameObject.SetActive(false);
            _snow[1].gameObject.SetActive(false);
            _twinkleStar.gameObject.SetActive(false);
            _meteor.SetActive(false);
            _Orora.SetActive(false);
            _sunSR.sprite = _sunSP[0];
            _animNight.SetBool("Orora", false);
            
            _underBackgroundSR.sprite = _underBackgroundSP[0];
            _onBackgroundSR.sprite = _onBackgroundSP[0];
            _backSR.sprite = _backSP[0];
            _smllCloud.SetActive(true);
            _bigCloud.SetActive(true);


            _sunTween = _sunRotation.transform.DOLocalRotate(new Vector3(0f, 0f, -45f), 30).SetEase(Ease.Linear);

                yield return TT.WaitForSeconds(35f);

                _isNight = true;
                if (_coNight != null)
                {
                    _coNight = null;
                }
                
             _coNight = StartCoroutine(yNight());

        

    }
    void yMeteorAct()
    {

        if (_count == 0)
        {
            _meteorSR.sprite = _meteorSP[0];
        }else if (_count == 1)
        {
            _meteorSR.sprite = _meteorSP[1];
        }else if (_count == 2)
        {
            _meteorSR.sprite = _meteorSP[2];
        }
        _meteorTween = _meteor.transform.DOMove(new Vector3(-12.31f, 0.93f, 0f), 26f).SetEase(Ease.Linear);


    }
    IEnumerator yMeteor()
    {
        _meteor.transform.localPosition = new Vector3(11.28f, 5.98f, 0f);
        while (_isNight == true)
        {
            yMeteorAct();
            yield return TT.WaitForSeconds(0.3f);
            _count++;
            if (_count > 2)
            {
                _count = 0;
            }
        }
        
    }

    public IEnumerator yBearAndSeal()
    {
        while (GameManager.xInstance._isGameStarted == true)
        {
            int randNum = Random.Range(0, 3);
            yield return new WaitUntil(() => Player.xInstance._peepeeHit == false && Player.xInstance._isFall == false);


            yield return new WaitUntil(() => !_animAfternoon.GetCurrentAnimatorStateInfo(0).IsName("SealStart") && !_animGomPlay.GetCurrentAnimatorStateInfo(0).IsName("gomgom"));
            switch (randNum)
            {
                case 0:                                                                                  
                    _animAfternoon.SetTrigger("Seal");
                    _animGomPlay.SetTrigger("Bear");
                    //Debug.Log("rand : " + randNum + " : 둘 다");
                    //Debug.Log(_animAfternoon.GetCurrentAnimatorStateInfo(0).IsName("SealStart"));
                    break;
                case 1:
                    _animGomPlay.SetTrigger("Bear");
                    //Debug.Log("rand : " + randNum + " : 곰만");
                    //Debug.Log(_animAfternoon.GetCurrentAnimatorStateInfo(0).IsName("gomgom"));
                    
                    break;
                case 2:
                    _animAfternoon.SetTrigger("Seal");
                    //Debug.Log("rand : " + randNum + " : 물개만");
                    //Debug.Log(_animAfternoon.GetCurrentAnimatorStateInfo(0).IsName("SealStart"));
                    break;
            }
            yield return new WaitUntil(()=> !_animAfternoon.GetCurrentAnimatorStateInfo(0).IsName("SealStart") && !_animGomPlay.GetCurrentAnimatorStateInfo(0).IsName("gomgom"));
        }      
    }



    void yObjectMove()
    {
        if (Player.xInstance._peepeeHit == true || Player.xInstance._isFall == true)
        {   
            _animAfternoon.GetComponent<Animator>().speed = 0f;
            _animGomPlay.GetComponent<Animator>().speed = 0f;
            _animBack.GetComponent<Animator>().speed = 0f;
        }
        else if (Player.xInstance._peepeeHit == false && Player.xInstance._isFall == false)
        {
            _animSC.GetComponent<Animator>().speed = TrPuzzlePeePee.xInstance._objectSpeed;
            _animAfternoon.GetComponent<Animator>().speed = TrPuzzlePeePee.xInstance._objectSpeed;
            _animGomPlay.GetComponent<Animator>().speed = TrPuzzlePeePee.xInstance._objectSpeed;
            _animBack.GetComponent<Animator>().speed = TrPuzzlePeePee.xInstance._objectSpeed;
        }

    }

    void Start()
    {
        _coAfternoon = StartCoroutine(yAfternoon());
        _animAfternoon.SetBool("StartAfternoon", true);
        _animGomPlay.SetBool("StartGom", true);
        _animBack.SetBool("Back", true);
        _animSC.SetBool("SmallCloud", true);

        
    }
    void Update()
    {
        yObjectMove();
    }

    void Awake()
    {

        if (_instance == null)
        {
            _instance = this;
        }

    }
}
