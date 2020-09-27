using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    private ActionSequence actionSequence;
    private float waitTime;


    public void SetActionSequence(ActionSequence actions, float startTime) {
        actionSequence = actions;
        waitTime = actionSequence.startDelay-startTime;
        CheckStart();
    }

    private void CheckStart() {
        if (waitTime <= 0f) {
            transform.position = actionSequence.GoToStart();
            while (waitTime + Time.fixedDeltaTime <= 0f) {
                PerformStep();
                waitTime += Time.fixedDeltaTime;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (waitTime > 0f) {
            waitTime -= Time.deltaTime;
            CheckStart();
        }
    }

    private void PerformStep() {
        Action nextAction = actionSequence.NextStep();
        if (!nextAction.IsNull()) {
            transform.position += ((MoveAction)nextAction).offset;
        }
    }

    private void FixedUpdate() {
        PerformStep();
    }


}
