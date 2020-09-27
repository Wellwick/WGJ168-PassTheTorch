using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField]
    private GameObject ghostPrefab;

    [SerializeField, Range(0f, 10f)]
    private float maxSpeed = 2f;

    [SerializeField, Min(0f)]
    private float acceleration = 1f;

    private Rigidbody2D rigidbody;

    private bool jumpRequest, onGround, waitToStart;

    private float spawnTime, startTime, totalDelay;

    private float targetX;

    private Vector3 lastPos;

    private List<Action> actions;

    private List<Ghost> ghosts;

    private LinkedList<ActionSequence> pastActions;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        jumpRequest = false;
        onGround = false;
        waitToStart = true;
        pastActions = new LinkedList<ActionSequence>();
        totalDelay = 0f;
    }

    private void StartTrackingActions() {
        actions = new List<Action>();
        startTime = Time.time - spawnTime;
        actions.Add(new StartPoint(transform.position));
        lastPos = transform.position;
    }

    private void StoreActionSequence() {
        totalDelay += startTime;
        pastActions.AddLast(new ActionSequence(actions, totalDelay));
        SetoffGhosts();
        spawnTime = Time.time;
        waitToStart = true;
    }

    // Update is called once per frame
    void Update() {
    }

    private void FixedUpdate() {
        if (Input.GetAxis("Horizontal") != 0f) {
            Vector2 velocity = rigidbody.velocity;
            velocity.x = Mathf.Clamp(velocity.x + (acceleration * Input.GetAxis("Horizontal")), -maxSpeed, maxSpeed);
            rigidbody.velocity = velocity;
        }
        if (Input.GetAxis("Jump") != 0f) {
            jumpRequest = true;
        } else {
            jumpRequest = false;
        }
        Vector3 offset = transform.position - lastPos;
        lastPos = transform.position;
        if (actions != null) {
            actions.Add(new MoveAction(offset));
        }
        if (jumpRequest && onGround) {
            onGround = false;
            jumpRequest = false;
            rigidbody.velocity += new Vector2(0f, 2.5f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (waitToStart) {
            StartTrackingActions();
            waitToStart = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        onGround = true;
    }

    private void OnCollisionExit2D(Collision2D collision) {
        onGround = false;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Teleporter t = collision.GetComponent<Teleporter>();
        if (t) {
            transform.position += t.Offset();
            StoreActionSequence();
        }
    }

    private void SetoffGhosts() {
        if (ghosts != null) {
            // Delete the old ghosts
            foreach (Ghost g in ghosts) {
                Destroy(g.gameObject);
            }
        }
        ghosts = new List<Ghost>();
        int index = 1;
        foreach (ActionSequence actionSequence in pastActions) {
            GameObject go = Instantiate(ghostPrefab);
            go.name = "Ghost " + index;
            Ghost ghost = go.AddComponent<Ghost>();
            ghost.SetActionSequence(actionSequence, totalDelay);
            ghosts.Add(ghost);
            index += 1;
        }
    }
}
