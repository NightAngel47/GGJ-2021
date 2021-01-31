using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; } = null;
    public static UnityEventInt SelectedFixtureChanged = new UnityEventInt();
    public static LightFixtureBehavior SelectedFixture { get; private set; } = null;

    [SerializeField] private Transform lightsParent = null;
    [SerializeField] private LightFixtureDisplay lightControls = null;

    [Space]

    [SerializeField] private List<SceneData> sceneData;
    private int currentDataIndex = 0;

    public SceneData CurrentSceneData => sceneData[currentDataIndex];

    private int currentActorIndex = 0;
    public List<ActorBehavior> ActorsInScene { get; private set; }
    public Queue<StagePoint> requests;

    private void OnDrawGizmos()
    {
        foreach (Vector3 point in CurrentSceneData.Positions)
        {
            Gizmos.DrawWireSphere(point, 0.5f);
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        //SelectedFixtureChanged.RemoveAllListeners();
    }

    private void Start()
    {
        ActorsInScene = new List<ActorBehavior>(FindObjectsOfType<ActorBehavior>());

        SetCurrentPuzzle();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void SetCurrentPuzzle()
    {
        for (int index = 0; index < lightsParent.childCount; index++)
        {
            lightsParent.GetChild(index).gameObject.SetActive(CurrentSceneData.LightIndexes.Contains(index));
        }
        lightControls.UpdateSliderMax(CurrentSceneData.LightIndexes.Count - 1);
        SelectedFixtureChanged?.Invoke(0);
    }

    public void NewSelectedFixture(int indexOfDataList)
    {
        SelectedFixture = lightsParent.GetChild(CurrentSceneData.LightIndexes[indexOfDataList]).GetComponent<LightFixtureBehavior>();
        SelectedFixtureChanged?.Invoke(CurrentSceneData.LightIndexes[indexOfDataList]);
    }

    [Button("Start Scene")]
    public void StartScene()
    {
        requests = new Queue<StagePoint>(CurrentSceneData.RequestsForScene);

        StartCoroutine(ContinueSceneSequence());
    }

    private IEnumerator ContinueSceneSequence()
    {
        ActorBehavior actor = GetCurrentActor(requests.Dequeue());
        actor.MoveOnStage();

        yield return new WaitWhile(() => actor.IsMoving);
        yield return new WaitForSeconds(1f);

        actor.MoveOffStage();

        if (requests.Count != 0)
            StartCoroutine(ContinueSceneSequence());
    }

    public ActorBehavior GetCurrentActor(StagePoint request)
    {
        ActorBehavior actor = ActorsInScene[currentActorIndex];
        actor.SetRequest(request);

        currentActorIndex = currentActorIndex + 1 != ActorsInScene.Count ? currentActorIndex + 1 : 0;

        return actor;
    }
}

[Serializable]
public class UnityEventInt : UnityEvent<int> { }