/**********************************************************************************************
 * Project: Our Word!
 * File:    DCluster.cs
 * Author:  John Wimbish
 * Created: 30 Aug 2004
 * Purpose: Clusters together the rows for the various views, e.g., given a Front translation's
 *          paragraph, it keeps track of which Target paragraph(s) go with it.
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.IO;

using JWTools;
using JWdb;

using OurWord.View;
#endregion

namespace OurWord.DataModel
{
	#region CLASS Hvo - Issues hvo's for different data types
	public class Hvo
	{
		#region Constants & Statics
		public const int c_Span = 1000;

		public const int c_baseCluster          = 1000;
		public const int c_baseFrontPara        = c_Span *  2;  // 2000
		public const int c_baseTargetPara       = c_Span *  3;  // 3000
		public const int c_baseFootnote         = c_Span *  4;  // 4000
		public const int c_baseTargetFootnote   = c_Span *  5;  // 5000
		public const int c_baseRelatedLanguages = c_Span *  6;  // 6000
		public const int c_baseNote             = c_Span *  7;  // 7000
		public const int c_baseIntBT            = c_Span *  8;  // 8000
		public const int c_baseRelLangPara      = c_Span *  9;  // 9000
		public const int c_baseString           = c_Span * 10;  // 10,000
		public const int c_baseDRun             = c_Span * 11;  // 11,000
		public const int c_baseBTPhrases        = 20000;

		static int s_nCluster          = c_baseCluster;
		static int s_nFrontPara        = c_baseFrontPara;
		static int s_nTargetPara       = c_baseTargetPara;
		static int s_nFootnote         = c_baseFootnote;
		static int s_nTargetFootnote   = c_baseTargetFootnote;
		static int s_nRelatedLanguages = c_baseRelatedLanguages;
		static int s_nNote             = c_baseNote;
		static int s_nIntBT            = c_baseIntBT;
		static int s_nRelLangParas     = c_baseRelLangPara;
		static int s_nString           = c_baseString;
		static int s_nDRun             = c_baseDRun;
		static int s_nBTPhrase         = c_baseBTPhrases;
		#endregion

		#region Method: int GetTargetParagraphIndex(int hvo)
		static public int GetTargetParagraphIndex(int hvo)
		{
			if (hvo >= c_baseTargetPara && hvo < c_baseTargetPara + c_Span)
				return hvo - c_baseTargetPara;
			return -1;
		}
		#endregion

		#region Attr{g}: int NextCluster
		static public int NextCluster
		{
			get
			{
				return s_nCluster++;
			}
		}
		#endregion
		#region Attr{g}: int NextFrontPara
		static public int NextFrontPara
		{
			get
			{
				return s_nFrontPara++;
			}
		}
		#endregion
		#region Attr{g}: int NextTargetPara
		static public int NextTargetPara
		{
			get
			{
				return s_nTargetPara++;
			}
		}
		#endregion
		#region Attr{g}: int NextFootnote
		static public int NextFootnote
		{
			get
			{
				return s_nFootnote++;
			}
		}
		#endregion
		#region Attr{g}: int NextTargetFootnote
		static public int NextTargetFootnote
		{
			get
			{
				return s_nTargetFootnote++;
			}
		}
		#endregion
		#region Attr{g}: int NextRelatedLanguage
		static public int NextRelatedLanguage
		{
			get
			{
				return s_nRelatedLanguages++;
			}
		}
		#endregion
		#region Attr{g}: int NextRelLangPara
		static public int NextRelLangPara
		{
			get
			{
				return s_nRelLangParas++;
			}
		}
		#endregion
		#region Attr{g}: int NextNote
		static public int NextNote
		{
			get
			{
				return s_nNote++;
			}
		}
		#endregion
		#region Attr{g}: int NextIntBT
		static public int NextIntBT
		{
			get
			{
				return s_nIntBT++;
			}
		}
		#endregion
		#region Attr{g}: int NextString
		static public int NextString
		{
			get
			{
				return s_nString++;
			}
		}
		#endregion
		#region Attr{g}: int NextDRun
		static public int NextDRun
		{
			get
			{
				return s_nDRun++;
			}
		}
		#endregion
		#region Attr{g}: int NextBTPhrase
		static public int NextBTPhrase
		{
			get
			{
				return s_nBTPhrase++;
			}
		}
		#endregion

		#region Method: void Reset()
		static public void Reset()
		{
			s_nCluster          = c_baseCluster;
			s_nFrontPara        = c_baseFrontPara;
			s_nTargetPara       = c_baseTargetPara;
			s_nFootnote         = c_baseFootnote;
			s_nTargetFootnote   = c_baseTargetFootnote;
			s_nRelatedLanguages = c_baseRelatedLanguages;
			s_nNote             = c_baseNote;
			s_nIntBT            = c_baseIntBT;
			s_nRelLangParas     = c_baseRelLangPara;
			s_nString           = c_baseString;
			s_nDRun             = c_baseDRun;
			s_nBTPhrase         = c_baseBTPhrases;
		}
		#endregion
	}
	#endregion

	#region CLASS DCluster
	public class DCluster
	{
		#region Attr{g}: int H - the cluster's hvo
		public int H
		{
			get
			{
				return m_hvo;
			}
		}
		int m_hvo = 0;
		#endregion
		#region Attr{g}: bool CanSynchronize - T if front and target para's match
		public bool CanSynchronize
		{
			get
			{
				return m_bCanSynchronize;
			}
		}
		private bool m_bCanSynchronize = false;
		#endregion
		#region Attr{g}: DParagraph[] PFronts - the front paragraph(s) in this cluster
		public DParagraph[] PFronts
		{
			get
			{
				Debug.Assert(null != m_vFronts);
				return m_vFronts;
			}
		}
		private DParagraph[] m_vFronts;
		#endregion
		#region Attr{g}: DParagraph[] PTargets - the target paragraph(s) in this cluster
		public DParagraph[] PTargets
		{
			get
			{
				Debug.Assert(null != m_vTargets);
				return m_vTargets;
			}
		}
		private DParagraph[] m_vTargets;
		#endregion

		// Derived attrs ---------------------------------------------------------------------
		#region Method: int GetIndexOfTargetParagraph(DParagraph pTarget)
		public int GetIndexOfTargetParagraph(DParagraph pTarget)
		{
			int i = 0;
			foreach(DParagraph p in PTargets)
			{
				if (p == pTarget)
					return i;
				i++;
			}
			return 0;
		}
		#endregion
		#region Attr{g}: bool FrontsHaveItalics - T if any of the Front paragraphs has italics
		bool FrontsHaveItalics
		{
			get
			{
				bool bHasItalics = false;

				foreach(DParagraph p in PFronts)
				{
					if (p.HasItalics)
						bHasItalics = true;
				}

				return bHasItalics;
			}
		}
		#endregion

		// Constructors ----------------------------------------------------------------------
		#region Constructor(DParagraph pFront, DParagraph pTarget, _CanSynchronize)
		public DCluster(DParagraph pFront, DParagraph pTarget, bool _CanSynchronize)
			: this(_CanSynchronize)
		{
			// Place the paragraphs into their respective arrays
			m_vFronts = new DParagraph[1];
			m_vFronts[0] = pFront;
			m_vTargets = new DParagraph[1];
			m_vTargets[0] = pTarget;

			// Determine the cluster type
//			m_Type = DCluster.GetParagraphType( PFronts[0] );

			// Related Languages
			//			BuildRelatedLanguages(pFront.Section);
		}
		#endregion
		#region Constructor(JOwnSeq vFront, JOwnSeq vTarget, _CanSynchronize)
		public DCluster(JOwnSeq vFront, JOwnSeq vTarget, bool _CanSynchronize)
			: this(_CanSynchronize)
		{
			// Front paragraphs
			m_vFronts = new DParagraph[ vFront.Count ];
			for(int i=0; i < vFront.Count; i++)
				m_vFronts[i] = vFront[i] as DParagraph;

			// Target paragraphs
			m_vTargets = new DParagraph[ vTarget.Count ];
			for(int i=0; i < vTarget.Count; i++)
				m_vTargets[i] = vTarget[i] as DParagraph;

			// We'll base the cluster type on the first front paragraph
//			if (0 == vFront.Count)
//				m_Type = DCluster.GetParagraphType( PTargets[0] );
//			else
//				m_Type = DCluster.GetParagraphType( PFronts[0] );
//
			// Related Languages
			//			BuildRelatedLanguages(PFronts[0].Section);
		}
		#endregion
		#region Constructor(vFront, iFront, cFront, vTarget, iTarget, cTarget, _CanSynchronize)
		public DCluster(JOwnSeq vFront,  int iFront,  int cFront,
			JOwnSeq vTarget, int iTarget, int cTarget, bool _CanSynchronize)
			: this(_CanSynchronize)
		{
			// Front paragraphs
			m_vFronts = new DParagraph[cFront];
			for(int i=0; i<cFront; i++)
				m_vFronts[i] = vFront[iFront + i] as DParagraph;

			// Target paragraphs
			m_vTargets = new DParagraph[cTarget];
			for(int i=0; i<cTarget; i++)
				m_vTargets[i] = vTarget[iTarget + i] as DParagraph;

			// We base he cluster type on the first front paragraph
//			m_Type = DCluster.GetParagraphType( PFronts[0] );

			// Related Languages
			//			BuildRelatedLanguages(PFronts[0].Section);
		}
		#endregion
		#region Constructor(bool _CanSynchronize) - Base constructor
		private DCluster(bool _CanSynchronize)
		{
			m_bCanSynchronize = _CanSynchronize;
		}
		#endregion


		// Cache Operations ------------------------------------------------------------------
		#region Method: void LoadIntoCache(int hvoCluster)
		public void LoadIntoCache(int hvoCluster)
		{
            /***
			m_hvo = hvoCluster;

			// Load the Front Paragraphs
			int[] hvosFront = new int[ PFronts.Length ];
			for(int k=0; k<PFronts.Length; k++)
			{
				hvosFront[k] = Hvo.NextFrontPara;
//				ParagraphToCache(PFronts[k], hvosFront[k]);
			}
//			JCache.CacheVecProp(hvoCluster, AFrontParas, hvosFront);

			// Is this a picture?
//			JCache.SetProp(hvoCluster, AHasPicture, IsPicture);
//			if (IsPicture)
//			{
//				JCache.SetProp(hvoCluster, APicturePathname,
//					(PFronts[0] as DPicture).PathName);

//				bool bPictureHasCaption = (PFronts[0].SimpleText.Length != 0);
//				JCache.SetProp(hvoCluster, APictureHasCaption, bPictureHasCaption);
//			}

			// Does the cluster have italics?
//			JCache.SetProp(hvoCluster, AHasItalics, FrontsHaveItalics);

			// Load the Target Paragraph(s) (or footnotes, depending on type of cluster)
			int[] hvosTarget = new int[ PTargets.Length ];
			int i = 0;
			foreach(DParagraph pTarget in PTargets)
			{
				Debug.Assert(null != pTarget.Owner);

				int hvoTarget = (IsFootnote) ? Hvo.NextTargetFootnote : Hvo.NextTargetPara;
				hvosTarget[i++] = hvoTarget;

				// Load the hvo of the Cluster containing this target paragraph, so
				// we can get back to it if needed.
//				JCache.SetProp(hvoTarget, AOwningCluster, hvoCluster);

				// Load the target paragraph
				if (CanSynchronize)
					pTarget.SynchRunsToModelParagraph(PFronts[0]);
				else
					pTarget.BestGuessAtInsertingTextPositions();
//				ParagraphToCache(pTarget, hvoTarget);
			}
//			JCache.CacheVecProp(hvoCluster, ATargetParas, hvosTarget);

			// Load the Related Languages
			_LoadRelatedLanguages(hvoCluster);

            ***/
		}
		#endregion

		// Related Languages -----------------------------------------------------------------
		#region Method: DReferenceSpan _GetClusterReferenceSpan()
		DReferenceSpan _GetClusterReferenceSpan()
		{
			DReferenceSpan span = new DReferenceSpan();

			foreach( DParagraph p in PFronts )
			{
				// The start values are those of the first paragraph that is non-zero
				if (span.Start.Chapter == 0)
					span.Start.Chapter = p.ChapterI;
				if (span.Start.Verse == 0)
					span.Start.Verse = p.VerseI;

				// The final values are those of the final paragraph
				span.End.Chapter = p.ChapterF;
				span.End.Verse   = p.VerseF;
			}
		
			return span;
		}
		#endregion
		#region Method: DParagraph[] _GetReferenceLanguageParagraphs(DTranslation t)
		DParagraph[] _GetReferenceLanguageParagraphs(DTranslation t)
		{
			// We'll place the answer here
			DParagraph[] vp = new DParagraph[0];

			// Retrieve the Chapter:Verse span that is covered in this section
			DReferenceSpan span = _GetClusterReferenceSpan();

			// Get the book from the reference translation
			string sBookAbbrev = G.Project.Nav.BookAbbrev;
			DBook book = G.Project.Nav.GetLoadedBook(t, sBookAbbrev);
			if (null == book)
				return vp;

			// Look at each section in this book for those which include the
			// <span> within.
			foreach(DSection section in book.Sections)
			{
				// If the end Chapter:Verse of the section is smaller than the
				// start C:V of the Cluster, then skip this section.
				if (section.ReferenceSpan.End < span.Start)
					continue;

				// If the start C:V of the section is larger than the end C:V
				// of this Cluster, then we're done looking.
				if (section.ReferenceSpan.Start > span.End)
					break;

				// So if we are here, then we have a section that has one or more
				// verses in it that we are interested in. So retrieve those
				// paragraphs.
				DParagraph[] v = section.GetParagraphs(span);

				// We need to add this to the <vp> destination
				if (null != v && v.Length > 0 )
				{
					DParagraph[] vTemp = new DParagraph[ vp.Length + v.Length ];
					int i = 0;
					for(; i<vp.Length; i++)
						vTemp[i] = vp[i];
					for(int k=0; i<v.Length; i++,k++)
						vTemp[i] = v[k];
					vp = vTemp;
				}
			}

			// The caller is expecting null rather than an empty array
			if (vp.Length == 0)
				return null;

			return vp;
		}
		#endregion
		#region Method: DParagraph[] _GetOtherLanguageParagraphs(DTranslation t)
		DParagraph[] _GetOtherLanguageParagraphs(DTranslation t)
		{
            /***
			// Retrieve the appropriate Other Translation section
			DSection SSibling = G.Project.Nav.GetSection(t);
			if (null == SSibling)
				return null;

			// If we are dealing with a footnote, we just return all of the footnotes, as
			// we have no way to connect these up.
			if (IsFootnote)
			{
				DParagraph[] vParas = new DParagraph[ SSibling.Footnotes.Count ];
				for(int i=0; i<  SSibling.Footnotes.Count; i++)
					vParas[i] = SSibling.Footnotes[i] as DParagraph;
				return vParas;
			}

			// Retrieve the verse references that are contained in the Front's paragraph(s)
			int nVerseI   = -1;
			int nVerseF   = -1;
			foreach(DParagraph p in PFronts)
			{
				if (-1 == nVerseI)
					nVerseI = p.VerseI;
				nVerseF = p.VerseF;
			}
			if (-1 == nVerseI || -1 == nVerseF)
				return null;

			// Get a series of paragraphs; one per verse, from that section.
			return SSibling.GetParagraphsForVerses(nVerseI, nVerseF);
            ***/ return null;
		}
		#endregion
		#region Method: void _LoadRelatedLanguages(int hvoCluster)
		void _LoadRelatedLanguages(int hvoCluster)
		{
			ArrayList v = new ArrayList();

			// Loop through the Siblings and the RelatedLanguages
            foreach (DTranslation t in G.Project.AllRelatedTranslations)
			{
				// Our preference is to display as a Sibling, because this has the
				// identical formatting.
                DParagraph[] aParagraphs = null; // _GetSiblingParas(t);

				// But if we can't, we will try as an "other" language (which means
				// that it still has the same Section structure)
				if (null == aParagraphs)
					aParagraphs = _GetOtherLanguageParagraphs(t);

				// And if this fails, we try as a "Reference" language, which makes
				// no assumptions about section or paragraph structure.
				if (null == aParagraphs)
					aParagraphs = _GetReferenceLanguageParagraphs(t);

				// And if that still fails, then we must give up.
				if (null == aParagraphs || aParagraphs.Length == 0)
					continue;

				// Get an hvo for this language
				int hvoRelLang = Hvo.NextRelatedLanguage;
				v.Add(hvoRelLang);

				// Store the language's name
//				JCache.SetProp( hvoRelLang, ARelLangName, t.DisplayName);

				// Store the language's paragraphs
				int[] hvosPara = new int[ aParagraphs.Length ];
				for(int i=0; i<aParagraphs.Length; i++)
				{
					DParagraph p = aParagraphs[i] as DParagraph;
					Debug.Assert(null != p);

					hvosPara[i] = Hvo.NextRelLangPara;

//					ParagraphToCache(p, hvosPara[i]);
				}
//				JCache.CacheVecProp(hvoRelLang, ARelLangParas, hvosPara);
			}

			// Store the RelLang's vector
			int[] vHvos = JWU.ArrayListToIntArray(v);
//			JCache.CacheVecProp(hvoCluster, ARelLangs, vHvos);
		}
		#endregion
/****
****/

		// Constants -------------------------------------------------------------------------
		#region CONSTANTS - ATags

		// DCluster Attributes
		public  const int AFrontParas        = 10;  // A vector of the cluster's front paras
		public  const int ATargetParas       = 11;  // A vector of the cluster's target paras
		private const int AHasItalics        = 12;  // A front para in the cluster has Italics
		public  const int ARelLangs          = 13;  // A vector of the related languages

		// DParagraph Attributes
		private const int AStyleAbbrev       = 20;  // The paragraph's style
		private const int AAddedByCluster    = 21;  // Para was added for alignment purposes
		private const int AIsUserEditable    = 22;  // T if user is allowed to edit this para
		public  const int AOwningCluster     = 23;  // The para's owning cluster
		public  const int ARuns              = 24;  // The para's DRuns vector

		// DPicture (subclass of DParagraph) attributes (attrs of DCluster)
		private const int AHasPicture        = 30;  // T if the DParagraph is a DPicture
		private const int APicturePathname   = 31;  // Where the picture is on the disk
		public  const int APictureBitmap     = 32;  // The loaded bitmap for the picture
		private const int APictureHasCaption = 33;  // T if a caption goes with the picture

		// Related Languages
		public  const int ARelLangName       = 40;  // The Rel Lang's display name
		public  const int ARelLangParas      = 41;  // The Rel Lang's contents (DRuns vector)

		#endregion


	}
	#endregion


	#region TEST
	public class Test_Clusters : Test
	{
		// Scaffolding -----------------------------------------------------------------------
		#region Attrs (TeamSettings, Project, Translation, Book, Section)
		DTeamSettings TeamSettings = null;
		DProject Project = null;
		DTranslation FTrans = null;
		DTranslation TTrans = null;
		DBook FBook = null;
		DBook TBook = null;
		DSection FSection = null;
		DSection TSection = null;
		#endregion
		#region Constructor()
		public Test_Clusters()
			: base("Clusters")
		{
			AddTest( new IndividualTest( SynchronizeParagraphs ),
				"Synchronize Paragraphs" );

		}
		#endregion
		#region Method: override void Setup()
		public override void Setup()
		{
			// Team Settings (uses program defaults)
			TeamSettings = new DTeamSettings();
			TeamSettings.InitializeFactoryStyleSheet();

			// Project
			Project = new DProject(TeamSettings);
			Project.DisplayName = "Test Project";

			// Initialize a front translation and a book and two sections
			FTrans = new DTranslation("Front", "Latin", "Latin");
			Project.FrontTranslation = FTrans;
			FBook = new DBook("LUK", "");
			FTrans.AddBook(FBook);
			FSection = CreateSection(FBook, 1, 1);

			// Initialize a target translation and a book and two sections
			TTrans = new DTranslation("Target", "Latin", "Latin");
			Project.TargetTranslation = TTrans;
			TBook = new DBook("LUK", "");
			TTrans.AddBook(TBook);
			TSection = CreateSection(TBook, 1, 1);
		}
		#endregion
		#region Method: DSection CreateSection(DBook book, int nChapter, int nSectionNo)
		DSection CreateSection(DBook book, int nChapter, int nSectionNo)
		{
			DReference reference = new DReference();
			reference.Chapter = nChapter;
			reference.Verse = 1;

			DSection s = new DSection(nSectionNo);
			s.ReferenceSpan.UpdateFromLinearRead(reference);
			book.Sections.Append(s);
			return s;
		}
		#endregion
		#region Method: override void TearDown()
		public override void TearDown()
		{
			FSection  = null;
			TSection = null;
			FBook = null;
			TBook = null;
			FTrans = null;
			TTrans = null;
			Project = null;
			TeamSettings = null;
		}
		#endregion
		#region Attribute: PathName - temporary (c:\Program Files\NUnit\bin\TestClusters.x)
		private string PathName
		{
			get 
			{
				return GetPathName("TestClusters");
			}
		}
		#endregion

		// Cluster-to-Cache operations -------------------------------------------------------
		#region Test: SynchronizeParagraphs
		public void SynchronizeParagraphs()
		{
			DParagraph pModel = new DParagraph(FTrans);
			FSection.Paragraphs.Append(pModel);

			DParagraph pTarget = new DParagraph(TTrans);
			TSection.Paragraphs.Append(pTarget);

			// Test 1: Simple string in Front, an empty string in the target
			pModel.SimpleText = "Tuhan pung ana bua dari sorga kasi tau memang Yohanis Tukang " +
				"Sarani pung jadi";
			pTarget.SynchRunsToModelParagraph(pModel);
			AreSame(1, pTarget.Runs.Count);
			IsTrue( null != pTarget.Runs[0] as DText);
			// Test that running it again does no harm
			pTarget.SynchRunsToModelParagraph(pModel);
			AreSame(1, pTarget.Runs.Count);

			// Test 2: Model is Verse-Text; Target is only a Verse
			pModel.Runs.Clear();
			pModel.Runs.Append( DVerse.Create("18") );
			pModel.Runs.Append( DText.CreateSimple( "Ma Sakaria bale tanya itu ana bua, " +
				"bilang,  <<Mana bisa?! Beta deng beta pung bini su tua bagini! Karmana " +
				"beta bisa tau ini samua bisa jadi?>>") );
			pTarget.Runs.Clear();
			pTarget.Runs.Append( DVerse.Create("18") );
			pTarget.SynchRunsToModelParagraph(pModel);
			AreSame(2, pTarget.Runs.Count);
			pTarget.SynchRunsToModelParagraph(pModel);
			AreSame(2, pTarget.Runs.Count);
			IsTrue( null != pTarget.Runs[0] as DVerse);
			IsTrue( null != pTarget.Runs[1] as DText);

			// Test 3: Model is Verse-Text-Verse-Text-...
			pModel.Runs.Clear();
			pModel.Runs.Append( DVerse.Create("8") );
			pModel.Runs.Append( DText.CreateSimple( "Satu kali, Sakaria deng dia pung " +
				"kalompok Abia dong dapa roster karjá di Ruma Sambayang Pusat di kota " +
				"Yerusalem. ") );
			pModel.Runs.Append( DVerse.Create("9") );
			pModel.Runs.Append( DText.CreateSimple( "Iko " +
				"dong pung biasa, itu kapala-kapala agama dong hela lot, ko mau tau sapa " +
				"yang dapa tugas maso pi di itu Ruma Sambayang Pusat pung Kamar Paling " +
				"Barisi. Di situ, itu orang musti bakar kayu wangi di Tuhan Allah pung " +
				"muka. Itu hari, lot jato kaná di Sakaria. ") );
			pModel.Runs.Append( DVerse.Create("10") );
			pModel.Runs.Append( DText.CreateSimple( "Ais ju dia maso pi. " +
				"Waktu dia ada bakar kayu wangi di dalam itu Kamar, orang banya-banya " +
				"dong ada sambayang di kintal situ." ) );
			pModel.Runs.Append( DVerse.Create("11") );
			pModel.Runs.Append( DText.CreateSimple( "Takuju sa, ju Tuhan Allah pung " +
				"ana bua satu dari sorga datang badiri di tampa bakar kayu wangi pung " +
				"sablá kanan." ) );
			pTarget.Runs.Clear();
			pTarget.Runs.Append( DVerse.Create("8") );
			pTarget.Runs.Append( DVerse.Create("9") );
			pTarget.Runs.Append( DVerse.Create("10") );
			pTarget.Runs.Append( DVerse.Create("11") );
			pTarget.SynchRunsToModelParagraph(pModel);
			AreSame(8, pTarget.Runs.Count);
			IsTrue( null != pTarget.Runs[0] as DVerse);
			IsTrue( null != pTarget.Runs[1] as DText);
			IsTrue( null != pTarget.Runs[2] as DVerse);
			IsTrue( null != pTarget.Runs[3] as DText);
			IsTrue( null != pTarget.Runs[4] as DVerse);
			IsTrue( null != pTarget.Runs[5] as DText);
			IsTrue( null != pTarget.Runs[6] as DVerse);
			IsTrue( null != pTarget.Runs[7] as DText);
			pTarget.SynchRunsToModelParagraph(pModel);
			AreSame(8, pTarget.Runs.Count);
			IsTrue( null != pTarget.Runs[0] as DVerse);
			IsTrue( null != pTarget.Runs[1] as DText);
			IsTrue( null != pTarget.Runs[2] as DVerse);
			IsTrue( null != pTarget.Runs[3] as DText);
			IsTrue( null != pTarget.Runs[4] as DVerse);
			IsTrue( null != pTarget.Runs[5] as DText);
			IsTrue( null != pTarget.Runs[6] as DVerse);
			IsTrue( null != pTarget.Runs[7] as DText);

			// Test 4: Verse-Text-VerseBridge-Text
			pModel.Runs.Clear();
			pModel.Runs.Append( DVerse.Create("23") );
			pModel.Runs.Append( DText.CreateSimple( "Dong pung tugas abis, ju Sakaria " +
				"pulang pi dia pung ruma. ") );
			pModel.Runs.Append( DVerse.Create("24-25") );
			pModel.Runs.Append( DText.CreateSimple( "Sonde lama ju Elisabet dudu parú. " +
				"Ju dia kurung diri dalam ruma sampe lima bulan.Dia omong, bilang, " +
				"<<Puji Tuhan! Ahirnya Tuhan kasi tunju Dia pung hati bae sang beta. " +
				"Kalo beta su dapa turunan, orang dong su sonde bekin malu sang beta lai.>>") );
			pTarget.Runs.Clear();
			pTarget.Runs.Append( DVerse.Create("23") );
			pTarget.Runs.Append( DVerse.Create("24-25") );
			pTarget.SynchRunsToModelParagraph(pModel);
			AreSame(4, pTarget.Runs.Count);
			IsTrue( null != pTarget.Runs[0] as DVerse);
			IsTrue( null != pTarget.Runs[1] as DText);
			IsTrue( null != pTarget.Runs[2] as DVerse);
			IsTrue( null != pTarget.Runs[3] as DText);
			pTarget.SynchRunsToModelParagraph(pModel);
			AreSame(4, pTarget.Runs.Count);
			IsTrue( null != pTarget.Runs[0] as DVerse);
			IsTrue( null != pTarget.Runs[1] as DText);
			IsTrue( null != pTarget.Runs[2] as DVerse);
			IsTrue( null != pTarget.Runs[3] as DText);

			// Test 5: Verse-Text-SeeAlso-Verse-Text
			pModel.Runs.Clear();
			pModel.Runs.Append( DVerse.Create("5") );
			pModel.Runs.Append( DText.CreateSimple( "Some Text after 5") );
			pModel.Runs.Append( DSeeAlso.Create('a', null) );
			pModel.Runs.Append( DVerse.Create("6") );
			pModel.Runs.Append( DText.CreateSimple( "Some Text after 6") );
			pTarget.Runs.Clear();
			pTarget.Runs.Append( DVerse.Create("5") );
			pTarget.Runs.Append( DSeeAlso.Create('a', null) );
			pTarget.Runs.Append( DVerse.Create("6") );
			pTarget.SynchRunsToModelParagraph(pModel);
			AreSame(5, pTarget.Runs.Count);
			IsTrue( null != pTarget.Runs[0] as DVerse);
			IsTrue( null != pTarget.Runs[1] as DText);
			IsTrue( null != pTarget.Runs[2] as DSeeAlso);
			IsTrue( null != pTarget.Runs[3] as DVerse);
			IsTrue( null != pTarget.Runs[4] as DText);
			pTarget.SynchRunsToModelParagraph(pModel);
			AreSame(5, pTarget.Runs.Count);
			IsTrue( null != pTarget.Runs[0] as DVerse);
			IsTrue( null != pTarget.Runs[1] as DText);
			IsTrue( null != pTarget.Runs[2] as DSeeAlso);
			IsTrue( null != pTarget.Runs[3] as DVerse);
			IsTrue( null != pTarget.Runs[4] as DText);
		}
		#endregion



	}
	#endregion
}
