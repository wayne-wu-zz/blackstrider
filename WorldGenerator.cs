using UnityEngine;
using System.Collections;
using System.Threading;

public class WorldGenerator : MonoBehaviour {

    //Generate Settings
    public int maxGroundHeight = 6; 
    public int minGroundHeight = 2;
    public int minLengthPerHeight = 2;
    public int maxLengthPerHeight = 6;
    public int maxStep = 2;

    //Terrains 
    public GameObject firstTerrain;
    public GameObject middleTerrain;
    public GameObject lastTerrain;

    //Hero
    public Transform target;

    private const int TILESPACE = 1;

    private int startHeight;
    private int endHeight;

	// Use this for initialization
	void Start () {
        firstTerrain.GetComponent<TileInfo>().mapHeight = maxGroundHeight + TILESPACE;
        middleTerrain.GetComponent<TileInfo>().mapHeight = maxGroundHeight + TILESPACE;
        lastTerrain.GetComponent<TileInfo>().mapHeight = maxGroundHeight + TILESPACE;

    }

    // Update is called once per frame
    void Update()
    {

        if (target.position.x > lastTerrain.transform.position.x)
        {
            GameObject temp = firstTerrain;
            TileInfo map = temp.GetComponent<TileInfo>();
            Vector2 worldPos = new Vector2(lastTerrain.transform.position.x + map.mapWidth - TILESPACE, lastTerrain.transform.position.y);
            firstTerrain = middleTerrain;
            middleTerrain = lastTerrain;
            temp.transform.position = lastTerrain.transform.position + new Vector3(map.mapWidth, 0, 0);
            StartCoroutine(_Regenerate(temp));
            lastTerrain = temp;
        }
        if (target.position.x < middleTerrain.transform.position.x)
        {
            GameObject temp = lastTerrain;
            TileInfo map = temp.GetComponent<TileInfo>();
            Vector2 worldPos = new Vector2(middleTerrain.transform.position.x - TILESPACE, firstTerrain.transform.position.y);
            lastTerrain = middleTerrain;
            middleTerrain = firstTerrain;
            temp.transform.position = firstTerrain.transform.position + new Vector3(-1*map.mapWidth, 0, 0);
            StartCoroutine(_Regenerate(temp));
            firstTerrain = temp;
        }

    }

    IEnumerator _Regenerate(GameObject terrain, int startHeight = 0, int endHeight = 0)
    {
        TileInfo map = terrain.GetComponent<TileInfo>();
        yield return StartCoroutine(_Regenerate(map, terrain.transform, startHeight, endHeight));
        UpdateMesh(map);
    }

    IEnumerator _Regenerate(TileInfo map, Transform transform, int startHeight, int endHeight)
    {
        int index = 0;
        int end = map.mapWidth;
        int previousHeight = 1;

        //Connect heights
        if (startHeight != 0)
        {
            int length = Random.Range(minLengthPerHeight, maxLengthPerHeight+1);
            yield return StartCoroutine(Fill(map, transform, index, startHeight, length));
            index += length;
            previousHeight = startHeight;
        }

        //Connect heights
        if (endHeight != 0)
        {
            int length = Random.Range(minLengthPerHeight, maxLengthPerHeight+1);
            end -= length;
            yield return StartCoroutine(Fill(map, transform, end, endHeight, length));
        }

        while (index < end)
        {
            int step = Random.Range(-1*maxStep, maxStep+1); 
            int height = (withinRange(previousHeight+step))?previousHeight+step:previousHeight;
            print(height);
            //int height = new System.Random().Next(minGroundHeight, maxGroundHeight);
            int length = Random.Range(minLengthPerHeight, maxLengthPerHeight+1);

            yield return StartCoroutine(Fill(map, transform, index, height, length));

            previousHeight = height; 
            index += length;
        }
    }

    private int findHeight(TileInfo map, Vector2 worldPos)
    {
        int count = 0;
        int mapIndex = map.WorldPointToMapIndex(worldPos);
        while (map.tiles[mapIndex] != Tile.empty)
        {
            print(mapIndex);
            count++;
            mapIndex = map.WorldPointToMapIndex(worldPos + new Vector2(0, TILESPACE));
        }
        return count;
    }

    private bool withinRange(int i)
    {
        return minGroundHeight <= i && maxGroundHeight >= i;
    }


    IEnumerator Fill(TileInfo map, Transform transform, int start, float height, float length)
    {
        for (int i = start; i < start + length; i++)
        {
            float x = transform.position.x;
            float y = transform.position.y;

            //TODO: Might have a better way of clearing the tiles
           yield return StartCoroutine(RemoveAndAdd(map, i, x, y, height));
             
        }
    }


    IEnumerator RemoveAndAdd(TileInfo map, int i, float x, float y, float height)
    { 
        //TODO: Optimize the speed
        for (int k = 0; k < height; k++)
        {
            map.AddTile(new Vector2(x + i, y + k), 0);
            yield return null;
        }

        for (int j = (int)height; j < map.mapHeight; j++)
        {
            map.RemoveTile(new Vector2(x + i, y + j));
            yield return null;
        }
    }
   

    void UpdateMesh(TileInfo map)
    {
        map.UpdateColliders();
        map.UpdateVisualMesh(false);
    }


    //Use coroutine for regenerate

    private class Regenerate : ThreadJob
    {
        private GameObject terrain;
        private TileInfo map;
        private Transform transform;
        private int startHeight;
        private int endHeight;
        private int minGroundHeight;
        private int maxGroundHeight;
        private int minLengthPerHeight;
        private int maxLengthPerHeight;

        public Regenerate(GameObject terrain, int minGroundHeight, int maxGroundHeight, int minLengthPerHeight, int maxLengthPerHeight)
        {
            this.terrain = terrain;
            this.map = terrain.GetComponent<TileInfo>();
            this.transform = terrain.transform;
            this.minGroundHeight = minGroundHeight;
            this.maxGroundHeight = maxGroundHeight;
            this.minLengthPerHeight = minLengthPerHeight;
            this.maxLengthPerHeight = maxLengthPerHeight;
        }

        protected override void ThreadFunction()
        {
            _Regenerate(map, transform, startHeight, endHeight);
        }

        protected override void OnFinished()
        {
            UpdateMesh(map);
        }

        private void _Regenerate(TileInfo map, Transform transform, int startHeight, int endHeight)
        {
            int index = 0;
            while (index < map.mapWidth)
            {
                int height = new System.Random().Next(minGroundHeight, maxGroundHeight);
                int length = new System.Random().Next(minLengthPerHeight, maxLengthPerHeight);

                Fill(map, transform, index, height, length);

                index += length;
            }

        }

        private void Fill(TileInfo map, Transform transform, int start, float height, float length)
        {
            for (int i = start; i < start + length; i++)
            {
                //TODO: Might have a better way of clearing the tiles
                for (int j = 0; j < map.mapHeight; j++)
                    map.RemoveTile(new Vector2(transform.position.x + i, transform.position.y + j));

                for (int k = 0; k < height; k++)
                    map.AddTile(new Vector2(transform.position.x + i, transform.position.y + k), 0);

                //yield return null;
            }
        }

        void UpdateMesh(TileInfo map)
        {
            map.UpdateColliders();
            map.UpdateVisualMesh(false);
        }

    }


}
