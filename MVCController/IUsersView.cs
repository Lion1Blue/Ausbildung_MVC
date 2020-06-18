using System.Windows.Forms;
using WinFormMVC.Model;

namespace WinFormMVC.Controller
{
    public interface IUsersView
    {
        void SetController(Controller controller);

        //Fractional Arithmetic
        Fraction FractionA                  { get; set; }
        Fraction FractionB                  { get; set; }
        Fraction FractionResult             { get; set; }

        //Numbersystems
        string NumbersystemText             { get; set; }

        //BooleanAlgebra
        DataGridView BooleanAlgebraInput    { get; set; }
        DataGridView BooleanAlgebraOutput   { get; set; }

        //Matrices
        DataGridView MatrixA                { get; set; }
        DataGridView MatrixB                { get; set; }
        DataGridView MatrixGauß             { get; set; }
        DataGridView MatrixResult           { get; set; }

        //SortingAlgorithms
        DataGridView SortingTable           { get; set; }
        PictureBox SortingPictureBox        { get; set; }
        int SortingMinValue                 { get; set; }

        int SortingMaxValue                 { get; set; }
        int SortingColumns                  { get; set; }
        int SortingRows                     { get; set; }

        //PolynomialFunction
        DataGridView FunctionsGrid { get; set; }
        PictureBox FunctionsGraph  { get; set; }
        float FunctionsXMin        { get; set; }
        float FunctionsXMax        { get; set; }
        float FunctionsYMin        { get; set; }
        float FunctionsYMax        { get; set; }

        //GraphAlgorithms
        uint StartValueGraphAlgorithm   { get; }
        Graph Graph                     { get; }
        string InvokeRTB                { set; }
        
    }   
}
