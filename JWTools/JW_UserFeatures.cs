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
		public void Add(string sName, 
            bool bEnabled, 
            bool bBasicFeature, 
            string sTreePath,
            string sCheckBoxName, 
            string sDescription)
		{
            JW_Feature feat = new JW_Feature(sName, bEnabled, bBasicFeature, 
                sTreePath, sCheckBoxName, sDescription);

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

        // Dialog controls -------------------------------------------------------------------
        #region VAttr{g}: TreeView Tree
        TreeView Tree
        {
            get
            {
                Debug.Assert(null != m_Tree);
                return m_Tree;
            }
        }
        #endregion
        #region Dialog Controls

        private Label    m_lblInstructions;
		private GroupBox m_box;
		private Label    m_lblDescription;
		private Button   m_btnAllOn;
		private Button   m_btnPasswordProtect;
        private Button   m_btnResetDefaults;
		private Button   m_btnAllOff;
		private Button   m_btnOK;
		private Button   m_btnCancel;
        private Button m_btnJustTheBasics;
        private TreeView m_Tree;
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
            this.m_btnJustTheBasics = new System.Windows.Forms.Button();
            this.m_Tree = new System.Windows.Forms.TreeView();
            this.m_box.SuspendLayout();
            this.SuspendLayout();
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
            this.m_box.Location = new System.Drawing.Point(8, 313);
            this.m_box.Name = "m_box";
            this.m_box.Size = new System.Drawing.Size(321, 94);
            this.m_box.TabIndex = 2;
            this.m_box.TabStop = false;
            this.m_box.Text = "Description";
            // 
            // m_lblDescription
            // 
            this.m_lblDescription.Location = new System.Drawing.Point(10, 15);
            this.m_lblDescription.Name = "m_lblDescription";
            this.m_lblDescription.Size = new System.Drawing.Size(305, 74);
            this.m_lblDescription.TabIndex = 0;
            this.m_lblDescription.Text = "(description goes here)";
            // 
            // m_btnOK
            // 
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(335, 326);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 3;
            this.m_btnOK.Text = "OK";
            this.m_btnOK.Click += new System.EventHandler(this.cmd_OnClick_OK);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(335, 355);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 4;
            this.m_btnCancel.Text = "Cancel";
            this.m_btnCancel.Click += new System.EventHandler(this.cmd_OnClick_Cancel);
            // 
            // m_btnAllOn
            // 
            this.m_btnAllOn.Location = new System.Drawing.Point(335, 24);
            this.m_btnAllOn.Name = "m_btnAllOn";
            this.m_btnAllOn.Size = new System.Drawing.Size(75, 23);
            this.m_btnAllOn.TabIndex = 5;
            this.m_btnAllOn.Text = "All &On";
            this.m_btnAllOn.Click += new System.EventHandler(this.cmd_OnClick_AllOn);
            // 
            // m_btnAllOff
            // 
            this.m_btnAllOff.Location = new System.Drawing.Point(335, 53);
            this.m_btnAllOff.Name = "m_btnAllOff";
            this.m_btnAllOff.Size = new System.Drawing.Size(75, 23);
            this.m_btnAllOff.TabIndex = 6;
            this.m_btnAllOff.Text = "All O&ff";
            this.m_btnAllOff.Click += new System.EventHandler(this.cmd_OnClick_AllOff);
            // 
            // m_btnPasswordProtect
            // 
            this.m_btnPasswordProtect.Location = new System.Drawing.Point(335, 198);
            this.m_btnPasswordProtect.Name = "m_btnPasswordProtect";
            this.m_btnPasswordProtect.Size = new System.Drawing.Size(75, 48);
            this.m_btnPasswordProtect.TabIndex = 7;
            this.m_btnPasswordProtect.Text = "&Password Protect...";
            this.m_btnPasswordProtect.Click += new System.EventHandler(this.cmd_onPasswordProtect);
            // 
            // m_btnHelp
            // 
            this.m_btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("m_btnHelp.Image")));
            this.m_btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_btnHelp.Location = new System.Drawing.Point(335, 384);
            this.m_btnHelp.Name = "m_btnHelp";
            this.m_btnHelp.Size = new System.Drawing.Size(75, 23);
            this.m_btnHelp.TabIndex = 8;
            this.m_btnHelp.Text = "Help...";
            this.m_btnHelp.Click += new System.EventHandler(this.cmdHelp);
            // 
            // m_btnResetDefaults
            // 
            this.m_btnResetDefaults.Location = new System.Drawing.Point(335, 82);
            this.m_btnResetDefaults.Name = "m_btnResetDefaults";
            this.m_btnResetDefaults.Size = new System.Drawing.Size(75, 49);
            this.m_btnResetDefaults.TabIndex = 9;
            this.m_btnResetDefaults.Text = "Reset &Defaults";
            this.m_btnResetDefaults.Click += new System.EventHandler(this.cmdResetDefaults);
            // 
            // m_btnJustTheBasics
            // 
            this.m_btnJustTheBasics.Location = new System.Drawing.Point(335, 137);
            this.m_btnJustTheBasics.Name = "m_btnJustTheBasics";
            this.m_btnJustTheBasics.Size = new System.Drawing.Size(75, 55);
            this.m_btnJustTheBasics.TabIndex = 10;
            this.m_btnJustTheBasics.Text = "&Just The Basic Features";
            this.m_btnJustTheBasics.Click += new System.EventHandler(this.cmdJustTheBasics);
            // 
            // m_Tree
            // 
            this.m_Tree.CheckBoxes = true;
            this.m_Tree.HideSelection = false;
            this.m_Tree.Location = new System.Drawing.Point(8, 24);
            this.m_Tree.Name = "m_Tree";
            this.m_Tree.ShowNodeToolTips = true;
            this.m_Tree.ShowRootLines = false;
            this.m_Tree.Size = new System.Drawing.Size(321, 283);
            this.m_Tree.TabIndex = 11;
            this.m_Tree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.cmdItemChecked);
            this.m_Tree.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.cmdBeforeCollapse);
            this.m_Tree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.cmdNodeSelected);
            // 
            // DialogSetupFeatures
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(422, 412);
            this.Controls.Add(this.m_Tree);
            this.Controls.Add(this.m_btnJustTheBasics);
            this.Controls.Add(this.m_btnResetDefaults);
            this.Controls.Add(this.m_btnHelp);
            this.Controls.Add(this.m_btnPasswordProtect);
            this.Controls.Add(this.m_btnAllOff);
            this.Controls.Add(this.m_btnAllOn);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_box);
            this.Controls.Add(this.m_lblInstructions);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 440);
            this.Name = "DialogSetupFeatures";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Set Up Features";
            this.Resize += new System.EventHandler(this.cmdResize);
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
        #region Method: AddNodeAlphabetic(TreeNodeCollection, TreeNode) - adds in sorted order
        public void AddNodeAlphabetic(TreeNodeCollection nodes, TreeNode node)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (node.Text.CompareTo(nodes[i].Text) < 0)
                {
                    nodes.Insert(i, node);
                    return;
                }
            }
            nodes.Add(node);
        }
        #endregion

        // Event Handling --------------------------------------------------------------------
        #region Method: void AddFeatureToTree(JW_Feature feat)
        void AddFeatureToTree(JW_Feature feat)
        {
            // We'll add the new node under this collection of nodes
            TreeNodeCollection tnc = null;

            // If we have a Path, then we want to place it under a node
            if (!string.IsNullOrEmpty(feat.TreePath))
            {
                foreach (TreeNode n in Tree.Nodes)
                {
                    if (n.Text == feat.TreePath)
                    {
                        tnc = n.Nodes;
                        break;
                    }
                }

                if (null == tnc)
                {
                    TreeNode n = new TreeNode(feat.TreePath);
                    AddNodeAlphabetic(Tree.Nodes, n);
                    tnc = n.Nodes;
                }
            }
            if (null == tnc)
                tnc = Tree.Nodes;

            // Create and Add the node
            TreeNode node = new TreeNode(feat.CheckBoxName);
            node.Checked = feat.Enabled;
            node.Tag = feat;
            AddNodeAlphabetic(tnc, node);
        }
        #endregion
        #region Method: void CheckTopLevelNodes()
        void CheckTopLevelNodes()
        {
            foreach (TreeNode node in Tree.Nodes)
            {
                // We're only interested in top-level nodes that have children
                if (node.Nodes.Count == 0)
                    continue;

                // Find out if all of its children nodes are checked
                bool bAllChecked = true;
                foreach (TreeNode n in node.Nodes)
                {
                    if (!n.Checked)
                        bAllChecked = false;
                }

                // If they are, then check this one, too
                if (node.Checked != bAllChecked)
                    node.Checked = bAllChecked;
            }
        }
        #endregion

        #region Event: cmd_OnLoad(...) - populates the checklist with the features
        private void cmd_OnLoad(object sender, System.EventArgs e)
			// Assumes Add has been called.
		{
			// Label text in the appropriate language
            Control[] vExclude = { };
            LocDB.Localize(this, vExclude);

            // Initialize the Tree of features
            Tree.Nodes.Clear();
            foreach (JW_Feature feat in m_rgFeatures)
                AddFeatureToTree(feat);
            CheckTopLevelNodes();
            Tree.ExpandAll();

			// Select the first item (if there is a first item, that is.)
            if (Tree.Nodes.Count > 0)
                Tree.SelectedNode = Tree.Nodes[0];
		}
		#endregion
		#region Event: cmd_onPasswordProtect(...) - prompts for a password
		private void cmd_onPasswordProtect(object sender, System.EventArgs e)
		{
			DialogPasswordProtect dlg = new DialogPasswordProtect();
			dlg.ShowDialog(this);
		}
		#endregion
		#region Event: cmdHelp(...)
		private void cmdHelp(object sender, System.EventArgs e)
		{
			HelpSystem.Show_DlgFeatures();
		}
		#endregion

		#region Cmd: cmd_OnClick_OK - saves everything (User has clicked OK button)
		private void cmd_OnClick_OK(object sender, System.EventArgs e)
			// User has accepted changes. Save these from the checkbox control to the
			// list, and write them out to the Registry.
		{
			string sPassword = DialogPassword.GetPassword();
			JW_Registry.DeleteSubKey("Features");

            foreach (JW_Feature feature in m_rgFeatures)
                feature.WriteToRegistry();

			DialogPassword.SetPassword(sPassword);

            DialogResult = DialogResult.OK;
		}
		#endregion
        #region Cmd: cmd_OnClick_Cancel
        private void cmd_OnClick_Cancel(object sender, EventArgs e)
            // Reset the previous values to the "Enabled" attr before we exit
        {
            foreach (JW_Feature feature in m_rgFeatures)
                feature.ReadFromRegistry();

            DialogResult = DialogResult.Cancel;
        }
        #endregion
        #region Method: TreeNode Find(JW_Feature feature)
        TreeNode Find(JW_Feature feature)
        {
            foreach (TreeNode node in Tree.Nodes)
            {
                if (node.Tag as JW_Feature == feature)
                    return node;

                foreach (TreeNode n in node.Nodes)
                {
                    if (n.Tag as JW_Feature == feature)
                        return n;
                }
            }
            return null;
        }
        #endregion
		#region Cmd: cmd_OnClick_AllOn(...) - checks all the items in response to All On button
		private void cmd_OnClick_AllOn(object sender, System.EventArgs e)
		{
            foreach (TreeNode node in Tree.Nodes)
                node.Checked = true;
		}
		#endregion
		#region Cmd: cmd_OnClick_AllOff(...) - unchecks all the items in response to All Off button
		private void cmd_OnClick_AllOff(object sender, System.EventArgs e)
		{
            foreach (TreeNode node in Tree.Nodes)
                node.Checked = false;
		}
		#endregion
        #region Cmd: cmdResetDefaults
        private void cmdResetDefaults(object sender, EventArgs e)
        {
            foreach (JW_Feature feature in m_rgFeatures)
            {
                TreeNode node = Find(feature);
                if (null == node)
                    return;

                node.Checked = feature.DefaultEnabled;
            }
            CheckTopLevelNodes();
        }
        #endregion
        #region Cmd: cmdJustTheBasics
        private void cmdJustTheBasics(object sender, EventArgs e)
        {
            foreach (JW_Feature feature in m_rgFeatures)
            {
                TreeNode node = Find(feature);
                if (null == node)
                    return;

                node.Checked = feature.BasicFeature;
            }
            CheckTopLevelNodes();
        }
        #endregion
        #region Cmd: cmdItemChecked - enable or disable a feture
        static bool bDontRecurse = false;
        private void cmdItemChecked(object sender, TreeViewEventArgs e)
        {
            if (bDontRecurse)
                return;
            bDontRecurse = true;

            // Get the selected node
            TreeNode node = e.Node;
            if (null == node)
                goto end;

            // If it is a parent, then we want everything under it to be checked, too
            if (node.Nodes.Count > 0)
            {
                foreach (TreeNode n in node.Nodes)
                {
                    if (n.Checked != node.Checked)
                    {
                        n.Checked = node.Checked;
                        if (null != (n.Tag as JW_Feature))
                            (n.Tag as JW_Feature).Enabled = n.Checked;
                        }
                }
                goto end;
            }

            // Otherwise, we have a feature to update
            JW_Feature feature = node.Tag as JW_Feature;
            if (null == feature)
                goto end;
            feature.Enabled = node.Checked;

            // Update the top level; but we don't want to come back into this method
            // while doing so!
            CheckTopLevelNodes();
        end:
            bDontRecurse = false;
        }
        #endregion
        #region Cmd: cmdBeforeCollapse - prevent nodes from being collapsed
        private void cmdBeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
        }
        #endregion
        #region Cmd: cmdNodeSelected - updates the Description text
        private void cmdNodeSelected(object sender, TreeViewEventArgs e)
        {
            // Default to blank, in case we select a node that has no description (e.g.,
            // a top-level node.)
            m_lblDescription.Text = "";

            TreeNode node = e.Node;
            if (null == node)
                return;

            JW_Feature feature = node.Tag as JW_Feature;
            if (null == feature)
                return;

            m_lblDescription.Text = feature.Description;
        }
        #endregion

        #region Cmd: cmdResize - permit the user to change the dialog's size
        int m_nPreviousWidth = -1;
        int m_nPreviousHeight = -1;
        private void cmdResize(object sender, EventArgs e)
        {
            if (m_nPreviousWidth > 0)
            {
                int nDeltaWidth = Width - m_nPreviousWidth;

                // Tree and Description Box: just change the width
                m_Tree.Width += nDeltaWidth;
                m_box.Width += nDeltaWidth;
                m_lblDescription.Width += nDeltaWidth;

                // Buttons: change the x position
                m_btnAllOn.Left += nDeltaWidth;
                m_btnAllOff.Left += nDeltaWidth;
                m_btnResetDefaults.Left += nDeltaWidth;
                m_btnJustTheBasics.Left += nDeltaWidth;
                m_btnPasswordProtect.Left += nDeltaWidth;
                m_btnOK.Left += nDeltaWidth;
                m_btnCancel.Left += nDeltaWidth;
                m_btnHelp.Left += nDeltaWidth;
            }
            m_nPreviousWidth = Width;

            if (m_nPreviousHeight > 0)
            {
                int nDeltaHeight = Height - m_nPreviousHeight;

                // Tree: height changes to reflect the dialog size
                m_Tree.Height += nDeltaHeight;

                // Description: just reposition
                m_box.Top += nDeltaHeight;

                // Lower buttons: reposition (upper buttons remain in place)
                m_btnOK.Top += nDeltaHeight;
                m_btnCancel.Top += nDeltaHeight;
                m_btnHelp.Top += nDeltaHeight;
            }
            m_nPreviousHeight = Height;
        }
        #endregion
    }
    #endregion

    #region CLASS: JW_Feature
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
        #region Attr{g/s}: bool BasicFeature - T if the feature is turned on
        public bool BasicFeature
        {
            get { return m_bBasicFeature; }
            set { m_bBasicFeature = value; }
        }
        private bool m_bBasicFeature;    // T if the feature is a "Just the Basics" feature
        #endregion
        #region Attr{g/s}: string TreePath - the path to the feature in the treeview
        public string TreePath
        {
            get { return m_sTreePath; }
            set { m_sTreePath = value; }
        }
        private string m_sTreePath;                  // program-understood name (stored in registry)
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

		#region public Constructor(...) - initializes the attributes
		public JW_Feature(
            string sName, 
            bool bEnabledDefault, 
            bool bJustTheBasicFeature,
            string sTreePath,
            string sCheckBoxName_EnglishDefault, 
            string sDescription_EnglishDefault)
		{
			// Get default values as passed in
			Name            = sName;
            m_sTreePath     = sTreePath;
			Enabled         = bEnabledDefault;  // This gets overridden from the registry
            BasicFeature = bJustTheBasicFeature;
            m_bDefaultEnabled = bEnabledDefault;

            m_sCheckBoxName_EnglishDefault = sCheckBoxName_EnglishDefault;
            m_sDescription_EnglishDefault  = sDescription_EnglishDefault;

			// Override the Enabled attribute from the registry
			ReadFromRegistry();
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
    #endregion
}
