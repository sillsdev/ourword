#region ***** DEventMessage.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    DEventMessage.cs
 * Author:  John Wimbish
 * Created: 04 Nov 2008
 * Purpose: Handles a single history annotation's message 
 * Legal:   Copyright (c) 2005-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Xml;
using JWTools;
#endregion

namespace OurWordData.DataModel.Annotations
{
    public class DEventMessage : DMessage
    {
        // Content Attrs ---------------------------------------------------------------------
        #region BAttr{g/s}: DateTime EventDate - Date this event took place
        public DateTime EventDate
        {
            get
            {
                if (m_dtEventDate.CompareTo(DefaultDate) == 0)
                    m_dtEventDate = UtcCreated;

                return m_dtEventDate;
            }
            set
            {
                SetValue(ref m_dtEventDate, value);
            }
        }
        private DateTime m_dtEventDate;
        #endregion
        #region Method void DeclareAttrs()
        protected override void DeclareAttrs()
        {
            base.DeclareAttrs();
            DefineAttr("Date", ref m_dtEventDate);
        }
        #endregion

        #region Attr{g/s}: Stage Stage
        public Stage Stage
        {
            get
            {
                return m_Stage;
            }
            set
            {
                if (m_Stage != value)
                {
                    m_Stage = value;
                    DeclareDirty();
                }
            }
        }
        Stage m_Stage;
        #endregion

        // VAttrs ----------------------------------------------------------------------------
        #region VAttr{g}: DateTime DefaultDate - default for the Date attr
        public static DateTime DefaultDate
        {
            get
            {
                return new DateTime(2009, 1, 1);
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public DEventMessage()
            : base()
        {
        }
        #endregion
        #region Constructor(XmlNode)
        public DEventMessage(XmlNode node)
            : this()
        {
            ReadFromOxes(node);
        }
        #endregion
        #region Constructor(utcDate, sStage, sDescription)
        public DEventMessage(DateTime utcDate, Stage stage, string sDescription)
            : this()
        {
            EventDate = utcDate;
            UtcCreated = EventDate;
            m_Stage = stage;
            SimpleText = sDescription;
        }
        #endregion
        #region OMethod: bool ContentEquals(DEventMessage)
        public override bool ContentEquals(JObject obj)
        {
            var e = obj as DEventMessage;
            if (null == e)
                return false;

            if (e.EventDate != EventDate)
                return false;
            if (e.Stage.EnglishAbbrev != Stage.EnglishAbbrev)
                return false;

            return base.ContentEquals(obj);
        }
        #endregion
        #region OMethod: DMessage Clone() - returns a DEventMessage
        public override DMessage Clone()
        {
            var em = new DEventMessage();
            em.Author = Author;
            em.UtcCreated = UtcCreated;
            em.Status = Status;
            em.CopyFrom(this, false);

            em.EventDate = EventDate;
            em.Stage = Stage;

            return em;
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region Constants
        const string c_sAttrEventDate = "when";
        const string c_sAttrStage = "stage";
        #endregion
        #region OMethod: bool ReadFromOxes(XmlNode node)

        protected override bool ReadFromOxes(XmlNode node)
        {
            // Read all of the base data
            if (!base.ReadFromOxes(node))
                return false;

            // Add the DEventMessage data
            // Stage
            var sStage = XmlDoc.GetAttrValue(node, c_sAttrStage, Stage.c_sDraft);
            // Normally we expect this to be the English Abbreviation
            Stage = DB.TeamSettings.Stages.Find(sStage);
            // But old data may have been saved some other way
            if (null == Stage)
                Stage = DB.TeamSettings.Stages.Find(StageList.FindBy.LocalizedAbbrev, sStage);
            if (null == Stage)
                Stage = DB.TeamSettings.Stages.Find(StageList.FindBy.LocalizedName, sStage);
            if (null == Stage)
                Stage = DB.TeamSettings.Stages.Find(StageList.FindBy.EnglishName, sStage);
            // Give Up
            if (null == Stage)
                Stage = DB.TeamSettings.Stages.Draft;

            // EventDate, including old-style which was "Date"
            EventDate = XmlDoc.GetAttrValue(node,
                new string[] { c_sAttrEventDate, "Date" },
                DateTime.Now);

            return true;
        }
        #endregion
        #region OMethod: XmlNode Save(oxes, nodeAnnotation)
        public override XmlNode Save(XmlDoc oxes, XmlNode nodeAnnotation)
        {
            // Save the superclass values
            XmlNode nodeEventMessage = base.Save(oxes, nodeAnnotation);

            // Add in our additional attributes
            oxes.AddAttr(nodeEventMessage, c_sAttrEventDate, EventDate);
            oxes.AddAttr(nodeEventMessage, c_sAttrStage, Stage.EnglishAbbrev);

            return nodeEventMessage;
        }
        #endregion
        #region SMethod: bool IsEventMessageNode(XmlNode node)
        static public bool IsEventMessageNode(XmlNode node)
        {
            if (XmlDoc.HasAttr(node, c_sAttrEventDate))
                return true;
            return false;
        }
        #endregion
    }
}
