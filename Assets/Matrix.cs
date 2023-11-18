using System.Collections;
using System.Collections.Generic;


public class Matrix<T> : IEnumerable<T>
{
    //IMPLEMENTAR: ESTRUCTURA INTERNA- DONDE GUARDO LOS DATOS?
    private T[] _data;
    
    private int _length;
    private int _widthX;
    private int _heightY;

    public Matrix(int widthX, int heightY)
    {
        //IMPLEMENTAR: constructor
        _widthX = widthX;
        _heightY = heightY;
        _length = widthX * heightY;

        _data = new T[_length];
    }

	public Matrix(T[,] copyFrom) //Lo modifico y le agrego que me pase cokumnas y filas
    {
        //IMPLEMENTAR: crea una version de Matrix a partir de una matriz básica de C#

        _widthX = copyFrom.GetLength(0);
        _heightY = copyFrom.GetLength(1);
        _length = _widthX * _heightY;

        _data = new T[_length];

        for (int i = 0; i < _heightY; i++)
			for (int j = 0; j < _widthX; j++)
		        this[j,i] = copyFrom[j, i];
    }

	public Matrix<T> Clone() 
	{
        Matrix<T> aux = new Matrix<T>(_widthX, _heightY);
        //IMPLEMENTAR

        for (int i = 0; i < _data.Length; i++)
        {
	        aux._data[i] = _data[i];
        }

        return aux;
    }

	public void SetRangeTo(int x0, int y0, int x1, int y1, T item) 
	{
        //Le iguala a los valores que recorre 
        //IMPLEMENTAR: iguala todo el rango pasado por parámetro a item

        for (int i = y0; i < y1; i++)
        {
            for (int j = x0; j < x1; j++)
            {
                this[j,i] = item;
            }
        }
    }

    //Todos los parametros son INCLUYENTES
    public List<T> GetRange(int x0, int y0, int x1, int y1)
    {
        List<T> l = new List<T>();
        //Recorrer desde x0 a x1 y de y0 a y1 siendo esto lo wue pasen por psrsmetro

        //IMPLEMENTAR

        for (int i = y0; i < y1; i++)
        {
            for (int j = x0; j < x1; j++)
            {
                l.Add(this[j,i]);
            }
        }

        return l;
	}

    //Para poder igualar valores en la matrix a algo
    public T this[int x, int y]
    {
		get
        {
            //IMPLEMENTAR
            //if (x >= _widthX || x < 0 || y >= _heightY || y < 0)
			//	throw new System.Exception("Index out");

            return _data[x + _widthX * y];
            //return default(T);
		}
		set 
		{
			//if (x >= _widthX || x < 0 || y >= _heightY || y < 0)
			//	throw new System.Exception("Index out");

			_data[x + _widthX * y] = value;
            //IMPLEMENTAR
		}
	}

    public int WidthX
    {
	    get { return _widthX;}
	    private set { }
    }

    public int HeightY 
    { 
	    get { return _heightY;}
	    private set { } 
    }

    public int Capacity
    {
	    get { return _length;}
	    private set { }
    }

    public IEnumerator<T> GetEnumerator()
    {
	    int count = 0;

	    while (count < _length)
	    {
		    yield return _data[count];
		    count++;
	    }
	    
        //IMPLEMENTAR
        //yield return default(T);
    }

	IEnumerator IEnumerable.GetEnumerator() 
	{
		return GetEnumerator();
	}
}
