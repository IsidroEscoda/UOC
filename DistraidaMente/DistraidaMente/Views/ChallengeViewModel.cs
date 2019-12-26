using DistraidaMente.Models;

namespace DistraidaMente.Views
{
    internal class ChallengeViewModel
    {
        public Challenge Item { get; set; }

        private Challenge item;

        public ChallengeViewModel(Challenge item)
        {
            this.item = item;
        }
    }
}