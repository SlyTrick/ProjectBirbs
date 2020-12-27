using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class FeatherHoarderController : ModeController
{
    private float counter = 0;
    private float featherBounds = 3;
    private float featherRate;
    private GameObject featherPrefab;
    private int generatedFeathers = 0;
    private int MAX_FEATHERS = 20;

    public PhotonView PVMatchController;

    public FeatherHoarderController(MatchController matchController) : base(matchController)
    {
        featherRate = matchController.featherRate;
        featherPrefab = matchController.featherPrefab;
        PVMatchController = matchController.PV;
    }
    // Start is called before the first frame update

    public override void Update()
    {
        base.Update();
        if (generatedFeathers < MAX_FEATHERS)
        {
            counter += Time.deltaTime;
            if (counter >= featherRate)
            {
                generatedFeathers++;
                SpawnFeather();
                counter = 0;
            }
        }
    }

    public override void PlayerKilled(Character victim, Character killer)
    {
        base.PlayerKilled(victim, killer);
        if (PhotonNetwork.IsConnected)
        {
            int lostFeathers = victim.LoseFeathers();
            matchController.SubstractPoints(victim, lostFeathers);
            for (int i = 0; i < lostFeathers; i++) { 
                float Posx = Random.Range(victim.transform.position.x - 0.5f, victim.transform.position.x + 0.5f);
                float Posz = Random.Range(victim.transform.position.z - 0.5f, victim.transform.position.z + 0.5f);
                float Dirx = Random.Range(-1.0f, 1.0f);
                float Dirz = Random.Range(-1.0f, 1.0f);
                PVMatchController.RPC("SpawnLostFeather_RPC", RpcTarget.All, Posx, Posz, Dirx, Dirz);
            }
        }
        else
        {
            int lostFeathers = victim.LoseFeathers();

            //Debug.Log(lostFeathers);
            matchController.SubstractPoints(victim, lostFeathers);
            for (int i = 0; i < lostFeathers; i++)
            {
                Vector3 spawnDir = new Vector3(
                    Random.Range(-1.0f, 1.0f),
                    0,
                    Random.Range(-1.0f, 1.0f)
                );
                Vector3 spawnPos = new Vector3(
                    Random.Range(victim.transform.position.x - 0.5f, victim.transform.position.x + 0.5f),
                    0,
                    Random.Range(victim.transform.position.z - 0.5f, victim.transform.position.z + 0.5f)
                );
                FeatherController feather = Object.Instantiate(featherPrefab, spawnPos, Quaternion.Euler(spawnDir)).GetComponent<FeatherController>();

                feather.rigidBody.AddForce(spawnDir * feather.acceleration * Time.fixedDeltaTime, ForceMode.Impulse);
            }
        }
    }
    public override void UpdateFeederScore(Character target)
    {
        base.UpdateFeederScore(target);
        // Si por alguna razon alguien acaba en un comedero en este modo pues no suma puntos
    }
    public override void AddFeather(Character target)
    {
        if (PhotonNetwork.IsConnected && PVMatchController.IsMine)
        {
            generatedFeathers--;

            target.PV.RPC("AddFeather_RPC", RpcTarget.All);
            matchController.AddPoints(target, 1);
        }
        else
        {
            base.AddFeather(target);
            generatedFeathers--;

            target.AddFeather();
            matchController.AddPoints(target, 1);
        }
    }
    private void SpawnFeather()
    {
        if (PhotonNetwork.IsConnected)
        {
            BoxCollider spawnPoint = matchController.GetFeatherSpawn();
            float x = Random.Range(spawnPoint.bounds.min.x, spawnPoint.bounds.max.x);
            float z = Random.Range(spawnPoint.bounds.min.z, spawnPoint.bounds.max.z);
            PVMatchController.RPC("SpawnFeather_RPC", RpcTarget.All, x, z);
        }
        else
        {
            BoxCollider spawnPoint = matchController.GetFeatherSpawn();
            Vector3 spawnPos = new Vector3(
                Random.Range(spawnPoint.bounds.min.x, spawnPoint.bounds.max.x),
                0,
                Random.Range(spawnPoint.bounds.min.z, spawnPoint.bounds.max.z)
            );
            Object.Instantiate(featherPrefab, spawnPos, Quaternion.identity);
        }
    }
}
