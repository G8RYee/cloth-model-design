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
using System.Drawing.Printing;
//ctrl+A => ctrl+M 兩下 全部摺疊
namespace 繪圖
{
    public partial class main : Form
    {
        #region parameter
        List<GraphPoint> PointsList = new List<GraphPoint>();
        bool is_Drowing = false;
        Mouse_Mode_Type Mouse_Mode = Mouse_Mode_Type.Cursor;
        List<GraphLine> LineList = new List<GraphLine>();
        List<GraphCurve> CurveList = new List<GraphCurve>();
        List<GraphGroup> GroupList = new List<GraphGroup>();
        List<GraphArc> ArcList = new List<GraphArc>();
        List<GraphText> TextList = new List<GraphText>();
        List<GraphGroup> PathList = new List<GraphGroup>();
        List<GroupBox> PorpertyList = new List<GroupBox>();
        List<PathDistance> PDistList = new List<PathDistance>();
        GraphLine SelectedLine = null;
        MoveInfo Moving = null;
        
        GraphPoint SelectedPoint = null;
        GraphCurve SelectedCurve = null;
        int SelectedCurveIndex = -1;
        GraphCurve SelectedCurveControl = null;
        int SelectedCurveControlIndex = -1;
        int SeletedCurveControlFS = -1;
        GraphGroup SelectedGroup = null;
        GraphArc SelectedArc = null;
        GraphArc SelectedArcControl = null;
        int SelectedGroupControl;
        GraphText SelectedText = null;
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
        TextBox Text_Temp;
        GraphText Previous_Text = null;
        GraphText Change_Text = null;
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

            public int GroupControl;
            public float GroupWidth;
            public float GroupHeight;
            public PointF GroupLeftUp;
            public float GroupStartWidth;
            public float GroupStartHeight;
            public PointF GroupMid;
            public List<GraphCurve> GroupCurveControl;

            public GraphText Text;

            public Moving_Type type;
        }
        public enum Moving_Type
        {
            Line, Point, Curve, Curve_Control, Mutiselect, Group, Arc, Arc_Control, GroupControl, Text
        }
        public enum Mouse_Mode_Type
        {
            Cursor, Line, Curve, Arc, Text, Pleated, Cutting, Dist_Hori, Dist_Verti
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
                co = Color.Black;
                Seam = 0;
                isSeam = false;
            }
            public GraphPoint StartPoint;
            public GraphPoint EndPoint;
            public float Seam;
            public float SeamText;
            public bool isSeam;
            public Color co;
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
            public GraphLine MarkL = null;
            public float part = 0;
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
            public bool isSeam = false;
            public float Seam = 0;
            public float SeamText;
            public Color co = Color.Black;
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
            public GraphPoint StartPoint, EndPoint;
            private PointF ControlPoint;
            public Color co = Color.Black;
            public GraphArc(GraphPoint st, GraphPoint en, PointF ctp)
            {
                StartPoint = st;
                EndPoint = en;
                ControlPoint = new PointF(ctp.X - st.P.X, ctp.Y - st.P.Y);
            }
            public GraphArc(GraphPoint st, GraphPoint en)
            {
                StartPoint = st;
                EndPoint = en;
                PointF mid = new PointF((st.P.X + en.P.X) / 2, (st.P.Y + en.P.Y) / 2);
                float x = en.P.X - st.P.X;
                float y = en.P.Y - st.P.Y;
                mid.X += y / 2;
                mid.Y += x / 2;
                mid.X -= st.P.X;
                mid.Y -= st.P.Y;
                ControlPoint = mid;
            }
            public PointF getControlPoint()
            {
                return new PointF(ControlPoint.X + StartPoint.P.X, ControlPoint.Y + StartPoint.P.Y);
            }
            public void setControlPoint(float x,float y)
            {
                ControlPoint = new PointF(x - StartPoint.P.X, y - StartPoint.P.Y);
            }
            public PointF[] to_cubic_bezier()
            {
                PointF p0 = StartPoint.P,
                       p1 = new PointF((StartPoint.P.X + (ControlPoint.X + StartPoint.P.X) * 2) / 3, (StartPoint.P.Y + (ControlPoint.Y+StartPoint.P.Y) * 2) / 3),
                       p2 = new PointF(((ControlPoint.X + StartPoint.P.X) * 2 + EndPoint.P.X) / 3, ((ControlPoint.Y + StartPoint.P.Y) * 2 + EndPoint.P.Y) / 3),
                       p3 = EndPoint.P;
                PointF[] ans = { p0, p1, p2, p3 };
                return ans;
            }
        }
        public class TabpageData
        {
            public List<GraphPoint> PointL = new List<GraphPoint>();
            public List<GraphLine> LineL = new List<GraphLine>();
            public List<GraphCurve> CurveL = new List<GraphCurve>();
            public List<GraphGroup> GroupL = new List<GraphGroup>();
            public List<GraphArc> ArcL = new List<GraphArc>();
            public List<GraphText> TextL = new List<GraphText>();
            public List<GraphGroup> PathL = new List<GraphGroup>();
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
        public class GraphText
        {
            public PointF P;
            public string S;
        }
        public class PathDistance
        {
            public bool T_hori_F_verti;
            public GraphLine L1 = null;
            public GraphLine L2 = null;
            public GraphCurve C1 = null;
            public int cindex1;
            public GraphCurve C2 = null;
            public int cindex2;
            public float XY_Pos;
            public int type;
            public PathDistance(GraphLine l1,GraphLine l2,float pos, bool thfv)
            {
                L1 = l1;
                L2 = l2;
                XY_Pos = pos;
                T_hori_F_verti = thfv;
                type = 0;
            }
            public PathDistance(GraphLine l1, GraphCurve c1,int ci1, float pos, bool thfv)
            {
                L1 = l1;
                C1 = c1;
                cindex1 = ci1;
                XY_Pos = pos;
                T_hori_F_verti = thfv;
                type = 1;
            }
            public PathDistance(GraphCurve c1, int ci1, GraphCurve c2, int ci2, float pos, bool thfv)
            {
                C1 = c1;
                cindex1 = ci1;
                C2 = c2;
                cindex2 = ci2;
                XY_Pos = pos;
                T_hori_F_verti = thfv;
                type = 2;
            }

            public void Get_Dist_Point(out float dist, out PointF p1, out PointF p2)
            {
                GraphLine HorVL;
                if (T_hori_F_verti)
                    HorVL = new GraphLine(new GraphPoint(-10000, XY_Pos), new GraphPoint(10000, XY_Pos));
                else
                    HorVL = new GraphLine(new GraphPoint(XY_Pos, -10000), new GraphPoint(XY_Pos, 10000));
                if (type == 0)
                {
                    p1 = computeIntersections_LL(L1, HorVL);
                    p2 = computeIntersections_LL(L2, HorVL);
                    dist = (float)dist_PP(p1, p2);
                }
                else if (type == 1)
                {
                    p1 = computeIntersections_LL(L1, HorVL);
                    PointF cc1 = new PointF(C1.path[cindex1 - 1].P.X + C1.disSecond[cindex1 - 1].X, C1.path[cindex1 - 1].P.Y + C1.disSecond[cindex1 - 1].Y);
                    PointF cc2 = new PointF(C1.path[cindex1].P.X + C1.disFirst[cindex1].X, C1.path[cindex1].P.Y + C1.disFirst[cindex1].Y);
                    PointF[] bez = { C1.path[cindex1 - 1].P, cc1, cc2, C1.path[cindex1].P };
                    PointF[] line = { HorVL.StartPoint.P, HorVL.EndPoint.P };
                    List<PointF> pl1 = computeIntersections_LC(bez, line);
                    double mindist = 9999999999999999999;
                    p2 = new PointF(-1, -1);
                    dist = 9999999999999999999;
                    for (int i = 0; i < pl1.Count; i++)
                    {
                        if (dist_PP(pl1[i], p1) < mindist)
                        {
                            mindist = dist_PP(pl1[i], p1);
                            dist = (float)mindist;
                            p2 = pl1[i];
                        }
                    }
                }
                else
                {
                    PointF[] line = { HorVL.StartPoint.P, HorVL.EndPoint.P };

                    PointF cc11 = new PointF(C1.path[cindex1 - 1].P.X + C1.disSecond[cindex1 - 1].X, C1.path[cindex1 - 1].P.Y + C1.disSecond[cindex1 - 1].Y);
                    PointF cc12 = new PointF(C1.path[cindex1].P.X + C1.disFirst[cindex1].X, C1.path[cindex1].P.Y + C1.disFirst[cindex1].Y);
                    PointF[] bez1 = { C1.path[cindex1 - 1].P, cc11, cc12, C1.path[cindex1].P };
                    List<PointF> pl1 = computeIntersections_LC(bez1, line);

                    PointF cc21 = new PointF(C2.path[cindex2 - 1].P.X + C2.disSecond[cindex2 - 1].X, C2.path[cindex2 - 1].P.Y + C2.disSecond[cindex2 - 1].Y);
                    PointF cc22 = new PointF(C2.path[cindex2].P.X + C2.disFirst[cindex2].X, C2.path[cindex2].P.Y + C2.disFirst[cindex2].Y);
                    PointF[] bez2 = { C2.path[cindex2 - 1].P, cc21, cc22, C2.path[cindex2].P };
                    List<PointF> pl2 = computeIntersections_LC(bez2, line);
                    
                    double mindist = 9999999999999999999;
                    p1 = new PointF(-1, -1);
                    p2 = new PointF(-1, -1);
                    dist = 9999999999999999999;
                    for (int i = 0; i < pl1.Count; i++)
                    {
                        for (int j = 0; j < pl2.Count; j++)
                        {
                            if (dist_PP(pl1[i], pl2[j]) < mindist)
                            {
                                mindist = dist_PP(pl1[i], pl2[j]);
                                dist = (float)mindist;
                                p1 = pl1[i];
                                p2 = pl2[j];
                            }
                        }
                    }
                }
            }
            private PointF computeIntersections_LL(GraphLine RealL, GraphLine HorVL)
            {
                float A1 = RealL.EndPoint.P.Y - RealL.StartPoint.P.Y,
                                  B1 = RealL.StartPoint.P.X - RealL.EndPoint.P.X,
                                  C1 = RealL.EndPoint.P.X * RealL.StartPoint.P.Y - RealL.StartPoint.P.X * RealL.EndPoint.P.Y;
                float A2 = HorVL.EndPoint.P.Y - HorVL.StartPoint.P.Y,
                      B2 = HorVL.StartPoint.P.X - HorVL.EndPoint.P.X,
                      C2 = HorVL.EndPoint.P.X * HorVL.StartPoint.P.Y - HorVL.StartPoint.P.X * HorVL.EndPoint.P.Y;
                float m = A1 * B2 - A2 * B1;
                float x = (C2 * B1 - C1 * B2) / m;
                float y = (C1 * A2 - C2 * A1) / m;

                PointF ans = new PointF(x, y);
                double t1 = dist_PP(RealL.StartPoint.P, ans) + dist_PP(RealL.EndPoint.P, ans);
                if (dist_PP(RealL.StartPoint.P, RealL.EndPoint.P) - t1 > -0.05)
                    return new PointF(x, y);
                else
                    return new PointF(-1, -1);
            }
            private double dist_PP(PointF p1, PointF p2)
            {
                float x = p2.X - p1.X;
                float y = p2.Y - p1.Y;
                return Math.Sqrt(x * x + y * y);
            }
            private List<PointF> computeIntersections_LC(PointF[] bez, PointF[] line)
            {
                var X = new double[2];

                var A = line[1].Y - line[0].Y;      //A=y2-y1
                var B = line[0].X - line[1].X;      //B=x1-x2
                var C = line[0].X * (line[0].Y - line[1].Y) +
                      line[0].Y * (line[1].X - line[0].X);  //C=x1*(y1-y2)+y1*(x2-x1)

                var bx = bezierCoeffs(bez[0].X, bez[1].X, bez[2].X, bez[3].X);
                var by = bezierCoeffs(bez[0].Y, bez[1].Y, bez[2].Y, bez[3].Y);

                var P = new double[4];
                P[0] = A * bx[0] + B * by[0];       /*t^3*/
                P[1] = A * bx[1] + B * by[1];       /*t^2*/
                P[2] = A * bx[2] + B * by[2];       /*t*/
                P[3] = A * bx[3] + B * by[3] + C;   /*1*/

                var r = cubicRoots(P);
                List<PointF> ans = new List<PointF>();
                /*verify the roots are in bounds of the linear segment*/
                for (var i = 0; i < 3; i++)
                {
                    var t = r[i];

                    X[0] = bx[0] * t * t * t + bx[1] * t * t + bx[2] * t + bx[3];
                    X[1] = by[0] * t * t * t + by[1] * t * t + by[2] * t + by[3];

                    /*above is intersection point assuming infinitely long line segment,
                      make sure we are also in bounds of the line*/
                    double s = 0;
                    if ((line[1].X - line[0].X) != 0)           /*if not vertical line*/
                        s = (X[0] - line[0].X) / (line[1].X - line[0].X);
                    else
                        s = (X[1] - line[0].Y) / (line[1].Y - line[0].Y);

                    /*in bounds?*/
                    if (t < 0 || t > 1.0 || s < 0 || s > 1.0)
                    {
                        X[0] = -100;  /*move off screen*/
                        X[1] = -100;
                    }
                    else
                    {
                        ans.Add(new PointF((float)X[0], (float)X[1]));
                    }
                }
                return ans;
            }
            private double[] cubicRoots(double[] P)
            {
                var a = P[0];
                var b = P[1];
                var c = P[2];
                var d = P[3];

                var A = b / a;
                var B = c / a;
                var C = d / a;

                double S = 0, T = 0, Im = 0;

                var Q = (3 * B - Math.Pow(A, 2)) / 9;
                var R = (9 * A * B - 27 * C - 2 * Math.Pow(A, 3)) / 54;
                var D = Math.Pow(Q, 3) + Math.Pow(R, 2);    // polynomial discriminant

                var t = new double[3];

                if (D >= 0)                                 // complex or duplicate roots
                {
                    S = ((R + Math.Sqrt(D)) < 0 ? -1 : 1) * Math.Pow(Math.Abs(R + Math.Sqrt(D)), ((double)1 / 3));
                    T = ((R - Math.Sqrt(D)) < 0 ? -1 : 1) * Math.Pow(Math.Abs(R - Math.Sqrt(D)), ((double)1 / 3));

                    t[0] = -A / 3 + (S + T);                    // real root
                    t[1] = -A / 3 - (S + T) / 2;                  // real part of complex root
                    t[2] = -A / 3 - (S + T) / 2;                  // real part of complex root
                    Im = Math.Abs(Math.Sqrt(3) * (S - T) / 2);    // complex part of root pair   

                    /*discard complex roots*/
                    if (Im != 0)
                    {
                        t[1] = -1;
                        t[2] = -1;
                    }


                }
                else                                          // distinct real roots
                {
                    var th = Math.Acos(R / Math.Sqrt(-Math.Pow(Q, 3)));

                    t[0] = 2 * Math.Sqrt(-Q) * Math.Cos(th / 3) - A / 3;
                    t[1] = 2 * Math.Sqrt(-Q) * Math.Cos((th + 2 * Math.PI) / 3) - A / 3;
                    t[2] = 2 * Math.Sqrt(-Q) * Math.Cos((th + 4 * Math.PI) / 3) - A / 3;
                    Im = 0.0;
                }

                /*discard out of spec roots*/
                for (var i = 0; i < 3; i++)
                    if (t[i] < 0 || t[i] > 1.0) t[i] = -1;
                return t;
            }
            private double[] bezierCoeffs(double P0, double P1, double P2, double P3)
            {
                var Z = new double[4];
                Z[0] = -P0 + 3 * P1 + -3 * P2 + P3;
                Z[1] = 3 * P0 - 6 * P1 + 3 * P2;
                Z[2] = -3 * P0 + 3 * P1;
                Z[3] = P0;
                return Z;
            }
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
                    PointF[] temp = a.to_cubic_bezier();
                    for(int i = 0; i < 4; i++)
                    {
                        temp[i].X = temp[i].X - p.X + size;
                        temp[i].Y = temp[i].Y - p.Y + size;
                    }
                    g.DrawBezier(new Pen(Color.Green, 5), temp[0], temp[1], temp[2], temp[3]);
                }

                if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    return a;
            }
            return null;
        }
        public GraphArc FindArcControlByPoint(List<GraphArc> arcs,Point p)
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
                            g.DrawRectangle(new Pen(Color.Green, 5), a.getControlPoint().X - p.X + size - 2, a.getControlPoint().Y - p.Y + size - 2, size, size);
                        }
                        if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                        {
                            return a;
                        }
                    }
                }
            }
            return null;
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
        static public GraphText FindTextPyPoint(List<GraphText> texts, Point p)
        {
            var size = 5;
            var buffer = new Bitmap(size * 2, size * 2);
            foreach (var t in texts)
            {
                //draw each line on small region around current point p and check pixel in point p 

                using (var g = Graphics.FromImage(buffer))
                {
                    g.Clear(Color.Black);
                    g.DrawString(t.S, new Font("新細明體", 12, FontStyle.Bold), Brushes.Green, new PointF(t.P.X - p.X + size, t.P.Y - p.Y + size));
                }

                if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    return t;
            }
            return null;
        }
        #endregion
        
        public main()
        {
            InitializeComponent();
            toolStripTextBox1.Enabled = false;
            toolStripTextBox1.Text = "1";
            toolStripTextBox2.Enabled = false;
            toolStripTextBox2.Text = "4";
            toolStripComboBox1.Enabled = false;
            toolStripComboBox1.SelectedIndex = 0;
            Text_Temp = textBox1;
            PorpertyList.Add(LineGroupBox);
            PorpertyList.Add(CurveGroupBox);
            PorpertyList.Add(PathGroupBox);
            PorpertyList[0].Location = new Point(0, 0);
            PorpertyList[1].Location = new Point(0, 0);
            PorpertyList[2].Location = new Point(0, 0);
            PorpertyList[0].Visible = false;
            PorpertyList[1].Visible = false;
            PorpertyList[2].Visible = false;
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
            foreach(var g in PorpertyList)
            {
                g.Width = splitContainer1.Panel2.Width;
                g.Height = splitContainer1.Panel2.Height;
            }
        }
        private void splitContainer1_Panel1_SizeChanged(object sender, EventArgs e)
        {
            tabControl1.Width = splitContainer1.Panel1.Width;
            tabControl1.Height = splitContainer1.Panel1.Height;
            foreach (var g in PorpertyList)
            {
                g.Width = splitContainer1.Panel2.Width;
                g.Height = splitContainer1.Panel2.Height;
            }
        }
        private void splitContainer1_Panel2_SizeChanged(object sender, EventArgs e)
        {
            tabControl1.Width = splitContainer1.Panel1.Width;
            tabControl1.Height = splitContainer1.Panel1.Height;
            foreach (var g in PorpertyList)
            {
                g.Width = splitContainer1.Panel2.Width;
                g.Height = splitContainer1.Panel2.Height;
            }
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
            data += dout.PointL.Count + "\n";
            foreach (var p in dout.PointL)
            {
                data += p.P.X + " " + p.P.Y + "\n";
            }
            data += dout.LineL.Count + "\n";
            foreach (var l in dout.LineL)
            {
                data += dout.PointL.FindIndex(x => x == l.StartPoint) + " " + dout.PointL.FindIndex(x => x == l.EndPoint)
                    + " " + (l.isSeam ? "T " : "F ") + l.Seam + "\n";
            }
            data += dout.CurveL.Count + "\n";
            foreach(var c in dout.CurveL)
            {
                data += c.path.Count + " " + (c.isSeam ? "T " : "F ") + c.Seam + "\n";
                for(int i = 0; i < c.path.Count; i++)
                {
                    data += dout.PointL.FindIndex(x => x == c.path[i]) + " " + c.disFirst[i].X + " " + c.disFirst[i].Y + " " 
                        + c.disSecond[i].X + " " + c.disSecond[i].Y + " " + c.type[i] + "\n";
                }
            }
            data += dout.ArcL.Count + "\n";
            foreach(var a in dout.ArcL)
            {
                data += dout.PointL.FindIndex(x => x == a.StartPoint) + " " + dout.PointL.FindIndex(x => x == a.EndPoint) 
                    +  " " + a.getControlPoint().X + " " + a.getControlPoint().Y + "\n";
            }
            data += dout.GroupL.Count + "\n";
            foreach(var g in dout.GroupL)
            {
                data += WriteGroup(dout, g);
            }
            data += dout.TextL.Count + "\n";
            foreach(var t in dout.TextL)
            {
                data += t.P.X + " " + t.P.Y + " " + t.S + "\n";
            }
            data += dout.PathL.Count + "\n";
            foreach (var pa in dout.PathL)
            {
                data += WriteGroup(dout, pa);
            }
            return data;
        }
        private static string WriteGroup(TabpageData dout, GraphGroup g)
        {
            string data = "";
            data += g.P.Count + "\n";
            foreach (var p in g.P)
            {
                data += dout.PointL.FindIndex(x => x == p) + "\n";
            }
            data += g.L.Count + "\n";
            foreach (var l in g.L)
            {
                data += dout.LineL.FindIndex(x => x == l) + "\n";
            }
            data += g.C.Count + "\n";
            foreach (var c in g.C)
            {
                data += dout.CurveL.FindIndex(x => x == c) + "\n";
            }
            data += g.A.Count + "\n";
            foreach (var a in g.A)
            {
                data += dout.ArcL.FindIndex(x => x == a) + "\n";
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
                var TempTabpageData = new List<TabpageData>();
                try
                {
                    StreamReader sr = new StreamReader(openFileDialog1.FileName);
                    string s = sr.ReadLine();
                    int size;
                    int.TryParse(s, out size);
                    for (int i = 0; i < size; i++)
                    {
                        TempTabpageData.Add(ReadTabData(sr));
                        if (TempTabpageData[i] == null)
                            return;
                    }
                    sr.Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "讀檔錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                TabpageDataList = TempTabpageData;
                TabpagesList = new List<TabPage>();
                tabControl1.TabPages.Clear();
                foreach(var td in TabpageDataList)
                {
                    TabPage tp = new TabPage(td.TabpageName);
                    td.Undo = new List<TabpageData>();
                    TabpagesList.Add(tp);
                    tabControl1.TabPages.Add(tp);
                    PointsList = td.PointL;
                    LineList = td.LineL;
                    CurveList = td.CurveL;
                    ArcList = td.ArcL;
                    GroupList = td.GroupL;
                    PathList = td.PathL;
                    TextList = td.TextL;
                    Undo_Data = td.Undo;
                    int tdindex = TabpageDataList.FindIndex(x => x == td);
                    Push_Undo_Data(tdindex);
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
                PointsList = TabpageDataList[0].PointL;
                LineList = TabpageDataList[0].LineL;
                CurveList = TabpageDataList[0].CurveL;
                ArcList = TabpageDataList[0].ArcL;
                GroupList = TabpageDataList[0].GroupL;
                PathList = TabpageDataList[0].PathL;
                TextList = TabpageDataList[0].TextL;
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
            try
            {
                TabpageData td = new TabpageData();
                string s = sr.ReadLine();
                string[] sarr = s.Split(' ');
                td.TabpageName = sarr[0];
                int width; int.TryParse(sarr[1], out width); td.width = width;
                int height; int.TryParse(sarr[2], out height); td.height = height;

                s = sr.ReadLine();
                int pnum; int.TryParse(s, out pnum);
                td.PointL = new List<GraphPoint>();
                for (int i = 0; i < pnum; i++)
                {
                    s = sr.ReadLine();
                    sarr = s.Split(' ');
                    float x; float.TryParse(sarr[0], out x);
                    float y; float.TryParse(sarr[1], out y);
                    GraphPoint p = new GraphPoint(x, y);
                    td.PointL.Add(p);
                }

                s = sr.ReadLine();
                int lnum; int.TryParse(s, out lnum);
                td.LineL = new List<GraphLine>();
                for (int i = 0; i < lnum; i++)
                {
                    s = sr.ReadLine();
                    sarr = s.Split(' ');
                    int start; int.TryParse(sarr[0], out start);
                    int end; int.TryParse(sarr[1], out end);
                    bool isSeam = sarr[2] == "T";
                    float Seam;float.TryParse(sarr[3], out Seam);
                    GraphLine l = new GraphLine(td.PointL[start], td.PointL[end]);
                    l.isSeam = isSeam;
                    l.Seam = Seam;
                    td.PointL[start].Relative++;
                    td.PointL[end].Relative++;
                    td.LineL.Add(l);
                }

                s = sr.ReadLine();
                int cnum; int.TryParse(s, out cnum);
                td.CurveL = new List<GraphCurve>();
                for (int i = 0; i < cnum; i++)
                {
                    s = sr.ReadLine();
                    sarr = s.Split(' ');
                    int pathnum; int.TryParse(sarr[0], out pathnum);
                    bool isSeam = sarr[1] == "T";
                    float Seam; float.TryParse(sarr[2], out Seam);
                    GraphCurve c = new GraphCurve();
                    for (int j = 0; j < pathnum; j++)
                    {
                        s = sr.ReadLine();
                        sarr = s.Split(' ');
                        int pid; int.TryParse(sarr[0], out pid);
                        c.path.Add(td.PointL[pid]);
                        td.PointL[pid].Relative++;

                        float fx; float.TryParse(sarr[1], out fx);
                        float fy; float.TryParse(sarr[2], out fy);
                        c.disFirst.Add(new PointF(fx, fy));

                        float sx; float.TryParse(sarr[3], out sx);
                        float sy; float.TryParse(sarr[4], out sy);
                        c.disSecond.Add(new PointF(sx, sy));

                        int type; int.TryParse(sarr[5], out type);
                        c.type.Add(type);
                    }
                    c.isSeam = isSeam;
                    c.Seam = Seam;
                    td.CurveL.Add(c);
                }

                s = sr.ReadLine();
                int anum; int.TryParse(s, out anum);
                td.ArcL = new List<GraphArc>();
                for(int i = 0; i < anum; i++)
                {
                    s = sr.ReadLine();
                    sarr = s.Split(' ');

                    int astart; int.TryParse(sarr[0], out astart);
                    GraphPoint st = td.PointL[astart];
                    td.PointL[astart].Relative++;

                    int aend; int.TryParse(sarr[1], out aend);
                    GraphPoint en = td.PointL[aend];
                    td.PointL[aend].Relative++;

                    float x; float.TryParse(sarr[2], out x);
                    float y; float.TryParse(sarr[3], out y);
                    PointF cp = new PointF(x, y);

                    GraphArc a = new GraphArc(st, en, cp);

                    td.ArcL.Add(a);
                }

                s = sr.ReadLine();
                int gnum; int.TryParse(s, out gnum);
                td.GroupL = new List<GraphGroup>();
                for (int i = 0; i < gnum; i++)
                {
                    td.GroupL.Add(ReadGroup(sr, td));
                }

                s = sr.ReadLine();
                int tnum; int.TryParse(s, out tnum);
                td.TextL = new List<GraphText>();
                for (int i = 0; i < tnum; i++)
                {
                    s = sr.ReadLine();
                    sarr = s.Split(' ');
                    float x; float.TryParse(sarr[0], out x);
                    float y; float.TryParse(sarr[1], out y);
                    string temps = "";
                    for (int j = 2; j < sarr.Count(); j++)
                        temps += sarr[j];
                    td.TextL.Add(new GraphText { P = new PointF(x, y), S = temps });
                }

                s = sr.ReadLine();
                int panum; int.TryParse(s, out panum);
                td.PathL = new List<GraphGroup>();
                for (int i = 0; i < panum; i++)
                {
                    td.PathL.Add(ReadGroup(sr, td));
                }

                return td;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "讀檔錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
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
                g.P.Add(td.PointL[id]);
            }

            s = sr.ReadLine();
            int lnum; int.TryParse(s, out lnum);
            for (int i = 0; i < lnum; i++)
            {
                s = sr.ReadLine();
                int id; int.TryParse(s, out id);
                if (id == -1)
                    g.L.Add(null);
                else
                    g.L.Add(td.LineL[id]);
            }

            s = sr.ReadLine();
            int cnum; int.TryParse(s, out cnum);
            for (int i = 0; i < cnum; i++)
            {
                s = sr.ReadLine();
                int id; int.TryParse(s, out id);
                if (id == -1)
                    g.C.Add(null);
                else
                    g.C.Add(td.CurveL[id]);
            }

            s = sr.ReadLine();
            int anum; int.TryParse(s, out anum);
            for (int i = 0; i < anum; i++)
            {
                s = sr.ReadLine();
                int id; int.TryParse(s, out id);
                g.A.Add(td.ArcL[id]);
            }

            s = sr.ReadLine();
            int gnum; int.TryParse(s, out gnum);
            for (int i = 0; i < gnum; i++)
            {
                g.G.Add(ReadGroup(sr, td));
            }

            return g;
        }
        private void 新影像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Width = (int)(595 * ZoomSize);
            pictureBox1.Height = (int)(842 * ZoomSize);
            pictureBox1.Image = new Bitmap((int)(595 * ZoomSize), (int)(842 * ZoomSize));
            pictureBox2.Width = (int)((595 * ZoomSize))+20;
            pictureBox2.Height = (int)((842) * ZoomSize)+20;
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(Color.WhiteSmoke);
            PointsList = new List<GraphPoint>();
            LineList = new List<GraphLine>();
            CurveList = new List<GraphCurve>();
            ArcList = new List<GraphArc>();
            TextList = new List<GraphText>();
            GroupList = new List<GraphGroup>();
            PathList = new List<GraphGroup>();

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
            a.CurveL = CurveList;
            a.PointL = PointsList;
            a.LineL = LineList;
            a.ArcL = ArcList;
            a.GroupL = GroupList;
            a.TextL = TextList;
            a.PathL = PathList;
            a.Undo = Undo_Data;
            a.width = 595;
            a.height = 842;
            a.TabpageName = TabpagesList[0].Text;
            TabpageDataList.Add(a);
            Push_Undo_Data();

            pictureBox1.Visible = true;
            pictureBox2.Visible = true;
        }
        private void 調整大小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int a = TabpagesList.FindIndex(x => x == tabControl1.SelectedTab);
            Size f2 = new Size((int)(pictureBox1.Width / ZoomSize), (int)(pictureBox1.Height / ZoomSize));
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
        #region Pleated parameter
        GraphPoint M6_TempP1 = null;
        GraphPoint M6_TempP2 = null;
        GraphLine M6_TempL = null;
        GraphCurve M6_TempC = null;
        GraphGroup M6_TempPath = null;
        int M6_TempC_Index = -1;
        int M6_State = 0;
        #endregion
        #region Cutting parameter
        GraphPoint M7_TempP = null;
        GraphLine M7_TempL = null;
        GraphCurve M7_TempC = null;
        GraphGroup M7_TempPath = null;
        int M7_TempC_Index = -1;
        int M7_State = 0;
        #endregion
        #region PathDist parameter
        GraphPoint M8_TempP = null;
        GraphLine M8_TempL = null;
        GraphCurve M8_TempC = null;
        int M8_TempC_Index = -1;
        int M8_State = 0;
        #endregion

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.Clear(Color.White);

            Paint_Net(e);
            Paint_Seam(e);
            Paint_Lines(e);
            Paint_Points(e);
            Paint_Curves(e);
            Paint_Arcs(e);
            Paint_PathDist(e);
            Paint_Text(e);
            

            if (In_Path(SelectedLine, SelectedCurve) != null)
                Paint_Path(e);

            if (SelectedGroup != null)
                Paint_SelectedGroup(e);

            if (Moving != null)
                Paint_Mutiselect(e);

            if (is_Drowing)
                Paint_Drawing(e);

            if (Mouse_Mode == Mouse_Mode_Type.Pleated)
                Paint_Pleated(e);
            if (Mouse_Mode == Mouse_Mode_Type.Cutting)
                Paint_Cutting(e);
        }
        private void Paint_Net(PaintEventArgs e)
        {
            if (Net_Mode)
            {
                for (float i = 0; i < this.pictureBox1.Height; i += (float)SizeOfNet)
                {
                    var pen = new Pen(Color.LightSlateGray, 1);
                    e.Graphics.DrawLine(pen, new PointF(0, i * ZoomSize), new PointF(this.pictureBox1.Width, i * ZoomSize));
                    pen = new Pen(Color.LightGray, 1);
                    for (int j = 1; j <= DesityOfNet; j++)
                    {
                        e.Graphics.DrawLine(pen, new PointF(0, (i + (float)SizeOfNet / DesityOfNet * j) * ZoomSize), new PointF(this.pictureBox1.Width, (i + (float)SizeOfNet / DesityOfNet * j) * ZoomSize));
                    }
                }
                for (float i = 0; i < this.pictureBox1.Width; i += (float)SizeOfNet)
                {
                    var pen = new Pen(Color.LightSlateGray, 1);
                    e.Graphics.DrawLine(pen, new PointF(i * ZoomSize, 0), new PointF(i * ZoomSize, this.pictureBox1.Height));
                    pen = new Pen(Color.LightGray, 1);
                    for (int j = 1; j <= DesityOfNet; j++)
                    {
                        e.Graphics.DrawLine(pen, new PointF((i + (float)SizeOfNet / DesityOfNet * j) * ZoomSize, 0), new PointF((i + (float)SizeOfNet / DesityOfNet * j) * ZoomSize, this.pictureBox1.Height));
                    }
                }
            }
        }
        private void Paint_Lines(PaintEventArgs e)
        {
            foreach (var line in LineList)
            {
                var color = line.co;
                var size = 1;
                if (SelectedGroup != null)
                {
                    color = SelectedGroup.L.FindIndex(x => x == line) < 0 ? line.co : Color.Red;
                    size = SelectedGroup.L.FindIndex(x => x == line) < 0 ? 1 : 2;
                }
                var pen = new Pen(color, size);
                e.Graphics.DrawLine(pen, line.StartPoint.P.X * ZoomSize, line.StartPoint.P.Y * ZoomSize, line.EndPoint.P.X * ZoomSize, line.EndPoint.P.Y * ZoomSize);
            }
        }
        private void Paint_Points(PaintEventArgs e)
        {
            if (!hidepoints)
            {
                foreach (var p in PointsList)
                {
                    var color = p == SelectedPoint ? Color.Red : p.MarkL != null ? Color.Orange : Color.Black;
                    var size = p == SelectedPoint ? 2 : 1;
                    var pen = new Pen(color, size);
                    e.Graphics.DrawRectangle(pen, p.P.X * ZoomSize - 3, p.P.Y * ZoomSize - 3, 6, 6);
                }
            }
        }
        private void Paint_Curves(PaintEventArgs e)
        {
            foreach (var c in CurveList)
            {
                var color = c.co;
                var size = 1;
                if (SelectedGroup != null)
                {
                    color = SelectedGroup.C.FindIndex(x => x == c) < 0 ? c.co : Color.Red;
                    size = SelectedGroup.C.FindIndex(x => x == c) < 0 ? 1 : 2;
                }
                var pen = new Pen(color, size);
                List<PointF> t = GraphCurveToBez(c);
                for (int i = 0; i < t.Count; i++)
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
                                e.Graphics.DrawRectangle(rec, t[i + 1].X - 2, t[i + 1].Y - 2, 4, 4);
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
        }
        private void Paint_Arcs(PaintEventArgs e)
        {
            foreach (var a in ArcList)
            {
                var color = a.co;
                var size = 1;
                if (SelectedGroup != null)
                {
                    color = SelectedGroup.A.FindIndex(x => x == a) < 0 ? a.co : Color.Red;
                    size = SelectedGroup.A.FindIndex(x => x == a) < 0 ? 1 : 2;
                }
                var pen = new Pen(color, size);
                var arr = a.to_cubic_bezier();
                for(int i = 0; i < 4; i++)
                {
                    arr[i].X *= ZoomSize;
                    arr[i].Y *= ZoomSize;
                }
                e.Graphics.DrawBezier(pen, arr[0], arr[1], arr[2], arr[3]);
                if (SelectedGroup != null)
                {
                    if (SelectedGroup.C.Count == 0 && SelectedGroup.L.Count == 0 && SelectedGroup.A.FindIndex(x => x == a) >= 0 && SelectedGroup.A.Count == 1)
                    {
                        var l = new Pen(Color.LightSkyBlue, 1);
                        var rec = new Pen(Color.Orange, 1);
                        float x0 = a.StartPoint.P.X * ZoomSize;
                        float y0 = a.StartPoint.P.Y * ZoomSize;
                        float x1 = a.getControlPoint().X * ZoomSize;
                        float y1 = a.getControlPoint().Y * ZoomSize;
                        float x2 = a.EndPoint.P.X * ZoomSize;
                        float y2 = a.EndPoint.P.Y * ZoomSize;
                        e.Graphics.DrawLine(l, new PointF(x0, y0), new PointF(x1, y1));
                        e.Graphics.DrawLine(l, new PointF(x1, y1), new PointF(x2, y2));
                        e.Graphics.DrawRectangle(rec, x1 - 2, y1 - 2, 4, 4);
                    }
                }
            }
        }
        private void Paint_Text(PaintEventArgs e)
        {
            foreach (var s in TextList)
            {
                var fo = new Font("新細明體", 12);
                e.Graphics.DrawString(s.S, fo, Brushes.Black, s.P.X * ZoomSize, s.P.Y * ZoomSize);
            }
        }
        private void Paint_Mutiselect(PaintEventArgs e)
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
        private void Paint_Path(PaintEventArgs e)
        {
            GraphGroup path = In_Path(SelectedLine, SelectedCurve);
            if (!hidepoints)
            {
                foreach (var p in path.P)
                {
                    var color = p == SelectedPoint ? Color.Green : Color.Black;
                    var size = p == SelectedPoint ? 2 : 1;
                    var pen = new Pen(color, size);
                    e.Graphics.DrawRectangle(pen, p.P.X * ZoomSize - 3, p.P.Y * ZoomSize - 3, 6, 6);
                }
            }
            foreach (var line in path.L)
            {
                if (line != null)
                {
                    var color = Color.Green;
                    var size = 2;
                    var pen = new Pen(color, size);
                    e.Graphics.DrawLine(pen, line.StartPoint.P.X * ZoomSize, line.StartPoint.P.Y * ZoomSize, line.EndPoint.P.X * ZoomSize, line.EndPoint.P.Y * ZoomSize);
                }
            }
            foreach (var c in path.C)
            {
                if (c != null)
                {
                    var color = Color.Green;
                    var size = 2;
                    var pen = new Pen(color, size);
                    List<PointF> t = GraphCurveToBez(c);
                    for (int i = 0; i < t.Count; i++)
                    {
                        t[i] = new PointF(t[i].X * ZoomSize, t[i].Y * ZoomSize);
                    }
                    e.Graphics.DrawBeziers(pen, t.ToArray());
                }
            }
        }
        private void Paint_SelectedGroup(PaintEventArgs e)
        {
            if (SelectedGroup.A.Count + SelectedGroup.L.Count + SelectedGroup.C.Count > 1)
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
        private void Paint_Drawing(PaintEventArgs e)
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
            if (Mouse_Mode == Mouse_Mode_Type.Line)
            {
                if (SelectedPoint == null)
                    e.Graphics.DrawLine(pe, previous_point.P.X * ZoomSize, previous_point.P.Y * ZoomSize, p.X * ZoomSize, p.Y * ZoomSize);
                else
                    e.Graphics.DrawLine(pe, previous_point.P.X * ZoomSize, previous_point.P.Y * ZoomSize, SelectedPoint.P.X * ZoomSize, SelectedPoint.P.Y * ZoomSize);
            }
            else if (Mouse_Mode == Mouse_Mode_Type.Curve)
            {
                GraphCurve c = PathToCurve(PreviousCruve.path, 1, p);
                List<PointF> t = GraphCurveToBez(c);
                for (int i = 0; i < t.Count; i++)
                {
                    t[i] = new PointF(t[i].X * ZoomSize, t[i].Y * ZoomSize);
                }
                e.Graphics.DrawBeziers(pe, t.ToArray());
            }
            else if (Mouse_Mode == Mouse_Mode_Type.Arc)
            {
                GraphArc a = new GraphArc(previous_point, new GraphPoint(p.X, p.Y));
                var arr = a.to_cubic_bezier();
                for(int i = 0; i < 4; i++)
                {
                    arr[i].X *= ZoomSize;
                    arr[i].Y *= ZoomSize;
                }
                e.Graphics.DrawBezier(pe, arr[0], arr[1], arr[2], arr[3]);
            }
        }
        private void Paint_Pleated(PaintEventArgs e)
        {
            if (M6_State == 2)
            {
                PointF p = For_Paint;
                var pe = new Pen(Color.Black, 1);
                e.Graphics.DrawRectangle(pe, M6_TempP1.P.X * ZoomSize - 2, M6_TempP1.P.Y * ZoomSize - 2, 4, 4);
                e.Graphics.DrawRectangle(pe, M6_TempP2.P.X * ZoomSize - 2, M6_TempP2.P.Y * ZoomSize - 2, 4, 4);
                e.Graphics.DrawLine(pe, M6_TempP1.P.X * ZoomSize, M6_TempP1.P.Y * ZoomSize, M6_TempP2.P.X * ZoomSize, M6_TempP2.P.Y * ZoomSize);
                e.Graphics.DrawLine(pe, M6_TempP2.P.X * ZoomSize, M6_TempP2.P.Y * ZoomSize, p.X * ZoomSize, p.Y * ZoomSize);
                GraphLine l = FindLineByPoint(LineList, For_Paint);
                GraphCurve c;
                int index;
                FindCurveByPoint(CurveList, For_Paint, out c, out index);
                pe = new Pen(Color.Red, 2);
                if (l != null)
                {
                    e.Graphics.DrawLine(pe, l.StartPoint.P.X * ZoomSize, l.StartPoint.P.Y * ZoomSize, l.EndPoint.P.X * ZoomSize, l.EndPoint.P.Y * ZoomSize);
                }
                else if (c != null)
                {
                    List<PointF> t = GraphCurveToBez(c);
                    for (int i = 0; i < t.Count; i++)
                    {
                        t[i] = new PointF(t[i].X * ZoomSize, t[i].Y * ZoomSize);
                    }
                    e.Graphics.DrawBeziers(pe, t.ToArray());
                }
            }
            if (M6_State == 1)
            {
                PointF p = For_Paint;
                if (Net_Mode)
                {
                    p.X += (float)SizeOfNet / DesityOfNet / 2;
                    p.Y += (float)SizeOfNet / DesityOfNet / 2;
                    p.X -= p.X % ((float)SizeOfNet / DesityOfNet);
                    p.Y -= p.Y % ((float)SizeOfNet / DesityOfNet);
                }
                var pe = new Pen(Color.Black, 1);
                e.Graphics.DrawRectangle(pe, M6_TempP1.P.X * ZoomSize - 2, M6_TempP1.P.Y * ZoomSize - 2, 4, 4);
                e.Graphics.DrawLine(pe, M6_TempP1.P.X * ZoomSize, M6_TempP1.P.Y * ZoomSize, p.X * ZoomSize, p.Y * ZoomSize);
            }
            if(M6_State == 0)
            {
                GraphLine l = FindLineByPoint(LineList, For_Paint);
                GraphCurve c;
                int index;
                FindCurveByPoint(CurveList, For_Paint, out c, out index);
                var pe = new Pen(Color.Red, 2);
                if (l != null)
                {
                    e.Graphics.DrawLine(pe, l.StartPoint.P.X * ZoomSize, l.StartPoint.P.Y * ZoomSize, l.EndPoint.P.X * ZoomSize, l.EndPoint.P.Y * ZoomSize);
                }
                else if (c != null)
                {
                    List<PointF> t = GraphCurveToBez(c);
                    for (int i = 0; i < t.Count; i++)
                    {
                        t[i] = new PointF(t[i].X * ZoomSize, t[i].Y * ZoomSize);
                    }
                    e.Graphics.DrawBeziers(pe, t.ToArray());
                }
            }
        }
        private void Paint_Cutting(PaintEventArgs e)
        {
            if (M7_State == 1)
            {
                PointF p = For_Paint;
                var pe = new Pen(Color.Black, 1);
                e.Graphics.DrawRectangle(pe, M7_TempP.P.X * ZoomSize - 2, M7_TempP.P.Y * ZoomSize - 2, 4, 4);
                e.Graphics.DrawLine(pe, M7_TempP.P.X * ZoomSize, M7_TempP.P.Y * ZoomSize, p.X * ZoomSize, p.Y * ZoomSize);
                GraphLine l = FindLineByPoint(LineList, For_Paint);
                GraphCurve c;
                int index;
                FindCurveByPoint(CurveList, For_Paint, out c, out index);
                pe = new Pen(Color.Red, 2);
                if (l != null)
                {
                    e.Graphics.DrawLine(pe, l.StartPoint.P.X * ZoomSize, l.StartPoint.P.Y * ZoomSize, l.EndPoint.P.X * ZoomSize, l.EndPoint.P.Y * ZoomSize);
                }
                else if (c != null)
                {
                    List<PointF> t = GraphCurveToBez(c);
                    for (int i = 0; i < t.Count; i++)
                    {
                        t[i] = new PointF(t[i].X * ZoomSize, t[i].Y * ZoomSize);
                    }
                    e.Graphics.DrawBeziers(pe, t.ToArray());
                }
            }
            if (M7_State == 0)
            {
                GraphLine l = FindLineByPoint(LineList, For_Paint);
                GraphCurve c;
                int index;
                FindCurveByPoint(CurveList, For_Paint, out c, out index);
                var pe = new Pen(Color.Red, 2);
                if (l != null)
                {
                    e.Graphics.DrawLine(pe, l.StartPoint.P.X * ZoomSize, l.StartPoint.P.Y * ZoomSize, l.EndPoint.P.X * ZoomSize, l.EndPoint.P.Y * ZoomSize);
                }
                else if (c != null)
                {
                    List<PointF> t = GraphCurveToBez(c);
                    for (int i = 0; i < t.Count; i++)
                    {
                        t[i] = new PointF(t[i].X * ZoomSize, t[i].Y * ZoomSize);
                    }
                    e.Graphics.DrawBeziers(pe, t.ToArray());
                }
            }
        }
        private void Paint_Seam(PaintEventArgs e)
        {
            var pe = new Pen(Color.Gray, 1);
            float[] dash = { 5, 5 };
            pe.DashPattern = dash;
            foreach(var path in PathList)
            {
                float minx, miny, maxx, maxy;
                minx = path.P[0].P.X;
                miny = path.P[0].P.Y;
                maxx = path.P[0].P.X;
                maxy = path.P[0].P.Y;
                foreach (var po in path.P)
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
                List<PointF> poly_vert = new List<PointF>();
                List<float> distList = new List<float>();
                bool[] check = new bool[path.L.Count]; for (int i = 0; i < check.Count(); i++) check[i] = false;
                GraphPoint p = path.P[0];
                poly_vert.Add(p.P);
                do
                {
                    List<GraphLine> l = new List<GraphLine>();
                    List<GraphCurve> c = new List<GraphCurve>();
                    l = path.L.FindAll(x => x != null ? (x.StartPoint == p || x.EndPoint == p) : false);
                    c = path.C.FindAll(x => x != null ? (x.path[0] == p || x.path.Last() == p) : false);
                    if (l.Count > 0)
                    {
                        if (check[path.L.FindIndex(x => x == l[0])] == true)
                            l.RemoveAt(0);
                    }
                    if (c.Count > 0)
                    {
                        if (check[path.C.FindIndex(x => x == c[0])] == true)
                            c.RemoveAt(0);
                    }
                    if (l.Count > 0)
                    {
                        if (l[0].StartPoint == p)
                            p = l[0].EndPoint;
                        else
                            p = l[0].StartPoint;
                        distList.Add(l[0].Seam);
                        poly_vert.Add(p.P);
                        check[path.L.FindIndex(x => x == l[0])] = true;
                    }
                    else if (c.Count > 0)
                    {
                        List<PointF> cPL = DevideCurve(c[0]);
                        if (c[0].path[0] == p)
                        {
                            p = c[0].path.Last();
                            for (int i = 1; i < cPL.Count; i++)
                            {
                                poly_vert.Add(cPL[i]);
                                distList.Add(c[0].Seam);
                            }
                        }
                        else
                        {
                            p = c[0].path[0];
                            for (int i = cPL.Count - 2; i >= 0; i--)
                            {
                                poly_vert.Add(cPL[i]);
                                distList.Add(c[0].Seam);
                            }
                        }
                        check[path.C.FindIndex(x => x == c[0])] = true;
                    }
                } while (p != path.P[0]);
                poly_vert.RemoveAt(0);
                List<PointF> forextend = new List<PointF>();
                forextend.Add(poly_vert[0]);
                float pre_m = (poly_vert[1].X - poly_vert[0].X) / (poly_vert[1].Y - poly_vert[0].X);
                for(int i = 1; i < poly_vert.Count; i++)
                {
                    int next = (i + 1) % poly_vert.Count;
                    float now_m = (poly_vert[next].X - poly_vert[i].X) / (poly_vert[next].Y - poly_vert[i].X);
                    if (now_m == pre_m)
                        forextend.Add(new PointF(poly_vert[i].X + 1, poly_vert[i].Y + 1));
                    else
                        forextend.Add(poly_vert[i]);
                    pre_m = now_m;
                }
                List<PointF> todrawl = extend_polygon(forextend, distList);
                for (int i = 0; i < todrawl.Count; i++)
                {
                    e.Graphics.DrawLine(pe, todrawl[i].X * ZoomSize, todrawl[i].Y * ZoomSize, todrawl[(i + 1) % todrawl.Count].X * ZoomSize, todrawl[(i + 1) % todrawl.Count].Y * ZoomSize);
                }
            }
        }
        private List<PointF> DevideCurve(GraphCurve c)
        {
            List<PointF> ans = new List<PointF>();
            for (int i = 0; i < c.path.Count - 1; i++)
            {
                PointF p1 = c.path[i].P, p2 = c.path[i + 1].P;
                PointF c1 = new PointF(p1.X + c.disSecond[i].X, p1.Y + c.disSecond[i].Y), c2 = new PointF(p2.X + c.disFirst[i + 1].X, p2.Y + c.disFirst[i + 1].Y);
                for (double j = 0; j < 1;j+=0.04)
                {
                    double x = p1.X * Math.Pow(1 - j, 3) +
                           3 * c1.X * Math.Pow(1 - j, 2) * j +
                           3 * c2.X * Math.Pow(j, 2) * (1 - j) +
                               p2.X * Math.Pow(j, 3);
                    double y = p1.Y * Math.Pow(1 - j, 3) +
                           3 * c1.Y * Math.Pow(1 - j, 2) * j +
                           3 * c2.Y * Math.Pow(j, 2) * (1 - j) +
                               p2.Y * Math.Pow(j, 3);
                    ans.Add(new PointF((float)x, (float)y));
                }
            }
            ans.Add(c.path.Last().P);
            return ans;
        }
        private List<PointF> extend_polygon(List<PointF> pList, List<float> distList)
        {
            List<PointF> DpList = new List<PointF>();
            List<PointF> nDpList = new List<PointF>();
            List<PointF> newList = new List<PointF>();


            int i, index, count;
            count = pList.Count;
            PointF p2d = new PointF(), p2 = new PointF();
            PointF p3d = new PointF(), p3 = new PointF();
            for (i = 0; i < count; i++)
            {
                index = (i + 1) % count;
                p2d = new PointF(pList[index].X - pList[i].X, pList[index].Y - pList[i].Y);
                DpList.Add(p2d);
            }
            // 初始化ndpList，单位化两顶点向量差
            float r;
            for (i = 0; i < count; i++)
            {
                r = (float)Math.Sqrt(DpList[i].X * DpList[i].X + DpList[i].Y * DpList[i].Y);
                r = 1 / r;
                p2d = new PointF(DpList[i].X * r, DpList[i].Y * r);
                nDpList.Add(p2d);
            }
            // 计算新顶点， 注意参数dist为负是向内收缩， 为正是向外扩张
            //上述说法只是对于顺时针而言
            float area = 0;
            float lenth;
            int startindex, endindex;
            List<GraphLine> lList = new List<GraphLine>();
            for (i = 0; i < count; i++)
            {
                startindex = i == 0 ? count - 1 : i - 1;
                endindex = i;
                area += pList[startindex].X * pList[endindex].Y - pList[startindex].Y * pList[endindex].X;
            }
            area = area < 0 ? 1 : -1;
            for (i = 0; i < count; i++)
            {
                startindex = i == 0 ? count - 1 : i - 1;
                endindex = i;
                float sina = (nDpList[startindex].X * nDpList[endindex].Y - nDpList[startindex].Y * nDpList[endindex].X) * area;
                if (sina == 0)
                {
                    nDpList[endindex] = new PointF(nDpList[endindex].X + 0.001F, nDpList[endindex].Y + 0.001F);
                    sina = (nDpList[startindex].X * nDpList[endindex].Y - nDpList[startindex].Y * nDpList[endindex].X) * area;
                }
                lenth = distList[i] / sina;
                p2d = new PointF(nDpList[endindex].X - nDpList[startindex].X, nDpList[endindex].Y - nDpList[startindex].Y);
                p2 = new PointF(pList[i].X + p2d.X * lenth, pList[i].Y + p2d.Y * lenth);

                startindex = (startindex - 1 + count) % count;
                endindex = (endindex - 1 + count) % count;
                sina = (nDpList[startindex].X * nDpList[endindex].Y - nDpList[startindex].Y * nDpList[endindex].X) * area;
                if (sina == 0)
                {
                    nDpList[endindex] = new PointF(nDpList[endindex].X + 0.001F, nDpList[endindex].Y + 0.001F);
                    sina = (nDpList[startindex].X * nDpList[endindex].Y - nDpList[startindex].Y * nDpList[endindex].X) * area;
                }
                lenth = distList[i] / sina;
                p3d = new PointF(nDpList[endindex].X - nDpList[startindex].X, nDpList[endindex].Y - nDpList[startindex].Y);
                p3 = new PointF(pList[i].X + p3d.X * lenth, pList[i].Y + p3d.Y * lenth);
                if(distList[i] != 0)
                    lList.Add(new GraphLine(new GraphPoint(p3.X, p3.Y), new GraphPoint(p2.X, p2.Y)));
                else
                    lList.Add(new GraphLine(new GraphPoint(pList[i].X, pList[i].Y), new GraphPoint(pList[(i - 1 + count) % count].X, pList[(i - 1 + count) % count].Y)));
            }
            for (i = 0; i < count; i++)
            {
                var l1 = lList[i]; var l2 = lList[(i + 1) % count];
                float A1 = l1.EndPoint.P.Y - l1.StartPoint.P.Y,
                                  B1 = l1.StartPoint.P.X - l1.EndPoint.P.X,
                                  C1 = l1.EndPoint.P.X * l1.StartPoint.P.Y - l1.StartPoint.P.X * l1.EndPoint.P.Y;
                float A2 = l2.EndPoint.P.Y - l2.StartPoint.P.Y,
                      B2 = l2.StartPoint.P.X - l2.EndPoint.P.X,
                      C2 = l2.EndPoint.P.X * l2.StartPoint.P.Y - l2.StartPoint.P.X * l2.EndPoint.P.Y;
                float m = A1 * B2 - A2 * B1;
                if (m == 0)
                {
                    PointF l1s = l1.StartPoint.P, l1e = l1.EndPoint.P, l2s = l2.StartPoint.P, l2e = l2.EndPoint.P;
                    if (l1s == l2s) newList.Add(l1s);
                    else if (l1s == l2e) newList.Add(l1s);
                    else if (l1e == l2s) newList.Add(l1e);
                    else  newList.Add(l1e);
                }
                else
                {
                    float x = (C2 * B1 - C1 * B2) / m;
                    float y = (C1 * A2 - C2 * A1) / m;
                    newList.Add(new PointF(x, y));
                }
                
            }
            return newList;

        }
        private void Paint_PathDist(PaintEventArgs e)
        {
            var pe_line = new Pen(Color.Gray, 1);
            var fo = new Font("新細明體", 12);
            List<PathDistance> pdl = new List<PathDistance>();
            foreach(var pd in PDistList)
            {
                PointF pt1, pt2;
                float dist;
                bool inlist = false;
                pd.Get_Dist_Point(out dist, out pt1, out pt2);
                if (pd.type == 0)
                    inlist = LineList.Exists(x => x == pd.L1) && LineList.Exists(x => x == pd.L2);
                else if (pd.type == 1)
                    inlist = LineList.Exists(x => x == pd.L1) && CurveList.Exists(x => x == pd.C1);
                else if (pd.type == 2)
                    inlist = CurveList.Exists(x => x == pd.C1) && CurveList.Exists(x => x == pd.C2);
                if ((pt1.X == -1 && pt1.Y == -1) || (pt2.X == -1 && pt2.Y == -1) || inlist == false)
                {
                    pdl.Add(pd);
                }
                else
                {
                    e.Graphics.DrawLine(pe_line, pt1.X * ZoomSize, pt1.Y * ZoomSize, pt2.X * ZoomSize, pt2.Y * ZoomSize);
                    PointF mid = new PointF((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2);
                    if (LenthUnit == 0)
                        dist /= 72/2.54F;
                    else
                        dist /= 72F;
                    e.Graphics.DrawString(dist.ToString("F") + (LenthUnit == 0 ? " cm" : " inch"), fo, Brushes.Black, mid.X * ZoomSize, mid.Y * ZoomSize);
                }
            }
            foreach (var pd in pdl)
                PDistList.Remove(pd);

            if (Mouse_Mode == Mouse_Mode_Type.Dist_Verti || Mouse_Mode == Mouse_Mode_Type.Dist_Hori)
            {
                if (M8_State == 1)
                {
                    PointF p = For_Paint;
                    var pe = new Pen(Color.Black, 1);
                    e.Graphics.DrawRectangle(pe, M8_TempP.P.X * ZoomSize - 2, M8_TempP.P.Y * ZoomSize - 2, 4, 4);
                    e.Graphics.DrawLine(pe, M8_TempP.P.X * ZoomSize, M8_TempP.P.Y * ZoomSize, p.X * ZoomSize, p.Y * ZoomSize);
                    GraphLine l = FindLineByPoint(LineList, For_Paint);
                    GraphCurve c;
                    int index;
                    FindCurveByPoint(CurveList, For_Paint, out c, out index);
                    pe = new Pen(Color.Red, 2);
                    if (l != null)
                    {
                        e.Graphics.DrawLine(pe, l.StartPoint.P.X * ZoomSize, l.StartPoint.P.Y * ZoomSize, l.EndPoint.P.X * ZoomSize, l.EndPoint.P.Y * ZoomSize);
                    }
                    else if (c != null)
                    {
                        List<PointF> t = GraphCurveToBez(c);
                        for (int i = 0; i < t.Count; i++)
                        {
                            t[i] = new PointF(t[i].X * ZoomSize, t[i].Y * ZoomSize);
                        }
                        e.Graphics.DrawBeziers(pe, t.ToArray());
                    }
                }
                if (M8_State == 0)
                {
                    GraphLine l = FindLineByPoint(LineList, For_Paint);
                    GraphCurve c;
                    int index;
                    FindCurveByPoint(CurveList, For_Paint, out c, out index);
                    var pe = new Pen(Color.Red, 2);
                    if (l != null)
                    {
                        e.Graphics.DrawLine(pe, l.StartPoint.P.X * ZoomSize, l.StartPoint.P.Y * ZoomSize, l.EndPoint.P.X * ZoomSize, l.EndPoint.P.Y * ZoomSize);
                    }
                    else if (c != null)
                    {
                        List<PointF> t = GraphCurveToBez(c);
                        for (int i = 0; i < t.Count; i++)
                        {
                            t[i] = new PointF(t[i].X * ZoomSize, t[i].Y * ZoomSize);
                        }
                        e.Graphics.DrawBeziers(pe, t.ToArray());
                    }
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
                if (Mouse_Mode == Mouse_Mode_Type.Cursor)
                {
                    if (Moving != null)
                    {
                        if (Moving.type == Moving_Type.Line)
                        {
                            LineMove(e);
                        }
                        else if (Moving.type == Moving_Type.Point)
                        {
                            PointMove(e);
                        }
                        else if (Moving.type == Moving_Type.Curve)
                        {
                            CurveMove(e);
                        }
                        else if (Moving.type == Moving_Type.Curve_Control)
                        {
                            CurveControlMove(e);
                        }
                        else if (Moving.type == Moving_Type.Group)
                        {
                            GroupMove(e);
                        }
                        else if (Moving.type == Moving_Type.Arc)
                        {
                            ArcMove(e);
                        }
                        else if (Moving.type == Moving_Type.Arc_Control)
                        {
                            ArcControlMove(e);
                        }
                        else if (Moving.type == Moving_Type.GroupControl)
                        {
                            GroupControlMove(e);
                        }
                        else if (Moving.type == Moving_Type.Text)
                        {
                            TextMove(e);
                        }
                    }
                }
                For_Paint = e;
                RefreshSelection(e);
                RefreshPorperty();
                pictureBox1.Refresh();
            }
        }
        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs ev)
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
            is_Drowing = false;
            previous_point = null;
            if (Mouse_Mode == Mouse_Mode_Type.Curve)
            {
                if (PreviousCruve.path.Count > 1)
                {
                    GraphCurve c = PathToCurve(PreviousCruve.path);
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
           PointsList.RemoveAll(x => x.Relative <= 0 && x.MarkL == null);
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs ev)
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
            if (Moving != null && Mouse_Mode == Mouse_Mode_Type.Cursor)
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
                else if(Moving.type==Moving_Type.Text && Moving.StartMoveMousePoint == e)
                {
                    Change_Text = Moving.Text;
                    textBox1.Location = MousePosition;
                    textBox1.Visible = true;
                    textBox1.Enabled = true;
                    textBox1.Text = SelectedText.S;
                    textBox1.Show();
                    textBox1.Focus();
                    toolStripButton1_Click(new object(), new EventArgs());
                }
                else
                {
                    Push_Undo_Data();
                }
                PointCombine();
                this.Capture = false;
                Moving = null;
            }
            RefreshSelection(e);
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs ev)
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
            textBox1_Leave(new object(), new EventArgs());
            RefreshSelection(e);
            if (ev.Button == MouseButtons.Left && Mouse_Mode == Mouse_Mode_Type.Cursor)
            {
                MBLeft_Cursor_DOWN(ev);
            }
            if (ev.Button == MouseButtons.Left && Mouse_Mode == Mouse_Mode_Type.Line)
            {
                MBLeft_Line_DOWN(ev);
            }
            if (ev.Button == MouseButtons.Left && Mouse_Mode == Mouse_Mode_Type.Curve)
            {
                MBLeft_Curve_DOWN(ev);
            }
            if (ev.Button == MouseButtons.Left && Mouse_Mode == Mouse_Mode_Type.Arc)
            {
                MBLeft_Arc_DOWN(ev);
            }
            if (ev.Button == MouseButtons.Left && Mouse_Mode == Mouse_Mode_Type.Text)
            {
                MBLeft_Text_DOWN(ev);
            }
            if (Mouse_Mode == Mouse_Mode_Type.Pleated)
            {
                if (ev.Button == MouseButtons.Left)
                    MBLeft_Pleated_DOWN(ev);
                else if (ev.Button == MouseButtons.Right)
                    M6_Temp_Clean();
            }
            if(Mouse_Mode == Mouse_Mode_Type.Cutting)
            {
                if (ev.Button == MouseButtons.Left)
                    MBLeft_Cutting_DOWN(ev);
                else if (ev.Button == MouseButtons.Right)
                    M7_Temp_Clean();
            }
            if (Mouse_Mode == Mouse_Mode_Type.Dist_Hori || Mouse_Mode == Mouse_Mode_Type.Dist_Verti)
            {
                if (ev.Button == MouseButtons.Left)
                    MBLeft_PathDist_DOWN(ev);
                else if (ev.Button == MouseButtons.Right)
                    M8_Temp_Clean();
            }
            if (ev.Button == MouseButtons.Right && Mouse_Mode == Mouse_Mode_Type.Cursor)
            {
                MBRight_Cursor_DOWN(ev);
            }
            if (ev.Button == MouseButtons.Right && (Mouse_Mode == Mouse_Mode_Type.Line || Mouse_Mode == Mouse_Mode_Type.Curve))
            {
                pictureBox1_MouseDoubleClick(sender, ev);
            }
            
            RefreshSelection(e);
        }
        private void MBLeft_Cursor_DOWN(MouseEventArgs ev)//游標
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
            bool in_Group = false;
            int in_List = -1;
            if (this.SelectedPoint != null && Moving == null)
            {
                if (this.SelectedPoint.MarkL != null)
                    return;
                if (SelectedGroup != null)
                    if (SelectedGroup.P.FindIndex(a => a == SelectedPoint) >= 0)
                        in_Group = true;
                foreach (var g in GroupList)
                {
                    if (g.P.FindIndex(a => a == SelectedPoint) >= 0)
                        in_List = GroupList.FindIndex(a => a == g);
                }
                Moving = new MoveInfo
                {
                    StartLinePoint = SelectedPoint,
                    StartMoveMousePoint = new Point((int)SelectedPoint.P.X, (int)SelectedPoint.P.Y),
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
                    StartLinePoint = new GraphPoint(SelectedArc.StartPoint.P.X, SelectedArc.StartPoint.P.Y),
                    EndLinePoint = new GraphPoint(SelectedArc.EndPoint.P.X, SelectedArc.EndPoint.P.Y),
                    Arc = SelectedArc,
                    type = Moving_Type.Arc
                };
            }
            else if (this.SelectedArcControl != null && Moving == null)
            {
                Moving = new MoveInfo
                {
                    StartMoveMousePoint = e,
                    StartLinePoint = new GraphPoint(SelectedArcControl.getControlPoint().X, SelectedArcControl.getControlPoint().Y),
                    Arc = SelectedArcControl,
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
            else if (SelectedText != null)
            {
                Moving = new MoveInfo
                {
                    StartMoveMousePoint = e,
                    StartLinePoint = new GraphPoint(SelectedText.P.X, SelectedText.P.Y),
                    Text = SelectedText,
                    type = Moving_Type.Text
                };
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
        private void MBLeft_Line_DOWN(MouseEventArgs ev)//直線
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
                    previous_point.MarkL = null;
                    nowp.MarkL = null;
                    Push_Undo_Data();
                }
                previous_point = nowp;
                is_Drowing = true;
            }
            /*else if (t.Arc != null)
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
            }*/
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
                    previous_point.MarkL = null;
                    t.MarkL = null;
                    Push_Undo_Data();
                }
                previous_point = t;
                is_Drowing = true;
            }
        }
        private void MBLeft_Curve_DOWN(MouseEventArgs ev)//曲線
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
        private void MBLeft_Arc_DOWN(MouseEventArgs ev)//圓弧
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
            GraphPoint nowp = new GraphPoint(e.X, e.Y);
            GraphPoint t = FindPointByPoint(PointsList, e);
            if (t == null)
            {
                if (Net_Mode)
                {
                    nowp.P.X += (float)SizeOfNet / DesityOfNet / 2;
                    nowp.P.Y += (float)SizeOfNet / DesityOfNet / 2;
                    nowp.P.X -= nowp.P.X % ((float)SizeOfNet / DesityOfNet);
                    nowp.P.Y -= nowp.P.Y % ((float)SizeOfNet / DesityOfNet);
                }
                if (previous_point != null)
                {
                    if (previous_point.P.X == nowp.P.X && previous_point.P.Y == nowp.P.Y)
                        return;
                    GraphArc a = new GraphArc(previous_point, nowp);
                    nowp.Relative++;
                    PointsList.Add(nowp);
                    if (!PointsList.Exists(x => x == previous_point))
                    {
                        PointsList.Add(previous_point);
                    }
                    previous_point.Relative++;
                    ArcList.Add(a);
                    Push_Undo_Data();
                    previous_point = null;
                    is_Drowing = false;
                }
                else
                {
                    previous_point = nowp;
                    is_Drowing = true;
                }
            }
            else
            {
                if (previous_point == t)
                    return;
                if(previous_point != null)
                {
                    GraphArc a = new GraphArc(previous_point, t);
                    ArcList.Add(a);
                    Push_Undo_Data();
                    previous_point.Relative++;
                    t.Relative++;
                    previous_point = null;
                    is_Drowing = false;
                }
                else
                {
                    previous_point = t;
                    is_Drowing = true;
                }
            }
        }
        private void MBLeft_Text_DOWN(MouseEventArgs ev)//文字
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
            Previous_Text = new GraphText();
            Previous_Text.P = e;
            textBox1.Location = MousePosition;
            textBox1.Visible = true;
            textBox1.Enabled = true;
            textBox1.Text = "";
            textBox1.Show();
            textBox1.Focus();
            toolStripButton1_Click(new object(), new EventArgs());
        }
        private void MBLeft_Pleated_DOWN(MouseEventArgs ev)//褶子
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
            if (M6_State == 0)
            {
                M6_Temp_Clean();
                M6_TempL = FindLineByPoint(LineList, e);
                FindCurveByPoint(CurveList, e, out M6_TempC, out M6_TempC_Index);
                M6_TempP1 = new GraphPoint(e.X, e.Y);
                if (M6_TempL != null)
                {
                    foreach (var path in PathList)
                    {
                        if (path.L.Exists(x => x == M6_TempL))
                        {
                            M6_TempPath = path;
                            break;
                        }
                    }
                }
                else if (M6_TempC != null)
                {
                    foreach (var path in PathList)
                    {
                        if (path.C.Exists(x => x == M6_TempC))
                        {
                            M6_TempPath = path;
                            break;
                        }
                    }
                }
                if (M6_TempPath == null)
                {
                    if (M6_TempC != null || M6_TempL != null)
                        MessageBox.Show("請將線段設為圖形再進行操作", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    M6_Temp_Clean();
                    return;
                }
                M6_State = 1;
            }
            else if (M6_State == 1)
            {
                if (Math.Pow(M6_TempP1.P.X - e.X, 2) + Math.Pow(M6_TempP1.P.Y - e.Y, 2) < 4)
                    return;
                M6_TempP2 = new GraphPoint(e.X, e.Y);
                if (Net_Mode)
                {
                    M6_TempP2.P.X += (float)SizeOfNet / DesityOfNet / 2;
                    M6_TempP2.P.Y += (float)SizeOfNet / DesityOfNet / 2;
                    M6_TempP2.P.X -= M6_TempP2.P.X % ((float)SizeOfNet / DesityOfNet);
                    M6_TempP2.P.Y -= M6_TempP2.P.Y % ((float)SizeOfNet / DesityOfNet);
                }
                M6_State = 2;
            }
            else if (M6_State == 2)
            {
                GraphPoint p1 = M6_TempP1, mid = M6_TempP2, p2 = new GraphPoint(e.X, e.Y);
                GraphLine L1 = M6_TempL, L2 = FindLineByPoint(LineList, e);
                GraphCurve C1 = M6_TempC, C2;
                int C1Index = M6_TempC_Index, C2Index;
                FindCurveByPoint(CurveList, e, out C2, out C2Index);
                if ((L2 == null && C2 == null) || !(M6_TempPath.L.Exists(x => x == L2) && M6_TempPath.C.Exists(x => x == C2)))
                {
                    return;
                }
                int index1, index2;
                index1 = L1 != null ? M6_TempPath.L.FindIndex(x => x == L1) : M6_TempPath.C.FindIndex(x => x == C1);
                index2 = L2 != null ? M6_TempPath.L.FindIndex(x => x == L2) : M6_TempPath.C.FindIndex(x => x == C2);
                int dis12 = index2 - index1;
                dis12 += dis12 < 0 ? M6_TempPath.L.Count : 0;
                int dis21 = index1 - index2;
                dis21 += dis21 < 0 ? M6_TempPath.L.Count : 0;
                bool countseq_tplusfsub = dis12 > dis21;
                bool startp_t1f2 = !(countseq_tplusfsub ^ index1 > index2);
                if ((index1 == 0 && index2 == M6_TempPath.L.Count - 1) || (index2 == 0 && index1 == M6_TempPath.L.Count - 1))
                {
                    countseq_tplusfsub = !countseq_tplusfsub;
                    //startp_t1f2 = !startp_t1f2;
                }
                if (index1 == index2)
                {
                    int nexti = (index1 + 1) % M6_TempPath.L.Count;
                    GraphPoint nextS, nextE, thisS, thisE, nextP;
                    if (M6_TempPath.L[nexti] != null)
                    {
                        nextS = M6_TempPath.L[nexti].StartPoint;
                        nextE = M6_TempPath.L[nexti].EndPoint;
                    }
                    else
                    {
                        nextS = M6_TempPath.C[nexti].path[0];
                        nextE = M6_TempPath.C[nexti].path.Last();
                    }
                    if (L1 != null)
                    {
                        thisE = L1.StartPoint;
                        thisS = L1.EndPoint;
                        if (thisS == nextS || thisE == nextS)
                            nextP = nextS;
                        else
                            nextP = nextE;
                        double p1tonext = Math.Pow(p1.P.X - nextP.P.X, 2) + Math.Pow(p1.P.Y - nextP.P.Y, 2);
                        double p2tonext = Math.Pow(p2.P.X - nextP.P.X, 2) + Math.Pow(p2.P.Y - nextP.P.Y, 2);
                        startp_t1f2 = p1tonext > p2tonext;
                        countseq_tplusfsub = false;
                    }
                    else
                    {
                        thisS = C1.path[0];
                        thisE = C1.path.Last();
                        if (thisS == nextS || thisE == nextS)
                            nextP = nextS;
                        else
                            nextP = nextE;
                        if (C1Index == C2Index)
                        {
                            int index = C1Index - (nextP == C1.path.Last() ? 0 : 1);
                            double p1tonext = Math.Pow(p1.P.X - C1.path[index].P.X, 2) + Math.Pow(p1.P.Y - C1.path[index].P.Y, 2);
                            double p2tonext = Math.Pow(p2.P.X - C1.path[index].P.X, 2) + Math.Pow(p2.P.Y - C1.path[index].P.Y, 2);
                            startp_t1f2 = p1tonext < p2tonext;
                            countseq_tplusfsub = true;
                        }
                        else if (thisS == nextP)
                        {
                            startp_t1f2 = C1Index < C2Index;
                            countseq_tplusfsub = true;
                        }
                        else
                        {
                            startp_t1f2 = C1Index > C2Index;
                            countseq_tplusfsub = true;
                        }
                    }
                    dis12 = M6_TempPath.L.Count;
                }
                GraphGroup newpath = new GraphGroup();
                for (int i = 0; i < (dis12 > dis21 ? dis12 : dis21) + 1; i++)
                {
                    int reali = (startp_t1f2 ? index1 : index2) + (countseq_tplusfsub ? i : -i);
                    reali %= M6_TempPath.L.Count;
                    reali += reali < 0 ? M6_TempPath.L.Count : 0;
                    if (i == 0)
                    {
                        #region
                        GraphPoint nextS, nextE, thisS, thisE;
                        int nexti = (reali + ((countseq_tplusfsub) ? 1 : -1)) % M6_TempPath.L.Count;
                        nexti += nexti < 0 ? M6_TempPath.L.Count : 0;
                        if (M6_TempPath.L[nexti] != null)
                        {
                            nextS = M6_TempPath.L[nexti].StartPoint;
                            nextE = M6_TempPath.L[nexti].EndPoint;
                        }
                        else
                        {
                            nextS = M6_TempPath.C[nexti].path[0];
                            nextE = M6_TempPath.C[nexti].path.Last();
                        }
                        if (M6_TempPath.L[reali] != null)
                        {
                            thisS = M6_TempPath.L[reali].StartPoint;
                            thisE = M6_TempPath.L[reali].EndPoint;
                            GraphLine StoM, MtoE;
                            GraphPoint cutp = startp_t1f2 ? p1 : p2;
                            LineInsert(M6_TempPath.L[reali], cutp, out StoM, out MtoE);
                            if (thisS == nextS || thisS == nextE)
                            {
                                newpath.L.Add(StoM);
                                newpath.C.Add(null);
                            }
                            else
                            {
                                newpath.L.Add(MtoE);
                                newpath.C.Add(null);
                            }
                        }
                        else
                        {
                            thisS = M6_TempPath.C[reali].path[0];
                            thisE = M6_TempPath.C[reali].path.Last();
                            GraphPoint cutp = startp_t1f2 ? p1 : p2;
                            int index = startp_t1f2 ? C1Index : C2Index;
                            GraphCurve tempc = CurveInsert(M6_TempPath.C[reali], index, cutp);
                            GraphCurve StoM = new GraphCurve(), MtoE = new GraphCurve();
                            int midindex = tempc.path.FindIndex(x => x == cutp);
                            for (int j = 0; j < tempc.path.Count; j++)
                            {
                                if (j <= midindex)
                                {
                                    StoM.path.Add(tempc.path[j]);
                                    StoM.disFirst.Add(tempc.disFirst[j]);
                                    StoM.disSecond.Add(tempc.disSecond[j]);
                                    StoM.type.Add(tempc.type[j]);
                                }
                                if (j >= midindex)
                                {
                                    MtoE.path.Add(tempc.path[j]);
                                    MtoE.disFirst.Add(tempc.disFirst[j]);
                                    MtoE.disSecond.Add(tempc.disSecond[j]);
                                    MtoE.type.Add(tempc.type[j]);
                                }
                            }
                            if (thisS == nextS || thisS == nextE)
                            {
                                newpath.L.Add(null);
                                newpath.C.Add(StoM);
                            }
                            else
                            {
                                newpath.L.Add(null);
                                newpath.C.Add(MtoE);
                            }
                        }
                        #endregion
                    }
                    else if (i == (dis12 > dis21 ? dis12 : dis21))
                    {
                        #region
                        GraphPoint preS, preE, thisS, thisE;
                        int prei = (reali + ((countseq_tplusfsub) ? -1 : 1)) % M6_TempPath.L.Count;
                        prei += prei < 0 ? M6_TempPath.L.Count : 0;
                        if (M6_TempPath.L[prei] != null)
                        {
                            preS = M6_TempPath.L[prei].StartPoint;
                            preE = M6_TempPath.L[prei].EndPoint;
                        }
                        else
                        {
                            preS = M6_TempPath.C[prei].path[0];
                            preE = M6_TempPath.C[prei].path.Last();
                        }
                        if (M6_TempPath.L[reali] != null)
                        {
                            thisS = M6_TempPath.L[reali].StartPoint;
                            thisE = M6_TempPath.L[reali].EndPoint;
                            GraphLine StoM, MtoE;
                            GraphPoint cutp = startp_t1f2 ? p2 : p1;
                            LineInsert(M6_TempPath.L[reali], cutp, out StoM, out MtoE);
                            if (thisS == preS || thisS == preE)
                            {
                                newpath.L.Add(StoM);
                                newpath.C.Add(null);
                            }
                            else
                            {
                                newpath.L.Add(MtoE);
                                newpath.C.Add(null);
                            }
                        }
                        else
                        {
                            thisS = M6_TempPath.C[reali].path[0];
                            thisE = M6_TempPath.C[reali].path.Last();
                            GraphPoint cutp = startp_t1f2 ? p2 : p1;
                            int index = startp_t1f2 ? C2Index : C1Index;
                            GraphCurve tempc = CurveInsert(M6_TempPath.C[reali], index, cutp);
                            GraphCurve StoM = new GraphCurve(), MtoE = new GraphCurve();
                            int midindex = tempc.path.FindIndex(x => x == cutp);
                            for (int j = 0; j < tempc.path.Count; j++)
                            {
                                if (j <= midindex)
                                {
                                    StoM.path.Add(tempc.path[j]);
                                    StoM.disFirst.Add(tempc.disFirst[j]);
                                    StoM.disSecond.Add(tempc.disSecond[j]);
                                    StoM.type.Add(tempc.type[j]);
                                }
                                if (j >= midindex)
                                {
                                    MtoE.path.Add(tempc.path[j]);
                                    MtoE.disFirst.Add(tempc.disFirst[j]);
                                    MtoE.disSecond.Add(tempc.disSecond[j]);
                                    MtoE.type.Add(tempc.type[j]);
                                }
                            }
                            if (thisS == preS || thisS == preE)
                            {
                                newpath.L.Add(null);
                                newpath.C.Add(StoM);
                            }
                            else
                            {
                                newpath.L.Add(null);
                                newpath.C.Add(MtoE);
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        newpath.L.Add(M6_TempPath.L[reali]);
                        newpath.C.Add(M6_TempPath.C[reali]);
                    }
                }
                newpath.L.Add(new GraphLine((startp_t1f2) ? p2 : p1, mid));
                newpath.L.Add(new GraphLine(mid, (startp_t1f2) ? p1 : p2));
                newpath.C.Add(null);
                newpath.C.Add(null);
                for (int i = 0; i < M6_TempPath.L.Count; i++)
                {
                    DeleteLine(M6_TempPath.L[i]);
                    DeleteHoleCurve(M6_TempPath.C[i]);
                }
                for (int i = 0; i < newpath.L.Count; i++)
                {
                    GraphPoint thisS, thisE;
                    if (newpath.L[i] != null)
                    {
                        thisS = newpath.L[i].StartPoint;
                        thisS.Relative++;
                        thisE = newpath.L[i].EndPoint;
                        thisE.Relative++;
                        LineList.Add(newpath.L[i]);
                        if (!PointsList.Exists(x => x == thisS))
                            PointsList.Add(thisS);
                        if (!PointsList.Exists(x => x == thisE))
                            PointsList.Add(thisE);
                    }
                    else
                    {
                        thisS = newpath.C[i].path[0];
                        thisE = newpath.C[i].path.Last();
                        foreach(var p in newpath.C[i].path)
                        {
                            p.Relative++;
                            if (!PointsList.Exists(x => x == p))
                                PointsList.Add(p);
                        }
                        CurveList.Add(newpath.C[i]);
                    }
                    if (!newpath.P.Exists(x => x == thisS))
                        newpath.P.Add(thisS);
                    if (!newpath.P.Exists(x => x == thisE))
                        newpath.P.Add(thisE);
                }
                PathList.Remove(M6_TempPath);
                PathList.Add(newpath);
                M6_State = 0;
                M6_Temp_Clean();
               PointsList.RemoveAll(x => x.Relative <= 0 && x.MarkL == null);
                Push_Undo_Data();
            }
        }
        private void MBLeft_Cutting_DOWN(MouseEventArgs ev)//裁剪
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
            if (M7_State == 0)
            {
                M7_Temp_Clean();
                M7_TempL = FindLineByPoint(LineList, e);
                FindCurveByPoint(CurveList, e, out M7_TempC, out M7_TempC_Index);
                M7_TempP = new GraphPoint(e.X, e.Y);
                if (M7_TempL != null)
                {
                    foreach (var path in PathList)
                    {
                        if (path.L.Exists(x => x == M7_TempL))
                        {
                            M7_TempPath = path;
                            break;
                        }
                    }
                }
                else if (M7_TempC != null)
                {
                    foreach (var path in PathList)
                    {
                        if (path.C.Exists(x => x == M7_TempC))
                        {
                            M7_TempPath = path;
                            break;
                        }
                    }
                }
                if (M7_TempPath == null)
                {
                    if (!(M7_TempL == null && M7_TempC == null))
                        MessageBox.Show("請將線段設為圖形再進行操作", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    M7_Temp_Clean();
                    return;
                }
                M7_State = 1;
            }
            else if (M7_State == 1)
            {
                GraphPoint p1 = M7_TempP, p2 = new GraphPoint(e.X, e.Y);
                GraphLine L1 = M7_TempL, L2 = FindLineByPoint(LineList, e);
                GraphCurve C1 = M7_TempC, C2;
                GraphGroup Path = M7_TempPath;
                int C1Index = M7_TempC_Index, C2Index;
                FindCurveByPoint(CurveList, e, out C2, out C2Index);
                int index1, index2;
                index1 = L1 != null ? Path.L.FindIndex(x => x == L1) : Path.C.FindIndex(x => x == C1);
                index2 = L2 != null ? Path.L.FindIndex(x => x == L2) : Path.C.FindIndex(x => x == C2);
                int dis = index2 - index1;
                dis += dis < 0 ? Path.L.Count : 0;
                if (L2 == null && C2 == null)
                    return;
                if (index1 == index2)
                {
                    MessageBox.Show("請切在不同線段上", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                GraphGroup newpath1 = new GraphGroup(), newpath2 = new GraphGroup();
                GraphLine P2L = null;
                GraphCurve P2C = null;
                GraphPoint P1S = null, P1E = null, P2S = null, P2E = null;
                for (int i = 0; i < Path.L.Count + 1; i++)
                {
                    int reali = index1 + i;
                    reali %= Path.L.Count;
                    if (i == 0)
                    {
                        #region
                        GraphPoint nextS, nextE, thisS, thisE;
                        int nexti = (reali + 1) % Path.L.Count;
                        if (Path.L[nexti] != null)
                        {
                            nextS = Path.L[nexti].StartPoint;
                            nextE = Path.L[nexti].EndPoint;
                        }
                        else
                        {
                            nextS = Path.C[nexti].path[0];
                            nextE = Path.C[nexti].path.Last();
                        }
                        if (Path.L[reali] != null)
                        {
                            thisS = Path.L[reali].StartPoint;
                            thisE = Path.L[reali].EndPoint;
                            GraphLine StoM, MtoE;
                            GraphPoint cutp = p1;
                            LineInsert(Path.L[reali], cutp, out StoM, out MtoE);
                            GraphPoint cutp2 = new GraphPoint(cutp.P.X, cutp.P.Y);
                            MtoE.StartPoint = cutp2;
                            if (thisS == nextS || thisS == nextE)
                            {
                                newpath1.L.Add(StoM);
                                newpath1.C.Add(null);
                                P2L = MtoE;
                                P2C = null;
                                P1S = cutp;
                                P2E = cutp2;
                            }
                            else
                            {
                                newpath1.L.Add(MtoE);
                                newpath1.C.Add(null);
                                P2L = StoM;
                                P2C = null;
                                P1S = cutp2;
                                P2E = cutp;
                            }
                        }
                        else
                        {
                            thisS = Path.C[reali].path[0];
                            thisE = Path.C[reali].path.Last();
                            GraphPoint cutp = p1;
                            int index = C1Index;
                            GraphCurve tempc = CurveInsert(Path.C[reali], index, cutp);
                            GraphCurve StoM = new GraphCurve(), MtoE = new GraphCurve();
                            int midindex = tempc.path.FindIndex(x => x == cutp);
                            for (int j = 0; j < tempc.path.Count; j++)
                            {
                                if (j <= midindex)
                                {
                                    StoM.path.Add(tempc.path[j]);
                                    StoM.disFirst.Add(tempc.disFirst[j]);
                                    StoM.disSecond.Add(tempc.disSecond[j]);
                                    StoM.type.Add(tempc.type[j]);
                                }
                                if (j >= midindex)
                                {
                                    MtoE.path.Add(tempc.path[j]);
                                    MtoE.disFirst.Add(tempc.disFirst[j]);
                                    MtoE.disSecond.Add(tempc.disSecond[j]);
                                    MtoE.type.Add(tempc.type[j]);
                                }
                            }
                            GraphPoint cutp2 = new GraphPoint(cutp.P.X, cutp.P.Y);
                            MtoE.path[0] = cutp2;
                            if (thisS == nextS || thisS == nextE)
                            {
                                newpath1.L.Add(null);
                                newpath1.C.Add(StoM);
                                P2L = null;
                                P2C = MtoE;
                                P1S = cutp;
                                P2E = cutp2;
                            }
                            else
                            {
                                newpath1.L.Add(null);
                                newpath1.C.Add(MtoE);
                                P2L = null;
                                P2C = StoM;
                                P1S = cutp2;
                                P2E = cutp;
                            }
                        }
                        #endregion
                    }
                    else if (i > 0 && i < dis)
                    {
                        newpath1.L.Add(Path.L[reali]);
                        newpath1.C.Add(Path.C[reali]);
                    }
                    else if (i == dis)
                    {
                        #region
                        GraphPoint nextS, nextE, thisS, thisE;
                        int nexti = (reali + 1) % Path.L.Count;
                        if (Path.L[nexti] != null)
                        {
                            nextS = Path.L[nexti].StartPoint;
                            nextE = Path.L[nexti].EndPoint;
                        }
                        else
                        {
                            nextS = Path.C[nexti].path[0];
                            nextE = Path.C[nexti].path.Last();
                        }
                        if (Path.L[reali] != null)
                        {
                            thisS = Path.L[reali].StartPoint;
                            thisE = Path.L[reali].EndPoint;
                            GraphLine StoM, MtoE;
                            GraphPoint cutp = p2;
                            LineInsert(Path.L[reali], cutp, out StoM, out MtoE);
                            GraphPoint cutp2 = new GraphPoint(cutp.P.X, cutp.P.Y);
                            MtoE.StartPoint = cutp2;
                            if (thisS == nextS || thisS == nextE)
                            {
                                newpath1.L.Add(MtoE);
                                newpath1.C.Add(null);
                                newpath2.L.Add(StoM);
                                newpath2.C.Add(null);
                                P1E = cutp2;
                                P2S = cutp;
                            }
                            else
                            {
                                newpath1.L.Add(StoM);
                                newpath1.C.Add(null);
                                newpath2.L.Add(MtoE);
                                newpath2.C.Add(null);
                                P1E = cutp;
                                P2S = cutp2;
                            }
                        }
                        else
                        {
                            thisS = Path.C[reali].path[0];
                            thisE = Path.C[reali].path.Last();
                            GraphPoint cutp = p2;
                            int index = C2Index;
                            GraphCurve tempc = CurveInsert(Path.C[reali], index, cutp);
                            GraphCurve StoM = new GraphCurve(), MtoE = new GraphCurve();
                            int midindex = tempc.path.FindIndex(x => x == cutp);
                            for (int j = 0; j < tempc.path.Count; j++)
                            {
                                if (j <= midindex)
                                {
                                    StoM.path.Add(tempc.path[j]);
                                    StoM.disFirst.Add(tempc.disFirst[j]);
                                    StoM.disSecond.Add(tempc.disSecond[j]);
                                    StoM.type.Add(tempc.type[j]);
                                }
                                if (j >= midindex)
                                {
                                    MtoE.path.Add(tempc.path[j]);
                                    MtoE.disFirst.Add(tempc.disFirst[j]);
                                    MtoE.disSecond.Add(tempc.disSecond[j]);
                                    MtoE.type.Add(tempc.type[j]);
                                }
                            }
                            GraphPoint cutp2 = new GraphPoint(cutp.P.X, cutp.P.Y);
                            MtoE.path[0] = cutp2;
                            if (thisS == nextS || thisS == nextE)
                            {
                                newpath1.L.Add(null);
                                newpath1.C.Add(MtoE);
                                newpath2.L.Add(null);
                                newpath2.C.Add(StoM);
                                P1E = cutp2;
                                P2S = cutp;
                            }
                            else
                            {
                                newpath1.L.Add(null);
                                newpath1.C.Add(StoM);
                                newpath2.L.Add(null);
                                newpath2.C.Add(MtoE);
                                P1E = cutp;
                                P2S = cutp2;
                            }
                        }
                        #endregion
                    }
                    else if (i > dis && i < Path.L.Count)
                    {
                        newpath2.L.Add(Path.L[reali]);
                        newpath2.C.Add(Path.C[reali]);
                    }
                    else if (i == Path.L.Count)
                    {
                        newpath2.L.Add(P2L);
                        newpath2.C.Add(P2C);
                    }
                }
                newpath1.L.Add(new GraphLine(P1S, P1E));
                newpath2.L.Add(new GraphLine(P2S, P2E));
                newpath1.C.Add(null);
                newpath2.C.Add(null);
                for (int i = 0; i < M7_TempPath.L.Count; i++)
                {
                    DeleteLine(M7_TempPath.L[i]);
                    DeleteHoleCurve(M7_TempPath.C[i]);
                }
                for (int i = 0; i < newpath1.L.Count; i++)
                {
                    GraphPoint thisS, thisE;
                    if (newpath1.L[i] != null)
                    {
                        thisS = newpath1.L[i].StartPoint;
                        thisS.Relative++;
                        thisE = newpath1.L[i].EndPoint;
                        thisE.Relative++;
                        LineList.Add(newpath1.L[i]);
                        if (!PointsList.Exists(x => x == thisS))
                            PointsList.Add(thisS);
                        if (!PointsList.Exists(x => x == thisE))
                            PointsList.Add(thisE);
                    }
                    else
                    {
                        thisS = newpath1.C[i].path[0];
                        thisE = newpath1.C[i].path.Last();
                        foreach (var p in newpath1.C[i].path)
                        {
                            p.Relative++;
                            if (!PointsList.Exists(x => x == p))
                                PointsList.Add(p);
                        }
                        CurveList.Add(newpath1.C[i]);
                    }
                    if (!newpath1.P.Exists(x => x == thisS))
                        newpath1.P.Add(thisS);
                    if (!newpath1.P.Exists(x => x == thisE))
                        newpath1.P.Add(thisE);
                }
                for (int i = 0; i < newpath2.L.Count; i++)
                {
                    GraphPoint thisS, thisE;
                    if (newpath2.L[i] != null)
                    {
                        thisS = newpath2.L[i].StartPoint;
                        thisS.Relative++;
                        thisE = newpath2.L[i].EndPoint;
                        thisE.Relative++;
                        LineList.Add(newpath2.L[i]);
                        if (!PointsList.Exists(x => x == thisS))
                            PointsList.Add(thisS);
                        if (!PointsList.Exists(x => x == thisE))
                            PointsList.Add(thisE);
                    }
                    else
                    {
                        thisS = newpath2.C[i].path[0];
                        thisE = newpath2.C[i].path.Last();
                        foreach (var p in newpath2.C[i].path)
                        {
                            p.Relative++;
                            if (!PointsList.Exists(x => x == p))
                                PointsList.Add(p);
                        }
                        CurveList.Add(newpath2.C[i]);
                    }
                    if (!newpath2.P.Exists(x => x == thisS))
                        newpath2.P.Add(thisS);
                    if (!newpath2.P.Exists(x => x == thisE))
                        newpath2.P.Add(thisE);
                }
                float p1x = 0, p1y = 0, p2x = 0, p2y = 0;
                foreach(var p in newpath1.P)
                {
                    p1x += p.P.X;
                    p1y += p.P.Y;
                }
                foreach (var p in newpath2.P)
                {
                    p2x += p.P.X;
                    p2y += p.P.Y;
                }
                PointF p1mid = new PointF(p1x / newpath1.P.Count, p1y / newpath1.P.Count);
                PointF p2mid = new PointF(p2x / newpath2.P.Count, p2y / newpath2.P.Count);
                float xdis = p2mid.X - p1mid.X, ydis = p2mid.Y - p1mid.Y;
                bool txfy = ((ydis == 0) ? true : (Math.Abs(xdis / ydis) > 1));
                xdis = xdis > 0 ? 15 : -15;
                ydis = ydis > 0 ? 15 : -15;
                foreach (var p in newpath2.P)
                {
                    if(txfy)
                        p.P.X += xdis;
                    else
                        p.P.Y += ydis;
                }
                foreach (var c in newpath2.C)
                {
                    if (c != null)
                    {
                        for(int i = 1; i < c.path.Count - 1; i++)
                        {
                            if(txfy)
                                c.path[i].P.X += xdis;
                            else
                                c.path[i].P.Y += ydis;
                        }
                    }
                }
                PathList.Remove(M7_TempPath);
                PathList.Add(newpath1);
                PathList.Add(newpath2);
                M7_State = 0;
                M7_Temp_Clean();
                PointsList.RemoveAll(x => x.Relative <= 0 && x.MarkL == null);
                Push_Undo_Data();
            }
        }
        private void MBLeft_PathDist_DOWN(MouseEventArgs ev)//距離
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
            if (M8_State == 0)
            {
                M8_Temp_Clean();
                M8_TempL = FindLineByPoint(LineList, e);
                FindCurveByPoint(CurveList, e, out M8_TempC, out M8_TempC_Index);
                M8_TempP = new GraphPoint(e.X, e.Y);
                M8_State = 1;
                if (M8_TempL == null && M8_TempC == null)
                {
                    M8_Temp_Clean();
                    return;
                }
            }
            else if (M8_State == 1)
            {
                GraphPoint p1 = M8_TempP, p2 = new GraphPoint(e.X, e.Y);
                GraphLine L1 = M8_TempL, L2 = FindLineByPoint(LineList, e);
                GraphCurve C1 = M8_TempC, C2;
                int C1Index = M8_TempC_Index, C2Index;
                FindCurveByPoint(CurveList, e, out C2, out C2Index);
                PathDistance pd;
                float pos = (Mouse_Mode == Mouse_Mode_Type.Dist_Hori) ? (p1.P.Y + p2.P.Y) / 2 : (p1.P.X + p2.P.X) / 2;
                if (L1 != null && L2 != null)
                {
                    pd = new PathDistance(L1, L2, pos, (Mouse_Mode == Mouse_Mode_Type.Dist_Hori));
                }
                else if (L1 != null && C2 != null)
                {
                    pd = new PathDistance(L1, C2, C2Index, pos, (Mouse_Mode == Mouse_Mode_Type.Dist_Hori));
                }
                else if (C1 != null && L2 != null)
                {
                    pd = new PathDistance(L2, C1, C1Index, pos, (Mouse_Mode == Mouse_Mode_Type.Dist_Hori));
                }
                else if (C1 != null && C2 != null)
                {
                    pd = new PathDistance(C1, C1Index, C2, C2Index, pos, (Mouse_Mode == Mouse_Mode_Type.Dist_Hori));
                }
                else
                {
                    M8_Temp_Clean();
                    return;
                }
                PointF pt1, pt2;
                float dist;
                pd.Get_Dist_Point(out dist, out pt1, out pt2);
                if (!((pt1.X == -1 && pt1.Y == -1) || (pt2.X == -1 && pt2.Y == -1)))
                {
                    PDistList.Add(pd);
                }
                M8_Temp_Clean();
                PointsList.RemoveAll(x => x.Relative <= 0 && x.MarkL == null);
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (Previous_Text != null)
                Previous_Text.S = textBox1.Text;
            else if (Change_Text != null)
                Change_Text.S = textBox1.Text;
            Refresh();
        }
        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (Previous_Text != null)
            {
                if (Previous_Text.S != "")
                    TextList.Add(Previous_Text);
                Previous_Text = null;
                textBox1.Hide();
            }
            else if (Change_Text != null)
            {
                Change_Text = null;
                textBox1.Hide();
            }
        }
        private void MBRight_Cursor_DOWN(MouseEventArgs ev)//右鍵
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
            Right_Temp_Line = null;
            Right_Temp_Point = null;
            Right_Temp_Curve = null;
            Right_Temp_Curve_Index = -1;
            Right_Temp_Group = null;
            Right_Temp_Text = null;
            bool incurve = false;
            GraphCurve tc = null;
            int tcindex = -1;
            圖形ToolStripMenuItem.Enabled = false;
            組為圖形ToolStripMenuItem.Enabled = false;
            解除圖形ToolStripMenuItem.Enabled = false;
            直線等分ToolStripMenuItem.Visible = false;
            選取整個圖形ToolStripMenuItem.Visible = false;
            移除距離標示ToolStripMenuItem.Visible = false;
            變更顏色ToolStripMenuItem.Visible = false;
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
                bool tb;
                if (TestPath(SelectedGroup, out tb) != null)
                {
                    圖形ToolStripMenuItem.Enabled = true;
                    組為圖形ToolStripMenuItem.Enabled = true;
                }
            }
            else
                群組ToolStripMenuItem.Enabled = false;

            if (In_Path(SelectedLine, SelectedCurve) != null)
            {
                圖形ToolStripMenuItem.Enabled = true;
                解除圖形ToolStripMenuItem.Enabled = true;
            }

            if (SelectedGroup != null)
            {
                if (SelectedGroup.L.Count == 1 && SelectedGroup.C.Count == 0 && SelectedGroup.A.Count == 0)
                {
                    SelectedLine = SelectedGroup.L[0];
                    foreach (var path in PathList)
                        if (path.L.Exists(x => x == SelectedLine))
                            選取整個圖形ToolStripMenuItem.Visible = true;
                    Right_Temp_Line = SelectedLine;
                    新增節點ToolStripMenuItem.Enabled = true;
                    轉換為曲線ToolStripMenuItem.Visible = false;
                    toolStripMenuItem2.Text = "刪除直線";
                    if (!PointsList.Exists(x => x.MarkL == SelectedLine))
                        直線等分ToolStripMenuItem.Text = "直線等分";
                    else
                        直線等分ToolStripMenuItem.Text = "取消等分標記";
                    直線等分ToolStripMenuItem.Visible = true;
                    變更顏色ToolStripMenuItem.Visible = true;
                    Right_Temp_Mouse_Pos = e;
                    contextMenuStrip1.Show(MousePosition);
                }
                else if(SelectedGroup.L.Count == 0 && SelectedGroup.C.Count == 1 && SelectedGroup.A.Count == 0)
                {
                    SelectedCurve = SelectedGroup.C[0];
                    foreach (var path in PathList)
                        if (path.C.Exists(x => x == SelectedCurve))
                            選取整個圖形ToolStripMenuItem.Visible = true;
                    新增節點ToolStripMenuItem.Enabled = true;
                    轉換為曲線ToolStripMenuItem.Visible = false;
                    Right_Temp_Curve = SelectedCurve;
                    Right_Temp_Curve_Index = SelectedCurveIndex;
                    toolStripMenuItem2.Text = "刪除整條曲線";
                    變更顏色ToolStripMenuItem.Visible = true;
                    Right_Temp_Mouse_Pos = e;
                    contextMenuStrip1.Show(MousePosition);
                }
                else
                {
                    Right_Temp_Point = SelectedPoint;
                    新增節點ToolStripMenuItem.Enabled = false;
                    轉換為曲線ToolStripMenuItem.Visible = false;
                    toolStripMenuItem2.Text = "刪除所有選取範圍";
                    Right_Temp_Mouse_Pos = e;
                    contextMenuStrip1.Show(MousePosition);
                }
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
                foreach(var path in PathList)
                    if (path.L.Exists(x => x == SelectedLine))
                        選取整個圖形ToolStripMenuItem.Visible = true;
                Right_Temp_Line = SelectedLine;
                新增節點ToolStripMenuItem.Enabled = true;
                轉換為曲線ToolStripMenuItem.Visible = false;
                toolStripMenuItem2.Text = "刪除直線";
                if (!PointsList.Exists(x => x.MarkL == SelectedLine))
                    直線等分ToolStripMenuItem.Text = "直線等分";
                else
                    直線等分ToolStripMenuItem.Text = "取消等分標記";
                直線等分ToolStripMenuItem.Visible = true;
                變更顏色ToolStripMenuItem.Visible = true;
                if (PDistList.Exists(x => x.L1 == SelectedLine || x.L2 == SelectedLine))
                    移除距離標示ToolStripMenuItem.Visible = true;
                Right_Temp_Mouse_Pos = e;
                contextMenuStrip1.Show(MousePosition);
            }
            else if (SelectedCurve != null)
            {
                foreach (var path in PathList)
                    if (path.C.Exists(x => x == SelectedCurve))
                        選取整個圖形ToolStripMenuItem.Visible = true;
                新增節點ToolStripMenuItem.Enabled = true;
                轉換為曲線ToolStripMenuItem.Visible = false;
                Right_Temp_Curve = SelectedCurve;
                Right_Temp_Curve_Index = SelectedCurveIndex;
                toolStripMenuItem2.Text = "刪除整條曲線";
                變更顏色ToolStripMenuItem.Visible = true;
                if (PDistList.Exists(x => x.C1 == SelectedCurve || x.C2 == SelectedCurve))
                    移除距離標示ToolStripMenuItem.Visible = true;
                Right_Temp_Mouse_Pos = e;
                contextMenuStrip1.Show(MousePosition);
            }
            else if (SelectedArc != null)
            {
                新增節點ToolStripMenuItem.Enabled = false;
                轉換為曲線ToolStripMenuItem.Visible = true;
                Right_Temp_Arc = SelectedArc;
                toolStripMenuItem2.Text = "刪除圓弧";
                變更顏色ToolStripMenuItem.Visible = true;
                Right_Temp_Mouse_Pos = e;
                contextMenuStrip1.Show(MousePosition);
            }
            else if (SelectedText != null)
            {
                新增節點ToolStripMenuItem.Enabled = false;
                轉換為曲線ToolStripMenuItem.Visible = false;
                Right_Temp_Text = SelectedText;
                toolStripMenuItem2.Text = "刪除文字";
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
                Right_Temp_Text = null;
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
                if (a.StartPoint.P.X > x && a.StartPoint.P.Y > y && a.EndPoint.P.X > x && a.EndPoint.P.Y > y)
                    if (a.StartPoint.P.X - x < width && a.StartPoint.P.Y - y < height && a.EndPoint.P.X - x < width && a.EndPoint.P.Y - y < height)
                    {
                        bool inG = false;
                        foreach (var g in GroupList)
                        {
                            if (g.A.FindIndex(c => c == a) >= 0)
                                inG = true;
                        }
                        if (!inG)
                        {
                            SelectedGroup.A.Add(a);
                            if (SelectedGroup.P.FindIndex(b => b == a.StartPoint) < 0)
                                SelectedGroup.P.Add(a.StartPoint);
                            if (SelectedGroup.P.FindIndex(b => b == a.EndPoint) < 0)
                                SelectedGroup.P.Add(a.EndPoint);
                        }
                    }
            }
            foreach (var g in GroupList)
            {
                bool outrange = false;
                foreach (var l in g.L)
                {
                    if (!(l.StartPoint.P.X > x && l.StartPoint.P.Y > y && l.EndPoint.P.X > x && l.EndPoint.P.Y > y))
                        if (!(l.StartPoint.P.X - x < width && l.StartPoint.P.Y - y < height && l.EndPoint.P.X - x < width && l.EndPoint.P.Y - y < height))
                            outrange = true;
                }
                foreach (var c in g.C)
                {
                    for (int i = 0; i < c.path.Count; i++)
                    {
                        if (!(c.path[i].P.X > x && c.path[i].P.Y > y && c.path[i].P.X - x < width && c.path[i].P.Y - y < height))
                            outrange = true;
                    }
                }
                foreach (var a in g.A)
                {
                    if (!(a.StartPoint.P.X > x && a.StartPoint.P.Y > y && a.EndPoint.P.X > x && a.EndPoint.P.Y > y))
                        if (!(a.StartPoint.P.X - x < width && a.StartPoint.P.Y - y < height && a.EndPoint.P.X - x < width && a.EndPoint.P.Y - y < height))
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
                    if (SelectedGroup.P.FindIndex(a => a == Moving.Arc.StartPoint) < 0)
                        SelectedGroup.P.Add(Moving.Arc.StartPoint);
                    if (SelectedGroup.P.FindIndex(a => a == Moving.Arc.EndPoint) < 0)
                        SelectedGroup.P.Add(Moving.Arc.EndPoint);
                }
            }
            else
            {
                SelectedGroup = new GraphGroup();
                SelectedGroup.A.Add(Moving.Arc);
                SelectedGroup.P.Add(Moving.Arc.StartPoint);
                SelectedGroup.P.Add(Moving.Arc.EndPoint);
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
        private void M6_Temp_Clean()
        {
            M6_State = 0;
            M6_TempP1 = null;
            M6_TempP2 = null;
            M6_TempL = null;
            M6_TempC = null;
            M6_TempC_Index = -1;
            M6_TempPath = null;
        }
        private void M7_Temp_Clean()
        {
            M7_TempP = null;
            M7_TempL = null;
            M7_TempC = null;
            M7_TempPath = null;
            M7_TempC_Index = -1;
            M7_State = 0;
        }
        private void M8_Temp_Clean()
        {
            M8_TempP = null;
            M8_TempL = null;
            M8_TempC = null;
            M8_TempC_Index = -1;
            M8_State = 0;
        }
        #endregion

        #region MovingFunction

        private void LineMove(Point e)
        {
            GraphPoint sp = new GraphPoint(Moving.StartLinePoint.P.X + e.X - Moving.StartMoveMousePoint.X, Moving.StartLinePoint.P.Y + e.Y - Moving.StartMoveMousePoint.Y);
            GraphPoint ep = new GraphPoint(Moving.EndLinePoint.P.X + e.X - Moving.StartMoveMousePoint.X, Moving.EndLinePoint.P.Y + e.Y - Moving.StartMoveMousePoint.Y);
            Moving.Line.StartPoint.P.X = sp.P.X;
            Moving.Line.StartPoint.P.Y = sp.P.Y;
            Moving.Line.EndPoint.P.X = ep.P.X;
            Moving.Line.EndPoint.P.Y = ep.P.Y;
            foreach(var p in PointsList.FindAll(x => x.MarkL == Moving.Line))
            {
                p.P.X = (ep.P.X - sp.P.X) * p.part + sp.P.X;
                p.P.Y = (ep.P.Y - sp.P.Y) * p.part + sp.P.Y;
            }
        }
        private void PointMove(Point e)
        {
            //if (Moving.StartLinePoint.Arc == null)
            //{
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
                foreach (var l in LineList.FindAll(x => x.StartPoint == Moving.StartLinePoint || x.EndPoint == Moving.StartLinePoint))
                {
                    foreach (var p in PointsList.FindAll(x => x.MarkL == l))
                    {
                        p.P.X = (l.EndPoint.P.X - l.StartPoint.P.X) * p.part + l.StartPoint.P.X;
                        p.P.Y = (l.EndPoint.P.Y - l.StartPoint.P.Y) * p.part + l.StartPoint.P.Y;
                    }
                }
            //}
            /*else
            {
                PointF mid = Moving.StartLinePoint.Arc.Middle();
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
            }*/
        }
        private void CurveMove(Point e)
        {
            for (int i = 0; i < Moving.Path.Count; i++)
            {
                Moving.Curve.path[i].P.X = Moving.Path[i].P.X + e.X - Moving.StartMoveMousePoint.X;
                Moving.Curve.path[i].P.Y = Moving.Path[i].P.Y + e.Y - Moving.StartMoveMousePoint.Y;
            }
        }
        private void CurveControlMove(Point e)
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
                else if (Moving.Curve.type[SelectedCurveControlIndex] == 1)
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
        private void GroupMove(Point e)
        {
            for (int i = 0; i < Moving.Group.P.Count; i++)
            {
                /*if (Moving.Group.P[i].Arc != null && Moving.Group.P[i].Arc_Start == true)
                {
                    a = new PointF(Moving.Group.P[i].P.X - Moving.Group.P[i].Arc.P.X, Moving.Group.P[i].P.Y - Moving.Group.P[i].Arc.P.Y);
                }*/
                Moving.Group.P[i].P.X = Moving.Path[i].P.X + e.X - Moving.StartMoveMousePoint.X;
                Moving.Group.P[i].P.Y = Moving.Path[i].P.Y + e.Y - Moving.StartMoveMousePoint.Y;
                /*if (Moving.Group.P[i].Arc != null && Moving.Group.P[i].Arc_Start == true)
                {
                    Moving.Group.P[i].Arc.P.X = Moving.Group.P[i].P.X - a.X;
                    Moving.Group.P[i].Arc.P.Y = Moving.Group.P[i].P.Y - a.Y;
                }*/
            }
            foreach(var l in Moving.Group.L)
            {
                foreach(var p in PointsList.FindAll(x => x.MarkL == l))
                {
                    p.P.X = (l.EndPoint.P.X - l.StartPoint.P.X) * p.part + l.StartPoint.P.X;
                    p.P.Y = (l.EndPoint.P.Y - l.StartPoint.P.Y) * p.part + l.StartPoint.P.Y;
                }
            }
        }
        private void ArcMove(Point e)
        {
            GraphPoint sp = new GraphPoint(Moving.StartLinePoint.P.X + e.X - Moving.StartMoveMousePoint.X, Moving.StartLinePoint.P.Y + e.Y - Moving.StartMoveMousePoint.Y);
            GraphPoint ep = new GraphPoint(Moving.EndLinePoint.P.X + e.X - Moving.StartMoveMousePoint.X, Moving.EndLinePoint.P.Y + e.Y - Moving.StartMoveMousePoint.Y);
            Moving.Arc.StartPoint.P.X = sp.P.X;
            Moving.Arc.StartPoint.P.Y = sp.P.Y;
            Moving.Arc.EndPoint.P.X = ep.P.X;
            Moving.Arc.EndPoint.P.Y = ep.P.Y;
        }
        private void ArcControlMove(Point e)
        {
            GraphPoint sp = new GraphPoint(e.X, e.Y);
            Moving.Arc.setControlPoint(sp.P.X, sp.P.Y);
            /*if (Moving.ArcSide == 0)
            {
                Moving.Arc.P.X = Moving.StartLinePoint.P.X + e.X - Moving.StartMoveMousePoint.X;
                Moving.Arc.P.Y = Moving.StartLinePoint.P.Y + e.Y - Moving.StartMoveMousePoint.Y;
                Moving.Arc.width = Moving.ArcWidth - e.X + Moving.StartMoveMousePoint.X;
                Moving.Arc.height = Moving.ArcHeight - e.Y + Moving.StartMoveMousePoint.Y;
                Moving.Arc.refreshAngle();
            }
            else if (Moving.ArcSide == 1)
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
            }*/
        }
        private void GroupControlMove(Point e)
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
                for (int i = 0; i < Moving.GroupCurveControl.Count; i++)
                {
                    GraphCurve c = Moving.GroupCurveControl[i];
                    for (int j = 0; j < c.disFirst.Count; j++)
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
            foreach (var l in Moving.Group.L)
            {
                foreach (var p in PointsList.FindAll(x => x.MarkL == l))
                {
                    p.P.X = (l.EndPoint.P.X - l.StartPoint.P.X) * p.part + l.StartPoint.P.X;
                    p.P.Y = (l.EndPoint.P.Y - l.StartPoint.P.Y) * p.part + l.StartPoint.P.Y;
                }
            }
        }
        private void TextMove(Point e)
        {
            GraphPoint p = new GraphPoint(Moving.StartLinePoint.P.X + e.X - Moving.StartMoveMousePoint.X, Moving.StartLinePoint.P.Y + e.Y - Moving.StartMoveMousePoint.Y);
            Moving.Text.P = p.P;
        }

        #endregion

        #region toolStripButtons

        private void toolStripButton1_Click(object sender, EventArgs e)//游標
        {
            Mouse_Mode = Mouse_Mode_Type.Cursor;
            is_Drowing = false;
            previous_point = null;
            PreviousCruve = new GraphCurve();
            Cursor = Cursors.Default;
            Refresh_toolStripButton_Checked(Mouse_Mode_Type.Cursor);
            Refresh();
        }
        private void toolStripButton2_Click(object sender, EventArgs e)//直線
        {
            Mouse_Mode = Mouse_Mode_Type.Line;
            previous_point = null;
            SelectedGroup = null;
            PreviousCruve = new GraphCurve();
            Cursor = Cursors.Cross;
            Refresh_toolStripButton_Checked(Mouse_Mode_Type.Line);
            Refresh();
        }
        private void toolStripButton3_Click(object sender, EventArgs e)//曲線
        {
            Mouse_Mode = Mouse_Mode_Type.Curve;
            previous_point = null;
            SelectedGroup = null;
            PreviousCruve = new GraphCurve();
            Cursor = Cursors.Cross;
            Refresh_toolStripButton_Checked(Mouse_Mode_Type.Curve);
            Refresh();
        }
        private void toolStripButton7_Click(object sender, EventArgs e)//圓弧
        {
            Mouse_Mode = Mouse_Mode_Type.Arc;
            previous_point = null;
            SelectedGroup = null;
            PreviousCruve = new GraphCurve();
            Cursor = Cursors.Cross;
            Refresh_toolStripButton_Checked(Mouse_Mode_Type.Arc);
            Refresh();
        }
        private void toolStripButton11_Click(object sender, EventArgs e)//文字
        {
            Mouse_Mode = Mouse_Mode_Type.Text;
            previous_point = null;
            SelectedGroup = null;
            PreviousCruve = new GraphCurve();
            Cursor = Cursors.Cross;
            Refresh_toolStripButton_Checked(Mouse_Mode_Type.Text);
            Refresh();
        }
        private void toolStripButton12_Click(object sender, EventArgs e)//褶子
        {
            Mouse_Mode = Mouse_Mode_Type.Pleated;
            is_Drowing = false;
            previous_point = null;
            PreviousCruve = new GraphCurve();
            Cursor = Cursors.Cross;
            Refresh_toolStripButton_Checked(Mouse_Mode_Type.Pleated);
            Refresh();
        }
        private void toolStripButton13_Click(object sender, EventArgs e)//裁剪
        {
            Mouse_Mode = Mouse_Mode_Type.Cutting;
            is_Drowing = false;
            previous_point = null;
            PreviousCruve = new GraphCurve();
            Cursor = Cursors.Cross;
            Refresh_toolStripButton_Checked(Mouse_Mode_Type.Cutting);
            Refresh();
        }
        private void toolStripButton14_Click(object sender, EventArgs e)//水平距離
        {
            Mouse_Mode = Mouse_Mode_Type.Dist_Hori;
            is_Drowing = false;
            previous_point = null;
            PreviousCruve = new GraphCurve();
            Cursor = Cursors.Default;
            Refresh_toolStripButton_Checked(Mouse_Mode_Type.Dist_Hori);
            Refresh();
        }
        private void toolStripButton15_Click(object sender, EventArgs e)//垂直距離
        {
            Mouse_Mode = Mouse_Mode_Type.Dist_Verti;
            is_Drowing = false;
            previous_point = null;
            PreviousCruve = new GraphCurve();
            Cursor = Cursors.Default;
            Refresh_toolStripButton_Checked(Mouse_Mode_Type.Dist_Verti);
            Refresh();
        }
        private void Refresh_toolStripButton_Checked(Mouse_Mode_Type a)
        {
            toolStripButton1.Checked = false;
            toolStripButton2.Checked = false;
            toolStripButton3.Checked = false;
            toolStripButton7.Checked = false;
            toolStripButton11.Checked = false;
            toolStripButton12.Checked = false;
            toolStripButton13.Checked = false;
            toolStripButton14.Checked = false;
            toolStripButton15.Checked = false;
            M6_Temp_Clean();
            M7_Temp_Clean();
            switch (a)
            {
                case Mouse_Mode_Type.Cursor: toolStripButton1.Checked = true;break;
                case Mouse_Mode_Type.Line: toolStripButton2.Checked = true; break;
                case Mouse_Mode_Type.Curve: toolStripButton3.Checked = true; break;
                case Mouse_Mode_Type.Arc: toolStripButton7.Checked = true; break;
                case Mouse_Mode_Type.Text: toolStripButton11.Checked = true; break;
                case Mouse_Mode_Type.Pleated: toolStripButton12.Checked = true; break;
                case Mouse_Mode_Type.Cutting: toolStripButton13.Checked = true; break;
                case Mouse_Mode_Type.Dist_Hori: toolStripButton14.Checked = true; break;
                case Mouse_Mode_Type.Dist_Verti: toolStripButton15.Checked = true; break;
            }
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
            foreach (var p in Undo_Data[Undo_Index].PointL)
            {
                GraphPoint gp = new GraphPoint(p.P.X, p.P.Y);
                t.PointL.Add(gp);
            }
            foreach (var l in Undo_Data[Undo_Index].LineL)
            {
                int s = Undo_Data[Undo_Index].PointL.FindIndex(x => x == l.StartPoint);
                int end = Undo_Data[Undo_Index].PointL.FindIndex(x => x == l.EndPoint);
                t.PointL[s].Relative++;
                t.PointL[end].Relative++;
                GraphLine tl = new GraphLine(t.PointL[s], t.PointL[end]);
                tl.Seam = l.Seam;
                tl.SeamText = l.SeamText;
                tl.isSeam = l.isSeam;
                tl.co = l.co;
                t.LineL.Add(tl);
            }
            foreach (var c in Undo_Data[Undo_Index].CurveL)
            {
                GraphCurve gc = new GraphCurve();
                for (int i = 0; i < c.path.Count; i++)
                {
                    int index = Undo_Data[Undo_Index].PointL.FindIndex(x => x == c.path[i]);
                    gc.path.Add(t.PointL[index]);
                    t.PointL[index].Relative++;
                    gc.disFirst.Add(new PointF(c.disFirst[i].X, c.disFirst[i].Y));
                    gc.disSecond.Add(new PointF(c.disSecond[i].X, c.disSecond[i].Y));
                    gc.type.Add(c.type[i]);
                }
                gc.co = c.co;
                t.CurveL.Add(gc);
            }
            foreach (var a in Undo_Data[Undo_Index].ArcL)
            {
                int s = Undo_Data[Undo_Index].PointL.FindIndex(x => x == a.StartPoint);
                int end = Undo_Data[Undo_Index].PointL.FindIndex(x => x == a.EndPoint);
                t.PointL[s].Relative++;
                t.PointL[end].Relative++;
                GraphArc ta = new GraphArc(t.PointL[s], t.PointL[end], a.getControlPoint());
                ta.co = a.co;
                t.ArcL.Add(ta);
            }
            foreach (var g in Undo_Data[Undo_Index].GroupL)
            {
                t.GroupL.Add(Group_Copy(g, t, Undo_Data[Undo_Index]));
            }
            foreach(var path in Undo_Data[Undo_Index].PathL)
            {
                t.PathL.Add(Group_Copy(path, t, Undo_Data[Undo_Index]));
            }
            for (int i = 0; i < Undo_Data[Undo_Index].PointL.Count; i++)
            {
                if (Undo_Data[Undo_Index].PointL[i].MarkL != null)
                {
                    t.PointL[i].MarkL = t.LineL[Undo_Data[Undo_Index].LineL.FindIndex(x => x == Undo_Data[Undo_Index].PointL[i].MarkL)];
                    t.PointL[i].part = Undo_Data[Undo_Index].PointL[i].part;
                }
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
            foreach (var p in Undo_Data[Undo_Index].PointL)
            {
                GraphPoint gp = new GraphPoint(p.P.X, p.P.Y);
                t.PointL.Add(gp);
            }
            foreach (var l in Undo_Data[Undo_Index].LineL)
            {
                int s = Undo_Data[Undo_Index].PointL.FindIndex(x => x == l.StartPoint);
                int end = Undo_Data[Undo_Index].PointL.FindIndex(x => x == l.EndPoint);
                t.PointL[s].Relative++;
                t.PointL[end].Relative++;
                GraphLine tl = new GraphLine(t.PointL[s], t.PointL[end]);
                tl.Seam = l.Seam;
                tl.SeamText = l.SeamText;
                tl.isSeam = l.isSeam;
                tl.co = l.co;
                t.LineL.Add(tl);
            }
            foreach (var c in Undo_Data[Undo_Index].CurveL)
            {
                GraphCurve gc = new GraphCurve();
                for (int i = 0; i < c.path.Count; i++)
                {
                    int index = Undo_Data[Undo_Index].PointL.FindIndex(x => x == c.path[i]);
                    gc.path.Add(t.PointL[index]);
                    t.PointL[index].Relative++;
                    gc.disFirst.Add(new PointF(c.disFirst[i].X, c.disFirst[i].Y));
                    gc.disSecond.Add(new PointF(c.disSecond[i].X, c.disSecond[i].Y));
                    gc.type.Add(c.type[i]);
                }
                gc.co = c.co;
                t.CurveL.Add(gc);
            }
            foreach (var a in Undo_Data[Undo_Index].ArcL)
            {
                int s = Undo_Data[Undo_Index].PointL.FindIndex(x => x == a.StartPoint);
                int end = Undo_Data[Undo_Index].PointL.FindIndex(x => x == a.EndPoint);
                t.PointL[s].Relative++;
                t.PointL[end].Relative++;
                GraphArc ta = new GraphArc(t.PointL[s], t.PointL[end], a.getControlPoint());
                ta.co = a.co;
                t.ArcL.Add(ta);
            }
            foreach (var g in Undo_Data[Undo_Index].GroupL)
            {
                t.GroupL.Add(Group_Copy(g, t, Undo_Data[Undo_Index]));
            }
            foreach(var path in Undo_Data[Undo_Index].PathL)
            {
                t.PathL.Add(Group_Copy(path, t, Undo_Data[Undo_Index]));
            }
            for (int i = 0; i < Undo_Data[Undo_Index].PointL.Count; i++)
            {
                if (Undo_Data[Undo_Index].PointL[i].MarkL != null)
                {
                    t.PointL[i].MarkL = t.LineL[Undo_Data[Undo_Index].LineL.FindIndex(x => x == Undo_Data[Undo_Index].PointL[i].MarkL)];
                    t.PointL[i].part = Undo_Data[Undo_Index].PointL[i].part;
                }
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
            if (ZoomSize > 0.25F)
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
        private void toolStripTextBox1_Leave(object sender, EventArgs e)//網格大小
        {
            double t;
            if (double.TryParse(toolStripTextBox1.Text, out t))
            {
                SizeOfNet = LenthUnit == 0 ? t * 72 / 2.54 : t * 72;
            }
            else
            {
                toolStripTextBox1.Text = SizeOfNet / (LenthUnit == 0 ? 72 / 2.54 : 72) + "";
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
        private void toolStripTextBox2_Leave(object sender, EventArgs e)//細緻度
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
        private void Push_Undo_Data(int tdindex = -1)
        {
            TabpageData t = new TabpageData();
            foreach(var p in PointsList)
            {
                GraphPoint gp = new GraphPoint(p.P.X, p.P.Y);
                t.PointL.Add(gp);
            }
            foreach(var l in LineList)
            {
                int s = PointsList.FindIndex(x => x == l.StartPoint);
                int e = PointsList.FindIndex(x => x == l.EndPoint);
                t.PointL[s].Relative++;
                t.PointL[e].Relative++;
                GraphLine tl = new GraphLine(t.PointL[s], t.PointL[e]);
                tl.Seam = l.Seam;
                tl.SeamText = l.SeamText;
                tl.isSeam = l.isSeam;
                tl.co = l.co;
                t.LineL.Add(tl);
            }
            foreach(var c in CurveList)
            {
                GraphCurve gc = new GraphCurve();
                for(int i = 0; i < c.path.Count; i++)
                {
                    int index = PointsList.FindIndex(x => x == c.path[i]);
                    gc.path.Add(t.PointL[index]);
                    t.PointL[index].Relative++;
                    gc.disFirst.Add(new PointF(c.disFirst[i].X, c.disFirst[i].Y));
                    gc.disSecond.Add(new PointF(c.disSecond[i].X, c.disSecond[i].Y));
                    gc.type.Add(c.type[i]);
                }
                gc.isSeam = c.isSeam;
                gc.Seam = c.Seam;
                gc.co = c.co;
                t.CurveL.Add(gc);
            }
            foreach(var a in ArcList)
            {
                int s = PointsList.FindIndex(x => x == a.StartPoint);
                int e = PointsList.FindIndex(x => x == a.EndPoint);
                t.PointL[s].Relative++;
                t.PointL[e].Relative++;
                GraphArc ta = new GraphArc(t.PointL[s], t.PointL[e], a.getControlPoint());
                ta.co = a.co;
                t.ArcL.Add(ta);
            }
            for(int i = 0; i < PointsList.Count; i++)
            {
                if (PointsList[i].MarkL != null)
                {
                    t.PointL[i].MarkL = t.LineL[LineList.FindIndex(x => x == PointsList[i].MarkL)];
                    t.PointL[i].part = PointsList[i].part;
                }
            }
            foreach (var g in GroupList)
            {
                if (tdindex == -1)
                    t.GroupL.Add(Group_Copy(g, t, TabpageDataList[tabControl1.SelectedIndex]));
                else
                    t.GroupL.Add(Group_Copy(g, t, TabpageDataList[tdindex]));
            }
            foreach(var path in PathList)
            {
                if (tdindex == -1)
                    t.PathL.Add(Group_Copy(path, t, TabpageDataList[tabControl1.SelectedIndex]));
                else
                    t.PathL.Add(Group_Copy(path, t, TabpageDataList[tdindex]));
            }
            t.width = (int)(pictureBox1.Width/ZoomSize);
            t.height = (int)(pictureBox1.Height/ZoomSize);
            if (tdindex == -1)
                for (int i = Undo_Data.Count - 1; i > TabpageDataList[tabControl1.SelectedIndex].Undo_index; i--)
                {
                    Undo_Data.RemoveAt(i);
                }
            Undo_Data.Add(t);
            if (tdindex == -1)
                TabpageDataList[tabControl1.SelectedIndex].Undo_index++;
            else
                TabpageDataList[tdindex].Undo_index++;
            RefreshUndoCheck();
        }
        private GraphGroup Group_Copy(GraphGroup PreG, TabpageData PreT, TabpageData Ref)
        {
            GraphGroup gg = new GraphGroup();
            foreach(var p in PreG.P)
            {
                int a = Ref.PointL.FindIndex(x => x == p);
                gg.P.Add(PreT.PointL[a]);
            }
            foreach(var l in PreG.L)
            {
                if (l != null)
                {
                    int a = Ref.LineL.FindIndex(x => x == l);
                    gg.L.Add(PreT.LineL[a]);
                }
                else
                {
                    gg.L.Add(null);
                }
            }
            foreach(var c in PreG.C)
            {
                if (c != null)
                {
                    int a = Ref.CurveL.FindIndex(x => x == c);
                    gg.C.Add(PreT.CurveL[a]);
                }
                else
                {
                    gg.C.Add(null);
                }
            }
            foreach(var a in PreG.A)
            {
                int b = Ref.ArcL.FindIndex(x => x == a);
                gg.A.Add(PreT.ArcL[b]);
            }
            foreach (var g in PreG.G)
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
            GraphArc selectarccontrol = FindArcControlByPoint(ArcList, point);
            int selectedgroupcontrol = FindGroupControlByPoint(SelectedGroup, point);
            GraphText selectedtext = FindTextPyPoint(TextList, point);
            if (selectedPoint != this.SelectedPoint)
            {
                this.SelectedPoint = selectedPoint;
                this.SelectedLine = null;
                this.SelectedCurve = null;
                this.Invalidate();
            }
            else if (selectedLine != this.SelectedLine && SelectedPoint == null && Mouse_Mode == Mouse_Mode_Type.Cursor)
            {
                this.SelectedLine = selectedLine;
                this.Invalidate();
            }
            else if (selectedCurve != this.SelectedCurve && Mouse_Mode == Mouse_Mode_Type.Cursor && SelectedPoint == null)
            {
                this.SelectedCurve = selectedCurve;
                this.SelectedCurveIndex = selectedCurveIndex;
                this.Invalidate();
            }
            else if (selectedcurvecontrol != this.SelectedCurveControl && Mouse_Mode == Mouse_Mode_Type.Cursor)
            {
                this.SelectedCurveControl = selectedcurvecontrol;
                this.SelectedCurveControlIndex = selectedcurveindex;
                this.SeletedCurveControlFS = selectedcurvefs;
                this.Invalidate();
            }
            else if(selectarc!=this.SelectedArc && Mouse_Mode == Mouse_Mode_Type.Cursor)
            {
                this.SelectedArc = selectarc;
                this.Invalidate();
            }
            else if (selectarccontrol != this.SelectedArcControl && Mouse_Mode == Mouse_Mode_Type.Cursor)
            {
                this.SelectedArcControl = selectarccontrol;
                this.Invalidate();
            }
            else if (selectedgroupcontrol != SelectedGroupControl)
            {
                this.SelectedGroupControl = selectedgroupcontrol;
                this.Invalidate();
            }
            else if(selectedtext != SelectedText)
            {
                this.SelectedText = selectedtext;
                this.Invalidate();
            }
            if (Moving != null)
                this.Invalidate();

            this.Cursor =
                Moving != null ? Cursors.Hand :
                SelectedLine != null ? Cursors.SizeAll :
                SelectedPoint != null && Mouse_Mode == Mouse_Mode_Type.Cursor ? Cursors.SizeAll :
                SelectedCurve != null ? Cursors.SizeAll :
                SelectedCurveControl != null ? Cursors.SizeAll :
                SelectedArc != null ? Cursors.SizeAll :
                SelectedArcControl != null ? Cursors.SizeAll :
                SelectedText != null ? Cursors.SizeAll :
                SelectedGroupControl == 0 || SelectedGroupControl == 3 ? Cursors.SizeNWSE :
                SelectedGroupControl == 1 || SelectedGroupControl == 2 ? Cursors.SizeNESW :
                SelectedGroupControl == 4 ? Cursors.SizeAll :
                Mouse_Mode == Mouse_Mode_Type.Line || Mouse_Mode == Mouse_Mode_Type.Curve || 
                Mouse_Mode == Mouse_Mode_Type.Arc || Mouse_Mode==Mouse_Mode_Type.Pleated ? Cursors.Cross :
                    Cursors.Default;
        }
        private void RefreshPorperty()
        {
            if (SelectedGroup != null)
            {
                if (SelectedGroup.C.Count + SelectedGroup.L.Count == 1)
                {
                    if (SelectedGroup.L.Count == 1)
                    {
                        GraphLine l = SelectedGroup.L[0];
                        PorpertyList[0].Visible = true;
                        PorpertyList[1].Visible = false;
                        PorpertyList[2].Visible = false;
                        double x = (l.EndPoint.P.X - l.StartPoint.P.X) * (l.EndPoint.P.X - l.StartPoint.P.X);
                        double y = (l.EndPoint.P.Y - l.StartPoint.P.Y) * (l.EndPoint.P.Y - l.StartPoint.P.Y);
                        LineLengthLable.Text = "長度:" + (Math.Sqrt(x + y) / (LenthUnit == 0 ? 10 : 25.4)).ToString("#0.00") + (LenthUnit == 0 ? " cm" : " inch");
                        if (!PathList.Exists(a => a.L.Exists(b => b == l)))
                        {
                            LineSeamCheck.Enabled = false;
                            LineSeamText.Enabled = false;
                            LineSeamText.Text = "";
                            l.isSeam = false;
                            l.Seam = 0;
                        }
                        else
                        {
                            LineSeamCheck.Enabled = true;
                            LineSeamCheck.Checked = l.isSeam;
                            LineSeamText.Text = "" + l.SeamText;
                            LineUnitLable.Text = (LenthUnit == 0 ? "cm" : "inch");
                            LineSeamText.Enabled = l.isSeam;
                        }
                    }
                    else if (SelectedGroup.C.Count == 1)
                    {
                        GraphCurve c = SelectedGroup.C[0];
                        PorpertyList[1].Visible = true;
                        PorpertyList[0].Visible = false;
                        PorpertyList[2].Visible = false;
                        if (!PathList.Exists(a => a.C.Exists(b => b == c)))
                        {
                            CurveSeamCheck.Enabled = false;
                            CurveSeamText.Enabled = false;
                            CurveSeamText.Text = "";
                            c.isSeam = false;
                            c.Seam = 0;
                        }
                        else
                        {
                            CurveSeamCheck.Enabled = true;
                            CurveSeamCheck.Checked = c.isSeam;
                            CurveSeamText.Text = "" + c.SeamText;
                            CurveUnitLable.Text = (LenthUnit == 0 ? "cm" : "inch");
                            CurveSeamText.Enabled = c.isSeam;
                        }
                    }
                }
                else
                {
                    PorpertyList[0].Visible = false;
                    PorpertyList[1].Visible = false;
                    bool isp = false;
                    foreach (var path in PathList)
                    {
                        bool diff = false;
                        var diffL = SelectedGroup.L.Except(path.L);
                        var diffC = SelectedGroup.C.Except(path.C);
                        foreach (var t1 in diffL)
                            if (t1 != null)
                                diff = true;
                        foreach (var t1 in diffC)
                            if (t1 != null)
                                diff = true;
                        diffL = path.L.Except(SelectedGroup.L);
                        diffC = path.C.Except(SelectedGroup.C);
                        foreach (var t1 in diffL)
                            if (t1 != null)
                                diff = true;
                        foreach (var t1 in diffC)
                            if (t1 != null)
                                diff = true;
                        if (!diff)
                        {
                            isp = true;
                            break;
                        }
                    }
                    if(isp) PorpertyList[2].Visible = true;
                }
            }
            else
            {
                PorpertyList[0].Visible = false;
                PorpertyList[1].Visible = false;
                PorpertyList[2].Visible = false;
            }
        }
        private GraphPoint TestPath(GraphGroup g, out bool haveArc)
        {
            haveArc = false;
            List<GraphPoint> tempp = new List<GraphPoint>();
            foreach (var c in g.C)
            {
                if (!tempp.Exists(x => x == c.path[0]))
                    tempp.Add(c.path[0]);
                if (!tempp.Exists(x => x == c.path.Last()))
                    tempp.Add(c.path.Last());
            }
            foreach (var l in g.L)
            {
                if (!tempp.Exists(x => x == l.StartPoint))
                    tempp.Add(l.StartPoint);
                if (!tempp.Exists(x => x == l.EndPoint))
                    tempp.Add(l.EndPoint);
            }
            foreach(var a in g.A)
            {
                haveArc = true;
                if (!tempp.Exists(x => x == a.StartPoint))
                    tempp.Add(a.StartPoint);
                if (!tempp.Exists(x => x == a.EndPoint))
                    tempp.Add(a.EndPoint);
            }
            foreach (var poi in tempp)
            {
                int count = 0;
                var l = g.L.FindAll(x => x.StartPoint == poi || x.EndPoint == poi);
                count += l.Count;
                var c = g.C.FindAll(x => x.path[0] == poi || x.path.Last() == poi);
                count += c.Count;
                var a = g.A.FindAll(x => x.StartPoint == poi || x.EndPoint == poi);
                count += a.Count;
                if (count != 2)
                {
                    //MessageBox.Show("必須是一個封閉的圖形", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return null;
                }
            }
            return g.P[0];
        }
        private GraphGroup In_Path(GraphLine l,GraphCurve c)
        {
            foreach(var path in PathList)
            {
                if (path.L.Exists(x => x == l) && l != null)
                    return path;
                if (path.C.Exists(x => x == c) && c != null)
                    return path;
            }
            return null;
        }
        public static bool IsInPolygon(List<PointF> vert, PointF test)
        {
            int i, j, c = 0, nvert = vert.Count;
            for (i = 0, j = nvert - 1; i < nvert; j = i++)
            {
                if (((vert[i].Y > test.Y) != (vert[j].Y > test.Y)) && 
                    (test.X < (vert[j].X - vert[i].X) * (test.Y - vert[i].Y) / (vert[j].Y - vert[i].Y) + vert[i].X))
                {
                    c = 1 + c;
                }
            }
            if (c % 2 == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
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
        private GraphCurve PathToCurve(List<GraphPoint> path, int type = 0, PointF p = new PointF())
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
        GraphText Right_Temp_Text = null;

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
                if (a.StartPoint == toDeleteP || a.EndPoint == toDeleteP)
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
            if (!LineList.Exists(x => x == toDeleteL))
                return;
            toDeleteL.EndPoint.Relative--;
            toDeleteL.StartPoint.Relative--;
            LineList.Remove(toDeleteL);
            foreach(var p in PointsList)
            {
                if (p.MarkL == toDeleteL)
                    p.MarkL = null;
            }
            PathList.RemoveAll(x => x.L.Exists(y => y == toDeleteL));
            PointsList.RemoveAll(x => x.Relative <= 0 && x.MarkL == null);
        }
        private void DeleteCurveP(GraphCurve c, GraphPoint p)
        {
            if (!CurveList.Exists(x => x == c))
                return;
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
           PointsList.RemoveAll(x => x.Relative <= 0 && x.MarkL == null);
        }
        private void DeleteHoleCurve(GraphCurve c)
        {
            if (!CurveList.Exists(x => x == c))
                return;
            for(int i = 0; i < c.path.Count; i++)
            {
                c.path[i].Relative--;
            }
            CurveList.Remove(c);
            PathList.RemoveAll(x => x.C.Exists(y => y == c));
            PointsList.RemoveAll(x => x.Relative <= 0 && x.MarkL == null);
        }
        private void DeleteArc(GraphArc a)
        {
           if (!ArcList.Exists(x => x == a))
               return;
            a.StartPoint.Relative--;
            a.EndPoint.Relative--;
            ArcList.Remove(a);
            PointsList.RemoveAll(x => x.Relative <= 0 && x.MarkL == null);
        }
        private void DeleteGroup(GraphGroup g)
        {
            foreach(var l in g.L)
            {
                PathList.RemoveAll(x => x.L.Exists(y => y == l));
                DeleteLine(l);
            }
            foreach(var c in g.C)
            {
                PathList.RemoveAll(x => x.C.Exists(y => y == c));
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
                    foreach(var a in ArcList)
                    {
                        if(a.StartPoint == t[1])
                        {
                            a.StartPoint = t[0];
                        }
                        else if (a.EndPoint == t[1])
                        {
                            a.EndPoint = t[0];
                        }
                    }
                    t[0].Relative += t[1].Relative;
                    PointsList.Remove(t[1]);
                }
            }
        }

        private void LineInsert(GraphLine l,GraphPoint p,out GraphLine StoM,out GraphLine MtoE)
        {
            GraphLine line1 = new GraphLine(l.StartPoint, p);
            GraphLine line2 = new GraphLine(p, l.EndPoint);
            StoM = line1;
            MtoE = line2;
        }
        private GraphCurve CurveInsert(GraphCurve c,int cindex, GraphPoint p)
        {
            cindex--;
            GraphCurve ans = new GraphCurve();
            for(int i = 0; i < c.path.Count; i++)
            {
                ans.path.Add(c.path[i]);
                ans.disFirst.Add(c.disFirst[i]);
                ans.disSecond.Add(c.disSecond[i]);
                ans.type.Add(c.type[i]);
                if (i == cindex)
                {
                    PointF p0 = c.path[i].P, p3 = c.path[i + 1].P;
                    PointF p1 = new PointF(p0.X + c.disSecond[i].X, p0.Y + c.disSecond[i].Y), 
                           p2 = new PointF(p3.X + c.disFirst[i + 1].X, p3.Y + c.disFirst[i + 1].Y);
                    double t = Bezier_Find_t(p0, p1, p2, p3, p.P);
                    PointF q0 = new PointF(p0.X + (p1.X - p0.X) * (float)t, p0.Y + (p1.Y - p0.Y) * (float)t),
                           q1 = new PointF(p1.X + (p2.X - p1.X) * (float)t, p1.Y + (p2.Y - p1.Y) * (float)t),
                           q2 = new PointF(p2.X + (p3.X - p2.X) * (float)t, p2.Y + (p3.Y - p2.Y) * (float)t);

                    PointF r0 = new PointF(q0.X + (q1.X - q0.X) * (float)t, q0.Y + (q1.Y - q0.Y) * (float)t),
                           r1 = new PointF(q1.X + (q2.X - q1.X) * (float)t, q1.Y + (q2.Y - q1.Y) * (float)t);
                    ans.disSecond[i] = new PointF(q0.X - p0.X, q0.Y - p0.Y);
                    ans.type[i] = 1;

                    ans.path.Add(p);
                    ans.disFirst.Add(new PointF((r0.X - p.P.X), (r0.Y - p.P.Y)));
                    ans.disSecond.Add(new PointF(r1.X - p.P.X, r1.Y - p.P.Y));
                    ans.type.Add(2);

                    i++;
                    ans.path.Add(c.path[i]);
                    ans.disFirst.Add(new PointF(q2.X - p3.X, q2.Y - p3.Y));
                    ans.disSecond.Add(c.disSecond[i]);
                    ans.type.Add(1);
                }
            }
            return ans;
        }
        static private double Bezier_Find_t(PointF p0, PointF p1, PointF p2, PointF p3, PointF cutp)
        {
            double mindis = 99999999999;
            double t = 0;
            for(double i = 0; i < 1; i += 0.005)
            {
                double x = p0.X * Math.Pow(1 - i, 3) + 3 * p1.X * Math.Pow(1 - i, 2) * i + 3 * p2.X * (1 - i) * i * i + p3.X * i * i * i;
                double y = p0.Y * Math.Pow(1 - i, 3) + 3 * p1.Y * Math.Pow(1 - i, 2) * i + 3 * p2.Y * (1 - i) * i * i + p3.Y * i * i * i;
                double dis = Math.Pow(cutp.X - x, 2) + Math.Pow(cutp.Y - y, 2);
                if (dis < mindis)
                {
                    t = i;
                    mindis = dis;
                }
            }
            return t;
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
                TextList = new List<GraphText>();
                PathList = new List<GraphGroup>();
                pictureBox1.Width = (int)(595 * ZoomSize);
                pictureBox1.Height = (int)(842 * ZoomSize);
                pictureBox2.Width = (int)(595 * ZoomSize) + 20;
                pictureBox2.Height = (int)(842 * ZoomSize) + 20;
                Undo_Data = new List<TabpageData>();
                TabpageData a = new TabpageData();
                a.CurveL = CurveList;
                a.PointL = PointsList;
                a.LineL = LineList;
                a.GroupL = GroupList;
                a.ArcL = ArcList;
                a.TextL = TextList;
                a.PathL = PathList;
                a.Undo = Undo_Data;
                a.TabpageName = t.Text;
                a.width = 595;
                a.height = 842;
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
            else if(tabControl1.SelectedIndex >= 0 && TabpagesList.Count > 0 && TabpageDataList.Count > 0)
            {
                TabpagesList[tabControl1.SelectedIndex].Controls.Add(pictureBox1);
                TabpagesList[tabControl1.SelectedIndex].Controls.Add(pictureBox2);
                CurveList = TabpageDataList[tabControl1.SelectedIndex].CurveL;
                PointsList = TabpageDataList[tabControl1.SelectedIndex].PointL;
                LineList = TabpageDataList[tabControl1.SelectedIndex].LineL;
                GroupList = TabpageDataList[tabControl1.SelectedIndex].GroupL;
                ArcList = TabpageDataList[tabControl1.SelectedIndex].ArcL;
                TextList = TabpageDataList[tabControl1.SelectedIndex].TextL;
                PathList = TabpageDataList[tabControl1.SelectedIndex].PathL;
                pictureBox1.Width = (int)(TabpageDataList[tabControl1.SelectedIndex].width*ZoomSize);
                pictureBox1.Height = (int)(TabpageDataList[tabControl1.SelectedIndex].height*ZoomSize);
                pictureBox2.Width = pictureBox1.Width+20;
                pictureBox2.Height = pictureBox1.Height+20;
                Undo_Data = TabpageDataList[tabControl1.SelectedIndex].Undo;
                RefreshUndoCheck();
            }
        }

        #region ContextMenuStrip1 右鍵選單
        private void toolStripMenuItem2_Click(object sender, EventArgs e)//右鍵選單>刪除
        {
            if (Right_Temp_Point != null)
            {
                PathList.RemoveAll(x => x.P.Exists(y => y == Right_Temp_Point));
                DeletePoint(Right_Temp_Point);
            }
            else if (Right_Temp_Line != null)
            {
                PathList.RemoveAll(x => x.L.Exists(y => y == Right_Temp_Line));
                DeleteLine(Right_Temp_Line);
            }
            else if (Right_Temp_Curve != null)
            {
                PathList.RemoveAll(x => x.C.Exists(y => y == Right_Temp_Curve));
                DeleteHoleCurve(Right_Temp_Curve);
            }
            else if (Right_Temp_Arc != null)
                DeleteArc(Right_Temp_Arc);
            else if (Right_Temp_Group != null)
                DeleteGroup(Right_Temp_Group);
            else if (Right_Temp_Text != null)
                TextList.Remove(Right_Temp_Text);
            ClearRighttemp();
            PointsList.RemoveAll(x => x.Relative <= 0 && x.MarkL == null);
            Push_Undo_Data();
        }
        private void 新增節點ToolStripMenuItem_Click(object sender, EventArgs e)//右鍵選單>新增節點
        {
            if (Right_Temp_Line != null)
            {
                GraphPoint p = new GraphPoint(Right_Temp_Mouse_Pos.X, Right_Temp_Mouse_Pos.Y);
                if (PathList.Exists(x => x.L.Exists(y => y == Right_Temp_Line)))
                    PathList.RemoveAll(x => x.L.Exists(y => y == Right_Temp_Line));
                PointsList.Add(p);
                GraphLine line1, line2;
                LineInsert(Right_Temp_Line, p, out line1, out line2);
                LineList.Add(line1);
                Right_Temp_Line.StartPoint.Relative++;
                p.Relative++;
                LineList.Add(line2);
                Right_Temp_Line.EndPoint.Relative++;
                p.Relative++;
                DeleteLine(Right_Temp_Line);
                Push_Undo_Data();
            }
            else if (Right_Temp_Curve != null)
            {
                GraphPoint p = new GraphPoint(Right_Temp_Mouse_Pos.X, Right_Temp_Mouse_Pos.Y);
                if (PathList.Exists(x => x.C.Exists(y => y == Right_Temp_Curve)))
                    PathList.RemoveAll(x => x.C.Exists(y => y == Right_Temp_Curve));
                PointsList.Add(p);
                GraphCurve c = CurveInsert(Right_Temp_Curve, Right_Temp_Curve_Index, p);
                CurveList.Add(c);
                foreach (var poi in c.path)
                {
                    poi.Relative++;
                }
                DeleteHoleCurve(Right_Temp_Curve);
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
            foreach (var g in GroupList)
            {
                if (g.A.Exists(x => x == Right_Temp_Arc))
                    ingroup = true;
            }
            if (ingroup)
            {
                MessageBox.Show("請將弧線解除群組後再進行轉換", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            GraphArc arc = Right_Temp_Arc;
            GraphCurve c = new GraphCurve();
            var parr = arc.to_cubic_bezier();
            float disx = parr[1].X - parr[0].X;
            float disy = parr[1].Y - parr[0].Y;

            c.path.Add(arc.StartPoint);
            arc.StartPoint.Relative++;
            c.disFirst.Add(new PointF(-disx, -disy));
            c.disSecond.Add(new PointF(disx, disy));
            c.type.Add(0);

            disx = parr[2].X - parr[3].X;
            disy = parr[2].Y - parr[3].Y;
            c.path.Add(arc.EndPoint);
            arc.EndPoint.Relative++;
            c.disFirst.Add(new PointF(disx, disy));
            c.disSecond.Add(new PointF(-disx, -disy));
            c.type.Add(0);

            
            DeleteArc(arc);
            CurveList.Add(c);
            PointCombine();
            Push_Undo_Data();

        }
        private void 組為圖形ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GraphGroup path = new GraphGroup();
            bool tarc;
            var p = TestPath(SelectedGroup, out tarc);
            if (tarc)
            {
                if (MessageBox.Show("此動作會將所選圓弧轉為曲線", "訊息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) != DialogResult.OK)
                {
                    return;
                }
            }
            var aL = new List<GraphArc>();
            var cL = new List<GraphCurve>();
            if (p == null)
                return;
            do
            {
                var l = SelectedGroup.L.FindAll(x => x.StartPoint == p || x.EndPoint == p);
                var c = SelectedGroup.C.FindAll(x => x.path[0] == p || x.path.Last() == p);
                var a = SelectedGroup.A.FindAll(x => x.StartPoint == p || x.EndPoint == p);
                if (l.Count > 0)
                {
                    if (l[0].StartPoint == p)
                        p = l[0].EndPoint;
                    else
                        p = l[0].StartPoint;
                    l[0].isSeam = false;
                    l[0].Seam = 0;
                    path.L.Add(l[0]);
                    path.C.Add(null);
                    SelectedGroup.L.Remove(l[0]);
                }
                else if (c.Count > 0)
                {
                    if (c[0].path[0] == p)
                        p = c[0].path.Last();
                    else
                        p = c[0].path[0];
                    c[0].isSeam = false;
                    c[0].Seam = 0;
                    path.L.Add(null);
                    path.C.Add(c[0]);
                    SelectedGroup.C.Remove(c[0]);
                }
                else if (a.Count > 0)
                {
                    var bez = a[0].to_cubic_bezier();
                    var curve = new GraphCurve();
                    if (a[0].StartPoint == p)
                    {
                        curve.path.Add(p);
                        p.Relative++;
                        curve.disFirst.Add(new PointF(p.P.X - bez[1].X, p.P.Y - bez[1].Y));
                        curve.disSecond.Add(new PointF(bez[1].X - p.P.X, bez[1].Y - p.P.Y));
                        curve.type.Add(0);

                        p = a[0].EndPoint;
                        curve.path.Add(p);
                        p.Relative++;
                        curve.disFirst.Add(new PointF(bez[2].X - p.P.X, bez[2].Y - p.P.Y));
                        curve.disSecond.Add(new PointF(p.P.X - bez[2].X, p.P.Y - bez[2].Y));
                        curve.type.Add(0);
                    }
                    else
                    {
                        curve.path.Add(p);
                        p.Relative++;
                        curve.disFirst.Add(new PointF(p.P.X - bez[2].X, p.P.Y - bez[2].Y));
                        curve.disSecond.Add(new PointF(bez[2].X - p.P.X, bez[2].Y - p.P.Y));
                        curve.type.Add(0);
                        p = a[0].StartPoint;
                        curve.path.Add(p);
                        p.Relative++;
                        curve.disFirst.Add(new PointF(bez[1].X - p.P.X, bez[1].Y - p.P.Y));
                        curve.disSecond.Add(new PointF(p.P.X - bez[1].X, p.P.Y - bez[1].Y));
                        curve.type.Add(0);
                    }
                    curve.isSeam = false;
                    curve.Seam = 0;
                    aL.Add(a[0]);
                    cL.Add(curve);
                    path.L.Add(null);
                    path.C.Add(curve);
                    SelectedGroup.A.Remove(a[0]);
                }
                else
                {
                    MessageBox.Show("必須是一個封閉的圖形", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            } while (p != SelectedGroup.P[0]);
            for(int i = 0; i < aL.Count; i++)
            {
                DeleteArc(aL[i]);
                CurveList.Add(cL[i]);
            }
            SelectedGroup = null;
            for (int i = 0; i < path.L.Count; i++)
            {
                GraphPoint thisS, thisE;
                if (path.L[i] != null)
                {
                    thisS = path.L[i].StartPoint;
                    thisE = path.L[i].EndPoint;
                }
                else
                {
                    thisS = path.C[i].path[0];
                    thisE = path.C[i].path.Last();
                }
                if (!path.P.Exists(x => x == thisS))
                    path.P.Add(thisS);
                if (!path.P.Exists(x => x == thisE))
                    path.P.Add(thisE);
            }
            PathList.Add(path);
            Push_Undo_Data();
        }
        private void 解除圖形ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedLine != null)
            {
                GraphGroup g = PathList.Find(x => x.L.FindAll(y => y == SelectedLine).Count > 0);
                foreach(var l in g.L)
                {
                    l.Seam = -1;
                    l.isSeam = false;
                }
                PathList.Remove(g);
                Push_Undo_Data();
            }
            else if (SelectedCurve != null)
            {
                GraphGroup g = PathList.Find(x => x.C.FindAll(y => y == SelectedCurve).Count > 0);
                PathList.Remove(g);
                Push_Undo_Data();
            }
        }
        private void 直線等分ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Right_Temp_Line == null)
                return;
            if (直線等分ToolStripMenuItem.Text == "直線等分")
            {
                等分 f2 = new 等分();
                if (f2.ShowDialog() == DialogResult.OK)
                {
                    int num = f2.ans;
                    if (num <= 1)
                        return;
                    GraphLine line = Right_Temp_Line;
                    float xdis = line.EndPoint.P.X - line.StartPoint.P.X, ydis = line.EndPoint.P.Y - line.StartPoint.P.Y;
                    xdis /= num;
                    ydis /= num;
                    for (int i = 1; i < num; i++)
                    {
                        GraphPoint p = new GraphPoint(line.StartPoint.P.X + xdis * i, line.StartPoint.P.Y + ydis * i);
                        p.MarkL = line;
                        p.part = (float)i / num;
                        PointsList.Add(p);
                    }
                    Push_Undo_Data();
                }
            }
            else
            {
                PointsList.RemoveAll(x => x.MarkL == SelectedLine);
            }
        }
        private void 選取整個圖形ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Right_Temp_Line != null || Right_Temp_Curve != null)
            {
                SelectedGroup = new GraphGroup();
                GraphGroup t = new GraphGroup();
                if (Right_Temp_Line != null)
                {
                    t = PathList.FindLast(x => x.L.Exists(y => y == Right_Temp_Line));
                }
                else if (Right_Temp_Curve != null)
                {
                    t = PathList.FindLast(x => x.C.Exists(y => y == Right_Temp_Curve));
                }
                foreach (var p in t.P)
                    if (p != null)
                        SelectedGroup.P.Add(p);
                foreach (var l in t.L)
                    if (l != null)
                        SelectedGroup.L.Add(l);
                foreach (var c in t.C)
                    if (c != null)
                        SelectedGroup.C.Add(c);
                foreach (var a in t.A)
                    if (a != null)
                        SelectedGroup.A.Add(a);
                foreach (var g in t.G)
                    if (g != null)
                        SelectedGroup.G.Add(g);
            }
            ClearRighttemp();
        }
        private void 移除距離標示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Right_Temp_Curve != null)
            {
                PDistList.RemoveAll(x => x.C1 == Right_Temp_Curve || x.C2 == Right_Temp_Curve);
            }
            else if (Right_Temp_Line != null)
            {
                PDistList.RemoveAll(x => x.L1 == Right_Temp_Line || x.L2 == Right_Temp_Line);
            }
        }
        private void 變更顏色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(Right_Temp_Line != null)
            {
                colorDialog1.Color = Right_Temp_Line.co;
                if(colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    Right_Temp_Line.co = colorDialog1.Color;
                    Push_Undo_Data();
                }
                Refresh();
            }
            else if (Right_Temp_Curve != null)
            {
                colorDialog1.Color = Right_Temp_Curve.co;
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    Right_Temp_Curve.co = colorDialog1.Color;
                    Push_Undo_Data();
                }
                Refresh();
            }
            else if (Right_Temp_Arc != null)
            {
                colorDialog1.Color = Right_Temp_Arc.co;
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    Right_Temp_Arc.co = colorDialog1.Color;
                    Push_Undo_Data();
                }
                Refresh();
            }
        }
        private void ClearRighttemp()
        {
            Right_Temp_Point = null;
            Right_Temp_Line = null;
            Right_Temp_Curve = null;
            Right_Temp_Curve_Index = -1;
            Right_Temp_Arc = null;
            Right_Temp_Group = null;
            Right_Temp_Text = null;
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
            Rename f2 = new Rename(TabpageDataList[a].TabpageName);
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
        private void 建立分頁副本ToolStripMenuItem_Click(object sender, EventArgs ev)
        {
            TabpageData t = new TabpageData();
            foreach (var p in PointsList)
            {
                GraphPoint gp = new GraphPoint(p.P.X, p.P.Y);
                t.PointL.Add(gp);
            }
            foreach (var l in LineList)
            {
                int s = PointsList.FindIndex(x => x == l.StartPoint);
                int e = PointsList.FindIndex(x => x == l.EndPoint);
                t.PointL[s].Relative++;
                t.PointL[e].Relative++;
                GraphLine tl = new GraphLine(t.PointL[s], t.PointL[e]);
                tl.Seam = l.Seam;
                tl.SeamText = l.SeamText;
                tl.isSeam = l.isSeam;
                tl.co = l.co;
                t.LineL.Add(tl);
            }
            foreach (var c in CurveList)
            {
                GraphCurve gc = new GraphCurve();
                for (int i = 0; i < c.path.Count; i++)
                {
                    int index = PointsList.FindIndex(x => x == c.path[i]);
                    gc.path.Add(t.PointL[index]);
                    t.PointL[index].Relative++;
                    gc.disFirst.Add(new PointF(c.disFirst[i].X, c.disFirst[i].Y));
                    gc.disSecond.Add(new PointF(c.disSecond[i].X, c.disSecond[i].Y));
                    gc.type.Add(c.type[i]);
                }
                gc.isSeam = c.isSeam;
                gc.Seam = c.Seam;
                gc.co = c.co;
                t.CurveL.Add(gc);
            }
            foreach (var a in ArcList)
            {
                int s = PointsList.FindIndex(x => x == a.StartPoint);
                int e = PointsList.FindIndex(x => x == a.EndPoint);
                t.PointL[s].Relative++;
                t.PointL[e].Relative++;
                GraphArc ta = new GraphArc(t.PointL[s], t.PointL[e], a.getControlPoint());
                ta.co = a.co;
                t.ArcL.Add(ta);
            }
            for (int i = 0; i < PointsList.Count; i++)
            {
                if (PointsList[i].MarkL != null)
                {
                    t.PointL[i].MarkL = t.LineL[LineList.FindIndex(x => x == PointsList[i].MarkL)];
                    t.PointL[i].part = PointsList[i].part;
                }
            }
            foreach (var g in GroupList)
            {
                t.GroupL.Add(Group_Copy(g, t, TabpageDataList[tabControl1.SelectedIndex]));
            }
            foreach (var path in PathList)
            {
                t.PathL.Add(Group_Copy(path, t, TabpageDataList[tabControl1.SelectedIndex]));
            }
            t.width = (int)(pictureBox1.Width / ZoomSize);
            t.height = (int)(pictureBox1.Height / ZoomSize);
            t.TabpageName = "複製-" + TabpageDataList[tabControl1.SelectedIndex].TabpageName;

            Undo_Data = new List<TabpageData>();

            TabPage tp = new TabPage(t.TabpageName);
            TabpageDataList.Add(t);
            tp.AutoScroll = true;
            tp.Controls.Add(pictureBox1);
            TabpagesList.Insert(TabpagesList.Count - 1, tp);
            tabControl1.TabPages.Insert(TabpagesList.Count - 2, tp);
            tabControl1.SelectedIndex = TabpagesList.Count - 2;
            TabpagesList[tabControl1.SelectedIndex].Controls.Add(pictureBox1);
            TabpagesList[tabControl1.SelectedIndex].Controls.Add(pictureBox2);
            Push_Undo_Data();
            RefreshUndoCheck();
            Invalidate();
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

        private void LineSeamCheck_Click(object sender, EventArgs e)
        {
            if (SelectedGroup != null)
            {
                SelectedGroup.L[0].isSeam = !SelectedGroup.L[0].isSeam;
                float temp;
                if(SelectedGroup.L[0].isSeam == false)
                {
                    SelectedGroup.L[0].Seam = 0;
                }
                else
                {
                    if (SelectedGroup.L[0].Seam == 0)
                    {
                        SelectedGroup.L[0].Seam = 0.5F * (LenthUnit == 0 ? 72 / 2.54F : 72);
                        SelectedGroup.L[0].SeamText = 0.5F;
                        LineSeamText.Text = "0.5";
                    }
                    else if (float.TryParse(LineSeamText.Text, out temp))
                    {
                        SelectedGroup.L[0].SeamText = temp;
                        SelectedGroup.L[0].Seam = temp * (LenthUnit == 0 ? 72 / 2.54F : 72);
                    }
                    else
                    {
                        LineSeamText.Text = "" + SelectedGroup.L[0].SeamText;
                    }
                }
            }
            RefreshPorperty();
        }
        private void LineSeamText_TextChanged(object sender, EventArgs e)
        {
            if (SelectedGroup != null)
            {
                float temp;
                if (SelectedGroup.L[0].Seam == -1)
                {
                    SelectedGroup.L[0].Seam = 0.5F * (LenthUnit == 0 ? 72 / 2.54F : 72);
                    SelectedGroup.L[0].SeamText = 0.5F;
                    LineSeamText.Text = "0.5";
                    RefreshPorperty();
                    pictureBox1.Refresh();
                }
                else if(LineSeamText.Text == "")
                {
                    ;
                }
                else if(LineSeamText.Text.Last() == '.' || (LineSeamText.Text.Last() == '0' && LineSeamText.Text.Contains(".")))
                {
                    ;
                }
                else if (float.TryParse(LineSeamText.Text, out temp))
                {
                    SelectedGroup.L[0].Seam = temp * (LenthUnit == 0 ? 72 / 2.54F : 72);
                    SelectedGroup.L[0].SeamText = temp;
                    var pa = PathList.Find(x => x.L.Exists(y => y == SelectedGroup.L[0]));
                    if(pa != null)
                    {
                        int index = pa.L.FindIndex(x => x == SelectedGroup.L[0]);
                        float pre_m = (pa.L[index].EndPoint.P.X-pa.L[index].StartPoint.P.X) / (pa.L[index].EndPoint.P.Y - pa.L[index].StartPoint.P.Y);
                        for(int i = 1; i < pa.L.Count(); i++)
                        {
                            int nowi = (i + index) % pa.L.Count();
                            if (pa.L[nowi] == null) break;
                            float now_m = (pa.L[nowi].EndPoint.P.X - pa.L[nowi].StartPoint.P.X) / (pa.L[nowi].EndPoint.P.Y - pa.L[nowi].StartPoint.P.Y);
                            if (Math.Abs(pre_m - now_m) < 0.001)
                            {
                                pa.L[nowi].Seam = temp;
                                pa.L[nowi].isSeam = true;
                                pre_m = now_m;
                            }
                            else break;
                        }
                        for (int i = 1; i < pa.L.Count(); i++)
                        {
                            int nowi = (index - i + pa.L.Count()) % pa.L.Count();
                            if (pa.L[nowi] == null) break;
                            float now_m = (pa.L[nowi].EndPoint.P.X - pa.L[nowi].StartPoint.P.X) / (pa.L[nowi].EndPoint.P.Y - pa.L[nowi].StartPoint.P.Y);
                            if (Math.Abs(pre_m - now_m) < 0.001)
                            {
                                pa.L[nowi].Seam = temp;
                                pa.L[nowi].isSeam = true;
                                pre_m = now_m;
                            }
                            else break;
                        }
                    }
                    RefreshPorperty();
                    pictureBox1.Refresh();
                }
                else
                {
                    //LineSeamText.Text = "" + SelectedGroup.L[0].SeamText;
                    //RefreshPorperty();
                    //pictureBox1.Refresh();
                    ;
                }
            }
        }

        private void CurveSeamCheck_Click(object sender, EventArgs e)
        {
            if (SelectedGroup != null)
            {
                SelectedGroup.C[0].isSeam = !SelectedGroup.C[0].isSeam;
                float temp;
                if(SelectedGroup.C[0].isSeam == false)
                {
                    SelectedGroup.C[0].Seam = 0;
                }
                else
                {
                    if (SelectedGroup.C[0].Seam == 0)
                    {
                        SelectedGroup.C[0].Seam = 0.5F * (LenthUnit == 0 ? 72 / 2.54F : 72);
                        SelectedGroup.C[0].SeamText = 0.5F;
                        CurveSeamText.Text = "0.5";
                    }
                    else if (float.TryParse(CurveSeamText.Text, out temp))
                    {
                        SelectedGroup.C[0].SeamText = temp;
                        SelectedGroup.C[0].Seam = temp * (LenthUnit == 0 ? 72 / 2.54F : 72);
                    }
                    else
                    {
                        CurveSeamText.Text = "" + SelectedGroup.C[0].SeamText;
                    }
                }
            }
            RefreshPorperty();
        }
        private void CurveSeamText_TextChanged(object sender, EventArgs e)
        {
            if (SelectedGroup != null)
            {
                float temp;
                if (SelectedGroup.C[0].Seam == -1)
                {
                    SelectedGroup.C[0].SeamText = 0.5F;
                    SelectedGroup.C[0].Seam = 0.5F * (LenthUnit == 0 ? 72 / 2.54F : 72);
                    CurveSeamText.Text = "0.5";
                    RefreshPorperty();
                    pictureBox1.Refresh();
                }
                else if (CurveSeamText.Text == "")
                {
                    ;
                }
                else if (CurveSeamText.Text.Last() == '.' || (CurveSeamText.Text.Last() == '0' && CurveSeamText.Text.Contains(".")))
                {
                    ;
                }
                else if (float.TryParse(CurveSeamText.Text, out temp))
                {
                    SelectedGroup.C[0].Seam = temp * (LenthUnit == 0 ? 72 / 2.54F : 72);
                    SelectedGroup.C[0].SeamText = temp;
                    RefreshPorperty();
                    pictureBox1.Refresh();
                }
                else
                {
                    //CurveSeamText.Text = "" + SelectedGroup.C[0].Seam / (LenthUnit == 0 ? 72 / 2.54F : 72);
                    //RefreshPorperty();
                    //pictureBox1.Refresh();
                    ;
                }
            }
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.PageUnit = GraphicsUnit.Document;
            Print_Paint_Lines(e);
            Print_Paint_Curves(e);
            Print_Paint_Arcs(e);
            Print_Paint_Seam(e);
            Print_Paint_Text(e);
            Print_Paint_PathDist(e);
        }

        private void Print_Paint_Lines(PrintPageEventArgs e)
        {
            foreach (var line in LineList)
            {
                var color = line.co;
                var size = 1;
                var pen = new Pen(color, size);
                e.Graphics.DrawLine(pen, line.StartPoint.P.X * 300 / 72, line.StartPoint.P.Y * 300 / 72, line.EndPoint.P.X * 300 / 72, line.EndPoint.P.Y * 300 / 72);
            }
        }
        private void Print_Paint_Curves(PrintPageEventArgs e)
        {
            foreach (var c in CurveList)
            {
                var color = c.co;
                var size = 1;
                var pen = new Pen(color, size);
                List<PointF> t = GraphCurveToBez(c);
                for (int i = 0; i < t.Count; i++)
                {
                    t[i] = new PointF(t[i].X * 300 / 72, t[i].Y * 300 / 72);
                }
                e.Graphics.DrawBeziers(pen, t.ToArray());
            }
        }
        private void Print_Paint_Arcs(PrintPageEventArgs e)
        {
            foreach (var a in ArcList)
            {
                var color = a.co;
                var size = 1;
                var pen = new Pen(color, size);
                var arr = a.to_cubic_bezier();
                for (int i = 0; i < 4; i++)
                {
                    arr[i].X *= 300 / 72;
                    arr[i].Y *= 300 / 72;
                }
                e.Graphics.DrawBezier(pen, arr[0], arr[1], arr[2], arr[3]);
            }
        }
        private void Print_Paint_Seam(PrintPageEventArgs e)
        {
            var pe = new Pen(Color.Gray, 1);
            float[] dash = { 5, 5 };
            pe.DashPattern = dash;
            foreach (var path in PathList)
            {
                float minx, miny, maxx, maxy;
                minx = path.P[0].P.X;
                miny = path.P[0].P.Y;
                maxx = path.P[0].P.X;
                maxy = path.P[0].P.Y;
                foreach (var po in path.P)
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
                List<PointF> poly_vert = new List<PointF>();
                List<float> distList = new List<float>();
                bool[] check = new bool[path.L.Count]; for (int i = 0; i < check.Count(); i++) check[i] = false;
                List<bool> IsCurveP = new List<bool>();
                GraphPoint p = path.P[0];
                poly_vert.Add(p.P);
                IsCurveP.Add(false);
                do
                {
                    List<GraphLine> l = new List<GraphLine>();
                    List<GraphCurve> c = new List<GraphCurve>();
                    l = path.L.FindAll(x => x != null ? (x.StartPoint == p || x.EndPoint == p) : false);
                    c = path.C.FindAll(x => x != null ? (x.path[0] == p || x.path.Last() == p) : false);
                    if (l.Count > 0)
                    {
                        if (check[path.L.FindIndex(x => x == l[0])] == true)
                            l.RemoveAt(0);
                    }
                    if (c.Count > 0)
                    {
                        if (check[path.C.FindIndex(x => x == c[0])] == true)
                            c.RemoveAt(0);
                    }
                    if (l.Count > 0)
                    {
                        if (l[0].StartPoint == p)
                            p = l[0].EndPoint;
                        else
                            p = l[0].StartPoint;
                        distList.Add(l[0].Seam);
                        poly_vert.Add(p.P);
                        IsCurveP.Add(false);
                        check[path.L.FindIndex(x => x == l[0])] = true;
                    }
                    else if (c.Count > 0)
                    {
                        IsCurveP[IsCurveP.Count - 1] = true;
                        if (c[0].path[0] == p)
                        {
                            p = c[0].path.Last();
                            for (int i = 1; i < c[0].path.Count; i++)
                            {
                                poly_vert.Add(c[0].path[i].P);
                                distList.Add(c[0].Seam);
                                IsCurveP.Add(true);
                            }
                        }
                        else
                        {
                            p = c[0].path[0];
                            for (int i = c[0].path.Count - 2; i >= 0; i--)
                            {
                                poly_vert.Add(c[0].path[i].P);
                                distList.Add(c[0].Seam);
                                IsCurveP.Add(true);
                            }
                        }
                        IsCurveP[IsCurveP.Count - 1] = false;
                        check[path.C.FindIndex(x => x == c[0])] = true;
                    }
                } while (p != path.P[0]);
                poly_vert.RemoveAt(0);
                IsCurveP.RemoveAt(0);
                do
                {
                    if (IsCurveP[0])
                    {
                        if (path.C.Exists(x => x != null ? (x.path[0].P.X == poly_vert[0].X && x.path[0].P.Y == poly_vert[0].Y) : false))
                            break;
                        else if (path.C.Exists(x => x != null ? (x.path.Last().P.X == poly_vert[0].X && x.path.Last().P.Y == poly_vert[0].Y) : false))
                            break;
                        IsCurveP.Add(IsCurveP[0]);
                        poly_vert.Add(poly_vert[0]);
                        IsCurveP.RemoveAt(0);
                        poly_vert.RemoveAt(0);
                        continue;
                    }
                    else
                        break;
                } while (true);
                List<PointF> forextend = new List<PointF>();
                forextend.Add(poly_vert[0]);
                float pre_m = (poly_vert[1].X - poly_vert[0].X) / (poly_vert[1].Y - poly_vert[0].X);
                for (int i = 1; i < poly_vert.Count; i++)
                {
                    int next = (i + 1) % poly_vert.Count;
                    float now_m = (poly_vert[next].X - poly_vert[i].X) / (poly_vert[next].Y - poly_vert[i].X);
                    if (now_m == pre_m)
                        forextend.Add(new PointF(poly_vert[i].X + 1, poly_vert[i].Y + 1));
                    else
                        forextend.Add(poly_vert[i]);
                    pre_m = now_m;
                }
                List<PointF> todrawl = extend_polygon(forextend, distList);
                for (int i = 0; i < todrawl.Count; i++)
                {
                    if (IsCurveP[i] == false)
                    {
                        e.Graphics.DrawLine(pe, todrawl[i].X * 300 / 72, todrawl[i].Y * 300 / 72, todrawl[(i + 1) % todrawl.Count].X * 300 / 72, todrawl[(i + 1) % todrawl.Count].Y * 300 / 72);
                    }
                    else
                    {
                        int pathcount = 0;
                        int cindex = path.C.FindIndex(x => x != null ? (x.path.Exists(y => y.P.X == poly_vert[(i + 1) % todrawl.Count].X && y.P.Y == poly_vert[(i + 1) % todrawl.Count].Y))
                                                                    && (x.path.Exists(y => y.P.X == poly_vert[(i) % todrawl.Count].X && y.P.Y == poly_vert[(i) % todrawl.Count].Y)) : false);
                        bool tsfe = (path.C[cindex].path[0].P.X == poly_vert[i].X && path.C[cindex].path[0].P.Y == poly_vert[i].Y);
                        int cpathindex = tsfe ? 0 : path.C[cindex].path.Count - 1;
                        int pors = tsfe ? 1 : -1;
                        while (IsCurveP[i % todrawl.Count] == true)
                        {
                            if (pathcount == path.C[cindex].path.Count - 1) break;
                            PointF c1, c2;
                            float b1 = (float)(Math.Sqrt(Math.Pow(todrawl[i].X, 2) + Math.Pow(todrawl[i].Y, 2)) / Math.Sqrt(Math.Pow(forextend[i].X, 2) + Math.Pow(forextend[i].Y, 2))),
                                b2 = (float)(Math.Sqrt(Math.Pow(todrawl[(i + 1) % todrawl.Count].X, 2) + Math.Pow(todrawl[(i + 1) % todrawl.Count].Y, 2)) /
                                Math.Sqrt(Math.Pow(forextend[(i + 1) % todrawl.Count].X, 2) + Math.Pow(forextend[(i + 1) % todrawl.Count].Y, 2)));
                            if (tsfe)
                            {
                                c1 = new PointF(todrawl[i].X + path.C[cindex].disSecond[cpathindex].X * b1, todrawl[i].Y + path.C[cindex].disSecond[cpathindex].Y * b1);
                                c2 = new PointF(todrawl[(i + 1) % todrawl.Count].X + path.C[cindex].disFirst[cpathindex + pors].X * b2, todrawl[(i + 1) % todrawl.Count].Y + path.C[cindex].disFirst[cpathindex + pors].Y * b2);
                            }
                            else
                            {
                                c1 = new PointF(todrawl[i].X + path.C[cindex].disFirst[cpathindex].X * b1, todrawl[i].Y + path.C[cindex].disFirst[cpathindex].Y * b1);
                                c2 = new PointF(todrawl[(i + 1) % todrawl.Count].X + path.C[cindex].disSecond[cpathindex + pors].X * b2, todrawl[(i + 1) % todrawl.Count].Y + path.C[cindex].disSecond[cpathindex + pors].Y * b2);
                            }
                            PointF[] cp = { new PointF(todrawl[i].X * 300 / 72 , todrawl[i].Y* 300 / 72), new PointF(c1.X* 300 / 72, c1.Y* 300 / 72), new PointF(c2.X* 300 / 72, c2.Y* 300 / 72),
                                            new PointF(todrawl[(i + 1) % todrawl.Count].X* 300 / 72, todrawl[(i + 1) % todrawl.Count].Y* 300 / 72) };
                            e.Graphics.DrawBeziers(pe, cp);
                            cpathindex += pors;
                            i++;
                            pathcount++;
                        }
                        i--;

                    }
                }
            }
        }
        private void Print_Paint_Text(PrintPageEventArgs e)
        {
            foreach (var s in TextList)
            {
                var fo = new Font("新細明體", 12);
                e.Graphics.DrawString(s.S, fo, Brushes.Black, s.P.X * 300 / 72, s.P.Y * 300 / 72);
            }
        }
        private void Print_Paint_PathDist(PrintPageEventArgs e)
        {
            var pe_line = new Pen(Color.Gray, 1);
            var fo = new Font("新細明體", 12);
            List<PathDistance> pdl = new List<PathDistance>();
            foreach (var pd in PDistList)
            {
                PointF pt1, pt2;
                float dist;
                bool inlist = false;
                pd.Get_Dist_Point(out dist, out pt1, out pt2);
                if (pd.type == 0)
                    inlist = LineList.Exists(x => x == pd.L1) && LineList.Exists(x => x == pd.L2);
                else if (pd.type == 1)
                    inlist = LineList.Exists(x => x == pd.L1) && CurveList.Exists(x => x == pd.C1);
                else if (pd.type == 2)
                    inlist = CurveList.Exists(x => x == pd.C1) && CurveList.Exists(x => x == pd.C2);
                if ((pt1.X == -1 && pt1.Y == -1) || (pt2.X == -1 && pt2.Y == -1) || inlist == false)
                {
                    pdl.Add(pd);
                }
                else
                {
                    e.Graphics.DrawLine(pe_line, pt1.X * 300 / 72, pt1.Y * 300 / 72, pt2.X * 300 / 72, pt2.Y * 300 / 72);
                    PointF mid = new PointF((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2);
                    if (LenthUnit == 0)
                        dist /= 72/2.54F;
                    else
                        dist /= 72F;
                    e.Graphics.DrawString(dist.ToString("F") + (LenthUnit == 0 ? " cm" : " inch"), fo, Brushes.Black, mid.X * 300 / 72, mid.Y * 300 / 72);
                }
            }
            foreach (var pd in pdl)
                PDistList.Remove(pd);
        }

        private void 預覽列印ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }

        

    }
} 
