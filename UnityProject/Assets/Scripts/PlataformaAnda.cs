using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformaAnda : MonoBehaviour
{

    public enum eixo {x,y,z}
    public eixo Eixo;
    public float Distance;
    public bool indo;
    public float velocity;
    public Vector3 startPos;
    public Vector3 AtualVelocity;
    public Transform Parent;
    public bool kimIsIn,bartoIsIn;
    bool started;
    public BagController barto;
    public Player Kim;
    
    // Start is called before the first frame update
    void Start()
    {

        startPos = transform.position;
        started = true;
        var scale = transform.localScale;
        scale.x = 1/ transform.localScale.x ;
        scale.y = 1/ transform.localScale.y ;
        scale.z = 1/ transform.localScale.z ;
        Parent.transform.localScale = scale;
    }

    // Update is called once per frame
    void Update()
    {

        switch (Eixo)
        {
            case eixo.x:
                if (indo)
                {
                    transform.position = new Vector3(transform.position.x+ velocity*Time.deltaTime, transform.position.y, transform.position.z);
                    AtualVelocity = new Vector3(velocity*Time.deltaTime,0, 0);
                }
                else
                {

                    transform.position = new Vector3(transform.position.x - velocity * Time.deltaTime, transform.position.y, transform.position.z);
                    AtualVelocity = new Vector3(-velocity * Time.deltaTime, 0, 0);
                }

                if(startPos.x + Distance/2 < transform.position.x && indo)
                {
                    indo = false;

                }
                else
                {
                    if(startPos.x - Distance/2 > transform.position.x && !indo)
                    {

                        indo = true;
                    }


                }


                break;
            case eixo.y:

                if (indo)
                {
                    transform.position = new Vector3(transform.position.x,transform.position.y + velocity * Time.deltaTime, transform.position.z);
                    AtualVelocity = new Vector3(0, velocity * Time.deltaTime, 0);
                }
                else
                {

                    transform.position = new Vector3(transform.position.x,transform.position.y - velocity * Time.deltaTime, transform.position.z);
                    AtualVelocity = new Vector3(0, -velocity * Time.deltaTime, 0);
                }

                if (startPos.y + Distance / 2 < transform.position.y && indo)
                {
                    indo = false;

                }
                else
                {
                    if (startPos.y - Distance / 2 > transform.position.y && !indo)
                    {

                        indo = true;
                    }


                }

                RaycastHit hit;
                if (Physics.SphereCast(transform.position, 1, Vector3.down, out hit, 1))
                {
                    if (hit.transform.CompareTag("Player") && !indo)
                    {
                        indo = true;
                    }
                }

                break;
            case eixo.z:
                if (indo)
                {
                    transform.position = new Vector3(transform.position.x,transform.position.y, transform.position.z + velocity * Time.deltaTime);
                    AtualVelocity = new Vector3(0, 0, velocity * Time.deltaTime);
                }
                else
                {

                    transform.position = new Vector3(transform.position.x , transform.position.y, transform.position.z - velocity * Time.deltaTime);
                    AtualVelocity = new Vector3(0, 0, -velocity * Time.deltaTime);
                }

                if (startPos.z + Distance / 2 < transform.position.z && indo)
                {
                    indo = false;

                }
                else
                {
                    if (startPos.z - Distance / 2 > transform.position.z && !indo)
                    {

                        indo = true;
                    }


                }

                break;
            default:
                break;
        }



        if(kimIsIn)
        {

          
            Player.Instance.transform.parent = Parent;
           

        }


    }

    
    private void OnTriggerEnter(Collider other)
    {
        
        if(other.transform.GetComponent<Player>() == Player.Instance)
        {

            kimIsIn = true;

        }
        if (other.transform.GetComponent<BagController>() == BagController.Instance)
        {

            bartoIsIn = true;

        }

    }


    private void OnTriggerExit(Collider other)
    {

       
        if (other.transform.GetComponent<Player>() == Player.Instance)
        {
            Player.Instance.transform.parent = null;
            kimIsIn = false;
        }
     
     





    }

    void OnDrawGizmosSelected()
    {
        if (!started)
        {
            startPos = transform.position;
        

        }
            
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 0.3f);
            if (Eixo == eixo.x)
            {

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(new Vector3(startPos.x + Distance / 2, startPos.y, startPos.z), transform.position);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(new Vector3(startPos.x - Distance / 2, startPos.y, startPos.z), transform.position);


            }
            if (Eixo == eixo.y)
            {

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(new Vector3(startPos.x, startPos.y + Distance / 2, startPos.z), transform.position);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(new Vector3(startPos.x, startPos.y - Distance / 2, startPos.z), transform.position);

            }
            if (Eixo == eixo.z)
            {

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(new Vector3(startPos.x, startPos.y, startPos.z + Distance / 2), transform.position);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(new Vector3(startPos.x, startPos.y, startPos.z - Distance / 2), transform.position);



            }
        
       
    }
}
