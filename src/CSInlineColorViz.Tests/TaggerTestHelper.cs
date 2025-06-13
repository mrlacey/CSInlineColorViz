﻿using System.Collections.Generic;
using CsInlineColorViz;

namespace CSInlineColorViz.Tests;

public class TaggerTestHelper
{
	public static IEnumerable<object[]> AllTaggerRegexs()
	{
		yield return [Color32Tagger.regularExpression];
		yield return [ColorArgbTagger.regularExpression];
		yield return [ColorCtorTagger.regularExpression];
		yield return [ColorFromIntTagger.regularExpression];
		yield return [ColorFromNameTagger.regularExpression];
		yield return [ColorHslaTagger.regularExpression];
		yield return [ColorHsvaTagger.regularExpression];
		yield return [ColorRgbaTagger.regularExpression];
		yield return [ColorRgbTagger.regularExpression];
		yield return [ColorTagger.regularExpression];
		yield return [FunColorTagger.regularExpression];
		yield return [HexIntTagger.regularExpression];
		yield return [HexStringTagger.regularExpression];
		yield return [MauiProjTagger.regularExpression];
		yield return [SvgTagger.regularExpression];
		yield return [UnityTagger.regularExpression];
		yield return [UnityTextTagger.regularExpression];
	}
}
