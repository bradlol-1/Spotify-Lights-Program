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
using System.Windows.Threading;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using System.Timers;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;
using System.IO.Ports;
using System.Data;
using System.Drawing;
using static SpotifyAPI.Web.Scopes;

namespace WpfApp1
{

    public partial class Window1 : Window 
    {
        //Create SerialPort Object
        private static SerialPort port = new SerialPort("COM3", 9600);
        //Create Time Object
        DispatcherTimer time = new DispatcherTimer();
        DispatcherTimer beat = new DispatcherTimer();

        //Create variable for the spotifyclient
        SpotifyClient spotifyclient;

        bool[] colors = new bool[8];

        bool t = true;
        bool overwritten;
        string oldID = "poop";
        
        public Window1(SpotifyClient _spot)
        {
            InitializeComponent();
            spotifyclient = _spot;
            //start update timer
            time.Interval = new TimeSpan(0, 0, 0, 0, 500);
            time.Start();
            time.Tick += new EventHandler(dispatcherTimer_Tick);

        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            
            //If spotify client is detected
            if (spotifyclient != null)
            {
                //gets the currently playing track - refer to the same code and comments in Mainwindow.xaml.cs 
                var current = new PlayerCurrentlyPlayingRequest(PlayerCurrentlyPlayingRequest.AdditionalTypes.Track);
                if (current != null)
                {
                    var currentlyplaying =  spotifyclient.Player.GetCurrentlyPlaying(current).Result;

                    if (currentlyplaying != null)
                    {

                        var currently = currentlyplaying.Item;


                        if (currently is FullTrack track1)
                        {
                            //creates a streamreader variable to read what is inside the txt file
                            var streamreader = new StreamReader("C:\\Text Samples\\Sample.txt");
                            //iterates through the txt file
                            var regulator = 0;

                            while (!streamreader.EndOfStream)
                            {

                                //creates a string variable to read the current line the stream reader has iterated to be on
                                string currentLine = streamreader.ReadLine();
                                //if the current line contains the ID of the currentlyplaying track 
                                if (currentLine.Contains(track1.Id))
                                {
                                    //If the track playing is new and wasnt just playing
                                    if(track1.Id != oldID)
                                    {
                                        regulator = 0;
                                    }

                                        //new track case 
                                        if(regulator == 0)
                                        {
                                            //Reset the information line
                                            presetinfo.Content = "";
                                            //runs the function to edit the preset color line
                                            presinfo(currentLine);

                                            //Write to arduino
                                            port.Open();
                                            port.Write(currentLine);
                                            port.Close();
                                            //resets the regulator variable so this will only run once
                                            regulator = 1;
                                            
                                            
                                            Info.Content = "this song has a preset";

                                            //experimental light changing for tempo, may not be in final build
                                            var trackfeat =  spotifyclient.Tracks.GetAudioFeatures(track1.Id).Result;
                                            beat.Interval = new TimeSpan(0, 0, 0, 0, (int)(60000 / trackfeat.Tempo));


                                        }

                                    //updates the oldID variable to the current song
                                    oldID = track1.Id;
                                }

                                
                            }
                            //if no saved light preset was saved, the user will be notified
                            if(regulator == 0)
                            {
                                Info.Content = "no light settings for this song";
                                presetinfo.Content = "";

                            }
                            streamreader.Close();
                        }
                    }
                }
            


            }
        }

        //if the 
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var current = new PlayerCurrentlyPlayingRequest(PlayerCurrentlyPlayingRequest.AdditionalTypes.Track);
            if (current != null)
            {
                var currentlyplaying = await spotifyclient.Player.GetCurrentlyPlaying(current);

                if (currentlyplaying != null)
                {

                    var currently = currentlyplaying.Item;


                    if (currently is FullTrack track1)
                    {
                        var streamreader = new StreamReader("C:\\Text Samples\\Sample.txt");

                        while (!streamreader.EndOfStream)
                        {
                            string currentLine = streamreader.ReadLine();
                            if (currentLine.Contains(track1.Id) && !overwritebox.IsChecked == true)
                            {
                                 overwritten = true;
                            }
                            
                        }
                        streamreader.Close();
                        if(overwritten != true)
                        {
                            var streamwriter = new StreamWriter("C:\\Text Samples\\Sample.txt", true);
                            streamwriter.WriteLine(track1.Id + " r" + RedBox.IsChecked + "o" + OrangeBox.IsChecked + "y" + YellowBox.IsChecked +
                               "g" + GreenBox.IsChecked + "b" + BlueBox.IsChecked + "s" + SkyBox.IsChecked + "p" + PurpleBox.IsChecked + "w" + WhiteBox.IsChecked);
                            streamwriter.Close();
                            overwritten = false;
                        }
                        else
                        {
                            Info.Content = "Preset already exists";
                            overwritten = false;
                        }

                    }
                }
            }
           // sw.WriteLine()
        }
        private void presinfo(string line)
        {
            if(presetinfo.Content == "")
            {
                if (line.Contains("rTrue"))
                {
                    presetinfo.Content += " Red";
                    colors[0] = true;
                }
                else
                {
                    colors[0] = false;
                }
                if (line.Contains("oTrue"))
                {
                    presetinfo.Content += " Orange";
                    colors[1] = true;
                }
                else
                {
                    colors[1] = false;
                }
                if (line.Contains("yTrue"))
                {
                    presetinfo.Content += " Yellow";
                    colors[2] = true;

                }
                else
                {
                    colors[2] = false;
                }
                if (line.Contains("gTrue"))
                {
                    presetinfo.Content += " Green";
                    colors[3] = true;

                }
                else
                {
                    colors[3] = false;
                }
                if (line.Contains("bTrue"))
                {
                    presetinfo.Content += " Blue";
                    colors[4] = true;

                }
                else
                {
                    colors[4] = false;
                }
                if (line.Contains("pTrue"))
                {
                    presetinfo.Content += " Purple";
                    colors[5] = true;

                }
                else
                {
                    colors[5] = false;
                }
                if (line.Contains("wTrue"))
                {
                    presetinfo.Content += " White";
                    colors[6] = true;

                }
                else
                {
                    colors[6] = false;
                }
                if (line.Contains("sTrue"))
                {
                    presetinfo.Content += " SkyBlue";
                    colors[7] = true;

                }
                else
                {
                    colors[7] = false;
                }
            }
           
        }


    }
}
