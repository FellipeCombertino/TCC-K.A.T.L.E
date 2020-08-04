using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ShowDirection : MonoBehaviour
{
    // Start is called before the first frame update
    public CameraController cam;
    public float radius;
    public GameObject arrow;
    
    public float angle;
    public float height;
   
    public Color minColor;
    public Color mediumColor;
    public Color maxColor;

    public float minPosToShow;
    public float mediumDistance;
    public float maxDistance;

    public float distance;
    public Material mat;

    static public ShowDirection Instance;

    void Start()
    {
        Instance = this;
        mat.color = new Color(minColor.r, minColor.g, minColor.b,0);
    }


    private void LateUpdate()
    {
        distance = Vector3.Distance(BagController.Instance.transform.position, Player.Instance.transform.position);

    

       
        if (!BagController.Instance.asBag)
        {
            if (!BagController.Instance.canBeCalled)
            {

                mat.color = Color.Lerp(mat.color, maxColor, Time.deltaTime * 3);

            }
            else
            {

                if (distance > minPosToShow)
                {
                    if (distance < mediumDistance)
                    {

                        mat.color = Color.Lerp(mat.color, minColor, Time.deltaTime * 3);

                    }
                    else
                    {
                        if (distance < maxDistance)
                        {

                            mat.color = Color.Lerp(mat.color, mediumColor, Time.deltaTime * 3);

                        }
                        else if (distance > maxDistance)
                        {


                            mat.color = Color.Lerp(mat.color, maxColor, Time.deltaTime * 3);

                        }
                    }
                }
            }



            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, Mathf.Lerp(mat.color.a,1,Time.deltaTime*5));

            if (Player.Instance.canControll)
            {

                arrow.transform.parent = transform.GetChild(0);
                arrow.transform.LookAt(new Vector3(BagController.Instance.transform.position.x, arrow.transform.position.y, BagController.Instance.transform.position.z));

            }
            else
            {


                arrow.transform.parent = BagController.Instance.transform;
                arrow.transform.LookAt(new Vector3(Player.Instance.transform.position.x,arrow.transform.position.y, Player.Instance.transform.position.z));

            }

            arrow.transform.localPosition = Vector3.zero;
            



        }
        else
        {

            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, Mathf.Lerp(mat.color.a, 0, Time.deltaTime * 5));

        }

    }
   
   
}
