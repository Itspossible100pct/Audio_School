using System.Collections.Generic;
using UnityEngine;
using NOT_Lonely;

public class RuntimePlacementExample : MonoBehaviour
{
    public ACC_Trail cablesTrail;
    public List<Transform> points = new List<Transform>();
    public Material[] newMaterials;
    public DragablePoint pointPrefab;
    private bool isRealtime = false;
    private Plane plane;
    private bool isRandomHeight;
    private Vector2 heightScale = new Vector2(0.2f, 0.2f);

    void Start()
    {
        CreateNewCable();
        plane = new Plane(Vector3.up, Vector3.zero);
    }

    public void CreateNewCable()
    {
        cablesTrail = new GameObject("Cable Trail", typeof(ACC_Trail)).GetComponent<ACC_Trail>();
        cablesTrail.CreateFirstCableSegment(cablesTrail.transform);

        CreateNewPoint(Vector3.left * 10);
        CreateNewPoint(Vector3.right * 10);
    }

    public void CreateNewPoint(Vector3 pos)
    {
        DragablePoint point = Instantiate(pointPrefab);
        point.transform.position = pos;
        point.onPointDestroy += RemovePointFromListAndUpdate;
        point.onPointRelease += UpdateCable;
        points.Add(point.transform);

        if (points.Count > 1)
        {
            if (!cablesTrail.gameObject.activeSelf) cablesTrail.gameObject.SetActive(true);
            UpdateCable();
        }
    }

    private void RemovePointFromListAndUpdate(Transform point)
    {
        points.Remove(point);
        if (points.Count > 1) UpdateCable();
        else cablesTrail.gameObject.SetActive(false);
    }

    private void RandomHeightScaleSwitch()
    {
        isRandomHeight = !isRandomHeight;

        if (isRandomHeight) heightScale = new Vector2(0.2f, 0.5f);
        else heightScale = new Vector2(0.2f, 0.2f);

        UpdateCable();
    }

    public void UpdateCable()
    {
        //change everything you need just before the update...
        cablesTrail.controlPoints = points;
        cablesTrail.heightScale = heightScale;
        cablesTrail.lengthDependentHeight = 0.1f;
        cablesTrail.lengthSegments = 12;
        cablesTrail.thickness = new Vector2(0.4f, 0.4f);
        cablesTrail.vertexAlphaBrightness = isRealtime ? 0 : 1;
        cablesTrail.stepSize = new Vector3(1, 0 , 0);
        cablesTrail.materials = newMaterials;
        //...etc

        //update
        cablesTrail.UpdateCableTrail();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand))
        {
            if (!Input.GetMouseButtonDown(0)) return;

            if (cablesTrail != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                float dist = 0;
                Vector3 hitPoint = Vector3.zero;

                if (plane.Raycast(ray, out dist))
                {
                    hitPoint = ray.GetPoint(dist);
                }

                CreateNewPoint(hitPoint);
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && cablesTrail.amount < 3)
            cablesTrail.ChangeAmount(1);

        if (Input.GetKeyDown(KeyCode.DownArrow) && cablesTrail.amount > 1)
            cablesTrail.ChangeAmount(-1);

        if (Input.GetKeyDown(KeyCode.Space)) RandomHeightScaleSwitch();
    }
}

