using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFixtureBehavior : MonoBehaviour
{
    [SerializeField] private SceneData data;

    [SerializeField] private StagePoint currentPoint;

    [Space]

    [SerializeField] private List<int> possiblePositions;
    [SerializeField] private List<StagePoint.Shapes> possibleShapes;
    [SerializeField] private List<Color> possibleColor;

    public StagePoint CurrentPoint => currentPoint;

    private void Awake()
    {
        UpdateCurrentPoint(0, 0, 0);
    }

    public void UpdateCurrentPoint(int positionIndex, int shapeIndex, int colorIndex)
    {
        currentPoint.position = possiblePositions[positionIndex];
        currentPoint.shape = possibleShapes[shapeIndex];
        currentPoint.color = possibleColor[colorIndex];

        Vector3 angleVector = (data.Positions[currentPoint.position] - transform.position).normalized;
        transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z - Vector3.Angle(transform.up, angleVector));
    }

    public GameObject CheckForObjectInLight()
    {
        RaycastHit2D hitObj = Physics2D.Raycast(transform.position, transform.up);

        if (hitObj.collider == null)
            return null;

        return hitObj.collider.gameObject;
    }
}