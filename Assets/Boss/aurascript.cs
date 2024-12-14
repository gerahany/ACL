using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aurascript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void TakeAuraDamage(int damage)
    {
        BossBehavior boss = GetComponentInParent<BossBehavior>();
        boss.TakeAuraDamage(damage);
    }
}
