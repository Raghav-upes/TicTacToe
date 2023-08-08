using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class NetworkManager : MonoBehaviourPunCallbacks
{
	[SerializeField]
	private UIController _UIController;
	[SerializeField]
	private MultiplayerBoard _multiplayerBoard;

	private PhotonView _photonView;

    private void Awake()
    {
		_photonView = GetComponent<PhotonView>();
		PhotonNetwork.AutomaticallySyncScene = true;
		PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnEnable()
    {
        base.OnEnable();
		GameManager.Instance.GameOver += KickPlayers;
	}

    public override void OnDisable()
	{
		base.OnDisable();
		GameManager.Instance.GameOver -= KickPlayers;
	}

    public void Connect()
	{
		if (PhotonNetwork.IsConnected)
		{
			PhotonNetwork.JoinRandomRoom(null, 2);
		}
		else
		{
			PhotonNetwork.ConnectUsingSettings();
		}
	}

	public override void OnConnectedToMaster()
	{
		Debug.LogError($"Connected to server. Looking for random room with level");
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.LogError($"Joining random room failed becuse of {message}. Creating new one with player level");
        var roomPorperties = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 2
        };
		string s = Random.Range(0, 5555).ToString();
        PhotonNetwork.CreateRoom(s, roomPorperties);
    }

	public void CreateRoomWithName()
    {
		if (!PhotonNetwork.IsConnected)
        {
			PhotonNetwork.ConnectUsingSettings();
			return;
		}
		if (_UIController._createRoomNameText.text != "")
        {
			var roomPorperties = new RoomOptions()
			{
				IsVisible = false,
				IsOpen = true,
				MaxPlayers = 2
			};
			PhotonNetwork.CreateRoom(_UIController._createRoomNameText.text, roomPorperties);
        }
    }

	public void JoinRoomWithName()
    {
		if (!PhotonNetwork.IsConnected)
		{
			PhotonNetwork.ConnectUsingSettings();
			return;
		}
		if (_UIController._joinRoomNameText.text != "")
        {
			PhotonNetwork.JoinRoom(_UIController._joinRoomNameText.text);
        }
	}

	public override void OnJoinedRoom()
	{
		Debug.LogError($"Player {PhotonNetwork.LocalPlayer.ActorNumber} joined a room with level: {PhotonNetwork.CurrentRoom.CustomProperties}");
		SetupRoom();
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		Debug.LogError($"Player {newPlayer.ActorNumber} entered a room");
		GameManager.Instance.State = GameState.START;
	}

	public override void OnPlayerLeftRoom(Player player)
    {
		if (player.IsInactive)
			return;
		Debug.LogError($"Player {player.ActorNumber} left room");
		if (GameManager.Instance.State == GameState.START)
        {
			Debug.Log("Game Ended Called");
			GameManager.Instance.State = GameState.END;
			GameManager.Instance.Winner = GameManager.Instance.Player;
			GameManager.Instance.GameEnded();
		}
    }

	internal bool IsRoomFull()
	{
		return PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers;
	}

	public void KickPlayers()
    {
		if (GameManager.Instance.Mode == GameMode.ONLINE)
        {
			_photonView.RPC(nameof(RPC_KickPlayers), RpcTarget.AllBuffered, new object[] { });
		}
    }

	[PunRPC]
	public void RPC_KickPlayers()
    {
		PhotonNetwork.LeaveRoom();
    }

	private void SetupRoom()
    {
		GameManager.Instance.Mode = GameMode.ONLINE;
		if (PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			GameManager.Instance.Player = Turn.PLAYER_A;
		}
		else
		{
			GameManager.Instance.Player = Turn.PLAYER_B;
			GameManager.Instance.State = GameState.START;
		}
		GameManager.Instance.OnlineStartSet();
		_multiplayerBoard.GenerateBoard();
	}
}
