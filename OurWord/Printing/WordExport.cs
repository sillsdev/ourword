#region ***** WordExport.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    WordExport.cs
 * Author:  John Wimbish
 * Created: 12 Mar 2010
 * Purpose: Exports a Scripture book to Microsoft Word using Open XML
 * Legal:   Copyright (c) 2005-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using OurWordData.DataModel;
using OurWordData.DataModel.Annotations;
using OurWordData.DataModel.Runs;
using OurWordData.Styles;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

// These correspond to the OpenXml prefixes
using a = DocumentFormat.OpenXml.Drawing;
using pic = DocumentFormat.OpenXml.Drawing.Pictures;
using wp = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using Color = DocumentFormat.OpenXml.Wordprocessing.Color;
using FontSize = DocumentFormat.OpenXml.Wordprocessing.FontSize;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using ParagraphProperties = DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties;
using Position = DocumentFormat.OpenXml.Wordprocessing.Position;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using RunProperties = DocumentFormat.OpenXml.Wordprocessing.RunProperties;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using TextWrappingValues = DocumentFormat.OpenXml.Wordprocessing.TextWrappingValues;
using Underline = DocumentFormat.OpenXml.Wordprocessing.Underline;
using VerticalTextAlignment = DocumentFormat.OpenXml.Wordprocessing.VerticalTextAlignment;
#endregion

namespace OurWord.Printing
{
    public class WordExport : IDisposable
    {
        // User Attrs ------------------------------------------------------------------------
        #region Attr{g}: DBook Book
        private DBook Book
        {
            get
            {
                Debug.Assert(null != m_Book);
                return m_Book;
            }
        }
        private readonly DBook m_Book;
        #endregion
        #region Attr{g}: string DocPath
        private string DocPath
        {
            get
            {
                Debug.Assert(!string.IsNullOrEmpty(m_sDocPath));
                return m_sDocPath;
            }
        }
        private readonly string m_sDocPath;
        #endregion
        public enum Target { Vernacular, BackTranslation };
        #region Attr{g}: Target WhatToExport
        Target WhatToExport
        {
            get
            {
                return m_WhatToExport;
            }
        }
        private readonly Target m_WhatToExport = Target.Vernacular;
        #endregion

        public bool ExportPictures = true;

        #region VAttr{g}: WritingSystem WritingSystem
        WritingSystem WritingSystem
        {
            get
            {
                return (WhatToExport == Target.Vernacular) ?
                    Book.Translation.WritingSystemVernacular :
                    Book.Translation.WritingSystemConsultant;
            }
        }
        #endregion
        private const char c_chNonBreakingSpace = '\u202F';

        // Public Interface ------------------------------------------------------------------
        #region Constructor(book, sPath, Target)
        public WordExport(DBook book, string sDocPath, Target target)
        {
            m_Book = book;
            m_sDocPath = sDocPath;
            m_WhatToExport = target;

            // Since these are statics, make certain they are reset to zero
            CompileFootnote.CurrentFootnoteId = 0;
            CompileComment.CurrentCommentId = 0;
            CompilePicture.Reset();

            // Create the document attributes
            m_WordDocument = WordprocessingDocument.Create(sDocPath, WordprocessingDocumentType.Document);
            var mainPart = m_WordDocument.AddMainDocumentPart();
            mainPart.Document = new Document();
            m_DocBody = new Body();
            mainPart.Document.Append(m_DocBody);

            var stylePart = mainPart.AddNewPart<StyleDefinitionsPart>();
            stylePart.Styles = new Styles();
            m_DocStyles = stylePart.Styles;

            var footnotesPart = mainPart.AddNewPart<FootnotesPart>();
            footnotesPart.Footnotes = new Footnotes();
            m_DocFootnotes = footnotesPart.Footnotes;

            var commentsPart = mainPart.AddNewPart<WordprocessingCommentsPart>();
            commentsPart.Comments = new Comments();
            m_DocComments = commentsPart.Comments;

        }
        #endregion
        #region Method: void Dispose()
        public void Dispose()
        {
            WordDocument.Dispose();
        }
        #endregion
        #region Method: void Do()
        public void Do()
        {
            CompileStyleSheet();

            var vParagraphs = GetAllParagraphs();
            foreach (var paragraph in vParagraphs)
            {
                // Skip pictures if we're supposed to not export them
                var bIsPicture = (null != paragraph as DPicture);
                if (!ExportPictures && bIsPicture)
                    continue;

                CompileParagraph(paragraph);
            }

            // Uncomment these to check for xml errors using the debugger
            // var validator = new OpenXmlValidator();
            // var result = validator.Validate(WordDocument);

            WordDocument.MainDocumentPart.Document.Save();
        }
        #endregion

        // Document Parts --------------------------------------------------------------------
        #region Attr{g}: WordprocessingDocument WordDocument
        WordprocessingDocument WordDocument
        {
            get
            {
                Debug.Assert(null != m_WordDocument);
                return m_WordDocument;
            }
        }
        private readonly WordprocessingDocument m_WordDocument;
        #endregion
        #region Attr{g}: Body DocBody
        Body DocBody
        {
            get
            {
                Debug.Assert(null != m_DocBody);
                return m_DocBody;
            }
        }
        private readonly Body m_DocBody;
        #endregion
        #region Attr{g}: Styles DocStyles
        Styles DocStyles
        {
            get
            {
                Debug.Assert(null != m_DocStyles);
                return m_DocStyles;
            }
        }
        private readonly Styles m_DocStyles;
        #endregion
        #region Attr{g}: Footnotes DocFootnotes
        Footnotes DocFootnotes
        {
            get
            {
                Debug.Assert(null != m_DocFootnotes);
                return m_DocFootnotes;
            }
        }
        private readonly Footnotes m_DocFootnotes;
        #endregion
        #region Attr{g}: Comments DocComments
        Comments DocComments
        {
            get
            {
                Debug.Assert(null != m_DocComments);
                return m_DocComments;
            }
        }
        private readonly Comments m_DocComments;
        #endregion

        // Stylesheet ------------------------------------------------------------------------
        #region Method: StyleRunProperties CompileRunProperties(CharacterStyle)
        StyleRunProperties CompileRunProperties(CharacterStyle style)
        {
            var factory = style.FindOrAddFontFactory(WritingSystem.Name);

            // Font Color
            var sColor = string.Format("{0:X2}{1:X2}{2:X2}",
                style.FontColor.R, style.FontColor.G, style.FontColor.B);

            // Font Size (specified in half-points, so multiply by two)
            var nFontSizeHalfPoints = (int) (factory.FontSize*2);
            // Adjust larger, as OurWord decreases in leu of superscript
            if (style.VerticalPosition != CharacterStyle.Position.Baseline)
                nFontSizeHalfPoints = (int)(nFontSizeHalfPoints / 0.8);

            // Build the properties object
            var properties = new StyleRunProperties
            {
                RunFonts = new RunFonts { Ascii = factory.FontName },
                Color = new Color { Val = sColor },
                FontSize = new FontSize { Val = nFontSizeHalfPoints.ToString() }
            };

            // FontStyles
            if (factory.IsBold)
                properties.Bold = new Bold();
            if (factory.IsItalic)
                properties.Italic = new Italic();
            if (factory.IsUnderline)
                properties.Underline = new Underline();
            if (factory.IsStrikeout)
                properties.Strike = new Strike();

            // Super/Subscript
            if (style.VerticalPosition == CharacterStyle.Position.Superscript)
                properties.Append(new VerticalTextAlignment { Val = VerticalPositionValues.Superscript});
            else if (style.VerticalPosition == CharacterStyle.Position.Subscript)
                properties.Append(new VerticalTextAlignment { Val = VerticalPositionValues.Subscript });

            return properties;
        }
        #endregion
        #region SMethod: StyleParagraphProperties CompileParagraphProperties(ParagraphStyle)
        static StyleParagraphProperties CompileParagraphProperties(ParagraphStyle style)
        {
            var properties = new StyleParagraphProperties();

            // Alignment
            if (style.IsLeft)
                properties.Append(new Justification { Val = JustificationValues.Left });
            else if (style.IsRight)
                properties.Append(new Justification { Val = JustificationValues.Right });
            else if (style.IsCentered)
                properties.Append(new Justification { Val = JustificationValues.Center });
            else
                properties.Append(new Justification { Val = JustificationValues.Both });

            // Spacing
            const int twipsPerPoint = 20;
            var sBefore = (twipsPerPoint * style.PointsBefore).ToString();
            var sAfter = (twipsPerPoint * style.PointsAfter).ToString();
            properties.SpacingBetweenLines = new SpacingBetweenLines {Before = sBefore, After = sAfter};

            // Indentation
            const int twipsPerInch = 1440;
            var sLeftIndent = (twipsPerInch * style.LeftMarginInches).ToString();
            var sRightIndent = (twipsPerInch * style.RightMarginInches).ToString();
            var sFirstIndent = (twipsPerInch * style.FirstLineIndentInches).ToString();
            properties.Indentation = new Indentation 
                { 
                    Left = sLeftIndent, 
                    Right = sRightIndent, 
                    Hanging = sFirstIndent 
                };

            // Keep With Next
            properties.KeepNext = new KeepNext {Val = OnOffValue.FromBoolean(style.KeepWithNextParagraph)};

            return properties;
        }
        #endregion
        #region Method: void CompileStyleSheet()
        void CompileStyleSheet()
        {
            foreach(var style in StyleSheet.StyleList)
            {
                if (style.OnlyInUserInterface)
                    continue;

                var paragraphStyle = style as ParagraphStyle;
                var styleType = (null == paragraphStyle) ? StyleValues.Character : StyleValues.Paragraph;

                var docStyle = new Style(
                    new StyleName { Val = style.StyleName }
                    ) { 
                        StyleId = style.StyleName, 
                        Type = styleType
                    };

                docStyle.Append(CompileRunProperties(style));

                if (null != paragraphStyle)
                    docStyle.StyleParagraphProperties = CompileParagraphProperties(paragraphStyle);

                DocStyles.Append(docStyle);
            }
        }
        #endregion

        // Content ---------------------------------------------------------------------------
        #region Method: void CompileInitialChapterNumber(DParagraph)
        void CompileInitialChapterNumber(DParagraph paragraph)
            // If the first run is a Chapter, we handle it as a separate frammed paragraph
            #region Example: Produced by Word for a dropdown
            /*     <w:p w:rsidR="00775FCC" w:rsidRPr="00775FCC" w:rsidRDefault="00775FCC" w:rsidP="00775FCC">
             *       <w:pPr>
             *         <w:pStyle w:val="Paragraph"/>
             *         <w:keepNext/>
             *         <w:framePr w:dropCap="drop" w:lines="3" w:wrap="around" w:vAnchor="text" w:hAnchor="text"/>
             *         <w:spacing w:after="0" w:line="842" w:lineRule="exact"/>
             *         <w:textAlignment w:val="baseline"/>
             *         <w:rPr>
             *           <w:rStyle w:val="VerseNumber"/>
             *         </w:rPr>
             *       </w:pPr>
             *       <w:r w:rsidRPr="00775FCC">
             *         <w:rPr>
             *           <w:rFonts w:hAnsi="Arial" w:cs="Arial"/>
             *           <w:position w:val="-8"/>
             *           <w:sz w:val="100"/>
             *         </w:rPr>
             *         <w:t>1</w:t>
             *       </w:r>
             *     </w:p>
             */
            #endregion
        {
            // Abort if the first run isn't a DChapter
            if (paragraph.Runs.Count == 0)
                return;
            var chapter = paragraph.Runs[0] as DChapter;
            if (null == chapter)
                return;

            // Preliminaty Calculations
            var factory = StyleSheet.ChapterNumber.FindOrAddFontFactory(WritingSystem.Name);
            const int cVerticalLines = 2;
            var LineSpacingTwips = (int)(factory.FontSize * 20);
            var nFontSizeInHalfPoints = (int)(factory.FontSize * 2);

            // Paragraph Properties
            var paragraphProperties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId { Val = paragraph.Style.StyleName },
                KeepNext = new KeepNext { Val = OnOffValue.FromBoolean(true) },
                FrameProperties = new FrameProperties
                    {
                        DropCap = DropCapLocationValues.Drop,
                        Lines = cVerticalLines,
                        Wrap = TextWrappingValues.Around,
                        HorizontalPosition = HorizontalAnchorValues.Text,
                        VerticalPosition = VerticalAnchorValues.Text
                    },
                SpacingBetweenLines = new SpacingBetweenLines 
                    { 
                        After = "0", 
                        LineRule = LineSpacingRuleValues.Exact,
                        Line = LineSpacingTwips.ToString()
                    },
                TextAlignment = new TextAlignment {Val = VerticalTextAlignmentValues.Baseline}
            };
  //          paragraphProperties.Append(new RunStyle { Val = StyleSheet.ChapterNumber.StyleName });

            // The Run containing the Text (the chapter number)
            var docRun = new Run(
                new RunProperties(
                    new RunStyle { Val = StyleSheet.ChapterNumber.StyleName }
                    ){
                        Position = new Position {Val = "-8"},
                        FontSize = new FontSize { Val = nFontSizeInHalfPoints.ToString() }
                    },
                new Text(chapter.Text)
                );

            // Owning Paragraph
            var docParagraph = new Paragraph();
            docParagraph.Append(paragraphProperties);
            docParagraph.Append(docRun);
            DocBody.Append(docParagraph);
        }
        #endregion
        #region SMethod: bool Compile(para, DVerse)
        static bool Compile(OpenXmlElement docParagraph, DVerse verse)
        {
            if (null == verse)
                return false;

            var docRun = new Run(
                new RunProperties(
                    new RunStyle { Val = StyleSheet.VerseNumber.StyleName }
                    ),
                new Text(verse.Text + c_chNonBreakingSpace)
                );
            docParagraph.Append(docRun);

            return true;
        }
        #endregion
        #region Method: bool Compile(para, DFoot)
        class CompileFootnote
        {
            #region SAttr{g/s}: int CurrentFootnoteId
            static public int CurrentFootnoteId { get; set; }
            #endregion
            #region SMethod: Run CreateReferenceMark()
            static public Run CreateReferenceMark()
                // This is the mark that appears in the body text, that refers to the footnote
                // down below.
            {
                var docRun = new Run();

                docRun.Append(new RunProperties
                {
                    RunStyle = new RunStyle { Val = StyleSheet.FootnoteLetter.StyleName }
                });

                docRun.Append(new FootnoteReference { Id = CurrentFootnoteId });

                return docRun;
            }
            #endregion
            #region SMethod: Footnote CreateFootnote(WordExport, DFootnote)
            static public Footnote CreateFootnote(WordExport export, DFootnote footnote)
            {
                var docParagraph = new Paragraph();

                // Footnote's paragraph properties
                docParagraph.Append(new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = footnote.Style.StyleName }
                });

                // Footnote's reference mark
                var docRefRun = new Run();
                docRefRun.Append(new RunProperties
                {
                    RunStyle = new RunStyle { Val = StyleSheet.FootnoteLetter.StyleName }
                });
                docRefRun.Append(new FootnoteReferenceMark());
                docParagraph.Append(docRefRun);

                // Footnote's Verse Reference string. In wellformed data we'll always have the
                // VerseReference, but some old data may be without it; thus we test.
                if (!string.IsNullOrEmpty(footnote.VerseReference))
                {
                    var docVerseRef = new Run();
                    docVerseRef.Append(new RunProperties { Italic = new Italic()});
                    docVerseRef.Append(new Text(c_chNonBreakingSpace + 
                        footnote.VerseReference + c_chNonBreakingSpace));
                    docParagraph.Append(docVerseRef);
                }

                // Footnote's contents
                foreach(DRun run in footnote.Runs)
                    export.Compile(docParagraph, run as DText);

                // Pleace the footnote paragraph into a footnote
                var docFootnote = new Footnote { Id = CurrentFootnoteId };
                docFootnote.Append(docParagraph);
                return docFootnote;
            }
            #endregion
        }

        bool Compile(OpenXmlElement docParagraph, DFoot foot)
        {
            if (null == foot)
                return false;

            // Create and Add the footnote
            var footnote = CompileFootnote.CreateFootnote(this, foot.Footnote);
            DocFootnotes.Append(footnote);

            // Create the reference to the footnote and add it to our paragraph
            var footnoteReferenceMark = CompileFootnote.CreateReferenceMark();
            docParagraph.Append(footnoteReferenceMark);

            // Ready for the next footnote
            CompileFootnote.CurrentFootnoteId++;
            return true;
        }
        #endregion
        #region Method: void Compile(para, TranslatorNote)
        class CompileComment
        {
            #region SAttr{g/s}: int CurrentCommentId
            static public int CurrentCommentId { get; set; }
            #endregion
            #region SMethod: Run CreateReference()
            static public Run CreateReference()
                // At this point our goal is just a point reference. Perhaps later we'll
                // add the commentRangeStart and commentRangeEnd.
                //
                //   <w:r>
                //       <w:commentReference w:id="0" />
                //   </w:r>
            {
                var docRun = new Run();
                docRun.Append(new CommentReference { Id = CurrentCommentId.ToString() });
                return docRun;
            }
            #endregion
            #region SMethod: Comment CreateComment(WordExport, TranslatorNote)
            static public Comment CreateComment(WordExport export, TranslatorNote note)
            /*
             *   <w:comment w:id="0" w:author="John Wimbish" w:date="2010-03-13T12:02:00Z" w:initials="JSW">
             *     <w:p w:rsidR="00831DA3" w:rsidRDefault="00831DA3">
             *     <w:pPr>
             *       <w:pStyle w:val="CommentText"/>
             *     </w:pPr>
             *     <w:r>
             *       <w:rPr>
             *         <w:rStyle w:val="CommentReference"/>
             *       </w:rPr>
             *       <w:annotationRef/>
             *     </w:r>
             *     <w:r>
             *       <w:t>This is a comment.</w:t>
             *     </w:r>
             *   </w:p>
             *   </w:comment>
             */
            {
                var docParagraph = new Paragraph();

                // Comments's paragraph properties
                var paragraphProperties = new ParagraphProperties 
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = "CommentText" }
                };
                docParagraph.Append(paragraphProperties);

                // Comment's reference mark
                var runRef = new Run();
                runRef.Append(new RunProperties { RunStyle = new RunStyle { Val = "CommentReference" } });
                runRef.Append(new AnnotationReferenceMark());
                docParagraph.Append(runRef);

                // Comment's contents
                foreach (DRun run in note.FirstMessage.Runs)
                    export.Compile(docParagraph, run as DText);

                // Place the paragraph into a comment
                var comment = new Comment 
                { 
                    Id = CurrentCommentId.ToString(),
                    Author = note.FirstMessage.Author,
                    Date = note.FirstMessage.UtcCreated
                };
                comment.Append(docParagraph);
                return comment;
            }
            #endregion
        }
        void Compile(OpenXmlElement docParagraph, TranslatorNote note)
        {
            // Create and add the Comment
            var comment = CompileComment.CreateComment(this, note);
            DocComments.Append(comment);

            // Create the reference to the comment and add it to our paragraph
            var commentReference = CompileComment.CreateReference();
            docParagraph.Append(commentReference);

            // Ready for the next comment
            CompileComment.CurrentCommentId++;
        }
        #endregion
        #region Method: bool Compile(para, DText)
        bool Compile(OpenXmlElement docParagraph, DText text)
        {
            if (null == text)
                return false;

            // Determine which of the text's phrases to show
            var phrases = (WhatToExport == Target.Vernacular) ?
                text.Phrases :
                text.PhrasesBT;

            // In some cases, the answer is the Vernacular anyway
            // 1. Cross reference paragraphs
            var style = text.Paragraph.Style;
            if (style == StyleSheet.SectionCrossReference ||
                style == StyleSheet.MajorSectionCrossReference)
            {
                phrases = text.Phrases;
            }
            // 2. SeeAlso footnotes
            var footnote = text.Paragraph as DFootnote;
            if (footnote != null && footnote.IsSeeAlso)
                phrases = text.Phrases;
            // 3. Annotations
            var message = text.Paragraph as DMessage;
            if (message != null && string.IsNullOrEmpty(phrases.AsString))
                phrases = text.Phrases;

            // Get the text's FontFactory, so that we know how to toggle styles
            var factory = text.Paragraph.Style.FindOrAddFontFactory(WritingSystem.Name);

            foreach(DPhrase phrase in phrases)
            {
                var docRun = new Run();

                // Adjust if this phrase has styles that are toggled from the FontFactory's style
                if (phrase.FontToggles != FontStyle.Regular)
                {
                    var properties = new RunProperties();

                    if (phrase.BoldIsToggled)
                        properties.Bold = new Bold {Val = OnOffValue.FromBoolean(!factory.IsBold)};

                    if (phrase.ItalicIsToggled)
                        properties.Italic = new Italic { Val = OnOffValue.FromBoolean(!factory.IsItalic) };

                    if (phrase.UnderlineIsToggled)
                    {
                        properties.Underline = new Underline
                        {
                            Val = (factory.IsUnderline) ?
                                UnderlineValues.None :
                                UnderlineValues.Single
                        };
                    }

                    if (phrase.StrikeoutIsToggled)
                        properties.Strike = new Strike {Val = OnOffValue.FromBoolean(!factory.IsStrikeout)};

                    docRun.Append(properties);
                }

                // Add the text to the run
                docRun.Append(new Text(phrase.Text));

                // Add the run to the paragraph
                docParagraph.Append(docRun);

                // Notes
                foreach(TranslatorNote note in text.TranslatorNotes)
                {
                    if (note.Status == Role.Closed)
                        continue;

                    Compile(docParagraph, note);
                }
            }

            return true;
        }
        #endregion
        #region Method: void CompileParagraph(DParagraph)
        void CompileParagraph(DParagraph paragraph)
        {
            // If the first run is a Chapter, we handle it as a separate framed paragraph
            CompileInitialChapterNumber(paragraph);

            // Create a styled paragraph
            var properties = new ParagraphProperties
            {
                ParagraphStyleId = new ParagraphStyleId {Val = paragraph.Style.StyleName}
            };
            var docParagraph = new Paragraph();
            docParagraph.Append(properties);

            // If a picture, we want to load it inline followed by a breaking space
            if (null != paragraph as DPicture)
            {
                var runPicture = Compile(paragraph as DPicture);
                if (null != runPicture)
                    docParagraph.Append(runPicture);
            }

            // Load its contents
            foreach(var run in paragraph.Runs)
            {
                if (Compile(docParagraph, run as DText))
                    continue;
                if (Compile(docParagraph, run as DVerse))
                    continue;
                if (Compile(docParagraph, run as DFoot))
                    continue;
            }

            DocBody.Append(docParagraph);
        }
        #endregion
        #region Method: IEnumerable<DParagraph> GetAllParagraphs()
        IEnumerable<DParagraph> GetAllParagraphs()
        {
            var v = new List<DParagraph>();

            foreach (DSection section in Book.Sections)
            {
                foreach (DParagraph paragraph in section.Paragraphs)
                    v.Add(paragraph);
            }

            return v;
        }
        #endregion
        #region Method: Compile(picture)
        #region CLASS: CompilePicture
        class CompilePicture
        {
            private string m_sImageId;
            static private uint s_Id;
            private readonly WordExport m_Export;
            private readonly string m_sImagePath;
            #region VAttr{g}: string FileName
            string FileName
            {
                get
                {
                    return Path.GetFileName(m_sImagePath);
                }
            }
            #endregion

            #region Constructor(WordExport, DPicture picture)
            public CompilePicture(WordExport export, DPicture picture)
            {
                m_Export = export;
                m_sImagePath = picture.FullPathName;
            }
            #endregion
            #region SMethod: void Reset()
            static public void Reset()
            {
                s_Id = 1;
            }
            #endregion
            #region Method: Run Do()
            public Run Do()
            {
                var imagePart = AddImagePart();

                long imageWidthEmu = 0;
                long imageHeightEmu = 0;
                GenerateImagePart(imagePart, ref imageWidthEmu, ref imageHeightEmu);

                var run = GenerateMainDocumentPart(imageWidthEmu, imageHeightEmu);

                s_Id++;

                return run;
            }
            #endregion

            #region Method: ImagePart AddImagePart()
            ImagePart AddImagePart()
            {
                var mainPart = m_Export.WordDocument.MainDocumentPart;

                var imageType = ImagePartType.Jpeg;
                var sExtension = Path.GetExtension(m_sImagePath).ToLowerInvariant();
                switch (sExtension)
                {
                    case ".gif":
                        imageType = ImagePartType.Gif;
                        break;
                    case ".png":
                        imageType = ImagePartType.Png;
                        break;
                    case ".bmp":
                        imageType = ImagePartType.Bmp;
                        break;
                    case ".jpeg":
                    case ".jpg":
                        imageType = ImagePartType.Jpeg;
                        break;
                    case ".pcx":
                        imageType = ImagePartType.Pcx;
                        break;
                    case ".tif":
                    case ".tiff":
                        imageType = ImagePartType.Tiff;
                        break;
                }

                var imagePart = mainPart.AddImagePart(imageType);
                m_sImageId = mainPart.GetIdOfPart(imagePart);

                return imagePart;
            }
            #endregion
            #region Method: GenerateImagePart(ImagePart, ref imageWidthEmu, ref ImageHeightEmu)
            void GenerateImagePart(ImagePart imagePart,
                ref long imageWidthEmu, ref long imageHeightEmu)
            {
                byte[] imageFileBytes;
                Bitmap imageFile;

                // Open a stream on the image file and read it's contents.
                using (var fsImageFile = File.OpenRead(m_sImagePath))
                {
                    imageFileBytes = new byte[fsImageFile.Length];
                    fsImageFile.Read(imageFileBytes, 0, imageFileBytes.Length);

                    imageFile = new Bitmap(fsImageFile);
                }

                // Get the dimensions of the image in English Metric Units (EMU)
                // for use when adding the markup for the image to the document.
                imageWidthEmu = (long)(
                  (imageFile.Width / imageFile.HorizontalResolution) * 914400L);
                imageHeightEmu = (long)(
                  (imageFile.Height / imageFile.VerticalResolution) * 914400L);

                // Write the contents of the image to the ImagePart.
                using (var writer = new BinaryWriter(imagePart.GetStream()))
                {
                    writer.Write(imageFileBytes);
                    writer.Flush();
                }
            }
            #endregion

            #region Method: pic.Picture GenerateDocPicture(imageWidthEmu, imageHeightEmu)
            pic.Picture GenerateDocPicture(long imageWidthEmu, long imageHeightEmu)
            {
                var pic = new pic.Picture(
                    new pic.NonVisualPictureProperties(
                        new pic.NonVisualDrawingProperties { Id = 0U, Name = FileName },
                        new pic.NonVisualPictureDrawingProperties()
                        ),

                    new pic.BlipFill(
                        new a.Blip
                            {
                            Embed = m_sImageId,
                            CompressionState = a.BlipCompressionValues.Print
                        },
                        new a.Stretch(new a.FillRectangle())
                        ),

                    new pic.ShapeProperties(
                        new a.Transform2D(
                            new a.Offset { X = 0L, Y = 0L },
                            new a.Extents { Cx = imageWidthEmu, Cy = imageHeightEmu }
                            ),
                        new a.PresetGeometry(
                            new a.AdjustValueList()
                            ) { Preset = a.ShapeTypeValues.Rectangle }
                        )
                    );
                return pic;
            }
            #endregion
            #region Method: Drawing GenerateDocDrawing(imageWidthEmu, imageHeightEmu, pic.Picture)
            Drawing GenerateDocDrawing(long imageWidthEmu, long imageHeightEmu, pic.Picture pic)
            {
                const string graphicDataUri = "http://schemas.openxmlformats.org/drawingml/2006/picture";

                var sDocPropName = string.Format("Picture {0}", s_Id - 1);

                var drawing = new Drawing(
                    new wp.Inline(
                        new wp.Extent { Cx = imageWidthEmu, Cy = imageHeightEmu },
                        new wp.EffectExtent
                            {
                                LeftEdge = 19050,
                                TopEdge = 0,
                                RightEdge = 0,
                                BottomEdge = 0
                            },
                        new wp.DocProperties
                            {
                                Id = s_Id,             // e.g., "1"
                                Name = sDocPropName,   // e.g., "Picture 0"
                                Description = FileName // e.g., "CN01600b.tif"
                            },
                        new wp.NonVisualGraphicFrameDrawingProperties(
                            new a.GraphicFrameLocks {NoChangeAspect = true}
                            ),
                        new a.Graphic(
                            new a.GraphicData(pic) { Uri = graphicDataUri }
                            )
                        )
                        {
                            DistanceFromTop = 0U,
                            DistanceFromBottom = 0U,
                            DistanceFromLeft = 0U,
                            DistanceFromRight = 0U,
                        }
                    );

                return drawing;
            }
            #endregion
            #region Method: GenerateMainDocumentPart(imageWidthEmu, imageHeightEmu)
            Run GenerateMainDocumentPart(long imageWidthEmu, long imageHeightEmu)
            {
                // The Picture
                var pic = GenerateDocPicture(imageWidthEmu, imageHeightEmu);

                // The picture is contained in a Drawing object
                var drawing = GenerateDocDrawing(imageWidthEmu, imageHeightEmu, pic);

                // Place in a run
                var run = new Run(
                    new RunProperties(new NoProof()),
                    drawing
                    );

                return run;
            }
            #endregion

        }
        #endregion

        Run Compile(DPicture picture)
        {
            Run run;

            if (!File.Exists(picture.FullPathName))
            {
                run = new Run(new Text("Missing picture <" + picture.FullPathName + ">"));
            }
            else if (".pcx" == Path.GetExtension(picture.FullPathName).ToLowerInvariant())
            {
                run = new Run(new Text("Unable to handle picture of format PCX: <" +
                    picture.FullPathName + ">"));
            }
            else
            {
                run = (new CompilePicture(this, picture)).Do();
            }

            // If the picture has a caption, then add a break, so the caption will appear 
            // underneath the picture rather than beside it
            if (WhatToExport == Target.Vernacular && !string.IsNullOrEmpty(picture.AsString))
                run.Append(new Break());
            if (WhatToExport == Target.BackTranslation && !string.IsNullOrEmpty(picture.ProseBTAsString))
                run.Append(new Break());

            return run;
        }
        #endregion
    }
}
