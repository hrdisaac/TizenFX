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
using ElmSharp;

namespace Tizen.Multimedia
{
    //TODO reimplementation needed
    /// <summary>
    /// Provides means to configure display settings for video <see cref="Player"/>.
    /// </summary>
    public class PlayerDisplay
    {
        private PlayerDisplay(PlayerDisplayType type, EvasObject target)
        {
            if (target == null)
            {
                Log.Error(PlayerLog.Tag, "evas object is null");
                throw new ArgumentNullException(nameof(target));
            }

            if (target == IntPtr.Zero)
            {
                Log.Error(PlayerLog.Tag, "The evas object is not realized.");
                throw new ArgumentException("The evas object is not realized.");
            }

            Type = type;
            EvasObject = target;
        }

        public PlayerDisplay(Window window) : this(PlayerDisplayType.Overlay, window)
        {
        }

        public PlayerDisplay(Image image) : this(PlayerDisplayType.Surface, image)
        {
        }

        public EvasObject EvasObject { get; }

        internal PlayerDisplayType Type { get; }

        /// <summary>
        /// Gets the player that the display is assigned to.
        /// </summary>
        public Player Player
        {
            get;
            internal set;
        }

        private void ValidatePlayer()
        {
            if (Player == null)
            {
                Log.Error(PlayerLog.Tag, "The display is not assigned, yet.");
                throw new InvalidOperationException("The display is not assigned, yet.");
            }

            Player.ValidateNotDisposed();
        }

        private PlayerDisplayMode _displayMode = PlayerDisplayMode.LetterBox;

        /// <summary>
        /// Set/Get Display mode.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The display is not assigned.
        /// <para>-or-</para>
        /// Operation failed; internal error.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The player already has been disposed of.</exception>
        /// <exception cref="ArgumentException">The specified value to set is invalid.</exception>
        public PlayerDisplayMode Mode
        {
            get
            {
                return _displayMode;
            }
            set
            {
                ValidatePlayer();

                if (_displayMode == value)
                {
                    return;
                }

                ValidationUtil.ValidateEnum(typeof(PlayerDisplayMode), value);

                Interop.Player.SetDisplayMode(Player.Handle, value).
                    ThrowIfFailed("Failed to set display mode");

                _displayMode = value;
            }
        }

        private bool _isVisible = true;

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        /// <exception cref="InvalidOperationException">
        /// The display is not assigned.
        /// <para>-or-</para>
        /// Operation failed; internal error.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The player already has been disposed of.</exception>
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                ValidatePlayer();

                if (_isVisible == value)
                {
                    return;
                }

                Interop.Player.SetDisplayVisible(Player.Handle, value).
                    ThrowIfFailed("Failed to set the visible state of the display");

                _isVisible = value;
            }
        }

        private PlayerDisplayRotation _rotation = PlayerDisplayRotation.RotationNone;

        /// <summary>
        /// Set/Get Display rotation.
        /// </summary>
        /// <value> RotationNone, Rotation90, Rotation180, Rotation270 </value>
        /// <exception cref="InvalidOperationException">
        /// The display is not assigned.
        /// <para>-or-</para>
        /// Operation failed; internal error.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The player already has been disposed of.</exception>
        /// <exception cref="ArgumentException">The specified value to set is invalid.</exception>
        public PlayerDisplayRotation Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                ValidatePlayer();

                if (_rotation == value)
                {
                    return;
                }

                ValidationUtil.ValidateEnum(typeof(PlayerDisplayRotation), value);

                Interop.Player.SetDisplayRotation(Player.Handle, value).
                    ThrowIfFailed("Failed to set the rotation state of the display");

                _rotation = value;
            }
        }

        /// <summary>
        /// Sets the roi(region of interest).
        /// </summary>
        /// <remarks>
        /// To set roi, <see cref="Mode"/> must be set to <see cref="PlayerDisplayMode.Roi"/> first.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// The display is not assigned.
        /// <para>-or-</para>
        /// Operation failed; internal error.
        /// <para>-or-</para>
        /// <see cref="Mode"/> is not set to <see cref="PlayerDisplayMode.Roi"/>
        /// </exception>
        /// <exception cref="ObjectDisposedException">The player already has been disposed of.</exception>
        /// <exception cref="ArgumentOutOfRangeException">width or height is less than or equal to zero.</exception>
        public void SetRoi(Rectangle roi)
        {
            ValidatePlayer();

            if (_displayMode != PlayerDisplayMode.Roi)
            {
                throw new InvalidOperationException("Mode is not set to Roi");
            }

            if (roi.Width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(roi), roi.Width,
                    $"The width of the roi can't be less than or equal to zero.");
            }
            if (roi.Height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(roi), roi.Height,
                    $"The height of the roi can't be less than or equal to zero.");
            }

            Interop.Player.SetDisplayRoi(Player.Handle, roi.X, roi.Y, roi.Width, roi.Height).
                ThrowIfFailed("Failed to set the roi");
        }
    }
}
