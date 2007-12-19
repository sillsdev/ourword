/**********************************************************************************************
 * Project: Our Word!
 * File:    Layouts\JobConfigure.cs
 * Author:  John Wimbish
 * Created: 13 Aug 2004
 * Purpose: Modifies / edits the settings for the Drafts layout.
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using JWTools;
using JWdb;
using OurWord.DataModel;
using OurWord.Edit;
#endregion


namespace OurWord.View
{
	#region CLASS JobConfigureDlg
	public class JobConfigureDlg : System.Windows.Forms.Form
	{
		// Tabs Management -------------------------------------------------------------------
		public enum Tabs 
		{ 
			kGeneral, kNotes, kUnspecified 
		};
		#region Method: void MakeTabActive( Tabs tab ) - brings the desired tab to the front
		public void MakeTabActive( Tabs tab )
		{
			switch (tab)
			{
				case Tabs.kGeneral:
					m_tabControl.SelectedTab = m_tabGeneral;
					break;
				case Tabs.kNotes:
					m_tabControl.SelectedTab = m_tabNotes;
					break;
			}
		}
		#endregion
		#region Method: void RemoveTab( Tabs tab ) - Removes the tab from the dialog
		public void RemoveTab( Tabs tab )
		{
			switch (tab)
			{
				case Tabs.kGeneral:
					m_tabControl.TabPages.Remove(m_tabGeneral);
					break;
				case Tabs.kNotes:
					m_tabControl.TabPages.Remove(m_tabNotes);
					break;
			}

			// Removing tabs can get the active tab in a wierd state
			MakeTabActive( Tabs.kGeneral );
		}
		#endregion

		// Options ---------------------------------------------------------------------------
		#region Attr{g}: Options Options - the Options class we are modifying with this dlg
		public Options Options
		{
			get
			{
				Debug.Assert(null != m_Options);
				return m_Options;
			}
		}
		Options m_Options = null;
		#endregion

        // UI Language Names (in the combo box) ----------------------------------------------
        #region CONTROLS
        // Also used in the TranslationStagesPage, need to generalize this somehow!
		public const string c_langEnglish    = "English";
        public const string c_langIndonesian = "Bahasa Indonesia";
        public const string c_langSwahili    = "Kiswahili";
        public const string c_langSpanish = "Español";
        private TabPage m_tabNotes;
        private GroupBox m_groupNotes;
        private Label m_lblNoteBackgroundColor;
        private CheckedListBox m_checkListNotes;
        private ColorCombo m_clrNoteBkg;
        private CheckBox m_checkHintsFromFront;
        private TabPage m_tabGeneral;
        private Label m_labelZoom;
        private ComboBox m_comboZoom;
        private TabControl m_tabControl;
        #endregion


        // Notes -----------------------------------------------------------------------------
		ArrayList aNoteDefs = new ArrayList();
		#region Method: void _Notes_OnLoad()
		void _Notes_OnLoad()
		{
			CheckedListBox.ObjectCollection items = m_checkListNotes.Items;

			items.Clear();
			aNoteDefs.Clear();

			int count = 0;

			foreach(DNoteDef nd in DNote.NoteDefs)
			{
				if (!nd.IsCombined)
				{
					aNoteDefs.Add(nd);
					items.Add(nd.DisplayName, nd.Show);
					++count;
				}
			}

			if (count > 0)
			{
				m_checkListNotes.SelectedIndex = 0;
				DNoteDef nd = DNote.NoteDefs[0] as DNoteDef;
				m_clrNoteBkg.InitialColor = nd.BackgroundColor.Name;
			}
		}
		#endregion
		#region Method: void _Notes_OnClosing()
		void _Notes_OnClosing()
		{
			for(int i=0; i < aNoteDefs.Count; i++)
			{
				DNoteDef nd = aNoteDefs[i] as DNoteDef;
				nd.Show = m_checkListNotes.GetItemChecked(i);
			}
		}
		#endregion
		#region Cmd: : cmd_OnNotesSelectedIndexChanged(...)
		private void cmd_OnNotesSelectedIndexChanged(object sender, System.EventArgs e)
		{
			int i = m_checkListNotes.SelectedIndex;

			DNoteDef nd = aNoteDefs[i] as DNoteDef;

			m_clrNoteBkg.ChosenColor = nd.BackgroundColor.Name;
		}
		#endregion
		#region Cmd: cmd_NoteBkgColorChanged
		private void cmd_NoteBkgColorChanged(object sender, System.EventArgs e)
		{
			int i = m_checkListNotes.SelectedIndex;

			if (i >=0 && i < aNoteDefs.Count)
			{
				DNoteDef nd = aNoteDefs[i] as DNoteDef;

				nd.BackgroundColorName = m_clrNoteBkg.ChosenColor;
			}
		}
		#endregion

        // Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public JobConfigureDlg(Options opts)
		{
			InitializeComponent();
			m_Options = opts;
		}
		#endregion
		#region Method: Dispose(...)
		protected override void Dispose( bool disposing )
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
		#region DIALOG CONTROLS

        // My subclasses of Dialog controls

        // Dialog Controls
		private Button     m_btnOK;
		private Button     m_btnCancel;
        private Button m_btnHelp;      // The result strings for when replacement is done
		#endregion
		#region Windows Form Designer generated code
		private System.ComponentModel.Container components = null;
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JobConfigureDlg));
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnHelp = new System.Windows.Forms.Button();
            this.m_tabNotes = new System.Windows.Forms.TabPage();
            this.m_groupNotes = new System.Windows.Forms.GroupBox();
            this.m_lblNoteBackgroundColor = new System.Windows.Forms.Label();
            this.m_checkListNotes = new System.Windows.Forms.CheckedListBox();
            this.m_clrNoteBkg = new OurWord.View.ColorCombo();
            this.m_checkHintsFromFront = new System.Windows.Forms.CheckBox();
            this.m_tabGeneral = new System.Windows.Forms.TabPage();
            this.m_labelZoom = new System.Windows.Forms.Label();
            this.m_comboZoom = new System.Windows.Forms.ComboBox();
            this.m_tabControl = new System.Windows.Forms.TabControl();
            this.m_tabNotes.SuspendLayout();
            this.m_groupNotes.SuspendLayout();
            this.m_tabGeneral.SuspendLayout();
            this.m_tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_btnOK
            // 
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(104, 272);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 20;
            this.m_btnOK.Text = "OK";
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(192, 272);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 21;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_btnHelp
            // 
            this.m_btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("m_btnHelp.Image")));
            this.m_btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_btnHelp.Location = new System.Drawing.Point(280, 272);
            this.m_btnHelp.Name = "m_btnHelp";
            this.m_btnHelp.Size = new System.Drawing.Size(75, 23);
            this.m_btnHelp.TabIndex = 26;
            this.m_btnHelp.Text = "Help";
            this.m_btnHelp.Click += new System.EventHandler(this.cmdHelp);
            // 
            // m_tabNotes
            // 
            this.m_tabNotes.Controls.Add(this.m_groupNotes);
            this.m_tabNotes.Controls.Add(this.m_checkHintsFromFront);
            this.m_tabNotes.Location = new System.Drawing.Point(4, 22);
            this.m_tabNotes.Name = "m_tabNotes";
            this.m_tabNotes.Size = new System.Drawing.Size(432, 230);
            this.m_tabNotes.TabIndex = 1;
            this.m_tabNotes.Text = "Notes";
            this.m_tabNotes.UseVisualStyleBackColor = true;
            // 
            // m_groupNotes
            // 
            this.m_groupNotes.Controls.Add(this.m_lblNoteBackgroundColor);
            this.m_groupNotes.Controls.Add(this.m_checkListNotes);
            this.m_groupNotes.Controls.Add(this.m_clrNoteBkg);
            this.m_groupNotes.Location = new System.Drawing.Point(16, 17);
            this.m_groupNotes.Name = "m_groupNotes";
            this.m_groupNotes.Size = new System.Drawing.Size(400, 177);
            this.m_groupNotes.TabIndex = 27;
            this.m_groupNotes.TabStop = false;
            this.m_groupNotes.Text = "Show Individual Notes:";
            // 
            // m_lblNoteBackgroundColor
            // 
            this.m_lblNoteBackgroundColor.Location = new System.Drawing.Point(250, 118);
            this.m_lblNoteBackgroundColor.Name = "m_lblNoteBackgroundColor";
            this.m_lblNoteBackgroundColor.Size = new System.Drawing.Size(136, 16);
            this.m_lblNoteBackgroundColor.TabIndex = 31;
            this.m_lblNoteBackgroundColor.Text = "Note Background Color:";
            // 
            // m_checkListNotes
            // 
            this.m_checkListNotes.Location = new System.Drawing.Point(8, 16);
            this.m_checkListNotes.Name = "m_checkListNotes";
            this.m_checkListNotes.Size = new System.Drawing.Size(231, 139);
            this.m_checkListNotes.TabIndex = 30;
            this.m_checkListNotes.SelectedIndexChanged += new System.EventHandler(this.cmd_OnNotesSelectedIndexChanged);
            // 
            // m_clrNoteBkg
            // 
            this.m_clrNoteBkg.ChosenColor = "Black";
            this.m_clrNoteBkg.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.m_clrNoteBkg.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_clrNoteBkg.InitialColor = "Black";
            this.m_clrNoteBkg.Items.AddRange(new object[] {
            System.Drawing.Color.AliceBlue,
            System.Drawing.Color.AntiqueWhite,
            System.Drawing.Color.Aqua,
            System.Drawing.Color.Aquamarine,
            System.Drawing.Color.Azure,
            System.Drawing.Color.Beige,
            System.Drawing.Color.Bisque,
            System.Drawing.Color.Black,
            System.Drawing.Color.BlanchedAlmond,
            System.Drawing.Color.Blue,
            System.Drawing.Color.BlueViolet,
            System.Drawing.Color.Brown,
            System.Drawing.Color.BurlyWood,
            System.Drawing.Color.CadetBlue,
            System.Drawing.Color.Chartreuse,
            System.Drawing.Color.Chocolate,
            System.Drawing.Color.Coral,
            System.Drawing.Color.CornflowerBlue,
            System.Drawing.Color.Cornsilk,
            System.Drawing.Color.Crimson,
            System.Drawing.Color.Cyan,
            System.Drawing.Color.DarkBlue,
            System.Drawing.Color.DarkCyan,
            System.Drawing.Color.DarkGoldenrod,
            System.Drawing.Color.DarkGray,
            System.Drawing.Color.DarkGreen,
            System.Drawing.Color.DarkKhaki,
            System.Drawing.Color.DarkMagenta,
            System.Drawing.Color.DarkOliveGreen,
            System.Drawing.Color.DarkOrange,
            System.Drawing.Color.DarkOrchid,
            System.Drawing.Color.DarkRed,
            System.Drawing.Color.DarkSalmon,
            System.Drawing.Color.DarkSeaGreen,
            System.Drawing.Color.DarkSlateBlue,
            System.Drawing.Color.DarkSlateGray,
            System.Drawing.Color.DarkTurquoise,
            System.Drawing.Color.DarkViolet,
            System.Drawing.Color.DeepPink,
            System.Drawing.Color.DeepSkyBlue,
            System.Drawing.Color.DimGray,
            System.Drawing.Color.DodgerBlue,
            System.Drawing.Color.Firebrick,
            System.Drawing.Color.FloralWhite,
            System.Drawing.Color.ForestGreen,
            System.Drawing.Color.Fuchsia,
            System.Drawing.Color.Gainsboro,
            System.Drawing.Color.GhostWhite,
            System.Drawing.Color.Gold,
            System.Drawing.Color.Goldenrod,
            System.Drawing.Color.Gray,
            System.Drawing.Color.Green,
            System.Drawing.Color.GreenYellow,
            System.Drawing.Color.Honeydew,
            System.Drawing.Color.HotPink,
            System.Drawing.Color.IndianRed,
            System.Drawing.Color.Indigo,
            System.Drawing.Color.Ivory,
            System.Drawing.Color.Khaki,
            System.Drawing.Color.Lavender,
            System.Drawing.Color.LavenderBlush,
            System.Drawing.Color.LawnGreen,
            System.Drawing.Color.LemonChiffon,
            System.Drawing.Color.LightBlue,
            System.Drawing.Color.LightCoral,
            System.Drawing.Color.LightCyan,
            System.Drawing.Color.LightGoldenrodYellow,
            System.Drawing.Color.LightGreen,
            System.Drawing.Color.LightGray,
            System.Drawing.Color.LightPink,
            System.Drawing.Color.LightSalmon,
            System.Drawing.Color.LightSeaGreen,
            System.Drawing.Color.LightSkyBlue,
            System.Drawing.Color.LightSlateGray,
            System.Drawing.Color.LightSteelBlue,
            System.Drawing.Color.LightYellow,
            System.Drawing.Color.Lime,
            System.Drawing.Color.LimeGreen,
            System.Drawing.Color.Linen,
            System.Drawing.Color.Magenta,
            System.Drawing.Color.Maroon,
            System.Drawing.Color.MediumAquamarine,
            System.Drawing.Color.MediumBlue,
            System.Drawing.Color.MediumOrchid,
            System.Drawing.Color.MediumPurple,
            System.Drawing.Color.MediumSeaGreen,
            System.Drawing.Color.MediumSlateBlue,
            System.Drawing.Color.MediumSpringGreen,
            System.Drawing.Color.MediumTurquoise,
            System.Drawing.Color.MediumVioletRed,
            System.Drawing.Color.MidnightBlue,
            System.Drawing.Color.MintCream,
            System.Drawing.Color.MistyRose,
            System.Drawing.Color.Moccasin,
            System.Drawing.Color.NavajoWhite,
            System.Drawing.Color.Navy,
            System.Drawing.Color.OldLace,
            System.Drawing.Color.Olive,
            System.Drawing.Color.OliveDrab,
            System.Drawing.Color.Orange,
            System.Drawing.Color.OrangeRed,
            System.Drawing.Color.Orchid,
            System.Drawing.Color.PaleGoldenrod,
            System.Drawing.Color.PaleGreen,
            System.Drawing.Color.PaleTurquoise,
            System.Drawing.Color.PaleVioletRed,
            System.Drawing.Color.PapayaWhip,
            System.Drawing.Color.PeachPuff,
            System.Drawing.Color.Peru,
            System.Drawing.Color.Pink,
            System.Drawing.Color.Plum,
            System.Drawing.Color.PowderBlue,
            System.Drawing.Color.Purple,
            System.Drawing.Color.Red,
            System.Drawing.Color.RosyBrown,
            System.Drawing.Color.RoyalBlue,
            System.Drawing.Color.SaddleBrown,
            System.Drawing.Color.Salmon,
            System.Drawing.Color.SandyBrown,
            System.Drawing.Color.SeaGreen,
            System.Drawing.Color.SeaShell,
            System.Drawing.Color.Sienna,
            System.Drawing.Color.Silver,
            System.Drawing.Color.SkyBlue,
            System.Drawing.Color.SlateBlue,
            System.Drawing.Color.SlateGray,
            System.Drawing.Color.Snow,
            System.Drawing.Color.SpringGreen,
            System.Drawing.Color.SteelBlue,
            System.Drawing.Color.Tan,
            System.Drawing.Color.Teal,
            System.Drawing.Color.Thistle,
            System.Drawing.Color.Tomato,
            System.Drawing.Color.Turquoise,
            System.Drawing.Color.Violet,
            System.Drawing.Color.Wheat,
            System.Drawing.Color.White,
            System.Drawing.Color.WhiteSmoke,
            System.Drawing.Color.Yellow,
            System.Drawing.Color.YellowGreen});
            this.m_clrNoteBkg.Location = new System.Drawing.Point(250, 134);
            this.m_clrNoteBkg.MaxDropDownItems = 16;
            this.m_clrNoteBkg.Name = "m_clrNoteBkg";
            this.m_clrNoteBkg.Size = new System.Drawing.Size(144, 21);
            this.m_clrNoteBkg.TabIndex = 30;
            this.m_clrNoteBkg.SelectionChangeCommitted += new System.EventHandler(this.cmd_NoteBkgColorChanged);
            // 
            // m_checkHintsFromFront
            // 
            this.m_checkHintsFromFront.Location = new System.Drawing.Point(16, 200);
            this.m_checkHintsFromFront.Name = "m_checkHintsFromFront";
            this.m_checkHintsFromFront.Size = new System.Drawing.Size(192, 16);
            this.m_checkHintsFromFront.TabIndex = 27;
            this.m_checkHintsFromFront.Text = "Hints from the Front";
            // 
            // m_tabGeneral
            // 
            this.m_tabGeneral.Controls.Add(this.m_labelZoom);
            this.m_tabGeneral.Controls.Add(this.m_comboZoom);
            this.m_tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.m_tabGeneral.Name = "m_tabGeneral";
            this.m_tabGeneral.Size = new System.Drawing.Size(432, 230);
            this.m_tabGeneral.TabIndex = 2;
            this.m_tabGeneral.Text = "General";
            this.m_tabGeneral.UseVisualStyleBackColor = true;
            // 
            // m_labelZoom
            // 
            this.m_labelZoom.Location = new System.Drawing.Point(16, 16);
            this.m_labelZoom.Name = "m_labelZoom";
            this.m_labelZoom.Size = new System.Drawing.Size(176, 23);
            this.m_labelZoom.TabIndex = 5;
            this.m_labelZoom.Text = "Zoom to (magnification factor):";
            this.m_labelZoom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_comboZoom
            // 
            this.m_comboZoom.Items.AddRange(new object[] {
            "250 %",
            "225 %",
            "200 %",
            "175 %",
            "150 %",
            "140 %",
            "130 %",
            "120 %",
            "110 %",
            "100 %",
            "90 %",
            "80 %",
            "70 %",
            "60 %"});
            this.m_comboZoom.Location = new System.Drawing.Point(16, 40);
            this.m_comboZoom.MaxDropDownItems = 15;
            this.m_comboZoom.Name = "m_comboZoom";
            this.m_comboZoom.Size = new System.Drawing.Size(176, 21);
            this.m_comboZoom.TabIndex = 4;
            // 
            // m_tabControl
            // 
            this.m_tabControl.Controls.Add(this.m_tabGeneral);
            this.m_tabControl.Controls.Add(this.m_tabNotes);
            this.m_tabControl.Location = new System.Drawing.Point(8, 8);
            this.m_tabControl.Name = "m_tabControl";
            this.m_tabControl.SelectedIndex = 0;
            this.m_tabControl.Size = new System.Drawing.Size(440, 256);
            this.m_tabControl.TabIndex = 25;
            // 
            // JobConfigureDlg
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(458, 301);
            this.Controls.Add(this.m_btnHelp);
            this.Controls.Add(this.m_tabControl);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "JobConfigureDlg";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.Click += new System.EventHandler(this.cmdHelp);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.onClosing);
            this.Load += new System.EventHandler(this.onLoad);
            this.m_tabNotes.ResumeLayout(false);
            this.m_groupNotes.ResumeLayout(false);
            this.m_tabGeneral.ResumeLayout(false);
            this.m_tabControl.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion
		#region Cmd: void onLoad(...)
		private void onLoad(object sender, System.EventArgs e)
		{
			// Localization of the UI
			Text = DlgOptionsRes.Title;
			m_tabGeneral.Text                = DlgOptionsRes.TabGeneral;
			m_labelZoom.Text                 = DlgOptionsRes.ZoomTo;

			m_tabNotes.Text                  = DlgOptionsRes.TabNotes;
			m_lblNoteBackgroundColor.Text    = DlgOptionsRes.NoteBackgroundColor;
			_Notes_OnLoad();
			m_checkHintsFromFront.Text       = DlgOptionsRes.HintFromFrontNote;

			m_btnOK.Text                     = DlgOptionsRes.OK;
			m_btnCancel.Text                 = DlgOptionsRes.Cancel;
			m_btnHelp.Text                   = DlgOptionsRes.Help;

			// General Setings
			string s = Options.ZoomPercent.ToString() + " %";
			m_comboZoom.Text = s;

			// Notes Settings
			m_checkHintsFromFront.Checked       = DNote.ShowHintsFromFront;
		}
		#endregion
		#region Cmd: void onClosing(...)
		private void onClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// We're only interested in further processing if the user has hit the OK
			// button, signallying he is done and wishes to save his results.
			if (DialogResult != DialogResult.OK)
				return;

			// Zoom
			string s = "";
			foreach(char c in m_comboZoom.Text)
			{
				if (c == ' ')
					break;
				s += c;
			}
			Options.ZoomPercent = Convert.ToInt16(s);

			// Save the UI settings
			Options.SaveToRegistry();

			// Notes
			_Notes_OnClosing();
			DNote.ShowHintsFromFront  = m_checkHintsFromFront.Checked;
		}
		#endregion

		#region Cmd: cmdHelp(...)
		private void cmdHelp(object sender, System.EventArgs e)
		{
			HelpSystem.Show_DlgOptions();
		}
		#endregion


	}
	#endregion

	#region CLASS: Options
	public class Options
	{
		// Attributes ------------------------------------------------------------------------
		#region Attr{g/s}: int ZoomPercent - e.g., 100 = zoom 100%; 200 = twice as big.
		private int m_ZoomPercent = 100;
		public int ZoomPercent
		{
			get
			{
				return m_ZoomPercent;
			}
			set
			{
				m_ZoomPercent = value;
			}
		}
		#endregion
		#region Attr{g}: float ZoomFactor - returns zoom as, e.g., 1.0, 1.2, etc.
		public float ZoomFactor
		{
			get
			{
				return ((float)ZoomPercent / 100.0F);
			}
		}
		#endregion
		#region Attr{g}:  string RegistrySubKey - returns "Options"
		static public string RegistrySubKey
		{
			get
			{
				return "Options";
			}
		}
		#endregion
        #region SAttr{g/s}: string PictureSearchPath
        static public string PictureSearchPath
        {
            get
            {
                return JW_Registry.GetValue(RegistrySubKey, c_keyPictureSearchPath, "");
            }
            set
            {
                JW_Registry.SetValue(RegistrySubKey, c_keyPictureSearchPath, value);
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
		#region Constructor() - initializes values to registry (if possible)
		public Options()
		{
			GetFromRegistry();
		}
		#endregion

		// Persistence -----------------------------------------------------------------------
		const string c_keyZoom   = "Zoom";
		const string c_keyLocale = "Locale";
		const string c_keyLanguage = "Language";
        const string c_keyPictureSearchPath = "PictureSearchPath";
		#region Method: void GetFromRegistry() - Retrieves user options from the registry
		public void GetFromRegistry()
		{
			ZoomPercent = JW_Registry.GetValue(RegistrySubKey, c_keyZoom, 100);

			InitLanguageResources();
		}
		#endregion
		#region Method: static void InitLanguageResources()
		static public void InitLanguageResources()
		{
			LanguageResources.Initialize(RegistrySubKey, c_keyLanguage);
		}
		#endregion
		#region Method: void SaveToRegistry() - saves user options to the registry
		public void SaveToRegistry()
		{
			JW_Registry.SetValue(RegistrySubKey, c_keyZoom,   ZoomPercent);
			JW_Registry.SetValue(RegistrySubKey, c_keyLanguage, (int)LanguageResources.Language);
		}
		#endregion
	}
	#endregion

	#region CLASS: BackupSystem
	public class BackupSystem
	{
		const string c_sRegKey  = "Backup";
		const string c_sFolder  = "Folder";
		const string c_sEnabled = "Enabled";
		const int c_cWeeksPast = 2;
		const int c_cMonthsPast = 2;

		// Attrs -----------------------------------------------------------------------------
		#region Attr{g}: string SourcePathName
		string SourcePathName
		{
			get
			{
				return m_sSourcePathName;
			}
		}
		string m_sSourcePathName = "";
		#endregion
		#region Attr{g/s}: static bool Enabled
		static public bool Enabled
		{
			get
			{
				return JW_Registry.GetValue(c_sRegKey, c_sEnabled, true);
			}
			set
			{
				JW_Registry.SetValue(c_sRegKey, c_sEnabled, value);
			}
		}
		#endregion

		// Derived Attrs ---------------------------------------------------------------------
		#region Attr{g}: string BackupFolder
		string BackupFolder
		{
			get
			{
				// Retrieve this from the registry
				string sFolder = RegistryBackupFolder;

				// If we got nothing, we need to prompt the user; if the user chickens out
				// (via the Cancel button), then we create a folder under his My Documents
				// folder.
				if (0 == sFolder.Length)
				{
					sFolder = BrowseForFolder("");
					if (0 == sFolder.Length)
						sFolder = FallbackBackupFolder;
					RegistryBackupFolder = sFolder;
				}

				return sFolder;
			}
		}
		#endregion
		#region Attr{g}: BackupPathName
		string BackupPathName
		{
			get
			{
				// Get the base name
				string sName = Path.GetFileNameWithoutExtension(SourcePathName);

				// Append the extension to it (Note: GetExtension returns the '.')
				sName += Path.GetExtension(SourcePathName);

				return BackupFolder + Path.DirectorySeparatorChar + sName;
			}
		}
		#endregion
		#region Attr{g}: FallbackBackupFolder - if all else fails, hopefully we can backup here
		string FallbackBackupFolder
		{
			get
			{
				string sFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				sFolder += (Path.DirectorySeparatorChar + "OurWordBackups");
				return sFolder;
			}
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor(sSourcePathName)
		public BackupSystem(string sSourcePathName)
		{
			m_sSourcePathName = sSourcePathName;
		}
		#endregion

		// Methods ---------------------------------------------------------------------------
		#region Method: bool _EnsureValidDestinationDirectory() - get a place to backup to
		private bool _EnsureValidDestinationDirectory()
		{
			// Does the destination device & path exist?
			int cAttempts = 3;
			while (!Directory.Exists(BackupFolder))
			{
				// Attempt to create it
				try
				{
					Directory.CreateDirectory(BackupFolder);
				}

				// If we fail, the device is likely not present. Ask the user to do 
				// something about it; a No answer means he is aborting.
				catch (Exception)
				{
					// If we have already tried twice, then we quietly change the
					// backup folder to a valid place on the data directory. This keeps
					// the backup happening, without further troubling the user.
					--cAttempts;
					if (cAttempts == 0)
					{
						RegistryBackupFolder = FallbackBackupFolder;
					}

					// If even the fallback fails, then we have no choice but to just
					// forget it; and completely turn off the feature
					else if (cAttempts < 0)
					{
						Enabled = false;
						return false;
					}

					// Display the message complaining about the missing flash card, and
					// offering to try again
					else if (! Messages.NeedFloppyForBackup( BackupPathName ) )
						return false;
				}
			}

			// Make sure we have enough disk space
			try
			{
				string sDrive = Directory.GetDirectoryRoot(BackupPathName);
				long lFreeDiskSpace = JW_Util.GetFreeDiskSpace(sDrive);
				FileInfo fi = new FileInfo(SourcePathName);
				long lNeededSpace = fi.Length;
				if (lNeededSpace + 1000 > lFreeDiskSpace)
				{
					Messages.InsufficentSpaceForBackup(sDrive);
					return false;
				}
			}
			catch (Exception)
			{
			}

			return true;
		}
		#endregion
		#region Method: void MakeBackup()
		public void MakeBackup()
		{
			// Don't execute if the feature has been turned off
			if (!Enabled)
				return;

			// Make sure we have a place to write the backup
			if (!_EnsureValidDestinationDirectory())
				return;

			// Copy the file to the backup filename
			try
			{
				File.Copy( SourcePathName, BackupPathName, true);
			}
			catch (UnauthorizedAccessException)
			{
                Messages.NoPermissionToWriteFile(BackupPathName);
			}
			catch (Exception)
			{
                Messages.UnableToSaveFile(BackupPathName);
			}

			// Delete older files if appropriate
			CleanUpOldFiles(BackupFolder);
		}
		#endregion

		// Helper methods for CleanUpOldFiles ------------------------------------------------
		#region Method: DateTime DateFromFileName(string sPath)
		DateTime DateFromFileName(string sPath)
			// We use the Create date, rather than fool with parsing the file's name.
		{
			return File.GetCreationTime(sPath);
		}
		#endregion
		#region Method: void CleanUpOldFiles(string sBackupFolder)
		public void CleanUpOldFiles(string sBackupFolder)
		{
			// Get the file name (less the date)
			string sPathPartial = sBackupFolder + Path.DirectorySeparatorChar + 
				Path.GetFileNameWithoutExtension(SourcePathName);

			// Get the list of files in the directory
			string[] sAllFiles = Directory.GetFiles( sBackupFolder );

			// Narrow it down to just those files that match ours
			ArrayList FileList = new ArrayList();
			foreach(string s in sAllFiles)
			{
				if (s.StartsWith( sPathPartial ))
					FileList.Add(s); 
			}

			// Get the date that ends this week
			DateTime dateThisWeekEnds = DateTime.Today;
			while (dateThisWeekEnds.DayOfWeek != DayOfWeek.Saturday)
				dateThisWeekEnds = dateThisWeekEnds.AddDays(1);

			// Get the cutoff before which we go to weekly updates
			DateTime dateDaily = dateThisWeekEnds.AddDays( -((c_cWeeksPast+1) * 7) );

			// Get the cutoff before which we go to monthly updates
			int nYear = dateThisWeekEnds.Year;
			int nMonth = dateThisWeekEnds.Month - ( c_cMonthsPast + 1);
			if (nMonth < 1)
			{
				nMonth += 12;
				nYear--;
			}
			int nDay = DateTime.DaysInMonth(nYear, nMonth);
			DateTime dateMonthly = new DateTime(nYear, nMonth, nDay);

			// Loop through the filenames
			for(int i = 0; i < FileList.Count; i++)
			{
				// Get the pathname
				string sName = FileList[i] as String;
				if (!File.Exists(sName))
					continue;

				// Get the file's date
				DateTime date = DateFromFileName(sName);

				// If it is after the Daily cutoff, then we're done.
				if (DateTime.Compare( dateDaily, date ) < 0 )
					continue;

				// If it is after the Weekly cutoff (therefore a Weekly File)
				if (DateTime.Compare( dateMonthly, date) < 0 )
				{
					DeleteOnWeeklyBasis(sName, ref FileList);
					continue;
				}

				// If we are here, it is a monthly file
				DeleteOnMonthlyBasis(sName, ref FileList);

			}

		}
		#endregion
		#region Method: void DeleteOnWeeklyBasis(string sName, ref ArrayList list)
		private void DeleteOnWeeklyBasis(string sName, ref ArrayList list)
		{
			DateTime date = DateFromFileName(sName);

			DateTime dateBegin = date;
			while (dateBegin.DayOfWeek != DayOfWeek.Sunday)
				dateBegin = dateBegin.AddDays(-1);

			DateTime dateEnd = date;
			while (dateEnd.DayOfWeek != DayOfWeek.Saturday)
				dateEnd = dateEnd.AddDays(-1);

			DeleteForPeriod(sName, ref list, dateBegin, dateEnd);
		}
		#endregion
		#region Method: void DeleteOnMonthlyBasis(string sName, ref ArrayList list)
		private void DeleteOnMonthlyBasis(string sName, ref ArrayList list)
		{
			DateTime date = DateFromFileName(sName);

			DateTime dateEnd = date;
			while ( (dateEnd.AddDays(1)).Month == date.Month)
				dateEnd = dateEnd.AddDays(1);

			DateTime dateBegin = new DateTime( date.Year, date.Month, 1);

			DeleteForPeriod(sName, ref list, dateBegin, dateEnd);
		}
		#endregion
		#region Method: void DeleteForPeriod(...)
		private void DeleteForPeriod(string sName, ref ArrayList list, 
			DateTime dateBegin, DateTime dateEnd)
		{
			DateTime date = DateFromFileName(sName);

			string sLatest = sName;
			for(int i = 0; i < list.Count; i++)
			{
				// Empty strings represent files we've already deleted
				string s = list[i] as string;
				if (!File.Exists(s))
					continue;

				// Get the file's date
				DateTime d = DateFromFileName(s);

				// Is the file (s) within this week?
				if ( DateTime.Compare( dateBegin, d) > 0)
					continue;
				if (DateTime.Compare( dateEnd, d) < 0)
					continue;

				// If the date is earlier, then we delete the file
				if (DateTime.Compare( d, date) < 0)
				{
					File.Delete(s);
					list[i] = "";
				}

					// If the date is later, then we delete the earlier file
				else if (DateTime.Compare( d, date) > 0)
				{
					if (File.Exists(sLatest))
						File.Delete(sLatest);
					sLatest = s;
				}
			}
		}
		#endregion

		// Registry --------------------------------------------------------------------------
		#region Attr{g/s}: static string RegistryBackupFolder
		static public string RegistryBackupFolder
		{
			get
			{
				return JW_Registry.GetValue(c_sRegKey, c_sFolder, "");
			}
			set
			{
				JW_Registry.SetValue(c_sRegKey, c_sFolder, value);
			}
		}
		#endregion
		#region Method: static string BrowseForFolder(string sOriginalFolder)
		static public string BrowseForFolder(string sOriginalFolder)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();

            dlg.Description = G.GetLoc_Files("BrowseForBackupFolderDescr", 
                "Select the folder where you wish to place your backup files. If " +
                "possible, this should not be your hard drive. A flash card is ideal; " +
                "or a floppy drive can also be used.");  

			dlg.RootFolder  = Environment.SpecialFolder.MyComputer;
			if (DialogResult.OK == dlg.ShowDialog())
				return dlg.SelectedPath;
			return sOriginalFolder;
		}
		#endregion
	}
	#endregion

	#region TEST
	public class Test_BackupSystem : Test
	{
		#region Constructor()
		public Test_BackupSystem()
			: base("BackupSystem")
		{
			AddTest( new IndividualTest( CleanUpFileNames ), "CleanUpFileNames" );
		}
		#endregion

		#region void CleanUpFileNames()
		public void CleanUpFileNames()
		{
			int cFilesToCreate = 400;

			// Create an empty working directory
			string sTestPath = Path.GetDirectoryName(Application.ExecutablePath) + 
				Path.DirectorySeparatorChar + "BackupTest";
			if (Directory.Exists(sTestPath))
				Directory.Delete(sTestPath, true);
			Directory.CreateDirectory(sTestPath);

			// Give the system time to get the directory stuff done
			Thread.Sleep(1000);

			// Base name & extension
			string sBaseName = "TestFile";
			string sExtension = ".db";

			// Get today's date; we'll do it based on "today"
			DateTime today = DateTime.Today;
			int nYear  = today.Year;
			int nMonth = today.Month;
			int nDay   = today.Day;

			// Create a whole bunch of files
			DateTime dt = today;
			for(int i = 0; i < cFilesToCreate; i++)
			{
				string sDate = dt.ToString("yyyy-MM-dd");
				string sPath = sTestPath + Path.DirectorySeparatorChar + sBaseName + " " + sDate + sExtension;

				FileStream f = File.Create(sPath);
				f.Flush();
				f.Close();
				File.SetCreationTime(sPath, dt);

				--nDay;
				if (nDay < 1)
				{
					--nMonth;
					if (nMonth < 1)
					{
						nMonth += 12;
						--nYear;
					}
					nDay = DateTime.DaysInMonth(nYear, nMonth);
				}
				dt = new DateTime(nYear, nMonth, nDay);
			}

			// We should have a lot of files
			AreSame( cFilesToCreate, Directory.GetFiles(sTestPath).Length );

			// Call the cleanup routine
			string sSourcePath = sTestPath + Path.DirectorySeparatorChar + sBaseName + sExtension;
			BackupSystem backup = new BackupSystem(sSourcePath);
			backup.CleanUpOldFiles(sTestPath);

			// The number of files we now have depends on the day of week, day of 
			// month, etc. There should be 14-to-21 daily files, 6-to-11 weekly
			// files, and then 1 per month before that. Taking the minimum,
			// and assuming an initial count of 400, we expect there to be
			// at least 30 files. 
			//   Then assuming the maximums, and an initial count of 400, we would
			// expect there to be fewer than 50 files.
			int cFiles = Directory.GetFiles(sTestPath).Length;
			IsTrue(cFiles > 30);
			IsTrue(cFiles < 50);

			// Clean up my disk!
			Directory.Delete(sTestPath, true);
		}
		#endregion
	}
	#endregion
}
