using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class TrUI_PuzzlePeePee : TrUI_PuzzleManager
{

    static TrUI_PuzzlePeePee _instance;
    public static TrUI_PuzzlePeePee xInstance { get { return _instance; } }
    [SerializeField] CanvasGroup _imgTimeTense;
    [HideInInspector] public int _peepeeEscapeJump;
    public void zOnButtonJumpPressed()
    {
        
        if (Player.xInstance._peepeeHit == false && Player.xInstance._isFall == false)
        {
            if (TrPuzzlePeePee.xInstance._canJump == true)
            {
                TrPuzzlePeePee.xInstance._currJumping = true;
                TrPuzzlePeePee.xInstance.zPeePeeJump();
                TrAudio_SFX.xInstance.zzPlayPeeJump();
                TrPuzzlePeePee.xInstance._canJump = false;
            }


                TrPuzzlePeePee.xInstance.zJump();
            

        }

        
        if (Player.xInstance._isFall == true && Player.xInstance._btnLock == false)
        {
            if (TrPuzzlePeePee.xInstance._canJump == true)
            {
                _peepeeEscapeJump++;
                
                Player.xInstance._jumpKey = true;
            }
                TrPuzzlePeePee.xInstance._canJump = false;
        }
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
