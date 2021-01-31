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
    public Queue<StagePoint> Requests { get; private set; }
    public Queue<ProgrammedLightFixture> ProgrammedLights { get; private set; }

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

        ProgrammedLights = new Queue<ProgrammedLightFixture>();
    }

    public void NewSelectedFixture(int indexOfDataList)
    {
        SelectedFixture = lightsParent.GetChild(CurrentSceneData.LightIndexes[indexOfDataList]).GetComponent<LightFixtureBehavior>();
        SelectedFixtureChanged?.Invoke(CurrentSceneData.LightIndexes[indexOfDataList]);
    }

    public void QueueSelectedLight()
    {
        ProgrammedLights.Enqueue(new ProgrammedLightFixture(SelectedFixture, SelectedFixture.CurrentPoint));
    }

    [Button("Start Scene")]
    public void StartScene()
    {
        Requests = new Queue<StagePoint>(CurrentSceneData.RequestsForScene);

        StartCoroutine(ContinueSceneSequence());
    }

    private IEnumerator ContinueSceneSequence()
    {
        ActorBehavior actor = GetCurrentActor(Requests.Dequeue());
        actor.MoveOnStage();

        if (ProgrammedLights.Count != 0)
        {
            ProgrammedLightFixture light = ProgrammedLights.Dequeue();

            SelectedFixture = light.fixture;
            SelectedFixtureChanged?.Invoke(light.fixture.transform.GetSiblingIndex());

            light.fixture.UpdatePosition(light.point.positionIndex);
            // Shape
            light.fixture.UpdateColor(light.fixture.PossibleColors.IndexOf(light.point.color));

            GameObject actorObj = light.fixture.CheckForObjectInLight();
            if (actorObj != null && actorObj.GetComponent<ActorBehavior>() == actor && light.point == actor.Request)
            {
                Debug.Log("Success");
                // Success
            }
            else
            {
                Debug.Log("Fail: Doesn't Match");
                // Fail
            }
        }
        else
        {
            Debug.Log("Fail: No More Programmed Lights");
            // Fail
        }

        yield return new WaitWhile(() => actor.IsMoving);

        yield return new WaitForSeconds(1f);

        actor.MoveOffStage();

        yield return new WaitWhile(() => actor.IsMoving);

        if (Requests.Count != 0)
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