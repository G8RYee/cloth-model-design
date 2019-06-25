namespace 繪圖
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.新影像ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.儲存影像ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.調整大小ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton7 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox2 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripButton8 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton9 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton10 = new System.Windows.Forms.ToolStripButton();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.重新命名ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.刪除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.新增節點ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.曲線樣式ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.平滑ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.拉直ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.無ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.群組ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.組成群組ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.取消群組ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.轉換為曲線ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1047, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3,
            this.新影像ToolStripMenuItem,
            this.儲存影像ToolStripMenuItem,
            this.調整大小ToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(43, 20);
            this.toolStripMenuItem1.Text = "檔案";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(122, 22);
            this.toolStripMenuItem3.Text = "開啟舊檔";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // 新影像ToolStripMenuItem
            // 
            this.新影像ToolStripMenuItem.Name = "新影像ToolStripMenuItem";
            this.新影像ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.新影像ToolStripMenuItem.Text = "新影像";
            this.新影像ToolStripMenuItem.Click += new System.EventHandler(this.新影像ToolStripMenuItem_Click);
            // 
            // 儲存影像ToolStripMenuItem
            // 
            this.儲存影像ToolStripMenuItem.Name = "儲存影像ToolStripMenuItem";
            this.儲存影像ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.儲存影像ToolStripMenuItem.Text = "儲存影像";
            this.儲存影像ToolStripMenuItem.Click += new System.EventHandler(this.儲存影像ToolStripMenuItem_Click);
            // 
            // 調整大小ToolStripMenuItem
            // 
            this.調整大小ToolStripMenuItem.Name = "調整大小ToolStripMenuItem";
            this.調整大小ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.調整大小ToolStripMenuItem.Text = "調整大小";
            this.調整大小ToolStripMenuItem.Click += new System.EventHandler(this.調整大小ToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton5,
            this.toolStripButton6,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripButton7,
            this.toolStripButton4,
            this.toolStripLabel1,
            this.toolStripTextBox1,
            this.toolStripComboBox1,
            this.toolStripLabel2,
            this.toolStripTextBox2,
            this.toolStripButton8,
            this.toolStripButton9,
            this.toolStripButton10});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1047, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.ToolTipText = "指標";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton5.Enabled = false;
            this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton5.Text = "toolStripButton5";
            this.toolStripButton5.ToolTipText = "復原";
            this.toolStripButton5.Click += new System.EventHandler(this.toolStripButton5_Click);
            // 
            // toolStripButton6
            // 
            this.toolStripButton6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton6.Enabled = false;
            this.toolStripButton6.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton6.Image")));
            this.toolStripButton6.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton6.Name = "toolStripButton6";
            this.toolStripButton6.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton6.Text = "toolStripButton6";
            this.toolStripButton6.ToolTipText = "重作";
            this.toolStripButton6.Click += new System.EventHandler(this.toolStripButton6_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "toolStripButton2";
            this.toolStripButton2.ToolTipText = "直線";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton3.Text = "toolStripButton3";
            this.toolStripButton3.ToolTipText = "曲線";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // toolStripButton7
            // 
            this.toolStripButton7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton7.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton7.Image")));
            this.toolStripButton7.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton7.Name = "toolStripButton7";
            this.toolStripButton7.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton7.Text = "toolStripButton7";
            this.toolStripButton7.ToolTipText = "圓弧弧線";
            this.toolStripButton7.Click += new System.EventHandler(this.toolStripButton7_Click);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton4.Text = "網格";
            this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(58, 22);
            this.toolStripLabel1.Text = "網格大小:";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.AutoSize = false;
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 25);
            this.toolStripTextBox1.Leave += new System.EventHandler(this.toolStripTextBox1_Leave);
            this.toolStripTextBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStripTextBox1_KeyDown);
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.AutoCompleteCustomSource.AddRange(new string[] {
            "inch",
            "cm"});
            this.toolStripComboBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.toolStripComboBox1.CausesValidation = false;
            this.toolStripComboBox1.Items.AddRange(new object[] {
            "inch",
            "cm"});
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(75, 25);
            this.toolStripComboBox1.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox1_SelectedIndexChanged);
            this.toolStripComboBox1.Leave += new System.EventHandler(this.toolStripComboBox1_Leave);
            this.toolStripComboBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStripComboBox1_KeyDown);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(43, 22);
            this.toolStripLabel2.Text = "細緻度";
            // 
            // toolStripTextBox2
            // 
            this.toolStripTextBox2.Name = "toolStripTextBox2";
            this.toolStripTextBox2.Size = new System.Drawing.Size(100, 25);
            this.toolStripTextBox2.Text = "4";
            this.toolStripTextBox2.Leave += new System.EventHandler(this.toolStripTextBox2_Leave);
            this.toolStripTextBox2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStripTextBox2_KeyDown);
            // 
            // toolStripButton8
            // 
            this.toolStripButton8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton8.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton8.Image")));
            this.toolStripButton8.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton8.Name = "toolStripButton8";
            this.toolStripButton8.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton8.Text = "toolStripButton8";
            this.toolStripButton8.ToolTipText = "放大檢視";
            this.toolStripButton8.Click += new System.EventHandler(this.toolStripButton8_Click);
            // 
            // toolStripButton9
            // 
            this.toolStripButton9.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton9.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton9.Image")));
            this.toolStripButton9.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton9.Name = "toolStripButton9";
            this.toolStripButton9.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton9.Text = "toolStripButton9";
            this.toolStripButton9.ToolTipText = "縮小檢視";
            this.toolStripButton9.Click += new System.EventHandler(this.toolStripButton9_Click);
            // 
            // toolStripButton10
            // 
            this.toolStripButton10.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton10.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton10.Image")));
            this.toolStripButton10.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton10.Name = "toolStripButton10";
            this.toolStripButton10.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton10.Text = "toolStripButton10";
            this.toolStripButton10.ToolTipText = "隱藏節點";
            this.toolStripButton10.Click += new System.EventHandler(this.toolStripButton10_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "專案檔|*.cds";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "專案檔|*.cds";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(0, 52);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.AutoScroll = true;
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            this.splitContainer1.Panel1.SizeChanged += new System.EventHandler(this.splitContainer1_Panel1_SizeChanged);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.splitContainer1.Panel2.SizeChanged += new System.EventHandler(this.splitContainer1_Panel2_SizeChanged);
            this.splitContainer1.Size = new System.Drawing.Size(873, 428);
            this.splitContainer1.SplitterDistance = 682;
            this.splitContainer1.TabIndex = 4;
            // 
            // tabControl1
            // 
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(679, 425);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tabControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tabControl1_KeyDown);
            this.tabControl1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tabControl1_KeyUp);
            this.tabControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tabControl1_MouseDown);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.重新命名ToolStripMenuItem,
            this.刪除ToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(123, 48);
            // 
            // 重新命名ToolStripMenuItem
            // 
            this.重新命名ToolStripMenuItem.Name = "重新命名ToolStripMenuItem";
            this.重新命名ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.重新命名ToolStripMenuItem.Text = "重新命名";
            this.重新命名ToolStripMenuItem.Click += new System.EventHandler(this.重新命名ToolStripMenuItem_Click);
            // 
            // 刪除ToolStripMenuItem
            // 
            this.刪除ToolStripMenuItem.Name = "刪除ToolStripMenuItem";
            this.刪除ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.刪除ToolStripMenuItem.Text = "刪除";
            this.刪除ToolStripMenuItem.Click += new System.EventHandler(this.刪除ToolStripMenuItem_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(879, 168);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(126, 92);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDoubleClick);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel3});
            this.statusStrip1.Location = new System.Drawing.Point(0, 507);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1047, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(18, 17);
            this.toolStripStatusLabel1.Text = "X:";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(17, 17);
            this.toolStripStatusLabel2.Text = "Y:";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(58, 17);
            this.toolStripStatusLabel3.Text = "縮放大小:";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.新增節點ToolStripMenuItem,
            this.曲線樣式ToolStripMenuItem,
            this.群組ToolStripMenuItem,
            this.轉換為曲線ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(135, 114);
            this.contextMenuStrip1.Tag = "畫線用";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(134, 22);
            this.toolStripMenuItem2.Text = "刪除";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // 新增節點ToolStripMenuItem
            // 
            this.新增節點ToolStripMenuItem.Name = "新增節點ToolStripMenuItem";
            this.新增節點ToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.新增節點ToolStripMenuItem.Text = "新增節點";
            this.新增節點ToolStripMenuItem.Click += new System.EventHandler(this.新增節點ToolStripMenuItem_Click);
            // 
            // 曲線樣式ToolStripMenuItem
            // 
            this.曲線樣式ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.平滑ToolStripMenuItem,
            this.拉直ToolStripMenuItem,
            this.無ToolStripMenuItem});
            this.曲線樣式ToolStripMenuItem.Name = "曲線樣式ToolStripMenuItem";
            this.曲線樣式ToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.曲線樣式ToolStripMenuItem.Text = "曲線樣式";
            // 
            // 平滑ToolStripMenuItem
            // 
            this.平滑ToolStripMenuItem.Name = "平滑ToolStripMenuItem";
            this.平滑ToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.平滑ToolStripMenuItem.Text = "平滑";
            this.平滑ToolStripMenuItem.Click += new System.EventHandler(this.平滑ToolStripMenuItem_Click);
            // 
            // 拉直ToolStripMenuItem
            // 
            this.拉直ToolStripMenuItem.Name = "拉直ToolStripMenuItem";
            this.拉直ToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.拉直ToolStripMenuItem.Text = "拉直";
            this.拉直ToolStripMenuItem.Click += new System.EventHandler(this.拉直ToolStripMenuItem_Click);
            // 
            // 無ToolStripMenuItem
            // 
            this.無ToolStripMenuItem.Name = "無ToolStripMenuItem";
            this.無ToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.無ToolStripMenuItem.Text = "無";
            this.無ToolStripMenuItem.Click += new System.EventHandler(this.無ToolStripMenuItem_Click);
            // 
            // 群組ToolStripMenuItem
            // 
            this.群組ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.組成群組ToolStripMenuItem,
            this.取消群組ToolStripMenuItem});
            this.群組ToolStripMenuItem.Name = "群組ToolStripMenuItem";
            this.群組ToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.群組ToolStripMenuItem.Text = "群組";
            // 
            // 組成群組ToolStripMenuItem
            // 
            this.組成群組ToolStripMenuItem.Name = "組成群組ToolStripMenuItem";
            this.組成群組ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.組成群組ToolStripMenuItem.Text = "組成群組";
            this.組成群組ToolStripMenuItem.Click += new System.EventHandler(this.組成群組ToolStripMenuItem_Click);
            // 
            // 取消群組ToolStripMenuItem
            // 
            this.取消群組ToolStripMenuItem.Name = "取消群組ToolStripMenuItem";
            this.取消群組ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.取消群組ToolStripMenuItem.Text = "取消群組";
            this.取消群組ToolStripMenuItem.Click += new System.EventHandler(this.取消群組ToolStripMenuItem_Click);
            // 
            // 轉換為曲線ToolStripMenuItem
            // 
            this.轉換為曲線ToolStripMenuItem.Name = "轉換為曲線ToolStripMenuItem";
            this.轉換為曲線ToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.轉換為曲線ToolStripMenuItem.Text = "轉換為曲線";
            this.轉換為曲線ToolStripMenuItem.Click += new System.EventHandler(this.轉換為曲線ToolStripMenuItem_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(879, 279);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(100, 50);
            this.pictureBox2.TabIndex = 6;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Visible = false;
            this.pictureBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox2_Paint);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1047, 529);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem 新影像ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 儲存影像ToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripMenuItem 新增節點ToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem 重新命名ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 刪除ToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripMenuItem 曲線樣式ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 平滑ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 拉直ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 無ToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripMenuItem 群組ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 組成群組ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 取消群組ToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton5;
        private System.Windows.Forms.ToolStripButton toolStripButton6;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.ToolStripButton toolStripButton7;
        private System.Windows.Forms.ToolStripMenuItem 轉換為曲線ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 調整大小ToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton8;
        private System.Windows.Forms.ToolStripButton toolStripButton9;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripButton toolStripButton10;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox2;
    }
}

