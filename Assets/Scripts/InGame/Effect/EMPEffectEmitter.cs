using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPEffectEmitter : MonoBehaviour
{
    [SerializeField]
    private GameObject _empEffects;
    [SerializeField]
    private GameObject _useText;

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

    public void DisplayUseText()
    {
        _useText.SetActive(true);
    }
    public void HideUseText()
    {
        _useText.SetActive(false);
    }
}
