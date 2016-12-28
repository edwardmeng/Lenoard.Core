using System;

namespace Lenoard.Core
{
    public enum TrackingState
    {
        /// <summary>
		/// The object has been added, and AcceptChanges has not been called.
		/// </summary>
		Added = 0,
        /// <summary>
        /// The object was deleted using the Delete method of the MetaObject.
        /// </summary>
        Deleted,
        /// <summary>
        /// The object has been modified and AcceptChanges has not been called.
        /// </summary>
        Modified,
        /// <summary>
        /// The object has not changed since AcceptChanges was last called.
        /// </summary>
        Unchanged
    }

    public interface ITrackingObject
    {
        void AcceptChanges();

        void RejectChanges();

        TrackingState TrackingState { get; }
    }

    public static class TrackingObjectExtensions
    {
        public static bool IsAdded(this ITrackingObject obj)
        {
            if(obj == null)throw new ArgumentNullException(nameof(obj));
            return obj.TrackingState == TrackingState.Added;
        }

        public static bool IsDeleted(this ITrackingObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return obj.TrackingState == TrackingState.Deleted;
        }

        public static bool IsModified(this ITrackingObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return obj.TrackingState == TrackingState.Modified;
        }

        public static bool IsUnchanged(this ITrackingObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return obj.TrackingState == TrackingState.Unchanged;
        }
    }
}
