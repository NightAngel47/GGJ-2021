using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFixtureBehavior : MonoBehaviour
{
    [SerializeField] private GameObject IsSelectedObject;

    [SerializeField] private StagePoint currentPoint;

    [Space]

    [SerializeField] private List<int> possiblePositions;
    [SerializeField] private List<StagePoint.Shapes> possibleShapes;
    [SerializeField] private List<Color> possibleColor;

    public StagePoint CurrentPoint => currentPoint;

    private void OnEnable()
    {
        PuzzleManager.SelectedFixtureChanged.AddListener((newIndex) => NewSelectedLight(newIndex));
    }

    private void Start()
    {
        UpdateCurrentPoint(0, 0, 0);
    }

    private void OnDisable()
    {
        PuzzleManager.SelectedFixtureChanged.RemoveListener((newIndex) => NewSelectedLight(newIndex));
    }

    private void NewSelectedLight(int newIndex)
    {
        IsSelectedObject.SetActive(transform.GetSiblingIndex() == newIndex);
    }

    public void UpdateCurrentPoint(int positionIndex, int shapeIndex, int colorIndex)
    {
        currentPoint.positionIndex = possiblePositions[positionIndex];
        currentPoint.shape = possibleShapes[shapeIndex];
        currentPoint.color = possibleColor[colorIndex];

        Vector3 angleVector = (PuzzleManager.Instance.CurrentSceneData.Positions[currentPoint.positionIndex] - transform.position).normalized;
        float angle = Vector3.Angle(transform.up, angleVector) * Mathf.Sign(Vector2.Dot(Vector2.left, angleVector));
        Debug.DrawRay(transform.position, angleVector, Color.red, 5f, false);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void ChangeSceneData()
    {
        UpdateCurrentPoint(0, 0, 0);
    }

    public GameObject CheckForObjectInLight()
    {
        RaycastHit2D hitObj = Physics2D.Raycast(transform.position, transform.up);

        if (hitObj.collider == null)
            return null;

        return hitObj.collider.gameObject;
    }
}