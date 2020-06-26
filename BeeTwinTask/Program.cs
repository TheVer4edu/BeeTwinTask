using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeeTwinTask
{
    /*
        -new *number of rows* *number of columns* *index* (создает новую матрицу (просит пользователя ввести числа по строкам) и сохраняет ее по индексу index)
        -add *index of first matrix* *index of second matrix* *index*(складывает матрицы)
        -sub *index of first matrix* *index of second matrix* *index*(вычитает матрицы)
        -mp *index of matrix* *number (double)* *index*(умножает матрицу на число)
        -t *index of matrix* *index*(транспoнирует матрицу)
            *index* в конце обозначает, что полученная матрица будет сохранена в *индекс*
        -show *index of matrix* (выводит в консоль матрицу по индексу)
     */
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            UserInterface.Launch();
        }
    }

    class Matrix
    {
        public double[,] Values { get; private set; }
        private readonly int _width, _height;
        private int _filledRows;
        public int GetWidth { get => _width; }
        public int GetHeight { get => _height; }

        public Matrix(int width, int height)
        {
            _width = width;
            _height = height;
            Values = new double[_height,_width];
        }

        public void Fill(double[,] values)
        {
            Values = values;
        }

        public void AddRow(double[] values)
        {
            for (int i = 0; i < values.Length; i++)
                Values[_filledRows, i] = values[i];
            _filledRows++;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            for(int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                    result.Append(Values[i, j] + " ");
                result.Append("\n");
            }
            return result.ToString();
        }
    }

    static class MatrixOperations
    {
        public static Matrix Add(this Matrix first, Matrix second)
        {
            var result = new Matrix(first.GetWidth, first.GetHeight);
            for (int row = 0; row < result.GetHeight; row++)
                for (int col = 0; col < result.GetWidth; col++)
                    result.Values[row, col] = first.Values[row, col] + second.Values[row, col];
            return result;
        }
        
        public static Matrix Sub(this Matrix matrix, Matrix second)
        {
            var result = new Matrix(matrix.GetWidth, matrix.GetHeight);
            for (int row = 0; row < result.GetHeight; row++)
                for (int col = 0; col < result.GetWidth; col++)
                    result.Values[row, col] = matrix.Values[row, col] - second.Values[row, col];
            return result;
        }
        
        public static Matrix Mul(this Matrix matrix, double digit)
        {
            var result = new Matrix(matrix.GetWidth, matrix.GetHeight);
            for (int row = 0; row < result.GetHeight; row++)
                for (int col = 0; col < result.GetWidth; col++)
                    result.Values[row, col] = matrix.Values[row, col] * digit;
            return result;
        }

        public static Matrix Transpose(this Matrix matrix)
        {
            var result = new Matrix(matrix.GetHeight, matrix.GetWidth);
            for (int row = 0; row < result.GetHeight; row++)
                for (int col = 0; col < result.GetWidth; col++)
                    result.Values[row, col] = matrix.Values[col, row];
            return result;
        }
    }

    static class MatrixEnvironment
    {
        public static Dictionary<int, Matrix> Matrices { get; set; } = new Dictionary<int, Matrix>();
    }

    static class UserInterface
    {
        public static void Launch()
        {
            string line;
            while ((line = Console.ReadLine()) != null)
            {
                string[] command = line.Split(' ');
                string operation = command[0];
                switch (operation)
                {
                    case "-new":
                        new NewMatrixCommand().Execute(int.Parse(command[1]), int.Parse(command[2]), int.Parse(command[3]));
                        break;
                    case "-add":
                        new AddMatrixCommand().Execute(int.Parse(command[1]), int.Parse(command[2]), int.Parse(command[3]));
                        break;
                    case "-sub":
                        new SubMatrixCommand().Execute(int.Parse(command[1]), int.Parse(command[2]), int.Parse(command[3]));
                        break;
                    case "-mul":
                        new MulMatrixCommand().Execute(int.Parse(command[1]), double.Parse(command[2]), int.Parse(command[3]));
                        break;
                    case "-t":
                        new TransposeMatrixCommand().Execute(int.Parse(command[1]), int.Parse(command[2]));
                        break;
                    case "-show":
                        new ShowMatrixCommand().Execute(int.Parse(command[1]));
                        break;
                }
            }
        }
    }

    interface IConsoleCommand<T1>
    {
        void Execute(T1 argument);
    }
    
    interface IConsoleCommand<T1, T2>
    {
        void Execute(T1 first, T2 second);
    }
    
    interface IConsoleCommand<T1, T2, T3>
    {
        void Execute(T1 first, T2 second, T3 third);
    }

    public class NewMatrixCommand : IConsoleCommand<int, int, int>
    {
        public void Execute(int rows, int cols, int index)
        {
            string line;
            MatrixEnvironment.Matrices[index] = new Matrix(rows, cols);
            for (int i = 0; i < rows; i++)
            {
                line = Console.ReadLine();
                double[] digits = line.Split(' ').Select(double.Parse).ToArray();
                MatrixEnvironment.Matrices[index].AddRow(digits);
            }
        }
    }
    public class AddMatrixCommand : IConsoleCommand<int, int, int>
    {
        public void Execute(int first, int second, int newIndex)
        {
            MatrixEnvironment.Matrices[newIndex] 
                = MatrixEnvironment.Matrices[first].Add(MatrixEnvironment.Matrices[second]);

        }
    }
    public class SubMatrixCommand : IConsoleCommand<int, int, int>
    {
        public void Execute(int first, int second, int newIndex)
        {
            MatrixEnvironment.Matrices[newIndex] 
                = MatrixEnvironment.Matrices[first].Sub(MatrixEnvironment.Matrices[second]);
        }
    }
    public class MulMatrixCommand : IConsoleCommand<int, double, int>
    {
        public void Execute(int matrixIndex, double digit, int newIndex)
        {
            MatrixEnvironment.Matrices[newIndex] = MatrixEnvironment.Matrices[matrixIndex].Mul(digit);
        }
    }
    public class TransposeMatrixCommand : IConsoleCommand<int, int>
    {
        public void Execute(int matrixIndex, int newIndex)
        {
            MatrixEnvironment.Matrices[newIndex] = MatrixEnvironment.Matrices[matrixIndex].Transpose();
        }
    }
    public class ShowMatrixCommand : IConsoleCommand<int>
    {
        public void Execute(int matrixIndex)
        {
            Matrix data = MatrixEnvironment.Matrices[matrixIndex];
            Console.Write(data);
        }
    }
}