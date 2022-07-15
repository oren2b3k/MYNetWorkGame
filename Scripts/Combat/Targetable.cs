using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Health))]
public class Targetable : NetworkBehaviour
{
    [SerializeField] Transform targetaim;
    public Transform GettargetTransform() => targetaim;
}
