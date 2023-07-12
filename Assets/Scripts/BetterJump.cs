using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterJump : MonoBehaviour
{
    private Rigidbody RB;

    //__________________________//

    [SerializeField] private float mFallMultiplier;
    [SerializeField] private float mLowJumpMultiplier;

    //__________________________//

    private void Start()
    {
        RB = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (RB.velocity.y < 0)
        {
            RB.velocity += Vector3.up * Physics.gravity.y * (mFallMultiplier - 1) * Time.deltaTime;
        }
        else if (RB.velocity.y > 0)
        {
            RB.velocity += Vector3.up * Physics.gravity.y * (mLowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
}
