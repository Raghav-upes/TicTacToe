using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BoardManager : MonoBehaviour
{
    //Serialized Fields
    [SerializeField]
    protected List<GameObject> _playerAPieces = new List<GameObject>(3);
    [SerializeField]
    protected List<GameObject> _playerBPieces = new List<GameObject>(3);
    [SerializeField]
    protected Transform _pieceTransform;
    [SerializeField]
    protected int _widthOfBoard,_heightOfBoard;
    [SerializeField]
    protected Tile _tilePrefab;
    [SerializeField]
    protected GameObject _pieceCollector;
    [SerializeField]
    protected bool _doCameraSwitchInGame = true;

    //Private Members

    protected List<Tile> _tiles = new List<Tile>();
    protected Transform _tilesParent;
    protected int[,] _board = new int[3, 3] ;
    protected float _gapBetweenEachTiles = 1f;
    public abstract void GenerateBoard();
    //public abstract void NewGame();
    public abstract void PlacePieceOnBoard(int tileIndex);


    #region BuiltIn Methods

    private void OnEnable() => GameManager.Instance.PiecePlaceOnBaord += PlacePieceOnBoard;
    private void OnDisable() => GameManager.Instance.PiecePlaceOnBaord -= PlacePieceOnBoard;

    protected virtual void Start()
    {
        Initialize();
    }

    #endregion

    #region Custom Methods

    private void Initialize()
    {
        _tilesParent = gameObject.transform.GetChild(0);
    }


    public void NewGame()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                _board[i, j] = 0;
            }
        }
        for (int i = 0; i < _tiles.Count; i++)
        {
            _tiles[i].TileTaken = TileTaken.NONE;
            _tiles[i].CurrentTilePiece = Piece.NONE;
        }
        foreach (Transform piece in _pieceCollector.transform)
        {
            Destroy(piece.gameObject);
        }
    }

    protected bool CheckWinner()
    {
        //Winner Check 
        for (int i = 0; i < 3; i++)
        {
            //Horizontal Check
            if (_board[i, 0] == _board[i, 1] && _board[i, 1] == _board[i, 2] && _board[i, 2] != 0)
            {
                Debug.Log(GameManager.Instance.PlayerTurn + "Winner");
                GameManager.Instance.State = GameState.END;
                GameManager.Instance.Winner = GameManager.Instance.PlayerTurn;
                GameManager.Instance.GameEnded();
                return true;
            }
            //Vertical Check
            if (_board[0, i] == _board[1, i] && _board[1, i] == _board[2, i] && _board[2, i] != 0)
            {
                Debug.Log(GameManager.Instance.PlayerTurn + "Winner");
                GameManager.Instance.State = GameState.END;
                GameManager.Instance.Winner = GameManager.Instance.PlayerTurn;
                GameManager.Instance.GameEnded();
                return true;
            }
        }
        //Diagonals Check
        if (_board[0, 0] == _board[1, 1] && _board[1, 1] == _board[2, 2] && _board[1, 1] != 0)
        {
            Debug.Log(GameManager.Instance.PlayerTurn + "Winner");
            GameManager.Instance.State = GameState.END;
            GameManager.Instance.Winner = GameManager.Instance.PlayerTurn;
            GameManager.Instance.GameEnded();
            return true;
        }
        else if (_board[0, 2] == _board[1, 1] && _board[1, 1] == _board[2, 0] && _board[1, 1] != 0)
        {
            Debug.Log(GameManager.Instance.PlayerTurn + "Winner");
            GameManager.Instance.State = GameState.END;
            GameManager.Instance.Winner = GameManager.Instance.PlayerTurn;
            GameManager.Instance.GameEnded();
            return true;
        }

        //If Not Winner return false
        return false;
    }

    #endregion
}
