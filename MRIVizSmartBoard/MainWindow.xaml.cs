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
            setImageOnDisplay(542,0,0);
            this.Closing += new System.ComponentModel.CancelEventHandler(OnClosing);
        }

        int slValue = 0;
        int sl2Value = 0;
        int sl3Value = 0;

        #region mock stuff
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.slValue = (int)slider1.Value;
            setImageOnDisplay(this.slValue, this.sl2Value, this.sl3Value);
            sendImageIndex(this.slValue, this.sl2Value, this.sl3Value);
        }
        private void slider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.sl2Value = (int)slider2.Value;
            setImageOnDisplay(this.slValue, this.sl2Value, this.sl3Value);
            sendImageIndex(this.slValue, this.sl2Value, this.sl3Value);
        }
        private void slider3_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.sl3Value = (int)slider3.Value;
            setImageOnDisplay(this.slValue, this.sl2Value, this.sl3Value);
            sendImageIndex(this.slValue, this.sl2Value, this.sl3Value);
        }
        #endregion


        #region Display-related functions
        void setImageOnDisplay(int imageIndex, int x, int y)
        {
            String imgUri = "MRIImages/IM-0001-0" + imageIndex + ".jpg";

            this.Dispatcher.Invoke(new Action(delegate()
            {
                image.Source = new BitmapImage(new Uri(imgUri, UriKind.Relative));
                image.SetValue(Canvas.LeftProperty, (double)x);
                
                image.SetValue(Canvas.TopProperty, (double)y);
            }));
        }


        int processCoordinates(double x, double y, double z)
        {
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

        void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Ensure that the server terminates when the window is closed.
            if (this._server != null && this._server.IsRunning)
            {
                this._server.Stop();
            }
        }

        #endregion


        private void sendImageIndex(int slValue, int x, int y)
        {
            // TODO MESSAGE
            Message msg = new Message("ChangeImg");
            msg.AddField("index", slValue);
            msg.AddField("x", x);
            msg.AddField("y", y);
            if (this._server != null)
                this._server.BroadcastMessage(msg);
            Console.WriteLine(msg.ToString() + ": " + msg.GetIntField("index"));
        }




    }
}
