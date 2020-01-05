using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform PlayerSpawnPoint1;
    public Transform PlayerSpawnPoint2;

    public PlayerShip PlayerShipPrefab;
    public LightController LightControllerPrefab;
    public Light PlayerLightPrefab;

    public PlayerShip SpawnPlayer(PlayerInfo playerInfo)
    {
        PlayerShip ship = Instantiate(PlayerShipPrefab, PlayerSpawnPoint1.position, Quaternion.identity);
        LightController lightController = Instantiate(LightControllerPrefab, Vector3.zero, Quaternion.identity);
        Light light = Instantiate(PlayerLightPrefab, PlayerLightPrefab.transform.position, Quaternion.identity);
        lightController.Init(ship, light);

        ship.Init(playerInfo);

        return ship;
    }
}