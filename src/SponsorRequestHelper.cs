﻿using System;
using System.Threading.Tasks;

namespace CsInlineColorViz
{
    public class SponsorRequestHelper
    {
        public static async Task CheckIfNeedToShowAsync()
        {
            if (await SponsorDetector.IsSponsorAsync())
            {
                if (new Random().Next(1, 10) == 2)
                {
                    await ShowThanksForSponsorshipMessageAsync();
                }
            }
            else
            {
                await ShowPromptForSponsorshipAsync();
            }
        }

        private static async Task ShowThanksForSponsorshipMessageAsync()
        {
            await OutputPane.Instance.WriteAsync("Thank you for your sponsorship. It really helps.");
            await OutputPane.Instance.WriteAsync("If you have ideas for new features or suggestions for new features");
            await OutputPane.Instance.WriteAsync("please raise an issue at https://github.com/mrlacey/CSInlineColorViz/issues");
            await OutputPane.Instance.WriteAsync(string.Empty);
            await OutputPane.Instance.WriteAsync("I have other extensions you might be intertested in at https://marketplace.visualstudio.com/publishers/MattLaceyLtd");
            await OutputPane.Instance.WriteAsync(string.Empty);
        }

        private static async Task ShowPromptForSponsorshipAsync()
        {
            await OutputPane.Instance.WriteAsync("Sorry to interrupt. I know your time is busy, presumably that's why you installed this extension (C# Inline Color Vizualizer).");
            await OutputPane.Instance.WriteAsync("I also have many other extensions you might be intertested in at https://marketplace.visualstudio.com/publishers/MattLaceyLtd");
            await OutputPane.Instance.WriteAsync("I'm happy that the extensions I've created have been able to help you and many others");
            await OutputPane.Instance.WriteAsync("but I also need to make a living, and limited paid work over the last few years has been a challenge. :(");
            await OutputPane.Instance.WriteAsync(string.Empty);
            await OutputPane.Instance.WriteAsync("Show your support by making a one-off or recurring donation at https://github.com/sponsors/mrlacey");
            await OutputPane.Instance.WriteAsync(string.Empty);
            await OutputPane.Instance.WriteAsync("If you become a sponsor, I'll tell you how to hide this message too. ;)");
            await OutputPane.Instance.WriteAsync(string.Empty);
            await OutputPane.Instance.ActivateAsync();
        }
    }
}
