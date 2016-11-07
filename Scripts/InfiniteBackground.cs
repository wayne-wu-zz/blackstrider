using UnityEngine;
using System.Collections;

public class InfiniteBackground : MonoBehaviour {

    public GameObject background;
    public Transform target;
    public float depth = 0;

    private GameObject firstBackground;
    private GameObject lastBackground;

    private float width;
    private Vector3 widthV;

	// Use this for initialization
	void Start () {
        width = background.GetComponent<Renderer>().bounds.size.x;
        widthV = new Vector3(width, 0, 0);

        firstBackground = Instantiate(background);
        firstBackground.transform.parent = transform;
        firstBackground.transform.position = background.transform.position - widthV;
        InvertX(firstBackground.transform);

        lastBackground = Instantiate(background);
        lastBackground.transform.parent = transform;
        lastBackground.transform.position = background.transform.position + widthV;
        InvertX(firstBackground.transform);
    }

    void Update()
    {
    }


    void FixedUpdate () {
        if(target.position.x > lastBackground.transform.position.x)
        {
            GameObject temp = firstBackground;
            firstBackground = background;
            background = lastBackground;
            temp.transform.position = lastBackground.transform.position + widthV;
            InvertX(temp.transform);
            lastBackground = temp;
        }
        else if(target.position.x < background.transform.position.x)
        {
            GameObject temp = lastBackground;
            lastBackground = background;
            background = firstBackground;
            temp.transform.position = firstBackground.transform.position - widthV;
            InvertX(temp.transform);
            firstBackground = temp;
        }

        SetX(firstBackground.transform, background.transform.position.x - width);
        SetX(lastBackground.transform, background.transform.position.x + width);

	}

    //void FollowCamera()
    //{
    //    SetX(firstBackground.transform, Location(firstX), firstX);
    //    SetX(lastBackground.transform, Location(lastX), middleX);
    //    SetX(background.transform, Location(middleX), lastX);
    //}

    void SetX(Transform bg, float x)
    {
        Vector3 pos = bg.position;
        pos.x = x;
        bg.position = pos;
    }

    void InvertX(Transform transform)
    {
        transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    //float Location(float offset)
    //{
    //    float x = Camera.main.transform.position.x - cameraX;
    //    int direction = (x < 0) ? -1 : 1;
    //    return offset + depth * Mathf.Sqrt(Mathf.Abs(x)) * direction;
    //}
}
