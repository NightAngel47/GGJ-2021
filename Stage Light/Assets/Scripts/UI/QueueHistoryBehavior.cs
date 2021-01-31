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
        PuzzleManager.LastLightUnqueued.AddListener(() => RemoveLastElement());
    }

    private void OnDisable()
    {
        PuzzleManager.NewLightQueued.RemoveListener((programmedLight) => AddElement(programmedLight.fixture.transform.GetSiblingIndex(), programmedLight.point.positionIndex, programmedLight.point.color));
        PuzzleManager.LastLightUnqueued.RemoveListener(() => RemoveLastElement());
    }

    private void Start()
    {
        undoButton.interactable = false;
    }

    public void AddElement(int fixtureIndex, int positionIndex, Color color)
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

    public void RemoveLastElement()
    {
        if (contentTransform.childCount > 0)
        {
            Destroy(contentTransform.GetChild(contentTransform.childCount - 1).gameObject);
            contentTransform.sizeDelta = new Vector2(contentTransform.rect.width - ElementWidth, 0f);
        }

        if (contentTransform.childCount == 0)
            undoButton.interactable = false;
    }
}