using UnityEngine;
using System.Collections.Generic;

public class RoomGenerator : MonoBehaviour
{
	public enum Direction
	{
		up = 0,
		down = 1,
		left = 2,
		right = 3,
	}

	public Direction direction;
	public GameObject rootPrefab;
	public int roomNumber;
	public Color startColor;
	public Color endColor;
	public GameObject endRoom;
	public Transform generatorPoint;
	public float xOffset;
	public float yOffset;
	public LayerMask roomLayer;
	public List<Room> rooms;
	public int maxStep;
	public WallType wallType;
}
