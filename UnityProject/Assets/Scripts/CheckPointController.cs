using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointController : MonoBehaviour
{

    public Vector3 LastCheckPointKim;
    public Vector3 LastCheckPointBarto;
    // Start is called before the first frame update
    static public CheckPointController Instance;
    public Animator fadeScreen;
    UnityEngine.UI.Image fadeImage;

    public float timeToCancelFadeScreen;

    public void Start()
    {
        Instance = this;
        fadeImage = fadeScreen.gameObject.GetComponent<UnityEngine.UI.Image>();
        timeToCancelFadeScreen = 1f;
    }

    public void Respawn(bool forceBothRespawn)
    {
        ShowFadeScreen(true);
        StartCoroutine(CancelFadeScreen(forceBothRespawn));
    }

    public void ShowFadeScreen(bool show)
    {
        fadeScreen.SetBool("fade", show);
    }

    IEnumerator CancelFadeScreen(bool forceBothRespawn)
    {
        yield return new WaitForSeconds(timeToCancelFadeScreen);
        if (forceBothRespawn)
        {
            BagController.Instance.transform.position = LastCheckPointBarto;
            Player.Instance.transform.position = LastCheckPointKim;
        }
        else
        {
            if (BagController.Instance.canControll)
            {
                BagController.Instance.transform.position = LastCheckPointBarto;
            }

            if (Player.Instance.canControll)
            {
                Player.Instance.transform.position = LastCheckPointKim;
            }
        }
        ShowFadeScreen(false);
        ChatSystem.Instance.ClearChat();
    }
}
