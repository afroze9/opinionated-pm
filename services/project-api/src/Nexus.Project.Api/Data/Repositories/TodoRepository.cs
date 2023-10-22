﻿using Nexus.Common.Attributes;
using Nexus.Persistence;
using Nexus.ProjectAPI.Entities;

namespace Nexus.ProjectAPI.Data.Repositories;

[NexusService(NexusServiceLifeTime.Scoped)]
public class TodoRepository : EfNexusRepository<TodoItem>
{
    public TodoRepository(ApplicationDbContext context) : base(context)
    {
    }
}