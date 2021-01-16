using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Camera mainCamera;
    public Vector3 centreMapPos;

    private GameObject focusObject;

    // Start is called before the first frame update
    void Start()
    {
        centreMapPos = transform.position;
        mainCamera = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(focusObject!= null)
        {
            FocusOnGameobject(focusObject);
        }

    }

    public void SetFocusObject(GameObject _object)
    {
        focusObject = _object;
    }
    public void ClearFocusObject()
    {
        focusObject = null;
    }

    // function to focus on a character or bullet

    public void FocusOnGameobject(GameObject _object)
    {
        mainCamera.orthographicSize = 5f;
        mainCamera.transform.position = new Vector3(_object.transform.position.x, _object.transform.position.y,-20);
    }

    // function to focus on whole map
    public void FocusFull()
    {
        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize,10f,10 * Time.deltaTime);
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, centreMapPos, 10 * Time.deltaTime);
    }

    public IEnumerator ZoomOut(float duration)
    {
        float time = 0;
        Vector3 startPosition = mainCamera.transform.position;
        float startOrthographicSize = mainCamera.orthographicSize;

        while (time < duration)
        {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, 10f, time / duration);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, centreMapPos, time / duration);

            time += Time.deltaTime;
            yield return null;
        }

        mainCamera.orthographicSize = 10f;
        mainCamera.transform.position = centreMapPos;
    }
}
