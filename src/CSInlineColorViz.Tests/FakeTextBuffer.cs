using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace CSInlineColorViz.Tests;

public class FakeTextBuffer : ITextBuffer
{
	public FakeTextBuffer()
	{
	}

	event EventHandler<SnapshotSpanEventArgs> ITextBuffer.ReadOnlyRegionsChanged
	{
		add { }

		remove
		{
		}
	}

	event EventHandler<TextContentChangedEventArgs> ITextBuffer.Changed
	{
		add
		{
		}

		remove
		{
		}
	}

	event EventHandler<TextContentChangingEventArgs> ITextBuffer.Changing
	{
		add
		{
		}

		remove
		{
		}
	}

	event EventHandler ITextBuffer.PostChanged
	{
		add
		{
		}

		remove
		{
		}
	}

	event EventHandler<ContentTypeChangedEventArgs> ITextBuffer.ContentTypeChanged
	{
		add
		{
		}

		remove
		{
		}
	}

#pragma warning disable CS0067 // events never used
	public event EventHandler<ContentTypeChangedEventArgs> ContentTypeChanged;
	public event EventHandler PostChanged;
	public event EventHandler<SnapshotSpanEventArgs> ReadOnlyRegionsChanged;
	public event EventHandler<TextContentChangedEventArgs> Changed;
	public event EventHandler<TextContentChangingEventArgs> Changing;
	public event EventHandler<TextContentChangedEventArgs> ChangedLowPriority;
	public event EventHandler<TextContentChangedEventArgs> ChangedHighPriority;
#pragma warning restore CS0067

	public IContentType ContentType { get; }
	public ITextSnapshot CurrentSnapshot { get; }
	IContentType ITextBuffer.ContentType { get; }
	public bool EditInProgress { get; }
	public PropertyCollection Properties { get; }

	public ITextEdit CreateEdit() => throw new NotImplementedException();
	public ITextEdit CreateEdit(EditOptions options, int? reiteratedVersionNumber, object editTag) => throw new NotImplementedException();
	public IReadOnlyRegionEdit CreateReadOnlyRegionEdit() => throw new NotImplementedException();
	public void TakeThreadOwnership() => throw new NotImplementedException();

	public bool CheckEditAccess()
	{
		throw new NotImplementedException();
	}

	public void ChangeContentType(IContentType newContentType, object editTag)
	{
		throw new NotImplementedException();
	}

	public ITextSnapshot Insert(int position, string text)
	{
		throw new NotImplementedException();
	}

	public ITextSnapshot Delete(Span deleteSpan)
	{
		throw new NotImplementedException();
	}

	public ITextSnapshot Replace(Span replaceSpan, string replaceWith)
	{
		throw new NotImplementedException();
	}

	public bool IsReadOnly(int position)
	{
		throw new NotImplementedException();
	}

	public bool IsReadOnly(int position, bool isEdit)
	{
		throw new NotImplementedException();
	}

	public bool IsReadOnly(Span span)
	{
		throw new NotImplementedException();
	}

	public bool IsReadOnly(Span span, bool isEdit)
	{
		throw new NotImplementedException();
	}

	public NormalizedSpanCollection GetReadOnlyExtents(Span span)
	{
		throw new NotImplementedException();
	}
}
