using System.Collections;
using System.Linq.Expressions;
using UnityEngine;

public class AnchorClamp : MonoBehaviour
{

    [SerializeField] private GameObject[] _connectors;
    private LineRenderer _line;
    
    // Start is called before the first frame update
    void Start()
    {
        _line = this.gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0; 1< _connectors.Length; i++ )
        {
            _line.SetPosition(i, _connectors[i].transform.position);
        }
    }
}
