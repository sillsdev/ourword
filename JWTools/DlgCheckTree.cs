#region ***** DlgCheckTree.cs *****
/**********************************************************************************************
 * Dll:     JWTools
 * File:    DlgCheckTree.cs
 * Author:  John Wimbish
 * Created: 22 Sep 2009
 * Purpose: Provides a generic dialog (and underlying mechanism) for turning features on/off.
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
#endregion

namespace JWTools
{
    public partial class DlgCheckTree : Form
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: List<CheckTreeItem> Items
        public List<CheckTreeItem> Items
        {
            get
            {
                Debug.Assert(null != m_vItems);
                return m_vItems;
            }
        }
        readonly List<CheckTreeItem> m_vItems;
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
        #region VAttr{g}: string Label_Instruction
        public string Label_Instruction
        {
            get
            {
                return m_lblInstructions.Text;
            }
            set
            {
                m_lblInstructions.Text = value;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public DlgCheckTree()
        {
            InitializeComponent();

            m_vItems = new List<CheckTreeItem>();
        }
        #endregion

        // Populate the Tree -----------------------------------------------------------------
        #region smethod: AddNodeAlphabetic(TreeNodeCollection, TreeNode) - adds in sorted order
        private static void AddNodeAlphabetic(TreeNodeCollection nodes, TreeNode node)
        {
            for (var i = 0; i < nodes.Count; i++)
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
        #region Method: void CheckTopLevelNodes()
        void CheckTopLevelNodes()
        {
            foreach (TreeNode node in Tree.Nodes)
            {
                // We're only interested in top-level nodes that have children
                if (node.Nodes.Count == 0)
                    continue;

                // Find out if all of its children nodes are checked
                var bAllChecked = true;
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
        #region Method: void AddItem(CheckTreeItem, TreeNodeCollection)
        void AddItem(CheckTreeItem item, TreeNodeCollection parent)
        {
            var node = new TreeNode(item.Name) {
                Checked = item.Checked, 
                Tag = item
            };
            AddNodeAlphabetic(parent, node);

            foreach (CheckTreeItem cti in item.SubItems)
                AddItem(cti, node.Nodes);
        }
        #endregion
        #region Method: void PopulateTheTree()
        void PopulateTheTree()
        {
            // Initialize the Tree of features
            Tree.Nodes.Clear();
            foreach (CheckTreeItem cti in Items)
                AddItem(cti, Tree.Nodes);
            CheckTopLevelNodes();
            Tree.ExpandAll();

            // Select the first item (if there is a first item, that is.)
            if (Tree.Nodes.Count > 0)
                Tree.SelectedNode = Tree.Nodes[0];
        }
        #endregion

        // Handlers --------------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            PopulateTheTree();
        }
        #endregion
        #region Cmd: cmdBeforeCollapse - prevent nodes from being collapsed
        private void cmdBeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;

        }
        #endregion
        #region Cmd: cmdItemChecked
        static bool bDontRecurse;
        private void cmdItemChecked(object sender, TreeViewEventArgs e)
        {
            if (bDontRecurse)
                return;
            bDontRecurse = true;

            // Get the selected node
            var node = e.Node;
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
                        if (null != (n.Tag as CheckTreeItem))
                            (n.Tag as CheckTreeItem).Checked = n.Checked;
                    }
                }
                goto end;
            }

            // Otherwise, we have a feature to update
            var cti = node.Tag as CheckTreeItem;
            if (null == cti)
                goto end;
            cti.Checked = node.Checked;

            // Update the top level; but we don't want to come back into this method
            // while doing so!
            CheckTopLevelNodes();
        end:
            bDontRecurse = false;
        }
        #endregion
    }

    public class CheckTreeItem
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g/s}: string Name
        public string Name
        {
            get
            {
                return m_sName;
            }
            set
            {
                m_sName = value;
            }
        }
        string m_sName;
        #endregion
        #region Attr{g/s}: bool Checked
        public bool Checked
        {
            get
            {
                return m_bChecked;
            }
            set
            {
                m_bChecked = value;
            }
        }
        bool m_bChecked;
        #endregion
        #region Attr{g/s}: object Tag
        public object Tag
        {
            get
            {
                return m_Tag;
            }
            set
            {
                m_Tag = value;
            }
        }
        object m_Tag;
        #endregion
        #region Attr{g}: List<CheckTreeItem> SubItems
        public List<CheckTreeItem> SubItems
        {
            get
            {
                Debug.Assert(null != m_vSubItems);
                return m_vSubItems;
            }
        }
        readonly List<CheckTreeItem> m_vSubItems;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(sName, bChecked, Tag)
        public CheckTreeItem(string sName, bool bChecked, object tag)
        {
            m_sName = sName;
            m_bChecked = bChecked;
            m_Tag = tag;

            m_vSubItems = new List<CheckTreeItem>();
        }
        #endregion
    }

}
