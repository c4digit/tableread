using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using frame8.Logic.Misc.Other.Extensions;
using frame8.ScrollRectItemsAdapter.Util;

/// <summary>
/// <para>Learn by example: The items can have a disabled ContentSizeFitter added, which will be enabled 1 frame, until </para> 
/// <para><see cref="ScrollRectItemsAdapter8{TParams, TItemViewsHolder}.OnItemHeightChangedPreTwinPass(TItemViewsHolder)"/> </para> 
/// <para>(or <see cref="ScrollRectItemsAdapter8{TParams, TItemViewsHolder}.OnItemWidthChangedPreTwinPass(TItemViewsHolder)"/> if horizontal ScrollRect)</para> 
/// <para>is called, after a "Twin" ComputeVisibility() pass of  executes, which lasts 1 frame.</para> 
/// <para>A "Twin" <see cref="ScrollRectItemsAdapter8{TParams, TItemViewsHolder}.ComputeVisibility(float)"/> pass is executed the next frame <see cref="ScrollRectItemsAdapter8{TParams, TItemViewsHolder}.ScheduleComputeVisibilityTwinPass()"/> is called.</para> 
/// <para>Because this setup relies on doing stuff on multiple frames, it only works with <see cref="adapterParams"/>.<see cref="BaseParams.updateMode"/> is set to <see cref="BaseParams.UpdateMode.MONOBEHAVIOUR_UPDATE"/>.</para> 
/// </summary>
public class scriptController : MonoBehaviour
{

	/// <summary>Configuration visible in the inspector</summary>
	public MyParams adapterParams;

	/// <summary>Holds the number of items which will be contained in the ScrollView</summary>
	//   public Text countText;

	//	public RectTransformEdgeDragger edgeDragger;

	// Instance of your ScrollRectItemsAdapter8 implementation
	public MyScrollRectAdapter _Adapter;

	//const string LOREM_IPSUM = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.";



	void Start()
	{
	//	Debug.Log ("scriptController Start===========================");
		_Adapter = new MyScrollRectAdapter();

		// Need to initialize adapter after a few frames, so Unity will finish initializing its layout
		StartCoroutine(DelayedInit());

		// Notify the adapter than its size had changed so it can re-layout the views
		//	edgeDragger.TargetDragged += _Adapter.NotifyScrollViewSizeChanged;
	}

	public void clear() {
		OnDestroy ();
		Start ();
	}

	void OnDestroy()
	{
		// The adapter has some resources that need to be disposed after you destroy the scroll view
		if (_Adapter != null)
			_Adapter.Dispose();
	}

	// Initialize the adapter after 3 frames
	// You can also try calling Canvas.ForceUpdateCanvases() instead if you for some reason can't wait 3 frames, although it wasn't tested
	IEnumerator DelayedInit()
	{
		// Wait 3 frames
		//   yield return null;
		//  yield return null;
		yield return null;

		_Adapter.Init(adapterParams);
		// Initially set the number of items to the number in the input field
		//  UpdateItems();
	}

	/// <summary>Callback from UI Button. Parses the text in <see cref="countText"/> as an int and sets it as the new item count, refreshing all the views. It mocks a basic server communication where you request x items and you receive them after a few seconds</summary>
	public void UpdateItems(int newCount = 1024)
	{
		if (newCount == 0) {
			Debug.Log ("error with script");
			return;
		}
		//   int.TryParse(countText.text, out newCount);
		// Generating some random models
		var newModelx = new ExampleItemModel[newCount];
		for (int i = 0; i < newCount; ++i)
		{	
			newModelx[i] = new ExampleItemModel();
			newModelx [i].Title = trglobals.instance._trvs._scriptlines [i].linestr;
			newModelx [i].index = i;
			//newModelx
			// newModelx[i].Title = LOREM_IPSUM.Substring(0, UnityEngine.Random.Range(LOREM_IPSUM.Length / 50 + 1, LOREM_IPSUM.Length / 2));
		}

		adapterParams.data.Clear();
		adapterParams.data.AddRange(newModelx);

		// Just notify the adapter the data changed, so it can refresh the views (even if the count is the same, this must be done)
		_Adapter.ChangeItemCountTo(newModelx.Length);
	}

	public class ExampleItemModel
	{
		/// <summary><see cref="enstimatedSize"/> is invalidated each time this property changes</summary>
		public string Title
		{
			get { return _Title; }
			set
			{
				if (_Title != value)
				{
					_Title = value;
					enstimatedSize = -1;
				}
			}
		}

		/// <summary><see cref="enstimatedSize"/> is invalidated each time this property changes</summary>
		public int index
		{
			get { return _index; }
			set
			{
				if (_index != value)
				{
					_index = value;
					enstimatedSize = -1;
				}
			}
		}

		string _Title;
		int _index;

		/// <summary>Part of the view state. Represents the last size that the content size fitter estimated for this model. Caching it here saves some memory and CPU cycles</summary>
		public float enstimatedSize = -1;
	}


	#region ScrollRectItemsAdapter8 code
	// This in almost all cases will contain the prefab and your list of models
	[Serializable] // serializable, so it can be shown in inspector
	public class MyParams : BaseParams
	{
		//  public Texture2D[] availableIcons; // used to randomly generate models;
		public RectTransform prefab;

		public List<ExampleItemModel> data = new List<ExampleItemModel>();
	}

	/// <summary>
	/// <para>The ContentSizeFitter should be attached to the item itself</para>
	/// <para>The <see cref="canvasGroup"/> is added here optionally, but if you want just a quick start, you can ignore it. 
	/// It's here to hide (not disable) the item during the frame it's being calculated its size, otherwise a quick flicker will appear from time to time</para>
	/// </summary>
	public class MyItemViewsHolder : BaseItemViewsHolder
	{
		public Text titleText;
		public scriptlineDetails	scriptlinedetail;
		public ContentSizeFitter contentSizeFitter;
		public	HorizontalLayoutGroup	horizontalLayoutGroup;
		CanvasGroup canvasGroup;

		/// <summary>
		/// <para>Becomes true before the frame where its size will be calculated by the ContentSizeFitter (which becomes enabled)</para> 
		/// <para>Set it to false when the new size was retrieved, so the ContentSizeFitter will be again disabled</para> 
		/// </summary>
		public bool ContentFitPending
		{
			get { return _IsContentFitPending; }
			set
			{
				_IsContentFitPending = value;
				contentSizeFitter.enabled = _IsContentFitPending;
				if (canvasGroup)
					canvasGroup.alpha = _IsContentFitPending ? 0f : 1f; // hide it when the content size is being calculated
			}
		}
		bool _IsContentFitPending;


		public override void CollectViews()
		{
			base.CollectViews();

			horizontalLayoutGroup = root.GetComponent<HorizontalLayoutGroup>();
			contentSizeFitter = root.GetComponent<ContentSizeFitter>();
			contentSizeFitter.enabled = false; // the content size fitter should not be enabled during normal lifecycle, only in the "Twin" pass frame
			canvasGroup = root.GetComponent<CanvasGroup>();
			titleText = root.Find("TitlePanel/scriptTXT").GetComponent<Text>();
			scriptlinedetail = root.GetComponent<scriptlineDetails>();
			//	icon1Image = root.Find("Icon1Image").GetComponent<RawImage>();
		}

		/// <summary>Utility getting rid of the need of manually writing assignments</summary>
		public void UpdateFromModel(ExampleItemModel model)
		{
			if (trglobals.instance._trvs._scriptlines.Count == 0)
				return;
			titleText.text = model.Title;
			scriptlinedetail.linenumber = model.index;
			horizontalLayoutGroup.padding.left = trglobals.instance._trvs._scriptlines [model.index].lmargin;
			horizontalLayoutGroup.padding.right = trglobals.instance._trvs._scriptlines [model.index].rmargin;
			if (trglobals.instance._trvs._scriptlines [model.index].hasNote && trglobals.instance.displayActiveNotes == 1 && trglobals.instance.goPRO)
				scriptlinedetail.note.gameObject.SetActive (true);
			else
				scriptlinedetail.note.gameObject.SetActive (false);
			if (trglobals.instance._trvs._scriptlines [model.index].type == trglobals.SCRIPTLINE_TYPE.SCENE)
				scriptlinedetail.text.color = trglobals.instance.scenelineCLR [1];
			else 
				scriptlinedetail.text.color = trglobals.instance.scenelineCLR [0];
			if (trglobals.instance._trvs._scriptlines [model.index].type == trglobals.SCRIPTLINE_TYPE.DIALOUGE && trglobals.instance.goPRO) {
				if (trglobals.instance._trvs.isRehearsalActor (trglobals.instance._trvs._scriptlines [model.index].actor))
					scriptlinedetail.cell.color = trglobals.instance.rehearseCol;
				else
					scriptlinedetail.cell.color = Color.white;
			}
			else
				scriptlinedetail.cell.color = Color.white;
			if (trglobals.instance._trvs._scriptlines [model.index].active) {
			//	Debug.Log ("OLD SCRIPT LINE IS NULL:" + (trglobals.instance._trvs.oldscriptline == null));
				if (trglobals.instance._trvs.oldscriptline == null)
					trglobals.instance._trvs.oldscriptline = scriptlinedetail;
				if (trglobals.instance._trvs.paused)
					scriptlinedetail.text.color = trglobals.instance.scenelineCLR [2];
				scriptlinedetail.setBorderGrey ();
			} else {
				scriptlinedetail.setBorderWhite ();
			}
		}
	}

		/// <summary><para>The custom adapter implementation that'll manage the list of items using rules customized for our use-case</para></summary>
		public class MyScrollRectAdapter : ScrollRectItemsAdapter8<MyParams, MyItemViewsHolder>
		{
			protected override float GetItemHeight(int index)
			{ return _Params.data[index].enstimatedSize == -1 ? _Params.prefab.rect.height : _Params.data[index].enstimatedSize; }

			// Not called in our case
			protected override float GetItemWidth(int index) { return -1; }

			/// <summary>See <see cref="ScrollRectItemsAdapter8{TParams, TItemViewsHolder}.CreateViewsHolder(int)"/></summary>
			protected override MyItemViewsHolder CreateViewsHolder(int itemIndex)
			{
				var instance = new MyItemViewsHolder();
				instance.Init(_Params.prefab, itemIndex);

				return instance;
			}

			/// <summary>See <see cref="ScrollRectItemsAdapter8{TParams, TItemViewsHolder}.OnItemHeightChangedPreTwinPass(TItemViewsHolder)"/></summary>
			protected override void OnItemHeightChangedPreTwinPass(MyItemViewsHolder viewHolder)
			{
				base.OnItemHeightChangedPreTwinPass(viewHolder);

				_Params.data[viewHolder.itemIndex].enstimatedSize = viewHolder.root.rect.height;
				viewHolder.ContentFitPending = false;
			}

			/// <summary>See <see cref="ScrollRectItemsAdapter8{TParams, TItemViewsHolder}.UpdateViewsHolder(TItemViewsHolder)"/></summary>
			protected override void UpdateViewsHolder(MyItemViewsHolder newOrRecycled)
			{
				// Initialize the views from the associated model
				ExampleItemModel model = _Params.data[newOrRecycled.itemIndex];

				newOrRecycled.UpdateFromModel(model);

				if (model.enstimatedSize == -1)
				{
					// Height will be available before the next 'twin' pass, inside OnItemHeightChangedPreTwinPass() callback (see above)
					newOrRecycled.ContentFitPending = true;
					ScheduleComputeVisibilityTwinPass();
				}
				else
					newOrRecycled.ContentFitPending = false;
			}

			protected override void RebuildLayoutDueToScrollViewSizeChange()
			{
				// Invalidate the estimated size. A new "Twin" pass will be done
				foreach (var model in _Params.data)
					model.enstimatedSize = -1;

				base.RebuildLayoutDueToScrollViewSizeChange();
			}
		}
		#endregion
	}