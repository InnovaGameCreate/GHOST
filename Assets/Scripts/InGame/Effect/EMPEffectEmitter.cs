using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EMPEffectEmitter : MonoBehaviour
{
    [SerializeField]
    private GameObject _empEffects;
    [SerializeField]
    private GameObject _useText;
    [SerializeField]
    private Image p1Image;
    [SerializeField]
    private Image p2Image;

    public static EMPEffectEmitter Instance { get; private set; }
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void EmitEffect()
    {
        _empEffects.SetActive(true);
    }

    public void FinishEffect()
    {
        _empEffects.SetActive(false);
    }
    /// <summary>
    /// empを使っていることを知らせるテキスト
    /// </summary>
    public void DisplayUseText(bool value)
    {
        _useText.SetActive(value);
    }

    /// <summary>
    /// プレイヤーが死んだときのエフェクト
    /// </summary>
    public void DeadEffect(bool isP1,bool value)
    {
        if (isP1)
        {
            if(value)
            {
                p1Image.DOFade(1, 2f).SetEase(Ease.InOutSine);
            }
            else
            {
                p1Image.DOFade(0, 2f).SetEase(Ease.InOutSine);
            }
        }
        else
        {
            if (value)
            {
                p2Image.DOFade(1, 2f).SetEase(Ease.InOutSine);
            }
            else
            {
                p2Image.DOFade(0, 2f).SetEase(Ease.InOutSine);
            }
        }
    }
}
