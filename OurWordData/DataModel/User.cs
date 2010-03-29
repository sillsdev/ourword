using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OurWordData.DataModel
{
    public class User
    {
        class TranslationSettings
        {
            private DTranslation m_Translation;

            public bool CanCreateGeneralNotes { get; set; }
           
            public TranslationSettings(DTranslation t)
            {
                m_Translation = t;
            }
        }

        static private Dictionary<DTranslation, TranslationSettings> m_vTranslationSettings;
        static void Initialize()
        {
            if (null == m_vTranslationSettings)
                m_vTranslationSettings = new Dictionary<DTranslation, TranslationSettings>();
        }

        static TranslationSettings GetTranslationSettings(DTranslation t)
        {
            Initialize();
            TranslationSettings settings;
            if (!m_vTranslationSettings.TryGetValue(t, out settings))
            {
                settings = new TranslationSettings(t);
                m_vTranslationSettings.Add(t, settings);
            }
            return settings;
        }

        static public bool GetPermission(DTranslation t, string sProperty)
        {
            return false;
        }

        static public bool GetCanCreateGeneralNotes(DTranslation t)
        {
            return GetTranslationSettings(t).CanCreateGeneralNotes;
        }



        // TranslatorNote
        public bool CanCreateGeneralNotes { get; set; }
        public bool CanCreateConsultantNotes { get; set; }
        public bool CanCreateHintForDaughterNotes { get; set; }
        public bool CanCreateReferenceNotes { get; set; }



        public User()
        {
            
        }

        // Set to defined personas
        public static User Observer = new User 
        {
            CanCreateGeneralNotes = false,
            CanCreateConsultantNotes = false,
            CanCreateHintForDaughterNotes = false,
            CanCreateReferenceNotes = false
        };

        public static User Translator = new User() 
        {
            CanCreateGeneralNotes = true,
            CanCreateConsultantNotes = false,
            CanCreateHintForDaughterNotes = false,
            CanCreateReferenceNotes = false
        };
        public static User Consultant = new User() 
        {
            CanCreateGeneralNotes = true,
            CanCreateConsultantNotes = false,
            CanCreateHintForDaughterNotes = false,
            CanCreateReferenceNotes = false
        };
        public static User Advisor = new User()
        {
            CanCreateGeneralNotes = true,
            CanCreateConsultantNotes = true,
            CanCreateHintForDaughterNotes = true,
            CanCreateReferenceNotes = true
        };


    }
}
