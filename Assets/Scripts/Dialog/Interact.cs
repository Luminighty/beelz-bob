using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interact : MonoBehaviour
{
	public bool DisableOnStart;

	#region variables

	public Collider InteractPosition;
	public Collider ClickPos;
	public static GameObject TextBubble;
	public GameObject OptionHolder;
	private GameObject canvas;
	public SpeakLine investigate;
	public SpeakLine interact;
	public int intEventSize;

	private static GameObject player;
	private static Inventory inventory;
	private static PlayerMovement playermov;
	private GameObject pref_text;

	#region Item

	public bool isItem;
	public Item item;
	public bool DestroyOnPickup;
	public SpeakLine haveMax;
	public int TakeMax;
	public int MaxCount;

	#endregion

	#region Dialog

	//soon
	public bool hasDialog;
	public Dialog dialog;

	#endregion

	#region InteractEvent


	public Item interactItem;
	public List<InteractEvent> interactEvents = new List<InteractEvent> ();

	#endregion

	#endregion

	void OnEnable ()
	{
		Awake ();

	}

	void Awake ()
	{
		if (player == null)
			player = GameObject.Find ("Player");
		if (TextBubble == null)
			TextBubble = GameObject.Find ("DefaultTextBubble");
		if (OptionHolder == null) {
			OptionHolder = GameObject.Find ("OptionHolder");
			pref_text = OptionHolder.transform.GetChild (0).gameObject;
			pref_text.SetActive (false);
		}
		if (canvas == null)
			canvas = GameObject.Find ("Canvas");
		if (inventory == null)
			inventory = GameObject.Find ("Inventory").GetComponent<Inventory> ();
		if (playermov == null)
			playermov = player.GetComponent<PlayerMovement> ();
		if (!sentDialog)
			SubDialog (dialog);	
		sentDialog = true;
	}

	bool sentDialog = false;

	void SubDialog (Dialog dial)
	{
		if (dial.ConditionEvent != null)
			dial.ConditionEvent.dialogs.Add (dial);
		if (dial.ConditionEvent2 != null)
			dial.ConditionEvent2.dialogs.Add (dial);
		for (int i = 0; i < dial.Options.Count; i++)
			SubDialog (dial.Options [i]);
	}

	public void onInteract ()
	{
		//Say Stuff
		StartCoroutine (startInteract ());
	}

	public void onInvestigate ()
	{

		StartCoroutine (startInvestigate ());
	}

	IEnumerator startInvestigate ()
	{
		if (investigate != null) {
			playermov.isInteracting = true;
			TextBubble.SetActive (true);
			SetSpeak (investigate, 2);				
			float lenght = (float)getLineLenght (investigate);
			float now = Time.unscaledTime;
			bool skip = false;
			bool isMouseUp = !Input.GetMouseButton (0);

			while (Time.unscaledTime < now + lenght && !skip) {		
				if (!Input.GetMouseButton (0))
					isMouseUp = true;
				if (Input.GetMouseButton (0) && isMouseUp) {
					skip = true;
				}
				yield return null;
			}

			if (investigate.Actor == null)
				investigate.Actor = player;
			if (investigate.Actor.GetComponent<Animator> () != null)
				investigate.Actor.GetComponent<Animator> ().SetBool ("isSpeaking", false);

			playermov.isInteracting = false;
			TextBubble.SetActive (false);
		}
	}

	IEnumerator startInteract ()
	{	
		playermov.isInteracting = true;
		if (isItem) {
			if (MaxCount > inventory.ItemCount (item)) {
				inventory.AddItem (item);
			} else {
				if (haveMax != null) {
					SetSpeak (haveMax, 2);	
					TextBubble.SetActive (true);			
					float lenght = (float)getLineLenght (haveMax);
					float now = Time.unscaledTime;
					bool skip = false;
					bool isMouseUp = !Input.GetMouseButton (0);

					while (Time.unscaledTime < now + lenght && !skip) {		
						if (!Input.GetMouseButton (0))
							isMouseUp = true;
						if (Input.GetMouseButton (0) && isMouseUp && Time.unscaledTime > now + (lenght / 10.0f)) {
							skip = true;
						}
						yield return null;
					}
					TextBubble.SetActive (false);
				}
				if (haveMax.Actor == null)
					haveMax.Actor = player;
				if (haveMax.Actor.GetComponent<Animator> () != null)
					haveMax.Actor.GetComponent<Animator> ().SetBool ("isSpeaking", false);
				playermov.isInteracting = false;
				yield break;
			}
		}

		InteractEvent e = new InteractEvent ();

		bool found = false;
		foreach (InteractEvent ev in interactEvents)
			if (ev.item == interactItem) {
				e = ev;
				found = true;
				break;
			}
		inventory.HeldItem = -1;

		if (found) {
			StartCoroutine (InteractEventTrigger (e));
			yield break;
		}

		if (hasDialog) {
			StartCoroutine (DoDialog (dialog, false));
			yield break;
		}

		if (interact != null && interact.text != "" && (interactEvents.Count == 0 || !found || e.DoInteract)) {
			TextBubble.SetActive (true);
			SetSpeak (interact, 2);		
			float lenght = (float)getLineLenght (interact);
			float now = Time.unscaledTime;
			bool skip = false;
			bool isMouseUp = !Input.GetMouseButton (0);

			while (Time.unscaledTime < now + lenght && !skip) {	
				if (!Input.GetMouseButton (0))
					isMouseUp = true;
				if (Input.GetMouseButton (0) && isMouseUp && Time.unscaledTime > now + (lenght / 10.0f)) {
					skip = true;
				}
				yield return null;
			}
			playermov.isInteracting = false;
			TextBubble.SetActive (false);
			if (interact.Actor == null)
				interact.Actor = player;
			if (interact.Actor.GetComponent<Animator> () != null)
				interact.Actor.GetComponent<Animator> ().SetBool ("isSpeaking", false);
		}


		playermov.isInteracting = false;
		if (isItem && DestroyOnPickup)
			Destroy (gameObject);
		if (GetComponent<Events> () != null && GetComponent<Events> ().trigger == Events.eventTrigger.onInteract)
			GetComponent<Events> ().Trigger ();
			
	}

	public IEnumerator DoDialog (Dialog d, bool skipLines)
	{

		if (d.getItem != null)
			inventory.AddItem (d.getItem);

		foreach (Transform trans in OptionHolder.transform) {
			if (trans.gameObject != pref_text)
				Destroy (trans.gameObject);
		}
		if (!skipLines) {
			TextBubble.SetActive (true);
			for (int i = 0; i < d.lines.Count; i++) {
				SetSpeak (d.lines [i], 2);
				float lenght = (float)getLineLenght (d.lines [i]);
				float now = Time.unscaledTime;
				bool skip = false;
				bool isMouseUp = !Input.GetMouseButton (0);

				while (Time.unscaledTime < now + lenght && !skip) {		
					if (!Input.GetMouseButton (0))
						isMouseUp = true;
					if (Input.GetMouseButton (0) && isMouseUp && Time.unscaledTime > now + (lenght / 10.0f)) {
						skip = true;
					}
					yield return null;
				}
				if (d.lines [i].Actor == null)
					d.lines [i].Actor = player;
				if (d.lines [i].Actor.GetComponent<Animator> () != null)
					d.lines [i].Actor.GetComponent<Animator> ().SetBool ("isSpeaking", false);
			}
			TextBubble.SetActive (false);
		}

		if (d.TriggerEvent != null)
			d.TriggerEvent.Trigger ();
		
		foreach (GameObject enable in d.EnableObjects.objects)
			enable.SetActive (true);
		foreach (GameObject disable in d.DisableObjects.objects)
			disable.SetActive (false);
		foreach (GameObject toggle in d.ToggleObjects.objects)
			toggle.SetActive (!toggle.activeSelf);

		if (d.DestroyOnPick)
			d.isAvailable = false;

		if (d.isEnd) {
			playermov.isInteracting = false;
			if (GetComponent<Events> () != null && GetComponent<Events> ().trigger == Events.eventTrigger.onInteract)
				GetComponent<Events> ().Trigger ();
			yield break;
		}

		if (d.GoUp > 0) {
			dialogHierarchy.Reverse ();
			int i = d.GoUp - 1;
			Dialog newDialog = dialogHierarchy [i];
			for (int j = 0; j < i; j++)
				dialogHierarchy.RemoveAt (j);
			dialogHierarchy.Reverse ();
			StartCoroutine (DoDialog (newDialog, true));
			yield break;
		}
		dialogHierarchy.Add (d);
		GameObject pref_Text = OptionHolder.transform.GetChild (0).gameObject;

		foreach (Dialog option in d.Options) {
			if (!option.isAvailable)
				continue;

			GameObject obj_option = Instantiate (pref_Text) as GameObject;
			obj_option.name = "not Default! Definietly not...";
			obj_option.SetActive (true);
			obj_option.transform.SetParent (OptionHolder.transform);

			Text text = obj_option.GetComponentInChildren<Text> ();
			text.text = option.optionName;
			text.color = Color.white;
			TextGenerator textgen = new TextGenerator ();
			TextGenerationSettings genset = text.GetGenerationSettings (text.rectTransform.rect.size);
			float width = textgen.GetPreferredWidth (option.optionName, genset);
			Vector2 size = obj_option.GetComponent<RectTransform> ().sizeDelta;
			size.x = width;
			obj_option.GetComponent<RectTransform> ().sizeDelta = size;
			obj_option.GetComponent<LayoutElement> ().preferredHeight = textgen.GetPreferredHeight (option.optionName, genset);
			OptionButton button = obj_option.GetComponent<OptionButton> ();
			button.dial = option;
			button.inter = this;

		}
	}


	float getLineLenght (SpeakLine line)
	{
		return Mathf.Max (line.text.Length * 0.1f, 2.5f);
	}

	IEnumerator InteractEventTrigger (InteractEvent e)
	{
		inventory.HeldItem = -1;
		interactItem = null;
		if (e.DestroyItem)
			inventory.RemoveItem (e.item);
		if (e.getItem != null)
			inventory.AddItem (e.getItem);

		GameObject disableSelf = null;

		foreach (GameObject obj in e.DisableObjects.objects) {
			if (obj == gameObject) {
				disableSelf = obj;
				continue;
			}
			obj.SetActive (false);
		}
		foreach (GameObject obj in e.EnableObjects.objects)
			obj.SetActive (true);
		foreach (GameObject obj in e.ToggleObjects.objects) {
			if (obj == gameObject) {
				disableSelf = obj;
				continue;
			}
			obj.SetActive (!obj.activeSelf);
		}
		

		if (e.speak.Count != 0) {
			TextBubble.SetActive (true);
			playermov.isInteracting = true;
			for (int i = 0; i < e.speak.Count; i++) {
				SetSpeak (e.speak [i], 2);				
				float lenght = (float)getLineLenght (e.speak [i]);
				float now = Time.unscaledTime;
				bool skip = false;
				bool isMouseUp = !Input.GetMouseButton (0);

				while (Time.unscaledTime < now + lenght && !skip) {		
					if (!Input.GetMouseButton (0))
						isMouseUp = true;
					if (Input.GetMouseButton (0) && isMouseUp && Time.unscaledTime > now + (lenght / 10.0f)) {
						skip = true;
					}
					yield return null;
				}
				if (e.speak [i].Actor == null)
					e.speak [i].Actor = player;		
				if (e.speak [i].Actor.GetComponent<Animator> () != null) {
					e.speak [i].Actor.GetComponent<Animator> ().SetBool ("isSpeaking", false);
				}
			}
		}

		TextBubble.SetActive (false);
		playermov.isInteracting = false;

		if (e.TriggerEvent != null)
			e.TriggerEvent.Trigger ();
		if (disableSelf != null)
			disableSelf.SetActive (false);
	}

	void SetSpeak (SpeakLine line, float yOffset)
	{

		TextBubble.GetComponent<Text> ().text = line.text;
		if (line.Actor == null)
			line.Actor = player;

		if (line.audio != null) {
			Camera.main.GetComponent<AudioSource> ().clip = line.audio;
			
			Camera.main.GetComponent<AudioSource> ().Play ();

		}


		Animator anim = line.Actor.GetComponent<Animator> ();
		if (anim != null) {
			if (line.animation != null && line.animation != "") {
				anim.SetTrigger (line.animation);

			} else {
				anim.SetBool ("isSpeaking", true);
			}
		}
		Vector3 pos = line.Actor.transform.position;

		if (line.Actor.GetComponent<BubbleOffset> () != null) {
			pos.x += line.Actor.GetComponent<BubbleOffset> ().offset.x;
			pos.z += line.Actor.GetComponent<BubbleOffset> ().offset.y;
		} else {
			pos.y += yOffset;
		}
		pos = Camera.main.WorldToScreenPoint (pos);
		TextBubble.transform.position = pos;
		TextBubble.GetComponent<TextFitter> ().NewPos ();


		//TextFitter

		//pos = TextBubble.GetComponent<TextFitter> ().NewPos ();

		//Debug.Log ("xmax: " + rect.rect.xMax + " - xmin: " + rect.rect.xMin);
	}


	[System.NonSerialized]
	List<Dialog> dialogHierarchy = new List<Dialog> ();

}


[System.Serializable]
public class InteractEvent
{

	public Item item;
	public bool DestroyItem;
	public bool DoInteract;
	//Do the default interact too

	public List<SpeakLine> speak = new List<SpeakLine> ();
	//What to say after interact.

	public int speakCount;
	public bool speak_isOpen;
	public List<bool> speakOpens = new List<bool> ();

	public Item getItem;


	public Events TriggerEvent = null;

	public EventObjects ToggleObjects;
	public EventObjects DisableObjects;
	public EventObjects EnableObjects;

	public int toggleCount;
	public int disableCount;
	public int enableCount;

	public bool toggle_isOpen;
	public bool disable_isOpen;
	public bool enable_isOpen;
}

[System.Serializable]
public class Dialog
{

	public string optionName = "";
	public List<SpeakLine> lines = new List<SpeakLine> ();
	public List<Dialog> Options = new List<Dialog> ();

	public int GoUp = 0;
	public bool isEnd = false;
	public bool DestroyOnPick = false;

	public bool isAvailable = true;

	public Item getItem = null;
	public Events ConditionEvent = null;
	public Events ConditionEvent2 = null;
	public Events TriggerEvent = null;

	public EventObjects ToggleObjects;
	public EventObjects DisableObjects;
	public EventObjects EnableObjects;


	public bool isOpen;
	public bool isLinesOpen;
	public bool isgroupFoldoutOpen;

	public int optionSize;
	public int linesSize;


	public void ChangeOptionsSize ()
	{
		if (this.Options == null)
			this.Options = new List<Dialog> ();
		while (this.optionSize > this.Options.Count) {
			this.Options.Add (null);
		}
		while (this.optionSize < this.Options.Count) {
			this.Options.RemoveAt (this.Options.Count - 1);
		}
	}

	public void ChangeLinesSize ()
	{

		if (this.lines == null)
			this.lines = new List<SpeakLine> ();
		while (this.linesSize > this.lines.Count) {
			this.lines.Add (null);
		}
		while (this.linesSize < this.lines.Count) {
			this.lines.RemoveAt (this.lines.Count - 1);
		}

	}

	public void TriggeredEvent ()
	{
		this.isAvailable = !this.isAvailable;
	}

}

[System.Serializable]
public class EventObjects
{
	public List<GameObject> objects = new List<GameObject> ();
	public bool isOpen;
	public int Size;

	public void ChangeSize ()
	{
		if (this.objects == null)
			this.objects = new List<GameObject> ();
		while (this.Size > this.objects.Count) {
			this.objects.Add (null);
		}
		while (this.Size < this.objects.Count) {
			this.objects.RemoveAt (this.objects.Count - 1);
		}
	}

}

[System.Serializable]
public class SpeakLine
{
	[TextArea]
	public string text = "";
	public AudioClip audio = null;
	public string animation = "";
	public GameObject Actor;

	public bool foldout_isOpen;
}

