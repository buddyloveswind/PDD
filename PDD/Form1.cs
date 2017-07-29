using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PDD
{



    public partial class Form1 : Form
    {

        string _DirectoryPath = string.Empty;
        public List<Picture> PicturesList = new List<Picture>();

        public Form1()
        {
            InitializeComponent();
            lblPath.Text = string.Empty;
            _DirectoryPath = @"C:\Users\tcl\Pictures";
            InitializeListView();
        }

        private void InitializeListView()
        {
            listView1.Clear();
            listView1.View = View.Details;
            listView1.Columns.Add("Delete", 50);
            listView1.Columns.Add("Name", 200);
            listView1.Columns.Add("FullName", 1000);
            listView1.Columns.Add("Size", 50);
            listView1.Columns.Add("CreationDate", 50);
            listView1.MultiSelect = false;
            PicturesList.Clear();
            btnDelete.Enabled = false;
            lblCount.Text = string.Empty;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChooseFolder();
        }

        public void ChooseFolder()
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                _DirectoryPath = folderBrowserDialog1.SelectedPath;
                lblPath.Text = _DirectoryPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            InitializeListView();

            if (Directory.Exists(_DirectoryPath))
            {
                // This path is a directory
                ProcessDirectory(_DirectoryPath);
            }
            else
            {
                //Console.WriteLine("{0} is not a valid file or directory.", _DirectoryPath);
            }

            ShowDoublons();
        }

        public void ShowDoublons()
        {
            List<Picture> SortedList = PicturesList.OrderBy(o => o.Name).ToList();
            SortedList.First();
            var previousName = string.Empty;
            var previousFullName = string.Empty;
            var firstDoublon = true;

            foreach (Picture t in SortedList)
            {
                if (t.Name == previousName)
                {
                    if (!(chkboxSize.Checked) || ((chkboxSize.Checked) && ExtraCheckSucceded(previousFullName, t.FullName)))
                    {
                        if (firstDoublon)
                        {
                            ListViewItem lvif = new ListViewItem();
                            lvif.SubItems.Add(previousName);
                            lvif.SubItems.Add(previousFullName);
                            listView1.Items.Add(lvif);
                            firstDoublon = false;
                        }
                        ListViewItem lvi = new ListViewItem();
                        lvi.SubItems.Add(t.Name);
                        lvi.SubItems.Add(t.FullName);
                        listView1.Items.Add(lvi);
                    }
                }
                else
                {
                    firstDoublon = true;
                }
                previousName = t.Name;
                previousFullName = t.FullName;
            }
            lblCount.Text = listView1.Items.Count.ToString() + " Doublons";
        }

        private bool ExtraCheckSucceded(string previousFullName, string fullName)
        {
            FileInfo previousFileInfo = new FileInfo(previousFullName);
            FileInfo currentFileInfo = new FileInfo(fullName);

            if (chkboxDate.Checked)
            {
                return ((previousFileInfo.Length == currentFileInfo.Length)
                     && (previousFileInfo.CreationTimeUtc == currentFileInfo.CreationTimeUtc));
            }
            else
            {
                return (previousFileInfo.Length == currentFileInfo.Length);
            }


        }

        // Process all files in the directory passed in, recurse on any directories 
        // that are found, and process the files they contain.
        public void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory, txtBoxExtensionOveride.Text);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }

        // Insert logic for processing found files here.
        public void ProcessFile(string path)
        {
            //add items to ListView
            PicturesList.Add(new Picture() { Name = path.Replace(Path.GetDirectoryName(path) + "\\", string.Empty), FullName = path });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ListView.CheckedListViewItemCollection checkedItems = listView1.CheckedItems;

            if (MessageBox.Show("Are you sure you want to delete " + checkedItems.Count.ToString() + " items ?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == DialogResult.OK)
            {
                progressBar1.Maximum = checkedItems.Count;
                progressBar1.Minimum = 0;
                foreach (ListViewItem item in checkedItems)
                {
                    File.Delete(item.SubItems[2].Text);
                    progressBar1.Step = progressBar1.Step + 1;
                }
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem lvItem = listView1.SelectedItems[0];
            System.Diagnostics.Process.Start(lvItem.SubItems[2].Text);
            listView1.SelectedItems[0].Checked = !listView1.SelectedItems[0].Checked;
        }

        private void listView1_MouseEnter(object sender, EventArgs e)
        {
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ListViewItem lvItem = listView1.SelectedItems[0];
            System.Diagnostics.Process.Start(Path.GetDirectoryName(lvItem.SubItems[2].Text));
        }
    }

}
