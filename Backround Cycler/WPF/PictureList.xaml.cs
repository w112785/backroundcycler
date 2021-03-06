﻿using System;
using System.Windows;
using System.Windows.Controls;
using frm = System.Windows.Forms;
using Backround_Cycler.Core;
using Backround_Cycler.EventArguments;
using System.IO;
using System.Windows.Threading;
using System.Threading;

namespace Backround_Cycler.WPF
{
	/// <summary>
	/// Interaction logic for PictureList.xaml
	/// </summary>
	public partial class PictureList : UserControl
	{
		private frm.FolderBrowserDialog folderBrowserDialog = new frm.FolderBrowserDialog();
		private frm.OpenFileDialog openFileDialog = new frm.OpenFileDialog();
		private FileList fileList = new FileList();

		public PictureList()
		{
			InitializeComponent();

			folderBrowserDialog.Description = "Chose A Folder To Load The Images From";
			folderBrowserDialog.ShowNewFolderButton = false;
			folderBrowserDialog.SelectedPath = ApplicationInfo.settings.PicturesFolder;

			openFileDialog.SupportMultiDottedExtensions = true;
			openFileDialog.Multiselect = true;
			openFileDialog.Filter =
				"Compatible Files | *.jpg; *.bmp; *.gif; *.png; *.jpeg | All Files (*.*) | *.*";

			fileList.FileAdded += new EventHandler<FileAddedEventArgs>(fileList_FileAdded);
			//fileList.FileRemoved += new EventHandler<FileRemovedEventArgs> (fileList_FileRemoved);
			fileList.FilesAdded += new EventHandler<FileAddedEventArgs>(fileList_FilesAdded);
			fileList.ListCleared += FileList_ListCleared;
			fileList.LoadFromFile();
		}

		private void FileList_ListCleared(object sender, EventArgs e)
		{
			listView1.Items.Clear();
		}

		private void AddFilesFromFolder()
		{
			SearchOption opt;
			if (ApplicationInfo.settings.SubFolders)
				opt = SearchOption.AllDirectories;
			else
				opt = SearchOption.TopDirectoryOnly;

			fileList.AddFilesFromFolder(
				folderBrowserDialog.SelectedPath, opt);

			fileList.SaveToFile();
		}

		internal void CheckForEmptyList ()
		{
			Thread thread = new Thread(new ThreadStart(fileList.CheckForEmptyList));
			thread.Start();
			//fileList.CheckForEmptyList();
		}

		#region UI Events

		private void LoadFolder_Click(object sender, RoutedEventArgs e)
		{
			/*
             folderBrowserDialog1.SelectedPath = ApplicationInfo.settings.PicturesFolder;

			if (folderBrowserDialog1.ShowDialog () == DialogResult.Cancel)
			{
				return;
			}
			ApplicationInfo.settings.PicturesFolder = folderBrowserDialog1.SelectedPath;

			HandleThreads.StartOneThread ("load file from folder", new System.Threading.ThreadStart (
			AddFilesFromFolder));
             */

			folderBrowserDialog.SelectedPath = ApplicationInfo.settings.PicturesFolder;

			if (folderBrowserDialog.ShowDialog() == frm.DialogResult.Cancel)
			{
				return;
			}
			ApplicationInfo.settings.PicturesFolder = folderBrowserDialog.SelectedPath;
			Thread thread = new Thread(new ThreadStart(AddFilesFromFolder));
			thread.Start();
			//this.AddFilesFromFolder();
		}

		private void LoadFile_Click(object sender, RoutedEventArgs e)
		{
			openFileDialog.InitialDirectory = ApplicationInfo.settings.PicturesFolder;
			if (openFileDialog.ShowDialog() != frm.DialogResult.Cancel)
			{
				foreach (string file in openFileDialog.FileNames)
				{
					fileList.Add(file);
				}
				ApplicationInfo.settings.PicturesFolder = Path.GetDirectoryName(openFileDialog.FileName);
			}
			fileList.SaveToFile();
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			fileList.SaveToFile();
		}

		private void DeleteSelected_Click(object sender, RoutedEventArgs e)
		{
			if (listView1.SelectedItem != null)
			{
				fileList.Remove(listView1.SelectedItem.ToString());
				listView1.Items.Remove(listView1.SelectedItem);
			}
		}

		private void Clear_Click(object sender, RoutedEventArgs e)
		{
			fileList.Clear();
		}

		#endregion

		#region FileList Events

		void fileList_FilesAdded(object sender, FileAddedEventArgs e)
		{
			// This method will more then likely be called from a different thread.
			// So lets make sure that we are in the main thread before we add to the list.
			this.Dispatcher.BeginInvoke(new EventHandler<FileAddedEventArgs>(FilesAdded), sender, e);
		}

		private void FilesAdded(object sender, FileAddedEventArgs e)
		{
			foreach (string FilePath in e.GetFiles)
			{
				this.listView1.Items.Add(FilePath);
			}
		}

		void fileList_FileAdded(object sender, FileAddedEventArgs e)
		{
			//    ListViewItem lvi = new ListViewItem ();
			//    lvi.Content = e.FilePath;
			//    listView1.Items.Add (lvi);
			this.Dispatcher.BeginInvoke(new EventHandler<FileAddedEventArgs>(FilesAdded), sender, e);
		}

		#endregion
	}
}
