﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public bool SHOW_COLLIDER = true;

    public static LevelManager instance;    

    // Level Spawning
    private const float DISTANCE_BETWEEN_SPAWN = 100.0f;
    private const int INITIAL_SEGMENTS = 10;
    private const int INITIAL_TRANSITION_SEGMENTS = 2;
    private const int MAX_SEGMENTS_ON_SCREEN = 15;
    private Transform cameraContainer;
    private int amountOfActiveSegments;
    private int continiousSegments;
    private int currentSpawnZ;
    private int currentLevel;
    private int y1, y2, y3;

    // List of pieces
    public List<Piece> ramps = new List<Piece>();
    public List<Piece> longblocks = new List<Piece>();
    public List<Piece> jumps = new List<Piece>();
    public List<Piece> slides = new List<Piece>();
    [HideInInspector]
    public List<Piece> pieces = new List<Piece>(); // All the pieces in the pool

    //List of segments
    public List<Segment> availableSegments = new List<Segment>();
    public List<Segment> availableTransitions = new List<Segment>();
    [HideInInspector]
    public List<Segment> segments = new List<Segment>();

    //Game play
    private bool isMoving = false;

    private void Awake()
    {
        instance = this;
        cameraContainer = Camera.main.transform;
        currentSpawnZ = 0;
        currentLevel = 0;
    }

    private void Start()
    {
        for (int i = 0; i < INITIAL_SEGMENTS; i++)
        {
            if (i < INITIAL_TRANSITION_SEGMENTS)
                SpawnTransition();
            else
                GenerateSegment();
        }
    }

    private void Update()
    {
        if (currentSpawnZ - cameraContainer.position.z < DISTANCE_BETWEEN_SPAWN)
        {
            GenerateSegment();
        }

        if (amountOfActiveSegments >= MAX_SEGMENTS_ON_SCREEN)
        {
            segments[amountOfActiveSegments - 1].DeSpqwn();
            amountOfActiveSegments--;
        }
    }

    private void GenerateSegment()
    {
        SpawnSegment();

        if (Random.Range(0f, 1f) < (continiousSegments *0.25f))
        {
            // Spawn transition seg
            continiousSegments = 0;
            SpawnTransition();
        }
        else
        {
            continiousSegments++;
        }
    }

    private void SpawnSegment()
    {
        List<Segment> possibleSeg = availableSegments.FindAll(x => x.beginY1 == y1 ||
                                                                   x.beginY2 == y2 ||
                                                                   x.beginY3 == y3);
        int id = Random.Range(0, possibleSeg.Count);

        Segment segment = GetSegment(id, false);

        y1 = segment.endY1;
        y2 = segment.endY2;
        y3 = segment.endY3;

        segment.transform.SetParent(transform);
        segment.transform.localPosition = Vector3.forward * currentSpawnZ;

        currentSpawnZ += segment.lenght;
        amountOfActiveSegments++;
        segment.Spawn();
    }

    private void SpawnTransition()
    {
        List<Segment> possibleTransition = availableTransitions.FindAll(x => x.beginY1 == y1 ||
                                                                   x.beginY2 == y2 ||
                                                                   x.beginY3 == y3);
        int id = Random.Range(0, possibleTransition.Count);

        Segment segment = GetSegment(id, true);

        y1 = segment.endY1;
        y2 = segment.endY2;
        y3 = segment.endY3;

        segment.transform.SetParent(transform);
        segment.transform.localPosition = Vector3.forward * currentSpawnZ;

        currentSpawnZ += segment.lenght;
        amountOfActiveSegments++;
        segment.Spawn();
    }

    public Segment GetSegment(int id, bool transition)
    {
        Segment segment = null;
        segment = segments.Find(x => x.SegId == id &&
                                     x.transition == transition &&
                                     !x.gameObject.activeSelf);

        if (segment == null)
        {
            GameObject go = Instantiate((transition) ? availableTransitions[id].gameObject : availableSegments[id].gameObject) as GameObject;
            segment = go.GetComponent<Segment>();

            segment.SegId = id;
            segment.transition = transition;

            segments.Insert(0, segment);
        }
        else
        {
            segments.Remove(segment);
            segments.Insert(0, segment);
        }

        return segment;
    }

    public Piece GetPiece(PieceType type, int visualIndex)
    {
        Piece piece = pieces.Find(x => x.type == type && x.visuaIndex == visualIndex && !x.gameObject.activeSelf);

        if (piece == null)
        {
            GameObject go = null;
            if (type == PieceType.ramp)
                go = ramps[visualIndex].gameObject;
            else if (type == PieceType.longblock)
                go = longblocks[visualIndex].gameObject;
            else if (type == PieceType.jump)
                go = jumps[visualIndex].gameObject;
            else if (type == PieceType.slide)
                go = slides[visualIndex].gameObject;

            go = Instantiate(go);
            piece = go.GetComponent<Piece>();
            pieces.Add(piece);
        }

        return piece;
    }
}
