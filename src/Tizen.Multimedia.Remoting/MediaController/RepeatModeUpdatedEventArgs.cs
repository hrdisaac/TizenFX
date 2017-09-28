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

namespace Tizen.Multimedia.Remoting
{
    /// <summary>
    /// Provides data for the <see cref="MediaController.RepeatModeUpdated"/> event.
    /// </summary>
    public class RepeatModeUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepeatModeUpdatedEventArgs"/> class.
        /// </summary>
        /// <param name="mode">A value indicating the updated repeat mode.</param>
        /// <exception cref="ArgumentException"><paramref name="mode"/> is invalid.</exception>
        public RepeatModeUpdatedEventArgs(MediaControlRepeatMode mode)
        {
            ValidationUtil.ValidateEnum(typeof(MediaControlRepeatMode), mode, nameof(mode));

            RepeatMode = mode;
        }

        /// <summary>
        /// Gets the updated repeat mode.
        /// </summary>
        /// <value>The <see cref="MediaControlRepeatMode"/>.</value>
        public MediaControlRepeatMode RepeatMode { get; }
    }
}