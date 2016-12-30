using System;

namespace Lenoard.Core
{
    /// <summary>
    /// The enumeration to indicate the tracking state of the implementations of <see cref="ITrackingObject"/>.
    /// </summary>
    public enum TrackingState
    {
        /// <summary>
		/// The object has been added, and <see cref="ITrackingObject.AcceptChanges"/> has not been called.
		/// </summary>
		Added = 0,
        /// <summary>
        /// The object was deleted.
        /// </summary>
        Deleted,
        /// <summary>
        /// The object has been modified and <see cref="ITrackingObject.AcceptChanges"/> has not been called.
        /// </summary>
        Modified,
        /// <summary>
        /// The object has not changed since <see cref="ITrackingObject.AcceptChanges"/> was last called.
        /// </summary>
        Unchanged
    }

    /// <summary>
    /// Represents the implemented types can be tracking changes.
    /// </summary>
    public interface ITrackingObject
    {
        /// <summary>
        /// Accpet the changes.
        /// </summary>
        void AcceptChanges();

        /// <summary>
        /// Reject the changes
        /// </summary>
        void RejectChanges();

        /// <summary>
        /// Gets the tracking state.
        /// </summary>
        TrackingState TrackingState { get; }
    }

    /// <summary>
    /// Provides a set of <see langword="static"/> extension methods for the <see cref="ITrackingObject"/>.
    /// </summary>
    public static class TrackingObjectExtensions
    {
        /// <summary>
        /// Determines whether the <see cref="ITrackingObject"/> is in the <see cref="TrackingState.Added"/> state.
        /// </summary>
        /// <param name="obj">The <see cref="ITrackingObject"/> to be detected.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is in the <see cref="TrackingState.Added"/> state; otherwise, <c>false</c>.</returns>
        public static bool IsAdded(this ITrackingObject obj)
        {
            if(obj == null)throw new ArgumentNullException(nameof(obj));
            return obj.TrackingState == TrackingState.Added;
        }

        /// <summary>
        /// Determines whether the <see cref="ITrackingObject"/> is in the <see cref="TrackingState.Deleted"/> state.
        /// </summary>
        /// <param name="obj">The <see cref="ITrackingObject"/> to be detected.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is in the <see cref="TrackingState.Deleted"/> state; otherwise, <c>false</c>.</returns>
        public static bool IsDeleted(this ITrackingObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return obj.TrackingState == TrackingState.Deleted;
        }

        /// <summary>
        /// Determines whether the <see cref="ITrackingObject"/> is in the <see cref="TrackingState.Modified"/> state.
        /// </summary>
        /// <param name="obj">The <see cref="ITrackingObject"/> to be detected.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is in the <see cref="TrackingState.Modified"/> state; otherwise, <c>false</c>.</returns>
        public static bool IsModified(this ITrackingObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return obj.TrackingState == TrackingState.Modified;
        }

        /// <summary>
        /// Determines whether the <see cref="ITrackingObject"/> is in the <see cref="TrackingState.Unchanged"/> state.
        /// </summary>
        /// <param name="obj">The <see cref="ITrackingObject"/> to be detected.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is in the <see cref="TrackingState.Unchanged"/> state; otherwise, <c>false</c>.</returns>
        public static bool IsUnchanged(this ITrackingObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return obj.TrackingState == TrackingState.Unchanged;
        }
    }
}
