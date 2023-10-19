using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    GameStateType _gameStateType;

    public GameStateType gameStateType { get => _gameStateType; }
    public static GameStateManager Instance { get; private set; }

    [SerializeField]
    GameStateText _gameStateText;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeState(GameStateType stateType)
    {
        switch (stateType)
        {
            case GameStateType.Lobby:
                _gameStateText.SetText(GameStateType.Lobby);
                break;
            case GameStateType.Matching:
                _gameStateText.SetText(GameStateType.Matching);
                break;
            case GameStateType.InGame:
                _gameStateText.SetText(GameStateType.InGame);
                break;
            default:
                break;
        }
        _gameStateType = stateType;
    }
}
