using UnityEngine;

public class RootCurveEditor : MonoBehaviour {	
	public int initialHeight = 600;
	private Transform cachedTransf;
	void Start()
	{
		cachedTransf = transform;
	}
	
	void Update()
	{
		//keep the size in pixels constant when the screenheight's varying
		float size = (float)initialHeight/Screen.height;
		Vector3 ls = cachedTransf.localScale;
			
		if (!(Mathf.Abs(ls.x - size) <= float.Epsilon) || !(Mathf.Abs(ls.y - size) <= float.Epsilon) || !(Mathf.Abs(ls.z - size) <= float.Epsilon))
		{
			cachedTransf.localScale = new Vector3(size, size, 1);
		}
	}
}

