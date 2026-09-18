using Phronesis.Domain.Collaboration;

namespace Phronesis.Application.Common.Interfaces;

public interface ICollaborationService
{
    // Resources
    Task<ClassResource> UploadClassResourceAsync(Guid virtualClassId, Guid teacherId, string title, string description, string fileUrl, string mimeType, long sizeInBytes, Guid? classSessionId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<ClassResource>> GetClassResourcesAsync(Guid virtualClassId, Guid userId, CancellationToken cancellationToken = default);
    Task DeleteClassResourceAsync(Guid resourceId, Guid teacherId, CancellationToken cancellationToken = default);

    // Discussions
    Task<ClassDiscussion> CreateDiscussionAsync(Guid virtualClassId, Guid authorId, string title, string content, CancellationToken cancellationToken = default);
    Task<IEnumerable<ClassDiscussion>> GetClassDiscussionsAsync(Guid virtualClassId, Guid userId, CancellationToken cancellationToken = default);
    Task<ClassDiscussion?> GetDiscussionByIdAsync(Guid discussionId, Guid userId, CancellationToken cancellationToken = default);
    Task ResolveDiscussionAsync(Guid discussionId, Guid teacherId, CancellationToken cancellationToken = default);

    // Replies
    Task<DiscussionReply> AddReplyAsync(Guid discussionId, Guid authorId, string content, CancellationToken cancellationToken = default);
    Task EndorseReplyAsync(Guid replyId, Guid teacherId, CancellationToken cancellationToken = default);
    Task RevokeEndorsementAsync(Guid replyId, Guid teacherId, CancellationToken cancellationToken = default);
}
