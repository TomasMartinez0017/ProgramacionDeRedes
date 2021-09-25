using System;
using System.Linq;
using System.Collections.Generic;
using Domain;
namespace DataAccess
{
    public class ReviewRepository
    {
        private readonly List<Review> _reviews;
        private object _reviewsLocker;
        private static ReviewRepository _instance;
        private static Object _instanceLocker = new Object();

        private ReviewRepository()
        {
            _reviews = new List<Review>();
            _reviewsLocker = new Object();
        }
        
        public static ReviewRepository GetInstance()
        {
            lock (_instanceLocker)
            {
                if (_instance == null)
                {
                    _instance = new ReviewRepository();
                }

                return _instance;
            }
        }
        
        public void AddReview(Review review)
        {
            lock (_reviewsLocker)
            {
                this._reviews.Add(review);    
            }
        }

        public List<Review> GetReviews(Game game)
        {
            lock (_instanceLocker)
            {
                List<Review> listToReturn = new List<Review>();
                foreach (Review review in _reviews)
                {
                    if (review.Game.Title.Equals(game.Title))
                    {
                        listToReturn.Add(review);
                    }
                }
                return listToReturn;
            }
            
        }

        public void DeleteReview(string gameName)
        {
            lock (_instanceLocker)
            {
                for (int i = 0; i < _reviews.Count; i++)
                {
                    if (_reviews.ElementAt(i).Game.Title.Equals(gameName))
                    {
                        _reviews.Remove(_reviews.ElementAt(i));
                    }
                }
            }
            
        }
    }
}