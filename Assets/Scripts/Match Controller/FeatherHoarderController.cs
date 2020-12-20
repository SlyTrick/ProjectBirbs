using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatherHoarderController : ModeController
{
    private float counter = 0;
    private float featherBounds = 3;
    private float featherRate;
    private GameObject featherPrefab;
    private int generatedFeathers = 0;
    private int MAX_FEATHERS = 20;
    public FeatherHoarderController(MatchController matchController) : base(matchController)
    {
        featherRate = matchController.featherRate;
        featherPrefab = matchController.featherPrefab;
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
    public override void UpdateFeederScore(Character target)
    {
        base.UpdateFeederScore(target);
        // Si por alguna razon alguien acaba en un comedero en este modo pues no suma puntos
    }
    public override void AddFeather(Character target)
    {
        base.AddFeather(target);
        generatedFeathers--;

        target.AddFeather();
        matchController.AddPoints(target, 1);
    }
    private void SpawnFeather()
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
