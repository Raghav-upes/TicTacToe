using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

[RequireComponent(typeof(PhotonView))]
public class UIController : MonoBehaviour
{
    //Serialized Fields
    [SerializeField]
    private Button[] _playerABtn = new Button[3];
    [SerializeField]
    private Button[] _playerBBtn = new Button[3];
    [SerializeField]
    private TextMeshProUGUI[] _playerABtnCountText = new TextMeshProUGUI[3];
    [SerializeField]
    private TextMeshProUGUI[] _playerBBtnCountText = new TextMeshProUGUI[3];
    [SerializeField]
    private Image _playerATimer, _playerBTimer, _winningScreen;
    [SerializeField]
    private Text _playerATimerText, _playerBTimerText, _winnerName;
    [SerializeField]
    private TextMeshProUGUI _playerANameText, _playerBNameText;
    [SerializeField]
    private InputField _playerNameInput;
    [SerializeField]
    private Text _playerNameText;
    [SerializeField]
    private Animator _upperUIAnimator, _lowerUIAnimator;
    [SerializeField]
    private RectTransform _upperUI, _lowerUI;
    [SerializeField]
    private GameObject _playUI, _multiplayerUI, _singleplayerUI,_gameOverUI;
    [SerializeField]
    private RectTransform _playerAName, _playerBName;
    [SerializeField]
    private RectTransform _playerAUI, _playerBUI;
    [SerializeField]
    private int[] _maxEachPieceCount = { 3, 2, 1 };


    [SerializeField]
    private float _maxTimeForEachPlayer = 40f;

    [SerializeField]
    private Text matchResult;

    Tile tile;
    public InputField _createRoomNameText, _joinRoomNameText;
    public InputField _playerANameTextInputField, _playerBNameTextInputField;

    private PhotonView _photonView;
    public static int[] _playerABtnCount = new int[3];
    public static int[] _playerBBtnCount = new int[3];
    public int _btnIndex;
    private float _timeRemainingForPlayerA, _timeRemainingForPlayerB;
    private string _playerName;

    public String PlayerName { get { return _playerName; } set { _playerName = value; } }

    private void Start()
    {
        InitializeVariables();
    }

    private void OnEnable()
    {
        GameManager.Instance.UIPlayerSwitcher += UISwitcher;
        GameManager.Instance.OnlineStart += OnlineStart;
        GameManager.Instance.GameOver += GameOver;
    }
    private void OnDisable()
    {
        GameManager.Instance.UIPlayerSwitcher -= UISwitcher;
        GameManager.Instance.OnlineStart -= OnlineStart;
        GameManager.Instance.GameOver -= GameOver;
    }

    private void Update()
    {
        if (GameManager.Instance.Mode != GameMode.NONE && GameManager.Instance.State == GameState.START) 
        {
            TimerUpdate();
        }
    }

    private void InitializeVariables()
    {
        _photonView = GetComponent<PhotonView>();

        SetVariables();

        PlayerName = PlayerPrefs.GetString("PlayerName");
        _playerNameText.text = PlayerName;
    }

    public void SetVariables()
    {
        for (int i = 0; i < _playerABtnCount.Length; i++)
        {
            _playerABtnCount[i] = _playerBBtnCount[i] = _maxEachPieceCount[i];
            _playerABtnCountText[i].text = _playerBBtnCountText[i].text = _maxEachPieceCount[i].ToString();
        }

        _timeRemainingForPlayerA = _timeRemainingForPlayerB = _maxTimeForEachPlayer;
        _playerATimerText.text = _playerBTimerText.text = _maxTimeForEachPlayer.ToString();
        _playerATimer.fillAmount = _timeRemainingForPlayerA / _maxTimeForEachPlayer;
        _playerBTimer.fillAmount = _timeRemainingForPlayerB / _maxTimeForEachPlayer;

        GameManager.Instance.State = GameState.IDLE;
    }

    public void AnimateIn()
    {
        _upperUIAnimator.CrossFade("upperUIIn", 0, 0);
        _lowerUIAnimator.CrossFade("lowerUIIn", 0, 0);
    }

    public void AnimateOut()
    {
        _upperUIAnimator.CrossFade("upperUIOut", 0, 0);
        _lowerUIAnimator.CrossFade("lowerUIOut", 0, 0);
    }

    public void PieceSelect(int index)
    {
        if(GameManager.Instance.Mode == GameMode.ONLINE)
        {
            _photonView.RPC(nameof(RPC_PieceSelect), RpcTarget.AllBuffered, new object[] { index });
        }
        else
        {
            GameManager.Instance.PlayerPiece = (Piece)index;
            _btnIndex = index;
            if(GameManager.Instance.PlayerTurn == Turn.PLAYER_B && InputManager.isAI)
            {
                GameManager.Instance.PlayerPiece = (Piece)0;
                _btnIndex = 0;
            }
            Debug.Log(GameManager.Instance.PlayerPiece);

        }
    }


    [PunRPC]
    public void RPC_PieceSelect(int index)
    {
        GameManager.Instance.PlayerPiece = (Piece)index;
        _btnIndex = index;
        Debug.Log(GameManager.Instance.PlayerPiece);
    }

    private void UISwitcher()
    {
        if (GameManager.Instance.PlayerTurn != Turn.PLAYER_A)
        {
            _playerABtnCount[_btnIndex] -= 1;
            _playerABtnCountText[_btnIndex].text = _playerABtnCount[_btnIndex].ToString();
            if (_playerABtnCount[_btnIndex] == 0)
            {
                _playerABtn[_btnIndex].interactable = false;
            }
        }
        else
        {
            _playerBBtnCount[_btnIndex] -= 1;
            _playerBBtnCountText[_btnIndex].text = _playerBBtnCount[_btnIndex].ToString();
            if (_playerBBtnCount[_btnIndex] == 0)
            {
                _playerBBtn[_btnIndex].interactable = false;
            }
        }
        if (GameManager.Instance.Mode == GameMode.OFFLINE)
        {
            ButtonsInteractableOffline();
        }
        else
        {
            ButtonsInteractableOnline();
        }
    }

    private void ButtonsInteractableOffline()
    {
        for (int i = 0; i < _playerABtn.Length; i++)
        {
            _playerABtn[i].interactable = (GameManager.Instance.PlayerTurn == Turn.PLAYER_A && _playerABtnCount[i] > 0) ? true : false;
            _playerBBtn[i].interactable = (GameManager.Instance.PlayerTurn == Turn.PLAYER_B && _playerBBtnCount[i] > 0) ? true : false;
        }
    }

    private void ButtonsInteractableOnline()
    {
        for (int i = 0; i < _playerABtn.Length; i++)
        {
            if (GameManager.Instance.Player == Turn.PLAYER_A)
            {
                _playerABtn[i].interactable = (GameManager.Instance.Player == GameManager.Instance.PlayerTurn && _playerABtnCount[i] > 0) ? true : false;
            }
            else
            {
                _playerBBtn[i].interactable = (GameManager.Instance.Player == GameManager.Instance.PlayerTurn && _playerBBtnCount[i] > 0) ? true : false;
            }
        }
    }

    private void TimerUpdate()
    {
        if (_timeRemainingForPlayerA <= 0 || _timeRemainingForPlayerB <= 0)
        {
            //Game Over
            GameManager.Instance.State = GameState.END;
            GameManager.Instance.Winner = (GameManager.Instance.PlayerTurn == Turn.PLAYER_A) ? Turn.PLAYER_B : Turn.PLAYER_A;
            GameManager.Instance.GameEnded();
            return;
        }

        if (GameManager.Instance.PlayerTurn == Turn.PLAYER_A)
        {
            _timeRemainingForPlayerA -= Time.deltaTime;
            _playerATimer.fillAmount = _timeRemainingForPlayerA / _maxTimeForEachPlayer;
            _playerATimerText.text = Mathf.FloorToInt(_timeRemainingForPlayerA + 1).ToString();
        }
        else
        {
            _timeRemainingForPlayerB -= Time.deltaTime;
            _playerBTimer.fillAmount = _timeRemainingForPlayerB / _maxTimeForEachPlayer;
            _playerBTimerText.text = Mathf.FloorToInt(_timeRemainingForPlayerB + 1).ToString();
        }
    }

    IEnumerator PlayAI()
    {
        yield return new WaitForSecondsRealtime(2);

        GameManager.Instance.PlayerPiece = Piece.BYPASS;
        GameManager.Instance.PlayTurn(null);
    }

    public void OfflineStart()
    {
        if (GameManager.Instance.PlayerTurn == Turn.PLAYER_B && InputManager.isAI)
        {
            StartCoroutine(PlayAI());
        }
        if (_playerANameTextInputField.text == "" )
        {
            return;
        }
        if (_playerBNameTextInputField.text == "" && !InputManager.isAI)
        {
            return;
        }
        
        _playerANameText.text = _playerANameTextInputField.text;
       
            _playerBNameText.text = _playerBNameTextInputField.text;
        if(InputManager.isAI )
        {
            _playerBNameText.text = "AI";
        }
        
        GameManager.Instance.State = GameState.START;
        _playUI.SetActive(true);
        _singleplayerUI.SetActive(false);
        AnimateIn();

        _timeRemainingForPlayerA = _timeRemainingForPlayerB = _maxTimeForEachPlayer;
        _playerATimerText.text = _playerBTimerText.text = _maxTimeForEachPlayer.ToString();
        _playerATimer.fillAmount = _timeRemainingForPlayerA / _maxTimeForEachPlayer;
        _playerBTimer.fillAmount = _timeRemainingForPlayerB / _maxTimeForEachPlayer;

        for (int i = 0; i < _playerABtnCount.Length; i++)
        {
            _playerABtnCount[i] = _playerBBtnCount[i] = _maxEachPieceCount[i];
            _playerABtnCountText[i].text = _playerBBtnCountText[i].text = _maxEachPieceCount[i].ToString();
        }
        Debug.LogWarning(GameManager.Instance.PlayerTurn);
        ButtonsInteractableOffline();
    }

    public void GameOver(bool draw=false)
    {
        StartCoroutine(OverAndCaptureScreenshot(draw));
    }

    IEnumerator OverAndCaptureScreenshot(bool draw)
    {
        Debug.Log(GameManager.Instance.PlayerTurn);
        ButtonsInteractableOffline();
        AnimateOut();
        yield return new WaitForSeconds(1f);

        yield return new WaitForEndOfFrame();
        Texture2D oldtexture = ScreenCapture.CaptureScreenshotAsTexture();
        Texture2D updatedTexture = new Texture2D(oldtexture.width, oldtexture.height, TextureFormat.RGB24, false);
        updatedTexture.SetPixels(oldtexture.GetPixels());
        updatedTexture.Apply();
        Sprite sp = Sprite.Create(updatedTexture, new Rect(0, 0, updatedTexture.width, updatedTexture.height), new Vector2(0.5f, 0.5f));
        _winningScreen.sprite = sp;

        _gameOverUI.SetActive(true);
        _winnerName.text = ((GameManager.Instance.Winner == Turn.PLAYER_A) ? _playerANameText.text : _playerBNameText.text);
        if (draw)
        {
            _winnerName.text = "Match";
            matchResult.text = "DRAW!";
        }
        else
        {
            matchResult.text = "WON!";
        }
        GameManager.Instance.Winner = Turn.NONE;
    }

    public void MenuSetup()
    {
        SetVariables();
        if (GameManager.Instance.Mode == GameMode.ONLINE)
        {
            _multiplayerUI.SetActive(true);
        }
        else
        {
            _singleplayerUI.SetActive(true);
        }
    }

    public void OnlineStart()
    {
        if (_playerNameInput.text == "")
            return;
        else
            SetPlayerName();
        _photonView.RPC(nameof(RPC_SetName), RpcTarget.AllBuffered, new object[] { _playerName });
        if (GameManager.Instance.Player == Turn.PLAYER_A)
        {
            OnlineSetup(_lowerUI, _upperUI, true);
        }
        else
        {
            OnlineSetup(_upperUI, _lowerUI, false);
        }
        _playUI.SetActive(true);
        _multiplayerUI.SetActive(false);
        AnimateIn();
    }

    public void OnlineSetup(RectTransform transformA, RectTransform transformB, bool btnInteractability)
    {
        _playerAUI.transform.SetParent(transformA.transform);
        _playerAName.transform.SetParent(transformA.transform);
        _playerBUI.transform.SetParent(transformB.transform);
        _playerBName.transform.SetParent(transformB.transform);
        for (int i = 0; i < _playerABtn.Length; i++)
        {
            _playerABtn[i].interactable = btnInteractability;
            _playerBBtn[i].interactable = false;
        }
    }

    [PunRPC]
    public void RPC_SetName(string playerName)
    {
        Debug.Log("Called");
        if (GameManager.Instance.Player == Turn.PLAYER_A)
        {
            _playerANameText.text = this._playerName;
            _playerBNameText.text = playerName;
        }
        else
        {
            _playerANameText.text = playerName;
            _playerBNameText.text = this._playerName;
        }
    }

    public void SetPlayerName()
    {
        if(_playerNameInput.text != "")
        {
            _playerName = _playerNameInput.text;
            PlayerPrefs.SetString("PlayerName", _playerName);
            _playerNameText.text = _playerName;
        }
    }

    public void backDefault()
    {
        GameManager.Instance.PlayerTurn = Turn.PLAYER_A;
    }
}
