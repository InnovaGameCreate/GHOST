using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
public class PlayerUiPresenter : MonoBehaviour
{
    PlayerStatus _myPlayerStatus;
    PlayerStatus _enemyPlayerStatus;
    [SerializeField]
    private Image _myCharacterHpBar;
    [SerializeField]
    private Image _enemyCharacterHpBar;


    void Start()
    {
        StartCoroutine(setPlayerStatis());
    }

    IEnumerator setPlayerStatis()
    {
        yield return new WaitUntil(() => set());
        _myPlayerStatus.currentHp
            .Subscribe(hp =>
            {
                _myCharacterHpBar.fillAmount = hp / _myPlayerStatus.maxBaseHp;
            }).AddTo(this);
        _enemyPlayerStatus.currentHp
            .Subscribe(hp =>
            {
                _enemyCharacterHpBar.fillAmount = hp / _enemyPlayerStatus.maxBaseHp;
            }).AddTo(this);
    }

    private bool set()
    {
        var _playerStatus = FindObjectsOfType<PlayerStatus>();
        foreach (var item in _playerStatus)
        {
            if (item.isLocalPlayer)
            {
                _myPlayerStatus = item;
            }
            else
            {
                _enemyPlayerStatus = item;
            }
        }
        if (_enemyPlayerStatus != null && _myPlayerStatus != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
