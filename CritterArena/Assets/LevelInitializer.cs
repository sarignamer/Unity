using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInitializer : MonoBehaviour
{
    [SerializeField] private Transform[] playerSpawnPoints;
    [SerializeField] private GameObject PlayerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        PlayerConfig[] playerConfigs = PlayersConfigurationManager.Instance.GetPlayerConfigs().ToArray();
        for (int i = 0; i < playerConfigs.Length; i++)
        {
            GameObject player = Instantiate(PlayerPrefab, playerSpawnPoints[i].position, playerSpawnPoints[i].rotation, gameObject.transform);
            player.name = $"Player{i + 1}";
            player.GetComponent<Player>().InitializePlayer(playerConfigs[i].GetCharacter(), playerConfigs[i].InputHandler);
        }
    }
}
