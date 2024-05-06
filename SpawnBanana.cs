using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SpawnBanana : NetworkBehaviour
{
    private BasicSpawner basicSpawner;

    private void Start()
    {
        basicSpawner = GameObject.FindObjectOfType(typeof(BasicSpawner)) as BasicSpawner;
    }

    public override void FixedUpdateNetwork()
    {
        //rotate the coin
        gameObject.transform.Rotate(0, 1, 0, Space.Self);
    }

    // ask server to destroy and spawn new coin
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //ask server to despawn old coin and spawn new coin
            Vector3 rand_pos = new Vector3(Random.Range(-10, 10), 1f, Random.Range(-10, 10));

            basicSpawner.CollideWithBanana(rand_pos);

            //make calculation on player's coin number
            PlayerController player = other.GetComponent<PlayerController>();
            player.LossCoin(1);

        }
    }
}

