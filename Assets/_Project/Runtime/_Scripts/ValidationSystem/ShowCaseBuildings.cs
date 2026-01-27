using UnityEngine;

public class ShowCaseBuildings : MonoBehaviour
{


    private Vector3 _rotationSpeed = new Vector3(0, 100, 0);


    private void Update()
    {
        transform.Rotate(_rotationSpeed*Time.deltaTime);
    }



}
