using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Items> itemStored;

    public static Inventory Instance { get; set; }

    public Transform dropItemPlayer, dropItemBarto;

    // para os spikes acenderem
    public Material bartoMaterial;
    public Texture[] bartoEmissive;

    public Transform inventoryHolder;
    public Transform[] itemPos;

    // cam Var
    public Transform camBag, camGround;
    Vector3 desiredBagPos, desiredGroundPos;
    Quaternion desiredBagRot, desiredGroundRot;
    Transform camToSet;
    public LayerMask ignoraCamCollision;
    public float cooldownOpenInv;

    public GameObject[] npcs;
    public List<Items> nearbyItems;
    float itemMinDistance = 999;
    public ChatSettings fullChat;

    void Start()
    {
        Instance = this;

        UpdateBartoSpikes();
        ignoraCamCollision = ~ignoraCamCollision;

        desiredBagPos = camBag.localPosition;
        desiredBagRot = camBag.localRotation;

        desiredGroundPos = camGround.localPosition;
        desiredGroundRot = camGround.localRotation;

        cooldownOpenInv = 0;
        npcs = GameObject.FindGameObjectsWithTag("NPC");
    }

    void Update()
    {
        // setar a camera do inventario
        if (BagController.Instance.forceOpenInvent)
        {
            TransformCamera(camToSet);
        }

        // abre ou fecha o inventário
        if (Input.GetKeyDown(KeyCode.Tab) && cooldownOpenInv <= 0 && !Player.Instance.kimTutorial)
        {
            cooldownOpenInv = 1;
            if (!BagController.Instance.forceOpenInvent)
            {
                OpenInventory();
            }
            else
            {
                ExitInventory();
            }
        }

        if (cooldownOpenInv > 0)
        {
            cooldownOpenInv -= Time.deltaTime;
        }

        // mecânica de drop e pickup
        PickUpDropItem();
    }

    #region OpenExitInventory

    void OpenInventory()
    {
        // condições p/ abrir o inventário: Barto vivo, podendo se mexer, enquanto estiver ou sendo controlado ou como mochila
        if (BagController.Instance.bartoCurrentState != BagController.BartoStates.Death && !BagController.Instance.forceBartoStop
            && (BagController.Instance.asBag || BagController.Instance.canControll))
        {
            RaycastHit hit;

            if (BagController.Instance.asBag && !Player.Instance.isGrounded) // se o player estiver pulando, então o inventário não abrirá
            {
                return;
            }
            else if (BagController.Instance.asBag && Physics.Linecast(BagController.Instance.transform.position, camBag.position, out hit, ignoraCamCollision)) // se a cam da bag colidir com algo
            {
                //Debug.Log(hit.transform.name); //-- emitir um som de erro futuramente
                if (!Player.Instance.playerSound.isPlaying)
                    Player.Instance.playerSound.PlayOneShot(Player.Instance.errorSound);
                return;
            }
            else if (Physics.Linecast(BagController.Instance.transform.position, camGround.position, out hit, ignoraCamCollision) && !BagController.Instance.asBag)
            {
                // Debug.Log(hit.transform.name); //-- emitir um som de erro futuramente
                if (!Player.Instance.playerSound.isPlaying)
                    Player.Instance.playerSound.PlayOneShot(Player.Instance.errorSound);
                return;
            }
            else if (!BagController.Instance.bartoNavMesh.enabled) // desde que a navmesh não esteja ativada
            {
                BagController.Instance.forceOpenInvent = true;
                BagController.Instance.forceExitInvent = false;

                BagController.Instance.forceBartoStop = true;
                Player.Instance.forceKimStop = true;

                Player.Instance.ForceAnimation(true, Player.PlayerStates.IdleBreath);
                Player.Instance.rb.velocity = Vector3.zero;

                inventoryHolder.gameObject.SetActive(true);

                if (BagController.Instance.asBag)
                {
                    camToSet = camBag;
                    camBag.position = CameraController.Instance.transform.position;
                    camBag.rotation = CameraController.Instance.transform.rotation;
                    camBag.gameObject.SetActive(true);
                }
                else
                {
                    camToSet = camGround;
                    camGround.position = CameraController.Instance.transform.position;
                    camGround.rotation = CameraController.Instance.transform.rotation;
                    camGround.gameObject.SetActive(true);
                }

                CameraController.Instance.lockCamera = true;
                CameraController.Instance.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    public void ExitInventory()
    {
        BagController.Instance.forceOpenInvent = false;
        BagController.Instance.forceExitInvent = true;

        BagController.Instance.forceBartoStop = false;
        Player.Instance.forceKimStop = false;

        Player.Instance.ForceAnimation(false);

        inventoryHolder.gameObject.SetActive(false);

        camBag.gameObject.SetActive(false);
        camGround.gameObject.SetActive(false);

        CameraController.Instance.lockCamera = false;
        CameraController.Instance.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void TransformCamera(Transform camera)
    {
        float timeToLerp = 1f;

        if (camera == camBag)
        {
            camera.transform.localPosition = Vector3.Lerp(camera.localPosition, desiredBagPos, Time.deltaTime * timeToLerp);
            camera.transform.localRotation = Quaternion.Lerp(camera.localRotation, desiredBagRot, Time.deltaTime * timeToLerp);
        }
        else if (camera == camGround)
        {
            camera.transform.localPosition = Vector3.Lerp(camera.localPosition, desiredGroundPos, Time.deltaTime * timeToLerp);
            camera.transform.localRotation = Quaternion.Lerp(camera.localRotation, desiredGroundRot, Time.deltaTime * timeToLerp);
        }
    }

    #endregion

    #region PickUpDrop

    bool NpcAround()
    {
        for (int i = 0; i < npcs.Length; i++)
        {
            if (Vector3.Distance(Player.Instance.transform.position, npcs[i].transform.position) < 3)
            {
                return true;
            }
        }
        return false;
    }

    bool CanDropOrPickUp()
    {
        return !BagController.Instance.forceBartoStop && !Player.Instance.forceKimStop && !ChatSystem.Instance.chatAnimator.GetBool("chatOn") && !NpcAround();
    }

    public void PickUpDropItem()
    {
        if (Input.GetKeyDown(KeyCode.F) && CanDropOrPickUp())
        {
            // se houver itens próximos e o inventário estiver cheio
            if (nearbyItems.Count > 0 && itemStored.Count == 4 && (BagController.Instance.asBag || CameraController.Instance.cameraFocus == BagController.Instance.transform))
            {
                ChatSystem.Instance.DisplayMessage(fullChat);
            }
            // se não houver nenhum item perto ou inventário estiver cheio ENQUANTO houver item no inventário, ele dropa
            else if ((nearbyItems.Count == 0 || itemStored.Count == 4) && itemStored.Count > 0)
            {
                // só pode dropar SE o barto estiver com o player enquanto o player estiver no chão --- OU --- se o barto estiver sozinho enquanto a camera foca ele
                if ((BagController.Instance.asBag && Player.Instance.isGrounded) || (!BagController.Instance.asBag && CameraController.Instance.cameraFocus == BagController.Instance.transform))
                {
                    Items itemToDrop = null;
                    itemToDrop = itemStored[Random.Range(0, itemStored.Count)];

                    if (itemToDrop)
                    {
                        ForceDropItem(itemToDrop);
                    }
                }
            }
            else if (itemStored.Count < 4 && nearbyItems.Count > 0) // se houver espaço no inventário e tiver algum item por perto
            {
                // caso eo barto esteja com o player --- OU --- o barto esteja sozinho enquanto a camera foca no barto
                if ((BagController.Instance.asBag) || (!BagController.Instance.asBag && CameraController.Instance.cameraFocus == BagController.Instance.transform))
                {
                    Items itemToPick = null;

                    foreach (Items item in nearbyItems)
                    {
                        if (Vector3.Distance(item.transform.position, transform.position) < itemMinDistance)
                        {
                            itemMinDistance = Vector3.Distance(transform.position, item.transform.position);
                            itemToPick = item;
                        }
                    }

                    itemToPick.transform.parent = itemPos[itemStored.Count];
                    itemStored.Add(itemToPick);
                    itemToPick.canBePicked = false;

                    itemToPick.transform.localPosition = Vector3.zero;
                    itemToPick.transform.localRotation = Quaternion.Euler(0, 0, 40); // valor teste p/ o item ficar rotacionado
                    itemToPick.transform.localScale = Vector3.one;

                    BagController.Instance.weight += itemToPick.weight;
                    BagController.Instance.forcePickUp = true;

                    nearbyItems.Remove(itemToPick);

                    //Player.Instance.forcePickUp = true;
                    //Vector3 dirRelative = Player.Instance.transform.position - itemToPick.transform.position;
                    //dirRelative.y = 0;
                    //Player.Instance.desiredRotation = Quaternion.LookRotation(dirRelative);

                    ChatSystem.Instance.DisplayMessage(itemToPick.chatConfigPick);
                    itemMinDistance = 999;
                    itemToPick = null;

                    Player.Instance.showInteract = false;

                    UpdateBartoSpikes();
                }
            }
            else // caso dê errado, um som de erro será mostrado
            {
                if (!Player.Instance.playerSound.isPlaying)
                {
                    Player.Instance.playerSound.PlayOneShot(Player.Instance.errorSound);
                }
            }
        }
    }

    public void ForceDropItem(Items itemToDrop) // eu fiz em um método separado p/ poder chamar no clique direito ao abrir inventário (class Items)
    {
        if (Player.Instance.transform.parent == null)
        {
            itemToDrop.transform.parent = null;
        }
        else
        {
            itemToDrop.transform.parent = Player.Instance.transform.parent;
        }

        itemToDrop.transform.localScale = Vector3.one;
        Quaternion rotationToGo = itemToDrop.transform.rotation;
        rotationToGo.x = 0;
        rotationToGo.z = 0;
        itemToDrop.transform.rotation = rotationToGo;

        if (BagController.Instance.canControll)
        {
            itemToDrop.transform.position = dropItemBarto.position + (Vector3.up * (itemToDrop.transform.localScale.y / 2));
        }
        else
        {
            itemToDrop.transform.position = dropItemPlayer.position + (Vector3.up * (itemToDrop.transform.localScale.y / 2));
        }

        BagController.Instance.weight -= itemToDrop.weight;
        itemStored.Remove(itemToDrop);
        BagController.Instance.forceDrop = true;

        itemToDrop.canBePicked = true;
        itemToDrop.chatConfigPick.message = new Message[0];

        UpdateExistingItens();
        UpdateBartoSpikes();
    }

    #endregion

    // bool para checar se eu tenho X item no meu inventário
    public bool CheckItem(Items.Item checkItem)
    {
        foreach (Items listItem in itemStored)
        {
            if (listItem.item == checkItem)
            {
                return true;
            }
        }

        return false;
    }

    // método para trocar um item do inventário com outro item
    public void SwapItem(Items.Item originalItem, Items.Item swappedItem)
    {
        bool canTrade = true;
        foreach (Items itemCheck in itemStored)
        {
            if (itemCheck.item == originalItem && canTrade) // se eu tenho o item que vc quer
            {
                itemCheck.LoadItem(swappedItem);
                canTrade = false;
            }
        }
    }

    // aqui eu faço os itens assumirem novos parents conforme sua ordem na lista quando o player dropar ou usar algum item
    public void UpdateExistingItens()
    {
        if (itemStored.Count > 0)
        {
            for (int i = 0; i < itemStored.Count; i++)
            {
                itemStored[i].transform.parent = itemPos[i].transform;
                itemStored[i].transform.localPosition = Vector3.zero;
            }
        }
    }

    // atualizo os espinhos nas costas do barto conforme a quantidade de itens no inventário
    public void UpdateBartoSpikes()
    {
        switch (itemStored.Count)
        {
            case 0:
                bartoMaterial.DisableKeyword("_EMISSION");
                break;
            default:
                bartoMaterial.EnableKeyword("_EMISSION");
                bartoMaterial.SetTexture("_EmissionMap", bartoEmissive[itemStored.Count - 1]);
                break;
        }
    }

    // poderes especiais dos itens
    #region ItemsUsabilities 

    public void GenericItem(Items item)
    {
        Player.Instance.weight -= item.weight;
    }

    public void InvisibleStone(Items item)
    {
        StopCoroutine(ReturnFromInvisible(item));
        Player.Instance.currentItemUsed = Player.ItemBeingUsed.InvisibleStone;

        Material[] mats = BagController.Instance.GetComponentInChildren<Renderer>().materials;
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i] = BagController.Instance.bartoInviMat;
        }
        BagController.Instance.GetComponentInChildren<Renderer>().materials = mats;
        Player.Instance.weight -= item.weight;

        StartCoroutine(ReturnFromInvisible(item));
    }

    IEnumerator ReturnFromInvisible(Items item)
    {
        yield return new WaitForSeconds(item.itemDuration);

        Material[] mats = BagController.Instance.GetComponentInChildren<Renderer>().materials;
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i] = BagController.Instance.bartoNormalMat;
        }
        BagController.Instance.GetComponentInChildren<Renderer>().materials = mats;

        Player.Instance.currentItemUsed = Player.ItemBeingUsed.None;

        ChatSystem.Instance.DisplayMessage(item.chatConfigEndDuration);
    }

    #endregion
}
