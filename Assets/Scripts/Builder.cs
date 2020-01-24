using Zenject;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Builder : MonoBehaviour
{
	public bool player = false;
	public string characterName;
	public Color color;
	public Material material;
	public List<Builder> attackedCharacters = new List<Builder>();

	[Header("Area")]
	public int startAreaPoints = 45;
	public float startAreaRadius = 3f;
	public float minPointDistance = 0.1f;
	public CharacterArea area;
	public GameObject areaOutline;
	public List<Vector3> areaVertices = new List<Vector3>();
	public List<Vector3> newAreaVertices = new List<Vector3>();

	private MeshRenderer areaMeshRend;
	private MeshFilter areaFilter;
	private MeshRenderer areaOutlineMeshRend;
	private MeshFilter areaOutlineFilter;

	[Header("Movement")]
	public float speed = 2f;
	public float turnSpeed = 14f;
	public TrailRenderer trail;
	public GameObject trailCollidersHolder;
	public List<SphereCollider> trailColls = new List<SphereCollider>();

	public SignalBus _signalBus;

	[Inject]
	public void Construct(SignalBus signalBus)
	{
		_signalBus = signalBus;
	}

	private void Start()
	{
		trail = transform.Find("Trail").GetComponent<TrailRenderer>();
		trail.material.color = new Color(color.r, color.g, color.b, 0.65f);
		GetComponent<MeshRenderer>().material.color = new Color(color.r * 1.3f, color.g * 1.3f, color.b * 1.3f);

		InitializeCharacter();
	}

	public virtual void Update()
	{
		var transPos = transform.position;
		bool isOutside = !IsPointInPolygon(new Vector2(transPos.x, transPos.z), Vertices2D(areaVertices));
		int count = newAreaVertices.Count;

		if (isOutside)
		{
			if (count == 0 || !newAreaVertices.Contains(transPos) && (newAreaVertices[count - 1] - transPos).magnitude >= minPointDistance)
			{
				count++;
				newAreaVertices.Add(transPos);

				int trailCollsCount = trailColls.Count;
				float trailWidth = trail.startWidth;
				SphereCollider lastColl = trailCollsCount > 0 ? trailColls[trailCollsCount - 1] : null;
				if (!lastColl || (transPos - lastColl.center).magnitude > trailWidth)
				{
					SphereCollider trailCollider = trailCollidersHolder.AddComponent<SphereCollider>();
					trailCollider.center = transPos;
					trailCollider.radius = trailWidth / 2f;
					trailCollider.isTrigger = true;
					trailCollider.enabled = false;
					trailColls.Add(trailCollider);

					if (trailCollsCount > 1)
					{
						trailColls[trailCollsCount - 2].enabled = true;
					}
				}
			}

			if (!trail.emitting)
			{
				trail.Clear();
				trail.emitting = true;
			}
		}
		else if (count > 0)
		{
			DeformCharacterArea(this, newAreaVertices);

			foreach (var character in attackedCharacters)
			{
				List<Vector3> newCharacterAreaVertices = new List<Vector3>();
				foreach (var vertex in newAreaVertices)
				{
					if (IsPointInPolygon(new Vector2(vertex.x, vertex.z), Vertices2D(character.areaVertices)))
					{
						newCharacterAreaVertices.Add(vertex);
					}
				}

				DeformCharacterArea(this, newCharacterAreaVertices);
			}
			attackedCharacters.Clear();
			newAreaVertices.Clear();

			if (trail.emitting)
			{
				trail.Clear();
				trail.emitting = false;
			}
			foreach (var trailColl in trailColls)
			{
				Destroy(trailColl);
			}
			trailColls.Clear();
		}
	}
	private void InitializeCharacter()
	{
		area = new GameObject().AddComponent<CharacterArea>();
		area.name = characterName + "Area";
		area.builder = this;
		Transform areaTrans = area.transform;
		areaFilter = area.gameObject.AddComponent<MeshFilter>();
		areaMeshRend = area.gameObject.AddComponent<MeshRenderer>();
		areaMeshRend.material = material;
		areaMeshRend.material.color = color;

		areaOutline = new GameObject();
		areaOutline.name = characterName + "AreaOutline";
		Transform areaOutlineTrans = areaOutline.transform;
		areaOutlineTrans.position += new Vector3(0, -0.495f, -0.1f);
		areaOutlineTrans.SetParent(areaTrans);
		areaOutlineFilter = areaOutline.AddComponent<MeshFilter>();
		areaOutlineMeshRend = areaOutline.AddComponent<MeshRenderer>();
		areaOutlineMeshRend.material = material;
		areaOutlineMeshRend.material.color = new Color(color.r * .7f, color.g * .7f, color.b * .7f);

		float step = 360f / startAreaPoints;
		for (int i = 0; i < startAreaPoints; i++)
		{
			areaVertices.Add(transform.position + Quaternion.Euler(new Vector3(0, step * i, 0)) * Vector3.forward * startAreaRadius);
		}
		UpdateArea();

		trailCollidersHolder = new GameObject();
		trailCollidersHolder.transform.SetParent(areaTrans);
		trailCollidersHolder.name = characterName + "TrailCollidersHolder";
		trailCollidersHolder.layer = 8;
	}

	public void UpdateArea()
	{
		if (areaFilter)
		{
			Mesh areaMesh = GenerateMesh(areaVertices, characterName);
			areaFilter.mesh = areaMesh;
			areaOutlineFilter.mesh = areaMesh;
			area.coll.sharedMesh = areaMesh;
		}
	}

	private Mesh GenerateMesh(List<Vector3> vertices, string meshName)
	{
		Triangulator tr = new Triangulator(Vertices2D(vertices));
		int[] indices = tr.Triangulate();

		Mesh msh = new Mesh();
		msh.vertices = vertices.ToArray();
		msh.triangles = indices;
		msh.RecalculateNormals();
		msh.RecalculateBounds();
		msh.name = meshName + "Mesh";

		return msh;
	}

	private Vector2[] Vertices2D(List<Vector3> vertices)
	{
		List<Vector2> areaVertices2D = new List<Vector2>();
		foreach (Vector3 vertex in vertices)
		{
			areaVertices2D.Add(new Vector2(vertex.x, vertex.z));
		}

		return areaVertices2D.ToArray();
	}

	public int GetClosestAreaVertice(Vector3 fromPos)
	{
		int closest = -1;
		float closestDist = Mathf.Infinity;
		for (int i = 0; i < areaVertices.Count; i++)
		{
			float dist = (areaVertices[i] - fromPos).magnitude;
			if (dist < closestDist)
			{
				closest = i;
				closestDist = dist;
			}
		}

		return closest;
	}

	private void OnTriggerEnter(Collider other)
	{
		CharacterArea otherCharacter = other.GetComponent<CharacterArea>();
		if (otherCharacter && otherCharacter != area && !attackedCharacters.Contains(otherCharacter.builder))
		{
			attackedCharacters.Add(otherCharacter.builder);
		}

		if (other.gameObject.layer == 8)
		{
			otherCharacter = other.transform.parent.GetComponent<CharacterArea>();
			otherCharacter.builder.Die();
		}
	}

	public static void DeformCharacterArea(Builder builder, List<Vector3> newAreaVertices)
	{
		int newAreaVerticesCount = newAreaVertices.Count;
		if (newAreaVerticesCount > 0)
		{
			List<Vector3> areaVertices = builder.areaVertices;
			int startPoint = builder.GetClosestAreaVertice(newAreaVertices[0]);
			int endPoint = builder.GetClosestAreaVertice(newAreaVertices[newAreaVerticesCount - 1]);

			// CLOCKWISE AREA
			// Select redundant vertices
			List<Vector3> redundantVertices = new List<Vector3>();
			for (int i = startPoint; i != endPoint; i++)
			{
				if (i == areaVertices.Count)
				{
					if (endPoint == 0)
					{
						break;
					}

					i = 0;
				}
				redundantVertices.Add(areaVertices[i]);
			}
			redundantVertices.Add(areaVertices[endPoint]);

			// Add new vertices to clockwise temp area
			List<Vector3> tempAreaClockwise = new List<Vector3>(areaVertices);
			for (int i = 0; i < newAreaVerticesCount; i++)
			{
				tempAreaClockwise.Insert(i + startPoint, newAreaVertices[i]);
			}

			// Remove the redundat vertices & calculate clockwise area's size
			tempAreaClockwise = tempAreaClockwise.Except(redundantVertices).ToList();
			float clockwiseArea = Mathf.Abs(tempAreaClockwise.Take(tempAreaClockwise.Count - 1).Select((p, i) => (tempAreaClockwise[i + 1].x - p.x) * (tempAreaClockwise[i + 1].z + p.z)).Sum() / 2f);

			// COUNTERCLOCKWISE AREA
			// Select redundant vertices
			redundantVertices.Clear();
			for (int i = startPoint; i != endPoint; i--)
			{
				if (i == -1)
				{
					if (endPoint == areaVertices.Count - 1)
					{
						break;
					}

					i = areaVertices.Count - 1;
				}
				redundantVertices.Add(areaVertices[i]);
			}
			redundantVertices.Add(areaVertices[endPoint]);

			// Add new vertices to clockwise temp area
			List<Vector3> tempAreaCounterclockwise = new List<Vector3>(areaVertices);
			for (int i = 0; i < newAreaVerticesCount; i++)
			{
				tempAreaCounterclockwise.Insert(startPoint, newAreaVertices[i]);
			}

			// Remove the redundant vertices & calculate counterclockwise area's size
			tempAreaCounterclockwise = tempAreaCounterclockwise.Except(redundantVertices).ToList();
			float counterclockwiseArea = Mathf.Abs(tempAreaCounterclockwise.Take(tempAreaCounterclockwise.Count - 1).Select((p, i) => (tempAreaCounterclockwise[i + 1].x - p.x) * (tempAreaCounterclockwise[i + 1].z + p.z)).Sum() / 2f);

			// Find the area with greatest size
			builder.areaVertices = clockwiseArea > counterclockwiseArea ? tempAreaClockwise : tempAreaCounterclockwise;
		}

		builder.UpdateArea();
	}

	// https://codereview.stackexchange.com/questions/108857/point-inside-polygon-check
	public static bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
	{
		int polygonLength = polygon.Length, i = 0;
		bool inside = false;
		float pointX = point.x, pointY = point.y;
		float startX, startY, endX, endY;
		Vector2 endPoint = polygon[polygonLength - 1];
		endX = endPoint.x;
		endY = endPoint.y;
		while (i < polygonLength)
		{
			startX = endX; startY = endY;
			endPoint = polygon[i++];
			endX = endPoint.x; endY = endPoint.y;
			inside ^= (endY > pointY ^ startY > pointY) && ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
		}
		return inside;
	}
	public void Die()
	{
		if (player)
		{
			_signalBus.Fire<PlayerDeadSignal>();
		}
		Destroy(area.gameObject);
		Destroy(areaOutline);
		Destroy(gameObject);
	}
}
