using UnityEngine;
using System.Collections;

public class ParallaxEffect : MonoBehaviour {

    public Transform[] backgrounds;
    public float[] parallaxScales;
    public float smoothing;
    public Transform target;

    private Vector3 previousPosition;

	// Use this for initialization
	void Start () {
        if (!target)
        {
            target = transform;
        }

        previousPosition = target.position;

        if (parallaxScales.Length ==0)
        {
            parallaxScales = new float[backgrounds.Length];
            for (int i = 0; i < parallaxScales.Length; i++)
            {
                parallaxScales[i] = GetParallaxFactor(backgrounds[i].transform.position.z);
            }
        }
	}
	
	// Update is called once per frame
	void LateUpdate () {
	    for(int i = 0; i < backgrounds.Length; i++)
        {
            //Parallax movement = Change in target position * 
            Vector3 parallax = (previousPosition - target.position) * -1* parallaxScales[i];

            Vector3 pos = backgrounds[i].position;
            pos.x += parallax.x;
            backgrounds[i].position = pos;
        }
        previousPosition = target.position;
    }

    float GetParallaxFactor(float scale)
    {
        //Factor should range from 0 to 1
        //0: least amount of movement
        //1: follows target exactly
        float factor = 1;

        factor = (scale / smoothing);

        if (factor < 0)
            factor = 0f;
        if (factor > 1)
            factor = 1f;

        return factor;
    }
}
