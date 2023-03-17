/*
 * LazerEyes MainPage.xaml.cs
 * Logic associated with MainPage.xaml
 * Course- IST440
 * Author- Cameron Grande
 * Date Developed- 9/26/22
 * Last Changed- 11/02/22
 * Rev: 4
 */

namespace Lazer_Eyes;

using Lazer_Eyes.Resources.Languages;
using Plugin.Maui.Audio;
using System.Diagnostics.Metrics;
using System.Numerics;

public partial class MainPage : ContentPage
{
    //define variables
    public Boolean IsStarted = true;
    public Settings SettingsObj;
    private readonly IAudioManager _audioManager;
    private LidarUtils.Obstacle lastObstacleDetected;
    private const string GREEN_VALUE = "D9FFDA";
    private const string YELLOW_VALUE = "FFFFD5";
    private const string RED_VALUE = "FFAEA8";
    private const string NO_OBJECT = "NO_OBJECT";
    public State CurrentState = State.SCAN;
    List<Image> greenCircles;
    List<Image> yellowTriangles;
    List<Image> redOctagons;
    List<List<Image>> shapes;
    IAudioPlayer player;
    public SpeechOptions TextToSpeachOptions;
    private int _buffSize = 7;
    private int _currBuffSize = 0;
    private int noObjectCount = 10;
    private int currNoObjCount = 0;
    private LidarUtils.Obstacle[] buff;
    

    public enum State
    {
        GO = 1,
        SLOW = 2,
        STOP = 3,
        SCAN = 4
    }


    /*
    * Constructor- init page and start main loop
    */
    public MainPage(IAudioManager audioManager)
    {
        buff = new LidarUtils.Obstacle[_buffSize];
        SettingsObj = Settings.Get();
        TextToSpeachOptions = new SpeechOptions()
        {
            Volume = (float)SettingsObj.GetVolume()
        };
        InitializeComponent();
        this._audioManager = audioManager;
        InitializeAudioPlayer();

        

        greenCircles = new List<Image> { greenCircle1, greenCircle2, greenCircle3 };
        yellowTriangles = new List<Image> { yellowTriangle1, yellowTriangle2, yellowTriangle3 };
        redOctagons = new List<Image> { redOctagon1, redOctagon2, redOctagon3 };
        shapes = new List<List<Image>> { greenCircles, yellowTriangles, redOctagons };

        AnimateMainPageAsync();

        try
        {
            LidarUtils.GetAlerts();
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
        }
    }

    /*
     *  Will start visuals and processing of LIDAR data
     */
    private async Task AnimateMainPageAsync()
    {

        InitShapes(shapes);

        StartShape(greenCircles);

        UIMainIOLoop();
    }

    /*
     *  UIMainIOLoop- will continually check polled enviroment data and change state.
     */
    private async Task UIMainIOLoop()
    {

        String _scanMessage = string.Empty;
        String _distanceMessage = string.Empty;
        
        while (IsStarted)
        {
            await Task.Delay(20);
            if (LidarUtils.IsLightingPoor)
            {
                lightQualityText.Text = ApplicationResource.PoorLight;
            }
            else
            {
                lightQualityText.Text = ApplicationResource.GoodLight;

            }
            LidarUtils.Obstacle currentObstacle = LidarUtils.CurrentObstacle;
            //System.Diagnostics.Debug.WriteLine($"current obstacle: {currentObstacle.ObstacleName}");

            if (currentObstacle.ObstacleName == null)
            {
                //continue;
                currentObstacle = new LidarUtils.Obstacle
                {
                    ObstacleName = NO_OBJECT,
                    Distance = 0,
                };
            }

            buff[_currBuffSize++] = currentObstacle;
            //System.Diagnostics.Debug.WriteLine(currentObstacle.ObstacleName);

            if (_currBuffSize == _buffSize)
            {
                var obstacleName = buff.GroupBy(o => o.ObstacleName).Select(g => new { Value = g.Key, Count = g.Count() }).OrderByDescending(e => e.Count).Take(1).Single().Value;
                var obstacleDist = buff.Where(o => o.ObstacleName.Equals(obstacleName)).Take(1).Single().Distance;

                currentObstacle = new LidarUtils.Obstacle
                {
                    ObstacleName = obstacleName,
                    Distance = obstacleDist,
                };
                System.Diagnostics.Debug.WriteLine($"EMPTYING BUFFER: {obstacleName} {obstacleDist}");

                //if (lastObstacleDetected.ObstacleName != null && !currentObstacle.ObstacleName.Equals(lastObstacleDetected.ObstacleName) && Math.Abs(currentObstacle.Distance.Value - lastObstacleDetected.Distance.Value) < 0.1)
                //    currentObstacle = lastObstacleDetected;

                if (currentObstacle.ObstacleName == NO_OBJECT)
                {
                    currNoObjCount++;
                    if (currNoObjCount == noObjectCount)
                    {
                        currNoObjCount = 0;
                    }
                    else
                    {
                        currentObstacle = lastObstacleDetected;
                    }
                }
                else
                {
                    currNoObjCount = 0;
                }


                try
                {
                    if (currentObstacle.ObstacleName != null && !currentObstacle.ObstacleName.Equals(NO_OBJECT))
                    {
                        _scanMessage = $"{currentObstacle.ObstacleName} detected";

                        var distance = currentObstacle.Distance.Value;
                        var units = (Settings.Units)SettingsObj.GetDistanceUnit();

                        if (units.Equals("Feet"))
                        {
                            distance = distance * 3.281;
                        }

                        if (SettingsObj.GetMeasureDistanceInStrides())
                        {
                            var strideLength = SettingsObj.GetStrideLength();
                            distance = distance / strideLength;
                            units = Settings.Units.Steps;
                        }

                        _distanceMessage = $"{String.Format("{0:0.##}", distance)} {units} away";
                        if (!currentObstacle.ObstacleName.Equals(lastObstacleDetected.ObstacleName))
                        {
                            NotifyUser(currentObstacle.ObstacleName, false);
                        }
                    } else
                    {
                        _scanMessage = "Scanning...";
                        _distanceMessage = "";
                    }
                    lastObstacleDetected = currentObstacle;
                }
                catch (Exception e)
                {
                    //catch excpetion and move on
                    System.Diagnostics.Debug.WriteLine($"Error: {e.Message}");

                }

                statusText.Text = _scanMessage;
                distanceText.Text = _distanceMessage;

                try
                {
                    //if go detected from different state
                    if (CurrentState != State.GO && currentObstacle.Distance.Value > SettingsObj.GetDistanceThreshold() * 1.25)
                    {
                        StopAllShapes(shapes);
                        StartShape(greenCircles);
                        CurrentState = State.GO;
                        MainPageGrid.BackgroundColor = Color.FromHex(GREEN_VALUE);
                        NotifyUser("Path is clear", true);
                    }

                    //if slow detected from different state
                    if (CurrentState != State.SLOW && currentObstacle.Distance.Value > SettingsObj.GetDistanceThreshold() / 2
                                            && currentObstacle.Distance.Value < SettingsObj.GetDistanceThreshold() * 1.25)
                    {
                        StopAllShapes(shapes);
                        StartShape(yellowTriangles);
                        CurrentState = State.SLOW;
                        MainPageGrid.BackgroundColor = Color.FromHex(YELLOW_VALUE);
                        NotifyUser("Slow down", true);
                    }

                    //if stop detected from different state
                    if (CurrentState != State.STOP && currentObstacle.Distance.Value < SettingsObj.GetDistanceThreshold() / 2 && currentObstacle.Distance.Value > 0)
                    {
                        StopAllShapes(shapes);
                        StartShape(redOctagons);
                        CurrentState = State.STOP;
                        MainPageGrid.BackgroundColor = Color.FromHex(RED_VALUE);
                        NotifyUser("Stop", true);
                    }

                    if (CurrentState != State.SCAN && currentObstacle.ObstacleName.Equals(NO_OBJECT))
                    {
                        StopAllShapes(shapes);
                        StartShape(greenCircles);
                        CurrentState = State.SCAN;
                        MainPageGrid.BackgroundColor = Color.FromHex(GREEN_VALUE);
                        NotifyUser("Scanning", true);
                        currNoObjCount = 0;
                    }
                }
                catch (Exception e)
                {
                    //catch exception and move on
                    System.Diagnostics.Debug.WriteLine($"Error2: {e.Message}");

                }
                _currBuffSize = 0;
            }
        }

        System.Diagnostics.Debug.WriteLine($"END OF LOOP");

    }
    
    /*
     * Helper and init methods go below
     */

    /*
     * InitializeAudioPlayer- prepares app for beeping sound to be performed 
     */
    private async Task InitializeAudioPlayer()
    {
        player = _audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("beep1.wav"));
    }

    /*
     * The following methods allow us to launch, display, and hide visuals in a dynamic way.
     */
    private async Task InitShapes(List<List<Image>> shapes)
    {
        foreach (List<Image> shape in shapes)
        {
            InitShape(shape);
        }
    }
    private async Task InitShape(List<Image> shape)
    {
        Image im1 = shape[0];
        Image im2 = shape[1];
        Image im3 = shape[2];


        AnimateShape(im1, im2, im3);
    }

    private async Task StartShape(List<Image> shape)
    {
        foreach (Image image in shape)
        {
            image.IsVisible = true;
        }
    }
    private async Task StopShape(List<Image> shape)
    {
        foreach (Image image in shape)
        {
            image.IsVisible = false;
        }
    }
    private async Task StopAllShapes(List<List<Image>> shapes)
    {
        foreach (List<Image> shape in shapes)
        {
            StopShape(shape);
        }
    }

    private void NotifyUser(string message, bool playSound)
    {
        if (SettingsObj.GetAuditoryDefault())
        {
            Announce(message);
            if (playSound)
                PlaySound();
            
        }
        if (SettingsObj.GetTactileSettingsDefault())
        {
            if (Vibration.Default.IsSupported)
                Vibration.Default.Vibrate(TimeSpan.FromSeconds(1));
        }
    }

    private void PlaySound()
    {
        player.Volume = SettingsObj.GetVolume();
        player.Play();
    }

    private async void Announce(string message)
    {
        TextToSpeachOptions.Volume = (float)SettingsObj.GetVolume();
        await TextToSpeech.Default.SpeakAsync(message, TextToSpeachOptions);
    }

    /*
    * Help- loads in the help/setting page
    * @param sender- object that triggered action
    * @param eventArgs- arguments from action
    * @returns none
    */
    private void Help(object sender, EventArgs e)
    {
        Navigation.PushAsync(new SettingsMenu());
    }

    private async Task AnimateShape(Image im1, Image im2, Image im3)
    {
        im1.Opacity = .175;
        im1.Scale = 5;

        im2.Opacity = .525;
        im2.Scale = 3;

        im3.Opacity = .875;
        im3.Scale = 1;
        while (IsStarted)
        {
            await Task.WhenAll
                (
                im1.FadeTo(0, 1000),
                im1.ScaleTo(6, 1000),

                im2.FadeTo(.175, 1000),
                im2.ScaleTo(5, 1000),

                im3.FadeTo(0.525, 1000),
                im3.ScaleTo(3, 1000)
                );

            im1.Opacity = 1;
            im1.Scale = 0;
            await Task.WhenAll
                (
                im1.FadeTo(.875, 1000),
                im1.ScaleTo(1, 1000),

                im2.FadeTo(0, 1000),
                im2.ScaleTo(6, 1000),

                im3.FadeTo(0.175, 1000),
                im3.ScaleTo(5, 1000)
                );

            im2.Opacity = 1;
            im2.Scale = 0;
            await Task.WhenAll
                (
                im1.FadeTo(.525, 1000),
                im1.ScaleTo(3, 1000),

                im2.FadeTo(.875, 1000),
                im2.ScaleTo(1, 1000),

                im3.FadeTo(0, 1000),
                im3.ScaleTo(6, 1000)
                );

            im3.Opacity = 1;
            im3.Scale = 0;
            await Task.WhenAll
                (
                im1.FadeTo(.175, 1000),
                im1.ScaleTo(5, 1000),

                im2.FadeTo(.525, 1000),
                im2.ScaleTo(3, 1000),

                im3.FadeTo(.875, 1000),
                im3.ScaleTo(1, 1000)
                );
        }

        return;
    }

}