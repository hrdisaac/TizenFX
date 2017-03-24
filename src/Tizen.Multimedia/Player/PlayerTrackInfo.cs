/*
 * Copyright (c) 2016 Samsung Electronics Co., Ltd All Rights Reserved
 *
 * Licensed under the Apache License, Version 2.0 (the License);
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an AS IS BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Tizen.Multimedia
{
    /// <summary>
    /// Provides means to retrieve track information.
    /// </summary>
    /// <seealso cref="Player.SubtitleTrackInfo"/>
    /// <seealso cref="Player.AudioTrackInfo"/>
    public class PlayerTrackInfo
    {
        private readonly int _streamType;
        private readonly Player _owner;

        internal PlayerTrackInfo(Player player, StreamType streamType)
        {
            Debug.Assert(player != null);

            Log.Debug(PlayerLog.Tag, "streamType : " + streamType);
            _streamType = (int)streamType;
            _owner = player;
        }

        /// <summary>
        /// Gets the number of tracks.
        /// </summary>
        /// <returns>The number of tracks.</returns>
        /// <remarks>The <see cref="Player"/> that owns this instance must be in the <see cref="PlayerState.Ready"/>, <see cref="PlayerState.Playing"/> or <see cref="PlayerState.Paused"/> state.</remarks>
        /// <exception cref="ObjectDisposedException">The <see cref="Player"/> that this instance belongs to has been disposed.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="Player"/> that this instance belongs to is not in the valid state.</exception>
        public int GetCount()
        {
            _owner.ValidatePlayerState(PlayerState.Ready, PlayerState.Playing, PlayerState.Paused);

            int count = 0;
            Interop.Player.GetTrackCount(_owner.Handle, _streamType, out count).
                ThrowIfFailed("Failed to get count of the track");
            Log.Info(PlayerLog.Tag, "get count : " + count);
            return count;
        }

        /// <summary>
        /// Gets the language code for the specified index or null if the language is undefined.
        /// </summary>
        /// <returns>The number of tracks.</returns>
        /// <remarks>
        ///     <para>The <see cref="Player"/> that owns this instance must be in the <see cref="PlayerState.Ready"/>, <see cref="PlayerState.Playing"/> or <see cref="PlayerState.Paused"/> state.</para>
        ///     <para>The language codes are defined in ISO 639-1.</para>
        /// </remarks>
        /// <exception cref="ObjectDisposedException">The <see cref="Player"/> that this instance belongs to has been disposed.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="Player"/> that this instance belongs to is not in the valid state.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     index is less than zero.
        ///     <para>-or-</para>
        ///     index is equal to or greater than <see cref="GetCount()"/>
        /// </exception>
        public string GetLanguageCode(int index)
        {
            _owner.ValidatePlayerState(PlayerState.Ready, PlayerState.Playing, PlayerState.Paused);

            if (index < 0 || GetCount() <= index)
            {
                Log.Error(PlayerLog.Tag, "invalid index");
                throw new ArgumentOutOfRangeException(nameof(index), index,
                    $"valid index range is 0 <= x < {nameof(GetCount)}(), but got { index }.");
            }

            IntPtr code = IntPtr.Zero;

            try
            {
                Interop.Player.GetTrackLanguageCode(_owner.Handle, _streamType, index, out code).
                    ThrowIfFailed("Failed to get the selected language of the player");

                string result = Marshal.PtrToStringAnsi(code);

                if (result == "und")
                {
                    Log.Error(PlayerLog.Tag, "not defined code");
                    return null;
                }
                Log.Info(PlayerLog.Tag, "get language code : " + result);
                return result;
            }
            finally
            {
                Interop.Libc.Free(code);
            }
        }

        /// <summary>
        /// Gets the selected track index.
        /// </summary>
        /// <returns>The currently selected track index.</returns>
        /// <remarks>The <see cref="Player"/> that owns this instance must be in the <see cref="PlayerState.Ready"/>, <see cref="PlayerState.Playing"/> or <see cref="PlayerState.Paused"/> state.</remarks>
        /// <exception cref="ObjectDisposedException">The <see cref="Player"/> that this instance belongs to has been disposed.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="Player"/> that this instance belongs to is not in the valid state.</exception>
        public int GetSelected()
        {
            _owner.ValidatePlayerState(PlayerState.Ready, PlayerState.Playing, PlayerState.Paused);

            int value = 0;

            Interop.Player.GetCurrentTrack(_owner.Handle, _streamType, out value).
                ThrowIfFailed("Failed to get the selected index of the player");
            Log.Debug(PlayerLog.Tag, "get selected index : " + value);
            return value;
        }

        /// <summary>
        /// Selects the track.
        /// </summary>
        /// <param name="index">The index to select.</param>
        /// <remarks>The <see cref="Player"/> that owns this instance must be in the <see cref="PlayerState.Ready"/>, <see cref="PlayerState.Playing"/> or <see cref="PlayerState.Paused"/> state.</remarks>
        /// <exception cref="ObjectDisposedException">The <see cref="Player"/> that this instance belongs to has been disposed.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="Player"/> that this instance belongs to is not in the valid state.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     index is less than zero.
        ///     <para>-or-</para>
        ///     index is equal to or greater than <see cref="GetCount()"/>
        /// </exception>
        public void SetSelected(int index)
        {
            if (index < 0 || GetCount() <= index)
            {
                Log.Error(PlayerLog.Tag, "invalid index");
                throw new ArgumentOutOfRangeException(nameof(index), index,
                    $"valid index range is 0 <= x < {nameof(GetCount)}(), but got { index }.");
            }

            _owner.ValidatePlayerState(PlayerState.Ready, PlayerState.Playing, PlayerState.Paused);

            Interop.Player.SelectTrack(_owner.Handle, _streamType, index).
                ThrowIfFailed("Failed to set the selected index of the player");
        }

    }
}
