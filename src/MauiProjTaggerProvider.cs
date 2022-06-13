using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;

namespace CsInlineColorViz
{
    // THis works for any XML file but the inteded application is within .csproj files (for .NET MAUI apps)
    [Export(typeof(ITaggerProvider))]
    [ContentType("Xml")]
    [TagType(typeof(ColorTag))]
    internal sealed class MauiProjTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer)
            where T : ITag
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            return buffer.Properties.GetOrCreateSingletonProperty(() => new MauiProjTagger(buffer)) as ITagger<T>;
        }
    }
}
