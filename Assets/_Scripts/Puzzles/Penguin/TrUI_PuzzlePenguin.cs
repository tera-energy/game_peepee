using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TrUI_PuzzlePenguin : TrUI_PuzzleManager
{

    static TrUI_PuzzlePenguin _instance;
    public static TrUI_PuzzlePenguin xInstance {  get { return _instance; } }
    [SerializeField] CanvasGroup _imgTimeTense;
    [HideInInspector] public bool _leftMove;
    [HideInInspector] public bool _rightMove;
    [HideInInspector] public bool _slide;
    [HideInInspector] public bool _jumpClick;


    public void zOnButtonLeftPressed()
    {
        _leftMove = true;
    }
    public void zOnButtonRightPressed()
    {
        _rightMove = true;
    }
    public void zOnButtonSlidePressed()
    {
        _slide = true;
    }
    public void zOnButtonJumpPressed()
    {

            _jumpClick = true;
            TrPuzzlePenguin.xInstance.zPeePeeJump();

        TrAudio_SFX.xInstance.zzPlayPeeJump();
    }
    public void zOnButtonLeftReleased()
    {
        //TrPuzzlePenguin.xInstance.zPeePeeMove(TT.enumButtonInput.Neutral);
        _leftMove = false;
    }
    public void zOnButtonRightReleased()
    {
        //TrPuzzlePenguin.xInstance.zPeePeeMove(TT.enumButtonInput.Neutral);
        _rightMove = false;
    }
    public void zOnButtonSlideReleased()
    {
        _slide = false;
    }


    IEnumerator yTimerTenseExec()
    {
        int reach = 1;
        while (true)
        {
            _imgTimeTense.DOFade(reach, 0.5f);
            yield return TT.WaitForSeconds(0.5f);
            reach = reach == 1 ? 0 : 1;

        }
        _imgTimeTense.alpha = 0;
    }
    public void zSetTimerTenseEffect()
    {
        StartCoroutine(yTimerTenseExec());        
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
