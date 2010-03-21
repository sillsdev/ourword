#region ***** Roles.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Roles.cs
 * Author:  John Wimbish
 * Created: 22 Feb 2010
 * Purpose: The roles (status) of a message/annotation 
 * Legal:   Copyright (c) 2005-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using JWTools;

#endregion

namespace OurWordData.DataModel.Annotations
{
    public class Role
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: string EnglishName
        public string EnglishName
        {
            get
            {
                Debug.Assert(!string.IsNullOrEmpty(m_sEnglishName));
                return m_sEnglishName;
            }
        }
        private readonly string m_sEnglishName;
        #endregion
        #region VAttr{g}: string LocalizedName
        public string LocalizedName
        {
            get
            {
                var sLookupKey = "k" + EnglishName.Replace(" ", "");
                return Loc.GetNotes(sLookupKey, EnglishName);
            }
        }
        #endregion
        #region Attr{g/s}: Color IconColor
        public Color IconColor = TranslatorNote.OriginalBitmapNoteColor;
        #endregion
        #region Attr{g}: string LocalizedToolTipText
        public string LocalizedToolTipText
        {
            get
            {
                var sLookupKey = "tip" + EnglishName.Replace(" ", "");
                return Loc.GetNotes(sLookupKey, EnglishToolTipText);
            }
        }
        private string EnglishToolTipText;
        #endregion
        #region Attr{g/s}: bool IsConversational
        // If set to true, then annotations of this role will by default be displayed having
        // multiple messages; that is, having a conversation. If false, then only the first
        // message is shown by default. Thus for example we would expect MTTs looking at
        // DaughterTeam annotations to only need to see the initial message.
        public bool IsConversational { get; set; }
        #endregion
        #region Attr{g/s}: string SfmMarker
        public string SfmMarker
            // Used for import/export to Toolbox sfm. Some of the roles are saved to markers
            // other than the \nt of most notes. As of v1.6 this is a small subset of the
            // many markers previously in use, though.
        {
            get
            {
                return m_sSfmMarker;
            }
            private set
            {
                Debug.Assert(!string.IsNullOrEmpty(value));
                m_sSfmMarker = value;
            }
        }
        private string m_sSfmMarker = "nt";
        #endregion

        // User Access -----------------------------------------------------------------------
        #region Attr{g/s}: CanCreatePropertyName
        // If it has a value, then we call the TranslatorNote.GetNoteSetting method to 
        // learn whether or not this Role is turned on for this User. If it is turned
        // on, then (1) it is displayed in the appropriate views, and (2) it appears
        // in the AssignedTo dropdown.
        private string CanCreatePropertyName { get; set; }
        #endregion
        #region Attr{g/s}: bool ThisUserCanAssignTo
        public bool ThisUserCanAssignTo
        {
            get
            {
                if (string.IsNullOrEmpty(CanCreatePropertyName))
                    return true;

                return TranslatorNote.GetNoteSetting(CanCreatePropertyName);
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(sEnglishName)
        private Role(string sEnglishName)
        {
            m_sEnglishName = sEnglishName;
            s_vRoles.Add(this);

            // By default, users can create Chats on annotation assigned to this Role
            IsConversational = true;
        }
        #endregion

        // List of all Roles -----------------------------------------------------------------
        #region IEnumerable<Role> AllRoles
        static public IEnumerable<Role> AllRoles
        {
            get
            {
                Debug.Assert(null != s_vRoles);
                return s_vRoles;
            }
        }
        private static readonly List<Role> s_vRoles = new List<Role>();
        #endregion
        #region SMethod: Role FindFromLocalizedName(string sLocalizedName)
        static public Role FindFromLocalizedName(string sLocalizedName)
        {
            // if Empty, then we return Closed
            if (string.IsNullOrEmpty(sLocalizedName))
                return Closed;

            foreach (var r in AllRoles)
            {
                if (r.EnglishName == sLocalizedName)
                    return r;
            }

            return Anyone;
        }
        #endregion
        #region SMethod: Role FindFromEnglishName(string sEnglishName)
        static public Role FindFromEnglishName(string sEnglishName)
        {
            // if Empty, then we return Closed
            if (string.IsNullOrEmpty(sEnglishName))
                return Closed;

            foreach (var r in AllRoles)
            {
                if (r.EnglishName == sEnglishName)
                    return r;
            }

            return Anyone;
        }
        #endregion
        #region SMethod: Role FindFromOxesName(string sOxesName)
        static public Role FindFromOxesName(string sOxesName)
        {
            // if Empty, then we return Closed
            if (string.IsNullOrEmpty(sOxesName))
                return Closed;

            // Try the English names first, as that's how we store them
            foreach (var r in AllRoles)
            {
                if (r.EnglishName == sOxesName)
                    return r;
            }

            // Now try the LocalizedNames, as that's how we used to store them, unfortunately
            foreach (var r in AllRoles)
            {
                if (r.LocalizedName == sOxesName)
                    return r;
            }

            // Give up. It means either a changed localization, or more likely an old
            // version where we stored as a person's real name.
            return Anyone;
        }
        #endregion

        // The set of Roles for Annotations used in Bible Translation ------------------------
        #region Roles
        static public readonly Role Anyone = new Role("Anyone") 
        {
            EnglishToolTipText = "This note needs attention. Click here to assign it\n" +
                "to someone, or to close it out as being finished.",
            IconColor = TranslatorNote.OriginalBitmapNoteColor
        };
        static public readonly Role Translator = new Role("Translator")
        {
            EnglishToolTipText = "One of the translators on the team needs to address\n" +
                "the issue raised in this note.",
            IconColor = Color.Yellow
        };
        static public readonly Role Consultant = new Role("Consultant")
        {
            EnglishToolTipText = "This note is either a question of,\n" +
                "or information for, the consultant.",
            IconColor = Color.LightGreen,
            CanCreatePropertyName = TranslatorNote.c_sCanCreateConsultantNotes
        };
        static public readonly Role Advisor = new Role("Advisor")
        {
            EnglishToolTipText = "This note needs input from the advisor.",
            IconColor = Color.LightBlue
        };
        public static readonly Role Closed = new Role("Closed") 
        {
            EnglishToolTipText = "This note has been closed out (considered finished).\n" +
                "Click here to re-open it, by assigning it to someone.",
            IconColor = Color.Gray
        };
        static public readonly Role DaughterTeam = new Role("Daughter Team")
        {
            EnglishToolTipText = "Assign to the Daughter Team if you wish to give them\n" +
                "help on translating this passage.",
            IconColor = Color.LightCyan,
            IsConversational = false,
            SfmMarker = "ntHint",
            CanCreatePropertyName = TranslatorNote.c_sCanCreateHintForDaughter
        };
        public static readonly Role Reference = new Role("Reference")
        {
            EnglishToolTipText = "This note is general-purpose information. It does not \n" + 
                "contain replies.",
            IconColor = Color.Goldenrod,
            IsConversational = false,
            SfmMarker = "ntcn",
            CanCreatePropertyName = TranslatorNote.c_sCanCreateReferenceNotes
        };
        #endregion
    }

}
