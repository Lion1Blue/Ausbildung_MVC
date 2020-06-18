﻿using System;
using System.Drawing;
using System.Windows.Forms;
using WinFormMVC.Model;

namespace WinFormMVC.Controller
{
    public class Controller : IController
    {
        IUsersView _view;

        public Controller(IUsersView view)
        {
            _view = view;
            view.SetController(this);
            sortingAlgorithm = new SortingAlgorithm(this);
            function = new Function(this);
            PolynomialFunctionMakeGraph();
        }

        #region Fractional Arithmetic

        //Fractional Arithmetic
        public enum FractionalArithmeticEnum { Addition, Subtraction, Division, Multiplication };
        FractionCalculation calculate = new FractionCalculation();
        Fraction fractionA = new Fraction();
        Fraction fractionB = new Fraction();
        Fraction fractionResult = new Fraction();

        private void FractionalArithmetic_UpdateViewWithFractionValues()
        {
            _view.FractionA = fractionA;
            _view.FractionB = fractionB;
            _view.FractionResult = fractionResult;
        }

        private void FractionalArithmetic_UpdateFractionWithViewValues()
        {
            fractionA = _view.FractionA;
            fractionB = _view.FractionB;
        }

        public void FractionalArithmetic_Operator(FractionalArithmeticEnum Operator)
        {
            FractionalArithmetic_UpdateFractionWithViewValues();

            switch (Operator)
            {
                case FractionalArithmeticEnum.Addition:
                    fractionResult = calculate.Addition(fractionA, fractionB);
                    break;

                case FractionalArithmeticEnum.Subtraction:
                    fractionResult = calculate.Subtraction(fractionA, fractionB);
                    break;

                case FractionalArithmeticEnum.Multiplication:
                    fractionResult = calculate.Multiplication(fractionA, fractionB);
                    break;

                case FractionalArithmeticEnum.Division:
                    fractionResult = calculate.Division(fractionA, fractionB);
                    if (fractionResult.Denominator == 0)
                    {
                        MessageBox.Show("indeterminate");
                    }
                    break;
            }

            FractionalArithmetic_UpdateViewWithFractionValues();
        }

        public void Reduce(string fraction)
        {
            FractionalArithmetic_UpdateFractionWithViewValues();
            switch (fraction)
            {
                case "A":
                    fractionA.Reduce();
                    break;

                case "B":
                    fractionB.Reduce();
                    break;

                case "Result":
                    fractionResult.Reduce();
                    break;
            }

            FractionalArithmetic_UpdateViewWithFractionValues();
        }

        #endregion
        //==================================================================================================================\\
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        //==================================================================================================================\\
        #region Numbersystems

        //Numbersystems
        string _numbersystemText;
        Conversion conversion = new Conversion();
        enum OldSystem { dec, bin, hex };
        OldSystem oldSystem = OldSystem.dec;

        private void Numbersystems_UpdateViewValuesWithNumbersystemText()
        {
            _view.NumbersystemText = _numbersystemText;
        }

        private void Numbersystems_UpdateNumbersystemTextWithViewValues()
        {
            _numbersystemText = _view.NumbersystemText;
        }

        public void Decimal_Enter()
        {
            Numbersystems_UpdateNumbersystemTextWithViewValues();

            if (_numbersystemText != "")
            {
                if (oldSystem == OldSystem.bin)
                {
                    _numbersystemText = conversion.binaryToDecimal(_numbersystemText);

                }
                else if (oldSystem == OldSystem.hex)
                {
                    _numbersystemText = conversion.hexadecimalToDecimal(_numbersystemText);
                }
            }
            Numbersystems_UpdateViewValuesWithNumbersystemText();
        }

        public void Binary_Enter()
        {
            Numbersystems_UpdateNumbersystemTextWithViewValues();

            if (_numbersystemText != "")
            {
                if (oldSystem == OldSystem.dec)
                {
                    decimal check = Convert.ToDecimal(_numbersystemText);

                    if (check > 18446744073709551615)
                    {
                        MessageBox.Show("overflow");
                        _numbersystemText = "";
                    }
                    else
                    {
                        _numbersystemText = conversion.decimalToBinary(_numbersystemText);
                    }
                }
                else if (oldSystem == OldSystem.hex)
                {
                    _numbersystemText = conversion.hexadecimalToBinary(_numbersystemText);
                }
            }
            Numbersystems_UpdateViewValuesWithNumbersystemText();
        }

        public void Hexadecimal_Enter()
        {
            Numbersystems_UpdateNumbersystemTextWithViewValues();

            if (_numbersystemText != "")
            {
                if (oldSystem == OldSystem.dec)
                {
                    decimal check = Convert.ToDecimal(_numbersystemText);

                    if (check > 18446744073709551615)
                    {
                        MessageBox.Show("overflow");
                        _numbersystemText = "";
                    }
                    else
                    {
                        _numbersystemText = conversion.decimalToHexdecimal(_numbersystemText);
                    }
                }
                else if (oldSystem == OldSystem.bin)
                {
                    _numbersystemText = conversion.binaryToHexdecimal(_numbersystemText);
                }
            }
            Numbersystems_UpdateViewValuesWithNumbersystemText();
        }

        public void Decimal_Leave()
        {
            oldSystem = OldSystem.dec;
        }

        public void Binary_Leave()
        {
            oldSystem = OldSystem.bin;
        }

        public void Hexadecimal_Leave()
        {
            oldSystem = OldSystem.hex;
        }



        #endregion
        //==================================================================================================================\\
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        //==================================================================================================================\\
        #region Boolean Algebra

        //Boolean Algebra
        DataGridView booleanAlgebraInput, booleanAlgebraOutput;
        public enum BooleanOperatorEnum { OR, AND, NOR, NAND, XOR }

        private void BooleanAlgebra_UpdateViewWithBooleanAlgebraValues()
        {
            //_view.BooleanAlgebraInput   = booleanAlgebraInput;
            _view.BooleanAlgebraOutput = booleanAlgebraOutput;
        }

        private void BooleanAlgebra_UpdateBooleanAlgebraValuesWithView()
        {
            booleanAlgebraInput = _view.BooleanAlgebraInput;
            booleanAlgebraOutput = _view.BooleanAlgebraOutput;
        }

        //Setting Empty Cells in a DataGridView to Zero
        private void SettingZeroBooleanAlgebra(DataGridView dataGridView)
        {
            for (int i = 0; i < dataGridView.RowCount; i++)
            {
                for (int j = 0; j < dataGridView.ColumnCount; j++)
                {
                    if (dataGridView[j, i].Value == null)
                    {
                        dataGridView[j, i].Value = "0";
                    }
                }
            }
        }

        //Convert 0 to false and 1 to true 
        private bool[,] ConvertDataGridViewInBoolean(DataGridView dataGridView)
        {
            bool[,] dual = new bool[dataGridView.RowCount, dataGridView.ColumnCount];

            for (int i = 0; i < dataGridView.RowCount; i++)
            {
                for (int j = 0; j < dataGridView.ColumnCount; j++)
                {
                    if (dataGridView[j, i].Value.ToString() == "1")
                    {
                        dual[i, j] = true;
                    }
                    else
                    {
                        dual[i, j] = false;
                    }
                }
            }

            return dual;
        }

        //Convert True and False in 1 and 0 
        private string ConvertToZeroOrOne(bool a)
        {
            if (a)
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }

        //OR
        private bool OR(bool a, bool b)
        {
            return (a || b);
        }

        //AND
        private bool AND(bool a, bool b)
        {
            return (a && b);
        }

        //NOR
        private bool NOR(bool a, bool b)
        {
            return !(a || b);
        }

        //NAND
        private bool NAND(bool a, bool b)
        {
            return !(a && b);
        }

        //XOR
        private bool XOR(bool a, bool b)
        {
            return ((!a && b) || (a && !b));
        }

        public void BooleanAlgebraOperator(BooleanOperatorEnum booleanOperator)
        {
            BooleanAlgebra_UpdateBooleanAlgebraValuesWithView();
            SettingZeroBooleanAlgebra(booleanAlgebraInput);
            bool[,] dual = ConvertDataGridViewInBoolean(booleanAlgebraInput);

            for (int i = 0; i < dual.GetLength(0); i++)
            {
                switch (booleanOperator)
                {
                    case BooleanOperatorEnum.OR:
                        {
                            booleanAlgebraOutput[0, i].Value = ConvertToZeroOrOne(OR(dual[i, 0], dual[i, 1]));
                        }
                        break;

                    case BooleanOperatorEnum.AND:
                        {
                            booleanAlgebraOutput[0, i].Value = ConvertToZeroOrOne(AND(dual[i, 0], dual[i, 1]));
                        }
                        break;

                    case BooleanOperatorEnum.NOR:
                        {
                            booleanAlgebraOutput[0, i].Value = ConvertToZeroOrOne(NOR(dual[i, 0], dual[i, 1]));
                        }
                        break;

                    case BooleanOperatorEnum.NAND:
                        {
                            booleanAlgebraOutput[0, i].Value = ConvertToZeroOrOne(NAND(dual[i, 0], dual[i, 1]));
                        }
                        break;

                    case BooleanOperatorEnum.XOR:
                        {
                            booleanAlgebraOutput[0, i].Value = ConvertToZeroOrOne(XOR(dual[i, 0], dual[i, 1]));
                        }
                        break;
                }
            }
            BooleanAlgebra_UpdateViewWithBooleanAlgebraValues();
        }






        #endregion
        //==================================================================================================================\\
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        //==================================================================================================================\\
        #region Matrices

        //Matrices
        DataGridView matrixA, matrixB, matrixGauß, matrixResult;
        MathAlgorithm mathAlgorithm = new MathAlgorithm();
        public enum MatricesEnum { APlusB, AMinusB, ATimesB, AInverse, DotProduct, CrossProduct, Gauß, ReducedGauß }

        private void Matrices_UpdateViewWithMatrixValues()
        {
            _view.MatrixB = matrixB;
            _view.MatrixResult = matrixResult;
        }

        private void Matrices_UpdateMatricesWithViewValues()
        {
            matrixA = _view.MatrixA;
            matrixB = _view.MatrixB;
            matrixGauß = _view.MatrixGauß;
            matrixResult = _view.MatrixResult;
        }

        public void Matrices_Operator(MatricesEnum matricesEnum)
        {
            Matrices_UpdateMatricesWithViewValues();
            SettingEmptyCellsToZero(matrixA);
            SettingEmptyCellsToZero(matrixB);
            SettingEmptyCellsToZero(matrixGauß);
            ResetDataGridView(matrixResult);

            switch (matricesEnum)
            {
                case MatricesEnum.APlusB:
                    matrixResult = CopyMatrixIntoDatagridview(mathAlgorithm.APlusB(CopyDataGridViewIntomatrix(matrixA), CopyDataGridViewIntomatrix(matrixB)));
                    break;

                case MatricesEnum.AMinusB:
                    matrixResult = CopyMatrixIntoDatagridview(mathAlgorithm.AMinusB(CopyDataGridViewIntomatrix(matrixA), CopyDataGridViewIntomatrix(matrixB)));
                    break;

                case MatricesEnum.ATimesB:
                    matrixResult = CopyMatrixIntoDatagridview(mathAlgorithm.ATimesB(CopyDataGridViewIntomatrix(matrixA), CopyDataGridViewIntomatrix(matrixB)));
                    break;

                case MatricesEnum.AInverse:
                    matrixResult = CopyMatrixIntoDatagridview(mathAlgorithm.AInverted(CopyDataGridViewIntomatrix(matrixA)));
                    break;

                case MatricesEnum.DotProduct:
                    matrixResult = CopyMatrixIntoDatagridview(mathAlgorithm.DotProduct(CopyDataGridViewIntomatrix(matrixA), CopyDataGridViewIntomatrix(matrixB)));
                    break;

                case MatricesEnum.CrossProduct:
                    matrixResult = CopyMatrixIntoDatagridview(mathAlgorithm.CrossProduct(CopyDataGridViewIntomatrix(matrixA), CopyDataGridViewIntomatrix(matrixB)));
                    break;

                case MatricesEnum.Gauß:
                    matrixResult = CopyMatrixIntoDatagridview(mathAlgorithm.Gaußalgorithm(CopyDataGridViewIntomatrix(matrixGauß)));
                    break;

                case MatricesEnum.ReducedGauß:
                    matrixResult = CopyMatrixIntoDatagridview(mathAlgorithm.ReducedGaußAlgorithm(CopyDataGridViewIntomatrix(matrixGauß)));
                    break;
            }
            Matrices_UpdateViewWithMatrixValues();
        }

        //Copy MatrixResult Into MatrixB
        public void CopyResultIntoB()
        {
            Matrices_UpdateMatricesWithViewValues();

            matrixB = matrixResult;

            Matrices_UpdateViewWithMatrixValues();
        }

        //Setting Empty Cells in the DataGridView To Zero
        private void SettingEmptyCellsToZero(DataGridView dataGridView)
        {
            for (int i = 0; i < dataGridView.RowCount - 1; i++)
            {
                for (int j = 0; j < dataGridView.ColumnCount - 1; j++)
                {
                    if (dataGridView[j, i].Value == null)
                    {
                        dataGridView[j, i].Value = "0";
                    }
                }
            }
        }

        //Copy DataGridView Into Matrix
        private decimal[,] CopyDataGridViewIntomatrix(DataGridView dataGridView)
        {
            decimal[,] matrix = new decimal[dataGridView.RowCount - 1, dataGridView.ColumnCount - 1];

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    matrix[i, j] = Convert.ToDecimal(dataGridView[j, i].Value);
                }
            }

            return matrix;
        }

        //Copy Matrix into DataGridView
        private DataGridView CopyMatrixIntoDatagridview(decimal[,] matrix)
        {
            DataGridView dataGridView = new DataGridView();
            dataGridView.RowCount = (matrix.GetLength(0) + 1);
            dataGridView.ColumnCount = (matrix.GetLength(1) + 1);

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    dataGridView[j, i].Value = matrix[i, j].ToString();
                }
            }

            return dataGridView;
        }

        //resetting DatagridView Columns and Rows
        private void ResetDataGridView(DataGridView dataGridView)
        {
            dataGridView.ColumnCount = 1;
            dataGridView.RowCount = 1;
        }

        #endregion
        //==================================================================================================================\\
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        //==================================================================================================================\\
        #region SortingAlgorithm

        //SortingAlgorithms
        int sortingMinvalue, sortingMaxValue, sortingColumns, sortingRows;
        DataGridView sortingTable;
        Random random = new Random();
        SortingAlgorithm sortingAlgorithm;
        decimal[] array;
        public enum SortingAlgorithmEnum { Qick, Selection, Insertion, Bubble, Shaker };

        //RefreshSortingPictureBox
        public void RefreshPictureBoxSorting()
        {
            _view.SortingPictureBox.Refresh();
        }

        private void SortingAlgorithm_UpdateViewWithMatrixValues()
        {
            //_view.SortingTable      = sortingTable;
            _view.SortingMaxValue = sortingMaxValue;
            _view.SortingMinValue = sortingMinvalue;
            _view.SortingRows = sortingRows;
            _view.SortingColumns = sortingColumns;

        }

        private void SortingAlgorithm_UpdateMatrixValuesWithView()
        {
            sortingTable = _view.SortingTable;
            sortingMaxValue = _view.SortingMaxValue;
            sortingMinvalue = _view.SortingMinValue;
            sortingRows = _view.SortingRows;
            sortingColumns = _view.SortingColumns;
        }

        //Drawing Lines on pictureBox with values from SortingTable
        public void SortingDrawingLines(object sender, PaintEventArgs e)
        {
            if (array != null)
            {
                Graphics g;
                g = e.Graphics;
                Random random = new Random();
                Pen pen = new Pen(Color.DarkSlateGray, 1);

                //g.Clear(Color.White);
                decimal biggestNumberInArray;
                int positionOfBiggestNumber = 0;

                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] > array[positionOfBiggestNumber])
                    {
                        positionOfBiggestNumber = i;
                    }
                }

                biggestNumberInArray = array[positionOfBiggestNumber];
                int y = _view.SortingPictureBox.Height;

                for (int i = 1; i < array.Length + 1; i++)
                {
                    int x = (int)(((double)(_view.SortingPictureBox.Width - 5) / array.Length) * i);
                    g.DrawLine(pen, new Point(x, y), new Point(x, (int)(y - (y * 0.95m * (array[i - 1] / biggestNumberInArray)))));
                }
            }
        }

        //Filing SortingDataGridView with random Values
        public void Random()
        {
            SortingAlgorithm_UpdateMatrixValuesWithView();
            ResetDataGridView(sortingTable);
            sortingTable.ColumnCount = sortingColumns + 1;
            sortingTable.RowCount = sortingRows + 1;

            for (int i = 0; i < sortingTable.RowCount - 1; i++)
            {
                for (int j = 0; j < sortingTable.ColumnCount - 1; j++)
                {
                    sortingTable[j, i].Value = random.Next(sortingMinvalue, sortingMaxValue);
                }
            }
            array = CopyDataGridViewXIn1DimensionalMatrix(sortingTable, NullArray(sortingTable));
            RefreshPictureBoxSorting();
            SortingAlgorithm_UpdateViewWithMatrixValues();
        }

        //Choosing the right Sorting-Algorithm
        public void SortingAlgorithm(SortingAlgorithmEnum sortingAlgorithmEnum)
        {
            SortingAlgorithm_UpdateMatrixValuesWithView();
            array = CopyDataGridViewXIn1DimensionalMatrix(sortingTable, NullArray(sortingTable));

            switch (sortingAlgorithmEnum)
            {
                case SortingAlgorithmEnum.Qick:
                    sortingAlgorithm.QuickSort(CopyDataGridViewXIn1DimensionalMatrix(sortingTable, NullArray(sortingTable)), 0, array.Length - 1);
                    break;

                case SortingAlgorithmEnum.Bubble:
                    sortingAlgorithm.BubbleSort(CopyDataGridViewXIn1DimensionalMatrix(sortingTable, NullArray(sortingTable)));
                    break;

                case SortingAlgorithmEnum.Insertion:
                    sortingAlgorithm.InsertionSort(CopyDataGridViewXIn1DimensionalMatrix(sortingTable, NullArray(sortingTable)));
                    break;

                case SortingAlgorithmEnum.Selection:
                    sortingAlgorithm.SelectionSort(CopyDataGridViewXIn1DimensionalMatrix(sortingTable, NullArray(sortingTable)));
                    break;
                case SortingAlgorithmEnum.Shaker:
                    sortingAlgorithm.ShakerSort(CopyDataGridViewXIn1DimensionalMatrix(sortingTable, NullArray(sortingTable)));
                    break;
            }

            TransferArrayIntoDataGridView(array, sortingTable, NullArray(sortingTable));
            SortingAlgorithm_UpdateViewWithMatrixValues();
        }

        //CopyDataGridViewXIn1-dimensionalMatrix
        private decimal[] CopyDataGridViewXIn1DimensionalMatrix(DataGridView dataGridView, int[] nullArray)
        {
            array = new decimal[(dataGridView.RowCount - 1) * (dataGridView.ColumnCount - 1) - nullArray.Length];
            int position = 0;

            for (int i = 0; i < dataGridView.RowCount - 1; i++)
            {
                for (int j = 0; j < dataGridView.ColumnCount - 1; j++)
                {
                    if (!(dataGridView[j, i].Value == null))
                    {
                        array[position] = Convert.ToDecimal(dataGridView[j, i].Value);
                        position++;
                    }
                }
            }

            return array;
        }

        //Array with all cells that are null
        private int[] NullArray(DataGridView dataGridView)
        {
            int arrayGroesse = 0;
            int position = 0;
            int zeahler = 0;
            for (int i = 0; i < dataGridView.RowCount - 1; i++)
            {
                for (int j = 0; j < dataGridView.ColumnCount - 1; j++)
                {
                    if (dataGridView[j, i].Value == null)
                    {
                        arrayGroesse++;
                    }
                }
            }

            int[] nullArray = new int[arrayGroesse];

            for (int i = 0; i < dataGridView.RowCount - 1; i++)
            {
                for (int j = 0; j < dataGridView.ColumnCount - 1; j++, position++)
                {
                    if (dataGridView[j, i].Value == null)
                    {
                        nullArray[zeahler] = position;
                        zeahler++;
                    }
                }
            }

            return nullArray;
        }

        //Transfer 1 dimensional Array into Datagridview
        private void TransferArrayIntoDataGridView(decimal[] array, DataGridView dataGridView, int[] nullArray)
        {
            int position = 0;
            for (int i = 0; i < dataGridView.RowCount - 1; i++)
            {
                for (int j = 0; j < dataGridView.ColumnCount - 1; j++)
                {
                    if (!(dataGridView[j, i].Value == null))
                    {
                        dataGridView[j, i].Value = array[position];
                        position++;
                    }
                }
            }
        }

        #endregion
        //==================================================================================================================\\
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        //==================================================================================================================\\
        #region Functions

        //Functions
        Bitmap bitmap;
        Function function;
        float xmin, xmax, ymin, ymax;

        public void UpdateFunctionTextBox(float _xmin, float _xmax, float _ymin, float _ymax)
        {
            xmin = _xmin;
            xmax = _xmax;
            ymin = _ymin;
            ymax = _ymax;
        }

        private void GetValuesFromView()
        {
            function.DataGridView = _view.FunctionsGrid;
            xmin = _view.FunctionsXMin;
            xmax = _view.FunctionsXMax;
            ymin = _view.FunctionsYMin;
            ymax = _view.FunctionsYMax;
        }

        private void SetValuesFromView()
        {
            _view.FunctionsXMin = xmin;
            _view.FunctionsXMax = xmax;
            _view.FunctionsYMin = ymin;
            _view.FunctionsYMax = ymax;
            _view.FunctionsGraph.Image = bitmap;
        }

        public void PolynomialFunctionMakeGraph()
        {
            GetValuesFromView();

            bitmap = function.MakeGraph(xmin, xmax, ymin, ymax);

            SetValuesFromView();
        }

        public void PolynomialFunctionMouseDown(PointF pointFMouseDown)
        {
            function.MouseDown = pointFMouseDown;
        }

        public void PolynomialFunctionMouseUp(PointF pointFMouseUp)
        {
            GetValuesFromView();
            function.MouseUP = pointFMouseUp;
            bitmap = function.ShiftCoordinateSystem();
            SetValuesFromView();
        }

        public void PolynomialFunctionMouseWheel(MouseEventArgs e)
        {
            GetValuesFromView();
            bitmap = function.MousWheel(e);
            SetValuesFromView();
        }

        public void SetCurrentFunctionType (Function.FunctionType functionType)
        {
            function.CurrentFunctionType = functionType;
        }

        #endregion
        //==================================================================================================================\\
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        //==================================================================================================================\\
        #region Search Algorithm

        bool dijkstraChecked = false;
        bool floydWarshallChecked = false;
        bool dfsChecked = false;
        bool bfsChecked = false;

        //Run Serach Algorithms
        public void GraphAlgorithmRun()
        {
            Graph invokeGraph = _view.Graph;
            RichTextBox invokeRTB = new RichTextBox();
            uint? test = _view.StartValueGraphAlgorithm;

            if (test != 0 && invokeGraph.V - 1 >= test)
            {
                int value = Convert.ToInt32(test);
                if (dijkstraChecked)
                {
                    invokeRTB.Text = string.Empty;
                    Graph.Dijkstra(invokeGraph, value - 1);

                    foreach (Vertex v in invokeGraph)
                    {
                        if (v.id != value - 1)
                        {
                            if (v.min_cost == int.MaxValue || v.min_cost == int.MaxValue * -1)
                                invokeRTB.Text += "Shortest distance between vertices " + value + " and " + (v.id + 1) + " is INFINITY" + Environment.NewLine;

                            else
                                invokeRTB.Text += "Shortest distance between vertices " + value + " and " + (v.id + 1) + " is " + v.min_cost + Environment.NewLine;
                        }

                        v.min_cost = int.MaxValue;
                        v.permanent = false;
                    }
                }

                else if (dfsChecked)
                {
                    foreach (Vertex v in invokeGraph)
                        v.visited = false;

                    invokeRTB.Text = "Depth First Search:\n>" + Graph.DFS(invokeGraph, value - 1);
                }

                else if (bfsChecked)
                {
                    foreach (Vertex v in invokeGraph)
                        v.visited = false;

                    invokeRTB.Text = "Breadth First Search:\n>" + Graph.BFS(invokeGraph, value - 1);
                }

                _view.InvokeRTB = invokeRTB.Text;
            }

            if (floydWarshallChecked)
            {
                Graph.Floyd_Warshall(invokeGraph);
            }
        }

        public void Dijkstra_Enter()
        {
            dijkstraChecked = true;
            floydWarshallChecked = false;
            dfsChecked = false;
            bfsChecked = false;
        }

        public void FloydWarschall_Enter()
        {
            dijkstraChecked = false;
            floydWarshallChecked = true;
            dfsChecked = false;
            bfsChecked = false;
        }

        public void DFS_Enter()
        {
            dijkstraChecked = false;
            floydWarshallChecked = false;
            dfsChecked = true;
            bfsChecked = false;
        }

        public void BFS_Enter()
        {
            dijkstraChecked = false;
            floydWarshallChecked = false;
            dfsChecked = false;
            bfsChecked = true;
        }

        #endregion
    }
}
