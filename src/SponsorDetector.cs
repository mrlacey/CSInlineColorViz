using System.Threading.Tasks;

namespace CsInlineColorViz
{
    public class SponsorDetector
    {
        // This might be the code you see, but it's not what I compile into the extensions when built ;)
        public static async Task<bool> IsSponsorAsync()
        {
            return await Task.FromResult(false);
        }
    }
}
