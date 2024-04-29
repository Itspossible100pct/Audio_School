using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpatialAnchorManager : MonoBehaviour
{
    public OVRSpatialAnchor anchorPrefab;

    private Canvas _canvas;
    private TextMeshProUGUI _uuidText;
    private TextMeshProUGUI _savedStatusText;
    private List<OVRSpatialAnchor> anchors = new List<OVRSpatialAnchor>();
    private OVRSpatialAnchor _lastCreatedAnchor;
    private AnchorLoader _anchorLoader;
    
    public static string NumUuidsPlayerPref = "NumUuids";

    void Awake() => _anchorLoader = GetComponent<AnchorLoader>();
    

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            CreateSpatialAnchor();
        }

        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            SaveLastCreatedAnchor();
        }
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            UnsaveLastCreatedAnchor();
        }
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
        {
            UnsaveAllAnchors();
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch))
        {
            LoadSavedAnchors();
        }
    }

    private void LoadSavedAnchors()
    {
        _anchorLoader.LoadAnchorByUuid();
    }

    private void SaveLastCreatedAnchor()
    {
        _lastCreatedAnchor.Save((_lastCreatedAnchor, success) =>
        {
            if (success)
            {
                _savedStatusText.text = "Saved";
            }
        });
        SaveUuidToPlayerPrefs(_lastCreatedAnchor.Uuid);
    }

    private void SaveUuidToPlayerPrefs(Guid uuid)
    {
        if (!PlayerPrefs.HasKey(NumUuidsPlayerPref))
        {
            PlayerPrefs.SetInt(NumUuidsPlayerPref, 0);
        }
       int playerNumUuids = PlayerPrefs.GetInt(NumUuidsPlayerPref);
       PlayerPrefs.SetString("uuid" + playerNumUuids, uuid.ToString());
       PlayerPrefs.SetInt(NumUuidsPlayerPref, ++playerNumUuids);
    }
    
    private void UnsaveLastCreatedAnchor()
    {
        _lastCreatedAnchor.Erase((_lastCreatedAnchor, success) =>
        {
            if (success)
            {
                _savedStatusText.text = "Not Saved";
            }
        });
    }

    private void UnsaveAllAnchors()
    {
        foreach (var anchor in anchors)
        {
            UnsaveAnchor(anchor);
        }
        anchors.Clear();
        ClearAllUuidsFromPlayerPrefs();
    }

    private void UnsaveAnchor(OVRSpatialAnchor anchor)
    {
        anchor.Erase((erasedAnchor, success) =>
        {
            if (success)
            {
                var textComponents = erasedAnchor.GetComponentsInChildren<TextMeshProUGUI>();
                if (textComponents.Length > 1)
                {
                    var savedStatusText = textComponents[1];
                    savedStatusText.text = "Not Saved";
                }
            }
        });
    }

    private void CreateSpatialAnchor()
    {
        OVRSpatialAnchor workingAnchor = Instantiate(anchorPrefab,
            OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch),
            OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch));

        _canvas = workingAnchor.gameObject.GetComponentInChildren<Canvas>();
        _uuidText = _canvas.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _savedStatusText = _canvas.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        StartCoroutine(AnchorCreated(workingAnchor));

    }

    private IEnumerator AnchorCreated(OVRSpatialAnchor workingAnchor)
    {
        while (!workingAnchor.Created && !workingAnchor.Localized)
        {
            yield return new WaitForEndOfFrame();
        }

        Guid anchorGuid = workingAnchor.Uuid;
        anchors.Add(workingAnchor);
        _lastCreatedAnchor = workingAnchor;

        _uuidText.text = "UUID: " + anchorGuid.ToString();
        _savedStatusText.text = "Not Saved";

    }

    private void ClearAllUuidsFromPlayerPrefs()
    {
        if (PlayerPrefs.HasKey(NumUuidsPlayerPref))
        {
            int playerNumUuids = PlayerPrefs.GetInt(NumUuidsPlayerPref);
            for (int i = 0; i < playerNumUuids; i++)
            {
                PlayerPrefs.DeleteKey("uuid" + i);
            }
            PlayerPrefs.DeleteKey(NumUuidsPlayerPref);
            PlayerPrefs.Save();
        }
    }
    
}
