using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistonBehaviour : MonoBehaviour
{
    public bool stompy;
    public Transform origin;
    public Transform destination;
    public float stationaryTime;
    public bool moving;
    public float speed = 2.5f;

    public bool Moving
    {
        get { return moving; }
        set { moving = value; StopAllCoroutines(); if (moving) { StartCoroutine(MovePiston()); } }
    }
    GameObject deathTrigger;
    bool enRoute;
    bool retracting;
    float currentDist = 0;
    float travelDist;

    public int[] activeStages;
    bool active;



    private void OnEnable()
    {
        GameStateManager.StageStarted += CheckForActivate;
    }

    private void OnDisable()
    {
        GameStateManager.StageStarted -= CheckForActivate;
    }




    void CheckForActivate(int currentStage)
    {
        foreach (var activeStage in activeStages)
        {
            ToggleActive(currentStage == activeStage);
        }
    }


    void ToggleActive(bool checkActiveStage)
    {
        if (checkActiveStage == active) return;
        active = checkActiveStage;

        if (active)
        {
            if (moving) { Moving = true; }
        }
        else
        {
            if (moving) { Moving = false; }
        }
    }



    private void Start()
    {
        transform.position = origin.position;
        deathTrigger = transform.GetChild(1).gameObject;
        deathTrigger.SetActive(false);
        travelDist = Vector3.Distance(destination.position, origin.position);
    }


    IEnumerator MovePiston()
    {
        while (moving)
        {
            Debug.Log("Gimme a sec...");
            yield return new WaitForSeconds(stationaryTime);
            enRoute = true;

            while (enRoute)
            {
                if (!retracting && transform.position != destination.position)
                {
                    currentDist += speed * Time.deltaTime;
                    currentDist = Mathf.Clamp(currentDist, 0, travelDist);
                    transform.position = Vector3.MoveTowards(transform.position, destination.position, currentDist);
                }
                else if (retracting && (transform.position != origin.position))
                {
                    currentDist += speed * Time.deltaTime;
                    currentDist = Mathf.Clamp(currentDist, 0, travelDist);
                    transform.position = Vector3.MoveTowards(transform.position, origin.position, currentDist);
                }

                if (transform.position == destination.position)
                {
                    enRoute = false;
                    retracting = true;
                    Debug.Log("Destination Reached!");
                    currentDist = 0;
                }
                if (transform.position == origin.position)
                {
                    enRoute = false;
                    retracting = false;
                    Debug.Log("Origin Reached!");
                    currentDist = 0;
                }

                yield return null;
            }
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("StompTrigger"))
        {
            if (stompy)
            {
                deathTrigger.SetActive(true);
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("StompTrigger"))
        {
            if (stompy)
            {
                deathTrigger.SetActive(false);
            }
        }
    }
}
