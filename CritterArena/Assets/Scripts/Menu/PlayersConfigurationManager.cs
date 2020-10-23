using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayersConfigurationManager : MonoBehaviour
{
    public static PlayersConfigurationManager Instance { get; private set; }

    private List<PlayerConfig> playersConfigs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            playersConfigs = new List<PlayerConfig>();
        }
    }

    public List<PlayerConfig> GetPlayerConfigs()
    {
        return playersConfigs;
    }

    public void SetPlayerCharacter(int index, GameObject character)
    {
        playersConfigs[index].SetCharacter(character);
    }

    public void SetPlayerReady(int index)
    {
        playersConfigs[index].IsReady = true;
        if (playersConfigs.All(p => p.IsReady == true))
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        Debug.Log($"Player joined {pi.playerIndex}");
        if (!playersConfigs.Any(p => p.Index == pi.playerIndex))
        {
            pi.transform.SetParent(transform);
            playersConfigs.Add(new PlayerConfig(pi));
        }
    }
}

public class PlayerConfig
{
    public PlayerInput Input { get; set; }
    public int Index { get; private set; }

    private GameObject character;
    public bool IsReady { get; set; }

    public PlayerInputHandler InputHandler { get; set; }


    public PlayerConfig(PlayerInput pi)
    {
        Input = pi;
        Index = pi.playerIndex;
        InputHandler = pi.gameObject.GetComponent<PlayerInputHandler>();
        IsReady = false;
    }

    public void SetCharacter(GameObject character)
    {
        this.character = character;
    }

    public GameObject GetCharacter()
    {
        return character;
    }
}
