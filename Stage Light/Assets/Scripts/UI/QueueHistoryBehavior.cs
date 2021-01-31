using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QueueHistoryBehavior : MonoBehaviour
{
    [SerializeField] private Button undoButton = null;

    [SerializeField] private RectTransform contentTransform = null;
    [SerializeField] private Scrollbar scrollBar = null;
    [SerializeField] private GameObject historyElement = null;

    private float ElementWidth => historyElement.GetComponent<RectTransform>().rect.width;

    private void OnEnable()
    {
        PuzzleManager.NewLightQueued.AddListener((programmedLight) => AddElement(programmedLight.fixture.transform.GetSiblingIndex(), programmedLight.point.positionIndex, programmedLight.point.color));
        PuzzleManager.FirstLightDequeued.AddListener(() => RemoveFirstElement());
        PuzzleManager.LastLightUnqueued.AddListener(() => RemoveLastElement());
        PuzzleManager.ClearQueue.AddListener(() => ClearHistory());
    }

    private void OnDisable()
    {
        PuzzleManager.NewLightQueued.RemoveListener((programmedLight) => AddElement(programmedLight.fixture.transform.GetSiblingIndex(), programmedLight.point.positionIndex, programmedLight.point.color));
        PuzzleManager.FirstLightDequeued.RemoveListener(() => RemoveFirstElement());
        PuzzleManager.LastLightUnqueued.RemoveListener(() => RemoveLastElement());
        PuzzleManager.ClearQueue.RemoveListener(() => ClearHistory());
    }

    private void Start()
    {
        undoButton.interactable = false;
    }

    private void AddElement(int fixtureIndex, int positionIndex, Color color)
    {
        contentTransform.sizeDelta = new Vector2(contentTransform.rect.width + ElementWidth, 0f);
        GameObject newElement = Instantiate<GameObject>(historyElement, contentTransform);
        newElement.GetComponent<RectTransform>().localPosition = new Vector3(ElementWidth * (contentTransform.childCount - 1), 0f);
        scrollBar.value = 1;

        newElement.GetComponentInChildren<TMP_Text>().text = $"fix: {PuzzleManager.Instance.CurrentSceneData.LightIndexes.IndexOf(fixtureIndex) + 1}\n" +
                                                             $"pos: {positionIndex + 1}\n" +
                                                             $"col:";
        newElement.GetComponentInChildren<Image>().color = color;
        
        if (undoButton.interactable == false)
            undoButton.interactable = true;
    }

    private void RemoveFirstElement()
    {
        if (contentTransform.childCount > 0)
        {
            Destroy(contentTransform.GetChild(0).gameObject);
            for (int index = 0; index < contentTransform.childCount; index++)
            {
                contentTransform.GetChild(index).GetComponent<RectTransform>().localPosition = new Vector3(ElementWidth * (index - 1), 0f);
            }
            contentTransform.sizeDelta = new Vector2(contentTransform.rect.width - ElementWidth, 0f);
        }

        if (contentTransform.childCount == 0)
            undoButton.interactable = false;
    }

    private void RemoveLastElement()
    {
        if (contentTransform.childCount > 0)
        {
            Destroy(contentTransform.GetChild(contentTransform.childCount - 1).gameObject);
            contentTransform.sizeDelta = new Vector2(contentTransform.rect.width - ElementWidth, 0f);
        }

        if (contentTransform.childCount == 0)
            undoButton.interactable = false;
    }

    private void ClearHistory()
    {
        for (int index = contentTransform.childCount - 1; index >= 0; index--)
        {
            Destroy(contentTransform.GetChild(index).gameObject);
        }
    }
}