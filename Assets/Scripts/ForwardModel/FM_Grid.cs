using UnityEngine;

public class FM_Grid
{
    public FM_Wall[] walls;

    public FM_Grid(GameObject[] level)
    {
        walls = new FM_Wall[level.Length];
        SetupGridWalls(level);           
    }

    //Setup the grid walls (internal and external) from the Unity scene.
    private void SetupGridWalls(GameObject[] level)
    {
        for (int i = 0; i < walls.Length; i++)
        {
            walls[i] = ExtractWallCoordinates(level[i]);
        }
    }

    //Get (Unity's) wall's position and size (latter based on scale) 
    private FM_Wall ExtractWallCoordinates(GameObject obj)
    {
        Vector3 pos = obj.transform.position;
        Vector2 size = new Vector2(obj.transform.localScale.x / 2, obj.transform.localScale.y / 2);

        return new FM_Wall(pos, size);
    }
}
