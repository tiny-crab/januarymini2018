using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Net.Http;
using System.Text;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DeviceInformation hrmInfo = null;
        BluetoothLEDevice bluetoothLeDevice = null;

        public MainPage()
        {
            this.InitializeComponent();
            Debug.WriteLine("TEst");
            CreateFile();
            QueryDevices();

        }

        private async void  CreateFile()
        {
            // Create sample file; replace if exists.
            Windows.Storage.StorageFolder storageFolder =
                Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile sampleFile =
                await storageFolder.CreateFileAsync("data.txt",
                    Windows.Storage.CreationCollisionOption.ReplaceExisting);
        }


        void QueryDevices()
        {
            // Query for extra properties you want returned
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

            DeviceWatcher deviceWatcher =
                        DeviceInformation.CreateWatcher(
                                BluetoothLEDevice.GetDeviceSelectorFromPairingState(false),
                                requestedProperties,
                                DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            // Added, Updated and Removed are required to get all nearby devices
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;

            // EnumerationCompleted and Stopped are optional to implement.
            deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            deviceWatcher.Stopped += DeviceWatcher_Stopped;

            // Start the watcher.
            deviceWatcher.Start();
        }

        private void DeviceWatcher_Stopped(DeviceWatcher sender, object deviceInfo)
        {
            Debug.WriteLine("DeviceWatcher_Stopped");
        }

        private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object deviceInfo)
        {
            Debug.WriteLine("DeviceWatcher_EnumerationCompleted");
        }

        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate deviceInfo)
        {
            Debug.WriteLine("DeviceWatcher_Removed " + deviceInfo.Id);
        }

        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceInfo)
        {
            Debug.WriteLine("DeviceWatcher_Updated " + deviceInfo.Id);
        }

        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            Debug.WriteLine("DeviceWatcher_Added " + deviceInfo.Id + " " + deviceInfo.Name);
            if (deviceInfo.Name == "RHYTHM+51532")
            {
                hrmInfo = deviceInfo;
                GetDevice();
            }
        }


        void Characteristic_ValueChanged(GattCharacteristic sender,
                                            GattValueChangedEventArgs args)
        {
            // An Indicate or Notify reported that the value has changed.
            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            // Parse the data however required.
            string res = "";// Keep reading until we consume the complete stream.

            Debug.Write("Byte ");
            byte[] bytes = new byte[reader.UnconsumedBufferLength];
            int i = 0;
            while (reader.UnconsumedBufferLength > 0)
            {
                // Note that the call to readString requires a length of "code units" 
                // to read. This is the reason each string is preceded by its length 
                // when "on the wire".
                byte newByte = reader.ReadByte();
                bytes[i++] = newByte;
            }
            Debug.Write(BitConverter.ToString(bytes));
            Debug.WriteLine("");

            int rate = bytes[1];
            Debug.WriteLine("Heart rate " + rate);
            WriteToFile(rate);

        }

        private async void WriteToFile(int rate)
        {
            // Create sample file; replace if exists.
            Windows.Storage.StorageFolder storageFolder =
                Windows.Storage.ApplicationData.Current.LocalFolder;

            Debug.Write(storageFolder.Path);

            Windows.Storage.StorageFile sampleFile =
                await storageFolder.GetFileAsync("data.txt");

            await Windows.Storage.FileIO.WriteTextAsync(sampleFile, rate.ToString());
        }

        private async void GetDevice()
        {
            if (hrmInfo != null)
            { // Note: BluetoothLEDevice.FromIdAsync must be called from a UI thread because it may prompt for consent.
                bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(hrmInfo.Id);


                Debug.WriteLine("ConnectDevice " + hrmInfo.Id + " " + hrmInfo.Name);
                // ...

                GattDeviceServicesResult result = await bluetoothLeDevice.GetGattServicesAsync();

                GattDeviceService service = null;

                if (result.Status == GattCommunicationStatus.Success)
                {
                    var services = result.Services;

                    foreach (GattDeviceService serv in services)
                    {
                        Debug.WriteLine("Service " + serv.Uuid.ToString());

                        if (serv.Uuid.ToString() == "0000180d-0000-1000-8000-00805f9b34fb")
                        {
                            service = serv;
                        }
                    }
                }



                GattCharacteristic selChar = null;
                if (service != null)
                {
                    GattCharacteristicsResult result2 = await service.GetCharacteristicsAsync();

                    if (result.Status == GattCommunicationStatus.Success)
                    {
                        var characteristics = result2.Characteristics;
                        Debug.WriteLine("Got characteristics");

                        foreach (GattCharacteristic characteristic in characteristics)
                        {
                            Debug.WriteLine("Characteristic " + characteristic.Uuid);
                            if (characteristic.Uuid.ToString() == "00002a37-0000-1000-8000-00805f9b34fb")
                            {
                                selChar = characteristic;
                            }
                        }
                    }
                }

                if (selChar != null)
                {
                    GattCharacteristicProperties properties = selChar.CharacteristicProperties;


                    if (properties.HasFlag(GattCharacteristicProperties.Read))
                    {
                        Debug.WriteLine("Can read");
                    }
                    if (properties.HasFlag(GattCharacteristicProperties.Write))
                    {
                        Debug.WriteLine("Can write");
                    }
                    if (properties.HasFlag(GattCharacteristicProperties.Notify))
                    {
                        Debug.WriteLine("Can notify");

                        GattCommunicationStatus status = await selChar.WriteClientCharacteristicConfigurationDescriptorAsync(
                        GattClientCharacteristicConfigurationDescriptorValue.Notify);
                        if (status == GattCommunicationStatus.Success)
                        {
                            Debug.WriteLine("Server has been informed of clients interest");
                            selChar.ValueChanged += Characteristic_ValueChanged;
                        }
                    }
                }
            }
        }
    }
}
