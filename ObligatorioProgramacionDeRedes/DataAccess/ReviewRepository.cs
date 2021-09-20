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
    }
}