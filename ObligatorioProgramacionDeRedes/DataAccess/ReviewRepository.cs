using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
namespace DataAccess
{
    public class ReviewRepository
    {
        private readonly List<Review> _reviews;
        private static ReviewRepository _instance;
        private readonly SemaphoreSlim _reviewsSemaphore;
        private static readonly SemaphoreSlim _instanceSemaphore = new SemaphoreSlim(1);

        private ReviewRepository()
        {
            _reviews = new List<Review>();
            _reviewsSemaphore = new SemaphoreSlim(1);
        }
        
        public static ReviewRepository GetInstance()
        {
            _instanceSemaphore.Wait();
            if (_instance == null)
            {
                _instance = new ReviewRepository();
            }
            _instanceSemaphore.Release();
            return _instance;
            
        }
        
        public async Task AddReviewAsync(Review review)
        {
            await _reviewsSemaphore.WaitAsync();
            this._reviews.Add(review);
            _reviewsSemaphore.Release();
        }

        public async Task<List<Review>> GetReviewsAsync(Game game)
        {
            await _reviewsSemaphore.WaitAsync();
            List<Review> listToReturn = new List<Review>();
            foreach (Review review in _reviews)
            {
                if (review.Game.Title.Equals(game.Title))
                {
                    listToReturn.Add(review);
                }
            }
            _reviewsSemaphore.Release();
            return listToReturn;
        }

        public async Task DeleteReviewAsync(string gameName)
        {
            await _reviewsSemaphore.WaitAsync();
            for (int i = 0; i < _reviews.Count; i++)
            {
                if (_reviews.ElementAt(i).Game.Title.Equals(gameName))
                {
                    _reviews.Remove(_reviews.ElementAt(i));
                }
            }
            _reviewsSemaphore.Release();

        }
    }
}