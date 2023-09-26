using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BtnClick : MonoBehaviour
{

    public Sprite OffSprite;
    public Sprite OnSprite;
    public Button but;
    public Color offColor;
    public Color onColor;
    public TextMeshProUGUI tmp;

    public GameObject player2name;

    public GameObject OfflineBoard;



    public void imageChange()
    {
        if (InputManager.isAI == false)
        {
            but.image.sprite = OnSprite;
            InputManager.isAI = true;
            tmp.color = onColor;
            (OfflineBoard.GetComponent("SinglePlayerBoard") as MonoBehaviour).enabled = false;
            (OfflineBoard.GetComponent("AIBoard1") as MonoBehaviour).enabled = true;
            player2name.SetActive(false);

        }
        else
        {
            but.image.sprite = OffSprite;
            InputManager.isAI=false;
            tmp.color = offColor;
            (OfflineBoard.GetComponent("SinglePlayerBoard") as MonoBehaviour).enabled = true; ;
            (OfflineBoard.GetComponent("AIBoard1") as MonoBehaviour).enabled = false;
            player2name.SetActive(true);
         
        }
    }
}
