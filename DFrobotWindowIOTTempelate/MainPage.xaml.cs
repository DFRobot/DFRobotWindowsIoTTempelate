using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Maker.RemoteWiring;
using Microsoft.Maker.Serial;
using System;
using System.Diagnostics;

namespace DFrobotWindowIoTTempelate
{
    public sealed partial class MainPage : Page
    {
        UsbSerial usb;                          //Handle the USB connction
        RemoteDevice arduino;                   //Handle the arduino
        private DispatcherTimer readTemperatureTimer;     //Timer for the LED to blink every one second
        private const string LM35_PIN = "A5";         //Pin number of the LM35 Sensor

        public MainPage()
        {
            this.InitializeComponent();

            //USB VID and PID of the "Arduino Expansion Shield for Raspberry Pi B+"
            usb = new UsbSerial("VID_2341", "PID_8036");

            //Arduino RemoteDevice Constractor via USB.
            arduino = new RemoteDevice(usb);
            //Add DeviceReady callback when connecting successfully
            arduino.DeviceReady += onDeviceReady;

            //Baudrate on 57600 and SerialConfig.8N1 is the default config for Arduino devices over USB
            usb.begin(57600, SerialConfig.SERIAL_8N1);
        }

        private void onDeviceReady()
        {
            //After device is ready this function will be evoked.

            //Debug message "Device Ready" will be shown in the "Output" dialog.
            Debug.WriteLine("Device Ready");
            var action = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
            {
                setup();
            }));
        }

        private void setup()
        {
            //Set the pin mode of the led.
            arduino.pinMode(LM35_PIN, PinMode.ANALOG);

            //Set the timer to schedule every 500 ms.
            readTemperatureTimer = new DispatcherTimer();
            readTemperatureTimer.Interval = TimeSpan.FromMilliseconds(500);
            readTemperatureTimer.Tick += readTemperature;
            readTemperatureTimer.Start();
        }

        private void readTemperature(object sender, object e)
        {
            //Read analog value from 0 to 1023, which maps from 0 to 5V
            int temperatureValue = arduino.analogRead(LM35_PIN);

            //Convert analog value to temperature.
            double temperature = (500.0 * temperatureValue) / 1024.0;

            //Print temperature to Output.
            Debug.WriteLine(temperature);
        }
    }
}