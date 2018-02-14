﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using YTApp.Classes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace YTApp.UserControls
{
    public sealed partial class LikeDislikeUserControl : UserControl
    {
        string VideoID;
        string CurrentSelection = "none";
        long LikeCountStr;
        long DislikeCountStr;

        public LikeDislikeUserControl(string VideoId)
        {
            this.InitializeComponent();
            VideoID = VideoId;
            UpdateData();
        }

        public async void UpdateData()
        {
            var service = await YoutubeItemMethodsStatic.GetServiceAsync();

            //Set like and dislike counts
            try
            {
                var videoStatsRequest = service.Videos.List("statistics");
                videoStatsRequest.Id = VideoID;
                var videoStats = await videoStatsRequest.ExecuteAsync();
                LikeCountStr = Convert.ToInt64(videoStats.Items[0].Statistics.LikeCount);
                DislikeCountStr = Convert.ToInt64(videoStats.Items[0].Statistics.DislikeCount);

                LikeCount.Text = Classes.YoutubeItemMethodsStatic.ViewCountShortner(LikeCountStr);
                DislikeCount.Text = Classes.YoutubeItemMethodsStatic.ViewCountShortner(DislikeCountStr);

                LikesBar.Value = LikeCountStr * 100 / (LikeCountStr + DislikeCountStr + 1);

                //Find and set the rating if it already exists
                var videoRequest = service.Videos.GetRating(VideoID);
                var video = await videoRequest.ExecuteAsync();

                if (video.Items[0].Rating == "like")
                {
                    LikeIcon.Fill = Application.Current.Resources["SystemControlHighlightAccentBrush"] as SolidColorBrush;
                    CurrentSelection = "like";
                    LikeCount.Text = Classes.YoutubeItemMethodsStatic.ViewCountShortner(LikeCountStr + 1);
                }
                else if (video.Items[0].Rating == "dislike")
                {
                    DislikeIcon.Fill = Application.Current.Resources["SystemControlHighlightAccentBrush"] as SolidColorBrush;
                    CurrentSelection = "dislike";
                    DislikeCount.Text = Classes.YoutubeItemMethodsStatic.ViewCountShortner(DislikeCountStr + 1);
                }
            }
            catch { }
        }

        private async void DislikeIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var service = await YoutubeItemMethodsStatic.GetServiceAsync();

            var DislikeVideo = service.Videos.Rate(VideoID, VideosResource.RateRequest.RatingEnum.Dislike);
            await DislikeVideo.ExecuteAsync();

            CurrentSelection = "dislike";

            LikeIcon.Fill = new SolidColorBrush(Color.FromArgb(255, 136, 136, 136));
            DislikeIcon.Fill = Application.Current.Resources["SystemControlHighlightAccentBrush"] as SolidColorBrush;

            LikeCount.Text = Classes.YoutubeItemMethodsStatic.ViewCountShortner(LikeCountStr);
            DislikeCount.Text = Classes.YoutubeItemMethodsStatic.ViewCountShortner(DislikeCountStr + 1);
        }

        private async void LikeIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var service = await YoutubeItemMethodsStatic.GetServiceAsync();

            var LikeVideo = service.Videos.Rate(VideoID, VideosResource.RateRequest.RatingEnum.Like);
            await LikeVideo.ExecuteAsync();

            CurrentSelection = "like";

            LikeIcon.Fill = Application.Current.Resources["SystemControlHighlightAccentBrush"] as SolidColorBrush;
            DislikeIcon.Fill = new SolidColorBrush(Color.FromArgb(255, 136, 136, 136));

            LikeCount.Text = Classes.YoutubeItemMethodsStatic.ViewCountShortner(LikeCountStr + 1);
            DislikeCount.Text = Classes.YoutubeItemMethodsStatic.ViewCountShortner(DislikeCountStr);
        }

        private void DislikeIcon_Entered(object sender, PointerRoutedEventArgs e)
        {
            DislikeIcon.Fill = new SolidColorBrush(Color.FromArgb(255, 160, 160, 160));

        }

        private void LikeIcon_Entered(object sender, PointerRoutedEventArgs e)
        {
            LikeIcon.Fill = new SolidColorBrush(Color.FromArgb(255, 160, 160, 160));
        }

        private void DislikeIcon_Exited(object sender, PointerRoutedEventArgs e)
        {
            if (CurrentSelection == "dislike") { DislikeIcon.Fill = Application.Current.Resources["SystemControlHighlightAccentBrush"] as SolidColorBrush; }
            else { DislikeIcon.Fill = new SolidColorBrush(Color.FromArgb(255, 136, 136, 136)); }
        }

        private void LikeIcon_Exited(object sender, PointerRoutedEventArgs e)
        {
            if (CurrentSelection == "like") { LikeIcon.Fill = Application.Current.Resources["SystemControlHighlightAccentBrush"] as SolidColorBrush; }
            else { LikeIcon.Fill = new SolidColorBrush(Color.FromArgb(255, 136, 136, 136)); }
        }
    }
}
