using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Items : MonoBehaviour
{
    public enum Item { PedraInvisivel, FrutaBrilha, PlantaRemedio, Engrenagem, Saleiro, Sino, Pote }
    public Item item;

    public float itemDuration;

    public OrbRotation orb;

    [ColorUsage(true, true)]
    public Color blue, purple, orange;

    public GameObject rotateAnimation;
    public GameObject[] itemMeshes;

    public ChatSettings chatConfigPick, chatConfigUse, chatConfigEndDuration;

    [HideInInspector]
    public int weight;

    //[HideInInspector]
    public bool canBePicked, useful;

    public void Start()
    {
        canBePicked = true;
        weight = 1;
        LoadItem(item);
    }

    private void Update()
    {
        PlayerNearby();
    }

    public void LoadItem(Item itemToLoad)
    {
        item = itemToLoad;

        switch (item)
        {
            case Item.PedraInvisivel:
                useful = true;
                break;
            case Item.FrutaBrilha:
                useful = true;
                break;
            default:
                useful = false;
                break;
        }

        if (useful)
        {
            orb.ChangeOrbColor(blue);
        }
        else
        {
            orb.ChangeOrbColor(Color.white);
        }

        if (rotateAnimation.transform.childCount > 0)
        {
            Destroy(rotateAnimation.transform.GetChild(0).gameObject);
        }
        var newItem = Instantiate(itemMeshes[(int)item]);
        newItem.transform.parent = rotateAnimation.transform;
        newItem.transform.localPosition = Vector3.zero;
        newItem.transform.localRotation = Quaternion.identity;
        newItem.transform.localScale = Vector3.one;
        if (!canBePicked)
        {
            ClearChatConfigs();
        }
    }

    public void ClearChatConfigs()
    {
        chatConfigUse.message = new Message[0];
        chatConfigPick.message = new Message[0];
    }

    public void Highlight(bool highlight)
    {
        if (Inventory.Instance.itemStored.Contains(this))
        {
            if (highlight)
            {
                orb.ChangeOrbColor(purple);
            }
            else
            {
                if (useful)
                {
                    orb.ChangeOrbColor(blue);
                }
                else
                {
                    orb.ChangeOrbColor(Color.white);
                }
            }
        }
    }

    public void UseItem(BaseEventData bed)
    {
        if (Inventory.Instance.itemStored.Contains(this))
        {
            PointerEventData ped = (PointerEventData)bed;

            if (ped.pointerId == -1 && Player.Instance.currentItemUsed == Player.ItemBeingUsed.None) // clique do lado esquerdo para usar o item
            {
                if (!useful) // se for um dos itens sem utilidade
                {
                    ChatSystem.Instance.DisplayMessage(chatConfigUse);
                }
                else
                {
                    switch (item)
                    {
                        case Item.PedraInvisivel:
                            Inventory.Instance.InvisibleStone(this);
                            break;
                        case Item.FrutaBrilha:
                            Inventory.Instance.GenericItem(this);
                            break;
                        default:
                            Inventory.Instance.GenericItem(this);
                            break;
                    }

                    Inventory.Instance.itemStored.Remove(this);
                    Inventory.Instance.UpdateExistingItens();
                    Inventory.Instance.UpdateBartoSpikes();
                    BagController.Instance.weight -= weight;
                    Player.Instance.weight += weight;

                    ChatSystem.Instance.DisplayMessage(chatConfigUse);
                    Destroy(gameObject);
                }
            }
            else if (ped.pointerId == -2) // clique do lado direito para dropar o item
            {
                Inventory.Instance.ForceDropItem(this);
                if (useful)
                {
                    orb.ChangeOrbColor(blue);
                }
                else
                {
                    orb.ChangeOrbColor(Color.white);
                }
            }
            else
            {
                if (!Player.Instance.playerSound.isPlaying)
                    Player.Instance.playerSound.PlayOneShot(Player.Instance.errorSound);
            }

            //Inventory.Instance.ExitInventory();
        }
    }

    void PlayerNearby()
    {
        if (canBePicked)
        {
            // se o player tiver com o barto, ele irá checar a distancia do player pro item
            if (BagController.Instance.asBag && Vector3.Distance(Player.Instance.transform.position, transform.position) < 1.5f)
            {
                if (!Inventory.Instance.nearbyItems.Contains(this))
                {
                    Inventory.Instance.nearbyItems.Add(this);
                    Player.Instance.showInteract = true;
                }
            }
            // senão ele irá checar a distancia do barto pro item
            else if (Vector3.Distance(BagController.Instance.transform.position, transform.position) < 1.5f)
            {
                if (!Inventory.Instance.nearbyItems.Contains(this))
                {
                    Inventory.Instance.nearbyItems.Add(this);
                    Player.Instance.showInteract = true;
                }
            }
            else
            {
                if (Inventory.Instance.nearbyItems.Contains(this))
                {
                    Inventory.Instance.nearbyItems.Remove(this);
                    Player.Instance.showInteract = false;
                }
            }
        }
    }
}
