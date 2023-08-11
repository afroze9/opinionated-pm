using LanguageExt.Common;
using Nexus.PeopleAPI.Abstractions;
using Nexus.PeopleAPI.Data;
using Nexus.PeopleAPI.Entities;
using Nexus.Persistence.Auditing;
using Quartz;

namespace Nexus.PeopleAPI.Services;

[DisallowConcurrentExecution]
public class PeopleSyncJob : IJob
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<PeopleSyncJob> _logger;
    private readonly UnitOfWork _unitOfWork;
    private readonly IDateTime _dateTime;

    public PeopleSyncJob(IIdentityService identityService, ILogger<PeopleSyncJob> logger, UnitOfWork unitOfWork, IDateTime dateTime)
    {
        _identityService = identityService;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        SyncStatus? syncStatus = await _unitOfWork.SyncStatuses.GetByJobNameAsync(nameof(PeopleSyncJob));
        DateTime syncTime = _dateTime.UtcNow;

        if (syncStatus is null)
        {
            DateTime baseDateTime = new DateTime(2023, 01, 01).ToUniversalTime();
            syncStatus = new SyncStatus() { JobName = nameof(PeopleSyncJob), LastSync = baseDateTime };
            _unitOfWork.SyncStatuses.Add(syncStatus);
            _unitOfWork.BeginTransaction();
            _unitOfWork.Commit();
        }

        int pageNum = 0;
        const int pageSize = 100;
        bool hasMore = true;
        Result<PaginatedList<Person>> userResult;
        do
        {
            userResult = await _identityService.GetUsersRegisteredAfterDate(syncStatus.LastSync, pageNum, pageSize);
            await userResult.Match<Task<bool>>(
                async users =>
                {
                    foreach (Person user in users.Items)
                    {
                        if (await _unitOfWork.People.ExistsWithIdentityIdAsync(user.IdentityId))
                        {
                            continue;
                        }

                        _unitOfWork.People.Add(user);
                        _unitOfWork.BeginTransaction();
                        _unitOfWork.Commit();
                    }

                    if (users.HasNextPage)
                    {
                        hasMore = true;
                        pageNum++;
                    }

                    return true;
                }, 
                _ =>
                {
                    hasMore = false;
                    return Task.FromResult(false);
                });
        } while (userResult.IsSuccess && hasMore);

        syncStatus.LastSync = syncTime;
        _unitOfWork.BeginTransaction();
        _unitOfWork.Commit();
    }
}