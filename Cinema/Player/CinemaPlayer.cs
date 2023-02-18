#region Copyright (C) 2007-2014 Team MediaPortal

/*
    Copyright (C) 2007-2014 Team MediaPortal
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
using DirectShow;
using DirectShow.Helper;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Common.ResourceAccess;
using MediaPortal.UI.Players.Video;
using MediaPortal.UI.Players.Video.Tools;
using MediaPortal.UI.Presentation.Players;

namespace Cinema.Player
{
  public class CinemaPlayer : VideoPlayer, IUIContributorPlayer
  {
    public const string CINEMA_MIMETYPE = "cinema/stream";
    private const string SOURCE_FILTER_NAME = "LAV Splitter Source";

    protected void AddFileSource()
    {
      IBaseFilter sourceFilter = null;
      try
      {
        sourceFilter = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, SOURCE_FILTER_NAME);
        FilterGraphTools.RenderOutputPins(_graphBuilder, sourceFilter);
      }
      finally
      {
        FilterGraphTools.TryRelease(ref sourceFilter);
      }
    }

    protected override void AddSourceFilter()
    {
      Guid CLSID_LAV_SPLITTER = new Guid("{B98D13E7-55DB-4385-A33D-09FD1BA26338}");
      var networkResourceAccessor = _resourceAccessor as INetworkResourceAccessor;
      if (networkResourceAccessor != null)
      {
        ServiceRegistration.Get<ILogger>().Debug("{0}: Initializing for network media item '{1}'", PlayerTitle, networkResourceAccessor.URL);

        var sourceFilter = FilterGraphTools.AddFilterFromClsid(_graphBuilder, CLSID_LAV_SPLITTER, "LAV Splitter Source");
        if (sourceFilter != null)
        {
          var fileSourceFilter = ((IFileSourceFilter)sourceFilter);
          var hr = (HRESULT)fileSourceFilter.Load(networkResourceAccessor.URL, null);

          new HRESULT(hr).Throw();

          using (DSFilter source2 = new DSFilter(sourceFilter))
            foreach (DSPin pin in source2.Output)
              hr = pin.Render(); // Some pins might fail rendering (i.e. subtitles), but the graph can be still playable
        }
        return;
      }
      base.AddSourceFilter();
    }

    public Type UIContributorType
    {
      get { return typeof(CinemaUiContributor); }
    }
  }
}
