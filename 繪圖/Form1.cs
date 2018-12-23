using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 繪圖
{
    public partial class Form1 : Form
    {
        List<GraphPoint> PointsList = new List<GraphPoint>();
        bool is_Drowing = false;
        int Mouse_Mode = 1;  //1=指標 2=畫線
        List<GraphLine> LineList = new List<GraphLine>();
        List<GraphCurve> CurveList = new List<GraphCurve>();
        List<GraphGroup> GroupList = new List<GraphGroup>();
        GraphLine SelectedLine = null;
        MoveInfo Moving = null;
        GraphPoint SelectedPoint = null;
        GraphCurve SelectedCurve = null;
        int SelectedCurveIndex = -1;
        GraphCurve SelectedCurveControl = null;
        int SelectedCurveControlIndex = -1;
        int SeletedCurveControlFS = -1;
        GraphGroup SelectedGroup = null;
        int SizeOfNet = 10;
        bool Ctrl = false;

        GraphCurve PreviousCruve = new GraphCurve();
        GraphPoint previous_point;
        List<TabPage> TabpagesList = new List<TabPage>();
        List<TabpageData> TabpageDataList = new List<TabpageData>();
        Point For_Paint;
        bool Net_Mode = false;

        public class MoveInfo
        {
            public GraphLine Line;
            public GraphCurve Curve;
            public GraphGroup Group;
            public List<GraphPoint> Path;
            public GraphPoint StartLinePoint;
            public GraphPoint EndLinePoint;
            public Point StartMoveMousePoint;
            public int CurveIndex;
            public int CurveFS;
            public int type;//1=line 2=point 3=curve 4=curveControl 5=mutiselect
        }
        public class GraphLine
        {
            /*public GraphLine(float x1, float y1, float x2, float y2, int a, int b)
            {
                this.StartPoint = new GraphPoint(x1, y1);
                this.EndPoint = new GraphPoint(x2, y2);
                this.StartP = a;
                this.EndP = b;
            }*/
            public GraphLine(GraphPoint a, GraphPoint b)
            {
                StartPoint = a;
                EndPoint = b;
            }
            public GraphPoint StartPoint;
            public GraphPoint EndPoint;
        }
        public class GraphPoint
        {
            public GraphPoint(float x,float y)
            {
                this.P = new PointF(x, y);
                Relative = 0;
            }
            public PointF P;
            public int Relative;
            /*
            public static bool operator ==(GraphPoint a, PointF b)
            {
                if (a.P == b)
                    return true;
                else
                    return false;
            }
            public static bool operator ==(GraphPoint a, GraphPoint b)
            {
                if (a.P == b.P)
                    return true;
                else
                    return false;
            }
            public static bool operator !=(GraphPoint a, PointF b)
            {
                if (a.P != b)
                    return true;
                else
                    return false;
            }
            public static bool operator !=(GraphPoint a, GraphPoint b)
            {
                if (a.P != b.P)
                    return true;
                else
                    return false;
            }
            */
        }
        public class GraphCurve
        {
            public List<GraphPoint> path = new List<GraphPoint>();
            public List<PointF> disFirst = new List<PointF>();
            public List<PointF> disSecond = new List<PointF>();
            public List<int> type = new List<int>();//0:L=R 1:L.angle=R 2:L!=R
            public bool equal(GraphCurve c)
            {
                if (path.Count != c.path.Count)
                    return false;
                for(int i = 0; i < path.Count; i++)
                {
                    if (path[i].P.X != c.path[i].P.X || path[i].P.Y != c.path[i].P.Y)
                        return false;
                }
                return true;
            }
        }
        public class TabpageData
        {
            public List<GraphPoint> PL = new List<GraphPoint>();
            public List<GraphLine> LL = new List<GraphLine>();
            public List<GraphCurve> CL = new List<GraphCurve>();
            public List<GraphGroup> GL = new List<GraphGroup>();
            public List<TabpageData> Undo = new List<TabpageData>();
            public int Undo_index = -1;
            public string TabpageName;
        }
        public class GraphGroup
        {
            public List<GraphPoint> P = new List<GraphPoint>();
            public List<GraphLine> L = new List<GraphLine>();
            public List<GraphCurve> C = new List<GraphCurve>();
            public List<GraphGroup> G = new List<GraphGroup>();
        }
        static GraphPoint FindPointByPoint(List<GraphPoint> points, Point p)
        {
            var size = 5;
            var buffer = new Bitmap(size * 2, size * 2);
            foreach (var poi in points)
            {
                //draw each line on small region around current point p and check pixel in point p 

                using (var g = Graphics.FromImage(buffer))
                {
                    g.Clear(Color.Black);
                    g.DrawRectangle(new Pen(Color.Green, 5), poi.P.X - p.X + size - 2, poi.P.Y - p.Y + size - 2, size, size);
                }

                if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    return poi;
            }
            return null;
        }
        static GraphLine FindLineByPoint(List<GraphLine> lines, Point p)
        {
            var size = 10;
            var buffer = new Bitmap(size * 2, size * 2);
            foreach (var line in lines)
            {
                //draw each line on small region around current point p and check pixel in point p 

                using (var g = Graphics.FromImage(buffer))
                {
                    g.Clear(Color.Black);
                    g.DrawLine(new Pen(Color.Green, 3), line.StartPoint.P.X - p.X + size, line.StartPoint.P.Y - p.Y + size, line.EndPoint.P.X - p.X + size, line.EndPoint.P.Y - p.Y + size);
                }

                if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    return line;
            }
            return null;
        }
        static public void FindCurveByPoint(List<GraphCurve> curves, Point p, out GraphCurve Cout, out int Cindex)
        {
            int size = 10;
            var buffer = new Bitmap(size * 2, size * 2);
            foreach (var curve in curves)
            {
                List<PointF> temp = GraphCurveToBez(curve);
                List<PointF> temp2 = new List<PointF>();
                for (int i = 0; i < temp.Count; i++)
                {
                    temp2.Add(new PointF(temp[i].X - p.X + size, temp[i].Y - p.Y + size));
                }
                for(int i = 0; i < temp2.Count-1; i += 3)
                {
                    PointF[] t = { temp2[i], temp2[i + 1], temp2[i + 2], temp2[i + 3] };
                    using (var g = Graphics.FromImage(buffer))
                    {
                        g.Clear(Color.Black);
                        g.DrawBeziers(new Pen(Color.Green, 3), t);
                    }

                    if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    {
                        Cout = curve;
                        Cindex = i / 3 + 1;
                        return;
                    }
                }
            }
            Cout = null;
            Cindex = -1;
            return;
        }
        public void FindCurveControlByPoint(List<GraphCurve> curves, Point p, out GraphCurve Cout,out int index, out int FS)//first=0 second=1
        {
            int size = 5;
            var buffer = new Bitmap(size * 2, size * 2);
            foreach (var curve in curves)
            {
                if (SelectedGroup != null)
                {
                    if (SelectedGroup.C.Count == 1 && SelectedGroup.L.Count == 0 && SelectedGroup.C.FindIndex(a => a == curve) >= 0)
                    {
                        List<PointF> temp = GraphCurveToBez(curve);
                        List<PointF> temp2 = new List<PointF>();
                        for (int i = 0; i < temp.Count; i++)
                        {
                            temp2.Add(new PointF(temp[i].X - p.X + size, temp[i].Y - p.Y + size));
                        }
                        for (int i = 0; i < temp2.Count; i += 3)
                        {
                            if (i - 1 > 0)
                            {
                                using (var g = Graphics.FromImage(buffer))
                                {
                                    g.Clear(Color.Black);
                                    g.DrawRectangle(new Pen(Color.Green, 5), temp2[i - 1].X - 2, temp2[i - 1].Y - 2, size, size);
                                }
                                if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                                {
                                    Cout = curve;
                                    FS = 0;
                                    index = i / 3;
                                    return;
                                }
                            }
                            if (i + 1 < temp2.Count)
                            {
                                using (var g = Graphics.FromImage(buffer))
                                {
                                    g.Clear(Color.Black);
                                    g.DrawRectangle(new Pen(Color.Green, 5), temp2[i + 1].X - 2, temp2[i + 1].Y - 2, size, size);
                                }
                                if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                                {
                                    Cout = curve;
                                    FS = 1;
                                    index = i / 3;
                                    return;
                                }
                            }
                        }
                    }
                }
                
            }
            Cout = null;
            FS = -1;
            index = -1;
            return;
        }

        public Form1()
        {
            InitializeComponent();
            toolStripTextBox1.Enabled = false;
        }
        private void tabControl1_KeyDown(object sender, KeyEventArgs e)
        {
            Ctrl = e.Control;
        }
        private void tabControl1_KeyUp(object sender, KeyEventArgs e)
        {
            Ctrl = e.Control;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e) // 開啟影像
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Load(openFileDialog1.FileName);
                ImgFitSize();
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                ImgFitSize();
            }
            splitContainer1.Width = this.Width;
            splitContainer1.Height = this.Height - menuStrip1.Height - toolStrip1.Height - statusStrip1.Height - 52;
            tabControl1.Width = splitContainer1.Panel1.Width;
            tabControl1.Height = splitContainer1.Panel1.Height;
        }
        private void splitContainer1_Panel1_SizeChanged(object sender, EventArgs e)
        {
            tabControl1.Width = splitContainer1.Panel1.Width;
            tabControl1.Height = splitContainer1.Panel1.Height;
        }
        private void splitContainer1_Panel2_SizeChanged(object sender, EventArgs e)
        {
            tabControl1.Width = splitContainer1.Panel1.Width;
            tabControl1.Height = splitContainer1.Panel1.Height;
        }

        private void 儲存影像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                int type = saveFileDialog1.FilterIndex;
                Bitmap t = new Bitmap(pictureBox1.Image);
                switch (type)
                {
                    case 1:
                        t.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case 2:
                        t.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                    case 3:
                        t.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    default:
                        break;
                }
            }
        }
        private void 新影像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Width = 800;
            pictureBox1.Height = 600;
            pictureBox1.Image = new Bitmap(800, 600);
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(Color.WhiteSmoke);
            PointsList = new List<GraphPoint>();
            LineList = new List<GraphLine>();
            CurveList = new List<GraphCurve>();
            GroupList = new List<GraphGroup>();

            TabpagesList.Clear();
            TabpageDataList.Clear();
            TabpagesList.Add(new TabPage("new page"));
            TabpagesList.Add(new TabPage("+"));
            tabControl1.TabPages.Clear();
            tabControl1.TabPages.Add(TabpagesList[0]);
            tabControl1.TabPages.Add(TabpagesList[1]);
            tabControl1.SelectedIndex = 0;
            TabpagesList[0].Controls.Add(pictureBox1);
            pictureBox1.Location = new Point(0, 0);

            TabpageData a = new TabpageData();
            a.CL = CurveList;
            a.PL = PointsList;
            a.LL = LineList;
            a.GL = GroupList;
            a.Undo = Undo_Data;
            a.TabpageName = TabpagesList[0].Text;
            TabpageDataList.Add(a);
            Push_Undo_Data();

            pictureBox1.Visible = true;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            if (Net_Mode)
            {
                for (int i = 0; i < this.pictureBox1.Height; i += SizeOfNet)
                {
                    var pen = new Pen(Color.LightGray, 2);
                    e.Graphics.DrawLine(pen, new Point(0, i), new Point(this.pictureBox1.Width, i));

                }
                for (int i = 0; i < this.pictureBox1.Width; i += SizeOfNet)
                {
                    var pen = new Pen(Color.LightGray, 2);
                    e.Graphics.DrawLine(pen, new Point(i, 0), new Point(i, this.pictureBox1.Height));
                }
            }
            foreach (var line in LineList)
            {
                var color = Color.Black;
                if (SelectedGroup!=null)
                    color = SelectedGroup.L.FindIndex(x => x == line) < 0 ? Color.Black : Color.Red;
                var pen = new Pen(color, 2);
                e.Graphics.DrawLine(pen, line.StartPoint.P, line.EndPoint.P);
            }
            foreach (var p in PointsList)
            {
                var color = p == SelectedPoint ? Color.Red : Color.Black;
                var size = p == SelectedPoint ? 2 : 1;
                var pen = new Pen(color, size);
                e.Graphics.DrawRectangle(pen, p.P.X - 3, p.P.Y - 3, 6, 6);
            }
            foreach (var c in CurveList)
            {
                var color = Color.Black;
                if (SelectedGroup != null)
                    color = SelectedGroup.C.FindIndex(x => x == c) < 0 ? Color.Black : Color.Red;
                var pen = new Pen(color, 2);
                List<PointF> t = GraphCurveToBez(c);
                e.Graphics.DrawBeziers(pen, t.ToArray());
                var l = new Pen(Color.LightSkyBlue, 1);
                var rec = new Pen(Color.Orange, 1);
                if (SelectedGroup != null)
                {
                    if (SelectedGroup.C.Count == 1 && SelectedGroup.L.Count == 0 && SelectedGroup.C.FindIndex(a => a == c) >= 0)
                    {
                        for (int i = 0; i < t.Count; i += 3)
                        {
                            if (i == 0)
                            {
                                e.Graphics.DrawLine(l, t[i], t[i + 1]);
                                e.Graphics.DrawRectangle(rec, t[i + 1].X - 2, t[i + 1].Y - 2, 4, 4);
                            }
                            else if (i == t.Count - 1)
                            {
                                e.Graphics.DrawLine(l, t[i], t[i - 1]);
                                e.Graphics.DrawRectangle(rec, t[i - 1].X - 2, t[i - 1].Y - 2, 4, 4);
                            }
                            else
                            {
                                e.Graphics.DrawLine(l, t[i], t[i + 1]);
                                e.Graphics.DrawRectangle(rec, t[i + 1].X - 2, t[i + 1].Y - 2, 4, 4);
                                e.Graphics.DrawLine(l, t[i], t[i - 1]);
                                e.Graphics.DrawRectangle(rec, t[i - 1].X - 2, t[i - 1].Y - 2, 4, 4);
                            }
                        }
                    }
                }
                
            }
            if (Moving != null)
            {
                if (Moving.type == 5)
                {
                    Pen pe = new Pen(Color.LightBlue, 1.5F);
                    float x = Moving.StartMoveMousePoint.X < For_Paint.X ? Moving.StartMoveMousePoint.X : For_Paint.X;
                    float y = Moving.StartMoveMousePoint.Y < For_Paint.Y ? Moving.StartMoveMousePoint.Y : For_Paint.Y;
                    float width = Math.Abs(Moving.StartMoveMousePoint.X - For_Paint.X);
                    float height = Math.Abs(Moving.StartMoveMousePoint.Y - For_Paint.Y);
                    e.Graphics.DrawRectangle(pe, x, y, width, height);
                }
            }
            if (is_Drowing)
            {
                Point p = For_Paint;
                if (Net_Mode)
                {
                    p.X += SizeOfNet / 2;
                    p.Y += SizeOfNet / 2;
                    p.X -= p.X % SizeOfNet;
                    p.Y -= p.Y % SizeOfNet;
                }
                Pen pe = new Pen(Color.Black, 2);
                if (Mouse_Mode == 2)
                {
                    if (SelectedPoint == null)
                        e.Graphics.DrawLine(pe, previous_point.P.X, previous_point.P.Y, p.X, p.Y);
                    else
                        e.Graphics.DrawLine(pe, previous_point.P.X, previous_point.P.Y, SelectedPoint.P.X, SelectedPoint.P.Y);
                }
                else if (Mouse_Mode == 3)
                {
                    GraphCurve c = CurveToBez(PreviousCruve.path, 1, p);
                    List<PointF> t = GraphCurveToBez(c);
                    e.Graphics.DrawBeziers(pe, t.ToArray());
                }
            }
            
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            toolStripStatusLabel1.Text = "X:" + e.X;
            toolStripStatusLabel2.Text = "Y:" + e.Y;
            if (pictureBox1.Image != null)
            {
                if (Mouse_Mode == 1)
                {
                    if (Moving != null)
                    {
                        if (Moving.type == 1)
                        {
                            GraphPoint sp = new GraphPoint(Moving.StartLinePoint.P.X + e.X - Moving.StartMoveMousePoint.X, Moving.StartLinePoint.P.Y + e.Y - Moving.StartMoveMousePoint.Y);
                            GraphPoint ep = new GraphPoint(Moving.EndLinePoint.P.X + e.X - Moving.StartMoveMousePoint.X, Moving.EndLinePoint.P.Y + e.Y - Moving.StartMoveMousePoint.Y);
                            Moving.Line.StartPoint.P.X = sp.P.X;
                            Moving.Line.StartPoint.P.Y = sp.P.Y;
                            Moving.Line.EndPoint.P.X = ep.P.X;
                            Moving.Line.EndPoint.P.Y = ep.P.Y;
                        }
                        else if (Moving.type == 2)
                        {
                            
                            GraphPoint sp = new GraphPoint(e.X, e.Y);
                            GraphPoint st = FindPointByPoint(PointsList, new Point((int)sp.P.X, (int)sp.P.Y));
                            if (Net_Mode)
                            {
                                sp.P.X += SizeOfNet / 2;
                                sp.P.Y += SizeOfNet / 2;
                                sp.P.X -= sp.P.X % SizeOfNet;
                                sp.P.Y -= sp.P.Y % SizeOfNet;
                            }
                            if (st != null)
                            {
                                sp.P.X = st.P.X;
                                sp.P.Y = st.P.Y;
                            }
                            Moving.StartLinePoint.P.X = sp.P.X;
                            Moving.StartLinePoint.P.Y = sp.P.Y;
                        }
                        else if (Moving.type == 3)
                        {
                            for(int i = 0; i < Moving.Path.Count; i++)
                            {
                                Moving.Curve.path[i].P.X = Moving.Path[i].P.X + e.X - Moving.StartMoveMousePoint.X;
                                Moving.Curve.path[i].P.Y = Moving.Path[i].P.Y + e.Y - Moving.StartMoveMousePoint.Y;
                            }
                        }
                        else if (Moving.type == 4)
                        {
                            if (Moving.CurveFS == 0)
                            {
                                float x = e.X - SelectedCurveControl.path[SelectedCurveControlIndex].P.X;
                                float y = e.Y - SelectedCurveControl.path[SelectedCurveControlIndex].P.Y;
                                SelectedCurveControl.disFirst[SelectedCurveControlIndex] = new PointF(x, y);
                                if (Moving.Curve.type[SelectedCurveControlIndex] == 0)
                                {
                                    SelectedCurveControl.disSecond[SelectedCurveControlIndex] = new PointF(-1 * x, -1 * y);
                                }
                                else if(Moving.Curve.type[SelectedCurveControlIndex] == 1)
                                {
                                    float newl = (float)Math.Sqrt(x * x + y * y);
                                    PointF t = Moving.Curve.disSecond[SelectedCurveControlIndex];
                                    float oldl = (float)Math.Sqrt(t.X * t.X + t.Y * t.Y);
                                    SelectedCurveControl.disSecond[SelectedCurveControlIndex] = new PointF(-1 * x * oldl / newl, -1 * y * oldl / newl);
                                }
                            }
                            else
                            {
                                float x = e.X - SelectedCurveControl.path[SelectedCurveControlIndex].P.X;
                                float y = e.Y - SelectedCurveControl.path[SelectedCurveControlIndex].P.Y;
                                SelectedCurveControl.disSecond[SelectedCurveControlIndex] = new PointF(x, y);
                                if (Moving.Curve.type[SelectedCurveControlIndex] == 0)
                                {
                                    SelectedCurveControl.disFirst[SelectedCurveControlIndex] = new PointF(-1 * x, -1 * y);
                                }
                                else if (Moving.Curve.type[SelectedCurveControlIndex] == 1)
                                {
                                    float newl = (float)Math.Sqrt(x * x + y * y);
                                    PointF t = Moving.Curve.disFirst[SelectedCurveControlIndex];
                                    float oldl = (float)Math.Sqrt(t.X * t.X + t.Y * t.Y);
                                    SelectedCurveControl.disFirst[SelectedCurveControlIndex] = new PointF(-1 * x * oldl / newl, -1 * y * oldl / newl);
                                }
                            }
                        }
                        else if (Moving.type == 6)
                        {
                            for (int i = 0; i < Moving.Group.P.Count; i++)
                            {
                                Moving.Group.P[i].P.X = Moving.Path[i].P.X + e.X - Moving.StartMoveMousePoint.X;
                                Moving.Group.P[i].P.Y = Moving.Path[i].P.Y + e.Y - Moving.StartMoveMousePoint.Y;
                            }
                        }
                    }
                }
                For_Paint = e.Location;
                RefreshSelection(e.Location);
                pictureBox1.Refresh();
            }
        }
        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            is_Drowing = false;
            previous_point = null;
            if (Mouse_Mode == 3)
            {
                if (PreviousCruve.path.Count != 1)
                {
                    GraphCurve c = CurveToBez(PreviousCruve.path);
                    List<int> t = new List<int>();
                    for (int i = 0; i < c.path.Count; i++)
                    {
                        c.path[i].Relative++;
                        t.Add(0);
                    }
                    c.type = t;
                    CurveList.Add(c);
                }
                Push_Undo_Data();
                PreviousCruve = new GraphCurve();
                PointCombine();
            }
            PointsList.RemoveAll(x => x.Relative == 0);
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (Moving != null && Mouse_Mode == 1)
            {
                if (Moving.type == 5)
                {
                    MutiSelect(e);
                }
                else if (Moving.type == 1 && Moving.StartMoveMousePoint == e.Location)
                {
                    if (Ctrl && SelectedGroup != null)
                    {
                        if (SelectedGroup.L.FindIndex(a => a == Moving.Line) < 0)
                        {
                            if (GroupList.FindIndex(a => a == SelectedGroup) >= 0)
                            {
                                int t = GroupList.FindIndex(a => a == SelectedGroup);
                                SelectedGroup = new GraphGroup();
                                SelectedGroup.G.Add(GroupList[t]);
                                foreach (var g in SelectedGroup.G)
                                {
                                    foreach (var l in g.L)
                                    {
                                        SelectedGroup.L.Add(l);
                                    }
                                    foreach (var c in CurveList)
                                    {
                                        SelectedGroup.C.Add(c);
                                    }
                                    for (int i = 0; i < g.P.Count; i++)
                                    {
                                        if (SelectedGroup.P.FindIndex(a => a == g.P[i]) < 0)
                                            SelectedGroup.P.Add(g.P[i]);
                                    }
                                }
                            }
                            SelectedGroup.L.Add(Moving.Line);
                            if (SelectedGroup.P.FindIndex(a => a == Moving.Line.StartPoint) < 0)
                                SelectedGroup.P.Add(Moving.Line.StartPoint);
                            if (SelectedGroup.P.FindIndex(a => a == Moving.Line.EndPoint) < 0)
                                SelectedGroup.P.Add(Moving.Line.EndPoint);
                        }
                    }
                    else
                    {
                        SelectedGroup = new GraphGroup();
                        SelectedGroup.L.Add(Moving.Line);
                        SelectedGroup.P.Add(Moving.Line.StartPoint);
                        SelectedGroup.P.Add(Moving.Line.EndPoint);
                    }
                }
                else if (Moving.type == 3 && Moving.StartMoveMousePoint == e.Location)
                {
                    if (Ctrl && SelectedGroup != null)
                    {
                        if (SelectedGroup.C.FindIndex(a => a == Moving.Curve) < 0)
                        {
                            if (GroupList.FindIndex(a => a == SelectedGroup) >= 0)
                            {
                                int t = GroupList.FindIndex(a => a == SelectedGroup);
                                SelectedGroup = new GraphGroup();
                                SelectedGroup.G.Add(GroupList[t]);
                                foreach (var g in SelectedGroup.G)
                                {
                                    foreach (var l in g.L)
                                    {
                                        SelectedGroup.L.Add(l);
                                    }
                                    foreach (var c in g.C)
                                    {
                                        SelectedGroup.C.Add(c);
                                    }
                                    for (int i = 0; i < g.P.Count; i++)
                                    {
                                        if (SelectedGroup.P.FindIndex(a => a == g.P[i]) < 0)
                                            SelectedGroup.P.Add(g.P[i]);
                                    }
                                }
                            }
                            SelectedGroup.C.Add(Moving.Curve);
                            for(int i = 0; i < Moving.Curve.path.Count; i++)
                            {
                                if (SelectedGroup.P.FindIndex(a => a == Moving.Curve.path[i]) < 0)
                                {
                                    SelectedGroup.P.Add(Moving.Curve.path[i]);
                                }
                            }
                        }
                    }
                    else
                    {
                        SelectedGroup = new GraphGroup();
                        SelectedGroup.C.Add(Moving.Curve);
                        for (int i = 0; i < Moving.Curve.path.Count; i++)
                        {
                            if (SelectedGroup.P.FindIndex(a => a == Moving.Curve.path[i]) < 0)
                            {
                                SelectedGroup.P.Add(Moving.Curve.path[i]);
                            }
                        }
                    }
                }
                else if (Moving.type == 6 && Moving.StartMoveMousePoint == e.Location)
                {
                    if (Moving.Group != SelectedGroup)
                    {
                        if (Ctrl && SelectedGroup != null)
                        {
                            if (GroupList.FindIndex(a => a == SelectedGroup) >= 0)
                            {
                                int t = GroupList.FindIndex(a => a == SelectedGroup);
                                SelectedGroup = new GraphGroup();
                                SelectedGroup.G.Add(GroupList[t]);
                                foreach (var g in SelectedGroup.G)
                                {
                                    foreach (var l in g.L)
                                    {
                                        SelectedGroup.L.Add(l);
                                    }
                                    foreach (var c in g.C)
                                    {
                                        SelectedGroup.C.Add(c);
                                    }
                                    for (int i = 0; i < g.P.Count; i++)
                                    {
                                        if (SelectedGroup.P.FindIndex(a => a == g.P[i]) < 0)
                                            SelectedGroup.P.Add(g.P[i]);
                                    }
                                }
                            }
                            SelectedGroup.G.Add(Moving.Group);
                            foreach (var l in Moving.Group.L)
                            {
                                SelectedGroup.L.Add(l);
                            }
                            foreach (var c in Moving.Group.C)
                            {
                                SelectedGroup.C.Add(c);
                            }
                            foreach (var g in Moving.Group.G)
                            {
                                SelectedGroup.G.Add(g);
                            }
                            for (int i = 0; i < Moving.Group.P.Count; i++)
                            {
                                if (SelectedGroup.P.FindIndex(a => a == Moving.Group.P[i]) < 0)
                                    SelectedGroup.P.Add(Moving.Group.P[i]);
                            }
                        }
                        else
                        {
                            SelectedGroup = Moving.Group;
                        }
                    }
                }
                else
                {
                    Push_Undo_Data();
                }
                PointCombine();
                this.Capture = false;
                Moving = null;
            }
            RefreshSelection(e.Location);
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            RefreshSelection(e.Location);
            if (e.Button == MouseButtons.Left && Mouse_Mode == 1)
            {
                MBL_M1_DOWN(e);
            }
            if (e.Button == MouseButtons.Left && Mouse_Mode == 2)
            {
                MBL_M2_DOWN(e);
            }
            if (e.Button == MouseButtons.Left && Mouse_Mode == 3)
            {
                MBL_M3_DOWN(e);
            }
            if (e.Button == MouseButtons.Right && Mouse_Mode == 1)
            {
                MBR_M1_DOWN(e);
            }
            if (e.Button == MouseButtons.Right && (Mouse_Mode == 2 || Mouse_Mode == 3))
            {
                pictureBox1_MouseDoubleClick(sender, e);
            }
            RefreshSelection(e.Location);
        }
        private void MBL_M1_DOWN(MouseEventArgs e)
        {
            bool in_Group = false;
            int in_List = -1;
            if (this.SelectedPoint != null && Moving == null)
            {
                if (SelectedGroup != null)
                    if (SelectedGroup.P.FindIndex(a => a == SelectedPoint) >= 0)
                        in_Group = true;
                foreach(var g in GroupList)
                {
                    if (g.P.FindIndex(a => a == SelectedPoint) >= 0)
                        in_List = GroupList.FindIndex(a => a == g);
                }
                //this.Capture = true;
                Moving = new MoveInfo
                {
                    StartLinePoint = SelectedPoint,
                    type = 2
                };
            }
            else if (this.SelectedLine != null && Moving == null)
            {
                if (SelectedGroup != null)
                    if (SelectedGroup.L.FindIndex(a => a == SelectedLine) >= 0)
                        in_Group = true;
                foreach (var g in GroupList)
                {
                    if (g.L.FindIndex(a => a == SelectedLine) >= 0)
                        in_List = GroupList.FindIndex(a => a == g);
                }
                GraphPoint selectS = new GraphPoint(SelectedLine.StartPoint.P.X, SelectedLine.StartPoint.P.Y);
                GraphPoint selectE = new GraphPoint(SelectedLine.EndPoint.P.X, SelectedLine.EndPoint.P.Y);
                //this.Capture = true;
                Moving = new MoveInfo
                {
                    Line = this.SelectedLine,
                    StartLinePoint = selectS,
                    EndLinePoint = selectE,
                    StartMoveMousePoint = e.Location,
                    type = 1
                };
            }
            else if (this.SelectedCurve != null && Moving == null && this.SelectedCurveControlIndex == -1)
            {
                if (SelectedGroup != null)
                    if (SelectedGroup.C.FindIndex(a => a == SelectedCurve) >= 0)
                        in_Group = true;
                foreach (var g in GroupList)
                {
                    if (g.C.FindIndex(a => a == SelectedCurve) >= 0)
                        in_List = GroupList.FindIndex(a => a == g);
                }
                List<GraphPoint> path = new List<GraphPoint>();
                for(int i = 0; i < SelectedCurve.path.Count; i++)
                {
                    path.Add(new GraphPoint(SelectedCurve.path[i].P.X, SelectedCurve.path[i].P.Y));
                }
                Moving = new MoveInfo
                {
                    Curve = this.SelectedCurve,
                    StartMoveMousePoint = e.Location,
                    Path = path,
                    type = 3
                };
            }
            else if (this.SelectedCurveControl != null && Moving == null && this.SelectedCurveControlIndex != -1)
            {
                Moving = new MoveInfo
                {
                    Curve = this.SelectedCurveControl,
                    CurveIndex = this.SelectedCurveControlIndex,
                    CurveFS = this.SeletedCurveControlFS,
                    type = 4
                };
            }
            else
            {
                Moving = new MoveInfo
                {
                    StartMoveMousePoint = e.Location,
                    type = 5
                };
            }
            if (in_Group)
            {
                List<GraphPoint> path = new List<GraphPoint>();
                for (int i = 0; i < SelectedGroup.P.Count; i++)
                {
                    path.Add(new GraphPoint(SelectedGroup.P[i].P.X, SelectedGroup.P[i].P.Y));
                }
                Moving = new MoveInfo
                {
                    StartMoveMousePoint = e.Location,
                    Group = SelectedGroup,
                    Path = path,
                    type = 6
                };
            }
            else if (in_List >= 0)
            {
                List<GraphPoint> path = new List<GraphPoint>();
                for (int i = 0; i < GroupList[in_List].P.Count; i++)
                {
                    path.Add(new GraphPoint(GroupList[in_List].P[i].P.X, GroupList[in_List].P[i].P.Y));
                }
                Moving = new MoveInfo
                {
                    StartMoveMousePoint = e.Location,
                    Group = GroupList[in_List],
                    Path = path,
                    type = 6
                };
            }
        }
        private void MBL_M2_DOWN(MouseEventArgs e)
        {
            GraphPoint nowp = new GraphPoint(e.X, e.Y);
            GraphPoint t = FindPointByPoint(PointsList, e.Location);
            if (t == null)
            {
                if (Net_Mode && previous_point != null)
                {
                    float x = nowp.P.X + SizeOfNet / 2;
                    float y = nowp.P.Y + SizeOfNet / 2;
                    x -= x % SizeOfNet;
                    y -= y % SizeOfNet;
                    if (previous_point.P.X == x && previous_point.P.Y == y)
                        return;
                }
                if (Net_Mode)
                {
                    nowp.P.X += SizeOfNet / 2;
                    nowp.P.Y += SizeOfNet / 2;
                    nowp.P.X -= nowp.P.X % SizeOfNet;
                    nowp.P.Y -= nowp.P.Y % SizeOfNet;
                }
                PointsList.Add(nowp);
                if (is_Drowing)
                {
                    GraphLine temp = new GraphLine(previous_point, nowp);
                    LineList.Add(temp);
                    previous_point.Relative++;
                    nowp.Relative++;
                    Push_Undo_Data();
                }
                previous_point = nowp;
                is_Drowing = true;
            }
            else
            {
                if (previous_point == t)
                    return;
                if (is_Drowing)
                {
                    GraphLine temp = new GraphLine(previous_point, t);
                    LineList.Add(temp);
                    previous_point.Relative++;
                    t.Relative++;
                    Push_Undo_Data();
                }
                previous_point = t;
                is_Drowing = true;
            }
        }
        private void MBL_M3_DOWN(MouseEventArgs e)
        {
            GraphPoint nowp = new GraphPoint(e.X, e.Y);
            GraphPoint t = FindPointByPoint(PointsList, e.Location);
            if (t == null)
            {
                if (Net_Mode && previous_point!=null)
                {
                    float x = nowp.P.X + SizeOfNet / 2;
                    float y = nowp.P.Y + SizeOfNet / 2;
                    x -= x % SizeOfNet;
                    y -= y % SizeOfNet;
                    if (previous_point.P.X == x && previous_point.P.Y == y)
                        return;
                }
                if (Net_Mode)
                {
                    nowp.P.X += SizeOfNet / 2;
                    nowp.P.Y += SizeOfNet / 2;
                    nowp.P.X -= nowp.P.X % SizeOfNet;
                    nowp.P.Y -= nowp.P.Y % SizeOfNet;
                }
                PointsList.Add(nowp);
                PreviousCruve.path.Add(nowp);
                previous_point = nowp;
                is_Drowing = true;
            }
            else
            {
                if (previous_point == t)
                    return;
                PreviousCruve.path.Add(t);
                previous_point = t;
                is_Drowing = true;
            }
        }
        private void MBR_M1_DOWN(MouseEventArgs e)
        {
            Right_Temp_Line = null;
            Right_Temp_Point = null;
            Right_Temp_Curve = null;
            Right_Temp_Curve_Index = -1;
            Right_Temp_Group = null;
            bool incurve = false;
            GraphCurve tc = null;
            int tcindex = -1;
            foreach (var c in CurveList)
            {
                if (c.path.Exists(x => x == SelectedPoint))
                {
                    incurve = true;
                    tc = c;
                    tcindex = c.path.FindIndex(x => x == SelectedPoint);
                }
            }
            if (incurve)
            {
                if (tc.type[tcindex] == 0)
                {
                    平滑ToolStripMenuItem.Checked = true;
                    拉直ToolStripMenuItem.Checked = false;
                    無ToolStripMenuItem.Checked = false;
                }
                else if (tc.type[tcindex] == 1)
                {
                    平滑ToolStripMenuItem.Checked = false;
                    拉直ToolStripMenuItem.Checked = true;
                    無ToolStripMenuItem.Checked = false;
                }
                else
                {
                    平滑ToolStripMenuItem.Checked = false;
                    拉直ToolStripMenuItem.Checked = false;
                    無ToolStripMenuItem.Checked = true;
                }
                曲線樣式ToolStripMenuItem.Visible = true;
            }
            else
                曲線樣式ToolStripMenuItem.Visible = false;
            if (SelectedGroup != null)
            {
                Right_Temp_Group = SelectedGroup;
                int a = SelectedGroup.L.Count + SelectedGroup.C.Count;
                群組ToolStripMenuItem.Enabled = a > 1 || SelectedGroup.G.Count > 0 ? true : false;
                組成群組ToolStripMenuItem.Enabled = a > 1 ? true : false;
                取消群組ToolStripMenuItem.Enabled = GroupList.FindIndex(b => b == SelectedGroup) >= 0 ? true : false;
            }
            else
                群組ToolStripMenuItem.Enabled = false;
            
            if (SelectedPoint != null)
            {
                Right_Temp_Point = SelectedPoint;
                新增節點ToolStripMenuItem.Enabled = false;
                toolStripMenuItem2.Text = "刪除節點";
                Right_Temp_Mouse_Pos = e.Location;
                contextMenuStrip1.Show(MousePosition);
            }
            else if (SelectedLine != null)
            {
                Right_Temp_Line = SelectedLine;
                新增節點ToolStripMenuItem.Enabled = true;
                toolStripMenuItem2.Text = "刪除直線";
                Right_Temp_Mouse_Pos = e.Location;
                contextMenuStrip1.Show(MousePosition);
            }
            else if (SelectedCurve != null)
            {
                新增節點ToolStripMenuItem.Enabled = true;
                Right_Temp_Curve = SelectedCurve;
                Right_Temp_Curve_Index = SelectedCurveIndex;
                toolStripMenuItem2.Text = "刪除整條曲線";
                Right_Temp_Mouse_Pos = e.Location;
                contextMenuStrip1.Show(MousePosition);
            }
            else
            {
                Right_Temp_Line = null;
                Right_Temp_Point = null;
                Right_Temp_Curve = null;
                Right_Temp_Curve_Index = -1;
            }
        }
        private void MutiSelect(MouseEventArgs e)
        {
            SelectedGroup = new GraphGroup();
            float x = Moving.StartMoveMousePoint.X < e.X ? Moving.StartMoveMousePoint.X : e.X;
            float y = Moving.StartMoveMousePoint.Y < e.Y ? Moving.StartMoveMousePoint.Y : e.Y;
            float width = Math.Abs(Moving.StartMoveMousePoint.X - e.X);
            float height = Math.Abs(Moving.StartMoveMousePoint.Y - e.Y);
            foreach (var l in LineList)
            {
                if (l.StartPoint.P.X > x && l.StartPoint.P.Y > y && l.EndPoint.P.X > x && l.EndPoint.P.Y > y)
                    if (l.StartPoint.P.X - x < width && l.StartPoint.P.Y - y < height && l.EndPoint.P.X - x < width && l.EndPoint.P.Y - y < height)
                    {
                        bool inG = false;
                        foreach (var g in GroupList)
                        {
                            if (g.L.FindIndex(a => a == l) >= 0)
                                inG = true;
                        }
                        if (!inG)
                        {
                            SelectedGroup.L.Add(l);
                            if (SelectedGroup.P.FindIndex(a => a == l.StartPoint) < 0)
                                SelectedGroup.P.Add(l.StartPoint);
                            if (SelectedGroup.P.FindIndex(a => a == l.EndPoint) < 0)
                                SelectedGroup.P.Add(l.EndPoint);
                        }
                    }
            }
            foreach (var c in CurveList)
            {
                bool outrange = false;
                for (int i = 0; i < c.path.Count; i++)
                {
                    if (!(c.path[i].P.X > x && c.path[i].P.Y > y && c.path[i].P.X - x < width && c.path[i].P.Y - y < height))
                    {
                        outrange = true;
                    }
                }
                if (!outrange)
                {
                    bool inG = false;
                    foreach (var g in GroupList)
                    {
                        if (g.C.FindIndex(a => a == c) >= 0)
                            inG = true;
                    }
                    if (!inG)
                    {
                        SelectedGroup.C.Add(c);
                        for(int i = 0; i < c.path.Count; i++)
                        {
                            if (SelectedGroup.P.FindIndex(a => a == c.path[i]) < 0)
                                SelectedGroup.P.Add(c.path[i]);
                        }
                    }
                        
                }
            }
            foreach (var g in GroupList)
            {
                bool outrange = false;
                foreach (var l in g.L)
                {
                    if (l.StartPoint.P.X > x && l.StartPoint.P.Y > y && l.EndPoint.P.X > x && l.EndPoint.P.Y > y)
                        if (l.StartPoint.P.X - x < width && l.StartPoint.P.Y - y < height && l.EndPoint.P.X - x < width && l.EndPoint.P.Y - y < height)
                            ;
                        else
                            outrange = true;
                    else
                        outrange = true;
                }
                foreach (var c in g.C)
                {
                    for (int i = 0; i < c.path.Count; i++)
                    {
                        if (c.path[i].P.X > x && c.path[i].P.Y > y && c.path[i].P.X - x < width && c.path[i].P.Y - y < height)
                            ;
                        else
                            outrange = true;
                    }
                }
                if (!outrange)
                {
                    SelectedGroup.G.Add(g);
                }
            }
            if (SelectedGroup.L.Count == 0 && SelectedGroup.C.Count == 0 && SelectedGroup.G.Count == 1)
            {
                GraphGroup t = SelectedGroup.G[0];
                SelectedGroup = t;
            }
            else
            {
                foreach(var g in SelectedGroup.G)
                {
                    foreach (var l in g.L)
                    {
                        SelectedGroup.L.Add(l);
                    }
                    foreach (var c in g.C)
                    {
                        SelectedGroup.C.Add(c);
                    }
                    for (int i = 0; i < g.P.Count; i++)
                    {
                        if (SelectedGroup.P.FindIndex(a => a == g.P[i]) < 0)
                            SelectedGroup.P.Add(g.P[i]);
                    }
                }
            }
            if (SelectedGroup.L.Count == 0 && SelectedGroup.C.Count == 0 && SelectedGroup.G.Count == 0)
                SelectedGroup = null;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)//游標
        {
            Mouse_Mode = 1;
            is_Drowing = false;
            previous_point = null;
            PreviousCruve = new GraphCurve();
            Cursor = Cursors.Default;
            toolStripButton1.Checked = true;
            toolStripButton2.Checked = false;
            toolStripButton3.Checked = false;
        }
        private void toolStripButton2_Click(object sender, EventArgs e)//鉛筆
        {
            Mouse_Mode = 2;
            previous_point = null;
            SelectedGroup = null;
            PreviousCruve = new GraphCurve();
            Cursor = Cursors.Cross;
            toolStripButton1.Checked = false;
            toolStripButton2.Checked = true;
            toolStripButton3.Checked = false;
            Refresh();
        }
        private void toolStripButton3_Click(object sender, EventArgs e)//曲線
        {
            Mouse_Mode = 3;
            previous_point = null;
            SelectedGroup = null;
            PreviousCruve = new GraphCurve();
            Cursor = Cursors.Cross;
            toolStripButton1.Checked = false;
            toolStripButton2.Checked = false;
            toolStripButton3.Checked = true;
            Refresh();
        }
        private void toolStripButton4_Click(object sender, EventArgs e)//網格
        {
            if (Net_Mode)
            {
                Net_Mode = false;
                toolStripButton4.Checked = false;
                toolStripTextBox1.Enabled = false;
            }
            else
            {
                Net_Mode = true;
                toolStripButton4.Checked = true;
                toolStripTextBox1.Enabled = true;
                toolStripTextBox1.Text = SizeOfNet + "";
            }
            Refresh();
        }
        private void toolStripButton5_Click(object sender, EventArgs e)//Undo
        {
            TabpageDataList[tabControl1.SelectedIndex].Undo_index--;
            int Undo_Index = TabpageDataList[tabControl1.SelectedIndex].Undo_index;
            TabpageData t = new TabpageData();
            foreach (var p in Undo_Data[Undo_Index].PL)
            {
                GraphPoint gp = new GraphPoint(p.P.X, p.P.Y);
                t.PL.Add(gp);
            }
            foreach (var l in Undo_Data[Undo_Index].LL)
            {
                int s = t.PL.FindIndex(x => x.P.X == l.StartPoint.P.X && x.P.Y == l.StartPoint.P.Y);
                int end = t.PL.FindIndex(x => x.P.X == l.EndPoint.P.X && x.P.Y == l.EndPoint.P.Y);
                t.PL[s].Relative++;
                t.PL[end].Relative++;
                t.LL.Add(new GraphLine(t.PL[s], t.PL[end]));
            }
            foreach (var c in Undo_Data[Undo_Index].CL)
            {
                GraphCurve gc = new GraphCurve();
                for (int i = 0; i < c.path.Count; i++)
                {
                    int index = t.PL.FindIndex(x => x.P.X == c.path[i].P.X && x.P.Y == c.path[i].P.Y);
                    gc.path.Add(t.PL[index]);
                    t.PL[index].Relative++;
                    gc.disFirst.Add(new PointF(c.disFirst[i].X, c.disFirst[i].Y));
                    gc.disSecond.Add(new PointF(c.disSecond[i].X, c.disSecond[i].Y));
                    gc.type.Add(c.type[i]);
                }
                t.CL.Add(gc);
            }
            foreach (var g in Undo_Data[Undo_Index].GL)
            {
                t.GL.Add(Group_Copy(g, t));
            }
            t.TabpageName = TabpageDataList[tabControl1.SelectedIndex].TabpageName;
            t.Undo = Undo_Data;
            TabpageDataList[tabControl1.SelectedIndex] = t;
            tabControl1_SelectedIndexChanged(new object(), new EventArgs());
            TabpageDataList[tabControl1.SelectedIndex].Undo_index = Undo_Index;
            RefreshUndoCheck();
            Refresh();
        }
        private void toolStripButton6_Click(object sender, EventArgs e)//Redo
        {
            TabpageDataList[tabControl1.SelectedIndex].Undo_index++;
            int Undo_Index = TabpageDataList[tabControl1.SelectedIndex].Undo_index;
            TabpageData t = new TabpageData();
            foreach (var p in Undo_Data[Undo_Index].PL)
            {
                GraphPoint gp = new GraphPoint(p.P.X, p.P.Y);
                t.PL.Add(gp);
            }
            foreach (var l in Undo_Data[Undo_Index].LL)
            {
                int s = t.PL.FindIndex(x => x.P.X == l.StartPoint.P.X && x.P.Y == l.StartPoint.P.Y);
                int end = t.PL.FindIndex(x => x.P.X == l.EndPoint.P.X && x.P.Y == l.EndPoint.P.Y);
                t.PL[s].Relative++;
                t.PL[end].Relative++;
                t.LL.Add(new GraphLine(t.PL[s], t.PL[end]));
            }
            foreach (var c in Undo_Data[Undo_Index].CL)
            {
                GraphCurve gc = new GraphCurve();
                for (int i = 0; i < c.path.Count; i++)
                {
                    int index = t.PL.FindIndex(x => x.P.X == c.path[i].P.X && x.P.Y == c.path[i].P.Y);
                    gc.path.Add(t.PL[index]);
                    t.PL[index].Relative++;
                    gc.disFirst.Add(new PointF(c.disFirst[i].X, c.disFirst[i].Y));
                    gc.disSecond.Add(new PointF(c.disSecond[i].X, c.disSecond[i].Y));
                    gc.type.Add(c.type[i]);
                }
                t.CL.Add(gc);
            }
            foreach (var g in Undo_Data[Undo_Index].GL)
            {
                t.GL.Add(Group_Copy(g, t));
            }
            t.TabpageName = TabpageDataList[tabControl1.SelectedIndex].TabpageName;
            t.Undo = Undo_Data;
            TabpageDataList[tabControl1.SelectedIndex] = t;
            tabControl1_SelectedIndexChanged(new object(), new EventArgs());
            TabpageDataList[tabControl1.SelectedIndex].Undo_index = Undo_Index;
            RefreshUndoCheck();
            Refresh();
        }
        private void toolStripTextBox1_Leave(object sender, EventArgs e)
        {
            int t;
            if (Int32.TryParse(toolStripTextBox1.Text, out t))
            {
                SizeOfNet = t;
            }
            else
            {
                toolStripTextBox1.Text = SizeOfNet + "";
            }
        }
        private void RefreshUndoCheck()
        {
            if (TabpageDataList[tabControl1.SelectedIndex].Undo_index == 0)
                toolStripButton5.Enabled = false;
            else
                toolStripButton5.Enabled = true;
            if (TabpageDataList[tabControl1.SelectedIndex].Undo_index == Undo_Data.Count - 1)
                toolStripButton6.Enabled = false;
            else
                toolStripButton6.Enabled = true;
        }

        private List<TabpageData> Undo_Data = new List<TabpageData>();
        private void Push_Undo_Data()
        {
            TabpageData t = new TabpageData();
            foreach(var p in PointsList)
            {
                GraphPoint gp = new GraphPoint(p.P.X, p.P.Y);
                t.PL.Add(gp);
            }
            foreach(var l in LineList)
            {
                int s = t.PL.FindIndex(x => x.P.X == l.StartPoint.P.X && x.P.Y == l.StartPoint.P.Y);
                int e = t.PL.FindIndex(x => x.P.X == l.EndPoint.P.X && x.P.Y == l.EndPoint.P.Y);
                t.PL[s].Relative++;
                t.PL[e].Relative++;
                t.LL.Add(new GraphLine(t.PL[s], t.PL[e]));
            }
            foreach(var c in CurveList)
            {
                GraphCurve gc = new GraphCurve();
                for(int i = 0; i < c.path.Count; i++)
                {
                    int index = t.PL.FindIndex(x => x.P.X == c.path[i].P.X && x.P.Y == c.path[i].P.Y);
                    gc.path.Add(t.PL[index]);
                    t.PL[index].Relative++;
                    gc.disFirst.Add(new PointF(c.disFirst[i].X, c.disFirst[i].Y));
                    gc.disSecond.Add(new PointF(c.disSecond[i].X, c.disSecond[i].Y));
                    gc.type.Add(c.type[i]);
                }
                t.CL.Add(gc);
            }
            foreach(var g in GroupList)
            {
                t.GL.Add(Group_Copy(g, t));
            }
            for(int i = Undo_Data.Count - 1; i > TabpageDataList[tabControl1.SelectedIndex].Undo_index; i--)
            {
                Undo_Data.RemoveAt(i);
            }
            Undo_Data.Add(t);
            TabpageDataList[tabControl1.SelectedIndex].Undo_index++;
            RefreshUndoCheck();
        }
        private GraphGroup Group_Copy(GraphGroup PreG, TabpageData PreT)
        {
            GraphGroup gg = new GraphGroup();
            foreach(var p in PreG.P)
            {
                gg.P.Add(PreT.PL.Find(x => x.P.X == p.P.X && x.P.Y == p.P.Y));
            }
            foreach(var l in PreG.L)
            {
                gg.L.Add(PreT.LL.Find(x => x.StartPoint.P.X == l.StartPoint.P.X && x.StartPoint.P.Y == l.StartPoint.P.Y
                                        && x.EndPoint.P.X == l.EndPoint.P.X && x.EndPoint.P.Y == l.EndPoint.P.Y));
            }
            foreach(var c in PreG.C)
            {
                gg.C.Add(PreT.CL.Find(x => x.equal(c)));
            }
            foreach(var g in PreG.G)
            {
                gg.G.Add(Group_Copy(g, PreT));
            }
            return gg;
        }
        private void RefreshSelection(Point point)
        {
            var selectedLine = FindLineByPoint(LineList, point);
            var selectedPoint = FindPointByPoint(PointsList, point);
            GraphCurve selectedCurve;
            int selectedCurveIndex;
            FindCurveByPoint(CurveList, point, out selectedCurve, out selectedCurveIndex);
            int selectedcurveindex, selectedcurvefs;
            GraphCurve selectedcurvecontrol;
            FindCurveControlByPoint(CurveList, point, out selectedcurvecontrol, out selectedcurveindex, out selectedcurvefs);
            if (selectedPoint != this.SelectedPoint)
            {
                this.SelectedPoint = selectedPoint;
                this.SelectedLine = null;
                this.SelectedCurve = null;
                this.Invalidate();
            }
            else if (selectedLine != this.SelectedLine && SelectedPoint == null && Mouse_Mode == 1)
            {
                this.SelectedLine = selectedLine;
                this.Invalidate();
            }
            else if (selectedCurve != this.SelectedCurve && Mouse_Mode == 1 && SelectedPoint == null)
            {
                this.SelectedCurve = selectedCurve;
                this.SelectedCurveIndex = selectedCurveIndex;
                this.Invalidate();
            }
            else if (selectedcurvecontrol != this.SelectedCurveControl && Mouse_Mode == 1)
            {
                this.SelectedCurveControl = selectedcurvecontrol;
                this.SelectedCurveControlIndex = selectedcurveindex;
                this.SeletedCurveControlFS = selectedcurvefs;
                this.Invalidate();
            }
            if (Moving != null)
                this.Invalidate();

            this.Cursor =
                Moving != null ? Cursors.Hand :
                SelectedLine != null ? Cursors.SizeAll :
                SelectedPoint != null && Mouse_Mode == 1 ? Cursors.SizeAll :
                SelectedCurve != null ? Cursors.SizeAll :
                SelectedCurveControl != null ? Cursors.SizeAll :
                Mouse_Mode == 2 || Mouse_Mode == 3  ? Cursors.Cross :
                    Cursors.Default;

        }

        private void ImgFitSize()
        {
            if (pictureBox1 != null)
            {
                pictureBox1.Width = pictureBox1.Image.Width;
                pictureBox1.Height = pictureBox1.Image.Height;
            }
        }
        static private List<PointF> GraphCurveToBez(GraphCurve c)
        {
            List<PointF> t = new List<PointF>();
            t.Add(c.path[0].P);
            t.Add(new PointF(c.path[0].P.X + c.disSecond[0].X, c.path[0].P.Y + c.disSecond[0].Y));
            for(int i = 1; i < c.path.Count-1; i++)
            {
                t.Add(new PointF(c.path[i].P.X + c.disFirst[i].X, c.path[i].P.Y + c.disFirst[i].Y));
                t.Add(c.path[i].P);
                t.Add(new PointF(c.path[i].P.X + c.disSecond[i].X, c.path[i].P.Y + c.disSecond[i].Y));
            }
            int a = c.path.Count;
            t.Add(new PointF(c.path[a - 1].P.X + c.disFirst[a - 1].X, c.path[a - 1].P.Y + c.disFirst[a - 1].Y));
            t.Add(c.path[a-1].P);
            return t;
        }
        private GraphCurve CurveToBez(List<GraphPoint> path, int type = 0, Point p = new Point())
        {
            if (type == 0 && path.Count > 2)
            {
                List<GraphPoint> t = new List<GraphPoint>();
                GraphCurve pr = new GraphCurve();
                pr.path = t;
                t.Add(path[0]);
                for (int i = 1; i < path.Count - 1; i++)
                {
                    float disX = (path[i + 1].P.X - path[i - 1].P.X) / 6;
                    float disY = (path[i + 1].P.Y - path[i - 1].P.Y) / 6;
                    pr.disFirst.Add(new PointF(disX * -1, disY * -1));
                    pr.disSecond.Add(new PointF(disX, disY));
                    t.Add(path[i]);
                }
                t.Add(path[path.Count - 1]);
                int a = t.Count;
                int b = pr.disFirst.Count;
                float disFX = (path[1].P.X + pr.disFirst[0].X - path[0].P.X) / 2;
                float disFY = (path[1].P.Y + pr.disFirst[0].Y - path[0].P.Y) / 2;
                float disLX = (path[a - 1].P.X - path[a - 2].P.X - pr.disSecond[b - 1].X) / 2;
                float disLY = (path[a - 1].P.Y - path[a - 2].P.Y - pr.disSecond[b - 1].Y) / 2;
                pr.disFirst.Insert(0, new PointF(disFX * -1, disFY * -1));
                pr.disSecond.Insert(0, new PointF(disFX, disFY));
                pr.disFirst.Add(new PointF(disLX * -1, disLY * -1));
                pr.disSecond.Add(new PointF(disLX, disLY));
                return pr;
            }
            else if(type == 0 && path.Count == 2)
            {
                List<GraphPoint> t = new List<GraphPoint>();
                GraphCurve pr = new GraphCurve();
                pr.path = t;
                t.Add(path[0]);
                t.Add(path[1]);
                float disX = (path[1].P.X - path[0].P.X) / 3;
                float disY = (path[1].P.Y - path[0].P.Y) / 3;
                pr.disFirst.Add(new PointF(-1 * disX, -1 * disY));
                pr.disFirst.Add(new PointF(-1 * disX, -1 * disY));
                pr.disSecond.Add(new PointF(disX, disY));
                pr.disSecond.Add(new PointF(disX, disY));
                return pr;
            }
            else if(path.Count == 1)
            {
                List<GraphPoint> t = new List<GraphPoint>();
                GraphCurve pr = new GraphCurve();
                pr.path = t;
                t.Add(path[0]);
                t.Add(new GraphPoint(p.X, p.Y));
                pr.disFirst.Add(new PointF(0, 0));
                pr.disFirst.Add(new PointF(0, 0));
                pr.disSecond.Add(new PointF(0, 0));
                pr.disSecond.Add(new PointF(0, 0));
                return pr;
            }
            else
            {
                List<GraphPoint> t = new List<GraphPoint>();
                GraphCurve pr = new GraphCurve();
                pr.path = t;
                for(int i = 0; i < path.Count; i++)
                {
                    t.Add(path[i]);
                }
                t.Add(new GraphPoint(p.X, p.Y));
                for (int i = 1; i < t.Count - 1; i++)
                {
                    float disX = (t[i + 1].P.X - t[i - 1].P.X) / 6;
                    float disY = (t[i + 1].P.Y - t[i - 1].P.Y) / 6;
                    pr.disFirst.Add(new PointF(disX * -1, disY * -1));
                    pr.disSecond.Add(new PointF(disX, disY));
                }
                int a = t.Count;
                int b = pr.disFirst.Count;
                float disFX = (t[1].P.X + pr.disFirst[0].X - t[0].P.X) / 2;
                float disFY = (t[1].P.Y + pr.disFirst[0].Y - t[0].P.Y) / 2;
                float disLX = (t[a - 1].P.X - t[a - 2].P.X - pr.disSecond[b - 1].X) / 2;
                float disLY = (t[a - 1].P.Y - t[a - 2].P.Y - pr.disSecond[b - 1].Y) / 2;
                pr.disFirst.Insert(0, new PointF(disFX * -1, disFY * -1));
                pr.disSecond.Insert(0, new PointF(disFX, disFY));
                pr.disFirst.Add(new PointF(disLX * -1, disLY * -1));
                pr.disSecond.Add(new PointF(disLX, disLY));
                return pr;
            }
        }

        GraphPoint Right_Temp_Point = null;
        GraphLine Right_Temp_Line = null;
        GraphCurve Right_Temp_Curve = null;
        int Right_Temp_Curve_Index = -1;
        Point Right_Temp_Mouse_Pos;
        GraphGroup Right_Temp_Group = null;

        private void DeletePoint(GraphPoint toDeleteP)
        {
            List<GraphLine> lt = new List<GraphLine>();
            foreach(var l in LineList)
            {
                if (l.StartPoint == toDeleteP || l.EndPoint == toDeleteP)
                {
                    lt.Add(l);
                }
            }
            foreach(var l in lt)
            {
                DeleteLine(l);
            }
            List<GraphCurve> ct = new List<GraphCurve>();
            foreach(var c in CurveList)
            {
                for(int i = 0; i < c.path.Count; i++)
                {
                    if (c.path[i] == toDeleteP)
                    {
                        if (c.path.Count == 2)
                            ct.Add(c);
                        else
                            DeleteCurveP(c, toDeleteP);
                    }
                }
            }
            foreach(var c in ct)
            {
                DeleteHoleCurve(c);
            }
            PointsList.Remove(toDeleteP);
        }
        private void DeleteLine(GraphLine toDeleteL)
        {
            toDeleteL.EndPoint.Relative--;
            toDeleteL.StartPoint.Relative--;
            LineList.Remove(toDeleteL);
            PointsList.RemoveAll(x => x.Relative == 0);
        }
        private void DeleteCurveP(GraphCurve c, GraphPoint p)
        {
            if (c.path.Count == 2)
            {
                DeleteHoleCurve(c);
            }
            else
            {
                p.Relative--;
                int a = c.path.FindIndex(x => x == p);
                c.path.Remove(p);
                c.disFirst.RemoveAt(a);
                c.disSecond.RemoveAt(a);
            }
        }
        private void DeleteHoleCurve(GraphCurve c)
        {
            for(int i = 0; i < c.path.Count; i++)
            {
                c.path[i].Relative--;
            }
            CurveList.Remove(c);
        }
        private void PointCombine()
        {
            for(int i = 0; i < PointsList.Count; i++)
            {
                List<GraphPoint> t = PointsList.FindAll(x => x.P == PointsList[i].P);
                if (t.Count == 2)
                {
                    foreach(var l in LineList)
                    {
                        if (l.StartPoint == t[1])
                        {
                            l.StartPoint = t[0];
                        }
                        else if(l.EndPoint == t[1])
                        {
                            l.EndPoint = t[0];
                        }
                    }
                    foreach (var c in CurveList)
                    {
                        for(int j = 0; j < c.path.Count; j++)
                        {
                            if (c.path[j] == t[1])
                            {
                                c.path[j] = t[0];
                            }
                        }
                    }
                    t[0].Relative += t[1].Relative;
                    PointsList.Remove(t[1]);
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)//切換分頁
        {
            if (tabControl1.SelectedIndex == TabpagesList.Count - 1)
            {
                TabPage t = new TabPage("new page");

                CurveList = new List<GraphCurve>();
                PointsList = new List<GraphPoint>();
                LineList = new List<GraphLine>();
                GroupList = new List<GraphGroup>();
                Undo_Data = new List<TabpageData>();
                TabpageData a = new TabpageData();
                a.CL = CurveList;
                a.PL = PointsList;
                a.LL = LineList;
                a.GL = GroupList;
                a.Undo = Undo_Data;
                a.TabpageName = t.Text;
                TabpageDataList.Add(a);

                t.Controls.Add(pictureBox1);
                TabpagesList.Insert(TabpagesList.Count - 1, t);
                tabControl1.TabPages.Insert(TabpagesList.Count - 2, t);
                tabControl1.SelectedIndex = TabpagesList.Count - 2;
                TabpagesList[tabControl1.SelectedIndex].Controls.Add(pictureBox1);
                Push_Undo_Data();
                RefreshUndoCheck();
                Invalidate();
            }
            else if(tabControl1.SelectedIndex >= 0)
            {
                TabpagesList[tabControl1.SelectedIndex].Controls.Add(pictureBox1);
                CurveList = TabpageDataList[tabControl1.SelectedIndex].CL;
                PointsList = TabpageDataList[tabControl1.SelectedIndex].PL;
                LineList = TabpageDataList[tabControl1.SelectedIndex].LL;
                GroupList = TabpageDataList[tabControl1.SelectedIndex].GL;
                Undo_Data = TabpageDataList[tabControl1.SelectedIndex].Undo;
                RefreshUndoCheck();
            }

        }

        //ContextMenuStrip1
        private void toolStripMenuItem2_Click(object sender, EventArgs e)//右鍵選單>刪除
        {
            if (Right_Temp_Point != null)
                DeletePoint(Right_Temp_Point);
            else if (Right_Temp_Line != null)
                DeleteLine(Right_Temp_Line);
            else if (Right_Temp_Curve != null)
                DeleteHoleCurve(Right_Temp_Curve);
            ClearRighttemp();
            PointsList.RemoveAll(x => x.Relative == 0);
            Push_Undo_Data();
        }
        private void 新增節點ToolStripMenuItem_Click(object sender, EventArgs e)//右鍵選單>新增節點
        {
            if (Right_Temp_Line != null)
            {
                GraphPoint p = new GraphPoint(Right_Temp_Mouse_Pos.X, Right_Temp_Mouse_Pos.Y);
                PointsList.Add(p);
                GraphLine line1 = new GraphLine(Right_Temp_Line.StartPoint, p);
                LineList.Add(line1);
                Right_Temp_Line.StartPoint.Relative++;
                p.Relative++;
                GraphLine line2 = new GraphLine(p, Right_Temp_Line.EndPoint);
                LineList.Add(line2);
                Right_Temp_Line.EndPoint.Relative++;
                p.Relative++;
                DeleteLine(Right_Temp_Line);
                Push_Undo_Data();
            }
            else if (Right_Temp_Curve != null)
            {
                GraphPoint p = new GraphPoint(Right_Temp_Mouse_Pos.X, Right_Temp_Mouse_Pos.Y);
                PointsList.Add(p);
                Right_Temp_Curve.path.Insert(Right_Temp_Curve_Index, p);
                p.Relative++;
                float x = (Right_Temp_Curve.path[Right_Temp_Curve_Index + 1].P.X - Right_Temp_Curve.path[Right_Temp_Curve_Index - 1].P.X) / 6;
                float y = (Right_Temp_Curve.path[Right_Temp_Curve_Index + 1].P.Y - Right_Temp_Curve.path[Right_Temp_Curve_Index - 1].P.Y) / 6;
                Right_Temp_Curve.disFirst.Insert(Right_Temp_Curve_Index, new PointF(x * -1, y * -1));
                Right_Temp_Curve.disSecond.Insert(Right_Temp_Curve_Index, new PointF(x, y));
                Right_Temp_Curve.type.Insert(Right_Temp_Curve_Index, 0);
                Push_Undo_Data();
            }
            ClearRighttemp();
        }
        private void 組成群組ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < Right_Temp_Group.G.Count; i++)
            {
                GroupList.Remove(Right_Temp_Group.G[i]);
            }
            GroupList.Add(Right_Temp_Group);
            Right_Temp_Group = null;
            SelectedGroup = null;
            Push_Undo_Data();
        }
        private void 取消群組ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Right_Temp_Group.G.Count; i++)
            {
                GroupList.Add(Right_Temp_Group.G[i]);
            }
            GroupList.Remove(Right_Temp_Group);
            Right_Temp_Group = null;
            SelectedGroup = null;
            Push_Undo_Data();
        }
        private void ClearRighttemp()
        {
            Right_Temp_Curve = null;
            Right_Temp_Curve_Index = -1;
            Right_Temp_Line = null;
            Right_Temp_Point = null;

        }

        private void 平滑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool incurve = false;
            GraphCurve tc = null;
            int tcindex = -1;
            foreach (var c in CurveList)
            {
                if (c.path.Exists(x => x == SelectedPoint))
                {
                    incurve = true;
                    tc = c;
                    tcindex = c.path.FindIndex(x => x == SelectedPoint);
                }
            }
            if (incurve)
            {
                tc.type[tcindex] = 0;
            }
            Push_Undo_Data();
        }
        private void 拉直ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool incurve = false;
            GraphCurve tc = null;
            int tcindex = -1;
            foreach (var c in CurveList)
            {
                if (c.path.Exists(x => x == SelectedPoint))
                {
                    incurve = true;
                    tc = c;
                    tcindex = c.path.FindIndex(x => x == SelectedPoint);
                }
            }
            if (incurve)
            {
                tc.type[tcindex] = 1;
            }
            Push_Undo_Data();
        }
        private void 無ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool incurve = false;
            GraphCurve tc = null;
            int tcindex = -1;
            foreach (var c in CurveList)
            {
                if (c.path.Exists(x => x == SelectedPoint))
                {
                    incurve = true;
                    tc = c;
                    tcindex = c.path.FindIndex(x => x == SelectedPoint);
                }
            }
            if (incurve)
            {
                tc.type[tcindex] = 2;
            }
            Push_Undo_Data();
        }

        //ContextMenuStrip2
        private void 重新命名ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int a = TabpagesList.FindIndex(x => x == tabControl1.SelectedTab);
            重新命名 f2 = new 重新命名(TabpageDataList[a].TabpageName);
            if (f2.ShowDialog() == DialogResult.OK)
            {
                TabpageDataList[a].TabpageName = f2.ans;
            }
            tabControl1.TabPages[a].Text = TabpageDataList[a].TabpageName;
        }
        private void 刪除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = tabControl1.SelectedIndex;
            int a = TabpagesList.FindIndex(x => x == tabControl1.SelectedTab);
            TabpagesList.RemoveAt(a);
            TabpageDataList.RemoveAt(a);
            tabControl1.TabPages.RemoveAt(a);
            tabControl1.SelectedIndex = i == TabpagesList.Count-1 ? i - 1 : i;
        }
        private void tabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = e.Location;
                for (int i = 0; i < tabControl1.TabCount; i++)
                {
                    Rectangle rect = tabControl1.GetTabRect(i);
                    if (rect.Contains(p))
                    {
                        tabControl1.SelectedIndex = i;
                        contextMenuStrip2.Show(MousePosition);
                    }
                        
                }
            }
        }


    }
}
