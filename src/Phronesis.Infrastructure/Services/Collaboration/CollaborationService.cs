using Microsoft.EntityFrameworkCore;
using Phronesis.Application.Common.Interfaces;
using Phronesis.Domain.Collaboration;

namespace Phronesis.Infrastructure.Services.Collaboration;

public class CollaborationService : ICollaborationService
{
    private readonly IApplicationDbContext _context;

    public CollaborationService(IApplicationDbContext context)
    {
        _context = context;
    }

    private async Task VerifyAccessAsync(Guid virtualClassId, Guid userId, CancellationToken cancellationToken)
    {
        var classInfo = await _context.VirtualClasses
            .AsNoTracking()
            .FirstOrDefaultAsync(vc => vc.Id == virtualClassId, cancellationToken);
            
        if (classInfo == null)
            throw new ArgumentException("Class not found.");

        if (classInfo.TeacherId == userId)
            return; // Teacher has access

        var isEnrolled = await _context.ClassEnrollments
            .AnyAsync(ce => ce.VirtualClassId == virtualClassId && ce.LearnerId == userId && ce.Status == Phronesis.Domain.Tuition.EnrollmentStatus.Active, cancellationToken);

        if (!isEnrolled)
            throw new UnauthorizedAccessException("You do not have access to this class's collaboration spaces.");
    }

    public async Task<ClassResource> UploadClassResourceAsync(Guid virtualClassId, Guid teacherId, string title, string description, string fileUrl, string mimeType, long sizeInBytes, Guid? classSessionId = null, CancellationToken cancellationToken = default)
    {
        var virtualClass = await _context.VirtualClasses.FindAsync(new object[] { virtualClassId }, cancellationToken);
        if (virtualClass == null || virtualClass.TeacherId != teacherId)
            throw new UnauthorizedAccessException("Only the assigned teacher can upload resources.");

        var resource = new ClassResource(virtualClassId, teacherId, title, description, fileUrl, mimeType, sizeInBytes, classSessionId);
        _context.ClassResources.Add(resource);
        await _context.SaveChangesAsync(cancellationToken);

        return resource;
    }

    public async Task<IEnumerable<ClassResource>> GetClassResourcesAsync(Guid virtualClassId, Guid userId, CancellationToken cancellationToken = default)
    {
        await VerifyAccessAsync(virtualClassId, userId, cancellationToken);

        return await _context.ClassResources
            .AsNoTracking()
            .Where(cr => cr.VirtualClassId == virtualClassId)
            .OrderByDescending(cr => cr.UploadedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteClassResourceAsync(Guid resourceId, Guid teacherId, CancellationToken cancellationToken = default)
    {
        var resource = await _context.ClassResources.FindAsync(new object[] { resourceId }, cancellationToken);
        if (resource == null) throw new ArgumentException("Resource not found.");

        if (resource.TeacherId != teacherId)
            throw new UnauthorizedAccessException("Only the teacher who uploaded the resource can delete it.");

        _context.ClassResources.Remove(resource);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<ClassDiscussion> CreateDiscussionAsync(Guid virtualClassId, Guid authorId, string title, string content, CancellationToken cancellationToken = default)
    {
        await VerifyAccessAsync(virtualClassId, authorId, cancellationToken);

        var discussion = new ClassDiscussion(virtualClassId, authorId, title, content);
        _context.ClassDiscussions.Add(discussion);
        await _context.SaveChangesAsync(cancellationToken);

        return discussion;
    }

    public async Task<IEnumerable<ClassDiscussion>> GetClassDiscussionsAsync(Guid virtualClassId, Guid userId, CancellationToken cancellationToken = default)
    {
        await VerifyAccessAsync(virtualClassId, userId, cancellationToken);

        return await _context.ClassDiscussions
            .AsNoTracking()
            .Include(d => d.Author)
            .Include(d => d.Replies)
            .Where(cd => cd.VirtualClassId == virtualClassId)
            .OrderByDescending(cd => cd.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ClassDiscussion?> GetDiscussionByIdAsync(Guid discussionId, Guid userId, CancellationToken cancellationToken = default)
    {
        var discussion = await _context.ClassDiscussions
            .AsNoTracking()
            .Include(d => d.Author)
            .Include(d => d.Replies)
                .ThenInclude(r => r.Author)
            .FirstOrDefaultAsync(cd => cd.Id == discussionId, cancellationToken);

        if (discussion != null)
        {
            await VerifyAccessAsync(discussion.VirtualClassId, userId, cancellationToken);
        }

        return discussion;
    }

    public async Task ResolveDiscussionAsync(Guid discussionId, Guid teacherId, CancellationToken cancellationToken = default)
    {
        var discussion = await _context.ClassDiscussions
            .Include(d => d.VirtualClass)
            .FirstOrDefaultAsync(cd => cd.Id == discussionId, cancellationToken);

        if (discussion == null) throw new ArgumentException("Discussion not found.");

        if (discussion.VirtualClass.TeacherId != teacherId)
            throw new UnauthorizedAccessException("Only the assigned teacher can resolve discussions.");

        discussion.MarkAsResolved();
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<DiscussionReply> AddReplyAsync(Guid discussionId, Guid authorId, string content, CancellationToken cancellationToken = default)
    {
        var discussion = await _context.ClassDiscussions.FindAsync(new object[] { discussionId }, cancellationToken);
        if (discussion == null) throw new ArgumentException("Discussion not found.");

        await VerifyAccessAsync(discussion.VirtualClassId, authorId, cancellationToken);

        var reply = new DiscussionReply(discussionId, authorId, content);
        discussion.AddReply(reply);
        
        await _context.SaveChangesAsync(cancellationToken);
        return reply;
    }

    public async Task EndorseReplyAsync(Guid replyId, Guid teacherId, CancellationToken cancellationToken = default)
    {
        var reply = await _context.DiscussionReplies
            .Include(r => r.ClassDiscussion)
                .ThenInclude(d => d.VirtualClass)
            .FirstOrDefaultAsync(r => r.Id == replyId, cancellationToken);

        if (reply == null) throw new ArgumentException("Reply not found.");

        if (reply.ClassDiscussion.VirtualClass.TeacherId != teacherId)
            throw new UnauthorizedAccessException("Only the assigned teacher can endorse replies.");

        reply.Endorse();
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeEndorsementAsync(Guid replyId, Guid teacherId, CancellationToken cancellationToken = default)
    {
        var reply = await _context.DiscussionReplies
            .Include(r => r.ClassDiscussion)
                .ThenInclude(d => d.VirtualClass)
            .FirstOrDefaultAsync(r => r.Id == replyId, cancellationToken);

        if (reply == null) throw new ArgumentException("Reply not found.");

        if (reply.ClassDiscussion.VirtualClass.TeacherId != teacherId)
            throw new UnauthorizedAccessException("Only the assigned teacher can revoke endorsements.");

        reply.RevokeEndorsement();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
