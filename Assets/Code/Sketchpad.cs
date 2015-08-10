using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sketchpad : MonoBehaviour
{
	public static Sketchpad _instance;
	public Transform planeObject;

	public float brushSize = 0.45f;
	protected float brushMultiplier = 0.025f;
	protected float brushBase = 0.005f;
	protected int selectedTexture;
	

	public ParticleSystem basicBrushParticleSystem;
	public ParticleSystem lflBrushParticleSystem;
	public ParticleSystem splatterBrushParticleSystem;

	Vector3? lastPoint;

	List<ParticleSystem.Particle> pointList;
	List<ParticleSystem.Particle> basicBrushPointList = new List<ParticleSystem.Particle>();
	List<ParticleSystem.Particle> lflBrushPointList = new List<ParticleSystem.Particle>();
	List<ParticleSystem.Particle> splatterBrushPointList = new List<ParticleSystem.Particle>();

	bool particleSystemNeedsUpdate = false;


	protected int selectedColor = 0;
	protected Color[] allColors;
	protected Color primaryColor_01 = new Color( 1f, 1f, 1f, 0.5f ); // white
	protected Color primaryColor_02 = new Color( .996f, .737f, .157f, 0.5f ); //yellow
	protected Color primaryColor_03 = new Color( .262f, .521f, .953f, 0.5f ); //blue
	protected Color primaryColor_04 = new Color( .121f, .592f, .341f, 0.5f ); //green
	protected Color primaryColor_05 = new Color( .733f, .263f, .992f, 0.5f ); //purple
	protected Color primaryColor_06 = new Color( 1f, .565f, .212f, 0.5f ); //orange
	protected Color primaryColor_07 = new Color( .910f, .090f, .549f, 0.5f ); //magenta
	protected Color primaryColor_08 = new Color( .263f, .878f, 1f, 0.5f ); // teal
	protected Color primaryColor_09 = new Color( .933f, .11f, .153f, 0.5f ); //red


	void Awake()
	{
		_instance = this;
		
		pointList = basicBrushPointList;

		// place all of our colors in an array for convenience
		allColors = new Color[9];
		allColors[ 0 ] = primaryColor_01;
		allColors[ 1 ] = primaryColor_02;
		allColors[ 2 ] = primaryColor_03;
		allColors[ 3 ] = primaryColor_04;
		allColors[ 4 ] = primaryColor_05;
		allColors[ 5 ] = primaryColor_06;
		allColors[ 6 ] = primaryColor_07;
		allColors[ 7 ] = primaryColor_08;
		allColors[ 8 ] = primaryColor_09;
	}

	void Start()
	{
		UI_Brain._instance.Initialize();
	}

	void Update()
	{
		if ( UI_Brain._instance.hasInitialized ) {
			CheckUserInput();

			if ( particleSystemNeedsUpdate ) {
				UpdateParticles ();
				particleSystemNeedsUpdate = false;
			}
		}
	}

	// ---------------------------------------------------------------------------------------------

	public void SetSelectedColor( int newSelectedColor ) { selectedColor = newSelectedColor; }

	public void SetSelectedTexture( int newSelectedTexture )
	{
		selectedTexture = newSelectedTexture;

		if ( selectedTexture == 0 ) {
			pointList = basicBrushPointList;
		} else if ( selectedTexture == 1 ) {
			pointList = lflBrushPointList;
		} else if ( selectedTexture == 2 ) {
			pointList = splatterBrushPointList;
		} else {
			Debug.Log ( "Something is wrong." );
		}
	}

	public Color PickRandomColor()
	{
		Color newColor = new Color( Random.Range( 0.1f, 0.9f ), Random.Range( 0.1f, 0.9f ), Random.Range( 0.1f, 0.9f ), 0.5f );
		return newColor;
	}

	// ---------------------------------------------------------------------------------------------

	void CheckUserInput()
	{
		if (Input.GetMouseButton( 0 )) {

			if ( UI_Brain._instance.uiState != UIState.AllClosed ) {
				Debug.Log ( Input.mousePosition.y );
				if ( Input.mousePosition.y < 180 ) {
					// do nothing
				} else {

					// close both palettes
					UI_Brain._instance.CloseInfoPalette();
					UI_Brain._instance.CloseColorPalette();

					// start drawing
					ApplyUserInput();
				}

			} else {

				if ( Input.mousePosition.y < 90 ) {
					// do nothing
				} else {
					// start drawing
					ApplyUserInput();
				}

			}

		} else {

			EndUserInput();

		}
	}

	void ApplyUserInput()
	{
		// Maps to CanvasPlane layer
		var layerMask = 1 << 8;
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		RaycastHit hit;

		if ( Physics.Raycast(ray, out hit, 20f, layerMask) ) {

			DrawPoint( hit.point );

			lastPoint = hit.point;
			particleSystemNeedsUpdate = true;

		}
	}
	
	// ---------------------------------------------------------------------------------------------

	void EndUserInput() { lastPoint = null; }

	void DrawLine( Vector3 p1, Vector3 p2 )
	{
		var dotsPerWorldSpace = 100f;
		var num = dotsPerWorldSpace * (p1 - p2).magnitude;

		for (var i = 0; i < num; i++) {
			var p = Vector3.Lerp( p1, p2, i / num );
			DrawPoint( p );
		}
	}

	// draw one mouse click, one brush to canvas
	void DrawPoint( Vector3 p )
	{
		var particle = new ParticleSystem.Particle ();

		particle.position = p;

		if ( selectedColor < 10 ) { particle.color = allColors[ selectedColor ]; }
		else { particle.color = PickRandomColor(); }

		// a little organic swelling
		particle.size = brushBase + brushMultiplier * brushSize;//*Random.Range( brushBase, (brushBase * 100) );// * Random.Range( 0.5f, 1.5f );

		particle.rotation = Random.Range( 0f, 359f );

		pointList.Add (particle);
	}

	// a complete array of particles is input into a particular particle system
	public void UpdateParticles()
	{
		// Note: this may not be the most efficient way to do this, if we hit performance issues start here
		var asArray = pointList.ToArray();

		if ( selectedTexture == 0 ) {
			basicBrushParticleSystem.SetParticles( asArray, asArray.Length );
		} else if ( selectedTexture == 1 ) {
			lflBrushParticleSystem.SetParticles( asArray, asArray.Length );
		} else if ( selectedTexture == 2 ) {
			splatterBrushParticleSystem.SetParticles( asArray, asArray.Length );
		} else {
			Debug.Log ( "Something is wrong." );
		}
	}

	// remove all of the points from one particular particle array
	public void ClearPoints()
	{
		pointList.Clear();

		/*
		basicBrushPointList.Clear();
		lflBrushPointList.Clear();
		splatterBrushPointList.Clear();

		basicBrushParticleSystem.Clear();
		lflBrushParticleSystem.Clear();
		splatterBrushParticleSystem.Clear();
		*/

		particleSystemNeedsUpdate = true;
	}

	// ---------------------------------------------------------------------------------------------
}