using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBCmerger
{
    public partial class Form1 : Form
    {
        string CANfileName1 = "";
        string CANfileName2 = "";
        public String line;
        public StreamWriter merged_DBC;
        public StringBuilder sbWrite = new StringBuilder();
        public int DBC1_CM = 0;
        public int DBC2_CM = 0;
        public Form1()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "CAN Database files (*.dbc)|*.dbc";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                CANfileName1 = textBox1.Text;
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog2 = new OpenFileDialog();

            openFileDialog2.InitialDirectory = "c:\\";
            openFileDialog2.Filter = "CAN Database files (*.dbc)|*.dbc";
            openFileDialog2.FilterIndex = 0;
            openFileDialog2.RestoreDirectory = true;

            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = openFileDialog2.FileName;
                CANfileName2 = textBox2.Text;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                MessageBox.Show("Please enter a valid file name!");
                return;
            }
            //sbwrite temizle
            sbWrite.Clear();

            MergeInitial();
            MergeBU();
            MergeMessages();
            MergeLast("BA_");
            MergeLast("VAL_");
            MergeLast("SIG_GROUP_ ");
            MergeLast("GOD");

            merged_DBC = new StreamWriter(textBox3.Text+".dbc");
            merged_DBC.WriteLine(sbWrite);
            merged_DBC.Close();

            MessageBox.Show("DBC file loaded succesfully.      ");

        }

    private void MergeInitial()
        {
            using (StreamReader sr = new StreamReader(CANfileName1))
            {

                System.IO.FileInfo ff = new System.IO.FileInfo(CANfileName1);
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("BU_"))
                    {
                        break;
                    }
                    sbWrite.Append(line);
                    sbWrite.Append("\n");
                }
            }
        }

        private void MergeBU()
        {

            //TODO: BU_2 node must be added

            using (StreamReader sr = new StreamReader(CANfileName1))
            {

                System.IO.FileInfo ff = new System.IO.FileInfo(CANfileName1);
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("BU_"))
                    {
                        sbWrite.Append("\n");
                        sbWrite.Append(line);

                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.StartsWith("BO_ "))
                            {
                                break;
                            }

                            sbWrite.Append("\n");
                            sbWrite.Append(line);
                        }
                    }
                    
                }
            }

            using (StreamReader sr = new StreamReader(CANfileName2))
            {

                System.IO.FileInfo ff = new System.IO.FileInfo(CANfileName2);
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("BU_"))
                    {
                        //  sbWrite.Append("\n");

                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.StartsWith("BO_ "))
                            {
                                break;
                            }

                            sbWrite.Append("\n");
                            sbWrite.Append(line);
                        }
                    }

                }
            }
        }


        private void MergeMessages()
    {
        int count = 0;
            
            using (StreamReader sr = new StreamReader(CANfileName1))
        {

            System.IO.FileInfo ff = new System.IO.FileInfo(CANfileName1);
            while ((line = sr.ReadLine()) != null)
            {
                if (line.StartsWith("BO_ "))
                {
                    sbWrite.Append("\n");
                    sbWrite.Append("\n");
                    sbWrite.Append(line);
                    //merged_DBC.WriteLine(line);
                }
                else if (line.StartsWith(" SG_ "))
                {
                    sbWrite.Append("\n");
                    sbWrite.Append(line);
                    DBC1_CM = count;
                    //merged_DBC.WriteLine(line);
                }
                    count++;
                }
        }

        using (StreamReader sr = new StreamReader(CANfileName2))
        {
                count = 0;
                System.IO.FileInfo ff = new System.IO.FileInfo(CANfileName2);
                          
            while ((line = sr.ReadLine()) != null)
            {
                if (line.StartsWith("BO_ "))
                {
                    sbWrite.Append("\n");
                    sbWrite.Append("\n");
                    sbWrite.Append(line);
                    //merged_DBC.WriteLine(line);
                }
                else if (line.StartsWith(" SG_ "))
                {
                    sbWrite.Append("\n");
                    sbWrite.Append(line);
                        DBC2_CM = count;
                        //merged_DBC.WriteLine(line);
                }
                    count++;
                }
        }
    }

        private void MergeLast(string segment)
        {
            int count = 0;

            using (StreamReader sr = new StreamReader(CANfileName1))
            {

                System.IO.FileInfo ff = new System.IO.FileInfo(CANfileName1);
                while ((line = sr.ReadLine()) != null)
                {
                    if (count == DBC1_CM)
                    {
                        while ((line = sr.ReadLine()) != null)
                        {

                            if (line.StartsWith(segment))
                            {
                                DBC1_CM = count;
                                break;        
                            }

                            count++;
                            sbWrite.Append("\n");
                            sbWrite.Append(line);
                        }
                    }
                    count++;
                }
            }



            using (StreamReader sr = new StreamReader(CANfileName2))
            {
                count = 0;
                System.IO.FileInfo ff = new System.IO.FileInfo(CANfileName2);
               while ((line = sr.ReadLine()) != null)
                {
                    if (count == DBC2_CM)
                    {
                        while ((line = sr.ReadLine()) != null)
                            {                            
                                
                            if (line.StartsWith(segment))
                            {
                                DBC2_CM = count;
                                break;
                            }

                            count++;
                            if ((segment != "BA_") && (segment != "VAL_"))
                            {
                            sbWrite.Append("\n");
                            sbWrite.Append(line);

                     
                            }
                    }                    
                }
                    count++; 
                    //commit2 
                }
            }
        }
    }
}
