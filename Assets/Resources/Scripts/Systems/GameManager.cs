using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private static GameManager _gameInstance;
    public static GameManager Instance { get { return _gameInstance; } }

    //Variables
    private Turn _playerTurn;
    private Piece _playerPiece;
    private GameMode _mode;
    private GameState _state;
    private Turn _player;
    private Turn _winner;


    //Properties
    public Turn Player { get { return _player; } set { _player = value; } }
    public Turn PlayerTurn { get { return _playerTurn; } set { _playerTurn = value; } }
    public Piece PlayerPiece { get { return _playerPiece; } set { _playerPiece = value;} }
    public GameMode Mode { get { return _mode; } set { _mode = value; } }
    public GameState State { get { return _state; } set { _state = value; } }
    public Turn Winner { get { return _winner; } set { _winner = value; } }
    public void OnlineStartSet() { OnlineStart(); }

    //Events
    public delegate void SpawnPiece(int tileIndex);
    public event SpawnPiece PiecePlaceOnBaord;

    public delegate void UISwitcher();
    public event UISwitcher UIPlayerSwitcher;

    public delegate void OnlineSetup();
    public event OnlineSetup OnlineStart;

    public delegate void GameEnd(bool draw=false);
    public event GameEnd GameOver;


    private void Awake()
    {
        InitializeInstance();
    }

    private void Start()
    {
        InitializeVariables();
    }

    private void InitializeInstance()
    {
        if (_gameInstance != null && _gameInstance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _gameInstance = this;
    }

    private void InitializeVariables()
    {
        _playerTurn = Turn.PLAYER_A;
    }

    public void PlayTurn(GameObject tile)
    {
        if(tile == null && InputManager.isAI && GameManager.Instance.PlayerTurn==Turn.PLAYER_B)
        {
            PiecePlaceOnBaord(0);
            return;
        }
        if(_playerPiece != Piece.NONE)
        {
            PiecePlaceOnBaord(tile.gameObject.GetComponent<Tile>().Index);
            Debug.Log(tile.gameObject.GetComponent<Tile>().Index);
        }
    }

    public void SwitchTurn()
    {
        PlayerPiece = Piece.NONE;
        PlayerTurn = (PlayerTurn == Turn.PLAYER_A) ? Turn.PLAYER_B : Turn.PLAYER_A;
        UIPlayerSwitcher();
    }

    public void GameModeSelect(int mode)
    {
        Mode = (GameMode)mode;
    }

    public void GameEnded(bool draw = false)
    {
        GameOver(draw);
    }
}
