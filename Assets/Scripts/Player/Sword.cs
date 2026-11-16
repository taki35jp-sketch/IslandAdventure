using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private GameObject swordObject;

    public GameObject GetSwordObject()
    {
        return swordObject;
    }
}