using CsInlineColorViz;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSInlineColorViz.Tests;

[TestClass]
public class ColorHelperTests
{
	[TestMethod]
	public void FloatToByte_LowerBoundary()
	{
		Assert.AreEqual(0, ColorHelper.FloatToByte(0.0f));
	}

	[TestMethod]
	public void FloatToByte_BelowLowerBoundary()
	{
		Assert.AreEqual(0, ColorHelper.FloatToByte(-0.001f));
	}

	[TestMethod]
	public void FloatToByte_UpperBoundary()
	{
		Assert.AreEqual(255, ColorHelper.FloatToByte(1.0f));
	}

	[TestMethod]
	public void FloatToByte_AboveUpperBoundary()
	{
		Assert.AreEqual(255, ColorHelper.FloatToByte(1.01f));
	}

	[TestMethod]
	public void FloatToByte_MidPoint()
	{
		Assert.AreEqual(127, ColorHelper.FloatToByte(0.5f));
	}
}
