﻿using System.IO;
using BizHawk.Emulation.Common;

namespace BizHawk.Client.Common
{
	public interface IMovieSession
	{
		IMovie Movie { get; set; }
		bool ReadOnly { get; set; }

		/// <summary>
		/// Gets a value indicating whether or not a new movie is queued for loading
		/// </summary>
		bool NewMovieQueued { get; }

		/// <summary>
		/// Gets the sync settings from a queued movie, if a movie is queued
		/// </summary>
		string QueuedSyncSettings { get; }

		IMovieController MovieController { get; }
		MultitrackRecorder MultiTrack { get; }

		/// <summary>
		/// Gets the controller state from the movie for the most recent frame
		/// </summary>
		IController CurrentInput { get; }

		IController PreviousFrame { get; }

		/// <summary>
		/// Recreates MovieController with the given controller definition
		/// with an empty controller state
		/// </summary>
		void RecreateMovieController(ControllerDefinition definition);

		/// <summary>
		/// Creates a <see cref="IMovieController" /> instance based on the
		/// given button definition if provided else the
		/// current <see cref="MovieController" /> button definition
		/// will be used
		/// </summary>
		IMovieController GenerateMovieController(ControllerDefinition definition = null);

		void HandleFrameBefore();
		void HandleFrameAfter();
		void HandleSaveState(TextWriter writer);

		bool CheckSavestateTimeline(TextReader reader);
		bool HandleLoadState(TextReader reader);

		/// <summary>
		/// Queues up a movie for loading
		/// When initializing a movie, it will be stored until Rom loading processes have been completed, then it will be moved to the Movie property
		/// If an existing movie is still active, it will remain in the Movie property while the new movie is queued
		/// </summary>
		void QueueNewMovie(IMovie movie, bool record, IEmulator emulator);

		/// <summary>
		/// Sets the Movie property with the QueuedMovie, clears the queued movie, and starts the new movie
		/// </summary>
		void RunQueuedMovie(bool recordMode);

		void ToggleMultitrack();

		void StopMovie(bool saveChanges = true);
	}
}
