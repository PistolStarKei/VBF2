using UnityEngine;

/// <summary>
/// This script based on UIAnchor. Automatically changes the size of box collider.
/// </summary>

[ExecuteInEditMode]
[RequireComponent(typeof (BoxCollider))]
public class UIAnchorCollider : MonoBehaviour
{
	public enum Side
	{
		BottomLeft,
		Left,
		TopLeft,
		Top,
		TopRight,
		Right,
		BottomRight,
		Bottom,
		Center,
	}

	bool mIsWindows = false;

	/// <summary>
	/// Camera used to determine the anchor bounds. Set automatically if none was specified.
	/// </summary>

	public Camera uiCamera = null;

	/// <summary>
	/// Side or corner to anchor to.
	/// </summary>

	public Side side = Side.Center;

	/// <summary>
	/// Whether a half-pixel offset will be applied on windows machines. Most of the time you'll want to leave this as 'true'.
	/// This value is only used if the widget and panel containers were not specified.
	/// </summary>

	public bool halfPixelOffset = true;

	/// <summary>
	/// Relative offset value, if any. For example "0.25" with 'side' set to Left, means 25% from the left side.
	/// </summary>

	public Vector2 relativeOffset = Vector2.zero;
	Transform mTrans;
	Animation mAnim;
	Rect mRect;
	//UIRoot mRoot;
	BoxCollider mCollider;
	float scaleX = 1f;
	float scaleY = 1f;
	
	
	void Awake()
	{
		mTrans = transform;
		mAnim = GetComponent<Animation>(); 
		mRect = new Rect();
		mCollider = (BoxCollider) GetComponent<Collider>();
	}

	/// <summary>
	/// Automatically find the camera responsible for drawing the widgets under this object.
	/// </summary>

	void Start()
	{
		//mRoot = NGUITools.FindInParents<UIRoot>(gameObject);
		mIsWindows = (Application.platform == RuntimePlatform.WindowsPlayer ||
			Application.platform == RuntimePlatform.WindowsWebPlayer ||
			Application.platform == RuntimePlatform.WindowsEditor);

		if (uiCamera == null)
			uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
		
		RecalcScale();		
		Update();
	}

	/// <summary>
	/// Anchor the object to the appropriate point.
	/// </summary>

	void Update()
	{
		if (mAnim != null && mAnim.enabled && mAnim.isPlaying)
			return;
		
		bool useCamera = false;

		if (uiCamera != null)
		{
			useCamera = true;
			mRect = uiCamera.pixelRect;
		}
		else
			return;

		float cx = (mRect.xMin + mRect.xMax) * 0.5f;
		float cy = (mRect.yMin + mRect.yMax) * 0.5f;
		Vector3 v = new Vector3(cx, cy, 0f);
		Vector3 c = new Vector3(cx, cy, 0f);	// center
		Vector3 v1 = new Vector3(mRect.xMin, mRect.yMin, 0f);	// bottom left

		if (side != Side.Center)
		{
			if (side == Side.Right || side == Side.TopRight || side == Side.BottomRight)
				v.x = mRect.xMax;
			else if (side == Side.Top || side == Side.Center || side == Side.Bottom)
				v.x = cx;
			else
				v.x = mRect.xMin;

			if (side == Side.Top || side == Side.TopRight || side == Side.TopLeft)
				v.y = mRect.yMax;
			else if (side == Side.Left || side == Side.Center || side == Side.Right)
				v.y = cy;
			else
				v.y = mRect.yMin;
		}

		float width = mRect.width;
		float height = mRect.height;

		v.x += relativeOffset.x * width;
		v.y += relativeOffset.y * height;
		c.x += relativeOffset.x * width;
		c.y += relativeOffset.y * height;
		v1.x += relativeOffset.x * width;
		v1.y += relativeOffset.y * height;

		if (useCamera)
		{
			if (uiCamera.orthographic)
			{
				v.x = Mathf.RoundToInt(v.x);
				v.y = Mathf.RoundToInt(v.y);
				c.x = Mathf.RoundToInt(c.x);
				c.y = Mathf.RoundToInt(c.y);
				v1.x = Mathf.RoundToInt(v1.x);
				v1.y = Mathf.RoundToInt(v1.y);

				if (halfPixelOffset && mIsWindows)
				{
					v.x -= 0.5f;
					v.y += 0.5f;
					c.x -= 0.5f;
					c.y += 0.5f;
					v1.x -= 0.5f;
					v1.y += 0.5f;
				}
			}

			// Convert from screen to world coordinates, since the two may not match(UIRoot set to manual size)
			v = uiCamera.ScreenToWorldPoint(v);
			c = uiCamera.ScreenToWorldPoint(c);
			v1 = uiCamera.ScreenToWorldPoint(v1);
		}
		else
		{
			v.x = Mathf.RoundToInt(v.x);
			v.y = Mathf.RoundToInt(v.y);
			c.x = Mathf.RoundToInt(c.x);
			c.y = Mathf.RoundToInt(c.y);
			v1.x = Mathf.RoundToInt(v1.x);
			v1.y = Mathf.RoundToInt(v1.y);
		}
		
		// Wrapped in an 'if' so the scene doesn't get marked as 'edited' every frame
		v.z = mTrans.position.z;
		c.z = mTrans.position.z;
		v1.z = mTrans.position.z;
		
		Vector3 newPos = (v + c) / 2f;
		if (mTrans.position != newPos || Application.isEditor)
		{
			mTrans.position = newPos;
			float halfwidth = c.x - v1.x;
			float halfheight = c.y - v1.y;
			
			if (Application.isEditor)
				RecalcScale();
			
			if (side == Side.Right || side == Side.Left)
				mCollider.size = new Vector3(halfwidth / scaleX, 		halfheight * 2f / scaleY, 	0);
			else if (side == Side.Top || side == Side.Bottom)
				mCollider.size = new Vector3(halfwidth * 2f / scaleX, 	halfheight / scaleY, 		0);
			else
				mCollider.size = new Vector3(halfwidth / scaleX, 		halfheight / scaleY, 		0);
		}
	}
	
	void RecalcScale()
	{
		scaleX = mTrans.localScale.x;
		scaleY = mTrans.localScale.y;
		Transform tr1 = mTrans;
		while (tr1.parent != null)
		{
			scaleX = scaleX * tr1.parent.localScale.x;
			scaleY = scaleY * tr1.parent.localScale.y;
			tr1 = tr1.parent;
		}
	}
}
