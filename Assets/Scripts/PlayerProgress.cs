using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProgress : MonoBehaviour
{
    [HideInInspector]
    public Transform Player;
    [HideInInspector]
    public Transform End;
    [HideInInspector]
    public Slider slider;

    private float maxDistance;


    #region DEFAULT FUNCTIONS
    void Start()
    {
        maxDistance = getDistance();

    }
    void Update()
    {
        if (Player.position.z <= maxDistance && Player.position.z >= End.position.z)
        {
            float distance = 1 - (getDistance() / maxDistance);
            setProgress(distance);
        }
    }
    #endregion

    #region CUSTOM FUNCTIONS
    float getDistance()
    {
        return Vector3.Distance(Player.position, End.position);
    }
    void setProgress(float p)
    {
        slider.value = p;
    }
    #endregion
}
