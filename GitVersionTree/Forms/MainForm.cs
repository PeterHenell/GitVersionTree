using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using System.IO;
using GitVersionTree.Classes;

namespace GitVersionTree
{
    public partial class MainForm : Form
    {
        private string DotFilename = Directory.GetParent(Application.ExecutablePath) + @"\" + Application.ProductName + ".dot";
        private string PdfFilename = Directory.GetParent(Application.ExecutablePath) + @"\" + Application.ProductName + ".pdf";
        private string LogFilename = Directory.GetParent(Application.ExecutablePath) + @"\" + Application.ProductName + ".log";
        string RepositoryName;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            GenerateButton.Select();
            Text = Application.ProductName + " - v" + Application.ProductVersion.Substring(0, 3);

            RefreshPath();
        }

        private void GitPathBrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog BrowseOpenFileDialog = new OpenFileDialog();
            BrowseOpenFileDialog.Title = "Select git.exe";
            if (!String.IsNullOrEmpty(Reg.Read("GitPath")))
            {
                BrowseOpenFileDialog.InitialDirectory = Reg.Read("GitPath");
            }
            BrowseOpenFileDialog.FileName = "git.exe";
            BrowseOpenFileDialog.Filter = "Git Application (git.exe)|git.exe";
            if (BrowseOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                Reg.Write("GitPath", BrowseOpenFileDialog.FileName);
                RefreshPath();
            }
        }

        private void GraphvizDotPathBrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog BrowseOpenFileDialog = new OpenFileDialog();
            BrowseOpenFileDialog.Title = "Select dot.exe";
            if (!String.IsNullOrEmpty(Reg.Read("GraphvizPath")))
            {
                BrowseOpenFileDialog.InitialDirectory = Reg.Read("GraphvizPath");
            }
            BrowseOpenFileDialog.FileName = "dot.exe";
            BrowseOpenFileDialog.Filter = "Graphviz Dot Application (dot.exe)|dot.exe";
            if (BrowseOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                Reg.Write("GraphvizPath", BrowseOpenFileDialog.FileName);
                RefreshPath();
            }
        }

        private void GitRepositoryPathBrowseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog BrowseFolderBrowserDialog = new FolderBrowserDialog();
            BrowseFolderBrowserDialog.Description = "Select Git repository";
            BrowseFolderBrowserDialog.ShowNewFolderButton = false;
            if (!String.IsNullOrEmpty(Reg.Read("GitRepositoryPath")))
            {
                BrowseFolderBrowserDialog.SelectedPath = Reg.Read("GitRepositoryPath");
            }
            if (BrowseFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                Reg.Write("GitRepositoryPath", BrowseFolderBrowserDialog.SelectedPath);
                RefreshPath();
            }
        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Reg.Read("GitPath")) ||
                String.IsNullOrEmpty(Reg.Read("GraphvizPath")) ||
                String.IsNullOrEmpty(Reg.Read("GitRepositoryPath")))
            {
                MessageBox.Show("Please select a Git, Graphviz & Git repository.", "Generate", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                StatusRichTextBox.Text = "";

                var graphManager = new GraphGenerationManager();

                RepositoryName = new DirectoryInfo(GitRepositoryPathTextBox.Text).Name;
                DotFilename = Directory.GetParent(Application.ExecutablePath) + @"\" + RepositoryName + ".dot";
                PdfFilename = Directory.GetParent(Application.ExecutablePath) + @"\" + RepositoryName + ".pdf";
                LogFilename = Directory.GetParent(Application.ExecutablePath) + @"\" + RepositoryName + ".log";
                ClearLogOutputFile();

                graphManager.Generate(Status, DotFilename, PdfFilename, LogFilename, RepositoryName, GraphvizDotPathTextBox.Text);

                ExitButton.Select();
            }
        }

        private void ClearLogOutputFile()
        {
            File.WriteAllText(LogFilename, "");
        }

        private void HomepageLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/crc8/GitVersionTree");
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void RefreshPath()
        {
            if (!String.IsNullOrEmpty(Reg.Read("GitPath")))
            {
                GitPathTextBox.Text = Reg.Read("GitPath");
            }
            if (!String.IsNullOrEmpty(Reg.Read("GraphvizPath")))
            {
                GraphvizDotPathTextBox.Text = Reg.Read("GraphvizPath");
            }
            if (!String.IsNullOrEmpty(Reg.Read("GitRepositoryPath")))
            {
                GitRepositoryPathTextBox.Text = Reg.Read("GitRepositoryPath");
            }
        }

        private void Status(string Message)
        {
            StatusRichTextBox.AppendText(DateTime.Now + " - " + Message + "\r\n");
            StatusRichTextBox.SelectionStart = StatusRichTextBox.Text.Length;
            StatusRichTextBox.ScrollToCaret();
            Refresh();
        }
    }
}
