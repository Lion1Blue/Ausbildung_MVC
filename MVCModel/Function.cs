using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormMVC.Model
{
    public class Function
    {
        DataGridView dataGridView;
        Graphics gr;
        PointF mouseDown, mouseUp;
        float xmin;
        float xmax;
        float ymin;
        float ymax;
        public enum FunctionType {Polynomial, Logarithm, Exponential};
        FunctionType currentFunction;

        public FunctionType CurrentFunctionType
        {
            set { currentFunction = value; }
        }
        
        IController controller;

        public Function(IController _controller)
        {
            controller = _controller;
        }

        public DataGridView DataGridView
        {
            set { dataGridView = value; }
        }

        public PointF MouseDown
        {
            set { mouseDown = value; }
        }

        public PointF MouseUP
        {
            set { mouseUp = value; }
        }

        public Bitmap ShiftCoordinateSystem()
        {
            System.Drawing.Drawing2D.Matrix inverse = gr.Transform;
            inverse.Invert();
            PointF[] pointFs =
            {
                        mouseDown,
                        mouseUp
            };

            //Transform Points in Coordinate-System Points
            inverse.TransformPoints(pointFs);

            float xOffSet = pointFs[0].X - pointFs[1].X;
            float yOffSet = pointFs[0].Y - pointFs[1].Y;

            xmin = xmin + xOffSet;
            xmax = xmax + xOffSet;
            ymin = ymin + yOffSet;
            ymax = ymax + yOffSet;

            return MakeGraph(xmin, xmax, ymin, ymax);
        }

        public Bitmap MousWheel(MouseEventArgs e)
        {   
            float sign;
            if (e.Delta > 0)
            {
                sign = -1;
            }
            else
            {
                sign = 1;
            }

            float _xmin = xmin + sign * -0.75f;
            float _xmax = xmax + sign * 0.75f;
            float _ymin = ymin + sign * -0.5f;
            float _ymax = ymax + sign * 0.5f;

            //Dont allow the user to zoom to deep into the graph
            if ((_xmin != 0 || _xmax != 0 || _ymin != 0 || _ymax != 0) && !(0.00001 > (_xmax - _xmin) && (_xmax - _xmin) >= -0.00001 ||
                                                                            0.00001 > (_ymax - _ymin) && (_ymax - _ymin) >= -0.00001))
            {
                xmin = _xmin;
                xmax = _xmax;
                ymin = _ymin;
                ymax = _ymax;
            }

            return MakeGraph(xmin, xmax, ymin, ymax);
        }

        private float PolynomialFunction(float x, int row)
        {
            float result = 0;

            for (int i = 0; i < dataGridView.ColumnCount - 1; i++)
            {
                result += Convert.ToSingle(Convert.ToDouble(dataGridView[i, row].Value) * Math.Pow(x, i));
            }

            return result;
        }

        private float LogarithmFunction(float x, int row)
        {
            double a  = Convert.ToDouble(dataGridView[0, row].Value);
            double b  = Convert.ToDouble(dataGridView[1, row].Value);
            double c  = Convert.ToDouble(dataGridView[2, row].Value);
            double d  = Convert.ToDouble(dataGridView[3, row].Value);
            double yo = Convert.ToDouble(dataGridView[4, row].Value);
            
            return Convert.ToSingle(a * Math.Log(c * (x - d), b) + yo);
        }

        private float ExponentialFunction(float x, int row)
        {
            double a  = Convert.ToDouble(dataGridView[0, row].Value);
            double b  = Convert.ToDouble(dataGridView[1, row].Value);
            double c  = Convert.ToDouble(dataGridView[2, row].Value);
            double d  = Convert.ToDouble(dataGridView[3, row].Value);
            double yo = Convert.ToDouble(dataGridView[4, row].Value);

            return Convert.ToSingle(a * Math.Pow(b, c * (x - d)) + yo);
        }

        public Bitmap MakeGraph(float _xmin, float _xmax, float _ymin, float _ymax)
        {
            //The bounds to draw.
            xmin = _xmin;
            xmax = _xmax;
            ymin = _ymin;
            ymax = _ymax;

            // Make the Bitmap.
            int wid = 750;
            int hgt = 500;
            Bitmap bm = new Bitmap(wid, hgt);
            gr = Graphics.FromImage(bm);
            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Transform to map the graph bounds to the Bitmap.
            RectangleF rect = new RectangleF(xmin, ymin, xmax - xmin, ymax - ymin);

            PointF[] pts =
            {
                    new PointF(0, hgt),
                    new PointF(wid, hgt),
                    new PointF(0,0)
            };

            gr.Transform = new System.Drawing.Drawing2D.Matrix(rect, pts);

            // Draw the graph.
            using (Pen graph_pen = new Pen(Color.Blue, 0), grid_pen = new Pen(Color.LightGray, 0))
            {
                for (int x = (int)xmin; x <= xmax; x++)
                {
                    //Draw Grid
                    gr.DrawLine(grid_pen, x, ymin, x, ymax);
                    //Draw AxesSeperation
                    gr.DrawLine(graph_pen, x, -0.1f, x, 0.1f);
                }
                for (int y = (int)ymin; y <= ymax; y++)
                {
                    //Draw Grid
                    gr.DrawLine(grid_pen, xmin, y, xmax, y);
                    //Draw AxesSeperation
                    gr.DrawLine(graph_pen, -0.1f, y, 0.1f, y);
                }
                // Draw the axes.
                gr.DrawLine(graph_pen, xmin, 0, xmax, 0);
                gr.DrawLine(graph_pen, 0, ymin, 0, ymax);

                graph_pen.Color = Color.Red;

                // See how big 1 pixel is horizontally.
                System.Drawing.Drawing2D.Matrix inverse = gr.Transform;
                inverse.Invert();
                PointF[] pixel_pts =
                {
                        new PointF(0, 0),
                        new PointF(1, 0)
                };
                inverse.TransformPoints(pixel_pts);
                float dx = pixel_pts[1].X - pixel_pts[0].X;
                dx /= 4;
                List<PointF> points = new List<PointF>();

                //Draw Function
                //Loop over every function in DataGridView
                for (int i = 0; i < dataGridView.RowCount - 1; i++)
                {
                    if (dataGridView.Rows[i].Selected == true)
                    {
                        // Loop over x values to generate points.
                        for (float x = xmin; x <= xmax; x += dx)
                        {
                            // Get the next point.
                            float y = 0;

                            switch (currentFunction)
                            {
                                case FunctionType.Polynomial:
                                    y = PolynomialFunction(x, i);
                                    break;

                                case FunctionType.Logarithm:
                                    y = LogarithmFunction(x, i);
                                    break;

                                case FunctionType.Exponential:
                                    y = ExponentialFunction(x, i);
                                    break;
                            }

                            if (!y.Equals(Single.NaN))
                            {
                                points.Add(new PointF(x, y));
                            }

                        }
                        if (points.Count != 0)
                        {
                            gr.DrawLines(graph_pen, points.ToArray());
                        }
                        points.Clear();
                    }
                }
            }
            
            controller.UpdateFunctionTextBox(xmin, xmax, ymin, ymax);

            // return the result.
            return bm;
        }
    }
}