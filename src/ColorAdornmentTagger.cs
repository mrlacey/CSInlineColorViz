using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace CsInlineColorViz
{
	internal sealed class ColorAdornmentTagger
		: IntraTextAdornmentTagger<ColorTag, ColorAdornment>
	{
		private readonly ITagAggregator<ColorTag> tagger;

		private ColorAdornmentTagger(IWpfTextView view, ITagAggregator<ColorTag> tagger)
			: base(view)
		{
			this.tagger = tagger;
		}

		public void Dispose()
		{
			this.tagger.Dispose();

			this.view.Properties.RemoveProperty(typeof(ColorTagger));
		}

		internal static ITagger<IntraTextAdornmentTag> GetTagger(IWpfTextView view, Lazy<ITagAggregator<ColorTag>> tagger)
		{
			return view.Properties.GetOrCreateSingletonProperty(
				() => new ColorAdornmentTagger(view, tagger.Value));
		}

		protected override IEnumerable<Tuple<SnapshotSpan, PositionAffinity?, ColorTag>> GetAdornmentData(NormalizedSnapshotSpanCollection spans)
		{
			if (spans.Count == 0)
			{
				yield break;
			}

			ITextSnapshot snapshot = spans[0].Snapshot;

			var clTags = this.tagger.GetTags(spans);

			foreach (IMappingTagSpan<ColorTag> dataTagSpan in clTags)
			{
				NormalizedSnapshotSpanCollection linkTagSpans = dataTagSpan.Span.GetSpans(snapshot);

				// Ignore data tags that are split by projection.
				// This is theoretically possible but unlikely in current scenarios.
				if (linkTagSpans.Count != 1)
				{
					continue;
				}

				var adornmentSpan = new SnapshotSpan(linkTagSpans[0].Start, 0);

				yield return Tuple.Create(adornmentSpan, (PositionAffinity?)PositionAffinity.Successor, dataTagSpan.Tag);
			}

			yield break;
		}

		protected override ColorAdornment CreateAdornment(ColorTag dataTag, SnapshotSpan span)
		{
			return new ColorAdornment(dataTag);
		}

		protected override bool UpdateAdornment(ColorAdornment adornment, ColorTag dataTag)
		{
			adornment.Update(dataTag);
			return true;
		}
	}
}
