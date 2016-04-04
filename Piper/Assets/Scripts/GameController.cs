using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public GameObject playerRefObject;
	public GameObject playerGroundRefObject;
	public GameObject playerObject;
	private int playerSection;
	private float playerPosWithinSection;
	private float playerAngle;
	private float playerHeight;
	private float playerJumpForce;
	private bool isGrounded;

	public float speed;
	public float turnSpeed;
	public float jump;

	public Material materialRing;
	public Material materialLink;

	//Do not update these during play
	public int initialSections;
	public int sectionsPerCircle;
	public float circleRadius;
	public int segmentsPerRing;
	public float ringRadius;
	//Do not update these during play

	private GameObject terrain;
	private List<GameObject> sections = new List<GameObject>();
	private List<GameObject> ringCenters = new List<GameObject> ();
	private List<List<GameObject>> ringSegmentMap = new List<List<GameObject>>();
	private List<List<GameObject>> linkSegmentMap = new List<List<GameObject>>();

	private float angleStep;
	private float prevAngle;

	void Start () {

		terrain = new GameObject ();
		terrain.name = "Terrain";

		angleStep = Mathf.PI * 2 / sectionsPerCircle;
		prevAngle = - angleStep;

		InitialGenerate ();

		playerAngle = Mathf.Deg2Rad * -90.0f;
		playerSection = 1;
		playerPosWithinSection = 0;
		isGrounded = true;
		playerJumpForce = 0;

	}

	void Update () {

		MovePlayerRef ();
		MovePlayerGroundRef ();
		MovePlayer ();

	}

	void MovePlayerRef(){
		playerPosWithinSection += speed/1000;
		if (playerPosWithinSection > 1) {
			playerPosWithinSection--;
			playerSection++;
			Generate ();
		}
		playerRefObject.transform.position = Vector3.Lerp(ringCenters[playerSection-1].transform.position, ringCenters[playerSection].transform.position, playerPosWithinSection);
		Vector3 fromAngle = ringCenters [playerSection - 1].transform.eulerAngles;
		Vector3 toAngle = ringCenters [playerSection].transform.eulerAngles;
		if (fromAngle.x > 270 && toAngle.x < 90) {
			fromAngle.x -= 360;
		}
		if (toAngle.x > 270 && fromAngle.x < 90) {
			toAngle.x -= 360;
		}
		if (fromAngle.y > 270 && toAngle.y < 90) {
			fromAngle.y -= 360;
		}
		if (toAngle.y > 270 && fromAngle.y < 90) {
			toAngle.y -= 360;
		}
		if (fromAngle.z > 270 && toAngle.z < 90) {
			fromAngle.z -= 360;
		}
		if (toAngle.z > 270 && fromAngle.z < 90) {
			toAngle.z -= 360;
		}
		playerRefObject.transform.eulerAngles = Vector3.Lerp(fromAngle, toAngle, playerPosWithinSection);
	}

	void MovePlayerGroundRef(){
		float xAxis = Input.GetAxis ("Horizontal");
		if (!isGrounded) {
			xAxis = 0;
		}
		playerAngle += turnSpeed/100 * xAxis;
		playerGroundRefObject.transform.localPosition = new Vector3 (Mathf.Cos(playerAngle) * ringRadius, Mathf.Sin(playerAngle) * ringRadius, 0);
		playerGroundRefObject.transform.localEulerAngles = new Vector3 (0, 0, Mathf.Rad2Deg * playerAngle + 90);
	}

	void MovePlayer(){
		if (Input.GetButtonDown ("Jump")) {
			if (isGrounded) {
				playerJumpForce = jump/100;
				isGrounded = false;
			}
		}
		if (!isGrounded) {
			playerJumpForce -= 0.01f;
			playerHeight += playerJumpForce;
			if (playerHeight < 0) {
				playerHeight = 0;
				isGrounded = true;
			}
			playerObject.transform.localPosition = new Vector3 (0, playerHeight, 0);
		}
	}

	void InitialGenerate(){

		for (int i = 0; i < initialSections + 1; i++) {
			NextSection (i);
		}

		for (int i = 1; i < ringCenters.Count - 1; i++) {
			UpdateRingCenter (i);
		}

		for (int i=0;i<ringCenters.Count-1;i++){
			CreateRing (i);
		}

		for (int i=1;i<ringCenters.Count-1;i++){
			CreateLink (i);
		}

	}

	void Generate(){
		
		int i = ringCenters.Count;

		NextSection (i);
		UpdateSection (i-1);

	}

	void NextSection(int i){
		sections.Add(new GameObject());
		sections[i].name = "Section-"+i;
		sections[i].transform.SetParent(terrain.transform);

		NextRingCenter (i);
	}

	void NextRingCenter(int i){
		//ringCenters.Add(GameObject.CreatePrimitive (PrimitiveType.Sphere));
		ringCenters.Add(new GameObject());
		ringCenters [i].name = "Ring-" + i;
		ringCenters [i].transform.SetParent (sections [i].transform);

		float currentAngle = prevAngle + angleStep;
		prevAngle = currentAngle;
		Vector3 currentPosition = new Vector3 (Mathf.Cos (currentAngle) * circleRadius, 0, Mathf.Sin (currentAngle) * circleRadius);
		if (i != 0) {
			currentPosition.y = ringCenters[i-1].transform.position.y + 1;
		}
		ringCenters [i].transform.position = currentPosition;
	}

	void UpdateSection(int i){
		UpdateRingCenter (i);
		CreateRing (i);
		CreateLink (i);
	}

	void UpdateRingCenter(int i){
		Vector3 previousDirection = ringCenters [i].transform.position - ringCenters [i - 1].transform.position;
		Vector3 nextDirection = ringCenters [i + 1].transform.position - ringCenters [i].transform.position;

		ringCenters [i].transform.forward = Vector3.Lerp (previousDirection, nextDirection, 0.5f);
	}

	void CreateRing(int i){
		ringSegmentMap.Add (new List<GameObject> ());
		for (int index = 0; index < segmentsPerRing; index++) {

			//GameObject
			ringSegmentMap[i].Add(new GameObject ());
			ringSegmentMap[i] [index].transform.SetParent (ringCenters [i].transform);
			ringSegmentMap[i] [index].name = "RingSegment-"+index;

			ringSegmentMap[i] [index].AddComponent<MeshFilter> ();
			ringSegmentMap[i] [index].AddComponent<MeshRenderer> ();
			ringSegmentMap[i] [index].GetComponent<MeshRenderer> ().material = materialRing;

			Mesh mesh = ringSegmentMap[i] [index].GetComponent<MeshFilter> ().mesh;
			mesh.name = "Custom";
			mesh.vertices = new Vector3[] {
				new Vector3 (0.5f, 0.5f, 0),
				new Vector3 (0.5f, -0.5f, 0),
				new Vector3 (-0.5f, 0.5f, 0),
				new Vector3 (-0.5f, -0.5f, 0)
			};
			mesh.uv = new Vector2[] {
				new Vector2 (1, 1),
				new Vector2 (1, 0),
				new Vector2 (0, 1),
				new Vector2 (0, 0)
			};
			mesh.triangles = new int[] {
				0, 1, 2,
				2, 1, 3
			};

			//2D
			float angleFrom = index * Mathf.PI * 2 / segmentsPerRing;
			float angleTo = (index + 1) * Mathf.PI * 2 / segmentsPerRing;
			float angleCenter = (angleFrom + angleTo) / 2;

			Vector2 coordFrom = new Vector2 (Mathf.Cos (angleFrom) * ringRadius, Mathf.Sin (angleFrom) * ringRadius);
			Vector2 coordTo = new Vector2 (Mathf.Cos (angleTo) * ringRadius, Mathf.Sin (angleTo) * ringRadius);
			Vector2 coordCenter = Vector2.Lerp (coordFrom, coordTo, 0.5f);

			float segmentLength = Vector2.Distance (coordFrom, coordTo);

			//3D
			ringSegmentMap[i] [index].transform.localPosition = new Vector3(coordCenter.x, coordCenter.y, 0);
			ringSegmentMap[i] [index].transform.localScale = new Vector3 (1.0f, segmentLength, 1.0f);

			ringSegmentMap[i] [index].transform.localEulerAngles = new Vector3 (-Mathf.Rad2Deg * angleCenter, 90, 0);

		}
	}

	void CreateLink(int i){
		linkSegmentMap.Add (new List<GameObject> ());
		GameObject linkParent = new GameObject ();
		linkParent.name = "Link-" + i;
		linkParent.transform.SetParent (sections [i].transform);

		for (int index = 0; index < segmentsPerRing; index++) {

			//GameObject
			linkSegmentMap[i-1].Add(new GameObject ());
			linkSegmentMap[i-1] [index].transform.SetParent (linkParent.transform);
			linkSegmentMap[i-1] [index].name = "LinkSegment-"+index;

			linkSegmentMap[i-1] [index].AddComponent<MeshFilter> ();
			linkSegmentMap[i-1] [index].AddComponent<MeshRenderer> ();
			linkSegmentMap[i-1] [index].GetComponent<MeshRenderer> ().material = materialLink;

			float angleFrom = index * Mathf.PI * 2 / segmentsPerRing;
			float angleTo = (index + 1) * Mathf.PI * 2 / segmentsPerRing;

			Vector2 coordFrom = new Vector2 (Mathf.Cos (angleFrom) * ringRadius, Mathf.Sin (angleFrom) * ringRadius);
			Vector2 coordTo = new Vector2 (Mathf.Cos (angleTo) * ringRadius, Mathf.Sin (angleTo) * ringRadius);

			Vector3 globalFrom_previous = ringCenters [i-1].transform.position + coordFrom.x * ringCenters [i-1].transform.right + coordFrom.y * ringCenters [i-1].transform.up + 0.5f * ringCenters [i-1].transform.forward;
			Vector3 globalTo_previous = ringCenters [i-1].transform.position + coordTo.x * ringCenters [i-1].transform.right + coordTo.y * ringCenters [i-1].transform.up + 0.5f * ringCenters [i-1].transform.forward;

			Vector3 globalFrom_next = ringCenters [i].transform.position + coordFrom.x * ringCenters [i].transform.right + coordFrom.y * ringCenters [i].transform.up - 0.5f * ringCenters [i].transform.forward;
			Vector3 globalTo_next = ringCenters [i].transform.position + coordTo.x * ringCenters [i].transform.right + coordTo.y * ringCenters [i].transform.up - 0.5f * ringCenters [i].transform.forward;

			Mesh mesh = linkSegmentMap[i-1] [index].GetComponent<MeshFilter> ().mesh;
			mesh.name = "Link";
			mesh.vertices = new Vector3[] {
				globalTo_previous,
				globalFrom_previous,
				globalTo_next,
				globalFrom_next
			};
			mesh.uv = new Vector2[] {
				new Vector2 (1, 1),
				new Vector2 (1, 0),
				new Vector2 (0, 1),
				new Vector2 (0, 0)
			};
			mesh.triangles = new int[] {
				0, 1, 2,
				2, 1, 3
			};

		}
	}

}
