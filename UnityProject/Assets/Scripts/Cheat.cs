using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheat : MonoBehaviour
{

    private string[] Jump,Invisible;
    private int JumpIndex, InvisibleIndex;
    public Items invisibleItem;

    void Start()
    {

        Jump = new string[] { "j", "u", "m", "p"};
        Invisible = new string[] { "i", "n", "v"};
        JumpIndex = 0;
        
    }

    void Update()
    {
        //=========JUMP=============
        if (Input.anyKeyDown) {

            if (Input.GetKeyDown(Jump[JumpIndex])) {

                JumpIndex++;
            }
     
         else {
                JumpIndex = 0;
            }
        }
        if (JumpIndex == Jump.Length)
        {


            Player.Instance.infinityJump = !Player.Instance.infinityJump;
            JumpIndex = 0;
        }

        //=========INVISIBLE=============

        if (Input.anyKeyDown)
        {

            if (Input.GetKeyDown(Invisible[InvisibleIndex]))
            {

                InvisibleIndex++;
            }

            else
            {
                InvisibleIndex = 0;
            }
        }
        if (InvisibleIndex == Invisible.Length)
        {

            if (Player.Instance.currentItemUsed == Player.ItemBeingUsed.InvisibleStone)
            {
                Material[] mats = BagController.Instance.GetComponentInChildren<Renderer>().materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = BagController.Instance.bartoNormalMat;
                }
                BagController.Instance.GetComponentInChildren<Renderer>().materials = mats;

                Player.Instance.currentItemUsed = Player.ItemBeingUsed.None;

            }
            else
            {
                Inventory.Instance.InvisibleStone(invisibleItem);


            }
    

           
            InvisibleIndex = 0;
        }

    }
}
