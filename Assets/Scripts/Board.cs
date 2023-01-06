using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    private static readonly KeyCode[] SUPPORTED_KEYS = new KeyCode[]
    {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F,
        KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R,
        KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X,
        KeyCode.Y, KeyCode.Z
    };

    private int _rowIndex;
    private int _tileIndex;

    private string _currentGuessWord;
    private string[] _solutionsWords;
    private string[] _validWords;

    [SerializeField]
    private Tile.State _emptyState;
    [SerializeField]
    private Tile.State _occupiedState;
    [SerializeField]
    private Tile.State _correctState;
    [SerializeField]
    private Tile.State _wrongSpotState;
    [SerializeField]
    private Tile.State _incorrectState;

    private Row[] _rows;

    [SerializeField]
    private TextMeshProUGUI _invalidWordText;
    [SerializeField]
    private Button _newWordButton;
    [SerializeField]
    private Button _tryAgainButton;

    private void Awake()
    {
        _rows = GetComponentsInChildren<Row>();
    }

    private void Start()
    {
        LoadData();
        SetRandomWord();
    }

    private void Update()
    {
        var currentRow = _rows[_rowIndex];

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            _tileIndex = Mathf.Max(_tileIndex - 1, 0);
            currentRow.Tiles[_tileIndex].SetLetter('\0');
            currentRow.Tiles[_tileIndex].SetState(_emptyState);

            _invalidWordText.gameObject.SetActive(false);
        }
        else if(_tileIndex >= _rows[_rowIndex].Tiles.Length)
        {
            if (Input.GetKeyDown(KeyCode.Return)) 
                SubmitRow(currentRow);
        }
        else
        {
            for (int i = 0; i < SUPPORTED_KEYS.Length; i++)
            {
                if (Input.GetKeyDown(SUPPORTED_KEYS[i]))
                {
                    currentRow.Tiles[_tileIndex].SetLetter((char)SUPPORTED_KEYS[i]);
                    currentRow.Tiles[_tileIndex].SetState(_occupiedState);
                    _tileIndex++;
                    break;
                }
            }
        }
    }

    public void TryAgain()
    {
        ClearBoard();

        enabled = true;
    }

    public void NewGame()
    {
        ClearBoard();
        SetRandomWord();

        enabled = true;
    }

    private void ClearBoard()
    {
        for(int row = 0; row < _rows.Length; row++)
        {
            for (int col = 0; col < _rows[row].Tiles.Length; col++)
            {
                _rows[row].Tiles[col].SetLetter('\0');
                _rows[row].Tiles[col].SetState(_emptyState);
            }
        }

        _rowIndex = 0;
        _tileIndex = 0;
    }

    private void LoadData()
    {
        var textFile = Resources.Load("official_wordle_all") as TextAsset;
        _validWords = textFile.text.Split("\n");

        textFile = Resources.Load("official_wordle_common") as TextAsset;
        _solutionsWords = textFile.text.Split("\n");
    }

    private void SetRandomWord()
    {
        _currentGuessWord = _solutionsWords[Random.Range(0, _solutionsWords.Length)];
        _currentGuessWord = _currentGuessWord.ToLower().Trim();
    }

    private void SubmitRow(Row row)
    {
        if (!CheckIfWordIsValid(row.Word))
        {
            _invalidWordText.gameObject.SetActive(true);
            return;
        }

        var remaining = _currentGuessWord;

        for (int i = 0; i < row.Tiles.Length; i++)
        {
            var tile = row.Tiles[i];

            if (tile.Letter == _currentGuessWord[i])
            {
                tile.SetState(_correctState);

                remaining = remaining.Remove(i, 1).Insert(i, " ");
            }
            else if (!_currentGuessWord.Contains(tile.Letter))
            {
                tile.SetState(_incorrectState);
            }
        }

        for (int i = 0; i < row.Tiles.Length; i++)
        {
            var tile = row.Tiles[i];

            if(tile.CurrentState != _correctState && tile.CurrentState != _incorrectState)
            {
                if (remaining.Contains(tile.Letter))
                {
                    tile.SetState(_wrongSpotState);

                    var correctIndex = remaining.IndexOf(tile.Letter);
                    remaining = remaining.Remove(correctIndex, 1).Insert(correctIndex, " ");
                }
                else
                {
                    tile.SetState(_incorrectState);
                }
            }
        }

        if (HasWon(row)) enabled = false;

        _rowIndex++;
        _tileIndex = 0;

        if(_rowIndex >= _rows.Length) enabled = false;
    }

    private bool CheckIfWordIsValid(string word)
    {
        return _validWords.Any(w => w == word);
    }

    private bool HasWon(Row row)
    {
        for (int i = 0; i < row.Tiles.Length; i++)
        {
            if (row.Tiles[i].CurrentState != _correctState)
            {
                return false;
            }
        }

        return true;
    }

    private void OnEnable()
    {
        _tryAgainButton.gameObject.SetActive(false);
        _newWordButton.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        _tryAgainButton.gameObject.SetActive(true);
        _newWordButton.gameObject.SetActive(true);
    }
}
