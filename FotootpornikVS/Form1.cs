using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Ports;
using System.Windows.Forms.DataVisualization.Charting;


namespace FotootpornikVS
{
    public partial class Form1 : Form
    {

      //  private double[] cpuArray = new double[60];

        private SerialPort myport;
        private DateTime datetime;
        private string in_data;
        int baud = 0;

        public Form1()
        {
            InitializeComponent();
/*
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(label9);

            groupBox1.Controls.Add(aGauge1);
            groupBox1.Controls.Add(aGauge2);
            groupBox1.Controls.Add(aGauge3);
            groupBox1.Controls.Add(aGauge4);
*/


            data_tb.Enabled = false; //Zasenjuje klikabilnost dugmeta ovaj kod
            stop_btn.Enabled = false;
            save_btn.Enabled = false;
            comboBox2.Items.Add("9600");
            comboBox2.Items.Add("19200");
            comboBox2.Items.Add("38400");
            comboBox2.Items.Add("57600");
            comboBox2.Items.Add("74880");
            comboBox2.Items.Add("115200");
            comboBox2.Items.Add("230400");
            comboBox2.Items.Add("250000");
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)


                


            {
                comboBox1.Items.Add(port);
            }   
              


        }
        protected override CreateParams CreateParams
        {
            get
            {
                const int CP_NOCLOSE = 0x200;
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE;
                return myCp;
            }
        }

        private void start_btn_Click_1(object sender, EventArgs e)
        {         
            try
            {
                    Int32.TryParse(comboBox2.Text, out baud);
                    myport = new SerialPort();
                    myport.BaudRate = baud;
                    myport.PortName = comboBox1.Text;
                    myport.Parity = Parity.None;
                    myport.DataBits = 8;
                    myport.StopBits = StopBits.One;
                    myport.DataReceived += myport_DataReceived;
                    myport.Open();
                    data_tb.Text = "";
                    start_btn.Enabled = false;
                    button1.Enabled = false;
                    stop_btn.Enabled = true;
                    save_btn.Enabled = true;
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    timer1.Enabled = true;
               

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška se javila, molimo unesite parametre za konekciju.");
            }
  
        }
//
        void myport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            in_data = myport.ReadLine();
            this.Invoke(new EventHandler(displaydata_event));
        }

        private void displaydata_event(object sender, EventArgs e)
        {
            datetime = DateTime.Now;
            string time = datetime.Day + "." + datetime.Month + "." + datetime.Year + "       " + datetime.Hour + ":" + datetime.Minute + ":" + datetime.Second;
            data_tb.AppendText(time + "              " + in_data + "\n");
            
            String dataFromArduino = myport.ReadLine().ToString();
            String[] dataSaPorta = dataFromArduino.Split(',');

            int potenciometar1 = (int)(Math.Round(Convert.ToDecimal(dataSaPorta[0]), 0));
            int potenciometar2 = (int)(Math.Round(Convert.ToDecimal(dataSaPorta[1]), 0));
            int potenciometar3 = (int)(Math.Round(Convert.ToDecimal(dataSaPorta[2]), 0));
            int potenciometar4 = (int)(Math.Round(Convert.ToDecimal(dataSaPorta[3]), 0));

            aGauge1.Value = potenciometar1;
            aGauge2.Value = potenciometar2;
            aGauge3.Value = potenciometar3;
            aGauge4.Value = potenciometar4;


            label10.Text = Convert.ToString(potenciometar1);
            label11.Text = Convert.ToString(potenciometar2);
            label12.Text = Convert.ToString(potenciometar3);
            label13.Text = Convert.ToString(potenciometar4);

            chart1.Series["t Kljuka"].Points.AddXY(time, potenciometar1);
            chart1.Series["t Hladnjaka"].Points.AddXY(time, potenciometar2);
            chart1.Series["Protok destilata"].Points.AddXY(time, potenciometar3);
            chart1.Series["Viskozitet kljuka"].Points.AddXY(time, potenciometar4);


            chart1.Series["t Kljuka"].ChartType = SeriesChartType.FastLine;
            chart1.Series["t Kljuka"].Color = Color.Red;

            chart1.Series["t Hladnjaka"].ChartType = SeriesChartType.FastLine;
            chart1.Series["t Hladnjaka"].Color = Color.Blue;

            chart1.Series["Protok destilata"].ChartType = SeriesChartType.FastLine;
            chart1.Series["Protok destilata"].Color = Color.ForestGreen;

            chart1.Series["Viskozitet kljuka"].ChartType = SeriesChartType.FastLine;
            chart1.Series["Viskozitet kljuka"].Color = Color.Black;
            
        }
        //


        private void stop_btn_Click(object sender, EventArgs e)
        {
            try
            {
                
                myport.Close();
                start_btn.Enabled = true;
                stop_btn.Enabled = false;
                button1.Enabled = true;
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                //     comboBox1.SelectedIndex = -1;
                //    comboBox2.SelectedIndex = -1;
                MessageBox.Show("Serijski port je prekinut");
                

            }
            catch (Exception ex3)
            {
                MessageBox.Show(ex3.Message, "Greška se javila, molimo resetujte program");
            }
        }
        private void save_btn_Click(object sender, EventArgs e)
        {
            try
            {

                string userName = Environment.UserName;
                string pathfile = @"C:\\Users\\" + userName + "\\Desktop\\";

                string filename = "Podatci očitani senzorom.txt";
                System.IO.File.WriteAllText(pathfile + filename, data_tb.Text);
                MessageBox.Show("Podatci su snimljeni u: " + pathfile, "Sačuvavanje podataka u .txt format");
                
            }
            catch (Exception ex4)
            {
                MessageBox.Show(ex4.Message, "Greška se javila, proverite šta nije u redu");
            }
        }

        private void aGauge1_ValueInRangeChanged(object sender, ValueInRangeChangedEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Dali želite da izađete iz programa ?", "EXIT Taster", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Application.Exit();
            }

        }


        private void button2_Click(object sender, EventArgs e)
        {

            

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void aGauge3_ValueInRangeChanged(object sender, ValueInRangeChangedEventArgs e)
        {

        }
    }
}
