using System;
using System.Diagnostics;
using System.IO;
using JWTools;
using OurWord.Printing;
using OurWordData.DataModel;

namespace OurWord.Dialogs.Export
{
    public class ExportMethod
    {
        // Subclass must override
        #region VirtAttr{g}: string Name
        protected virtual string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        #endregion
        #region VirtAttr{g}: string FileExtension
        public virtual string FileExtension
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        #endregion
        #region VirtMethod: void DoExport(book)
        virtual public void DoExport(DBook book)
        {
            throw new NotImplementedException();
        }
        #endregion

        // Methods
        #region Method: string GetSubFolder()
        public string GetSubFolder()
        {
            var sTranslation = Path.Combine("OurWordExport", m_Translation.DisplayName);
            return Path.Combine(sTranslation, Name);
        }
        #endregion
        #region Method: string GetPathToBook(DBook book)
        protected string GetPathToBook(DBook book)
        {
            // Compute filename
            Debug.Assert(FileExtension.StartsWith("."));
            var sFileName = book.BaseName + FileExtension;

            // Compute folder
            var sExportfolder = JWU.GetMyDocumentsFolder(GetSubFolder());
            if (!Directory.Exists(sExportfolder))
                Directory.CreateDirectory(sExportfolder);

            // Put 'em together
            return Path.Combine(sExportfolder, sFileName);
        }
        #endregion

        // Scaffolding
        private readonly DTranslation m_Translation;
        #region constructor(DTranslation)
        protected ExportMethod(DTranslation translation)
        {
            m_Translation = translation;
        }
        #endregion
    }

    public class ExportToParatext : ExportMethod
    {
        #region OAttr{g}: string Name
        protected override string Name
        {
            get
            {
                return "Paratext";
            }
        }
        #endregion
        #region OAttr{g}: string FileExtension
        public override string FileExtension
        {
            get
            {
                return ".ptx";
            }
        }
        #endregion
        #region OMethod: void DoExport(book)
        override public void DoExport(DBook book)
        {
            book.ExportToParatext(GetPathToBook(book));
        }
        #endregion

        #region Constructor(translation)
        public ExportToParatext(DTranslation translation)
            : base(translation)
        {
        }
        #endregion
    }

    public class ExportToGoBible : ExportMethod
    {
        #region OAttr{g}: string Name
        protected override string Name
        {
            get
            {
                return "GoBibleCreator";
            }
        }
        #endregion
        #region OAttr{g}: string FileExtension
        public override string FileExtension
        {
            get
            {
                return ".GoBible.ptx";
            }
        }
        #endregion
        #region OMethod: void DoExport(book)
        override public void DoExport(DBook book)
        {
            book.ExportToGoBible(GetPathToBook(book));
        }
        #endregion

        #region Constructor(translation)
        public ExportToGoBible(DTranslation translation)
            : base(translation)
        {
        }
        #endregion    
    }

    public class ExportToToolbox : ExportMethod
    {
        #region OAttr{g}: string Name
        protected override string Name
        {
            get
            {
                return "Toolbox";
            }
        }
        #endregion
        #region OAttr{g}: string FileExtension
        public override string FileExtension
        {
            get
            {
                return ".db";
            }
        }
        #endregion
        #region OMethod: void DoExport(book)
        override public void DoExport(DBook book)
        {
            book.ExportToToolbox(GetPathToBook(book));
        }
        #endregion

        #region Constructor(translation)
        public ExportToToolbox(DTranslation translation)
            : base(translation)
        {
        }
        #endregion  
    }

    public class ExportToWord : ExportMethod
    {
        public bool ExportBackTranslation;

        #region OAttr{g}: string Name
        protected override string Name
        {
            get
            {
                return "Word 2007";
            }
        }
        #endregion
        #region OAttr{g}: string FileExtension
        public override string FileExtension
        {
            get
            {
                return (ExportBackTranslation) ? ".bt.docx" : ".docx";
            }
        }
        #endregion
        #region OMethod: void DoExport(book)
        override public void DoExport(DBook book)
        {
            var sPath = GetPathToBook(book);

            var whatToExport = (ExportBackTranslation) ?
                WordExport.Target.BackTranslation :
                WordExport.Target.Vernacular;

            using (var export = new WordExport(book, sPath, whatToExport))
                export.Do();
        }
        #endregion

        #region Constructor(translation)
        public ExportToWord(DTranslation translation)
            : base(translation)
        {
        }
        #endregion  
    }
}
