// Runtime Curve Editor
// Copyright © 2013 Rus Artur PFA
//----------------------------------------------
using UnityEngine;


/// <summary>
/// Mouse input.
/// Forward mouse inputs to the CurveWindow component( as the collider observing the mouse inputs is on a different game object).
/// </summary>
public class MouseInput : MonoBehaviour {
	CurveWindow crvWindow;
	
	void Start()
	{
		Transform crvEditor = transform.parent.Find("CurveEditor");
		crvWindow = crvEditor.GetComponent<CurveWindow>();
	}
	
	void OnMouseDown()
	{
		crvWindow.MouseDown();
	}
	
	void OnMouseUp()
	{
		crvWindow.MouseUp();
	}	
	
	void OnMouseDrag()
	{
		crvWindow.MouseDrag();
	}
}
