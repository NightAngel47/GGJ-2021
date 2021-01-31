using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering.Universal;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; } = null;
    public static UnityEventInt SelectedFixtureChanged = new UnityEventInt();
    public static UnityEventProgrammedLightFixture NewLightQueued = new UnityEventProgrammedLightFixture();
    public static UnityEvent LastLightUnqueued = new UnityEvent();
    public static LightFixtureBehavior SelectedFixture { get; private set; } = null;

    [SerializeField] private Transform lightsParent = null;
    [SerializeField] private LightFixtureDisplay lightControls = null;
    [SerializeField] private Light2D globalLight;
    [SerializeField, MinMaxSlider(0, 1)] private Vector2 houseLightIntensity;

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
        ProgrammedLightFixture programmedLight = new ProgrammedLightFixture(SelectedFixture, SelectedFixture.CurrentPoint);
        ProgrammedLights.Enqueue(new ProgrammedLightFixture(SelectedFixture, SelectedFixture.CurrentPoint));
        NewLightQueued?.Invoke(programmedLight);

    }

    public void UndoLastQueued()
    {
        ProgrammedLightFixture[] array = ProgrammedLights.ToArray();
        ProgrammedLights.Clear();
        for (int index = 0; index < array.Length - 1; index++)
        {
            ProgrammedLights.Enqueue(array[index]);
        }
        LastLightUnqueued?.Invoke();
    }

    public void StartDryRun()
    {
        Requests = new Queue<StagePoint>(CurrentSceneData.RequestsForScene);

        StartCoroutine(DryRunSequence());
    }

    public void StartPerformance()
    {
        Requests = new Queue<StagePoint>(CurrentSceneData.RequestsForScene);

        StartCoroutine(PerformanceSequence());
    }

    private IEnumerator DryRunSequence()
    {
        globalLight.intensity = houseLightIntensity.y;
        ActorBehavior actor = GetCurrentActor(Requests.Dequeue());
        actor.MoveOnStage();

        yield return new WaitWhile(() => actor.IsMoving);

        yield return new WaitForSeconds(1f);

        actor.MoveOffStage();

        yield return new WaitWhile(() => actor.IsMoving);

        if (Requests.Count != 0)
        {
            StartCoroutine(DryRunSequence());
        }
        else
        {
            globalLight.intensity = houseLightIntensity.x;
        }
    }

    private IEnumerator PerformanceSequence()
    {
        globalLight.intensity = houseLightIntensity.x;
        ActorBehavior actor = GetCurrentActor(Requests.Dequeue());
        actor.MoveOnStage();

        ProgrammedLightFixture light = null;
        if (ProgrammedLights.Count != 0)
        {
            light = ProgrammedLights.Dequeue();

            SelectedFixture = light.fixture;
            SelectedFixtureChanged?.Invoke(light.fixture.transform.GetSiblingIndex());

            light.fixture.UpdatePosition(light.point.positionIndex);
            // Shape
            light.fixture.UpdateColor(light.fixture.PossibleColors.IndexOf(light.point.color));
        }

        yield return new WaitWhile(() => actor.IsMoving);

        if (light != null)
        {
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

        yield return new WaitForSeconds(1f);

        actor.MoveOffStage();

        yield return new WaitWhile(() => actor.IsMoving);

        if (Requests.Count != 0)
        {
            StartCoroutine(PerformanceSequence());
        }
        else
        {
            globalLight.intensity = houseLightIntensity.y;
        }
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

[Serializable]
public class UnityEventProgrammedLightFixture : UnityEvent<ProgrammedLightFixture> { }