using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSetupeMenuController : MonoBehaviour
{
    private int playerIndex;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject readyPanel;
    [SerializeField] private Button readyButton;

    private float ignoreInputTime = 1;
    private bool inputEnable = false;

    public void SetPlayerIndex(int index)
    {
        playerIndex = index;
        titleText.SetText($"Player {(index + 1)}");
        ignoreInputTime += Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > ignoreInputTime)
        {
            inputEnable = true;
        }
    }

    public void SetCharacter(GameObject character)
    {
        if (!inputEnable)
        {
            return;
        }

        PlayersConfigurationManager.Instance.SetPlayerCharacter(playerIndex, character);
        readyPanel.SetActive(true);
        readyButton.Select();
        menuPanel.SetActive(false);
    }

    public void ReadyPlayer()
    {
        if (!inputEnable)
        {
            return;
        }

        PlayersConfigurationManager.Instance.SetPlayerReady(playerIndex);
        readyButton.gameObject.SetActive(false);
    }
}
