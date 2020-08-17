using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MQTTnet;
using MQTTnet.Core.Client;
using comWithPlc;
using MQTTnet.Core;
using MQTTnet.Core.Packets;
using MQTTnet.Core.Protocol;






namespace mqttPS
{
    public partial class Form1 : Form
    {
        private MqttClient mqttClient = null;

        public Form1()
        {
            InitializeComponent();

            Task.Run(async () => { await ConnectMqttServerAsync(); });

           

        }

        private async Task ConnectMqttServerAsync()
        {
            if (mqttClient == null)
            {
                mqttClient = new MqttClientFactory().CreateMqttClient() as MqttClient;
                mqttClient.ApplicationMessageReceived += MqttClient_ApplicationMessageReceived;
                mqttClient.Connected += MqttClient_Connected;
              
            }

            try
            {
                var options = new MqttClientTcpOptions
                {
                    Server = "mqtt.eclipse.org",
                    ClientId = Guid.NewGuid().ToString().Substring(0, 5),
                    CleanSession = true
                };

                await mqttClient.ConnectAsync(options);
            }
            catch (Exception ex)
            {
                Invoke((new Action(() =>
                {
                    listInfo.Items.Add($"连接到MQTT服务器失败！" + Environment.NewLine + ex.Message + Environment.NewLine);
                })));
            }
        }

        private void MqttClient_Connected(object sender, EventArgs e)
        {
            Invoke((new Action(() =>
            {
                listInfo.Items.Add("已连接到MQTT服务器！" + Environment.NewLine);
            })));
        }

        private void MqttClient_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            Invoke((new Action(() =>
            {
               listInfo.Items.Add($">> {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}{Environment.NewLine}");
            })));
        }



            getPlcValues operatePlc = new getPlcValues();

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 300000;//执行间隔时间,单位为毫秒;此时时间间隔为60秒   


            label19.Text = Convert.ToString(operatePlc.readPlcDbdValues("10.228.140.46", 0, 3, 92, 0));
            label18.Text = Convert.ToString(operatePlc.readPlcDbdValues("10.228.140.54", 0, 3, 92, 0));
            label17.Text = Convert.ToString(operatePlc.readPlcDbdValues("10.228.140.98", 0, 3, 92, 0));
            label16.Text = Convert.ToString(operatePlc.readPlcDbdValues("10.228.141.38", 0, 3, 92, 0));
            label15.Text = Convert.ToString(operatePlc.readPlcDbdValues("10.228.141.46", 0, 3, 92, 0));
            label14.Text = Convert.ToString(operatePlc.readPlcDbdValues("10.228.141.82", 0, 3, 92, 0));
            label13.Text = Convert.ToString(operatePlc.readPlcDbdValues("10.228.141.126", 0, 3, 92, 0));
        }



        private void button1_Click(object sender, EventArgs e)
        {
            timer2.Start();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            string topics = topic.Text.Trim();

            timer2.Interval = 10000;

            string inputString = "烟囱温度:" + "电泳一" + label19.Text + "---" + "电泳二" + label18.Text + "---" + "PVC" + label17.Text + "---" + "中涂一" + label16.Text + "---" + "中涂二" + label15.Text + "---" + "面漆一" + label14.Text + "---" + "面漆二" + label13.Text;
            var appMsg = new MqttApplicationMessage(topics, Encoding.UTF8.GetBytes(inputString), MqttQualityOfServiceLevel.AtMostOnce, false);
            mqttClient.PublishAsync(appMsg);

        }

        //窗口关闭时，程序后台运行
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
        }
        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你确定要退出程序吗？", "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                notifyIcon1.Visible = false;
                this.Close();
                this.Dispose();
                Application.Exit();
            }

        }

        private void hideMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void showMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();

        }

        //托盘
        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)//判断鼠标的按键
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    this.WindowState = FormWindowState.Minimized;

                    this.Hide();
                }
                else if (this.WindowState == FormWindowState.Minimized)
                {
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                    this.Activate();
                }
            }

        }
    }
}
