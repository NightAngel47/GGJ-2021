using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorBehavior : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private StagePoint request;

    private Vector3 offstagePoint = Vector3.zero;
    private Vector3 startPoint = Vector3.zero;
    private Vector3 targetPoint = Vector3.zero;
    private bool isMoving = false;
    private float lerpStep = 0f;
    private GameObject speechBubble;

    public StagePoint Request => request;
    public bool IsMoving => isMoving;

    private void Awake()
    {
        offstagePoint = transform.position;
        speechBubble = transform.GetChild(1).gameObject;
        speechBubble.SetActive(false);
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.Lerp(startPoint, targetPoint, lerpStep);
            lerpStep += Time.deltaTime * speed;
            if (transform.position == targetPoint)
                isMoving = false;
        }
    }

    [Button("On Stage")]
    public void MoveOnStage()
    {
        lerpStep = 0f;
        startPoint = offstagePoint;
        targetPoint = PuzzleManager.Instance.CurrentSceneData.Positions[request.positionIndex];
        isMoving = true;
    }

    [Button("Off Stage")]
    public void MoveOffStage()
    {
        speechBubble.SetActive(false);
        lerpStep = 0f;
        startPoint = PuzzleManager.Instance.CurrentSceneData.Positions[request.positionIndex];
        targetPoint = offstagePoint;
        isMoving = true;
    }

    public void DisplayRequest()
    {
        speechBubble.SetActive(true);
    }

    public void SetRequest(StagePoint request)
    {
        this.request = request;
        speechBubble.transform.GetComponentInChildren<SpriteRenderer>().color = this.request.color;
    }
}