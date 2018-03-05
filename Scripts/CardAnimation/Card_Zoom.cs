using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card_Zoom : MonoBehaviour {

    //Put zoomed and moving in the main presenter so that each card instance will not make their own new variable
    public static Vector3 startLocation = new Vector3();
    public static Vector3 startSize;
    public static GameObject zoomedCard;
    private float timeDelta = 0.1f;

    public void OnPointerClick()
    {
        float width = Convert.ToSingle(Camera.main.orthographicSize * 2.0 * Screen.width / 10);
        float height = Convert.ToSingle(Camera.main.orthographicSize * 2.0 * Screen.height / 10);
        if (MainPresenter.moving) {
            return;
        }
        else if (MainPresenter.Zoomed)
        {
            //Check for centerness or size
            if (transform.position == new Vector3(width / 2, height / 2, 0) || transform.localScale.x > 2.4f)
            {
                MainPresenter.unitDetails.transform.SetAsLastSibling();
                MainPresenter.cardChoice.transform.SetAsLastSibling();

                this.StartCoroutine(SmoothMove(startLocation, startSize, timeDelta));

                MainPresenter.Zoomed = false;
                zoomedCard = null;
            }
        }
        else
        {
            zoomedCard = gameObject;

            // Get the target position
            Vector3 targetLocation = new Vector3(width/2, height/2, 0);

            Vector3 targetSize = new Vector3();
            if (transform.GetComponent<RectTransform>().rect.width > 130)
            {
                targetSize = new Vector3(2.5f, 2.5f, 1);
                GameObject leader = GameObject.Find("Canvas/PlayerDetails/Leader");
                if (leader != null)
                    leader.transform.SetAsLastSibling();
            }
            else if (transform.GetComponent<RectTransform>().rect.width > 100)
            {
                targetSize = new Vector3(3.5f, 3.5f, 1);
            }
            else {
                targetSize = new Vector3(5f, 5f, 1);
            }

            //Set starting position and size
            startLocation = transform.position;
            startSize = transform.localScale;

            //Set details last so that zoomed player details work properly
            if(transform.parent != null && transform.parent.transform.parent != null && transform.parent.transform.parent.name == "PlayerDetails")
                MainPresenter.playerDetails.transform.SetAsLastSibling();
            else if(this.transform.parent == MainPresenter.unitDetails)
                MainPresenter.unitDetails.transform.SetAsLastSibling();
            else if(this.transform.parent == MainPresenter.cardChoice)
                MainPresenter.cardChoice.transform.SetAsLastSibling();
            transform.SetAsLastSibling();

            // Start your coroutine
            this.StartCoroutine(SmoothMove(targetLocation, targetSize, timeDelta));

            MainPresenter.Zoomed = true;
        }
    }

    IEnumerator SmoothMove(Vector3 target, Vector3 Size, float delta)
    {
        MainPresenter.moving = true;
        // Will need to perform some of this process and yield until next frames
        float closeEnough = 0.05f;
        float distance = (this.transform.position - target).magnitude;

        // GC will trigger unless we define this ahead of time
        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        // Continue until we're there
        while (distance >= closeEnough)
        {
            // Move a bit then  wait until next  frame
            transform.position = Vector3.Slerp(this.transform.position, target, delta);
            transform.localScale = Vector3.Slerp(this.transform.localScale, Size, delta);
            yield return wait;

            // Check if we should repeat
            distance = (this.transform.position - target).magnitude;
        }

        // Complete the motion to prevent negligible sliding
        this.transform.position = target;

        if(! MainPresenter.Zoomed)
        {
            GameObject leader = GameObject.Find("Canvas/PlayerDetails/Leader");
            if (leader != null)
                leader.transform.SetAsFirstSibling();
        }

        //Allow zooming out
        MainPresenter.moving = false;
    }
}
