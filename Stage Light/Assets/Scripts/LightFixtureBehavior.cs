using System;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightFixtureBehavior : MonoBehaviour
{
    [SerializeField] private GameObject IsSelectedObject;
    [SerializeField] private SpriteRenderer lensSprite;

    [SerializeField] private StagePoint currentPoint;

    [Space]

    [SerializeField] private List<int> possiblePositions;
    [SerializeField] private List<StagePoint.Shapes> possibleShapes;
    [SerializeField] private List<Color> possibleColor;

    public StagePoint CurrentPoint => currentPoint;
    public List<int> PossiblePositions => possiblePositions;
    public List<StagePoint.Shapes> PossibleShapes => possibleShapes;
    public List<Color> PossibleColors => possibleColor;
    
    private Light2D pointLight;

    private void Awake()
    {
        pointLight = GetComponentInChildren<Light2D>();
    }

    private void OnEnable()
    {
        PuzzleManager.SelectedFixtureChanged.AddListener((newIndex) => NewSelectedLight(newIndex));
    }

    private void OnDisable()
    {
        PuzzleManager.SelectedFixtureChanged.RemoveListener((newIndex) => NewSelectedLight(newIndex));
    }

    private void Start()
    {
        UpdateCurrentPoint(0, 0, 0);
    }

    private void NewSelectedLight(int newIndex)
    {
        IsSelectedObject.SetActive(transform.GetSiblingIndex() == newIndex);
    }

    private void UpdateCurrentPoint(int positionIndex, int shapeIndex, int colorIndex)
    {
        currentPoint.positionIndex = possiblePositions[positionIndex];
        currentPoint.shape = possibleShapes[shapeIndex];
        //currentPoint.color = possibleColor[colorIndex];
        
        UpdateColor(colorIndex);
        MoveLight();
    }

    public void UpdatePosition(int gridPositionIndex)
    {
        foreach (var position in possiblePositions.Where(position => position == gridPositionIndex))
        {
            currentPoint.positionIndex = position;
            MoveLight();
            return;
        }
    }

    public void UpdateColor(int colorIndex)
    {
        currentPoint.color = possibleColor[colorIndex];
        pointLight.color = currentPoint.color;
        lensSprite.color = currentPoint.color;
    }

    private void MoveLight()
    {
        Vector3 angleVector = (PuzzleManager.Instance.CurrentSceneData.Positions[currentPoint.positionIndex] - transform.position).normalized;
        float angle = Vector3.Angle(Vector3.up, angleVector) * Mathf.Sign(Vector2.Dot(Vector2.left, angleVector));
        Debug.DrawRay(transform.position, angleVector * 10, Color.red, 5f, false);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void ChangeSceneData()
    {
        UpdateCurrentPoint(0, 0, 0);
    }

    public GameObject CheckForObjectInLight()
    {
        Debug.DrawRay(transform.position, transform.up * 10, Color.blue, 5f, false);
        RaycastHit2D hitObj = Physics2D.Raycast(transform.position, transform.up);

        if (hitObj.collider == null)
            return null;

        return hitObj.collider.gameObject;
    }
}