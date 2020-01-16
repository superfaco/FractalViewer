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
        
        private enum Fractal
        {
            RandomTree = 0,
            SierpinskiTriangule
        };

        private int maxTreeIterations;
        private Bitmap bmp;

        public Form1()
        {
            InitializeComponent();
        }

        #region Fractal draw methods
        #region Random tree methods
        private void DrawRandomTree(Bitmap bmp)
        {
            //Margin percentages.
            int topMarginPercentage = 10;
            int bottomMarginPercentage = 10;

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
            Color penColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256), random.Next(256));
            //And a final pen color as well.
            Color finalPenColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256), random.Next(256));
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
            Fractal fractal = (Fractal)cbxFractals.SelectedIndex;

            if (Enum.IsDefined(typeof(Fractal), fractal))
            {
                bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                switch (fractal)
                {
                    case Fractal.RandomTree:
                        DrawRandomTree(bmp);
                        break;
                    case Fractal.SierpinskiTriangule:
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
            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                bmp.Save(saveFileDialog1.FileName);
            }
        }
    }
}
