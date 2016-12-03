﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class Robot : MonoBehaviour {

	public int move_nJoint  = 0;
	public float move_Value = 0.0f;
	public int 		nWorlds_;
	GameObject[] 	worlds_;
	float[]			a,d,th,al;
	bool [,]		variableMatrix;

	public Text xText, yText, zText;

	// Use this for initialization
	void Start () {
		nWorlds_ = PlayerPrefs.GetInt ("nWorlds");
		variableMatrix = new bool[nWorlds_, 4];
		Set_DHMatrix ();
		CreateRobot ();
		CreateLinks_Joints ();
	}

	// Update is called once per frame
	void Update () {
		Vector4 pos = CinDir ();

		xText.text = "x = " + pos.x.ToString();
		yText.text = "y = " + pos.y.ToString();
		zText.text = "z = " + pos.z.ToString();

		if (Input.GetKeyUp (KeyCode.Escape)) {
			Application.LoadLevel ("SetUpRobotScene");
		}

	}

	public void SetJointMovement(string nJoint){
		move_nJoint = int.Parse(nJoint);
	}

	public void SetValueMovement(string value){
		move_Value = float.Parse(value);
	}

	public void MoveJoint(){

		int number = 0;
		while (!variableMatrix[move_nJoint, number])
			number++;


		if (number == 0){	//D
			d[move_nJoint] = GetNumber (PlayerPrefs.GetString ("world" + move_nJoint.ToString () + "d")) +  move_Value;
			Transform D = worlds_[move_nJoint].transform.parent.Find("linkD" + move_nJoint);
			D.localScale = new Vector3 (0.25f, (d [move_nJoint]) / 2, 0.25f);
			D.localPosition = new Vector3 (0.0f, 0.0f, (d [move_nJoint]) / 2);
			Vector3 pos = worlds_ [move_nJoint].transform.localPosition;
			pos.z = d [move_nJoint];
			worlds_ [move_nJoint].transform.localPosition = pos;
		}
		else if (number == 1){	//TH
			worlds_ [move_nJoint].transform.RotateAround(
				worlds_ [move_nJoint].transform.parent.position, 
				worlds_ [move_nJoint].transform.parent.forward, 
				th[move_nJoint]
			);

			th[move_nJoint] = GetNumber (PlayerPrefs.GetString ("world" + move_nJoint.ToString () + "th")) +  move_Value;

			worlds_ [move_nJoint].transform.RotateAround(
				worlds_ [move_nJoint].transform.parent.position, 
				worlds_ [move_nJoint].transform.parent.forward, 
				-th[move_nJoint]
			);
		}
		else if (number == 2) {	//A
			a [move_nJoint] = GetNumber (PlayerPrefs.GetString ("world" + move_nJoint.ToString () + "a")) +  move_Value;

			Transform A = worlds_[move_nJoint].transform.Find("linkA" + move_nJoint);
			A.localScale = new Vector3 (0.25f, (a [move_nJoint]) / 2, 0.25f);
			A.localPosition = new Vector3 (-(a [move_nJoint]) / 2, 0.0f, 0.0f);

			Vector3 pos = worlds_ [move_nJoint].transform.localPosition;
			pos.x = a [move_nJoint];
			worlds_ [move_nJoint].transform.localPosition = pos;
		}

		else if (number == 3){	//AL
			worlds_ [move_nJoint].transform.Rotate (new Vector3(al[move_nJoint], 0.0f,   0.0f));
			al[move_nJoint] = GetNumber (PlayerPrefs.GetString ("world" + move_nJoint.ToString () + "al")) +  move_Value;
			worlds_ [move_nJoint].transform.Rotate (new Vector3(-al[move_nJoint], 0.0f,   0.0f));
		}


	}

	void CreateLinks_Joints () {
		for (int i = 1; i < nWorlds_; i++) {
			// **************************** LINKS ********************************
			if (d [i] > 0) { //Z
				GameObject link = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
				link.name = "linkD" + i;
				link.transform.localScale = new Vector3 (0.25f, (d [i])/2, 0.25f);
				link.transform.parent = worlds_ [i].transform.parent;
				link.transform.rotation = worlds_ [i].transform.parent.rotation * Quaternion.Euler(90.0f, 0.0f, 0.0f);
				link.transform.localPosition = new Vector3 (0.0f, 0.0f, (d [i])/2);
			}
			if (a [i] > 0) {
				GameObject link = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
				link.name = "linkA" + i;
				link.transform.localScale = new Vector3 (0.25f, (a[i])/2, 0.25f);
				link.transform.parent = worlds_ [i].transform;
				link.transform.localRotation =  Quaternion.Euler(90.0f, 0.0f, 90.0f);

				link.transform.localPosition = new Vector3 (- (a[i])/2, 0.0f, 0.0f);
				//link.transform.parent = worlds_ [i-1].transform;
			}

			// **************************** JOINTS ********************************

			if (variableMatrix [i, 0]) {	//d
				GameObject j = GameObject.CreatePrimitive (PrimitiveType.Cube);
				j.transform.parent = worlds_ [i].transform.parent;
				j.transform.localScale = new Vector3 (0.3f, 0.6f, 0.3f);
				j.transform.localPosition = Vector3.zero;
			}
			if (variableMatrix [i, 1]) {	//th
				GameObject j = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
				j.transform.parent = worlds_ [i].transform;
				j.transform.localScale = new Vector3 (0.3f, 0.3f, 0.3f);

				j.transform.localRotation = Quaternion.Euler (90.0f, 0.0f, 0.0f);
				j.transform.parent =  worlds_ [i].transform.parent;
				j.transform.localPosition = Vector3.zero;
			}
			if (variableMatrix [i, 2]) {	//a
				GameObject j = GameObject.CreatePrimitive (PrimitiveType.Cube);
				j.transform.parent = worlds_ [i].transform.parent;
				j.transform.localScale = new Vector3 (0.6f, 0.3f, 0.3f);
				j.transform.localPosition = Vector3.zero;
			}
			if (variableMatrix [i, 3]) {	//al
				GameObject j = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
				j.transform.parent = worlds_ [i].transform.parent;
				j.transform.localScale = new Vector3 (0.3f, 0.3f, 0.3f);
				j.transform.localPosition = Vector3.zero;
				j.transform.localRotation = Quaternion.Euler (0.0f, 0.0f, 90.0f);
			}



		}
	}

	void CreateRobot () {
		worlds_ = new GameObject[nWorlds_];


		GameObject baseAxis = new GameObject();
		baseAxis.name = "world0";
		float firstXRotation = PlayerPrefs.GetFloat ("XRotation");
		float firstYRotation = 0f;
		float firstZRotation = PlayerPrefs.GetFloat ("ZRotation");
		baseAxis.transform.Rotate (new Vector3(firstXRotation, firstYRotation, firstZRotation));

		for (int i = 0; i < nWorlds_; i++) {

			worlds_[i] = new GameObject();
			if (i == 0)
				worlds_ [i].transform.parent = baseAxis.transform;
			else
				worlds_ [i].transform.parent = worlds_[i-1].transform;

			worlds_ [i].transform.localRotation = Quaternion.identity; 
			worlds_ [i].name = "world" + (i+1).ToString();


			// ROTATIONS AND TRANSLATIONS
			worlds_ [i].transform.localPosition = new Vector3 (a[i], 0.0f, 0.0f);


			worlds_ [i].transform.localRotation = Quaternion.Euler (0.0f,  0.0f, -th[i]);
			worlds_ [i].transform.Rotate (new Vector3(-al[i], 0.0f,   0.0f));


			worlds_ [i].transform.localPosition += new Vector3 (0.0f, 0.0f, d[i]);

		}
		//baseAxis.transform.DetachChildren();
		//Destroy (baseAxis);
	}

	Matrix4x4 GetT (int pos){
		Matrix4x4 T;

		T.m00 = Mathf.Cos(Mathf.Deg2Rad * th[pos]); 
		T.m01 = -Mathf.Sin(Mathf.Deg2Rad * th[pos]) * Mathf.Cos(Mathf.Deg2Rad * al[pos]);
		T.m02 = Mathf.Sin(Mathf.Deg2Rad * th[pos]) * Mathf.Sin(Mathf.Deg2Rad * al[pos]);
		T.m03 = a[pos] * Mathf.Cos(Mathf.Deg2Rad * th[pos]);

		T.m10 = Mathf.Sin(Mathf.Deg2Rad * th[pos]);
		T.m11 = Mathf.Cos(Mathf.Deg2Rad * th[pos]) * Mathf.Cos(Mathf.Deg2Rad * al[pos]);
		T.m12  = -Mathf.Sin(Mathf.Deg2Rad * al[pos]) * Mathf.Cos(Mathf.Deg2Rad* th[pos]);
		T.m13  = a[pos] * Mathf.Sin(Mathf.Deg2Rad * th[pos]);

		T.m20  = 0.0f;
		T.m21  = Mathf.Sin(Mathf.Deg2Rad * al[pos]);
		T.m22  = Mathf.Cos(Mathf.Deg2Rad * al[pos]);
		T.m23  = d[pos];

		T.m30 = 0.0f;
		T.m31 = 0.0f;
		T.m32 = 0.0f;
		T.m33 = 1.0f;

		return T;

	}

	Vector4 CinDir () {

		Matrix4x4 T = Matrix4x4.identity;

		for (int i = 0; i < nWorlds_; i++) 
			T = T * GetT (i);

		return T * (new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

	}


	float GetNumber (string value){
		string resultString = Regex.Match (value, @"[-+]?[0-9]*\.?[0-9]").Value;
		if (resultString == "")
			return 0;
		else
			return float.Parse (resultString);
	}

	void Set_DHMatrix () {
		a  = new float[nWorlds_];
		d  = new float[nWorlds_];
		th = new float[nWorlds_];
		al = new float[nWorlds_];

		for (int i = 1; i < nWorlds_; i++) {
			a[i] = GetNumber (PlayerPrefs.GetString ("world" + i.ToString () + "a"));
			d[i] = GetNumber (PlayerPrefs.GetString ("world" + i.ToString () + "d"));
			al[i] = GetNumber (PlayerPrefs.GetString ("world" + i.ToString () + "al"));
			th[i] = GetNumber (PlayerPrefs.GetString ("world" + i.ToString () + "th"));

			variableMatrix[i,0] = PlayerPrefs.GetInt ("variable" + i.ToString ()) == 0;
			variableMatrix[i,1] = PlayerPrefs.GetInt ("variable" + i.ToString ()) == 1;
			variableMatrix[i,2] = PlayerPrefs.GetInt ("variable" + i.ToString ()) == 2;
			variableMatrix[i,3] = PlayerPrefs.GetInt ("variable" + i.ToString ()) == 3;
		}
	}

}

