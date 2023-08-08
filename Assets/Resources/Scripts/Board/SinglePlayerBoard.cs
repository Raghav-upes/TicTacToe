using UnityEngine;

public class SinglePlayerBoard : BoardManager
{
    protected override void Start()
    {
        base.Start();
        GenerateBoard();
    }

    public override void GenerateBoard()
    {
        int index = 0;
        for (int i = 0; i < _widthOfBoard; i++)
        {
            for (int j = 0; j < _heightOfBoard; j++)
            {
                Tile newTile = Instantiate(_tilePrefab,
                new Vector3(j * _gapBetweenEachTiles - 1f, _tilesParent.position.y, i * _gapBetweenEachTiles - 1f),
                Quaternion.identity, _tilesParent);
                newTile.Initialize(index++, newTile.gameObject.transform.position, TileTaken.NONE);
                newTile.name = "Tile " + i + " " + j;
                _tiles.Add(newTile);
            }
        }
    }

    public override void PlacePieceOnBoard(int tileIndex)
    {
        if (!_tiles[tileIndex].IsValid())
        {
            return;
        }
        else
        {
            GameObject newPiece;
            int i = tileIndex / 3;
            int j = tileIndex % 3;
            if (GameManager.Instance.PlayerTurn == Turn.PLAYER_A)
            {
                newPiece = Instantiate(_playerAPieces[(int)GameManager.Instance.PlayerPiece], _tiles[tileIndex].Position, Quaternion.identity, _pieceTransform);
                _tiles[tileIndex].TileTaken = TileTaken.PLAYER_A_TAKEN;
                _board[i, j] = 1;
            }
            else
            {
                newPiece = Instantiate(_playerBPieces[(int)GameManager.Instance.PlayerPiece], _tiles[tileIndex].Position, Quaternion.identity, _pieceTransform);
                _tiles[tileIndex].TileTaken = TileTaken.PLAYER_B_TAKEN;
                _board[i, j] = -1;
            }
            if (!CheckWinner())
            {
                GameManager.Instance.SwitchTurn();
            }
        }
    }
}
