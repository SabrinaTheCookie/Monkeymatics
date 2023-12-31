using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fabricator : MonoBehaviour
{
    public Transform propsHolder;

    [SerializeField]
    GameObject banana;

    [SerializeField]
    Transform output;

    GameObject reward;

    public static event Action<int> OnFabricated;

    public int fabrications;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Checks if the inputed object is scrap - if so destroys the scrap and attempts to dispense refined metal
        Prop prop = collision.GetComponent<Prop>();
        if (!prop) return;
        if (prop.prop == Prop.Props.Refined)
        {
            Destroy(collision.gameObject);
            fabrications++;
            OnFabricated?.Invoke(fabrications);
            SpawnReward();
        }
    }

    private void SpawnReward()
    {
        AudioManager.Instance.PlaySound("fabricate");
        reward = Instantiate(banana, output.position, output.rotation, propsHolder);
        

    }
}
