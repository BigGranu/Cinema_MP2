#region Copyright (C) 2007-2023 Team MediaPortal

/*
    Copyright (C) 2007-2023 Team MediaPortal
    http://www.team-mediaportal.com

    This file is part of MediaPortal 2

    MediaPortal 2 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    MediaPortal 2 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MediaPortal 2. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Timers;
using Cinema.Dialoges;
using Cinema.OnlineLibraries.Data;
using Cinema.Player;
using Cinema.Settings;
using MediaPortal.Common;
using MediaPortal.Common.General;
using MediaPortal.Common.Logging;
using MediaPortal.Common.PluginManager;
using MediaPortal.Common.Settings;
using MediaPortal.Extensions.UserServices.FanArtService.Client.Models;
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Screens;
using MediaPortal.UI.Presentation.Workflow;
using MediaPortal.UI.SkinEngine.Controls.ImageSources;
using MediaPortal.UI.SkinEngine.MpfElements;
using Trailer = Cinema.Settings.Trailer;

namespace Cinema.Models
{
  public class CinemaHome : IWorkflowModel, IPluginStateTracker
  {
    public static Timer ATimer = new Timer();
    public static ItemsList Movies = new ItemsList();

    private static readonly ISettingsManager SETTINGS_MANAGER = ServiceRegistration.Get<ISettingsManager>();
    public ItemsList Cinemas = new ItemsList();

    public static Movies FullMovieList { get; set; }

    #region Consts

    public const string MODEL_ID_STR = "78E0D999-D87A-4340-B8D1-9CF97814D2FD";
    public const string NAME = "name";
    public const string TRAILER = "trailer";

    #endregion

    #region Propertys

    public static readonly AbstractProperty _selectedCinema = new WProperty(typeof(string), string.Empty);

    public AbstractProperty SelectedCinemaProperty => _selectedCinema;

    public static string SelectedCinema
    {
      get => (string)_selectedCinema.GetValue();
      set => _selectedCinema.SetValue(value);
    }

    #endregion

    #region public Methods

    public static void SelectCinema(string id)
    {
      foreach (var cd in FullMovieList.CinemaMovies)
      {
        if (cd.Cinema.Id == id)
        {
          SelectedCinema = cd.Cinema.Name;
          AddMoviesByCinema(cd.Cinema);
        }
      }
    }

    public static void SelectMovie(ListItem item)
    {
      var t = new Trailer { Title = (string)item.AdditionalProperties[NAME], Url = (string)item.AdditionalProperties[TRAILER] };
      if (t.Url != null ) CinemaPlayerHelper.PlayStream(t);
    }

    public static void AddMoviesByCinema(OnlineLibraries.Data.Cinema cinema)
    {
      Movies.Clear();

      List<OnlineLibraries.Data.Movie> ml = new List<Movie>();
      foreach (var cd in FullMovieList.CinemaMovies)
      {
        if (cd.Cinema.Id == cinema.Id)
        {
          ml = cd.Movies;
        }
      }

      foreach (var m in ml)
      {
        var item = new ListItem { AdditionalProperties = { [NAME] = m.Title } };
        item.SetLabel("Name", m.Title);

        try
        {
          for (var i = 0; i <= 3; i++)
          {
            item.SetLabel("Day" + i, m.Showtimes[i].Day);
            item.SetLabel("Day" + i + "_Time", m.Showtimes[i].Showtimes);
          }
        }
        catch (Exception e)
        {
          Console.WriteLine(e);
        }

        item.SetLabel("Title", m.Title);
        item.SetLabel("Poster", m.CoverUrl);
        item.SetLabel("Picture", m.Fanart);
        item.SetLabel("Description", m.Description);
        item.SetLabel("Year", m.Release);
        item.SetLabel("AgeLimit", m.Age);
        item.SetLabel("Genre", m.Genres);
        item.SetLabel("UserRating", m.UserRating);
        item.SetLabel("UserRatingScaled", m.UserRatingScaled);
        if (m.Trailer.Count > 0)
        {
          item.AdditionalProperties[TRAILER] = m.Trailer[0].Url;
        }

        item.SetLabel("Duration", m.Runtime);
        Movies.Add(item);
      }
      Movies.FireChange();
    }

    public static void SetSelectedItem(object sender, SelectionChangedEventArgs e)
    {
      var selectedItem = e.FirstAddedItem as ListItem;
      if (selectedItem != null)
      {
        var fanArtBgModel = (FanArtBackgroundModel)ServiceRegistration.Get<IWorkflowManager>().GetModel(FanArtBackgroundModel.FANART_MODEL_ID);
        if (fanArtBgModel != null)
        {
          var uriSource = selectedItem.Labels["Picture"].ToString();

          if (uriSource != "") fanArtBgModel.ImageSource = new MultiImageSource { UriSource = uriSource };
          else fanArtBgModel.ImageSource = new MultiImageSource { UriSource = null };
        }
      }
    }

    public static void MakeUpdate(bool dialog)
    {
      if (dialog)
        ServiceRegistration.Get<IWorkflowManager>().NavigatePushAsync(new Guid("48FE28A6-868D-4531-BF2F-1E746769B177"));

      DlgUpdate.MakeUpdate(dialog);

      if (dialog)
        ServiceRegistration.Get<IScreenManager>().CloseTopmostDialog();
    }

    #endregion

    #region private Methods

    private static void Init()
    {
      //CkeckUpdate(true);
      //todo Muss wieder durch Update ersetzt werden
      FullMovieList = SETTINGS_MANAGER.Load<Movies>();

      if (FullMovieList != null && FullMovieList.CinemaMovies != null && FullMovieList.CinemaMovies.Count > 0) SelectCinema(FullMovieList.CinemaMovies[0].Cinema.Id);
    }

    private static void CkeckUpdate(bool dialog)
    {
      //var dt1 = Convert.ToDateTime(SETTINGS_MANAGER.Load<Settings.CinemaSettings>().LastUpdate);
      //var dt = DateTime.Now - dt1;
      //// Is it a New Day ?
      //if (dt > new TimeSpan(1, 0, 0, 0))
      //{
      //    MakeUpdate(dialog);
      //}
      //else if (SETTINGS_MANAGER.Load<Locations>().Changed)
      //{
      //    MakeUpdate(dialog);
      //}
      //else
      //{
      //    GoogleMovies.GoogleMovies.Data = SETTINGS_MANAGER.Load<Datalist>().CinemaDataList;
      //}
    }

    //private static string ShowtimesByCinemaMovieDay(Settings.Cinema cinema, Movie movie, int day)
    //{
    //    //var st = GoogleMovies.GoogleMovies.GetShowTimesByCinemaAndMovieAndDay(cinema, movie, day).Aggregate("", (current, s) => current + (s + " | "));
    //    //return GoogleMovies.GoogleMovies.GetNewDay(day) + ": " + st;

    //    return "";
    //}

    private void ClearFanart()
    {
      var fanArtBgModel = (FanArtBackgroundModel)ServiceRegistration.Get<IWorkflowManager>().GetModel(FanArtBackgroundModel.FANART_MODEL_ID);
      if (fanArtBgModel != null) fanArtBgModel.ImageSource = new MultiImageSource { UriSource = null };
    }

    private static void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
      ServiceRegistration.Get<ILogger>().Info("Cinema Timer Thread Check Update");
      CkeckUpdate(false);
    }

    #endregion

    #region IWorkflowModel implementation

    public Guid ModelId => new Guid(MODEL_ID_STR);

    public bool CanEnterState(NavigationContext oldContext, NavigationContext newContext)
    {
      return true;
    }

    public void EnterModelContext(NavigationContext oldContext, NavigationContext newContext)
    {
      Init();
    }

    public void ExitModelContext(NavigationContext oldContext, NavigationContext newContext)
    {
      ClearFanart();
    }

    public void ChangeModelContext(NavigationContext oldContext, NavigationContext newContext, bool push)
    {
      // We could initialize some data here when changing the media navigation state
    }

    public void Deactivate(NavigationContext oldContext, NavigationContext newContext)
    {
    }

    public void Reactivate(NavigationContext oldContext, NavigationContext newContext)
    {
      // Todo: select any or the Last ListItem
    }

    public void UpdateMenuActions(NavigationContext context, IDictionary<Guid, WorkflowAction> actions)
    {
    }

    public ScreenUpdateMode UpdateScreen(NavigationContext context, ref string screen)
    {
      return ScreenUpdateMode.AutoWorkflowManager;
    }

    #endregion

    # region IPluginStateTracker implementation

    public void Activated(PluginRuntime pluginRuntime)
    {
      // Add Timer with event
      //ATimer.Elapsed += OnTimedEvent;
      //// Diff in sec To next Day 01.00.00
      //ATimer.Interval = DateTime.Today.AddHours(25).Subtract(DateTime.Now).TotalMilliseconds;
      //// Timer start
      //ATimer.Start();

      //// Make Update
      //CkeckUpdate(false);
    }

    public bool RequestEnd()
    {
      return true;
    }

    public void Stop()
    {
    }

    public void Continue()
    {
    }

    public void Shutdown()
    {
    }

    #endregion
  }
}
