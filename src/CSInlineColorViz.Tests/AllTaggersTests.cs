using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSInlineColorViz.Tests;

[TestClass]
public class AllTaggersTests : BaseTaggerTests
{
	[DataTestMethod]
	[DynamicData(nameof(TaggerTestHelper.AllTaggerRegexs), typeof(TaggerTestHelper), DynamicDataSourceType.Method)]
	public void DoesNotMatch_InconsequentialText(Regex type)
	{
		base.DoesNotMatch_Text(type, "just some inconsequential text string");
	}

	[DataTestMethod]
	[DynamicData(nameof(TaggerTestHelper.AllTaggerRegexs), typeof(TaggerTestHelper), DynamicDataSourceType.Method)]
	public void DoesNotMatch_EmptyString(Regex type)
	{
		base.DoesNotMatch_Text(type, string.Empty);
	}

	[DataTestMethod]
	[DynamicData(nameof(TaggerTestHelper.AllTaggerRegexs), typeof(TaggerTestHelper), DynamicDataSourceType.Method)]
	public void DoesNotMatch_Whitespace(Regex type)
	{
		base.DoesNotMatch_Text(type, "     ");
	}
}