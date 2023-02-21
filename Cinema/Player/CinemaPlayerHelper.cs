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
using System.Threading.Tasks;
using Cinema.Settings;
using MediaPortal.Common;
using MediaPortal.Common.MediaManagement;
using MediaPortal.Common.MediaManagement.DefaultItemAspects;
using MediaPortal.Common.ResourceAccess;
using MediaPortal.Common.Services.ResourceAccess.RawUrlResourceProvider;
using MediaPortal.Common.SystemResolver;
using MediaPortal.UiComponents.Media.Models;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Cinema.Player
{
  internal class CinemaPlayerHelper
  {
    public const string CINEMA_MIMETYPE = "cinema/stream";

    public static void PlayStream(Trailer trailer)
    {
      var mediaItem = CreateStreamMediaItem(trailer);
      PlayItemsModel.PlayItem(mediaItem);
    }

    /// <summary>
    /// Constructs a dynamic <see cref="MediaItem"/> that contains the URL for the given <paramref name="trailer"/>.
    /// </summary>
    /// <param name="trailer">Trailer.</param>
    private static MediaItem CreateStreamMediaItem(Trailer trailer)
    {
      IDictionary<Guid, IList<MediaItemAspect>> aspects = new Dictionary<Guid, IList<MediaItemAspect>>();

      MultipleMediaItemAspect providerResourceAspect = MediaItemAspect.CreateAspect(aspects, ProviderResourceAspect.Metadata);
      SingleMediaItemAspect mediaAspect = MediaItemAspect.GetOrCreateAspect(aspects, MediaAspect.Metadata);
      SingleMediaItemAspect audioAspect = MediaItemAspect.GetOrCreateAspect(aspects, VideoAspect.Metadata);

      providerResourceAspect.SetAttribute(ProviderResourceAspect.ATTR_TYPE, ProviderResourceAspect.TYPE_PRIMARY);
      var trailerUrl = TryGetDirectVideoUrl(trailer.Url).Result;
      providerResourceAspect.SetAttribute(ProviderResourceAspect.ATTR_RESOURCE_ACCESSOR_PATH, RawUrlResourceProvider.ToProviderResourcePath(trailerUrl).Serialize());
      providerResourceAspect.SetAttribute(ProviderResourceAspect.ATTR_SYSTEM_ID, ServiceRegistration.Get<ISystemResolver>().LocalSystemId);
      providerResourceAspect.SetAttribute(ProviderResourceAspect.ATTR_MIME_TYPE, CINEMA_MIMETYPE);

      mediaAspect.SetAttribute(MediaAspect.ATTR_TITLE, trailer.Title);

      var mediaItem = new MediaItem(Guid.Empty, aspects);
      return mediaItem;

    }

    private static async Task<string> TryGetDirectVideoUrl(string trailerUrl)
    {
      var youtube = new YoutubeClient();

      var streamManifest = await youtube.Videos.Streams.GetManifestAsync(trailerUrl);
      var streamInfo = streamManifest.GetMuxedStreams().TryGetWithHighestVideoQuality();

      return streamInfo?.Url;
    }
  }
}
