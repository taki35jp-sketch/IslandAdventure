using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [SerializeField] private BossController _bossCnt; 

    void Attack()
    {
        _bossCnt.Attack();
    }
}
