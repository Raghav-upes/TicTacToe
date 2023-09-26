using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AIBoard1 : BoardManager

{
    int pieceWeight = 0;
    [SerializeField] UIController uiController;
    Tile tile;

    private bool switchBtn = true;
    private bool anotherswitchBtn = true;
    private bool anotherswitchBtn2 = true;

    protected override void Start()
    {
        base.Start();
        GenerateBoard();
    }

    public override void GenerateBoard()
    {
        Debug.LogWarning(GameManager.Instance.PlayerTurn);
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
                tile = newTile;
            }
        }
    }



    private void placeObj(int pieceWeight)
    {
        GameObject newPiece;
        do
        {


            int randomNumber = UnityEngine.Random.Range(0, 9);
            int io = randomNumber / 3;
            int jo = randomNumber % 3;

            if (_tiles[randomNumber].CurrentTilePiece.ToString() == "NONE")
            {
                var pos = 3 * io + jo;

                uiController._btnIndex = pieceWeight;

                if (UIController._playerBBtnCount[uiController._btnIndex] == 0)
                {
                    pieceWeight = 0;
                    uiController._btnIndex = pieceWeight;
                    while (UIController._playerBBtnCount[uiController._btnIndex] == 0)
                    {
                        pieceWeight++;
                        if (pieceWeight == 2)
                            pieceWeight = 0;
                        uiController._btnIndex = pieceWeight;
                    }

                }
                newPiece = Instantiate(_playerBPieces[pieceWeight], _tiles[randomNumber].Position, Quaternion.identity, _pieceTransform);

                _tiles[randomNumber].TileTaken = TileTaken.PLAYER_B_TAKEN;
                _board[io, jo] = -1;
                Debug.Log(_tiles);
                GameManager.Instance.PlayerPiece = (Piece)pieceWeight;
                _tiles[randomNumber].CurrentTilePiece = GameManager.Instance.PlayerPiece;
                return;
            }
        } while (true);

    }

    private int decideWeight(int pos)
    {
        if (_tiles[pos].CurrentTilePiece.ToString() == "SMALL")
        {
            Debug.LogError("Winning bu piece is SMALL");
            if (UIController._playerBBtnCount[1] == 0)
                return 2;
            return 1;
        }
        else
            if (_tiles[pos].CurrentTilePiece.ToString() == "MEDIUM")
        {
            Debug.LogError("Winning bu piece is MEdium");
            return 2;
        }
        else if (_tiles[pos].CurrentTilePiece.ToString() == "LARGE")
        {
            Debug.LogError("Winning bu piece is Large");
            return -1;
        }
        return 0;
    }



    public override void PlacePieceOnBoard(int tileIndex)
    {
        pieceWeight = 0;
        bool exception = false;
        if (!_tiles[tileIndex].IsValid() && GameManager.Instance.PlayerTurn == Turn.PLAYER_A)
        {
            Debug.Log(_tiles[tileIndex].IsValid());
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
                _tiles[tileIndex].CurrentTilePiece = GameManager.Instance.PlayerPiece;
                _board[i, j] = 1;
            }
            else
            {

                for (int io = 0; io < 3; io++)
                {
                    for (int jo = 0; jo < 3; jo++)
                    {
                        if (_board[io, jo] != -1 && _tiles[io * 3 + jo].CurrentTilePiece.ToString() != "LARGE")
                        {
                            int temp = _board[io, jo];
                            _board[io, jo] = -1;

                            if (PseudoCheckWinner())
                            {
                                var pos = 3 * io + jo;
                                pieceWeight = decideWeight(pos);

                                uiController._btnIndex = pieceWeight;
                                if (UIController._playerBBtnCount[uiController._btnIndex] == 0)
                                {
                                    _board[io, jo] = temp;
                                    playerCheck();
                                    exception = true;
                                    break;
                                }

                                int[,] tempArr = { };
                                tempArr = CheckProtect(tempArr);

                                if (tempArr.Length != 0)
                                {
                                    for (int ion = 0; ion < 3; ion++)
                                    {
                                        if (_tiles[tempArr[ion, 0] * 3 + tempArr[ion, 1]].CurrentTilePiece.ToString() == "NONE")
                                        {
                                            newPiece = Instantiate(_playerBPieces[pieceWeight], _tiles[tempArr[ion, 0] * 3 + tempArr[ion, 1]].Position, Quaternion.identity, _pieceTransform);
                                            _tiles[tempArr[ion, 0] * 3 + tempArr[ion, 1]].TileTaken = TileTaken.PLAYER_B_TAKEN;
                                            GameManager.Instance.PlayerPiece = (Piece)pieceWeight;
                                            _tiles[tempArr[ion, 0] * 3 + tempArr[ion, 1]].CurrentTilePiece = GameManager.Instance.PlayerPiece;
                                            exception = true;
                                            break;
                                        }
                                    }
                                }


                                if (pieceWeight == -1)
                                {


                                    _board[io, jo] = temp;
                                    playerCheck();
                                    exception = true;
                                    break;
                                }
                                uiController._btnIndex = pieceWeight;
                                if (UIController._playerBBtnCount[uiController._btnIndex] == 0)
                                {
                                    if (pieceWeight == 2)
                                    {
                                        _board[io, jo] = temp;
                                        playerCheck();
                                        exception = true;
                                        break;
                                    }

                                    pieceWeight = 1;
                                    uiController._btnIndex = pieceWeight;
                                    if (UIController._playerBBtnCount[uiController._btnIndex] == 0)
                                    {
                                        pieceWeight = 2;
                                        uiController._btnIndex = pieceWeight;
                                        if (UIController._playerBBtnCount[uiController._btnIndex] == 0)
                                        {
                                            _board[io, jo] = temp;
                                            placeObj(pieceWeight);
                                            exception = true;
                                            break;
                                        }
                                    }




                                }
                                Debug.Log("Marker");
                                newPiece = Instantiate(_playerBPieces[pieceWeight], _tiles[pos].Position, Quaternion.identity, _pieceTransform);
                                _tiles[pos].TileTaken = TileTaken.PLAYER_B_TAKEN;
                                GameManager.Instance.PlayerPiece = (Piece)pieceWeight;
                                _tiles[pos].CurrentTilePiece = GameManager.Instance.PlayerPiece;

                                exception = true;
                                break;
                            }
                            else


                                _board[io, jo] = temp;
                        }


                    }
                    if (exception)
                    {
                        break;
                    }
                }
                if (!PseudoCheckWinner() && !exception)
                {
                    playerCheck();
                }


            }
            if (!CheckWinner())
            {
                FixBoard();
                Debug.Log(_board);
                if (GameManager.Instance.PlayerTurn==Turn.PLAYER_A)
                    StartCoroutine(PlayAI());
                GameManager.Instance.SwitchTurn();
                checkDarw();
            }
        }
    }
    private int[,] anotherWin(int[,] optionalParameter)
    {
        int[,] arr = { };
        for (int io = 0; io < 3; io++)
        {
            for (int jo = 0; jo < 3; jo++)
            {
                var pos = 3 * io + jo;
                int temp = _board[io, jo];
                _board[io, jo] = 1;
                arr = CheckProtect(optionalParameter);
                if (arr.Length != 0)
                {
                    _board[io, jo] = temp;
                    Debug.Log(arr);
                    return arr;
                }
                _board[io, jo] = temp;

            }
        }
        return arr;
    }

    private void aiGenerate(int pos, int[,] arr)
    {
        bool nonePlace = false;
        for (int i = 0; i < 3; i++)
        {

            if (i == 3)
                i = 0;
            var posPiece = arr[i, 0] * 3 + arr[i, 1];
            if (_tiles[posPiece].TileTaken.ToString() == "PLAYER_A_TAKEN")
            {
                int weigh = decideWeight(posPiece);
                if (weigh == -1)
                {
                    nonePlace = true;

                    int[,] tempArr = anotherWin(arr);
                    if (tempArr.Length != 0 && switchBtn)
                    {
                        switchBtn = false;
                        aiGenerate(pos, tempArr);
                        return;
                    }
                    else if (checkTwoMediumOneLarge(arr))
                    {
                        placeObj(0);
                        return;
                    }
                    else
                        continue;

                }
                uiController._btnIndex = weigh;
                if (UIController._playerBBtnCount[uiController._btnIndex] == 0)
                {
                    weigh = 2;
                    uiController._btnIndex = weigh;
                    while (UIController._playerBBtnCount[uiController._btnIndex] == 0 && weigh > -1)
                    {
                        weigh--;
                        uiController._btnIndex = weigh;
                    }

                    for (int jon = 0; jon < 3; jon++)
                    {
                        var posPieco = arr[jon, 0] * 3 + arr[jon, 1];

                        if (_tiles[posPieco].TileTaken.ToString() == "NONE")
                        {

                            var newPieco = Instantiate(_playerBPieces[weigh], _tiles[posPieco].Position, Quaternion.identity, _pieceTransform);
                            _board[posPieco / 3, posPieco % 3] = -1;

                            _tiles[posPieco].TileTaken = TileTaken.PLAYER_B_TAKEN;
                            GameManager.Instance.PlayerPiece = (Piece)weigh;
                            _tiles[posPieco].CurrentTilePiece = GameManager.Instance.PlayerPiece;
                            return;
                        }
                    }
                    int[,] tempArr = anotherWin(arr);
                    if (tempArr.Length != 0 && anotherswitchBtn2)
                    {
                        anotherswitchBtn2 = false;
                        aiGenerate(pos, tempArr);
                        return;
                    }
                    else
                    {
                        placeObj(0);
                        return;
                    }
                }
                var newPiece = Instantiate(_playerBPieces[weigh], _tiles[posPiece].Position, Quaternion.identity, _pieceTransform);
                _board[posPiece / 3, posPiece % 3] = -1;

                _tiles[posPiece].TileTaken = TileTaken.PLAYER_B_TAKEN;
                GameManager.Instance.PlayerPiece = (Piece)weigh;
                _tiles[posPiece].CurrentTilePiece = GameManager.Instance.PlayerPiece;
                return;
            }
            else if (nonePlace)
            {
                bool patternChange = true;
                int weigh = 1;
                uiController._btnIndex = weigh;
                if (UIController._playerBBtnCount[uiController._btnIndex] == 0)
                {
                    weigh = 2;
                    uiController._btnIndex = weigh;
                    if (UIController._playerBBtnCount[uiController._btnIndex] == 0)
                    {
                        weigh = 0;
                        uiController._btnIndex = weigh;
                    }
                }
                for (int kon = 0; kon < 3; kon++)
                {
                    var konPience = arr[kon, 0] * 3 + arr[kon, 1];
                    if (_tiles[konPience].TileTaken.ToString() == "NONE")
                    {
                        patternChange = false;
                        posPiece = konPience;
                        break;
                    }
                }

                if (patternChange)
                {
                    int[,] tempArr = anotherWin(arr);
                    if (tempArr.Length != 0 && anotherswitchBtn)
                    {
                        anotherswitchBtn = false;
                        aiGenerate(pos, tempArr);
                        return;
                    }
                    else
                    {
                        placeObj(0);
                        return;
                    }
                }

                var newPiece = Instantiate(_playerBPieces[weigh], _tiles[posPiece].Position, Quaternion.identity, _pieceTransform);
                _board[posPiece / 3, posPiece % 3] = -1;

                _tiles[posPiece].TileTaken = TileTaken.PLAYER_B_TAKEN;
                GameManager.Instance.PlayerPiece = (Piece)weigh;
                _tiles[posPiece].CurrentTilePiece = GameManager.Instance.PlayerPiece;
                return;
            }
        }
    }

    private bool checkTwoMediumOneLarge(int[,] arr)
    {
        int med = 0;
        int sma = 0;
        int lar = 0;
        for (int i = 0; i < 3; i++)
        {
            var posPiece = arr[i, 0] * 3 + arr[i, 1];

            if (_tiles[posPiece].CurrentTilePiece.ToString() == "MEDIUM")
                med++;
            else if (_tiles[posPiece].CurrentTilePiece.ToString() == "LARGE")
                lar++;
            else if (_tiles[posPiece].CurrentTilePiece.ToString() == "SMALL")
                sma++;

        }
        if ((lar == 1 && med == 2) || (med == 1 && lar == 2))
            return true;
        return false;
    }

    private void playerCheck()
    {
        int[,] tempo = { };
        for (int io = 0; io < 3; io++)
        {
            for (int jo = 0; jo < 3; jo++)
            {
                var pos = 3 * io + jo;
                int temp = _board[io, jo];
                _board[io, jo] = 1;
                int[,] arr = CheckProtect(tempo);

                if (arr.Length != 0)
                {
                    Debug.LogError("Hi");
                    aiGenerate(pos, arr);
                    _board[io, jo] = temp;
                    if (_tiles[pos].TileTaken.ToString() == "PLAYER_B_TAKEN")
                        _board[io, jo] = -1;
                    return;
                }

                _board[io, jo] = temp;

            }
        }
        placeObj(0);
    }

    IEnumerator PlayAI()
    {
        yield return new WaitForSecondsRealtime(2);

        GameManager.Instance.PlayerPiece = Piece.BYPASS;
        GameManager.Instance.PlayTurn(tile.transform.gameObject);
    }


    private void FixBoard()
    {
        for (int io = 0; io < 3; io++)
        {
            for (int jo = 0; jo < 3; jo++)
            {
                if (_tiles[3 * io + jo].TileTaken.ToString() == "PLAYER_B_TAKEN")
                    _board[io, jo] = -1;
                else if (_tiles[3 * io + jo].TileTaken.ToString() == "PLAYER_A_TAKEN")
                    _board[io, jo] = 1;
                else
                    _board[io, jo] = 0;
            }
        }
    }

}
