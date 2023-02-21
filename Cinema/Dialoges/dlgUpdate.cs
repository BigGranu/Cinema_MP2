﻿#region Copyright (C) 2007-2023 Team MediaPortal

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
using System.Threading;
using Cinema.Settings;
using MediaPortal.Common;
using MediaPortal.Common.General;
using MediaPortal.Common.Settings;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Workflow;
using Movies = Cinema.Settings.Movies;

namespace Cinema.Dialoges
{
  public class DlgUpdate : IWorkflowModel
  {
    #region Consts

    public const string MODEL_ID_STR = "B20F2D04-4E26-45A2-9714-E77E03191754";

    #endregion

    #region Propertys

    private static AbstractProperty _updateProgressProperty = new WProperty(typeof(int), 0);

    public AbstractProperty UpdateProgressProperty
    {
      get { return _updateProgressProperty; }
    }

    public static int UpdateProgress
    {
      get { return (int)_updateProgressProperty.GetValue(); }
      set { _updateProgressProperty.SetValue(value); }
    }

    private static AbstractProperty _infoProperty = new WProperty(typeof(string), string.Empty);

    public AbstractProperty InfoProperty
    {
      get { return _infoProperty; }
    }

    public static string Info
    {
      get { return (string)_infoProperty.GetValue(); }
      set { _infoProperty.SetValue(value); }
    }

    #endregion

    private static readonly ISettingsManager SETTINGS_MANAGER = ServiceRegistration.Get<ISettingsManager>();
    private static Locations _locations = new Locations();
    private static CinemaSettings _settings = new CinemaSettings();
    private static Movies _movies = new Movies();
    private static bool _updateIsRunning = false;

    public static void MakeUpdate(bool extThread)
    {
      if (_updateIsRunning)
        return;

      if (extThread)
      {     
        Update();
      }
      else
      {
        Thread newThread = new Thread(Update);
        newThread.Start();
      }
    }

    private static void Update()
    {
      _updateIsRunning = true;

      //_settings = SETTINGS_MANAGER.Load<CinemaSettings>();
      //_locations = SETTINGS_MANAGER.Load<Locations>();

      //GoogleMovies.GoogleMovies.Data = new CinemaDataList { List = new List<CinemaData>() };

      //if (_locations.LocationSetupList != null)
      //{
      //  var cl = _locations.LocationSetupList;
      //  int percent = 100 / (cl.Count);

      //  foreach (var c in cl)
      //  {
      //    Info = c.Name;
      //    GoogleMovies.GoogleMovies.Data.List.Add(GoogleMovies.GoogleMovies.GetCinemaData(c));
      //    UpdateProgress += percent;
      //  }

      //  UpdateProgress = 0;

      //  GrappOtherInfos();
      //}

      //_settings.LastUpdate = DateTime.Today;
      //ServiceRegistration.Get<ISettingsManager>().Save(_settings);

      //_locations.Changed = false;
      //ServiceRegistration.Get<ISettingsManager>().Save(_locations);

      //var datalist = new Datalist { CinemaDataList = GoogleMovies.GoogleMovies.Data };
      //ServiceRegistration.Get<ISettingsManager>().Save(datalist);

      _updateIsRunning = false;
    }

    public static void GrappOtherInfos()
    {
      //var ml = new List<Movie>();
      //var l = new List<string>();
      //_movies = new Movies();

      //foreach (var mo in from cd in GoogleMovies.GoogleMovies.Data.List from m in cd.MoviesOnDayList from mo in m.Movielist.Where(mo => !l.Contains(mo.Title)) select mo)
      //{
      //  l.Add(mo.Title);
      //  ml.Add(mo);
      //}

      //int percent = 100 / (ml.Count);

      //foreach (Movie m in ml)
      //{
      //  Info = m.Title;

      //  var gm = new GrappedMovie();
      //  var imdb = MovieInfo.GetImdbID(m.Title);
      //  gm.ImdbID = imdb;
      //  gm.Title = m.Title;

      //  var mi = new MovieInfo(imdb);

      //  gm.Poster = mi.Poster;
      //  gm.Picture = mi.Picture;
      //  gm.Description = mi.Description;
      //  gm.Year = mi.Year;
      //  gm.AgeLimit = mi.AgeLimit;
      //  gm.Genre = mi.Genre;
      //  gm.Trailer = mi.Trailer;

      //  _movies.MovieList.Add(gm);
      //  UpdateProgress += percent;
      //}

      //ServiceRegistration.Get<ISettingsManager>().Save(_movies);
      //UpdateProgress = 0;
    }

    #region IWorkflowModel implementation

    public Guid ModelId
    {
      get { return new Guid(MODEL_ID_STR); }
    }

    public bool CanEnterState(NavigationContext oldContext, NavigationContext newContext)
    {
      return true;
    }

    public void EnterModelContext(NavigationContext oldContext, NavigationContext newContext)
    {
     // Update();
    }

    public void ExitModelContext(NavigationContext oldContext, NavigationContext newContext)
    {
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
  }
}
