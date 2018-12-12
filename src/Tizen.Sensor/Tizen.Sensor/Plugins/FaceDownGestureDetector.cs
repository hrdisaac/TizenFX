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

namespace Tizen.Sensor
{
    /// <summary>
    /// The FaceDownGestureDetector class is used for registering callbacks for the face down gesture detector and getting the face down state.
    /// </summary>
    /// <since_tizen> 3 </since_tizen>
    public sealed class FaceDownGestureDetector : Sensor
    {
        private static string GestureDetectorKey = "http://tizen.org/feature/sensor.gesture_recognition";

        /// <summary>
        /// Gets the state of the face down gesture.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        /// <value> The face down state. </value>
        public DetectorState FaceDown { get; private set; } = DetectorState.Unknown;

        /// <summary>
        /// Returns true or false based on whether the face down gesture detector is supported by the device.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        /// <value><c>true</c> if supported; otherwise <c>false</c>.</value>
        public static bool IsSupported
        {
            get
            {
                Log.Info(Globals.LogTag, "Checking if the face down gesture detector is supported");
                return CheckIfSupported(SensorType.FaceDownGestureDetector, GestureDetectorKey);
            }
        }

        /// <summary>
        /// Returns the number of face down gesture detectors available on the device.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        /// <value> The count of face down gesture detectors. </value>
        public static int Count
        {
            get
            {
                Log.Info(Globals.LogTag, "Getting the count of face down gesture detectors");
                return GetCount();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tizen.Sensor.FaceDownGestureDetector"/> class.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        /// <feature>http://tizen.org/feature/sensor.gesture_recognition</feature>
        /// <exception cref="ArgumentException">Thrown when an invalid argument is used.</exception>
        /// <exception cref="NotSupportedException">Thrown when the sensor is not supported.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the operation is invalid for the current state.</exception>
        /// <param name='index'>
        /// Index. Default value for this is 0. Index refers to a particular face down gesture detector in case of multiple sensors.
        /// </param>
        public FaceDownGestureDetector(uint index = 0) : base(index)
        {
            Log.Info(Globals.LogTag, "Creating face down gesture detector object");
        }

        internal override SensorType GetSensorType()
        {
            return SensorType.FaceDownGestureDetector;
        }

        private static int GetCount()
        {
            IntPtr list;
            int count;
            int error = Interop.SensorManager.GetSensorList(SensorType.FaceDownGestureDetector, out list, out count);
            if (error != (int)SensorError.None)
            {
                Log.Error(Globals.LogTag, "Error getting sensor list for face down gesture detector");
                count = 0;
            }
            else
                Interop.Libc.Free(list);
            return count;
        }

        /// <summary>
        /// An event handler for storing the callback functions for the event corresponding to the change in the face down gesture detector data.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public event EventHandler<FaceDownGestureDetectorDataUpdatedEventArgs> DataUpdated;

        private static Interop.SensorListener.SensorEventCallback _callback;

        internal override void EventListenStart()
        {
            _callback = (IntPtr sensorHandle, IntPtr eventPtr, IntPtr data) => {
                Interop.SensorEventStruct sensorData = Interop.IntPtrToEventStruct(eventPtr);

                TimeSpan = new TimeSpan((Int64)sensorData.timestamp);
                FaceDown = (DetectorState) sensorData.values[0];

                DataUpdated?.Invoke(this, new FaceDownGestureDetectorDataUpdatedEventArgs(sensorData.values[0]));
            };

            int error = Interop.SensorListener.SetEventCallback(ListenerHandle, Interval, _callback, IntPtr.Zero);
            if (error != (int)SensorError.None)
            {
                Log.Error(Globals.LogTag, "Error setting event callback for face down gesture detector");
                throw SensorErrorFactory.CheckAndThrowException(error, "Unable to set event callback for face down gesture detector");
            }
        }

        internal override void EventListenStop()
        {
            int error = Interop.SensorListener.UnsetEventCallback(ListenerHandle);
            if (error != (int)SensorError.None)
            {
                Log.Error(Globals.LogTag, "Error unsetting event callback for face down gesture detector");
                throw SensorErrorFactory.CheckAndThrowException(error, "Unable to unset event callback for face down gesture detector");
            }
        }
    }
}