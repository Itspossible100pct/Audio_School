namespace NOT_Lonely
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [ExecuteInEditMode]
    public class ACC_Propagator : MonoBehaviour
    {
        public GameObject[] propPrefabs;    
        public int propCount = 10;
        public bool usePropGrouping = false;
        public int propGroupSize = 0;
        public float propVerticalPosOffset = 0;
        [Range(0, 1)] public float propPositionRandom = 0;
        public Vector3 propRotationMin = Vector3.zero;
        public Vector3 propRotationMax = Vector3.zero;
        public Vector2 propScaleMinMax = Vector2.one;
        [Range(0, 1)] public float propFollowPathRotation = 0;

        public enum PrefabSelectionMode
        {
            Sequental,
            Random
        }

        public PrefabSelectionMode prefabSelectionMode;

        public Vector2 startEndOffsets = new Vector2(0.5f, 0.5f);
        [SerializeField, HideInInspector] private Vector3[] pathPoints;
        [SerializeField, HideInInspector] private List<Transform> spawnedObjects = new List<Transform>();
        [SerializeField, HideInInspector] private ACC_Cable cable;

        private void OnEnable()
        {
            if (propPrefabs == null || propPrefabs.Length == 0)
            {
                Debug.LogWarning($"ACC_Propagator: Add at least one prefab into the Prefabs array.");
                return;
            }

            cable = GetComponent<ACC_Cable>();
            pathPoints = cable.linePointsSequence;

            if (!usePropGrouping) propGroupSize = propCount;

            SpawnObjectsAlongPath();
        }

        private void SpawnObjectsAlongPath()
        {
            if(spawnedObjects.Count > 0)
            {
                for (int i = 0; i < spawnedObjects.Count; i++)
                {
                    if (spawnedObjects[i] != null) DestroyImmediate(spawnedObjects[i].gameObject);
                }
            }
            spawnedObjects = new List<Transform>();

            float totalDistance = 0;
            for (int i = 0; i < pathPoints.Length - 1; i++)
            {
                totalDistance += Vector3.Distance(pathPoints[i], pathPoints[i + 1]);
            }

            //Convert from percentage to units
            float distFactor = 1 / totalDistance;
            Vector2 offsets = startEndOffsets / distFactor;

            totalDistance += (-offsets.y - offsets.x);

            float spawnInterval = totalDistance / (propCount - 1);
            float currentDistance = offsets.x;

            float posRandom = (propPositionRandom / distFactor) / propCount;

            int spawnCounter = propGroupSize;

            int prefabIndex = 0;

            GameObject currentStepObject = null;

            for (int i = 0; i < propCount; i++)
            {
                if (spawnCounter != 0)
                {
                    currentDistance += Random.Range(-posRandom, posRandom);

                    Vector3 spawnPosition = GetPointOnPath(currentDistance);
                    Quaternion spawnRotation = Quaternion.Lerp(Quaternion.identity, Quaternion.LookRotation(GetTangentOnPath(currentDistance)), propFollowPathRotation);
                    spawnRotation *= Quaternion.Euler(new Vector3(
                        Random.Range(propRotationMin.x, propRotationMax.x), 
                        Random.Range(propRotationMin.y, propRotationMax.y), 
                        Random.Range(propRotationMin.z, propRotationMax.z)));

                    Vector3 spawnScale = Vector3.one * Random.Range(propScaleMinMax.x, propScaleMinMax.y);
 
                    if(prefabSelectionMode == PrefabSelectionMode.Random)
                    {
                        currentStepObject = propPrefabs[Random.Range(0, propPrefabs.Length)];
                    }
                    else
                    {
                        //sequental selection 
                        currentStepObject = propPrefabs[prefabIndex];
                        prefabIndex = (prefabIndex < propPrefabs.Length - 1) ? prefabIndex + 1 : 0;
                    }

                    Transform spawnedObject = Instantiate(currentStepObject, spawnPosition, spawnRotation, cable.transform).transform;

                    spawnedObject.localScale = spawnScale;

                    spawnedObjects.Add(spawnedObject);
                    spawnCounter--;
                }
                else
                {
                    spawnCounter = propGroupSize;
                }

                currentDistance += spawnInterval;
            }
        }

        private Vector3 GetPointOnPath(float distance)
        {
            float currentDistance = 0;

            for (int i = 0; i < pathPoints.Length - 1; i++)
            {
                Vector3 startPoint = pathPoints[i];
                Vector3 endPoint = pathPoints[i + 1];
                float segmentDistance = Vector3.Distance(startPoint, endPoint);

                if (currentDistance + segmentDistance >= distance)
                {
                    float t = (distance - currentDistance) / segmentDistance;
                    return Vector3.Lerp(startPoint, endPoint, t) + Vector3.up * propVerticalPosOffset;
                }

                currentDistance += segmentDistance;
            }

            return pathPoints[pathPoints.Length - 1] + Vector3.up * propVerticalPosOffset;
        }

        private Vector3 GetTangentOnPath(float distance)
        {
            float currentDistance = 0f;

            for (int i = 0; i < pathPoints.Length - 1; i++)
            {
                Vector3 startPoint = pathPoints[i];
                Vector3 endPoint = pathPoints[i + 1];
                float segmentDistance = Vector3.Distance(startPoint, endPoint);

                if (currentDistance + segmentDistance >= distance)
                {
                    return (endPoint - startPoint).normalized;
                }

                currentDistance += segmentDistance;
            }

            return (pathPoints[pathPoints.Length - 1] - pathPoints[pathPoints.Length - 2]).normalized;
        }
    }
}
