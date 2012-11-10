using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Diagnostics;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json.Linq;
using UWAPIWrapper;

namespace PanoramaApp1
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;

            GenericRequest req = new GenericRequest();
            req.requestDataInJSONWithoutQuery("WatPark", "cc7004c25526969882ff31eddb1d18f4", jsonParseFail);
            req.responseCompletionHandler += new GenericRequest.onGetResponseFromRequest(jsonParseSuccessful);
        }

        private void jsonParseSuccessful(GenericRequest req, string method_name, JObject obj)
        {
            try
            {
                JToken response = obj["response"];
                response = response["data"];
                JArray parkings = (JArray)response["result"];
                foreach (JContainer parking_lot in parkings)
                {

                    string lot_name = (string)parking_lot["LotName"];
                    string latlong_combine = (string)parking_lot["LatLong"];
                    string opentime = (string)parking_lot["OpenTime"];
                    string closetime = (string)parking_lot["CloseTime"];
                    string curr_count = (string)parking_lot["LatestCount"];
                    string time_polled = (string)parking_lot["TimePolled"];
                    //int capacity = parking_lot["Capacity"];
                    string percentage_filled = (string)parking_lot["PercentFilled"];
                    Debug.WriteLine(lot_name);
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        setText(percentage_filled, opentime, closetime);
                    });  
                }
            }
            catch (NullReferenceException e)
            {
                Debug.WriteLine("ERROR! NULL pointer exception!");
            }
        }

        private void setText(string percentage_filled, string opentime, string closetime)
        {
            percentage_text_box.Text = "Percentage Filled: " + percentage_filled + "%";
            operation_text_box.Text = "Open From: " + opentime + " to " + closetime;
            progress_bar_c.Value = Convert.ToInt32(percentage_filled);
        }

        private void jsonParseFail(GenericRequest request, string method_name, Exception e)
        {
            Debug.WriteLine("ERROR! Request failed for some reason!");
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }

            
        }
    }
}