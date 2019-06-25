using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace 繪圖
{
    public partial class Form1 : Form
    {
        #region parameter
        List<GraphPoint> PointsList = new List<GraphPoint>();
        bool is_Drowing = false;
        int Mouse_Mode = 1;  //1=指標 2=畫線 3=畫弧線 4=畫圓弧
        List<GraphLine> LineList = new List<GraphLine>();
        List<GraphCurve> CurveList = new List<GraphCurve>();
        List<GraphGroup> GroupList = new List<GraphGroup>();
        List<GraphArc> ArcList = new List<GraphArc>();
        GraphLine SelectedLine = null;
        MoveInfo Moving = null;//0=line 1=point 2=curve 3=curveControl 4=mutiselect 5=group 6=arc 7=arccontrol
        
        GraphPoint SelectedPoint = null;
        GraphCurve SelectedCurve = null;
        int SelectedCurveIndex = -1;
        GraphCurve SelectedCurveControl = null;
        int SelectedCurveControlIndex = -1;
        int SeletedCurveControlFS = -1;
        GraphGroup SelectedGroup = null;
        GraphArc SelectedArc = null;
        GraphArc SelectedArcControl = null;
        int SelectedArcControlSide = -1;
        int SelectedGroupControl;
        double SizeOfNet = 10;
        int DesityOfNet = 4;
        int LenthUnit = 1;//0==cm 1==inch
        bool Ctrl = false;
        GraphCurve PreviousCruve = new GraphCurve();
        GraphPoint previous_point;
        List<TabPage> TabpagesList = new List<TabPage>();
        List<TabpageData> TabpageDataList = new List<TabpageData>();
        Point For_Paint;
        bool Net_Mode = false;
        float ZoomSize = 1.00F;
        bool hidepoints = false;
        #endregion

        #region classes
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

            public GraphArc Arc;
            public int ArcSide;
            public float ArcWidth;
            public float ArcHeight;

            public int GroupControl;
            public float GroupWidth;
            public float GroupHeight;
            public PointF GroupLeftUp;
            public float GroupStartWidth;
            public float GroupStartHeight;
            public PointF GroupMid;
            public List<GraphCurve> GroupCurveControl;

            public Moving_Type type;
        }
        public enum Moving_Type
        {
            Line, Point, Curve, Curve_Control, Mutiselect, Group, Arc, Arc_Control, GroupControl
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
                Arc = null;
            }
            public PointF P;
            public int Relative;
            public GraphArc Arc;
            public bool Arc_Start; // true==start
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
        public class GraphArc
        {
            public PointF P;
            public float width;
            public float height;
            public float startangle;
            public float angleLenth;
            public GraphPoint startP;
            public GraphPoint endP;
            public GraphArc()
            {

            }
            public GraphArc(PointF p, float w,float h)
            {
                P = p;
                width = w;
                height = h;
                startangle = 270;
                angleLenth = 90;
                PointF center = Middle();
                PointF s = PointOnEllipseFromAngle(startangle);
                PointF e = PointOnEllipseFromAngle(startangle + angleLenth);
                startP = new GraphPoint(s.X, s.Y);
                endP = new GraphPoint(e.X, e.Y);
                startP.Relative++;
                endP.Relative++;
                startP.Arc = this;
                endP.Arc = this;
                startP.Arc_Start = true;
                endP.Arc_Start = false;
            }
            public PointF Middle()
            {
                return new PointF(P.X + width / 2, P.Y + height / 2); 
            }
            public void refreshAngle()
            {
                PointF p = Middle();
                startP.P = PointOnEllipseFromAngle(startangle);
                endP.P = PointOnEllipseFromAngle(startangle + angleLenth);
            }
            public PointF PointOnEllipseFromAngle(float angle)
            {
                angle = angle % 360;
                PointF center = Middle();
                float radiusX = width / 2;
                float radiusY = height / 2;

                double tanAngle = Math.Abs(Math.Tan(angle * (Math.PI / 180.0)));
                double x = Math.Sqrt((Math.Pow(radiusX, 2) * Math.Pow(radiusY, 2)) / (Math.Pow(radiusY, 2) + Math.Pow(radiusX, 2) * Math.Pow(tanAngle, 2)));
                double y = x * tanAngle;

                if ((angle >= 0) && (angle < 90))
                {
                    return new PointF(center.X + (int)x, center.Y + (int)y);
                }
                else if ((angle >= 90) && (angle < 180))
                {
                    return new PointF(center.X - (int)x, center.Y + (int)y);
                }
                else if ((angle >= 180) && (angle < 270))
                {
                    return new PointF(center.X - (int)x, center.Y - (int)y);
                }
                else
                {
                    return new PointF(center.X + (int)x, center.Y - (int)y);
                }
            }
        }
        public class TabpageData
        {
            public List<GraphPoint> PL = new List<GraphPoint>();
            public List<GraphLine> LL = new List<GraphLine>();
            public List<GraphCurve> CL = new List<GraphCurve>();
            public List<GraphGroup> GL = new List<GraphGroup>();
            public List<GraphArc> AL = new List<GraphArc>();
            public List<TabpageData> Undo = new List<TabpageData>();
            public int width, height;
            public int Undo_index = -1;
            public string TabpageName;
        }
        public class GraphGroup
        {
            public List<GraphPoint> P = new List<GraphPoint>();
            public List<GraphLine> L = new List<GraphLine>();
            public List<GraphCurve> C = new List<GraphCurve>();
            public List<GraphArc> A = new List<GraphArc>();
            public List<GraphGroup> G = new List<GraphGroup>();
        }
        #endregion

        #region find_function
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
                    if (SelectedGroup.C.Count == 1 && SelectedGroup.L.Count == 0 && SelectedGroup.C.FindIndex(a => a == curve) >= 0 && SelectedGroup.A.Count == 0)
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
        static public GraphArc FindArcByPoint(List<GraphArc> arcs,Point p)
        {
            var size = 5;
            var buffer = new Bitmap(size * 2, size * 2);
            foreach (var a in arcs)
            {
                //draw each line on small region around current point p and check pixel in point p 

                using (var g = Graphics.FromImage(buffer))
                {
                    g.Clear(Color.Black);
                    g.DrawArc(new Pen(Color.Green, 5), a.P.X - p.X + size, a.P.Y - p.Y + size, a.width, a.height, a.startangle, a.angleLenth);
                }

                if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    return a;
            }
            return null;
        }
        public void FindArcControlByPoint(List<GraphArc> arcs,Point p,out GraphArc Aout,out int Side)
        {
            var size = 5;
            var buffer = new Bitmap(size * 2, size * 2);
            foreach (var a in arcs)
            {
                //draw each line on small region around current point p and check pixel in point p 
                if (SelectedGroup != null)
                {
                    if (SelectedGroup.C.Count == 0 && SelectedGroup.L.Count == 0 && SelectedGroup.A.FindIndex(x => x == a) >= 0 && SelectedGroup.A.Count == 1)
                    {
                        using (var g = Graphics.FromImage(buffer))
                        {
                            g.Clear(Color.Black);
                            g.DrawRectangle(new Pen(Color.Green, 5), a.P.X - p.X + size - 2, a.P.Y - p.Y + size - 2, size, size);
                        }
                        if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                        {
                            Aout = a;
                            Side = 0;
                            return;
                        }
                        using (var g = Graphics.FromImage(buffer))
                        {
                            g.Clear(Color.Black);
                            g.DrawRectangle(new Pen(Color.Green, 5), a.P.X + a.width - p.X + size - 2, a.P.Y - p.Y + size - 2, size, size);
                        }
                        if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                        {
                            Aout = a;
                            Side = 1;
                            return;
                        }
                        using (var g = Graphics.FromImage(buffer))
                        {
                            g.Clear(Color.Black);
                            g.DrawRectangle(new Pen(Color.Green, 5), a.P.X - p.X + size - 2, a.P.Y + a.height - p.Y + size - 2, size, size);
                        }
                        if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                        {
                            Aout = a;
                            Side = 2;
                            return;
                        }
                        using (var g = Graphics.FromImage(buffer))
                        {
                            g.Clear(Color.Black);
                            g.DrawRectangle(new Pen(Color.Green, 5), a.P.X + a.width - p.X + size - 2, a.P.Y + a.height - p.Y + size - 2, size, size);
                        }
                        if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                        {
                            Aout = a;
                            Side = 3;
                            return;
                        }
                    }
                }
            }
            Aout = null;
            Side = -1;
            return;
        }
        public int FindGroupControlByPoint(GraphGroup groups, Point p)
        {
            var size = 5;
            var buffer = new Bitmap(size * 2, size * 2);
            //draw each line on small region around current point p and check pixel in point p 
            if (groups != null)
            {
                if (groups.A.Count + groups.L.Count + groups.C.Count > 1)
                {
                    float minx, miny, maxx, maxy;
                    minx = groups.P[0].P.X;
                    miny = groups.P[0].P.Y;
                    maxx = groups.P[0].P.X;
                    maxy = groups.P[0].P.Y;
                    foreach (var po in SelectedGroup.P)
                    {
                        if (minx > po.P.X)
                            minx = po.P.X;
                        if (miny > po.P.Y)
                            miny = po.P.Y;
                        if (maxx < po.P.X)
                            maxx = po.P.X;
                        if (maxy < po.P.Y)
                            maxy = po.P.Y;
                    }
                    minx -= 5 / ZoomSize;
                    miny -= 5 / ZoomSize;
                    maxx += 5 / ZoomSize;
                    maxy += 5 / ZoomSize;
                    using (var g = Graphics.FromImage(buffer))
                    {
                        g.Clear(Color.Black);
                        g.DrawRectangle(new Pen(Color.Green, 5), minx - p.X + size - 2, miny - p.Y + size - 2, size, size);
                    }
                    if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    {
                        return 0;
                    }
                    using (var g = Graphics.FromImage(buffer))
                    {
                        g.Clear(Color.Black);
                        g.DrawRectangle(new Pen(Color.Green, 5), maxx - p.X + size - 2, miny - p.Y + size - 2, size, size);
                    }
                    if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    {
                        return 1;
                    }
                    using (var g = Graphics.FromImage(buffer))
                    {
                        g.Clear(Color.Black);
                        g.DrawRectangle(new Pen(Color.Green, 5), minx - p.X + size - 2, maxy - p.Y + size - 2, size, size);
                    }
                    if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    {
                        return 2;
                    }
                    using (var g = Graphics.FromImage(buffer))
                    {
                        g.Clear(Color.Black);
                        g.DrawRectangle(new Pen(Color.Green, 5), maxx - p.X + size - 2, maxy - p.Y + size - 2, size, size);
                    }
                    if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    {
                        return 3;
                    }
                    using (var g = Graphics.FromImage(buffer))
                    {
                        g.Clear(Color.Black);
                        g.DrawRectangle(new Pen(Color.Green, 5), (maxx + minx) / 2 - 3 / ZoomSize - p.X + size - 2, miny - 23 / ZoomSize - p.Y + size - 2, size, size);
                    }
                    if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    {
                        return 4;
                    }
                }
                
            }
            return -1;
        }
        #endregion
        
        public Form1()
        {
            InitializeComponent();
            toolStripTextBox1.Enabled = false;
            toolStripTextBox1.Text = "1";
            toolStripTextBox2.Enabled = false;
            toolStripTextBox2.Text = "4";
            toolStripComboBox1.Enabled = false;
            toolStripComboBox1.SelectedIndex = 0;
            toolStripTextBox1_Leave(new object(), new EventArgs());
        }
        private void tabControl1_KeyDown(object sender, KeyEventArgs e)
        {
            Ctrl = e.Control;
        }
        private void tabControl1_KeyUp(object sender, KeyEventArgs e)
        {
            Ctrl = e.Control;
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
                string data = "";
                data += TabpageDataList.Count + "\n";
                foreach(var t in TabpageDataList)
                {
                    data += WriteTabPageData(t);
                }
                StreamWriter s = new StreamWriter(saveFileDialog1.FileName);
                s.Write(data);
                s.Flush();
                s.Close();
            }
        }
        public static string WriteTabPageData(TabpageData dout)
        {
            string data = "";
            data += dout.TabpageName + " " + dout.width + " " + dout.height + "\n";
            data += dout.PL.Count + "\n";
            foreach (var p in dout.PL)
            {
                data += p.P.X + " " + p.P.Y + "\n";
            }
            data += dout.LL.Count + "\n";
            foreach (var l in dout.LL)
            {
                data += dout.PL.FindIndex(x => x == l.StartPoint) + " " + dout.PL.FindIndex(x => x == l.EndPoint) + "\n";
            }
            data += dout.CL.Count + "\n";
            foreach(var c in dout.CL)
            {
                data += c.path.Count + "\n";
                for(int i = 0; i < c.path.Count; i++)
                {
                    data += dout.PL.FindIndex(x => x == c.path[i]) + " " + c.disFirst[i].X + " " + c.disFirst[i].Y + " " 
                        + c.disSecond[i].X + " " + c.disSecond[i].Y + " " + c.type[i] + "\n";
                }
            }
            data += dout.AL.Count + "\n";
            foreach(var a in dout.AL)
            {
                data += dout.PL.FindIndex(x => x == a.startP) + " " + dout.PL.FindIndex(x => x == a.endP) + " " 
                    + a.P.X + " " + a.P.Y + " " + a.width + " " + a.height + " " + a.startangle + " " + a.angleLenth + "\n";
            }
            data += dout.GL.Count + "\n";
            foreach(var g in dout.GL)
            {
                data += WriteGroup(dout, g);
            }
            return data;
        }
        private static string WriteGroup(TabpageData dout, GraphGroup g)
        {
            string data = "";
            data += g.P.Count + "\n";
            foreach (var p in g.P)
            {
                data += dout.PL.FindIndex(x => x == p) + "\n";
            }
            data += g.L.Count + "\n";
            foreach (var l in g.L)
            {
                data += dout.LL.FindIndex(x => x == l) + "\n";
            }
            data += g.C.Count + "\n";
            foreach (var c in g.C)
            {
                data += dout.CL.FindIndex(x => x == c) + "\n";
            }
            data += g.A.Count + "\n";
            foreach (var a in g.A)
            {
                data += dout.AL.FindIndex(x => x == a) + "\n";
            }
            data += g.G.Count + "\n";
            foreach (var gr in g.G)
            {
                data += WriteGroup(dout, gr);
            }
            return data;
        }
        private void toolStripMenuItem3_Click(object sender, EventArgs e) // 開啟影像
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                TabpageDataList = new List<TabpageData>();

                StreamReader sr = new StreamReader(openFileDialog1.FileName);
                string s = sr.ReadLine();
                int size;
                int.TryParse(s, out size);
                for(int i = 0; i < size; i++)
                {
                    TabpageDataList.Add(ReadTabData(sr));
                }
                TabpagesList = new List<TabPage>();
                tabControl1.TabPages.Clear();
                foreach(var td in TabpageDataList)
                {
                    TabPage tp = new TabPage(td.TabpageName);
                    td.Undo = new List<TabpageData>();
                    TabpagesList.Add(tp);
                    tabControl1.TabPages.Add(tp);
                    PointsList = td.PL;
                    LineList = td.LL;
                    CurveList = td.CL;
                    ArcList = td.AL;
                    GroupList = td.GL;
                    Undo_Data = td.Undo;
                    Push_Undo_Data();
                }
                TabpagesList.Add(new TabPage("+"));
                tabControl1.TabPages.Add(TabpagesList[TabpagesList.Count - 1]);
                tabControl1.SelectedIndex = 0;

                pictureBox1.Width = (int)(TabpageDataList[0].width * ZoomSize);
                pictureBox1.Height = (int)(TabpageDataList[0].height * ZoomSize);
                pictureBox1.Image = new Bitmap((int)(TabpageDataList[0].width * ZoomSize), (int)(TabpageDataList[0].height * ZoomSize));
                pictureBox2.Width = (int)((TabpageDataList[0].width) * ZoomSize) + 20;
                pictureBox2.Height = (int)((TabpageDataList[0].height) * ZoomSize) + 20;
                Graphics g = Graphics.FromImage(pictureBox1.Image);
                g.Clear(Color.WhiteSmoke);
                PointsList = TabpageDataList[0].PL;
                LineList = TabpageDataList[0].LL;
                CurveList = TabpageDataList[0].CL;
                ArcList = TabpageDataList[0].AL;
                GroupList = TabpageDataList[0].GL;
                Undo_Data = TabpageDataList[0].Undo;


                TabpagesList[0].Controls.Add(pictureBox1);
                TabpagesList[0].Controls.Add(pictureBox2);
                pictureBox1.Location = new Point(20, 20);
                pictureBox2.Location = new Point(0, 0);

                pictureBox1.Visible = true;
                pictureBox2.Visible = true;
            }
        }
        public TabpageData ReadTabData(StreamReader sr)
        {
            TabpageData td = new TabpageData();
            string s = sr.ReadLine();
            string[] sarr = s.Split(' ');
            td.TabpageName = sarr[0];
            int width; int.TryParse(sarr[1], out width); td.width = width;
            int height; int.TryParse(sarr[2], out height); td.height = height;

            s = sr.ReadLine();
            int pnum; int.TryParse(s, out pnum);
            td.PL = new List<GraphPoint>();
            for(int i = 0; i < pnum; i++)
            {
                s = sr.ReadLine();
                sarr = s.Split(' ');
                float x; float.TryParse(sarr[0], out x);
                float y; float.TryParse(sarr[1], out y);
                GraphPoint p = new GraphPoint(x, y);
                td.PL.Add(p);
            }

            s = sr.ReadLine();
            int lnum; int.TryParse(s, out lnum);
            td.LL = new List<GraphLine>();
            for(int i = 0; i < lnum; i++)
            {
                s = sr.ReadLine();
                sarr = s.Split(' ');
                int start; int.TryParse(sarr[0],out start);
                int end; int.TryParse(sarr[1], out end);
                GraphLine l = new GraphLine(td.PL[start], td.PL[end]);
                td.PL[start].Relative++;
                td.PL[end].Relative++;
                td.LL.Add(l);
            }

            s = sr.ReadLine();
            int cnum; int.TryParse(s, out cnum);
            td.CL = new List<GraphCurve>();
            for(int i = 0; i < cnum; i++)
            {
                s = sr.ReadLine();
                int pathnum; int.TryParse(s, out pathnum);
                GraphCurve c = new GraphCurve();
                for(int j = 0; j < pathnum; j++)
                {
                    s = sr.ReadLine();
                    sarr = s.Split(' ');
                    int pid; int.TryParse(sarr[0], out pid);
                    c.path.Add(td.PL[pid]);
                    td.PL[pid].Relative++;

                    float fx; float.TryParse(sarr[1], out fx);
                    float fy; float.TryParse(sarr[2], out fy);
                    c.disFirst.Add(new PointF(fx, fy));

                    float sx; float.TryParse(sarr[3], out sx);
                    float sy; float.TryParse(sarr[4], out sy);
                    c.disSecond.Add(new PointF(sx, sy));

                    int type; int.TryParse(sarr[5], out type);
                    c.type.Add(type);
                }
                td.CL.Add(c);
            }

            s = sr.ReadLine();
            int anum; int.TryParse(s, out anum);
            td.AL = new List<GraphArc>();
            for(int i = 0; i < anum; i++)
            {
                GraphArc a = new GraphArc();
                s = sr.ReadLine();
                sarr = s.Split(' ');

                int astart; int.TryParse(sarr[0], out astart);
                a.startP = td.PL[astart];
                td.PL[astart].Arc = a;
                td.PL[astart].Arc_Start = true;
                td.PL[astart].Relative++;

                int aend; int.TryParse(sarr[1], out aend);
                a.startP = td.PL[aend];
                td.PL[aend].Arc = a;
                td.PL[aend].Arc_Start = false;
                td.PL[aend].Relative++;

                float x; float.TryParse(sarr[2], out x);
                float y; float.TryParse(sarr[3], out y);
                a.P = new PointF(x, y);

                int awidth; int.TryParse(sarr[4], out awidth);
                a.width = awidth;
                int aheight; int.TryParse(sarr[5], out aheight);
                a.height = aheight;

                float startangle; float.TryParse(sarr[6], out startangle);
                a.startangle = startangle;
                float anglelenth; float.TryParse(sarr[7], out anglelenth);
                a.angleLenth = anglelenth;

                td.AL.Add(a);
            }

            s = sr.ReadLine();
            int gnum; int.TryParse(s, out gnum);
            td.GL = new List<GraphGroup>();
            for(int i = 0; i < gnum; i++)
            {
                td.GL.Add(ReadGroup(sr, td));
            }

            return td;
        }
        private GraphGroup ReadGroup(StreamReader sr, TabpageData td)
        {
            GraphGroup g = new GraphGroup();
            string s = sr.ReadLine();
            int pnum; int.TryParse(s, out pnum);
            for (int i = 0; i < pnum; i++)
            {
                s = sr.ReadLine();
                int id; int.TryParse(s, out id);
                g.P.Add(td.PL[id]);
            }

            s = sr.ReadLine();
            int lnum; int.TryParse(s, out lnum);
            for (int i = 0; i < lnum; i++)
            {
                s = sr.ReadLine();
                int id; int.TryParse(s, out id);
                g.L.Add(td.LL[id]);
            }

            s = sr.ReadLine();
            int cnum; int.TryParse(s, out cnum);
            for (int i = 0; i < cnum; i++)
            {
                s = sr.ReadLine();
                int id; int.TryParse(s, out id);
                g.C.Add(td.CL[id]);
            }

            s = sr.ReadLine();
            int anum; int.TryParse(s, out anum);
            for (int i = 0; i < anum; i++)
            {
                s = sr.ReadLine();
                int id; int.TryParse(s, out id);
                g.A.Add(td.AL[id]);
            }

            s = sr.ReadLine();
            int gnum; int.TryParse(s, out gnum);
            for (int i = 0; i < pnum; i++)
            {
                g.G.Add(ReadGroup(sr, td));
            }

            return g;
        }
        private void 新影像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Width = (int)(800 * ZoomSize);
            pictureBox1.Height = (int)(600 * ZoomSize);
            pictureBox1.Image = new Bitmap((int)(800*ZoomSize), (int)(600 * ZoomSize));
            pictureBox2.Width = (int)((800) * ZoomSize)+20;
            pictureBox2.Height = (int)((600) * ZoomSize)+20;
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(Color.WhiteSmoke);
            PointsList = new List<GraphPoint>();
            LineList = new List<GraphLine>();
            CurveList = new List<GraphCurve>();
            ArcList = new List<GraphArc>();
            GroupList = new List<GraphGroup>();

            TabpagesList.Clear();
            TabpageDataList.Clear();
            TabpagesList.Add(new TabPage("new_page"));
            TabpagesList.Add(new TabPage("+"));
            tabControl1.TabPages.Clear();
            TabpagesList[0].AutoScroll = true;
            tabControl1.TabPages.Add(TabpagesList[0]);
            tabControl1.TabPages.Add(TabpagesList[1]);
            tabControl1.SelectedIndex = 0;
            
            TabpagesList[0].Controls.Add(pictureBox1);
            TabpagesList[0].Controls.Add(pictureBox2);
            pictureBox1.Location = new Point(20, 20);
            pictureBox2.Location = new Point(0, 0);

            TabpageData a = new TabpageData();
            a.CL = CurveList;
            a.PL = PointsList;
            a.LL = LineList;
            a.AL = ArcList;
            a.GL = GroupList;
            a.Undo = Undo_Data;
            a.width = 800;
            a.height = 600;
            a.TabpageName = TabpagesList[0].Text;
            TabpageDataList.Add(a);
            Push_Undo_Data();

            pictureBox1.Visible = true;
            pictureBox2.Visible = true;
        }
        private void 調整大小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int a = TabpagesList.FindIndex(x => x == tabControl1.SelectedTab);
            Form3 f2 = new Form3((int)(pictureBox1.Width / ZoomSize), (int)(pictureBox1.Height / ZoomSize));
            if (f2.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap((int)(f2.width * ZoomSize), (int)(f2.height * ZoomSize));
                pictureBox2.Image = new Bitmap((int)(f2.width * ZoomSize) + 20, (int)(f2.height * ZoomSize) + 20);
                Graphics g = Graphics.FromImage(pictureBox1.Image);
                g.Clear(Color.WhiteSmoke);
                ImgFitSize();
                TabpageDataList[tabControl1.SelectedIndex].width = (int)(f2.width * ZoomSize);
                TabpageDataList[tabControl1.SelectedIndex].height = (int)(f2.height * ZoomSize);
                Push_Undo_Data();
            }
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.Clear(Color.Aqua);
            Pen p = new Pen(Color.Black, 1);
            e.Graphics.DrawLine(p, new PointF(19, 19), new PointF(pictureBox2.Width, 19));
            e.Graphics.DrawLine(p, new PointF(19, 19), new PointF(19, pictureBox2.Height));
            //double size = SizeOfNet / (LenthUnit == 0 ? 10 * 2 : 25.4 * 2);
            double t = -1;
            if (double.TryParse(toolStripTextBox1.Text, out t))
            {
            }
            int totalW = (int)(pictureBox2.Width / t / SizeOfNet / 2 / ZoomSize) - 1;
            for (int i = 0; i * SizeOfNet < pictureBox2.Width - 20; i++)
            {
                e.Graphics.DrawLine(p, new PointF((i * (float)SizeOfNet) * ZoomSize + 20, 10), new PointF((i * (float)SizeOfNet) * ZoomSize + 20, 20));
                if (t > 0)
                    e.Graphics.DrawString(t * (i - totalW) + "", new Font("新細明體", 8, FontStyle.Regular), new SolidBrush(Color.Black), new PointF((i * (float)SizeOfNet) * ZoomSize + 20, 5));
                for(int j = 1; j <= DesityOfNet; j++)
                {
                    e.Graphics.DrawLine(p, new PointF((i * (float)SizeOfNet + (float)SizeOfNet / DesityOfNet * j) * ZoomSize + 20, 15)
                                    , new PointF((i * (float)SizeOfNet + (float)SizeOfNet / DesityOfNet * j) * ZoomSize + 20, 20));
                }
            }
            int totalH = (int)(pictureBox2.Height / t / SizeOfNet / 2 / ZoomSize) - 1;
            for (int i = 0; i * SizeOfNet < pictureBox2.Height - 20; i++)
            {
                e.Graphics.DrawLine(p, new PointF(10, (i * (float)SizeOfNet) * ZoomSize + 20),
                                        new PointF(20, (i * (float)SizeOfNet) * ZoomSize + 20));
                if (t > 0)
                    e.Graphics.DrawString(t * (i - totalH) + "", new Font("新細明體", 8, FontStyle.Regular), new SolidBrush(Color.Black), new PointF(2, (i * (float)SizeOfNet) * ZoomSize + 10));
                for (int j = 1; j <= DesityOfNet; j++)
                {
                    e.Graphics.DrawLine(p, new PointF(15, (i * (float)SizeOfNet + (float)SizeOfNet / DesityOfNet * j) * ZoomSize + 20),
                                        new PointF(20, (i * (float)SizeOfNet + (float)SizeOfNet / DesityOfNet * j) * ZoomSize + 20));
                }
            }
        }

        #region pictureBox1_event
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            if (Net_Mode)
            {
                for (float i = 0; i < this.pictureBox1.Height; i += (float)SizeOfNet)
                {
                    var pen = new Pen(Color.LightGray, 1);
                    e.Graphics.DrawLine(pen, new PointF(0, i * ZoomSize), new PointF(this.pictureBox1.Width, i * ZoomSize));
                    for (int j = 1; j <= DesityOfNet; j++)
                    {
                        e.Graphics.DrawLine(pen, new PointF(0, (i + (float)SizeOfNet / DesityOfNet * j) * ZoomSize), new PointF(this.pictureBox1.Width, (i + (float)SizeOfNet / DesityOfNet * j) * ZoomSize));
                    }
                }
                for (float i = 0; i < this.pictureBox1.Width; i += (float)SizeOfNet)
                {
                    var pen = new Pen(Color.LightGray, 1);
                    e.Graphics.DrawLine(pen, new PointF(i * ZoomSize, 0), new PointF(i * ZoomSize, this.pictureBox1.Height));
                    for (int j = 1; j <= DesityOfNet; j++)
                    {
                        e.Graphics.DrawLine(pen, new PointF((i + (float)SizeOfNet / DesityOfNet * j) * ZoomSize, 0), new PointF((i + (float)SizeOfNet / DesityOfNet * j) * ZoomSize, this.pictureBox1.Height));
                    }
                }
            }
            foreach (var line in LineList)
            {
                var color = Color.Black;
                var size = 1;
                if (SelectedGroup != null)
                {
                    color = SelectedGroup.L.FindIndex(x => x == line) < 0 ? Color.Black : Color.Red;
                    size = SelectedGroup.L.FindIndex(x => x == line) < 0 ? 1 : 2;
                }
                var pen = new Pen(color, size);
                e.Graphics.DrawLine(pen, line.StartPoint.P.X * ZoomSize, line.StartPoint.P.Y * ZoomSize, line.EndPoint.P.X * ZoomSize, line.EndPoint.P.Y * ZoomSize);
            }
            if (!hidepoints)
            {
                foreach (var p in PointsList)
                {
                    var color = p == SelectedPoint ? Color.Red : Color.Black;
                    var size = p == SelectedPoint ? 2 : 1;
                    var pen = new Pen(color, size);
                    e.Graphics.DrawRectangle(pen, p.P.X * ZoomSize - 3, p.P.Y * ZoomSize - 3, 6, 6);
                }
            }
            foreach (var c in CurveList)
            {
                var color = Color.Black;
                var size = 1;
                if (SelectedGroup != null)
                {
                    color = SelectedGroup.C.FindIndex(x => x == c) < 0 ? Color.Black : Color.Red;
                    size = SelectedGroup.C.FindIndex(x => x == c) < 0 ? 1 : 2;
                }
                var pen = new Pen(color, size);
                List<PointF> t = GraphCurveToBez(c);
                for(int i =0;i<t.Count;i++)
                {
                    t[i] = new PointF(t[i].X * ZoomSize, t[i].Y * ZoomSize);
                }
                e.Graphics.DrawBeziers(pen, t.ToArray());
                var l = new Pen(Color.LightSkyBlue, 1);
                var rec = new Pen(Color.Orange, 1);
                if (SelectedGroup != null)
                {
                    if (SelectedGroup.C.Count == 1 && SelectedGroup.L.Count == 0 && SelectedGroup.C.FindIndex(a => a == c) >= 0 && SelectedGroup.A.Count == 0)
                    {
                        for (int i = 0; i < t.Count; i += 3)
                        {
                            if (i == 0)
                            {
                                e.Graphics.DrawLine(l, t[i].X, t[i].Y, t[i + 1].X, t[i + 1].Y);
                                e.Graphics.DrawRectangle(rec, t[i + 1].X  - 2, t[i + 1].Y - 2, 4, 4);
                            }
                            else if (i == t.Count - 1)
                            {
                                e.Graphics.DrawLine(l, t[i].X, t[i].Y, t[i - 1].X, t[i - 1].Y);
                                e.Graphics.DrawRectangle(rec, t[i - 1].X - 2, t[i - 1].Y - 2, 4, 4);
                            }
                            else
                            {
                                e.Graphics.DrawLine(l, t[i].X, t[i].Y, t[i + 1].X, t[i + 1].Y);
                                e.Graphics.DrawRectangle(rec, t[i + 1].X - 2, t[i + 1].Y - 2, 4, 4);
                                e.Graphics.DrawLine(l, t[i].X, t[i].Y, t[i - 1].X, t[i - 1].Y);
                                e.Graphics.DrawRectangle(rec, t[i - 1].X - 2, t[i - 1].Y - 2, 4, 4);
                            }
                        }
                    }
                }
            }
            foreach(var a in ArcList)
            {
                var color = Color.Black;
                var size = 1;
                if (SelectedGroup != null)
                {
                    color = SelectedGroup.A.FindIndex(x => x == a) < 0 ? Color.Black : Color.Red;
                    size = SelectedGroup.A.FindIndex(x => x == a) < 0 ? 1 : 2;
                }
                var pen = new Pen(color, size);
                e.Graphics.DrawArc(pen, a.P.X * ZoomSize, a.P.Y * ZoomSize, a.width * ZoomSize, a.height * ZoomSize, a.startangle, a.angleLenth);
                if (SelectedGroup != null)
                {
                    if(SelectedGroup.C.Count == 0 && SelectedGroup.L.Count == 0 && SelectedGroup.A.FindIndex(x => x == a) >= 0 && SelectedGroup.A.Count == 1)
                    {
                        var l = new Pen(Color.LightSkyBlue, 1);
                        var rec = new Pen(Color.Orange, 1);
                        float x0 = a.P.X * ZoomSize;
                        float y0 = a.P.Y * ZoomSize;
                        float x1 = (a.P.X + a.width) * ZoomSize;
                        float y1 = (a.P.Y + a.height) * ZoomSize;
                        e.Graphics.DrawLine(l, new PointF(x0, y0), new PointF(x0, y1));
                        e.Graphics.DrawLine(l, new PointF(x0, y1), new PointF(x1, y1));
                        e.Graphics.DrawLine(l, new PointF(x1, y1), new PointF(x1, y0));
                        e.Graphics.DrawLine(l, new PointF(x1, y0), new PointF(x0, y0));
                        e.Graphics.DrawRectangle(rec, x0 - 2, y0 - 2, 4, 4);
                        e.Graphics.DrawRectangle(rec, x1 - 2, y0 - 2, 4, 4);
                        e.Graphics.DrawRectangle(rec, x0 - 2, y1 - 2, 4, 4);
                        e.Graphics.DrawRectangle(rec, x1 - 2, y1 - 2, 4, 4);
                    }
                }
            }
            if (SelectedGroup != null)
            {
                if(SelectedGroup.A.Count + SelectedGroup.L.Count + SelectedGroup.C.Count > 1)
                {
                    float minx, miny, maxx, maxy;
                    minx = SelectedGroup.P[0].P.X;
                    miny = SelectedGroup.P[0].P.Y;
                    maxx = SelectedGroup.P[0].P.X;
                    maxy = SelectedGroup.P[0].P.Y;
                    foreach (var p in SelectedGroup.P)
                    {
                        if (minx > p.P.X)
                            minx = p.P.X;
                        if (miny > p.P.Y)
                            miny = p.P.Y;
                        if (maxx < p.P.X)
                            maxx = p.P.X;
                        if (maxy < p.P.Y)
                            maxy = p.P.Y;
                    }
                    var l = new Pen(Color.LightSkyBlue, 1);
                    var rec = new Pen(Color.Orange, 1);
                    minx *= ZoomSize;
                    miny *= ZoomSize;
                    maxx *= ZoomSize;
                    maxy *= ZoomSize;
                    minx -= 5;
                    miny -= 5;
                    maxx += 5;
                    maxy += 5;
                    e.Graphics.DrawLine(l, new PointF(minx, miny), new PointF(minx, maxy));
                    e.Graphics.DrawLine(l, new PointF(minx, maxy), new PointF(maxx, maxy));
                    e.Graphics.DrawLine(l, new PointF(maxx, maxy), new PointF(maxx, miny));
                    e.Graphics.DrawLine(l, new PointF(maxx, miny), new PointF(minx, miny));
                    e.Graphics.DrawRectangle(rec, minx - 2, miny - 2, 4, 4);
                    e.Graphics.DrawRectangle(rec, maxx - 2, miny - 2, 4, 4);
                    e.Graphics.DrawRectangle(rec, minx - 2, maxy - 2, 4, 4);
                    e.Graphics.DrawRectangle(rec, maxx - 2, maxy - 2, 4, 4);
                    e.Graphics.DrawLine(l, new PointF((minx + maxx) / 2, miny), new PointF((minx + maxx) / 2, miny - 20));
                    e.Graphics.DrawEllipse(rec, (minx + maxx) / 2 - 3, miny - 23, 6, 6);
                }
            }
            if (Moving != null)
            {
                if (Moving.type == Moving_Type.Mutiselect)
                {
                    Pen pe = new Pen(Color.LightBlue, 1.5F);
                    float x = Moving.StartMoveMousePoint.X < For_Paint.X ? Moving.StartMoveMousePoint.X : For_Paint.X;
                    float y = Moving.StartMoveMousePoint.Y < For_Paint.Y ? Moving.StartMoveMousePoint.Y : For_Paint.Y;
                    float width = Math.Abs(Moving.StartMoveMousePoint.X - For_Paint.X);
                    float height = Math.Abs(Moving.StartMoveMousePoint.Y - For_Paint.Y);
                    e.Graphics.DrawRectangle(pe, x * ZoomSize, y * ZoomSize, width * ZoomSize, height * ZoomSize);
                }
            }
            if (is_Drowing)
            {
                PointF p = For_Paint;
                if (Net_Mode)
                {
                    p.X += (float)SizeOfNet / DesityOfNet / 2;
                    p.Y += (float)SizeOfNet / DesityOfNet / 2;
                    p.X -= p.X % ((float)SizeOfNet / DesityOfNet);
                    p.Y -= p.Y % ((float)SizeOfNet / DesityOfNet);
                }
                Pen pe = new Pen(Color.Black, 2);
                if (Mouse_Mode == 2)
                {
                    if (SelectedPoint == null)
                        e.Graphics.DrawLine(pe, previous_point.P.X * ZoomSize, previous_point.P.Y * ZoomSize, p.X * ZoomSize, p.Y * ZoomSize);
                    else
                        e.Graphics.DrawLine(pe, previous_point.P.X * ZoomSize, previous_point.P.Y * ZoomSize, SelectedPoint.P.X * ZoomSize, SelectedPoint.P.Y * ZoomSize);
                }
                else if (Mouse_Mode == 3)
                {
                    GraphCurve c = CurveToBez(PreviousCruve.path, 1, p);
                    List<PointF> t = GraphCurveToBez(c);
                    for (int i = 0; i < t.Count; i++)
                    {
                        t[i] = new PointF(t[i].X * ZoomSize, t[i].Y * ZoomSize);
                    }
                    e.Graphics.DrawBeziers(pe, t.ToArray());
                }
                else if (Mouse_Mode == 4)
                {
                    float x = p.X <= previous_point.P.X ? p.X : previous_point.P.X;
                    float y = p.Y <= previous_point.P.Y ? p.Y : previous_point.P.Y;
                    PointF lu = new PointF(x, y);
                    GraphArc a = new GraphArc(lu, Math.Abs(p.X - previous_point.P.X), Math.Abs(p.Y - previous_point.P.Y));
                    if(Math.Abs(p.X - previous_point.P.X) > 0 && Math.Abs(p.Y - previous_point.P.Y) > 0)
                        e.Graphics.DrawArc(pe, a.P.X * ZoomSize, a.P.Y * ZoomSize, a.width * ZoomSize, a.height * ZoomSize, a.startangle, a.angleLenth);
                }
            }
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs ev)
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
            toolStripStatusLabel1.Text = "X:" + e.X;
            toolStripStatusLabel2.Text = "Y:" + e.Y;
            if (pictureBox1.Image != null)
            {
                if (Mouse_Mode == 1)
                {
                    if (Moving != null)
                    {
                        if (Moving.type == Moving_Type.Line)
                        {
                            GraphPoint sp = new GraphPoint(Moving.StartLinePoint.P.X + e.X - Moving.StartMoveMousePoint.X, Moving.StartLinePoint.P.Y + e.Y - Moving.StartMoveMousePoint.Y);
                            GraphPoint ep = new GraphPoint(Moving.EndLinePoint.P.X + e.X - Moving.StartMoveMousePoint.X, Moving.EndLinePoint.P.Y + e.Y - Moving.StartMoveMousePoint.Y);
                            Moving.Line.StartPoint.P.X = sp.P.X;
                            Moving.Line.StartPoint.P.Y = sp.P.Y;
                            Moving.Line.EndPoint.P.X = ep.P.X;
                            Moving.Line.EndPoint.P.Y = ep.P.Y;
                        }
                        else if (Moving.type == Moving_Type.Point)
                        {
                            if (Moving.StartLinePoint.Arc == null)
                            {
                                GraphPoint sp = new GraphPoint(e.X, e.Y);
                                GraphPoint st = FindPointByPoint(PointsList, new Point((int)sp.P.X, (int)sp.P.Y));
                                if (Net_Mode)
                                {
                                    sp.P.X += (float)SizeOfNet / DesityOfNet / 2;
                                    sp.P.Y += (float)SizeOfNet / DesityOfNet / 2;
                                    sp.P.X -= sp.P.X % ((float)SizeOfNet / DesityOfNet);
                                    sp.P.Y -= sp.P.Y % ((float)SizeOfNet / DesityOfNet);
                                }
                                if (st != null)
                                {
                                    sp.P.X = st.P.X;
                                    sp.P.Y = st.P.Y;
                                }
                                Moving.StartLinePoint.P.X = sp.P.X;
                                Moving.StartLinePoint.P.Y = sp.P.Y;
                            }
                            else
                            {
                                PointF mid =  Moving.StartLinePoint.Arc.Middle();
                                float angle = (float)Math.Atan2(e.Y - mid.Y, e.X - mid.X);
                                angle = angle * 180 / (float)Math.PI;
                                angle = angle < 0 ? angle + 360 : angle;
                                float t = Moving.StartLinePoint.Arc.startangle + Moving.StartLinePoint.Arc.angleLenth;
                                
                                if (Moving.StartLinePoint.Arc_Start)
                                {
                                    Moving.StartLinePoint.Arc.startangle = angle;
                                    Moving.StartLinePoint.Arc.angleLenth = t - angle;
                                }
                                else
                                {
                                    //angle += t >= 360 ? 360 : 0;
                                    angle -= t;
                                    angle = angle < 0 ? angle + 360 : angle;
                                    Moving.StartLinePoint.Arc.angleLenth += angle;
                                }
                                if (Moving.StartLinePoint.Arc.angleLenth > 360)
                                    Moving.StartLinePoint.Arc.angleLenth %= 360;
                                if (Moving.StartLinePoint.Arc.angleLenth < 0)
                                    Moving.StartLinePoint.Arc.angleLenth += 360;
                                Moving.StartLinePoint.Arc.refreshAngle();
                            }
                        }
                        else if (Moving.type == Moving_Type.Curve)
                        {
                            for(int i = 0; i < Moving.Path.Count; i++)
                            {
                                Moving.Curve.path[i].P.X = Moving.Path[i].P.X + e.X - Moving.StartMoveMousePoint.X;
                                Moving.Curve.path[i].P.Y = Moving.Path[i].P.Y + e.Y - Moving.StartMoveMousePoint.Y;
                            }
                        }
                        else if (Moving.type == Moving_Type.Curve_Control)
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
                        else if (Moving.type == Moving_Type.Group)
                        {
                            for (int i = 0; i < Moving.Group.P.Count; i++)
                            {
                                PointF a = new PointF();
                                if (Moving.Group.P[i].Arc != null && Moving.Group.P[i].Arc_Start == true)
                                {
                                    a = new PointF(Moving.Group.P[i].P.X - Moving.Group.P[i].Arc.P.X, Moving.Group.P[i].P.Y - Moving.Group.P[i].Arc.P.Y);
                                }
                                Moving.Group.P[i].P.X = Moving.Path[i].P.X + e.X - Moving.StartMoveMousePoint.X;
                                Moving.Group.P[i].P.Y = Moving.Path[i].P.Y + e.Y - Moving.StartMoveMousePoint.Y;
                                if(Moving.Group.P[i].Arc != null && Moving.Group.P[i].Arc_Start == true)
                                {
                                    Moving.Group.P[i].Arc.P.X = Moving.Group.P[i].P.X - a.X;
                                    Moving.Group.P[i].Arc.P.Y = Moving.Group.P[i].P.Y - a.Y;
                                }
                            }
                        }
                        else if (Moving.type == Moving_Type.Arc)
                        {
                            Moving.Arc.P.X = Moving.StartLinePoint.P.X + e.X - Moving.StartMoveMousePoint.X;
                            Moving.Arc.P.Y = Moving.StartLinePoint.P.Y + e.Y - Moving.StartMoveMousePoint.Y;
                            Moving.Arc.refreshAngle();
                        }
                        else if (Moving.type == Moving_Type.Arc_Control)
                        {
                            if (Moving.ArcSide == 0)
                            {
                                Moving.Arc.P.X = Moving.StartLinePoint.P.X + e.X - Moving.StartMoveMousePoint.X;
                                Moving.Arc.P.Y = Moving.StartLinePoint.P.Y + e.Y - Moving.StartMoveMousePoint.Y;
                                Moving.Arc.width = Moving.ArcWidth - e.X + Moving.StartMoveMousePoint.X;
                                Moving.Arc.height = Moving.ArcHeight - e.Y + Moving.StartMoveMousePoint.Y;
                                Moving.Arc.refreshAngle();
                            }
                            else if(Moving.ArcSide == 1)
                            {
                                Moving.Arc.P.Y = Moving.StartLinePoint.P.Y + e.Y - Moving.StartMoveMousePoint.Y;
                                Moving.Arc.width = Moving.ArcWidth + e.X - Moving.StartMoveMousePoint.X;
                                Moving.Arc.height = Moving.ArcHeight - e.Y + Moving.StartMoveMousePoint.Y;
                                Moving.Arc.refreshAngle();
                            }
                            else if (Moving.ArcSide == 2)
                            {
                                Moving.Arc.P.X = Moving.StartLinePoint.P.X + e.X - Moving.StartMoveMousePoint.X;
                                Moving.Arc.width = Moving.ArcWidth - e.X + Moving.StartMoveMousePoint.X;
                                Moving.Arc.height = Moving.ArcHeight + e.Y - Moving.StartMoveMousePoint.Y;
                                Moving.Arc.refreshAngle();
                            }
                            else if (Moving.ArcSide == 3)
                            {
                                Moving.Arc.width = Moving.ArcWidth + e.X - Moving.StartMoveMousePoint.X;
                                Moving.Arc.height = Moving.ArcHeight + e.Y - Moving.StartMoveMousePoint.Y;
                                Moving.Arc.refreshAngle();
                            }
                        }
                        else if (Moving.type == Moving_Type.GroupControl)
                        {
                            if (Moving.GroupControl == 0)
                            {
                                Moving.GroupLeftUp.X = Moving.StartLinePoint.P.X + e.X - Moving.StartMoveMousePoint.X;
                                Moving.GroupLeftUp.Y = Moving.StartLinePoint.P.Y + e.Y - Moving.StartMoveMousePoint.Y;
                                Moving.GroupWidth = Moving.GroupStartWidth - e.X + Moving.StartMoveMousePoint.X;
                                Moving.GroupHeight = Moving.GroupStartHeight - e.Y + Moving.StartMoveMousePoint.Y;
                            }
                            else if (Moving.GroupControl == 1)
                            {
                                Moving.GroupLeftUp.Y = Moving.StartLinePoint.P.Y + e.Y - Moving.StartMoveMousePoint.Y;
                                Moving.GroupWidth = Moving.GroupStartWidth + e.X - Moving.StartMoveMousePoint.X;
                                Moving.GroupHeight = Moving.GroupStartHeight - e.Y + Moving.StartMoveMousePoint.Y;
                            }
                            else if (Moving.GroupControl == 2)
                            {
                                Moving.GroupLeftUp.X = Moving.StartLinePoint.P.X + e.X - Moving.StartMoveMousePoint.X;
                                Moving.GroupWidth = Moving.GroupStartWidth - e.X + Moving.StartMoveMousePoint.X;
                                Moving.GroupHeight = Moving.GroupStartHeight + e.Y - Moving.StartMoveMousePoint.Y;
                            }
                            else if (Moving.GroupControl == 3)
                            {
                                Moving.GroupWidth = Moving.GroupStartWidth + e.X - Moving.StartMoveMousePoint.X;
                                Moving.GroupHeight = Moving.GroupStartHeight + e.Y - Moving.StartMoveMousePoint.Y;
                            }
                            if (Moving.GroupControl > -1 && Moving.GroupControl < 4)
                            {
                                for (int i = 0; i < Moving.Group.P.Count; i++)
                                {
                                    Moving.Group.P[i].P.X = Moving.GroupLeftUp.X + Moving.Path[i].P.X * Moving.GroupWidth / Moving.GroupStartWidth;
                                    Moving.Group.P[i].P.Y = Moving.GroupLeftUp.Y + Moving.Path[i].P.Y * Moving.GroupHeight / Moving.GroupStartHeight;
                                }
                                for(int i = 0; i < Moving.GroupCurveControl.Count; i++)
                                {
                                    GraphCurve c = Moving.GroupCurveControl[i];
                                    for(int j = 0; j < c.disFirst.Count; j++)
                                    {
                                        float fx = Moving.GroupLeftUp.X + c.disFirst[j].X * Moving.GroupWidth / Moving.GroupStartWidth;
                                        float fy = Moving.GroupLeftUp.Y + c.disFirst[j].Y * Moving.GroupHeight / Moving.GroupStartHeight;
                                        Moving.Group.C[i].disFirst[j] = new PointF(fx - Moving.Group.C[i].path[j].P.X, fy - Moving.Group.C[i].path[j].P.Y);

                                        float sx = Moving.GroupLeftUp.X + c.disSecond[j].X * Moving.GroupWidth / Moving.GroupStartWidth;
                                        float sy = Moving.GroupLeftUp.Y + c.disSecond[j].Y * Moving.GroupHeight / Moving.GroupStartHeight;
                                        Moving.Group.C[i].disSecond[j] = new PointF(sx - Moving.Group.C[i].path[j].P.X, sy - Moving.Group.C[i].path[j].P.Y);
                                    }
                                }
                            }
                            if (Moving.GroupControl == 4)
                            {
                                float angle = (float)Math.Atan2(e.Y - Moving.GroupMid.Y, e.X - Moving.GroupMid.X) + (float)Math.PI / 2;
                                for (int i = 0; i < Moving.Group.P.Count; i++)
                                {
                                    float pangle = (float)Math.Atan2(Moving.Path[i].P.Y, Moving.Path[i].P.X);
                                    float r = (float)Math.Sqrt(Moving.Path[i].P.X * Moving.Path[i].P.X + Moving.Path[i].P.Y * Moving.Path[i].P.Y);
                                    Moving.Group.P[i].P.X = Moving.GroupMid.X + r * (float)Math.Cos(pangle + angle);
                                    Moving.Group.P[i].P.Y = Moving.GroupMid.Y + r * (float)Math.Sin(pangle + angle);
                                }
                                for (int i = 0; i < Moving.GroupCurveControl.Count; i++)
                                {
                                    GraphCurve c = Moving.GroupCurveControl[i];
                                    for (int j = 0; j < c.disFirst.Count; j++)
                                    {
                                        float fpangle = (float)Math.Atan2(c.disFirst[j].Y, c.disFirst[j].X);
                                        float fr = (float)Math.Sqrt(c.disFirst[j].X * c.disFirst[j].X + c.disFirst[j].Y * c.disFirst[j].Y);
                                        float fx = Moving.GroupMid.X + fr * (float)Math.Cos(fpangle + angle);
                                        float fy = Moving.GroupMid.Y + fr * (float)Math.Sin(fpangle + angle);
                                        Moving.Group.C[i].disFirst[j] = new PointF(fx - Moving.Group.C[i].path[j].P.X, fy - Moving.Group.C[i].path[j].P.Y);

                                        float spangle = (float)Math.Atan2(c.disSecond[j].Y, c.disSecond[j].X);
                                        float sr = (float)Math.Sqrt(c.disSecond[j].X * c.disSecond[j].X + c.disSecond[j].Y * c.disSecond[j].Y);
                                        float sx = Moving.GroupMid.X + sr * (float)Math.Cos(spangle + angle);
                                        float sy = Moving.GroupMid.Y + sr * (float)Math.Sin(spangle + angle);
                                        Moving.Group.C[i].disSecond[j] = new PointF(sx - Moving.Group.C[i].path[j].P.X, sy - Moving.Group.C[i].path[j].P.Y);
                                    }
                                }
                            }
                        }
                    }
                }
                For_Paint = e;
                RefreshSelection(e);
                pictureBox1.Refresh();
            }
        }
        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs ev)
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
            is_Drowing = false;
            previous_point = null;
            if (Mouse_Mode == 3)
            {
                if (PreviousCruve.path.Count > 1)
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
                    Push_Undo_Data();
                }
                PreviousCruve = new GraphCurve();
                PointCombine();
            }
            PointsList.RemoveAll(x => x.Relative == 0);
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs ev)
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
            if (Moving != null && Mouse_Mode == 1)
            {
                if (Moving.type == Moving_Type.Mutiselect)
                {
                    MutiSelect(ev);
                }
                else if (Moving.type == Moving_Type.Line && Moving.StartMoveMousePoint == e)
                {
                    Click_To_Select_Line();
                }
                else if (Moving.type == Moving_Type.Curve && Moving.StartMoveMousePoint == e)
                {
                    Click_To_Select_Curve();
                }
                else if(Moving.type == Moving_Type.Arc && Moving.StartMoveMousePoint == e)
                {
                    Click_To_Select_Arc();
                }
                else if (Moving.type == Moving_Type.Group && Moving.StartMoveMousePoint == e)
                {
                    Click_To_Select_Group();
                }
                else
                {
                    Push_Undo_Data();
                }
                PointCombine();
                this.Capture = false;
                Moving = null;
            }
            if (Mouse_Mode == 4 && ev.Button == MouseButtons.Left && previous_point != null)
            {
                float x = e.X < previous_point.P.X ? e.X : previous_point.P.X;
                float y = e.Y < previous_point.P.Y ? e.Y : previous_point.P.Y;
                PointF lu = new PointF(x, y);
                GraphArc a = new GraphArc(lu, Math.Abs(e.X - previous_point.P.X), Math.Abs(e.Y - previous_point.P.Y));
                if (Math.Abs(e.X - previous_point.P.X) > 0 && Math.Abs(e.Y - previous_point.P.Y) > 0)
                {
                    ArcList.Add(a);
                    PointsList.Add(a.startP);
                    PointsList.Add(a.endP);
                }
                is_Drowing = false;
                previous_point = null;
                Push_Undo_Data();
            }
            RefreshSelection(e);
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs ev)
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
            RefreshSelection(e);
            if (ev.Button == MouseButtons.Left && Mouse_Mode == 1)
            {
                MBL_M1_DOWN(ev);
            }
            if (ev.Button == MouseButtons.Left && Mouse_Mode == 2)
            {
                MBL_M2_DOWN(ev);
            }
            if (ev.Button == MouseButtons.Left && Mouse_Mode == 3)
            {
                MBL_M3_DOWN(ev);
            }
            if (ev.Button == MouseButtons.Left && Mouse_Mode == 4)
            {
                MBL_M4_DOWN(ev);
            }
            if (ev.Button == MouseButtons.Right && Mouse_Mode == 1)
            {
                MBR_M1_DOWN(ev);
            }
            if (ev.Button == MouseButtons.Right && (Mouse_Mode == 2 || Mouse_Mode == 3))
            {
                pictureBox1_MouseDoubleClick(sender, ev);
            }
            RefreshSelection(e);
        }
        private void MBL_M1_DOWN(MouseEventArgs ev)//游標
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
            bool in_Group = false;
            int in_List = -1;
            if (this.SelectedPoint != null && Moving == null)
            {
                if (SelectedGroup != null)
                    if (SelectedGroup.P.FindIndex(a => a == SelectedPoint) >= 0)
                        in_Group = true;
                foreach (var g in GroupList)
                {
                    if (g.P.FindIndex(a => a == SelectedPoint) >= 0)
                        in_List = GroupList.FindIndex(a => a == g);
                }
                //this.Capture = true;
                Moving = new MoveInfo
                {
                    StartLinePoint = SelectedPoint,
                    type = Moving_Type.Point
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
                    StartMoveMousePoint = e,
                    type = Moving_Type.Line
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
                for (int i = 0; i < SelectedCurve.path.Count; i++)
                {
                    path.Add(new GraphPoint(SelectedCurve.path[i].P.X, SelectedCurve.path[i].P.Y));
                }
                Moving = new MoveInfo
                {
                    Curve = this.SelectedCurve,
                    StartMoveMousePoint = e,
                    Path = path,
                    type = Moving_Type.Curve
                };
            }
            else if (this.SelectedCurveControl != null && Moving == null && this.SelectedCurveControlIndex != -1)
            {
                Moving = new MoveInfo
                {
                    Curve = this.SelectedCurveControl,
                    CurveIndex = this.SelectedCurveControlIndex,
                    CurveFS = this.SeletedCurveControlFS,
                    type = Moving_Type.Curve_Control
                };
            }
            else if (this.SelectedArc != null && Moving == null)
            {
                if (SelectedGroup != null)
                    if (SelectedGroup.A.FindIndex(x => x == SelectedArc) >= 0)
                        in_Group = true;
                foreach (var g in GroupList)
                {
                    if (g.A.FindIndex(x => x == SelectedArc) >= 0)
                        in_List = GroupList.FindIndex(x => x == g);
                }
                Moving = new MoveInfo
                {
                    StartMoveMousePoint = e,
                    StartLinePoint = new GraphPoint(SelectedArc.P.X, SelectedArc.P.Y),
                    Arc = SelectedArc,
                    type = Moving_Type.Arc
                };
            }
            else if (this.SelectedArcControl != null && Moving == null)
            {
                Moving = new MoveInfo
                {
                    StartMoveMousePoint = e,
                    StartLinePoint = new GraphPoint(SelectedArcControl.P.X, SelectedArcControl.P.Y),
                    Arc = SelectedArcControl,
                    ArcSide = SelectedArcControlSide,
                    ArcWidth = SelectedArcControl.width,
                    ArcHeight = SelectedArcControl.height,
                    type = Moving_Type.Arc_Control
                };
            }
            else if (this.SelectedGroupControl != -1 && Moving == null)
            {
                float minx, miny, maxx, maxy;
                minx = SelectedGroup.P[0].P.X;
                miny = SelectedGroup.P[0].P.Y;
                maxx = SelectedGroup.P[0].P.X;
                maxy = SelectedGroup.P[0].P.Y;
                foreach (var p in SelectedGroup.P)
                {
                    if (minx > p.P.X)
                        minx = p.P.X;
                    if (miny > p.P.Y)
                        miny = p.P.Y;
                    if (maxx < p.P.X)
                        maxx = p.P.X;
                    if (maxy < p.P.Y)
                        maxy = p.P.Y;
                }
                minx -= 5 / ZoomSize;
                miny -= 5 / ZoomSize;
                maxx += 5 / ZoomSize;
                maxy += 5 / ZoomSize;
                List<GraphPoint> path = new List<GraphPoint>();
                PointF mid = new PointF((minx + maxx) / 2, (miny + maxy) / 2);
                for (int i = 0; i < SelectedGroup.P.Count; i++)
                {
                    if(SelectedGroupControl == 4)
                        path.Add(new GraphPoint(SelectedGroup.P[i].P.X - mid.X, SelectedGroup.P[i].P.Y - mid.Y));
                    else
                        path.Add(new GraphPoint(SelectedGroup.P[i].P.X - minx, SelectedGroup.P[i].P.Y - miny));
                }
                List<GraphCurve> curvecontrol = new List<GraphCurve>();
                for(int i = 0; i < SelectedGroup.C.Count; i++)
                {
                    curvecontrol.Add(new GraphCurve());
                    GraphCurve c = SelectedGroup.C[i];
                    if (SelectedGroupControl == 4)
                    {
                        for (int j = 0; j < SelectedGroup.C[i].path.Count; j++)
                        {
                            curvecontrol[i].disFirst.Add(new PointF(c.path[j].P.X + c.disFirst[j].X - mid.X, c.path[j].P.Y + c.disFirst[j].Y - mid.Y));
                            curvecontrol[i].disSecond.Add(new PointF(c.path[j].P.X + c.disSecond[j].X - mid.X, c.path[j].P.Y + c.disSecond[j].Y - mid.Y));
                        }
                    }
                    else
                    {
                        for (int j = 0; j < SelectedGroup.C[i].path.Count; j++)
                        {
                            curvecontrol[i].disFirst.Add(new PointF(c.path[j].P.X + c.disFirst[j].X - minx, c.path[j].P.Y + c.disFirst[j].Y - miny));
                            curvecontrol[i].disSecond.Add(new PointF(c.path[j].P.X + c.disSecond[j].X - minx, c.path[j].P.Y + c.disSecond[j].Y - miny));
                        }
                    }
                }
                Moving = new MoveInfo
                {
                    StartMoveMousePoint = e,
                    StartLinePoint = new GraphPoint(minx, miny),
                    GroupLeftUp = new PointF(minx, miny),
                    Group = SelectedGroup,
                    Path = path,
                    GroupControl = SelectedGroupControl,
                    GroupWidth = maxx - minx,
                    GroupHeight = maxy - miny,
                    GroupStartWidth = maxx - minx,
                    GroupStartHeight = maxy - miny,
                    GroupMid = mid,
                    GroupCurveControl = curvecontrol,
                    type = Moving_Type.GroupControl
                };
                if (Moving.Group.A.Count > 0)
                {
                    MessageBox.Show("請將群組內的圓弧弧線轉換為曲線後再進行旋轉", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Moving = null;
                }
            }
            else
            {
                Moving = new MoveInfo
                {
                    StartMoveMousePoint = e,
                    type = Moving_Type.Mutiselect
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
                    StartMoveMousePoint = e,
                    Group = SelectedGroup,
                    Path = path,
                    type = Moving_Type.Group
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
                    StartMoveMousePoint = e,
                    Group = GroupList[in_List],
                    Path = path,
                    type = Moving_Type.Group
                };
            }
        }
        private void MBL_M2_DOWN(MouseEventArgs ev)//直線
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
            GraphPoint nowp = new GraphPoint(e.X, e.Y);
            GraphPoint t = FindPointByPoint(PointsList, e);
            if (t == null)
            {
                if (Net_Mode && previous_point != null)
                {
                    float x = nowp.P.X + (float)SizeOfNet / DesityOfNet / 2;
                    float y = nowp.P.Y + (float)SizeOfNet / DesityOfNet / 2;
                    x -= x % ((float)SizeOfNet / DesityOfNet);
                    y -= y % ((float)SizeOfNet / DesityOfNet);
                    if (previous_point.P.X == x && previous_point.P.Y == y)
                        return;
                }
                if (Net_Mode)
                {
                    nowp.P.X += (float)SizeOfNet / DesityOfNet / 2;
                    nowp.P.Y += (float)SizeOfNet / DesityOfNet / 2;
                    nowp.P.X -= nowp.P.X % ((float)SizeOfNet / DesityOfNet);
                    nowp.P.Y -= nowp.P.Y % ((float)SizeOfNet / DesityOfNet);
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
            else if (t.Arc != null)
            {
                if (Net_Mode && previous_point != null)
                {
                    float x = nowp.P.X + (float)SizeOfNet / DesityOfNet / 2;
                    float y = nowp.P.Y + (float)SizeOfNet / DesityOfNet / 2;
                    x -= x % ((float)SizeOfNet / DesityOfNet);
                    y -= y % ((float)SizeOfNet / DesityOfNet);
                    if (previous_point.P.X == x && previous_point.P.Y == y)
                        return;
                }
                nowp.P.X = t.P.X;
                nowp.P.Y = t.P.Y;
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
        private void MBL_M3_DOWN(MouseEventArgs ev)//曲線
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
            GraphPoint nowp = new GraphPoint(e.X, e.Y);
            GraphPoint t = FindPointByPoint(PointsList, e);
            if (t == null)
            {
                if (Net_Mode && previous_point!=null)
                {
                    float x = nowp.P.X + (float)SizeOfNet / DesityOfNet / 2;
                    float y = nowp.P.Y + (float)SizeOfNet / DesityOfNet / 2;
                    x -= x % ((float)SizeOfNet / DesityOfNet);
                    y -= y % ((float)SizeOfNet / DesityOfNet);
                    if (previous_point.P.X == x && previous_point.P.Y == y)
                        return;
                }
                if (Net_Mode)
                {
                    nowp.P.X += (float)SizeOfNet / DesityOfNet / 2;
                    nowp.P.Y += (float)SizeOfNet / DesityOfNet / 2;
                    nowp.P.X -= nowp.P.X % ((float)SizeOfNet / DesityOfNet);
                    nowp.P.Y -= nowp.P.Y % ((float)SizeOfNet / DesityOfNet);
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
        private void MBL_M4_DOWN(MouseEventArgs ev)//圓弧
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
            GraphPoint nowp = new GraphPoint(e.X, e.Y);
            previous_point = nowp;
            is_Drowing = true;
        }
        private void MBR_M1_DOWN(MouseEventArgs ev)
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
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
                int a = SelectedGroup.L.Count + SelectedGroup.C.Count + SelectedGroup.A.Count();
                群組ToolStripMenuItem.Enabled = a > 1 || SelectedGroup.G.Count > 0 ? true : false;
                組成群組ToolStripMenuItem.Enabled = a > 1 ? true : false;
                取消群組ToolStripMenuItem.Enabled = GroupList.FindIndex(b => b == SelectedGroup) >= 0 ? true : false;
            }
            else
                群組ToolStripMenuItem.Enabled = false;

            if (SelectedGroup != null)
            {
                Right_Temp_Point = SelectedPoint;
                新增節點ToolStripMenuItem.Enabled = false;
                轉換為曲線ToolStripMenuItem.Visible = false;
                toolStripMenuItem2.Text = "刪除所有選取範圍";
                Right_Temp_Mouse_Pos = e;
                contextMenuStrip1.Show(MousePosition);
            }
            else if (SelectedPoint != null)
            {
                Right_Temp_Point = SelectedPoint;
                新增節點ToolStripMenuItem.Enabled = false;
                轉換為曲線ToolStripMenuItem.Visible = false;
                toolStripMenuItem2.Text = "刪除節點";
                Right_Temp_Mouse_Pos = e;
                contextMenuStrip1.Show(MousePosition);
            }
            else if (SelectedLine != null)
            {
                Right_Temp_Line = SelectedLine;
                新增節點ToolStripMenuItem.Enabled = true;
                轉換為曲線ToolStripMenuItem.Visible = false;
                toolStripMenuItem2.Text = "刪除直線";
                Right_Temp_Mouse_Pos = e;
                contextMenuStrip1.Show(MousePosition);
            }
            else if (SelectedCurve != null)
            {
                新增節點ToolStripMenuItem.Enabled = true;
                轉換為曲線ToolStripMenuItem.Visible = false;
                Right_Temp_Curve = SelectedCurve;
                Right_Temp_Curve_Index = SelectedCurveIndex;
                toolStripMenuItem2.Text = "刪除整條曲線";
                Right_Temp_Mouse_Pos = e;
                contextMenuStrip1.Show(MousePosition);
            }
            else if (SelectedArc != null)
            {
                新增節點ToolStripMenuItem.Enabled = false;
                轉換為曲線ToolStripMenuItem.Visible = true;
                Right_Temp_Arc = SelectedArc;
                toolStripMenuItem2.Text = "刪除圓弧";
                Right_Temp_Mouse_Pos = e;
                contextMenuStrip1.Show(MousePosition);
            }
            else
            {
                Right_Temp_Line = null;
                Right_Temp_Point = null;
                Right_Temp_Curve = null;
                Right_Temp_Curve_Index = -1;
                Right_Temp_Arc = null;
            }
        }
        private void MutiSelect(MouseEventArgs ev)
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
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
            foreach (var a in ArcList)
            {
                if (a.P.X > x && a.P.Y > y && a.P.X + a.width > x && a.P.Y + a.height > y)
                    if (a.P.X - x < width && a.P.Y - y < height && a.P.X + a.width - x < width && a.P.Y + a.height - y < height)
                    {
                        bool inG = false;
                        foreach (var g in GroupList)
                        {
                            if (g.A.FindIndex(b => b == a) >= 0)
                                inG = true;
                        }
                        if (!inG)
                        {
                            SelectedGroup.A.Add(a);
                            if (SelectedGroup.P.FindIndex(b => b == a.startP) < 0)
                                SelectedGroup.P.Add(a.startP);
                            if (SelectedGroup.P.FindIndex(b => b == a.endP) < 0)
                                SelectedGroup.P.Add(a.endP);
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
                foreach (var a in g.A)
                {
                    if (a.P.X > x && a.P.Y > y && a.P.X + a.width > x && a.P.Y + a.height > y)
                        if (a.P.X - x < width && a.P.Y - y < height && a.P.X + a.width - x < width && a.P.Y + a.height - y < height)
                            ;
                        else
                            outrange = true;
                    else
                        outrange = true;
                }
                if (!outrange)
                {
                    SelectedGroup.G.Add(g);
                }
            }
            if (SelectedGroup.L.Count == 0 && SelectedGroup.C.Count == 0 && SelectedGroup.A.Count == 0 && SelectedGroup.G.Count == 1)
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
                    foreach(var a in g.A)
                    {
                        SelectedGroup.A.Add(a);
                    }
                    for (int i = 0; i < g.P.Count; i++)
                    {
                        if (SelectedGroup.P.FindIndex(a => a == g.P[i]) < 0)
                            SelectedGroup.P.Add(g.P[i]);
                    }
                }
            }
            if (SelectedGroup.L.Count == 0 && SelectedGroup.C.Count == 0 && SelectedGroup.A.Count == 0 && SelectedGroup.G.Count == 0)
                SelectedGroup = null;
        }
        private void Click_To_Select_Line()
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
                            foreach(var a in g.A)
                            {
                                SelectedGroup.A.Add(a);
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
        private void Click_To_Select_Curve()
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
                            foreach(var a in g.A)
                            {
                                SelectedGroup.A.Add(a);
                            }
                            for (int i = 0; i < g.P.Count; i++)
                            {
                                if (SelectedGroup.P.FindIndex(a => a == g.P[i]) < 0)
                                    SelectedGroup.P.Add(g.P[i]);
                            }
                        }
                    }
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
        private void Click_To_Select_Arc()
        {
            if (Ctrl && SelectedGroup != null)
            {
                if (SelectedGroup.A.FindIndex(x => x == Moving.Arc) < 0)
                {
                    if (GroupList.FindIndex(x => x == SelectedGroup) >= 0)
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
                            foreach(var a in g.A)
                            {
                                SelectedGroup.A.Add(a);
                            }
                            for (int i = 0; i < g.P.Count; i++)
                            {
                                if (SelectedGroup.P.FindIndex(a => a == g.P[i]) < 0)
                                    SelectedGroup.P.Add(g.P[i]);
                            }
                        }
                    }
                    SelectedGroup.A.Add(Moving.Arc);
                    if (SelectedGroup.P.FindIndex(a => a == Moving.Arc.startP) < 0)
                        SelectedGroup.P.Add(Moving.Arc.startP);
                    if (SelectedGroup.P.FindIndex(a => a == Moving.Arc.endP) < 0)
                        SelectedGroup.P.Add(Moving.Arc.endP);
                }
            }
            else
            {
                SelectedGroup = new GraphGroup();
                SelectedGroup.A.Add(Moving.Arc);
                SelectedGroup.P.Add(Moving.Arc.startP);
                SelectedGroup.P.Add(Moving.Arc.endP);
            }
        }
        private void Click_To_Select_Group()
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
                            foreach(var a in g.A)
                            {
                                SelectedGroup.A.Add(a);
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
                    foreach(var a in Moving.Group.A)
                    {
                        SelectedGroup.A.Add(a);
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
        #endregion

        #region toolStripButtons

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
            toolStripButton7.Checked = false;
            Refresh();
        }
        private void toolStripButton2_Click(object sender, EventArgs e)//直線
        {
            Mouse_Mode = 2;
            previous_point = null;
            SelectedGroup = null;
            PreviousCruve = new GraphCurve();
            Cursor = Cursors.Cross;
            toolStripButton1.Checked = false;
            toolStripButton2.Checked = true;
            toolStripButton3.Checked = false;
            toolStripButton7.Checked = false;
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
            toolStripButton7.Checked = false;
            Refresh();
        }
        private void toolStripButton7_Click(object sender, EventArgs e)//圓弧
        {
            Mouse_Mode = 4;
            previous_point = null;
            SelectedGroup = null;
            PreviousCruve = new GraphCurve();
            Cursor = Cursors.Cross;
            toolStripButton1.Checked = false;
            toolStripButton2.Checked = false;
            toolStripButton3.Checked = false;
            toolStripButton7.Checked = true;
            Refresh();
        }
        private void toolStripButton4_Click(object sender, EventArgs e)//網格
        {
            if (Net_Mode)
            {
                Net_Mode = false;
                toolStripButton4.Checked = false;
                toolStripTextBox1.Enabled = false;
                toolStripTextBox2.Enabled = false;
                toolStripComboBox1.Enabled = false;
            }
            else
            {
                Net_Mode = true;
                toolStripButton4.Checked = true;
                toolStripTextBox1.Enabled = true;
                toolStripTextBox2.Enabled = true;
                toolStripComboBox1.Enabled = true;
                toolStripTextBox1_Leave(sender, e);
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
                int s = Undo_Data[Undo_Index].PL.FindIndex(x => x == l.StartPoint);
                int end = Undo_Data[Undo_Index].PL.FindIndex(x => x == l.EndPoint);
                t.PL[s].Relative++;
                t.PL[end].Relative++;
                t.LL.Add(new GraphLine(t.PL[s], t.PL[end]));
            }
            foreach (var c in Undo_Data[Undo_Index].CL)
            {
                GraphCurve gc = new GraphCurve();
                for (int i = 0; i < c.path.Count; i++)
                {
                    int index = Undo_Data[Undo_Index].PL.FindIndex(x => x == c.path[i]);
                    gc.path.Add(t.PL[index]);
                    t.PL[index].Relative++;
                    gc.disFirst.Add(new PointF(c.disFirst[i].X, c.disFirst[i].Y));
                    gc.disSecond.Add(new PointF(c.disSecond[i].X, c.disSecond[i].Y));
                    gc.type.Add(c.type[i]);
                }
                t.CL.Add(gc);
            }
            foreach (var a in Undo_Data[Undo_Index].AL)
            {
                int s = Undo_Data[Undo_Index].PL.FindIndex(x => x == a.startP);
                int end = Undo_Data[Undo_Index].PL.FindIndex(x => x == a.endP);
                GraphArc ga = new GraphArc();
                ga.P = a.P;
                ga.height = a.height;
                ga.width = a.width;
                ga.startangle = a.startangle;
                ga.angleLenth = a.angleLenth;
                ga.startP = t.PL[s];
                ga.endP = t.PL[end];
                ga.startP.Relative++;
                ga.endP.Relative++;
                t.PL[s].Arc = ga;
                t.PL[s].Arc_Start = true;
                t.PL[end].Arc = ga;
                t.PL[end].Arc_Start = false;
                t.AL.Add(ga);
            }
            foreach (var g in Undo_Data[Undo_Index].GL)
            {
                t.GL.Add(Group_Copy(g, t, Undo_Data[Undo_Index]));
            }
            t.width = Undo_Data[Undo_Index].width;
            t.height = Undo_Data[Undo_Index].height;
            t.TabpageName = Undo_Data[Undo_Index].TabpageName;
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
                int s = Undo_Data[Undo_Index].PL.FindIndex(x => x == l.StartPoint);
                int end = Undo_Data[Undo_Index].PL.FindIndex(x => x == l.EndPoint);
                t.PL[s].Relative++;
                t.PL[end].Relative++;
                t.LL.Add(new GraphLine(t.PL[s], t.PL[end]));
            }
            foreach (var c in Undo_Data[Undo_Index].CL)
            {
                GraphCurve gc = new GraphCurve();
                for (int i = 0; i < c.path.Count; i++)
                {
                    int index = Undo_Data[Undo_Index].PL.FindIndex(x => x == c.path[i]);
                    gc.path.Add(t.PL[index]);
                    t.PL[index].Relative++;
                    gc.disFirst.Add(new PointF(c.disFirst[i].X, c.disFirst[i].Y));
                    gc.disSecond.Add(new PointF(c.disSecond[i].X, c.disSecond[i].Y));
                    gc.type.Add(c.type[i]);
                }
                t.CL.Add(gc);
            }
            foreach (var a in Undo_Data[Undo_Index].AL)
            {
                int s = Undo_Data[Undo_Index].PL.FindIndex(x => x == a.startP);
                int end = Undo_Data[Undo_Index].PL.FindIndex(x => x == a.endP);
                GraphArc ga = new GraphArc();
                ga.P = a.P;
                ga.height = a.height;
                ga.width = a.width;
                ga.startangle = a.startangle;
                ga.angleLenth = a.angleLenth;
                ga.startP = t.PL[s];
                ga.endP = t.PL[end];
                ga.startP.Relative++;
                ga.endP.Relative++;
                t.PL[s].Arc = ga;
                t.PL[s].Arc_Start = true;
                t.PL[end].Arc = ga;
                t.PL[end].Arc_Start = false;
                t.AL.Add(ga);
            }
            foreach (var g in Undo_Data[Undo_Index].GL)
            {
                t.GL.Add(Group_Copy(g, t, Undo_Data[Undo_Index]));
            }
            t.TabpageName = Undo_Data[Undo_Index].TabpageName;
            t.height = Undo_Data[Undo_Index].height;
            t.width = Undo_Data[Undo_Index].width;
            t.Undo = Undo_Data;
            TabpageDataList[tabControl1.SelectedIndex] = t;
            tabControl1_SelectedIndexChanged(new object(), new EventArgs());
            TabpageDataList[tabControl1.SelectedIndex].Undo_index = Undo_Index;
            RefreshUndoCheck();
            Refresh();
        }
        private void toolStripButton8_Click(object sender, EventArgs e)//放大
        {
            ZoomSize += 0.25F;
            pictureBox1.Image = new Bitmap((int)(TabpageDataList[tabControl1.SelectedIndex].width * ZoomSize), (int)(TabpageDataList[tabControl1.SelectedIndex].height * ZoomSize));
            pictureBox2.Image = new Bitmap((int)(pictureBox1.Width )+20, (int)(pictureBox1.Height)+20);
            toolStripStatusLabel3.Text = "縮放大小:" + ZoomSize * 100 + "%";
            ImgFitSize();
            Refresh();
        }
        private void toolStripButton9_Click(object sender, EventArgs e)//縮小 
        {
            ZoomSize -= 0.25F;
            pictureBox1.Image = new Bitmap((int)(TabpageDataList[tabControl1.SelectedIndex].width * ZoomSize), (int)(TabpageDataList[tabControl1.SelectedIndex].height * ZoomSize));
            pictureBox2.Image = new Bitmap((int)(pictureBox1.Width) + 20, (int)(pictureBox1.Height) + 20);
            toolStripStatusLabel3.Text = "縮放大小:" + ZoomSize * 100 + "%";
            ImgFitSize();
            Refresh();
        }
        private void toolStripButton10_Click(object sender, EventArgs e)//隱藏節點
        {
            if (hidepoints)
            {
                toolStripButton10.Checked = false;
                hidepoints = false;
            }
            else
            {
                toolStripButton10.Checked = true;
                hidepoints = true;
            }
            Refresh();
        }
        private void toolStripTextBox1_Leave(object sender, EventArgs e)
        {
            double t;
            if (double.TryParse(toolStripTextBox1.Text, out t))
            {
                SizeOfNet = LenthUnit == 0 ? t * 10 * 2 : t * 25.4 * 2;
            }
            else
            {
                toolStripTextBox1.Text = SizeOfNet / (LenthUnit == 0 ? 10 * 2 : 25.4 * 2) + "";
            }
            Refresh();
        }
        private void toolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                toolStripTextBox1_Leave(sender, e);
            }
        }
        private void toolStripTextBox2_Leave(object sender, EventArgs e)
        {
            int t;
            if(int.TryParse(toolStripTextBox2.Text, out t) && t > 0)
            {
                DesityOfNet = t;
            }
            else
            {
                toolStripTextBox2.Text = DesityOfNet + "";
            }
            Refresh();
        }
        private void toolStripTextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                toolStripTextBox2_Leave(sender, e);
            }
        }
        private void toolStripComboBox1_Leave(object sender, EventArgs e)
        {
            if (toolStripComboBox1.Text == "inch")
            {
                toolStripComboBox1.SelectedIndex = 0;
                LenthUnit = 1;
            }
            else if (toolStripComboBox1.Text == "cm")
            {
                toolStripComboBox1.SelectedIndex = 1;
                LenthUnit = 0;
            }
            else
            {
                toolStripComboBox1.SelectedIndex = LenthUnit;
            }
            toolStripTextBox1_Leave(sender, e);
        }
        private void toolStripComboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                toolStripComboBox1_Leave(sender, e);
            }
        }
        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            toolStripComboBox1_Leave(sender, e);
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

        #endregion

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
                int s = PointsList.FindIndex(x => x == l.StartPoint);
                int e = PointsList.FindIndex(x => x == l.EndPoint);
                t.PL[s].Relative++;
                t.PL[e].Relative++;
                t.LL.Add(new GraphLine(t.PL[s], t.PL[e]));
            }
            foreach(var c in CurveList)
            {
                GraphCurve gc = new GraphCurve();
                for(int i = 0; i < c.path.Count; i++)
                {
                    int index = PointsList.FindIndex(x => x == c.path[i]);
                    gc.path.Add(t.PL[index]);
                    t.PL[index].Relative++;
                    gc.disFirst.Add(new PointF(c.disFirst[i].X, c.disFirst[i].Y));
                    gc.disSecond.Add(new PointF(c.disSecond[i].X, c.disSecond[i].Y));
                    gc.type.Add(c.type[i]);
                }
                t.CL.Add(gc);
            }
            foreach(var a in ArcList)
            {
                int s = PointsList.FindIndex(x => x == a.startP);
                int e = PointsList.FindIndex(x => x == a.endP);
                GraphArc ga = new GraphArc();
                ga.P = a.P;
                ga.height = a.height;
                ga.width = a.width;
                ga.startangle = a.startangle;
                ga.angleLenth = a.angleLenth;
                ga.startP = t.PL[s];
                ga.endP = t.PL[e];
                ga.startP.Relative++;
                ga.endP.Relative++;
                t.PL[s].Arc = ga;
                t.PL[s].Arc_Start = true;
                t.PL[e].Arc = ga;
                t.PL[e].Arc_Start = false;
                t.AL.Add(ga);
            }
            foreach(var g in GroupList)
            {
                t.GL.Add(Group_Copy(g, t, TabpageDataList[tabControl1.SelectedIndex]));
            }
            t.width = (int)(pictureBox1.Width/ZoomSize);
            t.height = (int)(pictureBox1.Height/ZoomSize);
            for(int i = Undo_Data.Count - 1; i > TabpageDataList[tabControl1.SelectedIndex].Undo_index; i--)
            {
                Undo_Data.RemoveAt(i);
            }
            Undo_Data.Add(t);
            TabpageDataList[tabControl1.SelectedIndex].Undo_index++;
            RefreshUndoCheck();
        }
        private GraphGroup Group_Copy(GraphGroup PreG, TabpageData PreT, TabpageData Ref)
        {
            GraphGroup gg = new GraphGroup();
            foreach(var p in PreG.P)
            {
                int a = Ref.PL.FindIndex(x => x == p);
                gg.P.Add(PreT.PL[a]);
            }
            foreach(var l in PreG.L)
            {
                int a = Ref.LL.FindIndex(x => x == l);
                gg.L.Add(PreT.LL[a]);
            }
            foreach(var c in PreG.C)
            {
                int a = Ref.CL.FindIndex(x => x == c);
                gg.C.Add(PreT.CL[a]);
            }
            foreach(var a in PreG.A)
            {
                int b = Ref.AL.FindIndex(x => x == a);
                gg.A.Add(PreT.AL[b]);
            }
            foreach(var g in PreG.G)
            {
                gg.G.Add(Group_Copy(g, PreT, Ref));
            }
            return gg;
        }
        private void UnGroup(GraphGroup g)
        {
            for (int i = 0; i < g.G.Count; i++)
            {
                GroupList.Add(g.G[i]);
            }
            GroupList.Remove(g);
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
            GraphArc selectarc = FindArcByPoint(ArcList, point);
            GraphArc selectarccontrol;
            int selectarccontrolside = -1;
            FindArcControlByPoint(ArcList, point, out selectarccontrol, out selectarccontrolside);
            int selectedgroupcontrol = FindGroupControlByPoint(SelectedGroup, point);
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
            else if(selectarc!=this.SelectedArc && Mouse_Mode == 1)
            {
                this.SelectedArc = selectarc;
                this.Invalidate();
            }
            else if (selectarccontrol != this.SelectedArcControl && Mouse_Mode == 1)
            {
                this.SelectedArcControl = selectarccontrol;
                this.SelectedArcControlSide = selectarccontrolside;
                this.Invalidate();
            }
            else if (selectedgroupcontrol != SelectedGroupControl)
            {
                this.SelectedGroupControl = selectedgroupcontrol;
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
                SelectedArc != null ? Cursors.SizeAll :
                SelectedArcControlSide == 0 || SelectedArcControlSide == 3 ? Cursors.SizeNWSE :
                SelectedArcControlSide == 1 || SelectedArcControlSide == 2 ? Cursors.SizeNESW :
                SelectedGroupControl == 0 || SelectedGroupControl == 3 ? Cursors.SizeNWSE :
                SelectedGroupControl == 1 || SelectedGroupControl == 2 ? Cursors.SizeNESW :
                SelectedGroupControl == 4 ? Cursors.SizeAll :
                Mouse_Mode == 2 || Mouse_Mode == 3 || Mouse_Mode == 4 ? Cursors.Cross :
                    Cursors.Default;

        }

        private void ImgFitSize()
        {
            if (pictureBox1 != null)
            {
                pictureBox1.Width = pictureBox1.Image.Width;
                pictureBox1.Height = pictureBox1.Image.Height;
                pictureBox2.Width = pictureBox1.Image.Width+20;
                pictureBox2.Height = pictureBox1.Image.Height+20;
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
        private GraphCurve CurveToBez(List<GraphPoint> path, int type = 0, PointF p = new PointF())
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
        GraphArc Right_Temp_Arc = null;
        Point Right_Temp_Mouse_Pos;
        GraphGroup Right_Temp_Group = null;

        #region delete_function
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
            List<GraphArc> at = new List<GraphArc>();
            foreach (var a in ArcList)
            {
                if (a.startP == toDeleteP || a.endP == toDeleteP)
                {
                    at.Add(a);
                }
            }
            foreach (var a in at)
            {
                DeleteArc(a);
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
        private void DeleteArc(GraphArc a)
        {
            PointsList.Remove(a.startP);
            PointsList.Remove(a.endP);
            ArcList.Remove(a);
        }
        private void DeleteGroup(GraphGroup g)
        {
            foreach(var l in g.L)
            {
                DeleteLine(l);
            }
            foreach(var c in g.C)
            {
                DeleteHoleCurve(c);
            }
            foreach(var a in g.A)
            {
                DeleteArc(a);
            }
            GroupList.Remove(g);
        }
        #endregion

        private void PointCombine()
        {
            for(int i = 0; i < PointsList.Count; i++)
            {
                List<GraphPoint> t = PointsList.FindAll(x => x.P == PointsList[i].P);
                if (t.Count == 2)
                {
                    if (t[0].Arc != null || t[1].Arc != null)
                        continue;
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
            if (tabControl1.SelectedIndex == TabpagesList.Count - 1 && tabControl1.SelectedIndex >= 0)
            {
                TabPage t = new TabPage("new_page");
                t.AutoScroll = true;

                CurveList = new List<GraphCurve>();
                PointsList = new List<GraphPoint>();
                LineList = new List<GraphLine>();
                GroupList = new List<GraphGroup>();
                ArcList = new List<GraphArc>();
                pictureBox1.Width = (int)(800*ZoomSize);
                pictureBox1.Height = (int)(600 * ZoomSize);
                pictureBox2.Width = (int)(800 * ZoomSize) + 20;
                pictureBox2.Height = (int)(600 * ZoomSize) + 20;
                Undo_Data = new List<TabpageData>();
                TabpageData a = new TabpageData();
                a.CL = CurveList;
                a.PL = PointsList;
                a.LL = LineList;
                a.GL = GroupList;
                a.AL = ArcList;
                a.Undo = Undo_Data;
                a.TabpageName = t.Text;
                a.width = 800;
                a.height = 600;
                TabpageDataList.Add(a);

                t.Controls.Add(pictureBox1);
                TabpagesList.Insert(TabpagesList.Count - 1, t);
                tabControl1.TabPages.Insert(TabpagesList.Count - 2, t);
                tabControl1.SelectedIndex = TabpagesList.Count - 2;
                TabpagesList[tabControl1.SelectedIndex].Controls.Add(pictureBox1);
                TabpagesList[tabControl1.SelectedIndex].Controls.Add(pictureBox2);
                Push_Undo_Data();
                RefreshUndoCheck();
                Invalidate();
            }
            else if(tabControl1.SelectedIndex >= 0)
            {
                TabpagesList[tabControl1.SelectedIndex].Controls.Add(pictureBox1);
                TabpagesList[tabControl1.SelectedIndex].Controls.Add(pictureBox2);
                CurveList = TabpageDataList[tabControl1.SelectedIndex].CL;
                PointsList = TabpageDataList[tabControl1.SelectedIndex].PL;
                LineList = TabpageDataList[tabControl1.SelectedIndex].LL;
                GroupList = TabpageDataList[tabControl1.SelectedIndex].GL;
                ArcList = TabpageDataList[tabControl1.SelectedIndex].AL;
                pictureBox1.Width = (int)(TabpageDataList[tabControl1.SelectedIndex].width*ZoomSize);
                pictureBox1.Height = (int)(TabpageDataList[tabControl1.SelectedIndex].height*ZoomSize);
                pictureBox2.Width = pictureBox1.Width+20;
                pictureBox2.Height = pictureBox1.Height+20;
                Undo_Data = TabpageDataList[tabControl1.SelectedIndex].Undo;
                RefreshUndoCheck();
            }

        }

        #region ContextMenuStrip1
        private void toolStripMenuItem2_Click(object sender, EventArgs e)//右鍵選單>刪除
        {
            if (Right_Temp_Point != null)
                DeletePoint(Right_Temp_Point);
            else if (Right_Temp_Line != null)
                DeleteLine(Right_Temp_Line);
            else if (Right_Temp_Curve != null)
                DeleteHoleCurve(Right_Temp_Curve);
            else if (Right_Temp_Arc != null)
                DeleteArc(Right_Temp_Arc);
            else if(Right_Temp_Group != null)
                DeleteGroup(Right_Temp_Group);
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
            UnGroup(Right_Temp_Group);
            Right_Temp_Group = null;
            SelectedGroup = null;
            Push_Undo_Data();
        }
        private void 轉換為曲線ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool ingroup = false;
            foreach(var g in GroupList)
            {
                if (g.A.Exists(x => x == Right_Temp_Arc))
                    ingroup = true;
            }
            if (ingroup)
            {
                MessageBox.Show("請將弧線解除群組後再進行轉換", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            float K = 0.5522847498F;
            GraphArc arc = Right_Temp_Arc;
            float arclenth = arc.angleLenth;
            float a = arc.width / 2;
            float b = arc.height / 2;
            float x0, x1, y0, y1;
            float temp;
            PointF m0 = new PointF(), m1 = new PointF();
            PointF p0, p1;
            PointF center = arc.Middle();
            GraphCurve c = new GraphCurve();
            c.path.Add(new GraphPoint(arc.PointOnEllipseFromAngle(arc.startangle).X, arc.PointOnEllipseFromAngle(arc.startangle).Y));
            c.disFirst.Add(new PointF());
            while (arclenth > 90)
            { 
                p0 = arc.PointOnEllipseFromAngle(arc.startangle + arc.angleLenth - arclenth);
                p0.X -= arc.Middle().X;
                p0.Y -= arc.Middle().Y;
                temp = -1 * p0.X * b * b / a / a / p0.Y;
                m0 = p0.Y == 0 ? new PointF(0, 1) :
                     p0.X == 0 ? new PointF(1, 0) :
                     new PointF(1 / (float)Math.Sqrt(temp*temp + 1), temp / (float)Math.Sqrt(temp * temp + 1));
                x0 = m0.X * a * Math.Abs((float)Math.Sin((arc.startangle + arc.angleLenth - arclenth) * Math.PI / 180)) * K;
                y0 = m0.Y * b * Math.Abs((float)Math.Cos((arc.startangle + arc.angleLenth - arclenth) * Math.PI / 180)) * K;
                x0 *= (arc.startangle + arc.angleLenth - arclenth) % 360 <= 180 && (arc.startangle + arc.angleLenth - arclenth) % 360 != 0 ? -1 : 1;
                y0 *= (arc.startangle + arc.angleLenth - arclenth) % 360 <= 180 && (arc.startangle + arc.angleLenth - arclenth) % 360 != 0 ? -1 : 1;
                c.disSecond.Add(new PointF(x0, y0));

                arclenth -= 90;
                p1 = arc.PointOnEllipseFromAngle(arc.startangle + arc.angleLenth - arclenth);
                c.path.Add(new GraphPoint(p1.X, p1.Y));
                p1.X -= arc.Middle().X;
                p1.Y -= arc.Middle().Y;
                temp = -1 * p1.X * b * b / a / a / p1.Y;
                m1 = p1.Y == 0 ? new PointF(0, 1) :
                     p1.X == 0 ? new PointF(1, 0) :
                     new PointF(1 / (float)Math.Sqrt(temp * temp + 1), temp / (float)Math.Sqrt(temp * temp + 1));
                x1 = m1.X * a * Math.Abs((float)Math.Sin((arc.startangle + arc.angleLenth - arclenth) * Math.PI / 360)) * K;
                y1 = m1.Y * b * Math.Abs((float)Math.Cos((arc.startangle + arc.angleLenth - arclenth) * Math.PI / 360)) * K;
                x1 *= (arc.startangle + arc.angleLenth - arclenth) % 360 <= 180 && (arc.startangle + arc.angleLenth - arclenth) % 360 != 0 ? -1 : 1;
                y1 *= (arc.startangle + arc.angleLenth - arclenth) % 360 <= 180 && (arc.startangle + arc.angleLenth - arclenth) % 360 != 0 ? -1 : 1;
                c.disFirst.Add(new PointF(-x1, -y1));
            }
            p0 = arc.PointOnEllipseFromAngle(arc.startangle + arc.angleLenth - arclenth);
            p0.X -= arc.Middle().X;
            p0.Y -= arc.Middle().Y;
            temp = -1 * p0.X * b * b / a / a / p0.Y;
            m0 = p0.Y == 0 ? new PointF(0, 1) :
                 p0.X == 0 ? new PointF(1, 0) :
                 new PointF(1 / (float)Math.Sqrt(temp * temp + 1), temp / (float)Math.Sqrt(temp * temp + 1));
            x0 = m0.X * a * Math.Abs((float)Math.Sin((arc.startangle + arc.angleLenth - arclenth) * Math.PI / 180)) * K * arclenth / 90;
            y0 = m0.Y * b * Math.Abs((float)Math.Cos((arc.startangle + arc.angleLenth - arclenth) * Math.PI / 180)) * K * arclenth / 90;
            x0 *= (arc.startangle + arc.angleLenth - arclenth) % 360 <= 180 && (arc.startangle + arc.angleLenth - arclenth) % 360 != 0 ? -1 : 1;
            y0 *= (arc.startangle + arc.angleLenth - arclenth) % 360 <= 180 && (arc.startangle + arc.angleLenth - arclenth) % 360 != 0 ? -1 : 1;
            c.disSecond.Add(new PointF(x0, y0));
            
            p1 = arc.PointOnEllipseFromAngle(arc.startangle + arc.angleLenth);
            c.path.Add(new GraphPoint(p1.X, p1.Y));
            p1.X -= arc.Middle().X;
            p1.Y -= arc.Middle().Y;
            temp = -1 * p1.X * b * b / a / a / p1.Y;
            m1 = p1.Y == 0 ? new PointF(0, 1) :
                 p1.X == 0 ? new PointF(1, 0) :
                 new PointF(1 / (float)Math.Sqrt(temp * temp + 1), temp / (float)Math.Sqrt(temp * temp + 1));
            x1 = m1.X * a * Math.Abs((float)Math.Sin((arc.startangle + arc.angleLenth) * Math.PI / 180)) * K * arclenth / 90;
            y1 = m1.Y * b * Math.Abs((float)Math.Cos((arc.startangle + arc.angleLenth) * Math.PI / 180)) * K * arclenth / 90;
            x1 *= (arc.startangle + arc.angleLenth) % 360 <= 180 && (arc.startangle + arc.angleLenth) % 360 != 0 ? -1 : 1;
            y1 *= (arc.startangle + arc.angleLenth) % 360 <= 180 && (arc.startangle + arc.angleLenth) % 360 != 0 ? -1 : 1;
            c.disFirst.Add(new PointF(-x1, -y1));
            c.disSecond.Add(new PointF());
            foreach(var i in c.path)
            {
                c.type.Add(2);
                PointsList.Add(i);
                i.Relative++;
            }
            DeleteArc(arc);
            CurveList.Add(c);
            PointCombine();
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
        #endregion

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
