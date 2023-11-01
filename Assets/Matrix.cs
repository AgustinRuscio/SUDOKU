using System.Collections;
using System.Collections.Generic;


public class Matrix<T> : IEnumerable<T>
{
    //IMPLEMENTAR: ESTRUCTURA INTERNA- DONDE GUARDO LOS DATOS?
    private T[] _data;
    
    private int _length;
    private int _height;
    private int _width;

    public Matrix(int width, int height)
    {
        //IMPLEMENTAR: constructor
        _width = width;
        _height = height;
        _length = width * height;

        _data = new T[_length];
    }

	public Matrix(T[,] copyFrom) //Lo modifico y le agrego que me pase cokumnas y filas
    {
        //IMPLEMENTAR: crea una version de Matrix a partir de una matriz básica de C#

        _width = copyFrom.GetLength(1);
        _height = copyFrom.GetLength(0);
        _length = _width * _height;

        _data = new T[_length];

        for (int i = 0; i < _height; i++)
        {
	        for (int j = 0; j < _width; j++)
	        {
		        _data[i * _width + j] = copyFrom[i, j];
	        }
        }
    }

	public Matrix<T> Clone() {
        Matrix<T> aux = new Matrix<T>(Width, Height);
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

        for (int i = x0; i <= x1; i++)
        {
            for (int j = y0; j <= y1; j++)
            {
                _data[i * _width + j] = item;
            }
        }
    }

    //Todos los parametros son INCLUYENTES
    public List<T> GetRange(int x0, int y0, int x1, int y1)
    {
        List<T> l = new List<T>();
        //Recorrer desde x0 a x1 y de y0 a y1 siendo esto lo wue pasen por psrsmetro

        //IMPLEMENTAR

        for (int i = x0; i <= x1; i++)
        {
            for (int j = y0; j <= y1; j++)
            {
                l.Add(_data[i * _width + j]);
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
            if (x >= _width || x < 0 || y >= _height || y < 0)
				throw new System.Exception("Index out");

            return _data[x + _height * y];
            //return default(T);
		}
		set 
		{
			if (x >= _width || x < 0 || y >= _height || y < 0)
				throw new System.Exception("Index out");

			_data[x + _height * y] = value;
            //IMPLEMENTAR
		}
	}

    public int Width
    {
	    get { return _width;}
	    private set { }
    }

    public int Height 
    { 
	    get { return _height;}
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
