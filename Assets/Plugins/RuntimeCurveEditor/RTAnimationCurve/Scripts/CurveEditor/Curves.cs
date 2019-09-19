using UnityEngine;
using System.Collections.Generic;

public class Curves {
	public static Material lineMaterial;

	public static  Dictionary<AnimationCurve,List<ContextMenuStruct>> dictCurvesContextMenus;
		
	static Color lightGray = new Color(180,180,180);
	
	public static Vector2 SampleBezier(float t,Vector2 p1,Vector2 p2,Vector2 p3,Vector2 p4)
	{
		return (1 - t) * (1 - t) * (1 - t) * p1 + 3.0F*(1 - t) * (1 - t) * t * p2 + 3.0F*(1 - t) * t * t * p3 + t * t * t * p4;
	}
		
	static float mZ = -1.1f;
	public static void DrawBezier(AnimationCurve curve,Color color,int samples,Vector2 p1,Vector2 p2,Vector2 p3,Vector2 p4,Rect gridRect,Rect gradRect,float clip = 1.0f)
	{	
//		Vector2 v1 = Vector2.zero;
//		Vector2 v2 = Vector2.zero;
//		color.a = 1.0f;
//		GL.Color(Color.green);
//		for(int i = 0; i <= samples; ++i)
//		{
//			float t = (float)i/samples;
//			v2.x = p1.x + t*(p4.x - p1.x);
//			v2 = CurveLines.Convert(v2,gradRect,gridRect);
//			v2.y = curve.Evaluate(v2.x);
//			v2 = CurveLines.Convert(v2,gridRect,gradRect);
//			if(i >0)
//			{
//				GL.Begin(GL.LINES);
//           			GL.Vertex3(v1.x, v1.y, mZ);
//            		GL.Vertex3(v2.x, v2.y, mZ);
//				GL.End();
//			}
//			v1 = v2;
//		}
		
		Vector2 v1m = Vector2.zero;
		Vector2 v2m = Vector2.zero;
		color.a = 1.0f;	
		float margin = CurveLines.marginNoDpi * 0.4f;
		for(int i = 0; i <= samples; ++i)
		{
			float t = (float)i/samples;
			if(t > clip) break;
			v2m = SampleBezier(t,p1,p2,p3,p4);
			if(i >0 && (v1m.y <= (gridRect.yMax + margin)) && (v2m.y <= (gridRect.yMax + margin)) &&
						(v1m.y >= (gridRect.yMin - margin)) && (v2m.y >= (gridRect.yMin - margin)))
			{
				GL.Begin(GL.LINES);
					lineMaterial.color = color;
					lineMaterial.SetPass(0);				
           			GL.Vertex3(v1m.x, v1m.y, mZ);
            		GL.Vertex3(v2m.x, v2m.y, mZ);
				GL.End();
			}
			v1m = v2m;
		}
	}
	
	static void DrawConstant(Color color,Vector2 p1,Vector2 p2)
	{
		color.a = 1.0f;
		GL.Begin(GL.LINES);
			lineMaterial.color = color;
			lineMaterial.SetPass(0);
			if(p1.y == p2.y)
			{
           		GL.Vertex3(p1.x, p1.y, mZ);
            	GL.Vertex3(p2.x, p2.y, mZ);			
			}
			else
			{
           		GL.Vertex3(p1.x, p1.y, mZ);
            	GL.Vertex3(p2.x, p1.y, mZ);		
           		GL.Vertex3(p2.x, p1.y, mZ);
            	GL.Vertex3(p2.x, p2.y, mZ);					
			}
		GL.End();		
	}
	
	static Vector2 GetTangLength(Vector2 p1,Vector2 p2)
	{
		Vector2 tangLength = Vector2.zero;
		tangLength.x = Mathf.Abs(p1.x-p2.x)*0.333333f;
		tangLength.y = tangLength.x;
		return tangLength;
	}
		
	public static void GetControlPoints(Vector2 p1,Vector2 p2,float tangOut,float tangIn,out Vector2 c1,out Vector2 c2)
	{
		Vector2 tangLength = GetTangLength(p1,p2);
		
		c1 = p1;
		c2 = p2;
		c1.x += tangLength.x ; 
		c1.y += tangLength.y * tangOut; 
		c2.x -= tangLength.x ; 
		c2.y -= tangLength.y * tangIn; 		
	}
	
	public static void GetTangents(Vector2 p1,Vector2 p2,Vector2 c1,Vector2 c2,out float tangOut,out float tangIn)
	{
		Vector2 tangLength = GetTangLength(p1,p2);
		
		tangOut = (c1.y - p1.y)/tangLength.y;		
		tangIn = (c2.y - p2.y)/tangLength.y;				
	} 

	static void DrawOneCurve(Color color,AnimationCurve curve,bool activeCurve,int selectedKey,Rect gridRect,Rect gradRect,bool isIcon = false,float clip = 1.0f)
	{
		float margin = CurveLines.marginNoDpi*0.6f;
		for(int i = 0; i < curve.length; ++i)
		{
			Vector2 val = new Vector2(curve[i].time,curve[i].value);
			val = CurveLines.Convert(val,gridRect,gradRect);
			
			//outside of the interval,just draw straigt lines outside from the 1st and last key respectively
			if(i == 0 && curve.keys[i].time > gradRect.xMin)
			{
				GL.Begin(GL.LINES);		
					//GL.Color(color);
					lineMaterial.color = color;
					lineMaterial.SetPass(0);
	       			GL.Vertex3(gridRect.xMin, val.y , mZ);
	        		GL.Vertex3(val.x, val.y,mZ);
				GL.End();
			}
			if(i == curve.length - 1 && curve.keys[i].time < gradRect.xMax)
			{
				GL.Begin(GL.LINES);	
					//GL.Color(color);
					lineMaterial.color = color;
					lineMaterial.SetPass(0);
	       			GL.Vertex3(val.x, val.y , mZ);
	        		GL.Vertex3(gridRect.xMax, val.y, mZ);
				GL.End();		
			}
			
			if(curve.length > i + 1)
			{//draw bezier between consecutive keys
				Vector2 val2 = new Vector2(curve[i+1].time,curve[i+1].value);
				val2 = CurveLines.Convert(val2,gridRect,gradRect);
				Vector2 c1 = Vector2.zero; 
				Vector2 c2 = Vector2.zero;
				float tangOut = curve[i].outTangent;
				float tangIn = curve[i+1].inTangent;
				
				float ratio = (gridRect.height/gridRect.width)*(gradRect.width/gradRect.height);
				float tangOutScaled = Mathf.Atan(tangOut*ratio);
				float tangInScaled = Mathf.Atan(tangIn*ratio);
				
				if(tangOut != float.PositiveInfinity && tangIn != float.PositiveInfinity)
				{
					GetControlPoints(val,val2, tangOut*ratio, tangIn*ratio,out c1,out c2);
					int samples = (int)(Screen.height * 0.5f * (val2.x - val.x)); 
								
					DrawBezier(curve,color,samples,val,c1,c2,val2,gridRect,gradRect,clip);
				}
				else
				{
					DrawConstant(color,val,val2);
				}
				
				if(activeCurve)
				{
					if(selectedKey == i )
					{	
						ContextMenuStruct contextMenu = dictCurvesContextMenus[curve][selectedKey];
						if(!contextMenu.auto && (!contextMenu.broken || contextMenu.rightTangent.free))
						{
							Vector2 tangPeak = new Vector2(val.x+CurveLines.tangFixedLength*Mathf.Cos(tangOutScaled),
															val.y+CurveLines.tangFixedLength*Mathf.Sin(tangOutScaled));
							
							GL.Begin(GL.LINES);
							lineMaterial.color = Color.gray;
							lineMaterial.SetPass(0);
							GL.Vertex3(val.x, val.y , mZ);
				        	GL.Vertex3(tangPeak.x, tangPeak.y, mZ);
							GL.End();
							
							DrawQuad(Color.gray,tangPeak,margin);
						}
					}
					
					if(selectedKey == i+1)
					{	
						ContextMenuStruct contextMenu = dictCurvesContextMenus[curve][selectedKey];
						if(!contextMenu.auto && (!contextMenu.broken || contextMenu.leftTangent.free))
						{						
							Vector2 tangPeak = new Vector2(val2.x-CurveLines.tangFixedLength*Mathf.Cos(tangInScaled) ,
															val2.y-CurveLines.tangFixedLength*Mathf.Sin(tangInScaled));				
							
							GL.Begin(GL.LINES);
							lineMaterial.color = Color.gray;
							lineMaterial.SetPass(0);
							GL.Vertex3(val2.x, val2.y , mZ);
				        	GL.Vertex3(tangPeak.x, tangPeak.y, mZ);
							GL.End();					
							
							DrawQuad(Color.gray,tangPeak,margin);	
						}
					}
				}								
			}
			
			if(activeCurve)
			{			
				if(selectedKey == i)
				{	
					DrawQuad(lightGray,val,1.33333f*margin);
				}
			}
			if(!isIcon)
			{
				DrawQuad(color,val,margin);
			}
		}	
	}
	
	public static void DrawCurve(Color color,AnimationCurve curve1,AnimationCurve curve2,bool activeCurve1,bool activeCurve2,int selectedKey,Rect gridRect,Rect gradRect,bool isIcon = false,float clip = 1.0f)
	{
		if(curve2 != null)
		{
			int samples = (int)(Screen.height * 0.5f * gridRect.width);
			Color colorTransp = color;
			colorTransp.a /= 3f;
			Vector2 v1 = Vector2.zero;
			Vector2 v2 = Vector2.zero;
			Vector2 v1prev;
			Vector2 v2prev;
			for(int i = 0 ; i  < samples; ++i)
			{
				v1prev = v1;v2prev = v2;
				float t = (float)i/samples;
				v1.x = gradRect.xMin + t*(gradRect.xMax - gradRect.xMin);
				
				v2.x = v1.x;
				v1.y = curve1.Evaluate(v1.x);
				v1 = CurveLines.Convert(v1,gridRect,gradRect);
				
				v2.y = curve2.Evaluate(v2.x);
				v2 = CurveLines.Convert(v2,gridRect,gradRect);
				
				if(i > 0)
				{
					GL.Begin(GL.QUADS);
						lineMaterial.color = colorTransp;
						lineMaterial.SetPass(0);
						GL.Vertex3(v1prev.x, v1prev.y, mZ);
						GL.Vertex3(v2prev.x, v2prev.y, mZ);
		       			GL.Vertex3(v2.x, v2.y, mZ);
		        		GL.Vertex3(v1.x, v1.y, mZ);
					GL.End();	
				}
			}
			DrawOneCurve(color,curve2,activeCurve2,selectedKey,gridRect,gradRect,isIcon,clip);					
		}
		DrawOneCurve(color,curve1,activeCurve1,selectedKey,gridRect,gradRect,isIcon,clip);
	}
		
	static void DrawQuad(Color color,Vector2 pos,float m)
	{	
		GL.Begin(GL.QUADS);
			lineMaterial.color = color;	
			lineMaterial.SetPass(0);
			GL.Vertex3(pos.x, pos.y - m , mZ);
			GL.Vertex3(pos.x + m, pos.y, mZ);
			GL.Vertex3(pos.x, pos.y + m , mZ);
			GL.Vertex3(pos.x - m, pos.y, mZ);
		GL.End();
	}
}
