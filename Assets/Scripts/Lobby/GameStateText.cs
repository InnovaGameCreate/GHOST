using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateText : MonoBehaviour
{
    [SerializeField]
    private GameObject _lobbyText;
    [SerializeField]
    private GameObject _matchingText;
    [SerializeField]
    private GameObject _inGameText;
    void Start()
    {
        _lobbyText.SetActive(false);
        _matchingText.SetActive(false);
        _inGameText.SetActive(false);
    }

    
    public void SetText(GameStateType type)
    {
        switch (type)
        {
            case GameStateType.Lobby:
                _lobbyText.SetActive(true);
                _matchingText.SetActive(false);
                _inGameText.SetActive(false);
                break;
            case GameStateType.Matching:
                _lobbyText.SetActive(false);
                _matchingText.SetActive(true);
                _inGameText.SetActive(false);
                break;
            case GameStateType.InGame:
                _lobbyText.SetActive(false);
                _matchingText.SetActive(false);
                _inGameText.SetActive(true);
                break;
            default:
                break;
        }
    }
}
