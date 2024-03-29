﻿using System;
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
        private void beginRequestingData()
        {
            bottom_progress_bar.Visibility = System.Windows.Visibility.Visible;
            refresh_button.IsEnabled = false;

            GenericRequest req = new GenericRequest();
            req.requestDataInJSONWithoutQuery("WatPark", "cc7004c25526969882ff31eddb1d18f4");
            req.responseCompletionHandler += new GenericRequest.onGetResponseFromRequest(jsonParseSuccessful);
            req.requestFailedHandler += new GenericRequest.onFailToGetResponse(jsonParseFail);
        }

        private void showErrorMessageBox()
        {
            MessageBox.Show("Please try again later.", "No Network Connection", MessageBoxButton.OK);
            refresh_button.IsEnabled = true;
            bottom_progress_bar.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void showAboutMessageBox()
        {
            MessageBox.Show("WatPark for WP8, version 1.0.0\n"
                           + "2012 © Molly Luo, Elton Gao\n\n"
                           + "Parking Lots Data from\n"
                           + "UWaterloo Open Data Initiative\n"
                           + "\tapi.uwaterloo.ca\n"
                           + "with\n"
                           + "\tWatPark.ca\n"
                           + "Thank you for your guys' hard work!\n\n"
                           + "Note: This app is not affiliated with University of Waterloo or WatPark.ca"
                           + "\n\nThis app uses UWAPIWrapper\n"
                           + "Created by Elton Gao, UW Mobile Club\n"
                           + "Open sourced at github/uwmobile"
                           , "About WatPark", MessageBoxButton.OK);
        }

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
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
                    string capacity = (string)parking_lot["Capacity"];
                    string percentage_filled = (string)parking_lot["PercentFilled"];
                    Debug.WriteLine(lot_name);
                    if (lot_name.CompareTo("C") == 0)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            setText_C(percentage_filled, opentime, closetime, capacity);
                        });
                    }
                    else if (lot_name.CompareTo("W") == 0)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            setText_W(percentage_filled, opentime, closetime, capacity);
                        });
                    }
                    else if (lot_name.CompareTo("N") == 0)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            setText_N(percentage_filled, opentime, closetime, capacity);
                        });
                    }
                }
            }
            catch (NullReferenceException e)
            {
                Debug.WriteLine("ERROR! NULL pointer exception!");
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    showErrorMessageBox();
                });
            }
        }

        private void setProgressBarInvisible()
        {
            refresh_button.IsEnabled = true;
            bottom_progress_bar.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void setText_C(string percentage_filled, string opentime, string closetime, string capacity)
        {
            setProgressBarInvisible();

            int count = (100 - Convert.ToInt32(percentage_filled)) * Convert.ToInt32(capacity) / 100;
            slots_num_text_box_c.Text = Convert.ToString(count) + " Lots Available";
            percentage_text_box_c.Text = slots_num_text_box_c.Text + "\n" + capacity + " in Total";// "Percentage Filled: " + percentage_filled + "%";
            operation_text_box_c.Text = "Open From: " + opentime + " to " + closetime;
        }

        private void setText_W(string percentage_filled, string opentime, string closetime, string capacity)
        {
            setProgressBarInvisible();

            int count = (100 - Convert.ToInt32(percentage_filled)) * Convert.ToInt32(capacity) / 100;
            slots_num_text_box_w.Text = Convert.ToString(count) + " Lots Available";
            percentage_text_box_w.Text = slots_num_text_box_w.Text + "\n" + capacity + " in Total";// "Percentage Filled: " + percentage_filled + "%";
            operation_text_box_w.Text = "Open From: " + opentime + " to " + closetime;
        }

        private void setText_N(string percentage_filled, string opentime, string closetime, string capacity)
        {
            setProgressBarInvisible();

            int count = (100 - Convert.ToInt32(percentage_filled)) * Convert.ToInt32(capacity) / 100;
            slots_num_text_box_n.Text = Convert.ToString(count) + " Lots Available";
            percentage_text_box_n.Text = slots_num_text_box_n.Text + "\n" + capacity + " in Total";// "Percentage Filled: " + percentage_filled + "%";
            operation_text_box_n.Text = "Open From: " + opentime + " to " + closetime;
        }

        private void jsonParseFail(GenericRequest request, string method_name, Exception e)
        {
            Debug.WriteLine("ERROR! Request failed for some reason!");
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                showErrorMessageBox();
            });
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //if (!App.ViewModel.IsDataLoaded)
            //{
            //    App.ViewModel.LoadData();
            //}

            beginRequestingData();
        }

        private void onRefreshClicked(object sender, RoutedEventArgs e)
        {
            beginRequestingData();
        }

        private void onAboutClicked(object sender, RoutedEventArgs e)
        {
            showAboutMessageBox();
        }
    }
}