using UnityEngine;
using System.Collections;

public enum UIState{ AllClosed, InfoPaletteOpen, ToolPaletteOpen };

public class UI_Brain : MonoBehaviour
{
	public static UI_Brain _instance;

	public bool hasInitialized = false;

	public UIState uiState;

	// the UI_Brain object recieves events from various buttons within the UI and responds accordingly
	// most of the UI must be manually linked to this object from within the Unity authoring environment

	public GameObject touchToDrawButton;
	public GameObject touchToDrawLabel;
	public GameObject colorToolPaletteButton;
	public GameObject infoPaletteButton;
	public GameObject clearAllButton;
	public GameObject spaceSketcherLogo;
	
	//motion tween
	public TweenTransform iconPaletteTweenOpen;
	public TweenTransform infoPaletteTweenOpen;
	public TweenTransform colorPaletteTweenOpen;
	
	//color tween
	public TweenColor infoFadeTween;
	public TweenColor colorFadeTween;
	public TweenColor clearFadeTween;


	public UIButton[] texturePaletteButtons;
	public UISprite[] texturePaletteRings;
	public UIButton[] colorPaletteButtons;

	public UISlider brushSizeSlider;

	protected Color selectedSwatchColor;
	protected int selectedSwatchTexture;

	public GameObject infoPalette;
	public GameObject colorPalette;
	
	// ---------------------------------------------------------------------------------------------

	void Awake()
	{
		_instance = this;
		uiState = UIState.AllClosed;
	}
	
	void Start()
	{
		// doing nothing for now, wait until Sketchpad comes alive and initializes UI_Brain remotely
	}

	public void Initialize()
	{
		CloseColorPalette();
		CloseInfoPalette();

		hasInitialized = true;

		// set the initial texture to the basic brush and the initial color to white
		Color_Selected( Random.Range( 1, 8 ) );
		Texture_Selected( Random.Range( 1, 2 ) );
		
		brushSizeSlider.value = 0.5f;
	}

	// ---------------------------------------------------------------------------------------------
	
	public void OnBrushSizeSliderChange()
	{
		if ( hasInitialized ) Sketchpad._instance.brushSize = brushSizeSlider.value;
	}

	public void OnClearButtonPressed()
	{
		if ( hasInitialized ) Sketchpad._instance.ClearPoints();
	}
	
	// ---------------------------------------------------------------------------------------------

	public void Texture_01_Selected() { Texture_Selected( 0 ); }
	public void Texture_02_Selected() { Texture_Selected( 1 ); }
	public void Texture_03_Selected() { Texture_Selected( 2 ); }

	public void Texture_Selected( int selectedTexture )
	{
		if ( hasInitialized ) {
			selectedSwatchTexture = selectedTexture;

			ResetTextureSwatches();

			texturePaletteButtons[ selectedSwatchTexture ].defaultColor = new Color( selectedSwatchColor.r, selectedSwatchColor.g, selectedSwatchColor.b, 1f );
			texturePaletteRings[ selectedSwatchTexture ].color = new Color( selectedSwatchColor.r, selectedSwatchColor.g, selectedSwatchColor.b, 1f );
			
			Sketchpad._instance.SetSelectedTexture( selectedSwatchTexture );
			Sketchpad._instance.UpdateParticles();
		}
	}

	// ---------------------------------------------------------------------------------------------

	public void Color_01_Selected() { Color_Selected( 0 ); }
	public void Color_02_Selected() { Color_Selected( 1 ); }
	public void Color_03_Selected() { Color_Selected( 2 ); }
	public void Color_04_Selected() { Color_Selected( 3 ); }
	public void Color_05_Selected() { Color_Selected( 4 ); }
	public void Color_06_Selected() { Color_Selected( 5 ); }
	public void Color_07_Selected() { Color_Selected( 6 ); }
	public void Color_08_Selected() { Color_Selected( 7 ); }
	public void Color_09_Selected() { Color_Selected( 8 ); }

	public void Color_Selected( int selectedColor )
	{
		if ( hasInitialized ) {
			ResetColorSwatches();
			selectedSwatchColor = colorPaletteButtons[ selectedColor ].defaultColor;
			colorPaletteButtons[ selectedColor ].defaultColor = new Color( selectedSwatchColor.r, selectedSwatchColor.g, selectedSwatchColor.b, 1f );
			
			Sketchpad._instance.SetSelectedColor( selectedColor );

			Texture_Selected( selectedSwatchTexture );
		}
	}

	// ---------------------------------------------------------------------------------------------

	protected void ResetTextureSwatches()
	{
		if ( hasInitialized ) {
			// go through the whole array of swatches and de-select them all
			for ( int index = 0; index < texturePaletteButtons.Length; index++ ) {
				// set this swatch back to deselected, and make them white
				texturePaletteButtons[ index ].defaultColor = new Color( 1f, 1f, 1f, 0.5f );
				texturePaletteRings[ index ].color = new Color( 1f, 1f, 1f, 0.5f );
			}
		}
	}

	protected void ResetColorSwatches()
	{
		if ( hasInitialized ) {
			// go through the whole array of swatches and de-select them all
			for ( int index = 0; index < colorPaletteButtons.Length; index++ ) {
				// set this swatch back to deselected
				Color swatchColor = colorPaletteButtons[ index ].defaultColor;
				colorPaletteButtons[ index ].defaultColor = new Color( swatchColor.r, swatchColor.g, swatchColor.b, .10f );
			}
		}
	}

	// ---------------------------------------------------------------------------------------------

	public void TouchToDraw()
	{
		// button disappears on touch
		HideTouchToDrawInfo();
	}

	// ---------------------------------------------------------------------------------------------

	public void OpenCloseToolPaletteTouched() { if ( uiState == UIState.ToolPaletteOpen ) { CloseColorPalette(); } else { OpenColorPalette(); } }

	public void OpenColorPalette()
	{
		ShowTouchToDrawInfo();

		iconPaletteTweenOpen.PlayForward();
		colorPaletteTweenOpen.PlayForward();
		AllColorTweenOff ();

		uiState = UIState.ToolPaletteOpen;
	}

	public void CloseColorPalette()
	{
		HideTouchToDrawInfo();
		
		iconPaletteTweenOpen.PlayReverse();
		colorPaletteTweenOpen.PlayReverse();
		AllColorTweenOn ();

		uiState = UIState.AllClosed;
	}

	// ---------------------------------------------------------------------------------------------

	public void OpenCloseInfoPaletteTouched() { if ( uiState == UIState.InfoPaletteOpen ) { CloseInfoPalette(); } else { OpenInfoPalette(); } }

	public void OpenInfoPalette()
	{
		iconPaletteTweenOpen.PlayForward ();
		infoPaletteTweenOpen.PlayForward ();

		AllColorTweenOff ();

		uiState = UIState.InfoPaletteOpen;
	}
	
	public void CloseInfoPalette()
	{
		iconPaletteTweenOpen.PlayReverse();
		infoPaletteTweenOpen.PlayReverse ();

		AllColorTweenOn();
		
		uiState = UIState.AllClosed;
	}

	// ---------------------------------------------------------------------------------------------

	
	protected void HideTouchToDrawInfo()
	{
		// there are a few functions that turn off these items, so it's been consolidated to avoid duplication
		touchToDrawButton.gameObject.SetActive( false );
		touchToDrawLabel.gameObject.SetActive( false );
	}
	
	protected void ShowTouchToDrawInfo()
	{
		// there are a few functions that turn off these items, so it's been consolidated to avoid duplication
		touchToDrawButton.gameObject.SetActive( true );
		touchToDrawLabel.gameObject.SetActive( true );
		touchToDrawLabel.gameObject.GetComponent<UILabel>().text = "Touch to Draw";
	}
	
	// ---------------------------------------------------------------------------------------------


	public void AllColorTweenOn()
	{
		infoFadeTween.PlayReverse();
		colorFadeTween.PlayReverse();
		clearFadeTween.PlayReverse();
	}

	public void AllColorTweenOff()
	{
		infoFadeTween.PlayForward();
		colorFadeTween.PlayForward();
		clearFadeTween.PlayForward();
	}
}