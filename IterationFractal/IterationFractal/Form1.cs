using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IterationFractal
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region members

        List<PointF> _points = new List<PointF>();
        Bitmap _bitmap = null;
        Random _random = new Random();

        #endregion

        #region private

        void CreateFractal()
        {
            PointF curPoint = new PointF(pictureBox1.Width / 2, pictureBox1.Height / 2);
            PointF point;
            float level = 1.0f / 3.0f;

            for (int i = 0; i < 48000; i++)
            {
                double r = _random.NextDouble();

                if (r < level)
                {
                    point = _points[0];
                }
                else if (r < 2 * level)
                {
                    point = _points[1];
                }
                else
                {
                    point = _points[2];
                }

                float X = (curPoint.X + point.X) / 2;
                float Y = (curPoint.Y + point.Y) / 2;

                _points.Add(new PointF(X, Y));

                curPoint.X = X;
                curPoint.Y = Y;
            }
        }

        void CreateBitmap()
        {
            if (pictureBox1.Width < 1 || pictureBox1.Height < 1)
                return;

            _bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        }

        void Render()
        {
            if (_bitmap == null)
                return;

            Rectangle r = new Rectangle(0, 0, _bitmap.Width, _bitmap.Height);
            System.Drawing.Imaging.BitmapData bmpData = _bitmap.LockBits(r, System.Drawing.Imaging.ImageLockMode.ReadWrite, _bitmap.PixelFormat);

            int bytes = Math.Abs(bmpData.Stride) * _bitmap.Height;
            byte[] rgbValues = new byte[bytes];
            Array.Clear(rgbValues, 0, bytes);

            foreach (PointF point in _points)
            {
                int X = Convert.ToInt32(point.X);
                int Y = Convert.ToInt32(point.Y);

                rgbValues[Y * Math.Abs(bmpData.Stride) + X * 3 + 1] = 255; // зеленый
            }

            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, bmpData.Scan0, bytes);
            _bitmap.UnlockBits(bmpData);

            pictureBox1.Image = _bitmap;
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.White;
            CreateBitmap();

            _points.Add(new PointF(pictureBox1.Width / 2, 20));
            _points.Add(new PointF(pictureBox1.Width - 20, pictureBox1.Height - 20));
            _points.Add(new PointF(20, pictureBox1.Height - 20));

            CreateFractal();
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            CreateBitmap();
        }     

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }
    }
}
