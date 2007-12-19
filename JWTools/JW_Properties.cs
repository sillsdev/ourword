/**********************************************************************************************
 * Dll:     JWTools
 * File:    JW_Properties.cs
 * Author:  John Wimbish
 * Created: 28 Dec 2004
 * Purpose: Properties Page superclass
 * Legal:   Copyright (c) 2004-07, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using JWTools;
using DevComponents.DotNetBar;
#endregion

namespace JWTools
{
	public class JW_Properties : System.Windows.Forms.Form
	{
		// Attrs -----------------------------------------------------------------------------
		#region Attr{g}: JW_PropItem ActivePage
		public JW_PropItem ActivePage
		{
			get
			{
				foreach( JW_PropGroup pg in m_ExplorerBar.Groups)
				{
					foreach( JW_PropItem pi in pg.SubItems)
					{
						if (pi.Panel.Visible)
							return pi;
					}
				}
				return null;
			}
		}
		#endregion
		#region Attr{g}: ExplorerBar ExplorerBar
		public ExplorerBar ExplorerBar
		{
			get
			{
				return m_ExplorerBar;
			}
		}
		#endregion

		// Explorer Bar ----------------------------------------------------------------------
		#region Attr{g}: JW_PropGroup LastGroupAdded
		public JW_PropGroup LastGroupAdded
		{
			get
			{
				return m_LastGroupAdded;
			}
		}
		JW_PropGroup m_LastGroupAdded = null;
		#endregion
		#region Method: JW_PropGroup AddGroup(sText, bExpanded)
		public JW_PropGroup AddGroup(string sText, bool bExpanded)
		{
			JW_PropGroup pg = new JW_PropGroup(sText, bExpanded);
			m_ExplorerBar.Groups.Add(pg);
			m_LastGroupAdded = pg;
			return pg;
		}
		#endregion
		#region Method: JW_PropItem AddItem(sText)
		public JW_PropItem AddItem(string sText, UserControl wnd)
		{
			Debug.Assert(null != LastGroupAdded);
			JW_PropItem pi = new JW_PropItem(this,sText, wnd);
			LastGroupAdded.SubItems.Add(pi);

			pi.Panel.Left = m_ExplorerBar.Width;
			pi.Panel.Top  = 0;

			m_ExplorerBar.RecalcLayout();

			return pi;
		}
		#endregion
		#region Method: JW_PropItem InsertItem(sText)
		public JW_PropItem InsertItem(string sText, UserControl wnd, JW_PropGroup pg, int iPos)
		{
			Debug.Assert(null != pg);
			JW_PropItem pi = new JW_PropItem(this, sText, wnd);
			pg.SubItems.Insert(iPos, pi);

			pi.Panel.Left = m_ExplorerBar.Width;
			pi.Panel.Top  = 0;

			RecalcExplorerBarLayout();

			return pi;
		}
		#endregion
		#region Method: void RemoveItem(JW_PropItem)
		public void RemoveItem(JW_PropItem item)
		{
			foreach(JW_PropGroup pg in m_ExplorerBar.Groups)
			{
				foreach(JW_PropItem pi in pg.SubItems)
				{
					if (pi == item)
					{
						pg.SubItems.Remove(pi);
						return;
					}
				}
			}
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public JW_Properties()
		{
			// Required for Windows Form Designer support
			InitializeComponent();
		}
		#endregion
		#region Method: void Dispose(...)
		protected override void Dispose( bool disposing )
			// Clean up any resources being used.
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion
		#region Windows Form Designer generated code
		// Controls
		private DevComponents.DotNetBar.ExplorerBar m_ExplorerBar;
		private DevComponents.DotNetBar.PanelEx m_BottomPanel;
		private System.Windows.Forms.Button m_btnHelp;
		private System.Windows.Forms.Button m_btnOK;

		// Required designer variable
		private System.ComponentModel.Container components = null;

		#region InitializeComponent()
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(JW_Properties));
			this.m_ExplorerBar = new DevComponents.DotNetBar.ExplorerBar();
			this.m_BottomPanel = new DevComponents.DotNetBar.PanelEx();
			this.m_btnHelp = new System.Windows.Forms.Button();
			this.m_btnOK = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.m_ExplorerBar)).BeginInit();
			this.m_BottomPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_ExplorerBar
			// 
			this.m_ExplorerBar.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
			this.m_ExplorerBar.BackStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ExplorerBarBackground;
			this.m_ExplorerBar.BackStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.ExplorerBarBackground2;
			this.m_ExplorerBar.BackStyle.BackColorGradientAngle = 90;
			this.m_ExplorerBar.Dock = System.Windows.Forms.DockStyle.Left;
			this.m_ExplorerBar.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
			this.m_ExplorerBar.GroupImages = null;
			this.m_ExplorerBar.Images = null;
			this.m_ExplorerBar.Location = new System.Drawing.Point(0, 0);
			this.m_ExplorerBar.Name = "m_ExplorerBar";
			this.m_ExplorerBar.Size = new System.Drawing.Size(152, 483);
			this.m_ExplorerBar.TabIndex = 0;
			this.m_ExplorerBar.TabStop = false;
			// 
			// m_BottomPanel
			// 
			this.m_BottomPanel.Controls.Add(this.m_btnHelp);
			this.m_BottomPanel.Controls.Add(this.m_btnOK);
			this.m_BottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.m_BottomPanel.Location = new System.Drawing.Point(152, 443);
			this.m_BottomPanel.Name = "m_BottomPanel";
			this.m_BottomPanel.Size = new System.Drawing.Size(504, 40);
			this.m_BottomPanel.Style.Alignment = System.Drawing.StringAlignment.Center;
			this.m_BottomPanel.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
			this.m_BottomPanel.Style.BackColor2.Color = System.Drawing.Color.FromArgb(((System.Byte)(89)), ((System.Byte)(135)), ((System.Byte)(214)));
			this.m_BottomPanel.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
			this.m_BottomPanel.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
			this.m_BottomPanel.Style.GradientAngle = 90;
			this.m_BottomPanel.TabIndex = 41;
			// 
			// m_btnHelp
			// 
			this.m_btnHelp.BackColor = System.Drawing.SystemColors.Control;
			this.m_btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("m_btnHelp.Image")));
			this.m_btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.m_btnHelp.Location = new System.Drawing.Point(248, 8);
			this.m_btnHelp.Name = "m_btnHelp";
			this.m_btnHelp.TabIndex = 101;
			this.m_btnHelp.Text = "Help...";
			this.m_btnHelp.Click += new System.EventHandler(this.cmdHelp);
			// 
			// m_btnOK
			// 
			this.m_btnOK.BackColor = System.Drawing.SystemColors.Control;
			this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btnOK.Location = new System.Drawing.Point(168, 8);
			this.m_btnOK.Name = "m_btnOK";
			this.m_btnOK.TabIndex = 100;
			this.m_btnOK.Text = "OK";
			// 
			// JW_Properties
			// 
			this.AcceptButton = this.m_btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(656, 483);
			this.Controls.Add(this.m_BottomPanel);
			this.Controls.Add(this.m_ExplorerBar);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "JW_Properties";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Properties";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.cmdClosing);
			this.Load += new System.EventHandler(this.cmdLoad);
			((System.ComponentModel.ISupportInitialize)(this.m_ExplorerBar)).EndInit();
			this.m_BottomPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion

		// Subclass overload hooks -----------------------------------------------------------
		#region Method: virtual void onLoad() - hook for subclass
		protected virtual void onLoad()
		{
		}
		#endregion
		#region Method: virtual void onClosing() - hook for subclass
		protected virtual bool onClosing()
		{
			return true;
		}
		#endregion

		// Methods ---------------------------------------------------------------------------
		#region Method: void HidePanels()
		public void HidePanels()
		{
			foreach(JW_PropGroup pg in m_ExplorerBar.Groups)
			{
				foreach(JW_PropItem pi in pg.SubItems)
				{
					pi.Hide();
				}
			}
		}
		#endregion
		#region Method: void ShowPanel(JW_PropItem)
		public void ShowPanel( JW_PropItem item )
		{
			item.Show();
		}
		#endregion
		#region Method: void ShowPanel(string sText)
		public void ShowPanel(string sText)
		{
			foreach( JW_PropGroup pg in m_ExplorerBar.Groups)
			{
				foreach( JW_PropItem pi in pg.SubItems)
				{
					if (pi.Text == sText)
					{
						ShowPanel(pi);
						break;
					}
				}
			}
		}
		#endregion
		#region Method: void Localization()
		virtual protected void Localization()
		{
			m_btnOK.Text = DlgRes.OK;
			m_btnHelp.Text = DlgRes.Help;
		}
		#endregion
		#region Method: void RecalcExplorerBarLayout()
		public void RecalcExplorerBarLayout()
		{
			m_ExplorerBar.RecalcLayout();
			m_ExplorerBar.Refresh();
		}
		#endregion
		#region Method: bool ValidateActivePage() - true if page is OK
		public bool ValidateActivePage()
		{
			if (null != ActivePage)
			{
				IJW_PropPage pp = ActivePage.Panel as IJW_PropPage;
				return pp.Validate();
			}
			return true;
		}
		#endregion

		// Handlers --------------------------------------------------------------------------
		#region Handler: cmdLoad
		private void cmdLoad(object sender, System.EventArgs e)
		{
			// UI Language
			Localization();

			// Calculate the dimensions of the largest panel
			int nWidthMax = 0;
			int nHeightMax = 0;
			foreach(JW_PropGroup pg in m_ExplorerBar.Groups)
			{
				foreach(JW_PropItem pi in pg.SubItems)
				{
					// While we're at it, place into the proper left,top position
					pi.Panel.Left = m_ExplorerBar.Width;
					pi.Panel.Top  = 0;

					// Dimensions of the largest panel
					nWidthMax  = Math.Max(nWidthMax, pi.PanelRect.Width);
					nHeightMax = Math.Max(nHeightMax, pi.PanelRect.Height);
				}
			}

			// Set our size to exactly accomdate the largest
			int nWidthNonclient  = Width  - ClientRectangle.Width;
			int nHeightNonclient = Height - ClientRectangle.Height;
			int nWidth  = nWidthMax  + m_ExplorerBar.Width  + nWidthNonclient;
			int nHeight = nHeightMax + m_BottomPanel.Height + nHeightNonclient;
			Size = new Size(nWidth, nHeight);

			// Center the buttons in the bottom panel (assumes only an OK and a Help button)
			int nBtnSpace = m_btnHelp.Left - m_btnOK.Right;
			int nBtnTotalWidth = m_btnHelp.Right - m_btnOK.Left;
			m_btnOK.Left = ( m_BottomPanel.Width - nBtnTotalWidth ) / 2;
			m_btnHelp.Left = m_btnOK.Right + nBtnSpace;

			// Give the subclass a chance to do its own loading thing
			onLoad();
		}
		#endregion
		#region Handler: cmdClosing
		private void cmdClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// We're only interested in further processing if the user has hit the OK
			// button, signallying he is done and wishes to save his results.
			if (DialogResult != DialogResult.OK)
				return;

			// Validate the active page
			if (false == ValidateActivePage())
			{
				e.Cancel = true;
				return;
			}

			// Let the subclass do any Closing processing; abort if subclass returns false
			if (! onClosing() )
				e.Cancel = true;
		}
		#endregion
		#region Handler: virtual cmdHelp
		protected virtual void cmdHelp(object sender, System.EventArgs e)
		{
		}
		#endregion
	}

	public class JW_PropGroup : ExplorerBarGroupItem
	{
		#region Attr{g}: string NextName
		static string NextName
		{
			get
			{
				char ch = (char)((int)'a' + s_nNextName);
				string s = new string(ch, 1);
				s_nNextName++;
				return s;
			}
		}
		static int s_nNextName = 0;
		#endregion

		#region Constructor(sText)
		public JW_PropGroup(string sText, bool bExpanded)
			: base(NextName, sText)
		{
			Expanded = bExpanded;
			ExpandButtonVisible = (bExpanded) ? false : true;
			HeaderExpands = (bExpanded) ? false : true;

			BackStyle.BackColor = 
				System.Drawing.Color.FromArgb(((System.Byte)(214)), 
				((System.Byte)(223)), ((System.Byte)(247)));
			BackStyle.Border = DevComponents.DotNetBar.eStyleBorderType.Solid;
			BackStyle.BorderColor = System.Drawing.Color.White;
			TitleHotStyle.BackColor = System.Drawing.Color.White;
			TitleHotStyle.BackColor2 = 
				System.Drawing.Color.FromArgb(((System.Byte)(199)), 
				((System.Byte)(211)), ((System.Byte)(247)));
			TitleHotStyle.Font = new System.Drawing.Font("Tahoma", 11F, 
				System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
			TitleHotStyle.TextColor = 
				System.Drawing.Color.FromArgb(((System.Byte)(66)), 
				((System.Byte)(142)), ((System.Byte)(255)));
			TitleStyle.BackColor = System.Drawing.Color.White;
			TitleStyle.BackColor2 = 
				System.Drawing.Color.FromArgb(((System.Byte)(199)), 
				((System.Byte)(211)), ((System.Byte)(247)));
			TitleStyle.Font = new System.Drawing.Font("Tahoma", 11F, 
				System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
			TitleStyle.TextColor = 
				System.Drawing.Color.FromArgb(((System.Byte)(33)), 
				((System.Byte)(93)), ((System.Byte)(198)));
		}
		#endregion
	}

	public class JW_PropItem : ButtonItem
	{
		#region Attr{g}: Rectangle PanelRect
		public Rectangle PanelRect
		{
			get
			{
				Debug.Assert(null != m_Panel);
				Rectangle r = new Rectangle(0,0, m_Panel.Width, m_Panel.Height);
				return r;
			}
		}
		#endregion
		JW_Properties m_ParentDlg = null;
		#region Attr{g}: UserControl Panel
		public UserControl Panel
		{
			get
			{
				Debug.Assert(null != m_Panel);
				return m_Panel;
			}
		}
		UserControl m_Panel = null;
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Attr{g}: string NextName
		static string NextName
		{
			get
			{
				char ch = (char)((int)'a' + s_nNextName);
				string s = new string(ch, 1);
				s_nNextName++;
				return s;
			}
		}
		static int s_nNextName = 0;
		#endregion
		#region Constructor(...)
		public JW_PropItem(JW_Properties parent, string sText, UserControl panel)
			: base(NextName, sText)
		{
			m_ParentDlg = parent;
			m_Panel  = panel;

			ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
			Cursor = System.Windows.Forms.Cursors.Hand;
			SetNormalAttrs();
			HotFontUnderline = true;
			HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
			Click += new System.EventHandler(cmdClicked);
			Visible = true;
			m_ParentDlg.Controls.Add(m_Panel);

			Hide();
		}
		#endregion

		// Handlers --------------------------------------------------------------------------
		#region Handler: cmdClicked
		private void cmdClicked(object sender, System.EventArgs e)
		{
			// We don't want to change pages unless the active one validates properly
			if (false == m_ParentDlg.ValidateActivePage())
				return;

			// Hide all other pages; show this one
			Show();
		}
		#endregion

		// Methods ---------------------------------------------------------------------------
		#region Method: void SetNormalAttrs() - normal appearance of the nav text
		void SetNormalAttrs()
		{
			ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(33)), 
				((System.Byte)(93)), ((System.Byte)(198)));
			HotForeColor = System.Drawing.Color.FromArgb(((System.Byte)(66)), 
				((System.Byte)(142)), ((System.Byte)(255)));
		}
		#endregion
		#region Method: void SetShowAttrs() - selected appearance of the nav text
		void SetShowAttrs()
		{
			ForeColor    = Color.DarkRed;
			HotForeColor = Color.DarkRed;
		}
		#endregion

		#region Method: void Show() - Activates the panel (hides all others)
		public void Show()
		{
			m_ParentDlg.HidePanels();

			IJW_PropPage pp = m_Panel as IJW_PropPage;
			pp.OnActivate();

			m_Panel.Visible = true;

			SetShowAttrs();
		}
		#endregion
		#region Method: void Hide() - Hides this panel
		public void Hide()
		{
			m_Panel.Visible = false;
			SetNormalAttrs();
		}
		#endregion

		#region Method: void UpdateText(string sText)
		public void UpdateText(string sNewText)
		{
			Text = sNewText;
			m_ParentDlg.ExplorerBar.Refresh();
		}
		#endregion
	}

	public interface IJW_PropPage
	{
		void OnActivate();
		bool Validate();     // return trus if OK
		void ShowHelp();
	}

}
