using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FractalViewer
{
    

    public partial class Form1 : Form
    {

        private int maxTreeIterations;
        private int maxSierTriaIterations;
        private Bitmap bmp;
        private Fractal currentFractal;

        private enum Fractal
        {
            RandomTree = 0,
            SierpinskiTriangule
        };

        //Margin percentages.
        private int topMarginPercentage;
        private int bottomMarginPercentage;
        private int leftMarginPercentage;
        private int rightMarginPercentage;

        public Form1()
        {
            InitializeComponent();
            topMarginPercentage = 10;
            bottomMarginPercentage = 10;
            leftMarginPercentage = 10;
            rightMarginPercentage = 10;
        }

        #region Fractal draw methods
        #region Random tree methods
        private void DrawRandomTree(Bitmap bmp)
        {

            //For some randomness.
            Random random = new Random();
            maxTreeIterations = random.Next(9, 16);

            //We draw the tree trunk first.
            //It'll be aligned horizontally and its length is gonna be
            //half the height minus the margin.
            //We'll use polar coordinates btw.
            double treeTrunkOriginX = bmp.Width / 2.0;
            double treeTrunkOriginY = bmp.Height - (bottomMarginPercentage / 100.0 * bmp.Height);
            double treeTrunkLength = (bmp.Height - (bottomMarginPercentage / 100.0 * bmp.Height) 
                                        - (topMarginPercentage / 100.0 * bmp.Height)) / 3.0;
            //We want the trunk vertical, so the angle must be -90°, since our origin is beneath the final point
            //otherwise, i'll rotate down.
            double treeTrunkAngle = -Math.PI / 2.0;
            //We calculate the final X and Y coordinates using trigonometric functions.
            double treeTrunkFinalX = (treeTrunkOriginX + treeTrunkLength * Math.Cos(treeTrunkAngle));
            double treeTrunkFinalY = (treeTrunkOriginY + treeTrunkLength * Math.Sin(treeTrunkAngle));
            
            //Create a graphics object from the bmp, so we can access all the methods.
            Graphics g = Graphics.FromImage(bmp);
            //Dibujamos el fondo.
            g.DrawRectangle(Pens.White, 0, 0, bmp.Width, bmp.Height);
            //We simply draw the line.
            //First, let's create a starting random pen color.
            Color penColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
            //And a final pen color as well.
            Color finalPenColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
            float penWidth = 15;
            g.DrawLine(new Pen(penColor, penWidth), (float)treeTrunkOriginX, (float)treeTrunkOriginY, (float)treeTrunkFinalX, (float)treeTrunkFinalY);

            //Now, we start drawing the branches.
            //We'll reduce the width of the pen, as well as alter the color of the branches, based on the iteration.
            //The length, at first, will not be changed.
            //First the left one, so that's -45°.
            _DrawBranchesRecursively(g, treeTrunkFinalX, treeTrunkFinalY, treeTrunkAngle - Math.PI / 4.0, treeTrunkLength * 0.7, (penColor.ToArgb() + (finalPenColor.ToArgb() - penColor.ToArgb()) / maxTreeIterations), (finalPenColor.ToArgb() - penColor.ToArgb()) / maxTreeIterations, penWidth - (penWidth - 1.0f) / maxTreeIterations, (penWidth - 1.0f) / maxTreeIterations, 1);
            //Then the right one, +45°.
            _DrawBranchesRecursively(g, treeTrunkFinalX, treeTrunkFinalY, treeTrunkAngle + Math.PI / 4.0, treeTrunkLength * 0.7, (penColor.ToArgb() + (finalPenColor.ToArgb() - penColor.ToArgb()) / maxTreeIterations), (finalPenColor.ToArgb() - penColor.ToArgb()) / maxTreeIterations, penWidth - (penWidth - 1.0f) / maxTreeIterations, (penWidth - 1.0f) / maxTreeIterations, 1);
        }

        private void _DrawBranchesRecursively(Graphics g, double originX, double originY, double angle, double length, int penColor, int penColorIncrement, float penWidth, float penWidthIncrement, int iteration)
        {
            if(iteration <= maxTreeIterations)
            {
                //We calculate the final X and Y coordinates.
                double finalX = originX + length * Math.Cos(angle);
                double finalY = originY + length * Math.Sin(angle);

                //Now, let's draw the line.
                g.DrawLine(new Pen(Color.FromArgb(penColor), penWidth), (float)originX, (float)originY, (float)finalX, (float)finalY);

                //Now, we calculate how much the length is to be reduced..

                double newLength = length * 0.7;

                _DrawBranchesRecursively(g, finalX, finalY, angle - Math.PI / 4.0, newLength, (penColor + penColorIncrement), penColorIncrement, penWidth - penWidthIncrement, penWidthIncrement, iteration + 1);
                _DrawBranchesRecursively(g, finalX, finalY, angle + Math.PI / 4.0, newLength, (penColor + penColorIncrement), penColorIncrement, penWidth - penWidthIncrement, penWidthIncrement, iteration + 1);
            }
        }
        #endregion
        #region Sierpinski triangule methods
        private void DrawSierpinskiTriangule(Bitmap bmp)
        {
            //For some randomness
            Random random = new Random();

            //First, les find the 3 points of the main triangule.
            PointF leftPoint = new PointF();
            PointF rightPoint = new PointF();
            PointF middlePoint = new PointF();

            leftPoint.X = (float)(leftMarginPercentage / 100.0 * bmp.Width);
            leftPoint.Y = (float)(bmp.Height - bottomMarginPercentage / 100.0 * bmp.Height);

            rightPoint.X = (float)(bmp.Width - rightMarginPercentage / 100.0 * bmp.Width);
            rightPoint.Y = (float)(bmp.Height - bottomMarginPercentage / 100.0 * bmp.Height);

            middlePoint.X = (float)((leftMarginPercentage / 100.0 * bmp.Width) + ((bmp.Width - (leftMarginPercentage / 100.0 * bmp.Width) - (rightMarginPercentage / 100.0 * bmp.Width)) / 2.0));
            middlePoint.Y = (float)(0 + topMarginPercentage / 100.0 * bmp.Height);

            Graphics g = Graphics.FromImage(bmp);

            //We generate 2 random colors for use as range
            Color startingColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
            Color endingColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));

            //We generate random number of iterations..
            maxSierTriaIterations = random.Next(6, 13);

            //Let's calculate the color increment..
            int colorInc = (endingColor.ToArgb() - startingColor.ToArgb()) / maxSierTriaIterations;
            int penWidth = 1;

            //We draw the triangule.
            g.DrawLine(new Pen(startingColor, penWidth), leftPoint, rightPoint);
            g.DrawLine(new Pen(startingColor, penWidth), rightPoint, middlePoint);
            g.DrawLine(new Pen(startingColor, penWidth), middlePoint, leftPoint);

            //We draw the recursive inverted triangule..
            _DrawInvertedTrianguleRecursively(g, leftPoint, rightPoint, middlePoint, startingColor.ToArgb() + colorInc, penWidth, colorInc, 1);

        }

        private void _DrawInvertedTrianguleRecursively(Graphics g, PointF left, PointF right, PointF middle, int penColor, int penWidth, int colorInc, int iteration)
        {
            if(iteration <= maxSierTriaIterations)
            {
                //Let's find the points for the inverted triangule..
                PointF l = new PointF();
                PointF r = new PointF();
                PointF m = new PointF();

                l.X = (float)(left.X + ((middle.X - left.X) / 2.0));
                l.Y = (float)(middle.Y + ((left.Y - middle.Y) / 2.0));

                r.X = (float)(right.X - ((middle.X - left.X) / 2.0));
                r.Y = (float)(middle.Y + ((left.Y - middle.Y) / 2.0));

                m.X = (float)(left.X + ((right.X - left.X) / 2.0));
                m.Y = left.Y;

                //We draw the triangule..
                g.DrawLine(new Pen(Color.FromArgb(penColor), penWidth), l, r);
                g.DrawLine(new Pen(Color.FromArgb(penColor), penWidth), r, m);
                g.DrawLine(new Pen(Color.FromArgb(penColor), penWidth), m, l);

                //Call the recursive method for each of the 3 new triangules created..
                _DrawInvertedTrianguleRecursively(g, left, m, l, penColor + colorInc, penWidth, colorInc, iteration + 1);
                _DrawInvertedTrianguleRecursively(g, m, right, r, penColor + colorInc, penWidth, colorInc, iteration + 1);
                _DrawInvertedTrianguleRecursively(g, l, r, middle, penColor + colorInc, penWidth, colorInc, iteration + 1);

            }
        }
        #endregion
        #endregion

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if(bmp != null)
            {
                e.Graphics.DrawImage(bmp, 0, 0, pictureBox1.Width, pictureBox1.Height);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            currentFractal = (Fractal)cbxFractals.SelectedIndex;

            if (Enum.IsDefined(typeof(Fractal), currentFractal))
            {
                bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                switch (currentFractal)
                {
                    case Fractal.RandomTree:
                        DrawRandomTree(bmp);
                        break;
                    case Fractal.SierpinskiTriangule:
                        DrawSierpinskiTriangule(bmp);
                        break;
                }
            }
            pictureBox1.Invalidate();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void savePictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(bmp != null && Enum.IsDefined(typeof(Fractal), currentFractal))
            {
                //We find out what fractal is being saved.
                string name = "image";
                switch (currentFractal)
                {
                    case Fractal.RandomTree:
                        name = "random_tree";
                        break;
                    case Fractal.SierpinskiTriangule:
                        name = "sierpinski_triangule";
                        break;
                }
                name += ".bmp";
                saveFileDialog1.FileName = name;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    bmp.Save(saveFileDialog1.FileName);
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Display simple window message..
            MessageBox.Show("Created by Fernando Alfonso Caldera Olivas\r\n- FACO");
        }
    }
}
