using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragablePoint : MonoBehaviour
{
    private Vector3 offset;
    private float zPos;
    private MeshRenderer mRend;
    private MaterialPropertyBlock pb;
    private bool dragging;
    private Color colorHover = new Color(0.137f, 0.713f, 1, 1);
    private Color colorClick = new Color(0.1f, 0.5f, 0.8f, 1);
    private Color colorNormal = Color.white;

    public delegate void OnPointDestroy(Transform point);
    public OnPointDestroy onPointDestroy;

    public delegate void OnPointRelease();
    public OnPointRelease onPointRelease;

    private void Awake()
    {
        mRend = GetComponent<MeshRenderer>();
        pb = new MaterialPropertyBlock();
    }

    private void OnMouseDown()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            Destroy(gameObject);
            onPointDestroy?.Invoke(transform);
        }

        zPos = Camera.main.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMousePosWS();
        dragging = true;

        pb.SetColor("_Color", colorClick);
        mRend.SetPropertyBlock(pb);
    }

    private void OnMouseUp()
    {
        dragging = false;
        onPointRelease?.Invoke();

        pb.SetColor("_Color", colorNormal);
        mRend.SetPropertyBlock(pb);
    }

    private void OnMouseDrag()
    {
        transform.position = GetMousePosWS() + offset;
    }

    private void OnMouseEnter()
    {
        if (dragging) return;

        pb.SetColor("_Color", colorHover);
        mRend.SetPropertyBlock(pb);
    }

    private void OnMouseExit()
    {
        if (dragging) return;

        pb.SetColor("_Color", colorNormal);
        mRend.SetPropertyBlock(pb);
    }

    private Vector3 GetMousePosWS()
    {
        Vector3 pos = Input.mousePosition;
        pos.z = zPos;

        return Camera.main.ScreenToWorldPoint(pos);
    }
}
