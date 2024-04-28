namespace NOT_Lonely
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class ACC_Propagation : MonoBehaviour
    {
        public Vector3[] pathPoints;
        [SerializeField] private float propagationInterval;
        [SerializeField] private float currentDistance;
        [SerializeField] private float posRandom;
        [SerializeField] private Vector3 propPos;
        [SerializeField] private Quaternion propRot;
        [SerializeField] private Vector3 propScale;
        [SerializeField] private int count;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cable">A cable component which will be used to propagate objects.</param>
        /// <param name="propPrefabs">An array of prefabs for propagation.</param>
        /// <param name="propCount">Total amount of objects.</param>
        /// <param name="startEndOffsets">An offset from the start and from the end of the cable, where the objects must not appear.</param>
        /// <param name="positionRandom">A randomize value of the objects position along the cable.</param>
        /// <param name="rotationMin">Minimum rotation of the objects.</param>
        /// <param name="rotationMax">Maximum rotation of the objects.</param>
        /// <param name="scaleMinMax">Minimum and maximum uniform scale of the objects.</param>
        /// <param name="followPathRotation">How much the object direction must follow the cable curve.</param>
        /// <param name="verticalPosOffset">An offset of the object position along the Y axis.</param>
        /// <param name="prefabSelectionMode">How the prefabs must be picked from the array. 0 - sequential, 1 = random.</param>
        /// <returns></returns>
        public List<ACC_PropObject> SpawnObjectsAlongPath(ACC_Cable cable,
            GameObject[] propPrefabs, int propCount, Vector2 startEndOffsets, 
            float positionRandom, Vector3 rotationMin, Vector3 rotationMax, Vector2 scaleMinMax, float followPathRotation, 
            float verticalPosOffset, int prefabSelectionMode)
        {
            pathPoints = cable.linePointsSequence;
            count = propCount;

            List<ACC_PropObject> spawnedObjects = new List<ACC_PropObject>();

            CalculateValues(startEndOffsets, positionRandom);

            int prefabIndex = 0;

            for (int i = 0; i < count; i++)
            {
                GetTransformValues(rotationMin, rotationMax, scaleMinMax, followPathRotation, verticalPosOffset);
                SpawnObject(cable, propPrefabs, prefabSelectionMode, spawnedObjects, ref prefabIndex);
            }

            return spawnedObjects;
        }

        private void SpawnObject(ACC_Cable cable, GameObject[] propPrefabs, int prefabSelectionMode, List<ACC_PropObject> spawnedObjects, ref int prefabIndex)
        {
            GameObject currentStepObject;
            GameObject spawnedObject;

            if (prefabSelectionMode == 1) currentStepObject = propPrefabs[Random.Range(0, propPrefabs.Length)];
            else
            {
                currentStepObject = propPrefabs[prefabIndex];
                prefabIndex = (prefabIndex < propPrefabs.Length - 1) ? prefabIndex + 1 : 0;
            }

            if(currentStepObject != null)
            {
#if UNITY_EDITOR
                spawnedObject = PrefabUtility.InstantiatePrefab(currentStepObject) as GameObject;

#else
                spawnedObject = Instantiate(currentStepObject);
#endif
            }
            else
            {
                spawnedObject = new GameObject("Dummy Prop Object");
            }

            spawnedObject.transform.SetPositionAndRotation(propPos, propRot);
            spawnedObject.transform.parent = cable.transform;
            spawnedObject.transform.localScale = propScale;
            spawnedObjects.Add(spawnedObject.AddComponent<ACC_PropObject>());
        }

        private void GetTransformValues(Vector3 rotationMin, Vector3 rotationMax, Vector2 scaleMinMax, float followPathRotation, float verticalPosOffset)
        {
            currentDistance += Random.Range(-posRandom, posRandom);
            propPos = GetPointOnPath(pathPoints, currentDistance) + Vector3.up * verticalPosOffset;
            propRot = Quaternion.Lerp(Quaternion.identity, Quaternion.LookRotation(GetTangentOnPath(pathPoints, currentDistance)), followPathRotation);
            propRot *= Quaternion.Euler(new Vector3(
                Random.Range(rotationMin.x, rotationMax.x),
                Random.Range(rotationMin.y, rotationMax.y),
                Random.Range(rotationMin.z, rotationMax.z)));

            propScale = Vector3.one * Random.Range(scaleMinMax.x, scaleMinMax.y);
            currentDistance += propagationInterval;
        }

        private void CalculateValues(Vector2 startEndOffsets, float positionRandom)
        {
            float totalDistance = 0;
            for (int i = 0; i < pathPoints.Length - 1; i++)
            {
                totalDistance += Vector3.Distance(pathPoints[i], pathPoints[i + 1]);
            }

            //Convert from percentage to units
            float distFactor = 1 / totalDistance;
            Vector2 offsets = startEndOffsets / distFactor;

            totalDistance += (-offsets.y - offsets.x);

            propagationInterval = totalDistance / (count - 1);
            currentDistance = offsets.x;
            posRandom = (positionRandom / distFactor) / count;
        }

        public void UpdateSpawnedObjects(ACC_Cable cable, List<ACC_PropObject> spawnedObjects, Vector2 startEndOffsets, float positionRandom, float verticalPosOffset, Vector3 rotationMin, Vector3 rotationMax, Vector2 scaleMinMax, float followPathRotation)
        {
            if (spawnedObjects == null)
            {
                Debug.LogWarning("ACC_Propagation: No objects provided.");
                return;
            }

            pathPoints = cable.linePointsSequence;

            CalculateValues(startEndOffsets, positionRandom);
 
            for (int i = 0; i < spawnedObjects.Count; i++)
            {
                GetTransformValues(rotationMin, rotationMax, scaleMinMax, followPathRotation, verticalPosOffset);

                spawnedObjects[i].transform.SetPositionAndRotation(propPos, propRot);
                spawnedObjects[i].transform.localScale = propScale;
            }
        }

        private Vector3 GetPointOnPath(Vector3[] pathPoints, float distance)
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
                    return Vector3.Lerp(startPoint, endPoint, t);
                }

                currentDistance += segmentDistance;
            }

            return pathPoints[pathPoints.Length - 1];
        }

        private Vector3 GetTangentOnPath(Vector3[] pathPoints, float distance)
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
