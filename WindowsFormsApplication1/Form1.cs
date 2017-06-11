using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using System.Security.Permissions;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        static string fn = "DriveOrganization1.xml";
        TreeView treeview2 = new TreeView();
        private XmlTextWriter xr;
        public Form1()
        {
            treeView1 = new TreeView();
         
            this.SuspendLayout();

            // Initialize treeView1.
            treeView1.AllowDrop = true;
            treeView1.Dock = DockStyle.Left;
            treeView1.Size = new Size(531, 584);
            treeView1.HotTracking = true;
            treeView1.HideSelection = false;

            treeView1.ItemDrag += new ItemDragEventHandler(treeView1_ItemDrag);
            treeView1.DragEnter += new DragEventHandler(treeView1_DragEnter);
            treeView1.DragOver += new DragEventHandler(treeView1_DragOver);
            treeView1.DragDrop += new DragEventHandler(treeView1_DragDrop);
            this.ClientSize = new Size(292, 273);
            this.Controls.Add(treeView1);
            this.ResumeLayout(false);


            InitializeComponent();
            treeView1.BeforeExpand += treeView1_BeforeExpand;
            treeView1.AfterSelect += treeView1_AfterSelect;
            List<DriveInfo> de = DriveInfo.GetDrives().ToList();

            foreach (DriveInfo d in de)
            {
                if (d.DriveType == DriveType.CDRom)
                {
                    continue;
                }

                TreeNode node = new TreeNode(d.RootDirectory.ToString());
                treeView1.Nodes.Add(node);
                FillChildNodes(node);
                //BuildTree( d.RootDirectory, treeView1.Nodes);
            }
        }


        
       
        
       
        private void fillTree()    /////Function To Fill the treeview that holds all the paths in the hard drive
        {
            string[] drives = Environment.GetLogicalDrives();
            foreach (string dr in drives)
            {
                DriveInfo di = new DriveInfo(dr);
                if (di.IsReady)
                {
                    TreeNode node = RecursiveDirWalk(dr);
                    treeview2.Nodes.Add(node);
                }

            }
        }
        
       
        private TreeNode RecursiveDirWalk(string path)           /////Recursive function to get the directories and the files to add them in the treeview that holds all the hard drive
        {
            TreeNode node = new TreeNode(path.Substring(path.LastIndexOf("\\")));
            IEnumerable<string> files = Enumerable.Empty<string>();
            IEnumerable<string> dirs = Enumerable.Empty<string>();
            try
            {
                // The test for UnauthorizedAccessException.
                var permission = new FileIOPermission(FileIOPermissionAccess.PathDiscovery, path);
                permission.Demand();

                files = Directory.GetFiles(path);
                dirs = Directory.GetDirectories(path);
            }
            catch
            {
                // Ignore folder (access denied).
                path = null;

            }
           
                if(path!=null)
            foreach(var dir in dirs)
            {
                    if (path.ToLower().IndexOf("$recycle.bin") != -1 || path.ToLower().IndexOf("system") != -1 || path.ToLower().IndexOf("$.sys") != -1 || path.ToLower().IndexOf("microsoft") != -1 || path.ToLower().IndexOf("programdata") != -1 || path.ToLower().IndexOf("document") != -1 || path.ToLower().IndexOf("user") != -1 || path.ToLower().IndexOf("windows") != -1)
                    {
                        continue;
                    }
                    node.Nodes.Add(RecursiveDirWalk(dir));
            }
            foreach(var file in files)
            {
                if (path.ToLower().IndexOf("$recycle.bin") != -1 || path.ToLower().IndexOf("system") != -1 || path.ToLower().IndexOf("$.sys") != -1 || path.ToLower().IndexOf("microsoft") != -1 || path.ToLower().IndexOf("programdata") != -1 || path.ToLower().IndexOf("document") != -1 || path.ToLower().IndexOf("user") != -1 || path.ToLower().IndexOf("windows") != -1)
                {
                    continue;
                }
                TreeNode tn = new TreeNode(Path.GetFileName(file));
                node.Nodes.Add(tn);
            }
            return node;
        }
      



        void FillChildNodes(TreeNode node)
        {
            try
            {
                DirectoryInfo Di = new DirectoryInfo(node.FullPath);
                foreach (DirectoryInfo di in Di.GetDirectories())
                {
                    TreeNode Newnode = new TreeNode(di.Name);
                    node.Nodes.Add(Newnode);
                    Newnode.Nodes.Add("*");
                }
                foreach (FileInfo fi in Di.GetFiles())
                {
                    TreeNode Newnode = new TreeNode(fi.Name);
                    node.Nodes.Add(Newnode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
                
        }


        //Fills the childs of the node before it expand
        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes[0].Text == "*")
            {
                e.Node.Nodes.Clear();
                FillChildNodes(e.Node);
            }


        }

        public void Form1_Load(object sender, EventArgs e)
        {         
        }

        

        private void button1_Click(object sender, EventArgs e)//Rename
        {
            
            try
            {

                string FileFullPath = treeView1.SelectedNode.FullPath;
                string Filename = treeView1.SelectedNode.Text;
                
                //Gets the directory of the file/folder.
                string newname = FileFullPath.Remove(FileFullPath.Length - Filename.Length, Filename.Length);
                //adds the newfile/folder name and it's extention to the string.
                newname += textBox1.Text + Path.GetExtension(FileFullPath);

                string neww = textBox1.Text + Path.GetExtension(FileFullPath);

                if (File.Exists(newname) || Directory.Exists(newname))
                {

                    MessageBox.Show("Sorry Name Already exists");
                }
                else
                {
                   if(Path.GetExtension(FileFullPath)=="")//folder
                    {
                        Directory.Move(FileFullPath, @newname);
                    }
                   else//file
                    {
                        File.Move(FileFullPath, @newname);
                    }
                   
                    treeView1.SelectedNode.Text = neww;
                    treeView1.Refresh();
          
                }
                
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry Couldn't Rename This "+ ex.Message.ToString());
                
            }
           
        }
       
        //Gets the select file/folder name into the textbox
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string x = Path.GetFileNameWithoutExtension(treeView1.SelectedNode.FullPath);
            textBox1.Text = x;
            textBox1.Refresh();
            
        }



        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // Move the dragged node when the left mouse button is used.
            if (e.Button == MouseButtons.Left)
            {
                DoDragDrop(e.Item, DragDropEffects.Move);
            }

            // Copy the dragged node when the right mouse button is used.
            else if (e.Button == MouseButtons.Right)
            {
                DoDragDrop(e.Item, DragDropEffects.Copy);
                
            }
        }

        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void treeView1_DragOver(object sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the mouse position.
            Point targetPoint = treeView1.PointToClient(new Point(e.X, e.Y));

            // Select the node at the mouse position.
            treeView1.SelectedNode = treeView1.GetNodeAt(targetPoint);

            treeView1.Scroll();
        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the drop location.
            Point targetPoint = treeView1.PointToClient(new Point(e.X, e.Y));

            // Retrieve the node at the drop location.
            TreeNode targetNode = treeView1.GetNodeAt(targetPoint);
            // Retrieve the node that was dragged.
            TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
            /////////////////////////////////////////////////
            string targetpath = targetNode.FullPath;
            string draggedpath = draggedNode.FullPath;
            string fname = Path.GetFileName(draggedpath);
            
            
            // Confirm that the node at the drop location is not 
            // the dragged node or a descendant of the dragged node.
            if (!draggedNode.Equals(targetNode) && !ContainsNode(draggedNode, targetNode))
            {
                //Expand the node at this location
                //to how the dropped node
                targetNode.Expand();
                // If it is a move operation, remove the node from its current 
                // location and add it to the node at the drop location.
                if (e.Effect == DragDropEffects.Move)
                {
                    Copy(draggedpath, targetpath, fname);
                    Delete(draggedpath);

                    draggedNode.Remove();
                    targetNode.Nodes.Add(draggedNode);
                }

                // If it is a copy operation, clone the dragged node 
                // and add it to the node at the drop location.
                else if (e.Effect == DragDropEffects.Copy)
                {
                    Copy(draggedpath,targetpath,fname);
                    targetNode.Nodes.Add((TreeNode)draggedNode.Clone());
                }

                // Expand the node at the location 
                // to show the dropped node.
                targetNode.Expand();
            }
        }
        
        private void Copy(string source,string dest,string Name)
        {
            MessageBox.Show("Copying  " + Path.GetFileNameWithoutExtension(source) + "  Into  " +dest);
            if(Path.GetExtension(source)=="") //Folder
            {
                string DirName = Path.GetFileNameWithoutExtension(source);
                if (!Directory.GetDirectories(dest).Contains(DirName))
                {
                    dest += "\\" + DirName;
                    Directory.CreateDirectory(dest);
                }



                string []files = Directory.GetFiles(source);
                string[] dirs = Directory.GetDirectories(source);
                
                foreach(string file in files)
                {
                    string tmp = dest+"\\" + Path.GetFileName(file);
                    File.Copy(file, tmp);
                }

                foreach (string dir in dirs)
                {
                    Copy(dir, dest,Path.GetFileName(dir));
                }
            }
            else //files
            {
                dest += "\\" + Name;
                File.Copy(source, dest);

            }

        }
        private void Delete(string source)
        {
            try
            {
                if (Path.GetExtension(source) == "") //Folder
                {
                    
                    string[] files = Directory.GetFiles(source);
                    string[] dirs = Directory.GetDirectories(source);
                    foreach (string file in files)
                    {
                        File.Delete(file);
                    }
                    foreach (string dir in dirs)
                    {
                       Delete(dir);
                    }
                    Directory.Delete(source);
                }
                else
                {
                    File.Delete(source);
                }
            }
            catch(Exception e)
            {

                MessageBox.Show(e.Message);
            }
        }
        private bool ContainsNode(TreeNode node1, TreeNode node2)
        {
            // Check the parent node of the second node.
            if (node2.Parent == null) return false;
            if (node2.Parent.Equals(node1)) return true;

            // If the parent node is not null or equal to the first node, 
            // call the ContainsNode method recursively using the parent of 
            // the second node.
            return ContainsNode(node1, node2.Parent);
        }

        private void button1_Click_1(object sender, EventArgs e) //Delete
        {
            DialogResult dialogResult = MessageBox.Show("Are you Sure you want to delete this node?", "Warning!", MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                DialogResult dialogResult1 = MessageBox.Show("This would delete this node from your hard drive as well!", "Final Warning!", MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
                if (dialogResult1 == DialogResult.Yes)
                {
                    Delete(@treeView1.SelectedNode.FullPath);
                }
               treeView1.SelectedNode.Remove();
                
            }
           
        }

        public void exportToXml(TreeView tv, string filename)
        {
            xr = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
            /*xr.Formatting = Formatting.Indented;
            xr.Indentation = 4;*/
            xr.WriteStartDocument();

            xr.WriteStartElement(treeview2.Nodes[0].Text);
            foreach(TreeNode node in tv.Nodes)
            {
                saveNode(node.Nodes);
            }
            xr.WriteEndElement();
            xr.Close();
        }
        
        private void saveNode(TreeNodeCollection tnc)
        {
            foreach (TreeNode node in tnc)
            {
                if (node.Nodes.Count > 0)
                {
                    xr.WriteStartElement(node.Text);
                    saveNode(node.Nodes);
                    xr.WriteEndElement();
                }
                else
                {
                    xr.WriteString(node.Text);
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            fillTree();
            exportToXml(treeview2, fn);

        }
    }
    public static class NativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        public static void Scroll(this Control control)
        {
            var pt = control.PointToClient(Cursor.Position);

            if ((pt.Y + 20) > control.Height)
            {
                // scroll down
                SendMessage(control.Handle, 277, (IntPtr)1, (IntPtr)0);
            }
            else if (pt.Y < 20)
            {
                // scroll up
                SendMessage(control.Handle, 277, (IntPtr)0, (IntPtr)0);
            }
        }
    }

}
