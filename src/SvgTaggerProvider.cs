using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace CsInlineColorViz;

// different projects report SVG files with different content types
[Export(typeof(ITaggerProvider))]
[ContentType("HTML")]
[ContentType("XML")]
[TagType(typeof(ColorTag))]
internal sealed class SvgTaggerProvider : ITaggerProvider
{
	[Import]
	internal ITextDocumentFactoryService docFactory = null;

	public ITagger<T> CreateTagger<T>(ITextBuffer buffer)
		where T : ITag
	{
		if (buffer == null)
		{
			throw new ArgumentNullException(nameof(buffer));
		}

		string fileName = null;

		if (docFactory.TryGetTextDocument(buffer, out ITextDocument document))
		{
			fileName = document.FilePath;
			// You can now use the fileName variable as needed
		}

		if (string.IsNullOrWhiteSpace(fileName) || !fileName.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
		{
			return null;
		}

		return buffer.Properties.GetOrCreateSingletonProperty(() => new SvgTagger(buffer)) as ITagger<T>;
	}
}
