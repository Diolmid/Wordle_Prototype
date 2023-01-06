using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Row : MonoBehaviour
{
    public string Word { get { return string.Join("", Tiles.Select(t=> t.Letter.ToString())); } }

    public Tile[] Tiles { get; private set; }

    private void Awake()
    {
        Tiles = GetComponentsInChildren<Tile>();
    }
}
