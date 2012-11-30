using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using GroupLab.iNetwork;
using GroupLab.iNetwork.Tcp;

using System.Diagnostics;

namespace MRIVizSmartBoard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeServer();

            InitializeComponent();
            setImageOnDisplay(542);
        }

        #region mock stuff
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int slValue = (int)slider1.Value;
            setImageOnDisplay(slValue);
            sendImageIndex(slValue);
        }
        #endregion


        #region Display-related functions
        void setImageOnDisplay(int imageIndex)
        {
            String imgUri = "MRIImages/IM-0001-0" + imageIndex + ".jpg";

            this.Dispatcher.Invoke(new Action(delegate()
            {
                image.Source = new BitmapImage(new Uri(imgUri, UriKind.Relative));
            }));
        }

        int processCoordinates(double x, double y, double z)
        {
            int leftBound = -1000;
            int rightBound = 600;



            // Calculate the width of column in space [width in space divided by number of images]
            
            // Calculate the index of image = [position in space divided by calculated column's width]

            return 541;
        }
        

        #endregion


        private Server _server;
        private List<Connection> _clients;

        #region iNetwork Methods

        public void InitializeServer()
        {
            this._clients = new List<Connection>();

            // Create a new server, add name and port number
            this._server = new Server("ServerName", 12345);
            this._server.IsDiscoverable = true;
            this._server.Connection += new ConnectionEventHandler(OnServerConnection);

            this._server.Start();

            // Display the server info on the label

            //this.NetInfo.Content = "IP: " + this._server.Configuration.IPAddress.ToString() + ", Port: " + this._server.Configuration.Port.ToString();
        }


        private void OnServerConnection(object sender, ConnectionEventArgs e)
        {

            if (e.ConnectionEvent == ConnectionEvents.Connect)
            {
                // new client connected
                lock (this._clients)
                {
                    if (!(this._clients.Contains(e.Connection)))
                    {
                        this._clients.Add(e.Connection);
                        e.Connection.MessageReceived += new ConnectionMessageEventHandler(OnMessageReceived);

                        this.Dispatcher.Invoke(
                            new Action(
                                delegate()
                                {
                                    //this.clientList.Items.Add(e.Connection.ToString());
                                }));
                    }
                }
            }
            else if (e.ConnectionEvent == ConnectionEvents.Disconnect)
            {
                // client disconnected
                lock (this._clients)
                {
                    if (this._clients.Contains(e.Connection))
                    {
                        this._clients.Remove(e.Connection);
                        e.Connection.MessageReceived -= new ConnectionMessageEventHandler(OnMessageReceived);

                        this.Dispatcher.Invoke(
                            new Action(
                                delegate()
                                {
                                    //this.clientList.Items.Remove(e.Connection.ToString());
                                }));
                    }
                }
            }
        }

        private void OnMessageReceived(object sender, Message msg)
        {
            this.Dispatcher.Invoke(
                new Action(
                    delegate()
                    {
                        if (msg != null)
                        {
                            //this.messages.Text += msg.Name + "\n";
                            switch (msg.Name)
                            {
                                default:
                                    // don't do anything
                                    break;
                                // add cases with the message names
                            }
                        }
                    }));
        }

        #endregion


        private void sendImageIndex(int slValue)
        {
            // TODO MESSAGE
            Message msg = new Message("ChangeImg");
            msg.AddField("index", slValue);
            if (this._server != null)
                this._server.BroadcastMessage(msg);
            Console.WriteLine(msg.ToString() + ": " + msg.GetIntField("index"));
        }


    }
}
