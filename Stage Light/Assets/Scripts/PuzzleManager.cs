using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; } = null;
    public static UnityEventInt SelectedFixtureChanged = new UnityEventInt();
    public static UnityEventProgrammedLightFixture NewLightQueued = new UnityEventProgrammedLightFixture();
    public static UnityEvent FirstLightDequeued = new UnityEvent();
    public static UnityEvent LastLightUnqueued = new UnityEvent();
    public static UnityEvent ClearQueue = new UnityEvent();
    public static LightFixtureBehavior SelectedFixture { get; private set; } = null;

    [SerializeField] private Transform lightsParent = null;
    [SerializeField] private LightFixtureDisplay lightControls = null;
    [SerializeField] private Light2D globalLight;
    [SerializeField, MinMaxSlider(0, 1)] private Vector2 houseLightIntensity;
    [SerializeField] private GameObject uiControls;
    [SerializeField] private GameObject popupBackground;
    private enum PopupPanelType { DryRun, Success, Fail, MainMenu}
    [SerializeField] private List<GameObject> popupPanels = new List<GameObject>();

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
        if (CurrentSceneData != null)
        {
            foreach (Vector3 point in CurrentSceneData.Positions)
            {
                Gizmos.DrawWireSphere(point, 0.5f);
            }
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        globalLight.intensity = houseLightIntensity.y;
        
        // hide popup Ui
        for (int i = 0; i < popupBackground.transform.childCount; ++i)
        {
            popupBackground.transform.GetChild(i).gameObject.SetActive(false);
        }
        popupBackground.transform.gameObject.SetActive(false);

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

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
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

        // show dry run popup
        popupBackground.transform.gameObject.SetActive(true);
        popupPanels[(int)PopupPanelType.DryRun].SetActive(true);
        uiControls.SetActive(false);
    }

    public void SetNextPuzzle()
    {
        if (currentDataIndex < sceneData.Count)
        {
            currentDataIndex = currentDataIndex + 1;
            SetCurrentPuzzle();
        }
        else
        {
            ChangeScene("FinishScreen");
        }
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

        StartCoroutine(PerformanceSequence(true));
    }

    private IEnumerator DryRunSequence()
    {
        uiControls.SetActive(false);
        globalLight.intensity = houseLightIntensity.y;
        ActorBehavior actor = GetCurrentActor(Requests.Dequeue());
        actor.MoveOnStage();

        yield return new WaitWhile(() => actor.IsMoving);
        
        actor.DisplayRequest();

        yield return new WaitForSeconds(1f);

        actor.MoveOffStage();

        yield return new WaitWhile(() => actor.IsMoving);

        if (Requests.Count != 0)
        {
            StartCoroutine(DryRunSequence());
        }
        else
        {
            uiControls.SetActive(true);
            globalLight.intensity = houseLightIntensity.x;
        }
    }

    private IEnumerator PerformanceSequence(bool canSucceed)
    {
        uiControls.SetActive(false);
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

            FirstLightDequeued?.Invoke();
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
                canSucceed = false;
            }
        }
        else
        {
            Debug.Log("Fail: No More Programmed Lights");
            canSucceed = false;
        }

        yield return new WaitForSeconds(1f);

        actor.MoveOffStage();

        yield return new WaitWhile(() => actor.IsMoving);

        if (Requests.Count != 0)
        {
            StartCoroutine(PerformanceSequence(canSucceed));
        }
        else
        {
            ProgrammedLights.Clear();
            ClearQueue?.Invoke();
            if (canSucceed)
            {
                globalLight.intensity = houseLightIntensity.y;
                
                // show success popup
                popupBackground.transform.gameObject.SetActive(true);
                popupPanels[(int)PopupPanelType.Success].SetActive(true);
                
                // Called by popup
                //SetNextPuzzle();
            }
            else
            {
                // show fail popup
                popupBackground.transform.gameObject.SetActive(true);
                popupPanels[(int)PopupPanelType.Fail].SetActive(true);
            }
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