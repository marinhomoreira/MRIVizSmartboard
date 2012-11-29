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

        // iNetworking initialization
        private Connection _connection;
        // Tablet ipAddress
        private string _ipAddress = "10.11.19.165";
        private int _port = 12345;

        public MainWindow()
        {
            InitializeConnection();

            InitializeComponent();
            setImageOnDisplay(542);
        }

        #region mock stuff
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int slValue = (int)slider1.Value;
            setImageOnDisplay(slValue);
            sendRequestWithImageIndex(slValue);
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



        #region iNetwork Methods

        private void InitializeConnection()
        {
            // connect to the server
            this._connection = new Connection(this._ipAddress, this._port);
            this._connection.Connected += new ConnectionEventHandler(OnConnected);
            this._connection.Start();
        }

        void OnConnected(object sender, ConnectionEventArgs e)
        {
            this._connection.MessageReceived += new ConnectionMessageEventHandler(OnMessageReceived);
        }

        private void OnMessageReceived(object sender, Message msg)
        {
            try
            {
                if (msg != null)
                {
                    switch (msg.Name)
                    {
                        default:
                            // don't do anything
                            break;
                        case "Name-of-Message":
                            // do something here
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + "\n" + e.StackTrace);
            }

        }

        private void sendRequestWithImageIndex(int slValue)
        {
            // TODO MESSAGE
            Message msg = new Message("ChangeImg");
            msg.AddField("index", slValue);
            this._connection.SendMessage(msg);
        }

        #endregion

    }
}
