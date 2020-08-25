using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace IterationFractal
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region private

        void Start()
        {
            const float cShift = 16;
            float len = pictureBox1.Width - 2 * cShift;

            List<PointF> points = new List<PointF>
            {
                new PointF(cShift + len / 2, pictureBox1.Height - cShift - len * Convert.ToSingle(Math.Sin(Math.PI / 3))),
                new PointF(cShift + len, pictureBox1.Height - cShift),
                new PointF(cShift, pictureBox1.Height - cShift)
            };

            CreateFractal(points);
            Render(points);
        }

        void CreateFractal(List<PointF> pointFs)
        {
            PointF curPoint = new PointF((pointFs[0].X + pointFs[1].X) / 2, (pointFs[0].Y + pointFs[1].Y) / 2);
            Random rnd = new Random();

            for (int i = 0; i < 48000; i++)
            {
                int index = rnd.Next(3);

                float X = (curPoint.X + pointFs[index].X) / 2;
                float Y = (curPoint.Y + pointFs[index].Y) / 2;

                pointFs.Add(new PointF(X, Y));

                curPoint.X = X;
                curPoint.Y = Y;
            }
        }

        void Render(IEnumerable<PointF> points)
        {
            Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Rectangle r = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            System.Drawing.Imaging.BitmapData bmpData = bitmap.LockBits(r, System.Drawing.Imaging.ImageLockMode.ReadWrite, bitmap.PixelFormat);

            int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
            byte[] rgbValues = new byte[bytes];
            Array.Clear(rgbValues, 0, bytes);

            foreach (PointF point in points)
            {
                int X = Convert.ToInt32(point.X);
                int Y = Convert.ToInt32(point.Y);

                rgbValues[Y * Math.Abs(bmpData.Stride) + X * 3 + 0] = 255; // синий
                rgbValues[Y * Math.Abs(bmpData.Stride) + X * 3 + 1] = 255; // зеленый
            }

            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, bmpData.Scan0, bytes);
            bitmap.UnlockBits(bmpData);

            pictureBox1.Image = bitmap;
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            Start();
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            Start();
        }
    }
}
