using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace CsInlineColorViz
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("CSharp")]
    [TagType(typeof(ColorTag))]
    internal sealed class ColorCtorTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer)
            where T : ITag
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            return buffer.Properties.GetOrCreateSingletonProperty(() => new ColorCtorTagger(buffer)) as ITagger<T>;
        }
    }
}
