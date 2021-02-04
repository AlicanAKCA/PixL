using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AnimatedGif ;
//Alican AKCA
//4.02.2021
namespace PixL
{
    public partial class Form1 : Form
    {
        int whichOne = 0; //1 = OFD, 2=FV 
        string fileType; // type of selected file
        int imgWidth = 500; //Resized Image Width
        int imgHeight = 500; //Resized Image Height
        Boolean Square; //For control function
        Bitmap bmp1;
        Bitmap bmp2;
        string fullPath;
        System.Drawing.Image img;
        public Dictionary<int, int> imgR = new Dictionary<int, int>();
        public Dictionary<int, int> imgG = new Dictionary<int, int>();
        public Dictionary<int, int> imgB = new Dictionary<int, int>();
        int bb = 0;
        int k = 0;
        Graphics GFX;
        string filesDir_Dir;
        public Form1()
        {
            InitializeComponent();
        }
       
            private void Form1_Load(object sender, EventArgs e)
        {
            mainBoxPixL.SizeMode = PictureBoxSizeMode.Zoom;
            mainBox.SizeMode = PictureBoxSizeMode.Zoom;
            fileTypeIconPBox.SizeMode = PictureBoxSizeMode.Zoom;
        }
        public void LoadDirectory(string Dir)
        {
            DirectoryInfo di = new DirectoryInfo(Dir);
            //Setting ProgressBar Maximum Value  
            progressBar1.Maximum = Directory.GetFiles(Dir, "*.*", SearchOption.AllDirectories).Length + Directory.GetDirectories(Dir, "**", SearchOption.AllDirectories).Length;
            TreeNode tds = filesDir.Nodes.Add(di.Name);
            tds.Tag = di.FullName;
            tds.StateImageIndex = 0;
            LoadFiles(Dir, tds);
            LoadSubDirectories(Dir, tds);
        }
        private void LoadSubDirectories(string dir, TreeNode td)
        {
            // Get all subdirectories  
            string[] subdirectoryEntries = Directory.GetDirectories(dir);
            // Loop through them to see if they have any other subdirectories  
            foreach (string subdirectory in subdirectoryEntries)
            {

                DirectoryInfo di = new DirectoryInfo(subdirectory);
                TreeNode tds = td.Nodes.Add(di.Name);
                tds.StateImageIndex = 0;
                tds.Tag = di.FullName;
                LoadFiles(subdirectory, tds);
                LoadSubDirectories(subdirectory, tds);
                UpdateProgress();

            }
        }
        private void LoadFiles(string dir, TreeNode td)
        {
            string[] Files = Directory.GetFiles(dir, "*.*");

            // Loop through them to see files  
            foreach (string file in Files)
            {
                FileInfo fi = new FileInfo(file);
                TreeNode tds = td.Nodes.Add(fi.Name);
                tds.Tag = fi.FullName;
                tds.StateImageIndex = 1;
                UpdateProgress();

            }
        }
        private void UpdateProgress()
        {

            try {
                if (progressBar1.Value < progressBar1.Maximum)
                {
                    progressBar1.Value++;
                    int percent = (int)(((double)progressBar1.Value / (double)progressBar1.Maximum) * 100);
                    progressBar1.CreateGraphics().DrawString(percent.ToString() + "%", new Font("Arial", (float)8.25, FontStyle.Regular), Brushes.Black, new PointF(progressBar1.Width / 2 - 10, progressBar1.Height / 2 - 7));

                    Application.DoEvents();
                }
            }
            catch { }
         }
        private void filesDir_MouseMove(object sender, MouseEventArgs e)
        { 

           // Get the node at the current mouse pointer location.  
        TreeNode theNode = this.filesDir.GetNodeAt(e.X, e.Y);  
  
           // Set a ToolTip only if the mouse pointer is actually paused on a node.  
           if (theNode != null && theNode.Tag != null)  
           {  
               // Change the ToolTip only if the pointer moved to a new node.  
               if (theNode.Tag.ToString() != this.toolTip1.GetToolTip(this.filesDir))  
                   this.toolTip1.SetToolTip(this.filesDir, theNode.Tag.ToString());

            }  
           else     // Pointer is not over a node so clear the ToolTip.  
           {  
               this.toolTip1.SetToolTip(this.filesDir, "");
           }
        }

        private void FilesDir_AfterSelect(object sender, TreeViewEventArgs e)
        {
            fullPath = Path.Combine( //It gets the directory of the selected file
      Directory.GetParent(filesDir_Dir).FullName,
      filesDir.SelectedNode.FullPath);
            fileType = Path.GetExtension(fullPath);
            if (fileType !=".gif")
            {

                fileTypeIconPBox.Image = ımageList3.Images[0]; //GIF icon
                fileNameLabel.Text = filesDir_Dir; //Directory
                fileTypeLabel.Text = "Type of File: " + fileType; //Type

            }
         else
            {
                fileTypeIconPBox.Image = ımageList3.Images[1]; //Image icon
                fileNameLabel.Text = filesDir_Dir; //Directory
                fileTypeLabel.Text = "Type of File: " + fileType; //Type
            }         
            try { mainBox.Load(fullPath); } catch { }
        }
        Image imgx;
        private void PictureBox5_Click(object sender, EventArgs e)
        {
            whichOne = 2;
            //Will see files of directories.
            //filesDir_Dir is a main directory that is required

            filesDirDialog.SelectedPath = filesDir_Dir;
            DialogResult drResult = filesDirDialog.ShowDialog();
            if (drResult == System.Windows.Forms.DialogResult.OK)
                filesDir_Dir = filesDirDialog.SelectedPath;
            else
            filesDir.Nodes.Clear();
            if (filesDir_Dir != "" && Directory.Exists(filesDir_Dir))
                LoadDirectory(filesDir_Dir);
        }

        private void LoadImage()
        {
            //Will load image
            try
            {
                if (whichOne ==1)
                {
                    fullPath = openFileDialog1.FileName.ToString();
                    tabControl2.Tabs[0].Text = openFileDialog1.SafeFileName.ToString();
                    tabControl2.Tabs[1].Text = openFileDialog1.SafeFileName.ToString() + "*";
                }
                else {
                    fullPath = Path.Combine(
Directory.GetParent(filesDir_Dir).FullName,
filesDir.SelectedNode.FullPath);
                    tabControl2.Tabs[0].Text = filesDir.SelectedNode.Text.ToString();
                    tabControl2.Tabs[1].Text = filesDir.SelectedNode.Text.ToString() + "*";
                }
                img = System.Drawing.Image.FromFile(fullPath);
                imgx = ResizeImage(img, 500, 500);
                mainBox.Image = imgx;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public Boolean squareImgControl() //Control Function
        {
            if (img.Width == img.Height)
            {
                Square = true;
            }
            return Square;
        }
        int halfOfSquare = 5;

        private void pickPix()//Select a pixel of this image (imgx)
        {
            bmp1 = new Bitmap(imgx);
            
            for (int y = halfOfSquare; y <= imgHeight - halfOfSquare;)
            {
                for (int i = halfOfSquare; i <= imgWidth - halfOfSquare;)
                {
                    Color x = bmp1.GetPixel(i, y);
                    imgR.Add(bb, x.R); //Save a R code
                    imgG.Add(bb, x.G);//Save a G code
                    imgB.Add(bb, x.B); //Save a B code
                    bb += 1;
                    i += halfOfSquare * 2;
                }
                y += halfOfSquare * 2;
            }
        }
        private void applyPix()
        { //Will create a new bitmap that choosed sizes
            bmp2 = new Bitmap(imgWidth, imgHeight);
            GFX = Graphics.FromImage(bmp2);
            for (int ya = 0; ya <= imgHeight - 1;)
            {
                for (int xa = 0; xa <= imgWidth - 1;)
                {
                    if (xa == imgWidth) { break; }
                    SolidBrush sBrush = new SolidBrush(Color.FromArgb(imgR[k], imgG[k], imgB[k]));
                    GFX.FillRectangle(sBrush, xa + 1, ya + 1, 2 * halfOfSquare, 2 * halfOfSquare);
                    k += 1;
                    xa += 2 * halfOfSquare;


                }
                ya += 2 * halfOfSquare;
                if (ya == imgHeight + 2 * halfOfSquare) { break; }
            }
            mainBoxPixL.Image = bmp2;
            mainBoxPixL.Image.Save(Application.StartupPath + "\\Saves\\" + "myEpicIMAGE.jpg");

        }
        public static Bitmap ResizeImage(Image image, int width, int height)//Resize Function
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(500, 500);

            try { destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                using (var graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }
            } catch {  }

           
            return destImage;
        }

        private Image[] getFrames(Image originalImg)
        {
            int numberOfFrames = originalImg.GetFrameCount(FrameDimension.Time);
            Bitmap[] frames = new Bitmap[numberOfFrames];
            for (int i = 0; i < numberOfFrames -1;)
            {
                    originalImg.SelectActiveFrame(FrameDimension.Time, i);
                    frames[i] = ((Bitmap)originalImg.Clone());
                    i += 1;

            }
            return frames;
        }
        private void FilesDir_DoubleClick(object sender, EventArgs e) //Way 2 of these
        {
            fullPath = Path.Combine(
  Directory.GetParent(filesDir_Dir).FullName,
  filesDir.SelectedNode.FullPath);
            fileType = Path.GetExtension(fullPath);
            if (fileType != ".gif")
            {

                fileTypeIconPBox.Image = ımageList3.Images[0];
                fileNameLabel.Text = filesDir_Dir;
                fileTypeLabel.Text = "Type of File: " + fileType;
                LoadImage();
                squareImgControl();
                pickPix();
                applyPix();
            }
            else
            {
                fileTypeIconPBox.Image = ımageList3.Images[1];
                fileNameLabel.Text = filesDir_Dir;
                fileTypeLabel.Text = "Type of File: " + fileType;
                allProcessesForGIF();
            }
        }

        private void allProcessesForGIF() //GIF->Frames->PixedIMG->Frames->GIF
        {
            fullPath = Path.Combine(
          Directory.GetParent(filesDir_Dir).FullName,
          filesDir.SelectedNode.FullPath);
            fileType = Path.GetExtension(fullPath);
            if (fileType != ".gif")
            {

                fileTypeIconPBox.Image = ımageList3.Images[0];
                fileNameLabel.Text = filesDir_Dir;
                fileTypeLabel.Text = "Type of File: " + fileType;

            }
            else
            {
                fileTypeIconPBox.Image = ımageList3.Images[1];
                fileNameLabel.Text = filesDir_Dir;
                fileTypeLabel.Text = "Type of File: " + fileType;
            }
            if (fileType == ".gif")
            {
                Image[] framesX = getFrames(Image.FromFile(fullPath));
                foreach (var framesInImageList in framesX)
                {
                    Image res = ResizeImage(framesInImageList, 500, 500);
                    ımageList1.Images.Add(res);
                }
                _ = new Bitmap(500, 500);
                foreach (Image pixImg in ımageList1.Images)
                {
                    Bitmap pixImgX = new Bitmap(ResizeImage(pixImg, 500, 500));
                    for (int y = 5; y <= 500;)
                    {
                        for (int i = 5; i <= 500;)
                        {
                            Color x = pixImgX.GetPixel(i, y);
                            imgR.Add(bb, x.R);
                            imgG.Add(bb, x.G);
                            imgB.Add(bb, x.B);
                            bb += 1;
                            i += 10;
                        }
                        y += 10;
                    }
                    Bitmap bXX = new Bitmap(500, 500);
                    GFX = Graphics.FromImage(bXX);
                    for (int ya = -1; ya <= 498;)
                    {
                        for (int xa = -1; xa <= 498;)
                        {
                            SolidBrush sBrush = new SolidBrush(Color.FromArgb(imgR[k], imgG[k], imgB[k]));
                            GFX.FillRectangle(sBrush, xa + 1, ya + 1, 10, 10);
                            k += 1;
                            xa += 10;
                            if (xa == 510) { break; }
                        }
                        ya += 10;
                        if (ya == 510) { break; }
                    }
                    ımageList2.Images.Add(bXX);

                }
                using (var gif = AnimatedGif.AnimatedGif.Create("myEpicGIF.gif", 16))
                {
                    foreach (Image framesToGif in ımageList2.Images)
                    {
                        gif.AddFrame(framesToGif, delay: -1);
                    }

                }
                mainBox.Load(fullPath);
                mainBoxPixL.Load(Application.StartupPath + "\\myEpicGIF.gif");
            }
            else
            {
                if (Square == true)
                {
                    generalCleaning();
                    LoadImage();
                    squareImgControl();
                    pickPix();
                    applyPix();
                }
                else
                {
                    MessageBox.Show("It will be resize to square rate.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    generalCleaning();
                    LoadImage();
                    squareImgControl();
                    pickPix();
                    applyPix();
                }
            }
        }

        private void PictureBox6_Click(object sender, EventArgs e) //Way 1 of these
        {
            whichOne = 1;
            openFileDialog1.ShowDialog();
            fullPath = openFileDialog1.FileName;          
            fileType = Path.GetExtension(fullPath);
            if (fileType != ".gif")
            {
                fileTypeIconPBox.Image = ımageList3.Images[0];
                fileNameLabel.Text = filesDir_Dir;
                fileTypeLabel.Text = "Type of File: " + fileType;
                generalCleaning();
                LoadImage();
                squareImgControl();
                pickPix();
                applyPix();

            }
            else
            {
                fileTypeIconPBox.Image = ımageList3.Images[1];
                fileNameLabel.Text = filesDir_Dir;
                fileTypeLabel.Text = "Type of File: " + fileType;
                generalCleaning();
                allProcessesForGIF();
            }
        }
        private void generalCleaning()
        {
            bb = 0;
            k = 0;
            imgR.Clear();
            imgG.Clear();
            imgB.Clear();
            ımageList1.Images.Clear();
            ımageList2.Images.Clear();
            imgWidth = 500;
            imgHeight = 500;
            filesDir_Dir = "";
            fileType = "";
        }
    }
}
