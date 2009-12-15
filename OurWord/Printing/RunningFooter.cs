using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using OurWord.Edit;
using OurWordData;
using OurWordData.DataModel;

namespace OurWord.Printing
{
    public class RunningFooter : ERowOfColumns
    {
        // Content ---------------------------------------------------------------------------
        #region Method: void SetColumnText(int iColumn, string sText)
        void SetColumnText(int iColumn, string sText)
        {
            Debug.Assert(0 >= iColumn && iColumn < NumberOfColumns);
            var owp = new OWPara(WritingSystem, ParagraphStyle, sText);
            var column = GetColumn(iColumn);
            column.Clear();
            column.Append(owp);
        }
        #endregion
        #region Method: void SetPageNumber(int iColumn, int nNumber)
        public void SetPageNumber(int iColumn, int nNumber)
        {
            var sText = string.Format("- {0} -", nNumber);
            SetColumnText(iColumn, sText);

        }
        #endregion
        #region Method: void SetCopyrightNotice(int iColumn)
        private void SetCopyrightNotice(int iColumn)
        {
            var sCopyright = DB.TeamSettings.CopyrightNotice;
            if (DB.TargetBook.HasCopyrightNotice)
                sCopyright = DB.TargetBook.Copyright;

            if (!string.IsNullOrEmpty(sCopyright))
                SetColumnText(iColumn, sCopyright);
        }
        #endregion
        #region Method: void SetStageAndDate(int iColumn)
        private void SetStageAndDate(int iColumn)
        {
            var sStatus = DB.TargetBook.Stage.LocalizedName;
            var sDate = DateTime.Today.ToShortDateString();
            var sText = string.Format("{0} - {1}", sStatus, sDate);
            SetColumnText(iColumn, sText);
        }
        #endregion
        #region Method:  void SetLanguageNameStageAndDate(int iColumn)
        private void SetLanguageNameStageAndDate(int iColumn)
        {
            var sLanguageName = DB.TargetBook.Translation.DisplayName;
            var sStatus = DB.TargetBook.Stage.LocalizedName;
            var sDate = DateTime.Today.ToShortDateString();
            var sText = string.Format("{0} {1} - {2}", sLanguageName, sStatus, sDate);
            SetColumnText(iColumn, sText);
        }
        #endregion
        public void SetScriptureReference(int iColumn, int nChapter, int nVerse)
        {
            var sText = string.Format("{0}:{1}", nChapter, nVerse);
            SetColumnText(iColumn, sText);
        }

        // Context ---------------------------------------------------------------------------
        #region SVAttr{g}: JParagraphStyle ParagraphStyle
        static JParagraphStyle ParagraphStyle
        {
            get
            {
                return DB.StyleSheet.FindParagraphStyleOrNormal(DStyleSheet.c_sfmRunningHeader);
            }
        }
        #endregion
        #region SVAttr{g}: JWritingSystem WritingSystem
        static JWritingSystem WritingSystem
        {
            get
            {
                return DB.TargetTranslation.WritingSystemVernacular;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        private const int NumberOfColumns = 3;
        #region Constructor(nPageNumber)
        public RunningFooter(int nPageNumber)
            : base(NumberOfColumns)
        {
            var vContent = new List<DTeamSettings.FooterParts>
            {
                (IsEvenPage(nPageNumber)) ? DB.TeamSettings.EvenLeft : DB.TeamSettings.OddLeft,
                (IsEvenPage(nPageNumber)) ? DB.TeamSettings.EvenMiddle : DB.TeamSettings.OddMiddle,
                (IsEvenPage(nPageNumber)) ? DB.TeamSettings.EvenRight : DB.TeamSettings.OddRight
            };

            for (var iColumn = 0; iColumn < vContent.Count; iColumn++)
            {
                switch (vContent[iColumn])
                {
                   case DTeamSettings.FooterParts.kBlank:
                        SetColumnText(iColumn, "");
                        break;

                   case DTeamSettings.FooterParts.kPageNumber:
                        SetPageNumber(iColumn, nPageNumber);
                        break;

                   case DTeamSettings.FooterParts.kCopyrightNotice:
                        SetCopyrightNotice(iColumn);
                        break;

                   case DTeamSettings.FooterParts.kStageAndDate:
                        SetStageAndDate(iColumn);
                        break;

                   case DTeamSettings.FooterParts.kLanguageStageAndDate:
                        SetLanguageNameStageAndDate(iColumn);
                        break;

                   case DTeamSettings.FooterParts.kScriptureReference:
                        break;
                }
            }
        }
        #endregion
        #region SMethod: bool IsEvenPage(int nPageNumber)
        static bool IsEvenPage(int nPageNumber)
            // Divide the page number in half, any remainder (e.g., as will be true
            // with an odd number) will be dropped
        {
            var nHalf = nPageNumber / 2;
            return (nHalf * 2 == nPageNumber);
        }
        #endregion
    }
}
