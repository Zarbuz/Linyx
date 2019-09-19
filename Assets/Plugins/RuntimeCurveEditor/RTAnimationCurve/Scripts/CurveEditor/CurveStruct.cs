using UnityEngine;


/// <summary>
/// Keeps information for a curve(or two if the path between two curves is desired).
/// If curve2 is not null than a path between the two curves is drawn(that's the case when a random between curves could be achieved).
/// </summary>
public struct CurveStruct
{
	//default values for the gradations rectangle
	public const decimal xMaxim = 5.0m;
	public const decimal xMin = 0.0m;
	public const decimal yMaxim = 1.7m;
	public const decimal yMin = -1.7m;
	
	public AnimationCurve curve1;
	public AnimationCurve curve2;	
	public int curve1KeysCount;//backup keycount,so that if new keys are added outside of this curve editor, the context menu struct can be correctly updated
	public int curve2KeysCount;			
	public Color color;
	public bool firstCurveSelected;
	public Rect gradRect;
	public CurveStruct(AnimationCurve curve1,AnimationCurve curve2,Color color)
	{
		this.curve1 = curve1;this.curve2 = curve2;this.color = color;
		curve1KeysCount = 0;
		curve2KeysCount = 0;
		if(curve1 != null) curve1KeysCount = curve1.length;
		if(curve2 != null) curve2KeysCount = curve2.length;
		firstCurveSelected = true;
		gradRect = Rect.MinMaxRect((float)xMin,(float)yMin,(float)xMaxim,(float)yMaxim);
	}
};

