using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action {
    public abstract bool IsNull();

};

public class MoveAction : Action {
    public Vector3 offset;
    public MoveAction(Vector3 offset) {
        this.offset = offset;
    }

    public override bool IsNull() {
        return false;
    }
}

public class StartPoint : Action {
    public Vector3 location;
    public StartPoint(Vector3 startPos) {
        location = startPos;
    }

    public override bool IsNull() {
        return false;
    }
}

public class NullAction : Action {
    public NullAction() { }
    public override bool IsNull() {
        return true;
    }
}

public class ActionSequence {
    private Action[] actions;
    private int step;

    public float startDelay;
    public ActionSequence(List<Action> pastActions, float delay) {
        step = 0;
        actions = pastActions.ToArray();
        startDelay = delay;
    }

    public Action NextStep() {
        if (step >= actions.Length) {
            // Go to the last step and redo it
            return actions[actions.Length - 1];
        } else {
            return actions[step++];
        }
    }

    public Vector3 GoToStart() {
        Vector3 startPos = ((StartPoint)actions[0]).location;
        step = 1;
        return startPos;
    }
}
