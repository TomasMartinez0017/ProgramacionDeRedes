namespace LogsServer.Domain
{
    public class SearchCriteria
    {
        /*public int? PostId { get; set; }
        public int? ThemeId { get; set; }
        public string PostName { get; set; }

        public bool MatchesCriteria(Log logPost)
        {
            bool matchesCriteria = false;
            
            if (logPost.EntityType == typeof(PostDetailDTO) || logPost.EntityType == typeof(List<PostDetailDTO>))
            {
                matchesCriteria = MatchesPostId(logPost) && MatchesThemeId(logPost) && MatchesPostName(logPost) &&
                       MatchesLogTag(logPost);
            }

            return matchesCriteria;
        }

        private bool MatchesPostId(Log logPost)
        {
            bool matchesPostId = true;

            if (PostId != null)
            {
                if (logPost.IsEntityAList())
                {
                    List<PostDetailDTO> posts = logPost.Entity as List<PostDetailDTO>;
                    matchesPostId = posts.Any(p => p.Id == PostId);
                }
                else
                {
                    PostDetailDTO post = logPost.Entity as PostDetailDTO;
                    matchesPostId = post.Id == PostId;
                }
            }

            return matchesPostId;
        }

        private bool MatchesPostName(Log logPost)
        {
            bool matchesPostName = true;

            if (!String.IsNullOrEmpty(PostName))
            {
                if (logPost.IsEntityAList())
                {
                    List<PostDetailDTO> posts = logPost.Entity as List<PostDetailDTO>;
                    matchesPostName = posts.Any(p => p.Name.ToLower().Contains(PostName.ToLower()));
                }
                else
                {
                    PostDetailDTO post = logPost.Entity as PostDetailDTO;
                    matchesPostName = post.Name.ToLower().Contains(PostName.ToLower());
                }
            }

            return matchesPostName;
        }

        private bool MatchesThemeId(Log logPost)
        {
            bool matchesThemeId = true;
            
            if (ThemeId != null)
            {
                if (logPost.IsEntityAList())
                {
                    List<PostDetailDTO> posts = logPost.Entity as List<PostDetailDTO>;
                    matchesThemeId = posts.Any(p => p.Theme.Id == ThemeId);
                }
                else
                {
                    PostDetailDTO post = logPost.Entity as PostDetailDTO;
                    matchesThemeId =  post.Theme.Id == ThemeId;
                }
            }

            return matchesThemeId;
        }
        
        private bool MatchesLogTag(Log logPost)
        {
            return logPost.LogTag == LogTag.CreatePost || logPost.LogTag == LogTag.DeletePost ||
                   logPost.LogTag == LogTag.ShowPost || logPost.LogTag == LogTag.UpdatePost ||
                   logPost.LogTag == LogTag.ChangePostsTheme || logPost.LogTag == LogTag.IndexPostsByTheme;
        }*/
    }
}