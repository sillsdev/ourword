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

        // User Access -----------------------------------------------------------------------
        private const string c_sUserAccessRegKey = "Roles";
        #region Attr{g/s}: bool ThisUserCanAccess
        public bool ThisUserCanAccess
        {
            get
            {
                return JW_Registry.GetValue(c_sUserAccessRegKey, EnglishName, AlwaysAvailable);
            }
            set
            {
                JW_Registry.SetValue(c_sUserAccessRegKey, EnglishName, value);
            }
        }
        #endregion
        #region Attr{g}: bool AlwaysAvailable
        // If True, then this Role is available for all users in all views. Otherwise,
        // it can be turned on or off for particular users.
        public bool AlwaysAvailable;
        #endregion
        // For Config Dlg UI
        #region SAttr{g}: string RolesTurnedOnForThisUser
        static public string RolesTurnedOnForThisUser
        {
            get
            {
                var sAdditionalUserNames = "";
                foreach(var role in AllRoles)
                {
                    if (role.ThisUserCanAccess && !role.AlwaysAvailable)
                    {
                        if (!string.IsNullOrEmpty(sAdditionalUserNames))
                            sAdditionalUserNames += ", ";
                        sAdditionalUserNames += role.LocalizedName;
                    }
                }

                if (string.IsNullOrEmpty(sAdditionalUserNames))
                    sAdditionalUserNames = NoExtraRoles;

                return sAdditionalUserNames;
            }
        }
        #endregion
        #region SAttr{g}: string NoExtraRoles
        static public string NoExtraRoles
        {
            get
            {
                return Loc.GetNotes("None", "None");
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(sEnglishName)
        private Role(string sEnglishName)
        {
            m_sEnglishName = sEnglishName;
            s_vRoles.Add(this);
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
            IconColor = TranslatorNote.OriginalBitmapNoteColor,
            AlwaysAvailable = true
        };
        static public readonly Role Translator = new Role("Translator")
        {
            EnglishToolTipText = "One of the translators on the team needs to address\n" +
                "the issue raised in this note.",
            IconColor = Color.Yellow,
            AlwaysAvailable = true
        };
        static public readonly Role Consultant = new Role("Consultant")
        {
            EnglishToolTipText = "This note is either a question of,\n" +
                "or information for, the consultant.",
            IconColor = Color.LightGreen
        };
        static public readonly Role Advisor = new Role("Advisor")
        {
            EnglishToolTipText = "This note needs input from the advisor.",
            IconColor = Color.LightBlue,
            AlwaysAvailable = true
        };
        static public readonly Role DaughterTeam = new Role("Daughter Team")
        {
            EnglishToolTipText = "Assign to the Daughter Team if you wish to give them\n" +
                "help on translating this passage.",
            IconColor = Color.LightCyan
        };
        static public readonly Role FrontTeam = new Role("Front Team")
        {
            EnglishToolTipText = "Assign to the Front Team if you wish to alert them\n" +
                "to an issue or suggestion for their translation.",
            IconColor = Color.LightCoral
        };
        public static readonly Role Closed = new Role("Closed") 
        {
            EnglishToolTipText = "This note has been closed out (considered finished).\n" +
                "Click here to re-open it, by assigning it to someone.",
            IconColor = Color.Gray,
            AlwaysAvailable = true
        };
        #endregion
    }

}
