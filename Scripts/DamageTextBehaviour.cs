using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageTextBehaviour : MonoBehaviour {

    // Use this for initialization
    public float timeUntilGone = 2f;
    public float DistanceAbove = 2f;
    public float Speed = 5f;

    private bool started;
    private RectTransform rectTransform;
    private Transform host;
    private bool inversed = false;

	void Start () {
        rectTransform = GetComponent<RectTransform>();
        host = transform.parent.parent;
        started = false;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!started)
        {
            StartCoroutine(SelfDestroy());
            started = true;
        }
        if ((host.localScale.x < 0 && !inversed)||(host.localScale.x > 0 && inversed))
            Flip();
    }

    void Flip()
    {
        inversed = !inversed;
        Vector3 theScale = rectTransform.localScale;
        theScale.x *= -1;
        rectTransform.localScale = theScale;
    }

    IEnumerator SelfDestroy()
    {
        yield return new WaitForSeconds(timeUntilGone);
        Destroy(gameObject);
    }
}
