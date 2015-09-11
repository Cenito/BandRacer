using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.System.Display;
using BandRacer.Plugin;
using Microsoft.Band;
using Microsoft.Band.Personalization;
using Microsoft.Band.Sensors;
using UnityEngine.Windows;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Template
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private SplashScreen splash;
		private Rect splashImageRect;
		private WindowSizeChangedEventHandler onResizeHandler;

		public MainPage(SplashScreen splashScreen)
		{
			this.InitializeComponent();

			splash = splashScreen;
			GetSplashBackgroundColor();
			OnResize();
			onResizeHandler = new WindowSizeChangedEventHandler((o, e) => OnResize());
			Window.Current.SizeChanged += onResizeHandler;
		}

		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.  The Parameter
		/// property is typically used to configure the page.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			splash = (SplashScreen)e.Parameter;
			OnResize();
		}

	    private IBandClient m_Player1BandClient;
        private IBandClient m_Player2BandClient;
	    private string m_Player1Name;
	    private string m_Player2Name;
	    private DisplayRequest m_DisplayRequest;

	    private PlatformBase Current
	    {
	        get { return PlatformBase.Current; }
	    }

	    private async void ConnectBand()
	    {
	        var bandInitializationFailed = false;
	        var pairedBands = await BandClientManager.Instance.GetBandsAsync();
	        try
	        {
	            for (int index = 0; index < pairedBands.Length; index++)
	            {
                    if (index == 0)
                    {
                        m_Player1BandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[index]);
                        m_Player1BandClient.SensorManager.Accelerometer.ReportingInterval = TimeSpan.FromMilliseconds(16.0);
                        m_Player1Name = pairedBands[index].Name;
                        UpdatePlayer1Name();

                        m_Player1BandClient.SensorManager.Accelerometer.ReadingChanged += OnAccelerometerPlayer1OnReadingChanged;

                        await m_Player1BandClient.SensorManager.Accelerometer.StartReadingsAsync();   
                        
                    }
                    if (index == 1)
                    {
                        m_Player2BandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[index]);
                        m_Player2BandClient.SensorManager.Accelerometer.ReportingInterval = TimeSpan.FromMilliseconds(16.0);
                        m_Player2Name = pairedBands[index].Name;
                        UpdatePlayer2Name();

                        m_Player2BandClient.SensorManager.Accelerometer.ReadingChanged += OnAccelerometerPlayer2OnReadingChanged;

                        await m_Player2BandClient.SensorManager.Accelerometer.StartReadingsAsync();
                    }
	            }
	            
	        }
	        catch (Exception ex)
	        {
	            Debug.WriteLine(ex.Message);
	            bandInitializationFailed = true;
	        }

	        if (bandInitializationFailed)
	        {
                await DisconnectBands();
	        }
	    }

	    private void UpdatePlayer2Name()
	    {
	        if (Current != null && !string.Equals(m_Player2Name, Current.Player2Name, StringComparison.CurrentCultureIgnoreCase))
	        {
	            Current.Player2Name = m_Player2Name;
	        }
	    }

	    private void UpdatePlayer1Name()
	    {
	        if (Current != null && !string.Equals(Current.Player1Name, m_Player1Name, StringComparison.CurrentCultureIgnoreCase))
	        {
	            Current.Player1Name = m_Player1Name;
	        }
	    }

	    private async Task DisconnectBands()
	    {
	        if (m_Player1BandClient != null)
	        {
	            await m_Player1BandClient.SensorManager.Accelerometer.StopReadingsAsync();
	            m_Player1BandClient.SensorManager.Accelerometer.ReadingChanged -= OnAccelerometerPlayer1OnReadingChanged;
	            m_Player1BandClient.Dispose();
	            m_Player1BandClient = null;
	        }

            if (m_Player2BandClient != null)
            {
                await m_Player2BandClient.SensorManager.Accelerometer.StopReadingsAsync();
                m_Player2BandClient.SensorManager.Accelerometer.ReadingChanged -= OnAccelerometerPlayer2OnReadingChanged;
                m_Player2BandClient.Dispose();
                m_Player2BandClient = null;
            }
	    }

        private void OnAccelerometerPlayer2OnReadingChanged(object sender, BandSensorReadingEventArgs<IBandAccelerometerReading> args)
	    {
	        var yReading = args.SensorReading.AccelerationY;
	        var xReading = args.SensorReading.AccelerationX;

	        CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
	        {
	            if (Current != null)
	            {
	                Current.YAnglePlayer2 = (float) yReading;
	                Current.XAnglePlayer2 = (float) xReading;
                    UpdatePlayer2Name();
	            }
	        });
	    }

	    private void OnAccelerometerPlayer1OnReadingChanged(object sender, BandSensorReadingEventArgs<IBandAccelerometerReading> args)
	    {
	        var yReading = args.SensorReading.AccelerationY;
	        var xReading = args.SensorReading.AccelerationX;

	        CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
	        {
	            if (Current != null)
	            {
	                Current.YAnglePlayer1 = (float) yReading;
	                Current.XAnglePlayer1 = (float) xReading;
	                UpdatePlayer1Name();
	            }
	        });
	    }

	    private void OnResize()
		{
			if (splash != null)
			{
				splashImageRect = splash.ImageLocation;
				PositionImage();
			}
		}

		private void PositionImage()
		{
			var inverseScaleX = 1.0f / DXSwapChainPanel.CompositionScaleX;
			var inverseScaleY = 1.0f / DXSwapChainPanel.CompositionScaleY;

			ExtendedSplashImage.SetValue(Canvas.LeftProperty, splashImageRect.X * inverseScaleX);
			ExtendedSplashImage.SetValue(Canvas.TopProperty, splashImageRect.Y * inverseScaleY);
			ExtendedSplashImage.Height = splashImageRect.Height * inverseScaleY;
			ExtendedSplashImage.Width = splashImageRect.Width * inverseScaleX;
		}

		private async void GetSplashBackgroundColor()
		{
			try
			{
				StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///AppxManifest.xml"));
				string manifest = await FileIO.ReadTextAsync(file);
				int idx = manifest.IndexOf("SplashScreen");
				manifest = manifest.Substring(idx);
				idx = manifest.IndexOf("BackgroundColor");
				if (idx < 0)  // background is optional
					return;
				manifest = manifest.Substring(idx);
				idx = manifest.IndexOf("\"");
				manifest = manifest.Substring(idx + 2); // also remove quote and # char after it
				idx = manifest.IndexOf("\"");
				manifest = manifest.Substring(0, idx);
				int value = Convert.ToInt32(manifest, 16) & 0x00FFFFFF;
				byte r = (byte)(value >> 16);
				byte g = (byte)((value & 0x0000FF00) >> 8);
				byte b = (byte)(value & 0x000000FF);

				await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.High, delegate()
					{
						ExtendedSplashGrid.Background = new SolidColorBrush(Color.FromArgb(0xFF, r, g, b));
					});
			}
			catch (Exception)
			{ }
		}

		public SwapChainPanel GetSwapChainPanel()
		{
			return DXSwapChainPanel;
		}

		public void RemoveSplashScreen()
		{
			DXSwapChainPanel.Children.Remove(ExtendedSplashGrid);
			if (onResizeHandler != null)
			{
				Window.Current.SizeChanged -= onResizeHandler;
				onResizeHandler = null;
			}

            m_DisplayRequest = new Windows.System.Display.DisplayRequest();
            m_DisplayRequest.RequestActive();

		    Application.Current.Suspending -= OnApplicationSuspending;
		    Application.Current.Suspending += OnApplicationSuspending;

            ConnectBand();
		}

	    private async void OnApplicationSuspending(object sender, SuspendingEventArgs suspendingEventArgs)
	    {
            SuspendingDeferral deferral = suspendingEventArgs.SuspendingOperation.GetDeferral();
            m_DisplayRequest.RequestRelease();
	        await DisconnectBands();
            deferral.Complete();
	    }

#if !UNITY_WP_8_1
		protected override Windows.UI.Xaml.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
		{
			return new UnityPlayer.XamlPageAutomationPeer(this);
		}
#endif
	}
}
