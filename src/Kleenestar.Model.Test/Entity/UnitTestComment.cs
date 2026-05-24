using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the <see cref="Comment"/> entity and the
    /// <see cref="CommentState"/> extension methods.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestComment
    {
        /// <summary>
        /// Verifies that a new comment is assigned a non-empty unique identifier.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            var comment = new Comment();

            Assert.NotEqual(Guid.Empty, comment.Id);
        }

        /// <summary>
        /// Verifies that the explicit-id constructor sets the supplied id.
        /// </summary>
        [Fact]
        public void InitializeWithExplicitId()
        {
            var id = Guid.NewGuid();

            var comment = new Comment(id);

            Assert.Equal(id, comment.Id);
        }

        /// <summary>
        /// Verifies that scalar properties are stored verbatim.
        /// </summary>
        [Theory]
        [InlineData("Hello world.", CommentState.Active)]
        [InlineData("Edited later.", CommentState.Edited)]
        [InlineData("",              CommentState.Deleted)]
        public void SetScalarProperties(string content, CommentState state)
        {
            var comment = new Comment
            {
                Content = content,
                State = state
            };

            Assert.Equal(content, comment.Content);
            Assert.Equal(state, comment.State);
        }

        /// <summary>
        /// Verifies that the reply collection starts empty and accepts entries.
        /// </summary>
        [Fact]
        public void RepliesCollectionAcceptsEntries()
        {
            var parent = new Comment();

            Assert.Empty(parent.Replies);

            parent.Replies.Add(new Comment { Content = "reply 1" });
            parent.Replies.Add(new Comment { Content = "reply 2" });

            Assert.Equal(2, parent.Replies.Count);
        }

        /// <summary>
        /// Verifies the state-extension helpers (<c>IsVisible</c>, <c>Id</c>,
        /// <c>Text</c>, <c>Color</c>).
        /// </summary>
        [Theory]
        [InlineData(CommentState.Active, true)]
        [InlineData(CommentState.Edited, true)]
        [InlineData(CommentState.Deleted, false)]
        [InlineData(CommentState.Hidden, false)]
        public void CommentStateExtensions(CommentState state, bool expectedVisible)
        {
            Assert.Equal(expectedVisible, state.IsVisible());
            Assert.NotEqual(Guid.Empty, state.Id());
            Assert.False(string.IsNullOrEmpty(state.Text()));
            Assert.False(string.IsNullOrEmpty(state.Color()));
        }
    }
}
