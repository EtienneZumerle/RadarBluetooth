using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using wclCommon;
using wclBluetooth;



using System.IO.Ports;
using System.Threading;


namespace Beacons
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        
        
        arduinoCOM arduino = new arduinoCOM("9600");

        


        private wclBluetoothManager wclBluetoothManager;
        private wclBluetoothLeBeaconWatcher wclBluetoothLeBeaconWatcher;
        private wclBluetoothLeAdvertiser wclBluetoothLeAdvertiser;

        public MainWindow()
        {
            InitializeComponent();
            //arduino.openPort();
        }

        private wclBluetoothRadio GetRadio()
        {
            wclBluetoothRadio Radio;
            Int32 Res = wclBluetoothManager.GetLeRadio(out Radio);
            if (Res != wclErrors.WCL_E_SUCCESS)
            {
                MessageBox.Show("Get working radio failed: 0x" + Res.ToString("X8"));
                return null;
            }
            return Radio;
        }
       
       
        private void DumpData(Byte[] Data)
        {
            if (Data != null && Data.Length > 0)
            {
                String Str = "";
                Byte c = 0;
                foreach (Byte b in Data)
                {
                    Str = Str + b.ToString("X2");
                    if (c == 15)
                    {
                        lbFrames.Items.Add("    " + Str);
                        Str = "";
                        c = 0;
                    }
                    else
                        c++;
                }
                if (Str != "")
                    lbFrames.Items.Add("    " + Str);
            }
        }

        private void fmMain_Loaded(object sender, RoutedEventArgs e)
        {
            wclBluetoothManager = new wclBluetoothManager();
            wclBluetoothManager.AfterOpen += new EventHandler(wclBluetoothManager_AfterOpen);
            wclBluetoothManager.BeforeClose += new EventHandler(wclBluetoothManager_BeforeClose);
            wclBluetoothManager.OnStatusChanged += new wclBluetoothEvent(wclBluetoothManager_OnStatusChanged);
            
            wclBluetoothLeBeaconWatcher = new wclBluetoothLeBeaconWatcher();
            wclBluetoothLeBeaconWatcher.OnAdvertisementRawFrame += new wclBluetoothLeAdvertisementRawFrameEvent(wclBluetoothLeBeaconWatcher_OnAdvertisementRawFrame);
            wclBluetoothLeBeaconWatcher.OnEddystoneTlmFrame += new wclBluetoothLeEddystoneTlmFrameEvent(wclBluetoothLeBeaconWatcher_OnEddystoneTlmFrame);
            wclBluetoothLeBeaconWatcher.OnEddystoneUidFrame += new wclBluetoothLeEddystoneUidFrameEvent(wclBluetoothLeBeaconWatcher_OnEddystoneUidFrame);
            wclBluetoothLeBeaconWatcher.OnEddystoneUrlFrame += new wclBluetoothLeEddystoneUrlFrameEvent(wclBluetoothLeBeaconWatcher_OnEddystoneUrlFrame);
            wclBluetoothLeBeaconWatcher.OnManufacturerRawFrame += new wclBluetoothLeManufacturerRawFrameEvent(wclBluetoothLeBeaconWatcher_OnManufacturerRawFrame);

            wclBluetoothLeBeaconWatcher.OnProximityBeaconFrame += new wclBluetoothLeProximityBeaconFrameEvent(wclBluetoothLeBeaconWatcher_OnProximityBeaconFrame);

            wclBluetoothLeBeaconWatcher.OnAltBeaconFrame += new wclBluetoothLeAltBeaconFrameEvent(wclBluetoothLeBeaconWatcher_OnAltBeaconFrame);
            wclBluetoothLeBeaconWatcher.OnAdvertisementFrameInformation += new wclBluetoothLeAdvertisementFrameInformationEvent(wclBluetoothLeBeaconWatcher_OnAdvertisementFrameInformation);
            wclBluetoothLeBeaconWatcher.OnAdvertisementExtFrameInformation += new wclBluetoothLeAdvertisementExtFrameInformationEvent(wclBluetoothLeBeaconWatcher_OnAdvertisementExtFrameInformation);
            wclBluetoothLeBeaconWatcher.OnAdvertisementUuidFrame += new wclBluetoothLeAdvertisementUuidFrameEvent(wclBluetoothLeBeaconWatcher_OnAdvertisementUuidFrame);
            wclBluetoothLeBeaconWatcher.OnStarted += new EventHandler(wclBluetoothLeBeaconWatcher_OnStarted);
            wclBluetoothLeBeaconWatcher.OnStopped += new EventHandler(wclBluetoothLeBeaconWatcher_OnStopped);
            wclBluetoothLeBeaconWatcher.OnAdvertisementService128DataFrame += new wclBluetoothLeAdvertisementService128DataFrameEvent(wclBluetoothLeBeaconWatcher_OnAdvertisementService128DataFrame);
            wclBluetoothLeBeaconWatcher.OnAdvertisementService16DataFrame += new wclBluetoothLeAdvertisementService16DataFrameEvent(wclBluetoothLeBeaconWatcher_OnAdvertisementService16DataFrame);
            wclBluetoothLeBeaconWatcher.OnAdvertisementService32DataFrame += new wclBluetoothLeAdvertisementService32DataFrameEvent(wclBluetoothLeBeaconWatcher_OnAdvertisementService32DataFrame);
            wclBluetoothLeBeaconWatcher.OnAdvertisementServiceSol128Frame += new wclBluetoothLeAdvertisementServiceSol128FrameEvent(wclBluetoothLeBeaconWatcher_OnAdvertisementServiceSol128Frame);
            wclBluetoothLeBeaconWatcher.OnAdvertisementServiceSol16Frame += new wclBluetoothLeAdvertisementServiceSol16FrameEvent(wclBluetoothLeBeaconWatcher_OnAdvertisementServiceSol16Frame);
            wclBluetoothLeBeaconWatcher.OnAdvertisementServiceSol32Frame += new wclBluetoothLeAdvertisementServiceSol32FrameEvent(wclBluetoothLeBeaconWatcher_OnAdvertisementServiceSol32Frame);
            
            
            wclBluetoothLeBeaconWatcher.OnAdvertisementReceived += new wclBluetoothLeAdvertisementReceivedEvent(wclBluetoothLeBeaconWatcher_OnAdvertisementReceived);

            wclBluetoothLeAdvertiser = new wclBluetoothLeAdvertiser();
            wclBluetoothLeAdvertiser.OnStarted += new EventHandler(wclBluetoothLeAdvertiser_OnStarted);
            wclBluetoothLeAdvertiser.OnStopped += new EventHandler(wclBluetoothLeAdvertiser_OnStopped);
            wclBluetoothLeAdvertiser.OnAdvertisingBegin += new wclBluetoothLeAdvertiserAdvertisementEvent(wclBluetoothLeAdvertiser_OnAdvertisingBegin);
            wclBluetoothLeAdvertiser.OnAdvertisingEnd += new wclBluetoothLeAdvertiserAdvertisementEvent(wclBluetoothLeAdvertiser_OnAdvertisingEnd);
            wclBluetoothLeAdvertiser.OnAdvertisingError += new wclBluetoothLEAdvertiserAdvertisingError(wclBluetoothLeAdvertiser_OnAdvertisingError);

            wclBluetoothLeAdvertiser.Multiplier = 5;

            tbMultiplier.Value = wclBluetoothLeAdvertiser.Multiplier;
            laMultiplier.Content = wclBluetoothLeAdvertiser.Multiplier.ToString();
        }

        void wclBluetoothLeBeaconWatcher_OnAdvertisementReceived(object Sender, long Address, long Timestamp, sbyte Rssi, byte[] Data)
        {
            lbFrames.Items.Add("RAW ADVERTISEMENT FRAME");

            lbFrames.Items.Add("  AddressL " + Address.ToString("X12"));
            lbFrames.Items.Add("  Timestamp: " + DateTime.FromFileTime(Timestamp).ToString());
            lbFrames.Items.Add("  RSSI: " + Rssi.ToString());

            
            
            

            lbFrames.Items.Add("-------------------------------------------------------");
        }

        void wclBluetoothLeAdvertiser_OnAdvertisingError(object Sender, int Index, int Error)
        {
            ListBox.Items.Add("Advertising " + Index.ToString() + " error 0x" +
                Error.ToString("X8"));
        }

        void wclBluetoothLeAdvertiser_OnAdvertisingEnd(object Sender, int Index)
        {
            ListBox.Items.Add("Advertising " + Index.ToString() + " end");
        }

        void wclBluetoothLeAdvertiser_OnAdvertisingBegin(object Sender, int Index)
        {
            ListBox.Items.Add("Advertising " + Index.ToString() + " begin.");
        }

        void wclBluetoothLeAdvertiser_OnStopped(object sender, EventArgs e)
        {
            ListBox.Items.Add("Bluetooth LE Advertising Stopped");
            wclBluetoothLeAdvertiser.Clear();

            EnableAdvertisements(true);
        }

        void wclBluetoothLeAdvertiser_OnStarted(object sender, EventArgs e)
        {
            ListBox.Items.Add("Bluetooth LE Advertising Started");

            EnableAdvertisements(false);
        }

        void wclBluetoothLeBeaconWatcher_OnAdvertisementService32DataFrame(object Sender, long Address, long Timestamp, sbyte Rssi, uint Uuid, byte[] Data)
        {
            lbFrames.Items.Add("32 UUID SERVICE DATA");

            lbFrames.Items.Add("  Address: " + Address.ToString("X12"));
            lbFrames.Items.Add("  Timestamp: " + DateTime.FromFileTime(Timestamp).ToString());
            lbFrames.Items.Add("  RSSI: " + Rssi.ToString());
            lbFrames.Items.Add("  UUID: " + Uuid.ToString("X8"));

            DumpData(Data);

            lbFrames.Items.Add("-------------------------------------------------------");
        }

        void wclBluetoothLeBeaconWatcher_OnAdvertisementService16DataFrame(object Sender, long Address, long Timestamp, sbyte Rssi, ushort Uuid, byte[] Data)
        {
            lbFrames.Items.Add("16 UUID SERVICE DATA");

            lbFrames.Items.Add("  Address: " + Address.ToString("X12"));
            lbFrames.Items.Add("  Timestamp: " + DateTime.FromFileTime(Timestamp).ToString());
            lbFrames.Items.Add("  RSSI: " + Rssi.ToString());
            lbFrames.Items.Add("  UUID: " + Uuid.ToString("X4"));

            DumpData(Data);

            lbFrames.Items.Add("-------------------------------------------------------");
        }

        void wclBluetoothLeBeaconWatcher_OnAdvertisementService128DataFrame(object Sender, long Address, long Timestamp, sbyte Rssi, Guid Uuid, byte[] Data)
        {
            lbFrames.Items.Add("128 UUID SERVICE DATA");

            lbFrames.Items.Add("  Address: " + Address.ToString("X12"));
            lbFrames.Items.Add("  Timestamp: " + DateTime.FromFileTime(Timestamp).ToString());
            lbFrames.Items.Add("  RSSI: " + Rssi.ToString());
            lbFrames.Items.Add("  UUID: " + Uuid.ToString());

            DumpData(Data);

            lbFrames.Items.Add("-------------------------------------------------------");
        }

        void wclBluetoothLeBeaconWatcher_OnAdvertisementServiceSol32Frame(object Sender, long Address, long Timestamp, sbyte Rssi, uint Uuid)
        {
            lbFrames.Items.Add("32 UUID SERVICE");

            lbFrames.Items.Add("  Address: " + Address.ToString("X12"));
            lbFrames.Items.Add("  Timestamp: " + DateTime.FromFileTime(Timestamp).ToString());
            lbFrames.Items.Add("  RSSI: " + Rssi.ToString());
            lbFrames.Items.Add("  UUID: " + Uuid.ToString("X8"));

            lbFrames.Items.Add("-------------------------------------------------------");
        }

        void wclBluetoothLeBeaconWatcher_OnAdvertisementServiceSol16Frame(object Sender, long Address, long Timestamp, sbyte Rssi, ushort Uuid)
        {
            lbFrames.Items.Add("16 UUID SERVICE");

            lbFrames.Items.Add("  Address: " + Address.ToString("X12"));
            lbFrames.Items.Add("  Timestamp: " + DateTime.FromFileTime(Timestamp).ToString());
            lbFrames.Items.Add("  RSSI: " + Rssi.ToString());
            lbFrames.Items.Add("  UUID: " + Uuid.ToString("X4"));

            lbFrames.Items.Add("-------------------------------------------------------");
        }

        void wclBluetoothLeBeaconWatcher_OnAdvertisementServiceSol128Frame(object Sender, long Address, long Timestamp, sbyte Rssi, Guid Uuid)
        {
            lbFrames.Items.Add("128 UUID SERVICE");

            lbFrames.Items.Add("  Address: " + Address.ToString("X12"));
            lbFrames.Items.Add("  Timestamp: " + DateTime.FromFileTime(Timestamp).ToString());
            lbFrames.Items.Add("  RSSI: " + Rssi.ToString());
            lbFrames.Items.Add("  UUID: " + Uuid.ToString());

            lbFrames.Items.Add("-------------------------------------------------------");
        }

        void wclBluetoothLeBeaconWatcher_OnAdvertisementExtFrameInformation(object Sender, long Address, long Timestamp, sbyte Rssi, wclBluetoothAddressType AddressType, sbyte TxPower, wclBluetoothLeExtendedFrameFlag Flags)
        {
            lbFrames.Items.Add("EXTENDED FRAME INFORMATION");

            lbFrames.Items.Add("  Address: " + Address.ToString("X12"));
            lbFrames.Items.Add("  Timestamp: " + DateTime.FromFileTime(Timestamp).ToString());
            lbFrames.Items.Add("  RSSI: " + Rssi.ToString());

            String Str;
            switch (AddressType)
            {
                case wclBluetoothAddressType.atPublic:
                    Str = "Public";
                    break;
                case wclBluetoothAddressType.atRandom:
                    Str = "Random";
                    break;
                case wclBluetoothAddressType.atUnspecified:
                    Str = "Unspecified";
                    break;
                default:
                    Str = "Unknown";
                    break;
            }
            lbFrames.Items.Add("  Address type: " + Str);

            lbFrames.Items.Add("  TX Power: " + TxPower.ToString());

            Str = "[";
            if ((Flags & wclBluetoothLeExtendedFrameFlag.efAnonymous) != 0)
                Str = Str + " efAnonymous";
            if ((Flags & wclBluetoothLeExtendedFrameFlag.efConnectable) != 0)
                Str = Str + " efConnectable";
            if ((Flags & wclBluetoothLeExtendedFrameFlag.efDirected) != 0)
                Str = Str + " efDirected";
            if ((Flags & wclBluetoothLeExtendedFrameFlag.efScannable) != 0)
                Str = Str + " efScannable";
            if ((Flags & wclBluetoothLeExtendedFrameFlag.efScanResponse) != 0)
                Str = Str + " efScanResponse";
            Str = Str + " ]";
            lbFrames.Items.Add("  Flags: " + Str);

            lbFrames.Items.Add("-------------------------------------------------------");
        }

        void wclBluetoothLeBeaconWatcher_OnAdvertisementUuidFrame(object Sender, long Address, long Timestamp, sbyte Rssi, Guid Uuid)
        {
            lbFrames.Items.Add("UUID FRAME");

            lbFrames.Items.Add("  Address: " + Address.ToString("X12"));
            lbFrames.Items.Add("  Timestamp: " + DateTime.FromFileTime(Timestamp).ToString());
            lbFrames.Items.Add("  RSSI: " + Rssi.ToString());
            lbFrames.Items.Add("  UUID: " + Uuid.ToString());

            lbFrames.Items.Add("-------------------------------------------------------");
        }

        void wclBluetoothLeBeaconWatcher_OnAdvertisementFrameInformation(object Sender, long Address, long Timestamp, sbyte Rssi, string Name, wclBluetoothLeAdvertisementType PacketType, wclBluetoothLeAdvertisementFlag Flags)
        {
            lbFrames.Items.Add("FRAME INFORMATION");

            lbFrames.Items.Add("  Address: " + Address.ToString("X12"));
            lbFrames.Items.Add("  Timestamp: " + DateTime.FromFileTime(Timestamp).ToString());
            lbFrames.Items.Add("  RSSI: " + Rssi.ToString());
            lbFrames.Items.Add("  Name: " + Name);

            String Str;
            switch (PacketType)
            {
                case wclBluetoothLeAdvertisementType.atConnectableUndirected:
                    Str = "Connectable Undirected";
                    break;
                case wclBluetoothLeAdvertisementType.atConnectableDirected:
                    Str = "Connectable Directed";
                    break;
                case wclBluetoothLeAdvertisementType.atScannableUndirected:
                    Str = "Scannable Undirecte";
                    break;
                case wclBluetoothLeAdvertisementType.atNonConnectableUndirected:
                    Str = "Non Connectable Undirected";
                    break;
                case wclBluetoothLeAdvertisementType.atScanResponse:
                    Str = "Scan Response";
                    break;
                default:
                    Str = "Unknown";
                    break;
            }
            lbFrames.Items.Add("  Frame type: " + Str);

            Str = "[";
            if ((Flags & wclBluetoothLeAdvertisementFlag.afLimitedDiscoverableMode) != 0)
                Str = Str + " afLimitedDiscoverableMode";
            if ((Flags & wclBluetoothLeAdvertisementFlag.afGeneralDiscoverableMode) != 0)
                Str = Str + " afGeneralDiscoverableMode";
            if ((Flags & wclBluetoothLeAdvertisementFlag.afClassicNotSupported) != 0)
                Str = Str + " afClassicNotSupported";
            if ((Flags & wclBluetoothLeAdvertisementFlag.afDualModeControllerCapable) != 0)
                Str = Str + " afDualModeControllerCapable";
            if ((Flags & wclBluetoothLeAdvertisementFlag.afDualModeHostCapable) != 0)
                Str = Str + " afDualModeHostCapable";
            Str = Str + " ]";
            lbFrames.Items.Add("  Flags: " + Str);

            lbFrames.Items.Add("-------------------------------------------------------");
        }

        void wclBluetoothLeBeaconWatcher_OnAltBeaconFrame(object Sender, long Address, long Timestamp, sbyte Rssi, ushort CompanyId, ushort Major, ushort Minor, Guid Uuid, sbyte TxRssi, byte Reserved, Byte[] Data)
        {
            // Clculate distance.
            double Dist = Math.Pow(10.0, ((double)TxRssi - (double)Rssi) / 20.0);

            lbFrames.Items.Add("ALT BEACON FRAME");

            lbFrames.Items.Add("  Address: " + Address.ToString("X12"));
            lbFrames.Items.Add("  Company ID: " + CompanyId.ToString("X4"));
            lbFrames.Items.Add("  Timestamp: " + DateTime.FromFileTime(Timestamp).ToString());
            lbFrames.Items.Add("  RSSI: " + Rssi.ToString());

            lbFrames.Items.Add("  UUID: " + Uuid.ToString());
            lbFrames.Items.Add("  Major: " + Major.ToString("X4"));
            lbFrames.Items.Add("  Minor: " + Minor.ToString("X4"));
            lbFrames.Items.Add("  Reserved: " + Reserved.ToString("X2"));
            lbFrames.Items.Add("  TX RSSI: " + TxRssi.ToString());
            lbFrames.Items.Add("  Distance: " + Dist.ToString());

            DumpData(Data);

            lbFrames.Items.Add("-------------------------------------------------------");
        }

        void wclBluetoothManager_AfterOpen(object sender, EventArgs e)
        {
            ListBox.Items.Add("Bluetooth Manager Opened");
        }

        void wclBluetoothManager_BeforeClose(object sender, EventArgs e)
        {
            ListBox.Items.Add("Bluetooth Manager Closed");
        }

        void wclBluetoothManager_OnStatusChanged(object Sender, wclBluetoothRadio Radio)
        {
            String Str;
            if (Radio.Available)
                Str = "AVAILABLE";
            else
                Str = "UNAVAILABLE";
            ListBox.Items.Add("Radio " + Radio.ApiName + " status changed: " + Str);
        }

        void wclBluetoothLeBeaconWatcher_OnStarted(object sender, EventArgs e)
        {
            ListBox.Items.Add("Beacons Monitoring Started");
        }

        void wclBluetoothLeBeaconWatcher_OnStopped(object sender, EventArgs e)
        {
            ListBox.Items.Add("Beacons Monitoring Stopped");
        }

        void wclBluetoothLeBeaconWatcher_OnAdvertisementRawFrame(object Sender, long Address, long Timestamp, sbyte Rssi, byte DataType, byte[] Data)
        {
            lbFrames.Items.Add("UNKNOWN RAW FRAME");

            lbFrames.Items.Add("  Address: " + Address.ToString("X12"));
            lbFrames.Items.Add("  Timestamp: " + DateTime.FromFileTime(Timestamp).ToString());
            lbFrames.Items.Add("  RSSI: " + Rssi.ToString());

            lbFrames.Items.Add("  Data type: " + DataType.ToString("X2"));

            DumpData(Data);

            lbFrames.Items.Add("-------------------------------------------------------");
        }

        void wclBluetoothLeBeaconWatcher_OnEddystoneTlmFrame(object Sender, long Address, long Timestamp, sbyte Rssi, uint AdvCnt, ushort Batt, uint SecCnt, double Temp, Byte[] Data)
        {
            lbFrames.Items.Add("EDDYSTONE TLM FRAME");

            lbFrames.Items.Add("  Address: " + Address.ToString("X12"));
            lbFrames.Items.Add("  Timestamp: " + DateTime.FromFileTime(Timestamp).ToString());
            lbFrames.Items.Add("  RSSI: " + Rssi.ToString());

            lbFrames.Items.Add("  Advertisements counter: " + AdvCnt.ToString());
            lbFrames.Items.Add("  Battery (mv): " + Batt.ToString());
            lbFrames.Items.Add("  Seconds running: " + (SecCnt / 10).ToString());
            lbFrames.Items.Add("  Temperature: " + Temp.ToString());

            DumpData(Data);

            lbFrames.Items.Add("-------------------------------------------------------");
        }

        void wclBluetoothLeBeaconWatcher_OnEddystoneUidFrame(object Sender, long Address, long Timestamp, sbyte Rssi, sbyte TxRssi, Guid Uuid, Byte[] Data)
        {
            // Calculate distance.
            double Dist = Math.Pow(10.0, ((double)TxRssi - 41.0 - (double)Rssi) / 20.0);

            lbFrames.Items.Add("EDDYSTONE UID FRAME");

            lbFrames.Items.Add("  Address: " + Address.ToString("X12"));
            lbFrames.Items.Add("  Timestamp: " + DateTime.FromFileTime(Timestamp).ToString());
            lbFrames.Items.Add("  RSSI: " + Rssi.ToString());

            lbFrames.Items.Add("  UUID: " + Uuid.ToString());

            lbFrames.Items.Add("  TX RSSI: " + TxRssi.ToString());
            lbFrames.Items.Add("  Distance: " + Dist.ToString());

            DumpData(Data);

            lbFrames.Items.Add("-------------------------------------------------------");
        }

        void wclBluetoothLeBeaconWatcher_OnEddystoneUrlFrame(object Sender, long Address, long Timestamp, sbyte Rssi, sbyte TxRssi, string Url)
        {
            // Calculate distance.
            double Dist = Math.Pow(10.0, ((double)TxRssi - 41.0 - (double)Rssi) / 20.0);

            lbFrames.Items.Add("EDDYSTONE URL FRAME");

            lbFrames.Items.Add("  Address: " + Address.ToString("X12"));
            lbFrames.Items.Add("  Timestamp: " + DateTime.FromFileTime(Timestamp).ToString());
            lbFrames.Items.Add("  RSSI: " + Rssi.ToString());

            lbFrames.Items.Add("  URL: " + Url);

            lbFrames.Items.Add("  TX RSSI: " + TxRssi.ToString());
            lbFrames.Items.Add("  Distance: " + Dist.ToString());

            lbFrames.Items.Add("-------------------------------------------------------");
        }

        void wclBluetoothLeBeaconWatcher_OnManufacturerRawFrame(object Sender, long Address, long Timestamp, sbyte Rssi, ushort CompanyId, byte[] Data)
        {
            lbFrames.Items.Add("UNKNOWN MANUFACTURER RAW FRAME");

            lbFrames.Items.Add("  Address: " + Address.ToString("X12"));
            lbFrames.Items.Add("  Company ID: " + CompanyId.ToString("X4"));
            lbFrames.Items.Add("  Timestamp: " + DateTime.FromFileTime(Timestamp).ToString());
            lbFrames.Items.Add("  RSSI: " + Rssi.ToString());

            DumpData(Data);

            lbFrames.Items.Add("-------------------------------------------------------");
        }

        void wclBluetoothLeBeaconWatcher_OnProximityBeaconFrame(object Sender, long Address, long Timestamp, sbyte Rssi, ushort CompanyId, ushort Major, ushort Minor, Guid Uuid, sbyte TxRssi, Byte[] Data)
        {
            // Clculate distance.
            double Dist = Math.Pow(10.0, ((double)TxRssi - (double)Rssi) / 20.0);

            lbFrames.Items.Add("PROXIMITY BEACON FRAME");

            lbFrames.Items.Add("  Address: " + Address.ToString("X12"));
            lbFrames.Items.Add("  Company ID: " + CompanyId.ToString("X4"));
            lbFrames.Items.Add("  Timestamp: " + DateTime.FromFileTime(Timestamp).ToString());
            lbFrames.Items.Add("  RSSI: " + Rssi.ToString());

            lbFrames.Items.Add("  UUID: " + Uuid.ToString());
            lbFrames.Items.Add("  Major: " + Major.ToString("X4"));
            lbFrames.Items.Add("  Minor: " + Minor.ToString("X4"));
            lbFrames.Items.Add("  TX RSSI: " + TxRssi.ToString());
            lbFrames.Items.Add("  Distance: " + Dist.ToString());

            if (Address == 0xc749837e9d9b)
            {
                arduino.setRssi(Rssi.ToString());
                DumpData(Data);
            }

            lbFrames.Items.Add("-------------------------------------------------------");
        }

        private void AddAdvertisement(wclBluetoothLeAdvertisement Adv)
        {
            Int32 Res = wclBluetoothLeAdvertiser.Add(Adv);
            if (Res != wclErrors.WCL_E_SUCCESS)
                ListBox.Items.Add("Add advertisement failed: 0x" + Res.ToString("8"));
        }

        private void fmMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            wclBluetoothManager.Close();
        }

        private void btOpen_Click(object sender, RoutedEventArgs e)
        {
            Int32 Res = wclBluetoothManager.Open();
            if (Res != wclErrors.WCL_E_SUCCESS)
                MessageBox.Show("Error: 0x" + Res.ToString("X8"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            Int32 Res = wclBluetoothManager.Close();
            if (Res != wclErrors.WCL_E_SUCCESS)
                MessageBox.Show("Error: 0x" + Res.ToString("X8"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            wclBluetoothRadio Radio = GetRadio();
            if (Radio != null)
            {
                Int32 Res = wclBluetoothLeBeaconWatcher.Start(Radio);
                if (Res != wclErrors.WCL_E_SUCCESS)
                    MessageBox.Show("Error: 0x" + Res.ToString("X8"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btStop_Click(object sender, RoutedEventArgs e)
        {
            if (!wclBluetoothManager.Active)
                MessageBox.Show("Bluetooth Manager was not opened", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
            {
                Int32 Res = wclBluetoothLeBeaconWatcher.Stop();
                if (Res != wclErrors.WCL_E_SUCCESS)
                    MessageBox.Show("Error: 0x" + Res.ToString("X8"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btClear_Click(object sender, RoutedEventArgs e)
        {
            lbFrames.Items.Clear();
        }

        private void btClearLog_Click(object sender, RoutedEventArgs e)
        {
            ListBox.Items.Clear();
        }

        private void btStopAdvertising_Click(object sender, RoutedEventArgs e)
        {
            Int32 Res = wclBluetoothLeAdvertiser.Stop();
            if (Res != wclErrors.WCL_E_SUCCESS)
                ListBox.Items.Add("Stop advertiser failed: 0x" + Res.ToString("8"));
        }

        private void btStartAdvertising_Click(object sender, RoutedEventArgs e)
        {
            Guid BEACON_UUID = new Guid("{09039835-4A80-443B-87AA-DC565D09EA61}");

            wclBluetoothLeAdvertiser.Stop();

            wclBluetoothRadio Radio = GetRadio();
            if (Radio != null)
            {
                wclBluetoothLeAdvertisement Adv;

                // Create and add iBeacon advdertisement.
                if (cbIBeacon.IsChecked.Value)
                {
                    Adv = new wclBluetoothLeiBeaconAdvertisement(-5, 0x0101, 0x0202, BEACON_UUID);
                    AddAdvertisement(Adv);
                }

                // Create and add Proximity Beacon advertisement.
                if (cbProximityBeacon.IsChecked.Value)
                {
                    Adv = new wclBluetoothLeProximityBeaconAdvertisement(-5, 0x0101, 0x0202,
                        BEACON_UUID, 0xFFFE);
                    AddAdvertisement(Adv);
                }

                // Create and add Alt Beacon advertisement.
                if (cbAltBeacon.IsChecked.Value)
                {
                    Adv = new wclBluetoothLeAltBeaconAdvertisement(-5, 0x0101, 0x0202,
                        BEACON_UUID, 0xFFFE, 0x11);
                    AddAdvertisement(Adv);
                }

                // Create and add Eddystone UID advertisement.
                if (cbEddystoneUid.IsChecked.Value)
                {
                    Adv = new wclBluetoothLeEddystoneUidBeaconAdvertisement(-5, BEACON_UUID);
                    AddAdvertisement(Adv);
                }

                // Create and add Eddystone URL.
                if (cbEddystoneUrl.IsChecked.Value)
                {
                    Adv = new wclBluetoothLeEddystoneUrlBeaconAdvertisement(-5, "https://www.btframework.com");
                    AddAdvertisement(Adv);
                }

                Byte[] Data;

                // Create and add custom advertisement.
                if (cb128SolUuid.IsChecked.Value)
                {
                    Data = new Byte[16];
                    Data[0] = 0xD0;
                    Data[1] = 0x00;
                    Data[2] = 0x2D;
                    Data[3] = 0x12;
                    Data[4] = 0x1E;
                    Data[5] = 0x4B;
                    Data[6] = 0x0F;
                    Data[7] = 0xA4;
                    Data[8] = 0x99;
                    Data[9] = 0x4E;
                    Data[10] = 0xCE;
                    Data[11] = 0xB5;
                    Data[12] = 0x31;
                    Data[13] = 0xF4;
                    Data[14] = 0x05;
                    Data[15] = 0x79;
                    Adv = new wclBluetoothLeCustomAdvertisement(wclUUIDs.LE_GAP_AD_TYPE_SERVICE_SOL_UUIDS_128, Data);
                    AddAdvertisement(Adv);
                }

                // Create and add manufacturer specific advertisement.
                if (cbManufacturer.IsChecked.Value) 
                {
                    Data = new Byte[2];
                    Data[0] = 0x12;
                    Data[1] = 0x34;
                    Adv = new wclBluetoothLeManufacturerAdvertisement(0x010E, Data);
                    AddAdvertisement(Adv);
                }

                if (cb16UuidService.IsChecked.Value)
                {
                    Adv = new wclBluetoothLe16ServiceAdvertisement(0x9835);
                    AddAdvertisement(Adv);
                }

                if (cb32UuidService.IsChecked.Value)
                {
                    Adv = new wclBluetoothLe32ServiceAdvertisement(0x09039835);
                    AddAdvertisement(Adv);
                }

                if (cb128UuidService.IsChecked.Value)
                {
                    Adv = new wclBluetoothLe128ServiceAdvertisement(BEACON_UUID);
                    AddAdvertisement(Adv);
                }

                if (cb16UuidData.IsChecked.Value)
                {
                    Data = new Byte[2];
                    Data[0] = 0x12;
                    Data[1] = 0x34;
                    Adv = new wclBluetoothLe16ServiceDataAdvertisement(0x9835, Data);
                    AddAdvertisement(Adv);
                }

                if (cb32UuidData.IsChecked.Value)
                {
                    Data = new Byte[2];
                    Data[0] = 0x12;
                    Data[1] = 0x34;
                    Adv = new wclBluetoothLe32ServiceDataAdvertisement(0x09039835, Data);
                    AddAdvertisement(Adv);
                }

                if (cb128UuidData.IsChecked.Value)
                {
                    Data = new Byte[2];
                    Data[0] = 0x12;
                    Data[1] = 0x34;
                    Adv = new wclBluetoothLe128ServiceDataAdvertisement(BEACON_UUID,
                        Data);
                    AddAdvertisement(Adv);
                }

                if (cbCustomRaw.IsChecked.Value)
                {
                    Data = new Byte[20];

                    // 16 bit Service Data
                    Data[0] = 0x09; // Length.
                    Data[1] = wclUUIDs.LE_GAP_AD_TYPE_SERVICE_DATA_16; // Data type.
                    Data[2] = 0x12; // 16 UUID LO BYTE
                    Data[3] = 0x34; // 16 UUID HI BYTE
                    Data[4] = 0xFF; // Data 1st byte
                    Data[5] = 0xFE; // Data 2st byte
                    Data[6] = 0xFD; // Data 3st byte
                    Data[7] = 0xFC; // Data 4st byte
                    Data[8] = 0xFB; // Data 5st byte
                    Data[9] = 0xFA; // Data 6st byte
                    // Manufacturer specific data.
                    Data[10] = 0x09; // Length.
                    Data[11] = wclUUIDs.LE_GAP_AD_TYPE_MANUFACTURER; // Data type.
                    Data[12] = 0x10; // Company ID.
                    Data[13] = 0x12; // Company ID.
                    Data[14] = 0x01; // Data 1st byte
                    Data[15] = 0x02; // Data 2st byte
                    Data[16] = 0x03; // Data 3st byte
                    Data[17] = 0x04; // Data 4st byte
                    Data[18] = 0x05; // Data 5st byte
                    Data[19] = 0x06; // Data 6st byte

                    Adv = new wclBluetoothLeRawAdvertisement(Data);
                    AddAdvertisement(Adv);
                }

                if (wclBluetoothLeAdvertiser.Count == 0)
                    MessageBox.Show("Select at least one advertisement type.");
                else
                {
                    wclBluetoothLeAdvertiser.UseExtendedAdvertisement = cbUseExtended.IsChecked.Value;
                    wclBluetoothLeAdvertiser.Anonymous = cbAnonymous.IsChecked.Value;
                    wclBluetoothLeAdvertiser.IncludeTxRssi = cbIncludeTxRssi.IsChecked.Value;
                    wclBluetoothLeAdvertiser.PreferredTxRssi = 10;

                    Int32 Res = wclBluetoothLeAdvertiser.Start(Radio);
                    if (Res != wclErrors.WCL_E_SUCCESS)
                    {
                        ListBox.Items.Add("Start advertiser failed: 0x" + Res.ToString("8"));
                        wclBluetoothLeAdvertiser.Clear();
                    }
                }
            }
        }

        private void btGetScanParams_Click(object sender, RoutedEventArgs e)
        {
            wclBluetoothRadio Radio = GetRadio();
            if (Radio != null)
            {
                UInt16 Interval;
                UInt16 Window;
                Int32 Res = Radio.GetLeScanParams(out Interval, out Window);
                if (Res == wclErrors.WCL_E_SUCCESS)
                {
                    edScanInterval.Text = Interval.ToString();
                    edScanWindow.Text = Window.ToString();
                }
                else
                    MessageBox.Show("Error: 0x" + Res.ToString("X8"));
            }
        }

        private void btSetScanParams_Click(object sender, RoutedEventArgs e)
        {
            wclBluetoothRadio Radio = GetRadio();
            if (Radio != null)
            {
                UInt16 Interval = Convert.ToUInt16(edScanInterval.Text);
                UInt16 Window = Convert.ToUInt16(edScanWindow.Text);
                Int32 Res = Radio.SetLeScanParams(Interval, Window);
                if (Res != wclErrors.WCL_E_SUCCESS)
                    MessageBox.Show("Error: 0x" + Res.ToString("X8"));
                else
                    MessageBox.Show("Scan parameters have been changed. Restart Beacon Watcher");
            }
        }

        private void btGetAdvParams_Click(object sender, RoutedEventArgs e)
        {
            wclBluetoothRadio Radio = GetRadio();
            if (Radio != null)
            {
                UInt16 Interval;
                Int32 Res = Radio.GetLeAdvParams(out Interval);
                if (Res == wclErrors.WCL_E_SUCCESS)
                    edAdvInterval.Text = Interval.ToString();
                else
                    MessageBox.Show("Error: 0x" + Res.ToString("X8"));
            }
        }

        private void btSetAdvParams_Click(object sender, RoutedEventArgs e)
        {
            wclBluetoothRadio Radio = GetRadio();
            if (Radio != null)
            {
                UInt16 Interval = Convert.ToUInt16(edAdvInterval.Text);
                Int32 Res = Radio.SetLeAdvParams(Interval);
                if (Res != wclErrors.WCL_E_SUCCESS)
                    MessageBox.Show("Error: 0x" + Res.ToString("X8"));
                else
                    MessageBox.Show("Advertisement parameters have been changed. Restart Advertiser");
            }
        }

        private void EnableAdvertisements(Boolean Enable)
        {
            cbIBeacon.IsEnabled = Enable;
            cbProximityBeacon.IsEnabled = Enable;
            cbAltBeacon.IsEnabled = Enable;
            cbEddystoneUid.IsEnabled = Enable;
            cbEddystoneUrl.IsEnabled = Enable;
            cb128SolUuid.IsEnabled = Enable;
            cbManufacturer.IsEnabled = Enable;
            cb16UuidService.IsEnabled = Enable;
            cb32UuidService.IsEnabled = Enable;
            cb128UuidService.IsEnabled = Enable;
            cb16UuidData.IsEnabled = Enable;
            cb32UuidData.IsEnabled = Enable;
            cb128UuidData.IsEnabled = Enable;
            cbCustomRaw.IsEnabled = Enable;
        }

        private void tbMultiplier_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            wclBluetoothLeAdvertiser.Multiplier = (Byte)(tbMultiplier.Value);
            laMultiplier.Content = wclBluetoothLeAdvertiser.Multiplier.ToString();
        }
    }
}
