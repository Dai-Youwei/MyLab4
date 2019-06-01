using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ZedGraph;
using System.IO.Ports;
using Binding = System.Windows.Data.Binding;
using TextBox = System.Windows.Controls.TextBox;
using ComboBox = System.Windows.Controls.ComboBox;
using MessageBox = System.Windows.MessageBox;
using Color = System.Drawing.Color;
using System.IO;

namespace Lab4
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private byte yellowValue, greenValue, blueValue, redValue, whiteValue;
        SerialPort myPort = new SerialPort();
        Format recData = new Format();
        Format senData = new Format();
        Format pwm = new Format();       
        int tickStart = 0;
        LogFile logFile = new LogFile();
        private StreamWriter sw1 = null;
        private bool isRecord = false;
        public MainWindow()
        {
            InitializeComponent();

            //添加波特率
            combo2.Items.Clear();
            combo2.Items.Add("9600");
            combo2.Items.Add("19200");
            combo2.Items.Add("38400");
            combo2.Items.Add("57600");
            combo2.SelectedItem = combo2.Items[0];
            graphInit(); //初始化Graph
            Binding binding = new Binding("ReceiveMessage");
            binding.Source = recData;
            binding.Mode = BindingMode.OneWay;
            receiveData.SetBinding(TextBox.TextProperty, binding);
            
            binding = new Binding("SendMessage");
            binding.Source = senData;
            binding.Mode = BindingMode.OneWayToSource;
            sendData.SetBinding(TextBox.TextProperty, binding);
            
            binding = new Binding("getDegree");
            binding.Source = recData;
            binding.Mode = BindingMode.OneWay;
            textBox1.SetBinding(TextBlock.TextProperty, binding);
            
            binding = new Binding("getLight");
            binding.Source = recData;
            binding.Mode = BindingMode.OneWay;
            textBox2.SetBinding(TextBlock.TextProperty, binding);
            
        yellow.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(S_MouseLeftButtonUp), true);
            green.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(S_MouseLeftButtonUp), true);
            blue.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(S_MouseLeftButtonUp), true);
            red.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(S_MouseLeftButtonUp), true);
            white.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(S_MouseLeftButtonUp), true);
        }

        private void graphInit()
        {
            GraphPane graph = zedgraph.GraphPane;

            graph.Title.Text = "温度&光强实时动态图";
            graph.Title.FontSpec.Size = 30;
            graph.XAxis.Title.Text = "时间";
            graph.XAxis.Title.FontSpec.Size = 20;
            graph.YAxis.Title.Text = "数值";
            graph.YAxis.Title.FontSpec.Size = 20;
            RollingPointPairList list1 = new RollingPointPairList(1200);
            RollingPointPairList list2 = new RollingPointPairList(1200);
            LineItem tempre = graph.AddCurve("温度", list1, System.Drawing.Color.Yellow, SymbolType.None);
            LineItem light = graph.AddCurve("光强", list2, System.Drawing.Color.Black, SymbolType.None);
            graph.XAxis.Scale.Min = 0;
            graph.XAxis.Scale.MaxGrace = 0.01;
            graph.XAxis.Scale.MaxGrace = 0.01;
            graph.XAxis.Scale.Max = 30;
            graph.XAxis.Scale.MinorStep = 1;
            graph.XAxis.Scale.MajorStep = 5;

            tickStart = Environment.TickCount;
            zedgraph.AxisChange();
        }

        private void setBind(Slider slider)
        {
            Binding binding = new Binding("Pin");
            binding.Source = senData;
            binding.Mode = BindingMode.OneWayToSource;
            slider.SetBinding(Slider.TagProperty, binding);

            binding = new Binding("State");
            binding.Source = senData;
            binding.Mode = BindingMode.OneWayToSource;
            slider.SetBinding(Slider.ValueProperty, binding);
        }
        
        public void portName_DropDownOpened(object sender, EventArgs e)
        {
            string[] portStr = SerialPort.GetPortNames();
            ComboBox combo = sender as ComboBox;
            combo.Items.Clear();
            foreach (string str in portStr)
            {
                combo.Items.Add(str);
            }

        }
         
        private void close(object sender, RoutedEventArgs e)
        {
            if (myPort != null)
            {
                myPort.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);
                myPort.Close();
                MessageBox.Show("已断开");
            }
        }

        private void open(object sender, RoutedEventArgs e)
        {
            if (combo1.SelectedItem != null)
            {
                if (myPort != null)
                {
                    myPort.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);
                    myPort.Close();
                }
                myPort = new SerialPort(combo1.SelectedItem.ToString());
                myPort.BaudRate = int.Parse(combo2.SelectedItem.ToString());
                myPort.Parity = Parity.None;
                myPort.StopBits = StopBits.One;
                myPort.DataBits = 8;
                myPort.Handshake = Handshake.None;
                myPort.RtsEnable = false;
                myPort.ReceivedBytesThreshold = 1;
                myPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                myPort.Open();
                MessageBox.Show("已连接");

            }
        }

        private void display()
        {
            if ((recData.getReceive[0] & 0xf0) == 0xe0)
            {
                if ((recData.getReceive[0] & 0xf) == 0)
                {
                    recData.getDegree = recData.byte_to_int;
                    setReturnTextBlock(textBox1, recData.getDegree);
                }
                else if ((recData.getReceive[0] & 0xf) == 1)
                {
                    recData.getLight = recData.byte_to_int;
                    setReturnTextBlock(textBox2, recData.getLight);
                }
            }
        }
        
        private void DataSend(object sender, RoutedEventArgs e)
        {
            byte[] data = senData.SendMessageToByte;
            
            if (data != null && myPort != null && myPort.IsOpen)
            {
                myPort.Write(data, 0, data.Length);
            }
        }

        private void ColorSend()
        {
            try
            {
                yellowValue = (byte)yellow.Value;
                redValue = (byte)red.Value;
                greenValue = (byte)green.Value;
                blueValue = (byte)blue.Value;
                whiteValue = (byte)white.Value;                

                pwm.PWMFormat(yellowValue, 3);
                myPort.Write(pwm.getSend, 0, 3);
                pwm.PWMFormat(greenValue, 5);
                myPort.Write(pwm.getSend, 0, 3);
                pwm.PWMFormat(blueValue, 6);
                myPort.Write(pwm.getSend, 0, 3);
                pwm.PWMFormat(redValue, 9);
                myPort.Write(pwm.getSend, 0, 3);
                pwm.PWMFormat(whiteValue, 10);
                myPort.Write(pwm.getSend, 0, 3);
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (myPort == null) return;
            int byte_size = myPort.BytesToRead;
            for (int i = 0; i < byte_size; i++)
            {
                int inByte = myPort.ReadByte();
                if ((inByte & 0x80) == 0x80)
                {
                    recData.index = 0;
                    recData.getReceive[recData.index] = (byte)inByte;
                    recData.index++;
                }
                else if (recData.index != 0 && recData.index < recData.getReceive.Length)
                {
                    recData.getReceive[recData.index] = (byte)inByte;
                    recData.index++;
                }
                if (recData.index == 3)
                {                               
                    string str1 = string.Format("\n 接收数据:{0:X2}-{1:X2}-{2:X2}",
                        recData.getReceive[0], recData.getReceive[1],
                        recData.getReceive[2]);
                    setReturnTextBox(receiveData, str1);
                    string str2 = string.Format("\n 发送数据：0x{0:X4}",recData.byte_to_int);
                    setReturnTextBox(sendData,str2);
                    display();
                    if ((recData.getReceive[0] & 0xf0) == 0xe0)
                        drawline(recData.getReceive[0] & 0xf, recData.byte_to_int);
                    if (click == 1)
                    {
                        logFile.AD = recData.getReceive;
                        logFile.PWM = senData.getSend;
                        logFile.setString();
                    }
                }
            }
        }

        private delegate void setTextBox(TextBox textBox, string s);
        
        public void setReturnTextBox(TextBox textBox, string s)
        {
            if (textBox.Dispatcher.CheckAccess())
            {
                textBox.AppendText(s);
                textBox.ScrollToEnd();
            }
            else
            {
                setTextBox setText = new setTextBox(setReturnTextBox);
                Dispatcher.Invoke(setText, new object[] { textBox, s });
            }
        }

        
        private delegate void setTextBlock(TextBlock textBlock, int num);

        public void setReturnTextBlock(TextBlock textBlock, int num)
        {
            if (textBlock.Dispatcher.CheckAccess())
            {
                //num /= 10;
                textBlock.Text = num.ToString();
            }
            else
            {
                setTextBlock set = new setTextBlock(setReturnTextBlock);
                Dispatcher.Invoke(set, new object[] { textBlock, num });
            }
        }

        private void ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Color Red = Color.Red;
            Color Green = Color.Green;
            Color Yellow = Color.Yellow;
            Color Blue = Color.Blue;
            Color White = Color.White;
            double r =  Yellow.R * yellow.Value / 255 + Green.R * green.Value / 255 +
                Blue.R * blue.Value / 255 + Red.R * red.Value / 255 + White.R * white.Value / 255;
            double g = Yellow.G * yellow.Value / 255 + Green.G * green.Value / 255 +
                Blue.G * blue.Value / 255 + Red.G * red.Value / 255 + White.G * white.Value / 255;
            double b = Yellow.B * yellow.Value / 255 + Green.B * green.Value / 255 +
                Blue.B * blue.Value / 255 + Red.B * red.Value / 255 + White.B * white.Value / 255;
            ShowColor.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)r, (byte)g, (byte)b));
        }
        
        private void drawline(int channel, double data)
        {
            if (zedgraph.GraphPane.CurveList.Count <= 0) return;

            LineItem line;
            if (channel == 0)
            {             
                line = zedgraph.GraphPane.CurveList[0] as LineItem;
            }
            else
            {
                line = zedgraph.GraphPane.CurveList[1] as LineItem;
            }

            if (line == null) return;
            IPointListEdit list = line.Points as IPointListEdit;

            if (list == null) return;
            double time = (Environment.TickCount - tickStart) / 1000.0;
            list.Add(time, data);
            
            Scale x = zedgraph.GraphPane.XAxis.Scale;
            if (time > x.Max - x.MajorStep)
            {
                x.Max = time + x.MajorStep;
                x.Min = x.Max - 30.0;
            }
            
            zedgraph.AxisChange();
            zedgraph.Invalidate();
        }

        int click = 0;       
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            click = 1;
            if (myPort == null || !myPort.IsOpen)
            {
                System.Windows.MessageBox.Show("串口未连接");
            }
            if (isRecord)
            {
                return;
            }
            SaveFileDialog log1 = new SaveFileDialog();
            log1.Filter = "txt文件(*.txt)|*.txt";
            log1.FileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            if (log1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                isRecord = true;
                sw1 = File.CreateText(log1.FileName);
                sw1.WriteLine("串口号:" + combo1.SelectedItem.ToString());
                sw1.WriteLine("波特率:" + combo2.SelectedItem.ToString() + "\n");
                sw1.WriteLine("温度:" + textBox1.Text + "\n");
                sw1.WriteLine("光强:" + textBox2.Text + "\n");
                sw1.Flush();
            }
        }

        private void End_Click(object sender, RoutedEventArgs e)//log结束
        {
            if (isRecord)
            {
                sw1.Flush();
                sw1.Close();
                isRecord = false;
            }
        }

        private void S_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ColorSend();
        }
    }
}
