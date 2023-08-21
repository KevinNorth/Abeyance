using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MoreMountains.TopDownEngine;

///<summary>
///Automatically adds the player instance object to the specified
///Cinemachine Target Group when the level starts
///</summary>
public class GrabPlayerForTargetGroup : MonoBehaviour
{
    [SerializeField] CinemachineTargetGroup targetGroup;
    [SerializeField] LevelManager levelManager;
    [SerializeField] float playerWeight = 1f;
    [SerializeField] float playerRadius = 0f;

    private bool hasAddedPlayer = false;

    void Update()
    {
        // Can't add the player during Start() because the LevelManager needs to
        // be guaraneed to finish intializing itself first.
        if (!hasAddedPlayer)
        {
            var player = levelManager.Players[0];
            targetGroup.AddMember(player.GetComponent<Transform>(), playerWeight, playerRadius);
            hasAddedPlayer = true;
        }
    }
}
