using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace xml_tool
{
    public partial class Form1 : Form
    {
        public string ip;
        public string port = "80";
        string[] files;
        FileInfo[] Files;
        DirectoryInfo dinfo;
        int count = 1001;
        int i = 0;
        int o = 0;
        int fill = 0;
        string pkg;
        static string t = null;
        static string s = null;
        static string scid = null;

        string appPath = Path.GetDirectoryName(Application.ExecutablePath);
        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
                DialogResult result1 = MessageBox.Show("Export files and start serving packages now?",
                 "Auto Run",
                  MessageBoxButtons.YesNo);
                if (result1 == DialogResult.Yes)
                {
                    OneClick_Load();
                }

                else
                {
                    Normal_Load();
                }
            
        }

        private void OneClick_Load()
        {
            tt();
            o_folder();
            g_ip();
            extractIcons();
            s_xml("hdd");
            s_xml("usb");
            toolStripButton5.Text = " Stop Server";
            if (File.Exists(@"category_game.xml"))
            {
                patch_xml();
            }

            if (File.Exists("bin/ApacheHTTPD_PHP_Portable/ApacheHTTPD_PHP_Portable.exe"))
            {
                start_server();
                toolStripButton5.Enabled = true;
            }
            else
            {
                toolStripButton5.Enabled = false;
            }
        }

        private void Normal_Load()
        {
            tt();
            o_folder();
            g_ip();
            extractIcons();
            if (File.Exists("bin/ApacheHTTPD_PHP_Portable/ApacheHTTPD_PHP_Portable.exe"))
            {
                toolStripButton5.Enabled = true;
            }
            else
            {
                toolStripButton5.Enabled = false;
            }

        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            ip = toolStripTextBox1.Text;
        }

        private void saveXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {

            s_xml("hdd");
            s_xml("usb");
        }

        private void SlectFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            s_folder();
            extractIcons();
        }

        private void extractIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            extractIcons();
        }

        private void patchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            patch_xml();
        }

        private void savePKGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            s_xml("hdd");
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            toolStrip1.Enabled = false;
            dataGridView1.Enabled = false;
            textBox1.Text = port;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (toolStripButton5.Text == " Start Server")
            {
                toolStripButton5.Text = " Stop Server";
                start_server();
            }
            else 
            {
                toolStripButton5.Text = " Start Server";
                stop_server();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            toolStrip1.Enabled = true;
            dataGridView1.Enabled = true;
            if (textBox1.Text != port)
            {
                port = textBox1.Text;
            }

            if (radioButton2.Checked == true)
            {
                fill = 1;
            }
            else
            {
                fill = 0;

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            toolStrip1.Enabled = true;
            dataGridView1.Enabled = true;
            textBox1.Text = port;
            if (fill == 0)
            {
                radioButton1.Checked = true;
                radioButton2.Checked = false;
            }
            else
            {
                radioButton1.Checked = false;
                radioButton2.Checked = true;
            }

        }

        private void s_folder()
        {
            

            i = 0;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();
                pkg = folderBrowserDialog1.SelectedPath;
                dinfo = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
                files = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.pkg");
                Files = dinfo.GetFiles("*.pkg");

                foreach (FileInfo file in Files)
                {
                    s = file.ToString();
                    if (s != "Refresh_Package_List.pkg")
                    {

                        s = s.Replace("&", "and");

                        FileStream pkgFile = File.Open(file.FullName, FileMode.Open);
                        byte[] cid = new byte[0x30];
                        pkgFile.Seek(0x30, SeekOrigin.Begin);
                        pkgFile.Read(cid, 0, 0x30);
                        pkgFile.Close();
                        scid = Encoding.ASCII.GetString(cid);
                        int index = scid.IndexOf("\0");
                        if (index > 0)
                            scid = scid.Substring(0, index);
                        string sz = SizeSuffix(file.Length);
                        s = file.ToString();

                        dataGridView1.Rows.Add(true, s, scid, sz);


                        if (!Directory.Exists(file.Directory + "\\bin"))
                        {
                            Directory.CreateDirectory(file.Directory + "\\bin");
                        }

                        if (!Directory.Exists(file.Directory + "\\bin\\icons"))
                        {
                            Directory.CreateDirectory(file.Directory + "\\bin\\icons");
                        }
                        //File.Copy("bin\\icons\\refresh_Package_List.PNG", file.Directory + "\\bin\\icons\\refresh_Package_List.PNG");
                        //File.Copy("bin\\icons\\refresh_Package_List.PNG", file.Directory + "\\bin\\icons\\refresh_Package_List.PNG");

                        if (!File.Exists(file.Directory + "\\bin\\icons\\download.png"))
                        {
                            File.Copy(appPath + "\\bin\\icons\\download.png", file.Directory + "\\bin\\icons\\download.png");
                        }
                        if (!File.Exists(file.Directory + "\\bin\\icons\\Refresh_Package_List.png"))
                        {
                            File.Copy(appPath + "\\bin\\icons\\Refresh_Package_List.png", file.Directory + "\\bin\\icons\\Refresh_Package_List.png");
                        }

                        checkedListBox1.Items.Add(s);
                        checkedListBox1.SetItemChecked(i, true);
                        i++;
                    }

                }
            }
        }

        private void o_folder()
        {

            i = 0;

            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            pkg = appPath;
            rename();
            dinfo = new DirectoryInfo(appPath);
            files = Directory.GetFiles(appPath, "*.pkg");
            Files = dinfo.GetFiles("*.pkg");

            foreach (FileInfo file in Files)
            {
                s = file.ToString();
                if (s != "Package_List.pkg")
                {
                    s = s.Replace("&", "and");

                    FileStream pkgFile = File.Open(file.FullName, FileMode.Open);
                    byte[] cid = new byte[0x30];
                    pkgFile.Seek(0x30, SeekOrigin.Begin);
                    pkgFile.Read(cid, 0, 0x30);
                    pkgFile.Close();
                    scid = Encoding.ASCII.GetString(cid);
                    int index = scid.IndexOf("\0");
                    if (index > 0)
                        scid = scid.Substring(0, index);
                    string sz = SizeSuffix(file.Length);
                    s = file.ToString();

                    dataGridView1.Rows.Add(true, s, scid, sz);

                    /* if (!File.Exists(file.Directory + "/download.png"))
                     {
                         File.Copy("bin\\icons\\download.png", file.Directory + "/download.png");
                     }
                     if (!File.Exists(file.Directory + "/Refresh_Package_List.png"))
                     {
                         File.Copy("bin\\icons\\Refresh_Package_List.png", file.Directory + "/Refresh_Package_List.png");
                     }*/
                    checkedListBox1.Items.Add(s);
                    checkedListBox1.SetItemChecked(i, true);
                    i++;
                }
            }


        }

        private void s_xml(string xtype)
        {
            string ipd = "";
            string ics = "";
            count = 1001;
            if (xtype == "usb")
            {
                ipd = "http://" + ip + "/bin/icons/";
                ics = pkg + " on " + ip;
                toolStripLabel1.Text = "Saving XML";
            }
            if (xtype == "hdd")
            {
                ipd = "/dev_hdd0/game/PKGLINKER/USRDIR/icons/";
                ics = "/dev_hdd0/game/PKGLINKER/USRDIR/";
                toolStripLabel1.Text = "Saving PKG";
            }
            using (System.IO.StreamWriter nfile =
            new System.IO.StreamWriter(@"bin/package_link.xml", false))
            {

                nfile.WriteLine("<?xml version=" + '"' + "1.0" + '"' + " encoding=" + '"' + "UTF-8" + '"' + "?> \n \n<XMBML version=" + '"' + "1.0" + '"' + ">");
                if (xtype == "usb")
                {
                    nfile.WriteLine("	<View id=" + '"' + "package_link" + '"' + "> \n		<Attributes> \n			<Table key=" + '"' + "pkg_main" + '"' + ">");
                }
                else if (xtype == "hdd")
                {
                    nfile.WriteLine("	<View id=" + '"' + "package_link_game" + '"' + "> \n		<Attributes> \n			<Table key=" + '"' + "pkg_main" + '"' + ">");
                }
                nfile.WriteLine("				<Pair key=" + '"' + "icon_rsc" + '"' + "><String>tex_album_icon</String></Pair>");
                nfile.WriteLine("				<Pair key=\"icon_notation\"><String>WNT_XmbItemAlbum</String></Pair>");
                nfile.WriteLine("				<Pair key=" + '"' + "title" + '"' + "><String>★ Install Packages From Webserver</String></Pair>");
                nfile.WriteLine("				<Pair key=" + '"' + "info" + '"' + "><String>" + pkg + " on " + ip + "</String></Pair>");
                nfile.WriteLine("				<Pair key=" + '"' + "ingame" + '"' + "><String>disable</String></Pair>\n			</Table>\n		</Attributes>\n			<Items>\n			<Query ");
                nfile.WriteLine("				class=" + '"' + "type:x-xmb/folder-pixmap" + '"');
                nfile.WriteLine("				key=" + '"' + "pkg_main" + '"');
                nfile.WriteLine("				attr=" + '"' + "pkg_main" + '"');
                nfile.WriteLine("				src=" + '"' + "#pkg_items" + '"' + "/>");
                nfile.WriteLine("			</Items>\n		</View>\n		<View id=" + '"' + "pkg_items" + '"' + ">	\n		<Attributes>");
                i = 0;

                string pkgt1 = "			<Table key=\"pkg_000\">\n				<Pair key=\"icon\"><String>http://" + ip + "/bin/icons/Refresh_Package_List.PNG</String></Pair>\n				<Pair key=\"title\"><String>Refresh Package List</String></Pair>\n 				<Pair key=\"info\"><String>Reboot Required After Refreshing List</String></Pair>\n 			</Table>";
                string pkgt2 = "			<Table key=\"pkg_000\">\n				<Pair key=\"icon\"><String>/dev_hdd0/game/PKGLINKER/USRDIR/icons/Refresh_Package_List.PNG</String></Pair>\n				<Pair key=\"title\"><String>Refresh Package List</String></Pair>\n 				<Pair key=\"info\"><String>Reboot Required After Refreshing List</String></Pair>\n 			</Table>";
                string pkgm = "			<Query\n				class=\"type:x-xmb/folder-pixmap\"\n				key=\"pkg_000\"\n				attr=\"pkg_000\"\n				src=\"#pkg_000_item\"/>";
                string pkgb = "		<View id=\"pkg_000_item\">\n		<Attributes>\n			<Table key=\"link000\">\n				<Pair key=\"info\"><String>net_package_install</String></Pair>\n				<Pair key=\"pkg_src\"><String>http://" + ip + "/Package_List.pkg</String></Pair>\n				<Pair key=\"pkg_src_qa\"><String>http://" + ip + "/Package_List.pkg</String></Pair>\n				<Pair key=\"content_name\"><String>pkg_install_pc</String></Pair>\n				<Pair key=\"content_id\"><String>UP0100-PKGLINKER_00-PINK1000DEVIL303</String></Pair>\n				<Pair key=\"prod_pict_path\"><String>" + ipd + "Refresh_Package_List.PNG</String></Pair>\n			</Table>\n 		</Attributes>\n 			<Items>\n			<Item class=\"type:x-xmb/xmlnpsignup\" key=\"link000\" attr=\"link000\"/>\n		</Items>\n 		</View>";
               
                if (xtype == "usb")
                {
                    nfile.WriteLine(pkgt1); ;
                }
                else
                {
                    nfile.WriteLine(pkgt2); ;
                }



                foreach (FileInfo file in Files)
                {
                    s = file.ToString();
                    if (s != "Package_List.pkg")
                    {

                        t = Convert.ToString(count);
                        t = t.Remove(0, 1);
                        FileStream pkgFile = File.Open(file.FullName, FileMode.Open);
                        byte[] cid = new byte[0x30];
                        pkgFile.Seek(0x30, SeekOrigin.Begin);
                        pkgFile.Read(cid, 0, 0x30);
                        byte[] cids = new byte[0x9];
                        pkgFile.Seek(0x37, SeekOrigin.Begin);
                        pkgFile.Read(cids, 0, 0x9);
                        pkgFile.Close();

                        scid = Encoding.ASCII.GetString(cid);

                        string scids = Encoding.ASCII.GetString(cids);

                        int index = scid.IndexOf("\0");
                        if (index > 0)
                            scid = scid.Substring(0, index);



                        string sz = SizeSuffix(file.Length);
                        s = file.ToString();


                        /* s = s.ToLower();
                         s = s.Replace("?", "");
                         s = s.Replace("%", "");
                         s = s.Replace("(", "");
                         s = s.Replace(")", "");
                         s = s.Replace("!", "");
                         s = s.Replace(":", "");
                         s = s.Replace("-", "");*/
                        s = s.Replace("&", "and");
                        // s = s.Replace(" ", "_");
                        //s = s.Replace("__", "_");

                        dataGridView1.CurrentCell = null;
                        DataGridViewCheckBoxCell chk = dataGridView1.Rows[i].Cells[0] as DataGridViewCheckBoxCell;

                        if (Convert.ToBoolean(chk.Value) == true)

                        //if (checkedListBox1.GetItemCheckState(i) == CheckState.Checked)
                        {
                            string imf = s;
                            imf = imf.Replace(".PKG", ".PNG");
                            imf = imf.Replace(".pkg", ".PNG");
                            imf = imf.Replace("&", "and");
                            imf = imf.Replace(" ", "%20");
                            //imf = imf.Replace("_", "%20");

                            string imj = s;
                            imj = imj.Replace(".PKG", ".JPG");
                            imj = imj.Replace(".pkg", ".JPG");
                            imj = imj.Replace("&", "and");
                            imj = imj.Replace(" ", "%20");
                           // imj = imj.Replace("_", "%20");

                            string imn = s;
                            imn = imn.Replace(".PKG", ".PNG");
                            imn = imn.Replace(".pkg", ".PNG");
                            imn = imn.Replace("&", "and");
                            imn = imn.Replace(" ", "%20");
                            //imn = imn.Replace("_", "%20");
                            //imn = "bin/icons/" + imn;

                            string imnj = s;
                            imnj = imnj.Replace(".PKG", ".JPG");
                            imnj = imnj.Replace(".pkg", ".JPG");
                            imnj = imnj.Replace("&", "and");
                            imnj = imnj.Replace(" ", "%20");
                            //imnj = imnj.Replace("_", "%20");
                            //imnj = "bin/icons/" + imnj;

                            s = s.Replace(".pkg", "");
                            s = s.Replace(".PKG", "");
                            s = s.Replace("_", " ");
                           
                            nfile.WriteLine("			<Table key=" + '"' + "pkg_" + t + '"' + ">");
                            if (File.Exists("bin/icons/" + imn))
                            {
                                nfile.WriteLine("				<Pair key=" + '"' + "icon" + '"' + "><String>" + ipd + imn + "</String></Pair>");
                            }
                            else if (File.Exists("bin/icons/" + imnj))
                            {
                                nfile.WriteLine("				<Pair key=" + '"' + "icon" + '"' + "><String>" + ipd + imnj + "</String></Pair>");
                            }
                            else
                            {
                                nfile.WriteLine("				<Pair key=" + '"' + "icon" + '"' + "><String>" + ipd + "download.png</String></Pair>");
                            }
                            nfile.WriteLine("				<Pair key=" + '"' + "title" + '"' + "><String>" + s + "</String></Pair>");
                            nfile.WriteLine("				<Pair key=" + '"' + "info" + '"' + "><String>" + scids + "   " + sz + "</String></Pair>");
                            nfile.WriteLine(" 			</Table>");

                            count++;
                        }
                        i++;
                    }
                }

                nfile.WriteLine("		</Attributes>\n			<Items>");
                nfile.WriteLine(pkgm);
                count = 1001;
                i = 0;
                foreach (FileInfo file in Files)
                {
                    s = file.ToString();
                    if (s != "Package_List.pkg")
                    {
                        t = Convert.ToString(count);
                        t = t.Remove(0, 1);

                        DataGridViewCheckBoxCell chk = dataGridView1.Rows[i].Cells[0] as DataGridViewCheckBoxCell;

                        if (Convert.ToBoolean(chk.Value) == true)

                        //if (checkedListBox1.GetItemCheckState(i) == CheckState.Checked)
                        {

                            nfile.WriteLine("			<Query\n				class=" + '"' + "type:x-xmb/folder-pixmap" + '"');
                            nfile.WriteLine("				key=" + '"' + "pkg_" + t + '"');
                            nfile.WriteLine("				attr=" + '"' + "pkg_" + t + '"');
                            nfile.WriteLine("				src=" + '"' + "#pkg_" + t + "_item" + '"' + "/>");

                            count++;
                        }
                        i++;
                    }
                }
                nfile.WriteLine("			</Items>\n		</View>\n");
                nfile.WriteLine(pkgb);
                count = 1001;
                i = 0;
                foreach (FileInfo file in Files)
                {
                    s = file.ToString();
                    if (s != "Package_List.pkg")
                    {
                        t = Convert.ToString(count);
                        t = t.Remove(0, 1);
                        FileStream pkgFile = File.Open(file.FullName, FileMode.Open);
                        byte[] cid = new byte[0x30];
                        pkgFile.Seek(0x30, SeekOrigin.Begin);
                        pkgFile.Read(cid, 0, 0x30);
                        pkgFile.Close();
                        scid = Encoding.ASCII.GetString(cid);
                        int index = scid.IndexOf("\0");
                        if (index > 0)
                            scid = scid.Substring(0, index);

                        string imf = s;
                        imf = imf.Replace(".PKG", ".PNG");
                        imf = imf.Replace(".pkg", ".PNG");
                        imf = imf.Replace("&", "and");
                        imf = imf.Replace(" ", "%20");
                        //imf = imf.Replace("_", "%20");

                        string imj = s;
                        imj = imj.Replace(".PKG", ".JPG");
                        imj = imj.Replace(".pkg", ".JPG");
                        imj = imj.Replace("&", "and");
                        imj = imj.Replace(" ", "%20");
                        //imj = imj.Replace("_", "%20");

                        string imn = s;
                        imn = imn.Replace(".PKG", ".PNG");
                        imn = imn.Replace(".pkg", ".PNG");
                        imn = imn.Replace("&", "and");
                        imn = imn.Replace(" ", "%20");
                        //imn = imn.Replace("_", "%20");
                        //imn = "bin/icons/" + imn;
                        string imnj = s;
                        imnj = imnj.Replace(".PKG", ".JPG");
                        imnj = imnj.Replace(".pkg", ".JPG");
                        imnj = imnj.Replace("&", "and");
                        imnj = imnj.Replace(" ", "%20");
                        //imnj = imnj.Replace("_", "%20");
                        //imnj = "bin/icons/" + imnj;
                        DataGridViewCheckBoxCell chk = dataGridView1.Rows[i].Cells[0] as DataGridViewCheckBoxCell;
                        string wfile = file.ToString();
                        wfile = wfile.Replace(" ", "%20");

                        if (Convert.ToBoolean(chk.Value) == true)

                            

                        //if (checkedListBox1.GetItemCheckState(i) == CheckState.Checked)
                        {

                            nfile.WriteLine("		<View id=" + '"' + "pkg_" + t + "_item" + '"' + ">");
                            nfile.WriteLine("		<Attributes>");
                            nfile.WriteLine("			<Table key=" + '"' + "link" + t + '"' + ">");
                            nfile.WriteLine("				<Pair key=" + '"' + "info" + '"' + "><String>net_package_install</String></Pair>");
                            nfile.WriteLine("				<Pair key=" + '"' + "pkg_src" + '"' + "><String>http://" + ip + "/" + file + "</String></Pair>");
                            nfile.WriteLine("				<Pair key=" + '"' + "pkg_src_qa" + '"' + "><String>http://" + ip + "/" + file + "</String></Pair>");
                            nfile.WriteLine("				<Pair key=" + '"' + "content_name" + '"' + "><String>pkg_install_pc</String></Pair>");
                            nfile.WriteLine("				<Pair key=" + '"' + "content_id" + '"' + "><String>" + scid + "</String></Pair>");
                            if (File.Exists("bin/icons/" + imn))
                            {
                                nfile.WriteLine("				<Pair key=" + '"' + "prod_pict_path" + '"' + "><String>" + ipd + imn + "</String></Pair>");
                            }
                            else if (File.Exists("bin/icons/" + imnj))
                            {
                                nfile.WriteLine("				<Pair key=" + '"' + "prod_pict_path" + '"' + "><String>" + ipd + imnj + "</String></Pair>");
                            }
                            else
                            {
                                nfile.WriteLine("				<Pair key=" + '"' + "prod_pict_path" + '"' + "><String>" + ipd + "download.png</String></Pair>");
                            }

                            nfile.WriteLine(" 			</Table> \n		</Attributes> \n			<Items>");
                            nfile.WriteLine("			<Item class=" + '"' + "type:x-xmb/xmlnpsignup" + '"' + " key=" + '"' + "link" + t + '"' + " attr=" + '"' + "link" + t + '"' + "/>");
                            nfile.WriteLine("		</Items> \n		</View> \n\n");



                            count++;
                        }
                        i++;
                    }
                }
                nfile.WriteLine("</XMBML>");

            }
            if (xtype == "usb")
            {
                toolStripLabel1.Text = "";
                MessageBox.Show("Saved " + appPath + "bin\\package_link.xml \nSaved " + pkg + "\\Package_List.pkg ", "File Saved", MessageBoxButtons.OK);
            }
            else if (xtype == "hdd")
            {
                if (File.Exists("bin\\PS3_GAME\\USRDIR\\package_link.xml"))
                {
                    File.Delete("bin\\PS3_GAME\\USRDIR\\package_link.xml");
                }
                File.Move("bin\\package_link.xml", "bin\\PS3_GAME\\USRDIR\\package_link.xml");


                // Use ProcessStartInfo class.
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.WorkingDirectory = appPath + "\\bin";
                //startInfo.RedirectStandardError = true;
                // startInfo.RedirectStandardInput = true;
                //startInfo.RedirectStandardOutput = true;
                startInfo.FileName = "bin/psn_package_npdrm2.exe";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Arguments = "package.conf PS3_GAME";
                // Start the process with the info we specified.
                // Call WaitForExit and then the using-statement will close.
                Process exeProcess = Process.Start(startInfo);
                exeProcess.WaitForExit();

                DialogResult result1 = MessageBox.Show("Resign with PS3xploit-resigner for OFW?",
             "Resign",
              MessageBoxButtons.YesNo);
                if (result1 == DialogResult.Yes)
                {
                    o = 1;

                    // Use ProcessStartInfo class.
                    ProcessStartInfo startInfo2 = new ProcessStartInfo();
                    startInfo2.CreateNoWindow = false;// true;
                    startInfo2.UseShellExecute = false;
                    startInfo2.WorkingDirectory = appPath + "\\bin";
                    //startInfo.RedirectStandardError = true;
                    // startInfo.RedirectStandardInput = true;
                    //startInfo.RedirectStandardOutput = true;
                    startInfo2.FileName = "bin/ps3xploit_rifgen_edatresign.exe"; //UP0100-PKGLINKER_00-PINK1000DE";L//3.pkg h";
                    //startInfo2.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo2.Arguments = "UP0100-PKGLINKER_00-PINK1000DEVIL303.pkg h";
                    // Start the process with the info we specified.
                    // Call WaitForExit and then the using-statement will close.
                    Process exeProcess2 = Process.Start(startInfo2);




                    exeProcess2.WaitForExit();


                    //MessageBox.Show("Saved " + pkg + "\\UP0100-PKGLINKER_00-DEVIL303000PINK1.pkg", "File Saved", MessageBoxButtons.OK);
                }

                if (o == 0)
                {
                    if (File.Exists("bin\\UP0100-PKGLINKER_00-PINK1000DEVIL303.pkg"))
                    {
                        if (File.Exists(pkg + "\\Package_List.pkg"))
                        {
                            File.Delete(pkg + "\\Package_List.pkg");
                        }
                        File.Move("bin\\UP0100-PKGLINKER_00-PINK1000DEVIL303.pkg", pkg + "\\Package_List.pkg");
                    }
                }

                else if (o == 1)
                {
                    o = 0;
                    if (File.Exists("bin\\UP0100-PKGLINKER_00-PINK1000DEVIL303.pkg_signed.pkg"))
                    {
                        if (File.Exists(pkg + "\\bin\\UP0100-PKGLINKER_00-PINK1000DEVIL303.pkg"))
                        {
                            File.Delete(pkg + "\\bin\\UP0100-PKGLINKER_00-PINK1000DEVIL303.pkg");
                        }
                        if (File.Exists(pkg + "\\Package_List.pkg"))
                        {
                            File.Delete(pkg + "\\Package_List.pkg");
                        }
                        File.Move("bin\\UP0100-PKGLINKER_00-PINK1000DEVIL303.pkg_signed.pkg", pkg + "\\Package_List.pkg");
                    }
                
                }

                toolStripLabel1.Text = "";
            }
        }

        private void g_ip()
        {
            string hostName = Dns.GetHostName();
            ip = Dns.GetHostByName(hostName).AddressList[0].ToString();
            toolStripTextBox1.Text = ip;
        }

        static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }

            int c = 0;
            decimal dValue = (decimal)value;
            while (Math.Round(dValue, decimalPlaces) >= 1000)
            {
                dValue /= 1024;
                c++;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", dValue, SizeSuffixes[c]);
        }

        private void GenerateImage(string bkImage, string cname)
        {
            Image backImage = (Bitmap)Image.FromFile(bkImage);
            int targetHeight = 320; //height and width of the finished image
            int targetWidth = 320;

            //be sure to use a pixelformat that supports transparency
            using (var bitmap = new Bitmap(targetWidth, targetHeight,
                    PixelFormat.Format32bppArgb))
            {
                using (var canvas = Graphics.FromImage(bitmap))
                {
                    //this ensures that the backgroundcolor is transparent
                    canvas.Clear(Color.Transparent);

                    //this selects the entire backimage and and paints
                    //it on the new image in the same size, so its not distorted.
                    int w = (320 - backImage.Width) / 2;
                    int h = (320 - backImage.Height) / 2;
                    canvas.DrawImage(backImage,
                              new Rectangle(w, h, backImage.Width, backImage.Height),
                              new Rectangle(0, 0, backImage.Width, backImage.Height),
                              GraphicsUnit.Pixel);

                    //this paints the frontimage with a offset at the given coordinates
                    //canvas.DrawImage(frontImage, 5, 25);

                    canvas.Save();
                    bitmap.Save(cname, ImageFormat.Png);
                    backImage.Dispose();

                }
            }
        }

        private void extractIcons()
        {

            foreach (FileInfo file in Files)
            {
                // Use ProcessStartInfo class.
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.FileName = "bin/PS3P_PKG_Ripper_1.3.exe";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Arguments = "-o " + '"' + file.Directory + '"' + " -s ICON0.PNG " + '"' + file.FullName + '"';

                try
                {
                    // Start the process with the info we specified.
                    // Call WaitForExit and then the using-statement will close.
                    using (Process exeProcess = Process.Start(startInfo))
                    {
                        string imn = file.FullName;
                        imn = imn.Replace(".PKG", ".PNG");
                        imn = imn.Replace(".pkg", ".PNG");
                        imn = imn.Replace("&", "and");
                        string imo = file.Name;
                        imo = imo.Replace(".PKG", ".PNG");
                        imo = imo.Replace(".pkg", ".PNG");
                        imo = imo.Replace("&", "and");
                        exeProcess.WaitForExit();

                        if (File.Exists(file.Directory + "\\ICON0.PNG"))
                        {
                            GenerateImage(file.Directory + "\\ICON0.PNG", imn);
                            //File.Move(file.Directory + "\\ICON0.PNG", imn);
                            File.Delete(file.Directory + "\\ICON0.PNG");
                            if (File.Exists(file.Directory + "\\bin\\icons\\" + imo))
                            {
                                File.Delete(file.Directory + "\\bin\\icons\\" + imo);
                            }
                            
                            //File.Copy(imn, file.Directory + "\\bin\\PS3_GAME\\USRDIR\\icons\\" + imo);

                            File.Move(imn, file.Directory + "\\bin\\icons\\" + imo);



                            if (File.Exists(file.Directory + "\\bin\\icons\\" + imo))
                            {
                                if (File.Exists(file.Directory + "\\bin\\PS3_GAME\\USRDIR\\icons\\" + imo))
                                {
                                    File.Delete(file.Directory + "\\bin\\PS3_GAME\\USRDIR\\icons\\" + imo);
                                }
                                File.Copy(file.Directory + "\\bin\\icons\\" + imo, file.Directory + "\\bin\\PS3_GAME\\USRDIR\\icons\\" + imo);
                                
                            }
                        }

                        
                    }
                }
                catch
                {
                    // Log error.
                }

            }
        }

        private void start_server()
        {

            if (fill == 0)
            {

                // Use ProcessStartInfo class.
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                //startInfo.WorkingDirectory = ;
                startInfo.RedirectStandardError = true;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.FileName = "bin/ApacheHTTPD_PHP_Portable/ApacheHTTPD_PHP_Portable.exe";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process exeProcess = Process.Start(startInfo);//(appPath + "/bin/ApacheHTTPD_PHP_Portable/ApacheHTTPD_PHP_Portable.exe");

                
            }
            else
            {
                // Use ProcessStartInfo class.
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;//true;
                startInfo.UseShellExecute = false;
                startInfo.WorkingDirectory = pkg;
                
                //startInfo.RedirectStandardError = true;
                startInfo.RedirectStandardInput = false;
                //startInfo.RedirectStandardOutput = true;
                startInfo.FileName = "bin/ApacheHTTPD_PHP_Portable/ApacheHTTPD_PHP_Portable.exe";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                // Start the process with the info we specified.
                // Call WaitForExit and then the using-statement will close.
                Process exeProcess = Process.Start(startInfo);
            }

            MessageBox.Show("Server is running on " + ip + ":" + port + "\nPress ok to continue", "Server running", MessageBoxButtons.OK);

        }

        private void stop_server()
        {
            int procID;
            Process[] processes;
           string procName = "cmd";
           processes = Process.GetProcessesByName(procName);
           foreach (Process proc in processes)
           {
               string temps = proc.MainWindowTitle;
               int tempi = proc.Id;
               if (proc.MainWindowTitle == "Apache 2.4.6 Portable...")
               {
                   procID = tempi;
                   Process tempProc = Process.GetProcessById(procID);
                   tempProc.CloseMainWindow();
                   tempProc.WaitForExit();
               }
            }

           /*8 Process[] processes = Process.GetProcessesByName("HHTCtrlp");
            foreach (var process in processes)
            {
                process.Kill();
            }*/
        }

        private void patch_xml()
        {
            if (File.Exists(@"category_game.xml"))
            {
                using (StreamReader sr = new StreamReader(@"category_game.xml"))
                {

                    string findpm1 = "<View id=\"seg_package_fixed\">";
                    string findpm2 = "<Query class=\"type:x-xmb/folder-pixmap\" key=\"hdd0_delete\" attr=\"hdd0_delete\" src=\"#seg_package_files_delete\"/>";
                    string rppm = "<Query class=\"type:x-xmb/folder-pixmap\" key=\"hdd0_delete\" attr=\"hdd0_delete\" src=\"#seg_package_files_delete\"/>\n			<Query class=\"type:x-xmb/folder-pixmap\" key=\"pkg_link0\" src=\"xmb://localhost/dev_usb000/package_link.xml#package_link\"/>\n            <Query class=\"type:x-xmb/folder-pixmap\" key=\"pkg_link1\" src=\"xmb://localhost/dev_usb001/package_link.xml#package_link\"/>\n            <Query class=\"type:x-xmb/folder-pixmap\" key=\"pkg_link2\" src=\"xmb://localhost/dev_hdd0/game/PKGLINKER/package_link.xml#package_link_game\"/>";
                    string findipf1 = "<View id=\"seg_packages\">";
                    string findipf2 = "<Query class=\"type:x-xmb/xmlpackagefolder\" key=\"host_provider_usb7\" src=\"host://localhost/q?path=/dev_usb007&suffix=.pkg&subclass=x-host/package\" />";
                    string rpipf1 = "<Query class=\"type:x-xmb/xmlpackagefolder\" key=\"host_provider_usb7\" src=\"host://localhost/q?path=/dev_usb007&suffix=.pkg&subclass=x-host/package\" />\n			<Query class=\"type:x-xmb/folder-pixmap\" key=\"pkg_link0\" src=\"xmb://localhost/dev_usb000/package_link.xml#package_link\"/>\n            <Query class=\"type:x-xmb/folder-pixmap\" key=\"pkg_link1\" src=\"xmb://localhost/dev_usb001/package_link.xml#package_link\"/>\n            <Query class=\"type:x-xmb/folder-pixmap\" key=\"pkg_link2\" src=\"xmb://localhost/dev_hdd0/game/PKGLINKER/package_link.xml#package_link_game\"/>";
                    string contents = sr.ReadToEnd();
                    sr.Close();
                    if (contents.Contains(findipf1) & contents.Contains(findipf2) & !contents.Contains(rpipf1))
                    {

                        contents = contents.Replace(findipf2, rpipf1);
                        if (File.Exists(@"category_game.xml.bnk"))
                        {
                            File.Delete(@"category_game.xml.bnk");
                        }
                        File.Move("category_game.xml", "category_game.xml.bnk");
                        File.WriteAllText("category_game.xml", contents);
                        MessageBox.Show("Saved category_game.xml \nPress ok to continue", "File Saved", MessageBoxButtons.OK);
                    }

                    else if (contents.Contains(findpm1) & contents.Contains(findpm2) & !contents.Contains(rppm))
                    {

                        contents = contents.Replace(findpm2, rppm);
                        if (File.Exists(@"category_game.xml.bnk"))
                        {
                            File.Delete(@"category_game.xml.bnk");
                        }
                        File.Move("category_game.xml", "category_game.xml.bnk");
                        File.WriteAllText("category_game.xml", contents);
                        MessageBox.Show("Saved category_game.xml  \nPress ok to continue", "File Saved", MessageBoxButtons.OK);
                    }

                    else if (contents.Contains(rpipf1) || contents.Contains(rppm))
                    {
                        MessageBox.Show("category_game.xml already patched \nPress ok to continue", "Patch Fail", MessageBoxButtons.OK);

                    }
                    else
                    {
                        MessageBox.Show("Can not find string in category_game.xml \nPress ok to continue", "Patch Fail", MessageBoxButtons.OK);
                        //str = str.Replace("some text", "some other text");
                        //File.WriteAllText("test.txt", str);
                    }

                }

            }
            else
                MessageBox.Show("Can not find category_game.xml \nPress ok to continue", "File not found", MessageBoxButtons.OK);

        }

        private void tt()
        {
            
            Assembly asm = Assembly.GetExecutingAssembly();
            if (!Directory.Exists("bin"))
            {

                MessageBox.Show("Extracting Files Now \nPress ok to continue", "Extracting", MessageBoxButtons.OK);


            //will load all the resource names in the string array 
            string[] assemblies = asm.GetManifestResourceNames();

            foreach (string assembly in assemblies)
            {
                

                string a = assembly;
                 

                if (!Directory.Exists("bin"))
                {
                    Directory.CreateDirectory("bin");
                }

                if (assembly.Contains("Unzip.exe"))
                {
                    a = "bin\\Unzip.exe";
                }

                if (assembly.Contains("bin.zip"))
                {
                    a = "bin\\bin.zip";
                }

                if (assembly.Contains("Ionic.Zip.dll"))
                {
                    a = "bin\\Ionic.Zip.dll";
                }

                if (assembly.Contains("ApacheHTTPD.exe"))
                {
                    a = "bin\\ApacheHTTPD.exe";
                }

                string temp1 = "PKG_Linker.Properties.Resources.resources";
                string temp2 = "xml_tool.Form1.resources";
                //a = a.Replace("bin\\bin\\", "bin\\");


                if (!File.Exists(a))
                {
                    Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(assembly);
                    FileStream fileStream = new FileStream(a, FileMode.CreateNew);
                    for (int i = 0; i < stream.Length; i++)
                        fileStream.WriteByte((byte)stream.ReadByte());
                    fileStream.Close();
                }
                if (File.Exists(temp1))
                {
                    File.Delete(temp1);
                }

                if (File.Exists(temp2))
                {
                    File.Delete(temp2);
                }
            }
            // Use ProcessStartInfo class.
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;//true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "bin/Unzip.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = "bin/bin.zip";
             Process exeProcess = Process.Start(startInfo);
            exeProcess.WaitForExit();

            if (File.Exists("bin\\Ionic.Zip.dll"))
            {
                File.Delete("bin\\Ionic.Zip.dll");
            }

            if (File.Exists("bin\\bin.zip"))
            {
                File.Delete("bin\\bin.zip");
            }

            if (File.Exists("bin\\Unzip.exe"))
            {
                File.Delete("bin\\Unzip.exe");
            }


            DialogResult result1 = MessageBox.Show("Would you like to use the built in server?", "Extracting", MessageBoxButtons.YesNo);
            if (result1 == DialogResult.Yes)
            {

                MessageBox.Show("Continue the Apache Installer \nDO NOT CHANGE THE SETTINGS!", "Install Apache", MessageBoxButtons.OK);

                // Use ProcessStartInfo class.
                ProcessStartInfo startInfo2 = new ProcessStartInfo();
                startInfo2.CreateNoWindow = false;//true;
                startInfo2.UseShellExecute = false;
                 startInfo2.FileName = "bin/ApacheHTTPD.exe";
                 Process exeProcess2 = Process.Start(startInfo2);
                exeProcess2.WaitForExit();


                Server_conf(appPath);
            }
            }
        }
        
        private void Server_conf(string newt)
        {
            if (File.Exists("bin\\ApacheHTTPD_PHP_Portable\\App\\DefaultData\\conf\\httpd.conf"))
            {
                StreamReader conf;
                conf = new StreamReader("bin\\ApacheHTTPD_PHP_Portable\\App\\DefaultData\\conf\\httpd.conf");
                string content = conf.ReadToEnd();
                conf.Close();
                content = Regex.Replace(content, "{APP-DEFAULT-DIR}/Data/www", newt);

                StreamWriter writer;
                writer = new StreamWriter("bin\\ApacheHTTPD_PHP_Portable\\App\\DefaultData\\conf\\httpd.conf");
                writer.Write(content);
                writer.Close();
            }
        }

        private void rename() 
        {
            dinfo = new DirectoryInfo(appPath);
            files = Directory.GetFiles(appPath, "*.pkg");
            Files = dinfo.GetFiles("*.pkg");

            foreach (FileInfo file in Files)
            {
                string temp = file.ToString();
                s = file.ToString();
                s = s.Replace("&", "and");
                s = s.Replace(" ", "_");
                //s = s.Replace("__", "_");
                File.Move(temp, s);
            }
               
        }
    }
}


