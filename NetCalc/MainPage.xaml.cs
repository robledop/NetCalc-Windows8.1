using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using System.Windows;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NetCalc
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<string> masksList;

        public MainPage()
        {
            this.InitializeComponent();

            masksList = new List<string>();

            masksList.Add("128.0.0.0");
            masksList.Add("192.0.0.0");
            masksList.Add("224.0.0.0");
            masksList.Add("240.0.0.0");
            masksList.Add("248.0.0.0");
            masksList.Add("252.0.0.0");
            masksList.Add("254.0.0.0");
            masksList.Add("255.0.0.0");
            masksList.Add("255.128.0.0");
            masksList.Add("255.192.0.0");
            masksList.Add("255.224.0.0");
            masksList.Add("255.240.0.0");
            masksList.Add("255.248.0.0");
            masksList.Add("255.252.0.0");
            masksList.Add("255.254.0.0");
            masksList.Add("255.255.0.0");
            masksList.Add("255.255.128.0");
            masksList.Add("255.255.192.0");
            masksList.Add("255.255.224.0");
            masksList.Add("255.255.240.0");
            masksList.Add("255.255.248.0");
            masksList.Add("255.255.252.0");
            masksList.Add("255.255.254.0");
            masksList.Add("255.255.255.0");
            masksList.Add("255.255.255.128");
            masksList.Add("255.255.255.192");
            masksList.Add("255.255.255.224");
            masksList.Add("255.255.255.240");
            masksList.Add("255.255.255.248");
            masksList.Add("255.255.255.252");
            masksList.Add("255.255.255.254");
            masksList.Add("255.255.255.255");

            cmbSubnetMask.ItemsSource = masksList;
            cmbSubnetMask.SelectedIndex = 23;
            cmbNumberOfSubnets.SelectedIndex = 0;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //await Windows.System.Threading.ThreadPool.RunAsync((a) =>
            //    {
            
            //    });

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Calculate();    
            });
            
        }

        private void cmbSubnetMask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbSubnetMask.SelectedItem != null)
                {
                    IPSegment ipNetwork = new IPSegment(txtAddressBlock.Text, cmbSubnetMask.SelectedItem.ToString());
                    IPSegmentCollection ipNetCollection = new IPSegmentCollection(ipNetwork, 32);

                    uint maxSubnets = Convert.ToUInt32(ipNetCollection.Count);
                    List<uint> sub = new List<uint>();

                    do
                    {
                        sub.Add(maxSubnets);
                        maxSubnets = maxSubnets / 2;
                    } while (maxSubnets >= 1);

                    sub.Reverse();

                    cmbNumberOfSubnets.ItemsSource = sub;
                    cmbNumberOfSubnets.SelectedIndex = 0;
                }
            }
            catch (Exception)
            {
                MessageDialog msg = new MessageDialog("Invalid IP Address");
                msg.ShowAsync();
            }
        }

        private void Calculate()
        {
            try
            {
                txtAddressBlock.Text = txtAddressBlock.Text.Replace(",", ".");

                IPSegment ipNetwork = new IPSegment(txtAddressBlock.Text, cmbSubnetMask.SelectedItem.ToString());

                double numberOfSubnettingOptions = 0;
                if ((uint)cmbNumberOfSubnets.SelectedItem != 1)
                {
                    numberOfSubnettingOptions = Math.Round(Math.Log(Convert.ToDouble(cmbNumberOfSubnets.SelectedItem), 2));
                }

                byte subnetting = (byte)(ipNetwork.CIDR + numberOfSubnettingOptions);

                IPSegmentCollection ipSegmentCollection = new IPSegmentCollection(ipNetwork, subnetting);

                lstSubnets.ItemsSource = ipSegmentCollection;
                if (ipSegmentCollection.Count >= 65535)
                {
                    this.listLimit.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    this.listLimit.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }

                if (ipSegmentCollection.Current.CIDR == 31)
                {
                    hyperlinkRFC3021.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    hyperlinkRFC3021.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageDialog dlg = new MessageDialog("Invalid IP Address");
                dlg.ShowAsync();
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                //progressBar.Visibility = System.Windows.Visibility.Collapsed;
            }

        }

        private async void aboutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog about = new MessageDialog("NETCALC", "About");
            await about.ShowAsync();
        }

        private async void AdControl_ErrorOccurred(object sender, Microsoft.Advertising.WinRT.UI.AdErrorEventArgs e)
        {
            //MessageDialog msg = new MessageDialog(e.Error.Message);
            //await msg.ShowAsync();
        }
    }
}
