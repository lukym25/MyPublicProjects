using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable.Collections;

public class ServerOperations : NetworkBehaviour
{
    private PlayersInGameInfo PGI;
    private HpSystem  HPsystem;

    private GameObject MovedPlayer;
    private Rigidbody2D body;

    private int keyNum = 0;
    private InventoryManager IM;

    //private float bulletSpeedServ;

    public NetworkList<GameObject> pandingProjectiles = new NetworkList<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        if(SettingsScript.singlePlayerMode) { Destroy(this); return; }

        PGI = GameObject.FindObjectOfType<PlayersInGameInfo>();
        HPsystem = GameObject.FindObjectOfType<HpSystem>();
    }

    private void FixedUpdate()
    {
        if(!NetworkManager.Singleton.IsHost) { return; }
        /*
        for(int i = 0; i < pandingProjectiles.Count; i++)
        {
            if (pandingProjectiles[i] != null)
            {
                Rigidbody2D rb = pandingProjectiles[i].GetComponent<Rigidbody2D>();

                //rb.MovePosition(rb.transform.position + rb.transform.up * bulletSpeedServ * Time.deltaTime * -1);

                TravelingProjectileClientRpc(i, pandingProjectiles[i].transform.position);
                Debug.Log("servermove");
            } else
            {
                pandingProjectiles.Remove(pandingProjectiles[i]);
            }
        }*/

        //Projectile tracking => send positions to clients

        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");

        Vector3[] positions = new Vector3[projectiles.Length];

        ulong[] networkIds = new ulong[projectiles.Length];

        for (int i = 0; i < projectiles.Length; i++)
        {
            positions[i] = projectiles[i].transform.position;

            networkIds[i] = projectiles[i].GetComponent<NetworkObject>().NetworkObjectId;
        }

        string payload = JsonUtility.ToJson(new ProjectileInfoPayload(networkIds, positions));

        MoveProjectilesClientRpc(payload);
    }

    /*
    Movemant Messaging
    */

    [ServerRpc(RequireOwnership = false)]
    public void MovePlayerServerRpc(ulong plId, Vector2 dir, float movemantSpeed, float cRadius)
    {
        MovedPlayer = PGI.playerDict[plId].gameObject;

        body = MovedPlayer.GetComponent<Rigidbody2D>();

        body.MovePosition(body.position + dir * movemantSpeed * Time.deltaTime);

        Vector3 pos = MovedPlayer.transform.position;

        MovePlayerClientRpc(plId, pos);

        PGI.playerDict[plId].chaseRadius = cRadius;
    }

    [ClientRpc]
    public void MovePlayerClientRpc(ulong plId, Vector3 positoin)
    {
        if(NetworkManager.IsHost) { return; }

        Debug.Log("moving to " + positoin);

        MovedPlayer = PGI.playerDict[plId].gameObject;

        MovedPlayer.transform.position = positoin;
    }

    /*
    Rotationg Messaging
    */

    [ServerRpc(RequireOwnership = false)]
    public void RotatePlayerServerRpc(ulong plId, Vector2 curPos, Vector2 finalDes)
    {
        MovedPlayer = PGI.playerDict[plId].gameObject;
        body = MovedPlayer.GetComponent<Rigidbody2D>();

        HandyScript.LookAt(curPos, finalDes, body);

        Quaternion rot = MovedPlayer.transform.rotation;

        RotatePlayerClientRpc(plId, rot);
    }

    [ClientRpc]
    public void RotatePlayerClientRpc(ulong plId, Quaternion rotation)
    {
        if (NetworkManager.Singleton.IsHost) { return; }
        Debug.Log("rotateOnClient");

        MovedPlayer = PGI.playerDict[plId].gameObject;
        Debug.Log(rotation + "/" + MovedPlayer.transform.rotation);

        MovedPlayer.transform.rotation = rotation;        
    }

    /*
   Animator Messaging
   */

    [ServerRpc(RequireOwnership = false)]
    public void SetMovingAnimServerRpc(ulong plId, bool isMoving)
    {
        SetMovingAnimClientRpc(plId, isMoving);
    }

    [ClientRpc]
    public void SetMovingAnimClientRpc(ulong plId, bool isMoving)
    {
        Debug.Log("changing :" + plId);

        PGI.playerDict[plId].gameObject.GetComponent<Animator>().SetBool("Moving", isMoving);

        if (!isMoving) 
        {
            PGI.playerDict[plId].chaseRadius = 2;
        }
    }

    /*
    Creates Messaging
    */

    [ServerRpc(RequireOwnership = false)]
    public void CreateOpenedServerRpc(Vector3 position)
    {
        CreateOpenedClientRpc(position);
    }

    [ClientRpc]
    public void CreateOpenedClientRpc(Vector3 position)
    {
        CrateScript[] Creates = GameObject.FindObjectsOfType<CrateScript>();

        foreach(CrateScript cr in Creates)
        {
            if(cr.gameObject.transform.position == position)
            {
                cr.opened = true;
                Destroy(cr.gameObject.transform.GetChild(0).gameObject);
            }
        }
    }

    /*
    EscItem Messaging
    */

    [ServerRpc(RequireOwnership = false)]
    public void KeyPickedServerRpc(Vector3 pos)
    {
        keyNum++;
        KeyPickedClientRpc(keyNum, pos);
    }

    [ClientRpc]
    public void KeyPickedClientRpc(int keys, Vector3 position)
    {
        if (IM == null)
        {
            IM = GameObject.FindObjectOfType<InventoryManager>();
        }

        while (keys > IM.numOfEscItems)
        {
            IM.numOfEscItems++;
            IM.KeyColleted();
        }

        GameObject[] keysObj = GameObject.FindGameObjectsWithTag("Key");

        foreach (GameObject keyObj in keysObj)
        {
            if (keyObj.transform.position == position)
            {
                keyObj.SetActive(false);
            }
        }
    }

    /*
    PlayerAttack Messaging
    */

    [ServerRpc(RequireOwnership = false)]
    public void GunFiredServerRpc(ulong id, float randAngle, float bulletSpeed)
    {
        GameObject gunModel = PGI.playerDict[id].gameObject.transform.GetChild(1).gameObject;

        Transform FirePoint = gunModel.transform.GetChild(0).transform;

        Debug.Log(FirePoint.name);

        GunList[] EveryGun = FindObjectOfType<GunManager>().startingGuns;

        Debug.Log(gunModel.name.Remove(gunModel.name.Length - 8));

        Debug.Log("ServerAttack");
        foreach (GunList GL in EveryGun)
        {
            if(GL.nameOfGun == gunModel.name.Remove(gunModel.name.Length - 8))
            {
                GameObject newProjectile = PlayerAttack.FireGun(GL.AmmoPrefab, FirePoint, randAngle, bulletSpeed);
                newProjectile.GetComponent<NetworkObject>().Spawn();

                GunFiredClientRpc();
                Debug.Log("plserAttackcommplete");
                return;
            }
        }
    }

    [ClientRpc]
    public void GunFiredClientRpc()
    {
        FindObjectOfType<audioManagerScript>().PlaySound("fire");
    }

    [ClientRpc]
    public void MoveProjectilesClientRpc(string payloadInJson)
    {
        if(NetworkManager.Singleton.IsHost) { return; } 

        ProjectileInfoPayload payload = JsonUtility.FromJson<ProjectileInfoPayload>(payloadInJson);

        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");

        for(int i = 0; i < payload.ids.Length; i++)
        {
            foreach (GameObject GO in projectiles)
            {
                if(GO.GetComponent<NetworkObject>().NetworkObjectId == payload.ids[i])
                {
                    GO.transform.position = payload.pos[i];
                    break;
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DealDamageServerRpc(string nameHit, int damageAmout)
    {
        Debug.Log("hit");

        DealDamageClientRpc(nameHit, damageAmout);        
    }

    [ClientRpc]
    public void DealDamageClientRpc(string name, int damageAmout)
    {
        HPsystem.Damage(name, damageAmout);
    }
}

public class ProjectileInfoPayload
{
    public ulong[] ids;
    public Vector3[] pos;

    public ProjectileInfoPayload(ulong[] inputIds, Vector3[] inputPos)
    {
        ids = inputIds;
        pos = inputPos;
    }
}

