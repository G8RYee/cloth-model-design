using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Printing;
using 繪圖用lib;
using BrightIdeasSoftware;
using System.Data.SqlClient;
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
        List<GraphPoint> SupPointsList = new List<GraphPoint>();
        List<GraphLine> SupLineList = new List<GraphLine>();
        List<Formula> FormulaList = new List<Formula>();
        List<FormulaToLine> FormulaToLineList = new List<FormulaToLine>();

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
        PathDistance SelectedDist = null;
        GraphPoint SelectedSupPoint = null;
        GraphLine SelectedSupLine = null;
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
        GrainMark ClothGrainMark = new GrainMark();
        bool SupLine_Visible = true;
        bool SupLine_Lock = false;
        bool Object_Name_Visible = true;
        double[] ClothStandardSize = new double[29];
        string ClothStandardName = "";
        string ClothStandardNumber = "";
        bool EditFL = false;
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
            Cursor, Line, Curve, Arc, Text, Pleated, Cutting, Dist_Hori, Dist_Verti, Dist_2points, Dist_RightLine, SupLine, FormulaToL
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
                dash = false;
            }
            public GraphPoint StartPoint;
            public GraphPoint EndPoint;
            public string name = "";
            public float Seam;
            public float SeamText;
            public bool isSeam;
            public Color co;
            public bool dash;
            public bool fix = true;
        }
        public class GraphPoint
        {
            public GraphPoint(float x, float y)
            {
                this.P = new PointF(x, y);
                Relative = 0;
            }
            public PointF P;
            public int Relative;
            public GraphLine MarkL = null;
            public float part = 0;
            public string name = "";
            public string check_name(List<GraphPoint> pl)
            {
                string name = "";
                for (int i = 1; i < 10000; i++)
                {
                    if (!pl.Exists(x => x.name == "P" + i))
                    {
                        name = "P" + i;
                        break;
                    }
                }
                return name;
            }
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
            public string name = "";
            public bool isSeam = false;
            public float Seam = 0;
            public float SeamText;
            public Color co = Color.Black;
            public bool dash = false;
            public bool fix = true;
            public bool equal(GraphCurve c)
            {
                if (path.Count != c.path.Count)
                    return false;
                for (int i = 0; i < path.Count; i++)
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
            public string name = "";
            public Color co = Color.Black;
            public bool dash = false;
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
            public void setControlPoint(float x, float y)
            {
                ControlPoint = new PointF(x - StartPoint.P.X, y - StartPoint.P.Y);
            }
            public PointF[] to_cubic_bezier()
            {
                PointF p0 = StartPoint.P,
                       p1 = new PointF((StartPoint.P.X + (ControlPoint.X + StartPoint.P.X) * 2) / 3, (StartPoint.P.Y + (ControlPoint.Y + StartPoint.P.Y) * 2) / 3),
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
            public List<PathDistance> PDistL = new List<PathDistance>();
            public List<GraphPoint> SupPointL = new List<GraphPoint>();
            public List<GraphLine> SupLineL = new List<GraphLine>();
            public List<FormulaToLine> FtoLL = new List<FormulaToLine>();
            public List<TabpageData> Undo = new List<TabpageData>();
            public int width, height;
            public int Undo_index = -1;
            public int Pos;
            public double Angle;
            public string TabpageName;
            public GrainMark ClothGrainMark = new GrainMark();
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
            public bool straight = false;
        }
        public class PathDistance
        {
            public bool Is_HV;
            public bool T_hori_F_verti;
            public GraphLine L1 = null;
            public GraphLine L2 = null;
            public GraphCurve C1 = null;
            public int cindex1;
            public GraphCurve C2 = null;
            public int cindex2;
            public GraphArc A1 = null;
            public GraphArc A2 = null;
            public float XY_Pos;
            public PointF anchor1, anchor2;
            public int type;  //ll lc cc la ca aa
            public bool is_SupL1 = false, is_SupL2 = false;
            public PathDistance() { }
            public PathDistance(GraphLine l1, GraphLine l2, float pos, bool thfv)
            {
                L1 = l1;
                L2 = l2;
                XY_Pos = pos;
                T_hori_F_verti = thfv;
                Is_HV = true;
                type = 0;
            }
            public PathDistance(GraphLine l1, GraphCurve c1, int ci1, float pos, bool thfv)
            {
                L1 = l1;
                C1 = c1;
                cindex1 = ci1;
                XY_Pos = pos;
                T_hori_F_verti = thfv;
                Is_HV = true;
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
                Is_HV = true;
                type = 2;
            }
            public PathDistance(GraphLine l1, GraphArc ar1, float pos, bool thfv)
            {
                L1 = l1;
                A1 = ar1;
                XY_Pos = pos;
                T_hori_F_verti = thfv;
                Is_HV = true;
                type = 3;
            }
            public PathDistance(GraphCurve c1, int ci1, GraphArc ar1, float pos, bool thfv)
            {
                C1 = c1;
                cindex1 = ci1;
                A1 = ar1;
                XY_Pos = pos;
                T_hori_F_verti = thfv;
                Is_HV = true;
                type = 4;
            }
            public PathDistance(GraphArc ar1, GraphArc ar2, float pos, bool thfv)
            {
                A1 = ar1;
                A2 = ar2;
                XY_Pos = pos;
                T_hori_F_verti = thfv;
                Is_HV = true;
                type = 5;
            }

            public PathDistance(GraphLine l1, GraphLine l2, PointF a1, PointF a2)
            {
                L1 = l1;
                L2 = l2;
                Is_HV = false;
                anchor1 = a1;
                anchor2 = a2;
                type = 0;
            }
            public PathDistance(GraphLine l1, GraphCurve c1, int ci1, PointF a1, PointF a2)
            {
                L1 = l1;
                C1 = c1;
                cindex1 = ci1;
                Is_HV = false;
                anchor1 = a1;
                anchor2 = a2;
                type = 1;
            }
            public PathDistance(GraphCurve c1, int ci1, GraphCurve c2, int ci2, PointF a1, PointF a2)
            {
                C1 = c1;
                cindex1 = ci1;
                C2 = c2;
                cindex2 = ci2;
                Is_HV = false;
                anchor1 = a1;
                anchor2 = a2;
                type = 2;
            }
            public PathDistance(GraphLine l1, GraphArc ar1, PointF a1, PointF a2)
            {
                L1 = l1;
                A1 = ar1;
                Is_HV = false;
                anchor1 = a1;
                anchor2 = a2;
                type = 3;
            }
            public PathDistance(GraphCurve c1, int ci1, GraphArc ar1, PointF a1, PointF a2)
            {
                C1 = c1;
                cindex1 = ci1;
                A1 = ar1;
                Is_HV = false;
                anchor1 = a1;
                anchor2 = a2;
                type = 4;
            }
            public PathDistance(GraphArc ar1, GraphArc ar2, PointF a1, PointF a2)
            {
                A1 = ar1;
                A2 = ar2;
                Is_HV = false;
                anchor1 = a1;
                anchor2 = a2;
                type = 5;
            }

            public bool in_List(List<GraphLine> L, List<GraphCurve> C, List<GraphArc> A, List<GraphLine> SL)//0=>普通線條 1=>輔助線
            {
                PointF pt1, pt2;
                float dist;
                bool inlist = false;
                Get_Dist_Point(out dist, out pt1, out pt2);
                if (type == 0 && !(is_SupL1 || is_SupL2))
                    inlist = L.Exists(x => x == L1) && L.Exists(x => x == L2);
                else if (type == 1 && !is_SupL1)
                    inlist = L.Exists(x => x == L1) && C.Exists(x => x == C1);
                else if (type == 2)
                    inlist = C.Exists(x => x == C1) && C.Exists(x => x == C2);
                else if (type == 3 && !is_SupL1)
                    inlist = A.Exists(x => x == A1) && L.Exists(x => x == L1);
                else if (type == 4)
                    inlist = A.Exists(x => x == A1) && C.Exists(x => x == C1);
                else if (type == 5)
                    inlist = A.Exists(x => x == A1) && A.Exists(x => x == A2);
                else if (type == 0 && is_SupL1 && is_SupL2)
                    inlist = SL.Exists(x => x == L1) && SL.Exists(x => x == L2);
                else if (type == 0 && is_SupL1)
                    inlist = SL.Exists(x => x == L1) && L.Exists(x => x == L2);
                else if (type == 0 && is_SupL2)
                    inlist = L.Exists(x => x == L1) && SL.Exists(x => x == L2);
                else if (type == 1 && is_SupL1)
                    inlist = SL.Exists(x => x == L1) && C.Exists(x => x == C1);
                else if (type == 3 && is_SupL1)
                    inlist = A.Exists(x => x == A1) && SL.Exists(x => x == L1);
                else
                    inlist = false;
                if ((pt1.X == -1 && pt1.Y == -1) || (pt2.X == -1 && pt2.Y == -1))
                    inlist = false;
                return inlist;
            }
            public void Get_Dist_Point(out float dist, out PointF p1, out PointF p2)
            {
                GraphLine AnchorL;
                if (Is_HV)
                {
                    if (T_hori_F_verti)
                        AnchorL = new GraphLine(new GraphPoint(-10000, XY_Pos), new GraphPoint(10000, XY_Pos));
                    else
                        AnchorL = new GraphLine(new GraphPoint(XY_Pos, -10000), new GraphPoint(XY_Pos, 10000));
                }
                else
                {
                    AnchorL = new GraphLine(new GraphPoint(anchor1.X, anchor1.Y), new GraphPoint(anchor2.X, anchor2.Y));
                }
                if (type == 0)
                {
                    p1 = computeIntersections_LL(L1, AnchorL);
                    p2 = computeIntersections_LL(L2, AnchorL);
                    dist = (float)dist_PP(p1, p2);
                }
                else if (type == 1)
                {
                    p1 = computeIntersections_LL(L1, AnchorL);
                    PointF cc1 = new PointF(C1.path[cindex1 - 1].P.X + C1.disSecond[cindex1 - 1].X, C1.path[cindex1 - 1].P.Y + C1.disSecond[cindex1 - 1].Y);
                    PointF cc2 = new PointF(C1.path[cindex1].P.X + C1.disFirst[cindex1].X, C1.path[cindex1].P.Y + C1.disFirst[cindex1].Y);
                    PointF[] bez = { C1.path[cindex1 - 1].P, cc1, cc2, C1.path[cindex1].P };
                    PointF[] line = { AnchorL.StartPoint.P, AnchorL.EndPoint.P };
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
                else if (type == 2)
                {
                    PointF[] line = { AnchorL.StartPoint.P, AnchorL.EndPoint.P };

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
                else if (type == 3)
                {
                    p1 = computeIntersections_LL(L1, AnchorL);
                    PointF[] bez = A1.to_cubic_bezier();
                    PointF[] line = { AnchorL.StartPoint.P, AnchorL.EndPoint.P };
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
                else if (type == 4)
                {
                    PointF[] line = { AnchorL.StartPoint.P, AnchorL.EndPoint.P };

                    PointF cc11 = new PointF(C1.path[cindex1 - 1].P.X + C1.disSecond[cindex1 - 1].X, C1.path[cindex1 - 1].P.Y + C1.disSecond[cindex1 - 1].Y);
                    PointF cc12 = new PointF(C1.path[cindex1].P.X + C1.disFirst[cindex1].X, C1.path[cindex1].P.Y + C1.disFirst[cindex1].Y);
                    PointF[] bez1 = { C1.path[cindex1 - 1].P, cc11, cc12, C1.path[cindex1].P };
                    List<PointF> pl1 = computeIntersections_LC(bez1, line);

                    PointF[] bez2 = A1.to_cubic_bezier();
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
                else
                {
                    PointF[] line = { AnchorL.StartPoint.P, AnchorL.EndPoint.P };

                    PointF[] bez1 = A1.to_cubic_bezier();
                    List<PointF> pl1 = computeIntersections_LC(bez1, line);

                    PointF[] bez2 = A2.to_cubic_bezier();
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
            private PointF computeIntersections_LL(GraphLine RealL, GraphLine AnchorL)
            {
                float A1 = RealL.EndPoint.P.Y - RealL.StartPoint.P.Y,
                                  B1 = RealL.StartPoint.P.X - RealL.EndPoint.P.X,
                                  C1 = RealL.EndPoint.P.X * RealL.StartPoint.P.Y - RealL.StartPoint.P.X * RealL.EndPoint.P.Y;
                float A2 = AnchorL.EndPoint.P.Y - AnchorL.StartPoint.P.Y,
                      B2 = AnchorL.StartPoint.P.X - AnchorL.EndPoint.P.X,
                      C2 = AnchorL.EndPoint.P.X * AnchorL.StartPoint.P.Y - AnchorL.StartPoint.P.X * AnchorL.EndPoint.P.Y;
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
            public PointF computeIntersections_LL(GraphLine RealL, GraphLine AnchorL, out bool Outline)
            {
                float A1 = RealL.EndPoint.P.Y - RealL.StartPoint.P.Y,
                                  B1 = RealL.StartPoint.P.X - RealL.EndPoint.P.X,
                                  C1 = RealL.EndPoint.P.X * RealL.StartPoint.P.Y - RealL.StartPoint.P.X * RealL.EndPoint.P.Y;
                float A2 = AnchorL.EndPoint.P.Y - AnchorL.StartPoint.P.Y,
                      B2 = AnchorL.StartPoint.P.X - AnchorL.EndPoint.P.X,
                      C2 = AnchorL.EndPoint.P.X * AnchorL.StartPoint.P.Y - AnchorL.StartPoint.P.X * AnchorL.EndPoint.P.Y;
                float m = A1 * B2 - A2 * B1;
                float x = (C2 * B1 - C1 * B2) / m;
                float y = (C1 * A2 - C2 * A1) / m;

                PointF ans = new PointF(x, y);
                double t1 = dist_PP(RealL.StartPoint.P, ans) + dist_PP(RealL.EndPoint.P, ans);
                
                if(dist_PP(RealL.StartPoint.P, RealL.EndPoint.P) - t1 > -0.05)
                {
                    Outline = false;
                    return new PointF(x, y);
                }
                else
                {
                    Outline = true;
                    return new PointF(x, y);
                }
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
                    if (t < 0 || t > 1.0/* || s < 0 || s > 1.0*/)
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
            public List<PointF> computeIntersections_LC(PointF[] bez, PointF[] line, out bool[] Outline)
            {
                Outline = new bool[3];
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
                    if (t < 0 || t > 1.0/* || s < 0 || s > 1.0*/)
                    {
                        ans.Add(new PointF((float)X[0], (float)X[1]));
                        Outline[i] = true;
                    }
                    else
                    {
                        ans.Add(new PointF((float)X[0], (float)X[1]));
                        Outline[i] = false;
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
                if (Z[0] == 0) Z[0] = 0.00001;
                Z[1] = 3 * P0 - 6 * P1 + 3 * P2;
                Z[2] = -3 * P0 + 3 * P1;
                Z[3] = P0;
                return Z;
            }
        }
        public class GrainMark
        {
            public bool visible;
            public int loc_type, dir_type;
            public PointF Loc;
            public double Dir;
            public double size;
            public GrainMark()
            {
                visible = false;
                loc_type = 0;
                dir_type = 0;
                Loc = new PointF(-1, -1);
                Dir = -1;
                size = 100;
            }
        }
        public class FormulaToLine
        {
            public Formula fml;
            public string name = "";
            public List<Element> formula_eleL;
            public GraphPoint unfixed_P1 = null;
            public GraphPoint unfixed_P2 = null;
            public GraphLine L1 = null;
            public GraphLine L2 = null;
            public GraphCurve C1 = null;
            public int cindex1;
            public GraphCurve C2 = null;
            public int cindex2;
            public PointF anchor1, anchor2;
            public int type;  //0=ll lc cc pl pc pp
            public int mode; //0=平 垂
            public int path_type;
            public string prop12 = "1:1"; // 當兩邊可動時移動比例
            public FormulaToLine(GraphLine l1, GraphLine l2, PointF a1, PointF a2, int m, int pt)
            {
                L1 = l1;
                labled1 = new GraphGroup();
                labled1.C.Add(null);
                labled1.L.Add(L1);
                zoom_lable1 = new GraphGroup();
                zoom_lable1.C.Add(null);
                zoom_lable1.L.Add(L1);
                L2 = l2;
                labled2 = new GraphGroup();
                labled2.C.Add(null);
                labled2.L.Add(L2);
                zoom_lable2 = new GraphGroup();
                zoom_lable2.C.Add(null);
                zoom_lable2.L.Add(L1);
                anchor1 = a1;
                anchor2 = a2;
                type = 0;
                mode = m;
                path_type = pt;
                refresh_lableP();
            }
            public FormulaToLine(GraphLine l1, GraphCurve c1, int ci1, PointF a1, PointF a2, int m, int pt)
            {
                L1 = l1;
                labled1 = new GraphGroup();
                labled1.C.Add(null);
                labled1.L.Add(L1);
                zoom_lable1 = new GraphGroup();
                zoom_lable1.C.Add(null);
                zoom_lable1.L.Add(L1);
                C1 = c1;
                cindex1 = ci1;
                labled2 = new GraphGroup();
                labled2.C.Add(C1);
                labled2.L.Add(null);
                zoom_lable2 = new GraphGroup();
                zoom_lable2.C.Add(C1);
                zoom_lable2.L.Add(null);
                anchor1 = a1;
                anchor2 = a2;
                type = 1;
                mode = m;
                path_type = pt;
                refresh_lableP();
            }
            public FormulaToLine(GraphCurve c1, int ci1, GraphCurve c2, int ci2, PointF a1, PointF a2, int m, int pt)
            {
                C1 = c1;
                cindex1 = ci1;
                labled1 = new GraphGroup();
                labled1.C.Add(C1);
                labled1.L.Add(null);
                zoom_lable1 = new GraphGroup();
                zoom_lable1.C.Add(C1);
                zoom_lable1.L.Add(null);
                C2 = c2;
                cindex2 = ci2;
                labled2 = new GraphGroup();
                labled2.C.Add(C2);
                labled2.L.Add(null);
                zoom_lable2 = new GraphGroup();
                zoom_lable2.C.Add(C2);
                zoom_lable2.L.Add(null);
                anchor1 = a1;
                anchor2 = a2;
                type = 2;
                mode = m;
                path_type = pt;
                refresh_lableP();
            }
            public FormulaToLine(GraphPoint p, GraphLine l1, PointF a1, PointF a2, int m)
            {
                unfixed_P1 = p;
                L1 = l1;
                anchor1 = a1;
                anchor2 = a2;
                type = 3;
                mode = m;
            }
            public FormulaToLine(GraphPoint p, GraphCurve c1, int ci1, PointF a1, PointF a2, int m)
            {
                unfixed_P1 = p;
                C1 = c1;
                cindex1 = ci1;
                anchor1 = a1;
                anchor2 = a2;
                type = 4;
                mode = m;
            }
            public FormulaToLine(GraphPoint p1, GraphPoint p2, PointF a1, PointF a2, int m)
            {
                unfixed_P1 = p1;
                unfixed_P2 = p2;
                anchor1 = a1;
                anchor2 = a2;
                type = 5;
                mode = m;
            }
            public string check_name(List<FormulaToLine> ftll)
            {
                string name = "";
                if (fml == null)
                {
                    for (int i = 1; i < 10000; i++)
                    {
                        if (!ftll.Exists(x => x.name == "對應" + i))
                        {
                            name = "對應" + i;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 1; i < 10000; i++)
                    {
                        if (!ftll.Exists(x => x.name == fml.name + i))
                        {
                            name = fml.name + i;
                            break;
                        }
                    }
                }
                return name;
            }

            public GraphGroup labled1 = null;
            public GraphGroup labled2 = null;
            public GraphGroup zoom_lable1 = null;
            public GraphGroup zoom_lable2 = null;
            public void addlable(GraphLine l, GraphGroup path, int num, bool is_zoom)
            {
                if (num == 1)
                {
                    GraphGroup lb;
                    if (is_zoom)
                    {
                        lb = new GraphGroup();
                        for(int i = 0; i < labled1.L.Count; i++)
                        {
                            lb.L.Add(labled1.L[i]);
                            lb.C.Add(labled1.C[i]);
                        }
                    }
                    else
                        lb = labled1;
                    int index = 0;
                    int addindex = 0;
                    bool t_incre_f_decre = false;
                    if (type == 0 || type == 1)
                    {
                        index = path.L.FindLastIndex(x => x == L1);
                        addindex = path.L.FindLastIndex(x => x == l);
                        if (lb == null)
                        {
                            lb = new GraphGroup();
                            lb.L.Add(L1);
                            lb.C.Add(null);
                        }
                        int dist = addindex - index;
                        dist += (dist < 0) ? path.L.Count : 0;
                        t_incre_f_decre = dist < (path.L.Count / 2F);
                        for (int i = lb.L.FindIndex(x => x == L1);
                            i + (t_incre_f_decre ? 1 : -1) >= 0 && i + (t_incre_f_decre ? 1 : -1) < lb.L.Count;
                            i = lb.L.FindIndex(x => x == L1))
                        {
                            lb.L.RemoveAt(i + (t_incre_f_decre ? 1 : -1));
                            lb.C.RemoveAt(i + (t_incre_f_decre ? 1 : -1));
                        }
                    }
                    if (type == 2)
                    {
                        index = path.C.FindLastIndex(x => x == C1);
                        addindex = path.L.FindLastIndex(x => x == l);
                        if (lb == null)
                        {
                            lb = new GraphGroup();
                            lb.C.Add(C1);
                            lb.L.Add(null);
                        }
                        int dist = addindex - index;
                        dist += (dist < 0) ? path.L.Count : 0;
                        t_incre_f_decre = dist < (path.L.Count / 2F);
                        for (int i = lb.C.FindIndex(x => x == C1);
                            i + (t_incre_f_decre ? 1 : -1) >= 0 && i + (t_incre_f_decre ? 1 : -1) < lb.L.Count;
                            i = lb.C.FindIndex(x => x == C1))
                        {
                            lb.L.RemoveAt(i + (t_incre_f_decre ? 1 : -1));
                            lb.C.RemoveAt(i + (t_incre_f_decre ? 1 : -1));
                        }
                    }
                    if (index != addindex)
                        for (int i = index + (t_incre_f_decre ? 1 : -1); ; i = i + (t_incre_f_decre ? 1 : -1))
                        {
                            i += (i < 0) ? path.L.Count : 0;
                            i %= path.L.Count;
                            if (t_incre_f_decre)
                            {
                                lb.L.Add(path.L[i]);
                                lb.C.Add(path.C[i]);
                            }
                            else
                            {
                                lb.L.Insert(0, path.L[i]);
                                lb.C.Insert(0, path.C[i]);
                            }
                            if (i == addindex)
                                break;
                        }
                    else
                    {
                        lb = new GraphGroup();
                        if (type == 0 || type == 1)
                        {
                            lb.C.Add(null);
                            lb.L.Add(L1);
                        }
                        else if (type == 2)
                        {
                            lb.C.Add(C1);
                            lb.L.Add(null);
                        }
                    }
                    /*
                    if (is_zoom && labled1 != null)
                    {
                        for(int i = 0; i < labled1.L.Count; i++)
                        {
                            if (labled1.L[i] != null)
                            {
                                int ind = lb.L.FindIndex(x => x == labled1.L[i]);
                                if (ind >= 0)
                                {
                                    lb.L.RemoveAt(ind);
                                    lb.C.RemoveAt(ind);
                                }
                            }
                            else
                            {
                                int ind = lb.C.FindIndex(x => x == labled1.C[i]);
                                if (ind >= 0)
                                {
                                    lb.L.RemoveAt(ind);
                                    lb.C.RemoveAt(ind);
                                }
                            }
                        }
                    }
                    */
                    int ind = type == 0 || type == 1 ? lb.L.FindIndex(x => x == L1) : lb.C.FindIndex(x => x == C1);
                    int count = type == 0 || type == 1 ? lb.L.FindAll(x => x == L1).Count : lb.C.FindAll(x => x == C1).Count;
                    if (ind >= 0 && count > 1)
                    {
                        lb.L.RemoveAt(ind);
                        lb.C.RemoveAt(ind);
                    }
                    if (is_zoom)
                        zoom_lable1 = lb;
                    else
                        labled1 = lb;
                    refresh_lableP();
                }
                else if(num == 2)
                {
                    GraphGroup lb;
                    if (is_zoom)
                    {
                        lb = new GraphGroup();
                        for (int i = 0; i < labled2.L.Count; i++)
                        {
                            lb.L.Add(labled2.L[i]);
                            lb.C.Add(labled2.C[i]);
                        }
                    }
                    else
                        lb = labled2;
                    int index = 0;
                    int addindex = 0;
                    bool t_incre_f_decre = false;
                    if (type == 0) //L2
                    {
                        index = path.L.FindLastIndex(x => x == L2);
                        addindex = path.L.FindLastIndex(x => x == l);
                        if (lb == null)
                        {
                            lb = new GraphGroup();
                            lb.L.Add(L2);
                            lb.C.Add(null);
                        }
                        int dist = addindex - index;
                        dist += (dist < 0) ? path.L.Count : 0;
                        t_incre_f_decre = dist < (path.L.Count / 2F);
                        for (int i = lb.L.FindIndex(x => x == L2);
                            i + (t_incre_f_decre ? 1 : -1) >= 0 && i + (t_incre_f_decre ? 1 : -1) < lb.L.Count;
                            i = lb.L.FindIndex(x => x == L2))
                        {
                            lb.L.RemoveAt(i + (t_incre_f_decre ? 1 : -1));
                            lb.C.RemoveAt(i + (t_incre_f_decre ? 1 : -1));
                        }
                    }
                    else if (type == 1) //C1
                    {
                        index = path.C.FindLastIndex(x => x == C1);
                        addindex = path.L.FindLastIndex(x => x == l);
                        if (lb == null)
                        {
                            lb = new GraphGroup();
                            lb.C.Add(C1);
                            lb.L.Add(null);
                        }
                        int dist = addindex - index;
                        dist += (dist < 0) ? path.L.Count : 0;
                        t_incre_f_decre = dist < (path.L.Count / 2F);
                        for (int i = lb.C.FindIndex(x => x == C1);
                            i + (t_incre_f_decre ? 1 : -1) >= 0 && i + (t_incre_f_decre ? 1 : -1) < lb.L.Count;
                            i = lb.C.FindIndex(x => x == C1))
                        {
                            lb.L.RemoveAt(i + (t_incre_f_decre ? 1 : -1));
                            lb.C.RemoveAt(i + (t_incre_f_decre ? 1 : -1));
                        }
                    }
                    else if (type == 2) //C2
                    {
                        index = path.C.FindLastIndex(x => x == C2);
                        addindex = path.L.FindLastIndex(x => x == l);
                        if (lb == null)
                        {
                            lb = new GraphGroup();
                            lb.C.Add(C2);
                            lb.L.Add(null);
                        }
                        int dist = addindex - index;
                        dist += (dist < 0) ? path.L.Count : 0;
                        t_incre_f_decre = dist < (path.L.Count / 2F);
                        for (int i = lb.C.FindIndex(x => x == C2);
                            i + (t_incre_f_decre ? 1 : -1) >= 0 && i + (t_incre_f_decre ? 1 : -1) < lb.L.Count;
                            i = lb.C.FindIndex(x => x == C2))
                        {
                            lb.L.RemoveAt(i + (t_incre_f_decre ? 1 : -1));
                            lb.C.RemoveAt(i + (t_incre_f_decre ? 1 : -1));
                        }
                    }
                    if (index != addindex)
                        for (int i = index + (t_incre_f_decre ? 1 : -1); ; i = i + (t_incre_f_decre ? 1 : -1))
                        {
                            i += (i < 0) ? path.L.Count : 0;
                            i %= path.L.Count;
                            if (t_incre_f_decre)
                            {
                                lb.L.Add(path.L[i]);
                                lb.C.Add(path.C[i]);
                            }
                            else
                            {
                                lb.L.Insert(0, path.L[i]);
                                lb.C.Insert(0, path.C[i]);
                            }
                            if (i == addindex)
                                break;
                        }
                    else
                    {
                        lb = new GraphGroup();
                        if(type == 0)
                        {
                            lb.C.Add(null);
                            lb.L.Add(L2);
                        }
                        else if (type == 1)
                        {
                            lb.C.Add(C1);
                            lb.L.Add(null);
                        }
                        else if(type == 2)
                        {
                            lb.C.Add(C2);
                            lb.L.Add(null);
                        }
                    }
                    /*
                    if (is_zoom && labled2 != null)
                    {
                        for (int i = 0; i < labled2.L.Count; i++)
                        {
                            if(labled2.L[i] != null)
                            {
                                int ind = lb.L.FindIndex(x => x == labled2.L[i]);
                                if(ind >= 0)
                                {
                                    lb.L.RemoveAt(ind);
                                    lb.C.RemoveAt(ind);
                                }
                            }
                            else
                            {
                                int ind = lb.C.FindIndex(x => x == labled2.C[i]);
                                if (ind >= 0)
                                {
                                    lb.L.RemoveAt(ind);
                                    lb.C.RemoveAt(ind);
                                }
                            }
                        }
                    }
                    */
                    int ind = type == 0 ? lb.L.FindIndex(x => x == L2) :
                              type == 1 ? lb.C.FindIndex(x => x == C1) : lb.C.FindIndex(x => x == C2);
                    int count = type == 0 ? lb.L.FindAll(x => x == L2).Count :
                                type == 1 ? lb.C.FindAll(x => x == C1).Count : lb.C.FindAll(x => x == C2).Count;
                    if (ind >= 0 && count > 1)
                    {
                        lb.L.RemoveAt(ind);
                        lb.C.RemoveAt(ind);
                    }
                    if (is_zoom)
                        zoom_lable2 = lb;
                    else
                        labled2 = lb;
                    refresh_lableP();
                }
            }
            public void addlable(GraphCurve c, GraphGroup path, int num, bool is_zoom)
            {
                if (num == 1)
                {
                    GraphGroup lb;
                    if (is_zoom)
                    {
                        lb = new GraphGroup();
                        for (int i = 0; i < labled1.L.Count; i++)
                        {
                            lb.L.Add(labled1.L[i]);
                            lb.C.Add(labled1.C[i]);
                        }
                    }
                    else
                        lb = labled1;
                    int index = 0;
                    int addindex = 0;
                    bool t_incre_f_decre = false;
                    if (type == 2) // C1 lable1
                    {
                        index = path.C.FindLastIndex(x => x == C1);
                        addindex = path.C.FindLastIndex(x => x == c);
                        if (lb == null)
                        {
                            lb = new GraphGroup();
                            lb.C.Add(C1);
                            lb.L.Add(null);
                        }
                        int dist = addindex - index;
                        dist += (dist < 0) ? path.L.Count : 0;
                        t_incre_f_decre = dist < (path.L.Count / 2F);
                        for (int i = lb.C.FindIndex(x => x == C1);
                            i + (t_incre_f_decre ? 1 : -1) >= 0 && i + (t_incre_f_decre ? 1 : -1) < lb.L.Count;
                            i = lb.C.FindIndex(x => x == C1))
                        {
                            lb.L.RemoveAt(i + (t_incre_f_decre ? 1 : -1));
                            lb.C.RemoveAt(i + (t_incre_f_decre ? 1 : -1));
                        }
                    }
                    else
                    {
                        index = path.L.FindLastIndex(x => x == L1);
                        addindex = path.C.FindLastIndex(x => x == c);
                        if (lb == null)
                        {
                            lb = new GraphGroup();
                            lb.L.Add(L1);
                            lb.C.Add(null);
                        }
                        int dist = addindex - index;
                        dist += (dist < 0) ? path.L.Count : 0;
                        t_incre_f_decre = dist < (path.L.Count / 2F);
                        for (int i = lb.L.FindIndex(x => x == L1);
                            i + (t_incre_f_decre ? 1 : -1) >= 0 && i + (t_incre_f_decre ? 1 : -1) < lb.L.Count;
                            i = lb.L.FindIndex(x => x == L1))
                        {
                            lb.L.RemoveAt(i + (t_incre_f_decre ? 1 : -1));
                            lb.C.RemoveAt(i + (t_incre_f_decre ? 1 : -1));
                        }
                    }
                    for (int i = index; ; i = i + (t_incre_f_decre ? 1 : -1))
                    {
                        i += (i < 0) ? path.L.Count : 0;
                        i %= path.L.Count;
                        if (t_incre_f_decre)
                        {
                            lb.L.Add(path.L[i]);
                            lb.C.Add(path.C[i]);
                        }
                        else
                        {
                            lb.L.Insert(0, path.L[i]);
                            lb.C.Insert(0, path.C[i]);
                        }
                        if (i == addindex)
                            break;
                    }
                    /*if (is_zoom && labled1 != null)
                    {
                        for (int i = 0; i < labled1.L.Count; i++)
                        {
                            if (labled1.L[i] != null)
                            {
                                int ind = lb.L.FindIndex(x => x == labled1.L[i]);
                                if (ind >= 0)
                                {
                                    lb.L.RemoveAt(ind);
                                    lb.C.RemoveAt(ind);
                                }
                            }
                            else
                            {
                                int ind = lb.C.FindIndex(x => x == labled1.C[i]);
                                if (ind >= 0)
                                {
                                    lb.L.RemoveAt(ind);
                                    lb.C.RemoveAt(ind);
                                }
                            }
                        }
                    }*/
                    int ind = type == 0 || type == 1 ? lb.L.FindIndex(x => x == L1) : lb.C.FindIndex(x => x == C1);
                    int count = type == 0 || type == 1 ? lb.L.FindAll(x => x == L1).Count : lb.C.FindAll(x => x == C1).Count;
                    if (ind >= 0 && count > 1)
                    {
                        lb.L.RemoveAt(ind);
                        lb.C.RemoveAt(ind);
                    }
                    if (is_zoom)
                        zoom_lable1 = lb;
                    else
                        labled1 = lb;
                    refresh_lableP();
                }
                else if(num == 2)
                {
                    GraphGroup lb;
                    if (is_zoom)
                    {
                        lb = new GraphGroup();
                        for (int i = 0; i < labled2.L.Count; i++)
                        {
                            lb.L.Add(labled2.L[i]);
                            lb.C.Add(labled2.C[i]);
                        }
                    }
                    else
                        lb = labled2;
                    int index = 0;
                    int addindex = 0;
                    bool t_incre_f_decre = false;
                    if(type == 0)
                    {
                        index = path.L.FindLastIndex(x => x == L2);
                        addindex = path.C.FindLastIndex(x => x == c);
                        if (lb == null)
                        {
                            lb = new GraphGroup();
                            lb.L.Add(L2);
                            lb.C.Add(null);
                        }
                        int dist = addindex - index;
                        dist += (dist < 0) ? path.L.Count : 0;
                        t_incre_f_decre = dist < (path.L.Count / 2F);
                        for (int i = lb.L.FindIndex(x => x == L2);
                            i + (t_incre_f_decre ? 1 : -1) >= 0 && i + (t_incre_f_decre ? 1 : -1) < lb.L.Count;
                            i = lb.L.FindIndex(x => x == L2))
                        {
                            lb.L.RemoveAt(i + (t_incre_f_decre ? 1 : -1));
                            lb.C.RemoveAt(i + (t_incre_f_decre ? 1 : -1));
                        }
                    }
                    else if (type == 1)
                    {
                        index = path.C.FindLastIndex(x => x == C1);
                        addindex = path.C.FindLastIndex(x => x == c);
                        if (lb == null)
                        {
                            lb = new GraphGroup();
                            lb.C.Add(C1);
                            lb.L.Add(null);
                        }
                        int dist = addindex - index;
                        dist += (dist < 0) ? path.L.Count : 0;
                        t_incre_f_decre = dist < (path.L.Count / 2F);
                        for (int i = lb.C.FindIndex(x => x == C1);
                            i + (t_incre_f_decre ? 1 : -1) >= 0 && i + (t_incre_f_decre ? 1 : -1) < lb.L.Count;
                            i = lb.C.FindIndex(x => x == C1))
                        {
                            lb.L.RemoveAt(i + (t_incre_f_decre ? 1 : -1));
                            lb.C.RemoveAt(i + (t_incre_f_decre ? 1 : -1));
                        }
                    }
                    else if(type == 2)
                    {
                        index = path.C.FindLastIndex(x => x == C2);
                        addindex = path.C.FindLastIndex(x => x == c);
                        if (lb == null)
                        {
                            lb = new GraphGroup();
                            lb.C.Add(C2);
                            lb.L.Add(null);
                        }
                        int dist = addindex - index;
                        dist += (dist < 0) ? path.L.Count : 0;
                        t_incre_f_decre = dist < (path.L.Count / 2F);
                        for (int i = lb.C.FindIndex(x => x == C2);
                            i + (t_incre_f_decre ? 1 : -1) >= 0 && i + (t_incre_f_decre ? 1 : -1) < lb.L.Count;
                            i = lb.C.FindIndex(x => x == C2))
                        {
                            lb.L.RemoveAt(i + (t_incre_f_decre ? 1 : -1));
                            lb.C.RemoveAt(i + (t_incre_f_decre ? 1 : -1));
                        }
                    }
                    for (int i = index; ; i = i + (t_incre_f_decre ? 1 : -1))
                    {
                        i += (i < 0) ? path.L.Count : 0;
                        i %= path.L.Count;
                        if (t_incre_f_decre)
                        {
                            lb.L.Add(path.L[i]);
                            lb.C.Add(path.C[i]);
                        }
                        else
                        {
                            lb.L.Insert(0, path.L[i]);
                            lb.C.Insert(0, path.C[i]);
                        }
                        if (i == addindex)
                            break;
                    }
                    /*
                    if (is_zoom && labled2 != null)
                    {
                        for (int i = 0; i < labled2.L.Count; i++)
                        {
                            if (labled2.L[i] != null)
                            {
                                int ind = lb.L.FindIndex(x => x == labled2.L[i]);
                                if (ind >= 0)
                                {
                                    lb.L.RemoveAt(ind);
                                    lb.C.RemoveAt(ind);
                                }
                            }
                            else
                            {
                                int ind = lb.C.FindIndex(x => x == labled2.C[i]);
                                if (ind >= 0)
                                {
                                    lb.L.RemoveAt(ind);
                                    lb.C.RemoveAt(ind);
                                }
                            }
                        }
                    }
                    */
                    int ind = type == 0 ? lb.L.FindIndex(x => x == L2) :
                              type == 1 ? lb.C.FindIndex(x => x == C1) : lb.C.FindIndex(x => x == C2);
                    int count = type == 0 ? lb.L.FindAll(x => x == L2).Count :
                                type == 1 ? lb.C.FindAll(x => x == C1).Count : lb.C.FindAll(x => x == C2).Count;
                    if (ind >= 0 && count > 1)
                    {
                        lb.L.RemoveAt(ind);
                        lb.C.RemoveAt(ind);
                    }
                    if (is_zoom)
                        zoom_lable2 = lb;
                    else
                        labled2 = lb;
                    refresh_lableP();
                }
            }
            public void get_lable_sten(int lablenum, out bool st_tsfe,  out bool en_tsfe)
            {
                GraphGroup lab = (lablenum == 1) ? labled1 : labled2;
                if (lab.L[0] != null)
                {
                    if (lab.L[1] != null)
                    {
                        st_tsfe = (lab.L[0].EndPoint == lab.L[1].StartPoint || lab.L[0].EndPoint == lab.L[1].EndPoint);
                    }
                    else
                    {
                        st_tsfe = (lab.L[0].EndPoint == lab.C[1].path[0] || lab.L[0].EndPoint == lab.C[1].path.Last());
                    }
                }
                else
                {
                    if (lab.L[1] != null)
                    {
                        st_tsfe = (lab.C[0].path.Last() == lab.L[1].StartPoint || lab.C[0].path.Last() == lab.L[1].EndPoint);
                    }
                    else
                    {
                        st_tsfe = (lab.C[0].path.Last() == lab.C[1].path[0] || lab.C[0].path.Last() == lab.C[1].path.Last());
                    }
                }

                if (lab.L.Last() != null)
                {
                    if (lab.L[lab.L.Count - 2] != null)
                    {
                        en_tsfe = (lab.L.Last().EndPoint == lab.L[lab.L.Count - 2].StartPoint || lab.L.Last().EndPoint == lab.L[lab.L.Count - 2].EndPoint);
                    }
                    else
                    {
                        en_tsfe = (lab.L.Last().EndPoint == lab.C[lab.L.Count - 2].path[0] || lab.L.Last().EndPoint == lab.C[lab.L.Count - 2].path.Last());
                    }
                }
                else
                {
                    if (lab.L[lab.L.Count - 2] != null)
                    {
                        en_tsfe = (lab.C.Last().path.Last() == lab.L[lab.L.Count - 2].StartPoint || lab.C.Last().path.Last() == lab.L[lab.L.Count - 2].EndPoint);
                    }
                    else
                    {
                        en_tsfe = (lab.C.Last().path.Last() == lab.C[lab.L.Count - 2].path[0] || lab.C.Last().path.Last() == lab.C[lab.L.Count - 2].path.Last());
                    }
                }
            }
            public void refresh_lableP()
            {
                List<GraphPoint> pl = new List<GraphPoint>();
                GraphGroup g;

                if (labled1 != null)
                {
                    g = labled1;
                    for (int i = 0; i < g.L.Count; i++)
                    {
                        if (g.L[i] != null)
                        {
                            if (!pl.Exists(x => x == g.L[i].StartPoint))
                                pl.Add(g.L[i].StartPoint);
                            if (!pl.Exists(x => x == g.L[i].EndPoint))
                                pl.Add(g.L[i].EndPoint);
                        }
                        else
                        {
                            foreach (var p in g.C[i].path)
                            {
                                if (!pl.Exists(x => x == p))
                                    pl.Add(p);
                            }
                        }
                    }
                    g.P = pl;
                }

                if (labled2 != null)
                {
                    g = labled2;
                    pl = new List<GraphPoint>();
                    for (int i = 0; i < g.L.Count; i++)
                    {
                        if (g.L[i] != null)
                        {
                            if (!pl.Exists(x => x == g.L[i].StartPoint))
                                pl.Add(g.L[i].StartPoint);
                            if (!pl.Exists(x => x == g.L[i].EndPoint))
                                pl.Add(g.L[i].EndPoint);
                        }
                        else
                        {
                            foreach (var p in g.C[i].path)
                            {
                                if (!pl.Exists(x => x == p))
                                    pl.Add(p);
                            }
                        }
                    }
                    g.P = pl;
                }

                if (zoom_lable1 != null)
                {
                    g = zoom_lable1;
                    pl = new List<GraphPoint>();
                    for (int i = 0; i < g.L.Count; i++)
                    {
                        if (g.L[i] != null)
                        {
                            if (!pl.Exists(x => x == g.L[i].StartPoint))
                                pl.Add(g.L[i].StartPoint);
                            if (!pl.Exists(x => x == g.L[i].EndPoint))
                                pl.Add(g.L[i].EndPoint);
                        }
                        else
                        {
                            foreach (var p in g.C[i].path)
                            {
                                if (!pl.Exists(x => x == p))
                                    pl.Add(p);
                            }
                        }
                    }
                    g.P = pl;
                }
                if (zoom_lable2 != null)
                {
                    g = zoom_lable2;
                    pl = new List<GraphPoint>();
                    for (int i = 0; i < g.L.Count; i++)
                    {
                        if (g.L[i] != null)
                        {
                            if (!pl.Exists(x => x == g.L[i].StartPoint))
                                pl.Add(g.L[i].StartPoint);
                            if (!pl.Exists(x => x == g.L[i].EndPoint))
                                pl.Add(g.L[i].EndPoint);
                        }
                        else
                        {
                            foreach (var p in g.C[i].path)
                            {
                                if (!pl.Exists(x => x == p))
                                    pl.Add(p);
                            }
                        }
                    }
                    g.P = pl;
                }
            }
            public bool get_tst_fen(int lablenum, bool t_forword_f_back, out bool tLfC, out int testi, GraphGroup path)
            {
                GraphGroup lb = lablenum == 1 ? labled1 : labled2;
                GraphLine tl = t_forword_f_back ? lb.L[0] : lb.L.Last();
                GraphCurve tc = t_forword_f_back ? lb.C[0] : lb.C.Last();
                int index = tl != null ? path.L.FindIndex(x => x == tl) : path.C.FindIndex(x => x == tc);
                testi = (index + (t_forword_f_back ? -1 : 1) + path.L.Count) % path.L.Count;
                if (lb.L.Count == 1 && path != null)
                {
                    GraphPoint stp = type == 0 ? (lablenum == 1 ? L1.StartPoint : L2.StartPoint) :
                                     type == 1 ? (lablenum == 1 ? L1.StartPoint : C1.path[0]) :
                                                 (lablenum == 1 ? C1.path[0] : C2.path[0]);
                    if (path.L[index] != null)
                    {
                        tLfC = true;
                        
                    }
                    else
                    {
                        tLfC = false;
                    }
                    if (path.L[testi] != null)
                    {
                        return (path.L[testi].StartPoint == stp || path.L[testi].EndPoint == stp);
                    }
                    else
                    {
                        return (path.C[testi].path[0] == stp || path.C[testi].path.Last() == stp);
                    }
                }
                else if (t_forword_f_back)
                {
                    GraphPoint stp = lb.L[0] != null ? lb.L[0].StartPoint : lb.C[0].path[0];
                    tLfC = lb.L[0] != null;
                    if (lb.L[1] != null)
                    {
                        return !(lb.L[1].StartPoint == stp || lb.L[1].EndPoint == stp);
                    }
                    else
                    {
                        return !(lb.C[1].path[0] == stp || lb.C[1].path.Last() == stp);
                    }
                }
                else
                {
                    GraphPoint stp = lb.L.Last() != null ? lb.L.Last().StartPoint : lb.C.Last().path[0];
                    tLfC = lb.L.Last() != null;
                    if (lb.L[lb.L.Count - 2] != null)
                    {
                        return !(lb.L[lb.L.Count - 2].StartPoint == stp || lb.L[lb.L.Count - 2].EndPoint == stp);
                    }
                    else
                    {
                        return !(lb.C[lb.L.Count - 2].path[0] == stp || lb.C[lb.L.Count - 2].path.Last() == stp);
                    }
                }
            }
            public bool get_fix(int n)
            {
                GraphGroup lb = n == 1 ? labled1 : labled2;
                if (lb != null)
                    if (lb.L.Count != 1)
                        return false;
                if (type == 0)
                    return (n == 1 ? L1.fix : L2.fix);
                else if (type == 1)
                    return (n == 1 ? L1.fix : C1.fix);
                else if (type == 2)
                    return (n == 1 ? C1.fix : C2.fix);
                else
                    return true;
            }
            public void get_zoomlable_FB(int num, out GraphGroup front, out GraphGroup back)
            {
                GraphGroup lb = (num == 1 ? labled1 : labled2);
                GraphGroup zlb = num == 1 ? zoom_lable1 : zoom_lable2;
                int findex, bindex;
                if (lb.L[0] != null)
                    findex = zlb.L.FindIndex(x => x == lb.L[0]);
                else
                    findex = zlb.C.FindIndex(x => x == lb.C[0]);
                if (lb.L.Last() != null)
                    bindex = zlb.L.FindIndex(x => x == lb.L.Last());
                else
                    bindex = zlb.C.FindIndex(x => x == lb.C.Last());
                front = new GraphGroup();
                if (findex != -1)
                    for (int i = 0; i < findex; i++)
                    {
                        front.L.Add(zlb.L[i]);
                        front.C.Add(zlb.C[i]);
                    }
                back = new GraphGroup();
                if (bindex != -1)
                    for (int i = bindex + 1; i < zlb.L.Count; i++)
                    {
                        back.L.Add(zlb.L[i]);
                        back.C.Add(zlb.C[i]);
                    }
                List<GraphPoint> pl = new List<GraphPoint>();
                GraphGroup g;

                if (front != null)
                {
                    g = front;
                    for (int i = 0; i < g.L.Count; i++)
                    {
                        if (g.L[i] != null)
                        {
                            if (!pl.Exists(x => x == g.L[i].StartPoint))
                                pl.Add(g.L[i].StartPoint);
                            if (!pl.Exists(x => x == g.L[i].EndPoint))
                                pl.Add(g.L[i].EndPoint);
                        }
                        else
                        {
                            foreach (var p in g.C[i].path)
                            {
                                if (!pl.Exists(x => x == p))
                                    pl.Add(p);
                            }
                        }
                    }
                    g.P = pl;
                }
                if (back != null)
                {
                    g = back;
                    pl = new List<GraphPoint>();
                    for (int i = 0; i < g.L.Count; i++)
                    {
                        if (g.L[i] != null)
                        {
                            if (!pl.Exists(x => x == g.L[i].StartPoint))
                                pl.Add(g.L[i].StartPoint);
                            if (!pl.Exists(x => x == g.L[i].EndPoint))
                                pl.Add(g.L[i].EndPoint);
                        }
                        else
                        {
                            foreach (var p in g.C[i].path)
                            {
                                if (!pl.Exists(x => x == p))
                                    pl.Add(p);
                            }
                        }
                    }
                    g.P = pl;
                }
            }

            public bool in_List(List<GraphPoint> P, List<GraphLine> L, List<GraphCurve> C)//0=>普通線條 1=>輔助線
            {
                bool inlist = false;
                refreshAnchor();
                if (type == 0)
                    inlist = L.Exists(x => x == L1) && L.Exists(x => x == L2);
                else if (type == 1)
                    inlist = L.Exists(x => x == L1) && C.Exists(x => x == C1);
                else if (type == 2)
                    inlist = C.Exists(x => x == C1) && C.Exists(x => x == C2);
                else if (type == 3)
                    inlist = L.Exists(x => x == L1) && P.Exists(x => x == unfixed_P1);
                else if (type == 4)
                    inlist = C.Exists(x => x == C1) && P.Exists(x => x == unfixed_P1);
                else if (type == 5)
                    inlist = P.Exists(x => x == unfixed_P1) && P.Exists(x => x == unfixed_P2);
                else
                    inlist = false;
                if ((anchor1.X == -1 && anchor1.Y == -1) || (anchor2.X == -1 && anchor2.Y == -1))
                    inlist = false;
                return inlist;
            }
            private void refreshAnchor()
            {
                GraphLine AnchorL = new GraphLine(new GraphPoint(anchor1.X, anchor1.Y), new GraphPoint(anchor2.X, anchor2.Y));
                if (type == 0)
                {
                    anchor1 = computeIntersections_LL(L1, AnchorL);
                    anchor2 = computeIntersections_LL(L2, AnchorL);
                }
                else if (type == 1)
                {
                    anchor1 = computeIntersections_LL(L1, AnchorL);
                    PointF cc1 = new PointF(C1.path[cindex1 - 1].P.X + C1.disSecond[cindex1 - 1].X, C1.path[cindex1 - 1].P.Y + C1.disSecond[cindex1 - 1].Y);
                    PointF cc2 = new PointF(C1.path[cindex1].P.X + C1.disFirst[cindex1].X, C1.path[cindex1].P.Y + C1.disFirst[cindex1].Y);
                    PointF[] bez = { C1.path[cindex1 - 1].P, cc1, cc2, C1.path[cindex1].P };
                    PointF[] line = { AnchorL.StartPoint.P, AnchorL.EndPoint.P };
                    List<PointF> pl1 = computeIntersections_LC(bez, line);
                    double mindist = 9999999999999999999;
                    anchor2 = new PointF(-1, -1);
                    for (int i = 0; i < pl1.Count; i++)
                    {
                        if (dist_PP(pl1[i], anchor1) < mindist)
                        {
                            mindist = dist_PP(pl1[i], anchor1);
                            anchor2 = pl1[i];
                        }
                    }
                }
                else if (type == 2)
                {
                    PointF[] line = { AnchorL.StartPoint.P, AnchorL.EndPoint.P };

                    PointF cc11 = new PointF(C1.path[cindex1 - 1].P.X + C1.disSecond[cindex1 - 1].X, C1.path[cindex1 - 1].P.Y + C1.disSecond[cindex1 - 1].Y);
                    PointF cc12 = new PointF(C1.path[cindex1].P.X + C1.disFirst[cindex1].X, C1.path[cindex1].P.Y + C1.disFirst[cindex1].Y);
                    PointF[] bez1 = { C1.path[cindex1 - 1].P, cc11, cc12, C1.path[cindex1].P };
                    List<PointF> pl1 = computeIntersections_LC(bez1, line);

                    PointF cc21 = new PointF(C2.path[cindex2 - 1].P.X + C2.disSecond[cindex2 - 1].X, C2.path[cindex2 - 1].P.Y + C2.disSecond[cindex2 - 1].Y);
                    PointF cc22 = new PointF(C2.path[cindex2].P.X + C2.disFirst[cindex2].X, C2.path[cindex2].P.Y + C2.disFirst[cindex2].Y);
                    PointF[] bez2 = { C2.path[cindex2 - 1].P, cc21, cc22, C2.path[cindex2].P };
                    List<PointF> pl2 = computeIntersections_LC(bez2, line);

                    double mindist = 9999999999999999999;
                    anchor1 = new PointF(-1, -1);
                    anchor2 = new PointF(-1, -1);
                    for (int i = 0; i < pl1.Count; i++)
                    {
                        for (int j = 0; j < pl2.Count; j++)
                        {
                            if (dist_PP(pl1[i], pl2[j]) < mindist)
                            {
                                mindist = dist_PP(pl1[i], pl2[j]);
                                anchor1 = pl1[i];
                                anchor2 = pl2[j];
                            }
                        }
                    }
                }
                else if(type == 3)
                {
                    anchor1 = unfixed_P1.P;
                    if (mode == 0)
                        anchor2.Y = anchor1.Y;
                    else if (mode == 1)
                        anchor2.X = anchor1.X;
                    AnchorL.EndPoint.P = anchor2;
                    anchor2 = computeIntersections_LL(L1, AnchorL);
                }
                else if(type == 4)
                {
                    anchor1 = unfixed_P1.P;
                    if (mode == 0)
                        anchor2.Y = anchor1.Y;
                    else if (mode == 1)
                        anchor2.X = anchor1.X;
                    AnchorL.EndPoint.P = anchor2;
                    PointF cc1 = new PointF(C1.path[cindex1 - 1].P.X + C1.disSecond[cindex1 - 1].X, C1.path[cindex1 - 1].P.Y + C1.disSecond[cindex1 - 1].Y);
                    PointF cc2 = new PointF(C1.path[cindex1].P.X + C1.disFirst[cindex1].X, C1.path[cindex1].P.Y + C1.disFirst[cindex1].Y);
                    PointF[] bez = { C1.path[cindex1 - 1].P, cc1, cc2, C1.path[cindex1].P };
                    PointF[] line = { AnchorL.StartPoint.P, AnchorL.EndPoint.P };
                    List<PointF> pl1 = computeIntersections_LC(bez, line);
                    double mindist = 9999999999999999999;
                    anchor2 = new PointF(-1, -1);
                    for (int i = 0; i < pl1.Count; i++)
                    {
                        if (dist_PP(pl1[i], anchor1) < mindist)
                        {
                            mindist = dist_PP(pl1[i], anchor1);
                            anchor2 = pl1[i];
                        }
                    }
                }
                else if(type == 5)
                {
                    anchor1 = unfixed_P1.P;
                    anchor2 = unfixed_P2.P;
                }
            }
            private double dist_PP(PointF p1, PointF p2)
            {
                float x = p2.X - p1.X;
                float y = p2.Y - p1.Y;
                return Math.Sqrt(x * x + y * y);
            }
            private PointF computeIntersections_LL(GraphLine RealL, GraphLine AnchorL)
            {
                float A1 = RealL.EndPoint.P.Y - RealL.StartPoint.P.Y,
                                  B1 = RealL.StartPoint.P.X - RealL.EndPoint.P.X,
                                  C1 = RealL.EndPoint.P.X * RealL.StartPoint.P.Y - RealL.StartPoint.P.X * RealL.EndPoint.P.Y;
                float A2 = AnchorL.EndPoint.P.Y - AnchorL.StartPoint.P.Y,
                      B2 = AnchorL.StartPoint.P.X - AnchorL.EndPoint.P.X,
                      C2 = AnchorL.EndPoint.P.X * AnchorL.StartPoint.P.Y - AnchorL.StartPoint.P.X * AnchorL.EndPoint.P.Y;
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
                    if (t < 0 || t > 1.0/* || s < 0 || s > 1.0*/)
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
                if (Z[0] == 0) Z[0] = 0.00001;
                Z[1] = 3 * P0 - 6 * P1 + 3 * P2;
                Z[2] = -3 * P0 + 3 * P1;
                Z[3] = P0;
                return Z;
            }
            /*
            public FormulaToLine(GraphLine l1, GraphArc ar1, PointF a1, PointF a2)
            {
                L1 = l1;
                A1 = ar1;
                anchor1 = a1;
                anchor2 = a2;
                type = 3;
            }
            public FormulaToLine(GraphCurve c1, GraphArc ar1, PointF a1, PointF a2)
            {
                C1 = c1;
                A1 = ar1;
                anchor1 = a1;
                anchor2 = a2;
                type = 4;
            }
            public FormulaToLine(GraphArc ar1, GraphArc ar2, PointF a1, PointF a2)
            {
                A1 = ar1;
                A2 = ar2;
                anchor1 = a1;
                anchor2 = a2;
                type = 5;
            }*/
            
        }
        #endregion

        #region find_function
        GraphPoint FindPointByPoint(List<GraphPoint> points, Point p)
        {
            var size = 5;
            var buffer = new Bitmap(size * 2, size * 2);
            foreach (var poi in points)
            {
                //draw each line on small region around current point p and check pixel in point p 

                using (var g = Graphics.FromImage(buffer))
                {
                    g.Clear(Color.Black);
                    g.DrawRectangle(new Pen(Color.Green, 5 / ZoomSize), poi.P.X - p.X + size - 2, poi.P.Y - p.Y + size - 2, size, size);
                }
                if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    return poi;
            }
            return null;
        }
        GraphLine FindLineByPoint(List<GraphLine> lines, Point p)
        {
            var size = 10;
            var buffer = new Bitmap(size * 2, size * 2);
            foreach (var line in lines)
            {
                //draw each line on small region around current point p and check pixel in point p 

                using (var g = Graphics.FromImage(buffer))
                {
                    g.Clear(Color.Black);
                    g.DrawLine(new Pen(Color.Green, 3 / ZoomSize), line.StartPoint.P.X - p.X + size, line.StartPoint.P.Y - p.Y + size, line.EndPoint.P.X - p.X + size, line.EndPoint.P.Y - p.Y + size);
                }

                if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    return line;
            }
            return null;
        }
        public void FindCurveByPoint(List<GraphCurve> curves, Point p, out GraphCurve Cout, out int Cindex)
        {
            int size = 10;
            Bitmap buffer = new Bitmap(size * 2, size * 2);
            foreach (var curve in curves)
            {
                List<PointF> temp = GraphCurveToBez(curve);
                List<PointF> temp2 = new List<PointF>();
                for (int i = 0; i < temp.Count; i++)
                {
                    temp2.Add(new PointF(temp[i].X - p.X + size, temp[i].Y - p.Y + size));
                }
                for (int i = 0; i < temp2.Count - 1; i += 3)
                {
                    try
                    {
                        PointF[] t = { temp2[i], temp2[i + 1], temp2[i + 2], temp2[i + 3] };
                        using (var g = Graphics.FromImage(buffer))
                        {
                            g.Clear(Color.Black);
                            g.DrawBeziers(new Pen(Color.Green, 3 / ZoomSize), t);
                        }

                        if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                        {
                            Cout = curve;
                            Cindex = i / 3 + 1;
                            return;
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.Write(ex.Message);
                        Cout = null;
                        Cindex = -1;
                        return;
                    }
                }
            }
            Cout = null;
            Cindex = -1;
            return;
        }
        public void FindCurveControlByPoint(List<GraphCurve> curves, Point p, out GraphCurve Cout, out int index, out int FS)//first=0 second=1
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
                                    g.DrawRectangle(new Pen(Color.Green, 5 / ZoomSize), temp2[i - 1].X - 2, temp2[i - 1].Y - 2, size, size);
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
                                    g.DrawRectangle(new Pen(Color.Green, 5 / ZoomSize), temp2[i + 1].X - 2, temp2[i + 1].Y - 2, size, size);
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
        public GraphArc FindArcByPoint(List<GraphArc> arcs, Point p)
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
                    for (int i = 0; i < 4; i++)
                    {
                        temp[i].X = temp[i].X - p.X + size;
                        temp[i].Y = temp[i].Y - p.Y + size;
                    }
                    g.DrawBezier(new Pen(Color.Green, 5 / ZoomSize), temp[0], temp[1], temp[2], temp[3]);
                }

                if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    return a;
            }
            return null;
        }
        public GraphArc FindArcControlByPoint(List<GraphArc> arcs, Point p)
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
                            g.DrawRectangle(new Pen(Color.Green, 5 / ZoomSize), a.getControlPoint().X - p.X + size - 2, a.getControlPoint().Y - p.Y + size - 2, size, size);
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
                        g.DrawRectangle(new Pen(Color.Green, 5 / ZoomSize), minx - p.X + size - 2, miny - p.Y + size - 2, size, size);
                    }
                    if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    {
                        return 0;
                    }
                    using (var g = Graphics.FromImage(buffer))
                    {
                        g.Clear(Color.Black);
                        g.DrawRectangle(new Pen(Color.Green, 5 / ZoomSize), maxx - p.X + size - 2, miny - p.Y + size - 2, size, size);
                    }
                    if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    {
                        return 1;
                    }
                    using (var g = Graphics.FromImage(buffer))
                    {
                        g.Clear(Color.Black);
                        g.DrawRectangle(new Pen(Color.Green, 5 / ZoomSize), minx - p.X + size - 2, maxy - p.Y + size - 2, size, size);
                    }
                    if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    {
                        return 2;
                    }
                    using (var g = Graphics.FromImage(buffer))
                    {
                        g.Clear(Color.Black);
                        g.DrawRectangle(new Pen(Color.Green, 5 / ZoomSize), maxx - p.X + size - 2, maxy - p.Y + size - 2, size, size);
                    }
                    if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    {
                        return 3;
                    }
                    using (var g = Graphics.FromImage(buffer))
                    {
                        g.Clear(Color.Black);
                        g.DrawRectangle(new Pen(Color.Green, 5 / ZoomSize), (maxx + minx) / 2 - 3 / ZoomSize - p.X + size - 2, miny - 23 / ZoomSize - p.Y + size - 2, size, size);
                    }
                    if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    {
                        return 4;
                    }
                }

            }
            return -1;
        }
        public GraphText FindTextByPoint(List<GraphText> texts, Point p)
        {
            var size = 5;
            var buffer = new Bitmap(size * 2, size * 2);
            foreach (var t in texts)
            {
                //draw each line on small region around current point p and check pixel in point p 

                using (var g = Graphics.FromImage(buffer))
                {
                    g.Clear(Color.Black);
                    if (t.straight == false)
                    {
                        g.DrawString(t.S, new Font("新細明體", 12, FontStyle.Bold), Brushes.Green, new PointF(t.P.X - p.X + size, t.P.Y - p.Y + size));
                    }
                    else
                    {
                        for (int i = 0; i < t.S.Length; i++)
                        {
                            g.DrawString(t.S[i].ToString(), new Font("新細明體", 12, FontStyle.Bold), Brushes.Green, new PointF(t.P.X - p.X + size, t.P.Y - p.Y + i * 12 + size));
                        }
                    }
                }

                if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    return t;
            }
            return null;
        }
        public PathDistance FindDistByPoint(List<PathDistance> PDistList, Point p)
        {
            var size = 10;
            var buffer = new Bitmap(size * 2, size * 2);
            foreach (var path in PDistList)
            {
                //draw each line on small region around current point p and check pixel in point p 
                PointF pt1, pt2;
                float dist;
                path.Get_Dist_Point(out dist, out pt1, out pt2);
                GraphLine line = new GraphLine(new GraphPoint(pt1.X, pt1.Y), new GraphPoint(pt2.X, pt2.Y));
                using (var g = Graphics.FromImage(buffer))
                {
                    g.Clear(Color.Black);
                    g.DrawLine(new Pen(Color.Green, 3 / ZoomSize), line.StartPoint.P.X - p.X + size, line.StartPoint.P.Y - p.Y + size, line.EndPoint.P.X - p.X + size, line.EndPoint.P.Y - p.Y + size);
                }

                if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    return path;
            }
            return null;
        }
        public FormulaToLine FindFTLByPoint(List<FormulaToLine> FTLList, Point p)
        {
            var size = 10;
            var buffer = new Bitmap(size * 2, size * 2);
            foreach (var ftl in FTLList)
            {
                //draw each line on small region around current point p and check pixel in point p 

                using (var g = Graphics.FromImage(buffer))
                {
                    g.Clear(Color.Black);
                    g.DrawLine(new Pen(Color.Green, 3 / ZoomSize), ftl.anchor1.X - p.X + size, ftl.anchor1.Y - p.Y + size, ftl.anchor2.X - p.X + size, ftl.anchor2.Y - p.Y + size);
                }

                if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    return ftl;
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
            PorpertyList.Add(FormulaToLineGroupBox);
            PorpertyList.Add(PointGroupBox);
            PorpertyList[0].Location = new Point(0, 0);
            PorpertyList[1].Location = new Point(0, 0);
            PorpertyList[2].Location = new Point(0, 0);
            PorpertyList[3].Location = new Point(0, 0);
            PorpertyList[4].Location = new Point(0, 0);
            PorpertyList[0].Visible = false;
            PorpertyList[1].Visible = false;
            PorpertyList[2].Visible = false;
            PorpertyList[3].Visible = false;
            PorpertyList[4].Visible = false;
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
            /*foreach (var g in PorpertyList)
            {
                /*foreach (var obj in g.Controls)
                {
                    if (obj.GetType() == typeof(TextBox))
                    {
                        TextBox tb = (TextBox)obj;
                        tb.Text = "123";
                    }
                }
                g.Width = splitContainer1.Panel2.Width;
                g.Height = splitContainer1.Panel2.Height;
            }*/
        }
        private void splitContainer1_Panel1_SizeChanged(object sender, EventArgs e)
        {
            tabControl1.Width = splitContainer1.Panel1.Width;
            tabControl1.Height = splitContainer1.Panel1.Height;
            /*foreach (var g in PorpertyList)
            {
                g.Width = splitContainer1.Panel2.Width;
                g.Height = splitContainer1.Panel2.Height;
            }*/
        }
        private void splitContainer1_Panel2_SizeChanged(object sender, EventArgs e)
        {
            tabControl1.Width = splitContainer1.Panel1.Width;
            tabControl1.Height = splitContainer1.Panel1.Height;
            /*foreach (var g in PorpertyList)
            {
                g.Width = splitContainer1.Panel2.Width;
                g.Height = splitContainer1.Panel2.Height;
            }*/
        }

        private void 儲存影像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string data = "V200905\n";
                data += "ClothStandardSize\n";
                foreach (var css in ClothStandardSize)
                    data += css.ToString() + " ";
                data = data.Remove(data.Length - 1);
                data += "\n";
                data += (ClothStandardName == "" ? "0" : ClothStandardName) + " " + (ClothStandardNumber == "" ? "0" : ClothStandardNumber) + "\n";//200903

                data += "Formula\n";
                data += FormulaList.Count + "\n";
                foreach(var f in FormulaList)
                {
                    data += f.name + "\n";
                    foreach (var ele in f.EleL)
                        data += ele.ToString() + " ";
                    data += "\n";
                }
                foreach (var t in TabpageDataList)
                {
                    data += WriteTabPageData(t, FormulaList);
                }
                StreamWriter s = new StreamWriter(File.Open(saveFileDialog1.FileName, FileMode.Create), Encoding.UTF8);
                s.Write(data);
                s.Flush();
                s.Close();
            }
        }
        public static string WriteTabPageData(TabpageData dout, List<Formula> fmllist)
        {
            string data = "";
            data += "TabpageData\n";
            data += dout.TabpageName + " " + dout.width + " " + dout.height + "\n";
            data += (dout.ClothGrainMark.visible == true ? "T " : "F ") + dout.ClothGrainMark.loc_type + " " + dout.ClothGrainMark.dir_type + " " +
                dout.ClothGrainMark.Loc.X + " " + dout.ClothGrainMark.Loc.Y + " " + dout.ClothGrainMark.Dir + " " + dout.ClothGrainMark.size + "\n";

            data += "Points\n";
            data += dout.PointL.Count + "\n";
            foreach (var p in dout.PointL)
            {
                data += p.P.X + " " + p.P.Y + "\n";
                data += p.name + "\n"; //200819
            }

            data += "Lines\n";
            data += dout.LineL.Count + "\n";
            foreach (var l in dout.LineL)
            {
                data += dout.PointL.FindIndex(x => x == l.StartPoint) + " " + dout.PointL.FindIndex(x => x == l.EndPoint)
                    + " " + (l.isSeam ? "T " : "F ") + l.Seam + " " + ColorTranslator.ToHtml(l.co) + " " + (l.dash ? "T" : "F") + "\n";
                data += "Fix " + (l.fix ? "T" : "F") + "\n";
                data += l.name + "\n"; //200819
            }

            data += "SupPoints\n";
            data += dout.SupPointL.Count + "\n";
            foreach (var p in dout.SupPointL)
            {
                data += p.P.X + " " + p.P.Y + "\n";
            }

            data += "SupLines\n";
            data += dout.SupLineL.Count + "\n";
            foreach (var l in dout.SupLineL)
            {
                data += dout.SupPointL.FindIndex(x => x == l.StartPoint) + " " + dout.SupPointL.FindIndex(x => x == l.EndPoint)
                    + " " + ColorTranslator.ToHtml(l.co) + "\n";
            }

            data += "Curves\n";
            data += dout.CurveL.Count + "\n";
            foreach (var c in dout.CurveL)
            {
                data += c.path.Count + " " + (c.isSeam ? "T " : "F ") + c.Seam + " " + ColorTranslator.ToHtml(c.co) + " " + (c.dash ? "T" : "F") + "\n";
                data += "Fix " + (c.fix ? "T" : "F") + "\n";
                data += c.name + "\n"; //200819
                for (int i = 0; i < c.path.Count; i++)
                {
                    data += dout.PointL.FindIndex(x => x == c.path[i]) + " " + c.disFirst[i].X + " " + c.disFirst[i].Y + " "
                        + c.disSecond[i].X + " " + c.disSecond[i].Y + " " + c.type[i] + "\n";
                }
            }

            data += "Arcs\n";
            data += dout.ArcL.Count + "\n";
            foreach (var a in dout.ArcL)
            {
                data += dout.PointL.FindIndex(x => x == a.StartPoint) + " " + dout.PointL.FindIndex(x => x == a.EndPoint)
                    + " " + a.getControlPoint().X + " " + a.getControlPoint().Y + " " + ColorTranslator.ToHtml(a.co) + " " + (a.dash ? "T" : "F") + "\n";
                data += a.name + "\n"; //200819
            }

            data += "Group\n";
            data += dout.GroupL.Count + "\n";
            foreach (var g in dout.GroupL)
            {
                data += WriteGroup(dout, g);
            }

            data += "Text\n";
            data += dout.TextL.Count + "\n";
            foreach (var t in dout.TextL)
            {
                data += t.P.X + " " + t.P.Y + " " + (t.straight ? "T" : "F") + " " + t.S  + "\n";
            }

            data += "Path\n";
            data += dout.PathL.Count + "\n";
            foreach (var pa in dout.PathL)
            {
                data += WriteGroup(dout, pa);
            }

            data += "PathDistance\n";
            data += dout.PDistL.Count + "\n";
            foreach (var pd in dout.PDistL)
            {
                #region PD write
                if (pd.Is_HV)
                {
                    data += "T\n";
                    data += pd.XY_Pos + "\n";
                    data += pd.T_hori_F_verti ? "T\n" : "F\n";
                }
                else
                {
                    data += "F\n";
                    data += pd.anchor1.X + " " + pd.anchor1.Y + "\n";
                    data += pd.anchor2.X + " " + pd.anchor2.Y + "\n";
                }
                data += pd.type + "\n";
                if (pd.type == 0 && pd.is_SupL1 && pd.is_SupL2)
                {
                    data += "S" + dout.SupLineL.FindIndex(x => x == pd.L1) + "\n";
                    data += "S" + dout.SupLineL.FindIndex(x => x == pd.L2) + "\n";
                }
                else if (pd.type == 0 && pd.is_SupL1)
                {
                    data += "S" + dout.SupLineL.FindIndex(x => x == pd.L1) + "\n";
                    data += dout.LineL.FindIndex(x => x == pd.L2) + "\n";
                }
                else if (pd.type == 0 && pd.is_SupL2)
                {
                    data += dout.LineL.FindIndex(x => x == pd.L1) + "\n";
                    data += "S" + dout.SupLineL.FindIndex(x => x == pd.L2) + "\n";
                }
                else if (pd.type == 0)
                {
                    data += dout.LineL.FindIndex(x => x == pd.L1) + "\n";
                    data += dout.LineL.FindIndex(x => x == pd.L2) + "\n";
                }
                else if (pd.type == 1 && pd.is_SupL1)
                {
                    data += "S" + dout.SupLineL.FindIndex(x => x == pd.L1) + "\n";
                    data += dout.CurveL.FindIndex(x => x == pd.C1) + " " + pd.cindex1 + "\n";
                }
                else if (pd.type == 1)
                {
                    data += dout.LineL.FindIndex(x => x == pd.L1) + "\n";
                    data += dout.CurveL.FindIndex(x => x == pd.C1) + " " + pd.cindex1 + "\n";
                }
                else if (pd.type == 2)
                {
                    data += dout.CurveL.FindIndex(x => x == pd.C1) + " " + pd.cindex1 + "\n";
                    data += dout.CurveL.FindIndex(x => x == pd.C2) + " " + pd.cindex2 + "\n";
                }
                else if (pd.type == 3 && pd.is_SupL1)
                {
                    data += "S" + dout.SupLineL.FindIndex(x => x == pd.L1) + "\n";
                    data += dout.ArcL.FindIndex(x => x == pd.A1) + "\n";
                }
                else if (pd.type == 3)
                {
                    data += dout.LineL.FindIndex(x => x == pd.L1) + "\n";
                    data += dout.ArcL.FindIndex(x => x == pd.A1) + "\n";
                }
                else if (pd.type == 4)
                {
                    data += dout.CurveL.FindIndex(x => x == pd.C1) + " " + pd.cindex1 + "\n";
                    data += dout.ArcL.FindIndex(x => x == pd.A1) + "\n";
                }
                else if (pd.type == 5)
                {
                    data += dout.ArcL.FindIndex(x => x == pd.A1) + "\n";
                    data += dout.ArcL.FindIndex(x => x == pd.A2) + "\n";
                }
                #endregion
            }

            data += "FormulaToLine\n";
            data += dout.FtoLL.Count + "\n";
            foreach(var ftl in dout.FtoLL)
            {
                #region FormulaToLine write  
                //ll lc cc
                data += ftl.type + " " + ftl.mode + " " + ftl.anchor1.X + " " + ftl.anchor1.Y + " " 
                    + ftl.anchor2.X + " " + ftl.anchor2.Y + " " + fmllist.FindIndex(x => x == ftl.fml) + " " + ftl.path_type + "\n";
                if (ftl.type < 3)
                {
                    if (ftl.type == 0)
                    {
                        data += dout.LineL.FindIndex(x => x == ftl.L1) + " " + dout.LineL.FindIndex(x => x == ftl.L2) + "\n";

                    }
                    else if (ftl.type == 1)
                    {
                        data += dout.LineL.FindIndex(x => x == ftl.L1) + " " + dout.CurveL.FindIndex(x => x == ftl.C1) + " " + ftl.cindex1 + "\n";
                    }
                    else if (ftl.type == 2)
                    {
                        data += dout.CurveL.FindIndex(x => x == ftl.C1) + " " + ftl.cindex1 + " " + dout.CurveL.FindIndex(x => x == ftl.C2) + " " + ftl.cindex2 + "\n";
                    }
                    data += "Lable\n";
                    data += WriteGroup(dout, ftl.labled1);
                    data += WriteGroup(dout, ftl.labled2);
                    data += WriteGroup(dout, ftl.zoom_lable1);
                    data += WriteGroup(dout, ftl.zoom_lable2);
                }
                else
                {
                    data += dout.PointL.FindIndex(x => x == ftl.unfixed_P1) + " ";
                    if (ftl.type == 3)
                    {
                        data += dout.LineL.FindIndex(x => x == ftl.L1) + "\n";
                    }
                    else if (ftl.type == 4)
                    {
                        data += dout.CurveL.FindIndex(x => x == ftl.C1) + " " + ftl.cindex1 + "\n";
                    }
                    else if (ftl.type == 5)
                    {
                        data += dout.PointL.FindIndex(x => x == ftl.unfixed_P2) + "\n";
                    }
                }
                string fml = "";//200819
                foreach (var ele in ftl.formula_eleL)
                    fml += ele.ToString() + " ";
                data += fml + "\n";
                data += ftl.prop12 + "\n";
                data += ftl.name + "\n";//200905
                #endregion
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
                double[] cssarr = new double[29];
                string csname = "", csnum = "";
                List<Formula> fmllist = new List<Formula>();
                int save_version = 0;
                StreamReader sr = new StreamReader(File.Open(openFileDialog1.FileName, FileMode.Open), Encoding.Default);
                try
                {
                    if(sr.Peek() == 'V')
                    {
                        string s = sr.ReadLine();
                        s = s.Remove(0, 1);
                        int.TryParse(s, out save_version);
                    }
                    if (sr.Peek() == 'C')
                    {
                        sr.ReadLine();
                        string s = sr.ReadLine();
                        string[] arr = s.Split(' ');
                        for (int i = 0; i < arr.Count() && i < cssarr.Count(); i++)
                            double.TryParse(arr[i], out cssarr[i]);
                        if (save_version >= 200904)
                        {
                            s = sr.ReadLine();
                            arr = s.Split(' ');
                            csname = arr[0];
                            csnum = arr[1];
                        }
                    }
                    if(sr.Peek() == 'F')
                    {
                        sr.ReadLine();
                        string s = sr.ReadLine();
                        int fnum;int.TryParse(s, out fnum);
                        for (int i = 0; i < fnum; i++)
                        {
                            string n = sr.ReadLine();
                            Parser p = new Parser();
                            string el = sr.ReadLine();
                            Formula f = new Formula(n, p.Parse(el));
                            fmllist.Add(f);
                        }
                    }
                    TempTabpageData = ReadTabData(sr, fmllist, save_version);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "讀檔錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    sr.Close();
                }
                TabpageDataList = TempTabpageData;
                TabpagesList = new List<TabPage>();
                tabControl1.TabPages.Clear();
                foreach (var td in TabpageDataList)
                {
                    TabPage tp = new TabPage(td.TabpageName);
                    tp.AutoScroll = true;
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
                    PDistList = td.PDistL;
                    SupPointsList = td.SupPointL;
                    SupLineList = td.SupLineL;
                    FormulaToLineList = td.FtoLL;
                    ClothGrainMark = td.ClothGrainMark;
                    Undo_Data = td.Undo;
                    pictureBox1.Width = (int)(td.width * ZoomSize);
                    pictureBox1.Height = (int)(td.height * ZoomSize);
                    int tdindex = TabpageDataList.FindIndex(x => x == td);
                    PointCombine();
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
                ClothStandardSize = cssarr;
                ClothStandardName = csname;
                ClothStandardNumber = csnum;
                FormulaList = fmllist;
                PointsList = TabpageDataList[0].PointL;
                LineList = TabpageDataList[0].LineL;
                CurveList = TabpageDataList[0].CurveL;
                ArcList = TabpageDataList[0].ArcL;
                GroupList = TabpageDataList[0].GroupL;
                PathList = TabpageDataList[0].PathL;
                TextList = TabpageDataList[0].TextL;
                PDistList = TabpageDataList[0].PDistL;
                SupPointsList = TabpageDataList[0].SupPointL;
                SupLineList = TabpageDataList[0].SupLineL;
                FormulaToLineList = TabpageDataList[0].FtoLL;
                ClothGrainMark = TabpageDataList[0].ClothGrainMark;
                Undo_Data = TabpageDataList[0].Undo;
                TabpagesList[0].Controls.Add(pictureBox1);
                TabpagesList[0].Controls.Add(pictureBox2);
                TabpagesList[0].AutoScroll = true;
                pictureBox1.Location = new Point(20, 20);
                pictureBox2.Location = new Point(0, 0);

                pictureBox1.Visible = true;
                pictureBox2.Visible = true;
                Form1_SizeChanged(new object(), new EventArgs());
            }
        }
        public List<TabpageData> ReadTabData(StreamReader sr, List<Formula> fmllist, int version)
        {
            try
            {
                List<TabpageData> tdl = new List<TabpageData>();
                TabpageData td = new TabpageData();
                string s;
                string[] sarr;
                bool first = true;

                while (!sr.EndOfStream)
                {
                    s = sr.ReadLine();
                    switch (s)
                    {
                        case "TabpageData":
                            #region
                            if (!first)
                            {
                                tdl.Add(td);
                                td = new TabpageData();
                            }
                            first = false;
                            s = sr.ReadLine();
                            sarr = s.Split(' ');
                            td.TabpageName = sarr[0];
                            int width; int.TryParse(sarr[1], out width); td.width = width;
                            int height; int.TryParse(sarr[2], out height); td.height = height;
                            s = sr.ReadLine();
                            sarr = s.Split(' ');
                            bool v = sarr[0] == "T";
                            int lt, dt;
                            float lx, ly;
                            double d;
                            double size;
                            int.TryParse(sarr[1], out lt);
                            int.TryParse(sarr[2], out dt);
                            float.TryParse(sarr[3], out lx);
                            float.TryParse(sarr[4], out ly);
                            double.TryParse(sarr[5], out d);
                            double.TryParse(sarr[6], out size);
                            td.ClothGrainMark = new GrainMark()
                            {
                                visible = v,
                                loc_type = lt,
                                dir_type = dt,
                                Loc = new PointF(lx, ly),
                                Dir = d,
                                size = size
                            };
                            break;
                        #endregion

                        case "Points":
                            #region
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
                                if (version >= 200819)
                                {
                                    s = sr.ReadLine();
                                    p.name = s;
                                }
                                td.PointL.Add(p);
                            }
                            break;
                        #endregion

                        case "Lines":
                            #region
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
                                float Seam; float.TryParse(sarr[3], out Seam);
                                GraphLine l = new GraphLine(td.PointL[start], td.PointL[end]);
                                l.isSeam = isSeam;
                                l.Seam = Seam;
                                if (sarr.Length > 4)
                                {
                                    l.co = ColorTranslator.FromHtml(sarr[4]);
                                    l.dash = sarr[5] == "T";
                                }
                                if(sr.Peek() == 'F')
                                {
                                    s = sr.ReadLine();
                                    sarr = s.Split(' ');
                                    l.fix = sarr[1] == "T";
                                }
                                if(version >= 200819)
                                {
                                    s = sr.ReadLine();
                                    l.name = s;
                                }
                                td.PointL[start].Relative++;
                                td.PointL[end].Relative++;
                                td.LineL.Add(l);
                            }
                            break;
                        #endregion

                        case "Curves":
                            #region
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
                                if (sarr.Length > 3)
                                {
                                    c.co = ColorTranslator.FromHtml(sarr[3]);
                                    c.dash = sarr[4] == "T";
                                }
                                if (sr.Peek() == 'F')
                                {
                                    s = sr.ReadLine();
                                    sarr = s.Split(' ');
                                    c.fix = sarr[1] == "T";
                                }
                                if(version >= 200819)
                                {
                                    s = sr.ReadLine();
                                    c.name = s;
                                }
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
                            break;
                        #endregion

                        case "Arcs":
                            #region
                            s = sr.ReadLine();
                            int anum; int.TryParse(s, out anum);
                            td.ArcL = new List<GraphArc>();
                            for (int i = 0; i < anum; i++)
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
                                if (sarr.Length > 4)
                                {
                                    a.co = ColorTranslator.FromHtml(sarr[4]);
                                    a.dash = sarr[5] == "T";
                                }
                                if (version >= 200819)
                                {
                                    s = sr.ReadLine();
                                    a.name = s;
                                }
                                td.ArcL.Add(a);
                            }
                            break;
                        #endregion

                        case "Group":
                            #region
                            s = sr.ReadLine();
                            int gnum; int.TryParse(s, out gnum);
                            td.GroupL = new List<GraphGroup>();
                            for (int i = 0; i < gnum; i++)
                            {
                                td.GroupL.Add(ReadGroup(sr, td));
                            }
                            break;
                        #endregion

                        case "Text":
                            #region
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
                                bool straight = false;
                                int jstart = 2;
                                if (sarr.Length > 2)
                                {
                                    straight = sarr[2] == "T";
                                    jstart = (sarr[2] == "T" || sarr[2] == "F") ? 3 : 2;
                                }
                                for (int j = jstart; j < sarr.Count(); j++)
                                    temps += sarr[j];
                                td.TextL.Add(new GraphText { P = new PointF(x, y), S = temps, straight = straight });
                            }
                            break;
                        #endregion

                        case "Path":
                            #region
                            s = sr.ReadLine();
                            int panum; int.TryParse(s, out panum);
                            td.PathL = new List<GraphGroup>();
                            for (int i = 0; i < panum; i++)
                            {
                                td.PathL.Add(ReadGroup(sr, td));
                            }
                            break;
                        #endregion

                        case "PathDistance":
                            #region
                            s = sr.ReadLine();
                            int pdnum; int.TryParse(s, out pdnum);
                            td.PDistL = new List<PathDistance>();
                            for (int i = 0; i < pdnum; i++)
                            {
                                #region PD read
                                float pos = -1;
                                bool thfv = false, Is_HV = false;
                                PointF p1 = new PointF(), p2 = new PointF();
                                s = sr.ReadLine();
                                Is_HV = s == "T";
                                if (Is_HV)
                                {
                                    s = sr.ReadLine();
                                    float.TryParse(s, out pos);
                                    s = sr.ReadLine();
                                    thfv = s == "T";
                                }
                                else
                                {
                                    s = sr.ReadLine();
                                    sarr = s.Split(' ');
                                    float x; float.TryParse(sarr[0], out x);
                                    float y; float.TryParse(sarr[1], out y);
                                    p1 = new PointF(x, y);
                                    s = sr.ReadLine();
                                    sarr = s.Split(' ');
                                    float.TryParse(sarr[0], out x);
                                    float.TryParse(sarr[1], out y);
                                    p2 = new PointF(x, y);
                                }
                                s = sr.ReadLine();
                                int type; int.TryParse(s, out type);
                                if (type == 0)
                                {
                                    s = sr.ReadLine();
                                    bool is_SupL1 = false, is_SupL2 = false;
                                    if (s[0] == 'S')
                                    {
                                        s = s.Remove(0, 1);
                                        is_SupL1 = true;
                                    }
                                    int L1i; int.TryParse(s, out L1i);
                                    s = sr.ReadLine();
                                    if (s[0] == 'S')
                                    {
                                        s = s.Remove(0, 1);
                                        is_SupL2 = true;
                                    }
                                    int L2i; int.TryParse(s, out L2i);
                                    if (Is_HV)
                                    {
                                        if (is_SupL1 && is_SupL2)
                                            td.PDistL.Add(new PathDistance(td.SupLineL[L1i], td.SupLineL[L2i], pos, thfv)
                                            { is_SupL1 = is_SupL1, is_SupL2 = is_SupL2 });
                                        else if (is_SupL1)
                                            td.PDistL.Add(new PathDistance(td.SupLineL[L1i], td.LineL[L2i], pos, thfv)
                                            { is_SupL1 = is_SupL1, is_SupL2 = is_SupL2 });
                                        else if(is_SupL2)
                                            td.PDistL.Add(new PathDistance(td.LineL[L1i], td.SupLineL[L2i], pos, thfv)
                                            { is_SupL1 = is_SupL1, is_SupL2 = is_SupL2 });
                                        else
                                            td.PDistL.Add(new PathDistance(td.LineL[L1i], td.LineL[L2i], pos, thfv)
                                            { is_SupL1 = is_SupL1, is_SupL2 = is_SupL2 });
                                    }
                                    else
                                    {
                                        if (is_SupL1 && is_SupL2)
                                            td.PDistL.Add(new PathDistance(td.SupLineL[L1i], td.SupLineL[L2i], p1, p2)
                                            { is_SupL1 = is_SupL1, is_SupL2 = is_SupL2 });
                                        else if (is_SupL1)
                                            td.PDistL.Add(new PathDistance(td.SupLineL[L1i], td.LineL[L2i], p1, p2)
                                            { is_SupL1 = is_SupL1, is_SupL2 = is_SupL2 });
                                        else if (is_SupL2)
                                            td.PDistL.Add(new PathDistance(td.LineL[L1i], td.SupLineL[L2i], p1, p2)
                                            { is_SupL1 = is_SupL1, is_SupL2 = is_SupL2 });
                                        else
                                            td.PDistL.Add(new PathDistance(td.LineL[L1i], td.LineL[L2i], p1, p2)
                                            { is_SupL1 = is_SupL1, is_SupL2 = is_SupL2 });
                                    }
                                }
                                else if (type == 1)
                                {
                                    s = sr.ReadLine();
                                    bool is_SupL1 = false;
                                    if (s[0] == 'S')
                                    {
                                        s = s.Remove(0, 1);
                                        is_SupL1 = true;
                                    }
                                    int L1i; int.TryParse(s, out L1i);
                                    s = sr.ReadLine();
                                    sarr = s.Split(' ');
                                    int C1i, C1i_index; int.TryParse(sarr[0], out C1i); int.TryParse(sarr[1], out C1i_index);
                                    if (Is_HV)
                                    {
                                        if (is_SupL1)
                                            td.PDistL.Add(new PathDistance(td.SupLineL[L1i], td.CurveL[C1i], C1i_index, pos, thfv)
                                            { is_SupL1 = is_SupL1 });
                                        else
                                            td.PDistL.Add(new PathDistance(td.LineL[L1i], td.CurveL[C1i], C1i_index, pos, thfv)
                                            { is_SupL1 = is_SupL1 });
                                    }
                                    else
                                    {
                                        if (is_SupL1)
                                            td.PDistL.Add(new PathDistance(td.SupLineL[L1i], td.CurveL[C1i], C1i_index, p1, p2)
                                            { is_SupL1 = is_SupL1 });
                                        else
                                            td.PDistL.Add(new PathDistance(td.LineL[L1i], td.CurveL[C1i], C1i_index, p1, p2)
                                            { is_SupL1 = is_SupL1 });
                                    }
                                }
                                else if (type == 2)
                                {
                                    s = sr.ReadLine();
                                    sarr = s.Split(' ');
                                    int C1i, C1i_index; int.TryParse(sarr[0], out C1i); int.TryParse(sarr[1], out C1i_index);
                                    s = sr.ReadLine();
                                    sarr = s.Split(' ');
                                    int C2i, C2i_index; int.TryParse(sarr[0], out C2i); int.TryParse(sarr[1], out C2i_index);
                                    if (Is_HV)
                                    {
                                        td.PDistL.Add(new PathDistance(td.CurveL[C1i], C1i_index, td.CurveL[C2i], C2i_index, pos, thfv));
                                    }
                                    else
                                    {
                                        td.PDistL.Add(new PathDistance(td.CurveL[C1i], C1i_index, td.CurveL[C2i], C2i_index, p1, p2));
                                    }
                                }
                                else if (type == 3)
                                {
                                    s = sr.ReadLine();
                                    bool is_SupL1 = false;
                                    if (s[0] == 'S')
                                    {
                                        s = s.Remove(0, 1);
                                        is_SupL1 = true;
                                    }
                                    int L1i; int.TryParse(s, out L1i);
                                    s = sr.ReadLine();
                                    int A1i; int.TryParse(s, out A1i);
                                    if (Is_HV)
                                    {
                                        if (is_SupL1)
                                            td.PDistL.Add(new PathDistance(td.SupLineL[L1i], td.ArcL[A1i], pos, thfv)
                                            { is_SupL1 = is_SupL1 });
                                        else
                                            td.PDistL.Add(new PathDistance(td.LineL[L1i], td.ArcL[A1i], pos, thfv)
                                            { is_SupL1 = is_SupL1 });
                                    }
                                    else
                                    {
                                        if (is_SupL1)
                                            td.PDistL.Add(new PathDistance(td.SupLineL[L1i], td.ArcL[A1i], p1, p2)
                                            { is_SupL1 = is_SupL1 });
                                        else
                                            td.PDistL.Add(new PathDistance(td.LineL[L1i], td.ArcL[A1i], p1, p2)
                                            { is_SupL1 = is_SupL1 });
                                    }
                                }
                                else if (type == 4)
                                {
                                    s = sr.ReadLine();
                                    sarr = s.Split(' ');
                                    int C1i, C1i_index; int.TryParse(sarr[0], out C1i); int.TryParse(sarr[1], out C1i_index);
                                    s = sr.ReadLine();
                                    int A1i; int.TryParse(s, out A1i);
                                    if (Is_HV)
                                    {
                                        td.PDistL.Add(new PathDistance(td.CurveL[C1i], C1i_index, td.ArcL[A1i], pos, thfv));
                                    }
                                    else
                                    {
                                        td.PDistL.Add(new PathDistance(td.CurveL[C1i], C1i_index, td.ArcL[A1i], p1, p2));
                                    }
                                }
                                else if (type == 5)
                                {
                                    s = sr.ReadLine();
                                    int A1i; int.TryParse(s, out A1i);
                                    s = sr.ReadLine();
                                    int A2i; int.TryParse(s, out A2i);
                                    if (Is_HV)
                                    {
                                        td.PDistL.Add(new PathDistance(td.ArcL[A1i], td.ArcL[A2i], pos, thfv));
                                    }
                                    else
                                    {
                                        td.PDistL.Add(new PathDistance(td.ArcL[A1i], td.ArcL[A2i], p1, p2));
                                    }
                                }
                                #endregion
                            }
                            break;
                        #endregion

                        case "SupPoints":
                            #region
                            s = sr.ReadLine();
                            int spnum; int.TryParse(s, out spnum);
                            td.SupPointL = new List<GraphPoint>();
                            for (int i = 0; i < spnum; i++)
                            {
                                s = sr.ReadLine();
                                sarr = s.Split(' ');
                                float x; float.TryParse(sarr[0], out x);
                                float y; float.TryParse(sarr[1], out y);
                                GraphPoint p = new GraphPoint(x, y);
                                td.SupPointL.Add(p);
                            }
                            break;
                        #endregion

                        case "SupLines":
                            #region
                            s = sr.ReadLine();
                            int slnum; int.TryParse(s, out slnum);
                            td.SupLineL = new List<GraphLine>();
                            for (int i = 0; i < slnum; i++)
                            {
                                s = sr.ReadLine();
                                sarr = s.Split(' ');
                                int start; int.TryParse(sarr[0], out start);
                                int end; int.TryParse(sarr[1], out end);
                                GraphLine l = new GraphLine(td.SupPointL[start], td.SupPointL[end]);
                                l.co = ColorTranslator.FromHtml(sarr[2]);
                                td.SupPointL[start].Relative++;
                                td.SupPointL[end].Relative++;
                                td.SupLineL.Add(l);
                            }
                            break;
                        #endregion

                        case "ClothGrainMark":
                            #region
                            int dirt, loct;
                            double dir;
                            float locx,locy;
                            double cgmsize;
                            bool vis;
                            s = sr.ReadLine();
                            sarr = s.Split(' ');
                            vis = sarr[0] == "T";
                            int.TryParse(sarr[1], out loct);
                            int.TryParse(sarr[2], out dirt);
                            double.TryParse(sarr[5], out dir);
                            float.TryParse(sarr[3], out locx);
                            float.TryParse(sarr[4], out locy);
                            double.TryParse(sarr[6], out cgmsize);
                            GrainMark cgm = new GrainMark
                            {
                                dir_type = dirt,
                                loc_type = loct,
                                Dir = dir,
                                Loc = new PointF(locx, locy),
                                size = cgmsize,
                                visible = vis
                            };
                            break;
                        #endregion

                        case "FormulaToLine":
                            #region
                            s = sr.ReadLine();
                            int ftlnum;int.TryParse(s, out ftlnum);
                            td.FtoLL = new List<FormulaToLine>();
                            for(int i = 0; i < ftlnum; i++)
                            {
                                FormulaToLine ftl;
                                s = sr.ReadLine();
                                sarr = s.Split(' ');
                                int type, mode, fmlindex, path_type;
                                float a1x, a1y, a2x, a2y;
                                int.TryParse(sarr[0], out type);
                                int.TryParse(sarr[1], out mode);
                                float.TryParse(sarr[2], out a1x);
                                float.TryParse(sarr[3], out a1y);
                                float.TryParse(sarr[4], out a2x);
                                float.TryParse(sarr[5], out a2y);
                                int.TryParse(sarr[6], out fmlindex);
                                int.TryParse(sarr[7], out path_type);
                                s = sr.ReadLine();
                                sarr = s.Split(' ');
                                if (type == 0)
                                {
                                    int l1, l2;
                                    int.TryParse(sarr[0], out l1);
                                    int.TryParse(sarr[1], out l2);
                                    ftl = new FormulaToLine(td.LineL[l1], td.LineL[l2], new PointF(a1x, a1y), new PointF(a2x, a2y), mode, path_type);
                                    ftl.fml = fmllist[fmlindex];
                                    if (sr.Peek() == 'L')
                                    {
                                        sr.ReadLine();
                                        ftl.labled1 = ReadGroup(sr, td);
                                        ftl.labled2 = ReadGroup(sr, td);
                                        ftl.zoom_lable1 = ReadGroup(sr, td);
                                        ftl.zoom_lable2 = ReadGroup(sr, td);
                                    }
                                }
                                else if (type == 1)
                                {
                                    int l1, c1,c1i;
                                    int.TryParse(sarr[0], out l1);
                                    int.TryParse(sarr[1], out c1);
                                    int.TryParse(sarr[2], out c1i);
                                    ftl = new FormulaToLine(td.LineL[l1], td.CurveL[c1], c1i, new PointF(a1x, a1y), new PointF(a2x, a2y), mode, path_type);
                                    ftl.fml = fmllist[fmlindex];
                                    if (sr.Peek() == 'L')
                                    {
                                        sr.ReadLine();
                                        ftl.labled1 = ReadGroup(sr, td);
                                        ftl.labled2 = ReadGroup(sr, td);
                                        ftl.zoom_lable1 = ReadGroup(sr, td);
                                        ftl.zoom_lable2 = ReadGroup(sr, td);
                                    }
                                }
                                else if (type == 2)
                                {
                                    int c1, c1i, c2, c2i;
                                    int.TryParse(sarr[0], out c1);
                                    int.TryParse(sarr[1], out c1i);
                                    int.TryParse(sarr[2], out c2);
                                    int.TryParse(sarr[3], out c2i);
                                    ftl = new FormulaToLine(td.CurveL[c1], c1i, td.CurveL[c2], c2i, new PointF(a1x, a1y), new PointF(a2x, a2y), mode, path_type);
                                    ftl.fml = fmllist[fmlindex];
                                    if (sr.Peek() == 'L')
                                    {
                                        sr.ReadLine();
                                        ftl.labled1 = ReadGroup(sr, td);
                                        ftl.labled2 = ReadGroup(sr, td);
                                        ftl.zoom_lable1 = ReadGroup(sr, td);
                                        ftl.zoom_lable2 = ReadGroup(sr, td);
                                    }
                                }
                                else if (type == 3)
                                {
                                    int unfixedp, l1;
                                    int.TryParse(sarr[0], out unfixedp);
                                    int.TryParse(sarr[1], out l1);
                                    ftl = new FormulaToLine(td.PointL[unfixedp], td.LineL[l1], new PointF(a1x, a1y), new PointF(a2x, a2y), mode);
                                    ftl.fml = fmllist[fmlindex];
                                }
                                else if (type == 4)
                                {
                                    int unfixedp, c1, c1i;
                                    int.TryParse(sarr[0], out unfixedp);
                                    int.TryParse(sarr[1], out c1);
                                    int.TryParse(sarr[2], out c1i);
                                    ftl = new FormulaToLine(td.PointL[unfixedp], td.CurveL[c1], c1i, new PointF(a1x, a1y), new PointF(a2x, a2y), mode);
                                    ftl.fml = fmllist[fmlindex];
                                }
                                else if(type == 5)
                                {
                                    int unfixedp1, unfixedp2;
                                    int.TryParse(sarr[0], out unfixedp1);
                                    int.TryParse(sarr[1], out unfixedp2);
                                    ftl = new FormulaToLine(td.PointL[unfixedp1], td.PointL[unfixedp2], new PointF(a1x, a1y), new PointF(a2x, a2y), mode);
                                    ftl.fml = fmllist[fmlindex];
                                }
                                else
                                {
                                    throw new Exception("FTL未知type");
                                }
                                if(version >= 200819)
                                {
                                    Parser p = new Parser();
                                    string el = sr.ReadLine();
                                    ftl.formula_eleL = p.Parse(el);
                                    s = sr.ReadLine();
                                    ftl.prop12 = s;
                                }
                                if (version >= 200905)
                                {
                                    ftl.name = sr.ReadLine();
                                }
                                td.FtoLL.Add(ftl);
                            }
                            break;
                            #endregion
                    }
                }
                tdl.Add(td);
                return tdl;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "讀檔錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            pictureBox1.Width = (int)(8.3 * 72 * ZoomSize);
            pictureBox1.Height = (int)(11.7 * 72 * ZoomSize);
            pictureBox1.Image = new Bitmap((int)(8.3 * 72 * ZoomSize), (int)(11.7 * 72 * ZoomSize));
            pictureBox2.Width = (int)((8.3 * 72 * ZoomSize)) + 20;
            pictureBox2.Height = (int)((11.7 * 72) * ZoomSize) + 20;
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(Color.WhiteSmoke);
            PointsList = new List<GraphPoint>();
            LineList = new List<GraphLine>();
            CurveList = new List<GraphCurve>();
            ArcList = new List<GraphArc>();
            TextList = new List<GraphText>();
            GroupList = new List<GraphGroup>();
            PathList = new List<GraphGroup>();
            PDistList = new List<PathDistance>();
            SupPointsList = new List<GraphPoint>();
            SupLineList = new List<GraphLine>();
            FormulaList = new List<Formula>();
            FormulaToLineList = new List<FormulaToLine>();
            ClothGrainMark = new GrainMark();
            ClothStandardSize = new double[29];
            ClothStandardSize[0] = -1;

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
            a.PDistL = PDistList;
            a.SupPointL = SupPointsList;
            a.SupLineL = SupLineList;
            a.FtoLL = FormulaToLineList;
            a.ClothGrainMark = ClothGrainMark;
            a.Undo = Undo_Data;
            a.width = (int)(8.3 * 72);
            a.height = (int)(11.7 * 72);
            a.TabpageName = TabpagesList[0].Text;
            TabpageDataList.Add(a);
            Push_Undo_Data();
            Form1_SizeChanged(new object(), new EventArgs());
            pictureBox1.Visible = true;
            pictureBox2.Visible = true;
        }
        private void 調整大小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int a = TabpagesList.FindIndex(x => x == tabControl1.SelectedTab);
            Size f2 = new Size();
            if (f2.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap((int)(f2.width * ZoomSize), (int)(f2.height * ZoomSize));
                pictureBox2.Image = new Bitmap((int)(f2.width * ZoomSize) + 20, (int)(f2.height * ZoomSize) + 20);
                Graphics g = Graphics.FromImage(pictureBox1.Image);
                g.Clear(Color.WhiteSmoke);
                Form1_SizeChanged(new object(), new EventArgs());
                TabpageDataList[tabControl1.SelectedIndex].width = (int)(f2.width);
                TabpageDataList[tabControl1.SelectedIndex].height = (int)(f2.height);
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
            int totalW = (int)(pictureBox1.Width / t / SizeOfNet / 2 / ZoomSize) - 1;
            for (int i = 0; i * SizeOfNet * ZoomSize < pictureBox1.Width; i++)
            {
                e.Graphics.DrawLine(p, new PointF((i * (float)SizeOfNet) * ZoomSize + 20, 10), new PointF((i * (float)SizeOfNet) * ZoomSize + 20, 20));
                if (t > 0)
                    e.Graphics.DrawString(t * (i - totalW) + "", new Font("新細明體", 8, FontStyle.Regular), new SolidBrush(Color.Black), new PointF((i * (float)SizeOfNet) * ZoomSize + 20, 5));
                for (int j = 1; j <= DesityOfNet; j++)
                {
                    e.Graphics.DrawLine(p, new PointF((i * (float)SizeOfNet + (float)SizeOfNet / DesityOfNet * j) * ZoomSize + 20, 15)
                                    , new PointF((i * (float)SizeOfNet + (float)SizeOfNet / DesityOfNet * j) * ZoomSize + 20, 20));
                }
            }
            int totalH = (int)(pictureBox1.Height / t / SizeOfNet / 2 / ZoomSize) - 1;
            for (int i = 0; i * SizeOfNet * ZoomSize < pictureBox1.Height; i++)
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
        GraphArc M8_TempA = null;
        int M8_TempC_Index = -1;
        bool M8_is_SupL1 = false, M8_is_SupL2 = false;
        int M8_State = 0;
        #endregion

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.Clear(Color.White);

            Paint_GrainMark(e);
            Paint_Net(e);
            if (SupLine_Visible == true)
            {
                if(!SupLine_Lock)
                    Paint_SupPoint(e);
                Paint_SupLine(e);
            }
            if (Object_Name_Visible)
                Print_Names(e);
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
            if (Mouse_Mode == Mouse_Mode_Type.FormulaToL)
                Paint_FormulaToLines(e);
        }
        private void Paint_Net(PaintEventArgs e)
        {
            if (Net_Mode)
            {
                for (float i = 0; i * ZoomSize < this.pictureBox1.Height; i += (float)SizeOfNet)
                {
                    var pen = new Pen(Color.LightSlateGray, 1);
                    e.Graphics.DrawLine(pen, new PointF(0, i * ZoomSize), new PointF(this.pictureBox1.Width, i * ZoomSize));
                    pen = new Pen(Color.LightGray, 1);
                    for (int j = 1; j <= DesityOfNet; j++)
                    {
                        e.Graphics.DrawLine(pen, new PointF(0, (i + (float)SizeOfNet / DesityOfNet * j) * ZoomSize), new PointF(this.pictureBox1.Width, (i + (float)SizeOfNet / DesityOfNet * j) * ZoomSize));
                    }
                }
                for (float i = 0; i * ZoomSize < this.pictureBox1.Width; i += (float)SizeOfNet)
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
        private void Paint_GrainMark(PaintEventArgs e)
        {
            if (ClothGrainMark.visible)
            {
                var cgm = ClothGrainMark;
                var td = TabpageDataList[tabControl1.SelectedIndex];
                var color = Color.Black;
                float size = (float)cgm.size / 100;
                var textsize = 2 * size;
                var pen = new Pen(color, textsize);
                //pen.StartCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                //pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                PointF mid = cgm.loc_type == 0 ? new PointF(50, 50) :
                             cgm.loc_type == 1 ? new PointF(td.width - 50, 50) :
                             cgm.loc_type == 2 ? new PointF(50, td.height - 50) :
                             cgm.loc_type == 3 ? new PointF(td.width - 50, td.height - 50) : cgm.Loc;
                double angle = cgm.dir_type == 0 ? 90 :
                               cgm.dir_type == 1 ? 0 :
                               cgm.dir_type == 2 ? 45 :
                               cgm.dir_type == 3 ? 135 : cgm.Dir;
                PointF start = new PointF(mid.X + 30 * (float)Math.Cos(angle / 180 * Math.PI) * size, mid.Y + 30 * (float)Math.Sin(angle / 180 * Math.PI) * size),
                       end = new PointF(mid.X - 30 * (float)Math.Cos(angle / 180 * Math.PI) * size, mid.Y - 30 * (float)Math.Sin(angle / 180 * Math.PI) * size);
                e.Graphics.DrawLine(pen, start.X * ZoomSize, start.Y * ZoomSize, end.X * ZoomSize, end.Y * ZoomSize);
                PointF st_ar1 = new PointF(start.X - (float)Math.Cos((angle - 30) / 180 * Math.PI) * 10 * size, start.Y - (float)Math.Sin((angle - 30) / 180 * Math.PI) * 10 * size);
                PointF st_ar2 = new PointF(start.X - (float)Math.Cos((angle + 30) / 180 * Math.PI) * 10 * size, start.Y - (float)Math.Sin((angle + 30) / 180 * Math.PI) * 10 * size);
                e.Graphics.DrawLine(pen, start.X * ZoomSize, start.Y * ZoomSize, st_ar1.X * ZoomSize, st_ar1.Y * ZoomSize);
                e.Graphics.DrawLine(pen, start.X * ZoomSize, start.Y * ZoomSize, st_ar2.X * ZoomSize, st_ar2.Y * ZoomSize);
                PointF en_ar1 = new PointF(end.X + (float)Math.Cos((angle - 30) / 180 * Math.PI) * 10 * size, end.Y + (float)Math.Sin((angle - 30) / 180 * Math.PI) * 10 * size);
                PointF en_ar2 = new PointF(end.X + (float)Math.Cos((angle + 30) / 180 * Math.PI) * 10 * size, end.Y + (float)Math.Sin((angle + 30) / 180 * Math.PI) * 10 * size);
                e.Graphics.DrawLine(pen, end.X * ZoomSize, end.Y * ZoomSize, en_ar1.X * ZoomSize, en_ar1.Y * ZoomSize);
                e.Graphics.DrawLine(pen, end.X * ZoomSize, end.Y * ZoomSize, en_ar2.X * ZoomSize, en_ar2.Y * ZoomSize);
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
                if (line.dash)
                    pen.DashPattern = new float[] { 5, 5 };
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
                if (c.dash)
                    pen.DashPattern = new float[] { 5, 5 };
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
                if (a.dash)
                    pen.DashPattern = new float[] { 5, 5 };
                var arr = a.to_cubic_bezier();
                for (int i = 0; i < 4; i++)
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
                if (s.straight == false)
                {
                    e.Graphics.DrawString(s.S, fo, Brushes.Black, s.P.X * ZoomSize, s.P.Y * ZoomSize);
                }
                else
                {
                    for(int i = 0; i < s.S.Length; i++)
                    {
                        e.Graphics.DrawString(s.S[i].ToString(), fo, Brushes.Black, s.P.X * ZoomSize, (s.P.Y + i * 14) * ZoomSize);
                    }
                }
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
                for (int i = 0; i < 4; i++)
                {
                    arr[i].X *= ZoomSize;
                    arr[i].Y *= ZoomSize;
                }
                e.Graphics.DrawBezier(pe, arr[0], arr[1], arr[2], arr[3]);
            }
            else if (Mouse_Mode == Mouse_Mode_Type.SupLine)
            {
                pe.DashPattern = new float[] { 5, 5 };
                if (SelectedPoint == null)
                    e.Graphics.DrawLine(pe, previous_point.P.X * ZoomSize, previous_point.P.Y * ZoomSize, p.X * ZoomSize, p.Y * ZoomSize);
                else
                    e.Graphics.DrawLine(pe, previous_point.P.X * ZoomSize, previous_point.P.Y * ZoomSize, SelectedPoint.P.X * ZoomSize, SelectedPoint.P.Y * ZoomSize);
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
            if (M6_State == 0)
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
            foreach (var path in PathList)
            {
                /*float minx, miny, maxx, maxy;
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
                }*/
                List<PointF> poly_vert = new List<PointF>();
                List<float> distList = new List<float>();
                bool[] check = new bool[path.L.Count]; for (int i = 0; i < check.Count(); i++) check[i] = false;
                GraphPoint p = path.L[0] != null ? path.L[0].StartPoint : path.C[0].path[0];
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
                } while (p != (path.L[0] != null ? path.L[0].StartPoint : path.C[0].path[0]));
                poly_vert.RemoveAt(0);
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
                    e.Graphics.DrawLine(pe, todrawl[i].X * ZoomSize, todrawl[i].Y * ZoomSize, todrawl[(i + 1) % todrawl.Count].X * ZoomSize, todrawl[(i + 1) % todrawl.Count].Y * ZoomSize);
                }
            }
        }
        private void Paint_SupPoint(PaintEventArgs e)
        {
            foreach (var p in SupPointsList)
            {
                var color = p == SelectedSupPoint ? Color.Red : p.MarkL != null ? Color.Orange : Color.Black;
                var size = p == SelectedSupPoint ? 2 : 1;
                var pen = new Pen(color, size);
                e.Graphics.DrawRectangle(pen, p.P.X * ZoomSize - 3, p.P.Y * ZoomSize - 3, 6, 6);
            }
        }
        private void Paint_SupLine(PaintEventArgs e)
        {
            foreach (var line in SupLineList)
            {
                var color = line.co;
                var size = 1;
                color = (line == SelectedSupLine && SupLine_Lock == false) ? Color.Red : line.co; 
                 size = line == SelectedSupLine ? 1 : 2;
                var pen = new Pen(color, size);
                pen.DashPattern = new float[] { 5, 5 };
                e.Graphics.DrawLine(pen, line.StartPoint.P.X * ZoomSize, line.StartPoint.P.Y * ZoomSize, line.EndPoint.P.X * ZoomSize, line.EndPoint.P.Y * ZoomSize);
            }
        }
        private void Print_Names(PaintEventArgs e)
        {
            foreach (var l in LineList)
            {
                var fo = new Font("新細明體", 12);
                e.Graphics.DrawString(l.name, fo, Brushes.Black, (l.StartPoint.P.X+l.EndPoint.P.X) / 2 * ZoomSize - l.name.Length * 6, (l.StartPoint.P.Y + l.EndPoint.P.Y) / 2 * ZoomSize - 18);
            }
            foreach (var c in CurveList)
            {
                var fo = new Font("新細明體", 12);
                e.Graphics.DrawString(c.name, fo, Brushes.Black, c.path[c.path.Count/2].P.X * ZoomSize - c.name.Length * 6, c.path[c.path.Count / 2].P.Y * ZoomSize - 18);
            }
        }
        private List<PointF> DevideCurve(GraphCurve c)
        {
            List<PointF> ans = new List<PointF>();
            for (int i = 0; i < c.path.Count - 1; i++)
            {
                PointF p1 = c.path[i].P, p2 = c.path[i + 1].P;
                PointF c1 = new PointF(p1.X + c.disSecond[i].X, p1.Y + c.disSecond[i].Y), c2 = new PointF(p2.X + c.disFirst[i + 1].X, p2.Y + c.disFirst[i + 1].Y);
                for (double j = 0; j < 1; j += 0.04)
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
                if (distList[i] != 0)
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
                    else newList.Add(l1e);
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
            var fo = new Font("新細明體", 12);
            List<PathDistance> pdl = new List<PathDistance>();
            foreach (var pd in PDistList)
            {
                bool inL = pd.in_List(LineList, CurveList, ArcList, SupLineList);
                PointF pt1, pt2;
                float dist;
                pd.Get_Dist_Point(out dist, out pt1, out pt2);
                if (inL)
                {
                    if (!(pd.is_SupL1 || pd.is_SupL2))
                    {
                        var pe_line = new Pen(pd == SelectedDist ? Color.Red : Color.Gray, 1);
                        e.Graphics.DrawLine(pe_line, pt1.X * ZoomSize, pt1.Y * ZoomSize, pt2.X * ZoomSize, pt2.Y * ZoomSize);
                        PointF mid = new PointF((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2);
                        if (LenthUnit == 0)
                            dist /= 72 / 2.54F;
                        else
                            dist /= 72F;
                        e.Graphics.DrawString(dist.ToString("F") + (LenthUnit == 0 ? " cm" : " inch"), fo, Brushes.Black, mid.X * ZoomSize, mid.Y * ZoomSize);
                    }
                    else if (SupLine_Visible)
                    {
                        var pe_line = new Pen(pd == SelectedDist ? Color.Red : Color.Gray, 1);
                        e.Graphics.DrawLine(pe_line, pt1.X * ZoomSize, pt1.Y * ZoomSize, pt2.X * ZoomSize, pt2.Y * ZoomSize);
                        PointF mid = new PointF((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2);
                        if (LenthUnit == 0)
                            dist /= 72 / 2.54F;
                        else
                            dist /= 72F;
                        e.Graphics.DrawString(dist.ToString("F") + (LenthUnit == 0 ? " cm" : " inch"), fo, Brushes.Black, mid.X * ZoomSize, mid.Y * ZoomSize);
                    }
                }
                else
                {
                    pdl.Add(pd);
                }
            }
            foreach (var pd in pdl)
                PDistList.Remove(pd);

            if (Mouse_Mode == Mouse_Mode_Type.Dist_Verti || Mouse_Mode == Mouse_Mode_Type.Dist_Hori || Mouse_Mode == Mouse_Mode_Type.Dist_2points || Mouse_Mode==Mouse_Mode_Type.Dist_RightLine)
            {
                if (M8_State == 1)
                {
                    PointF p = For_Paint;
                    var pe = new Pen(Color.Black, 1);
                    e.Graphics.DrawRectangle(pe, M8_TempP.P.X * ZoomSize - 2, M8_TempP.P.Y * ZoomSize - 2, 4, 4);
                    e.Graphics.DrawLine(pe, M8_TempP.P.X * ZoomSize, M8_TempP.P.Y * ZoomSize, p.X * ZoomSize, p.Y * ZoomSize);
                    GraphLine l = FindLineByPoint(LineList, For_Paint);
                    GraphCurve c;
                    GraphArc a = FindArcByPoint(ArcList, For_Paint);
                    GraphLine sl = FindLineByPoint(SupLineList, For_Paint);
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
                    else if (a != null)
                    {
                        PointF[] pl = a.to_cubic_bezier();
                        for (int i = 0; i < 4; i++)
                        {
                            pl[i] = new PointF(pl[i].X * ZoomSize, pl[i].Y * ZoomSize);
                        }
                        e.Graphics.DrawBeziers(pe, pl);
                    }
                    else if(sl != null)
                    {
                        pe.DashPattern = new float[] { 5, 5 };
                        e.Graphics.DrawLine(pe, sl.StartPoint.P.X * ZoomSize, sl.StartPoint.P.Y * ZoomSize, sl.EndPoint.P.X * ZoomSize, sl.EndPoint.P.Y * ZoomSize);
                    }
                }
                if (M8_State == 0)
                {
                    GraphLine l = FindLineByPoint(LineList, For_Paint);
                    GraphCurve c;
                    GraphArc a = FindArcByPoint(ArcList, For_Paint);
                    int index;
                    FindCurveByPoint(CurveList, For_Paint, out c, out index);
                    GraphLine sl = FindLineByPoint(SupLineList, For_Paint);
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
                    else if (a != null)
                    {
                        PointF[] pl = a.to_cubic_bezier();
                        for (int i = 0; i < 4; i++)
                        {
                            pl[i] = new PointF(pl[i].X * ZoomSize, pl[i].Y * ZoomSize);
                        }
                        e.Graphics.DrawBeziers(pe, pl);
                    }
                    else if (sl != null)
                    {
                        pe.DashPattern = new float[] { 5, 5 };
                        e.Graphics.DrawLine(pe, sl.StartPoint.P.X * ZoomSize, sl.StartPoint.P.Y * ZoomSize, sl.EndPoint.P.X * ZoomSize, sl.EndPoint.P.Y * ZoomSize);
                    }
                }
            }
        }
        private void Paint_FormulaToLines(PaintEventArgs e)
        {
            foreach (var line in LineList)
            {
                var color = line.fix ? Color.Black : Color.Green;
                var size = 2;
                var pen = new Pen(color, size);
                if (line.dash)
                    pen.DashPattern = new float[] { 5, 5 };
                e.Graphics.DrawLine(pen, line.StartPoint.P.X * ZoomSize, line.StartPoint.P.Y * ZoomSize, line.EndPoint.P.X * ZoomSize, line.EndPoint.P.Y * ZoomSize);
            }
            foreach (var c in CurveList)
            {
                var color = c.fix ? Color.Black : Color.Green;
                var size = 2;
                var pen = new Pen(color, size);
                if (c.dash)
                    pen.DashPattern = new float[] { 5, 5 };
                List<PointF> t = GraphCurveToBez(c);
                for (int i = 0; i < t.Count; i++)
                {
                    t[i] = new PointF(t[i].X * ZoomSize, t[i].Y * ZoomSize);
                }
                e.Graphics.DrawBeziers(pen, t.ToArray());
            }

            var fo = new Font("新細明體", 12);
            List<FormulaToLine> todelete = new List<FormulaToLine>();
            foreach (var ftl in FormulaToLineList)
            {
                bool inL = ftl.in_List(PointsList, LineList, CurveList);
                PointF pt1 = ftl.anchor1, pt2 = ftl.anchor2;
                if (inL)
                {
                    int index = FormulaToLineList.FindIndex(x => x == ftl);
                    var pe_line = new Pen(Edit_FL_Temp_FTL_Priority_oblistview_index == index ? Color.Red : Color.Gray, 1);
                    e.Graphics.DrawLine(pe_line, pt1.X * ZoomSize, pt1.Y * ZoomSize, pt2.X * ZoomSize, pt2.Y * ZoomSize);
                    PointF mid = new PointF((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2);
                    double len = Math.Sqrt(Math.Pow(ftl.anchor1.X - ftl.anchor2.X, 2) + Math.Pow(ftl.anchor1.Y - ftl.anchor2.Y, 2));
                    /*
                    if (LenthUnit == 0)
                        dist /= 72 / 2.54F;
                    else
                        dist /= 72F;
                        */
                    len /= 72;
                    e.Graphics.DrawString(ftl.fml.name + " 長=" + len.ToString("F") + "inch", fo, Brushes.Black, mid.X * ZoomSize, mid.Y * ZoomSize);

                    if (ftl.labled1 != null)
                    {
                        GraphGroup zlf, zlb;
                        ftl.get_zoomlable_FB(1, out zlf, out zlb);
                        GraphGroup[] gl = { ftl.labled1, zlf, zlb };
                        for (int j = ftl.labled1.L.Count > 1 ? 0 : 1; j < 3; j++)
                        {
                            GraphGroup g = gl[j];
                            var color = j == 0 ? Color.Orange : Color.Aqua;
                            var size = 2;
                            var pen = new Pen(color, size);
                            foreach (var l in g.L)
                                if (l != null)
                                {
                                    if (l.dash)
                                        pen.DashPattern = new float[] { 5, 5 };
                                    e.Graphics.DrawLine(pen, l.StartPoint.P.X * ZoomSize, l.StartPoint.P.Y * ZoomSize, l.EndPoint.P.X * ZoomSize, l.EndPoint.P.Y * ZoomSize);
                                }
                            foreach (var c in g.C)
                                if (c != null)
                                {
                                    if (c.dash)
                                        pen.DashPattern = new float[] { 5, 5 };
                                    List<PointF> t = GraphCurveToBez(c);
                                    for (int i = 0; i < t.Count; i++)
                                    {
                                        t[i] = new PointF(t[i].X * ZoomSize, t[i].Y * ZoomSize);
                                    }
                                    e.Graphics.DrawBeziers(pen, t.ToArray());
                                }
                        }
                    }

                    if (ftl.labled2 != null)
                    {
                        GraphGroup zlf, zlb;
                        ftl.get_zoomlable_FB(2, out zlf, out zlb);
                        GraphGroup[] gl = { ftl.labled2, zlf, zlb };
                        for (int j = ftl.labled2.L.Count > 1 ? 0 : 1; j < 3; j++)
                        {
                            GraphGroup g = gl[j];
                            var color = j == 0 ? Color.Orange : Color.Aqua;
                            var size = 2;
                            var pen = new Pen(color, size);
                            foreach (var l in g.L)
                                if (l != null)
                                {
                                    if (l.dash)
                                        pen.DashPattern = new float[] { 5, 5 };
                                    e.Graphics.DrawLine(pen, l.StartPoint.P.X * ZoomSize, l.StartPoint.P.Y * ZoomSize, l.EndPoint.P.X * ZoomSize, l.EndPoint.P.Y * ZoomSize);
                                }
                            foreach (var c in g.C)
                                if (c != null)
                                {
                                    if (c.dash)
                                        pen.DashPattern = new float[] { 5, 5 };
                                    List<PointF> t = GraphCurveToBez(c);
                                    for (int i = 0; i < t.Count; i++)
                                    {
                                        t[i] = new PointF(t[i].X * ZoomSize, t[i].Y * ZoomSize);
                                    }
                                    e.Graphics.DrawBeziers(pen, t.ToArray());
                                }
                        }
                    }
                }
                else
                {
                    todelete.Add(ftl);
                }
            }
            foreach (var del in todelete)
                FormulaToLineList.Remove(del);

            if (Mouse_Mode == Mouse_Mode_Type.FormulaToL)
            {
                if (FSLB_State == 0 && FSLB_TempFormulaToLine != null && FSLB_TempPath != null)
                {
                    GraphLine l = FindLineByPoint(LineList, For_Paint);
                    GraphCurve c; int ci;
                    FindCurveByPoint(CurveList, For_Paint, out c, out ci);
                    var pe = new Pen(Color.Red, 2);
                    if (FSLB_TempL != null)
                        e.Graphics.DrawLine(pe, FSLB_TempL.StartPoint.P.X * ZoomSize, FSLB_TempL.StartPoint.P.Y * ZoomSize, FSLB_TempL.EndPoint.P.X * ZoomSize, FSLB_TempL.EndPoint.P.Y * ZoomSize);
                    if (l != null)
                        e.Graphics.DrawLine(pe, l.StartPoint.P.X * ZoomSize, l.StartPoint.P.Y * ZoomSize, l.EndPoint.P.X * ZoomSize, l.EndPoint.P.Y * ZoomSize);
                    if(FSLB_TempC != null)
                    {
                        List<PointF> t = GraphCurveToBez(FSLB_TempC);
                        for (int i = 0; i < t.Count; i++)
                        {
                            t[i] = new PointF(t[i].X * ZoomSize, t[i].Y * ZoomSize);
                        }
                        e.Graphics.DrawBeziers(pe, t.ToArray());
                    }
                    if(c != null)
                    {
                        List<PointF> t = GraphCurveToBez(c);
                        for (int i = 0; i < t.Count; i++)
                        {
                            t[i] = new PointF(t[i].X * ZoomSize, t[i].Y * ZoomSize);
                        }
                        e.Graphics.DrawBeziers(pe, t.ToArray());
                    }
                }
                if (FSLB_State == 2)
                {
                    PointF p = For_Paint;
                    var pe = new Pen(Color.Black, 1);
                    e.Graphics.DrawRectangle(pe, FSLB_TempP.P.X * ZoomSize - 2, FSLB_TempP.P.Y * ZoomSize - 2, 4, 4);
                    e.Graphics.DrawLine(pe, FSLB_TempP.P.X * ZoomSize, FSLB_TempP.P.Y * ZoomSize, p.X * ZoomSize, p.Y * ZoomSize);
                    GraphPoint gp = FindPointByPoint(PointsList, For_Paint);
                    GraphLine l = FindLineByPoint(LineList, For_Paint);
                    GraphCurve c;
                    int index;
                    FindCurveByPoint(CurveList, For_Paint, out c, out index);
                    pe = new Pen(Color.Red, 2);
                    if(gp != null)
                    {
                        e.Graphics.DrawRectangle(pe, gp.P.X * ZoomSize - 3, gp.P.Y * ZoomSize - 3, 6, 6);
                    }
                    else if (l != null)
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
                if (FSLB_State == 1)
                {
                    GraphPoint gp = FindPointByPoint(PointsList, For_Paint);
                    GraphLine l = FindLineByPoint(LineList, For_Paint);
                    GraphCurve c;
                    int index;
                    FindCurveByPoint(CurveList, For_Paint, out c, out index);
                    var pe = new Pen(Color.Red, 2);
                    if (gp != null)
                    {
                        e.Graphics.DrawRectangle(pe, gp.P.X * ZoomSize - 3, gp.P.Y * ZoomSize - 3, 6, 6);
                    }
                    else if(l != null)
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
                    for (int i = 0; i < 1000; i++)
                    {
                        if (!CurveList.Exists(x => x.name == "C" + i))
                        {
                            c.name = "C" + i;
                            break;
                        }
                    }
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
                else if (Moving.type == Moving_Type.Point && Moving.StartMoveMousePoint == e)
                {
                    Click_To_Select_Point();
                }
                else if (Moving.type == Moving_Type.Line && Moving.StartMoveMousePoint == e)
                {
                    Click_To_Select_Line();
                }
                else if (Moving.type == Moving_Type.Curve && Moving.StartMoveMousePoint == e)
                {
                    Click_To_Select_Curve();
                }
                else if (Moving.type == Moving_Type.Arc && Moving.StartMoveMousePoint == e)
                {
                    Click_To_Select_Arc();
                }
                else if (Moving.type == Moving_Type.Group && Moving.StartMoveMousePoint == e)
                {
                    Click_To_Select_Group();
                }
                else if (Moving.type == Moving_Type.Text && Moving.StartMoveMousePoint == e)
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
            else if (Mouse_Mode == Mouse_Mode_Type.FormulaToL && ev.Button == MouseButtons.Left)
            {
                MBLeft_FormulaToL_Up(ev);
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
            if (Mouse_Mode == Mouse_Mode_Type.Arc)
            {
                if(ev.Button == MouseButtons.Left)
                    MBLeft_Arc_DOWN(ev);
                else if(ev.Button == MouseButtons.Right)
                {
                    previous_point = null;
                    is_Drowing = false;
                }
            }
            if (ev.Button == MouseButtons.Left && Mouse_Mode == Mouse_Mode_Type.SupLine)
            {
                MBLeft_SupLine_DOWN(ev);
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
            if (Mouse_Mode == Mouse_Mode_Type.Cutting)
            {
                if (ev.Button == MouseButtons.Left)
                    MBLeft_Cutting_DOWN(ev);
                else if (ev.Button == MouseButtons.Right)
                    M7_Temp_Clean();
            }
            if (Mouse_Mode == Mouse_Mode_Type.Dist_Hori || Mouse_Mode == Mouse_Mode_Type.Dist_Verti || Mouse_Mode == Mouse_Mode_Type.Dist_2points || Mouse_Mode == Mouse_Mode_Type.Dist_RightLine)
            {
                if (ev.Button == MouseButtons.Left)
                    MBLeft_PathDist_DOWN(ev);
                else if (ev.Button == MouseButtons.Right)
                    M8_Temp_Clean();
            }
            if(Mouse_Mode == Mouse_Mode_Type.FormulaToL)
            {
                if (ev.Button == MouseButtons.Left)
                    MBLeft_FormulaToL_DOWN(ev);
                else if (ev.Button == MouseButtons.Right)
                {
                    MBRight_FormulaToL_DOWN(ev);
                    if (Right_Temp_FTL == null)
                        FormulaToL_Temp_Clean();
                }
            }
            if (ev.Button == MouseButtons.Right && Mouse_Mode == Mouse_Mode_Type.Cursor)
            {
                MBRight_Cursor_DOWN(ev);
            }
            if (ev.Button == MouseButtons.Right && (Mouse_Mode == Mouse_Mode_Type.Line || Mouse_Mode == Mouse_Mode_Type.Curve || Mouse_Mode == Mouse_Mode_Type.SupLine))
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
                    //StartMoveMousePoint = new Point((int)SelectedPoint.P.X, (int)SelectedPoint.P.Y),
                    StartMoveMousePoint = e,
                    type = Moving_Type.Point
                };
            }
            else if(SelectedSupPoint != null && Moving == null)
            {
                Moving = new MoveInfo
                {
                    StartLinePoint = SelectedSupPoint,
                    StartMoveMousePoint = e,
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
            else if (SelectedSupLine != null && Moving == null)
            {
                GraphPoint selectS = new GraphPoint(SelectedSupLine.StartPoint.P.X, SelectedSupLine.StartPoint.P.Y);
                GraphPoint selectE = new GraphPoint(SelectedSupLine.EndPoint.P.X, SelectedSupLine.EndPoint.P.Y);
                //this.Capture = true;
                Moving = new MoveInfo
                {
                    Line = this.SelectedSupLine,
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
                    StartMoveMousePoint = e,
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
                    if (SelectedGroupControl == 4)
                        path.Add(new GraphPoint(SelectedGroup.P[i].P.X - mid.X, SelectedGroup.P[i].P.Y - mid.Y));
                    else
                        path.Add(new GraphPoint(SelectedGroup.P[i].P.X - minx, SelectedGroup.P[i].P.Y - miny));
                }
                List<GraphCurve> curvecontrol = new List<GraphCurve>();
                for (int i = 0; i < SelectedGroup.C.Count; i++)
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
                nowp.name = nowp.check_name(PointsList);
                PointsList.Add(nowp);
                if (is_Drowing)
                {
                    GraphLine temp = new GraphLine(previous_point, nowp);
                    for(int i = 0; i < 1000; i++)
                    {
                        if (!LineList.Exists(x => x.name == "L" + i))
                        {
                            temp.name = "L" + i;
                            break;
                        }
                    }
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
                    for (int i = 0; i < 1000; i++)
                    {
                        if (!LineList.Exists(x => x.name == "L" + i))
                        {
                            temp.name = "L" + i;
                            break;
                        }
                    }
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
                nowp.name = nowp.check_name(PointsList);
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
                    nowp.name = nowp.check_name(PointsList);
                    PointsList.Add(nowp);
                    if (!PointsList.Exists(x => x == previous_point))
                    {
                        previous_point.name = previous_point.check_name(PointsList);
                        PointsList.Add(previous_point);
                    }
                    previous_point.Relative++;
                    for (int i = 0; i < 1000; i++)
                    {
                        if (!ArcList.Exists(x => x.name == "A" + i))
                        {
                            a.name = "A" + i;
                            break;
                        }
                    }
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
                if (previous_point != null)
                {
                    GraphArc a = new GraphArc(previous_point, t);
                    previous_point.Relative++;
                    t.Relative++;
                    if (!PointsList.Exists(x => x == previous_point))
                    {
                        previous_point.name = previous_point.check_name(PointsList);
                        PointsList.Add(previous_point);
                    }
                    for (int i = 0; i < 1000; i++)
                    {
                        if (!ArcList.Exists(x => x.name == "A" + i))
                        {
                            a.name = "A" + i;
                            break;
                        }
                    }
                    ArcList.Add(a);
                    Push_Undo_Data();
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
                        {
                            thisS.name = thisS.check_name(PointsList);
                            PointsList.Add(thisS);
                        }
                        if (!PointsList.Exists(x => x == thisE))
                        {
                            thisE.name = thisE.check_name(PointsList);
                            PointsList.Add(thisE);
                        }
                    }
                    else
                    {
                        thisS = newpath.C[i].path[0];
                        thisE = newpath.C[i].path.Last();
                        foreach (var p in newpath.C[i].path)
                        {
                            p.Relative++;
                            if (!PointsList.Exists(x => x == p))
                            {
                                p.name = p.check_name(PointsList);
                                PointsList.Add(p);
                            }
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
                if (index1 == -1 || index2 == -1)
                {
                    MessageBox.Show("請將線段切在同一圖形", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    M7_Temp_Clean();
                    return;
                }
                GraphGroup newpath1 = new GraphGroup(), newpath2 = new GraphGroup();
                StraightCutPath(Path, L1, L2, C1, C1Index, C2, C2Index, p1, p2, out newpath1, out newpath2);
                for (int i = 0; i < M7_TempPath.L.Count; i++)
                {
                    DeleteLine(M7_TempPath.L[i]);
                    DeleteHoleCurve(M7_TempPath.C[i]);
                }
                float p1x = 0, p1y = 0, p2x = 0, p2y = 0;
                foreach (var p in newpath1.P)
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
                    if (txfy)
                        p.P.X += xdis;
                    else
                        p.P.Y += ydis;
                }
                foreach (var c in newpath2.C)
                {
                    if (c != null)
                    {
                        for (int i = 1; i < c.path.Count - 1; i++)
                        {
                            if (txfy)
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
                if (M8_TempL == null)
                {
                    M8_TempL = FindLineByPoint(SupLineList, e);
                    if (M8_TempL != null)
                        M8_is_SupL1 = true;
                }
                FindCurveByPoint(CurveList, e, out M8_TempC, out M8_TempC_Index);
                M8_TempA = FindArcByPoint(ArcList, e);
                M8_TempP = new GraphPoint(e.X, e.Y);
                M8_State = 1;
                if (M8_TempL == null && M8_TempC == null && M8_TempA == null)
                {
                    M8_Temp_Clean();
                    return;
                }
            }
            else if (M8_State == 1)
            {
                GraphPoint p1 = M8_TempP, p2 = new GraphPoint(e.X, e.Y);
                GraphLine L1 = M8_TempL, L2 = FindLineByPoint(LineList, e);
                if (L2 == null)
                {
                    L2 = FindLineByPoint(SupLineList, e);
                    if (L2 != null)
                    {
                            M8_is_SupL2 = true;
                    }
                }
                GraphCurve C1 = M8_TempC, C2;
                GraphArc A1 = M8_TempA, A2 = FindArcByPoint(ArcList, e);
                int C1Index = M8_TempC_Index, C2Index;
                FindCurveByPoint(CurveList, e, out C2, out C2Index);
                PathDistance pd;
                float pos = (Mouse_Mode == Mouse_Mode_Type.Dist_Hori) ? (p1.P.Y + p2.P.Y) / 2 : (p1.P.X + p2.P.X) / 2;
                if (Mouse_Mode == Mouse_Mode_Type.Dist_2points)
                {
                    if (L1 != null && L2 != null)
                    {
                        pd = new PathDistance(L1, L2, p1.P, p2.P);
                        pd.is_SupL1 = M8_is_SupL1;
                        pd.is_SupL2 = M8_is_SupL2;
                    }
                    else if (L1 != null && C2 != null)
                    {
                        pd = new PathDistance(L1, C2, C2Index, p1.P, p2.P);
                        pd.is_SupL1 = M8_is_SupL1;
                    }
                    else if (C1 != null && L2 != null)
                    {
                        pd = new PathDistance(L2, C1, C1Index, p1.P, p2.P);
                        pd.is_SupL1 = M8_is_SupL2;
                    }
                    else if (C1 != null && C2 != null)
                    {
                        pd = new PathDistance(C1, C1Index, C2, C2Index, p1.P, p2.P);
                    }
                    else if (L1 != null && A2 != null)
                    {
                        pd = new PathDistance(L1, A2, p1.P, p2.P);
                        pd.is_SupL1 = M8_is_SupL1;
                    }
                    else if (A1 != null && L2 != null)
                    {
                        pd = new PathDistance(L2, A1, p1.P, p2.P);
                        pd.is_SupL1 = M8_is_SupL2;
                    }
                    else if (C1 != null && A2 != null)
                    {
                        pd = new PathDistance(C1, C1Index, A2, p1.P, p2.P);
                    }
                    else if (A1 != null && C2 != null)
                    {
                        pd = new PathDistance(C2, C2Index, A1, p1.P, p2.P);
                    }
                    else if (A1 != null && A2 != null)
                    {
                        pd = new PathDistance(A1, A2, p1.P, p2.P);
                    }
                    else
                    {
                        M8_Temp_Clean();
                        return;
                    }
                }
                else if (Mouse_Mode == Mouse_Mode_Type.Dist_RightLine)
                {
                    if (L1 != null)
                    {
                        float xm = L1.EndPoint.P.X - L1.StartPoint.P.X;
                        float ym = L1.EndPoint.P.Y - L1.StartPoint.P.Y;
                        p2 = new GraphPoint(p1.P.X + ym, p1.P.Y - xm);
                    }
                    else if (C1 != null)
                    {
                        List<PointF> holebez = GraphCurveToBez(C1);
                        PointF[] bez = { holebez[C1Index * 3 - 3], holebez[C1Index * 3 + 1 - 3], holebez[C1Index * 3 + 2 - 3], holebez[C1Index * 3 + 3 - 3] };
                        float t = BezierFindT(bez, p1.P);
                        PointF temp1 = BezierGiveTFindPoint(bez, t - 0.01F);
                        PointF temp2 = BezierGiveTFindPoint(bez, t + 0.01F);
                        float xm = temp1.X - temp2.X;
                        float ym = temp1.Y - temp2.Y;
                        p2 = new GraphPoint(p1.P.X + ym, p1.P.Y - xm);
                    }
                    else if (A1 != null)
                    {
                        PointF[] bez = A1.to_cubic_bezier();
                        float t = BezierFindT(bez, p1.P);
                        PointF temp1 = BezierGiveTFindPoint(bez, t - 0.01F);
                        PointF temp2 = BezierGiveTFindPoint(bez, t + 0.01F);
                        float xm = temp1.X - temp2.X;
                        float ym = temp1.Y - temp2.Y;
                        p2 = new GraphPoint(p1.P.X + ym, p1.P.Y - xm);
                    }
                    else
                        return;
                    if (L1 != null && L2 != null)
                    {
                        pd = new PathDistance(L1, L2, p1.P, p2.P);
                        pd.is_SupL1 = M8_is_SupL1;
                        pd.is_SupL2 = M8_is_SupL2;
                    }
                    else if (L1 != null && C2 != null)
                    {
                        pd = new PathDistance(L1, C2, C2Index, p1.P, p2.P);
                        pd.is_SupL1 = M8_is_SupL1;
                    }
                    else if (C1 != null && L2 != null)
                    {
                        pd = new PathDistance(L2, C1, C1Index, p1.P, p2.P);
                        pd.is_SupL1 = M8_is_SupL2;
                    }
                    else if (C1 != null && C2 != null)
                    {
                        pd = new PathDistance(C1, C1Index, C2, C2Index, p1.P, p2.P);
                    }
                    else if (L1 != null && A2 != null)
                    {
                        pd = new PathDistance(L1, A2, p1.P, p2.P);
                        pd.is_SupL1 = M8_is_SupL1;
                    }
                    else if (A1 != null && L2 != null)
                    {
                        pd = new PathDistance(L2, A1, p1.P, p2.P);
                        pd.is_SupL1 = M8_is_SupL2;
                    }
                    else if (C1 != null && A2 != null)
                    {
                        pd = new PathDistance(C1, C1Index, A2, p1.P, p2.P);
                    }
                    else if (A1 != null && C2 != null)
                    {
                        pd = new PathDistance(C2, C2Index, A1, p1.P, p2.P);
                    }
                    else if (A1 != null && A2 != null)
                    {
                        pd = new PathDistance(A1, A2, p1.P, p2.P);
                    }
                    else
                    {
                        M8_Temp_Clean();
                        return;
                    }
                }
                else
                {
                    if (L1 != null && L2 != null)
                    {
                        pd = new PathDistance(L1, L2, pos, (Mouse_Mode == Mouse_Mode_Type.Dist_Hori));
                        pd.is_SupL1 = M8_is_SupL1;
                        pd.is_SupL2 = M8_is_SupL2;
                    }
                    else if (L1 != null && C2 != null)
                    {
                        pd = new PathDistance(L1, C2, C2Index, pos, (Mouse_Mode == Mouse_Mode_Type.Dist_Hori));
                        pd.is_SupL1 = M8_is_SupL1;
                    }
                    else if (C1 != null && L2 != null)
                    {
                        pd = new PathDistance(L2, C1, C1Index, pos, (Mouse_Mode == Mouse_Mode_Type.Dist_Hori));
                        pd.is_SupL1 = M8_is_SupL2;
                    }
                    else if (C1 != null && C2 != null)
                    {
                        pd = new PathDistance(C1, C1Index, C2, C2Index, pos, (Mouse_Mode == Mouse_Mode_Type.Dist_Hori));
                    }
                    else if (L1 != null && A2 != null)
                    {
                        pd = new PathDistance(L1, A2, pos, (Mouse_Mode == Mouse_Mode_Type.Dist_Hori));
                        pd.is_SupL1 = M8_is_SupL1;
                    }
                    else if (A1 != null && L2 != null)
                    {
                        pd = new PathDistance(L2, A1, pos, (Mouse_Mode == Mouse_Mode_Type.Dist_Hori));
                        pd.is_SupL1 = M8_is_SupL2;
                    }
                    else if (C1 != null && A2 != null)
                    {
                        pd = new PathDistance(C1, C1Index, A2, pos, (Mouse_Mode == Mouse_Mode_Type.Dist_Hori));
                    }
                    else if (A1 != null && C2 != null)
                    {
                        pd = new PathDistance(C2, C2Index, A1, pos, (Mouse_Mode == Mouse_Mode_Type.Dist_Hori));
                    }
                    else if (A1 != null && A2 != null)
                    {
                        pd = new PathDistance(A1, A2, pos, (Mouse_Mode == Mouse_Mode_Type.Dist_Hori));
                    }
                    else
                    {
                        M8_Temp_Clean();
                        return;
                    }
                }
                PointF pt1, pt2;
                float dist;
                pd.Get_Dist_Point(out dist, out pt1, out pt2);
                if (!((pt1.X == -1 && pt1.Y == -1) || (pt2.X == -1 && pt2.Y == -1)))
                {
                    PDistList.Add(pd);
                    Push_Undo_Data();
                }
                M8_Temp_Clean();
                PointsList.RemoveAll(x => x.Relative <= 0 && x.MarkL == null);
            }
        }
        private void MBLeft_SupLine_DOWN(MouseEventArgs ev)//輔助線
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
            GraphPoint nowp = new GraphPoint(e.X, e.Y);
            GraphPoint t = FindPointByPoint(SupPointsList, e);
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
                SupPointsList.Add(nowp);
                if (is_Drowing)
                {
                    GraphLine temp = new GraphLine(previous_point, nowp);
                    SupLineList.Add(temp);
                    temp.co = Color.Gray;
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
                    SupLineList.Add(temp);
                    temp.co = Color.Gray;
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
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(Mouse_Mode == Mouse_Mode_Type.FormulaToL)
            {

            }
            else
            {
                if (Previous_Text != null)
                    Previous_Text.S = textBox1.Text;
                else if (Change_Text != null)
                    Change_Text.S = textBox1.Text;
                Refresh();
            }
        }
        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (Mouse_Mode == Mouse_Mode_Type.FormulaToL)
            {

            }
            else
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
        }
        private void MBRight_Cursor_DOWN(MouseEventArgs ev)//右鍵
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
            Right_Temp_Clear();
            ContextMenusStrip1_invisible();

            if (SelectedPoint != null)
            {
                Right_click_Point(e);
            }
            else if (SelectedGroup != null)
            {
                Right_click_Group(e);
            }
            else if (SelectedLine != null)
            {
                Right_click_Line(e);
            }
            else if (SelectedCurve != null)
            {
                Right_click_Curve(e);
            }
            else if (SelectedArc != null)
            {
                Right_click_Arc(e);
            }
            else if (SelectedText != null)
            {
                Right_click_Text(e);
            }
            else if (SelectedDist != null)
            {
                Right_click_Dist(e);
            }
            else if(SelectedSupLine != null)
            {
                Right_click_SupLine(e);
            }
            else if(SelectedSupPoint != null)
            {
                Right_click_SupPoint(e);
            }
            else
            {
                Right_Temp_Clear();
            }
        }
        private void Right_Temp_Clear()
        {
            Right_Temp_Line = null;
            Right_Temp_Point = null;
            Right_Temp_Curve = null;
            Right_Temp_Curve_Index = -1;
            Right_Temp_Arc = null;
            Right_Temp_Text = null;
            Right_Temp_Dist = null;
            Right_Temp_SupPoint = null;
            Right_Temp_SupLine = null;
        }
        private void Right_click_Point(Point e)
        {
            toolStripMenuItem2.Text = "刪除節點";
            toolStripMenuItem2.Visible = true;
            bool incurve = false;
            int tcindex = -1;
            GraphCurve tc = null;
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
            Right_Temp_Point = SelectedPoint;
            Right_Temp_Mouse_Pos = e;
            contextMenuStrip1.Show(MousePosition);
        }
        private void Right_click_Line(Point e)
        {
            toolStripMenuItem2.Text = "刪除直線";
            toolStripMenuItem2.Visible = true;
            新增節點ToolStripMenuItem.Visible = true;

            圖形ToolStripMenuItem.Visible = true;
            if (In_Path(SelectedLine, SelectedCurve) != null)
            {
                圖形ToolStripMenuItem.Enabled = true;
                組為圖形ToolStripMenuItem.Enabled = false;
                解除圖形ToolStripMenuItem.Enabled = true;
            }
            else
            {
                圖形ToolStripMenuItem.Enabled = false;
                組為圖形ToolStripMenuItem.Enabled = false;
                解除圖形ToolStripMenuItem.Enabled = false;
            }

            選取整個圖形ToolStripMenuItem.Visible = true;
            選取整個圖形ToolStripMenuItem.Enabled = false;
            foreach (var path in PathList)
                if (path.L.Exists(x => x == SelectedLine))
                    選取整個圖形ToolStripMenuItem.Enabled = true;

            直線等分ToolStripMenuItem.Visible = true;
            if (!PointsList.Exists(x => x.MarkL == SelectedLine))
                直線等分ToolStripMenuItem.Text = "直線等分";
            else
                直線等分ToolStripMenuItem.Text = "取消等分標記";

            變更顏色ToolStripMenuItem.Visible = true;
            上下左右移動ToolStripMenuItem.Visible = true;
            線條樣式ToolStripMenuItem.Visible = true;
            實線ToolStripMenuItem.Checked = !SelectedLine.dash;
            虛線ToolStripMenuItem.Checked = SelectedLine.dash;

            移除距離標示ToolStripMenuItem.Visible = true;
            移除距離標示ToolStripMenuItem.Enabled = false;
            if (PDistList.Exists(x => x.L1 == SelectedLine || x.L2 == SelectedLine))
                移除距離標示ToolStripMenuItem.Enabled = true;

            公式對應ToolStripMenuItem.Visible = true;
            if (SelectedLine.fix)
                公式對應ToolStripMenuItem.Text = "公式對應:固定";
            else
                公式對應ToolStripMenuItem.Text = "公式對應:可動";

            Right_Temp_Mouse_Pos = e;
            Right_Temp_Line = SelectedLine;
            contextMenuStrip1.Show(MousePosition);
        }
        private void Right_click_Curve(Point e)
        {
            toolStripMenuItem2.Text = "刪除整條曲線";
            toolStripMenuItem2.Visible = true;
            新增節點ToolStripMenuItem.Visible = true;

            圖形ToolStripMenuItem.Visible = true;
            if (In_Path(SelectedLine, SelectedCurve) != null)
            {
                圖形ToolStripMenuItem.Enabled = true;
                組為圖形ToolStripMenuItem.Enabled = false;
                解除圖形ToolStripMenuItem.Enabled = true;
            }
            else
            {
                圖形ToolStripMenuItem.Enabled = false;
                組為圖形ToolStripMenuItem.Enabled = false;
                解除圖形ToolStripMenuItem.Enabled = false;
            }

            選取整個圖形ToolStripMenuItem.Visible = true;
            選取整個圖形ToolStripMenuItem.Enabled = false;
            foreach (var path in PathList)
                if (path.C.Exists(x => x == SelectedCurve))
                    選取整個圖形ToolStripMenuItem.Enabled = true;

            變更顏色ToolStripMenuItem.Visible = true;
            上下左右移動ToolStripMenuItem.Visible = true;
            線條樣式ToolStripMenuItem.Visible = true;
            實線ToolStripMenuItem.Checked = !SelectedCurve.dash;
            虛線ToolStripMenuItem.Checked = SelectedCurve.dash;

            移除距離標示ToolStripMenuItem.Visible = true;
            移除距離標示ToolStripMenuItem.Enabled = false;
            if (PDistList.Exists(x => x.C1 == SelectedCurve || x.C2 == SelectedCurve))
                移除距離標示ToolStripMenuItem.Enabled = true;

            公式對應ToolStripMenuItem.Visible = true;
            if (SelectedCurve.fix)
                公式對應ToolStripMenuItem.Text = "公式對應:固定";
            else
                公式對應ToolStripMenuItem.Text = "公式對應:可動";

            曲線平滑化ToolStripMenuItem.Visible = true;

            Right_Temp_Curve = SelectedCurve;
            Right_Temp_Curve_Index = SelectedCurveIndex;
            Right_Temp_Mouse_Pos = e;
            contextMenuStrip1.Show(MousePosition);
        }
        private void Right_click_Arc(Point e)
        {
            toolStripMenuItem2.Text = "刪除弧線";
            toolStripMenuItem2.Visible = true;
            轉換為曲線ToolStripMenuItem.Visible = true;

            圖形ToolStripMenuItem.Visible = true;
            圖形ToolStripMenuItem.Enabled = false;

            選取整個圖形ToolStripMenuItem.Visible = true;
            選取整個圖形ToolStripMenuItem.Enabled = false;
            foreach (var path in PathList)
                if (path.C.Exists(x => x == SelectedCurve))
                    選取整個圖形ToolStripMenuItem.Enabled = true;

            變更顏色ToolStripMenuItem.Visible = true;
            上下左右移動ToolStripMenuItem.Visible = true;
            線條樣式ToolStripMenuItem.Visible = true;
            實線ToolStripMenuItem.Checked = !SelectedArc.dash;
            虛線ToolStripMenuItem.Checked = SelectedArc.dash;

            if (PDistList.Exists(x => x.A1 == SelectedArc || x.A2 == SelectedArc))
                移除距離標示ToolStripMenuItem.Visible = true;
            Right_Temp_Arc = SelectedArc;
            Right_Temp_Mouse_Pos = e;
            contextMenuStrip1.Show(MousePosition);
        }
        private void Right_click_Text(Point e)
        {
            toolStripMenuItem2.Text = "刪除文字";
            toolStripMenuItem2.Visible = true;

            文字橫豎ToolStripMenuItem.Visible = true;
            直書ToolStripMenuItem.Checked = SelectedText.straight;
            橫書ToolStripMenuItem.Checked = !SelectedText.straight;

            Right_Temp_Text = SelectedText;
            Right_Temp_Mouse_Pos = e;
            contextMenuStrip1.Show(MousePosition);
        }
        private void Right_click_Dist(Point e)
        {
            toolStripMenuItem2.Text = "刪除距離標示";
            toolStripMenuItem2.Visible = true;

            Right_Temp_Dist = SelectedDist;
            Right_Temp_Mouse_Pos = e;
            contextMenuStrip1.Show(MousePosition);
        }
        private void Right_click_Group(Point e)
        {
            if (SelectedGroup.L.Count == 1 && SelectedGroup.C.Count == 0 && SelectedGroup.A.Count == 0)
            {
                Right_Temp_Line = SelectedGroup.L[0];
                SelectedLine = SelectedGroup.L[0];
                Right_click_Line(e);
            }
            else if (SelectedGroup.L.Count == 0 && SelectedGroup.C.Count == 1 && SelectedGroup.A.Count == 0)
            {
                Right_Temp_Curve = SelectedGroup.C[0];
                SelectedCurve = SelectedGroup.C[0];
                Right_click_Curve(e);
            }
            else if (SelectedGroup.L.Count == 0 && SelectedGroup.C.Count == 0 && SelectedGroup.A.Count == 1)
            {
                Right_Temp_Arc = SelectedGroup.A[0];
                SelectedArc = SelectedGroup.A[0];
                Right_click_Arc(e);
            }
            else
            {
                toolStripMenuItem2.Text = "刪除選取範圍";
                toolStripMenuItem2.Visible = true;

                群組ToolStripMenuItem.Visible = true;
                bool inGroup = GroupList.FindIndex(x => x == SelectedGroup) >= 0;
                組成群組ToolStripMenuItem.Enabled = !inGroup;
                取消群組ToolStripMenuItem.Enabled = inGroup;
                bool tb;
                if (TestPath(SelectedGroup, out tb) != null)
                {
                    圖形ToolStripMenuItem.Visible = true;
                    圖形ToolStripMenuItem.Enabled = true;
                    組為圖形ToolStripMenuItem.Enabled = true;
                    解除圖形ToolStripMenuItem.Enabled = false;
                }
                else
                {
                    圖形ToolStripMenuItem.Visible = true;
                    圖形ToolStripMenuItem.Enabled = false;
                    組為圖形ToolStripMenuItem.Enabled = false;
                    解除圖形ToolStripMenuItem.Enabled = false;
                }


                Right_Temp_Group = SelectedGroup;
                Right_Temp_Mouse_Pos = e;
                contextMenuStrip1.Show(MousePosition);
            }
        }
        private void Right_click_SupPoint(Point e)
        {
            toolStripMenuItem2.Text = "刪除輔助線節點";
            toolStripMenuItem2.Visible = true;
            
            Right_Temp_SupPoint = SelectedSupPoint;
            Right_Temp_Mouse_Pos = e;
            contextMenuStrip1.Show(MousePosition);
        }
        private void Right_click_SupLine(Point e)
        {
            toolStripMenuItem2.Text = "刪除輔助線";
            toolStripMenuItem2.Visible = true;
            
            變更顏色ToolStripMenuItem.Visible = true;
            上下左右移動ToolStripMenuItem.Visible = true;

            直線等分ToolStripMenuItem.Visible = true;
            if (!SupPointsList.Exists(x => x.MarkL == SelectedSupLine))
                直線等分ToolStripMenuItem.Text = "直線等分";
            else
                直線等分ToolStripMenuItem.Text = "取消等分標記";

            Right_Temp_Mouse_Pos = e;
            Right_Temp_SupLine = SelectedSupLine;
            contextMenuStrip1.Show(MousePosition);
        }
        private void ContextMenusStrip1_invisible()
        {
            foreach (ToolStripMenuItem temp in contextMenuStrip1.Items)
                temp.Visible = false;
            /*
            toolStripMenuItem2.Visible = false;
            新增節點ToolStripMenuItem.Visible = false;
            曲線樣式ToolStripMenuItem.Visible = false;
            群組ToolStripMenuItem.Visible = false;
            轉換為曲線ToolStripMenuItem.Visible = false;
            圖形ToolStripMenuItem.Visible = false;
            直線等分ToolStripMenuItem.Visible = false;
            選取整個圖形ToolStripMenuItem.Visible = false;
            移除距離標示ToolStripMenuItem.Visible = false;
            變更顏色ToolStripMenuItem.Visible = false;
            上下左右移動ToolStripMenuItem.Visible = false;*/
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
                        for (int i = 0; i < c.path.Count; i++)
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
                    foreach (var a in g.A)
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
        private void Click_To_Select_Point()
        {
            if (Moving.type == Moving_Type.Point)
            {
                SelectedGroup = new GraphGroup();
                SelectedGroup.P.Add(Moving.StartLinePoint);
            }
        }
        private void Click_To_Select_Line()
        {
            if (Ctrl && SelectedGroup != null)
            {
                if (SupLineList.Exists(x => x == Moving.Line))
                    return;
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
                            foreach (var a in g.A)
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
                if (SupLineList.Exists(x => x == Moving.Line))
                    return;
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
                            foreach (var a in g.A)
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
                            foreach (var a in g.A)
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
                            foreach (var a in g.A)
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
                    foreach (var a in Moving.Group.A)
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
            M8_TempA = null;
            M8_is_SupL1 = false;
            M8_is_SupL2 = false;
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
            foreach (var p in PointsList.FindAll(x => x.MarkL == Moving.Line))
            {
                p.P.X = (ep.P.X - sp.P.X) * p.part + sp.P.X;
                p.P.Y = (ep.P.Y - sp.P.Y) * p.part + sp.P.Y;
            }
            foreach (var p in SupPointsList.FindAll(x => x.MarkL == Moving.Line))
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
            foreach (var l in SupLineList.FindAll(x => x.StartPoint == Moving.StartLinePoint || x.EndPoint == Moving.StartLinePoint))
            {
                foreach (var p in SupPointsList.FindAll(x => x.MarkL == l))
                {
                    p.P.X = (l.EndPoint.P.X - l.StartPoint.P.X) * p.part + l.StartPoint.P.X;
                    p.P.Y = (l.EndPoint.P.Y - l.StartPoint.P.Y) * p.part + l.StartPoint.P.Y;
                }
            }
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
                float x = e.X - Moving.Curve.path[Moving.CurveIndex].P.X;
                float y = e.Y - Moving.Curve.path[Moving.CurveIndex].P.Y;
                Moving.Curve.disFirst[Moving.CurveIndex] = new PointF(x, y);
                if (Moving.Curve.type[Moving.CurveIndex] == 0)
                {
                    Moving.Curve.disSecond[Moving.CurveIndex] = new PointF(-1 * x, -1 * y);
                }
                else if (Moving.Curve.type[Moving.CurveIndex] == 1)
                {
                    float newl = (float)Math.Sqrt(x * x + y * y);
                    PointF t = Moving.Curve.disSecond[Moving.CurveIndex];
                    float oldl = (float)Math.Sqrt(t.X * t.X + t.Y * t.Y);
                    Moving.Curve.disSecond[Moving.CurveIndex] = new PointF(-1 * x * oldl / newl, -1 * y * oldl / newl);
                }
            }
            else
            {
                float x = e.X - Moving.Curve.path[Moving.CurveIndex].P.X;
                float y = e.Y - Moving.Curve.path[Moving.CurveIndex].P.Y;
                Moving.Curve.disSecond[Moving.CurveIndex] = new PointF(x, y);
                if (Moving.Curve.type[Moving.CurveIndex] == 0)
                {
                    Moving.Curve.disFirst[Moving.CurveIndex] = new PointF(-1 * x, -1 * y);
                }
                else if (Moving.Curve.type[Moving.CurveIndex] == 1)
                {
                    float newl = (float)Math.Sqrt(x * x + y * y);
                    PointF t = Moving.Curve.disFirst[Moving.CurveIndex];
                    float oldl = (float)Math.Sqrt(t.X * t.X + t.Y * t.Y);
                    Moving.Curve.disFirst[Moving.CurveIndex] = new PointF(-1 * x * oldl / newl, -1 * y * oldl / newl);
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
            foreach (var l in Moving.Group.L)
            {
                foreach (var p in PointsList.FindAll(x => x.MarkL == l))
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
        private void 水平距離ToolStripMenuItem_Click(object sender, EventArgs e)//水平距離
        {
            Mouse_Mode = Mouse_Mode_Type.Dist_Hori;
            is_Drowing = false;
            previous_point = null;
            PreviousCruve = new GraphCurve();
            Cursor = Cursors.Default;
            Refresh_toolStripButton_Checked(Mouse_Mode_Type.Dist_Hori);
            Refresh();
        }
        private void 垂直距離ToolStripMenuItem_Click(object sender, EventArgs e)//垂直距離
        {
            Mouse_Mode = Mouse_Mode_Type.Dist_Verti;
            is_Drowing = false;
            previous_point = null;
            PreviousCruve = new GraphCurve();
            Cursor = Cursors.Default;
            Refresh_toolStripButton_Checked(Mouse_Mode_Type.Dist_Verti);
            Refresh();
        }
        private void 任意角度兩線距離ToolStripMenuItem_Click(object sender, EventArgs e) //兩線距離
        {

            Mouse_Mode = Mouse_Mode_Type.Dist_2points;
            is_Drowing = false;
            previous_point = null;
            PreviousCruve = new GraphCurve();
            Cursor = Cursors.Default;
            Refresh_toolStripButton_Checked(Mouse_Mode_Type.Dist_2points);
            Refresh();
        }
        private void 垂直於一線的兩線距離ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Mouse_Mode = Mouse_Mode_Type.Dist_RightLine;
            is_Drowing = false;
            previous_point = null;
            PreviousCruve = new GraphCurve();
            Cursor = Cursors.Default;
            Refresh_toolStripButton_Checked(Mouse_Mode_Type.Dist_RightLine);
            Refresh();
        }
        private void toolStripButton17_Click(object sender, EventArgs e) //輔助線
        {
            Mouse_Mode = Mouse_Mode_Type.SupLine;
            previous_point = null;
            SelectedGroup = null;
            PreviousCruve = new GraphCurve();
            Cursor = Cursors.Cross;
            Refresh_toolStripButton_Checked(Mouse_Mode_Type.SupLine);
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
            水平距離ToolStripMenuItem.Checked = false;
            垂直距離ToolStripMenuItem.Checked = false;
            任意角度兩線距離ToolStripMenuItem.Checked = false;
            垂直於一線的兩線距離ToolStripMenuItem.Checked = false;
            toolStripButton17.Checked = false;
            M6_Temp_Clean();
            M7_Temp_Clean();
            switch (a)
            {
                case Mouse_Mode_Type.Cursor: toolStripButton1.Checked = true; break;
                case Mouse_Mode_Type.Line: toolStripButton2.Checked = true; break;
                case Mouse_Mode_Type.Curve: toolStripButton3.Checked = true; break;
                case Mouse_Mode_Type.Arc: toolStripButton7.Checked = true; break;
                case Mouse_Mode_Type.Text: toolStripButton11.Checked = true; break;
                case Mouse_Mode_Type.Pleated: toolStripButton12.Checked = true; break;
                case Mouse_Mode_Type.Cutting: toolStripButton13.Checked = true; break;
                case Mouse_Mode_Type.Dist_Hori: 水平距離ToolStripMenuItem.Checked = true; break;
                case Mouse_Mode_Type.Dist_Verti: 垂直距離ToolStripMenuItem.Checked = true; break;
                case Mouse_Mode_Type.Dist_2points: 任意角度兩線距離ToolStripMenuItem.Checked = true; break;
                case Mouse_Mode_Type.Dist_RightLine: 垂直於一線的兩線距離ToolStripMenuItem.Checked = true; break;
                case Mouse_Mode_Type.SupLine: toolStripButton17.Checked = true; break;
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
            TabpageData t = TabPageData_Copy(Undo_Data[Undo_Index]);
            /*
            for (int i = 0; i < Undo_Data[Undo_Index].PointL.Count; i++)
            {
                if (Undo_Data[Undo_Index].PointL[i].MarkL != null)
                {
                    t.PointL[i].MarkL = t.LineL[Undo_Data[Undo_Index].LineL.FindIndex(x => x == Undo_Data[Undo_Index].PointL[i].MarkL)];
                    t.PointL[i].part = Undo_Data[Undo_Index].PointL[i].part;
                }
            }
            */
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
            TabpageData t = TabPageData_Copy(Undo_Data[Undo_Index]);

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
            if (ZoomSize < 5)
                ZoomSize += 0.1f;
            pictureBox1.Image = new Bitmap((int)(TabpageDataList[tabControl1.SelectedIndex].width * ZoomSize), (int)(TabpageDataList[tabControl1.SelectedIndex].height * ZoomSize));
            pictureBox2.Image = new Bitmap((int)(pictureBox1.Width) + 20, (int)(pictureBox1.Height) + 20);
            toolStripStatusLabel3.Text = "縮放大小:" + Math.Round(ZoomSize * 100) + "%";
            Form1_SizeChanged(new object(), new EventArgs());
            Refresh();
        }
        private void toolStripButton9_Click(object sender, EventArgs e)//縮小 
        {
            if (ZoomSize > 0.3F)
                ZoomSize -= 0.1f;
            pictureBox1.Image = new Bitmap((int)(TabpageDataList[tabControl1.SelectedIndex].width * ZoomSize), (int)(TabpageDataList[tabControl1.SelectedIndex].height * ZoomSize));
            pictureBox2.Image = new Bitmap((int)(pictureBox1.Width) + 20, (int)(pictureBox1.Height) + 20);
            toolStripStatusLabel3.Text = "縮放大小:" + Math.Round(ZoomSize * 100) + "%";
            Form1_SizeChanged(new object(), new EventArgs());
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
        private void toolStripButton15_Click(object sender, EventArgs e)//鎖定輔助線
        {
            if (SupLine_Lock)
            {
                toolStripButton15.Checked = false;
                SupLine_Lock = false;
            }
            else
            {
                toolStripButton15.Checked = true;
                SupLine_Lock = true;
            }
            Refresh();
        }
        private void toolStripButton16_Click(object sender, EventArgs e)//隱藏輔助線
        {
            if (SupLine_Visible)
            {
                toolStripButton16.Checked = false;
                toolStripButton17.Enabled = false;
                if (toolStripButton17.Checked)
                {
                    toolStripButton1_Click(new object(), new EventArgs());
                }
                SupLine_Visible = false;
            }
            else
            {
                toolStripButton16.Checked = true;
                toolStripButton17.Enabled = true;
                SupLine_Visible = true;
            }
            Refresh();
        }
        private void Object_Name_Visible_toolStripButton_Click(object sender, EventArgs e)//顯示/隱藏名稱
        {
            if (Object_Name_Visible)
            {
                Object_Name_Visible_toolStripButton.Checked = false;
                Object_Name_Visible = false;
            }
            else
            {
                Object_Name_Visible_toolStripButton.Checked = true;
                Object_Name_Visible = true;
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
            if (int.TryParse(toolStripTextBox2.Text, out t) && t > 0)
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

        int Edit_FL_Temp_FML_oblistview_index = -1;
        int Edit_FL_Temp_FTL_Priority_oblistview_index = -1;
        bool onUpdateProperty = false;
        private List<TabpageData> Undo_Data = new List<TabpageData>();
        private void Push_Undo_Data(int tdindex = -1)
        {
            TabpageData temp = new TabpageData();
            temp.CurveL = CurveList;
            temp.PointL = PointsList;
            temp.LineL = LineList;
            temp.GroupL = new List<GraphGroup>();
            temp.ArcL = ArcList;
            temp.TextL = TextList;
            temp.PathL = new List<GraphGroup>();
            temp.PDistL = PDistList;
            temp.SupPointL = SupPointsList;
            temp.SupLineL = SupLineList;
            temp.FtoLL = FormulaToLineList;
            temp.ClothGrainMark = ClothGrainMark;
            TabpageData t = TabPageData_Copy(temp);

            foreach (var g in GroupList)
            {
                if (tdindex == -1)
                    t.GroupL.Add(Group_Copy(g, t, TabpageDataList[tabControl1.SelectedIndex]));
                else
                    t.GroupL.Add(Group_Copy(g, t, TabpageDataList[tdindex]));
            }
            foreach (var path in PathList)
            {
                if (tdindex == -1)
                    t.PathL.Add(Group_Copy(path, t, TabpageDataList[tabControl1.SelectedIndex]));
                else
                    t.PathL.Add(Group_Copy(path, t, TabpageDataList[tdindex]));
            }
            t.width = (int)(pictureBox1.Width / ZoomSize);
            t.height = (int)(pictureBox1.Height / ZoomSize);
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
        private TabpageData TabPageData_Copy(TabpageData target)
        {
            TabpageData t = new TabpageData();
            foreach (var p in target.PointL)
            {
                GraphPoint gp = new GraphPoint(p.P.X, p.P.Y);
                t.PointL.Add(gp);
            }
            foreach (var p in target.SupPointL)
            {
                GraphPoint gp = new GraphPoint(p.P.X, p.P.Y);
                t.SupPointL.Add(gp);
            }
            foreach (var l in target.LineL)
            {
                int s = target.PointL.FindIndex(x => x == l.StartPoint);
                int e = target.PointL.FindIndex(x => x == l.EndPoint);
                t.PointL[s].Relative++;
                t.PointL[e].Relative++;
                GraphLine tl = new GraphLine(t.PointL[s], t.PointL[e]);
                tl.Seam = l.Seam;
                tl.SeamText = l.SeamText;
                tl.isSeam = l.isSeam;
                tl.co = l.co;
                tl.dash = l.dash;
                tl.fix = l.fix;
                tl.name = l.name;
                t.LineL.Add(tl);
            }
            foreach (var l in target.SupLineL)
            {
                int s = target.SupPointL.FindIndex(x => x == l.StartPoint);
                int e = target.SupPointL.FindIndex(x => x == l.EndPoint);
                t.SupPointL[s].Relative++;
                t.SupPointL[e].Relative++;
                GraphLine tl = new GraphLine(t.SupPointL[s], t.SupPointL[e]);
                tl.Seam = l.Seam;
                tl.SeamText = l.SeamText;
                tl.isSeam = l.isSeam;
                tl.co = l.co;
                tl.dash = l.dash;
                t.SupLineL.Add(tl);
            }
            foreach (var c in target.CurveL)
            {
                GraphCurve gc = new GraphCurve();
                for (int i = 0; i < c.path.Count; i++)
                {
                    int index = target.PointL.FindIndex(x => x == c.path[i]);
                    gc.path.Add(t.PointL[index]);
                    t.PointL[index].Relative++;
                    gc.disFirst.Add(new PointF(c.disFirst[i].X, c.disFirst[i].Y));
                    gc.disSecond.Add(new PointF(c.disSecond[i].X, c.disSecond[i].Y));
                    gc.type.Add(c.type[i]);
                }
                gc.isSeam = c.isSeam;
                gc.Seam = c.Seam;
                gc.co = c.co;
                gc.dash = c.dash;
                gc.fix = c.fix;
                gc.name = c.name;
                t.CurveL.Add(gc);
            }
            foreach (var a in target.ArcL)
            {
                int s = target.PointL.FindIndex(x => x == a.StartPoint);
                int e = target.PointL.FindIndex(x => x == a.EndPoint);
                t.PointL[s].Relative++;
                t.PointL[e].Relative++;
                GraphArc ta = new GraphArc(t.PointL[s], t.PointL[e], a.getControlPoint());
                ta.co = a.co;
                ta.dash = a.dash;
                ta.name = a.name;
                t.ArcL.Add(ta);
            }
            for (int i = 0; i < target.PointL.Count; i++)
            {
                if (target.PointL[i].MarkL != null)
                {
                    t.PointL[i].MarkL = t.LineL[target.LineL.FindIndex(x => x == target.PointL[i].MarkL)];
                    t.PointL[i].part = target.PointL[i].part;
                }
            }
            for (int i = 0; i < target.SupPointL.Count; i++)
            {
                if (target.SupPointL[i].MarkL != null)
                {
                    t.SupPointL[i].MarkL = t.SupLineL[target.SupLineL.FindIndex(x => x == target.SupPointL[i].MarkL)];
                    t.SupPointL[i].part = target.SupPointL[i].part;
                }
            }
            foreach (var g in target.GroupL)
            {
                t.GroupL.Add(Group_Copy(g, t, target));
            }
            foreach (var path in target.PathL)
            {
                t.PathL.Add(Group_Copy(path, t, target));
            }
            foreach (var s in target.TextL)
            {
                t.TextL.Add(new GraphText() { S = s.S, P = s.P, straight = s.straight });
            }
            foreach (var pd in target.PDistL)
            {
                PathDistance tpd = null;
                #region
                if (pd.Is_HV == true)
                {
                    switch (pd.type)
                    {
                        case 0:
                            if (pd.is_SupL1 && pd.is_SupL2)
                                tpd = new PathDistance(t.SupLineL[target.SupLineL.FindIndex(x => x == pd.L1)],
                                                       t.SupLineL[target.SupLineL.FindIndex(x => x == pd.L2)],
                                                       pd.XY_Pos, pd.T_hori_F_verti);
                            else if (pd.is_SupL1)
                                tpd = new PathDistance(t.SupLineL[target.SupLineL.FindIndex(x => x == pd.L1)],
                                                       t.LineL[target.LineL.FindIndex(x => x == pd.L2)],
                                                       pd.XY_Pos, pd.T_hori_F_verti);
                            else if (pd.is_SupL2)
                                tpd = new PathDistance(t.LineL[target.LineL.FindIndex(x => x == pd.L1)],
                                                       t.SupLineL[target.SupLineL.FindIndex(x => x == pd.L2)],
                                                       pd.XY_Pos, pd.T_hori_F_verti);
                            else
                                tpd = new PathDistance(t.LineL[target.LineL.FindIndex(x => x == pd.L1)],
                                                       t.LineL[target.LineL.FindIndex(x => x == pd.L2)],
                                                       pd.XY_Pos, pd.T_hori_F_verti);
                            break;
                        case 1:
                            if (pd.is_SupL1)
                                tpd = new PathDistance(t.SupLineL[target.SupLineL.FindIndex(x => x == pd.L1)],
                                                       t.CurveL[target.CurveL.FindIndex(x => x == pd.C1)], pd.cindex1,
                                                       pd.XY_Pos, pd.T_hori_F_verti);
                            else
                                tpd = new PathDistance(t.LineL[target.LineL.FindIndex(x => x == pd.L1)],
                                                       t.CurveL[target.CurveL.FindIndex(x => x == pd.C1)], pd.cindex1,
                                                       pd.XY_Pos, pd.T_hori_F_verti);
                            break;
                        case 2:
                            tpd = new PathDistance(t.CurveL[target.CurveL.FindIndex(x => x == pd.C1)], pd.cindex1,
                                                  t.CurveL[target.CurveL.FindIndex(x => x == pd.C2)], pd.cindex2,
                                                  pd.XY_Pos, pd.T_hori_F_verti);
                            break;
                        case 3:
                            if (pd.is_SupL1)
                                tpd = new PathDistance(t.SupLineL[target.SupLineL.FindIndex(x => x == pd.L1)],
                                                       t.ArcL[target.ArcL.FindIndex(x => x == pd.A1)],
                                                       pd.XY_Pos, pd.T_hori_F_verti);
                            else
                                tpd = new PathDistance(t.LineL[target.LineL.FindIndex(x => x == pd.L1)],
                                                       t.ArcL[target.ArcL.FindIndex(x => x == pd.A1)],
                                                       pd.XY_Pos, pd.T_hori_F_verti);
                            break;
                        case 4:
                            tpd = new PathDistance(t.CurveL[target.CurveL.FindIndex(x => x == pd.C1)], pd.cindex1,
                                                  t.ArcL[target.ArcL.FindIndex(x => x == pd.A1)],
                                                  pd.XY_Pos, pd.T_hori_F_verti);
                            break;
                        case 5:
                            tpd = new PathDistance(t.ArcL[target.ArcL.FindIndex(x => x == pd.A1)],
                                                  t.ArcL[target.ArcL.FindIndex(x => x == pd.A2)],
                                                  pd.XY_Pos, pd.T_hori_F_verti);
                            break;
                    }
                }
                else
                {
                    switch (pd.type)
                    {
                        case 0:
                            if(pd.is_SupL1 && pd.is_SupL2)
                                tpd = new PathDistance(t.SupLineL[target.SupLineL.FindIndex(x => x == pd.L1)],
                                                       t.SupLineL[target.SupLineL.FindIndex(x => x == pd.L2)],
                                                       pd.anchor1, pd.anchor2);
                            else if (pd.is_SupL1)
                                tpd = new PathDistance(t.SupLineL[target.SupLineL.FindIndex(x => x == pd.L1)],
                                                       t.LineL[target.LineL.FindIndex(x => x == pd.L2)],
                                                       pd.anchor1, pd.anchor2);
                            else if(pd.is_SupL2)
                                tpd = new PathDistance(t.LineL[target.LineL.FindIndex(x => x == pd.L1)],
                                                       t.SupLineL[target.SupLineL.FindIndex(x => x == pd.L2)],
                                                       pd.anchor1, pd.anchor2);
                            else
                                tpd = new PathDistance(t.LineL[target.LineL.FindIndex(x => x == pd.L1)],
                                                       t.LineL[target.LineL.FindIndex(x => x == pd.L2)],
                                                       pd.anchor1, pd.anchor2);
                            break;
                        case 1:
                            if(pd.is_SupL1)
                                tpd = new PathDistance(t.SupLineL[target.SupLineL.FindIndex(x => x == pd.L1)],
                                                       t.CurveL[target.CurveL.FindIndex(x => x == pd.C1)], pd.cindex1,
                                                       pd.anchor1, pd.anchor2);
                            else
                                tpd = new PathDistance(t.LineL[target.LineL.FindIndex(x => x == pd.L1)],
                                                       t.CurveL[target.CurveL.FindIndex(x => x == pd.C1)], pd.cindex1,
                                                       pd.anchor1, pd.anchor2);
                            break;
                        case 2:
                            tpd = new PathDistance(t.CurveL[target.CurveL.FindIndex(x => x == pd.C1)], pd.cindex1,
                                                  t.CurveL[target.CurveL.FindIndex(x => x == pd.C2)], pd.cindex2,
                                                  pd.anchor1, pd.anchor2);
                            break;
                        case 3:
                            if(pd.is_SupL1)
                                tpd = new PathDistance(t.SupLineL[target.SupLineL.FindIndex(x => x == pd.L1)],
                                                       t.ArcL[target.ArcL.FindIndex(x => x == pd.A1)],
                                                       pd.anchor1, pd.anchor2);
                            else
                                tpd = new PathDistance(t.LineL[target.LineL.FindIndex(x => x == pd.L1)],
                                                       t.ArcL[target.ArcL.FindIndex(x => x == pd.A1)],
                                                       pd.anchor1, pd.anchor2);
                            break;
                        case 4:
                            tpd = new PathDistance(t.CurveL[target.CurveL.FindIndex(x => x == pd.C1)], pd.cindex1,
                                                  t.ArcL[target.ArcL.FindIndex(x => x == pd.A1)],
                                                  pd.anchor1, pd.anchor2);
                            break;
                        case 5:
                            tpd = new PathDistance(t.ArcL[target.ArcL.FindIndex(x => x == pd.A1)],
                                                  t.ArcL[target.ArcL.FindIndex(x => x == pd.A2)],
                                                  pd.anchor1, pd.anchor2);
                            break;
                    }
                }
                tpd.is_SupL1 = pd.is_SupL1;
                tpd.is_SupL2 = pd.is_SupL2;
                #endregion
                t.PDistL.Add(tpd);
            }
            foreach (var ftl in target.FtoLL)
            {
                FormulaToLine tempf = null;
                List<Element> elel = new List<Element>();
                foreach(var ele in ftl.formula_eleL)
                {
                    if(ele.GetType() == typeof(NumberElement))
                    {
                        elel.Add(new NumberElement(ele.ToString()));
                    }
                    else if (ele.GetType() == typeof(VariableElement))
                    {
                        VariableElement vele = (VariableElement)ele;
                        elel.Add(new VariableElement(vele.getIndex()));
                    }
                    else if (ele.GetType() == typeof(OperatorElement))
                    {
                        elel.Add(new OperatorElement(ele.ToString()[0]));
                    }
                }
                switch (ftl.type)
                {
                    case 0:
                        tempf = new FormulaToLine(t.LineL[target.LineL.FindIndex(x => x == ftl.L1)],
                                                   t.LineL[target.LineL.FindIndex(x => x == ftl.L2)],
                                                   ftl.anchor1, ftl.anchor2, ftl.mode, ftl.path_type);
                        tempf.labled1 = Group_Copy(ftl.labled1, t, target);
                        tempf.labled2 = Group_Copy(ftl.labled2, t, target);
                        tempf.fml = ftl.fml;
                        break;
                    case 1:
                        tempf = new FormulaToLine(t.LineL[target.LineL.FindIndex(x => x == ftl.L1)],
                                                   t.CurveL[target.CurveL.FindIndex(x => x == ftl.C1)], ftl.cindex1,
                                                   ftl.anchor1, ftl.anchor2, ftl.mode, ftl.path_type);
                        tempf.labled1 = Group_Copy(ftl.labled1, t, target);
                        tempf.labled2 = Group_Copy(ftl.labled2, t, target);
                        tempf.fml = ftl.fml;
                        break;
                    case 2:
                        tempf = new FormulaToLine(t.CurveL[target.CurveL.FindIndex(x => x == ftl.C1)], ftl.cindex1,
                                              t.CurveL[target.CurveL.FindIndex(x => x == ftl.C2)], ftl.cindex2,
                                              ftl.anchor1, ftl.anchor2, ftl.mode, ftl.path_type);
                        tempf.labled1 = Group_Copy(ftl.labled1, t, target);
                        tempf.labled2 = Group_Copy(ftl.labled2, t, target);
                        tempf.fml = ftl.fml;
                        break;
                    case 3:
                        tempf = new FormulaToLine(t.PointL[target.PointL.FindIndex(x=>x==ftl.unfixed_P1)],
                                                t.LineL[target.LineL.FindIndex(x => x == ftl.L1)],
                                                ftl.anchor1, ftl.anchor2, ftl.mode);
                        tempf.fml = ftl.fml;
                        break;
                    case 4:
                        tempf = new FormulaToLine(t.PointL[target.PointL.FindIndex(x => x == ftl.unfixed_P1)],
                                                t.CurveL[target.CurveL.FindIndex(x => x == ftl.C1)], ftl.cindex1,
                                                ftl.anchor1, ftl.anchor2, ftl.mode);
                        tempf.fml = ftl.fml;
                        break;
                    case 5:
                        tempf = new FormulaToLine(t.PointL[target.PointL.FindIndex(x => x == ftl.unfixed_P1)],
                                                t.PointL[target.PointL.FindIndex(x => x == ftl.unfixed_P2)],
                                                ftl.anchor1, ftl.anchor2, ftl.mode);
                        tempf.fml = ftl.fml;
                        tempf.formula_eleL = ftl.formula_eleL;
                        break;
                }
                tempf.formula_eleL = elel;
                if (tempf != null)
                    t.FtoLL.Add(tempf);
            }
            PointF tpf = new PointF(target.ClothGrainMark.Loc.X, target.ClothGrainMark.Loc.Y);
            t.ClothGrainMark = new GrainMark()
            {
                visible = target.ClothGrainMark.visible,
                loc_type = target.ClothGrainMark.loc_type,
                dir_type = target.ClothGrainMark.dir_type,
                Loc = tpf,
                Dir = target.ClothGrainMark.Dir,
                size = target.ClothGrainMark.size
            };
            return t;
        }
        private GraphGroup Group_Copy(GraphGroup PreG, TabpageData PreT, TabpageData Ref)
        {
            GraphGroup gg = new GraphGroup();
            foreach (var p in PreG.P)
            {
                int a = Ref.PointL.FindIndex(x => x == p);
                gg.P.Add(PreT.PointL[a]);
            }
            foreach (var l in PreG.L)
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
            foreach (var c in PreG.C)
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
            foreach (var a in PreG.A)
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
            GraphText selectedtext = FindTextByPoint(TextList, point);
            PathDistance selecteddist = FindDistByPoint(PDistList, point);
            GraphPoint selectedsuppoint = FindPointByPoint(SupPointsList, point);
            GraphLine selectedsupline = FindLineByPoint(SupLineList, point);
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
            else if (selectarc != this.SelectedArc && Mouse_Mode == Mouse_Mode_Type.Cursor)
            {
                this.SelectedArc = selectarc;
                this.Invalidate();
            }
            else if (selectarccontrol != this.SelectedArcControl && Mouse_Mode == Mouse_Mode_Type.Cursor)
            {
                this.SelectedArcControl = selectarccontrol;
                this.Invalidate();
            }
            else if (selectedgroupcontrol != SelectedGroupControl && Mouse_Mode == Mouse_Mode_Type.Cursor)
            {
                this.SelectedGroupControl = selectedgroupcontrol;
                this.Invalidate();
            }
            else if (selectedtext != SelectedText && Mouse_Mode == Mouse_Mode_Type.Cursor)
            {
                this.SelectedText = selectedtext;
                this.Invalidate();
            }
            else if (selecteddist != SelectedDist && Mouse_Mode == Mouse_Mode_Type.Cursor)
            {
                this.SelectedDist = selecteddist;
                this.Invalidate();
            }
            else if (selectedsuppoint != SelectedSupPoint && Mouse_Mode == Mouse_Mode_Type.Cursor && SupLine_Lock == false && SupLine_Visible == true)
            {
                SelectedSupPoint = selectedsuppoint;
                Invalidate();
            }
            else if (selectedsupline != SelectedSupLine && Mouse_Mode == Mouse_Mode_Type.Cursor && SupLine_Lock == false && SupLine_Visible == true)
            {
                SelectedSupLine = selectedsupline;
                Invalidate();
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
                Mouse_Mode == Mouse_Mode_Type.Line || Mouse_Mode == Mouse_Mode_Type.Curve ||
                Mouse_Mode == Mouse_Mode_Type.Arc || Mouse_Mode == Mouse_Mode_Type.Pleated || Mouse_Mode == Mouse_Mode_Type.SupLine ? Cursors.Cross :
                SelectedGroupControl == 0 || SelectedGroupControl == 3 ? Cursors.SizeNWSE :
                SelectedGroupControl == 1 || SelectedGroupControl == 2 ? Cursors.SizeNESW :
                SelectedGroupControl == 4 ? Cursors.SizeAll :
                    Cursors.Default;
        }
        private void RefreshPorperty()
        {
            onUpdateProperty = true;
            if (EditFL == true)
            {
                //objectListView1.Focus();
                objectListView1.Items.Clear();
                objectListView1.BeginUpdate();
                List<Formula_obListItem> templ = new List<Formula_obListItem>();
                double ans = 0;
                for (int i = 0; i < FormulaList.Count; i++)
                {
                    string s = "";
                    foreach (var ele in FormulaList[i].EleL)
                    {
                        if (ele.GetType().Equals(typeof(VariableElement)))
                        {
                            VariableElement vele = (VariableElement)ele;
                            s += vele.getName();
                        }
                        else
                        {
                            s += ele.ToString() + " ";
                        }
                    }
                    if (FormulaList[i].EleL.Count != 0)
                    {
                        var element_comvert = new List<Element>();
                        foreach (var ele in FormulaList[i].EleL)
                        {
                            if (ele.GetType().Equals(typeof(VariableElement)))
                            {
                                VariableElement vele = (VariableElement)ele;
                                int index = vele.getIndex();
                                double v = ClothStandardSize[index - 1];
                                element_comvert.Add(new NumberElement(v.ToString()));
                            }
                            else
                            {
                                element_comvert.Add(ele);
                            }
                        }
                        InfixToPostfix itp = new InfixToPostfix();
                        var PFele = itp.ConvertFromInfixToPostFix(element_comvert);
                        PostFixEvaluator pfe = new PostFixEvaluator();
                        ans = pfe.Evaluate(PFele);
                    }
                    Formula_obListItem l;
                    if (FormulaList[i].EleL.Count == 0)
                    {
                        l = new Formula_obListItem(FormulaList[i].name, s, "");
                    }
                    else
                    {
                        l = new Formula_obListItem(FormulaList[i].name, s, ans.ToString());
                    }
                    templ.Add(l);
                }
                objectListView1.SetObjects(templ);
                objectListView1.EndUpdate();
                if (Edit_FL_Temp_FML_oblistview_index != -1 && Edit_FL_Temp_FML_oblistview_index < objectListView1.Items.Count)
                    objectListView1.Items[Edit_FL_Temp_FML_oblistview_index].Selected = true;
                else
                    Edit_FL_Temp_FML_oblistview_index = -1;
                List<FTL_Priority_obListItem> templ2 = new List<FTL_Priority_obListItem>();
                for(int i = 0; i < FormulaToLineList.Count; i++)
                {
                    FTL_Priority_obListItem l = new FTL_Priority_obListItem((i + 1).ToString(), FormulaToLineList[i].name);
                    templ2.Add(l);
                }
                //FTL_Priority_oblistview.Focus();
                FTL_Priority_oblistview.Items.Clear();
                FTL_Priority_oblistview.BeginUpdate();
                FTL_Priority_oblistview.SetObjects(templ2);
                FTL_Priority_oblistview.EndUpdate();
                if (Edit_FL_Temp_FTL_Priority_oblistview_index != -1 && Edit_FL_Temp_FTL_Priority_oblistview_index < FTL_Priority_oblistview.Items.Count)
                    FTL_Priority_oblistview.Items[Edit_FL_Temp_FTL_Priority_oblistview_index].Selected = true;
                else
                    Edit_FL_Temp_FTL_Priority_oblistview_index = -1;

                if (Edit_FL_Temp_FTL_Priority_oblistview_index != -1)
                {
                    FormulaToLine ftl = FormulaToLineList[Edit_FL_Temp_FTL_Priority_oblistview_index];
                    FTL_Priority_Down_Button.Enabled = true;
                    FTL_Priority_Up_Button.Enabled = true;
                    FTL_Config_dataGridView.Enabled = true;
                    FTL_Priority_Down_Button.Visible = true;
                    FTL_Priority_Up_Button.Visible = true;
                    FTL_Config_dataGridView.Visible = true;

                    if (FTL_Config_dataGridView.Rows.Count == 0)
                    {
                        FTL_Config_dataGridView.Columns[0].DefaultCellStyle.BackColor = Color.LightGray;
                        FTL_Config_dataGridView.Rows.Add();
                        FTL_Config_dataGridView.Rows[FTL_Config_dataGridView.Rows.Count - 1].Cells[0].Value = "名稱";
                        FTL_Config_dataGridView.Rows.Add();
                        FTL_Config_dataGridView.Rows[FTL_Config_dataGridView.Rows.Count - 1].Cells[0].Value = "套用標籤";
                        FTL_Config_dataGridView.Rows[FTL_Config_dataGridView.Rows.Count - 1].Cells[1].ReadOnly = true;
                        FTL_Config_dataGridView.Rows[FTL_Config_dataGridView.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightGray;
                        FTL_Config_dataGridView.Rows.Add();
                        FTL_Config_dataGridView.Rows[FTL_Config_dataGridView.Rows.Count - 1].Cells[0].Value = "套用公式";
                        FTL_Config_dataGridView.Rows[FTL_Config_dataGridView.Rows.Count - 1].Cells[1].ReadOnly = true;
                        FTL_Config_dataGridView.Rows.Add();
                        FTL_Config_dataGridView.Rows[FTL_Config_dataGridView.Rows.Count - 1].Cells[0].Value = "套用中線條";
                        FTL_Config_dataGridView.Rows[FTL_Config_dataGridView.Rows.Count - 1].Cells[1].ReadOnly = true;
                        FTL_Config_dataGridView.Rows[FTL_Config_dataGridView.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightGray;
                        FTL_Config_dataGridView.Rows.Add();
                        FTL_Config_dataGridView.Rows[FTL_Config_dataGridView.Rows.Count - 1].Cells[0].Value = "移動比例";
                        FTL_Config_dataGridView.Rows.Add();
                        FTL_Config_dataGridView.Rows[FTL_Config_dataGridView.Rows.Count - 1].Cells[0].Value = "形式";
                        FTL_Config_dataGridView.Rows[FTL_Config_dataGridView.Rows.Count - 1].Cells[1].ReadOnly = true;
                        FTL_Config_dataGridView.Rows[FTL_Config_dataGridView.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    string lname = "";
                    if (ftl.type == 0)
                        lname += ftl.L1.name + " " + ftl.L2.name;
                    else if (ftl.type == 1)
                        lname += ftl.L1.name + " " + ftl.C1.name;
                    else if (ftl.type == 2)
                        lname += ftl.C1.name + " " + ftl.C2.name;
                    else if (ftl.type == 3)
                        lname += ftl.L1.name + " " + ftl.unfixed_P1.name;
                    else if (ftl.type == 4)
                        lname += ftl.C1.name + " " + ftl.unfixed_P1.name;
                    else if (ftl.type == 5)
                        lname += ftl.unfixed_P1.name + " " + ftl.unfixed_P2.name;

                    string s = "";
                    foreach (var ele in ftl.formula_eleL)
                    {
                        if (ele.GetType().Equals(typeof(VariableElement)))
                        {
                            VariableElement vele = (VariableElement)ele;
                            s += vele.getName();
                        }
                        else
                        {
                            s += ele.ToString() + " ";
                        }
                    }
                    FTL_Config_dataGridView.Rows[0].Cells[1].Value = ftl.name;
                    FTL_Config_dataGridView.Rows[1].Cells[1].Value = ftl.fml.name;
                    FTL_Config_dataGridView.Rows[2].Cells[1].Value = s;
                    FTL_Config_dataGridView.Rows[3].Cells[1].Value = lname;
                    FTL_Config_dataGridView.Rows[4].Cells[1].Value = ftl.prop12;
                    FTL_Config_dataGridView.Rows[5].Cells[1].Value = ftl.mode == 0 ? "平行移動" : "垂直移動";
                }
                else
                {
                    FTL_Priority_Down_Button.Enabled = false;
                    FTL_Priority_Up_Button.Enabled = false;
                    FTL_Config_dataGridView.Enabled = false;
                    FTL_Priority_Down_Button.Visible = false;
                    FTL_Priority_Up_Button.Visible = false;
                    FTL_Config_dataGridView.Visible = false;
                    FTL_Config_dataGridView.Rows.Clear();
                }

                PorpertyList[3].Visible = true;
                PorpertyList[0].Visible = false;
                PorpertyList[1].Visible = false;
                PorpertyList[2].Visible = false;
                PorpertyList[4].Visible = false;
            }
            else if (SelectedGroup != null)
            {
                if (SelectedGroup.C.Count + SelectedGroup.L.Count == 0 && SelectedGroup.P.Count == 1)
                {
                    GraphPoint p = SelectedGroup.P[0];
                    PorpertyList[0].Visible = false;
                    PorpertyList[1].Visible = false;
                    PorpertyList[2].Visible = false;
                    PorpertyList[3].Visible = false;
                    PorpertyList[4].Visible = true;
                    PointNameTextbox.Text = p.name;
                    DataGridView pc = Point_Config_Datagridview;
                    if (pc.Rows.Count == 0)
                    {
                        pc.Rows.Clear();
                        pc.Columns[0].DefaultCellStyle.BackColor = Color.LightGray;
                        pc.Rows.Add();
                        pc.Rows[pc.Rows.Count - 1].Cells[0].Value = "名稱";//0
                        pc.Rows.Add();
                        pc.Rows[pc.Rows.Count - 1].Cells[0].Value = "座標";//1
                        pc.Rows[pc.Rows.Count - 1].ReadOnly = true;
                        pc.Rows[pc.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightGray;
                        pc.Rows.Add();
                        pc.Rows[pc.Rows.Count - 1].Cells[0].Value = "相關線條";//2
                        pc.Rows[pc.Rows.Count - 1].ReadOnly = true;
                        pc.Rows[pc.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    pc.Rows[0].Cells[1].Value = p.name;
                    pc.Rows[1].Cells[1].Value = "X:" + p.P.X + " Y:" + p.P.Y;
                    string re = "";
                    foreach(var l in LineList)
                    {
                        if (l.StartPoint == p || l.EndPoint == p)
                            re += l.name + ",";
                    }
                    foreach (var c in CurveList)
                    {
                        if (c.path.Exists(x => x == p))
                            re += c.name + ",";
                    }
                    foreach (var a in ArcList)
                    {
                        if (a.StartPoint == p || a.EndPoint == p)
                            re += a.name + ",";
                    }
                    if (re.Length > 0)
                        re = re.Remove(re.Length - 1);
                    pc.Rows[2].Cells[1].Value = re;
                }
                else if (SelectedGroup.C.Count + SelectedGroup.L.Count == 1)
                {
                    if (SelectedGroup.L.Count == 1)
                    {
                        GraphLine l = SelectedGroup.L[0];
                        PorpertyList[0].Visible = true;
                        PorpertyList[1].Visible = false;
                        PorpertyList[2].Visible = false;
                        PorpertyList[3].Visible = false;
                        PorpertyList[4].Visible = false;
                        double x = (l.EndPoint.P.X - l.StartPoint.P.X) * (l.EndPoint.P.X - l.StartPoint.P.X);
                        double y = (l.EndPoint.P.Y - l.StartPoint.P.Y) * (l.EndPoint.P.Y - l.StartPoint.P.Y);
                        LineNameTextbox.Text = l.name;
                        LineLengthLable.Text = "長度:" + (Math.Sqrt(x + y) / (LenthUnit == 0 ? 72 / 2.54F : 72)).ToString("#0.00") + (LenthUnit == 0 ? " cm" : " inch");
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
                        DataGridView lc = Line_Config_Datagridview;
                        if (lc.Rows.Count == 0)
                        {
                            lc.Rows.Clear();
                            lc.Columns[0].DefaultCellStyle.BackColor = Color.LightGray;
                            lc.Rows.Add();
                            lc.Rows[lc.Rows.Count - 1].Cells[0].Value = "名稱";//0
                            lc.Rows.Add();
                            lc.Rows[lc.Rows.Count - 1].Cells[0].Value = "長度";//1
                            lc.Rows[lc.Rows.Count - 1].ReadOnly = true;
                            lc.Rows[lc.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightGray;
                            lc.Rows.Add();
                            lc.Rows[lc.Rows.Count - 1].Cells[0].Value = "顏色";//2
                            lc.Rows.Add();
                            lc.Rows[lc.Rows.Count - 1].Cells[0].Value = "固定/可動";//3
                            lc.Rows.Add();
                            lc.Rows[lc.Rows.Count - 1].Cells[0].Value = "縫分寬度";//4
                            lc.Rows.Add();
                            lc.Rows[lc.Rows.Count - 1].Cells[0].Value = "縫份長度";//5
                            lc.Rows.Add();
                            lc.Rows[lc.Rows.Count - 1].Cells[0].Value = "實/虛線";//6
                            lc.Rows.Add();
                            lc.Rows[lc.Rows.Count - 1].Cells[0].Value = "連接節點";//7
                            lc.Rows[lc.Rows.Count - 1].ReadOnly = true;
                            lc.Rows[lc.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightGray;
                        }
                        DataGridViewTextBoxCell temp_text_cell = new DataGridViewTextBoxCell();
                        temp_text_cell.Value = l.name;
                        lc.Rows[0].Cells[1] = temp_text_cell;

                        temp_text_cell = new DataGridViewTextBoxCell();
                        temp_text_cell.Value = (Math.Sqrt(x + y) / (LenthUnit == 0 ? 72 / 2.54F : 72)).ToString("#0.00") +
                                                                    (LenthUnit == 0 ? " cm" : " inch");
                        lc.Rows[1].Cells[1] = temp_text_cell;

                        temp_text_cell = new DataGridViewTextBoxCell();
                        temp_text_cell.Value = l.co.ToString();
                        lc.Rows[2].Cells[1] = temp_text_cell;

                        DataGridViewComboBoxCell temp_combo_cell = new DataGridViewComboBoxCell();
                        temp_combo_cell.Items.Add("固定");
                        temp_combo_cell.Items.Add("可動");
                        temp_combo_cell.Value = temp_combo_cell.Items[l.fix ? 1 : 0];
                        lc.Rows[3].Cells[1] = temp_combo_cell;

                        temp_combo_cell = new DataGridViewComboBoxCell();
                        temp_combo_cell.Items.Add("有");
                        temp_combo_cell.Items.Add("無");
                        temp_combo_cell.Value = temp_combo_cell.Items[l.isSeam ? 0 : 1];
                        lc.Rows[4].Cells[1] = temp_combo_cell;

                        temp_text_cell = new DataGridViewTextBoxCell();
                        temp_text_cell.Value = l.Seam;
                        lc.Rows[5].Cells[1] = temp_text_cell;

                        temp_combo_cell = new DataGridViewComboBoxCell();
                        temp_combo_cell.Items.Add("實");
                        temp_combo_cell.Items.Add("虛");
                        temp_combo_cell.Value = temp_combo_cell.Items[l.dash ? 1 : 0];
                        lc.Rows[6].Cells[1] = temp_combo_cell;

                        temp_text_cell = new DataGridViewTextBoxCell();
                        temp_text_cell.Value = l.StartPoint.name + " " + l.EndPoint.name;
                        lc.Rows[7].Cells[1] = temp_text_cell;
                    }
                    else if (SelectedGroup.C.Count == 1)
                    {
                        GraphCurve c = SelectedGroup.C[0];
                        PorpertyList[1].Visible = true;
                        PorpertyList[0].Visible = false;
                        PorpertyList[2].Visible = false;
                        PorpertyList[3].Visible = false;
                        PorpertyList[4].Visible = false;
                        CurveNameTextbox.Text = c.name;
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

                        DataGridView cc = Curve_Config_Datagridview;
                        if (cc.Rows.Count == 0)
                        {
                            cc.Rows.Clear();
                            cc.Columns[0].DefaultCellStyle.BackColor = Color.LightGray;
                            cc.Rows.Add();
                            cc.Rows[cc.Rows.Count - 1].Cells[0].Value = "名稱";//0
                            cc.Rows.Add();
                            cc.Rows[cc.Rows.Count - 1].Cells[0].Value = "顏色";//1
                            cc.Rows.Add();
                            cc.Rows[cc.Rows.Count - 1].Cells[0].Value = "固定/可動";//2
                            cc.Rows.Add();
                            cc.Rows[cc.Rows.Count - 1].Cells[0].Value = "縫分寬度";//3
                            cc.Rows.Add();
                            cc.Rows[cc.Rows.Count - 1].Cells[0].Value = "縫份長度";//4
                            cc.Rows.Add();
                            cc.Rows[cc.Rows.Count - 1].Cells[0].Value = "實/虛線";//5
                            cc.Rows.Add();
                            cc.Rows[cc.Rows.Count - 1].Cells[0].Value = "連接節點";//6
                            cc.Rows[cc.Rows.Count - 1].ReadOnly = true;
                            cc.Rows[cc.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightGray;
                        }
                        DataGridViewTextBoxCell temp_text_cell = new DataGridViewTextBoxCell();
                        temp_text_cell.Value = c.name;
                        cc.Rows[0].Cells[1] = temp_text_cell;

                        temp_text_cell = new DataGridViewTextBoxCell();
                        temp_text_cell.Value = c.co.ToString();
                        cc.Rows[1].Cells[1] = temp_text_cell;

                        DataGridViewComboBoxCell temp_combo_cell = new DataGridViewComboBoxCell();
                        temp_combo_cell.Items.Add("固定");
                        temp_combo_cell.Items.Add("可動");
                        temp_combo_cell.Value = temp_combo_cell.Items[c.fix ? 1 : 0];
                        cc.Rows[2].Cells[1] = temp_combo_cell;

                        temp_combo_cell = new DataGridViewComboBoxCell();
                        temp_combo_cell.Items.Add("有");
                        temp_combo_cell.Items.Add("無");
                        temp_combo_cell.Value = temp_combo_cell.Items[c.isSeam ? 0 : 1];
                        cc.Rows[3].Cells[1] = temp_combo_cell;

                        temp_text_cell = new DataGridViewTextBoxCell();
                        temp_text_cell.Value = c.Seam;
                        cc.Rows[4].Cells[1] = temp_text_cell;

                        temp_combo_cell = new DataGridViewComboBoxCell();
                        temp_combo_cell.Items.Add("實");
                        temp_combo_cell.Items.Add("虛");
                        temp_combo_cell.Value = temp_combo_cell.Items[c.dash ? 1 : 0];
                        cc.Rows[5].Cells[1] = temp_combo_cell;

                        string pname = "";
                        foreach (var p in c.path)
                            pname += p.name + " ";
                        pname.Remove(pname.Length - 1);
                        temp_text_cell = new DataGridViewTextBoxCell();
                        temp_text_cell.Value = pname;
                        cc.Rows[6].Cells[1] = temp_text_cell;
                    }
                }
                else
                {
                    PorpertyList[0].Visible = false;
                    PorpertyList[1].Visible = false;
                    PorpertyList[3].Visible = false;
                    PorpertyList[4].Visible = false;
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
                    if (isp) PorpertyList[2].Visible = true;
                }
            }
            else
            {
                PorpertyList[0].Visible = false;
                PorpertyList[1].Visible = false;
                PorpertyList[2].Visible = false;
                PorpertyList[3].Visible = false;
                PorpertyList[4].Visible = false;
            }

            onUpdateProperty = false;
        }
        private void Line_Config_Datagridview_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (Line_Config_Datagridview.IsCurrentCellDirty)
            {
                // This fires the cell value changed handler below
                Line_Config_Datagridview.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        private void Line_Config_Datagridview_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (SelectedGroup != null && !onUpdateProperty)
            {
                GraphLine l = SelectedGroup.L[0];
                DataGridView lc = Line_Config_Datagridview;
                l.name = lc.Rows[0].Cells[1].Value.ToString();
                l.fix = lc.Rows[3].Cells[1].Value.ToString() == "可動";
                l.isSeam = lc.Rows[4].Cells[1].Value.ToString() == "有";
                try
                {
                    float.TryParse(lc.Rows[5].Cells[1].Value.ToString(), out l.Seam);
                }
                catch
                {
                    ;
                }
                l.dash = lc.Rows[6].Cells[1].Value.ToString() == "虛";
                Refresh();
            }
        }
        private void Curve_Config_Datagridview_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (Curve_Config_Datagridview.IsCurrentCellDirty)
            {
                // This fires the cell value changed handler below
                Curve_Config_Datagridview.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        private void Curve_Config_Datagridview_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (SelectedGroup != null && !onUpdateProperty)
            {
                GraphCurve c = SelectedGroup.C[0];
                DataGridView cc = Curve_Config_Datagridview;
                c.name = cc.Rows[0].Cells[1].Value.ToString();
                c.fix = cc.Rows[2].Cells[1].Value.ToString() == "可動";
                c.isSeam = cc.Rows[3].Cells[1].Value.ToString() == "有";
                try
                {
                    float.TryParse(cc.Rows[4].Cells[1].Value.ToString(), out c.Seam);
                }
                catch
                {
                    ;
                }
                c.dash = cc.Rows[5].Cells[1].Value.ToString() == "虛";
                Refresh();
            }
        }
        private void FTL_Config_dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(Edit_FL_Temp_FTL_Priority_oblistview_index > -1 && Edit_FL_Temp_FTL_Priority_oblistview_index < FormulaToLineList.Count
                && !onUpdateProperty)
            {
                if (e.RowIndex == 4)
                {
                    FormulaToLine ftl = FormulaToLineList[Edit_FL_Temp_FTL_Priority_oblistview_index];
                    try
                    {
                        string[] s = FTL_Config_dataGridView.Rows[4].Cells[1].Value.ToString().Split(':');
                        double.Parse(s[0]);
                        double.Parse(s[1]);
                        ftl.prop12 = FTL_Config_dataGridView.Rows[4].Cells[1].Value.ToString();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                }
                else
                {
                    FormulaToLine ftl = FormulaToLineList[Edit_FL_Temp_FTL_Priority_oblistview_index];
                    ftl.name = FTL_Config_dataGridView.Rows[0].Cells[1].Value.ToString();
                }
                Refresh();
            }
        }
        private void FTL_Config_dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1 && e.RowIndex == 2)
            {
                FormulaToLine ftl = FormulaToLineList[Edit_FL_Temp_FTL_Priority_oblistview_index];
                string s = "";
                foreach (var ele in ftl.formula_eleL)
                {
                    if (ele.GetType().Equals(typeof(VariableElement)))
                    {
                        VariableElement vele = (VariableElement)ele;
                        s += vele.getName();
                    }
                    else
                    {
                        s += ele.ToString() + " ";
                    }
                }
                輸入算式 f = new 輸入算式(s);
                f.ShowDialog();
                if (f.DialogResult == DialogResult.OK)
                {
                    ftl.formula_eleL = f.EleL;
                    RefreshPorperty();
                }
            }
        }
        private void Point_Config_Datagridview_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (SelectedGroup != null && !onUpdateProperty)
            {
                GraphPoint p = SelectedGroup.P[0];
                DataGridView pc = Point_Config_Datagridview;
                p.name = pc.Rows[0].Cells[1].Value.ToString();
                Refresh();
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
            foreach (var a in g.A)
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
        private GraphGroup In_Path(GraphLine l, GraphCurve c)
        {
            foreach (var path in PathList)
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
                pictureBox2.Width = pictureBox1.Image.Width + 20;
                pictureBox2.Height = pictureBox1.Image.Height + 20;
            }
        }
        static private List<PointF> GraphCurveToBez(GraphCurve c)
        {
            List<PointF> t = new List<PointF>();
            t.Add(c.path[0].P);
            t.Add(new PointF(c.path[0].P.X + c.disSecond[0].X, c.path[0].P.Y + c.disSecond[0].Y));
            for (int i = 1; i < c.path.Count - 1; i++)
            {
                t.Add(new PointF(c.path[i].P.X + c.disFirst[i].X, c.path[i].P.Y + c.disFirst[i].Y));
                t.Add(c.path[i].P);
                t.Add(new PointF(c.path[i].P.X + c.disSecond[i].X, c.path[i].P.Y + c.disSecond[i].Y));
            }
            int a = c.path.Count;
            t.Add(new PointF(c.path[a - 1].P.X + c.disFirst[a - 1].X, c.path[a - 1].P.Y + c.disFirst[a - 1].Y));
            t.Add(c.path[a - 1].P);
            return t;
        }
        private float BezierFindT(PointF[] bez, PointF p)
        {
            float t = -1;
            double dist = 999999999;
            for(float i = 0; i < 1; i += 0.005F)
            {
                PointF temp = BezierGiveTFindPoint(bez, i);
                float x = temp.X, y = temp.Y;
                if ((x - p.X) * (x - p.X) + (y - p.Y) * (y - p.Y) < dist)
                {
                    dist = (x - p.X) * (x - p.X) + (y - p.Y) * (y - p.Y);
                    t = i;
                }
            }
            return t;
        }
        private PointF BezierGiveTFindPoint(PointF[] bez, float t)
        {
            double x = bez[0].X * Math.Pow(1 - t, 3) + bez[1].X * 3 * Math.Pow(1 - t, 2) * t + bez[2].X * 3 * (1 - t) * Math.Pow(t, 2) + bez[3].X * Math.Pow(t, 3);
            double y = bez[0].Y * Math.Pow(1 - t, 3) + bez[1].Y * 3 * Math.Pow(1 - t, 2) * t + bez[2].Y * 3 * (1 - t) * Math.Pow(t, 2) + bez[3].Y * Math.Pow(t, 3);
            return new PointF((float)x, (float)y);
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
            else if (type == 0 && path.Count == 2)
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
            else if (path.Count == 1)
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
                for (int i = 0; i < path.Count; i++)
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
        private void StraightCutPath(GraphGroup Path, GraphLine cutted_L1, GraphLine cutted_L2, 
            GraphCurve cutted_c1, int cutted_c1i, GraphCurve cutted_c2, int cutted_c2i,
            GraphPoint cut_p1, GraphPoint cut_p2,
            out GraphGroup newpath1, out GraphGroup newpath2)
        {
            GraphLine L1 = cutted_L1, L2 = cutted_L2;
            GraphCurve C1 = cutted_c1, C2 = cutted_c2;
            int C1Index = cutted_c1i, C2Index = cutted_c2i;
            GraphPoint p1 = cut_p1, p2 = cut_p2;
            int index1, index2;
            index1 = L1 != null ? Path.L.FindIndex(x => x == L1) : Path.C.FindIndex(x => x == C1);
            index2 = L2 != null ? Path.L.FindIndex(x => x == L2) : Path.C.FindIndex(x => x == C2);
            newpath1 = new GraphGroup(); newpath2 = new GraphGroup();
            GraphLine P2L = null;
            GraphCurve P2C = null;
            GraphPoint P1S = null, P1E = null, P2S = null, P2E = null;
            int dis = index2 - index1;
            dis += dis < 0 ? Path.L.Count : 0;
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
                    {
                        thisS.name = thisS.check_name(PointsList);
                        PointsList.Add(thisS);
                    }
                    if (!PointsList.Exists(x => x == thisE))
                    {
                        thisE.name = thisE.check_name(PointsList);
                        PointsList.Add(thisE);
                    }
                }
                else
                {
                    thisS = newpath1.C[i].path[0];
                    thisE = newpath1.C[i].path.Last();
                    foreach (var p in newpath1.C[i].path)
                    {
                        p.Relative++;
                        if (!PointsList.Exists(x => x == p))
                        {
                            p.name = p.check_name(PointsList);
                            PointsList.Add(p);
                        }
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
                    {
                        thisS.name = thisS.check_name(PointsList);
                        PointsList.Add(thisS);
                    }
                    if (!PointsList.Exists(x => x == thisE))
                    {
                        thisE.name = thisE.check_name(PointsList);
                        PointsList.Add(thisE);
                    }
                }
                else
                {
                    thisS = newpath2.C[i].path[0];
                    thisE = newpath2.C[i].path.Last();
                    foreach (var p in newpath2.C[i].path)
                    {
                        p.Relative++;
                        if (!PointsList.Exists(x => x == p))
                        {
                            p.name = p.check_name(PointsList);
                            PointsList.Add(p);
                        }
                    }
                    CurveList.Add(newpath2.C[i]);
                }
                if (!newpath2.P.Exists(x => x == thisS))
                    newpath2.P.Add(thisS);
                if (!newpath2.P.Exists(x => x == thisE))
                    newpath2.P.Add(thisE);
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
        PathDistance Right_Temp_Dist = null;
        GraphPoint Right_Temp_SupPoint = null;
        GraphLine Right_Temp_SupLine = null;

        #region delete_function
        private void DeletePoint(GraphPoint toDeleteP)
        {
            List<GraphLine> lt = new List<GraphLine>();
            foreach (var l in LineList)
            {
                if (l.StartPoint == toDeleteP || l.EndPoint == toDeleteP)
                {
                    lt.Add(l);
                }
            }
            foreach (var l in lt)
            {
                DeleteLine(l);
            }
            List<GraphCurve> ct = new List<GraphCurve>();
            foreach (var c in CurveList)
            {
                for (int i = 0; i < c.path.Count; i++)
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
            foreach (var c in ct)
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
            List<FormulaToLine> ftlt = new List<FormulaToLine>();
            foreach(var f in FormulaToLineList)
            {
                if (f.labled1.P.Exists(x => x == toDeleteP) || f.labled2.P.Exists(x => x == toDeleteP))
                    ftlt.Add(f);
            }
            foreach (var f in ftlt)
                FormulaToLineList.Remove(f);
            PointsList.Remove(toDeleteP);
        }
        private void DeleteLine(GraphLine toDeleteL)
        {
            if (!LineList.Exists(x => x == toDeleteL))
                return;
            toDeleteL.EndPoint.Relative--;
            toDeleteL.StartPoint.Relative--;
            LineList.Remove(toDeleteL);
            foreach (var p in PointsList)
            {
                if (p.MarkL == toDeleteL)
                    p.MarkL = null;
            }
            List<FormulaToLine> ftlt = new List<FormulaToLine>();
            foreach (var f in FormulaToLineList)
            {
                if (f.labled1 != null)
                    if (f.labled1.L.Exists(x => x == toDeleteL))
                        ftlt.Add(f);
                if (f.labled2 != null)
                    if (f.labled2.L.Exists(x => x == toDeleteL))
                        ftlt.Add(f);
            }
            foreach (var f in ftlt)
                FormulaToLineList.Remove(f);
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

                List<FormulaToLine> ftlt = new List<FormulaToLine>();
                foreach (var f in FormulaToLineList)
                {
                    if (f.labled1.C.Exists(x => x == c) || f.labled2.C.Exists(x => x == c))
                        ftlt.Add(f);
                }
                foreach (var f in ftlt)
                    FormulaToLineList.Remove(f);
            }
            PointsList.RemoveAll(x => x.Relative <= 0 && x.MarkL == null);
        }
        private void DeleteHoleCurve(GraphCurve c)
        {
            if (!CurveList.Exists(x => x == c))
                return;
            for (int i = 0; i < c.path.Count; i++)
            {
                c.path[i].Relative--;
            }
            List<FormulaToLine> ftlt = new List<FormulaToLine>();
            foreach (var f in FormulaToLineList)
            {
                if (f.labled1.C.Exists(x => x == c) || f.labled2.C.Exists(x => x == c))
                    ftlt.Add(f);
            }
            foreach (var f in ftlt)
                FormulaToLineList.Remove(f);
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
            foreach (var l in g.L)
            {
                PathList.RemoveAll(x => x.L.Exists(y => y == l));
                DeleteLine(l);
            }
            foreach (var c in g.C)
            {
                PathList.RemoveAll(x => x.C.Exists(y => y == c));
                DeleteHoleCurve(c);
            }
            foreach (var a in g.A)
            {
                DeleteArc(a);
            }

            GroupList.Remove(g);
        }
        private void DeleteSupPoint(GraphPoint SupP)
        {
            List<GraphLine> lt = new List<GraphLine>();
            foreach (var l in SupLineList)
            {
                if (l.StartPoint == SupP || l.EndPoint == SupP)
                {
                    lt.Add(l);
                }
            }
            foreach (var l in lt)
            {
                DeleteSupLine(l);
            }
            SupPointsList.Remove(SupP);
        }
        private void DeleteSupLine(GraphLine SupL)
        {
            if (!SupLineList.Exists(x => x == SupL))
                return;
            SupL.EndPoint.Relative--;
            SupL.StartPoint.Relative--;
            SupLineList.Remove(SupL);
            SupPointsList.RemoveAll(x => x.Relative <= 0 && x.MarkL == null);
        }
        #endregion

        private void PointCombine()
        {
            for (int i = 0; i < PointsList.Count; i++)
            {
                List<GraphPoint> t = PointsList.FindAll(x => (x.P.X - PointsList[i].P.X) * (x.P.X - PointsList[i].P.X) +
                                                             (x.P.Y - PointsList[i].P.Y) * (x.P.Y - PointsList[i].P.Y) < 0.3);
                if (t.Count >= 2)
                {
                    foreach (var l in LineList)
                    {
                        if (l.StartPoint == t[1])
                        {
                            l.StartPoint = t[0];
                        }
                        else if (l.EndPoint == t[1])
                        {
                            l.EndPoint = t[0];
                        }
                    }
                    foreach (var c in CurveList)
                    {
                        for (int j = 0; j < c.path.Count; j++)
                        {
                            if (c.path[j] == t[1])
                            {
                                if (c.path.Exists(x => x == t[0]))
                                {
                                    int a = c.path.FindIndex(x => x == t[0]);
                                    int b = c.path.FindIndex(x => x == t[1]);
                                    if (b == 0 || b == c.path.Count - 1)
                                    {
                                        c.path[b] = c.path[a];
                                        c.path.RemoveAt(a);
                                        c.disFirst.RemoveAt(a);
                                        c.disSecond.RemoveAt(a);
                                        c.type.RemoveAt(a);
                                    }
                                    else
                                    {
                                        c.path.RemoveAt(b);
                                        c.disFirst.RemoveAt(b);
                                        c.disSecond.RemoveAt(b);
                                        c.type.RemoveAt(b);
                                    }
                                }
                                else
                                {
                                    c.path[j] = t[0];
                                }
                            }
                        }
                    }
                    foreach (var a in ArcList)
                    {
                        if (a.StartPoint == t[1])
                        {
                            a.StartPoint = t[0];
                        }
                        else if (a.EndPoint == t[1])
                        {
                            a.EndPoint = t[0];
                        }
                    }
                    foreach(var g in GroupList)
                    {
                        if (g.P.Exists(x => x == t[1]))
                        {
                            g.P.Remove(t[1]);
                            if (!g.P.Exists(x => x == t[0]))
                            {
                                g.P.Add(t[0]);
                            }
                        }
                    }
                    foreach (var g in PathList)
                    {
                        if (g.P.Exists(x => x == t[1]))
                        {
                            g.P.Remove(t[1]);
                            if (!g.P.Exists(x => x == t[0]))
                            {
                                g.P.Add(t[0]);
                            }
                        }
                    }
                    foreach(var f in FormulaToLineList)
                    {
                        if (f.unfixed_P1 == t[1])
                            f.unfixed_P1 = t[0];
                        if (f.unfixed_P2 == t[1])
                            f.unfixed_P2 = t[0];
                    }

                    t[0].Relative += t[1].Relative - 1;
                    PointsList.Remove(t[1]);
                }
            }
        }

        private void LineInsert(GraphLine l, GraphPoint p, out GraphLine StoM, out GraphLine MtoE)
        {
            GraphLine line1 = new GraphLine(l.StartPoint, p);
            GraphLine line2 = new GraphLine(p, l.EndPoint);
            StoM = line1;
            MtoE = line2;
        }
        private GraphCurve CurveInsert(GraphCurve c, int cindex, GraphPoint p)
        {
            cindex--;
            GraphCurve ans = new GraphCurve();
            for (int i = 0; i < c.path.Count; i++)
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
                    PointF[] pts = { p0, p1, p2, p3 };
                    double t = BezierFindT(pts, p.P);
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
        /*static private double Bezier_Find_t(PointF p0, PointF p1, PointF p2, PointF p3, PointF cutp)
        {
            double mindis = 99999999999;
            double t = 0;
            for (double i = 0; i < 1; i += 0.005)
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
        }*/
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)//切換分頁
        {
            if (tabControl1.SelectedIndex == TabpagesList.Count - 1 && tabControl1.SelectedIndex >= 0)
            {
                TabPage t = new TabPage("new_page");
                t.AutoScroll = true;
                t.AutoScrollPosition = new Point(0, 0);
                CurveList = new List<GraphCurve>();
                PointsList = new List<GraphPoint>();
                LineList = new List<GraphLine>();
                GroupList = new List<GraphGroup>();
                ArcList = new List<GraphArc>();
                TextList = new List<GraphText>();
                PathList = new List<GraphGroup>();
                PDistList = new List<PathDistance>();
                SupPointsList = new List<GraphPoint>();
                SupLineList = new List<GraphLine>();
                FormulaToLineList = new List<FormulaToLine>();
                ClothGrainMark = new GrainMark();
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
                a.PDistL = PDistList;
                a.SupPointL = SupPointsList;
                a.SupLineL = SupLineList;
                a.FtoLL = FormulaToLineList;
                a.TabpageName = t.Text;
                a.width = 595;
                a.height = 842;
                a.ClothGrainMark = ClothGrainMark;
                TabpageDataList.Add(a);

                t.Controls.Add(pictureBox1);
                TabpagesList.Insert(TabpagesList.Count - 1, t);
                tabControl1.TabPages.Insert(TabpagesList.Count - 2, t);
                tabControl1.SelectedIndex = TabpagesList.Count - 2;
                TabpagesList[tabControl1.SelectedIndex].Controls.Add(pictureBox1);
                TabpagesList[tabControl1.SelectedIndex].Controls.Add(pictureBox2);
                pictureBox1.Location = new Point(20, 20);
                pictureBox2.Location = new Point(0, 0);
                Push_Undo_Data();
                RefreshUndoCheck();
            }
            else if (tabControl1.SelectedIndex >= 0 && TabpagesList.Count > 0 && TabpageDataList.Count > 0)
            {
                TabpagesList[tabControl1.SelectedIndex].AutoScroll = true;
                TabpagesList[tabControl1.SelectedIndex].AutoScrollPosition = new Point(0, 0);
                pictureBox1.Location = new Point(20, 20);
                pictureBox2.Location = new Point(0, 0);
                TabpagesList[tabControl1.SelectedIndex].Controls.Add(pictureBox1);
                TabpagesList[tabControl1.SelectedIndex].Controls.Add(pictureBox2);
                CurveList = TabpageDataList[tabControl1.SelectedIndex].CurveL;
                PointsList = TabpageDataList[tabControl1.SelectedIndex].PointL;
                LineList = TabpageDataList[tabControl1.SelectedIndex].LineL;
                GroupList = TabpageDataList[tabControl1.SelectedIndex].GroupL;
                ArcList = TabpageDataList[tabControl1.SelectedIndex].ArcL;
                TextList = TabpageDataList[tabControl1.SelectedIndex].TextL;
                PathList = TabpageDataList[tabControl1.SelectedIndex].PathL;
                PDistList = TabpageDataList[tabControl1.SelectedIndex].PDistL;
                SupPointsList = TabpageDataList[tabControl1.SelectedIndex].SupPointL;
                SupLineList = TabpageDataList[tabControl1.SelectedIndex].SupLineL;
                FormulaToLineList = TabpageDataList[tabControl1.SelectedIndex].FtoLL;
                ClothGrainMark = TabpageDataList[tabControl1.SelectedIndex].ClothGrainMark;
                pictureBox1.Width = (int)(TabpageDataList[tabControl1.SelectedIndex].width * ZoomSize);
                pictureBox1.Height = (int)(TabpageDataList[tabControl1.SelectedIndex].height * ZoomSize);
                pictureBox2.Width = pictureBox1.Width + 20;
                pictureBox2.Height = pictureBox1.Height + 20;
                Undo_Data = TabpageDataList[tabControl1.SelectedIndex].Undo;
                RefreshUndoCheck();
            }
            Refresh();
        }

        #region ContextMenuStrip1 右鍵選單
        private void toolStripMenuItem2_Click(object sender, EventArgs e)//右鍵選單>刪除
        {
            if (Right_Temp_Point != null)
            {
                PathList.RemoveAll(x => x.P.Exists(y => y == Right_Temp_Point));
                DeletePoint(Right_Temp_Point);
            }
            else if (Right_Temp_SupPoint != null)
            {
                DeleteSupPoint(Right_Temp_SupPoint);
            }
            else if (Right_Temp_Line != null)
            {
                PathList.RemoveAll(x => x.L.Exists(y => y == Right_Temp_Line));
                DeleteLine(Right_Temp_Line);
            }
            else if (Right_Temp_SupLine != null)
            {
                DeleteSupLine(Right_Temp_SupLine);
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
            else if (Right_Temp_Dist != null)
                PDistList.Remove(Right_Temp_Dist);
            Right_Temp_Clear();
            PointsList.RemoveAll(x => x.Relative <= 0 && x.MarkL == null);
            SupPointsList.RemoveAll(x => x.Relative <= 0 && x.MarkL == null);

            List<PathDistance> pdl = new List<PathDistance>();
            foreach (var pd in PDistList)
            {
                if (!pd.in_List(LineList, CurveList, ArcList, SupLineList))
                    pdl.Add(pd);
            }
            foreach (var pd in pdl)
                PDistList.Remove(pd);

            Push_Undo_Data();
        }
        private void 新增節點ToolStripMenuItem_Click(object sender, EventArgs e)//右鍵選單>新增節點
        {
            if (Right_Temp_Line != null)
            {
                bool inPDList = PDistList.Exists(x => x.L1 == Right_Temp_Line || x.L2 == Right_Temp_Line),
                     inPathList = PathList.Exists(x => x.L.Exists(y => y == Right_Temp_Line)),
                     inGroupList = GroupList.Exists(x => x.L.Exists(y => y == Right_Temp_Line));
                if (inPDList)
                {
                    if (MessageBox.Show("此動作將會取消此線的距離標示", "訊息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        PDistList.RemoveAll(x => x.L1 == Right_Temp_Line || x.L2 == Right_Temp_Line);
                    }
                    else
                        return;
                }
                if (inPathList)
                {
                    if (MessageBox.Show("此動作將會取消此線的圖形", "訊息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        PathList.RemoveAll(x => x.L.Exists(y => y == Right_Temp_Line));
                    }
                    else
                        return;
                }
                if (inGroupList)
                {
                    if (MessageBox.Show("此動作將會取消此線的群組", "訊息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        GroupList.RemoveAll(x => x.L.Exists(y => y == Right_Temp_Line));
                    }
                    else
                        return;
                }
                GraphPoint p = new GraphPoint(Right_Temp_Mouse_Pos.X, Right_Temp_Mouse_Pos.Y);
                if (PathList.Exists(x => x.L.Exists(y => y == Right_Temp_Line)))
                    PathList.RemoveAll(x => x.L.Exists(y => y == Right_Temp_Line));
                p.name = p.check_name(PointsList);
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
                bool inPDList = PDistList.Exists(x => x.C1 == Right_Temp_Curve || x.C2 == Right_Temp_Curve),
                     inPathList = PathList.Exists(x => x.C.Exists(y => y == Right_Temp_Curve)),
                     inGroupList = GroupList.Exists(x => x.C.Exists(y => y == Right_Temp_Curve));
                if (inPDList)
                {
                    if (MessageBox.Show("此動作將會取消此曲線的距離標示", "訊息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        PDistList.RemoveAll(x => x.C1 == Right_Temp_Curve || x.C2 == Right_Temp_Curve);
                    }
                    else
                        return;
                }
                if (inPathList)
                {
                    if (MessageBox.Show("此動作將會取消此曲線的圖形", "訊息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        PathList.RemoveAll(x => x.C.Exists(y => y == Right_Temp_Curve));
                    }
                    else
                        return;
                }
                if (inGroupList)
                {
                    if (MessageBox.Show("此動作將會取消此曲線的群組", "訊息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        GroupList.RemoveAll(x => x.C.Exists(y => y == Right_Temp_Curve));
                    }
                    else
                        return;
                }
                GraphPoint p = new GraphPoint(Right_Temp_Mouse_Pos.X, Right_Temp_Mouse_Pos.Y);
                if (PathList.Exists(x => x.C.Exists(y => y == Right_Temp_Curve)))
                    PathList.RemoveAll(x => x.C.Exists(y => y == Right_Temp_Curve));
                p.name = p.check_name(PointsList);
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
            Right_Temp_Clear();
        }
        private void 組成群組ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Right_Temp_Group.G.Count; i++)
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

            PDistList.RemoveAll(x => x.A1 == arc || x.A2 == arc);
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
                    PDistList.RemoveAll(x => x.L1 == l[0] || x.L2 == l[0]);
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
                    PDistList.RemoveAll(x => x.C1 == c[0] || x.C2 == c[0]);
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
                    curve.name = a[0].name;
                    aL.Add(a[0]);
                    cL.Add(curve);
                    path.L.Add(null);
                    path.C.Add(curve);
                    PDistList.RemoveAll(x => x.A1 == a[0] || x.A2 == a[0]);
                    SelectedGroup.A.Remove(a[0]);
                }
                else
                {
                    MessageBox.Show("必須是一個封閉的圖形", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            } while (p != SelectedGroup.P[0]);
            for (int i = 0; i < aL.Count; i++)
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
            for(int i = 0; i < path.L.Count; i++)
            {
                if (path.L[i] != null)
                {
                    FormulaToLineList.RemoveAll(x => x.L1 == path.L[i] || x.L2 == path.L[i]);
                }
                else
                {
                    FormulaToLineList.RemoveAll(x => x.C1 == path.C[i] || x.C2 == path.C[i]);
                }
            }
            PathList.Add(path);
            Push_Undo_Data();
        }
        private void 解除圖形ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedLine != null)
            {
                GraphGroup g = PathList.Find(x => x.L.FindAll(y => y == SelectedLine).Count > 0);
                foreach (var l in g.L)
                {
                    if (l != null)
                    {
                        l.Seam = -1;
                        l.isSeam = false;
                    }
                }
                foreach (var c in g.C)
                {
                    if (c != null)
                    {
                        c.Seam = -1;
                        c.isSeam = false;
                    }
                }
                for (int i = 0; i < g.L.Count; i++)
                {
                    if (g.L[i] != null)
                    {
                        FormulaToLineList.RemoveAll(x => x.L1 == g.L[i] || x.L2 == g.L[i]);
                    }
                    else
                    {
                        FormulaToLineList.RemoveAll(x => x.C1 == g.C[i] || x.C2 == g.C[i]);
                    }
                }
                PathList.Remove(g);
                Push_Undo_Data();
            }
            else if (SelectedCurve != null)
            {
                GraphGroup g = PathList.Find(x => x.C.FindAll(y => y == SelectedCurve).Count > 0);
                foreach (var l in g.L)
                {
                    if (l != null)
                    {
                        l.Seam = -1;
                        l.isSeam = false;
                    }
                }
                foreach (var c in g.C)
                {
                    if (c != null)
                    {
                        c.Seam = -1;
                        c.isSeam = false;
                    }
                }
                for (int i = 0; i < g.L.Count; i++)
                {
                    if (g.L[i] != null)
                    {
                        FormulaToLineList.RemoveAll(x => x.L1 == g.L[i] || x.L2 == g.L[i]);
                    }
                    else
                    {
                        FormulaToLineList.RemoveAll(x => x.C1 == g.C[i] || x.C2 == g.C[i]);
                    }
                }
                PathList.Remove(g);
                Push_Undo_Data();
            }
        }
        private void 直線等分ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Right_Temp_Line != null)
            {
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
                            p.name = p.check_name(PointsList);
                            PointsList.Add(p);
                        }
                        Push_Undo_Data();
                    }
                }
                else
                {
                    PointsList.RemoveAll(x => x.MarkL == Right_Temp_Line);
                    Push_Undo_Data();
                }
            }
            else if (Right_Temp_SupLine != null)
            {
                if (直線等分ToolStripMenuItem.Text == "直線等分")
                {
                    等分 f2 = new 等分();
                    if (f2.ShowDialog() == DialogResult.OK)
                    {
                        int num = f2.ans;
                        if (num <= 1)
                            return;
                        GraphLine line = Right_Temp_SupLine;
                        float xdis = line.EndPoint.P.X - line.StartPoint.P.X, ydis = line.EndPoint.P.Y - line.StartPoint.P.Y;
                        xdis /= num;
                        ydis /= num;
                        for (int i = 1; i < num; i++)
                        {
                            GraphPoint p = new GraphPoint(line.StartPoint.P.X + xdis * i, line.StartPoint.P.Y + ydis * i);
                            p.MarkL = line;
                            p.part = (float)i / num;
                            SupPointsList.Add(p);
                        }
                        Push_Undo_Data();
                    }
                }
                else
                {
                    SupPointsList.RemoveAll(x => x.Relative <= 0 && x.MarkL == null);
                    Push_Undo_Data();
                }
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
            Right_Temp_Clear();
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
            if (Right_Temp_Line != null)
            {
                colorDialog1.Color = Right_Temp_Line.co;
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    Right_Temp_Line.co = colorDialog1.Color;
                    Push_Undo_Data();
                }
                Refresh();
            }
            else if (Right_Temp_SupLine != null)
            {
                colorDialog1.Color = Right_Temp_SupLine.co;
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    Right_Temp_SupLine.co = colorDialog1.Color;
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
        private void 上下左右移動ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            上下左右移動 form = new 上下左右移動();
            if (form.ShowDialog() == DialogResult.OK)
            {
                float ud = form.updown * (form.utinch_fcm ? 72 : 72 / 2.54F) * (form.tufd ? -1 : 1);
                float lr = form.leftright * (form.ltinch_fcm ? 72 : 72 / 2.54F) * (form.tlfr ? -1 : 1);
                if (Right_Temp_Line != null)
                {
                    Right_Temp_Line.StartPoint.P.X += lr;
                    Right_Temp_Line.StartPoint.P.Y += ud;
                    Right_Temp_Line.EndPoint.P.X += lr;
                    Right_Temp_Line.EndPoint.P.Y += ud;
                }
                else if (Right_Temp_SupLine != null)
                {
                    Right_Temp_SupLine.StartPoint.P.X += lr;
                    Right_Temp_SupLine.StartPoint.P.Y += ud;
                    Right_Temp_SupLine.EndPoint.P.X += lr;
                    Right_Temp_SupLine.EndPoint.P.Y += ud;
                }
                else if (Right_Temp_Curve != null)
                {
                    for (int i = 0; i < Right_Temp_Curve.path.Count; i++)
                    {
                        Right_Temp_Curve.path[i].P.X += lr;
                        Right_Temp_Curve.path[i].P.Y += ud;
                    }
                }
                else if (Right_Temp_Arc != null)
                {
                    Right_Temp_Arc.StartPoint.P.X += lr;
                    Right_Temp_Arc.StartPoint.P.Y += ud;
                    Right_Temp_Arc.EndPoint.P.X += lr;
                    Right_Temp_Arc.EndPoint.P.Y += ud;
                }
                Right_Temp_Clear();
                PointsList.RemoveAll(x => x.Relative <= 0 && x.MarkL == null);
                Push_Undo_Data();
            }
            else
                return;
        }
        private void 公式對應ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Right_Temp_Line != null)
            {
                Right_Temp_Line.fix = !Right_Temp_Line.fix;
            }
            else if (Right_Temp_Curve != null)
            {
                Right_Temp_Curve.fix = !Right_Temp_Curve.fix;
            }
        }
        private void 曲線平滑化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(Right_Temp_Curve != null)
            {
                GraphCurve c = PathToCurve(Right_Temp_Curve.path);
                Right_Temp_Curve.disFirst = c.disFirst;
                Right_Temp_Curve.disSecond = c.disSecond;
                Push_Undo_Data();
            }
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
        private void 實線ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Right_Temp_Line != null)
            {
                Right_Temp_Line.dash = false;
            }
            else if (Right_Temp_Curve != null)
            {
                Right_Temp_Curve.dash = false;
            }
            else if (Right_Temp_Arc != null)
            {
                Right_Temp_Arc.dash = false;
            }
            Push_Undo_Data();
        }
        private void 虛線ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Right_Temp_Line != null)
            {
                Right_Temp_Line.dash = true;
            }
            else if (Right_Temp_Curve != null)
            {
                Right_Temp_Curve.dash = true;
            }
            else if (Right_Temp_Arc != null)
            {
                Right_Temp_Arc.dash = true;
            }
            Push_Undo_Data();
        }
        private void 橫書ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(Right_Temp_Text != null)
            {
                Right_Temp_Text.straight = false;
            }
        }
        private void 直書ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Right_Temp_Text != null)
            {
                Right_Temp_Text.straight = true;
            }
        }

        #endregion

        #region 身形調整

        class Formula_obListItem
        {
            string name;
            string formula;
            string result;
            public Formula_obListItem(string name, string formula, string result)
            {
                this.name = name;
                this.formula = formula;
                this.result = result;
            }

            public string Name
            {
                get { return name; }
                set { name = value; }
            }
            public string Formula
            {
                get { return formula; }
                set { formula = value; }
            }
            public string Result
            {
                get { return result; }
                set { result = value; }
            }
        }
        class FTL_Priority_obListItem
        {
            string priority;
            string ftl_name;
            public FTL_Priority_obListItem(string priority, string ftl_name)
            {
                this.priority = priority;
                this.ftl_name = ftl_name;
            }

            public string FTL_Name
            {
                get { return ftl_name; }
                set { ftl_name = value; }
            }
            public string Priority
            {
                get { return priority; }
                set { priority = value; }
            }
        }
        class FTL_Config_obListItem
        {
            string column_name;
            string contain;
            public FTL_Config_obListItem(string column_name, string contain)
            {
                this.column_name = column_name;
                this.contain = contain;
            }

            public string Column_Name
            {
                get { return column_name; }
                set { column_name = value; }
            }
            public string Contain
            {
                get { return contain; }
                set { contain = value; }
            }
        }
        Formula FSLB_TempFML;
        GraphPoint FSLB_TempP = null;
        GraphPoint FSLB_TempGP = null;
        GraphLine FSLB_TempL = null;
        GraphCurve FSLB_TempC = null;
        GraphGroup FSLB_TempPath = null;
        FormulaToLine FSLB_TempFormulaToLine = null;
        int FSLB_TempC_index = -1;
        int FSLB_State = 0;
        FormulaToLine Right_Temp_FTL = null;
        double[] new_bodysize = new double[29];
        bool fit_bodysize = false;

        private void 設定標準身形ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            輸入基礎尺寸 form = new 輸入基礎尺寸(ClothStandardSize, ClothStandardName, ClothStandardNumber);
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK)
            {
                ClothStandardSize = form.Sizes;
                ClothStandardName = form.Prototype_Name;
                ClothStandardNumber = form.Prototype_Number;
            }
        }
        private void 輸入算式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            輸入算式 f = new 輸入算式();
            f.ShowDialog();
        }
        private void 標籤一覽輸入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            標籤一覽 f = new 標籤一覽(FormulaList);
            f.ShowDialog();
            if (f.DialogResult == DialogResult.OK)
            {
                FormulaList = f.FL;
            }
        }
        private void 對應編輯模式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (EditFL == true)
            {
                EditFL = false;
                對應編輯模式ToolStripMenuItem.Checked = false;
                Mouse_Mode = Mouse_Mode_Type.Cursor;
                toolStrip1.Enabled = true;
            }
            else if (EditFL == false)
            {
                EditFL = true;
                對應編輯模式ToolStripMenuItem.Checked = true;
                Mouse_Mode = Mouse_Mode_Type.FormulaToL;
                toolStrip1.Enabled = false;
                FtoL_checkedListBox.SetItemChecked(0, true);
                FtoL_Lclick_checkedListBox.SetItemChecked(0, true);
            }
            List<FormulaToLine> del = new List<FormulaToLine>();
            foreach(var ftl in FormulaToLineList)
            {
                if (!ftl.in_List(PointsList, LineList, CurveList))
                    del.Add(ftl);
            }
            foreach (var d in del)
                FormulaToLineList.Remove(d);
            RefreshPorperty();
        }
        private void 刪除對應ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormulaToLineList.Remove(Right_Temp_FTL);
            Push_Undo_Data();
            Refresh();
        }
        private void 編輯所有對應ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            編輯所有對應 f = new 編輯所有對應(TabpageDataList);
            if (f.ShowDialog() == DialogResult.OK)
            {
                
            }
        }
        private void ExitEditButton_Click(object sender, EventArgs e)
        {
            if (EditFL == true)
            {
                EditFL = false;
                對應編輯模式ToolStripMenuItem.Checked = false;
                Mouse_Mode = Mouse_Mode_Type.Cursor;
                toolStrip1.Enabled = true;
            }
            RefreshPorperty();
        }
        private void FormulaSelectLineButton_Click(object sender, EventArgs e)
        {
            if (FSLB_State == 0 && Edit_FL_Temp_FML_oblistview_index != -1)
            {
                int index = Edit_FL_Temp_FML_oblistview_index;
                if (index >= 0)
                {
                    FSLB_TempFML = FormulaList[index];
                    FSLB_State = 1;
                }
            }
            else
            {
                FormulaToL_Temp_Clean();
            }
        }
        private void objectListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (objectListView1.FocusedItem != null)
                Edit_FL_Temp_FML_oblistview_index = objectListView1.FocusedItem.Index;
        }
        private void FTL_Priority_oblistview_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FTL_Priority_oblistview.FocusedItem != null)
                Edit_FL_Temp_FTL_Priority_oblistview_index = FTL_Priority_oblistview.FocusedItem.Index;
        }
        private void objectListView1_MouseUp(object sender, MouseEventArgs e)
        {
            RefreshPorperty();
        }
        private void FTL_Priority_oblistview_MouseUp(object sender, MouseEventArgs e)
        {
            RefreshPorperty();
        }
        private void FTL_Priority_Up_Button_Click(object sender, EventArgs e)
        {
            if(Edit_FL_Temp_FTL_Priority_oblistview_index != -1 && Edit_FL_Temp_FTL_Priority_oblistview_index < FormulaToLineList.Count)
            {
                if(Edit_FL_Temp_FTL_Priority_oblistview_index > 0)
                {
                    FormulaToLine temp = FormulaToLineList[Edit_FL_Temp_FTL_Priority_oblistview_index];
                    FormulaToLineList[Edit_FL_Temp_FTL_Priority_oblistview_index] = FormulaToLineList[Edit_FL_Temp_FTL_Priority_oblistview_index - 1];
                    FormulaToLineList[Edit_FL_Temp_FTL_Priority_oblistview_index - 1] = temp;
                    Edit_FL_Temp_FTL_Priority_oblistview_index -= 1;
                    RefreshPorperty();
                    Refresh();
                }
            }
        }
        private void FTL_Priority_Down_Button_Click(object sender, EventArgs e)
        {
            if (Edit_FL_Temp_FTL_Priority_oblistview_index != -1 && Edit_FL_Temp_FTL_Priority_oblistview_index < FormulaToLineList.Count)
            {
                if (Edit_FL_Temp_FTL_Priority_oblistview_index < FormulaToLineList.Count - 1)
                {
                    FormulaToLine temp = FormulaToLineList[Edit_FL_Temp_FTL_Priority_oblistview_index];
                    FormulaToLineList[Edit_FL_Temp_FTL_Priority_oblistview_index] = FormulaToLineList[Edit_FL_Temp_FTL_Priority_oblistview_index + 1];
                    FormulaToLineList[Edit_FL_Temp_FTL_Priority_oblistview_index + 1] = temp;
                    Edit_FL_Temp_FTL_Priority_oblistview_index += 1;
                    RefreshPorperty();
                }
            }
        }
        private void MBLeft_FormulaToL_DOWN(MouseEventArgs ev)
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
            if (FSLB_State == 0)
            {
                FSLB_TempL = FindLineByPoint(LineList, e);
                FindCurveByPoint(CurveList, e, out FSLB_TempC, out FSLB_TempC_index);
                if (FSLB_TempL != null)
                {
                    FSLB_TempFormulaToLine = FormulaToLineList.FindLast(x => x.L1 == FSLB_TempL || x.L2 == FSLB_TempL);
                }
                else
                {
                    FSLB_TempFormulaToLine = FormulaToLineList.FindLast(x => x.C1 == FSLB_TempC || x.C2 == FSLB_TempC);
                }
                if (FSLB_TempL != null)
                {
                    foreach (var path in PathList)
                    {
                        if (path.L.Exists(x => x == FSLB_TempL))
                        {
                            FSLB_TempPath = path;
                            break;
                        }
                    }
                }
                else if (FSLB_TempC != null)
                {
                    foreach (var path in PathList)
                    {
                        if (path.C.Exists(x => x == FSLB_TempC))
                        {
                            FSLB_TempPath = path;
                            break;
                        }
                    }
                }
            }
            if (FSLB_State == 1)
            {
                FSLB_TempL = FindLineByPoint(LineList, e);
                FindCurveByPoint(CurveList, e, out FSLB_TempC, out FSLB_TempC_index);
                FSLB_TempP = new GraphPoint(e.X, e.Y);
                FSLB_TempGP = FindPointByPoint(PointsList, e);
                FSLB_State = 2;
                if(FindArcByPoint(ArcList, e) != null)
                {
                    MessageBox.Show("將弧線轉為曲線再進行操作", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FormulaToL_Temp_Clean();
                    return;
                }
                if (FSLB_TempL == null && FSLB_TempC == null && FSLB_TempGP == null)
                {
                    FormulaToL_Temp_Clean();
                    return;
                }
            }
            else if (FSLB_State == 2)
            {
                GraphPoint p1 = FSLB_TempP, p2 = new GraphPoint(e.X, e.Y);
                GraphLine L1 = FSLB_TempL, L2 = FindLineByPoint(LineList, e);
                GraphCurve C1 = FSLB_TempC, C2;
                GraphGroup path2 = null;
                GraphPoint GP = FindPointByPoint(PointsList, e);
                int ci1 = FSLB_TempC_index, ci2;
                if (FindArcByPoint(ArcList, e) != null)
                {
                    MessageBox.Show("將弧線轉為曲線再進行操作", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FormulaToL_Temp_Clean();
                    return;
                }
                FindCurveByPoint(CurveList, e, out C2, out ci2);
                if (FSLB_TempL != null)
                {
                    foreach (var path in PathList)
                    {
                        if (path.L.Exists(x => x == FSLB_TempL))
                        {
                            FSLB_TempPath = path;
                            break;
                        }
                    }
                }
                else if (FSLB_TempC != null)
                {
                    foreach (var path in PathList)
                    {
                        if (path.C.Exists(x => x == FSLB_TempC))
                        {
                            FSLB_TempPath = path;
                            break;
                        }
                    }
                }
                if (L2 == null && C2 == null && GP == null)
                {
                    FormulaToL_Temp_Clean();
                    return;
                }
                if (L2 != null)
                {
                    foreach (var path in PathList)
                    {
                        if (path.L.Exists(x => x == L2))
                        {
                            path2 = path;
                            break;
                        }
                    }
                }
                else if (C2 != null)
                {
                    foreach (var path in PathList)
                    {
                        if (path.C.Exists(x => x == C2))
                        {
                            path2 = path;
                            break;
                        }
                    }
                }
                int path_type = -1;
                if (GP == null && FSLB_TempGP == null)
                {
                    if (FSLB_TempPath == null && path2 == null)
                        path_type = 0;
                    else if (FSLB_TempPath == null)
                        path_type = 1;
                    else if (path2 == null)
                        path_type = 2;
                    else if (FSLB_TempPath == path2)
                        path_type = 3;
                    else
                    {
                        MessageBox.Show("請將線段切在同一圖形", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FormulaToL_Temp_Clean();
                        return;
                    }
                }

                FormulaToLine ftl;
                int ftl_mode = 0;
                for(int i = 0; i < 3; i++)
                {
                    if (FtoL_checkedListBox.GetItemChecked(i))
                    {
                        ftl_mode = i;
                        break;
                    }
                }
                if (ftl_mode == 0)
                {
                    float temp = p1.P.Y + p2.P.Y;
                    p1.P.Y = temp / 2;
                    p2.P.Y = temp / 2;
                }
                else if (ftl_mode == 1)
                {
                    float temp = p1.P.X + p2.P.X;
                    p1.P.X = temp / 2;
                    p2.P.X = temp / 2;
                }
                if (FSLB_TempGP != null && GP != null)
                {
                    ftl = new FormulaToLine(FSLB_TempGP, GP, FSLB_TempGP.P, GP.P, ftl_mode);
                }
                else if (FSLB_TempGP != null && L2 != null)
                {
                    ftl = new FormulaToLine(FSLB_TempGP, L2, FSLB_TempGP.P, p2.P, ftl_mode);
                }
                else if (FSLB_TempGP != null && C2 != null)
                {
                    ftl = new FormulaToLine(FSLB_TempGP, C2, ci2, FSLB_TempGP.P, p2.P, ftl_mode);
                }
                else if (L1 != null && GP != null)
                {
                    ftl = new FormulaToLine(GP, L1, p1.P, GP.P, ftl_mode);
                }
                else if (C1 != null && GP != null)
                {
                    ftl = new FormulaToLine(GP, C1, ci1, p1.P, GP.P, ftl_mode);
                }
                else if (L1 != null && L2 != null)
                {
                    ftl = new FormulaToLine(L1, L2, p1.P, p2.P, ftl_mode, path_type);
                }
                else if (L1 != null && C2 != null)
                {
                    ftl = new FormulaToLine(L1, C2, ci2, p1.P, p2.P, ftl_mode, path_type);
                }
                else if (C1 != null && L2 != null)
                {
                    ftl = new FormulaToLine(L2, C1, ci1, p2.P, p1.P, ftl_mode, path_type);
                }
                else if (C1 != null && C2 != null)
                {
                    ftl = new FormulaToLine(C1, ci1, C2, ci2, p1.P, p2.P, ftl_mode, path_type);
                }
                else
                {
                    FormulaToL_Temp_Clean();
                    return;
                }
                if (ftl.in_List(PointsList, LineList, CurveList))
                {

                    ftl.fml = FSLB_TempFML;
                    ftl.formula_eleL = FSLB_TempFML.EleL;
                    ftl.name = ftl.check_name(FormulaToLineList);
                    FormulaToLineList.Add(ftl);
                    FormulaToL_Temp_Clean();
                    PointsList.RemoveAll(x => x.Relative <= 0 && x.MarkL == null);
                    Refresh();
                    RefreshPorperty();
                    Push_Undo_Data();
                }
                else
                {
                    FormulaToL_Temp_Clean();
                    return;
                }
            }
        }
        private void MBLeft_FormulaToL_Up(MouseEventArgs ev)
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
            if(FSLB_State == 0)
            {
                GraphLine l = FindLineByPoint(LineList, e);
                GraphCurve c;int ci;
                FindCurveByPoint(CurveList, e, out c, out ci);
                bool lable_type = FtoL_Lclick_checkedListBox.GetItemChecked(1);
                if (FSLB_TempFormulaToLine != null && FSLB_TempPath != null)
                {
                    if (l != null && FSLB_TempPath.L.Exists(x => x == l))
                    {
                        if (FSLB_TempFormulaToLine.type == 0)
                        {
                            if (FSLB_TempFormulaToLine.L1 == FSLB_TempL && FSLB_TempL != null)
                                FSLB_TempFormulaToLine.addlable(l, FSLB_TempPath, 1, lable_type);
                            else if (FSLB_TempFormulaToLine.L2 == FSLB_TempL && FSLB_TempL != null)
                                FSLB_TempFormulaToLine.addlable(l, FSLB_TempPath, 2, lable_type);
                        }
                        else if (FSLB_TempFormulaToLine.type == 1)
                        {
                            if (FSLB_TempFormulaToLine.L1 == FSLB_TempL && FSLB_TempL != null)
                                FSLB_TempFormulaToLine.addlable(l, FSLB_TempPath, 1, lable_type);
                            else if (FSLB_TempFormulaToLine.C1 == FSLB_TempC && FSLB_TempC != null)
                                FSLB_TempFormulaToLine.addlable(l, FSLB_TempPath, 2, lable_type);
                        }
                        else if (FSLB_TempFormulaToLine.type == 2)
                        {
                            if (FSLB_TempFormulaToLine.C1 == FSLB_TempC && FSLB_TempC != null)
                                FSLB_TempFormulaToLine.addlable(l, FSLB_TempPath, 1, lable_type);
                            else if (FSLB_TempFormulaToLine.C2 == FSLB_TempC && FSLB_TempC != null)
                                FSLB_TempFormulaToLine.addlable(l, FSLB_TempPath, 2, lable_type);
                        }
                    }
                    else if (c != null && FSLB_TempPath.C.Exists(x => x == c))
                    {
                        if (FSLB_TempFormulaToLine.type == 0)
                        {
                            if (FSLB_TempFormulaToLine.L1 == FSLB_TempL && FSLB_TempL != null)
                                FSLB_TempFormulaToLine.addlable(c, FSLB_TempPath, 1, lable_type);
                            else if (FSLB_TempFormulaToLine.L2 == FSLB_TempL && FSLB_TempL != null)
                                FSLB_TempFormulaToLine.addlable(c, FSLB_TempPath, 2, lable_type);
                        }
                        else if (FSLB_TempFormulaToLine.type == 1)
                        {
                            if (FSLB_TempFormulaToLine.L1 == FSLB_TempL && FSLB_TempL != null)
                                FSLB_TempFormulaToLine.addlable(c, FSLB_TempPath, 1, lable_type);
                            else if (FSLB_TempFormulaToLine.C1 == FSLB_TempC && FSLB_TempC != null)
                                FSLB_TempFormulaToLine.addlable(c, FSLB_TempPath, 2, lable_type);
                        }
                        else if(FSLB_TempFormulaToLine.type == 2)
                        {
                            if (FSLB_TempFormulaToLine.C1 == FSLB_TempC && FSLB_TempC != null)
                                FSLB_TempFormulaToLine.addlable(c, FSLB_TempPath, 1, lable_type);
                            else if (FSLB_TempFormulaToLine.C2 == FSLB_TempC && FSLB_TempC != null)
                                FSLB_TempFormulaToLine.addlable(c, FSLB_TempPath, 2, lable_type);
                        }
                    }
                }
                FormulaToL_Temp_Clean();
            }
        }
        private void MBRight_FormulaToL_DOWN(MouseEventArgs ev)
        {
            Point e = new Point((int)(ev.X / ZoomSize), (int)(ev.Y / ZoomSize));
            GraphLine line = FindLineByPoint(LineList, e);
            GraphCurve curve;int cindex;
            FindCurveByPoint(CurveList, e, out curve, out cindex);
            FormulaToLine ftl = FindFTLByPoint(FormulaToLineList, e);
            if (ftl != null)
            {
                Right_Temp_FTL = ftl;
                contextMenuStrip3.Show(MousePosition);
            }
            else if (line != null)
            {
                line.fix = !line.fix;
                Push_Undo_Data();
                Refresh();
            }
            else if (curve != null)
            {
                curve.fix = !curve.fix;
                Push_Undo_Data();
                Refresh();
            }
        }
        private void FormulaToL_Temp_Clean()
        {
            FSLB_TempFML = null;
            FSLB_TempP = null;
            FSLB_TempGP = null;
            FSLB_TempL = null;
            FSLB_TempC = null;
            FSLB_TempPath = null;
            FSLB_TempFormulaToLine = null;
            Right_Temp_FTL = null;
            FSLB_State = 0;
            new_bodysize = new double[29];
            fit_bodysize = false;
            Edit_FL_Temp_FML_oblistview_index = -1;
        }

        private void 調整所有版型ToolStripMenuItem_Click(object sender, EventArgs e)//公式
        {
            int nowselect = tabControl1.SelectedIndex;
            for(int i = 0; i < tabControl1.TabCount - 1; i++)
            {
                tabControl1.SelectedIndex = i;
                FitFormula();
            }
            tabControl1.SelectedIndex = nowselect;
        }
        private void 調整所有版型ToolStripMenuItem1_Click(object sender, EventArgs e)//身形
        {
            依照公式調整 f = new 依照公式調整(ClothStandardSize);
            if (f.ShowDialog() == DialogResult.OK)
            {
                new_bodysize = f.custom_size;
                List<double> shift = new List<double>();

                int nowselect = tabControl1.SelectedIndex;
                for (int j = 0; j < tabControl1.TabCount - 1; j++)
                {
                    tabControl1.SelectedIndex = j;


                    for (int i = 0; i < FormulaToLineList.Count; i++)
                    {
                        string s = "";
                        foreach (var ele in FormulaToLineList[i].fml.EleL)
                        {
                            s += ele.ToString();
                        }

                        var element_comvert = new List<Element>();
                        foreach (var ele in FormulaToLineList[i].formula_eleL)
                        {
                            if (ele.GetType().Equals(typeof(VariableElement)))
                            {
                                string temps = ele.ToString();
                                temps = temps.Remove(temps.Length - 1, 1);
                                temps = temps.Remove(0, 1);
                                int index = 0;
                                int.TryParse(temps, out index);
                                double v = new_bodysize[index - 1] - ClothStandardSize[index - 1];
                                element_comvert.Add(new NumberElement(v.ToString()));
                            }
                            else
                            {
                                element_comvert.Add(ele);
                            }
                        }
                        InfixToPostfix itp = new InfixToPostfix();
                        var PFele = itp.ConvertFromInfixToPostFix(element_comvert);
                        PostFixEvaluator pfe = new PostFixEvaluator();
                        double ans;
                        if (element_comvert.Count == 0)
                            ans = 0;
                        else
                            ans = pfe.Evaluate(PFele);

                        shift.Add(ans);
                    }
                    
                    調整差值確認 fo = new 調整差值確認(shift, FormulaToLineList);
                    if (fo.ShowDialog() == DialogResult.OK)
                    {
                        fit_bodysize = true;
                        FitFormula();
                    }
                    shift.Clear();
                }
                tabControl1.SelectedIndex = nowselect;
            }
        }
        private void FitFormulaButton_Click(object sender, EventArgs e)
        {
            fit_bodysize = false;
            FitFormula();
        }
        private void FitBodysizebutton_Click(object sender, EventArgs e)
        {
            /*
            輸入基礎尺寸 f = new 輸入基礎尺寸();
            f.Text = "輸入變更後尺寸";
            if(f.ShowDialog() == DialogResult.OK)
            {
                new_bodysize = f.Sizes;
                fit_bodysize = true;
                FitFormulaButton_Click(sender, e);
            }
            */
            依照公式調整 f = new 依照公式調整(ClothStandardSize);
            if (f.ShowDialog() == DialogResult.OK)
            {
                new_bodysize = f.custom_size;
                List<double> shift = new List<double>();
                for (int i = 0; i < FormulaToLineList.Count; i++)
                {
                    string s = "";
                    foreach (var ele in FormulaToLineList[i].fml.EleL)
                    {
                        s += ele.ToString();
                    }

                    var element_comvert = new List<Element>();
                    foreach (var ele in FormulaToLineList[i].formula_eleL)
                    {
                        if (ele.GetType().Equals(typeof(VariableElement)))
                        {
                            string temps = ele.ToString();
                            temps = temps.Remove(temps.Length - 1, 1);
                            temps = temps.Remove(0, 1);
                            int index = 0;
                            int.TryParse(temps, out index);
                            double v = new_bodysize[index - 1] - ClothStandardSize[index - 1];
                            element_comvert.Add(new NumberElement(v.ToString()));
                        }
                        else
                        {
                            element_comvert.Add(ele);
                        }
                    }
                    InfixToPostfix itp = new InfixToPostfix();
                    var PFele = itp.ConvertFromInfixToPostFix(element_comvert);
                    PostFixEvaluator pfe = new PostFixEvaluator();
                    double ans;
                    if (element_comvert.Count == 0)
                        ans = 0;
                    else
                        ans = pfe.Evaluate(PFele);

                    shift.Add(ans);
                }


                調整差值確認 fo = new 調整差值確認(shift, FormulaToLineList);
                if (fo.ShowDialog() == DialogResult.OK)
                {
                    fit_bodysize = true;
                    FitFormula();
                }
            }
        }
        private void FitFormula()
        {
            List<FormulaToLine> toRemove = new List<FormulaToLine>();
            foreach (var ftl in FormulaToLineList)
            {
                GraphGroup pa1 = null, pa2 = null;
                if (ftl.type == 0)
                {
                    pa1 = PathList.Find(x => x.L.Exists(y => y == ftl.L1));
                    pa2 = PathList.Find(x => x.L.Exists(y => y == ftl.L2));
                }
                else if (ftl.type == 1)
                {
                    pa1 = PathList.Find(x => x.L.Exists(y => y == ftl.L1));
                    pa2 = PathList.Find(x => x.C.Exists(y => y == ftl.C1));
                }
                else if (ftl.type == 2)
                {
                    pa1 = PathList.Find(x => x.C.Exists(y => y == ftl.C1));
                    pa2 = PathList.Find(x => x.C.Exists(y => y == ftl.C2));
                }
                if ((pa1 != pa2 && pa1 != null && pa2 != null) && ftl.unfixed_P1 == null)
                    toRemove.Add(ftl);
            }
            if (toRemove.Count != 0)
            {
                if (MessageBox.Show("將清除不在同一圖形的調整公式", "訊息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    foreach (var ftl in toRemove)
                        FormulaToLineList.Remove(ftl);
                }
                else
                    return;
            }
            //if (MessageBox.Show("將依照公式調整線條", "訊息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            //{
                foreach (var ftl in FormulaToLineList)
                {
                    if (ftl.formula_eleL.Count == 0)
                        continue;
                    var element_comvert = new List<Element>();
                    foreach (var ele in ftl.formula_eleL)
                    {
                        if (ele.GetType().Equals(typeof(VariableElement)))
                        {
                            VariableElement vele = (VariableElement)ele;
                            int index = vele.getIndex();
                            double v = ClothStandardSize[index - 1];
                            element_comvert.Add(new NumberElement(v.ToString()));
                        }
                        else
                        {
                            element_comvert.Add(ele);
                        }
                    }
                    InfixToPostfix itp = new InfixToPostfix();
                    var PFele = itp.ConvertFromInfixToPostFix(element_comvert);
                    PostFixEvaluator pfe = new PostFixEvaluator();
                    double len = Math.Sqrt(Math.Pow(ftl.anchor1.X - ftl.anchor2.X, 2) + Math.Pow(ftl.anchor1.Y - ftl.anchor2.Y, 2));
                    double target = pfe.Evaluate(PFele);
                    target *= 72;
                    target = target + (fit_bodysize ? len : 0);
                    double dist = Math.Sqrt(Math.Pow(ftl.anchor1.X - ftl.anchor2.X, 2) + Math.Pow(ftl.anchor1.Y - ftl.anchor2.Y, 2));
                    double shift = (target - dist);
                    PointF mid = new PointF((ftl.anchor1.X + ftl.anchor2.X) / 2, (ftl.anchor1.Y + ftl.anchor2.Y) / 2);

                    if (ftl.type < 3)
                    {
                        double shift1 = 0, shift2 = 0;
                        bool shift1_fix = ftl.get_fix(1),
                             shift2_fix = ftl.get_fix(2);
                        if (shift1_fix == true && shift2_fix == true)
                        {
                            shift1 = 0;
                            shift2 = 0;
                        }
                        else if (shift1_fix == false && shift2_fix == true)
                        {
                            shift1 = shift;
                        }
                        else if (shift1_fix == true && shift2_fix == false)
                        {
                            shift2 = shift;
                        }
                        else if (shift1_fix == false && shift2_fix == false)
                        {
                            string[] ts = ftl.prop12.Split(':');
                            double s1 = double.Parse(ts[0]),
                                   s2 = double.Parse(ts[1]),
                                   prop1 = s1 / (s1 + s2),
                                   prop2 = s2 / (s1 + s2);
                            shift1 = shift * prop1;
                            shift2 = shift * prop2;
                        }
                        double shift1_X, shift1_Y, shift2_X, shift2_Y;
                        if (ftl.mode == 0)
                        {
                            shift1_X = (ftl.anchor1.X - mid.X) > 0 ? shift1 : -shift1;
                            shift1_Y = 0;
                            shift2_X = (ftl.anchor2.X - mid.X) > 0 ? shift2 : -shift2;
                            shift2_Y = 0;
                        }
                        else if (ftl.mode == 1)
                        {
                            shift1_X = 0;
                            shift1_Y = (ftl.anchor1.Y - mid.Y) > 0 ? shift1 : -shift1;
                            shift2_X = 0;
                            shift2_Y = (ftl.anchor2.Y - mid.Y) > 0 ? shift2 : -shift2;
                        }
                        else
                        {
                            shift1_X = shift1;
                            shift1_Y = shift1;
                            shift2_X = shift2;
                            shift2_Y = shift2;
                        }
                        GraphGroup path1 = (ftl.type == 2 ? PathList.Find(x => x.C.Exists(y => y == ftl.C1)) : PathList.Find(x => x.L.Exists(y => y == ftl.L1)));
                        GraphGroup path2 = (ftl.type == 2 ? PathList.Find(x => x.C.Exists(y => y == ftl.C2)) :
                                            ftl.type == 1 ? PathList.Find(x => x.C.Exists(y => y == ftl.C1)) : PathList.Find(x => x.L.Exists(y => y == ftl.L2)));
                        GraphGroup path = path1 != null ? path1 : path2;
                        bool forward_tsfe1 = false, backward_tsfe1 = false, forward_tsfe2 = false, backward_tsfe2 = false,
                             forward_tLfC1 = false, backward_tLfC1 = false, forward_tLfC2 = false, backward_tLfC2 = false;
                        int forward_index1 = -1, backward_index1 = -1, forward_index2 = -1, backward_index2 = -1;
                        if (ftl.path_type == 1 || ftl.path_type == 3)
                        {
                            forward_tsfe1 = ftl.get_tst_fen(1, true, out forward_tLfC1, out forward_index1, path);
                            backward_tsfe1 = ftl.get_tst_fen(1, false, out backward_tLfC1, out backward_index1, path);
                        }
                        if (ftl.path_type == 2 || ftl.path_type == 3)
                        {
                            forward_tsfe2 = ftl.get_tst_fen(2, true, out forward_tLfC2, out forward_index2, path);
                            backward_tsfe2 = ftl.get_tst_fen(2, false, out backward_tLfC2, out backward_index2, path);
                        }
                        GraphGroup cutLable1 = new GraphGroup(), cutLable2 = new GraphGroup();
                        GraphGroup zlable1f, zlable1b, zlable2f, zlable2b;
                        #region copy lable
                        for (int i = 0; i < ftl.labled1.L.Count; i++)
                        {
                            if (ftl.labled1.L[i] != null)
                            {
                                GraphPoint p1 = cutLable1.P.Find(x => x.P.X == ftl.labled1.L[i].StartPoint.P.X && x.P.Y == ftl.labled1.L[i].StartPoint.P.Y),
                                           p2 = cutLable1.P.Find(x => x.P.X == ftl.labled1.L[i].EndPoint.P.X && x.P.Y == ftl.labled1.L[i].EndPoint.P.Y);
                                if (p1 == null)
                                {
                                    p1 = new GraphPoint(ftl.labled1.L[i].StartPoint.P.X, ftl.labled1.L[i].StartPoint.P.Y);
                                    cutLable1.P.Add(p1);
                                }
                                if (p2 == null)
                                {
                                    p2 = new GraphPoint(ftl.labled1.L[i].EndPoint.P.X, ftl.labled1.L[i].EndPoint.P.Y);
                                    cutLable1.P.Add(p2);
                                }
                                cutLable1.L.Add(new GraphLine(p1, p2));
                                cutLable1.C.Add(null);
                            }
                            else
                            {
                                GraphCurve tempc = new GraphCurve();
                                for (int j = 0; j < ftl.labled1.C[i].path.Count; j++)
                                {
                                    GraphPoint tempp = cutLable1.P.Find(x => x.P.X == ftl.labled1.C[i].path[j].P.X && x.P.Y == ftl.labled1.C[i].path[j].P.Y);
                                    if (tempp == null)
                                    {
                                        tempp = new GraphPoint(ftl.labled1.C[i].path[j].P.X, ftl.labled1.C[i].path[j].P.Y);
                                        cutLable1.P.Add(tempp);
                                    }
                                    tempc.path.Add(tempp);
                                    tempc.disFirst.Add(ftl.labled1.C[i].disFirst[j]);
                                    tempc.disSecond.Add(ftl.labled1.C[i].disSecond[j]);
                                    tempc.type.Add(ftl.labled1.C[i].type[j]);
                                }
                                cutLable1.C.Add(tempc);
                                cutLable1.L.Add(null);
                            }
                        }

                        for (int i = 0; i < ftl.labled2.L.Count; i++)
                        {
                            if (ftl.labled2.L[i] != null)
                            {
                                GraphPoint p1 = cutLable2.P.Find(x => x.P.X == ftl.labled2.L[i].StartPoint.P.X && x.P.Y == ftl.labled2.L[i].StartPoint.P.Y),
                                           p2 = cutLable2.P.Find(x => x.P.X == ftl.labled2.L[i].EndPoint.P.X && x.P.Y == ftl.labled2.L[i].EndPoint.P.Y);
                                if (p1 == null)
                                {
                                    p1 = new GraphPoint(ftl.labled2.L[i].StartPoint.P.X, ftl.labled2.L[i].StartPoint.P.Y);
                                    cutLable2.P.Add(p1);
                                }
                                if (p2 == null)
                                {
                                    p2 = new GraphPoint(ftl.labled2.L[i].EndPoint.P.X, ftl.labled2.L[i].EndPoint.P.Y);
                                    cutLable2.P.Add(p2);
                                }
                                cutLable2.L.Add(new GraphLine(p1, p2));
                                cutLable2.C.Add(null);
                            }
                            else
                            {
                                GraphCurve tempc = new GraphCurve();
                                for (int j = 0; j < ftl.labled2.C[i].path.Count; j++)
                                {
                                    GraphPoint tempp = cutLable2.P.Find(x => x.P.X == ftl.labled2.C[i].path[j].P.X && x.P.Y == ftl.labled2.C[i].path[j].P.Y);
                                    if (tempp == null)
                                    {
                                        tempp = new GraphPoint(ftl.labled2.C[i].path[j].P.X, ftl.labled2.C[i].path[j].P.Y);
                                        cutLable2.P.Add(tempp);
                                    }
                                    tempc.path.Add(tempp);
                                    tempc.disFirst.Add(ftl.labled2.C[i].disFirst[j]);
                                    tempc.disSecond.Add(ftl.labled2.C[i].disSecond[j]);
                                    tempc.type.Add(ftl.labled2.C[i].type[j]);
                                }
                                cutLable2.C.Add(tempc);
                                cutLable2.L.Add(null);
                            }
                        }
                        #endregion
                        for (int i = 0; i < cutLable1.P.Count; i++)
                            cutLable1.P[i].P = new PointF(cutLable1.P[i].P.X + (float)shift1_X, cutLable1.P[i].P.Y + (float)shift1_Y);
                        for (int i = 0; i < cutLable2.P.Count; i++)
                            cutLable2.P[i].P = new PointF(cutLable2.P[i].P.X + (float)shift2_X, cutLable2.P[i].P.Y + (float)shift2_Y);
                        bool check1, check2, check3, check4;
                        PointF lb1_forward_insert, lb1_backward_insert, lb2_forward_insert, lb2_backward_insert;
                        int lb1_forward_extend, lb1_backward_extend, lb2_forward_extend, lb2_backward_extend;
                        #region findinsert
                        if (shift1 != 0 && (ftl.path_type == 1 || ftl.path_type == 3))
                        {
                            if (forward_tLfC1)
                            {
                                bool b;
                                check1 = FindInsert_L_Half(path, ftl.labled1.L[0], cutLable1.L[0], true, out lb1_forward_insert, out b);
                                lb1_forward_extend = b ? 1 : 0;
                            }
                            else
                                check1 = FindInsert_C_Half(path, ftl.labled1.C[0], cutLable1.C[0], true, out lb1_forward_insert, out lb1_forward_extend);
                            if (backward_tLfC1)
                            {
                                bool b;
                                check2 = FindInsert_L_Half(path, ftl.labled1.L.Last(), cutLable1.L.Last(), false, out lb1_backward_insert, out b);
                                lb1_backward_extend = b ? 1 : 0;
                            }
                            else
                                check2 = FindInsert_C_Half(path, ftl.labled1.C.Last(), cutLable1.C.Last(), false, out lb1_backward_insert, out lb1_backward_extend);
                        }
                        else
                        {
                            check1 = true;
                            check2 = true;
                            lb1_backward_extend = 0;
                            lb1_forward_extend = 0;
                            lb1_forward_insert = new PointF();
                            lb1_backward_insert = new PointF();
                        }

                        if (shift2 != 0 && (ftl.path_type == 2 || ftl.path_type == 3))
                        {
                            if (forward_tLfC2)
                            {
                                bool b;
                                check3 = FindInsert_L_Half(path, ftl.labled2.L[0], cutLable2.L[0], true, out lb2_forward_insert, out b);
                                lb2_forward_extend = b ? 1 : 0;
                            }
                            else
                                check3 = FindInsert_C_Half(path, ftl.labled2.C[0], cutLable2.C[0], true, out lb2_forward_insert, out lb2_forward_extend);
                            if (backward_tLfC2)
                            {
                                bool b;
                                check4 = FindInsert_L_Half(path, ftl.labled2.L.Last(), cutLable2.L.Last(), false, out lb2_backward_insert, out b);
                                lb2_backward_extend = b ? 1 : 0;
                            }
                            else
                                check4 = FindInsert_C_Half(path, ftl.labled2.C.Last(), cutLable2.C.Last(), false, out lb2_backward_insert, out lb2_backward_extend);
                        }
                        else
                        {
                            check3 = true;
                            check4 = true;
                            lb2_backward_extend = 0;
                            lb2_forward_extend = 0;
                            lb2_forward_insert = new PointF();
                            lb2_backward_insert = new PointF();
                        }
                        #endregion
                        ftl.get_zoomlable_FB(1, out zlable1f, out zlable1b);
                        ftl.get_zoomlable_FB(2, out zlable2f, out zlable2b);
                        check1 = (zlable1f.L.Count != 0) ? true : check1;
                        check2 = (zlable1b.L.Count != 0) ? true : check2;
                        check3 = (zlable2f.L.Count != 0) ? true : check3;
                        check4 = (zlable2b.L.Count != 0) ? true : check4;

                        check1 = (ftl.path_type == 0 || ftl.path_type == 1) ? true : check1;
                        check2 = (ftl.path_type == 0 || ftl.path_type == 1) ? true : check2;
                        check3 = (ftl.path_type == 0 || ftl.path_type == 2) ? true : check3;
                        check4 = (ftl.path_type == 0 || ftl.path_type == 2) ? true : check4;
                        if (check1 && check2 && check3 && check4)
                        {
                            #region shift1
                            if (shift1 != 0)
                            {
                                for (int i = 0; i < cutLable1.L.Count; i++)
                                {
                                    if (cutLable1.L.Count == 1)
                                    {
                                        if (cutLable1.L[i] != null)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            for (int j = 1; j < cutLable1.C[i].path.Count - 1; j++)
                                            {
                                                ftl.labled1.C[i].path[j].P = new PointF(cutLable1.C[i].path[j].P.X, cutLable1.C[i].path[j].P.Y);
                                            }
                                        }
                                        continue;
                                    }
                                    else if (i == 0)
                                    {
                                        if (cutLable1.L[i] != null)
                                        {
                                            if (!forward_tsfe1)//t=skip s f=skip e
                                                ftl.labled1.L[i].StartPoint.P = new PointF(cutLable1.L[i].StartPoint.P.X, cutLable1.L[i].StartPoint.P.Y);
                                            else
                                                ftl.labled1.L[i].EndPoint.P = new PointF(cutLable1.L[i].EndPoint.P.X, cutLable1.L[i].EndPoint.P.Y);
                                        }
                                        else
                                        {
                                            for (int j = 0; j < cutLable1.C[i].path.Count; j++)
                                            {
                                                if (!forward_tsfe1 && j == cutLable1.C[i].path.Count - 1)//t=skip s f=skip e
                                                    continue;
                                                else if (j == 0)
                                                    continue;
                                                ftl.labled1.C[i].path[j].P = new PointF(cutLable1.C[i].path[j].P.X, cutLable1.C[i].path[j].P.Y);
                                            }
                                        }
                                        continue;
                                    }
                                    if (i == cutLable1.L.Count - 1)
                                    {
                                        if (cutLable1.L[i] != null)
                                        {
                                            if (!backward_tsfe1)//t=skip s f=skip e
                                                ftl.labled1.L[i].StartPoint.P = new PointF(cutLable1.L[i].StartPoint.P.X, cutLable1.L[i].StartPoint.P.Y);
                                            else
                                                ftl.labled1.L[i].EndPoint.P = new PointF(cutLable1.L[i].EndPoint.P.X, cutLable1.L[i].EndPoint.P.Y);
                                        }
                                        else
                                        {
                                            for (int j = 0; j < cutLable1.C[i].path.Count; j++)
                                            {
                                                if (!backward_tsfe1 && j == cutLable1.C[i].path.Count - 1)//t=skip s f=skip e
                                                    continue;
                                                else if (j == 0)
                                                    continue;
                                                ftl.labled1.C[i].path[j].P = new PointF(cutLable1.C[i].path[j].P.X, cutLable1.C[i].path[j].P.Y);
                                            }
                                        }
                                    }
                                    if (cutLable1.L[i] != null)
                                    {
                                        ftl.labled1.L[i].StartPoint.P = new PointF(cutLable1.L[i].StartPoint.P.X, cutLable1.L[i].StartPoint.P.Y);
                                        ftl.labled1.L[i].EndPoint.P = new PointF(cutLable1.L[i].EndPoint.P.X, cutLable1.L[i].EndPoint.P.Y);
                                    }
                                    else
                                    {
                                        for (int j = 0; j < cutLable1.C[i].path.Count; j++)
                                        {
                                            ftl.labled1.C[i].path[j].P = new PointF(cutLable1.C[i].path[j].P.X, cutLable1.C[i].path[j].P.Y);
                                        }
                                    }
                                }
                                //Refresh();
                                if (ftl.path_type == 0 || ftl.path_type == 2)
                                {
                                    for (int i = 0; i < cutLable1.L.Count; i++)
                                    {
                                        if (cutLable1.L[i] != null)
                                        {
                                            ftl.labled1.L[i].StartPoint.P = new PointF(cutLable1.L[i].StartPoint.P.X, cutLable1.L[i].StartPoint.P.Y);
                                            ftl.labled1.L[i].EndPoint.P = new PointF(cutLable1.L[i].EndPoint.P.X, cutLable1.L[i].EndPoint.P.Y);
                                        }
                                        else
                                        {
                                            for (int j = 0; j < cutLable1.C[i].path.Count; j++)
                                            {
                                                ftl.labled1.C[i].path[j].P = new PointF(cutLable1.C[i].path[j].P.X, cutLable1.C[i].path[j].P.Y);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (zlable1f.L.Count != 0)
                                    {
                                        GraphPoint p = forward_tLfC1 ? (forward_tsfe1 ? ftl.labled1.L[0].StartPoint : ftl.labled1.L[0].EndPoint) :
                                                                       (forward_tsfe1 ? ftl.labled1.C[0].path[0] : ftl.labled1.C[0].path.Last());
                                        FtoL_Zoom_Shift(zlable1f, new PointF((float)shift1_X, (float)shift1_Y), p);
                                    }
                                    else if (forward_tLfC1)
                                    {
                                        if (forward_tsfe1)
                                            FtoL_Line_Fit_Shift_startpoint(ftl.labled1.L[0], path, forward_index1, lb1_forward_insert, lb1_forward_extend == 1);
                                        else
                                            FtoL_Line_Fit_Shift_endpoint(ftl.labled1.L[0], path, forward_index1, lb1_forward_insert, lb1_forward_extend == 1);
                                    }
                                    else
                                    {
                                        if (forward_tsfe1)
                                            FtoL_Curve_Fit_Shift_startpoint(ftl.labled1.C[0], cutLable1.C[0], path, forward_index1, lb1_forward_insert, lb1_forward_extend);
                                        else
                                            FtoL_Curve_Fit_Shift_endpoint(ftl.labled1.C[0], cutLable1.C[0], path, forward_index1, lb1_forward_insert, lb1_forward_extend);
                                    }
                                    if (zlable1b.L.Count != 0)
                                    {
                                        GraphPoint p = backward_tLfC1 ? (backward_tsfe1 ? ftl.labled1.L[0].StartPoint : ftl.labled1.L[0].EndPoint) :
                                                                       (backward_tsfe1 ? ftl.labled1.C[0].path[0] : ftl.labled1.C[0].path.Last());
                                        FtoL_Zoom_Shift(zlable1b, new PointF((float)shift1_X, (float)shift1_Y), p);
                                    }
                                    else if (backward_tLfC1)
                                    {
                                        if (backward_tsfe1)
                                            FtoL_Line_Fit_Shift_startpoint(ftl.labled1.L.Last(), path, backward_index1, lb1_backward_insert, lb1_backward_extend == 1);
                                        else
                                            FtoL_Line_Fit_Shift_endpoint(ftl.labled1.L.Last(), path, backward_index1, lb1_backward_insert, lb1_backward_extend == 1);
                                    }
                                    else
                                    {
                                        if (backward_tsfe1)
                                            FtoL_Curve_Fit_Shift_startpoint(ftl.labled1.C.Last(), cutLable1.C.Last(), path, backward_index1, lb1_backward_insert, lb1_backward_extend);
                                        else
                                            FtoL_Curve_Fit_Shift_endpoint(ftl.labled1.C.Last(), cutLable1.C.Last(), path, backward_index1, lb1_backward_insert, lb1_backward_extend);
                                    }
                                }

                            }
                            #endregion
                            #region shift2
                            if (shift2 != 0)
                            {
                                for (int i = 0; i < cutLable2.L.Count; i++)
                                {
                                    if (cutLable2.L.Count == 1)
                                    {
                                        if (cutLable2.L[i] != null)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            for (int j = 1; j < cutLable2.C[i].path.Count - 1; j++)
                                            {
                                                ftl.labled2.C[i].path[j].P = new PointF(cutLable2.C[i].path[j].P.X, cutLable2.C[i].path[j].P.Y);
                                            }
                                        }
                                        continue;
                                    }
                                    else if (i == 0)
                                    {
                                        if (cutLable2.L[i] != null)
                                        {
                                            if (!forward_tsfe2)//t=skip s f=skip e
                                                ftl.labled2.L[i].StartPoint.P = new PointF(cutLable2.L[i].StartPoint.P.X, cutLable2.L[i].StartPoint.P.Y);
                                            else
                                                ftl.labled2.L[i].EndPoint.P = new PointF(cutLable2.L[i].EndPoint.P.X, cutLable2.L[i].EndPoint.P.Y);
                                        }
                                        else
                                        {
                                            for (int j = 0; j < cutLable2.C[i].path.Count; j++)
                                            {
                                                if (!forward_tsfe2 && j == cutLable2.C[i].path.Count - 1)//t=skip s f=skip e
                                                    continue;
                                                else if (j == 0)
                                                    continue;
                                                ftl.labled2.C[i].path[j].P = new PointF(cutLable2.C[i].path[j].P.X, cutLable2.C[i].path[j].P.Y);
                                            }
                                        }
                                        continue;
                                    }
                                    if (i == cutLable2.L.Count - 1)
                                    {
                                        if (cutLable2.L[i] != null)
                                        {
                                            if (!backward_tsfe2)//t=skip s f=skip e
                                                ftl.labled2.L[i].StartPoint.P = new PointF(cutLable2.L[i].StartPoint.P.X, cutLable2.L[i].StartPoint.P.Y);
                                            else
                                                ftl.labled2.L[i].EndPoint.P = new PointF(cutLable2.L[i].EndPoint.P.X, cutLable2.L[i].EndPoint.P.Y);
                                        }
                                        else
                                        {
                                            for (int j = 0; j < cutLable2.C[i].path.Count; j++)
                                            {
                                                if (!backward_tsfe2 && j == cutLable2.C[i].path.Count - 1)//t=skip s f=skip e
                                                    continue;
                                                else if (j == 0)
                                                    continue;
                                                ftl.labled2.C[i].path[j].P = new PointF(cutLable2.C[i].path[j].P.X, cutLable2.C[i].path[j].P.Y);
                                            }
                                        }
                                    }
                                    if (cutLable2.L[i] != null)
                                    {
                                        ftl.labled2.L[i].StartPoint.P = new PointF(cutLable2.L[i].StartPoint.P.X, cutLable2.L[i].StartPoint.P.Y);
                                        ftl.labled2.L[i].EndPoint.P = new PointF(cutLable2.L[i].EndPoint.P.X, cutLable2.L[i].EndPoint.P.Y);
                                    }
                                    else
                                    {
                                        for (int j = 0; j < cutLable2.C[i].path.Count; j++)
                                        {
                                            ftl.labled2.C[i].path[j].P = new PointF(cutLable2.C[i].path[j].P.X, cutLable2.C[i].path[j].P.Y);
                                        }
                                    }
                                }
                                if (ftl.path_type == 0 || ftl.path_type == 1)
                                {
                                    for (int i = 0; i < cutLable2.L.Count; i++)
                                    {
                                        if (cutLable2.L[i] != null)
                                        {
                                            ftl.labled2.L[i].StartPoint.P = new PointF(cutLable2.L[i].StartPoint.P.X, cutLable2.L[i].StartPoint.P.Y);
                                            ftl.labled2.L[i].EndPoint.P = new PointF(cutLable2.L[i].EndPoint.P.X, cutLable2.L[i].EndPoint.P.Y);
                                        }
                                        else
                                        {
                                            for (int j = 0; j < cutLable2.C[i].path.Count; j++)
                                            {
                                                ftl.labled2.C[i].path[j].P = new PointF(cutLable2.C[i].path[j].P.X, cutLable2.C[i].path[j].P.Y);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (zlable2f.L.Count != 0)
                                    {
                                        GraphPoint p = forward_tLfC2 ? (forward_tsfe2 ? ftl.labled2.L[0].StartPoint : ftl.labled2.L[0].EndPoint) :
                                                                       (forward_tsfe2 ? ftl.labled2.C[0].path[0] : ftl.labled2.C[0].path.Last());
                                        FtoL_Zoom_Shift(zlable2f, new PointF((float)shift2_X, (float)shift2_Y), p);
                                    }
                                    else if (forward_tLfC2)
                                    {
                                        if (forward_tsfe2)
                                            FtoL_Line_Fit_Shift_startpoint(ftl.labled2.L[0], path, forward_index2, lb2_forward_insert, lb2_forward_extend == 1);
                                        else
                                            FtoL_Line_Fit_Shift_endpoint(ftl.labled2.L[0], path, forward_index2, lb2_forward_insert, lb2_forward_extend == 1);
                                    }
                                    else
                                    {
                                        if (forward_tsfe2)
                                            FtoL_Curve_Fit_Shift_startpoint(ftl.labled2.C[0], cutLable2.C[0], path, forward_index2, lb2_forward_insert, lb2_forward_extend);
                                        else
                                            FtoL_Curve_Fit_Shift_endpoint(ftl.labled2.C[0], cutLable2.C[0], path, forward_index2, lb2_forward_insert, lb2_forward_extend);
                                    }
                                    if (zlable2b.L.Count != 0)
                                    {
                                        GraphPoint p = backward_tLfC2 ? (backward_tsfe2 ? ftl.labled2.L[0].StartPoint : ftl.labled2.L[0].EndPoint) :
                                                                       (backward_tsfe2 ? ftl.labled2.C[0].path[0] : ftl.labled2.C[0].path.Last());
                                        FtoL_Zoom_Shift(zlable2b, new PointF((float)shift2_X, (float)shift2_Y), p);
                                    }
                                    else if (backward_tLfC2)
                                    {
                                        if (backward_tsfe2)
                                            FtoL_Line_Fit_Shift_startpoint(ftl.labled2.L.Last(), path, backward_index2, lb2_backward_insert, lb2_backward_extend == 1);
                                        else
                                            FtoL_Line_Fit_Shift_endpoint(ftl.labled2.L.Last(), path, backward_index2, lb2_backward_insert, lb2_backward_extend == 1);
                                    }
                                    else
                                    {
                                        if (backward_tsfe2)
                                            FtoL_Curve_Fit_Shift_startpoint(ftl.labled2.C.Last(), cutLable2.C[0], path, backward_index2, lb2_backward_insert, lb2_backward_extend);
                                        else
                                            FtoL_Curve_Fit_Shift_endpoint(ftl.labled2.C.Last(), cutLable2.C[0], path, backward_index2, lb2_backward_insert, lb2_backward_extend);
                                    }
                                }

                            }
                            #endregion
                            if (path != null)
                            {
                                path.P = new List<GraphPoint>();
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
                            }
                        }
                    }
                    else if (ftl.type == 3 || ftl.type == 4)
                    {

                        if (ftl.mode == 0)
                            ftl.unfixed_P1.P.X += (float)shift * (ftl.unfixed_P1.P.X - mid.X > 0 ? 1 : -1);
                        else if (ftl.mode == 1)
                            ftl.unfixed_P1.P.Y += (float)shift * (ftl.unfixed_P1.P.Y - mid.Y > 0 ? 1 : -1);
                    }
                    else if (ftl.type == 5)
                    {
                        string[] ts = ftl.prop12.Split(':');
                        double s1 = double.Parse(ts[0]),
                               s2 = double.Parse(ts[1]),
                               prop1 = s1 / (s1 + s2),
                               prop2 = s2 / (s1 + s2);
                        if (ftl.mode == 0)
                        {
                            ftl.unfixed_P1.P.X += (float)(shift * prop1 * (ftl.unfixed_P1.P.X - mid.X > 0 ? 1 : -1));
                            ftl.unfixed_P2.P.X += (float)(shift * prop2 * (ftl.unfixed_P2.P.X - mid.X > 0 ? 1 : -1));
                        }
                        else if (ftl.mode == 1)
                        {
                            ftl.unfixed_P1.P.Y += (float)(shift * prop1 * (ftl.unfixed_P1.P.Y - mid.Y > 0 ? 1 : -1));
                            ftl.unfixed_P2.P.Y += (float)(shift * prop2 * (ftl.unfixed_P2.P.Y - mid.Y > 0 ? 1 : -1));
                        }
                    }
                }
                List<FormulaToLine> del = new List<FormulaToLine>();
                foreach (var ftl in FormulaToLineList)
                {
                    if (!ftl.in_List(PointsList, LineList, CurveList))
                        del.Add(ftl);
                }
                foreach (var d in del)
                    FormulaToLineList.Remove(d);
                PointCombine();
                Push_Undo_Data();
                Refresh();
            //}
        }
        private void FtoL_Line_Fit_Shift(GraphLine L ,GraphGroup path, int index1, PointF move_to1, bool extend1, int index2, PointF move_to2, bool extend2)
        {
            if (path.L[index1] != null)
            {
                L.StartPoint.P = move_to1;
            }
            else
            {
                if (extend1)
                {
                    GraphCurve c = path.C[index1];
                    if (c.path[0] == L.StartPoint)
                    {
                        PointF cp1 = new PointF((c.path[0].P.X - move_to1.X) / 3, (c.path[0].P.Y - move_to1.Y) / 3);
                        PointF cp2 = new PointF((move_to1.X - c.path[0].P.X) / 3, (move_to1.Y - c.path[0].P.Y) / 3);
                        GraphPoint newp = new GraphPoint(move_to1.X, move_to1.Y);
                        newp.Relative += 2;
                        path.P.Remove(L.StartPoint);
                        L.StartPoint = newp;
                        path.P.Add(newp);
                        c.path[0].Relative--;
                        c.path.Insert(0, newp);
                        c.disFirst.Insert(0, cp2);
                        c.disSecond.Insert(0, cp1);
                        c.type.Insert(0, 0);
                        c.disFirst[1] = new PointF(cp2.X, cp2.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                        AdjustCindex(c);
                    }
                    else
                    {
                        PointF cp1 = new PointF((move_to1.X - c.path.Last().P.X) / 3, (move_to1.Y - c.path.Last().P.Y) / 3);
                        PointF cp2 = new PointF((c.path.Last().P.X - move_to1.X) / 3, (c.path.Last().P.Y - move_to1.Y) / 3);
                        GraphPoint newp = new GraphPoint(move_to1.X, move_to1.Y);
                        newp.Relative += 2;
                        path.P.Remove(L.StartPoint);
                        L.StartPoint = newp;
                        path.P.Add(newp);
                        c.path.Last().Relative--;
                        c.path.Add(newp);
                        c.disFirst.Add(cp2);
                        c.disSecond.Add(cp1);
                        c.type.Add(0);
                        c.disSecond[c.disSecond.Count - 2] = new PointF(cp1.X, cp1.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                    }
                }
                else
                {
                    GraphCurve c = path.C[index1];
                    if (c.path[0] == L.StartPoint)
                    {
                        GraphCurve tempc = CurveInsert(c, 1, new GraphPoint(move_to1.X, move_to1.Y));
                        c.disFirst[0] = tempc.disFirst[1];
                        c.disSecond[0] = tempc.disSecond[1];
                        c.path[0].P = tempc.path[1].P;
                        c.disFirst[1] = tempc.disFirst[2];
                        c.disSecond[1] = tempc.disSecond[2];
                    }
                    else
                    {
                        GraphCurve tempc = CurveInsert(c, c.path.Count - 1, new GraphPoint(move_to1.X, move_to1.Y));
                        int last = c.path.Count - 1;
                        c.disFirst[last] = tempc.disFirst[last];
                        c.disSecond[last] = tempc.disSecond[last];
                        c.path[last].P = tempc.path[last].P;
                        c.disFirst[last - 1] = tempc.disFirst[last - 1];
                        c.disSecond[last - 1] = tempc.disSecond[last - 1];
                    }
                }
            }

            if (path.L[index2] != null)
            {
                L.EndPoint.P = move_to2;
            }
            else
            {
                if (extend2)
                {
                    GraphCurve c = path.C[index2];
                    if (c.path[0] == L.EndPoint)
                    {
                        PointF cp1 = new PointF((c.path[0].P.X - move_to2.X) / 3, (c.path[0].P.Y - move_to2.Y) / 3);
                        PointF cp2 = new PointF((move_to2.X - c.path[0].P.X) / 3, (move_to2.Y - c.path[0].P.Y) / 3);
                        GraphPoint newp = new GraphPoint(move_to2.X, move_to2.Y);
                        newp.Relative += 2;
                        path.P.Remove(L.EndPoint);
                        L.EndPoint = newp;
                        path.P.Add(newp);
                        c.path[0].Relative--;
                        c.path.Insert(0, newp);
                        c.disFirst.Insert(0, cp2);
                        c.disSecond.Insert(0, cp1);
                        c.type.Insert(0, 0);
                        c.disFirst[1] = new PointF(cp2.X, cp2.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                        AdjustCindex(c);
                    }
                    else
                    {
                        PointF cp1 = new PointF((move_to2.X - c.path.Last().P.X) / 3, (move_to2.Y - c.path.Last().P.Y) / 3);
                        PointF cp2 = new PointF((c.path.Last().P.X - move_to2.X) / 3, (c.path.Last().P.Y - move_to2.Y) / 3);
                        GraphPoint newp = new GraphPoint(move_to2.X, move_to2.Y);
                        newp.Relative += 2;
                        path.P.Remove(L.EndPoint);
                        L.EndPoint = newp;
                        path.P.Add(newp);
                        c.path.Last().Relative--;
                        c.path.Add(newp);
                        c.disFirst.Add(cp2);
                        c.disSecond.Add(cp1);
                        c.type.Add(0);
                        c.disSecond[c.disSecond.Count - 2] = new PointF(cp1.X, cp1.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                    }
                }
                else
                {
                    GraphCurve c = path.C[index2];
                    if (c.path[0] == L.EndPoint)
                    {
                        GraphCurve tempc = CurveInsert(c, 1, new GraphPoint(move_to2.X, move_to2.Y));
                        c.disFirst[0] = tempc.disFirst[1];
                        c.disSecond[0] = tempc.disSecond[1];
                        c.path[0].P = tempc.path[1].P;
                        c.disFirst[1] = tempc.disFirst[2];
                        c.disSecond[1] = tempc.disSecond[2];
                    }
                    else
                    {
                        GraphCurve tempc = CurveInsert(c, c.path.Count - 1, new GraphPoint(move_to2.X, move_to2.Y));
                        int last = c.path.Count - 1;
                        c.disFirst[last] = tempc.disFirst[last];
                        c.disSecond[last] = tempc.disSecond[last];
                        c.path[last].P = tempc.path[last].P;
                        c.disFirst[last - 1] = tempc.disFirst[last - 1];
                        c.disSecond[last - 1] = tempc.disSecond[last - 1];
                    }
                }
            }
        }
        private void FtoL_Line_Fit_Shift_startpoint(GraphLine L, GraphGroup path, int index1, PointF move_to1, bool extend1)
        {
            if (path.L[index1] != null)
            {
                L.StartPoint.P = move_to1;
            }
            else
            {
                if (extend1)
                {
                    GraphCurve c = path.C[index1];
                    if (c.path[0] == L.StartPoint)
                    {
                        PointF cp1 = new PointF((c.path[0].P.X - move_to1.X) / 3, (c.path[0].P.Y - move_to1.Y) / 3);
                        PointF cp2 = new PointF((move_to1.X - c.path[0].P.X) / 3, (move_to1.Y - c.path[0].P.Y) / 3);
                        GraphPoint newp = new GraphPoint(move_to1.X, move_to1.Y);
                        newp.Relative += 2;
                        path.P.Remove(L.StartPoint);
                        L.StartPoint = newp;
                        path.P.Add(newp);
                        c.path[0].Relative--;
                        c.path.Insert(0, newp);
                        c.disFirst.Insert(0, cp2);
                        c.disSecond.Insert(0, cp1);
                        c.type.Insert(0, 0);
                        c.disFirst[1] = new PointF(cp2.X, cp2.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                        AdjustCindex(c);
                    }
                    else
                    {
                        PointF cp1 = new PointF((move_to1.X - c.path.Last().P.X) / 3, (move_to1.Y - c.path.Last().P.Y) / 3);
                        PointF cp2 = new PointF((c.path.Last().P.X - move_to1.X) / 3, (c.path.Last().P.Y - move_to1.Y) / 3);
                        GraphPoint newp = new GraphPoint(move_to1.X, move_to1.Y);
                        newp.Relative += 2;
                        path.P.Remove(L.StartPoint);
                        L.StartPoint = newp;
                        path.P.Add(newp);
                        c.path.Last().Relative--;
                        c.path.Add(newp);
                        c.disFirst.Add(cp2);
                        c.disSecond.Add(cp1);
                        c.type.Add(0);
                        c.disSecond[c.disSecond.Count - 2] = new PointF(cp1.X, cp1.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                    }
                }
                else
                {
                    GraphCurve c = path.C[index1];
                    if (c.path[0] == L.StartPoint)
                    {
                        GraphCurve tempc = CurveInsert(c, 1, new GraphPoint(move_to1.X, move_to1.Y));
                        c.disFirst[0] = tempc.disFirst[1];
                        c.disSecond[0] = tempc.disSecond[1];
                        c.path[0].P = tempc.path[1].P;
                        c.disFirst[1] = tempc.disFirst[2];
                        c.disSecond[1] = tempc.disSecond[2];
                    }
                    else
                    {
                        GraphCurve tempc = CurveInsert(c, c.path.Count - 1, new GraphPoint(move_to1.X, move_to1.Y));
                        int last = c.path.Count - 1;
                        c.disFirst[last] = tempc.disFirst[last];
                        c.disSecond[last] = tempc.disSecond[last];
                        c.path[last].P = tempc.path[last].P;
                        c.disFirst[last - 1] = tempc.disFirst[last - 1];
                        c.disSecond[last - 1] = tempc.disSecond[last - 1];
                    }
                }
            }
        }
        private void FtoL_Line_Fit_Shift_endpoint(GraphLine L, GraphGroup path, int index2, PointF move_to2, bool extend2)
        {
            if (path.L[index2] != null)
            {
                L.EndPoint.P = move_to2;
            }
            else
            {
                if (extend2)
                {
                    GraphCurve c = path.C[index2];
                    if (c.path[0] == L.EndPoint)
                    {
                        PointF cp1 = new PointF((c.path[0].P.X - move_to2.X) / 3, (c.path[0].P.Y - move_to2.Y) / 3);
                        PointF cp2 = new PointF((move_to2.X - c.path[0].P.X) / 3, (move_to2.Y - c.path[0].P.Y) / 3);
                        GraphPoint newp = new GraphPoint(move_to2.X, move_to2.Y);
                        newp.Relative += 2;
                        path.P.Remove(L.EndPoint);
                        L.EndPoint = newp;
                        path.P.Add(newp);
                        c.path[0].Relative--;
                        c.path.Insert(0, newp);
                        c.disFirst.Insert(0, cp2);
                        c.disSecond.Insert(0, cp1);
                        c.type.Insert(0, 0);
                        c.disFirst[1] = new PointF(cp2.X, cp2.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                        AdjustCindex(c);
                    }
                    else
                    {
                        PointF cp1 = new PointF((move_to2.X - c.path.Last().P.X) / 3, (move_to2.Y - c.path.Last().P.Y) / 3);
                        PointF cp2 = new PointF((c.path.Last().P.X - move_to2.X) / 3, (c.path.Last().P.Y - move_to2.Y) / 3);
                        GraphPoint newp = new GraphPoint(move_to2.X, move_to2.Y);
                        newp.Relative += 2;
                        path.P.Remove(L.EndPoint);
                        L.EndPoint = newp;
                        path.P.Add(newp);
                        c.path.Last().Relative--;
                        c.path.Add(newp);
                        c.disFirst.Add(cp2);
                        c.disSecond.Add(cp1);
                        c.type.Add(0);
                        c.disSecond[c.disSecond.Count - 2] = new PointF(cp1.X, cp1.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                    }
                }
                else
                {
                    GraphCurve c = path.C[index2];
                    if (c.path[0] == L.EndPoint)
                    {
                        GraphCurve tempc = CurveInsert(c, 1, new GraphPoint(move_to2.X, move_to2.Y));
                        c.disFirst[0] = tempc.disFirst[1];
                        c.disSecond[0] = tempc.disSecond[1];
                        c.path[0].P = tempc.path[1].P;
                        c.disFirst[1] = tempc.disFirst[2];
                        c.disSecond[1] = tempc.disSecond[2];
                    }
                    else
                    {
                        GraphCurve tempc = CurveInsert(c, c.path.Count - 1, new GraphPoint(move_to2.X, move_to2.Y));
                        int last = c.path.Count - 1;
                        c.disFirst[last] = tempc.disFirst[last];
                        c.disSecond[last] = tempc.disSecond[last];
                        c.path[last].P = tempc.path[last].P;
                        c.disFirst[last - 1] = tempc.disFirst[last - 1];
                        c.disSecond[last - 1] = tempc.disSecond[last - 1];
                    }
                }
            }
        }
        private void FtoL_Curve_Fit_Shift(GraphCurve C,GraphCurve CutC, GraphGroup path, int index1, PointF move_to1, int extend1, int index2, PointF move_to2, int extend2)
        {// 0=兩線內 1=延長cutC 2=延長相鄰C 3=都延長
            for (int i = 1; i < C.path.Count - 1; i++)
            {
                C.path[i].P = CutC.path[i].P;
            }
            if (path.L[index1] != null)
            {
                if (extend1 == 1)
                {
                    GraphLine l = path.L[index1];
                    if (C.path[0] == l.StartPoint)
                    {
                        GraphPoint extrap = new GraphPoint(CutC.path[0].P.X, CutC.path[0].P.Y);
                        C.path[0] = extrap;
                        extrap.Relative++;
                        extrap.name = extrap.check_name(PointsList);
                        PointsList.Add(extrap);
                        PointF cp1 = new PointF((CutC.path[0].P.X - move_to1.X) / 3, (CutC.path[0].P.Y - move_to1.Y) / 3);
                        PointF cp2 = new PointF((move_to1.X - CutC.path[0].P.X) / 3, (move_to1.Y - CutC.path[0].P.Y) / 3);
                        GraphPoint newp = new GraphPoint(move_to1.X, move_to1.Y);
                        newp.Relative += 2;
                        path.P.Remove(l.StartPoint);
                        l.StartPoint = newp;
                        path.P.Add(newp);
                        C.path[0].Relative--;
                        C.path.Insert(0, newp);
                        C.disFirst.Insert(0, cp2);
                        C.disSecond.Insert(0, cp1);
                        C.type.Insert(0, 0);
                        AdjustCindex(C);
                        C.disFirst[1] = new PointF(cp2.X, cp2.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                    }
                    else if(C.path[0] == l.EndPoint)
                    {
                        GraphPoint extrap = new GraphPoint(CutC.path[0].P.X, CutC.path[0].P.Y);
                        C.path[0] = extrap;
                        extrap.Relative++;
                        extrap.name = extrap.check_name(PointsList);
                        PointsList.Add(extrap);
                        PointF cp1 = new PointF((CutC.path[0].P.X - move_to1.X) / 3, (CutC.path[0].P.Y - move_to1.Y) / 3);
                        PointF cp2 = new PointF((move_to1.X - CutC.path[0].P.X) / 3, (move_to1.Y - CutC.path[0].P.Y) / 3);
                        GraphPoint newp = new GraphPoint(move_to1.X, move_to1.Y);
                        newp.Relative += 2;
                        path.P.Remove(l.EndPoint);
                        l.EndPoint = newp;
                        path.P.Add(newp);
                        C.path[0].Relative--;
                        C.path.Insert(0, newp);
                        C.disFirst.Insert(0, cp2);
                        C.disSecond.Insert(0, cp1);
                        C.type.Insert(0, 0);
                        AdjustCindex(C);
                        C.disFirst[1] = new PointF(cp2.X, cp2.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                    }
                }
                else
                {
                    GraphCurve tempc = CurveInsert(CutC, 1, new GraphPoint(move_to1.X, move_to1.Y));
                    tempc.path.RemoveAt(0);
                    tempc.disFirst.RemoveAt(0);
                    tempc.disSecond.RemoveAt(0);

                    C.path[0].P = tempc.path[0].P;
                    C.disSecond[0] = tempc.disSecond[0];
                    C.disFirst[1] = tempc.disFirst[1];

                    CutC = tempc;
                }
            }
            else
            {
                GraphCurve pathc = path.C[index1];
                if (extend1 == 0)//0=兩線插入
                {
                    #region type0
                    if (C.path[0] == pathc.path[0])
                    {
                        GraphCurve cutpathc = CurveInsert(CutC, 1, new GraphPoint(move_to1.X, move_to1.Y));
                        cutpathc.path.RemoveAt(0);
                        cutpathc.disFirst.RemoveAt(0);
                        cutpathc.disSecond.RemoveAt(0);
                        GraphCurve temppathc = CurveInsert(pathc, 1, new GraphPoint(move_to1.X, move_to1.Y));
                        temppathc.path.RemoveAt(0);
                        temppathc.disFirst.RemoveAt(0);
                        temppathc.disSecond.RemoveAt(0);
                        C.path[0].P = cutpathc.path[0].P;
                        C.disSecond[0] = cutpathc.disSecond[0];
                        C.disFirst[1] = cutpathc.disFirst[1];
                        CutC = cutpathc;
                        pathc.path[0].P = temppathc.path[0].P;
                        pathc.disSecond[0] = temppathc.disSecond[0];
                        pathc.disFirst[1] = temppathc.disFirst[1];
                    }
                    else
                    {
                        GraphCurve cutpathc = CurveInsert(CutC, 1, new GraphPoint(move_to1.X, move_to1.Y));
                        cutpathc.path.RemoveAt(0);
                        cutpathc.disFirst.RemoveAt(0);
                        cutpathc.disSecond.RemoveAt(0);
                        GraphCurve temppathc = CurveInsert(pathc, pathc.path.Count - 1, new GraphPoint(move_to1.X, move_to1.Y));
                        temppathc.path.RemoveAt(pathc.path.Count - 1);
                        temppathc.disFirst.RemoveAt(pathc.disFirst.Count - 1);
                        temppathc.disSecond.RemoveAt(pathc.disSecond.Count - 1);
                        C.path[0].P = cutpathc.path[0].P;
                        C.disSecond[0] = cutpathc.disSecond[0];
                        C.disFirst[1] = cutpathc.disFirst[1];
                        CutC = cutpathc;
                        pathc.path[pathc.path.Count - 1].P = temppathc.path[pathc.path.Count - 1].P;
                        pathc.disSecond[pathc.path.Count - 1] = temppathc.disSecond[pathc.path.Count - 1];
                        pathc.disFirst[pathc.path.Count - 2] = temppathc.disFirst[pathc.path.Count - 2];
                    }
                    #endregion
                }
                else if (extend1 == 1)//1=延長cutC 插入pathc
                {
                    #region type1
                    if (C.path[0] == pathc.path[0])
                    {
                        PointF cp1 = new PointF((move_to1.X - C.path[0].P.X) / 3, (move_to1.Y - C.path[0].P.Y) / 3);
                        PointF cp2 = new PointF((C.path[0].P.X - move_to1.X) / 3, (C.path[0].P.Y - move_to1.Y) / 3);
                        GraphCurve temppathc = CurveInsert(pathc, 1, new GraphPoint(move_to1.X, move_to1.Y));

                        temppathc.path.RemoveAt(0);
                        temppathc.disFirst.RemoveAt(0);
                        temppathc.disSecond.RemoveAt(0);
                        GraphPoint newp = new GraphPoint(move_to1.X, move_to1.Y);

                        newp.Relative += 2;
                        path.P.Remove(pathc.path[0]);
                        pathc.path[0] = newp;
                        path.P.Add(newp);
                        C.path[0].Relative--;
                        C.path.Insert(0, newp);
                        C.disFirst.Insert(0, cp2);
                        C.disSecond.Insert(0, cp1);
                        C.type.Insert(0, 0);
                        C.disFirst[1] = new PointF(cp2.X, cp2.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                        AdjustCindex(C);

                        pathc.path[0].P = temppathc.path[0].P;
                        pathc.disSecond[0] = temppathc.disSecond[0];
                        pathc.disFirst[1] = temppathc.disFirst[1];
                    }
                    else
                    {
                        PointF cp1 = new PointF((move_to1.X - C.path[0].P.X) / 3, (move_to1.Y - C.path[0].P.Y) / 3);
                        PointF cp2 = new PointF((C.path[0].P.X - move_to1.X) / 3, (C.path[0].P.Y - move_to1.Y) / 3);
                        GraphCurve temppathc = CurveInsert(pathc, pathc.path.Count - 1, new GraphPoint(move_to1.X, move_to1.Y));
                        temppathc.path.RemoveAt(pathc.path.Count - 1);
                        temppathc.disFirst.RemoveAt(pathc.disFirst.Count - 1);
                        temppathc.disSecond.RemoveAt(pathc.disSecond.Count - 1);
                        GraphPoint newp = new GraphPoint(move_to1.X, move_to1.Y);

                        newp.Relative += 2;
                        path.P.Remove(pathc.path[pathc.path.Count - 1]);
                        pathc.path[pathc.path.Count - 1] = newp;
                        path.P.Add(newp);
                        C.path[0].Relative--;
                        C.path.Insert(0, newp);
                        C.disFirst.Insert(0, cp2);
                        C.disSecond.Insert(0, cp1);
                        C.type.Insert(0, 0);
                        C.disFirst[1] = new PointF(cp2.X, cp2.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                        AdjustCindex(C);

                        pathc.path[pathc.path.Count - 1].P = temppathc.path[pathc.path.Count - 1].P;
                        pathc.disSecond[pathc.path.Count - 1] = temppathc.disSecond[pathc.path.Count - 1];
                        pathc.disFirst[pathc.path.Count - 2] = temppathc.disFirst[pathc.path.Count - 2];
                    }
                    #endregion
                }
                else if (extend1 == 2)//2=延長相鄰C 插入cutc
                {
                    #region type2
                    if (C.path[0] == pathc.path[0])
                    {
                        PointF cp1 = new PointF((pathc.path[0].P.X - move_to1.X) / 3, (pathc.path[0].P.Y - move_to1.Y) / 3);
                        PointF cp2 = new PointF((move_to1.X - pathc.path[0].P.X) / 3, (move_to1.Y - pathc.path[0].P.Y) / 3);
                        GraphCurve cutpathc = CurveInsert(CutC, 1, new GraphPoint(move_to1.X, move_to1.Y));
                        cutpathc.path.RemoveAt(0);
                        cutpathc.disFirst.RemoveAt(0);
                        cutpathc.disSecond.RemoveAt(0);
                        GraphPoint newp = new GraphPoint(move_to1.X, move_to1.Y);

                        newp.Relative += 2;
                        path.P.Remove(C.path[0]);
                        C.path[0] = newp;
                        path.P.Add(newp);
                        pathc.path[0].Relative--;
                        pathc.path.Insert(0, newp);
                        pathc.disFirst.Insert(0, cp2);
                        pathc.disSecond.Insert(0, cp1);
                        pathc.type.Insert(0, 0);
                        pathc.disFirst[1] = new PointF(cp2.X, cp2.Y);
                        AdjustCindex(pathc);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);

                        C.path[0].P = cutpathc.path[0].P;
                        C.disSecond[0] = cutpathc.disSecond[0];
                        C.disFirst[1] = cutpathc.disFirst[1];
                        CutC = cutpathc;
                    }
                    else
                    {
                        PointF cp1 = new PointF((move_to1.X - pathc.path.Last().P.X) / 3, (move_to1.Y - pathc.path.Last().P.Y) / 3);
                        PointF cp2 = new PointF((pathc.path.Last().P.X - move_to1.X) / 3, (pathc.path.Last().P.Y - move_to1.Y) / 3);
                        GraphCurve cutpathc = CurveInsert(CutC, 1, new GraphPoint(move_to1.X, move_to1.Y));
                        cutpathc.path.RemoveAt(0);
                        cutpathc.disFirst.RemoveAt(0);
                        cutpathc.disSecond.RemoveAt(0);
                        GraphPoint newp = new GraphPoint(move_to1.X, move_to1.Y);


                        newp.Relative += 2;
                        path.P.Remove(C.path[0]);
                        C.path[0] = newp;
                        path.P.Add(newp);
                        pathc.path.Last().Relative--;
                        pathc.path.Add(newp);
                        pathc.disFirst.Add(cp2);
                        pathc.disSecond.Add(cp1);
                        pathc.type.Add(0);
                        pathc.disSecond[pathc.disSecond.Count - 2] = new PointF(cp1.X, cp1.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);

                        C.path[0].P = cutpathc.path[0].P;
                        C.disSecond[0] = cutpathc.disSecond[0];
                        C.disFirst[1] = cutpathc.disFirst[1];
                        CutC = cutpathc;
                    }
                    #endregion
                }
                else if (extend1 == 3)//3=都延長
                {
                    #region type3
                    if (C.path[0] == pathc.path[0])
                    {
                        GraphPoint extrap = new GraphPoint(CutC.path[0].P.X, CutC.path[0].P.Y);
                        C.path[0] = extrap;
                        extrap.Relative++;
                        extrap.name = extrap.check_name(PointsList);
                        PointsList.Add(extrap);
                        PointF cutcp1 = new PointF((move_to1.X - CutC.path[0].P.X) / 3, (move_to1.Y - CutC.path[0].P.Y) / 3);
                        PointF cutcp2 = new PointF((CutC.path[0].P.X - move_to1.X) / 3, (CutC.path[0].P.Y - move_to1.Y) / 3);
                        PointF pathcp1 = new PointF((pathc.path[0].P.X - move_to1.X) / 3, (pathc.path[0].P.Y - move_to1.Y) / 3);
                        PointF pathcp2 = new PointF((move_to1.X - pathc.path[0].P.X) / 3, (move_to1.Y - pathc.path[0].P.Y) / 3);

                        GraphPoint newp = new GraphPoint(move_to1.X, move_to1.Y);

                        newp.Relative += 2;
                        C.path[0].Relative--;
                        path.P.Remove(pathc.path[0]);
                        path.P.Add(newp);
                        C.path.Insert(0, newp);
                        C.disFirst.Insert(0, cutcp2);
                        C.disSecond.Insert(0, cutcp1);
                        C.type.Insert(0, 0);
                        C.disFirst[1] = new PointF(cutcp2.X, cutcp2.Y);
                        AdjustCindex(C);

                        path.P.Remove(C.path[0]);
                        pathc.disFirst.Insert(0, pathcp2);
                        pathc.disSecond.Insert(0, pathcp1);
                        pathc.type.Insert(0, 0);
                        pathc.disFirst[1] = new PointF(pathcp2.X, pathcp2.Y);
                        AdjustCindex(pathc);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                    }
                    else
                    {
                        GraphPoint extrap = new GraphPoint(CutC.path[0].P.X, CutC.path[0].P.Y);
                        C.path[0] = extrap;
                        extrap.Relative++;
                        extrap.name = extrap.check_name(PointsList);
                        PointsList.Add(extrap);
                        PointF cutcp1 = new PointF((move_to1.X - C.path[0].P.X) / 3, (move_to1.Y - C.path[0].P.Y) / 3);
                        PointF cutcp2 = new PointF((C.path[0].P.X - move_to1.X) / 3, (C.path[0].P.Y - move_to1.Y) / 3);
                        PointF pathcp1 = new PointF((move_to1.X - pathc.path.Last().P.X) / 3, (move_to1.Y - pathc.path.Last().P.Y) / 3);
                        PointF pathcp2 = new PointF((pathc.path.Last().P.X - move_to1.X) / 3, (pathc.path.Last().P.Y - move_to1.Y) / 3);

                        GraphPoint newp = new GraphPoint(move_to1.X, move_to1.Y);

                        newp.Relative += 2;
                        C.path[0].Relative--;
                        path.P.Remove(pathc.path.Last());
                        path.P.Add(newp);
                        C.path.Insert(0, newp);
                        C.disFirst.Insert(0, cutcp2);
                        C.disSecond.Insert(0, cutcp1);
                        C.type.Insert(0, 0);
                        C.disFirst[1] = new PointF(cutcp2.X, cutcp2.Y);
                        AdjustCindex(C);

                        path.P.Remove(C.path[0]);
                        pathc.disFirst.Add(pathcp2);
                        pathc.disSecond.Add(pathcp1);
                        pathc.type.Add(0);
                        pathc.disSecond[pathc.disSecond.Count - 2] = new PointF(pathcp1.X, pathcp1.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                    }
                    #endregion
                }
            }

            if (path.L[index2] != null)
            {
                if (extend2 == 1)
                {
                    GraphLine l = path.L[index2];
                    int last = CutC.path.Count - 1;
                    if (C.path.Last() == l.StartPoint)
                    {
                        GraphPoint extrap = new GraphPoint(CutC.path.Last().P.X, CutC.path.Last().P.Y);
                        C.path[C.path.Count - 1] = extrap;
                        extrap.Relative++;
                        extrap.name = extrap.check_name(PointsList);
                        PointsList.Add(extrap);
                        PointF cp1 = new PointF((move_to2.X - CutC.path.Last().P.X) / 3, (move_to2.Y - CutC.path.Last().P.Y) / 3);
                        PointF cp2 = new PointF((CutC.path.Last().P.X - move_to2.X) / 3, (CutC.path.Last().P.Y - move_to2.Y) / 3);
                        GraphPoint newp = new GraphPoint(move_to2.X, move_to2.Y);
                        newp.Relative += 2;
                        path.P.Remove(l.StartPoint);
                        l.StartPoint = newp;
                        path.P.Add(newp);
                        C.path.Last().Relative--;
                        C.path.Add(newp);
                        C.disFirst.Add(cp2);
                        C.disSecond.Add(cp1);
                        C.type.Add(0);
                        C.disSecond[C.disSecond.Count - 2] = new PointF(cp1.X, cp1.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                    }
                    else if (C.path.Last() == l.EndPoint)
                    {
                        GraphPoint extrap = new GraphPoint(CutC.path.Last().P.X, CutC.path.Last().P.Y);
                        C.path[C.path.Count - 1] = extrap;
                        extrap.Relative++;
                        extrap.name = extrap.check_name(PointsList);
                        PointsList.Add(extrap);
                        PointF cp1 = new PointF((move_to2.X - CutC.path.Last().P.X) / 3, (move_to2.Y - CutC.path.Last().P.Y) / 3);
                        PointF cp2 = new PointF((CutC.path.Last().P.X - move_to2.X) / 3, (CutC.path.Last().P.Y - move_to2.Y) / 3);
                        GraphPoint newp = new GraphPoint(move_to2.X, move_to2.Y);
                        newp.Relative += 2;
                        path.P.Remove(l.EndPoint);
                        l.EndPoint = newp;
                        path.P.Add(newp);
                        C.path.Last().Relative--;
                        C.path.Add(newp);
                        C.disFirst.Add(cp2);
                        C.disSecond.Add(cp1);
                        C.type.Add(0);
                        C.disSecond[C.disSecond.Count - 2] = new PointF(cp1.X, cp1.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                    }
                }
                else
                {
                    GraphCurve tempc = CurveInsert(CutC, CutC.path.Count - 1, new GraphPoint(move_to2.X, move_to2.Y));
                    tempc.path.RemoveAt(tempc.path.Count-1);
                    tempc.disFirst.RemoveAt(tempc.disFirst.Count - 1);
                    tempc.disSecond.RemoveAt(tempc.disSecond.Count - 1);

                    C.path[CutC.path.Count - 1].P = tempc.path[CutC.path.Count - 1].P;
                    C.disFirst[CutC.path.Count - 1] = tempc.disFirst[CutC.path.Count - 1];
                    C.disSecond[CutC.path.Count - 2] = tempc.disSecond[CutC.path.Count - 2];

                    CutC = tempc;
                }
            }
            else
            {
                GraphCurve pathc = path.C[index2];
                if (extend2 == 0)//0=兩線插入
                {
                    #region type0
                    if (C.path.Last() == pathc.path[0])
                    {
                        GraphCurve cutpathc = CurveInsert(CutC, CutC.path.Count - 1, new GraphPoint(move_to2.X, move_to2.Y));
                        cutpathc.path.RemoveAt(cutpathc.path.Count - 1);
                        cutpathc.disFirst.RemoveAt(cutpathc.disFirst.Count - 1);
                        cutpathc.disSecond.RemoveAt(cutpathc.disSecond.Count - 1);
                        GraphCurve temppathc = CurveInsert(pathc, 1, new GraphPoint(move_to2.X, move_to2.Y));
                        temppathc.path.RemoveAt(0);
                        temppathc.disFirst.RemoveAt(0);
                        temppathc.disSecond.RemoveAt(0);
                        C.path[C.path.Count - 1].P = cutpathc.path[C.path.Count - 1].P;
                        C.disSecond[C.path.Count - 1] = cutpathc.disSecond[C.path.Count - 1];
                        C.disFirst[C.path.Count - 2] = cutpathc.disFirst[C.path.Count - 2];
                        CutC = cutpathc;
                        pathc.path[0].P = temppathc.path[0].P;
                        pathc.disSecond[0] = temppathc.disSecond[0];
                        pathc.disFirst[1] = temppathc.disFirst[1];
                    }
                    else
                    {
                        GraphCurve cutpathc = CurveInsert(CutC, CutC.path.Count - 1, new GraphPoint(move_to2.X, move_to2.Y));
                        cutpathc.path.RemoveAt(cutpathc.path.Count - 1);
                        cutpathc.disFirst.RemoveAt(cutpathc.disFirst.Count - 1);
                        cutpathc.disSecond.RemoveAt(cutpathc.disSecond.Count - 1);
                        GraphCurve temppathc = CurveInsert(pathc, pathc.path.Count - 1, new GraphPoint(move_to2.X, move_to2.Y));
                        temppathc.path.RemoveAt(pathc.path.Count - 1);
                        temppathc.disFirst.RemoveAt(pathc.disFirst.Count - 1);
                        temppathc.disSecond.RemoveAt(pathc.disSecond.Count - 1);
                        C.path[C.path.Count - 1].P = cutpathc.path[C.path.Count - 1].P;
                        C.disSecond[C.path.Count - 1] = cutpathc.disSecond[C.path.Count - 1];
                        C.disFirst[C.path.Count - 2] = cutpathc.disFirst[C.path.Count - 2];
                        CutC = cutpathc;
                        pathc.path[pathc.path.Count - 1].P = temppathc.path[pathc.path.Count - 1].P;
                        pathc.disSecond[pathc.path.Count - 1] = temppathc.disSecond[pathc.path.Count - 1];
                        pathc.disFirst[pathc.path.Count - 2] = temppathc.disFirst[pathc.path.Count - 2];
                    }
                    #endregion
                }
                else if (extend2 == 1)//1=延長cutC 插入pathc
                {
                    #region type1
                    if (C.path.Last() == pathc.path[0])
                    {
                        PointF cp1 = new PointF((move_to2.X - C.path.Last().P.X) / 3, (move_to2.Y - C.path.Last().P.Y) / 3);
                        PointF cp2 = new PointF((C.path.Last().P.X - move_to2.X) / 3, (C.path.Last().P.Y - move_to2.Y) / 3);
                        GraphCurve temppathc = CurveInsert(pathc, 1, new GraphPoint(move_to2.X, move_to2.Y));

                        temppathc.path.RemoveAt(0);
                        temppathc.disFirst.RemoveAt(0);
                        temppathc.disSecond.RemoveAt(0);
                        GraphPoint newp = new GraphPoint(move_to2.X, move_to2.Y);
                        
                        newp.Relative += 2;
                        path.P.Remove(pathc.path[0]);
                        pathc.path[0] = newp;
                        path.P.Add(newp);
                        C.path.Last().Relative--;
                        C.path.Add(newp);
                        C.disFirst.Add(cp2);
                        C.disSecond.Add(cp1);
                        C.type.Add(0);
                        C.disSecond[C.disSecond.Count - 2] = new PointF(cp1.X, cp1.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                        
                        pathc.path[0].P = temppathc.path[0].P;
                        pathc.disSecond[0] = temppathc.disSecond[0];
                        pathc.disFirst[1] = temppathc.disFirst[1];
                    }
                    else
                    {
                        PointF cp1 = new PointF((move_to2.X - C.path.Last().P.X) / 3, (move_to2.Y - C.path.Last().P.Y) / 3);
                        PointF cp2 = new PointF((C.path.Last().P.X - move_to2.X) / 3, (C.path.Last().P.Y - move_to2.Y) / 3);
                        GraphCurve temppathc = CurveInsert(pathc, pathc.path.Count - 1, new GraphPoint(move_to2.X, move_to2.Y));
                        temppathc.path.RemoveAt(pathc.path.Count - 1);
                        temppathc.disFirst.RemoveAt(pathc.disFirst.Count - 1);
                        temppathc.disSecond.RemoveAt(pathc.disSecond.Count - 1);
                        GraphPoint newp = new GraphPoint(move_to2.X, move_to2.Y);

                        newp.Relative += 2;
                        path.P.Remove(pathc.path.Last());
                        pathc.path[pathc.path.Count-1] = newp;
                        path.P.Add(newp);
                        C.path.Last().Relative--;
                        C.path.Add(newp);
                        C.disFirst.Add(cp2);
                        C.disSecond.Add(cp1);
                        C.type.Add(0);
                        C.disSecond[C.disSecond.Count - 2] = new PointF(cp1.X, cp1.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);

                        pathc.path[pathc.path.Count - 1].P = temppathc.path[pathc.path.Count - 1].P;
                        pathc.disSecond[pathc.path.Count - 1] = temppathc.disSecond[pathc.path.Count - 1];
                        pathc.disFirst[pathc.path.Count - 2] = temppathc.disFirst[pathc.path.Count - 2];
                    }
                    #endregion
                }
                else if (extend2 == 2)//2=延長相鄰C 插入cutc
                {
                    #region type2
                    if (C.path.Last() == pathc.path[0])
                    {
                        PointF cp1 = new PointF((pathc.path[0].P.X - move_to2.X) / 3, (pathc.path[0].P.Y - move_to2.Y) / 3);
                        PointF cp2 = new PointF((move_to2.X - pathc.path[0].P.X) / 3, (move_to2.Y - pathc.path[0].P.Y) / 3);
                        GraphCurve cutpathc = CurveInsert(CutC, CutC.path.Count - 1, new GraphPoint(move_to2.X, move_to2.Y));
                        cutpathc.path.RemoveAt(CutC.path.Count - 1);
                        cutpathc.disFirst.RemoveAt(CutC.disFirst.Count - 1);
                        cutpathc.disSecond.RemoveAt(CutC.disSecond.Count - 1);
                        GraphPoint newp = new GraphPoint(move_to2.X, move_to2.Y);

                        newp.Relative += 2;
                        path.P.Remove(C.path.Last());
                        C.path[C.path.Count-1] = newp;
                        path.P.Add(newp);
                        pathc.path[0].Relative--;
                        pathc.path.Insert(0, newp);
                        pathc.disFirst.Insert(0, cp2);
                        pathc.disSecond.Insert(0, cp1);
                        pathc.type.Insert(0, 0);
                        AdjustCindex(pathc);
                        pathc.disFirst[1] = new PointF(cp2.X, cp2.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);

                        C.path[CutC.path.Count - 1].P = cutpathc.path[CutC.path.Count - 1].P;
                        C.disSecond[CutC.path.Count - 1] = cutpathc.disSecond[CutC.path.Count - 1];
                        C.disFirst[CutC.path.Count - 2] = cutpathc.disFirst[CutC.path.Count - 2];
                        CutC = cutpathc;
                    }
                    else
                    {
                        PointF cp1 = new PointF((move_to2.X - pathc.path.Last().P.X) / 3, (move_to2.Y - pathc.path.Last().P.Y) / 3);
                        PointF cp2 = new PointF((pathc.path.Last().P.X - move_to2.X) / 3, (pathc.path.Last().P.Y - move_to2.Y) / 3);
                        GraphCurve cutpathc = CurveInsert(CutC, CutC.path.Count - 1, new GraphPoint(move_to2.X, move_to2.Y));
                        cutpathc.path.RemoveAt(CutC.path.Count - 1);
                        cutpathc.disFirst.RemoveAt(CutC.disFirst.Count - 1);
                        cutpathc.disSecond.RemoveAt(CutC.disSecond.Count - 1);
                        GraphPoint newp = new GraphPoint(move_to2.X, move_to2.Y);


                        newp.Relative += 2;
                        path.P.Remove(C.path.Last());
                        C.path[C.path.Count - 1] = newp;
                        path.P.Add(newp);
                        pathc.path.Last().Relative--;
                        pathc.path.Add(newp);
                        pathc.disFirst.Add(cp2);
                        pathc.disSecond.Add(cp1);
                        pathc.type.Add(0);
                        pathc.disSecond[pathc.disSecond.Count - 2] = new PointF(cp1.X, cp1.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);

                        C.path[CutC.path.Count - 1].P = cutpathc.path[CutC.path.Count - 1].P;
                        C.disSecond[CutC.path.Count - 1] = cutpathc.disSecond[CutC.path.Count - 1];
                        C.disFirst[CutC.path.Count - 2] = cutpathc.disFirst[CutC.path.Count - 2];
                        CutC = cutpathc;
                    }
                    #endregion
                }
                else if (extend2 == 3)//3=都延長
                {
                    #region type3
                    if (C.path.Last() == pathc.path[0])
                    {
                        GraphPoint extrap = new GraphPoint(CutC.path.Last().P.X, CutC.path.Last().P.Y);
                        C.path[C.path.Count - 1] = extrap;
                        extrap.Relative++;
                        extrap.name = extrap.check_name(PointsList);
                        PointsList.Add(extrap);
                        PointF cutcp1 = new PointF((move_to2.X - CutC.path.Last().P.X) / 3, (move_to2.Y - CutC.path.Last().P.Y) / 3);
                        PointF cutcp2 = new PointF((CutC.path.Last().P.X - move_to2.X) / 3, (CutC.path.Last().P.Y - move_to2.Y) / 3);
                        PointF pathcp1 = new PointF((pathc.path[0].P.X - move_to2.X) / 3, (pathc.path[0].P.Y - move_to2.Y) / 3);
                        PointF pathcp2 = new PointF((move_to2.X - pathc.path[0].P.X) / 3, (move_to2.Y - pathc.path[0].P.Y) / 3);

                        GraphPoint newp = new GraphPoint(move_to2.X, move_to2.Y);

                        newp.Relative += 2;
                        C.path.Last().Relative--;

                        path.P.Remove(pathc.path[0]);
                        path.P.Add(newp);
                        C.path.Add(newp);
                        C.disFirst.Add(cutcp2);
                        C.disSecond.Add(cutcp1);
                        C.type.Add(0);
                        C.disSecond[C.disSecond.Count - 2] = new PointF(cutcp1.X, cutcp1.Y);
                        
                        pathc.path.Insert(0, newp);
                        pathc.disFirst.Insert(0, pathcp2);
                        pathc.disSecond.Insert(0, pathcp1);
                        pathc.type.Insert(0, 0);
                        AdjustCindex(pathc);
                        pathc.disFirst[1] = new PointF(pathcp2.X, pathcp2.Y);

                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                    }
                    else
                    {
                        GraphPoint extrap = new GraphPoint(CutC.path.Last().P.X, CutC.path.Last().P.Y);
                        C.path[C.path.Count - 1] = extrap;
                        extrap.Relative++;
                        extrap.name = extrap.check_name(PointsList);
                        PointsList.Add(extrap);
                        PointF cutcp1 = new PointF((move_to2.X - C.path.Last().P.X) / 3, (move_to2.Y - C.path.Last().P.Y) / 3);
                        PointF cutcp2 = new PointF((C.path.Last().P.X - move_to2.X) / 3, (C.path.Last().P.Y - move_to2.Y) / 3);
                        PointF pathcp1 = new PointF((move_to2.X - pathc.path.Last().P.X) / 3, (move_to2.Y - pathc.path.Last().P.Y) / 3);
                        PointF pathcp2 = new PointF((pathc.path.Last().P.X - move_to2.X) / 3, (pathc.path.Last().P.Y - move_to2.Y) / 3);

                        GraphPoint newp = new GraphPoint(move_to2.X, move_to2.Y);

                        newp.Relative += 2;
                        C.path.Last().Relative--;

                        path.P.Remove(pathc.path[0]);
                        path.P.Add(newp);
                        C.path.Add(newp);
                        C.disFirst.Add(cutcp2);
                        C.disSecond.Add(cutcp1);
                        C.type.Add(0);
                        C.disSecond[C.disSecond.Count - 2] = new PointF(cutcp1.X, cutcp1.Y);
                        
                        pathc.path.Add(newp);
                        pathc.disFirst.Add(pathcp2);
                        pathc.disSecond.Add(pathcp1);
                        pathc.type.Add(0);
                        pathc.disSecond[pathc.disSecond.Count - 2] = new PointF(pathcp1.X, pathcp1.Y);
                        
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                    }
                    #endregion
                }
            }
        }
        private void FtoL_Curve_Fit_Shift_startpoint(GraphCurve C, GraphCurve CutC, GraphGroup path, int index1, PointF move_to1, int extend1)
        {
            if (path.L[index1] != null)
            {
                if (extend1 == 1)
                {
                    GraphLine l = path.L[index1];
                    if (C.path[0] == l.StartPoint)
                    {
                        PointF cp1 = new PointF((CutC.path[0].P.X - move_to1.X) / 3, (CutC.path[0].P.Y - move_to1.Y) / 3);
                        PointF cp2 = new PointF((move_to1.X - CutC.path[0].P.X) / 3, (move_to1.Y - CutC.path[0].P.Y) / 3);
                        GraphPoint newp = new GraphPoint(move_to1.X, move_to1.Y);
                        newp.Relative += 2;
                        path.P.Remove(l.StartPoint);
                        l.StartPoint.Relative--;
                        l.StartPoint = newp;
                        path.P.Add(newp);
                        C.path[0].Relative--;
                        C.path[0].P = CutC.path[0].P;
                        C.path.Insert(0, newp);
                        C.disFirst.Insert(0, cp2);
                        C.disSecond.Insert(0, cp1);
                        C.type.Insert(0, 0);
                        AdjustCindex(C);
                        C.disFirst[1] = new PointF(cp2.X, cp2.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                    }
                    else if (C.path[0] == l.EndPoint)
                    {
                        PointF cp1 = new PointF((CutC.path[0].P.X - move_to1.X) / 3, (CutC.path[0].P.Y - move_to1.Y) / 3);
                        PointF cp2 = new PointF((move_to1.X - CutC.path[0].P.X) / 3, (move_to1.Y - CutC.path[0].P.Y) / 3);
                        GraphPoint newp = new GraphPoint(move_to1.X, move_to1.Y);
                        newp.Relative += 2;
                        path.P.Remove(l.EndPoint);
                        l.EndPoint.Relative--;
                        l.EndPoint = newp;
                        path.P.Add(newp);
                        C.path[0].Relative--;
                        C.path[0].P = CutC.path[0].P;
                        C.path.Insert(0, newp);
                        C.disFirst.Insert(0, cp2);
                        C.disSecond.Insert(0, cp1);
                        C.type.Insert(0, 0);
                        AdjustCindex(C);
                        C.disFirst[1] = new PointF(cp2.X, cp2.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                    }
                }
                else
                {
                    GraphCurve tempc = CurveInsert(CutC, 1, new GraphPoint(move_to1.X, move_to1.Y));
                    tempc.path.RemoveAt(0);
                    tempc.disFirst.RemoveAt(0);
                    tempc.disSecond.RemoveAt(0);

                    C.path[0].P = tempc.path[0].P;
                    C.disSecond[0] = tempc.disSecond[0];
                    C.disFirst[1] = tempc.disFirst[1];

                    CutC = tempc;
                }
            }
            else
            {
                GraphCurve pathc = path.C[index1];
                if (extend1 == 0)//0=兩線插入
                {
                    #region type0
                    if (C.path[0] == pathc.path[0])
                    {
                        GraphCurve cutpathc = CurveInsert(CutC, 1, new GraphPoint(move_to1.X, move_to1.Y));
                        cutpathc.path.RemoveAt(0);
                        cutpathc.disFirst.RemoveAt(0);
                        cutpathc.disSecond.RemoveAt(0);
                        GraphCurve temppathc = CurveInsert(pathc, 1, new GraphPoint(move_to1.X, move_to1.Y));
                        temppathc.path.RemoveAt(0);
                        temppathc.disFirst.RemoveAt(0);
                        temppathc.disSecond.RemoveAt(0);
                        C.path[0].P = cutpathc.path[0].P;
                        C.disSecond[0] = cutpathc.disSecond[0];
                        C.disFirst[1] = cutpathc.disFirst[1];
                        CutC = cutpathc;
                        pathc.path[0].P = temppathc.path[0].P;
                        pathc.disSecond[0] = temppathc.disSecond[0];
                        pathc.disFirst[1] = temppathc.disFirst[1];
                    }
                    else
                    {
                        GraphCurve cutpathc = CurveInsert(CutC, 1, new GraphPoint(move_to1.X, move_to1.Y));
                        cutpathc.path.RemoveAt(0);
                        cutpathc.disFirst.RemoveAt(0);
                        cutpathc.disSecond.RemoveAt(0);
                        GraphCurve temppathc = CurveInsert(pathc, pathc.path.Count - 1, new GraphPoint(move_to1.X, move_to1.Y));
                        temppathc.path.RemoveAt(temppathc.path.Count - 1);
                        temppathc.disFirst.RemoveAt(temppathc.disFirst.Count - 1);
                        temppathc.disSecond.RemoveAt(temppathc.disSecond.Count - 1);
                        C.path[0].P = cutpathc.path[0].P;
                        C.disSecond[0] = cutpathc.disSecond[0];
                        C.disFirst[1] = cutpathc.disFirst[1];
                        CutC = cutpathc;
                        pathc.path[pathc.path.Count - 1].P = temppathc.path[pathc.path.Count - 1].P;
                        pathc.disFirst[pathc.path.Count - 1] = temppathc.disFirst[pathc.path.Count - 1];
                        pathc.disSecond[pathc.path.Count - 2] = temppathc.disSecond[pathc.path.Count - 2];
                    }
                    #endregion
                }
                else if (extend1 == 1)//1=延長cutC 插入pathc
                {
                    #region type1
                    if (C.path[0] == pathc.path[0])
                    {
                        GraphCurve temppathc = CurveInsert(pathc, 1, new GraphPoint(move_to1.X, move_to1.Y));
                        C.path[0].P = CutC.path[0].P;
                        PointF cp1 = new PointF(-(move_to1.X - C.path[0].P.X) / 3, -(move_to1.Y - C.path[0].P.Y) / 3);
                        PointF cp2 = new PointF(-(C.path[0].P.X - move_to1.X) / 3, -(C.path[0].P.Y - move_to1.Y) / 3);

                        temppathc.path.RemoveAt(0);
                        temppathc.disFirst.RemoveAt(0);
                        temppathc.disSecond.RemoveAt(0);
                        GraphPoint newp = new GraphPoint(move_to1.X, move_to1.Y);

                        newp.Relative += 2;
                        path.P.Remove(pathc.path[0]);
                        pathc.path[0] = newp;
                        path.P.Add(newp);
                        C.path[0].Relative--;
                        C.path.Insert(0, newp);
                        C.disFirst.Insert(0, cp2);
                        C.disSecond.Insert(0, cp1);
                        C.type.Insert(0, 0);
                        C.disFirst[1] = new PointF(cp2.X, cp2.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                        AdjustCindex(C);

                        pathc.path[0].P = temppathc.path[0].P;
                        pathc.disSecond[0] = temppathc.disSecond[0];
                        pathc.disFirst[1] = temppathc.disFirst[1];
                    }
                    else
                    {
                        GraphCurve temppathc = CurveInsert(pathc, pathc.path.Count - 1, new GraphPoint(move_to1.X, move_to1.Y));
                        C.path[0].P = CutC.path[0].P;
                        PointF cp1 = new PointF(-(move_to1.X - C.path[0].P.X) / 3, -(move_to1.Y - C.path[0].P.Y) / 3);
                        PointF cp2 = new PointF(-(C.path[0].P.X - move_to1.X) / 3, -(C.path[0].P.Y - move_to1.Y) / 3);

                        temppathc.path.RemoveAt(temppathc.path.Count - 1);
                        temppathc.disFirst.RemoveAt(temppathc.disFirst.Count - 1);
                        temppathc.disSecond.RemoveAt(temppathc.disSecond.Count - 1);
                        GraphPoint newp = new GraphPoint(move_to1.X, move_to1.Y);

                        newp.Relative += 2;
                        path.P.Remove(pathc.path[pathc.path.Count - 1]);
                        pathc.path[pathc.path.Count - 1] = newp;
                        path.P.Add(newp);
                        C.path[0].Relative--;
                        C.path.Insert(0, newp);
                        C.disFirst.Insert(0, cp2);
                        C.disSecond.Insert(0, cp1);
                        C.type.Insert(0, 0);
                        C.disFirst[1] = new PointF(cp2.X, cp2.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                        AdjustCindex(C);

                        pathc.path[pathc.path.Count - 1].P = temppathc.path[pathc.path.Count - 1].P;
                        pathc.disFirst[pathc.disFirst.Count - 1] = temppathc.disFirst[pathc.disFirst.Count - 1];
                        pathc.disSecond[pathc.disSecond.Count - 2] = temppathc.disSecond[pathc.disSecond.Count - 2];
                    }
                    #endregion
                }
                else if (extend1 == 2)//2=延長相鄰C 插入cutc
                {
                    #region type2
                    if (C.path[0] == pathc.path[0])
                    {
                        PointF cp1 = new PointF((pathc.path[0].P.X - move_to1.X) / 3, (pathc.path[0].P.Y - move_to1.Y) / 3);
                        PointF cp2 = new PointF((move_to1.X - pathc.path[0].P.X) / 3, (move_to1.Y - pathc.path[0].P.Y) / 3);
                        GraphCurve cutpathc = CurveInsert(CutC, 1, new GraphPoint(move_to1.X, move_to1.Y));
                        cutpathc.path.RemoveAt(0);
                        cutpathc.disFirst.RemoveAt(0);
                        cutpathc.disSecond.RemoveAt(0);
                        GraphPoint newp = new GraphPoint(move_to1.X, move_to1.Y);

                        newp.Relative += 2;
                        path.P.Remove(C.path[0]);
                        C.path[0] = newp;
                        path.P.Add(newp);
                        pathc.path[0].Relative--;
                        pathc.path.Insert(0, newp);
                        pathc.disFirst.Insert(0, cp2);
                        pathc.disSecond.Insert(0, cp1);
                        pathc.type.Insert(0, 0);
                        pathc.disFirst[1] = new PointF(cp2.X, cp2.Y);
                        AdjustCindex(pathc);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);

                        C.path[0].P = cutpathc.path[0].P;
                        C.disSecond[0] = cutpathc.disSecond[0];
                        C.disFirst[1] = cutpathc.disFirst[1];
                        CutC = cutpathc;
                    }
                    else
                    {
                        PointF cp1 = new PointF((move_to1.X - pathc.path.Last().P.X) / 3, (move_to1.Y - pathc.path.Last().P.Y) / 3);
                        PointF cp2 = new PointF((pathc.path.Last().P.X - move_to1.X) / 3, (pathc.path.Last().P.Y - move_to1.Y) / 3);
                        GraphCurve cutpathc = CurveInsert(CutC, 1, new GraphPoint(move_to1.X, move_to1.Y));
                        cutpathc.path.RemoveAt(0);
                        cutpathc.disFirst.RemoveAt(0);
                        cutpathc.disSecond.RemoveAt(0);
                        GraphPoint newp = new GraphPoint(move_to1.X, move_to1.Y);


                        newp.Relative += 2;
                        path.P.Remove(C.path[0]);
                        C.path[0] = newp;
                        path.P.Add(newp);
                        pathc.path.Last().Relative--;
                        pathc.path.Add(newp);
                        pathc.disFirst.Add(cp2);
                        pathc.disSecond.Add(cp1);
                        pathc.type.Add(0);
                        pathc.disSecond[pathc.disSecond.Count - 2] = new PointF(cp1.X, cp1.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);

                        C.path[0].P = cutpathc.path[0].P;
                        C.disSecond[0] = cutpathc.disSecond[0];
                        C.disFirst[1] = cutpathc.disFirst[1];
                        CutC = cutpathc;
                    }
                    #endregion
                }
                else if (extend1 == 3)//3=都延長
                {
                    #region type3
                    if (C.path[0] == pathc.path[0])
                    {
                        GraphPoint extrap = new GraphPoint(CutC.path[0].P.X, CutC.path[0].P.Y);
                        C.path[0] = extrap;
                        extrap.Relative++;
                        extrap.name = extrap.check_name(PointsList);
                        PointsList.Add(extrap);
                        PointF cutcp1 = new PointF((move_to1.X - CutC.path[0].P.X) / 3, (move_to1.Y - CutC.path[0].P.Y) / 3);
                        PointF cutcp2 = new PointF((CutC.path[0].P.X - move_to1.X) / 3, (CutC.path[0].P.Y - move_to1.Y) / 3);
                        PointF pathcp1 = new PointF((pathc.path[0].P.X - move_to1.X) / 3, (pathc.path[0].P.Y - move_to1.Y) / 3);
                        PointF pathcp2 = new PointF((move_to1.X - pathc.path[0].P.X) / 3, (move_to1.Y - pathc.path[0].P.Y) / 3);

                        GraphPoint newp = new GraphPoint(move_to1.X, move_to1.Y);

                        newp.Relative += 2;
                        C.path[0].Relative--;
                        path.P.Remove(pathc.path[0]);
                        path.P.Add(newp);
                        C.path.Insert(0, newp);
                        C.disFirst.Insert(0, cutcp2);
                        C.disSecond.Insert(0, cutcp1);
                        C.type.Insert(0, 0);
                        C.disFirst[1] = new PointF(cutcp2.X, cutcp2.Y);
                        AdjustCindex(C);
                        
                        pathc.path.Insert(0, newp);
                        pathc.disFirst.Insert(0, pathcp2);
                        pathc.disSecond.Insert(0, pathcp1);
                        pathc.type.Insert(0, 0);
                        pathc.disFirst[1] = new PointF(pathcp2.X, pathcp2.Y);
                        AdjustCindex(pathc);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                    }
                    else
                    {
                        GraphPoint extrap = new GraphPoint(CutC.path[0].P.X, CutC.path[0].P.Y);
                        C.path[0] = extrap;
                        extrap.Relative++;
                        extrap.name = extrap.check_name(PointsList);
                        PointsList.Add(extrap);
                        PointF cutcp1 = new PointF(-(move_to1.X - C.path[0].P.X) / 3, -(move_to1.Y - C.path[0].P.Y) / 3);
                        PointF cutcp2 = new PointF(-(C.path[0].P.X - move_to1.X) / 3, -(C.path[0].P.Y - move_to1.Y) / 3);
                        PointF pathcp1 = new PointF((move_to1.X - pathc.path.Last().P.X) / 3, (move_to1.Y - pathc.path.Last().P.Y) / 3);
                        PointF pathcp2 = new PointF((pathc.path.Last().P.X - move_to1.X) / 3, (pathc.path.Last().P.Y - move_to1.Y) / 3);

                        GraphPoint newp = new GraphPoint(move_to1.X, move_to1.Y);

                        newp.Relative += 2;
                        C.path[0].Relative--;
                        path.P.Remove(pathc.path.Last());
                        path.P.Add(newp);
                        C.path.Insert(0, newp);
                        C.disFirst.Insert(0, cutcp2);
                        C.disSecond.Insert(0, cutcp1);
                        C.type.Insert(0, 0);
                        C.disFirst[1] = new PointF(cutcp2.X, cutcp2.Y);
                        AdjustCindex(C);
                        
                        pathc.path.Add(newp);
                        pathc.disFirst.Add(pathcp2);
                        pathc.disSecond.Add(pathcp1);
                        pathc.type.Add(0);
                        pathc.disSecond[pathc.disSecond.Count - 2] = new PointF(pathcp1.X, pathcp1.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                    }
                    #endregion
                }
            }
        }
        private void FtoL_Curve_Fit_Shift_endpoint(GraphCurve C, GraphCurve CutC, GraphGroup path, int index2, PointF move_to2, int extend2)
        {
            if (path.L[index2] != null)
            {
                if (extend2 == 1)
                {
                    GraphLine l = path.L[index2];
                    int last = CutC.path.Count - 1;
                    if (C.path.Last() == l.StartPoint)
                    {
                        PointF cp1 = new PointF((move_to2.X - CutC.path.Last().P.X) / 3, (move_to2.Y - CutC.path.Last().P.Y) / 3);
                        PointF cp2 = new PointF((CutC.path.Last().P.X - move_to2.X) / 3, (CutC.path.Last().P.Y - move_to2.Y) / 3);
                        GraphPoint newp = new GraphPoint(move_to2.X, move_to2.Y);
                        newp.Relative += 2;
                        path.P.Remove(l.StartPoint);
                        l.StartPoint.Relative--;
                        l.StartPoint = newp;
                        path.P.Add(newp);
                        C.path.Last().Relative--;
                        C.path.Last().P = CutC.path.Last().P;
                        C.path.Add(newp);
                        C.disFirst.Add(cp2);
                        C.disSecond.Add(cp1);
                        C.type.Add(0);
                        C.disSecond[C.disSecond.Count - 2] = new PointF(cp1.X, cp1.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                    }
                    else if (C.path.Last() == l.EndPoint)
                    {
                        PointF cp1 = new PointF((move_to2.X - CutC.path.Last().P.X) / 3, (move_to2.Y - CutC.path.Last().P.Y) / 3);
                        PointF cp2 = new PointF((CutC.path.Last().P.X - move_to2.X) / 3, (CutC.path.Last().P.Y - move_to2.Y) / 3);
                        GraphPoint newp = new GraphPoint(move_to2.X, move_to2.Y);
                        newp.Relative += 2;
                        path.P.Remove(l.EndPoint);
                        l.EndPoint.Relative--;
                        l.EndPoint = newp;
                        path.P.Add(newp);
                        C.path.Last().Relative--;
                        C.path.Last().P = CutC.path.Last().P;
                        C.path.Add(newp);
                        C.disFirst.Add(cp2);
                        C.disSecond.Add(cp1);
                        C.type.Add(0);
                        C.disSecond[C.disSecond.Count - 2] = new PointF(cp1.X, cp1.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                    }
                }
                else
                {
                    GraphCurve tempc = CurveInsert(CutC, CutC.path.Count - 1, new GraphPoint(move_to2.X, move_to2.Y));
                    tempc.path.RemoveAt(tempc.path.Count-1);
                    tempc.disFirst.RemoveAt(tempc.disFirst.Count - 1);
                    tempc.disSecond.RemoveAt(tempc.disSecond.Count - 1);

                    C.path[CutC.path.Count - 1].P = tempc.path[CutC.path.Count - 1].P;
                    C.disFirst[CutC.path.Count - 1] = tempc.disFirst[CutC.path.Count - 1];
                    C.disSecond[CutC.path.Count - 2] = tempc.disSecond[CutC.path.Count - 2];

                    CutC = tempc;
                }
            }
            else
            {
                GraphCurve pathc = path.C[index2];
                if (extend2 == 0)//0=兩線插入
                {
                    #region type0
                    if (C.path.Last() == pathc.path[0])
                    {
                        GraphCurve cutpathc = CurveInsert(CutC, CutC.path.Count - 1, new GraphPoint(move_to2.X, move_to2.Y));
                        cutpathc.path.RemoveAt(cutpathc.path.Count - 1);
                        cutpathc.disFirst.RemoveAt(cutpathc.disFirst.Count - 1);
                        cutpathc.disSecond.RemoveAt(cutpathc.disSecond.Count - 1);
                        GraphCurve temppathc = CurveInsert(pathc, 1, new GraphPoint(move_to2.X, move_to2.Y));
                        temppathc.path.RemoveAt(0);
                        temppathc.disFirst.RemoveAt(0);
                        temppathc.disSecond.RemoveAt(0);
                        C.path[C.path.Count - 1].P = cutpathc.path[C.path.Count - 1].P;
                        C.disFirst[C.path.Count - 1] = cutpathc.disFirst[C.path.Count - 1];
                        C.disSecond[C.path.Count - 2] = cutpathc.disSecond[C.path.Count - 2];
                        CutC = cutpathc;
                        pathc.path[0].P = temppathc.path[0].P;
                        pathc.disSecond[0] = temppathc.disSecond[0];
                        pathc.disFirst[1] = temppathc.disFirst[1];
                    }
                    else
                    {
                        GraphCurve cutpathc = CurveInsert(CutC, CutC.path.Count - 1, new GraphPoint(move_to2.X, move_to2.Y));
                        cutpathc.path.RemoveAt(cutpathc.path.Count - 1);
                        cutpathc.disFirst.RemoveAt(cutpathc.disFirst.Count - 1);
                        cutpathc.disSecond.RemoveAt(cutpathc.disSecond.Count - 1);
                        GraphCurve temppathc = CurveInsert(pathc, pathc.path.Count - 1, new GraphPoint(move_to2.X, move_to2.Y));
                        temppathc.path.RemoveAt(temppathc.path.Count - 1);
                        temppathc.disFirst.RemoveAt(temppathc.disFirst.Count - 1);
                        temppathc.disSecond.RemoveAt(temppathc.disSecond.Count - 1);
                        C.path[C.path.Count - 1].P = cutpathc.path[C.path.Count - 1].P;
                        C.disFirst[C.path.Count - 1] = cutpathc.disFirst[C.path.Count - 1];
                        C.disSecond[C.path.Count - 2] = cutpathc.disSecond[C.path.Count - 2];
                        CutC = cutpathc;
                        pathc.path[pathc.path.Count - 1].P = temppathc.path[pathc.path.Count - 1].P;
                        pathc.disFirst[pathc.path.Count - 1] = temppathc.disFirst[pathc.path.Count - 1];
                        pathc.disSecond[pathc.path.Count - 2] = temppathc.disSecond[pathc.path.Count - 2];
                    }
                    #endregion
                }
                else if (extend2 == 1)//1=延長cutC 插入pathc
                {
                    #region type1
                    if (C.path.Last() == pathc.path[0])
                    {
                        GraphCurve temppathc = CurveInsert(pathc, 1, new GraphPoint(move_to2.X, move_to2.Y));
                        C.path.Last().P = CutC.path.Last().P;
                        PointF cp1 = new PointF((move_to2.X - C.path.Last().P.X) / 3, (move_to2.Y - C.path.Last().P.Y) / 3);
                        PointF cp2 = new PointF((C.path.Last().P.X - move_to2.X) / 3, (C.path.Last().P.Y - move_to2.Y) / 3);
                        
                        temppathc.path.RemoveAt(0);
                        temppathc.disFirst.RemoveAt(0);
                        temppathc.disSecond.RemoveAt(0);
                        GraphPoint newp = new GraphPoint(move_to2.X, move_to2.Y);
                        
                        newp.Relative += 2;
                        path.P.Remove(pathc.path[0]);
                        pathc.path[0] = newp;
                        path.P.Add(newp);
                        C.path.Last().Relative--;
                        C.path.Add(newp);
                        C.disFirst.Add(cp2);
                        C.disSecond.Add(cp1);
                        C.type.Add(0);
                        C.disSecond[C.disSecond.Count - 2] = new PointF(cp1.X, cp1.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                        
                        pathc.path[0].P = temppathc.path[0].P;
                        pathc.disSecond[0] = temppathc.disSecond[0];
                        pathc.disFirst[1] = temppathc.disFirst[1];
                    }
                    else
                    {
                        GraphCurve temppathc = CurveInsert(pathc, pathc.path.Count - 1, new GraphPoint(move_to2.X, move_to2.Y));
                        C.path.Last().P = CutC.path.Last().P;
                        PointF cp1 = new PointF((move_to2.X - C.path.Last().P.X) / 3, (move_to2.Y - C.path.Last().P.Y) / 3);
                        PointF cp2 = new PointF((C.path.Last().P.X - move_to2.X) / 3, (C.path.Last().P.Y - move_to2.Y) / 3);

                        temppathc.path.RemoveAt(temppathc.path.Count - 1);
                        temppathc.disFirst.RemoveAt(temppathc.disFirst.Count - 1);
                        temppathc.disSecond.RemoveAt(temppathc.disSecond.Count - 1);
                        GraphPoint newp = new GraphPoint(move_to2.X, move_to2.Y);

                        newp.Relative += 2;
                        path.P.Remove(pathc.path.Last());
                        pathc.path[pathc.path.Count-1] = newp;
                        path.P.Add(newp);
                        C.path.Last().Relative--;
                        C.path.Add(newp);
                        C.disFirst.Add(cp2);
                        C.disSecond.Add(cp1);
                        C.type.Add(0);
                        C.disSecond[C.disSecond.Count - 2] = new PointF(cp1.X, cp1.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);

                        pathc.path[pathc.path.Count - 1].P = temppathc.path[pathc.path.Count - 1].P;
                        pathc.disFirst[pathc.path.Count - 1] = temppathc.disFirst[pathc.path.Count - 1];
                        pathc.disSecond[pathc.path.Count - 2] = temppathc.disSecond[pathc.path.Count - 2];
                    }
                    #endregion
                }
                else if (extend2 == 2)//2=延長相鄰C 插入cutc
                {
                    #region type2
                    if (C.path.Last() == pathc.path[0])
                    {
                        PointF cp1 = new PointF((pathc.path[0].P.X - move_to2.X) / 3, (pathc.path[0].P.Y - move_to2.Y) / 3);
                        PointF cp2 = new PointF((move_to2.X - pathc.path[0].P.X) / 3, (move_to2.Y - pathc.path[0].P.Y) / 3);
                        GraphCurve cutpathc = CurveInsert(CutC, CutC.path.Count - 1, new GraphPoint(move_to2.X, move_to2.Y));
                        cutpathc.path.RemoveAt(cutpathc.path.Count - 1);
                        cutpathc.disFirst.RemoveAt(cutpathc.disFirst.Count - 1);
                        cutpathc.disSecond.RemoveAt(cutpathc.disSecond.Count - 1);
                        GraphPoint newp = new GraphPoint(move_to2.X, move_to2.Y);

                        newp.Relative += 2;
                        path.P.Remove(C.path.Last());
                        C.path[C.path.Count-1] = newp;
                        path.P.Add(newp);
                        pathc.path[0].Relative--;
                        pathc.path.Insert(0, newp);
                        pathc.disFirst.Insert(0, cp2);
                        pathc.disSecond.Insert(0, cp1);
                        pathc.type.Insert(0, 0);
                        AdjustCindex(pathc);
                        pathc.disFirst[1] = new PointF(cp2.X, cp2.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);

                        C.path[CutC.path.Count - 1].P = cutpathc.path[CutC.path.Count - 1].P;
                        C.disFirst[CutC.path.Count - 1] = cutpathc.disFirst[CutC.path.Count - 1];
                        C.disSecond[CutC.path.Count - 2] = cutpathc.disSecond[CutC.path.Count - 2];
                        CutC = cutpathc;
                    }
                    else
                    {
                        PointF cp1 = new PointF((move_to2.X - pathc.path.Last().P.X) / 3, (move_to2.Y - pathc.path.Last().P.Y) / 3);
                        PointF cp2 = new PointF((pathc.path.Last().P.X - move_to2.X) / 3, (pathc.path.Last().P.Y - move_to2.Y) / 3);
                        GraphCurve cutpathc = CurveInsert(CutC, CutC.path.Count - 1, new GraphPoint(move_to2.X, move_to2.Y));
                        cutpathc.path.RemoveAt(cutpathc.path.Count - 1);
                        cutpathc.disFirst.RemoveAt(cutpathc.disFirst.Count - 1);
                        cutpathc.disSecond.RemoveAt(cutpathc.disSecond.Count - 1);
                        GraphPoint newp = new GraphPoint(move_to2.X, move_to2.Y);


                        newp.Relative += 2;
                        path.P.Remove(C.path.Last());
                        C.path[C.path.Count - 1] = newp;
                        path.P.Add(newp);
                        pathc.path.Last().Relative--;
                        pathc.path.Add(newp);
                        pathc.disFirst.Add(cp2);
                        pathc.disSecond.Add(cp1);
                        pathc.type.Add(0);
                        pathc.disSecond[pathc.disSecond.Count - 2] = new PointF(cp1.X, cp1.Y);
                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);

                        C.path[CutC.path.Count - 1].P = cutpathc.path[CutC.path.Count - 1].P;
                        C.disFirst[CutC.path.Count - 1] = cutpathc.disFirst[CutC.path.Count - 1];
                        C.disSecond[CutC.path.Count - 2] = cutpathc.disSecond[CutC.path.Count - 2];
                        CutC = cutpathc;
                    }
                    #endregion
                }
                else if (extend2 == 3)//3=都延長
                {
                    #region type3
                    if (C.path.Last() == pathc.path[0])
                    {
                        GraphPoint extrap = new GraphPoint(CutC.path.Last().P.X, CutC.path.Last().P.Y);
                        C.path[C.path.Count - 1] = extrap;
                        extrap.Relative++;
                        extrap.name = extrap.check_name(PointsList);
                        PointsList.Add(extrap);
                        PointF cutcp1 = new PointF((move_to2.X - CutC.path.Last().P.X) / 3, (move_to2.Y - CutC.path.Last().P.Y) / 3);
                        PointF cutcp2 = new PointF((CutC.path.Last().P.X - move_to2.X) / 3, (CutC.path.Last().P.Y - move_to2.Y) / 3);
                        PointF pathcp1 = new PointF((pathc.path[0].P.X - move_to2.X) / 3, (pathc.path[0].P.Y - move_to2.Y) / 3);
                        PointF pathcp2 = new PointF((move_to2.X - pathc.path[0].P.X) / 3, (move_to2.Y - pathc.path[0].P.Y) / 3);

                        GraphPoint newp = new GraphPoint(move_to2.X, move_to2.Y);

                        newp.Relative += 2;
                        C.path.Last().Relative--;

                        path.P.Remove(pathc.path[0]);
                        path.P.Add(newp);
                        C.path.Add(newp);
                        C.disFirst.Add(cutcp2);
                        C.disSecond.Add(cutcp1);
                        C.type.Add(0);
                        C.disSecond[C.disSecond.Count - 2] = new PointF(cutcp1.X, cutcp1.Y);
                        
                        pathc.path.Insert(0, newp);
                        pathc.disFirst.Insert(0, pathcp2);
                        pathc.disSecond.Insert(0, pathcp1);
                        pathc.type.Insert(0, 0);
                        AdjustCindex(pathc);
                        pathc.disFirst[1] = new PointF(pathcp2.X, pathcp2.Y);

                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                    }
                    else
                    {
                        GraphPoint extrap = new GraphPoint(CutC.path.Last().P.X, CutC.path.Last().P.Y);
                        C.path[C.path.Count - 1] = extrap;
                        extrap.Relative++;
                        extrap.name = extrap.check_name(PointsList);
                        PointsList.Add(extrap);
                        PointF cutcp1 = new PointF((move_to2.X - C.path.Last().P.X) / 3, (move_to2.Y - C.path.Last().P.Y) / 3);
                        PointF cutcp2 = new PointF((C.path.Last().P.X - move_to2.X) / 3, (C.path.Last().P.Y - move_to2.Y) / 3);
                        PointF pathcp1 = new PointF((move_to2.X - pathc.path.Last().P.X) / 3, (move_to2.Y - pathc.path.Last().P.Y) / 3);
                        PointF pathcp2 = new PointF((pathc.path.Last().P.X - move_to2.X) / 3, (pathc.path.Last().P.Y - move_to2.Y) / 3);

                        GraphPoint newp = new GraphPoint(move_to2.X, move_to2.Y);

                        newp.Relative += 2;
                        C.path.Last().Relative--;

                        path.P.Remove(pathc.path[0]);
                        path.P.Add(newp);
                        C.path.Add(newp);
                        C.disFirst.Add(cutcp2);
                        C.disSecond.Add(cutcp1);
                        C.type.Add(0);
                        C.disSecond[C.disSecond.Count - 2] = new PointF(cutcp1.X, cutcp1.Y);
                        
                        pathc.path.Add(newp);
                        pathc.disFirst.Add(pathcp2);
                        pathc.disSecond.Add(pathcp1);
                        pathc.type.Add(0);
                        pathc.disSecond[pathc.disSecond.Count - 2] = new PointF(pathcp1.X, pathcp1.Y);


                        newp.name = newp.check_name(PointsList);
                        PointsList.Add(newp);
                    }
                    #endregion
                }
            }
        }
        private bool FindInsert_L(GraphGroup path,GraphLine l, GraphLine cutL, out bool T_Pre_F_Next,out PointF sp, out bool StExtraL,out PointF ep, out bool EnExtraL)
        {
            PathDistance pd = new PathDistance();
            int index = path.L.FindIndex(x => x == l);
            int nexti = (index + path.L.Count + 1) % path.L.Count, prei = (index + path.L.Count - 1) % path.L.Count;
            sp = new PointF(-1, -1);
            ep = new PointF(-1, -1);
            StExtraL = false;
            EnExtraL = false;
            T_Pre_F_Next = false;
            if (path.L[nexti] != null)
            {
                #region CutL PathL
                PointF nextinsert;
                int check;
                bool extra;
                Insert_LL_With_Check(l, cutL, path.L[nexti], out nextinsert, out check, out extra);
                if (check == -1)
                {
                    return false;
                }
                if (check == 0)
                {
                    sp = nextinsert;
                    StExtraL = extra;
                }
                else if(check == 1)
                {
                    T_Pre_F_Next = true;
                    ep = nextinsert;
                    EnExtraL = extra;
                }
                #endregion
            }
            else
            {
                #region CutL PathC
                PointF nextinsert;
                int check;
                bool extra;
                Insert_LC_With_Check(l, cutL, path.C[nexti], out nextinsert, out check, out extra);
                if (extra)
                {
                    if (check == -1)
                        return false;
                    else if (check == 0)
                    {
                        sp = nextinsert;
                        StExtraL = extra;
                    }
                    else
                    {
                        T_Pre_F_Next = true;
                        ep = nextinsert;
                        EnExtraL = extra;
                    }
                }
                else if (check == 0)
                {
                    sp = nextinsert;
                    StExtraL = extra;
                }
                else
                {
                    T_Pre_F_Next = true;
                    ep = nextinsert;
                    EnExtraL = extra;
                }
                #endregion
            }

            if (path.L[prei] != null)
            {
                #region CutL PathL
                PointF nextinsert;
                int check;
                bool extra;
                Insert_LL_With_Check(l, cutL, path.L[prei], out nextinsert, out check, out extra);
                if (check == -1)
                {
                    return false;
                }
                if (check == 0)
                {
                    sp = nextinsert;
                    StExtraL = extra;
                }
                else if (check == 1)
                {
                    T_Pre_F_Next = true;
                    ep = nextinsert;
                    EnExtraL = extra;
                }
                #endregion
            }
            else
            {
                #region CutL PathC
                PointF nextinsert;
                int check;
                bool extra;
                Insert_LC_With_Check(l, cutL, path.C[prei], out nextinsert, out check, out extra);
                if (extra)
                {
                    if (check == -1)
                        return false;
                    else if (check == 0)
                    {
                        sp = nextinsert;
                        StExtraL = extra;
                    }
                    else
                    {
                        T_Pre_F_Next = true;
                        ep = nextinsert;
                        EnExtraL = extra;
                    }
                }
                else if (check == 0)
                {
                    sp = nextinsert;
                    StExtraL = extra;
                }
                else
                {
                    T_Pre_F_Next = true;
                    ep = nextinsert;
                    EnExtraL = extra;
                }
                #endregion
            }
            return true;
        }
        private bool FindInsert_L_Half(GraphGroup path, GraphLine l, GraphLine cutL, bool T_pre_F_next, out PointF insertP,out bool extraL)
        {
            PathDistance pd = new PathDistance();
            int targeti = path.L.FindIndex(x => x == l) + (T_pre_F_next ? -1 : 1);
            targeti = (targeti + path.L.Count) % path.L.Count;
            insertP = new PointF(-1, -1);
            extraL = false;
            if (path.L[targeti] != null)
            {
                #region CutL PathL
                PointF nextinsert;
                int check;
                bool extra;
                Insert_LL_With_Check(l, cutL, path.L[targeti], out nextinsert, out check, out extra);
                if (check == -1)
                {
                    return false;
                }
                else
                {
                    insertP = nextinsert;
                    extraL = extra;
                }
                #endregion
            }
            else
            {
                #region CutL PathC
                PointF nextinsert;
                int check;
                bool extra;
                Insert_LC_With_Check(l, cutL, path.C[targeti], out nextinsert, out check, out extra);
                if (extra)
                {
                    if (check == -1)
                    {
                        return false;
                    }
                    else
                    {
                        insertP = nextinsert;
                        extraL = extra;
                    }
                }
                else if (check == -1)
                {
                    return false;
                }
                else 
                {
                    insertP = nextinsert;
                    extraL = extra;
                }
                #endregion
            }
            return true;
        }
        private void Insert_LL_With_Check(GraphLine originalL, GraphLine shiftedL, GraphLine fixL, out PointF nextinsert, out int check, out bool extra)
        {
            int type = fixL.StartPoint == originalL.StartPoint ? 1 :
                           fixL.EndPoint == originalL.StartPoint ? 2 :
                           fixL.StartPoint == originalL.EndPoint ? 3 : 4;//ss es se ee
            PathDistance pd = new PathDistance();
            nextinsert = pd.computeIntersections_LL(fixL, shiftedL, out extra);
            double distSN = (fixL.StartPoint.P.X - nextinsert.X) * (fixL.StartPoint.P.X - nextinsert.X)
                + (fixL.StartPoint.P.Y - nextinsert.Y) * (fixL.StartPoint.P.Y - nextinsert.Y);
            double distEN = (fixL.EndPoint.P.X - nextinsert.X) * (fixL.EndPoint.P.X - nextinsert.X)
                + (fixL.EndPoint.P.Y - nextinsert.Y) * (fixL.EndPoint.P.Y - nextinsert.Y);
            if (type == 1 || type == 3)
            {
                if (distSN > distEN && extra == true)
                    check = -1;
            }
            else
            {
                if ((distSN < distEN) && extra == true)
                    check = -1;
            }
            if (type == 1 || type == 2)
            {
                check = 0;
            }
            else
            {
                check = 1;
            }
        }
        private void Insert_LC_With_Check(GraphLine originalL, GraphLine shiftedL, GraphCurve fixC, out PointF nextinsert, out int check, out bool extra)
        {
            PathDistance pd = new PathDistance();
            int type = fixC.path[0] == originalL.StartPoint ? 1 :
                           fixC.path.Last() == originalL.StartPoint ? 2 :
                           fixC.path[0] == originalL.EndPoint ? 3 : 4;//ss es se ee
            bool[] ooL;
            int ci = (type == 1 || type == 3) ? 1 : fixC.path.Count - 1;
            PointF cc1 = new PointF(fixC.path[ci - 1].P.X + fixC.disSecond[ci - 1].X, fixC.path[ci - 1].P.Y + fixC.disSecond[ci - 1].Y);
            PointF cc2 = new PointF(fixC.path[ci].P.X + fixC.disFirst[ci].X, fixC.path[ci].P.Y + fixC.disFirst[ci].Y);
            PointF[] bez = { fixC.path[ci - 1].P, cc1, cc2, fixC.path[ci].P };
            PointF[] line = { shiftedL.StartPoint.P, shiftedL.EndPoint.P };
            List<PointF> pl1 = pd.computeIntersections_LC(bez, line, out ooL);
            double dist = double.MaxValue;
            int nextinserti = -1;
            bool allooL = false;
            PointF p = (type == 1 || type == 2) ? originalL.StartPoint.P : originalL.EndPoint.P;
            for (int i = 0; i < 3; i++)
            {
                if (!ooL[i] || allooL)
                {
                    double d = (pl1[i].X - p.X) * (pl1[i].X - p.X) + (pl1[i].Y - p.Y) * (pl1[i].Y - p.Y);
                    if (dist > d)
                    {
                        dist = d;
                        nextinserti = i;
                    }
                }
                if (i == 2 && nextinserti == -1)
                {
                    allooL = true;
                }
            }
            if (allooL)
            {
                PointF st, en;
                if (type == 1 || type == 3)
                {
                    en = BezierGiveTFindPoint(bez, 0);
                    st = BezierGiveTFindPoint(bez, 0.01F);
                }
                else
                {
                    en = BezierGiveTFindPoint(bez, 1);
                    st = BezierGiveTFindPoint(bez, 0.99F);
                }
                GraphLine extendL = new GraphLine(new GraphPoint(st.X, st.Y), new GraphPoint(en.X, en.Y));
                bool extraa;
                nextinsert = pd.computeIntersections_LL(extendL, shiftedL, out extraa);
                if ((st.X - en.X) / (st.X - nextinsert.X) < 0)
                    check = -1;
                else if (type == 1 || type == 2)
                {
                    check = 0;
                }
                else
                {
                    check = 1;
                }
            }
            else if (type == 1 || type == 2)
            {
                check = 0;
                nextinsert = pl1[nextinserti];
            }
            else
            {
                check = 1;
                nextinsert = pl1[nextinserti];
            }
            extra = allooL;
        }
        private bool FindInsert_C(GraphGroup path, GraphCurve c, GraphCurve cutC, out bool T_Pre_F_Next, out PointF sp, out int StExtraL, out PointF ep, out int EnExtraL)
        {
            PathDistance pd = new PathDistance();
            int index = path.C.FindIndex(x => x == c);
            int nexti = (index + path.L.Count + 1) % path.L.Count, prei = (index + path.L.Count - 1) % path.L.Count;
            sp = new PointF(-1, -1);
            ep = new PointF(-1, -1);
            StExtraL = 0;// 0=兩線內 1=延長cutC 2=延長相鄰C 3=都延長
            EnExtraL = 0;
            T_Pre_F_Next = false;
            if (path.L[nexti] != null)//cl
            {
                #region cutC PathL
                PointF nextinsert;
                int check;
                bool extra;
                Insert_CL_With_Check(c, cutC, path.L[nexti], out nextinsert, out check, out extra);

                if (extra)
                {
                    if (check == -1)
                        return false;
                    else if (check == 0)
                    {
                        sp = nextinsert;
                        StExtraL = extra ? 1 : 0;
                    }
                    else
                    {
                        T_Pre_F_Next = true;
                        ep = nextinsert;
                        EnExtraL = extra ? 1 : 0;
                    }
                }
                else if (check == 0)
                {
                    sp = nextinsert;
                    StExtraL = 0;
                }
                else
                {
                    T_Pre_F_Next = true;
                    ep = nextinsert;
                    EnExtraL = 0;
                }
                #endregion
            }
            else//cc
            {
                #region cutC pathC
                PointF nextinsert;
                int check;
                bool extra;
                Insert_CC_With_Check(c, cutC, path.C[nexti], out nextinsert, out check, out extra);

                if (check == 0)
                {
                    sp = nextinsert;
                    StExtraL = 0;
                }
                else if (check == 1)
                {
                    T_Pre_F_Next = true;
                    ep = nextinsert;
                    EnExtraL = 0;
                }
                if (check == 2)
                {
                    sp = nextinsert;
                    StExtraL = 1;
                }
                else if (check == 3)
                {
                    T_Pre_F_Next = true;
                    ep = nextinsert;
                    EnExtraL = 1;
                }
                if (check == 4)
                {
                    sp = nextinsert;
                    StExtraL = 2;
                }
                else if (check == 5)
                {
                    T_Pre_F_Next = true;
                    ep = nextinsert;
                    EnExtraL = 2;
                }
                else if (check == 6)
                {
                    sp = nextinsert;
                    StExtraL = 3;
                }
                else if (check == 7)
                {
                    T_Pre_F_Next = true;
                    ep = nextinsert;
                    EnExtraL = 3;
                }
                else if (check == -1)
                    return false;
                #endregion
            }

            if (path.L[prei] != null)//cl
            {
                #region cutC PathL
                PointF nextinsert;
                int check;
                bool extra;
                Insert_CL_With_Check(c, cutC, path.L[prei], out nextinsert, out check, out extra);

                if (extra)
                {
                    if (check == -1)
                        return false;
                    else if (check == 0)
                    {
                        sp = nextinsert;
                        StExtraL = extra ? 1 : 0;
                    }
                    else
                    {
                        T_Pre_F_Next = true;
                        ep = nextinsert;
                        EnExtraL = extra ? 1 : 0;
                    }
                }
                else if (check == 0)
                {
                    sp = nextinsert;
                    StExtraL = 0;
                }
                else
                {
                    T_Pre_F_Next = true;
                    ep = nextinsert;
                    EnExtraL = 0;
                }
                #endregion
            }
            else//cc
            {
                #region cutC pathC
                PointF nextinsert;
                int check;
                bool extra;
                Insert_CC_With_Check(c, cutC, path.C[prei], out nextinsert, out check, out extra);

                if (check == 0)
                {
                    sp = nextinsert;
                    StExtraL = 0;
                }
                else if (check == 1)
                {
                    T_Pre_F_Next = true;
                    ep = nextinsert;
                    EnExtraL = 0;
                }
                if (check == 2)
                {
                    sp = nextinsert;
                    StExtraL = 1;
                }
                else if (check == 3)
                {
                    T_Pre_F_Next = true;
                    ep = nextinsert;
                    EnExtraL = 1;
                }
                if (check == 4)
                {
                    sp = nextinsert;
                    StExtraL = 2;
                }
                else if (check == 5)
                {
                    T_Pre_F_Next = true;
                    ep = nextinsert;
                    EnExtraL = 2;
                }
                else if (check == 6)
                {
                    sp = nextinsert;
                    StExtraL = 3;
                }
                else if (check == 7)
                {
                    T_Pre_F_Next = true;
                    ep = nextinsert;
                    EnExtraL = 3;
                }
                else if (check == -1)
                    return false;
                #endregion
            }
            return true;
        }
        private bool FindInsert_C_Half(GraphGroup path, GraphCurve c, GraphCurve cutC, bool T_pre_F_next, out PointF insertP, out int extraL)
        {
            PathDistance pd = new PathDistance();
            int targeti = path.C.FindIndex(x => x == c) + (T_pre_F_next ? -1 : 1);
            targeti = (targeti + path.L.Count) % path.L.Count;
            insertP = new PointF(-1, -1);
            extraL = 0;// 0=兩線內 1=延長cutC 2=延長相鄰C 3=都延長
            if (path.L[targeti] != null)//cl
            {
                #region cutC PathL
                PointF nextinsert;
                int check;
                bool extra;
                Insert_CL_With_Check(c, cutC, path.L[targeti], out nextinsert, out check, out extra);

                if (extra)
                {
                    if (check == -1)
                        return false;
                    else
                    {
                        insertP = nextinsert;
                        extraL = extra ? 1 : 0;
                    }
                }
                else
                {
                    insertP = nextinsert;
                    extraL = 0;
                }
                #endregion
            }
            else//cc
            {
                #region cutC pathC
                PointF nextinsert;
                int check;
                bool extra;
                Insert_CC_With_Check(c, cutC, path.C[targeti], out nextinsert, out check, out extra);

                if (check == 0)
                {
                    insertP = nextinsert;
                    extraL = 0;
                }
                else if (check == 1)
                {
                    insertP = nextinsert;
                    extraL = 0;
                }
                if (check == 2)
                {
                    insertP = nextinsert;
                    extraL = 1;
                }
                else if (check == 3)
                {
                    insertP = nextinsert;
                    extraL = 1;
                }
                if (check == 4)
                {
                    insertP = nextinsert;
                    extraL = 2;
                }
                else if (check == 5)
                {
                    insertP = nextinsert;
                    extraL = 2;
                }
                else if (check == 6)
                {
                    insertP = nextinsert;
                    extraL = 3;
                }
                else if (check == 7)
                {
                    insertP = nextinsert;
                    extraL = 3;
                }
                else if (check == -1)
                    return false;
                #endregion
            }
            return true;
        }
        private void Insert_CL_With_Check(GraphCurve originalC, GraphCurve shiftedC, GraphLine fixL, out PointF nextinsert, out int check, out bool extra)
        {
            PathDistance pd = new PathDistance();
            int type = fixL.StartPoint == originalC.path[0] ? 1 :
                           fixL.EndPoint == originalC.path[0] ? 2 :
                           fixL.StartPoint == originalC.path.Last() ? 3 : 4;//ss es se ee
            bool[] ooL;
            PointF cc1 = new PointF();
            PointF cc2 = new PointF();
            PointF[] bez = new PointF[4];
            if (type == 1 || type == 2)
            {
                cc1 = new PointF(shiftedC.path[1].P.X + shiftedC.disFirst[1].X, shiftedC.path[1].P.Y + shiftedC.disFirst[1].Y);
                cc2 = new PointF(shiftedC.path[0].P.X + shiftedC.disSecond[0].X, shiftedC.path[0].P.Y + shiftedC.disSecond[0].Y);
                bez = new PointF[] { shiftedC.path[1].P, cc1, cc2, shiftedC.path[0].P };
            }
            else
            {
                int ci = shiftedC.path.Count - 1;
                cc1 = new PointF(shiftedC.path[ci - 1].P.X + shiftedC.disSecond[ci - 1].X, shiftedC.path[ci - 1].P.Y + shiftedC.disSecond[ci - 1].Y);
                cc2 = new PointF(shiftedC.path[ci].P.X + shiftedC.disFirst[ci].X, shiftedC.path[ci].P.Y + shiftedC.disFirst[ci].Y);
                bez = new PointF[] { shiftedC.path[ci - 1].P, cc1, cc2, shiftedC.path[ci].P };
            }
            PointF[] line = { fixL.StartPoint.P, fixL.EndPoint.P };
            List<PointF> pl1 = pd.computeIntersections_LC(bez, line, out ooL);
            double dist = double.MaxValue;
            int nextinserti = -1;
            bool allooL = false;
            PointF p = (type == 1 || type == 2) ? originalC.path[0].P : originalC.path.Last().P;
            for (int i = 0; i < pl1.Count; i++)
            {
                if (!ooL[i] || allooL)
                {
                    double d = (pl1[i].X - p.X) * (pl1[i].X - p.X) + (pl1[i].Y - p.Y) * (pl1[i].Y - p.Y);
                    if (dist > d)
                    {
                        dist = d;
                        nextinserti = i;
                    }
                }
                if (i == pl1.Count - 1 && nextinserti == -1)
                {
                    allooL = true;
                }
            }
            if (allooL)
            {
                PointF st, en;
                en = BezierGiveTFindPoint(bez, 1);
                st = BezierGiveTFindPoint(bez, 0.99F);
                GraphLine extendL = new GraphLine(new GraphPoint(st.X, st.Y), new GraphPoint(en.X, en.Y));
                bool extraa;
                nextinsert = pd.computeIntersections_LL(extendL, fixL, out extraa);
                if ((st.X - en.X) * (st.X - nextinsert.X) < 0)
                    check = -1;
                else if (type == 1 || type == 2)
                {
                    check = 0;
                }
                else
                {
                    check = 1;
                }
            }
            else if (type == 1 || type == 2)
            {
                nextinsert = pl1[nextinserti];
                check = 0;
            }
            else
            {
                nextinsert = pl1[nextinserti];
                check = 1;
            }
            extra = allooL;
        }
        private void Insert_CC_With_Check(GraphCurve originalC, GraphCurve shiftedC, GraphCurve fixC, out PointF nextinsert, out int check, out bool extra)
        {
            PathDistance pd = new PathDistance();
            int type = fixC.path[0] == originalC.path[0] ? 1 :
                           fixC.path.Last() == originalC.path[0] ? 2 :
                           fixC.path[0] == originalC.path.Last() ? 3 : 4;//ss es se ee
            PointF cc1 = new PointF();
            PointF cc2 = new PointF();
            PointF[] cutbez = new PointF[4];
            if (type == 1 || type == 2)
            {
                cc1 = new PointF(shiftedC.path[1].P.X + shiftedC.disFirst[1].X, shiftedC.path[1].P.Y + shiftedC.disFirst[1].Y);
                cc2 = new PointF(shiftedC.path[0].P.X + shiftedC.disSecond[0].X, shiftedC.path[0].P.Y + shiftedC.disSecond[0].Y);
                cutbez = new PointF[] { shiftedC.path[1].P, cc1, cc2, shiftedC.path[0].P };
            }
            else
            {
                int ci = shiftedC.path.Count - 1;
                cc1 = new PointF(shiftedC.path[ci - 1].P.X + shiftedC.disSecond[ci - 1].X, shiftedC.path[ci - 1].P.Y + shiftedC.disSecond[ci - 1].Y);
                cc2 = new PointF(shiftedC.path[ci].P.X + shiftedC.disFirst[ci].X, shiftedC.path[ci].P.Y + shiftedC.disFirst[ci].Y);
                cutbez = new PointF[] { shiftedC.path[ci - 1].P, cc1, cc2, shiftedC.path[ci].P };
            }
            cc1 = new PointF();
            cc2 = new PointF();
            PointF[] pathbez = new PointF[4];
            if (type == 1 || type == 3)
            {
                cc1 = new PointF(fixC.path[1].P.X + fixC.disFirst[1].X, fixC.path[1].P.Y + fixC.disFirst[1].Y);
                cc2 = new PointF(fixC.path[0].P.X + fixC.disSecond[0].X, fixC.path[0].P.Y + fixC.disSecond[0].Y);
                pathbez = new PointF[] { fixC.path[1].P, cc1, cc2, fixC.path[0].P };
            }
            else
            {
                int ci = fixC.path.Count - 1;
                cc1 = new PointF(fixC.path[ci - 1].P.X + fixC.disSecond[ci - 1].X, fixC.path[ci - 1].P.Y + fixC.disSecond[ci - 1].Y);
                cc2 = new PointF(fixC.path[ci].P.X + fixC.disFirst[ci].X, fixC.path[ci].P.Y + fixC.disFirst[ci].Y);
                pathbez = new PointF[] { fixC.path[ci - 1].P, cc1, cc2, fixC.path[ci].P };
            }
            #region CutC pathC
            List<PointF> pl_CC = computeInsertion_CC(cutbez, pathbez);
            if (pl_CC.Count != 0)
            {
                double dist = double.MaxValue;
                int nextinserti = -1;
                PointF p = (type == 1 || type == 2) ? originalC.path[0].P : originalC.path.Last().P;
                for (int i = 0; i < pl_CC.Count; i++)
                {
                    double d = (pl_CC[i].X - p.X) * (pl_CC[i].X - p.X) + (pl_CC[i].Y - p.Y) * (pl_CC[i].Y - p.Y);
                    if (dist > d)
                    {
                        dist = d;
                        nextinserti = i;
                    }
                }
                if (type == 1 || type == 2)
                {
                    nextinsert = pl_CC[nextinserti];
                    check = 0;
                }
                else
                {
                    nextinsert = pl_CC[nextinserti];
                    check = 1;
                }
                extra = false;
            }
            #endregion
            else
            {
                #region extend CutC PathC
                PointF st, en;
                bool[] ooL;
                en = BezierGiveTFindPoint(cutbez, 1);
                st = BezierGiveTFindPoint(cutbez, 0.99F);
                PointF[] extend_cutC = { st, en };
                List<PointF> pl_exCutC_path = pd.computeIntersections_LC(pathbez, extend_cutC, out ooL);
                double dist = double.MaxValue;
                int nextinserti = -1;
                bool allooL = false;
                PointF p = (type == 1 || type == 2) ? originalC.path[0].P : originalC.path.Last().P;
                for (int i = 0; i < pl_exCutC_path.Count; i++)
                {
                    if (!ooL[i] || allooL)
                    {
                        double d = (pl_exCutC_path[i].X - p.X) * (pl_exCutC_path[i].X - p.X) + (pl_exCutC_path[i].Y - p.Y) * (pl_exCutC_path[i].Y - p.Y);
                        if (dist > d)
                        {
                            dist = d;
                            nextinserti = i;
                        }
                    }
                    if (i == pl_exCutC_path.Count - 1 && nextinserti == -1)
                    {
                        allooL = true;
                    }
                }

                if ((type == 1 || type == 2) && !allooL)
                {
                    nextinsert = pl_exCutC_path[nextinserti];
                    check = 2;
                    extra = allooL;
                }
                else if (!allooL)
                {
                    nextinsert = pl_exCutC_path[nextinserti];
                    check = 3;
                    extra = allooL;
                }
                #endregion

                #region CutC extend PathC
                else
                {
                    en = BezierGiveTFindPoint(pathbez, 1);
                    st = BezierGiveTFindPoint(pathbez, 0.99F);
                    PointF[] extend_pathC = { st, en };
                    List<PointF> pl_extendpath_cutC = pd.computeIntersections_LC(cutbez, extend_pathC, out ooL);
                    dist = double.MaxValue;
                    nextinserti = -1;
                    allooL = false;
                    p = (type == 1 || type == 2) ? originalC.path[0].P : originalC.path.Last().P;
                    for (int i = 0; i < pl_extendpath_cutC.Count; i++)
                    {
                        if (!ooL[i] || allooL)
                        {
                            double d = (pl_extendpath_cutC[i].X - p.X) * (pl_extendpath_cutC[i].X - p.X) + (pl_extendpath_cutC[i].Y - p.Y) * (pl_extendpath_cutC[i].Y - p.Y);
                            if (dist > d)
                            {
                                dist = d;
                                nextinserti = i;
                            }
                        }
                        if (i == pl_extendpath_cutC.Count - 1 && nextinserti == -1)
                        {
                            allooL = true;
                        }
                    }

                    if ((type == 1 || type == 2) && !allooL)
                    {
                        nextinsert = pl_extendpath_cutC[nextinserti];
                        check = 4;
                    }
                    else if (!allooL)
                    {
                        nextinsert = pl_extendpath_cutC[nextinserti];
                        check = 5;
                    }
                    #endregion

                    #region extend CutC extend PathC
                    else
                    {
                        GraphLine extendCutC = new GraphLine(new GraphPoint(extend_cutC[0].X, extend_cutC[0].Y), new GraphPoint(extend_cutC[1].X, extend_cutC[1].Y));
                        GraphLine extendpathC = new GraphLine(new GraphPoint(extend_pathC[0].X, extend_pathC[0].Y), new GraphPoint(extend_pathC[1].X, extend_pathC[1].Y));
                        bool extraa;
                        nextinsert = pd.computeIntersections_LL(extendCutC, extendpathC, out extraa);
                        if ((extend_cutC[0].X - extend_cutC[1].X) / (extend_cutC[0].X - nextinsert.X) < 0 ||
                            (extend_pathC[0].X - extend_pathC[1].X) / (extend_pathC[0].X - nextinsert.X) < 0)
                            check = -1;
                        else if (type == 1 || type == 2)
                        {
                            check = 6;
                        }
                        else
                        {
                            check = 7;
                        }
                    }
                    extra = allooL;
                }
                #endregion
            }
        }
        private void AdjustCindex(GraphCurve c)
        {
            List<PathDistance> pdlc1 = PDistList.FindAll(x => x.C1 == c);
            List<PathDistance> pdlc2 = PDistList.FindAll(x => x.C2 == c);
            List<FormulaToLine> ftlc1 = FormulaToLineList.FindAll(x => x.C1 == c);
            List<FormulaToLine> ftlc2 = FormulaToLineList.FindAll(x => x.C2 == c);
            foreach (var pd in pdlc1)
                pd.cindex1++;
            foreach (var pd in pdlc2)
                pd.cindex2++;
            foreach (var ftl in ftlc1)
                ftl.cindex1++;
            foreach (var ftl in ftlc2)
                ftl.cindex2++;
        }
        private void FtoL_Zoom_Shift(GraphGroup zlable, PointF shift, GraphPoint rimp)
        {
            float maxx = float.MinValue, minx = float.MaxValue, maxy = float.MinValue, miny = float.MaxValue;
            float before_width, before_height;
            foreach (var po in zlable.P)
            {
                maxx = maxx < po.P.X ? po.P.X : maxx;
                minx = minx > po.P.X ? po.P.X : minx;
                maxy = maxy < po.P.Y ? po.P.Y : maxy;
                miny = miny > po.P.Y ? po.P.Y : miny;
            }
            before_width = maxx - minx;
            before_height = maxy - miny;
            PointF before_lu = new PointF(minx, miny);
            maxx = float.MinValue; minx = float.MaxValue; maxy = float.MinValue; miny = float.MaxValue;
            float after_width, after_height;
            foreach (var po in zlable.P)
            {
                if (po == rimp)
                {
                    PointF poi = new PointF(po.P.X + shift.X, po.P.Y + shift.Y);
                    maxx = maxx < poi.X ? poi.X : maxx;
                    minx = minx > poi.X ? poi.X : minx;
                    maxy = maxy < poi.Y ? poi.Y : maxy;
                    miny = miny > poi.Y ? poi.Y : miny;
                }
                else
                {
                    maxx = maxx < po.P.X ? po.P.X : maxx;
                    minx = minx > po.P.X ? po.P.X : minx;
                    maxy = maxy < po.P.Y ? po.P.Y : maxy;
                    miny = miny > po.P.Y ? po.P.Y : miny;
                }
            }
            after_width = maxx - minx;
            after_height = maxy - miny;
            PointF after_lu = new PointF(minx, miny);
            PointF para = new PointF(after_width / before_width, after_height / before_height);
            zoom_group(zlable, para, before_lu, after_lu);
        }
        private void zoom_group(GraphGroup g, PointF zoom_para, PointF beforelu, PointF afterlu)
        {
            for (int i = 0; i < g.C.Count; i++)
            {
                GraphCurve c = g.C[i];
                if (c != null)
                {
                    for (int j = 0; j < c.disFirst.Count; j++)
                    {
                        c.disFirst[j] = new PointF((c.disFirst[j].X + c.path[j].P.X - beforelu.X) * zoom_para.X + afterlu.X, 
                                                   (c.disFirst[j].Y + c.path[j].P.Y - beforelu.Y) * zoom_para.Y + afterlu.Y);
                        c.disSecond[j] = new PointF((c.disSecond[j].X + c.path[j].P.X - beforelu.X) * zoom_para.X + afterlu.X,
                                                    (c.disSecond[j].Y + c.path[j].P.Y - beforelu.Y) * zoom_para.Y + afterlu.Y);
                    }
                }
            }
            for (int i = 0; i < g.P.Count; i++)
            {
                g.P[i].P.X = (g.P[i].P.X - beforelu.X) * zoom_para.X + afterlu.X;
                g.P[i].P.Y = (g.P[i].P.Y - beforelu.Y) * zoom_para.Y + afterlu.Y;
            }
            for (int i = 0; i < g.C.Count; i++)
            {
                GraphCurve c = g.C[i];
                if (c != null)
                {
                    for (int j = 0; j < c.disFirst.Count; j++)
                    {
                        c.disFirst[j] = new PointF(c.disFirst[j].X - c.path[j].P.X,
                                                   c.disFirst[j].Y - c.path[j].P.Y);
                        c.disSecond[j] = new PointF(c.disSecond[j].X - c.path[j].P.X,
                                                    c.disSecond[j].Y - c.path[j].P.Y);
                    }
                }
            }
        }
        private void FtoL_checkedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                foreach (int i in FtoL_checkedListBox.CheckedIndices)
                    FtoL_checkedListBox.SetItemCheckState(i, CheckState.Unchecked);
            }
        }
        private void FtoL_Lclick_checkedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                foreach (int i in FtoL_Lclick_checkedListBox.CheckedIndices)
                    FtoL_Lclick_checkedListBox.SetItemCheckState(i, CheckState.Unchecked);
            }
        }
        private List<PointF> computeInsertion_CC(PointF[] b1, PointF[] b2)
        {
            List<PointF> ans = new List<PointF>();
            double mindis = double.MaxValue;
            PointF nearp = new PointF();
            double disb1 = (b1[0].X - b1[3].X) * (b1[0].X - b1[3].X) + (b1[0].Y - b1[3].Y) * (b1[0].Y - b1[3].Y);
            double disb2 = (b2[0].X - b2[3].X) * (b2[0].X - b2[3].X) + (b2[0].Y - b2[3].Y) * (b2[0].Y - b2[3].Y);
            double dev = ((disb1 > disb2) ? Math.Sqrt(disb1) : Math.Sqrt(disb2)) * 0.02;
            for (double i = 0; i <= 1; i+=0.01)
            {
                for(double j = 0; j <= 1; j += 0.01)
                {
                    PointF b1p = BezierGiveTFindPoint(b1, (float)i);
                    PointF b2p = BezierGiveTFindPoint(b2, (float)j);
                    double dis = (b1p.X - b2p.X) * (b1p.X - b2p.X) + (b1p.Y - b2p.Y) * (b1p.Y - b2p.Y);
                    dis = Math.Sqrt(dis);
                    if (mindis > dis)
                    {
                        mindis = dis;
                        nearp = b1p;
                    }
                    else if(mindis < dis)
                    {
                        if(mindis < dev)
                        {
                            ans.Add(nearp);
                            mindis = double.MaxValue;
                            nearp = new PointF();
                        }
                        else
                        {
                            mindis = double.MaxValue;
                            nearp = new PointF();
                        }
                    }
                }
            }
            return ans;
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
            tabControl1.SelectedIndex = i == TabpagesList.Count - 1 ? i - 1 : i;
        }
        private void 建立分頁副本ToolStripMenuItem_Click(object sender, EventArgs ev)
        {

            TabpageData t = TabPageData_Copy(TabpageDataList[tabControl1.SelectedIndex]);

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
        private void 設定布紋標示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            設定布紋標示 f2 = new 設定布紋標示(ClothGrainMark.visible, ClothGrainMark.loc_type, 
                ClothGrainMark.dir_type, ClothGrainMark.Loc.X, ClothGrainMark.Loc.Y, ClothGrainMark.Dir, ClothGrainMark.size);
            if (f2.ShowDialog() == DialogResult.OK)
            {
                ClothGrainMark = new GrainMark()
                {
                    visible = f2.visible,
                    loc_type = f2.loc_type,
                    dir_type = f2.dir_type,
                    Loc = f2.Loc,
                    Dir = f2.Dir,
                    size = f2.size
                };
                Push_Undo_Data();
            }
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
                if (SelectedGroup.L[0].isSeam == false)
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
            Refresh();
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
                else if (LineSeamText.Text == "")
                {
                    ;
                }
                else if (LineSeamText.Text.Last() == '.' || (LineSeamText.Text.Last() == '0' && LineSeamText.Text.Contains(".")))
                {
                    ;
                }
                else if (float.TryParse(LineSeamText.Text, out temp))
                {
                    SelectedGroup.L[0].Seam = temp * (LenthUnit == 0 ? 72 / 2.54F : 72);
                    SelectedGroup.L[0].SeamText = temp;
                    var pa = PathList.Find(x => x.L.Exists(y => y == SelectedGroup.L[0]));
                    if (pa != null)
                    {
                        int index = pa.L.FindIndex(x => x == SelectedGroup.L[0]);
                        float pre_m = (pa.L[index].EndPoint.P.X - pa.L[index].StartPoint.P.X) / (pa.L[index].EndPoint.P.Y - pa.L[index].StartPoint.P.Y);
                        for (int i = 1; i < pa.L.Count(); i++)
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
        private void LineNameTextbox_TextChanged(object sender, EventArgs e)
        {
            if (SelectedGroup != null)
            {
                SelectedGroup.L[0].name = LineNameTextbox.Text;
                RefreshPorperty();
                pictureBox1.Refresh();
            }
        }

        private void CurveSeamCheck_Click(object sender, EventArgs e)
        {
            if (SelectedGroup != null)
            {
                SelectedGroup.C[0].isSeam = !SelectedGroup.C[0].isSeam;
                float temp;
                if (SelectedGroup.C[0].isSeam == false)
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
            Refresh();
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
        private void CurveNameTextbox_TextChanged(object sender, EventArgs e)
        {
            if (SelectedGroup != null)
            {
                SelectedGroup.C[0].name = CurveNameTextbox.Text;
                RefreshPorperty();
                pictureBox1.Refresh();
            }
        }


        private bool[] output_containt;//線 縫份 輔 距 輔距 點 布紋 字 
        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.PageUnit = GraphicsUnit.Document;
            if (output_containt[6])
                Print_Paint_GrainMark(e);
            if (output_containt[5])
                Print_Paint_Points(e);
            if (output_containt[2])
                Print_Paint_SupLines(e);
            Print_Paint_PathDist(e, output_containt[3], output_containt[4]);
            Print_Paint_Lines(e);
            Print_Paint_Curves(e);
            Print_Paint_Arcs(e);
            if (output_containt[1])
                Print_Paint_Seam(e);
            if (output_containt[7])
                Print_Paint_Text(e);
        }

        private void Print_Paint_Points(PrintPageEventArgs e)
        {
            foreach (var p in PointsList)
            {
                var color = p.MarkL != null ? Color.Orange : Color.Black;
                var size = 1;
                var pen = new Pen(color, size);
                e.Graphics.DrawRectangle(pen, (p.P.X  - 3) * 300 / 72, (p.P.Y - 3) * 300 / 72, 6 * 300 / 72, 6 * 300 / 72);
            }
        }
        private void Print_Paint_Lines(PrintPageEventArgs e)
        {
            foreach (var line in LineList)
            {
                var color = line.co;
                var size = 1;
                var pen = new Pen(color, size);
                if (line.dash)
                    pen.DashPattern = new float[] { 5, 5 };
                e.Graphics.DrawLine(pen, line.StartPoint.P.X * 300 / 72, line.StartPoint.P.Y * 300 / 72, line.EndPoint.P.X * 300 / 72, line.EndPoint.P.Y * 300 / 72);
            }
        }
        private void Print_Paint_SupLines(PrintPageEventArgs e)
        {
            foreach (var line in SupLineList)
            {
                var color = line.co;
                var size = 1;
                var pen = new Pen(color, size);
                pen.DashPattern = new float[] { 5, 5 };
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
                if (c.dash)
                    pen.DashPattern = new float[] { 5, 5 };
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
                if (a.dash)
                    pen.DashPattern = new float[] { 5, 5 };
                var arr = a.to_cubic_bezier();
                PointF[] bez = new PointF[4];
                for (int i = 0; i < 4; i++)
                {
                    bez[i] = new PointF(arr[i].X * 300 / 72, arr[i].Y * 300 / 72);
                }
                e.Graphics.DrawBezier(pen, bez[0], bez[1], bez[2], bez[3]);
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
                    e.Graphics.DrawLine(pe, todrawl[i].X * 300 / 72, todrawl[i].Y * 300 / 72, todrawl[(i + 1) % todrawl.Count].X * 300 / 72, todrawl[(i + 1) % todrawl.Count].Y * 300 / 72);
                }
            }
        }
        private void Print_Paint_Text(PrintPageEventArgs e)
        {
            foreach (var s in TextList)
            {
                var fo = new Font("新細明體", 12);
                if (s.straight == false)
                {
                    e.Graphics.DrawString(s.S, fo, Brushes.Black, s.P.X * 300 / 72, s.P.Y * 300 / 72);
                }
                else
                {
                    for (int i = 0; i < s.S.Length; i++)
                    {
                        e.Graphics.DrawString(s.S[i].ToString(), fo, Brushes.Black, s.P.X * 300 / 72, (s.P.Y + i * 14) * 300 / 72);
                    }
                }
            }
        }
        private void Print_Paint_PathDist(PrintPageEventArgs e, bool normalL_visible, bool Sup_visible)
        {
            var pe_line = new Pen(Color.Gray, 1);
            var fo = new Font("新細明體", 12);
            List<PathDistance> pdl = new List<PathDistance>();
            foreach (var pd in PDistList)
            {
                bool inL = pd.in_List(LineList, CurveList, ArcList, SupLineList);
                PointF pt1, pt2;
                float dist;
                pd.Get_Dist_Point(out dist, out pt1, out pt2);
                if (inL)
                {
                    if (!(pd.is_SupL1 || pd.is_SupL2) && normalL_visible)
                    {
                        e.Graphics.DrawLine(pe_line, pt1.X * 300 / 72, pt1.Y * 300 / 72, pt2.X * 300 / 72, pt2.Y * 300 / 72);
                        PointF mid = new PointF((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2);
                        if (LenthUnit == 0)
                            dist /= 72 / 2.54F;
                        else
                            dist /= 72F;
                        e.Graphics.DrawString(dist.ToString("F") + (LenthUnit == 0 ? " cm" : " inch"), fo, Brushes.Black, mid.X * 300 / 72, mid.Y * 300 / 72);
                    }
                    else if (Sup_visible)
                    {
                        e.Graphics.DrawLine(pe_line, pt1.X * 300 / 72, pt1.Y * 300 / 72, pt2.X * 300 / 72, pt2.Y * 300 / 72);
                        PointF mid = new PointF((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2);
                        if (LenthUnit == 0)
                            dist /= 72 / 2.54F;
                        else
                            dist /= 72F;
                        e.Graphics.DrawString(dist.ToString("F") + (LenthUnit == 0 ? " cm" : " inch"), fo, Brushes.Black, mid.X * 300 / 72, mid.Y * 300 / 72);
                    }
                }
                else
                {
                    pdl.Add(pd);
                }
            }
            foreach (var pd in pdl)
                PDistList.Remove(pd);
        }
        private void Print_Paint_GrainMark(PrintPageEventArgs e)
        {
            if (ClothGrainMark.visible)
            {
                var cgm = ClothGrainMark;
                var td = TabpageDataList[tabControl1.SelectedIndex];
                var color = Color.Black;
                float size = (float)ClothGrainMark.size / 100;
                var textsize = 5 * size;
                var pen = new Pen(color, textsize);
                //pen.StartCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                //pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                PointF mid = cgm.loc_type == 0 ? new PointF(50, 50) :
                             cgm.loc_type == 1 ? new PointF(td.width - 50, 50) :
                             cgm.loc_type == 2 ? new PointF(50, td.height - 50) :
                             cgm.loc_type == 3 ? new PointF(td.width - 50, td.height - 50) : cgm.Loc;
                double angle = cgm.dir_type == 0 ? 90 :
                               cgm.dir_type == 1 ? 0 :
                               cgm.dir_type == 2 ? 45 :
                               cgm.dir_type == 3 ? 135 : cgm.Dir;
                PointF start = new PointF(mid.X + 30 * (float)Math.Cos(angle / 180 * Math.PI) * size, mid.Y + 30 * (float)Math.Sin(angle / 180 * Math.PI) * size),
                       end = new PointF(mid.X - 30 * (float)Math.Cos(angle / 180 * Math.PI) * size, mid.Y - 30 * (float)Math.Sin(angle / 180 * Math.PI) * size);
                e.Graphics.DrawLine(pen, start.X * 300 / 72, start.Y * 300 / 72, end.X * 300 / 72, end.Y * 300 / 72);

                PointF st_ar1 = new PointF(start.X - (float)Math.Cos((angle - 30) / 180 * Math.PI) * 10 * size, start.Y - (float)Math.Sin((angle - 30) / 180 * Math.PI) * 10 * size);
                PointF st_ar2 = new PointF(start.X - (float)Math.Cos((angle + 30) / 180 * Math.PI) * 10 * size, start.Y - (float)Math.Sin((angle + 30) / 180 * Math.PI) * 10 * size);
                e.Graphics.DrawLine(pen, start.X * 300 / 72, start.Y * 300 / 72, st_ar1.X * 300 / 72, st_ar1.Y * 300 / 72);
                e.Graphics.DrawLine(pen, start.X * 300 / 72, start.Y * 300 / 72, st_ar2.X * 300 / 72, st_ar2.Y * 300 / 72);
                PointF en_ar1 = new PointF(end.X + (float)Math.Cos((angle - 30) / 180 * Math.PI) * 10 * size, end.Y + (float)Math.Sin((angle - 30) / 180 * Math.PI) * 10 * size);
                PointF en_ar2 = new PointF(end.X + (float)Math.Cos((angle + 30) / 180 * Math.PI) * 10 * size, end.Y + (float)Math.Sin((angle + 30) / 180 * Math.PI) * 10 * size);
                e.Graphics.DrawLine(pen, end.X * 300 / 72, end.Y * 300 / 72, en_ar1.X * 300 / 72, en_ar1.Y * 300 / 72);
                e.Graphics.DrawLine(pen, end.X * 300 / 72, end.Y * 300 / 72, en_ar2.X * 300 / 72, en_ar2.Y * 300 / 72);
            }
        }

        private void PrintPDF_Paint_Points(Graphics e)
        {
            foreach (var p in PointsList)
            {
                var color = p.MarkL != null ? Color.Orange : Color.Black;
                var size = 1;
                var pen = new Pen(color, size);
                e.DrawRectangle(pen, (p.P.X - 3) * 300 / 96, (p.P.Y - 3) * 300 / 96, 6 * 300 / 96, 6 * 300 / 96);
            }
        }
        private void PrintPDF_Paint_Lines(Graphics e)
        {
            foreach (var line in LineList)
            {
                var color = line.co;
                var size = 1;
                var pen = new Pen(color, size);
                if (line.dash)
                    pen.DashPattern = new float[] { 5, 5 };
                e.DrawLine(pen, line.StartPoint.P.X * 300 / 96, line.StartPoint.P.Y * 300 / 96, line.EndPoint.P.X * 300 / 96, line.EndPoint.P.Y * 300 / 96);
            }
        }
        private void PrintPDF_Paint_SupLines(Graphics e)
        {
            foreach (var line in SupLineList)
            {
                var color = line.co;
                var size = 1;
                var pen = new Pen(color, size);
                pen.DashPattern = new float[] { 5, 5 };
                e.DrawLine(pen, line.StartPoint.P.X * 300 / 96, line.StartPoint.P.Y * 300 / 96, line.EndPoint.P.X * 300 / 96, line.EndPoint.P.Y * 300 / 96);
            }
        }
        private void PrintPDF_Paint_Curves(Graphics e)
        {
            foreach (var c in CurveList)
            {
                var color = c.co;
                var size = 1;
                var pen = new Pen(color, size);
                if (c.dash)
                    pen.DashPattern = new float[] { 5, 5 };
                List<PointF> t = GraphCurveToBez(c);
                for (int i = 0; i < t.Count; i++)
                {
                    t[i] = new PointF(t[i].X * 300 / 96, t[i].Y * 300 / 96);
                }
                e.DrawBeziers(pen, t.ToArray());
            }
        }
        private void PrintPDF_Paint_Arcs(Graphics e)
        {
            foreach (var a in ArcList)
            {
                var color = a.co;
                var size = 1;
                var pen = new Pen(color, size);
                if (a.dash)
                    pen.DashPattern = new float[] { 5, 5 };
                var arr = a.to_cubic_bezier();
                PointF[] bez = new PointF[4];
                for (int i = 0; i < 4; i++)
                {
                    bez[i] = new PointF(arr[i].X * 300 / 96, arr[i].Y * 300 / 96);
                }
                e.DrawBezier(pen, bez[0], bez[1], bez[2], bez[3]);
            }
        }
        private void PrintPDF_Paint_Seam(Graphics e)
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
                    e.DrawLine(pe, todrawl[i].X * 300 / 96, todrawl[i].Y * 300 / 96, todrawl[(i + 1) % todrawl.Count].X * 300 / 96, todrawl[(i + 1) % todrawl.Count].Y * 300 / 96);
                }
            }
        }
        private void PrintPDF_Paint_Text(Graphics e)
        {
            foreach (var s in TextList)
            {
                var fo = new Font("新細明體", 12);
                if (s.straight == false)
                {
                    e.DrawString(s.S, fo, Brushes.Black, s.P.X * 300 / 96, s.P.Y * 300 / 96);
                }
                else
                {
                    for (int i = 0; i < s.S.Length; i++)
                    {
                        e.DrawString(s.S[i].ToString(), fo, Brushes.Black, s.P.X * 300 / 96, (s.P.Y + i * 14) * 300 / 96);
                    }
                }
            }
        }
        private void PrintPDF_Paint_PathDist(Graphics e, bool normalL_visible, bool Sup_visible)
        {
            var pe_line = new Pen(Color.Gray, 1);
            var fo = new Font("新細明體", 12);
            List<PathDistance> pdl = new List<PathDistance>();
            foreach (var pd in PDistList)
            {
                bool inL = pd.in_List(LineList, CurveList, ArcList, SupLineList);
                PointF pt1, pt2;
                float dist;
                pd.Get_Dist_Point(out dist, out pt1, out pt2);
                if (inL)
                {
                    if (!(pd.is_SupL1 || pd.is_SupL2) && normalL_visible)
                    {
                        e.DrawLine(pe_line, pt1.X * 300 / 96, pt1.Y * 300 / 96, pt2.X * 300 / 96, pt2.Y * 300 / 96);
                        PointF mid = new PointF((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2);
                        if (LenthUnit == 0)
                            dist /= 72 / 2.54F;
                        else
                            dist /= 72F;
                        e.DrawString(dist.ToString("F") + (LenthUnit == 0 ? " cm" : " inch"), fo, Brushes.Black, mid.X * 300 / 96, mid.Y * 300 / 96);
                    }
                    else if (Sup_visible)
                    {
                        e.DrawLine(pe_line, pt1.X * 300 / 96, pt1.Y * 300 / 96, pt2.X * 300 / 96, pt2.Y * 300 / 96);
                        PointF mid = new PointF((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2);
                        if (LenthUnit == 0)
                            dist /= 72 / 2.54F;
                        else
                            dist /= 72F;
                        e.DrawString(dist.ToString("F") + (LenthUnit == 0 ? " cm" : " inch"), fo, Brushes.Black, mid.X * 300 / 96, mid.Y * 300 / 96);
                    }
                }
                else
                {
                    pdl.Add(pd);
                }
            }
            foreach (var pd in pdl)
                PDistList.Remove(pd);
        }
        private void PrintPDF_Paint_GrainMark(Graphics e)
        {
            if (ClothGrainMark.visible)
            {
                var cgm = ClothGrainMark;
                var td = TabpageDataList[tabControl1.SelectedIndex];
                var color = Color.Black;
                float size = (float)ClothGrainMark.size / 100;
                var textsize = 5 * size;
                var pen = new Pen(color, textsize);
                pen.StartCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                PointF mid = cgm.loc_type == 0 ? new PointF(50, 50) :
                             cgm.loc_type == 1 ? new PointF(td.width - 50, 50) :
                             cgm.loc_type == 2 ? new PointF(50, td.height - 50) :
                             cgm.loc_type == 3 ? new PointF(td.width - 50, td.height - 50) : cgm.Loc;
                double angle = cgm.dir_type == 0 ? 90 :
                               cgm.dir_type == 1 ? 0 :
                               cgm.dir_type == 2 ? 45 :
                               cgm.dir_type == 3 ? 135 : cgm.Dir;
                PointF start = new PointF(mid.X + 30 * (float)Math.Cos(angle / 180 * Math.PI) * size, mid.Y + 30 * (float)Math.Sin(angle / 180 * Math.PI) * size),
                       end = new PointF(mid.X - 30 * (float)Math.Cos(angle / 180 * Math.PI) * size, mid.Y - 30 * (float)Math.Sin(angle / 180 * Math.PI) * size);
                e.DrawLine(pen, start.X * 300 / 96, start.Y * 300 / 96, end.X * 300 / 96, end.Y * 300 / 96);

                PointF st_ar1 = new PointF(start.X - (float)Math.Cos((angle - 30) / 180 * Math.PI) * 10 * size, start.Y - (float)Math.Sin((angle - 30) / 180 * Math.PI) * 10 * size);
                PointF st_ar2 = new PointF(start.X - (float)Math.Cos((angle + 30) / 180 * Math.PI) * 10 * size, start.Y - (float)Math.Sin((angle + 30) / 180 * Math.PI) * 10 * size);
                e.DrawLine(pen, start.X * 300 / 96, start.Y * 300 / 96, st_ar1.X * 300 / 96, st_ar1.Y * 300 / 96);
                e.DrawLine(pen, start.X * 300 / 96, start.Y * 300 / 96, st_ar2.X * 300 / 96, st_ar2.Y * 300 / 96);
                PointF en_ar1 = new PointF(end.X + (float)Math.Cos((angle - 30) / 180 * Math.PI) * 10 * size, end.Y + (float)Math.Sin((angle - 30) / 180 * Math.PI) * 10 * size);
                PointF en_ar2 = new PointF(end.X + (float)Math.Cos((angle + 30) / 180 * Math.PI) * 10 * size, end.Y + (float)Math.Sin((angle + 30) / 180 * Math.PI) * 10 * size);
                e.DrawLine(pen, end.X * 300 / 96, end.Y * 300 / 96, en_ar1.X * 300 / 96, en_ar1.Y * 300 / 96);
                e.DrawLine(pen, end.X * 300 / 96, end.Y * 300 / 96, en_ar2.X * 300 / 96, en_ar2.Y * 300 / 96);
            }
        }

        private void 預覽列印ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            輸出設定 form = new 輸出設定();
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK)
            {
                output_containt = form.output_containt;
                printDocument1.DefaultPageSettings.PaperSize = new PaperSize("paper size", TabpageDataList[tabControl1.SelectedIndex].width * 100 / 72, TabpageDataList[tabControl1.SelectedIndex].height * 100 / 72);
                printPreviewDialog1.Document = printDocument1;
                printPreviewDialog1.ShowDialog();
                
            }
        }

        private void 匯出PDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            輸出設定 form = new 輸出設定();
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK)
            {
                output_containt = form.output_containt;
                string fname = "";
                if (saveFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    fname = saveFileDialog2.FileName;
                }
                else
                    return;


                TabpageData td = TabpageDataList[tabControl1.SelectedIndex];
                Image bmp = new Bitmap(td.width, td.height);
                Graphics g = Graphics.FromImage(bmp);
                iTextSharp.text.Document myDoc = new iTextSharp.text.Document(new iTextSharp.text.Rectangle(td.width, td.height), 0, 0, 0, 0);
                g.Clear(Color.White);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PageUnit = GraphicsUnit.Document;//線 縫份 輔 距 輔距 點 布紋 字 
                if(output_containt[6])
                    PrintPDF_Paint_GrainMark(g);
                if (output_containt[5])
                    PrintPDF_Paint_Points(g);
                if (output_containt[2])
                    PrintPDF_Paint_SupLines(g);
                PrintPDF_Paint_Lines(g);
                PrintPDF_Paint_Curves(g);
                PrintPDF_Paint_Arcs(g);
                if(output_containt[1])
                    PrintPDF_Paint_Seam(g);
                if(output_containt[7])
                    PrintPDF_Paint_Text(g);
                PrintPDF_Paint_PathDist(g, output_containt[3], output_containt[4]);
                bmp.Save("testjpg.jpg", ImageFormat.Jpeg);
                try
                {
                    FileStream fs = new FileStream(fname, FileMode.Create);
                    iTextSharp.text.pdf.PdfWriter.GetInstance(myDoc, fs);
                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance("testjpg.jpg");
                    myDoc.Open();
                    myDoc.Add(image);//加入影像
                    //myDoc.AddTitle("Tutorial-Add image files");//文件標題
                    //myDoc.AddAuthor("einboch");//文件作者
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (myDoc.IsOpen()) myDoc.Close();

                }
            }

            
        }
    }
}
