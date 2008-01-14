/**********************************************************************************************
 * Dll:     JWTools
 * File:    JW_UserFeatures.cs
 * Author:  John Wimbish
 * Created: 03 Oct 2003
 * Purpose: Provides a dialog (and underlying mechanism) for turning features on/off.
 * Legal:   Copyright (c) 2005-07, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/

#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
#endregion

namespace JWTools
{
	#region FORM CLASS: JW_UserFeatures
	public class DialogSetupFeatures : System.Windows.Forms.Form
	{
		// Array of features ----------------------------------------------------------------
		private ArrayList m_rgFeatures;
		#region Method: void Add(...) - add a Feature to the list during setup
		public void Add(string sName, bool bEnabled, string sCheckBoxName, string sDescription)
		{
			JW_Feature feat = new JW_Feature(sName, bEnabled, sCheckBoxName, sDescription);
			for(int i=0; i<m_rgFeatures.Count; i++)
			{
				JW_Feature f = m_rgFeatures[i] as JW_Feature;
				if (sCheckBoxName.CompareTo(f.CheckBoxName) < 0)
				{
					m_rgFeatures.Insert(i, feat);
					return;
				}
			}

			m_rgFeatures.Add(feat);
		}
		#endregion
		#region Method: void Clear()
		public void Clear()
		{
			m_rgFeatures.Clear();
		}
		#endregion
		#region Method: bool GetEnabledState(sName) - returns T or F for the feature
		public bool GetEnabledState(string sName)
		{
			foreach (JW_Feature feat in m_rgFeatures)
			{
				if (feat.Name == sName)
					return feat.Enabled;
			}
			Debug.Assert(false); // the feature wasn't found in the list.
			return false;
		}
		#endregion
		#region Method: void AddDependency(sNameTrigger, sNameDependency)
		public void AddDependency(string sNameTrigger, string sNameDependency)
		{
			// Find the Trigger and the Dependency
			JW_Feature fTrigger    = null;
			JW_Feature fDependency = null;
			foreach (JW_Feature feat in m_rgFeatures)
			{
				if (feat.Name == sNameTrigger)
					fTrigger = feat;
				if (feat.Name == sNameDependency)
					fDependency = feat;
			}

			// Should have something for both; else programmer hasn't set it up correctly
			Debug.Assert (null != fTrigger && null != fDependency, "Improper Features setup");

			// Add the dependency
			fTrigger.AddDependency(fDependency);
		}
		#endregion

		// Dialog controls -------------------------------------------------------------------
		#region Dialog Controls
		private CheckedListBox m_checkListFeatures;
		private Label    m_lblInstructions;
		private GroupBox m_box;
		private Label    m_lblDescription;
		private Button   m_btnAllOn;
		private Button   m_btnPasswordProtect;
        private Button   m_btnResetDefaults;
		private Button   m_btnAllOff;
		private Button   m_btnOK;
		private Button   m_btnCancel;
		private Button   m_btnHelp;
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor() - initializes the features list
		public DialogSetupFeatures()
			// Constructor
		{
			InitializeComponent();
			m_rgFeatures = new ArrayList();
		}
		#endregion
		#region Windows Form Designer generated code

		// Required designer variable.
		private System.ComponentModel.Container components = null;

		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		/// -----------------------------------------------------------------------------------
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogSetupFeatures));
            this.m_checkListFeatures = new System.Windows.Forms.CheckedListBox();
            this.m_lblInstructions = new System.Windows.Forms.Label();
            this.m_box = new System.Windows.Forms.GroupBox();
            this.m_lblDescription = new System.Windows.Forms.Label();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnAllOn = new System.Windows.Forms.Button();
            this.m_btnAllOff = new System.Windows.Forms.Button();
            this.m_btnPasswordProtect = new System.Windows.Forms.Button();
            this.m_btnHelp = new System.Windows.Forms.Button();
            this.m_btnResetDefaults = new System.Windows.Forms.Button();
            this.m_box.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_checkListFeatures
            // 
            this.m_checkListFeatures.Location = new System.Drawing.Point(8, 24);
            this.m_checkListFeatures.Name = "m_checkListFeatures";
            this.m_checkListFeatures.Size = new System.Drawing.Size(272, 184);
            this.m_checkListFeatures.TabIndex = 0;
            this.m_checkListFeatures.SelectedIndexChanged += new System.EventHandler(this.cmd_OnSelectedIndexChanged);
            this.m_checkListFeatures.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.cmdItemCheck);
            // 
            // m_lblInstructions
            // 
            this.m_lblInstructions.Location = new System.Drawing.Point(8, 8);
            this.m_lblInstructions.Name = "m_lblInstructions";
            this.m_lblInstructions.Size = new System.Drawing.Size(374, 16);
            this.m_lblInstructions.TabIndex = 1;
            this.m_lblInstructions.Text = "Place a check beside the features you want to turn on in the user interface.";
            // 
            // m_box
            // 
            this.m_box.Controls.Add(this.m_lblDescription);
            this.m_box.Location = new System.Drawing.Point(8, 216);
            this.m_box.Name = "m_box";
            this.m_box.Size = new System.Drawing.Size(272, 112);
            this.m_box.TabIndex = 2;
            this.m_box.TabStop = false;
            this.m_box.Text = "Description";
            // 
            // m_lblDescription
            // 
            this.m_lblDescription.Location = new System.Drawing.Point(8, 16);
            this.m_lblDescription.Name = "m_lblDescription";
            this.m_lblDescription.Size = new System.Drawing.Size(256, 88);
            this.m_lblDescription.TabIndex = 0;
            this.m_lblDescription.Text = "(description goes here)";
            // 
            // m_btnOK
            // 
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(296, 246);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 3;
            this.m_btnOK.Text = "OK";
            this.m_btnOK.Click += new System.EventHandler(this.cmd_OnClick_OK);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(296, 275);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 4;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_btnAllOn
            // 
            this.m_btnAllOn.Location = new System.Drawing.Point(296, 24);
            this.m_btnAllOn.Name = "m_btnAllOn";
            this.m_btnAllOn.Size = new System.Drawing.Size(75, 23);
            this.m_btnAllOn.TabIndex = 5;
            this.m_btnAllOn.Text = "All &On";
            this.m_btnAllOn.Click += new System.EventHandler(this.cmd_OnClick_AllOn);
            // 
            // m_btnAllOff
            // 
            this.m_btnAllOff.Location = new System.Drawing.Point(296, 53);
            this.m_btnAllOff.Name = "m_btnAllOff";
            this.m_btnAllOff.Size = new System.Drawing.Size(75, 23);
            this.m_btnAllOff.TabIndex = 6;
            this.m_btnAllOff.Text = "All O&ff";
            this.m_btnAllOff.Click += new System.EventHandler(this.cmd_OnClick_AllOff);
            // 
            // m_btnPasswordProtect
            // 
            this.m_btnPasswordProtect.Location = new System.Drawing.Point(296, 173);
            this.m_btnPasswordProtect.Name = "m_btnPasswordProtect";
            this.m_btnPasswordProtect.Size = new System.Drawing.Size(75, 48);
            this.m_btnPasswordProtect.TabIndex = 7;
            this.m_btnPasswordProtect.Text = "Password Protect...";
            this.m_btnPasswordProtect.Click += new System.EventHandler(this.cmd_onPasswordProtect);
            // 
            // m_btnHelp
            // 
            this.m_btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("m_btnHelp.Image")));
            this.m_btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_btnHelp.Location = new System.Drawing.Point(296, 304);
            this.m_btnHelp.Name = "m_btnHelp";
            this.m_btnHelp.Size = new System.Drawing.Size(75, 23);
            this.m_btnHelp.TabIndex = 8;
            this.m_btnHelp.Text = "Help...";
            this.m_btnHelp.Click += new System.EventHandler(this.cmdHelp);
            // 
            // m_btnResetDefaults
            // 
            this.m_btnResetDefaults.Location = new System.Drawing.Point(296, 82);
            this.m_btnResetDefaults.Name = "m_btnResetDefaults";
            this.m_btnResetDefaults.Size = new System.Drawing.Size(75, 39);
            this.m_btnResetDefaults.TabIndex = 9;
            this.m_btnResetDefaults.Text = "Reset Defaults";
            this.m_btnResetDefaults.Click += new System.EventHandler(this.cmdResetDefaults);
            // 
            // DialogSetupFeatures
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(386, 336);
            this.Controls.Add(this.m_btnResetDefaults);
            this.Controls.Add(this.m_btnHelp);
            this.Controls.Add(this.m_btnPasswordProtect);
            this.Controls.Add(this.m_btnAllOff);
            this.Controls.Add(this.m_btnAllOn);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_box);
            this.Controls.Add(this.m_lblInstructions);
            this.Controls.Add(this.m_checkListFeatures);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogSetupFeatures";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Set Up Features";
            this.Load += new System.EventHandler(this.cmd_OnLoad);
            this.m_box.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion
		#region Dispose(disposing)
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

		// Methods ---------------------------------------------------------------------------
		#region Method: DialogResult ShowDialog(Form formParent, bool bSaveStateInRegistry)
		public DialogResult ShowDialog(Form formParent)
		{
			// If a password is being used, then we need to present the dialog
			// to ask for a password. (Otherwise, we can assume that the user does not care
			// to use a password to protect access to this dialog.) The Dialog resturns
			// DialogResult.OK if the proper password has been entered.
			if (DialogPassword.IsPasswordProtected)
			{
				DialogPassword dlgPassword = new DialogPassword();
				DialogResult result = dlgPassword.ShowDialog(formParent);
				if (DialogResult.OK != result)
					return DialogResult.Cancel;
			}

			return base.ShowDialog(formParent);
		}
		#endregion

		// Event Handling --------------------------------------------------------------------
		#region Event: cmd_OnLoad(...) - populates the checklist with the features
		private void cmd_OnLoad(object sender, System.EventArgs e)
			// Assumes Add has been called.
		{
			// Label text in the appropriate language
            Control[] vExclude = { };
            LocDB.Localize(this, vExclude);

            // Initialize the list of features
			m_checkListFeatures.Items.Clear();
			foreach(JW_Feature feat in m_rgFeatures)
				m_checkListFeatures.Items.Add(feat.CheckBoxName, feat.Enabled);

			// Select the first item (if there is a first item, that is.)
			if (m_checkListFeatures.Items.Count > 0)
				m_checkListFeatures.SetSelected(0, true);
		}
		#endregion
		#region Event: cmd_OnSelectionIndexChanged(...) - updates the Description label
		private void cmd_OnSelectedIndexChanged(object sender, System.EventArgs e)
		{
			int i = m_checkListFeatures.SelectedIndex;
			JW_Feature feat = (JW_Feature)m_rgFeatures[i];
			m_lblDescription.Text = feat.Description;
		}
		#endregion
		#region Event: cmd_OnClick_AllOn(...) - checks all the items in response to All On button
		private void cmd_OnClick_AllOn(object sender, System.EventArgs e)
		{
			for(int i=0; i<m_checkListFeatures.Items.Count; i++)
			{
                // Get the feature definition
                JW_Feature feature = (JW_Feature)m_rgFeatures[i];

                // If it has dependancies, then we don't turn it on (Thus in OurWord,
                // JustTheBasics does not get turned on, because it is incompatable with
                // a large number of dependencies.)
                if (feature.Dependencies.Count > 0)
                    continue;

                // Otherwise, set the check
				m_checkListFeatures.SetItemChecked(i, true);
			}
		}
		#endregion
		#region Event: cmd_OnClick_AllOff(...) - unchecks all the items in response to All Off button
		private void cmd_OnClick_AllOff(object sender, System.EventArgs e)
		{
			for(int i=0; i<m_checkListFeatures.Items.Count; i++)
			{
				m_checkListFeatures.SetItemChecked(i, false);
			}		
		}
		#endregion
		#region Event: cmd_OnClick_OK(...) - saves everything (User has clicked OK button
		private void cmd_OnClick_OK(object sender, System.EventArgs e)
			// User has accepted changes. Save these from the checkbox control to the
			// list, and write them out to the Registry.
		{
			string sPassword = DialogPassword.GetPassword();
			JW_Registry.DeleteSubKey("Features");
			for(int i=0; i<m_checkListFeatures.Items.Count; i++)
			{
				((JW_Feature)m_rgFeatures[i]).Enabled = m_checkListFeatures.GetItemChecked(i);
				((JW_Feature)m_rgFeatures[i]).WriteToRegistry();
			}
			DialogPassword.SetPassword(sPassword);
		}
		#endregion
		#region Event: cmd_onPasswordProtect(...) - prompts for a password
		private void cmd_onPasswordProtect(object sender, System.EventArgs e)
		{
			DialogPasswordProtect dlg = new DialogPasswordProtect();
			dlg.ShowDialog(this);
		}
		#endregion
		#region Event: cmdItemCheck(...) - clears any dependencies
		bool m_cmdItemCheckInProgress = false;
		private void cmdItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
		{
			// Get the selected item (if any)
			int i = m_checkListFeatures.SelectedIndex;
			if (-1 == i)
				return;

			// Get the value it will be; we only care if it is going to be checked
			JW_Feature feature = (JW_Feature)m_rgFeatures[i];
			bool bValueAfterCheck = ! m_checkListFeatures.GetItemChecked(i);
			if (!bValueAfterCheck)
				return;

			// We get a stack overflow if we let this method recurse; it will otherwise
			// because we are unchecking other lines.
			if (m_cmdItemCheckInProgress)
				return;
			m_cmdItemCheckInProgress = true;

            // If we are a dependency of another checked item, then we cannot permit the 
            // item to be checked
            foreach (JW_Feature fParent in m_rgFeatures)
            {
                // Find out if the potential parent item is checked. If not, then keep
                // looping
                int k = m_checkListFeatures.Items.IndexOf(fParent.CheckBoxName);
                if (-1 == k || !m_checkListFeatures.GetItemChecked(k))
                    continue;

                // We have a checked parent. Look through its children and see if
                // we are a dependancy. If so, we must force an Unchecked.
                foreach (JW_Feature fChild in fParent.Dependencies)
                {
                    if (fChild == feature)
                    {
                        e.NewValue = CheckState.Unchecked;
                        m_cmdItemCheckInProgress = false;
                        return;
                    }
                }
            }

			// Clear any dependencies
			foreach ( JW_Feature fDependency in feature.Dependencies)
			{
				// Get the position in the index
				int iPos = 0;
				foreach (JW_Feature fUnCheck in m_rgFeatures)
				{
					if (fUnCheck.Name == fDependency.Name)
						break;
					iPos++;
				}

				// Clear its value
				m_checkListFeatures.SetItemChecked(iPos, false);
			}

			m_cmdItemCheckInProgress = false;
		}
		#endregion
		#region Event: cmdHelp(...)
		private void cmdHelp(object sender, System.EventArgs e)
		{
			JW_Help.Show_DlgFeatures();
		}
		#endregion
        #region Event: cmdResetDefaults
        private void cmdResetDefaults(object sender, EventArgs e)
        {
            for (int i = 0; i < m_checkListFeatures.Items.Count; i++)
            {
                JW_Feature feature = (JW_Feature)m_rgFeatures[i];
                m_checkListFeatures.SetItemChecked(i, feature.DefaultEnabled);
            }
        }
        #endregion
    }

    #endregion

    public class JW_Feature
	{
        const string s_LocGroupName = "DialogSetupFeatures";

		#region Attr{g/s}: string Name - the internal program name of the feature
		public string Name 
		{ 
			get { return m_sName; } 
			set { m_sName = value; }
		}
		private string m_sName;                  // program-understood name (stored in registry)
		#endregion
		#region Attr{g/s}: bool Enabled - T if the feature is turned on
		public bool Enabled
		{
			get { return m_bEnabled; }
			set { m_bEnabled = value; }
		}
		private bool   m_bEnabled;               // T if the feature is turned on
		#endregion
        #region Attr{g}: bool DefaultEnabled - T if the feature is turned on by default
        public bool DefaultEnabled
        {
            get 
            {
                return m_bDefaultEnabled; 
            }
        }
        private bool m_bDefaultEnabled; 
        #endregion

        #region Attr{g/s}: string CheckBoxName_EnglishDefault - the default English name for the checkbox
        public string CheckBoxName_EnglishDefault
        {
            get
            {
                return m_sCheckBoxName_EnglishDefault;
            }
            set
            {
                m_sCheckBoxName_EnglishDefault = value;
            }
        }
        private string m_sCheckBoxName_EnglishDefault;          // Name as seen in the UI (can be localized)
        #endregion
        #region VAttr{g}: string CheckBoxName - the localized checkbox name as seen in the UI
		public string CheckBoxName 
		{ 
			get 
            {
                return LocDB.GetValue(
                    new string[] { s_LocGroupName },
                    Name + "_CheckBox",
                    CheckBoxName_EnglishDefault,
                    null,
                    null);
            } 
		}
		#endregion

        #region Attr{g/s}: string Description_EnglishDefault - the default English name for the help text
        public string Description_EnglishDefault
		{ 
			get 
            {
                return m_sDescription_EnglishDefault; 
            } 
			set 
            {
                m_sDescription_EnglishDefault = value; 
            }
		}
        private string m_sDescription_EnglishDefault;           // Description (displayed in the UI)
		#endregion
        #region VAttr{g}: string Description - the localized English name for the help text
        public string Description
        {
            get
            {
                return LocDB.GetValue(
                    new string[] { s_LocGroupName },
                    Name + "_Description",
                    Description_EnglishDefault,
                    null,
                    null);
            }
        }
        #endregion

		#region ArrayList Dependencies - if we check this feature, the dependencies get cleared
		public ArrayList Dependencies
		{
			get
			{
				Debug.Assert(null != m_vDependencies);
				return m_vDependencies;
			}
		}
		ArrayList m_vDependencies = null;
		#endregion

		#region public Constructor(...) - initializes the attributes
		public JW_Feature(
            string sName, 
            bool bEnabledDefault, 
            string sCheckBoxName_EnglishDefault, 
            string sDescription_EnglishDefault)
		{
			// Initializations
			m_vDependencies = new ArrayList();

			// Get default values as passed in
			Name            = sName;
			Enabled         = bEnabledDefault;  // This gets overridden from the registry
            m_bDefaultEnabled = bEnabledDefault;

            m_sCheckBoxName_EnglishDefault = sCheckBoxName_EnglishDefault;
            m_sDescription_EnglishDefault  = sDescription_EnglishDefault;

			// Override the Enabled attribute from the registry
			ReadFromRegistry();
		}
		#endregion

		#region Method: void AddDependency(JW_Feature feat)
		public void AddDependency(JW_Feature feat)
		{
			foreach(JW_Feature f in Dependencies)
			{
				if (f == feat)
					return;
			}
			Dependencies.Add(feat);
		}
		#endregion

		#region Method: public WriteToRegistry() - saves the value to the registry
		public void WriteToRegistry()
		{
			JW_Registry.SetValue(DialogPassword.c_RegistryKey, Name, Enabled);
		}
		#endregion
		#region Method: public ReadFromRegistry() - reads the value from the registry
		public void ReadFromRegistry()
		{
			Enabled = JW_Registry.GetValue(DialogPassword.c_RegistryKey, Name, Enabled);
		}
		#endregion

	}

}
