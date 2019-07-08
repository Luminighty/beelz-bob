using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;



[CustomEditor (typeof(Interact))]
public class InteractInspector : Editor
{

	bool interactOpen;
	bool investigateOpen;
	bool haveMax;

	bool intEventArray;
	List<bool> intEvents = new List<bool> ();


	Interact myTarget;


	public override void OnInspectorGUI ()
	{
		myTarget = (Interact)target;

		GUI.enabled = false;
		EditorGUILayout.ObjectField ("Script", MonoScript.FromMonoBehaviour ((Interact)target), typeof(Interact), false);
		GUI.enabled = true;

		myTarget.InteractPosition = (Collider)EditorGUILayout.ObjectField (new GUIContent ("Interact Position", "The position the player needs to be in to interact with the object."), myTarget.InteractPosition, typeof(Collider), true);
		myTarget.ClickPos = (Collider)EditorGUILayout.ObjectField (new GUIContent ("Click Position", "The position the player needs to click to interact with the object."), myTarget.ClickPos, typeof(Collider), true);

		myTarget.DisableOnStart = EditorGUILayout.ToggleLeft (new GUIContent ("Disable on Start", "Useful when you want to subscribe an event before this gets activated"), myTarget.DisableOnStart);

		myTarget.investigate = AddSpeakLine (myTarget.investigate, "Investigate");
		if (!myTarget.hasDialog)
			myTarget.interact = AddSpeakLine (myTarget.interact, "Interact");


		#region Item
		EditorGUILayout.Space ();
		myTarget.isItem = EditorGUILayout.ToggleLeft ("Is Item", myTarget.isItem);
		if (myTarget.isItem) {
			EditorGUI.indentLevel++;
			myTarget.item = (Item)EditorGUILayout.ObjectField (new GUIContent ("Item", "The item the player will get."), myTarget.item, typeof(Item), false);
			myTarget.DestroyOnPickup = EditorGUILayout.Toggle (new GUIContent ("Destroy on Pickup", "Should the object be destroyed if the player picks it up?"), myTarget.DestroyOnPickup);
			if (!myTarget.DestroyOnPickup) {
				myTarget.TakeMax = EditorGUILayout.DelayedIntField (new GUIContent ("Take Max", "Maximum number the player can take of this item. For infinite set it to negative"), myTarget.TakeMax);
				if (myTarget.TakeMax < 0) {
					if (myTarget.TakeMax < -1) {
						myTarget.TakeMax = -1;
						myTarget.MaxCount = 1;
					}
					myTarget.MaxCount = EditorGUILayout.DelayedIntField (new GUIContent ("Max Count", "Maximum number the player can have of this item."), myTarget.MaxCount);
					haveMax = EditorGUILayout.Foldout (haveMax, new GUIContent ("Have Max", "What should the player say if he has enough"), true);
					if (haveMax) {
						EditorGUI.indentLevel++;
						myTarget.haveMax.text = EditorGUILayout.DelayedTextField ("Text", myTarget.haveMax.text, EditorStyles.textArea);
						myTarget.haveMax.audio = (AudioClip)EditorGUILayout.ObjectField ("audio", myTarget.haveMax.audio, typeof(AudioClip), false);
						EditorGUI.indentLevel--;
					} 
				}
			} else {
				myTarget.TakeMax = -1;
				myTarget.MaxCount = 1;
			}
			EditorGUI.indentLevel--;
		}
		EditorGUILayout.Space ();
		#endregion

		#region Dialog
		EditorGUILayout.Space ();
		myTarget.hasDialog = EditorGUILayout.ToggleLeft (new GUIContent ("HasDialog", "Does the object has Dialog (if so it ignores the Interact Speakline)"), myTarget.hasDialog);
		if (myTarget.hasDialog)
			myTarget.dialog = AddDialog (myTarget.dialog, new GUIContent ("Dialog"), false);

		EditorGUILayout.Space ();
		#endregion

		#region InteractEvents
		/*if (myTarget.interactEvents == null) {
			myTarget.interactEvents = new List<InteractEvent> ();
			Debug.Log ("New Interactevents");
		}*/
		

		intEventArray = EditorGUILayout.Foldout (intEventArray, "Interact Events", true);
		if (intEventArray) {
			EditorGUI.indentLevel++;
			myTarget.intEventSize = EditorGUILayout.DelayedIntField ("Size", myTarget.intEventSize);

			if (myTarget.intEventSize != myTarget.interactEvents.Count || intEvents.Count != myTarget.interactEvents.Count)
				ChangeSize ();

			for (int i = 0; i < myTarget.intEventSize; i++) {
				EditorGUI.indentLevel++;
				InteractEvent intEvent = myTarget.interactEvents [i];
				string itemName = "Hand";
				if (intEvent.item != null)
					itemName = myTarget.interactEvents [i].item.name;

				intEvents [i] = EditorGUILayout.Foldout (intEvents [i], itemName + " : " + i.ToString (), true);
				if (!intEvents [i]) {
					EditorGUI.indentLevel--;
					continue;
				}
				EditorGUI.indentLevel++;
				intEvent.TriggerEvent = (Events)EditorGUILayout.ObjectField (new GUIContent ("Trigger Event", "Triggers the event"), intEvent.TriggerEvent, typeof(Events), true);

				intEvent.item = (Item)EditorGUILayout.ObjectField (new GUIContent ("Item needed", "The item the player will interact with this object."), intEvent.item, typeof(Item), false);
				intEvent.getItem = (Item)EditorGUILayout.ObjectField (new GUIContent ("Get Item", "The item the player will get when interacts with the object."), intEvent.getItem, typeof(Item), false);
				intEvent.DestroyItem = EditorGUILayout.ToggleLeft (new GUIContent ("Destroy Item", "Should the item be destroyed when used?"), intEvent.DestroyItem);
				intEvent.DoInteract = EditorGUILayout.ToggleLeft (new GUIContent ("Do Interact", "Should the upper interact be played when this event occurs?"), intEvent.DoInteract);

				EditorGUILayout.Space ();

				intEvent.speak_isOpen = EditorGUILayout.Foldout (intEvent.speak_isOpen, "Speak Lines", true);
				if (intEvent.speak_isOpen) {
					
					EditorGUI.indentLevel++;
					intEvent.speakCount = EditorGUILayout.DelayedIntField ("Size", intEvent.speakCount);
					if (intEvent.speak.Count != intEvent.speakCount)
						ChangeSpeakSize (intEvent.speakCount, ref intEvent.speak, ref intEvent.speakOpens);
					for (int l = 0; l < intEvent.speakCount; l++) {
						intEvent.speak [l] = AddSpeakLine (intEvent.speak [l], "Element " + l.ToString ());
					}

					EditorGUI.indentLevel--;
				}

				EditorGUILayout.Space ();

				AddGameObjectArray (ref intEvent.ToggleObjects, new GUIContent ("Toggle GameObjects", "Toggle Gameobject active state on interact"));
				AddGameObjectArray (ref intEvent.EnableObjects, new GUIContent ("Enable GameObjects", "Enable Gameobject active state on interact"));
				AddGameObjectArray (ref intEvent.DisableObjects, new GUIContent ("Disable GameObjects", "Disable Gameobject active state on interact"));

				EditorGUI.indentLevel--;
				EditorGUI.indentLevel--;
				EditorGUILayout.Space ();
			}
			EditorGUI.indentLevel--;
		}

		#endregion

	}
	/*
	void ChangeGameObjectSize (int size, ref List<GameObject> list)
	{
		if (list == null)
			list = new List<GameObject> ();
		while (size > list.Count) {
			list.Add (null);
		}
		while (size < list.Count) {
			list.RemoveAt (list.Count - 1);
		}
	}
*/
	void ChangeSpeakSize (int size, ref List<SpeakLine> list, ref List<bool> speakOpens)
	{

		while (list.Count != speakOpens.Count) {
			if (speakOpens.Count > list.Count) {
				speakOpens.RemoveAt (intEvents.Count - 1);
			} else {
				speakOpens.Add (false);
			}
		}

		if (list == null)
			list = new List<SpeakLine> ();
		while (size > list.Count) {
			list.Add (null);
			speakOpens.Add (false);
		}
		while (size < list.Count) {
			list.RemoveAt (list.Count - 1);
			speakOpens.RemoveAt (speakOpens.Count - 1);
		}
	}

	void ChangeSize ()
	{
		//Makes the bool and the interactevents the same size
		while (intEvents.Count != myTarget.interactEvents.Count) {
			if (intEvents.Count > myTarget.interactEvents.Count) {
				intEvents.RemoveAt (intEvents.Count - 1);
			} else {
				intEvents.Add (false);
			}
		}

		//Sets the size to the myTarget.intEventSize
		while (myTarget.intEventSize > intEvents.Count) {
			intEvents.Add (false);
			myTarget.interactEvents.Add (new InteractEvent ());
		}
		while (myTarget.intEventSize < intEvents.Count) {
			intEvents.RemoveAt (intEvents.Count - 1);
			myTarget.interactEvents.RemoveAt (myTarget.interactEvents.Count - 1);
		}
	}

	Dialog AddDialog (Dialog dialog)
	{
		if (dialog == null)
			dialog = new Dialog ();
		GUIContent content = new GUIContent (dialog.optionName);
		return AddDialog (dialog, content, true);
	}

	Dialog AddDialog (Dialog dialog, GUIContent content, bool canChangeOptionText)
	{
		GUIStyle boldFoldOut = new GUIStyle (EditorStyles.foldout);
		boldFoldOut.font = EditorStyles.boldFont;
		if (dialog == null)
			dialog = new Dialog ();
		string name = content.text;
		if (name == "" || name == null)
			name = "Empty";
		if (!canChangeOptionText) {
			dialog.isOpen = EditorGUILayout.Foldout (dialog.isOpen, name, true, boldFoldOut);
			if (!dialog.isOpen)
				return dialog;
		}
		EditorGUI.indentLevel++;

		if (canChangeOptionText)
			dialog.optionName = EditorGUILayout.DelayedTextField (new GUIContent ("Option Name", "The text the player will click"), dialog.optionName);

		dialog.isEnd = EditorGUILayout.ToggleLeft (new GUIContent ("IsEnd", "Should the conversation end?"), dialog.isEnd);
		dialog.DestroyOnPick = EditorGUILayout.ToggleLeft (new GUIContent ("DestroyOnPick", "Should the option be destroyed upon picking it?"), dialog.DestroyOnPick);
		if (!dialog.isEnd)
			dialog.GoUp = EditorGUILayout.DelayedIntField (new GUIContent ("GoUp", "After this dialog, how much should the conversation go up (0 to do nothing"), dialog.GoUp);
		dialog.GoUp = Mathf.Max (dialog.GoUp, 0);

		dialog.isAvailable = EditorGUILayout.ToggleLeft (new GUIContent ("isAvailable", "Can this be choosen by default (Note: Condition event toggles it)"), dialog.isAvailable);
		dialog.ConditionEvent = (Events)EditorGUILayout.ObjectField (new GUIContent ("Condition Event", "On trigger toggles the is Available"), dialog.ConditionEvent, typeof(Events), true);
		dialog.ConditionEvent2 = (Events)EditorGUILayout.ObjectField (new GUIContent ("Condition Event2", "On trigger toggles the is Available"), dialog.ConditionEvent2, typeof(Events), true);
		dialog.TriggerEvent = (Events)EditorGUILayout.ObjectField (new GUIContent ("Trigger Event", "Triggers the event"), dialog.TriggerEvent, typeof(Events), true);
		dialog.getItem = (Item)EditorGUILayout.ObjectField (new GUIContent ("Get Item", "Get an item when this is happening."), dialog.getItem, typeof(Item), true);

		EditorGUILayout.Space ();

		dialog.isLinesOpen = EditorGUILayout.Foldout (dialog.isLinesOpen, "SpeakLines", true, boldFoldOut);
		if (dialog.isLinesOpen) {
			EditorGUI.indentLevel++;
			dialog.linesSize = EditorGUILayout.DelayedIntField ("Size", dialog.linesSize);
			if (dialog.linesSize != dialog.lines.Count)
				dialog.ChangeLinesSize ();

			for (int i = 0; i < dialog.lines.Count; i++) {
				AddSpeakLine (dialog.lines [i], "Element " + i.ToString (), dialog, i);
				
			}
			EditorGUI.indentLevel--;
		}

		if (!dialog.isEnd && dialog.GoUp < 1) {
			EditorGUILayout.Space ();
			dialog.isgroupFoldoutOpen = EditorGUILayout.Foldout (dialog.isgroupFoldoutOpen, "Options", true, boldFoldOut);
			if (dialog.isgroupFoldoutOpen) {
				EditorGUI.indentLevel++;
				dialog.optionSize = EditorGUILayout.DelayedIntField ("Size", dialog.optionSize);
				if (dialog.optionSize != dialog.Options.Count)
					dialog.ChangeOptionsSize ();

				for (int i = 0; i < dialog.Options.Count; i++) {
					if (dialog.Options [i] == null)
						dialog.Options [i] = new Dialog ();
					EditorGUILayout.BeginHorizontal ();
					dialog.Options [i].isOpen = EditorGUILayout.Foldout (dialog.Options [i].isOpen, dialog.Options [i].optionName, true, boldFoldOut);
					EditorGUILayout.Space ();
					EditorGUILayout.Space ();
					if (GUILayout.Button ("Move Up") && i > 0)
						MoveUp (ref dialog, i);
					if (GUILayout.Button ("Delete")) {
						Remove (ref dialog, dialog.Options [i]);
						continue;
					}
					if (GUILayout.Button ("Move Down") && i < dialog.Options.Count - 1)
						MoveDown (ref dialog, i);

					EditorGUILayout.EndHorizontal ();

					if (dialog.Options [i].isOpen)
						AddDialog (dialog.Options [i]);

				}
				EditorGUI.indentLevel--;
			}
		}

		EditorGUILayout.Space ();

		AddGameObjectArray (ref dialog.ToggleObjects, new GUIContent ("Toggle GameObjects", "Toggle Gameobject active state on interact"));
		AddGameObjectArray (ref dialog.EnableObjects, new GUIContent ("Enable GameObjects", "Enable Gameobject active state on interact"));
		AddGameObjectArray (ref dialog.DisableObjects, new GUIContent ("Disable GameObjects", "Disable Gameobject active state on interact"));

		EditorGUI.indentLevel--;
		return dialog;
	}

	void AddGameObjectArray (ref EventObjects objs, GUIContent content)
	{
		objs.isOpen = EditorGUILayout.Foldout (objs.isOpen, content, true);
		if (objs.isOpen) {
			EditorGUI.indentLevel++;

			objs.Size = EditorGUILayout.DelayedIntField ("Size", objs.Size);
			if (objs == null)
				objs.objects = new List<GameObject> ();

			if (objs.Size != objs.objects.Count)
				objs.ChangeSize ();
			for (int j = 0; j < objs.Size; j++)
				objs.objects [j] = (GameObject)EditorGUILayout.ObjectField (new GUIContent (("Element " + j.ToString ()), content.tooltip), objs.objects [j], typeof(GameObject), true);

			EditorGUI.indentLevel--;
		}

	}

	SpeakLine AddSpeakLine (SpeakLine line, string name)
	{
		if (line == null)
			line = new SpeakLine ();
		line.foldout_isOpen = EditorGUILayout.Foldout (line.foldout_isOpen, name, true);
		if (line.foldout_isOpen) {
			EditorGUI.indentLevel++;
			line.text = EditorGUILayout.TextField ("Text", line.text);
			line.audio = (AudioClip)EditorGUILayout.ObjectField ("Audio", line.audio, typeof(AudioClip), false);
			line.animation = EditorGUILayout.TextField ("Animation", line.animation);
			line.Actor = (GameObject)EditorGUILayout.ObjectField ("Actor", line.Actor, typeof(GameObject), true);
			EditorGUI.indentLevel--;
		}
		return line;
	}

	SpeakLine AddSpeakLine (SpeakLine line, string name, Dialog parent, int index)
	{
		if (line == null)
			line = new SpeakLine ();


		EditorGUILayout.BeginHorizontal ();
		GUIStyle bold = new GUIStyle (EditorStyles.foldout);
		bold.font = EditorStyles.boldFont;
		line.foldout_isOpen = EditorGUILayout.Foldout (line.foldout_isOpen, name, true, bold);
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		if (GUILayout.Button ("Move Up") && index > 0)
			MoveUp (ref parent.lines, index);
		if (GUILayout.Button ("Delete")) {
			Remove (ref parent.lines, line);
			parent.linesSize = parent.lines.Count;
		}
		if (GUILayout.Button ("Move Down") && index < parent.lines.Count - 1)
			MoveDown (ref parent.lines, index);

		EditorGUILayout.EndHorizontal ();


		if (line.foldout_isOpen) {
			EditorGUI.indentLevel++;
			line.text = EditorGUILayout.TextField ("Text", line.text);
			line.audio = (AudioClip)EditorGUILayout.ObjectField ("Audio", line.audio, typeof(AudioClip), false);
			line.animation = EditorGUILayout.TextField ("Animation", line.animation);
			line.Actor = (GameObject)EditorGUILayout.ObjectField ("Actor", line.Actor, typeof(GameObject), true);
			EditorGUI.indentLevel--;
		}
		return line;
	}

	void MoveUp (ref Dialog parent, int i)
	{

		Dialog save = parent.Options [i - 1];
		parent.Options [i - 1] = parent.Options [i];
		parent.Options [i] = save;

	}

	void MoveDown (ref Dialog parent, int i)
	{
		Dialog save = parent.Options [i + 1];
		parent.Options [i + 1] = parent.Options [i];
		parent.Options [i] = save;
	}

	void Remove (ref Dialog parent, Dialog self)
	{
		parent.Options.Remove (self);
		parent.optionSize--;
	}

	void MoveUp (ref List<SpeakLine> parent, int i)
	{

		SpeakLine save = parent [i - 1];
		parent [i - 1] = parent [i];
		parent [i] = save;

	}

	void MoveDown (ref List<SpeakLine> parent, int i)
	{
		SpeakLine save = parent [i + 1];
		parent [i + 1] = parent [i];
		parent [i] = save;
	}

	void Remove (ref List<SpeakLine> parent, SpeakLine self)
	{
		parent.Remove (self);
	}

}
#endif