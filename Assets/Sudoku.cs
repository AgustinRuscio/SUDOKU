using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class Sudoku : MonoBehaviour 
{
	public Cell prefabCell;
	public Canvas canvas;
	public Text feedback;
	public float stepDuration = 0.5f;
	[Range(1, 82)]public int difficulty = 40;

	Matrix<Cell> _board;
	Matrix<int> _createdMatrix;
    List<int> posibles = new List<int>();
	int _smallSide;
	int _bigSide;
    string memory = "";
    string canSolve = "";
    bool canPlayMusic = false;
    List<int> nums = new List<int>();



    float r = 1.0594f;
    float frequency = 440;
    float gain = 0.5f;
    float increment;
    float phase;
    float samplingF = 48000;

    //-------Variables propias
    private int _lastX = 0, _lastY = 0;

    [Header("Variables propias")] 
    
    [SerializeField]
    private bool _autoCreateSudoku = true;
    [SerializeField]
    private bool _sequence;
    
    [SerializeField]
    private int xSize = 3, ySize = 3;

    
    void Start()
    {
        long mem = System.GC.GetTotalMemory(true);
        feedback.text = string.Format("MEM: {0:f2}MB", mem / (1024f * 1024f));
        memory = feedback.text;
        _smallSide = xSize;
        _bigSide = _smallSide * ySize;
        frequency = frequency * Mathf.Pow(r, 2);
        CreateEmptyBoard();
        ClearBoard();
        
        if(_autoCreateSudoku)
			CreateNew();

        _lastX = 0; _lastY = 0;
    }

    void ClearBoard() {
		_createdMatrix = new Matrix<int>(_bigSide, _bigSide);
		foreach(var cell in _board) {
			cell.number = 0;
			cell.locked = cell.invalid = false;
		}
	}

	void CreateEmptyBoard() {
		float spacing = 68f;
		float startX = -spacing * 4f;
		float startY = spacing * 4f;

		_board = new Matrix<Cell>(_bigSide, _bigSide);
		for(int x = 0; x<_board.WidthX; x++) {
			for(int y = 0; y<_board.HeightY; y++) {
                var cell = _board[x, y] = Instantiate(prefabCell);
				cell.transform.SetParent(canvas.transform, false);
				cell.transform.localPosition = new Vector3(startX + x * spacing, startY - y * spacing, 0);
			}
		}
	}
	


	//IMPLEMENTAR
	int watchdog = 0;
	private int count = 0;
	bool RecuSolve(Matrix<int> matrixParent, int x, int y, int protectMaxDepth, List<Matrix<int>> solution)
    {
	    //Aplicar recuercion para ir resolviendo

	    watchdog--;
	    if (watchdog <= 0)
		    return false;
	    
	    Debug.Log($"Ni bien empiezo : X= {x} ; Y= {y}");
	    
	    if (_board[x, y].locked)
	    {
		    Debug.Log($"La Casilla x= {x} e y= {y} esta llena");
		    
		    x++;
		    if (x >= matrixParent.WidthX)
		    {
			    x = 0;
			    y++;

			    if (y >= matrixParent.HeightY)
			    {
					Debug.Log("La ultima esta llena");
				    return true;
			    }
			    
			    Debug.Log($"La Casilla x ahora vale {x}");
			    Debug.Log($"La Casilla y ahora vale {y}");
		    }
	
		    return RecuSolve(matrixParent, x, y, matrixParent.Capacity, solution);
	    }
	    
	    Debug.Log($"La casilla : X= {x} ; Y= {y} Está vacia");
	    Debug.Log($"{matrixParent[x,y]} Valor en casilla");
	    Debug.Log($"{matrixParent.Capacity} capacity total");

	    
	    for (int i = 1; i <= xSize*ySize; i++) //Cambiar el 9 a variable X*Y
	    {
		    Debug.Log($"Puebo el valor {i} En La casilla : X= {x} ; Y= {y}");
		   
		    if (CanPlaceValue(matrixParent,i,x,y))
		    {
			    matrixParent[x, y] = i;
			    
			    //Sequenciad
			    Matrix<int> p;
			    p = matrixParent.Clone();
			    
			    solution.Add(p);

			    //--
			    Debug.Log($"En la casilla : X= {x} ; Y= {y} va el valor : {i}");
			    _lastX = x + 1;
			    _lastY = y;
			    
			    if (_lastX >= matrixParent.WidthX)
			    {
				    _lastX = 0;
				    _lastY = y+1;
				
				    if (_lastY >= matrixParent.HeightY)
				    {
					    return true; //Aca se termina termina
				    }
			    }

			    if (RecuSolve(matrixParent, _lastX, _lastY, protectMaxDepth, solution)) //Recursar asi con un slo click sigue
			    {
				    //Si vuelve true, se completa
				    return true;
			    }
			    else
			    {
				    matrixParent[x, y] = 0;
			    }
		    }
	    }

	    Debug.Log($"No pude poner ningun valor");
	    return false;
    }
	
	
    void OnAudioFilterRead(float[] array, int channels)
    {
        if(canPlayMusic)
        {
            increment = frequency * Mathf.PI / samplingF;
            for (int i = 0; i < array.Length; i++)
            {
                phase = phase + increment;
                array[i] = (float)(gain * Mathf.Sin((float)phase));
            }
        }
        
    }
    void changeFreq(int num)
    {
        frequency = 440 + num * 80;
    }

	//IMPLEMENTAR - punto 3
	IEnumerator ShowSequence(List<Matrix<int>> seq)
	{
		int ca = 0;
		
		Debug.Log("Count: " + count);
		
        while (ca < seq.Count)
        {
	        yield return new WaitForSeconds(stepDuration);
	        TranslateAllValues(seq[ca]);
	        ca++;
	        count--;
			Debug.Log("Count iterando: " + count);
        }
    }

	void Update () {
		if(Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(1))
            SolvedSudoku();
        else if(Input.GetKeyDown(KeyCode.C) || Input.GetMouseButtonDown(0)) 
            CreateSudoku();	
	}

	//modificar lo necesario para que funcione.
    void SolvedSudoku()
    {
        StopAllCoroutines();
        nums = new List<int>();
        var solution = new List<Matrix<int>>();
        watchdog = 100000;
        var result = RecuSolve(_createdMatrix, 0,0,1, solution);//????
        long mem = System.GC.GetTotalMemory(true);
        memory = string.Format("MEM: {0:f2}MB", mem / (1024f * 1024f));
        canSolve = result ? " VALID" : " INVALID";
		
        if(_sequence)
			StartCoroutine(ShowSequence(solution));
        else
			TranslateAllValues(_createdMatrix);

        //???
    }

    void CreateSudoku()
    {
	    StopAllCoroutines();
	    nums = new List<int>();
	    canPlayMusic = false;
	    ClearBoard();
	    List<Matrix<int>> l = new List<Matrix<int>>();
	    watchdog = 100000;
	    GenerateValidLine(_createdMatrix, 0, 0);
	    var result = RecuSolve(_createdMatrix, 0,1, _createdMatrix.Capacity,l);
	    _createdMatrix = l[l.Count-1].Clone();
	    LockRandomCells();
	    ClearUnlocked(_createdMatrix);
	    TranslateAllValues(_createdMatrix);
	    long mem = System.GC.GetTotalMemory(true);
	    memory = string.Format("MEM: {0:f2}MB", mem / (1024f * 1024f));
	    canSolve = result ? " VALID" : " INVALID";
	    feedback.text = "Pasos: " + l.Count + "/" + l.Count + " - " + memory + " - " + canSolve;
	    
	    
	    
	    
	    /*
	    //difficulty = 82;	
        StopAllCoroutines();
        nums = new List<int>();
        canPlayMusic = false;
        ClearBoard();
        List<Matrix<int>> l = new List<Matrix<int>>();
        watchdog = 100000;
        
        GenerateValidLine(_createdMatrix, 0, 0);
        //int random = Random.Range(1, Tests.validBoards.Length-1);
        //
        //_createdMatrix = new Matrix<int>(Tests.validBoards[random]);
        var result = RecuSolve(_createdMatrix, _lastX,1, 1 , l);
       
        _createdMatrix = l[l.Count-1].Clone();
 
        //
        //difficulty = Random.Range(1, 82);
        LockRandomCells();
        ClearUnlocked(_createdMatrix);
        TranslateAllValues(_createdMatrix);
        
        
        long mem = System.GC.GetTotalMemory(true);
        memory = string.Format("MEM: {0:f2}MB", mem / (1024f * 1024f));
        canSolve = result ? " VALID" : " INVALID";
        feedback.text = "Pasos: " + l.Count + "/" + l.Count + " - " + memory + " - " + canSolve;*/
    }
	void GenerateValidLine(Matrix<int> mtx, int x, int y)
	{
		int[]aux = new int[xSize*ySize];
		for (int i = 0; i < xSize*ySize; i++) 
		{
			aux [i] = i + 1;
		}
		int numAux = 0;
		for (int j = 0; j < aux.Length; j++) 
		{
			int r = 1 + Random.Range(j,aux.Length);
			numAux = aux [r-1];
			aux [r-1] = aux [j];
			aux [j] = numAux;
		}
		for (int k = 0; k < aux.Length; k++) 
		{
			mtx [k, 0] = aux [k];
		}
	}


	void ClearUnlocked(Matrix<int> mtx)
	{
		for (int i = 0; i < _board.HeightY; i++) 
		{
			for (int j = 0; j < _board.WidthX; j++) 
			{
				if (!_board [j, i].locked)
					mtx[j,i] = Cell.EMPTY;
			}
		}
	}

	void LockRandomCells()
	{
		List<Vector2> posibles = new List<Vector2> ();
		for (int i = 0; i < _board.HeightY; i++)
		{
			for (int j = 0; j < _board.WidthX; j++)
			{
				if (!_board [j, i].locked)
					posibles.Add (new Vector2(j,i));
			}
		}
		for (int k = 0; k < 82-difficulty; k++) 
		{
			int r = Random.Range (0, posibles.Count);
			_board [(int)posibles [r].x, (int)posibles [r].y].locked = true;
			posibles.RemoveAt (r);
		}
	}

    void TranslateAllValues(Matrix<int> matrix)
    {
        for (int y = 0; y < _board.HeightY; y++) 
			for (int x = 0; x < _board.WidthX; x++)
				_board[x, y].number = matrix[x, y];
    }

    void TranslateSpecific(int value, int x, int y)
    {
        _board[x, y].number = value;
    }

    void TranslateRange(int x0, int y0, int xf, int yf)
    {
        for (int x = x0; x < xf; x++)
			for (int y = y0; y < yf; y++)
				_board[x, y].number = _createdMatrix[x, y];
    }
    
    void CreateNew()
    {
        _createdMatrix = new Matrix<int>(Tests.validBoards[Tests.validBoards.Length-1]);
        LockRandomCells();
        ClearUnlocked(_createdMatrix);
        
        TranslateAllValues(_createdMatrix);
    }

    bool CanPlaceValue(Matrix<int> mtx, int value, int x, int y)
    {
        List<int> row = new List<int>();
        List<int> column = new List<int>();
        List<int> region = new List<int>();
        List<int> total = new List<int>();

        for (int i = 0; i < mtx.WidthX; i++)
        {
            if (i != x)
                row.Add(mtx[i, y]);
        }

        for (int j = 0; j < mtx.HeightY; j++)
        {
            if (j != y)
                column.Add(mtx[x, j]);
        }

        int regionStartX = (x / xSize) * xSize;
        int regionStartY = (y / ySize) * ySize;
        for (int i = regionStartX; i < regionStartX + xSize; i++)
        {
            for (int j = regionStartY; j < regionStartY + ySize; j++)
            {
                if (i != x || j != y)
                    region.Add(mtx[i, j]);
            }
        }

        total.AddRange(row);
        total.AddRange(column);
        total.AddRange(region);
        total = FilterZeros(total);

        return !total.Contains(value);
    }


    List<int> FilterZeros(List<int> list)
    {
        List<int> aux = new List<int>();
        for (int i = 0; i < list.Count; i++)
	        if (list[i] != 0) aux.Add(list[i]);
        
        
        return aux;
    }
}