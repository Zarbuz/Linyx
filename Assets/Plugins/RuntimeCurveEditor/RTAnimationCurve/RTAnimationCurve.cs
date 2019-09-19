// Runtime Curve Editor
// Copyright © 2013 Rus Artur PFA
// rus_artur@yahoo.com
//----------------------------------------------
using UnityEngine;
using System.Collections.Generic;

public class RTAnimationCurve : MonoBehaviour
{
    //root game object for the whole curve editor window
    Transform rootCurveEditor = null;
    //use this for showing/hiding the curve editor window
    CurveLines mCrvLines = null;

    //use this to make possible the calling(and effect) of SetLayers method,both before or after
    //the rootCurveEditor is instantiated
    bool setLayers = false;
    //the below values are used if the setLayers is true
    int curveEditorLayer = 0;
    int colliderLayer = 0;

    void Init()
    {
        if (rootCurveEditor == null)
        {
            rootCurveEditor = (Instantiate(Resources.Load("RootCurveEditor")) as GameObject)?.transform;
            if (setLayers)
            {
                SetLayers(curveEditorLayer, colliderLayer);
            }

            if (mCrvLines != null) return;
            if (rootCurveEditor != null) mCrvLines = rootCurveEditor.Find("CameraCurve").GetComponent<CurveLines>();
        }
    }

    /// <summary>
    /// Shows the curve editor(instantiate if it's not available).
    /// </summary>
    public void ShowCurveEditor()
    {
        Init();
        if (mCrvLines != null)
        {
            mCrvLines.ShowWindow();
        }
    }

    /// <summary>
    /// Closes(hides) the curve editor.
    /// </summary>
    public void CloseCurveEditor()
    {
        if (mCrvLines != null)
        {
            mCrvLines.CloseWindow();
        }
    }

    public bool IsCurveEditorClosed()
    {
        if (mCrvLines != null)
        {
            return mCrvLines.windowClosed;
        }
        else return true;
    }

    /// <summary>
    /// Add the specified animCurve to the curve editor(if it's already added,nothing happens).
    /// </summary>
    /// <returns>false if the curve couldn't be added.
    /// </returns>
    public bool Add(ref AnimationCurve animCurve)
    {
        if (mCrvLines == null) return false;
        if (!mCrvLines.CurveShown(animCurve))
        {
            if (animCurve == null)
            {
                animCurve = new AnimationCurve();
            }
            if (animCurve.length == 0)
            {
                animCurve.AddKey(0f, 0f);
            }
            mCrvLines.AddCurveStruct(animCurve, null);
        }
        return true;
    }

    /// <summary>
    /// Similar to the above Add method, but it adds a path of two curves.
    /// If the pair of curve is already added ,nothing happens,if animCurve1 is added, then the second curve is updated.
    /// </summary>
    public bool Add(ref AnimationCurve animCurve1, ref AnimationCurve animCurve2)
    {
        if (mCrvLines == null) return false;
        if (!mCrvLines.CurveShown(animCurve1, animCurve2))
        {
            if (animCurve1 == null)
            {
                animCurve1 = new AnimationCurve();
            }
            if (animCurve1.length == 0)
            {
                animCurve1.AddKey(0f, 0f);
            }

            if (animCurve2 == null)
            {
                animCurve2 = new AnimationCurve();
            }
            if (animCurve2.length == 0)
            {
                animCurve2.AddKey(0f, 0f);
            }

            mCrvLines.AddCurveStruct(animCurve1, animCurve2);
        }
        return true;
    }

    /// <summary>
    /// Remove the specified curve(or pairs of curves,if curve1 equals the given curve).
    /// </summary>
    public void Remove(AnimationCurve curve)
    {
        if (null != mCrvLines)
        {
            mCrvLines.RemoveCurve(curve);
        }
    }

    /// <summary>
    /// Gets or sets the gradations rect.
    /// </summary>
    public Rect gradRect
    {
        set { if (mCrvLines != null) mCrvLines.gradRect = value; }
        get
        {
            if (mCrvLines == null) return new Rect(0, 0, 0, 0);
            return mCrvLines.gradRect;
        }
    }

    /// <summary>
    /// Set the gradations' Y range(and keeps the existing X range) .
    /// </summary>
    public void SetGradYRange(float yMin, float yMax)
    {
        if (mCrvLines != null)
        {
            Rect tempRect = mCrvLines.gradRect;
            tempRect.yMin = yMin;
            tempRect.yMax = yMax;
            mCrvLines.gradRect = tempRect;
        }
    }

    /// <summary>
    /// Set the gradations' X range(and keeps the existing Y range) .
    /// </summary>
    public void SetGradXRange(float xMin, float xMax)
    {
        if (mCrvLines != null)
        {
            Rect tempRect = mCrvLines.gradRect;
            tempRect.xMin = xMin;
            tempRect.xMax = xMax;
            mCrvLines.gradRect = tempRect;
        }
    }

    /// <summary>
    /// Gets the active curve(or curve 1 ,in the case a path with two curves is active).
    /// </summary>
    public AnimationCurve activeCurve
    {
        get
        {
            if (null == mCrvLines) return null;
            return mCrvLines.activeCurveStr.curve1;
        }
    }

    /// <summary>
    /// Gets the color of the active curve.
    /// </summary>
    public Color activeCurveColor
    {
        get
        {
            if (null == mCrvLines) return Color.clear;
            return mCrvLines.activeCurveStr.color;
        }
    }

    /// <summary>
    /// Use this method if you want to set specific layers for curve editors and colliders respectively
    /// Also note,that the camera's culling mask will be set to the curve editor's layer
    /// </summary>
    /// <param name='curveEditorLayer'>
    /// Curve editor layer.
    /// </param>
    /// <param name='colliderLayer'>
    /// Collider layer.
    /// </param>
    public void SetLayers(int curveEditorLayer, int colliderLayer)
    {
        if (rootCurveEditor == null)
        {//this is the case ,rootCurveEditor is not yet instantiated(so this method will be called once when it will be instantiated)
            setLayers = true;
            this.curveEditorLayer = curveEditorLayer;
            this.colliderLayer = colliderLayer;
        }
        else
        {
            setLayers = false;
            rootCurveEditor.Find("Collider").gameObject.layer = colliderLayer;
            rootCurveEditor.gameObject.layer = curveEditorLayer;
            rootCurveEditor.Find("CurveEditor").gameObject.layer = curveEditorLayer;
            Transform cameraCurve = rootCurveEditor.Find("CameraCurve");
            cameraCurve.gameObject.layer = curveEditorLayer;
            cameraCurve.GetComponent<Camera>().cullingMask = 1 << curveEditorLayer;
        }
    }

    /// <summary>
    /// Saves the data.
    /// </summary>
    /// <param name='name'>
    /// Name of the configuration to be stored.
    /// </param>
    /// <param name='obj'>
    /// Object whose fields of AnimationCurve type ,will be stored.
    /// </param>
    public void SaveData(string name, Object obj)
    {
        Init();
        if (mCrvLines != null)
        {
            mCrvLines.SaveData(name, obj);
        }
    }

    /// <summary>
    /// Loads the data.
    /// </summary>
    /// <param name='name'>
    /// Name of the configuration to be loaded.
    /// </param>
    /// <param name='obj'>
    /// Object whose fields of AnimationCurve type,will be loaded.
    /// </param>
    public void LoadData(string name, Object obj)
    {
        Init();
        if (mCrvLines != null)
        {
            mCrvLines.ShowWindow();
            mCrvLines.LoadData(name, obj);
        }
    }

    /// <summary>
    /// Remove all the curves and place/resize the window to initial.
    /// </summary>
    public void NewWindow()
    {
        if (mCrvLines != null)
        {
            mCrvLines.NewWindow();
        }
    }

    /// <summary>
    /// True if the curve is visible in the editor(if it's added to the editor).
    /// </summary>
    public bool CurveVisible(AnimationCurve curve1)
    {
        if (mCrvLines != null)
        {
            return mCrvLines.CurveShown(curve1);
        }
        return false;
    }

    /// <summary>
    /// True if the curves are visible ,as path, in the editor(if the path is added to the editor).
    /// </summary>	
    public bool CurvesVisible(AnimationCurve curve1, AnimationCurve curve2)
    {
        if (mCrvLines != null)
        {
            return mCrvLines.CurveShown(curve1, curve2);
        }
        return false;
    }

    /// <summary>
    /// Returns true if something has been modified in the current configuration
    /// (a curve added/removed/modified/selected,the editor's window resized/moved,grad Y range changed,
    /// or any key's context menu changed)
    /// </summary>
    public bool DataAltered()
    {
        if (mCrvLines != null)
        {
            return mCrvLines.alteredData;
        }
        return false;
    }

    /// <summary>
    /// Returns the names of all saved configurations.
    /// </summary>
    public List<string> GetNamesList()
    {
        Init();
        if (mCrvLines != null)
        {
            return mCrvLines.GetNamesList();
        }
        return new List<string>();
    }

    /// <summary>
    /// Deletes file.
    /// </summary>
    public void DeleteFile(string name)
    {
        if (mCrvLines != null)
        {
            mCrvLines.DeleteFile(name);
        }
    }

    /// <summary>
    /// Gets the last loaded file(if no such file,returns null).
    /// </summary>
    public string GetLastFile()
    {
        Init();
        if (mCrvLines != null)
        {
            return mCrvLines.GetLastFile();
        }
        return null;
    }

}
