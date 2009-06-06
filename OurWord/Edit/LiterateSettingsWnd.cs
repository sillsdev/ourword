/**********************************************************************************************
 * Project: Our Word!
 * File:    LiterateSettingsWnd.cs
 * Author:  John Wimbish
 * Created: 13 Feb 2009
 * Purpose: User Control that houses Literate Settings or Property Grid, allowing user
 *          to switch; and storing that state in the registry.
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Resources;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

using JWTools;
using JWdb;
using JWdb.DataModel;

using OurWord.Dialogs;
#endregion

// TODO: How do we do error checking with this generic Settings mechanism?

namespace OurWord.Edit
{
	public partial class LiterateSettingsWnd : UserControl
	{
		// Whether or not to show the documentation ------------------------------------------
		#region VAttr{g/s}: bool ShowDocumentation - T if doc is desired
		bool ShowDocumentation
		{
			get
			{
				return m_cbShowDocumentation.Checked;
			}
			set
			{
				m_cbShowDocumentation.Checked = value;
			}
		}
		#endregion
		#region Cmd: cmdShowDocumentationChanged
		private void cmdShowDocumentationChanged(object sender, EventArgs e)
		{
            JW_Registry.SetValue(c_sRegSubKey, Name, ShowDocumentation);
			LoadContents();
		}
		#endregion
        #region Attr{g/s}: bool DontAllowPropertyGrid
        public bool DontAllowPropertyGrid
        {
            get
            {
                return m_bDontAllowPropertyGrid;
            }
            set
            {
                m_bDontAllowPropertyGrid = value;
            }
        }
        bool m_bDontAllowPropertyGrid = false;
        #endregion

        // Documentation OWWindow ------------------------------------------------------------
		#region Attr{g}: OWWindow Verbose
		public OWWindow Verbose
		{
			get
			{
				return m_wndVerbose;
			}
		}
		OWWindow m_wndVerbose;
		#endregion
		#region Method: void ClearVerbose()
		void ClearVerbose()
		{
			if (null == Verbose)
				return;

			Verbose.Dispose();
			m_panelSettingsContainer.Controls.Remove(Verbose);
			m_wndVerbose = null;
		}
		#endregion
		#region Method: CreateVerbose()
		void CreateVerbose()
		{
			m_wndVerbose = new OWWindow("LS-" + Name, 1);
			m_panelSettingsContainer.Controls.Add(Verbose);
			Verbose.Dock = DockStyle.Fill;
			Verbose.Visible = true;

			// Populate the window
			Verbose.Contents.Clear();
			foreach (Setting setting in Settings)
				setting.BuildVerbose();
			Verbose.LoadData();
		}
		#endregion

		// Property Grid ---------------------------------------------------------------------
		#region Cmd: bag_SetValue
		void bag_SetValue(object sender, PropertySpecEventArgs e)
		{
			foreach (Setting setting in Settings)
				setting.BagSetValue(e);
		}
		#endregion
		#region Cmd: bag_GetValue
		void bag_GetValue(object sender, PropertySpecEventArgs e)
		{
			foreach (Setting setting in Settings)
				setting.BagGetValue(e);
		}
		#endregion
		#region Attr{g}: PropertyBag TerseBag
		public PropertyBag TerseBag
		{
			get
			{
				return m_bagTerse;
			}
		}
		PropertyBag m_bagTerse;
		#endregion
		#region Attr{g}: PropertyGrid TerseGrid
		PropertyGrid TerseGrid
		{
			get
			{
				return m_PropGridTerse;
			}
		}
		PropertyGrid m_PropGridTerse;
		#endregion
		#region Method: void ClearTerse()
		void ClearTerse()
		{
			if (null == TerseGrid)
				return;

            m_bagTerse = null;
            TerseGrid.SelectedObject = null;

			m_panelSettingsContainer.Controls.Remove(TerseGrid);
			TerseGrid.Dispose();
			m_PropGridTerse = null;
		}
		#endregion
		#region Method: CreateTerse()
		void CreateTerse()
		{
			// Create the property grid
			m_PropGridTerse = new PropertyGrid();
			m_panelSettingsContainer.Controls.Add(TerseGrid);
			TerseGrid.Dock = DockStyle.Fill;
			TerseGrid.ToolbarVisible = false;
			TerseGrid.Visible = true;
			TerseGrid.PropertySort = PropertySort.NoSort;

			// Create and set up its corresponding property bag
			m_bagTerse = new PropertyBag();
			TerseBag.GetValue += new PropertySpecEventHandler(bag_GetValue);
			TerseBag.SetValue += new PropertySpecEventHandler(bag_SetValue);

			// Populate the grid (we have to go through them backwards)
			foreach (Setting setting in Settings)
			{
				if (string.IsNullOrEmpty(setting.Group))
					TerseGrid.PropertySort = PropertySort.Categorized;

				setting.BuildTerse();
			}

			// Setting the SelectedObject must happen after the settings
			// are all in, in order for the PropertyGrid to show them.
			LocDB.Localize(this, TerseBag);
			TerseGrid.SelectedObject = TerseBag;
		}
		#endregion

		// Content Building ------------------------------------------------------------------
		#region Attr{g}: List<Setting> Settings
		List<Setting> Settings
		{
			get
			{
				Debug.Assert(null != m_vSettings);
				return m_vSettings;
			}
		}
		List<Setting> m_vSettings = new List<Setting>();
		#endregion
		#region Method: Setting GetSettingFor(string sID)
		Setting GetSettingFor(string sID)
		{
			foreach (Setting setting in Settings)
			{
				if (setting.ID == sID)
					return setting;
			}

			return null;
		}
		#endregion
		#region Method: void AddSetting(Setting setting)
		public Setting AddSetting(Setting setting)
		{
			Debug.Assert(null == GetSettingFor(setting.ID));
			Settings.Add(setting);
			return setting;
		}
		#endregion
		#region Method: YesNoSetting AddYesNo(sID, sLabel, sDescription, bInitialValue)
		public YesNoSetting AddYesNo(string sID, string sLabel, string sDescription, bool bInitialValue)
		{
			YesNoSetting setting = new YesNoSetting(
				this, sID, sLabel, sDescription, null, bInitialValue);
			return AddSetting(setting) as YesNoSetting;
		}
		#endregion
        #region Method: StringChoiceSetting AddAtringChoice(...)
        public StringChoiceSetting AddAtringChoice(string sID, string sLabel, 
            string sDescription, string sInitialValue, string[] vPossiblities)
        {
            StringChoiceSetting setting = new StringChoiceSetting(
                this, sID, sLabel, sDescription, null, sInitialValue, vPossiblities);

            return AddSetting(setting) as StringChoiceSetting;
        }
        #endregion
        #region Method: EditTextSetting AddEditText(sID, sLabel, sDescription, sInitialValue)
        public EditTextSetting AddEditText(string sID, string sLabel, string sDescription, string sInitialValue)
		{
			EditTextSetting setting = new EditTextSetting(
				this, sID, sLabel, sDescription, null, sInitialValue);
			return AddSetting(setting) as EditTextSetting;
		}
		#endregion
		#region Method: IntSetting AddInt(sID, sLabel, sDescription, nInitialValue, nMinValue)
		public IntSetting AddInt(string sID, string sLabel, string sDescription, 
			int nInitialValue, int nMinValue)
		{
			IntSetting setting = new IntSetting(
				this, sID, sLabel, sDescription, null, 
				nInitialValue, nMinValue);
			return AddSetting(setting) as IntSetting;
		}
		#endregion
		#region Method: Information AddInformation(sID, JParagraphStyle, sParagraphText)
		public Information AddInformation(string sID, JParagraphStyle pstyle, string sParagraphText)
		{
			Information setting = new Information(this, sID, sParagraphText, pstyle);
			return AddSetting(setting) as Information;
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
        public const string c_sRegSubKey = "LiterateSettingsWnd";
		#region Constructor()
		public LiterateSettingsWnd()
		{
			Information.InitStyleSheet();

			InitializeComponent();
		}
		#endregion
        #region Constructor(bDontAllowPropertyGrid)
        public LiterateSettingsWnd(bool bDontAllowPropertyGrid)
            : this()
        {
            m_bDontAllowPropertyGrid = bDontAllowPropertyGrid;
        }
        #endregion
        #region Method: string GetLocalizedValue(sLocID, sContents)
        string GetLocalizedValue(string sLocID, string sContents)
		{
			string[] vGroupID = new string[] { "Configuration", sLocID };
			return LocDB.GetValue(vGroupID, sLocID, sContents, null, null);
		}
		#endregion
		#region Method: void LoadContents()
		void LoadContents()
		{
            // By default, we start by showing the verbose documentation, as
            // currently indicated in the combo box
            bool bShowVerbose = ShowDocumentation;

            // If the programmer has indicated to not allow the property grid, then
            // Verbose is on, regardless of registry, user decision, etc.
            if (DontAllowPropertyGrid)
            {
                m_cbShowDocumentation.Visible = false;
                m_panelSettingsContainer.Height = Height;
                bShowVerbose = true;
            }

			// Get rid of what we previously had; build the one we want
            if (bShowVerbose)
			{
				ClearTerse();
				CreateVerbose();
			}
			else
			{
				ClearVerbose();
				CreateTerse();
			}
		}
		#endregion

		// Command Handlers ------------------------------------------------------------------
		#region Cmd: cmdResize - Tell the OWWindow to recalculate itself
		private void cmdResize(object sender, EventArgs e)
		{
			if (ShowDocumentation && null != Verbose)
				Verbose.SetSize(Width, Height);
		}
		#endregion
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            // The initial value of the ShowDocumentation checkbox is stored in the registry;
            // the default value is to show it. We do this during Load, rather than during
            // construction, in order to give time for the Name to be changed from the
            // default, to whatever will be used in the given context.
            bool bShowVerbose = JW_Registry.GetValue(c_sRegSubKey, Name, true);

            // This is tricky. We have an event handler that fires when the Checked is
            // changed. Which would result in LoadContents being called twice, which
            // causes some flaky behavior. So we do it via this convulated manner.
            if (bShowVerbose == m_cbShowDocumentation.Checked)
                LoadContents();
            else
                m_cbShowDocumentation.Checked = bShowVerbose;
        }
        #endregion
    }

	#region CLASS: Setting
	public class Setting
	{
		#region Attr{g}: LiterateSettingsWnd LS
		LiterateSettingsWnd LS
		{
			get
			{
				Debug.Assert(null != m_LS);
				return m_LS;
			}
		}
		LiterateSettingsWnd m_LS;
		#endregion
		#region VAttr{g}: OWWindow Verbose
		protected OWWindow Verbose
		{
			get
			{
				return LS.Verbose;
			}
		}
		#endregion
		#region VAttr{g}: PropertyBag Terse
		protected PropertyBag Terse
		{
			get
			{
				return LS.TerseBag;
			}
		}
		#endregion

		#region Attr{g}: string ID
		public string ID
		{
			get
			{
				return m_sID;
			}
		}
		string m_sID;
		#endregion
		#region Attr{g}: string LabelText
		protected string LabelText
		{
			get
			{
				return m_sLabelText;
			}
		}
		string m_sLabelText;
		#endregion
		#region Attr{g}: string Description
		protected string Description
		{
			get
			{
				return m_sDescription;
			}
		}
		string m_sDescription;
		#endregion
		#region Attr{g/s}: string Group
		public string Group
		{
			get
			{
				return m_sGroup;
			}
            set
            {
                m_sGroup = value;
            }
		}
		string m_sGroup;
		#endregion

		#region Constructor(LiterateSettingsWnd, sID, sLabel, sDescription, sGroup)
		public Setting(LiterateSettingsWnd ls, string sID, string sLabelText,
			string sDescription, string sGroup)
		{
			m_LS = ls;
			m_sID = sID;
			m_sLabelText = sLabelText;
			m_sDescription = sDescription;
			m_sGroup = sGroup;
		}
		#endregion

		#region VirtMethod: void BuildVerbose()
		virtual public void BuildVerbose()
		{
		}
		#endregion
		#region VirtMethod: void BuildTerse()
		virtual public void BuildTerse()
		{
		}
		#endregion

		// Verbose Support -------------------------------------------------------------------
		#region Attr{g}: Color ToolStripBackColor
		Color ToolStripBackColor
		{
			get
			{
				return Color.LightYellow;
			}
		}
		#endregion
		#region SAttr{g/s}: int AlignmentColumn
		public static int AlignmentColumn
		{
			get
			{
				return s_nAlignmentColumn;
			}
			set
			{
				s_nAlignmentColumn = value;
			}
		}
		static int s_nAlignmentColumn = 100;
		#endregion
		#region SAttr{g/s}: int TextBoxWidth
		public static int TextBoxWidth
		{
			get
			{
				return s_nTextBoxWidth;
			}
			set
			{
				s_nTextBoxWidth = value;
			}
		}
		static int s_nTextBoxWidth = 100;
		#endregion
		#region Method: ToolStrip AddToolStrip()
		protected ToolStrip AddToolStrip()
		{
			// Create a container with rounded borders
			EColumn column = new EColumn();
			column.Border = new EContainer.RoundedBorder(column, 6);
			column.Border.FillColor = ToolStripBackColor;
			column.Border.Margin.Left = 15;
			column.Border.Padding.Left = 5;
			column.Border.Margin.Right = 15;
			column.Border.Padding.Right = 0;

			// Create the toolstrip and place it in our container
			EToolStrip ts = new EToolStrip(Verbose);
			ts.ToolStrip.Name = "strip_" + ID;
			ts.ToolStrip.BackColor = ToolStripBackColor;
			column.Append(ts);

			// Place all in the window contents
			Verbose.Contents.Append(column);
			return ts.ToolStrip;
		}
		#endregion

		// Terse Support ---------------------------------------------------------------------
		#region OMethod: void BagGetValue(PropertySpecEventArgs e)
		virtual public void BagSetValue(PropertySpecEventArgs e)
		{
		}
		#endregion
		#region OMethod: void BagSetValue(PropertySpecEventArgs e)
		virtual public void BagGetValue(PropertySpecEventArgs e)
		{
		}
		#endregion
	}
	#endregion
	#region CLASS: EditTextSetting : Setting
	public class EditTextSetting : Setting
	{
		// Scaffolding -----------------------------------------------------------------------
		#region Attr{g/s}: string Value
		public string Value
		{
			get
			{
				return m_sValue;
			}
			set
			{
				m_sValue = value;
			}
		}
		string m_sValue;
		#endregion
		#region Constructor(...)
		public EditTextSetting(LiterateSettingsWnd ls, string sID, string sLabel,
			string sDescription, string sGroup, string sInitialValue)
			: base(ls, sID, sLabel, sDescription, sGroup)
		{
			m_sValue = sInitialValue;
		}
		#endregion

		// Verbose ---------------------------------------------------------------------------
		#region Cmd: cmdVerboseControlChanged
		private void cmdVerboseControlChanged(object sender, EventArgs e)
		{
			ToolStripTextBox ctrl = sender as ToolStripTextBox;
			if (null == ctrl)
				return;
			Value = ctrl.Text;
		}
		#endregion
		#region OMethod: void BuildVerbose()
		public override void BuildVerbose()
		{
			ToolStrip strip = AddToolStrip();

			// Add the label
			ToolStripLabel label = new ToolStripLabel(LabelText);
			label.Width = AlignmentColumn;
			label.AutoSize = false;
			label.TextAlign = ContentAlignment.MiddleRight;
			strip.Items.Add(label);

			// Add the textbox
			ToolStripTextBox box = new ToolStripTextBox();
			box.Text = Value;
			box.AutoSize = false;
			box.Width = TextBoxWidth;
			box.TextChanged += new EventHandler(cmdVerboseControlChanged);
			strip.Items.Add(box);
		}
		#endregion

		// Terse -----------------------------------------------------------------------------
		#region OMethod: void BuildTerse()
		public override void BuildTerse()
		{
			PropertySpec ps = new PropertySpec(
				ID,
				LabelText,
				typeof(string),
				Group,
				Description,
				"",
				"",
				null);
			Terse.Properties.Add(ps);
		}
		#endregion
		#region OMethod: void BagGetValue(PropertySpecEventArgs e)
		public override void BagGetValue(PropertySpecEventArgs e)
		{
			if (e.Property.ID != ID)
				return;
			e.Value = Value;
		}
		#endregion
		#region OMethod: void BagSetValue(PropertySpecEventArgs e)
		public override void BagSetValue(PropertySpecEventArgs e)
		{
			if (e.Property.ID != ID)
				return;
			Value = (string)e.Value;
		}
		#endregion
	}
	#endregion
	#region CLASS: YesNoSetting : Setting
	public class YesNoSetting : Setting
	{
		// Scaffolding -----------------------------------------------------------------------
		#region Attr{g/s}: bool Value
		public bool Value
		{
			get
			{
				return m_bValue;
			}
			set
			{
				m_bValue = value;
			}
		}
		bool m_bValue;
		#endregion
		#region Constructor(...)
		public YesNoSetting(LiterateSettingsWnd ls, string sID, string sLabel, 
			string sDescription, string sGroup, bool bInitialValue)
			: base(ls, sID, sLabel, sDescription, sGroup)
		{
			m_bValue = bInitialValue;
		}
		#endregion

		// Verbose ---------------------------------------------------------------------------
		#region Cmd: cmdVerboseControlChanged
		private void cmdVerboseControlChanged(object sender, EventArgs e)
		{
			CheckBox ctrl = sender as CheckBox;
			if (null == ctrl)
				return;
			Value = ctrl.Checked;
		}
		#endregion
		#region OMethod: void BuildVerbose()
		public override void BuildVerbose()
		{
			ToolStrip strip = AddToolStrip();

			// Create and initialize the checkbox
			CheckBox cb = new CheckBox();
			cb.Text = LabelText; //  GetLocalizedValue(sLocID, sText);
			cb.Checked = Value;
			cb.CheckedChanged += new EventHandler(cmdVerboseControlChanged);

			// Add it to the toolstrip (we must use a host object)
			ToolStripControlHost host = new ToolStripControlHost(cb);
			strip.Items.Add(host);
		}
		#endregion

		// Terse -----------------------------------------------------------------------------
		#region OMethod: void BuildTerse()
		public override void BuildTerse()
		{
			YesNoPropertySpec ps = new YesNoPropertySpec(
				ID,
				LabelText,
				Group,
				Description,
				Value
				);
			Terse.Properties.Add(ps);
		}
		#endregion
		#region OMethod: void BagGetValue(PropertySpecEventArgs e)
		public override void BagGetValue(PropertySpecEventArgs e)
		{
			if (e.Property.ID != ID)
				return;
			YesNoPropertySpec yn = e.Property as YesNoPropertySpec;
			Debug.Assert(null != yn);
			e.Value = yn.GetBoolString(Value);
		}
		#endregion
		#region OMethod: void BagSetValue(PropertySpecEventArgs e)
		public override void BagSetValue(PropertySpecEventArgs e)
		{
			if (e.Property.ID != ID)
				return;
			YesNoPropertySpec yn = e.Property as YesNoPropertySpec;
			Debug.Assert(null != yn);
			Value = yn.IsTrue(e.Value);
		}
		#endregion
	}
	#endregion
    #region CLASS: IntSetting : Setting
    public class IntSetting : Setting
	{
		// Scaffolding -----------------------------------------------------------------------
		#region Attr{g/s}: int Value
		public int Value
		{
			get
			{
				return m_nValue;
			}
			set
			{
				m_nValue = value;
			}
		}
		int m_nValue;
		#endregion
		#region Attr{g}: int MinValue
		public int MinValue
		{
			get
			{
				return m_nMinValue;
			}
			set
			{
				m_nMinValue = value;
			}
		}
		int m_nMinValue;
		#endregion
		#region Constructor(...)
		public IntSetting(LiterateSettingsWnd ls, string sID, string sLabel,
			string sDescription, string sGroup, int nInitialValue, int nMinValue)
			: base(ls, sID, sLabel, sDescription, sGroup)
		{
			m_nValue = nInitialValue;
			m_nMinValue = nMinValue;
		}
		#endregion
		#region Method: int ConvertToInt(string s, int nDefault)
		int ConvertToInt(string s, int nDefault)
		{
			// Extract the numerical part of the string
			string sValue = "";
			foreach (char ch in s)
			{
				if (char.IsDigit(ch) || ch == '-')
					sValue += ch;
				else if (char.IsLetter(ch))
					break;
			}

			// Convert it to an int
			try
			{
				int n = Convert.ToInt32(sValue);
				return Math.Max(n, MinValue);
			}
			catch (Exception)
			{
			}

			return nDefault;
		}
		#endregion

		// Verbose ---------------------------------------------------------------------------
		#region Cmd: cmdVerboseControlChanged
		private void cmdVerboseControlChanged(object sender, EventArgs e)
		{
			ToolStripTextBox ctrl = sender as ToolStripTextBox;
			if (null == ctrl)
				return;

			Value = ConvertToInt(ctrl.Text, Value);
		}
		#endregion
		#region OMethod: void BuildVerbose()
		public override void BuildVerbose()
		{
			ToolStrip strip = AddToolStrip();

			// Add the label
			ToolStripLabel label = new ToolStripLabel(LabelText);
			label.Width = AlignmentColumn;
			label.AutoSize = false;
			label.TextAlign = ContentAlignment.MiddleRight;
			strip.Items.Add(label);

			// Add the textbox
			ToolStripTextBox box = new ToolStripTextBox();
			box.Text = Value.ToString();
			box.AutoSize = false;
			box.Width = TextBoxWidth;
			box.TextChanged += new EventHandler(cmdVerboseControlChanged);
			strip.Items.Add(box);
		}
		#endregion

		// Terse -----------------------------------------------------------------------------
		#region OMethod: void BuildTerse()
		public override void BuildTerse()
		{
			PropertySpec ps = new PropertySpec(
				ID,
				LabelText,
				typeof(string),
				Group,
				Description,
				"",
				"",
				null);
			Terse.Properties.Add(ps);
		}
		#endregion
		#region OMethod: void BagGetValue(PropertySpecEventArgs e)
		public override void BagGetValue(PropertySpecEventArgs e)
		{
			if (e.Property.ID != ID)
				return;
			e.Value = Value.ToString();
		}
		#endregion
		#region OMethod: void BagSetValue(PropertySpecEventArgs e)
		public override void BagSetValue(PropertySpecEventArgs e)
		{
			if (e.Property.ID != ID)
				return;
			Value = ConvertToInt((string)e.Value, Value);
		}
		#endregion
	}
	#endregion
	#region CLASS: Information : Setting
	public class Information : Setting
	{
		// Scaffolding -----------------------------------------------------------------------
		#region Attr{g}: JParagraphStyle PStyle
		JParagraphStyle PStyle
		{
			get
			{
				Debug.Assert(null != m_PStyle);
				return m_PStyle;
			}
		}
		JParagraphStyle m_PStyle;
		#endregion
		#region Constructor(...)
		public Information(LiterateSettingsWnd ls, string sID, string sParagraphText, JParagraphStyle PStyle)
			: base(ls, sID, sParagraphText, null, null)
		{
			m_PStyle = PStyle;
		}
		#endregion

		// StyleSheet ------------------------------------------------------------------------
		const string c_sNormal = "";
		const string c_sBold = "Bold";
		const string c_sItalic = "Italic";
		const string c_sBoldItalic = "BoldItalic";
		#region SAttr{g}: JStyleSheet StyleSheet
		static JStyleSheet StyleSheet
		{
			get
			{
				Debug.Assert(null != s_StyleSheet);
				return s_StyleSheet;
			}
		}
		static JStyleSheet s_StyleSheet;
		#endregion
		#region SAttr{g}: JWritingSystem WritingSystem
		static JWritingSystem WritingSystem
		{
			get
			{
				return s_WritingSystem;
			}
		}
		static JWritingSystem s_WritingSystem;
		#endregion
		#region SAttr{g}: JParagraphStyle PStyleHeading1
		static public JParagraphStyle PStyleHeading1
		{
			get
			{
				Debug.Assert(null != s_PStyleHeading1);
				return s_PStyleHeading1;
			}
		}
		static JParagraphStyle s_PStyleHeading1;
		#endregion
		#region SAttr{g}: JParagraphStyle PStyleNormal
		static public JParagraphStyle PStyleNormal
		{
			get
			{
				Debug.Assert(null != s_PStyleNormal);
				return s_PStyleNormal;
			}
		}
		static JParagraphStyle s_PStyleNormal;
		#endregion
		#region SAttr{g}: JParagraphStyle PStyleList1
		static public JParagraphStyle PStyleList1
		{
			get
			{
				Debug.Assert(null != s_PStyleList1);
				return s_PStyleList1;
			}
		}
		static JParagraphStyle s_PStyleList1;
		#endregion

        #region SAttr{g}: JParagraphStyle PStyleAttention
        static public JParagraphStyle PStyleAttention
        {
            get
            {
                Debug.Assert(null != s_PStyleAttention);
                return s_PStyleAttention;
            }
        }
        static JParagraphStyle s_PStyleAttention;
        #endregion

		#region SMethod: void InitStyleSheet()
		static public void InitStyleSheet()
		{
			if (null != s_StyleSheet)
				return;

			// Create the new style sheet
			s_StyleSheet = new JStyleSheet();

			// Create an 'English' writing system, using default values
			string sWritingSystem = "English";
			s_WritingSystem = StyleSheet.FindOrAddWritingSystem(sWritingSystem);

			// Create the paragraph styles we'll need

			// Header
			s_PStyleHeading1 = StyleSheet.AddParagraphStyle("s", "Heading 1");
			PStyleHeading1.SetFonts(false, 9, true);
			PStyleHeading1.IsJustified = true;
			PStyleHeading1.SpaceBefore = 6;
            PStyleHeading1.SpaceAfter = 3;

			// Normal
			s_PStyleNormal = StyleSheet.AddParagraphStyle("p", "Normal");
			PStyleNormal.SetFonts(false, 8, false);
			PStyleNormal.IsJustified = true;
			PStyleNormal.SpaceBefore = 3;
            PStyleNormal.SpaceAfter = 3;

            // Attention
            s_PStyleAttention = StyleSheet.AddParagraphStyle("a", "Attention");
            PStyleAttention.SetFonts(false, 8, true, false, false, false, Color.DarkRed);
            PStyleAttention.IsJustified = true;
            PStyleAttention.SpaceBefore = 3;
            PStyleAttention.SpaceAfter = 3;

			// List
			s_PStyleList1 = StyleSheet.AddParagraphStyle("L1", "List 1");
			PStyleList1.SetFonts(false, 8, false);
			PStyleList1.IsJustified = true;
			PStyleList1.LeftMargin = 0.2;
			PStyleList1.SpaceBefore = 0;
            PStyleList1.SpaceAfter = 0;
            PStyleList1.Bulleted = true;

			// Character Styles
			JCharacterStyle cs = StyleSheet.AddCharacterStyle(c_sBold, "Bold");
			cs.SetFonts(false, 8, true);

			cs = StyleSheet.AddCharacterStyle(c_sItalic, "Italic");
			cs.SetFonts(false, 8, false, true, false, false, Color.Black);

			cs = StyleSheet.AddCharacterStyle(c_sBoldItalic, "BoldItalic");
			cs.SetFonts(false, 8, true, true, false, false, Color.Black);

			// Zoom factor set to 1.0, no zoom
			StyleSheet.ZoomFactor = 1;
		}
		#endregion

		// Helper Methods --------------------------------------------------------------------
		#region Method: CreatePhrase(bIsBold, bIsItalic, sText)
		DPhrase CreatePhrase(bool bBold, bool bItalic, string sText)
		{
			if (string.IsNullOrEmpty(sText))
				return null;

			string sAbbrev =
				(bBold && bItalic) ? c_sBoldItalic :
				bBold ? c_sBold :
				bItalic ? c_sItalic :
				c_sNormal;

			// Because we're not using the G.StyleSheet, we cannot use the DPhrase
			// constructor we'd normally use to set these. So instead, we set them
			// one at a time through their attribute sets. Kludgy, and worth
			// repairing at some point.
			DPhrase p = new DPhrase();
			p.Text = sText;
			if (!string.IsNullOrEmpty(sAbbrev))
				p.CharacterStyleAbbrev = sAbbrev;

			return p;
		}
		#endregion
		#region Method: List<DPhrase> ParseIntoPhrases(sContents)
		List<DPhrase> ParseIntoPhrases(string sContents)
		{
			var vPhrases = new List<DPhrase>();
			bool bBold = false;
			bool bItalic = false;

			string sCollectText = "";

			foreach (char ch in sContents)
			{
				// Do we need to save a phrase?
				if (ch == '_' || ch == '*')
				{
					if (!string.IsNullOrEmpty(sCollectText))
						vPhrases.Add(CreatePhrase(bBold, bItalic, sCollectText));
					sCollectText = "";
				}

				if (ch == '*')
					bBold = !bBold;
				else if (ch == '_')
					bItalic = !bItalic;
				else
					sCollectText += ch;
			}

			// May have one remaining phrase
			if (!string.IsNullOrEmpty(sCollectText))
				vPhrases.Add(CreatePhrase(bBold, bItalic, sCollectText));

			return vPhrases;
		}
		#endregion

		// Verbose ---------------------------------------------------------------------------
		#region OMethod: void BuildVerbose()
		public override void BuildVerbose()
		{
			// Make sure its in our own stylesheet
			Debug.Assert(StyleSheet.ParagraphStyles.FindObj(PStyle) != -1);

			// Deal with italic/bold
			DPhrase[] vPhrases = ParseIntoPhrases(LabelText).ToArray();

			// Create and append the paragraph
			OWPara para = new OWPara(WritingSystem, PStyle, vPhrases);
			Verbose.Contents.Append(para);
		}
		#endregion

		// Terse -----------------------------------------------------------------------------
		// We do nothing here.
	}
	#endregion
    #region CLASS: StringChoiceSetting : Setting
    public class StringChoiceSetting : Setting
    {
        #region Attr{g}: string[] Possibilities
        string[] Possibilities
        {
            get
            {
                Debug.Assert(null != m_vPossilibities);
                return m_vPossilibities;
            }
        }
        string[] m_vPossilibities;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Attr{g/s}: string Value
        public string Value
        {
            get
            {
                return m_sValue;
            }
            set
            {
                m_sValue = value;
            }
        }
        string m_sValue;
        #endregion
        #region Constructor(...)
        public StringChoiceSetting(LiterateSettingsWnd ls, string sID, string sLabel,
            string sDescription, string sGroup, string sInitialValue, string[] vPossibilities)
            : base(ls, sID, sLabel, sDescription, sGroup)
        {
            m_vPossilibities = vPossibilities;
            m_sValue = sInitialValue;
        }
        #endregion

        // Verbose ---------------------------------------------------------------------------
        #region Cmd: cmdVerboseControlChanged
        private void cmdVerboseControlChanged(object sender, EventArgs e)
        {
            ComboBox ctrl = sender as ComboBox;
            if (null == ctrl)
                return;
            Value = (string)ctrl.SelectedItem;
        }
        #endregion
        #region OMethod: void BuildVerbose()
        public override void BuildVerbose()
        {
            ToolStrip strip = AddToolStrip();

            // Add the label
            ToolStripLabel label = new ToolStripLabel(LabelText);
            label.Width = AlignmentColumn;
            label.AutoSize = false;
            label.TextAlign = ContentAlignment.MiddleRight;
            strip.Items.Add(label);

            // Create and initialize the combo box
            ComboBox cb = new ComboBox();
            foreach (string s in Possibilities)
                cb.Items.Add(s);
            cb.Text = Value;
            cb.SelectionChangeCommitted += new EventHandler(cmdVerboseControlChanged);

            // Add it to the toolstrip (we must use a host object)
            ToolStripControlHost host = new ToolStripControlHost(cb);
            strip.Items.Add(host);
        }
        #endregion

        // Terse -----------------------------------------------------------------------------
        #region OMethod: void BuildTerse()
        public override void BuildTerse()
        {
            PropertySpec ps = new PropertySpec(
               ID,
               LabelText,
               Group,
               Description,
               m_vPossilibities,
               Value);
            Terse.Properties.Add(ps);
        }
        #endregion
        #region OMethod: void BagGetValue(PropertySpecEventArgs e)
        public override void BagGetValue(PropertySpecEventArgs e)
        {
            if (e.Property.ID != ID)
                return;
            e.Value = Value;
        }
        #endregion
        #region OMethod: void BagSetValue(PropertySpecEventArgs e)
        public override void BagSetValue(PropertySpecEventArgs e)
        {
            if (e.Property.ID != ID)
                return;
            Value = (string)e.Value;
        }
        #endregion
    }
    #endregion

}
