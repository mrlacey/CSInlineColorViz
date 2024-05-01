using System;

namespace CsInlineColorViz
{
	/// <summary>
	/// These are internal settings that are not exposed to the user.
	/// Reusing the capabilities of VS Options to store values.
	/// </summary>
	internal class InternalSettings : BaseOptionModel<InternalSettings>
	{
		/// <summary>
		/// The first time we know the extension was used.
		/// </summary>
		public DateTime FirstUse { get; set; } = DateTime.MinValue;

		/// <summary>
		/// How many different document instances 
		/// </summary>
		public int UseCount { get; set; } = 0;
	}
}
