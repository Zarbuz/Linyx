using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the context menus for all the curves' lists of key.
/// </summary>
class ContextMenuManager{
	/// <summary>
	/// Keeps the context menu struct for all curves' keys 
	/// </summary>
	const float errorMarg = 1E-5f;
	
	internal Dictionary<AnimationCurve,List<ContextMenuStruct>> dictCurvesContextMenus = new Dictionary<AnimationCurve,List<ContextMenuStruct>>();
	
	internal static bool IsKeyframeFlat(Keyframe keyframe)
	{
		return (Mathf.Abs(keyframe.inTangent) < errorMarg && Mathf.Abs(keyframe.outTangent) < errorMarg);
	}
	
	internal void AddContextMenuStructs(AnimationCurve curve)
	{
		if(curve != null)
		{
			List<ContextMenuStruct> listContextMenus = new List<ContextMenuStruct>();
			foreach(Keyframe keyframe in curve.keys)
			{
				ContextMenuStruct contextMenuStruct = new ContextMenuStruct();			
				contextMenuStruct.freeSmooth = true;
				if(IsKeyframeFlat(keyframe))
				{
					contextMenuStruct.flat = true;
				}
				listContextMenus.Add(contextMenuStruct);
			}
			dictCurvesContextMenus[curve] = listContextMenus;
		}		
	}
	
	internal  void Remove(AnimationCurve curve){
		dictCurvesContextMenus.Remove(curve);
	}
	
	internal  void UpdateContextMenuList(AnimationCurve curve){
		List<ContextMenuStruct> listContextMenus = new List<ContextMenuStruct>();
		foreach(Keyframe keyframe in curve.keys)
		{
			ContextMenuStruct contextMenuStruct = new ContextMenuStruct();
			contextMenuStruct.freeSmooth = true;
			if(IsKeyframeFlat(keyframe))
			{
				contextMenuStruct.flat = true;
			}
			listContextMenus.Add(contextMenuStruct);
		}
		dictCurvesContextMenus[curve] = listContextMenus;	
	}
	
	/// <summary>
	/// Updates the dictionary of context menu for the given curve.
	/// </summary>
	/// <param name='keysAddedCount'>
	/// Number of keys added(removed, if is negative) outside of this curve editor
	/// </param>
	internal void UpdateDictContextMenu(AnimationCurve curve,int keysAddedCount)
	{
		//TODO
		//a much more correct way of updating the list of context menu structs ,for example inserting or deleting the exact item in/from the list
		//for now,it's just adding/removing at the end of the list, so when just adding a key in the middle of the curve,outside of this curve editor,
		//the curve in this curve editor ,might have some context menu changed(shifted) for the existing keys after the newly added key
		//for now,this easier solution,shouldn't harm , as there is not likely to change a curve outside of this curve editor
		
		List<ContextMenuStruct> listContextMenus = dictCurvesContextMenus[curve];
		if(keysAddedCount > 0)
		{
			for(int i = 0; i < keysAddedCount; ++i)
			{
				ContextMenuStruct contextMenuStruct = new ContextMenuStruct();	
				contextMenuStruct.freeSmooth = true;		
				listContextMenus.Add(contextMenuStruct);
			}
		}
		else
		{
			for(int i = 0; i < -keysAddedCount; ++i)
			{		
				listContextMenus.RemoveAt(curve.length);
			}			
		}
		
	}	
	
}

