using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour {

    public PieceType type;
    private Piece currentPiece;

    public void Spawn()
    {
        int typeCount = 0;

        if (type == PieceType.jump)
            typeCount = LevelManager.instance.jumps.Count;
        else if (type == PieceType.slide)
            typeCount = LevelManager.instance.slides.Count;
        else if (type == PieceType.longblock)
            typeCount = LevelManager.instance.longblocks.Count;
        else if (type == PieceType.ramp)
            typeCount = LevelManager.instance.ramps.Count;

        currentPiece = LevelManager.instance.GetPiece(type, Random.Range(0, typeCount));
        currentPiece.gameObject.SetActive(true);
        currentPiece.transform.SetParent(transform, false);
    }

    public void Despawn()
    {
        currentPiece.gameObject.SetActive(false);
    }

}
