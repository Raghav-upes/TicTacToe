using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera _playerA_Cam, _playerB_Cam;

    private void Start()
    {
        GameManager.Instance.OnlineStart += CameraSetup;
    }
    //private void OnEnable() => GameManager.Instance.GameSetups += CameraSetup;
    private void OnDisable() => GameManager.Instance.OnlineStart -= CameraSetup;

    public void CameraSetup()
    {
        if(GameManager.Instance.Mode == GameMode.ONLINE)
        {
            if (GameManager.Instance.Player == Turn.PLAYER_A)
            {
                _playerA_Cam.Priority = 1;
                _playerB_Cam.Priority = 0;
            }
            else
            {
                _playerA_Cam.Priority = 0;
                _playerB_Cam.Priority = 1;
            }
        }
    }
}
