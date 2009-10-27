/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_TeamSettings.cs
 * Author:  John Wimbish
 * Created: 19 Feb 2009
 * Purpose: Tests the DTeamSettings class
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

using NUnit.Framework;

using JWTools;
using OurWordData;
using OurWordData.DataModel;

using OurWord;
using OurWord.Utilities;
#endregion

namespace OurWordTests.DataModel
{
	[TestFixture] public class T_TeamSettings
	{
		// Sample Clusters -------------------------------------------------------------------
		#region Attr{g}: string[] Clusters
		string[] Clusters
		{
			get
			{
				Debug.Assert(null != m_vClusters);
				return m_vClusters;
			}
		}
		string[] m_vClusters;
		#endregion
		const string c_sAru = "Test Cluster Aru";
		const string c_sTimor = "Test Cluster Timor";
		const string c_sSelaru = "Test Cluster Selaru";
		#region Method: void InitClusters()
		void InitClusters()
		{
			m_vClusters = new string[] 
			{ 
				c_sAru, 
				c_sTimor, 
				c_sSelaru  
			};
		}
		#endregion
		#region Method: void CreateSampleClusters()
		void CreateSampleClusters()
		{
			foreach (string sCluster in Clusters)
			{
				string sFolderRoot = JWU.GetMyDocumentsFolder(sCluster);
				string sFolderSettings = sFolderRoot + DTeamSettings.SettingsFolderName + 
                    Path.DirectorySeparatorChar;
				Directory.CreateDirectory(sFolderSettings);
				string sSettingsPath = sFolderSettings + sCluster + ".owt";
				StreamWriter w = new StreamWriter(sSettingsPath, false, Encoding.UTF8);
				TextWriter tw = TextWriter.Synchronized(w);
				tw.WriteLine(sSettingsPath);
				tw.Close();			
			}
		}
		#endregion
		#region Method: void DeleteSampleClusters()
		void DeleteSampleClusters()
		{
			foreach (string sCluster in Clusters)
			{
				string sFolderRoot = JWU.GetMyDocumentsFolder(sCluster);
				Directory.Delete(sFolderRoot, true);
			}
		}
		#endregion

		// Sample Languages ------------------------------------------------------------------
		#region Attr{g}: string[] Languages
		string[] Languages
		{
			get
			{
				Debug.Assert(null != m_vLanguages);
				return m_vLanguages;
			}
		}
		string[] m_vLanguages;
		#endregion
		const string c_sLangKupang = "Kupang";
		const string c_sLangNdao = "Ndao";
		const string c_sLangHelong = "Helong";
		const string c_sLangTetunBelu = "Tetun Belu";
		const string c_sLangPura = "Pura";
		#region Method: void InitLanguages()
		void InitLanguages()
		{
			m_vLanguages = new string[] 
			{ 
				c_sLangKupang, 
				c_sLangNdao, 
				c_sLangHelong,
				c_sLangTetunBelu,
				c_sLangPura
			};
		}
		#endregion
		#region Method: void CreateSampleLanguages(sCluster)
		void CreateSampleLanguages(string sCluster)
		{
			string sFolderCluster = JWU.GetMyDocumentsFolder(sCluster);
			string sSettingsFolder = sFolderCluster +
                Path.DirectorySeparatorChar +
                DTeamSettings.SettingsFolderName + 
				Path.DirectorySeparatorChar;

			foreach (string sLanguage in m_vLanguages)
			{
				// Create a language folder
				string sFolderLanguage = sFolderCluster + 
					Path.DirectorySeparatorChar + sLanguage;
				Directory.CreateDirectory(sFolderLanguage);

				// Create a owp settings file
				StreamWriter w = new StreamWriter(sSettingsFolder + sLanguage + 
					DProject.FileExtension);
				w.WriteLine(sLanguage);
				w.Close();
			}
		}
		#endregion

		// Setup / Teardown ------------------------------------------------------------------
		#region Setup - Init m_vX, CreateClusters
		[SetUp] public void Setup()
		{
			// Init the variables
			InitClusters();
			InitLanguages();

			// Set up the sample clusters
			CreateSampleClusters();
            ClusterList.ScanForClusters();
		}
		#endregion
		#region TearDown
		[TearDown] public void TearDown()
		{
			DeleteSampleClusters();
            ClusterList.ScanForClusters();
		}
		#endregion

		// Tests -----------------------------------------------------------------------------
		#region Test: GetClusterList
		[Test] public void GetClusterList()
			// Setup: the cluster folders are created
			// Teardown: the cluster folders will be deleted
		{
			// Get the list of clusters currently on the disk
            ClusterList.ScanForClusters();
			List<ClusterInfo> v = ClusterList.Clusters;

			// Ours should be included (there will likely be more, as I have my test data
			// on MyDocuments as well.)
			Assert.IsTrue(v.Count >= Clusters.Length, "We didn't find enough clusters");
			foreach (string sCluster in Clusters)
			{
                bool bFound = false;
                foreach (ClusterInfo ci in v)
                {
                    if (ci.Name == sCluster)
                        bFound = true;
                }
				Assert.IsTrue(bFound,  "Cluster [" + sCluster + "] was not found.");
			}
		}
		#endregion
		#region Test: GetLanguageList
		[Test] public void GetLanguageList()
		{
			// Place the languages under the Aru cluster
			CreateSampleLanguages(c_sAru);

			// Get the list of languages there
            ClusterInfo ci = ClusterList.FindClusterInfo(c_sAru);
            List<string> v = ci.GetClusterLanguageList(true);

			// Should be exactly what we have in Languages
			Assert.AreEqual(Languages.Length, v.Count);
			foreach (string sLanguage in Languages)
			{
				Assert.IsTrue(v.IndexOf(sLanguage) != -1, 
					"Language [" + sLanguage + "] was not found.");
			}
		}
		#endregion
	}
}
